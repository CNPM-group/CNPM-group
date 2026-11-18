using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SmartDrone.Infrastructure.Databases;

#nullable disable

namespace SmartDrone.Migrations
{
    /// <summary>v0.11: thêm tên và số điện thoại người nhận vào đơn hàng (quy tắc nghiệp vụ số 2).</summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20261116060000_AddOrderRecipient")]
    public partial class AddOrderRecipient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "recipient_name",
                table: "delivery_orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipient_phone",
                table: "delivery_orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "recipient_name", table: "delivery_orders");
            migrationBuilder.DropColumn(name: "recipient_phone", table: "delivery_orders");
        }
    }
}
