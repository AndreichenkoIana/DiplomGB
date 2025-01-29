using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketDb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "productgroups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("productgroup_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductGroupId = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_productgroups_ProductGroupId",
                        column: x => x.ProductGroupId,
                        principalTable: "productgroups",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "stores",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("store_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_stores_products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "products",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_products_ProductGroupId",
                table: "products",
                column: "ProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_stores_ProductId",
                table: "stores",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stores");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "productgroups");
        }
    }
}
