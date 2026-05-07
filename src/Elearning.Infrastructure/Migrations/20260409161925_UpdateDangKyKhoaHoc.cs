using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDangKyKhoaHoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TienDoHoc_NguoiDungId",
                table: "TienDoHoc");

            migrationBuilder.DropIndex(
                name: "IX_DangKyKhoaHoc_NguoiDungId",
                table: "DangKyKhoaHoc");

            migrationBuilder.AlterColumn<bool>(
                name: "DaHoanThanh",
                table: "TienDoHoc",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "LastTimePosition",
                table: "TienDoHoc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayDangKy",
                table: "DangKyKhoaHoc",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "TienDo",
                table: "DangKyKhoaHoc",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TrangThai",
                table: "DangKyKhoaHoc",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Unique_NguoiDung_BaiHoc",
                table: "TienDoHoc",
                columns: new[] { "NguoiDungId", "BaiHocId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unique_NguoiDung_KhoaHoc",
                table: "DangKyKhoaHoc",
                columns: new[] { "NguoiDungId", "KhoaHocId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unique_NguoiDung_BaiHoc",
                table: "TienDoHoc");

            migrationBuilder.DropIndex(
                name: "IX_Unique_NguoiDung_KhoaHoc",
                table: "DangKyKhoaHoc");

            migrationBuilder.DropColumn(
                name: "LastTimePosition",
                table: "TienDoHoc");

            migrationBuilder.DropColumn(
                name: "NgayDangKy",
                table: "DangKyKhoaHoc");

            migrationBuilder.DropColumn(
                name: "TienDo",
                table: "DangKyKhoaHoc");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "DangKyKhoaHoc");

            migrationBuilder.AlterColumn<bool>(
                name: "DaHoanThanh",
                table: "TienDoHoc",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TienDoHoc_NguoiDungId",
                table: "TienDoHoc",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyKhoaHoc_NguoiDungId",
                table: "DangKyKhoaHoc",
                column: "NguoiDungId");
        }
    }
}
