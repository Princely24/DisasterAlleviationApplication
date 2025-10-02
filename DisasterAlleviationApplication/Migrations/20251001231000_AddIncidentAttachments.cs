using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DisasterAlleviationApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentContentType",
                table: "DisasterIncidents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                table: "DisasterIncidents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentFilePath",
                table: "DisasterIncidents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AttachmentFileSize",
                table: "DisasterIncidents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AttachmentUploadedDate",
                table: "DisasterIncidents",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentContentType",
                table: "DisasterIncidents");

            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                table: "DisasterIncidents");

            migrationBuilder.DropColumn(
                name: "AttachmentFilePath",
                table: "DisasterIncidents");

            migrationBuilder.DropColumn(
                name: "AttachmentFileSize",
                table: "DisasterIncidents");

            migrationBuilder.DropColumn(
                name: "AttachmentUploadedDate",
                table: "DisasterIncidents");
        }
    }
}
