using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLotMediaIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                DO $$
                                BEGIN
                                    IF EXISTS (
                                        SELECT FROM information_schema.tables 
                                        WHERE table_schema = 'public' 
                                        AND table_name = 'lot_media'
                                    ) THEN
                                        DELETE FROM lot_media;
                                    END IF;
                                END $$;
                                """);
            migrationBuilder.DropPrimaryKey(
                name: "PK_lot_media",
                table: "lot_media");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "lot_media");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "lot_media",
                type: "uuid",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_lot_media",
                table: "lot_media",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_lot_media",
                table: "lot_media");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "lot_media");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "lot_media",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_lot_media",
                table: "lot_media",
                column: "Id");
        }
    }
}
