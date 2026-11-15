using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportRequestUserAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "support_requests",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "New");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "support_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_support_requests_Status",
                table: "support_requests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_support_requests_UserId",
                table: "support_requests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_support_requests_users_UserId",
                table: "support_requests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_support_requests_users_UserId",
                table: "support_requests");

            migrationBuilder.DropIndex(
                name: "IX_support_requests_Status",
                table: "support_requests");

            migrationBuilder.DropIndex(
                name: "IX_support_requests_UserId",
                table: "support_requests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "support_requests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "support_requests");
        }
    }
}
