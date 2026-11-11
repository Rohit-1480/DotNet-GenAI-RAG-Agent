using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "DocumentChunks",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentChunks_Source",
                table: "DocumentChunks",
                column: "Source");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocumentChunks_Source",
                table: "DocumentChunks");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "DocumentChunks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldCollation: "NOCASE");
        }
    }
}
