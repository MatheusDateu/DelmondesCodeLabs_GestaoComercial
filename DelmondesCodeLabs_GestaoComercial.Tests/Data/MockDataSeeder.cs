#region Usings
using Bogus;
using Bogus.Extensions.Brazil;
using DelmondesCodeLabs_GestaoComercial.Data;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Tests.Data
{
    #region Mock Data Seeder
    /// <summary>
    /// Generates deterministic mock data using Bogus for load testing and QA.
    /// </summary>
    public class MockDataSeeder
    {
        #region Fields
        private readonly ApplicationDbContext _dbContext;
        private readonly string _systemUser;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the MockDataSeeder.
        /// </summary>
        public MockDataSeeder(ApplicationDbContext dbContext, string systemUser = "System QA")
        {
            _dbContext = dbContext;
            _systemUser = systemUser;

            // Ensures that the generated data is identical every time the test runs
            Randomizer.Seed = new Random(2026);
        }
        #endregion Constructors

        #region Tier 1 - Core Foundations
        /// <summary>
        /// Generates and saves Categories and Suppliers to the database.
        /// </summary>
        public async Task<List<Category>> SeedTier1CategoriesAsync(int count)
        {
            Faker<Category> categoryFaker = new Faker<Category>("pt_BR")
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.Description, f => f.Commerce.ProductDescription())
                .RuleFor(c => c.IsActive, f => true)
                // Forçando UTC para contornar a restrição do PostgreSQL (Npgsql)
                .RuleFor(c => c.CreatedAt, f => f.Date.PastOffset(2).ToUniversalTime())
                .RuleFor(c => c.CreatedBy, _systemUser);

            List<Category> categories = categoryFaker.Generate(count);
            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            return categories;
        }

        /// <summary>
        /// Generates and saves Suppliers to the database.
        /// </summary>
        public async Task<List<Supplier>> SeedTier1SuppliersAsync(int count)
        {
            Faker<Supplier> supplierFaker = new Faker<Supplier>("pt_BR")
                .RuleFor(s => s.Name, f => f.Company.CompanyName())
                .RuleFor(s => s.Cnpj, f => f.Company.Cnpj())
                .RuleFor(s => s.ContactEmail, f => f.Internet.Email())
                .RuleFor(s => s.ContactPhone, f => f.Phone.PhoneNumber())
                // Forçando UTC
                .RuleFor(s => s.CreatedAt, f => f.Date.PastOffset(2).ToUniversalTime())
                .RuleFor(s => s.CreatedBy, _systemUser);

            List<Supplier> suppliers = supplierFaker.Generate(count);
            await _dbContext.Suppliers.AddRangeAsync(suppliers);
            await _dbContext.SaveChangesAsync();

            return suppliers;
        }
        #endregion Tier 1 - Core Foundations

        #region Tier 2 - Catalog
        /// <summary>
        /// Generates and saves Products linked to existing Categories.
        /// </summary>
        public async Task<List<Product>> SeedTier2ProductsAsync(int count, List<Category> categories)
        {
            Faker<Product> productFaker = new Faker<Product>("pt_BR")
                .RuleFor(p => p.CategoryId, f => f.PickRandom(categories).Id)
                .RuleFor(p => p.Sku, f => f.Commerce.Ean13())
                .RuleFor(p => p.Barcode, f => f.Commerce.Ean8())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductAdjective())
                .RuleFor(p => p.Type, f => f.PickRandom<ProductType>())
                .RuleFor(p => p.CostPrice, f => f.Random.Decimal(5m, 100m))
                .RuleFor(p => p.SellingPrice, (f, p) => p.CostPrice * f.Random.Decimal(1.3m, 2.5m))
                .RuleFor(p => p.MinimumStockAlert, f => f.Random.Decimal(5m, 20m))
                .RuleFor(p => p.CurrentStock, 0m)
                .RuleFor(p => p.IsActive, f => true)
                // Forçando UTC
                .RuleFor(p => p.CreatedAt, f => f.Date.PastOffset(1).ToUniversalTime())
                .RuleFor(p => p.CreatedBy, _systemUser);

            List<Product> products = productFaker.Generate(count);
            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();

            return products;
        }
        #endregion Tier 2 - Catalog

        #region Tier 3 - Inventory Engine
        /// <summary>
        /// Generates and saves Stock Entries (Invoices) alongside their respective Items and immutable Stock Movements.
        /// </summary>
        public async Task SeedTier3InventoryAsync(int entryCount, List<Supplier> suppliers, List<Product> products)
        {
            Faker<StockEntry> entryFaker = new Faker<StockEntry>("pt_BR")
                .RuleFor(e => e.SupplierId, f => f.PickRandom(suppliers).Id)
                .RuleFor(e => e.InvoiceNumber, f => f.Random.AlphaNumeric(10).ToUpper())
                // Forçando UTC
                .RuleFor(e => e.EntryDate, f => f.Date.PastOffset(1).ToUniversalTime())
                .RuleFor(e => e.CreatedAt, (f, e) => e.EntryDate)
                .RuleFor(e => e.CreatedBy, _systemUser);

            List<StockEntry> entries = entryFaker.Generate(entryCount);

            Faker itemFaker = new Faker("pt_BR");
            List<StockMovement> movements = new();

            foreach (StockEntry entry in entries)
            {
                int itemCount = itemFaker.Random.Int(1, 5); // 1 to 5 items per invoice
                entry.Items = new List<StockEntryItem>();
                decimal totalCost = 0;

                for (int i = 0; i < itemCount; i++)
                {
                    Product randomProduct = itemFaker.PickRandom(products);
                    decimal quantity = itemFaker.Random.Decimal(10m, 200m);
                    decimal unitCost = randomProduct.CostPrice; // Inherits the product's base cost

                    StockEntryItem item = new StockEntryItem
                    {
                        ProductId = randomProduct.Id,
                        Quantity = quantity,
                        UnitCost = unitCost,
                        CreatedAt = entry.EntryDate,
                        CreatedBy = _systemUser
                    };

                    entry.Items.Add(item);
                    totalCost += (quantity * unitCost);

                    // Creates the corresponding immutable stock movement
                    movements.Add(new StockMovement
                    {
                        ProductId = randomProduct.Id,
                        Quantity = quantity,
                        Type = MovementType.Entry,
                        Reason = $"Initial mock entry via Invoice {entry.InvoiceNumber}",
                        MovementDate = entry.EntryDate,
                        CreatedAt = entry.EntryDate,
                        CreatedBy = _systemUser
                    });

                    // Updates the cached CurrentStock projection
                    randomProduct.CurrentStock += quantity;
                }

                entry.TotalCost = totalCost;
            }

            await _dbContext.StockEntries.AddRangeAsync(entries);
            await _dbContext.StockMovements.AddRangeAsync(movements);

            // Saves the updated stock quantities to the database
            _dbContext.Products.UpdateRange(products);
            await _dbContext.SaveChangesAsync();
        }
        #endregion Tier 3 - Inventory Engine

        #region Tier 4 - Financial Engine
        /// <summary>
        /// Generates and saves historical administrative and operational Expenses.
        /// </summary>
        public async Task SeedTier4ExpensesAsync(int count)
        {
            Faker<Expense> expenseFaker = new Faker<Expense>("pt_BR")
                .RuleFor(e => e.Description, f => f.Finance.AccountName() + " (Mock Expense)")
                .RuleFor(e => e.Amount, f => f.Random.Decimal(50m, 5000m))
                .RuleFor(e => e.Category, f => f.PickRandom<ExpenseCategory>())
                // Forçando UTC
                .RuleFor(e => e.DueDate, f => f.Date.PastOffset(1).ToUniversalTime())
                .RuleFor(e => e.IsPaid, f => f.Random.Bool(0.8f)) // 80% chance of being already paid
                .RuleFor(e => e.PaymentDate, (f, e) => e.IsPaid ? e.DueDate.AddDays(f.Random.Int(-2, 5)) : null)
                .RuleFor(e => e.CreatedAt, (f, e) => e.DueDate.AddDays(-15))
                .RuleFor(e => e.CreatedBy, _systemUser);

            List<Expense> expenses = expenseFaker.Generate(count);
            await _dbContext.Expenses.AddRangeAsync(expenses);
            await _dbContext.SaveChangesAsync();
        }
        #endregion Tier 4 - Financial Engine

        #region Tier 5 - Pricing & Promotions
        /// <summary>
        /// Generates and saves active and expired discount Promotions.
        /// </summary>
        public async Task<List<Promotion>> SeedTier5PromotionsAsync(int count, List<Product> products)
        {
            Faker<Promotion> promoFaker = new Faker<Promotion>("pt_BR")
                .RuleFor(p => p.Name, f => f.Commerce.Department() + " " + f.Commerce.ProductAdjective() + " Promo")
                // Forçando UTC
                .RuleFor(p => p.StartDate, f => f.Date.PastOffset(1).ToUniversalTime())
                .RuleFor(p => p.EndDate, (f, p) => p.StartDate.AddDays(f.Random.Int(5, 30)))
                .RuleFor(p => p.DiscountType, f => f.PickRandom<DiscountType>())
                .RuleFor(p => p.DiscountValue, (f, p) => p.DiscountType == DiscountType.Percentage ? f.Random.Decimal(5m, 30m) : f.Random.Decimal(2m, 15m))
                .RuleFor(p => p.IsActive, f => f.Random.Bool(0.7f)) // 70% active
                .RuleFor(p => p.CreatedAt, (f, p) => p.StartDate.AddDays(-2))
                .RuleFor(p => p.CreatedBy, _systemUser);

            List<Promotion> promotions = promoFaker.Generate(count);

            Faker itemFaker = new Faker("pt_BR");
            foreach (Promotion promo in promotions)
            {
                promo.Items = new List<PromotionItem>();
                int itemCount = itemFaker.Random.Int(1, 4);

                for (int i = 0; i < itemCount; i++)
                {
                    Product randomProduct = itemFaker.PickRandom(products);
                    promo.Items.Add(new PromotionItem
                    {
                        PromotionId = promo.Id,
                        ProductId = randomProduct.Id,
                        CreatedAt = promo.CreatedAt,
                        CreatedBy = _systemUser
                    });
                }
            }

            await _dbContext.Promotions.AddRangeAsync(promotions);
            await _dbContext.SaveChangesAsync();

            return promotions;
        }
        #endregion Tier 5 - Pricing & Promotions

        #region Tier 6 - Point of Sale Engine
        /// <summary>
        /// Generates and saves historical Sales, Sale Items, and their corresponding Stock Movements enforcing business rules.
        /// </summary>
        public async Task SeedTier6SalesAsync(int saleCount, List<Product> products)
        {
            Faker<Sale> saleFaker = new Faker<Sale>("pt_BR")
                .RuleFor(s => s.ReceiptNumber, f => f.Random.AlphaNumeric(12).ToUpper())
                .RuleFor(s => s.CustomerIdentifier, f => f.Random.Bool(0.3f) ? f.Person.FullName : "Mesa " + f.Random.Int(1, 20))
                // Forçando UTC
                .RuleFor(s => s.SaleDate, f => f.Date.PastOffset(1).ToUniversalTime())
                .RuleFor(s => s.Status, f => f.PickRandom<SaleStatus>())
                // Enforces that Pending sales have Pending payment method
                .RuleFor(s => s.PaymentMethod, (f, s) => s.Status == SaleStatus.Pending ? PaymentMethod.Pending : f.PickRandom<PaymentMethod>())
                .RuleFor(s => s.CreatedAt, (f, s) => s.SaleDate)
                .RuleFor(s => s.CreatedBy, _systemUser);

            List<Sale> sales = saleFaker.Generate(saleCount);
            Faker itemFaker = new Faker("pt_BR");
            List<StockMovement> movements = new();

            foreach (Sale sale in sales)
            {
                int itemCount = itemFaker.Random.Int(1, 8);
                sale.Items = new List<SaleItem>();
                decimal subTotal = 0;
                decimal discountTotal = 0;

                for (int i = 0; i < itemCount; i++)
                {
                    Product randomProduct = itemFaker.PickRandom(products);
                    decimal quantity = itemFaker.Random.Decimal(1m, 5m);
                    decimal unitPrice = randomProduct.SellingPrice;

                    // 10% chance of applying a manual discount on the item
                    decimal discount = itemFaker.Random.Bool(0.1f) ? (unitPrice * 0.1m * quantity) : 0m;
                    decimal total = (unitPrice * quantity) - discount;

                    SaleItemStatus itemStatus = SaleItemStatus.Active;
                    if (sale.Status == SaleStatus.Cancelled)
                    {
                        itemStatus = SaleItemStatus.Canceled;
                    }
                    else if (itemFaker.Random.Bool(0.05f))
                    {
                        // 5% chance of individual item cancellation or damage within an active sale
                        itemStatus = itemFaker.PickRandom(SaleItemStatus.Canceled, SaleItemStatus.Damaged);
                    }

                    SaleItem saleItem = new SaleItem
                    {
                        ProductId = randomProduct.Id,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        DiscountAmount = discount,
                        Total = total,
                        Status = itemStatus,
                        CreatedAt = sale.SaleDate,
                        CreatedBy = _systemUser
                    };

                    sale.Items.Add(saleItem);

                    // Only active items compute towards the final receipt totals
                    if (itemStatus == SaleItemStatus.Active)
                    {
                        subTotal += (unitPrice * quantity);
                        discountTotal += discount;
                    }

                    MovementType moveType = MovementType.Sale;
                    string moveReason = $"Sale deduction via Receipt {sale.ReceiptNumber}";
                    decimal moveMultiplier = -1m; // Deducts stock

                    if (itemStatus == SaleItemStatus.Canceled)
                    {
                        moveType = MovementType.Entry;
                        moveReason = $"Sale cancellation return via Receipt {sale.ReceiptNumber}";
                        moveMultiplier = 1m; // Returns stock
                    }
                    else if (itemStatus == SaleItemStatus.Damaged)
                    {
                        moveType = MovementType.Loss;
                        moveReason = $"Item damaged during Sale via Receipt {sale.ReceiptNumber}";
                        moveMultiplier = -1m; // Deducts stock but categorises as Loss
                    }

                    movements.Add(new StockMovement
                    {
                        ProductId = randomProduct.Id,
                        Quantity = quantity,
                        Type = moveType,
                        Reason = moveReason,
                        MovementDate = sale.SaleDate,
                        CreatedAt = sale.SaleDate,
                        CreatedBy = _systemUser
                    });

                    // Update cached product stock based on the multiplier mapping
                    randomProduct.CurrentStock += (quantity * moveMultiplier);
                }

                sale.SubTotal = subTotal;
                sale.DiscountAmount = discountTotal;
                sale.TotalAmount = subTotal - discountTotal;
            }

            await _dbContext.Sales.AddRangeAsync(sales);
            await _dbContext.StockMovements.AddRangeAsync(movements);

            _dbContext.Products.UpdateRange(products);
            await _dbContext.SaveChangesAsync();
        }
        #endregion Tier 6 - Point of Sale Engine
    }
    #endregion Mock Data Seeder
}