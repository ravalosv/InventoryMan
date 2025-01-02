﻿// <auto-generated />
using System;
using InventoryMan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InventoryMan.Infrastructure.Migrations
{
    [DbContext(typeof(InventoryDbContext))]
    [Migration("20250102165634_MakeTargetStoreIdNullable")]
    partial class MakeTargetStoreIdNullable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("InventoryMan.Core.Entities.Inventory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("MinStock")
                        .HasColumnType("integer");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProductId1")
                        .HasColumnType("text");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("StoreId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProductId1");

                    b.HasIndex("StoreId");

                    b.HasIndex("ProductId", "StoreId")
                        .IsUnique();

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.Movement", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ProductId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("SourceStoreId")
                        .HasColumnType("text");

                    b.Property<string>("TargetStoreId")
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Movements");
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.Product", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Sku")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("Sku")
                        .IsUnique();

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = "01aac1bc-7040-4c27-94f0-49d55ed666f9",
                            CategoryId = 1,
                            Description = "Producto electrónico versátil",
                            Name = "Smartphone X1",
                            Price = 299.99m,
                            Sku = "SMRT001"
                        },
                        new
                        {
                            Id = "19791365-7235-4769-bc8d-a777130c7a29",
                            CategoryId = 2,
                            Description = "Accesorio deportivo premium",
                            Name = "Reloj Deportivo Pro",
                            Price = 149.99m,
                            Sku = "SPRT002"
                        },
                        new
                        {
                            Id = "5f12e42e-5190-4c04-8ffa-122273e334ee",
                            CategoryId = 1,
                            Description = "Audio de alta calidad",
                            Name = "Auriculares Wireless",
                            Price = 89.99m,
                            Sku = "AUDIO003"
                        },
                        new
                        {
                            Id = "58c70461-1ac9-4052-86ad-8980cf783343",
                            CategoryId = 2,
                            Description = "Equipo fitness resistente",
                            Name = "Pesas Ajustables",
                            Price = 199.99m,
                            Sku = "FIT004"
                        },
                        new
                        {
                            Id = "d4d3a7ba-f98e-4f4e-80e2-5d1d98aaf1a9",
                            CategoryId = 1,
                            Description = "Tablet última generación",
                            Name = "Tablet Pro 10",
                            Price = 399.99m,
                            Sku = "TAB005"
                        },
                        new
                        {
                            Id = "722c528b-b126-4e30-845a-87ef62df794c",
                            CategoryId = 2,
                            Description = "Equipamiento deportivo",
                            Name = "Balón Profesional",
                            Price = 29.99m,
                            Sku = "BALL006"
                        },
                        new
                        {
                            Id = "7e5f151d-c8bc-46a0-9344-b253bfcc66dc",
                            CategoryId = 1,
                            Description = "Cámara compacta HD",
                            Name = "Cámara Digital X20",
                            Price = 249.99m,
                            Sku = "CAM007"
                        },
                        new
                        {
                            Id = "6bb77eb5-0749-4e8c-984c-eff8ce932faa",
                            CategoryId = 2,
                            Description = "Ropa deportiva premium",
                            Name = "Camiseta Tech-Fit",
                            Price = 34.99m,
                            Sku = "WEAR008"
                        },
                        new
                        {
                            Id = "1d0f94d7-b44c-43f4-babf-e0a21dfb6140",
                            CategoryId = 1,
                            Description = "Altavoz potente",
                            Name = "Speaker Bluetooth",
                            Price = 79.99m,
                            Sku = "SPK009"
                        },
                        new
                        {
                            Id = "26d62b66-c179-4f86-8ce9-bc2648d47bd9",
                            CategoryId = 2,
                            Description = "Calzado deportivo",
                            Name = "Zapatillas Pro-Run",
                            Price = 89.99m,
                            Sku = "SHOE010"
                        },
                        new
                        {
                            Id = "5bf06244-b7b8-40e6-b21f-92c61bdc6bed",
                            CategoryId = 1,
                            Description = "Smartwatch avanzado",
                            Name = "Reloj Smart V2",
                            Price = 199.99m,
                            Sku = "WATCH011"
                        },
                        new
                        {
                            Id = "3a72cb56-8fde-40b8-a424-90c1d016e518",
                            CategoryId = 2,
                            Description = "Equipo de entrenamiento",
                            Name = "Banda Elástica Pro",
                            Price = 19.99m,
                            Sku = "BAND012"
                        },
                        new
                        {
                            Id = "00f8d837-692d-4fdf-887a-986fccc618c0",
                            CategoryId = 1,
                            Description = "Teclado gaming",
                            Name = "Teclado RGB Pro",
                            Price = 129.99m,
                            Sku = "KEY013"
                        },
                        new
                        {
                            Id = "551b442e-5b8b-42d9-a86b-de8b6cf1cfc6",
                            CategoryId = 2,
                            Description = "Accesorio yoga",
                            Name = "Mat Premium",
                            Price = 39.99m,
                            Sku = "YOGA014"
                        },
                        new
                        {
                            Id = "1d9ff12a-bc0f-40ee-bd90-2d0a77682977",
                            CategoryId = 1,
                            Description = "Mouse gaming",
                            Name = "Mouse Gamer X",
                            Price = 59.99m,
                            Sku = "MOUSE015"
                        },
                        new
                        {
                            Id = "1ae1cf9c-9491-4f73-9b71-6419596114cd",
                            CategoryId = 2,
                            Description = "Botella deportiva",
                            Name = "Botella Térmica",
                            Price = 24.99m,
                            Sku = "BTL016"
                        },
                        new
                        {
                            Id = "dafb0014-3391-426f-b649-c31359d2aa39",
                            CategoryId = 1,
                            Description = "Webcam HD",
                            Name = "Webcam Pro 4K",
                            Price = 89.99m,
                            Sku = "CAM017"
                        },
                        new
                        {
                            Id = "bd640ac5-a3d7-4b9d-a86e-931b4b8170d9",
                            CategoryId = 2,
                            Description = "Mochila deportiva",
                            Name = "Mochila Sport Pro",
                            Price = 44.99m,
                            Sku = "BAG018"
                        },
                        new
                        {
                            Id = "fa7906a5-42f7-426d-98de-9ff0fab77e71",
                            CategoryId = 1,
                            Description = "Batería portátil",
                            Name = "PowerBank 20000",
                            Price = 49.99m,
                            Sku = "PWR019"
                        },
                        new
                        {
                            Id = "f200307a-ffc4-4d0f-80fc-7b124de70ef9",
                            CategoryId = 2,
                            Description = "Guantes fitness",
                            Name = "Guantes Pro-Lift",
                            Price = 29.99m,
                            Sku = "GLV020"
                        },
                        new
                        {
                            Id = "3fa46745-d9a7-416c-927a-4e2bc3ad43c2",
                            CategoryId = 1,
                            Description = "Hub USB",
                            Name = "USB Hub 4-Port",
                            Price = 29.99m,
                            Sku = "USB021"
                        },
                        new
                        {
                            Id = "97344665-5fa8-4268-83ee-1f908093a13d",
                            CategoryId = 2,
                            Description = "Toalla deportiva",
                            Name = "Toalla Microfibra",
                            Price = 14.99m,
                            Sku = "TWL022"
                        },
                        new
                        {
                            Id = "20c5d19a-145a-4f4f-a0e5-ec9eebc9065c",
                            CategoryId = 1,
                            Description = "Soporte laptop",
                            Name = "Soporte Ergonómico",
                            Price = 39.99m,
                            Sku = "STND023"
                        },
                        new
                        {
                            Id = "00da811e-78e0-42a2-bc9b-3e6bce11f231",
                            CategoryId = 2,
                            Description = "Bolsa deporte",
                            Name = "Bolsa Gym Pro",
                            Price = 34.99m,
                            Sku = "GYM024"
                        },
                        new
                        {
                            Id = "36d9d3af-4b88-47af-a71c-21951785b740",
                            CategoryId = 1,
                            Description = "Cable HDMI",
                            Name = "HDMI Premium 2m",
                            Price = 19.99m,
                            Sku = "HDMI025"
                        },
                        new
                        {
                            Id = "8518ec3e-4ca9-47f4-87d6-6ddfa9549626",
                            CategoryId = 2,
                            Description = "Banda cardio",
                            Name = "Monitor Cardíaco",
                            Price = 49.99m,
                            Sku = "HRT026"
                        },
                        new
                        {
                            Id = "f24fda16-985a-4ea9-a939-24d09a8645c4",
                            CategoryId = 1,
                            Description = "Funda tablet",
                            Name = "Funda Protectora",
                            Price = 24.99m,
                            Sku = "CASE027"
                        },
                        new
                        {
                            Id = "daf3c3bd-c0e2-4598-98e9-359665742955",
                            CategoryId = 2,
                            Description = "Rodilleras pro",
                            Name = "Rodilleras Sport",
                            Price = 19.99m,
                            Sku = "KNEE028"
                        },
                        new
                        {
                            Id = "2b1c6faf-71f5-4a5f-b108-c4bc35e66514",
                            CategoryId = 1,
                            Description = "Adaptador wifi",
                            Name = "WiFi Adapter Pro",
                            Price = 29.99m,
                            Sku = "WIFI029"
                        },
                        new
                        {
                            Id = "a303a9dd-f571-4c32-804c-c9b8f564399e",
                            CategoryId = 2,
                            Description = "Cuerda saltar",
                            Name = "Cuerda Pro-Jump",
                            Price = 14.99m,
                            Sku = "JUMP030"
                        },
                        new
                        {
                            Id = "a09ecea8-ea49-4614-a33f-516cd0963af3",
                            CategoryId = 1,
                            Description = "Luz LED USB",
                            Name = "LED Light Strip",
                            Price = 19.99m,
                            Sku = "LED031"
                        },
                        new
                        {
                            Id = "b035b289-2fc4-44a3-9288-2295c6666dec",
                            CategoryId = 2,
                            Description = "Banda cabeza",
                            Name = "Banda Deportiva",
                            Price = 9.99m,
                            Sku = "HEAD032"
                        },
                        new
                        {
                            Id = "6c8bc185-a24b-43c4-b2ad-2e9fc7c764a2",
                            CategoryId = 1,
                            Description = "Protector pantalla",
                            Name = "Screen Guard Pro",
                            Price = 14.99m,
                            Sku = "SCRN033"
                        },
                        new
                        {
                            Id = "d7f6c600-6bc2-4fea-9471-7f8baa0ac800",
                            CategoryId = 2,
                            Description = "Botella shake",
                            Name = "Shaker Pro",
                            Price = 16.99m,
                            Sku = "SHAKE034"
                        },
                        new
                        {
                            Id = "dbfe92c9-54cb-4c7b-aaf0-149c458985d9",
                            CategoryId = 1,
                            Description = "Soporte móvil",
                            Name = "Phone Stand",
                            Price = 15.99m,
                            Sku = "STND035"
                        },
                        new
                        {
                            Id = "fa5035a1-c936-4d25-abec-78ae72179ee0",
                            CategoryId = 2,
                            Description = "Muñequeras",
                            Name = "Wrist Support",
                            Price = 12.99m,
                            Sku = "WRST036"
                        },
                        new
                        {
                            Id = "b58c9da5-00a7-448c-b77f-8f51b0b3453d",
                            CategoryId = 1,
                            Description = "Limpiador pantalla",
                            Name = "Screen Cleaner",
                            Price = 9.99m,
                            Sku = "CLN037"
                        },
                        new
                        {
                            Id = "fa3981dc-a09f-4521-ade0-9982d96920de",
                            CategoryId = 2,
                            Description = "Bolsa zapatos",
                            Name = "Shoe Bag Sport",
                            Price = 11.99m,
                            Sku = "BAG038"
                        },
                        new
                        {
                            Id = "3e1f8a29-c5da-4450-9bf2-29226350fff0",
                            CategoryId = 1,
                            Description = "Organizador cables",
                            Name = "Cable Manager",
                            Price = 8.99m,
                            Sku = "ORG039"
                        },
                        new
                        {
                            Id = "f12abe76-33a8-449e-98a6-fe174b0023b3",
                            CategoryId = 2,
                            Description = "Toalla pequeña",
                            Name = "Mini Towel",
                            Price = 7.99m,
                            Sku = "TWL040"
                        },
                        new
                        {
                            Id = "bde30e60-9698-4bd2-be7b-418d303c2a69",
                            CategoryId = 1,
                            Description = "Protector cable",
                            Name = "Cable Protector",
                            Price = 6.99m,
                            Sku = "PROT041"
                        },
                        new
                        {
                            Id = "ae4be2bb-184c-4e79-a848-f3f2b184f8cc",
                            CategoryId = 2,
                            Description = "Banda medición",
                            Name = "Measure Tape",
                            Price = 8.99m,
                            Sku = "MEAS042"
                        },
                        new
                        {
                            Id = "9d598117-eeab-4f28-89ad-698902f521fa",
                            CategoryId = 1,
                            Description = "Mini ventilador",
                            Name = "USB Fan",
                            Price = 12.99m,
                            Sku = "FAN043"
                        },
                        new
                        {
                            Id = "901ab5aa-1e74-416d-9e5a-11708d90eccc",
                            CategoryId = 2,
                            Description = "Candado gym",
                            Name = "Gym Lock",
                            Price = 9.99m,
                            Sku = "LOCK044"
                        },
                        new
                        {
                            Id = "9a39a021-4b4b-4235-a990-038c8c0b210c",
                            CategoryId = 1,
                            Description = "Almohadilla mouse",
                            Name = "Mouse Pad Pro",
                            Price = 11.99m,
                            Sku = "PAD045"
                        },
                        new
                        {
                            Id = "81ccf222-9334-4a01-9e29-2614e2bba73f",
                            CategoryId = 2,
                            Description = "Banda resistencia",
                            Name = "Resistance Band",
                            Price = 13.99m,
                            Sku = "BAND046"
                        },
                        new
                        {
                            Id = "dc098401-26fc-4683-9454-24e5bfe5e112",
                            CategoryId = 1,
                            Description = "Soporte tablet",
                            Name = "Tablet Mount",
                            Price = 16.99m,
                            Sku = "MNT047"
                        },
                        new
                        {
                            Id = "9dac7463-8b25-41ed-92f4-bf7c0c4d90c8",
                            CategoryId = 2,
                            Description = "Bolsa agua",
                            Name = "Water Bag",
                            Price = 15.99m,
                            Sku = "H2O048"
                        },
                        new
                        {
                            Id = "ccc169fd-ec7f-4cf0-abc4-de06303ddf6f",
                            CategoryId = 1,
                            Description = "Hub cargador",
                            Name = "Charging Hub",
                            Price = 24.99m,
                            Sku = "CHRG049"
                        },
                        new
                        {
                            Id = "065ef81d-5b54-47b5-befb-f09948de4777",
                            CategoryId = 2,
                            Description = "Timer digital",
                            Name = "Sport Timer",
                            Price = 18.99m,
                            Sku = "TIME050"
                        });
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ProductCategories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Electrónicos y accesorios tecnológicos"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Deportivos y fitness"
                        });
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.Store", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Stores");

                    b.HasData(
                        new
                        {
                            Id = "02c50015-c16e-40f6-97e0-52629ac3bdfb",
                            Address = "Address 1",
                            Name = "Store 1",
                            Phone = "123456789"
                        },
                        new
                        {
                            Id = "f5f21f1a-ce60-449c-9458-37281658f038",
                            Address = "Address 2",
                            Name = "Store 2",
                            Phone = "987654321"
                        });
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.Inventory", b =>
                {
                    b.HasOne("InventoryMan.Core.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InventoryMan.Core.Entities.Product", null)
                        .WithMany("Inventories")
                        .HasForeignKey("ProductId1");

                    b.HasOne("InventoryMan.Core.Entities.Store", "Store")
                        .WithMany("Inventories")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.Movement", b =>
                {
                    b.HasOne("InventoryMan.Core.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.Product", b =>
                {
                    b.HasOne("InventoryMan.Core.Entities.ProductCategory", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.Product", b =>
                {
                    b.Navigation("Inventories");
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.ProductCategory", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("InventoryMan.Core.Entities.Store", b =>
                {
                    b.Navigation("Inventories");
                });
#pragma warning restore 612, 618
        }
    }
}
