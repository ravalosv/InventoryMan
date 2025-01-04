using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryMan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                name: "Test",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.Id);
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
                    { "STR-01", "Address 1", "Store 1", "123456789" },
                    { "STR-02", "Address 2", "Store 2", "987654321" },
                    { "STR-03", "Address 3", "Store 3", "985632147" }
                });

            migrationBuilder.InsertData(
                table: "Test",
                columns: new[] { "Id", "Data" },
                values: new object[] { 1, "Database connection successfully established" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Price", "Sku" },
                values: new object[,]
                {
                    { "ELEC-AUDIO-001", 1, "Audio de alta calidad", "Auriculares Wireless", 89.99m, "AUDIO003" },
                    { "ELEC-CABL-001", 1, "Cable HDMI", "HDMI Premium 2m", 19.99m, "HDMI025" },
                    { "ELEC-CAM-001", 1, "Cámara compacta HD", "Cámara Digital X20", 249.99m, "CAM007" },
                    { "ELEC-CASE-001", 1, "Funda tablet", "Funda Protectora", 24.99m, "CASE027" },
                    { "ELEC-CHRG-001", 1, "Hub cargador", "Charging Hub", 24.99m, "CHRG049" },
                    { "ELEC-CLNR-001", 1, "Limpiador pantalla", "Screen Cleaner", 9.99m, "CLN037" },
                    { "ELEC-FAN-001", 1, "Mini ventilador", "USB Fan", 12.99m, "FAN043" },
                    { "ELEC-HUB-001", 1, "Hub USB", "USB Hub 4-Port", 29.99m, "USB021" },
                    { "ELEC-KEYB-001", 1, "Teclado gaming", "Teclado RGB Pro", 129.99m, "KEY013" },
                    { "ELEC-LED-001", 1, "Luz LED USB", "LED Light Strip", 19.99m, "LED031" },
                    { "ELEC-MNT-001", 1, "Soporte tablet", "Tablet Mount", 16.99m, "MNT047" },
                    { "ELEC-MOUS-001", 1, "Mouse gaming", "Mouse Gamer X", 59.99m, "MOUSE015" },
                    { "ELEC-ORG-001", 1, "Organizador cables", "Cable Manager", 8.99m, "ORG039" },
                    { "ELEC-PAD-001", 1, "Almohadilla mouse", "Mouse Pad Pro", 11.99m, "PAD045" },
                    { "ELEC-PHST-001", 1, "Soporte móvil", "Phone Stand", 15.99m, "STND035" },
                    { "ELEC-POWR-001", 1, "Batería portátil", "PowerBank 20000", 49.99m, "PWR019" },
                    { "ELEC-PROT-001", 1, "Protector cable", "Cable Protector", 6.99m, "PROT041" },
                    { "ELEC-SCRN-001", 1, "Protector pantalla", "Screen Guard Pro", 14.99m, "SCRN033" },
                    { "ELEC-SMART-001", 1, "Producto electrónico versátil", "Smartphone X1", 299.99m, "SMRT001" },
                    { "ELEC-SPKR-001", 1, "Altavoz potente", "Speaker Bluetooth", 79.99m, "SPK009" },
                    { "ELEC-STND-001", 1, "Soporte laptop", "Soporte Ergonómico", 39.99m, "STND023" },
                    { "ELEC-TABL-001", 1, "Tablet última generación", "Tablet Pro 10", 399.99m, "TAB005" },
                    { "ELEC-WATCH-001", 1, "Smartwatch avanzado", "Reloj Smart V2", 199.99m, "WATCH011" },
                    { "ELEC-WCAM-001", 1, "Webcam HD", "Webcam Pro 4K", 89.99m, "CAM017" },
                    { "ELEC-WIFI-001", 1, "Adaptador wifi", "WiFi Adapter Pro", 29.99m, "WIFI029" },
                    { "SPRT-BAG-001", 2, "Bolsa deporte", "Bolsa Gym Pro", 34.99m, "GYM024" },
                    { "SPRT-BALL-001", 2, "Equipamiento deportivo", "Balón Profesional", 29.99m, "BALL006" },
                    { "SPRT-BAND-001", 2, "Equipo de entrenamiento", "Banda Elástica Pro", 19.99m, "BAND012" },
                    { "SPRT-BOTL-001", 2, "Botella deportiva", "Botella Térmica", 24.99m, "BTL016" },
                    { "SPRT-FITNS-001", 2, "Equipo fitness resistente", "Pesas Ajustables", 199.99m, "FIT004" },
                    { "SPRT-GLOV-001", 2, "Guantes fitness", "Guantes Pro-Lift", 29.99m, "GLV020" },
                    { "SPRT-H2O-001", 2, "Bolsa agua", "Water Bag", 15.99m, "H2O048" },
                    { "SPRT-HEAD-001", 2, "Banda cabeza", "Banda Deportiva", 9.99m, "HEAD032" },
                    { "SPRT-HRTE-001", 2, "Banda cardio", "Monitor Cardíaco", 49.99m, "HRT026" },
                    { "SPRT-KNEE-001", 2, "Rodilleras pro", "Rodilleras Sport", 19.99m, "KNEE028" },
                    { "SPRT-LOCK-001", 2, "Candado gym", "Gym Lock", 9.99m, "LOCK044" },
                    { "SPRT-MEAS-001", 2, "Banda medición", "Measure Tape", 8.99m, "MEAS042" },
                    { "SPRT-MTOW-001", 2, "Toalla pequeña", "Mini Towel", 7.99m, "TWL040" },
                    { "SPRT-PACK-001", 2, "Mochila deportiva", "Mochila Sport Pro", 44.99m, "BAG018" },
                    { "SPRT-RBAN-001", 2, "Banda resistencia", "Resistance Band", 13.99m, "BAND046" },
                    { "SPRT-ROPE-001", 2, "Cuerda saltar", "Cuerda Pro-Jump", 14.99m, "JUMP030" },
                    { "SPRT-SBAG-001", 2, "Bolsa zapatos", "Shoe Bag Sport", 11.99m, "BAG038" },
                    { "SPRT-SHKR-001", 2, "Botella shake", "Shaker Pro", 16.99m, "SHAKE034" },
                    { "SPRT-SHOE-001", 2, "Calzado deportivo", "Zapatillas Pro-Run", 89.99m, "SHOE010" },
                    { "SPRT-TIME-001", 2, "Timer digital", "Sport Timer", 19.99m, "TIME050" },
                    { "SPRT-TOWL-001", 2, "Toalla deportiva", "Toalla Microfibra", 14.99m, "TWL022" },
                    { "SPRT-WATCH-001", 2, "Accesorio deportivo premium", "Reloj Deportivo Pro", 149.99m, "SPRT002" },
                    { "SPRT-WEAR-001", 2, "Ropa deportiva premium", "Camiseta Tech-Fit", 34.99m, "WEAR008" },
                    { "SPRT-WRST-001", 2, "Muñequeras", "Wrist Support", 12.99m, "WRST036" },
                    { "SPRT-YOGA-001", 2, "Accesorio yoga", "Mat Premium", 39.99m, "YOGA014" }
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
                name: "Test");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ProductCategories");
        }
    }
}
