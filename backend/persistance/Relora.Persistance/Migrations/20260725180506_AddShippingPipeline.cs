using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingPipeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarrierName",
                table: "orders",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAtUtc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShipByUtc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippedAtUtc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressLine1",
                table: "orders",
                type: "character varying(240)",
                maxLength: 240,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddressLine2",
                table: "orders",
                type: "character varying(240)",
                maxLength: 240,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCity",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCountry",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCountryCode",
                table: "orders",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCurrency",
                table: "orders",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.AddColumn<string>(
                name: "ShippingFullName",
                table: "orders",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingHandlingDays",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "ShippingOriginCountry",
                table: "orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Unknown");

            migrationBuilder.AddColumn<string>(
                name: "ShippingPhone",
                table: "orders",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingPostalCode",
                table: "orders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingPrice",
                table: "orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShipsToCountries",
                table: "orders",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "Worldwide");

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "orders",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCurrency",
                table: "lots",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.AddColumn<int>(
                name: "ShippingHandlingDays",
                table: "lots",
                type: "integer",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "ShippingOriginCountry",
                table: "lots",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Unknown");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingPrice",
                table: "lots",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShipsToCountries",
                table: "lots",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "Worldwide");

            migrationBuilder.Sql("""
                UPDATE lots
                SET
                    "ShippingCurrency" = COALESCE(NULLIF("price_currency", ''), 'EUR'),
                    "ShippingOriginCountry" = COALESCE(NULLIF("Country", ''), 'Unknown'),
                    "ShipsToCountries" = COALESCE(NULLIF("Country", ''), 'Worldwide'),
                    "ShippingHandlingDays" = 3
                """);

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status_ShipByUtc",
                table: "orders",
                columns: new[] { "Status", "ShipByUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_orders_Status_ShipByUtc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "CarrierName",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "DeliveredAtUtc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShipByUtc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippedAtUtc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddressLine1",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddressLine2",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingCity",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingCountry",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingCountryCode",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingCurrency",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingFullName",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingHandlingDays",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingOriginCountry",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingPhone",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingPostalCode",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingPrice",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShipsToCountries",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "ShippingCurrency",
                table: "lots");

            migrationBuilder.DropColumn(
                name: "ShippingHandlingDays",
                table: "lots");

            migrationBuilder.DropColumn(
                name: "ShippingOriginCountry",
                table: "lots");

            migrationBuilder.DropColumn(
                name: "ShippingPrice",
                table: "lots");

            migrationBuilder.DropColumn(
                name: "ShipsToCountries",
                table: "lots");
        }
    }
}
