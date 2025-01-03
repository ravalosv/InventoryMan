using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryMan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Sku = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    StoreId = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    MinStock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventories_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    SourceStoreId = table.Column<string>(type: "text", nullable: true),
                    TargetStoreId = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Electrónicos y accesorios tecnológicos" },
                    { 2, "Deportivos y fitness" }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Address", "Name", "Phone" },
                values: new object[,]
                {
                    { "1390508a-e79a-451f-8be7-975a9b6a2283", "Address 2", "Store 2", "987654321" },
                    { "15617380-1ce6-4a3d-9214-81dc79b6557b", "Address 1", "Store 1", "123456789" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Price", "Sku" },
                values: new object[,]
                {
                    { "00bb8276-4385-4c5a-b0bf-63b5181c83aa", 1, "Webcam HD", "Webcam Pro 4K", 89.99m, "CAM017" },
                    { "09e93d66-76c4-4e92-88da-6b6a6c2845ed", 2, "Muñequeras", "Wrist Support", 12.99m, "WRST036" },
                    { "1bc7071f-16cf-46f3-9362-219bcf6cb67c", 2, "Banda resistencia", "Resistance Band", 13.99m, "BAND046" },
                    { "1bd341ed-eda7-4705-b194-3389cc669379", 2, "Equipo de entrenamiento", "Banda Elástica Pro", 19.99m, "BAND012" },
                    { "1db82939-f5ce-4f25-9eeb-8e856e758d91", 1, "Hub USB", "USB Hub 4-Port", 29.99m, "USB021" },
                    { "1e52119c-c91f-4650-a6cc-4175dffdecb8", 1, "Organizador cables", "Cable Manager", 8.99m, "ORG039" },
                    { "2b40099a-f712-46c1-9c53-3428a59a2dce", 2, "Timer digital", "Sport Timer", 18.99m, "TIME050" },
                    { "2b407211-b97a-446b-b4fd-11e4f12bae09", 1, "Hub cargador", "Charging Hub", 24.99m, "CHRG049" },
                    { "2c2bc398-6c45-4b3b-9814-cae92100f875", 2, "Bolsa deporte", "Bolsa Gym Pro", 34.99m, "GYM024" },
                    { "2d492c20-9142-4b48-9a3e-a40248bff6c9", 2, "Accesorio deportivo premium", "Reloj Deportivo Pro", 149.99m, "SPRT002" },
                    { "2e34be53-22a2-4391-b69d-ebaa93fda91a", 2, "Bolsa zapatos", "Shoe Bag Sport", 11.99m, "BAG038" },
                    { "2f4b8631-cf65-416a-9e88-04e494051cec", 1, "Mini ventilador", "USB Fan", 12.99m, "FAN043" },
                    { "3a4181b4-cecf-45c1-94e8-6a1664789521", 2, "Bolsa agua", "Water Bag", 15.99m, "H2O048" },
                    { "3dbbea4d-47ad-4b87-92c7-c9ef86cc13f2", 1, "Batería portátil", "PowerBank 20000", 49.99m, "PWR019" },
                    { "3eb60abe-615e-438f-9e23-dc9081d545fb", 1, "Audio de alta calidad", "Auriculares Wireless", 89.99m, "AUDIO003" },
                    { "4754213a-7302-4d51-a9ba-cb35b96fd816", 2, "Calzado deportivo", "Zapatillas Pro-Run", 89.99m, "SHOE010" },
                    { "47770dd4-248c-48e5-95d4-5e5bdedcf7f7", 1, "Adaptador wifi", "WiFi Adapter Pro", 29.99m, "WIFI029" },
                    { "47a483fc-ec63-46cb-8b14-3bcd1a7d9aba", 2, "Toalla pequeña", "Mini Towel", 7.99m, "TWL040" },
                    { "50da2d59-02f7-4343-8c61-85b6eb2e6416", 2, "Banda cabeza", "Banda Deportiva", 9.99m, "HEAD032" },
                    { "531d603f-0a07-4074-a0ec-92d9d4c5f7a0", 2, "Ropa deportiva premium", "Camiseta Tech-Fit", 34.99m, "WEAR008" },
                    { "5913e21b-b218-4f92-964e-70ba2ddce88c", 1, "Funda tablet", "Funda Protectora", 24.99m, "CASE027" },
                    { "591d74d8-5697-4a1c-a20f-e746cc9236db", 1, "Tablet última generación", "Tablet Pro 10", 399.99m, "TAB005" },
                    { "5a71b745-2327-4897-a3bd-534763c9778b", 1, "Limpiador pantalla", "Screen Cleaner", 9.99m, "CLN037" },
                    { "5b8df2cf-c6f0-4291-83fc-e34a0a6b1304", 1, "Protector pantalla", "Screen Guard Pro", 14.99m, "SCRN033" },
                    { "60f92368-1856-4080-a722-7ac4e4d285a1", 2, "Equipo fitness resistente", "Pesas Ajustables", 199.99m, "FIT004" },
                    { "6362dc1b-9d03-4076-9d61-d925fd75a64b", 1, "Protector cable", "Cable Protector", 6.99m, "PROT041" },
                    { "67565054-b183-4d07-b2f5-ec35d0ee3b69", 2, "Cuerda saltar", "Cuerda Pro-Jump", 14.99m, "JUMP030" },
                    { "69fde627-996e-4c3d-98a7-e07720d4b4a3", 2, "Banda medición", "Measure Tape", 8.99m, "MEAS042" },
                    { "6b4dc4a0-9f8e-4c46-a81d-285d7439b5d6", 1, "Mouse gaming", "Mouse Gamer X", 59.99m, "MOUSE015" },
                    { "6e04c586-7bdb-46ec-9cf4-ec4951a5bc5a", 1, "Soporte móvil", "Phone Stand", 15.99m, "STND035" },
                    { "78167a58-c0e7-4c69-bdd4-082393605df5", 1, "Teclado gaming", "Teclado RGB Pro", 129.99m, "KEY013" },
                    { "88ed405b-3170-49e8-987b-765affbfe0b3", 2, "Accesorio yoga", "Mat Premium", 39.99m, "YOGA014" },
                    { "8ae3adfa-5e62-4215-8610-244701336ab7", 2, "Botella deportiva", "Botella Térmica", 24.99m, "BTL016" },
                    { "94bdecb3-c518-4faa-9f2c-3a9bfe098c00", 1, "Cable HDMI", "HDMI Premium 2m", 19.99m, "HDMI025" },
                    { "958e3313-d442-4905-916c-586c36fe13fa", 1, "Smartwatch avanzado", "Reloj Smart V2", 199.99m, "WATCH011" },
                    { "9abda882-d75d-453c-b44c-3210bbfa7dd5", 2, "Mochila deportiva", "Mochila Sport Pro", 44.99m, "BAG018" },
                    { "9d6f3e06-9a0a-47fe-a5d5-fb9dd670f95e", 2, "Toalla deportiva", "Toalla Microfibra", 14.99m, "TWL022" },
                    { "9ddecdab-9eaa-40ff-8cb1-499cc31cc645", 1, "Luz LED USB", "LED Light Strip", 19.99m, "LED031" },
                    { "a177f35e-1d05-4185-9db7-53cc1a51cd63", 1, "Producto electrónico versátil", "Smartphone X1", 299.99m, "SMRT001" },
                    { "a317bc38-7b1b-473c-a49f-24a2f0704dfa", 1, "Almohadilla mouse", "Mouse Pad Pro", 11.99m, "PAD045" },
                    { "b6a559c6-8765-4bbf-a9ee-0f5edfc42d32", 1, "Altavoz potente", "Speaker Bluetooth", 79.99m, "SPK009" },
                    { "ba59b6d2-b0d1-4eae-b64b-1fd835133f83", 2, "Banda cardio", "Monitor Cardíaco", 49.99m, "HRT026" },
                    { "c54d10ba-410c-4214-970d-dd4d36b45da1", 1, "Soporte laptop", "Soporte Ergonómico", 39.99m, "STND023" },
                    { "d16c3aba-c08b-41cf-8e30-0ecfb035ad2d", 2, "Equipamiento deportivo", "Balón Profesional", 29.99m, "BALL006" },
                    { "e683209c-aed1-40d0-a6f2-2459ed1138a1", 2, "Guantes fitness", "Guantes Pro-Lift", 29.99m, "GLV020" },
                    { "ea185c87-a647-49bf-bce0-497b501e1701", 2, "Botella shake", "Shaker Pro", 16.99m, "SHAKE034" },
                    { "f5606e42-12df-45b5-9a28-79f156103f59", 2, "Candado gym", "Gym Lock", 9.99m, "LOCK044" },
                    { "fbb4230f-cf9d-4eec-b9e6-93b266372e9f", 2, "Rodilleras pro", "Rodilleras Sport", 19.99m, "KNEE028" },
                    { "fdaac353-77f4-4984-8856-b192147355a9", 1, "Cámara compacta HD", "Cámara Digital X20", 249.99m, "CAM007" },
                    { "fe135bab-8bfa-47f2-b1b5-b35652a67fdb", 1, "Soporte tablet", "Tablet Mount", 16.99m, "MNT047" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductId_StoreId",
                table: "Inventories",
                columns: new[] { "ProductId", "StoreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_StoreId",
                table: "Inventories",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Movements_ProductId",
                table: "Movements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                table: "Products",
                column: "Sku",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Movements");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");
        }
    }
}
