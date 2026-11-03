using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProofDocumentsAndDeliveryIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // move orders from removed statuses to the closest remaining ones
            migrationBuilder.Sql("UPDATE orders SET \"Status\" = 'Shipped' WHERE \"Status\" = 'BuyerProtection';");
            migrationBuilder.Sql("UPDATE orders SET \"Status\" = 'Completed' WHERE \"Status\" = 'PayoutPending';");
            migrationBuilder.Sql("UPDATE orders SET \"Status\" = 'Cancelled' WHERE \"Status\" IN ('Refunded', 'Failed');");

            migrationBuilder.DropTable(
                name: "lot_proof_documents");

            migrationBuilder.DropTable(
                name: "pending_lot_proof_document_uploads");

            migrationBuilder.DropTable(
                name: "proof_document_types");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "lot_proof_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    LotId = table.Column<Guid>(type: "uuid", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lot_proof_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lot_proof_documents_lots_LotId",
                        column: x => x.LotId,
                        principalTable: "lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pending_lot_proof_document_uploads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    MimeType = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_lot_proof_document_uploads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "proof_document_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    NameKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proof_document_types", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status_DeliveryIssueReportedAtUtc",
                table: "orders",
                columns: new[] { "Status", "DeliveryIssueReportedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_lot_proof_documents_LotId_StorageKey",
                table: "lot_proof_documents",
                columns: new[] { "LotId", "StorageKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lot_proof_documents_OwnerId",
                table: "lot_proof_documents",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_pending_lot_proof_document_uploads_CreatedAtUtc",
                table: "pending_lot_proof_document_uploads",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_pending_lot_proof_document_uploads_OwnerId",
                table: "pending_lot_proof_document_uploads",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_pending_lot_proof_document_uploads_StorageKey",
                table: "pending_lot_proof_document_uploads",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proof_document_types_Code",
                table: "proof_document_types",
                column: "Code",
                unique: true);
        }
    }
}
