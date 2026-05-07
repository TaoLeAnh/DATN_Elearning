using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMaTran : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaTranDeThiMacDinh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MonHoc = table.Column<int>(type: "int", nullable: false),
                    TenMaTran = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModerationStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaTranDeThiMacDinh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietMaTranMacDinh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaTranDeThiMacDinhId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhanThi = table.Column<int>(type: "int", nullable: false),
                    LoaiCauHoi = table.Column<int>(type: "int", nullable: false),
                    MucDo = table.Column<int>(type: "int", nullable: false),
                    ChuDe = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModerationStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietMaTranMacDinh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietMaTranMacDinh_MaTranDeThiMacDinh_MaTranDeThiMacDinhId",
                        column: x => x.MaTranDeThiMacDinhId,
                        principalTable: "MaTranDeThiMacDinh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietMaTranMacDinh_MaTranDeThiMacDinhId",
                table: "ChiTietMaTranMacDinh",
                column: "MaTranDeThiMacDinhId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietMaTranMacDinh");

            migrationBuilder.DropTable(
                name: "MaTranDeThiMacDinh");
        }
    }
}
