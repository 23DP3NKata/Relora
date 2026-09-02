using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddLotMediaOrderingAndCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_cover",
                table: "lot_media",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "lot_media",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE lot_media AS target
                SET sort_order = ranked.sort_order,
                    is_cover = (ranked.sort_order = 0)
                FROM (
                    SELECT "Id",
                           ROW_NUMBER() OVER (PARTITION BY "LotId" ORDER BY "Id") - 1 AS sort_order
                    FROM lot_media
                    WHERE media_type = 'photo'
                ) AS ranked
                WHERE target."Id" = ranked."Id";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_lot_media_LotId_sort_order",
                table: "lot_media",
                columns: new[] { "LotId", "sort_order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_lot_media_LotId_sort_order",
                table: "lot_media");

            migrationBuilder.DropColumn(
                name: "is_cover",
                table: "lot_media");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "lot_media");
        }
    }
}
