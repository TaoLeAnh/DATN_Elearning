using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModuleThi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CauHoiId1",
                table: "DapAnDienKetQua",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhanThi",
                table: "CauHoiKyThi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThuTu",
                table: "CauHoiKyThi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChiTietTraLoiMenhDe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiTietBaiLamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenhDeDungSaiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LuaChonCuaHocVien = table.Column<bool>(type: "bit", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModerationStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietTraLoiMenhDe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietTraLoiMenhDe_ChiTietBaiLam_ChiTietBaiLamId",
                        column: x => x.ChiTietBaiLamId,
                        principalTable: "ChiTietBaiLam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietTraLoiMenhDe_MenhDeDungSai_MenhDeDungSaiId",
                        column: x => x.MenhDeDungSaiId,
                        principalTable: "MenhDeDungSai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DapAnDienKetQua_CauHoiId1",
                table: "DapAnDienKetQua",
                column: "CauHoiId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietTraLoiMenhDe_ChiTietBaiLamId",
                table: "ChiTietTraLoiMenhDe",
                column: "ChiTietBaiLamId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietTraLoiMenhDe_MenhDeDungSaiId",
                table: "ChiTietTraLoiMenhDe",
                column: "MenhDeDungSaiId");

            migrationBuilder.AddForeignKey(
                name: "FK_DapAnDienKetQua_CauHoi_CauHoiId1",
                table: "DapAnDienKetQua",
                column: "CauHoiId1",
                principalTable: "CauHoi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DapAnDienKetQua_CauHoi_CauHoiId1",
                table: "DapAnDienKetQua");

            migrationBuilder.DropTable(
                name: "ChiTietTraLoiMenhDe");

            migrationBuilder.DropIndex(
                name: "IX_DapAnDienKetQua_CauHoiId1",
                table: "DapAnDienKetQua");

            migrationBuilder.DropColumn(
                name: "CauHoiId1",
                table: "DapAnDienKetQua");

            migrationBuilder.DropColumn(
                name: "PhanThi",
                table: "CauHoiKyThi");

            migrationBuilder.DropColumn(
                name: "ThuTu",
                table: "CauHoiKyThi");
        }
    }
}
