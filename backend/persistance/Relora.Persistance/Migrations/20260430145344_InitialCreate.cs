using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    price_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<int>(type: "integer", maxLength: 20, nullable: false),
                    Brand = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PaymentDeadlineUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StripeCheckoutSessionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PaidAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "auctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    current_price_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    current_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    LotId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_auctions_lots_LotId",
                        column: x => x.LotId,
                        principalTable: "lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lot_media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    media_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LotId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lot_media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lot_media_lots_LotId",
                        column: x => x.LotId,
                        principalTable: "lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BidderId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    amount_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PlacedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_bids_auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "auctions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auctions_LotId",
                table: "auctions",
                column: "LotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bids_AuctionId",
                table: "bids",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_lot_media_LotId_media_key_media_type",
                table: "lot_media",
                columns: new[] { "LotId", "media_key", "media_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_AuctionId",
                table: "orders",
                column: "AuctionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_BuyerId",
                table: "orders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_SellerId",
                table: "orders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_Status_PaymentDeadlineUtc",
                table: "orders",
                columns: new[] { "Status", "PaymentDeadlineUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_Token",
                table: "refresh_tokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_UserName",
                table: "users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bids");

            migrationBuilder.DropTable(
                name: "lot_media");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "auctions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "lots");
        }
    }
}
