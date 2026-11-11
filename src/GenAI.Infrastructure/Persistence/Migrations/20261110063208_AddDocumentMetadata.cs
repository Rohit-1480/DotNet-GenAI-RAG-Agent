using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<Guid>(
            //    name: "DocumentId",
            //    table: "DocumentChunks",
            //    type: "TEXT",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            // Create a parent document record for every existing source
            migrationBuilder.Sql("""
    INSERT INTO Documents
        (Id, FileName, Category, ContentType, UploadedAt)
    SELECT
        lower(
            hex(randomblob(4)) || '-' ||
            hex(randomblob(2)) || '-' ||
            hex(randomblob(2)) || '-' ||
            hex(randomblob(2)) || '-' ||
            hex(randomblob(6))
        ),
        Source,
        'Uncategorized',
        'text/plain',
        CURRENT_TIMESTAMP
    FROM DocumentChunks
    GROUP BY Source;
    """);
            // Add nullable DocumentId temporarily
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "DocumentChunks",
                type: "TEXT",
                nullable: true);
            // Link existing chunks to their parent documents
            migrationBuilder.Sql("""
    UPDATE DocumentChunks
    SET DocumentId = (
        SELECT Id
        FROM Documents
        WHERE Documents.FileName = DocumentChunks.Source
    );
    """);
            // Make DocumentId mandatory after existing chunks are linked
            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentId",
                table: "DocumentChunks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentChunks_DocumentId",
                table: "DocumentChunks",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentChunks_Documents_DocumentId",
                table: "DocumentChunks",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        //protected override void Down(MigrationBuilder migrationBuilder)
        //{
        //    migrationBuilder.DropForeignKey(
        //        name: "FK_DocumentChunks_Documents_DocumentId",
        //        table: "DocumentChunks");

        //    migrationBuilder.DropTable(
        //        name: "Documents");

        //    migrationBuilder.DropIndex(
        //        name: "IX_DocumentChunks_DocumentId",
        //        table: "DocumentChunks");

        //    migrationBuilder.DropColumn(
        //        name: "DocumentId",
        //        table: "DocumentChunks");
        // }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentChunks_Documents_DocumentId",
                table: "DocumentChunks");

            migrationBuilder.DropIndex(
                name: "IX_DocumentChunks_DocumentId",
                table: "DocumentChunks");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "DocumentChunks");

            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
