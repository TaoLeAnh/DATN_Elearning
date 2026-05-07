using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBaiLamv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "KyThiId",
                table: "BaiLam",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "BoCauHoiOnTapId",
                table: "BaiLam",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BoCauHoiOnTap_KhoaHocId",
                table: "BoCauHoiOnTap",
                column: "KhoaHocId");

            migrationBuilder.CreateIndex(
                name: "IX_BaiLam_BoCauHoiOnTapId",
                table: "BaiLam",
                column: "BoCauHoiOnTapId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaiLam_BoCauHoiOnTap_BoCauHoiOnTapId",
                table: "BaiLam",
                column: "BoCauHoiOnTapId",
                principalTable: "BoCauHoiOnTap",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BoCauHoiOnTap_KhoaHoc_KhoaHocId",
                table: "BoCauHoiOnTap",
                column: "KhoaHocId",
                principalTable: "KhoaHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaiLam_BoCauHoiOnTap_BoCauHoiOnTapId",
                table: "BaiLam");

            migrationBuilder.DropForeignKey(
                name: "FK_BoCauHoiOnTap_KhoaHoc_KhoaHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropIndex(
                name: "IX_BoCauHoiOnTap_KhoaHocId",
                table: "BoCauHoiOnTap");

            migrationBuilder.DropIndex(
                name: "IX_BaiLam_BoCauHoiOnTapId",
                table: "BaiLam");

            migrationBuilder.DropColumn(
                name: "BoCauHoiOnTapId",
                table: "BaiLam");

            migrationBuilder.AlterColumn<Guid>(
                name: "KyThiId",
                table: "BaiLam",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
