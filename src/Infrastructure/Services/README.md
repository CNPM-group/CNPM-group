# Infrastructure/Services

Dịch vụ phụ thuộc thư viện hoặc hệ thống bên ngoài (tương đương `infrastructure/services` bên Flask, ví dụ email service). Khác `src/Services` (nghiệp vụ): ở đây chỉ là "ống nối", không có luật nghiệp vụ.

| File | Interface | Tác dụng | Cấu hình |
|---|---|---|---|
| `JwtTokenService.cs` | `ITokenService` | Tạo JWT ký HMAC-SHA256, claim `sub`, `email`, `name`, `role` | `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` (≥ 32 ký tự, đặt bằng biến môi trường `Jwt__Key`), `Jwt:ExpiryMinutes` |
| `MinioFileStorage.cs` | `IFileStorage` | Lưu và đọc file trong MinIO (gói NuGet `Minio` 6.x). Lần lưu đầu tự tạo bucket nếu chưa có. Lỗi kết nối, sai mật khẩu → `ExternalServiceException` (502); chưa cấu hình → 503 | `Minio:Endpoint` (vd `minio:9000`), `Minio:AccessKey`, `Minio:SecretKey` (biến môi trường `Minio__SecretKey`), `Minio:Bucket` (mặc định `smartdrone`), `Minio:UseSsl` |
| `LlmClient.cs` | `ILlmClient` | Gọi `POST {BaseUrl}v1/chat/completions` (chuẩn OpenAI, provider nào tương thích cũng dùng được), header `Authorization: Bearer <ApiKey>`, timeout `TimeoutSeconds` | `Llm:BaseUrl` (bắt buộc https, có `/` cuối), `Llm:Model`, `Llm:ApiKey` (biến môi trường `Llm__ApiKey`), `Llm:TimeoutSeconds` |

## Lỗi của LlmClient

| Tình huống | Exception | HTTP |
|---|---|---|
| Thiếu key/model, URL không phải https | `ExternalServiceException` | 503 |
| Provider trả lỗi HTTP, mất kết nối, JSON hỏng | `ExternalServiceException` | 502 |
| Quá thời gian chờ | `ExternalServiceException` | 504 |

Đổi nhà cung cấp LLM (OpenAI → Gemini/Groq...): chỉ sửa `LlmClient.cs` hoặc cấu hình, `AiService` không đổi.
