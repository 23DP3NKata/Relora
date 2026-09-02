using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class HardenPaymentsAndConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_order_disputes_OrderId",
                table: "order_disputes");

            migrationBuilder.AddColumn<int>(
                name: "FailureCount",
                table: "Payouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAttemptAtUtc",
                table: "Payouts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "Payouts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundedAt",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeRefundId",
                table: "Payments",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CheckoutSessionVersion",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payouts_OrderId",
                table: "Payouts",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payouts_Status",
                table: "Payouts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StripeSessionId",
                table: "Payments",
                column: "StripeSessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_disputes_OrderId",
                table: "order_disputes",
                column: "OrderId",
                unique: true,
                filter: "\"Status\" = 'Open'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payouts_OrderId",
                table: "Payouts");

            migrationBuilder.DropIndex(
                name: "IX_Payouts_Status",
                table: "Payouts");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_StripeSessionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_order_disputes_OrderId",
                table: "order_disputes");

            migrationBuilder.DropColumn(
                name: "FailureCount",
                table: "Payouts");

            migrationBuilder.DropColumn(
                name: "LastAttemptAtUtc",
                table: "Payouts");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "Payouts");

            migrationBuilder.DropColumn(
                name: "RefundedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StripeRefundId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CheckoutSessionVersion",
                table: "orders");

            migrationBuilder.CreateIndex(
                name: "IX_order_disputes_OrderId",
                table: "order_disputes",
                column: "OrderId");
        }
    }
}
