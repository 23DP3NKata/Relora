using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddMinimalOrderDisputes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order_disputes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    OpenedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResolvedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    Decision = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    DecisionReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_disputes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "order_dispute_evidence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisputeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_dispute_evidence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_dispute_evidence_order_disputes_DisputeId",
                        column: x => x.DisputeId,
                        principalTable: "order_disputes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_dispute_evidence_DisputeId",
                table: "order_dispute_evidence",
                column: "DisputeId");

            migrationBuilder.CreateIndex(
                name: "IX_order_disputes_OrderId",
                table: "order_disputes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_order_disputes_Status_OpenedAtUtc",
                table: "order_disputes",
                columns: new[] { "Status", "OpenedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_dispute_evidence");

            migrationBuilder.DropTable(
                name: "order_disputes");
        }
    }
}
