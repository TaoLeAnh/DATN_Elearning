using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModuleOntap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThuTu",
                table: "ChiTietBoCauHoi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "MucDo",
                table: "CauHoi",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "GiaiThich",
                table: "CauHoi",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "BoCauHoiOnTap",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "BaiHocId",
                table: "BoCauHoiOnTap",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChuongHocId",
                table: "BoCauHoiOnTap",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KhoaHocId",
                table: "BoCauHoiOnTap",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoaiBoCauHoi",
                table: "BoCauHoiOnTap",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BoCauHoiOnTap_BaiHocId",
                table: "BoCauHoiOnTap",
                column: "BaiHocId");

            migrationBuilder.CreateIndex(
                name: "IX_BoCauHoiOnTap_ChuongHocId",
                table: "BoCauHoiOnTap",
                column: "ChuongHocId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoCauHoiOnTap_BaiHoc_BaiHocId",
                table: "BoCauHoiOnTap",
                column: "BaiHocId",
                principalTable: "BaiHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BoCauHoiOnTap_ChuongHoc_ChuongHocId",
                table: "BoCauHoiOnTap",
                column: "ChuongHocId",
                principalTable: "ChuongHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoCauHoiOnTap_BaiHoc_BaiHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropForeignKey(
                name: "FK_BoCauHoiOnTap_ChuongHoc_ChuongHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropIndex(
                name: "IX_BoCauHoiOnTap_BaiHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropIndex(
                name: "IX_BoCauHoiOnTap_ChuongHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropColumn(
                name: "ThuTu",
                table: "ChiTietBoCauHoi");

            migrationBuilder.DropColumn(
                name: "GiaiThich",
                table: "CauHoi");

            migrationBuilder.DropColumn(
                name: "BaiHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropColumn(
                name: "ChuongHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropColumn(
                name: "KhoaHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropColumn(
                name: "LoaiBoCauHoi",
                table: "BoCauHoiOnTap");

            migrationBuilder.AlterColumn<string>(
                name: "MucDo",
                table: "CauHoi",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "BoCauHoiOnTap",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
