using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DelmondesCodeLabs_GestaoComercial.Data
{
    #region Database Context
    /// <summary>
    /// Represents the primary database context for the application, handling Identity and domain entities.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="httpContextAccessor"></param>
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : IdentityDbContext<ApplicationUser>(options)
    {
        #region DbSets
        /// <summary>
        /// Gets or sets the product categories in the database.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Gets or sets the catalog products in the database.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets the product compositions (Bill of Materials) in the database.
        /// </summary>
        public DbSet<ProductComposition> ProductCompositions { get; set; }

        /// <summary>
        /// Gets or sets the suppliers in the database.
        /// </summary>
        public DbSet<Supplier> Suppliers { get; set; }

        /// <summary>
        /// Gets or sets the stock entry invoices in the database.
        /// </summary>
        public DbSet<StockEntry> StockEntries { get; set; }

        /// <summary>
        /// Gets or sets the individual items within stock entries in the database.
        /// </summary>
        public DbSet<StockEntryItem> StockEntryItems { get; set; }

        /// <summary>
        /// Gets or sets the immutable stock movement logs in the database.
        /// </summary>
        public DbSet<StockMovement> StockMovements { get; set; }

        /// <summary>
        /// Gets or sets the operational expenses in the database.
        /// </summary>
        public DbSet<Expense> Expenses { get; set; }

        /// <summary>
        /// Gets or sets the discount promotions in the database.
        /// </summary>
        public DbSet<Promotion> Promotions { get; set; }

        /// <summary>
        /// Gets or sets the individual items linked to promotions in the database.
        /// </summary>
        public DbSet<PromotionItem> PromotionItems { get; set; }

        /// <summary>
        /// Gets or sets the sales transactions in the database.
        /// </summary>
        public DbSet<Sale> Sales { get; set; }

        /// <summary>
        /// Gets or sets the individual items within a sale transaction in the database.
        /// </summary>
        public DbSet<SaleItem> SaleItems { get; set; }

        #endregion DbSets

        #region Model Configuration
        /// <summary>
        /// Configures entity mapppings and database schema relationships.
        /// </summary>
        /// <param name="builder">The ModelBuilder instance.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresExtension("unaccent");

            #region Category Entity Configuration

            builder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(c => c.Description)
                      .HasMaxLength(500);
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            #endregion Category Entity Configuration

            #region Product Configuration
            builder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.Id);

                // Unique indexes to prevent fatal duplication errors in the POS
                entity.HasIndex(p => p.Sku).IsUnique();
                entity.HasIndex(p => p.Barcode).IsUnique();

                entity.Property(p => p.Sku).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Barcode).HasMaxLength(50);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);

                // Strict financial precision mappings
                entity.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.SellingPrice).HasColumnType("decimal(18,2)");

                // 3 decimal places for stock allows fractional sales (e.g., 0.500 kg of snacks) if needed in the future
                entity.Property(p => p.CurrentStock).HasColumnType("decimal(18,3)");

                entity.Property(p => p.MinimumStockAlert).HasColumnType("decimal(18,3)");

                // Concurrency configuration
                entity.Property(p => p.Version).IsConcurrencyToken();

                // Relationship mapping to prevent deleting a category that has active products
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Global query filter for soft deletes
                entity.HasQueryFilter(p => !p.IsDeleted);
            });
            #endregion Product Configuration

            #region Product Composition Configuration
            builder.Entity<ProductComposition>(entity =>
            {
                entity.ToTable("ProductCompositions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");

                entity.HasOne(e => e.ParentProduct)
                      .WithMany()
                      .HasForeignKey(e => e.ParentProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ChildProduct)
                      .WithMany()
                      .HasForeignKey(e => e.ChildProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Product Composition Configuration

            #region Supplier Configuration
            builder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Cnpj).HasMaxLength(18);
                entity.Property(e => e.ContactEmail).HasMaxLength(100);
                entity.Property(e => e.ContactPhone).HasMaxLength(20);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Supplier Configuration

            #region Stock Entry Configuration
            builder.Entity<StockEntry>(entity =>
            {
                entity.ToTable("StockEntries");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNumber).HasMaxLength(50);
                entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Supplier)
                      .WithMany(s => s.StockEntries)
                      .HasForeignKey(e => e.SupplierId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Stock Entry Configuration

            #region Stock Entry Item Configuration
            builder.Entity<StockEntryItem>(entity =>
            {
                entity.ToTable("StockEntryItems");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.StockEntry)
                      .WithMany(se => se.Items)
                      .HasForeignKey(e => e.StockEntryId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Stock Entry Item Configuration

            #region Stock Movement Configuration
            builder.Entity<StockMovement>(entity =>
            {
                entity.ToTable("StockMovements");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");
                entity.Property(e => e.Reason).HasMaxLength(255);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Stock Movement Configuration

            #region Expense Configuration
            builder.Entity<Expense>(entity =>
            {
                entity.ToTable("Expenses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(250);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Version).IsConcurrencyToken();
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Expense Configuration

            #region Promotion Configuration
            builder.Entity<Promotion>(entity =>
            {
                entity.ToTable("Promotions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.DiscountValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Version).IsConcurrencyToken();
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Promotion Configuration

            #region Promotion Item Configuration
            builder.Entity<PromotionItem>(entity =>
            {
                entity.ToTable("PromotionItems");
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Promotion)
                      .WithMany(p => p.Items)
                      .HasForeignKey(e => e.PromotionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Promotion Item Configuration

            #region Sale Configuration
            builder.Entity<Sale>(entity =>
            {
                entity.ToTable("Sales");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReceiptNumber).IsRequired().HasMaxLength(50);

                // Add mapping for CustomerIdentifier (Open Tab identification)
                entity.Property(e => e.CustomerIdentifier).IsRequired().HasMaxLength(150);

                // Strict financial precision mappings
                entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Version).IsConcurrencyToken();
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Sale Configuration

            #region Sale Item Configuration
            builder.Entity<SaleItem>(entity =>
            {
                entity.ToTable("SaleItems");
                entity.HasKey(e => e.Id);

                // 3 decimal places for stock allows fractional sales
                entity.Property(e => e.Quantity).HasColumnType("decimal(18,3)");

                // Strict financial precision mappings
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Sale)
                      .WithMany(s => s.Items)
                      .HasForeignKey(e => e.SaleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });
            #endregion Sale Item Configuration
        }
        #endregion Model Configuration

        #region Save Changes Interception
        /// <summary>
        /// Overrides the default save behaviour to automatically populate audit fields and enforce soft deletion.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            string currentUser = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
            DateTimeOffset now = DateTimeOffset.UtcNow;

            foreach (EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.CreatedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = currentUser;

                        // Updates the concurrency token to force validation against concurrent edits
                        entry.Entity.Version = Guid.NewGuid();
                        break;

                    case EntityState.Deleted:
                        // Intercepts hard deletes and transforms them into soft deletes seamlessly
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.UpdatedAt = now;
                        entry.Entity.UpdatedBy = currentUser;
                        entry.Entity.Version = Guid.NewGuid();
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
        #endregion Save Changes Interception
    }
    #endregion Database Context
}