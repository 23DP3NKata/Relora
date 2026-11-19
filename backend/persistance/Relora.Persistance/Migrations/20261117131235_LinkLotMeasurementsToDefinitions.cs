using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class LinkLotMeasurementsToDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // vispirms jauna kolonna, kas var būt tukša
            migrationBuilder.AddColumn<Guid>(
                name: "MeasurementDefinitionId",
                table: "lot_measurements",
                type: "uuid",
                nullable: true);

            // vecajiem ierakstiem atrodam definīciju pēc Key un kategorijas profila
            migrationBuilder.Sql(@"
                UPDATE lot_measurements AS lm
                SET ""MeasurementDefinitionId"" = md.""Id""
                FROM lots AS l
                JOIN categories AS c ON c.""Id"" = l.""CategoryId""
                JOIN measurement_definitions AS md ON md.""Profile"" = c.""MeasurementProfile""
                WHERE l.""Id"" = lm.""LotId""
                    AND lower(md.""Key"") = lower(lm.""Key"");");

            // ja definīcija netika atrasta, tādu mēru nevar saglabāt
            migrationBuilder.Sql(@"DELETE FROM lot_measurements WHERE ""MeasurementDefinitionId"" IS NULL;");

            migrationBuilder.AlterColumn<Guid>(
                name: "MeasurementDefinitionId",
                table: "lot_measurements",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropIndex(
                name: "IX_lot_measurements_LotId_Key",
                table: "lot_measurements");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "lot_measurements");

            migrationBuilder.CreateIndex(
                name: "IX_lot_measurements_LotId_MeasurementDefinitionId",
                table: "lot_measurements",
                columns: new[] { "LotId", "MeasurementDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lot_measurements_MeasurementDefinitionId",
                table: "lot_measurements",
                column: "MeasurementDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_lot_measurements_measurement_definitions_MeasurementDefinit~",
                table: "lot_measurements",
                column: "MeasurementDefinitionId",
                principalTable: "measurement_definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lot_measurements_measurement_definitions_MeasurementDefinit~",
                table: "lot_measurements");

            migrationBuilder.DropIndex(
                name: "IX_lot_measurements_LotId_MeasurementDefinitionId",
                table: "lot_measurements");

            migrationBuilder.DropIndex(
                name: "IX_lot_measurements_MeasurementDefinitionId",
                table: "lot_measurements");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "lot_measurements",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE lot_measurements AS lm
                SET ""Key"" = md.""Key""
                FROM measurement_definitions AS md
                WHERE md.""Id"" = lm.""MeasurementDefinitionId"";");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "lot_measurements",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "MeasurementDefinitionId",
                table: "lot_measurements");

            migrationBuilder.CreateIndex(
                name: "IX_lot_measurements_LotId_Key",
                table: "lot_measurements",
                columns: new[] { "LotId", "Key" },
                unique: true);
        }
    }
}
