# PackageImageService.cs

Nghiệp vụ ảnh kiện hàng (KAN-33). Không biết gì về MinIO, chỉ gọi interface `IFileStorage`. Muốn đổi sang S3 hay lưu ổ đĩa thì viết class khác cài `IFileStorage`, file này giữ nguyên.

## UploadAsync(content, length)

Dùng cho `POST /api/v1/packages/images`.

1. Rỗng → 400. Quá **5 MB** → 400.
2. Đọc file vào bộ nhớ, xem **mấy byte đầu** (magic bytes) để biết loại ảnh thật:

   | Byte đầu | Loại | Đuôi |
   |---|---|---|
   | `FF D8 FF` | JPEG | `.jpg` |
   | `89 50 4E 47 0D 0A 1A 0A` | PNG | `.png` |
   | `RIFF....WEBP` | WEBP | `.webp` |

   Không khớp → 400 "Chỉ nhận ảnh JPG, PNG hoặc WEBP". Không tin tên file hay `Content-Type` client gửi, vì đổi đuôi `.exe` thành `.jpg` là qua mặt được. App Flutter còn gửi `application/octet-stream` cho mọi file.
3. Đặt key `packages/<năm>/<tháng>/<guid>.<đuôi>` rồi lưu qua `IFileStorage.SaveAsync`.
4. Trả key. Client gửi key này trong `package.imageKey` khi tạo đơn; DB chỉ lưu key, không lưu ảnh.

## OpenForOrderAsync(customerScope, orderId)

Dùng cho `GET /api/v1/orders/{id}/package-image`.

- Tìm đơn theo `customerScope`, giống `GET /orders/{id}`: khách chỉ mở được ảnh đơn của mình, nhân viên mở được mọi đơn.
- Đơn không có ảnh, hoặc ảnh không còn trong MinIO → 404.

Ảnh đi qua API chứ không đưa thẳng link MinIO cho client vì: (1) kiểm được quyền xem, (2) MinIO trong Docker có địa chỉ `minio:9000`, điện thoại và trình duyệt không gọi tới được.
