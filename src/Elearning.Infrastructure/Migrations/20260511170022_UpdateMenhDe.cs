using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMenhDe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DapAnDienKetQua_CauHoi_CauHoiId1",
                table: "DapAnDienKetQua");

            migrationBuilder.DropIndex(
                name: "IX_DapAnDienKetQua_CauHoiId1",
                table: "DapAnDienKetQua");

            migrationBuilder.DropColumn(
                name: "CauHoiId1",
                table: "DapAnDienKetQua");

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "MenhDeDungSai",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "MenhDeDungSai",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CauHoiId1",
                table: "DapAnDienKetQua",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DapAnDienKetQua_CauHoiId1",
                table: "DapAnDienKetQua",
                column: "CauHoiId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DapAnDienKetQua_CauHoi_CauHoiId1",
                table: "DapAnDienKetQua",
                column: "CauHoiId1",
                principalTable: "CauHoi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
