using System;

using Relora.Persistance;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relora.Persistance.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ReloraDbContext))]
    [Migration("20260803120000_ExpandLotCatalogTaxonomy")]
    public partial class ExpandLotCatalogTaxonomy : Migration
    {
        private const string OtherClothingCategoryId = "11111111-1111-1111-1111-000000000019";
        private const string OtherColorId = "33333333-3333-3333-3333-000000000019";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    NameKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MeasurementProfile = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    NameKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "colors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    NameKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "measurement_definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Profile = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Key = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    LabelKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    Unit = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurement_definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "proof_document_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    NameKey = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proof_document_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "category_departments",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Department = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category_departments", x => new { x.CategoryId, x.Department });
                    table.ForeignKey(
                        name: "FK_category_departments_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "lots",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Parse(OtherClothingCategoryId));

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "lots",
                type: "text",
                nullable: false,
                defaultValue: "Unisex");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryColorId",
                table: "lots",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Parse(OtherColorId));

            migrationBuilder.AddColumn<string>(
                name: "ModelName",
                table: "lots",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcquisitionYear",
                table: "lots",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductionYear",
                table: "lots",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVintage",
                table: "lots",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VintageNotes",
                table: "lots",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "lot_materials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LotId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    OtherName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lot_materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lot_materials_lots_LotId",
                        column: x => x.LotId,
                        principalTable: "lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lot_secondary_colors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LotId = table.Column<Guid>(type: "uuid", nullable: false),
                    ColorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lot_secondary_colors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lot_secondary_colors_lots_LotId",
                        column: x => x.LotId,
                        principalTable: "lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lot_measurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lot_measurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lot_measurements_lots_LotId",
                        column: x => x.LotId,
                        principalTable: "lots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lot_proof_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LotId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalFileName = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_lot_proof_document_uploads", x => x.Id);
                });

            SeedCatalog(migrationBuilder);
            BackfillLots(migrationBuilder);
            CreateIndexes(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "category_departments");
            migrationBuilder.DropTable(name: "lot_materials");
            migrationBuilder.DropTable(name: "lot_secondary_colors");
            migrationBuilder.DropTable(name: "lot_measurements");
            migrationBuilder.DropTable(name: "lot_proof_documents");
            migrationBuilder.DropTable(name: "pending_lot_proof_document_uploads");
            migrationBuilder.DropTable(name: "measurement_definitions");
            migrationBuilder.DropTable(name: "proof_document_types");
            migrationBuilder.DropTable(name: "materials");
            migrationBuilder.DropTable(name: "colors");
            migrationBuilder.DropTable(name: "categories");

            migrationBuilder.DropIndex(name: "IX_lots_CategoryId", table: "lots");
            migrationBuilder.DropIndex(name: "IX_lots_Department", table: "lots");
            migrationBuilder.DropIndex(name: "IX_lots_PrimaryColorId", table: "lots");
            migrationBuilder.DropIndex(name: "IX_lots_IsVintage", table: "lots");
            migrationBuilder.DropIndex(name: "IX_lots_ProductionYear", table: "lots");

            migrationBuilder.DropColumn(name: "CategoryId", table: "lots");
            migrationBuilder.DropColumn(name: "Department", table: "lots");
            migrationBuilder.DropColumn(name: "PrimaryColorId", table: "lots");
            migrationBuilder.DropColumn(name: "ModelName", table: "lots");
            migrationBuilder.DropColumn(name: "AcquisitionYear", table: "lots");
            migrationBuilder.DropColumn(name: "ProductionYear", table: "lots");
            migrationBuilder.DropColumn(name: "IsVintage", table: "lots");
            migrationBuilder.DropColumn(name: "VintageNotes", table: "lots");
        }

        private static void CreateIndexes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(name: "IX_categories_ParentId", table: "categories", column: "ParentId");
            migrationBuilder.CreateIndex(name: "IX_categories_Slug", table: "categories", column: "Slug", unique: true);
            migrationBuilder.CreateIndex(name: "IX_materials_Code", table: "materials", column: "Code", unique: true);
            migrationBuilder.CreateIndex(name: "IX_colors_Code", table: "colors", column: "Code", unique: true);
            migrationBuilder.CreateIndex(name: "IX_measurement_definitions_Profile_Key", table: "measurement_definitions", columns: new[] { "Profile", "Key" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_proof_document_types_Code", table: "proof_document_types", column: "Code", unique: true);
            migrationBuilder.CreateIndex(name: "IX_lots_CategoryId", table: "lots", column: "CategoryId");
            migrationBuilder.CreateIndex(name: "IX_lots_Department", table: "lots", column: "Department");
            migrationBuilder.CreateIndex(name: "IX_lots_PrimaryColorId", table: "lots", column: "PrimaryColorId");
            migrationBuilder.CreateIndex(name: "IX_lots_IsVintage", table: "lots", column: "IsVintage");
            migrationBuilder.CreateIndex(name: "IX_lots_ProductionYear", table: "lots", column: "ProductionYear");
            migrationBuilder.CreateIndex(name: "IX_lot_materials_LotId_MaterialId", table: "lot_materials", columns: new[] { "LotId", "MaterialId" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_lot_secondary_colors_LotId_ColorId", table: "lot_secondary_colors", columns: new[] { "LotId", "ColorId" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_lot_measurements_LotId_Key", table: "lot_measurements", columns: new[] { "LotId", "Key" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_lot_proof_documents_LotId_StorageKey", table: "lot_proof_documents", columns: new[] { "LotId", "StorageKey" }, unique: true);
            migrationBuilder.CreateIndex(name: "IX_lot_proof_documents_OwnerId", table: "lot_proof_documents", column: "OwnerId");
            migrationBuilder.CreateIndex(name: "IX_pending_lot_proof_document_uploads_StorageKey", table: "pending_lot_proof_document_uploads", column: "StorageKey", unique: true);
            migrationBuilder.CreateIndex(name: "IX_pending_lot_proof_document_uploads_OwnerId", table: "pending_lot_proof_document_uploads", column: "OwnerId");
            migrationBuilder.CreateIndex(name: "IX_pending_lot_proof_document_uploads_CreatedAtUtc", table: "pending_lot_proof_document_uploads", column: "CreatedAtUtc");
        }

        private static void SeedCatalog(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                INSERT INTO categories ("Id", "ParentId", "NameKey", "Slug", "SortOrder", "IsActive", "MeasurementProfile", "CreatedAt")
                VALUES
                ('11111111-1111-1111-1111-000000000001', NULL, 'categories.clothing', 'clothing', 10, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000002', NULL, 'categories.shoes', 'shoes', 20, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000003', NULL, 'categories.bags', 'bags', 30, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000004', NULL, 'categories.accessories', 'accessories', 40, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000005', NULL, 'categories.jewellery', 'jewellery', 50, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000006', NULL, 'categories.watches', 'watches', 60, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000101', '11111111-1111-1111-1111-000000000001', 'categories.tShirts', 't-shirts', 101, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000102', '11111111-1111-1111-1111-000000000001', 'categories.shirts', 'shirts', 102, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000103', '11111111-1111-1111-1111-000000000001', 'categories.poloShirts', 'polo-shirts', 103, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000104', '11111111-1111-1111-1111-000000000001', 'categories.hoodies', 'hoodies', 104, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000105', '11111111-1111-1111-1111-000000000001', 'categories.sweatshirts', 'sweatshirts', 105, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000106', '11111111-1111-1111-1111-000000000001', 'categories.knitwear', 'knitwear', 106, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000107', '11111111-1111-1111-1111-000000000001', 'categories.jackets', 'jackets', 107, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000108', '11111111-1111-1111-1111-000000000001', 'categories.coats', 'coats', 108, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000109', '11111111-1111-1111-1111-000000000001', 'categories.blazers', 'blazers', 109, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000110', '11111111-1111-1111-1111-000000000001', 'categories.suits', 'suits', 110, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000111', '11111111-1111-1111-1111-000000000001', 'categories.jeans', 'jeans', 111, TRUE, 'trousers', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000112', '11111111-1111-1111-1111-000000000001', 'categories.trousers', 'trousers', 112, TRUE, 'trousers', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000113', '11111111-1111-1111-1111-000000000001', 'categories.shorts', 'shorts', 113, TRUE, 'trousers', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000114', '11111111-1111-1111-1111-000000000001', 'categories.skirts', 'skirts', 114, TRUE, 'skirts', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000115', '11111111-1111-1111-1111-000000000001', 'categories.dresses', 'dresses', 115, TRUE, 'dresses', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000116', '11111111-1111-1111-1111-000000000001', 'categories.jumpsuits', 'jumpsuits', 116, TRUE, 'dresses', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000117', '11111111-1111-1111-1111-000000000001', 'categories.swimwear', 'swimwear', 117, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000118', '11111111-1111-1111-1111-000000000001', 'categories.underwear', 'underwear', 118, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000019', '11111111-1111-1111-1111-000000000001', 'categories.otherClothing', 'other-clothing', 199, TRUE, 'tops', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000201', '11111111-1111-1111-1111-000000000002', 'categories.sneakers', 'sneakers', 201, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000202', '11111111-1111-1111-1111-000000000002', 'categories.boots', 'boots', 202, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000203', '11111111-1111-1111-1111-000000000002', 'categories.loafers', 'loafers', 203, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000204', '11111111-1111-1111-1111-000000000002', 'categories.laceUps', 'lace-ups', 204, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000205', '11111111-1111-1111-1111-000000000002', 'categories.sandals', 'sandals', 205, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000206', '11111111-1111-1111-1111-000000000002', 'categories.heels', 'heels', 206, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000207', '11111111-1111-1111-1111-000000000002', 'categories.flats', 'flats', 207, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000208', '11111111-1111-1111-1111-000000000002', 'categories.espadrilles', 'espadrilles', 208, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000209', '11111111-1111-1111-1111-000000000002', 'categories.slippers', 'slippers', 209, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000020', '11111111-1111-1111-1111-000000000002', 'categories.otherShoes', 'other-shoes', 299, TRUE, 'shoes', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000301', '11111111-1111-1111-1111-000000000003', 'categories.backpacks', 'backpacks', 301, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000302', '11111111-1111-1111-1111-000000000003', 'categories.toteBags', 'tote-bags', 302, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000303', '11111111-1111-1111-1111-000000000003', 'categories.shoulderBags', 'shoulder-bags', 303, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000304', '11111111-1111-1111-1111-000000000003', 'categories.crossbodyBags', 'crossbody-bags', 304, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000305', '11111111-1111-1111-1111-000000000003', 'categories.clutches', 'clutches', 305, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000306', '11111111-1111-1111-1111-000000000003', 'categories.briefcases', 'briefcases', 306, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000307', '11111111-1111-1111-1111-000000000003', 'categories.travelBags', 'travel-bags', 307, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000308', '11111111-1111-1111-1111-000000000003', 'categories.beltBags', 'belt-bags', 308, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000309', '11111111-1111-1111-1111-000000000003', 'categories.wallets', 'wallets', 309, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000310', '11111111-1111-1111-1111-000000000003', 'categories.cardHolders', 'card-holders', 310, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000021', '11111111-1111-1111-1111-000000000003', 'categories.otherBags', 'other-bags', 399, TRUE, 'bags', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000401', '11111111-1111-1111-1111-000000000004', 'categories.belts', 'belts', 401, TRUE, 'belts', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000402', '11111111-1111-1111-1111-000000000004', 'categories.sunglasses', 'sunglasses', 402, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000403', '11111111-1111-1111-1111-000000000004', 'categories.scarves', 'scarves', 403, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000404', '11111111-1111-1111-1111-000000000004', 'categories.hats', 'hats', 404, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000405', '11111111-1111-1111-1111-000000000004', 'categories.gloves', 'gloves', 405, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000406', '11111111-1111-1111-1111-000000000004', 'categories.ties', 'ties', 406, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000407', '11111111-1111-1111-1111-000000000004', 'categories.pocketSquares', 'pocket-squares', 407, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000408', '11111111-1111-1111-1111-000000000004', 'categories.cufflinks', 'cufflinks', 408, TRUE, 'jewellery_generic', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000409', '11111111-1111-1111-1111-000000000004', 'categories.keychains', 'keychains', 409, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000022', '11111111-1111-1111-1111-000000000004', 'categories.otherAccessories', 'other-accessories', 499, TRUE, 'none', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000501', '11111111-1111-1111-1111-000000000005', 'categories.rings', 'rings', 501, TRUE, 'jewellery_ring', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000502', '11111111-1111-1111-1111-000000000005', 'categories.bracelets', 'bracelets', 502, TRUE, 'jewellery_bracelet', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000503', '11111111-1111-1111-1111-000000000005', 'categories.necklaces', 'necklaces', 503, TRUE, 'jewellery_necklace', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000504', '11111111-1111-1111-1111-000000000005', 'categories.earrings', 'earrings', 504, TRUE, 'jewellery_generic', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000505', '11111111-1111-1111-1111-000000000005', 'categories.brooches', 'brooches', 505, TRUE, 'jewellery_generic', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000506', '11111111-1111-1111-1111-000000000005', 'categories.pendants', 'pendants', 506, TRUE, 'jewellery_necklace', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000023', '11111111-1111-1111-1111-000000000005', 'categories.otherJewellery', 'other-jewellery', 599, TRUE, 'jewellery_generic', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000601', '11111111-1111-1111-1111-000000000006', 'categories.wristWatches', 'wrist-watches', 601, TRUE, 'watches', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000602', '11111111-1111-1111-1111-000000000006', 'categories.pocketWatches', 'pocket-watches', 602, TRUE, 'watches', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000603', '11111111-1111-1111-1111-000000000006', 'categories.watchAccessories', 'watch-accessories', 603, TRUE, 'watches', '2026-08-03T00:00:00Z'),
                ('11111111-1111-1111-1111-000000000024', '11111111-1111-1111-1111-000000000006', 'categories.otherWatches', 'other-watches', 699, TRUE, 'watches', '2026-08-03T00:00:00Z')
                """);

            migrationBuilder.Sql("""
                INSERT INTO category_departments ("CategoryId", "Department")
                SELECT "Id", department
                FROM categories
                CROSS JOIN (VALUES ('Women'), ('Men'), ('Unisex')) AS allowed(department)
                """);

            migrationBuilder.Sql("""
                INSERT INTO materials ("Id", "Code", "NameKey", "SortOrder", "IsActive")
                VALUES
                ('22222222-2222-2222-2222-000000000001', 'cotton', 'materials.cotton', 1, TRUE),
                ('22222222-2222-2222-2222-000000000002', 'wool', 'materials.wool', 2, TRUE),
                ('22222222-2222-2222-2222-000000000003', 'cashmere', 'materials.cashmere', 3, TRUE),
                ('22222222-2222-2222-2222-000000000004', 'silk', 'materials.silk', 4, TRUE),
                ('22222222-2222-2222-2222-000000000005', 'linen', 'materials.linen', 5, TRUE),
                ('22222222-2222-2222-2222-000000000006', 'leather', 'materials.leather', 6, TRUE),
                ('22222222-2222-2222-2222-000000000007', 'suede', 'materials.suede', 7, TRUE),
                ('22222222-2222-2222-2222-000000000008', 'denim', 'materials.denim', 8, TRUE),
                ('22222222-2222-2222-2222-000000000009', 'polyester', 'materials.polyester', 9, TRUE),
                ('22222222-2222-2222-2222-000000000010', 'nylon', 'materials.nylon', 10, TRUE),
                ('22222222-2222-2222-2222-000000000011', 'viscose', 'materials.viscose', 11, TRUE),
                ('22222222-2222-2222-2222-000000000012', 'acrylic', 'materials.acrylic', 12, TRUE),
                ('22222222-2222-2222-2222-000000000013', 'elastane', 'materials.elastane', 13, TRUE),
                ('22222222-2222-2222-2222-000000000014', 'fur', 'materials.fur', 14, TRUE),
                ('22222222-2222-2222-2222-000000000015', 'synthetic-leather', 'materials.syntheticLeather', 15, TRUE),
                ('22222222-2222-2222-2222-000000000016', 'rubber', 'materials.rubber', 16, TRUE),
                ('22222222-2222-2222-2222-000000000017', 'metal', 'materials.metal', 17, TRUE),
                ('22222222-2222-2222-2222-000000000018', 'gold', 'materials.gold', 18, TRUE),
                ('22222222-2222-2222-2222-000000000019', 'silver', 'materials.silver', 19, TRUE),
                ('22222222-2222-2222-2222-000000000020', 'platinum', 'materials.platinum', 20, TRUE),
                ('22222222-2222-2222-2222-000000000021', 'ceramic', 'materials.ceramic', 21, TRUE),
                ('22222222-2222-2222-2222-000000000022', 'canvas', 'materials.canvas', 22, TRUE),
                ('22222222-2222-2222-2222-000000000023', 'velvet', 'materials.velvet', 23, TRUE),
                ('22222222-2222-2222-2222-000000000024', 'other', 'materials.other', 99, TRUE)
                """);

            migrationBuilder.Sql("""
                INSERT INTO colors ("Id", "Code", "NameKey", "SortOrder", "IsActive")
                VALUES
                ('33333333-3333-3333-3333-000000000001', 'black', 'colors.black', 1, TRUE),
                ('33333333-3333-3333-3333-000000000002', 'white', 'colors.white', 2, TRUE),
                ('33333333-3333-3333-3333-000000000003', 'grey', 'colors.grey', 3, TRUE),
                ('33333333-3333-3333-3333-000000000004', 'beige', 'colors.beige', 4, TRUE),
                ('33333333-3333-3333-3333-000000000005', 'brown', 'colors.brown', 5, TRUE),
                ('33333333-3333-3333-3333-000000000006', 'red', 'colors.red', 6, TRUE),
                ('33333333-3333-3333-3333-000000000007', 'burgundy', 'colors.burgundy', 7, TRUE),
                ('33333333-3333-3333-3333-000000000008', 'orange', 'colors.orange', 8, TRUE),
                ('33333333-3333-3333-3333-000000000009', 'yellow', 'colors.yellow', 9, TRUE),
                ('33333333-3333-3333-3333-000000000010', 'green', 'colors.green', 10, TRUE),
                ('33333333-3333-3333-3333-000000000011', 'blue', 'colors.blue', 11, TRUE),
                ('33333333-3333-3333-3333-000000000012', 'navy', 'colors.navy', 12, TRUE),
                ('33333333-3333-3333-3333-000000000013', 'purple', 'colors.purple', 13, TRUE),
                ('33333333-3333-3333-3333-000000000014', 'pink', 'colors.pink', 14, TRUE),
                ('33333333-3333-3333-3333-000000000015', 'gold', 'colors.gold', 15, TRUE),
                ('33333333-3333-3333-3333-000000000016', 'silver', 'colors.silver', 16, TRUE),
                ('33333333-3333-3333-3333-000000000017', 'multicolor', 'colors.multicolor', 17, TRUE),
                ('33333333-3333-3333-3333-000000000018', 'transparent', 'colors.transparent', 18, TRUE),
                ('33333333-3333-3333-3333-000000000019', 'other', 'colors.other', 99, TRUE)
                """);

            migrationBuilder.Sql("""
                INSERT INTO proof_document_types ("Id", "Code", "NameKey", "SortOrder", "IsActive")
                VALUES
                ('44444444-4444-4444-4444-000000000001', 'receipt', 'proofDocuments.receipt', 1, TRUE),
                ('44444444-4444-4444-4444-000000000002', 'invoice', 'proofDocuments.invoice', 2, TRUE),
                ('44444444-4444-4444-4444-000000000003', 'authenticity-card', 'proofDocuments.authenticityCard', 3, TRUE),
                ('44444444-4444-4444-4444-000000000004', 'order-confirmation', 'proofDocuments.orderConfirmation', 4, TRUE),
                ('44444444-4444-4444-4444-000000000005', 'other-supporting-document', 'proofDocuments.otherSupportingDocument', 99, TRUE)
                """);

            migrationBuilder.Sql("""
                INSERT INTO measurement_definitions ("Id", "Profile", "Key", "LabelKey", "IsRequired", "SortOrder", "Unit", "IsActive")
                VALUES
                ('55555555-5555-5555-5555-000000000001', 'tops', 'chest_width', 'measurements.chestWidth', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000002', 'tops', 'shoulder_width', 'measurements.shoulderWidth', TRUE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000003', 'tops', 'length', 'measurements.length', TRUE, 3, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000004', 'tops', 'sleeve_length', 'measurements.sleeveLength', TRUE, 4, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000005', 'tops', 'hem_width', 'measurements.hemWidth', FALSE, 5, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000006', 'trousers', 'waist', 'measurements.waist', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000007', 'trousers', 'rise', 'measurements.rise', TRUE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000008', 'trousers', 'inseam', 'measurements.inseam', TRUE, 3, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000009', 'trousers', 'outseam', 'measurements.outseam', TRUE, 4, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000010', 'trousers', 'leg_opening', 'measurements.legOpening', TRUE, 5, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000011', 'trousers', 'thigh_width', 'measurements.thighWidth', FALSE, 6, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000012', 'skirts', 'waist', 'measurements.waist', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000013', 'skirts', 'length', 'measurements.length', TRUE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000014', 'skirts', 'hips', 'measurements.hips', FALSE, 3, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000015', 'dresses', 'chest_width', 'measurements.chestWidth', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000016', 'dresses', 'waist', 'measurements.waist', TRUE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000017', 'dresses', 'hips', 'measurements.hips', TRUE, 3, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000018', 'dresses', 'shoulder_width', 'measurements.shoulderWidth', FALSE, 4, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000019', 'dresses', 'sleeve_length', 'measurements.sleeveLength', FALSE, 5, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000020', 'dresses', 'total_length', 'measurements.totalLength', TRUE, 6, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000021', 'shoes', 'insole_length', 'measurements.insoleLength', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000022', 'shoes', 'outsole_length', 'measurements.outsoleLength', FALSE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000023', 'shoes', 'width', 'measurements.width', FALSE, 3, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000024', 'shoes', 'heel_height', 'measurements.heelHeight', FALSE, 4, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000025', 'bags', 'width', 'measurements.width', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000026', 'bags', 'height', 'measurements.height', TRUE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000027', 'bags', 'depth', 'measurements.depth', TRUE, 3, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000028', 'bags', 'strap_drop', 'measurements.strapDrop', FALSE, 4, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000029', 'belts', 'total_length', 'measurements.totalLength', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000030', 'belts', 'width', 'measurements.width', TRUE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000031', 'belts', 'minimum_hole', 'measurements.minimumHole', TRUE, 3, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000032', 'belts', 'maximum_hole', 'measurements.maximumHole', TRUE, 4, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000033', 'jewellery_ring', 'ring_size', 'measurements.ringSize', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000034', 'jewellery_necklace', 'chain_length', 'measurements.chainLength', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000035', 'jewellery_bracelet', 'bracelet_length', 'measurements.braceletLength', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000036', 'jewellery_generic', 'weight', 'measurements.weight', FALSE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000037', 'jewellery_generic', 'dimensions', 'measurements.dimensions', FALSE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000038', 'watches', 'case_diameter', 'measurements.caseDiameter', TRUE, 1, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000039', 'watches', 'case_thickness', 'measurements.caseThickness', FALSE, 2, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000040', 'watches', 'lug_width', 'measurements.lugWidth', FALSE, 3, 'cm', TRUE),
                ('55555555-5555-5555-5555-000000000041', 'watches', 'strap_length', 'measurements.strapLength', FALSE, 4, 'cm', TRUE)
                """);
        }

        private static void BackfillLots(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                UPDATE lots
                SET
                    "Department" = CASE
                        WHEN "Gender" = 'Women' THEN 'Women'
                        WHEN "Gender" = 'Men' THEN 'Men'
                        ELSE 'Unisex'
                    END,
                    "IsVintage" = CASE WHEN "Category" = 'Vintage' THEN TRUE ELSE "IsVintage" END,
                    "VintageNotes" = CASE
                        WHEN "Category" = 'Vintage' AND "VintageNotes" IS NULL
                        THEN 'Legacy category value was Vintage before taxonomy migration.'
                        ELSE "VintageNotes"
                    END,
                    "CategoryId" = CASE
                        WHEN "Category" = 'Shoes' THEN '11111111-1111-1111-1111-000000000020'::uuid
                        WHEN "Category" = 'Accessories' THEN '11111111-1111-1111-1111-000000000022'::uuid
                        WHEN "Category" = 'Bags' THEN '11111111-1111-1111-1111-000000000021'::uuid
                        WHEN "Category" = 'Jewellery' THEN '11111111-1111-1111-1111-000000000023'::uuid
                        ELSE '{OtherClothingCategoryId}'::uuid
                    END,
                    "PrimaryColorId" = CASE LOWER(COALESCE("Color", ''))
                        WHEN 'black' THEN '33333333-3333-3333-3333-000000000001'::uuid
                        WHEN 'white' THEN '33333333-3333-3333-3333-000000000002'::uuid
                        WHEN 'grey' THEN '33333333-3333-3333-3333-000000000003'::uuid
                        WHEN 'gray' THEN '33333333-3333-3333-3333-000000000003'::uuid
                        WHEN 'beige' THEN '33333333-3333-3333-3333-000000000004'::uuid
                        WHEN 'brown' THEN '33333333-3333-3333-3333-000000000005'::uuid
                        WHEN 'red' THEN '33333333-3333-3333-3333-000000000006'::uuid
                        WHEN 'burgundy' THEN '33333333-3333-3333-3333-000000000007'::uuid
                        WHEN 'orange' THEN '33333333-3333-3333-3333-000000000008'::uuid
                        WHEN 'yellow' THEN '33333333-3333-3333-3333-000000000009'::uuid
                        WHEN 'green' THEN '33333333-3333-3333-3333-000000000010'::uuid
                        WHEN 'blue' THEN '33333333-3333-3333-3333-000000000011'::uuid
                        WHEN 'navy' THEN '33333333-3333-3333-3333-000000000012'::uuid
                        WHEN 'purple' THEN '33333333-3333-3333-3333-000000000013'::uuid
                        WHEN 'pink' THEN '33333333-3333-3333-3333-000000000014'::uuid
                        WHEN 'gold' THEN '33333333-3333-3333-3333-000000000015'::uuid
                        WHEN 'silver' THEN '33333333-3333-3333-3333-000000000016'::uuid
                        WHEN 'multicolor' THEN '33333333-3333-3333-3333-000000000017'::uuid
                        WHEN 'transparent' THEN '33333333-3333-3333-3333-000000000018'::uuid
                        ELSE '{OtherColorId}'::uuid
                    END
                """);
        }
    }
}
