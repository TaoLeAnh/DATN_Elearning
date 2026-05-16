using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMenhDeDungSai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhAnhUrl",
                table: "MenhDeDungSai",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnhUrl",
                table: "MenhDeDungSai");
        }
    }
}
