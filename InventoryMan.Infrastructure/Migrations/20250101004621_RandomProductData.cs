using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryMan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RandomProductData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: "5d0afaec-9769-48ec-b040-ca000c8aae97");

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: "803f2448-2174-476e-8c85-084cc84ae7dd");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Price", "Sku" },
                values: new object[,]
                {
                    { "0458142f-5b46-44e6-8446-7e289d7c8367", 2, "Accesorio deportivo premium", "Reloj Deportivo Pro", 149.99m, "SPRT002" },
                    { "0be35a5f-8ce6-4e94-82db-69f0af49794d", 2, "Candado gym", "Gym Lock", 9.99m, "LOCK044" },
                    { "0d52d68f-d41d-4bb7-b038-26ee7f899fee", 1, "Producto electrónico versátil", "Smartphone X1", 299.99m, "SMRT001" },
                    { "129d4f38-db03-4676-aa9b-b66142b3da47", 1, "Webcam HD", "Webcam Pro 4K", 89.99m, "CAM017" },
                    { "1e0d9ee1-8783-4f40-8181-8adf9cdbdc2f", 2, "Banda cabeza", "Banda Deportiva", 9.99m, "HEAD032" },
                    { "244afcbb-e776-4cad-a8bf-c42ac14d21a2", 1, "Protector pantalla", "Screen Guard Pro", 14.99m, "SCRN033" },
                    { "2986537b-c53e-4b5f-8b88-46979e610973", 2, "Bolsa deporte", "Bolsa Gym Pro", 34.99m, "GYM024" },
                    { "2c0232f0-f13c-48e6-b980-26fa1d30bc69", 2, "Rodilleras pro", "Rodilleras Sport", 19.99m, "KNEE028" },
                    { "2ca94535-474b-484a-acc3-e7c66b02694f", 1, "Mini ventilador", "USB Fan", 12.99m, "FAN043" },
                    { "2d5e4f23-2a92-4298-a0f6-3fe2b1add03d", 1, "Cámara compacta HD", "Cámara Digital X20", 249.99m, "CAM007" },
                    { "31e9fe95-d564-45b7-8948-898af3c887b2", 1, "Protector cable", "Cable Protector", 6.99m, "PROT041" },
                    { "3691ec91-3f16-4ab3-a233-4aecdb0f4a89", 1, "Organizador cables", "Cable Manager", 8.99m, "ORG039" },
                    { "46fc73b3-51de-4d7c-98d0-dd9e77e7cac1", 1, "Cable HDMI", "HDMI Premium 2m", 19.99m, "HDMI025" },
                    { "4ddff097-fb1e-438c-b234-062c65db30bd", 2, "Timer digital", "Sport Timer", 18.99m, "TIME050" },
                    { "4f8f7fa9-dcfa-4e91-a336-27e5aa71add6", 2, "Equipo fitness resistente", "Pesas Ajustables", 199.99m, "FIT004" },
                    { "528fc4fa-4b7e-4c94-94ce-63b7464bff42", 2, "Botella deportiva", "Botella Térmica", 24.99m, "BTL016" },
                    { "5529cdb3-bfdd-43eb-b62f-a5de6c31743d", 1, "Limpiador pantalla", "Screen Cleaner", 9.99m, "CLN037" },
                    { "5885822d-62c3-44d3-875f-80f56f7e20e5", 1, "Batería portátil", "PowerBank 20000", 49.99m, "PWR019" },
                    { "58a4db39-c2bb-4952-b1d1-fd7973c8b032", 1, "Tablet última generación", "Tablet Pro 10", 399.99m, "TAB005" },
                    { "5e51e2fe-9b9d-449b-a764-996274219b42", 1, "Funda tablet", "Funda Protectora", 24.99m, "CASE027" },
                    { "6c21a565-45b1-48ba-8270-2d98bd760c12", 2, "Equipo de entrenamiento", "Banda Elástica Pro", 19.99m, "BAND012" },
                    { "6d4c0bc3-8f04-4060-aa10-6816c6e49d00", 1, "Hub USB", "USB Hub 4-Port", 29.99m, "USB021" },
                    { "6dc5e238-6b32-4a8e-bc8e-80f500bda866", 2, "Muñequeras", "Wrist Support", 12.99m, "WRST036" },
                    { "71f9470f-1dec-4c69-bd23-bdece962a85e", 1, "Audio de alta calidad", "Auriculares Wireless", 89.99m, "AUDIO003" },
                    { "7251e7c5-d643-4bb8-b789-abbc9a2e8a12", 1, "Altavoz potente", "Speaker Bluetooth", 79.99m, "SPK009" },
                    { "74556b06-0cf0-47db-b44e-dffc17f83a07", 2, "Botella shake", "Shaker Pro", 16.99m, "SHAKE034" },
                    { "74ea1ae3-6213-475e-8a78-b9b71b8fd269", 1, "Smartwatch avanzado", "Reloj Smart V2", 199.99m, "WATCH011" },
                    { "75cfc489-a9fc-4fd8-be30-38ee542597be", 1, "Teclado gaming", "Teclado RGB Pro", 129.99m, "KEY013" },
                    { "77ba6783-53be-4e64-9431-a2e4145623c2", 1, "Hub cargador", "Charging Hub", 24.99m, "CHRG049" },
                    { "8a981b75-6542-4d9b-a486-dc38f35517f5", 2, "Calzado deportivo", "Zapatillas Pro-Run", 89.99m, "SHOE010" },
                    { "8d29bcf5-da58-4150-8dd2-5abfe0d08e23", 1, "Mouse gaming", "Mouse Gamer X", 59.99m, "MOUSE015" },
                    { "9ce5e3d1-89ca-463a-b7f3-2f196eb4e5cd", 2, "Toalla deportiva", "Toalla Microfibra", 14.99m, "TWL022" },
                    { "a41a0467-c531-4516-8670-ca1ae9080e7c", 2, "Banda cardio", "Monitor Cardíaco", 49.99m, "HRT026" },
                    { "a9e0651a-5178-4bd5-9c14-2194874c69ca", 1, "Almohadilla mouse", "Mouse Pad Pro", 11.99m, "PAD045" },
                    { "afed10c6-502d-4b3a-b15c-d454cc22913a", 2, "Accesorio yoga", "Mat Premium", 39.99m, "YOGA014" },
                    { "b157232c-ecee-42d6-b661-862936e888b3", 2, "Bolsa zapatos", "Shoe Bag Sport", 11.99m, "BAG038" },
                    { "b20ce64f-d1e0-4d8e-81f3-2021ecaeb68f", 2, "Guantes fitness", "Guantes Pro-Lift", 29.99m, "GLV020" },
                    { "b3410ac7-3c57-40af-a661-53319845cac3", 2, "Ropa deportiva premium", "Camiseta Tech-Fit", 34.99m, "WEAR008" },
                    { "b690e635-7e4d-4c48-b998-4089e1ca924f", 2, "Bolsa agua", "Water Bag", 15.99m, "H2O048" },
                    { "bbab8657-68ff-4433-9f5e-d52d79a4790a", 1, "Soporte tablet", "Tablet Mount", 16.99m, "MNT047" },
                    { "bc388c54-e3e9-40cc-9823-e8e2bd97ae6e", 2, "Mochila deportiva", "Mochila Sport Pro", 44.99m, "BAG018" },
                    { "bdba064f-ddd2-4125-b709-40ab9c1db8d1", 2, "Banda medición", "Measure Tape", 8.99m, "MEAS042" },
                    { "c1bb248f-cfc1-4f8f-b75d-81ce289fb1c2", 2, "Banda resistencia", "Resistance Band", 13.99m, "BAND046" },
                    { "c34a70e5-23ba-4749-b84e-e37074ef27f4", 2, "Equipamiento deportivo", "Balón Profesional", 29.99m, "BALL006" },
                    { "ca9a7f9c-3e4b-4565-942c-887723a1960e", 2, "Toalla pequeña", "Mini Towel", 7.99m, "TWL040" },
                    { "cf2dc6a4-21f6-4d31-8b5e-bd740cd8f785", 1, "Soporte laptop", "Soporte Ergonómico", 39.99m, "STND023" },
                    { "d81c63ac-c475-4b96-8be8-6b9cc55b6492", 1, "Luz LED USB", "LED Light Strip", 19.99m, "LED031" },
                    { "e2b71c5e-7a5e-408b-baa5-cb3d067ce8bb", 1, "Adaptador wifi", "WiFi Adapter Pro", 29.99m, "WIFI029" },
                    { "edf57f7c-c9d9-4735-b892-9d97aa8735f2", 2, "Cuerda saltar", "Cuerda Pro-Jump", 14.99m, "JUMP030" },
                    { "fcd719a9-ac17-458e-b99d-232aa1311abc", 1, "Soporte móvil", "Phone Stand", 15.99m, "STND035" }
                });

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Address", "Name", "Phone" },
                values: new object[,]
                {
                    { "32ba25e3-5b51-4dab-a889-d743fb2d8a61", "Address 1", "Store 1", "123456789" },
                    { "57589581-4939-4282-b47c-acf9a4906c37", "Address 2", "Store 2", "987654321" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "0458142f-5b46-44e6-8446-7e289d7c8367");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "0be35a5f-8ce6-4e94-82db-69f0af49794d");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "0d52d68f-d41d-4bb7-b038-26ee7f899fee");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "129d4f38-db03-4676-aa9b-b66142b3da47");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "1e0d9ee1-8783-4f40-8181-8adf9cdbdc2f");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "244afcbb-e776-4cad-a8bf-c42ac14d21a2");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "2986537b-c53e-4b5f-8b88-46979e610973");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "2c0232f0-f13c-48e6-b980-26fa1d30bc69");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "2ca94535-474b-484a-acc3-e7c66b02694f");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "2d5e4f23-2a92-4298-a0f6-3fe2b1add03d");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "31e9fe95-d564-45b7-8948-898af3c887b2");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "3691ec91-3f16-4ab3-a233-4aecdb0f4a89");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "46fc73b3-51de-4d7c-98d0-dd9e77e7cac1");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "4ddff097-fb1e-438c-b234-062c65db30bd");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "4f8f7fa9-dcfa-4e91-a336-27e5aa71add6");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "528fc4fa-4b7e-4c94-94ce-63b7464bff42");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "5529cdb3-bfdd-43eb-b62f-a5de6c31743d");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "5885822d-62c3-44d3-875f-80f56f7e20e5");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "58a4db39-c2bb-4952-b1d1-fd7973c8b032");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "5e51e2fe-9b9d-449b-a764-996274219b42");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "6c21a565-45b1-48ba-8270-2d98bd760c12");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "6d4c0bc3-8f04-4060-aa10-6816c6e49d00");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "6dc5e238-6b32-4a8e-bc8e-80f500bda866");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "71f9470f-1dec-4c69-bd23-bdece962a85e");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "7251e7c5-d643-4bb8-b789-abbc9a2e8a12");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "74556b06-0cf0-47db-b44e-dffc17f83a07");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "74ea1ae3-6213-475e-8a78-b9b71b8fd269");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "75cfc489-a9fc-4fd8-be30-38ee542597be");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "77ba6783-53be-4e64-9431-a2e4145623c2");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "8a981b75-6542-4d9b-a486-dc38f35517f5");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "8d29bcf5-da58-4150-8dd2-5abfe0d08e23");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "9ce5e3d1-89ca-463a-b7f3-2f196eb4e5cd");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "a41a0467-c531-4516-8670-ca1ae9080e7c");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "a9e0651a-5178-4bd5-9c14-2194874c69ca");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "afed10c6-502d-4b3a-b15c-d454cc22913a");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "b157232c-ecee-42d6-b661-862936e888b3");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "b20ce64f-d1e0-4d8e-81f3-2021ecaeb68f");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "b3410ac7-3c57-40af-a661-53319845cac3");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "b690e635-7e4d-4c48-b998-4089e1ca924f");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "bbab8657-68ff-4433-9f5e-d52d79a4790a");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "bc388c54-e3e9-40cc-9823-e8e2bd97ae6e");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "bdba064f-ddd2-4125-b709-40ab9c1db8d1");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "c1bb248f-cfc1-4f8f-b75d-81ce289fb1c2");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "c34a70e5-23ba-4749-b84e-e37074ef27f4");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "ca9a7f9c-3e4b-4565-942c-887723a1960e");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "cf2dc6a4-21f6-4d31-8b5e-bd740cd8f785");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "d81c63ac-c475-4b96-8be8-6b9cc55b6492");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "e2b71c5e-7a5e-408b-baa5-cb3d067ce8bb");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "edf57f7c-c9d9-4735-b892-9d97aa8735f2");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "fcd719a9-ac17-458e-b99d-232aa1311abc");

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: "32ba25e3-5b51-4dab-a889-d743fb2d8a61");

            migrationBuilder.DeleteData(
                table: "Stores",
                keyColumn: "Id",
                keyValue: "57589581-4939-4282-b47c-acf9a4906c37");

            migrationBuilder.InsertData(
                table: "Stores",
                columns: new[] { "Id", "Address", "Name", "Phone" },
                values: new object[,]
                {
                    { "5d0afaec-9769-48ec-b040-ca000c8aae97", "Address 1", "Store 1", "123456789" },
                    { "803f2448-2174-476e-8c85-084cc84ae7dd", "Address 2", "Store 2", "987654321" }
                });
        }
    }
}
