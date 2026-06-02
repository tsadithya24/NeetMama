using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeetMama.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessingStatusToUploadedBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "UploadedBooks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "UploadedBooks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "UploadedBooks");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "UploadedBooks");
        }
    }
}
