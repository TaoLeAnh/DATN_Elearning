using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKyThi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ThoiGianKetThuc",
                table: "KyThi",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ThoiGianBatDau",
                table: "KyThi",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "KhoaHocId",
                table: "KyThi",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "KyThi",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiDeThi",
                table: "KyThi",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonHoc",
                table: "KyThi",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NamThi",
                table: "KyThi",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenTruong",
                table: "KyThi",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TinhThanh",
                table: "KyThi",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "KyThi");

            migrationBuilder.DropColumn(
                name: "LoaiDeThi",
                table: "KyThi");

            migrationBuilder.DropColumn(
                name: "MonHoc",
                table: "KyThi");

            migrationBuilder.DropColumn(
                name: "NamThi",
                table: "KyThi");

            migrationBuilder.DropColumn(
                name: "TenTruong",
                table: "KyThi");

            migrationBuilder.DropColumn(
                name: "TinhThanh",
                table: "KyThi");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ThoiGianKetThuc",
                table: "KyThi",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ThoiGianBatDau",
                table: "KyThi",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "KhoaHocId",
                table: "KyThi",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
