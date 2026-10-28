
using System.ComponentModel.DataAnnotations;

namespace SmartDrone.Application.Customers;

// Dữ liệu khách hàng gửi lên khi thêm hoặc sửa địa chỉ
public sealed class CustomerAddressRequest
{
    [Required]
    [MaxLength(50)]
    public string Label { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string AddressLine { get; set; } = string.Empty;

    [Range(typeof(decimal), "-90", "90")]
    public decimal? Latitude { get; set; }

    [Range(typeof(decimal), "-180", "180")]
    public decimal? Longitude { get; set; }

    public bool IsDefault { get; set; }
}

// Dữ liệu địa chỉ trả về cho khách hàng
public sealed record CustomerAddressDto(
    Guid Id,
    string Label,
    string AddressLine,
    decimal? Latitude,
    decimal? Longitude,
    bool IsDefault,
    DateTime CreatedAtUtc
);
