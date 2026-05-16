using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBaiHocV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaiLieuAI",
                table: "BaiHoc",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaiLieuAI",
                table: "BaiHoc");
        }
    }
}
