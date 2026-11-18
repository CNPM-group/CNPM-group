using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class PackageImageService(IFileStorage storage, IOrderRepository orders)
{
    public const long MaxBytes = 5 * 1024 * 1024;

    public async Task<string> UploadAsync(Stream content, long length, CancellationToken ct = default)
    {
        if (length <= 0)
            throw new ValidationException("File ảnh rỗng.");
        if (length > MaxBytes)
            throw new ValidationException("Ảnh tối đa 5 MB.");

        // Đọc vào bộ nhớ để xem mấy byte đầu: biết chắc là ảnh thật, không tin tên file hay Content-Type client gửi.
        var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, ct);
        var (contentType, extension) = Detect(buffer.GetBuffer().AsSpan(0, (int)buffer.Length))
            ?? throw new ValidationException("Chỉ nhận ảnh JPG, PNG hoặc WEBP.");

        buffer.Position = 0;
        var key = $"packages/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension}";
        return await storage.SaveAsync(buffer, buffer.Length, contentType, key, ct);
    }

    public async Task<StoredFile> OpenForOrderAsync(Guid? customerScope, Guid orderId, CancellationToken ct = default)
    {
        var order = await orders.GetAsync(orderId, customerScope, ct)
            ?? throw new NotFoundException("Không tìm thấy đơn hàng.");
        var package = await orders.GetPackageAsync(order.PackageId, ct)
            ?? throw new NotFoundException("Không tìm thấy kiện hàng của đơn.");
        if (string.IsNullOrEmpty(package.ImageKey))
            throw new NotFoundException("Đơn này không có ảnh kiện hàng.");

        return await storage.OpenAsync(package.ImageKey, ct)
            ?? throw new NotFoundException("Không tìm thấy ảnh trong kho lưu trữ.");
    }

    private static (string ContentType, string Extension)? Detect(ReadOnlySpan<byte> b)
    {
        if (b.Length >= 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF)
            return ("image/jpeg", ".jpg");
        if (b.Length >= 8 && b[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
            return ("image/png", ".png");
        if (b.Length >= 12 && b[..4].SequenceEqual("RIFF"u8) && b[8..12].SequenceEqual("WEBP"u8))
            return ("image/webp", ".webp");
        return null;
    }
}
