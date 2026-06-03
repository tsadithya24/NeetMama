using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeetMama.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentChunksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentChunks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UploadedBookId = table.Column<int>(type: "int", nullable: false),
                    ChunkNumber = table.Column<int>(type: "int", nullable: false),
                    ChunkText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChunkLength = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentChunks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentChunks");
        }
    }
}
