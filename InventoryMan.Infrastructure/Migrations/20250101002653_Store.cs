using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryMan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Store : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductId1",
                table: "Inventories",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Address", "Name", "Phone" },
                values: new object[,]
                {
                    { "5d0afaec-9769-48ec-b040-ca000c8aae97", "Address 1", "Store 1", "123456789" },
                    { "803f2448-2174-476e-8c85-084cc84ae7dd", "Address 2", "Store 2", "987654321" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductId1",
                table: "Inventories",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_StoreId",
                table: "Inventories",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Products_ProductId1",
                table: "Inventories",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Stores_StoreId",
                table: "Inventories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Products_ProductId1",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Stores_StoreId",
                table: "Inventories");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_ProductId1",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_StoreId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "Inventories");
        }
    }
}
