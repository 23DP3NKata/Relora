using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentDeadlineAndDeliveryIssueFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryIssueAvailableFromUtc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryIssueReason",
                table: "orders",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryIssueReportedAtUtc",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE orders
                SET "PaymentDeadlineUtc" = "PaymentDeadlineUtc" + INTERVAL '24 hours'
                WHERE "Status" = 'PendingPayment';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status_DeliveryIssueReportedAtUtc",
                table: "orders",
                columns: new[] { "Status", "DeliveryIssueReportedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE orders
                SET "PaymentDeadlineUtc" = "PaymentDeadlineUtc" - INTERVAL '24 hours'
                WHERE "Status" = 'PendingPayment';
                """);

            migrationBuilder.DropIndex(
                name: "IX_orders_Status_DeliveryIssueReportedAtUtc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "DeliveryIssueAvailableFromUtc",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "DeliveryIssueReason",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "DeliveryIssueReportedAtUtc",
                table: "orders");
        }
    }
}
