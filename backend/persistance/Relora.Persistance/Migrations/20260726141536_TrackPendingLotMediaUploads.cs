using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class TrackPendingLotMediaUploads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pending_lot_media_uploads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_lot_media_uploads", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pending_lot_media_uploads_CreatedAtUtc",
                table: "pending_lot_media_uploads",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_pending_lot_media_uploads_Key",
                table: "pending_lot_media_uploads",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pending_lot_media_uploads_OwnerId",
                table: "pending_lot_media_uploads",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pending_lot_media_uploads");
        }
    }
}
