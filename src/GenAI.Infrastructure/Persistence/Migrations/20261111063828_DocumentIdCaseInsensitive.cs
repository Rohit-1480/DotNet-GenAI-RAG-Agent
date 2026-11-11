using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DocumentIdCaseInsensitive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                collation: "NOCASE",
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Documents",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldCollation: "NOCASE");
        }
    }
}
