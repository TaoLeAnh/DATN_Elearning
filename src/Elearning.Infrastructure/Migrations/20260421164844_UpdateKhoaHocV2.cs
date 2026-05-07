using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKhoaHocV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GiaBan",
                table: "KhoaHoc",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GiaGoc",
                table: "KhoaHoc",
                type: "decimal(18,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnhUrl",
                table: "KhoaHoc",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiaBan",
                table: "KhoaHoc");

            migrationBuilder.DropColumn(
                name: "GiaGoc",
                table: "KhoaHoc");

            migrationBuilder.DropColumn(
                name: "HinhAnhUrl",
                table: "KhoaHoc");
        }
    }
}
