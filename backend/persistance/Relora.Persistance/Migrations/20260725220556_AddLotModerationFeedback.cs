using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddLotModerationFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModerationRejectionReason",
                table: "lots",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModerationReviewedAtUtc",
                table: "lots",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModerationReviewedByAdminId",
                table: "lots",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModerationRejectionReason",
                table: "lots");

            migrationBuilder.DropColumn(
                name: "ModerationReviewedAtUtc",
                table: "lots");

            migrationBuilder.DropColumn(
                name: "ModerationReviewedByAdminId",
                table: "lots");
        }
    }
}
