using InventoryMan.Core.Entities;
using Microsoft.EntityFrameworkCore;


/*
    add-migration InitialCreate
    update-database
 
 */
namespace InventoryMan.Infrastructure.Data.Context
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Test> Test { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Sku).IsRequired();
                entity.HasIndex(e => e.Sku).IsUnique();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                // Configuración de la relación con ProductCategory
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId);

            });

            // Configuración de Inventory
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.ProductId, e.StoreId }).IsUnique();
            });

            // Configuración de Movement
            modelBuilder.Entity<Movement>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId);
                entity.Property(e => e.Timestamp)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(e => e.Products)
                      .WithOne(e => e.Category)
                      .HasForeignKey(e => e.CategoryId);
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Address).IsRequired();
                entity.Property(e => e.Phone).IsRequired();
                entity.HasMany(e => e.Inventories)
                      .WithOne(e => e.Store)
                      .HasForeignKey(e => e.StoreId);
            });

            // Some data for testing
            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { Id = 1, Name = "Electrónicos y accesorios tecnológicos" },
                new ProductCategory { Id = 2, Name = "Deportivos y fitness" }
            );

            modelBuilder.Entity<Store>().HasData(
                new Store { Id = "STR-01", Name = "Store 1", Address = "Address 1", Phone = "123456789" },
                new Store { Id = "STR-02", Name = "Store 2", Address = "Address 2", Phone = "987654321" },
                new Store { Id = "STR-03", Name = "Store 3", Address = "Address 3", Phone = "985632147" }
            );

            // Database connection successfully established
            modelBuilder.Entity<Test>().HasData(
                new Test { Id = 1, Data = "Database connection successfully established" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = "ELEC-SMART-001", CategoryId = 1, Description = "Producto electrónico versátil", Name = "Smartphone X1", Price = 299.99m, Sku = "SMRT001" },
                new Product { Id = "SPRT-WATCH-001", CategoryId = 2, Description = "Accesorio deportivo premium", Name = "Reloj Deportivo Pro", Price = 149.99m, Sku = "SPRT002" },
                new Product { Id = "ELEC-AUDIO-001", CategoryId = 1, Description = "Audio de alta calidad", Name = "Auriculares Wireless", Price = 89.99m, Sku = "AUDIO003" },
                new Product { Id = "SPRT-FITNS-001", CategoryId = 2, Description = "Equipo fitness resistente", Name = "Pesas Ajustables", Price = 199.99m, Sku = "FIT004" },
                new Product { Id = "ELEC-TABL-001", CategoryId = 1, Description = "Tablet última generación", Name = "Tablet Pro 10", Price = 399.99m, Sku = "TAB005" },
                new Product { Id = "SPRT-BALL-001", CategoryId = 2, Description = "Equipamiento deportivo", Name = "Balón Profesional", Price = 29.99m, Sku = "BALL006" },
                new Product { Id = "ELEC-CAM-001", CategoryId = 1, Description = "Cámara compacta HD", Name = "Cámara Digital X20", Price = 249.99m, Sku = "CAM007" },
                new Product { Id = "SPRT-WEAR-001", CategoryId = 2, Description = "Ropa deportiva premium", Name = "Camiseta Tech-Fit", Price = 34.99m, Sku = "WEAR008" },
                new Product { Id = "ELEC-SPKR-001", CategoryId = 1, Description = "Altavoz potente", Name = "Speaker Bluetooth", Price = 79.99m, Sku = "SPK009" },
                new Product { Id = "SPRT-SHOE-001", CategoryId = 2, Description = "Calzado deportivo", Name = "Zapatillas Pro-Run", Price = 89.99m, Sku = "SHOE010" },
                new Product { Id = "ELEC-WATCH-001", CategoryId = 1, Description = "Smartwatch avanzado", Name = "Reloj Smart V2", Price = 199.99m, Sku = "WATCH011" },
                new Product { Id = "SPRT-BAND-001", CategoryId = 2, Description = "Equipo de entrenamiento", Name = "Banda Elástica Pro", Price = 19.99m, Sku = "BAND012" },
                new Product { Id = "ELEC-KEYB-001", CategoryId = 1, Description = "Teclado gaming", Name = "Teclado RGB Pro", Price = 129.99m, Sku = "KEY013" },
                new Product { Id = "SPRT-YOGA-001", CategoryId = 2, Description = "Accesorio yoga", Name = "Mat Premium", Price = 39.99m, Sku = "YOGA014" },
                new Product { Id = "ELEC-MOUS-001", CategoryId = 1, Description = "Mouse gaming", Name = "Mouse Gamer X", Price = 59.99m, Sku = "MOUSE015" },
                new Product { Id = "SPRT-BOTL-001", CategoryId = 2, Description = "Botella deportiva", Name = "Botella Térmica", Price = 24.99m, Sku = "BTL016" },
                new Product { Id = "ELEC-WCAM-001", CategoryId = 1, Description = "Webcam HD", Name = "Webcam Pro 4K", Price = 89.99m, Sku = "CAM017" },
                new Product { Id = "SPRT-PACK-001", CategoryId = 2, Description = "Mochila deportiva", Name = "Mochila Sport Pro", Price = 44.99m, Sku = "BAG018" },
                new Product { Id = "ELEC-POWR-001", CategoryId = 1, Description = "Batería portátil", Name = "PowerBank 20000", Price = 49.99m, Sku = "PWR019" },
                new Product { Id = "SPRT-GLOV-001", CategoryId = 2, Description = "Guantes fitness", Name = "Guantes Pro-Lift", Price = 29.99m, Sku = "GLV020" },
                new Product { Id = "ELEC-HUB-001", CategoryId = 1, Description = "Hub USB", Name = "USB Hub 4-Port", Price = 29.99m, Sku = "USB021" },
                new Product { Id = "SPRT-TOWL-001", CategoryId = 2, Description = "Toalla deportiva", Name = "Toalla Microfibra", Price = 14.99m, Sku = "TWL022" },
                new Product { Id = "ELEC-STND-001", CategoryId = 1, Description = "Soporte laptop", Name = "Soporte Ergonómico", Price = 39.99m, Sku = "STND023" },
                new Product { Id = "SPRT-BAG-001", CategoryId = 2, Description = "Bolsa deporte", Name = "Bolsa Gym Pro", Price = 34.99m, Sku = "GYM024" },
                new Product { Id = "ELEC-CABL-001", CategoryId = 1, Description = "Cable HDMI", Name = "HDMI Premium 2m", Price = 19.99m, Sku = "HDMI025" },
                new Product { Id = "SPRT-HRTE-001", CategoryId = 2, Description = "Banda cardio", Name = "Monitor Cardíaco", Price = 49.99m, Sku = "HRT026" },
                new Product { Id = "ELEC-CASE-001", CategoryId = 1, Description = "Funda tablet", Name = "Funda Protectora", Price = 24.99m, Sku = "CASE027" },
                new Product { Id = "SPRT-KNEE-001", CategoryId = 2, Description = "Rodilleras pro", Name = "Rodilleras Sport", Price = 19.99m, Sku = "KNEE028" },
                new Product { Id = "ELEC-WIFI-001", CategoryId = 1, Description = "Adaptador wifi", Name = "WiFi Adapter Pro", Price = 29.99m, Sku = "WIFI029" },
                new Product { Id = "SPRT-ROPE-001", CategoryId = 2, Description = "Cuerda saltar", Name = "Cuerda Pro-Jump", Price = 14.99m, Sku = "JUMP030" },
                new Product { Id = "ELEC-LED-001", CategoryId = 1, Description = "Luz LED USB", Name = "LED Light Strip", Price = 19.99m, Sku = "LED031" },
                new Product { Id = "SPRT-HEAD-001", CategoryId = 2, Description = "Banda cabeza", Name = "Banda Deportiva", Price = 9.99m, Sku = "HEAD032" },
                new Product { Id = "ELEC-SCRN-001", CategoryId = 1, Description = "Protector pantalla", Name = "Screen Guard Pro", Price = 14.99m, Sku = "SCRN033" },
                new Product { Id = "SPRT-SHKR-001", CategoryId = 2, Description = "Botella shake", Name = "Shaker Pro", Price = 16.99m, Sku = "SHAKE034" },
                new Product { Id = "ELEC-PHST-001", CategoryId = 1, Description = "Soporte móvil", Name = "Phone Stand", Price = 15.99m, Sku = "STND035" },
                new Product { Id = "SPRT-WRST-001", CategoryId = 2, Description = "Muñequeras", Name = "Wrist Support", Price = 12.99m, Sku = "WRST036" },
                new Product { Id = "ELEC-CLNR-001", CategoryId = 1, Description = "Limpiador pantalla", Name = "Screen Cleaner", Price = 9.99m, Sku = "CLN037" },
                new Product { Id = "SPRT-SBAG-001", CategoryId = 2, Description = "Bolsa zapatos", Name = "Shoe Bag Sport", Price = 11.99m, Sku = "BAG038" },
                new Product { Id = "ELEC-ORG-001", CategoryId = 1, Description = "Organizador cables", Name = "Cable Manager", Price = 8.99m, Sku = "ORG039" },
                new Product { Id = "SPRT-MTOW-001", CategoryId = 2, Description = "Toalla pequeña", Name = "Mini Towel", Price = 7.99m, Sku = "TWL040" },
                new Product { Id = "ELEC-PROT-001", CategoryId = 1, Description = "Protector cable", Name = "Cable Protector", Price = 6.99m, Sku = "PROT041" },
                new Product { Id = "SPRT-MEAS-001", CategoryId = 2, Description = "Banda medición", Name = "Measure Tape", Price = 8.99m, Sku = "MEAS042" },
                new Product { Id = "ELEC-FAN-001", CategoryId = 1, Description = "Mini ventilador", Name = "USB Fan", Price = 12.99m, Sku = "FAN043" },
                new Product { Id = "SPRT-LOCK-001", CategoryId = 2, Description = "Candado gym", Name = "Gym Lock", Price = 9.99m, Sku = "LOCK044" },
                new Product { Id = "ELEC-PAD-001", CategoryId = 1, Description = "Almohadilla mouse", Name = "Mouse Pad Pro", Price = 11.99m, Sku = "PAD045" },
                new Product { Id = "SPRT-RBAN-001", CategoryId = 2, Description = "Banda resistencia", Name = "Resistance Band", Price = 13.99m, Sku = "BAND046" },
                new Product { Id = "ELEC-MNT-001", CategoryId = 1, Description = "Soporte tablet", Name = "Tablet Mount", Price = 16.99m, Sku = "MNT047" },
                new Product { Id = "SPRT-H2O-001", CategoryId = 2, Description = "Bolsa agua", Name = "Water Bag", Price = 15.99m, Sku = "H2O048" },
                new Product { Id = "ELEC-CHRG-001", CategoryId = 1, Description = "Hub cargador", Name = "Charging Hub", Price = 24.99m, Sku = "CHRG049" },
                new Product { Id = "SPRT-TIME-001", CategoryId = 2, Description = "Timer digital", Name = "Sport Timer", Price = 19.99m, Sku = "TIME050" }
            );
        }
    }

}
