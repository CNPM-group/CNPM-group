using SmartDrone.Domain;
using SmartDrone.Services;
using Xunit;

namespace SmartDrone.Tests;

public class ProfileRulesTests
{
    [Theory]
    [InlineData("0901234567")]
    [InlineData("+84 901 234 567")]
    [InlineData("090.123.4567")]
    public void So_dien_thoai_hop_le(string phone) => Assert.Equal(phone, ProfileRules.Phone(phone));

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("09012345678901234567")]
    public void So_dien_thoai_sai_bi_chan(string phone) =>
        Assert.Throws<ValidationException>(() => ProfileRules.Phone(phone));

    [Fact]
    public void So_dien_thoai_trong_thanh_null() => Assert.Null(ProfileRules.Phone("   "));

    [Fact]
    public void Ho_ten_qua_ngan_bi_chan() => Assert.Throws<ValidationException>(() => ProfileRules.Name(" A "));

    [Fact]
    public void Ho_ten_duoc_cat_khoang_trang() => Assert.Equal("Trần Bình", ProfileRules.Name("  Trần Bình "));
}
