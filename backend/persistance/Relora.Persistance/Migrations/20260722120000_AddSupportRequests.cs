using System;

using Relora.Persistance;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations;

[DbContext(typeof(ReloraDbContext))]
[Migration("20260722120000_AddSupportRequests")]
public partial class AddSupportRequests : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "support_requests",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Message = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_support_requests", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_support_requests_CreatedAt",
            table: "support_requests",
            column: "CreatedAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "support_requests");
    }
}
