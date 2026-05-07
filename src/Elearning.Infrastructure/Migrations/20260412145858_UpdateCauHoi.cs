using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCauHoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KhoaHocId",
                table: "CauHoi",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CauHoi_KhoaHocId",
                table: "CauHoi",
                column: "KhoaHocId");

            migrationBuilder.AddForeignKey(
                name: "FK_CauHoi_KhoaHoc_KhoaHocId",
                table: "CauHoi",
                column: "KhoaHocId",
                principalTable: "KhoaHoc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CauHoi_KhoaHoc_KhoaHocId",
                table: "CauHoi");

            migrationBuilder.DropIndex(
                name: "IX_CauHoi_KhoaHocId",
                table: "CauHoi");

            migrationBuilder.DropColumn(
                name: "KhoaHocId",
                table: "CauHoi");
        }
    }
}
