using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModuleOntapV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChiTietBoCauHoi_BoCauHoiOnTapId",
                table: "ChiTietBoCauHoi");

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "DapAn",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnhUrl",
                table: "DapAn",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "CauHoi",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "HinhAnhUrl",
                table: "CauHoi",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unique_BoCauHoi_CauHoi",
                table: "ChiTietBoCauHoi",
                columns: new[] { "BoCauHoiOnTapId", "CauHoiId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unique_BoCauHoi_CauHoi",
                table: "ChiTietBoCauHoi");

            migrationBuilder.DropColumn(
                name: "HinhAnhUrl",
                table: "DapAn");

            migrationBuilder.DropColumn(
                name: "HinhAnhUrl",
                table: "CauHoi");

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "DapAn",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "CauHoi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietBoCauHoi_BoCauHoiOnTapId",
                table: "ChiTietBoCauHoi",
                column: "BoCauHoiOnTapId");
        }
    }
}
