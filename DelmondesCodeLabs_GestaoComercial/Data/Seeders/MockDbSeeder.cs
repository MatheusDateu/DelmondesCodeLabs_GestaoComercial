#region Usings
using Bogus;
using Bogus.Extensions.Brazil;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Data.Seeders
{
    #region Mock Database Seeder
    /// <summary>
    /// Generates tailored, realistic mock data for an Adega environment to ensure maximum impact during client presentations.
    /// </summary>
    public static class MockDbSeeder
    {
        #region Methods
        /// <summary>
        /// Orchestrates the generation of all Tiers of mock data if the database is currently empty.
        /// </summary>
        public static async Task SeedDevelopmentDataAsync(ApplicationDbContext dbContext)
        {
            if (await dbContext.Categories.AnyAsync())
            {
                return;
            }

            string systemUser = "System QA";
            Randomizer.Seed = new Random(2026);

            List<Category> categories = await SeedCategoriesAsync(dbContext, systemUser);
            List<Supplier> suppliers = await SeedSuppliersAsync(dbContext, systemUser);

            List<Product> products = await SeedProductsAsync(dbContext, systemUser, categories);

            await SeedCompositionsAsync(dbContext, systemUser, products);

            // Increased stock entries to support the massive sales volume
            await SeedInventoryAsync(dbContext, systemUser, 150, suppliers, products);

            await SeedExpensesRealisticAsync(dbContext, systemUser);

            await SeedPromotionsAsync(dbContext, systemUser, products);

            // Increased sales to 15,000 to guarantee a highly profitable scenario
            await SeedSalesAsync(dbContext, systemUser, 15000, products);
        }

        private static async Task SeedExpensesRealisticAsync(ApplicationDbContext dbContext, string user)
        {
            List<Expense> expenses = new List<Expense>();
            DateTimeOffset now = DateTimeOffset.UtcNow;
            Faker faker = new Faker("pt_BR");

            // Generates exactly 1 set of fixed bills per month for the last 12 months
            for (int i = 0; i <= 12; i++)
            {
                DateTimeOffset monthDate = now.AddMonths(-i);

                expenses.Add(new Expense { Description = "Aluguel do Ponto", Amount = 2500m, Category = ExpenseCategory.Rent, DueDate = monthDate, IsPaid = true, PaymentDate = monthDate.AddDays(-1), CreatedAt = monthDate.AddDays(-15), CreatedBy = user });
                expenses.Add(new Expense { Description = "Conta de Luz (Equatorial)", Amount = 850m, Category = ExpenseCategory.Utilities, DueDate = monthDate, IsPaid = true, PaymentDate = monthDate.AddDays(-2), CreatedAt = monthDate.AddDays(-10), CreatedBy = user });
                expenses.Add(new Expense { Description = "Conta de Água (Caer)", Amount = 150m, Category = ExpenseCategory.Utilities, DueDate = monthDate, IsPaid = true, PaymentDate = monthDate.AddDays(-2), CreatedAt = monthDate.AddDays(-10), CreatedBy = user });
                expenses.Add(new Expense { Description = "Internet Fibra", Amount = 120m, Category = ExpenseCategory.Operational, DueDate = monthDate, IsPaid = true, PaymentDate = monthDate.AddDays(-5), CreatedAt = monthDate.AddDays(-10), CreatedBy = user });

                // Add some random operational micro-expenses per month
                int randomExpenses = faker.Random.Int(2, 5);
                for (int j = 0; j < randomExpenses; j++)
                {
                    expenses.Add(new Expense { Description = "Manutenção/Material Limpeza", Amount = faker.Random.Decimal(50m, 200m), Category = ExpenseCategory.Operational, DueDate = monthDate.AddDays(faker.Random.Int(1, 20)), IsPaid = true, PaymentDate = monthDate, CreatedAt = monthDate, CreatedBy = user });
                }
            }

            await dbContext.Expenses.AddRangeAsync(expenses);
            await dbContext.SaveChangesAsync();
        }
        private static async Task<List<Category>> SeedCategoriesAsync(ApplicationDbContext dbContext, string user)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            List<Category> categories = new List<Category>
            {
                new Category { Name = "Copões", Description = "Bebidas preparadas no copão", IsActive = true, CreatedAt = now, CreatedBy = user },
                new Category { Name = "Caipirinhas", Description = "Caipirinhas de diversos tamanhos", IsActive = true, CreatedAt = now, CreatedBy = user },
                new Category { Name = "Narguilé", Description = "Sessões e insumos para narguilé", IsActive = true, CreatedAt = now, CreatedBy = user },
                new Category { Name = "Entretenimento", Description = "Fichas de sinuca e jogos", IsActive = true, CreatedAt = now, CreatedBy = user },
                new Category { Name = "Conveniência", Description = "Balas, salgadinhos e cigarros", IsActive = true, CreatedAt = now, CreatedBy = user }
            };

            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.SaveChangesAsync();
            return categories;
        }

        private static async Task<List<Supplier>> SeedSuppliersAsync(ApplicationDbContext dbContext, string user)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            List<Supplier> suppliers = new List<Supplier>
            {
                new Supplier { Name = "Ambev Distribuidora", Cnpj = "07.526.557/0001-00", ContactEmail = "vendas@ambev.com.br", ContactPhone = "0800 888 1234", CreatedAt = now, CreatedBy = user },
                new Supplier { Name = "Souza Cruz", Cnpj = "33.009.911/0001-39", ContactEmail = "comercial@souzacruz.com.br", ContactPhone = "0800 777 5555", CreatedAt = now, CreatedBy = user },
                new Supplier { Name = "Zomo Hookah Brasil", Cnpj = "12.345.678/0001-99", ContactEmail = "pedidos@zomo.com.br", ContactPhone = "11 99999-8888", CreatedAt = now, CreatedBy = user }
            };

            await dbContext.Suppliers.AddRangeAsync(suppliers);
            await dbContext.SaveChangesAsync();
            return suppliers;
        }

        private static async Task<List<Product>> SeedProductsAsync(ApplicationDbContext dbContext, string user, List<Category> categories)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            Guid catCopoes = categories.First(c => c.Name == "Copões").Id;
            Guid catCaipi = categories.First(c => c.Name == "Caipirinhas").Id;
            Guid catNarguile = categories.First(c => c.Name == "Narguilé").Id;
            Guid catJogos = categories.First(c => c.Name == "Entretenimento").Id;
            Guid catConv = categories.First(c => c.Name == "Conveniência").Id;

            List<Product> products = new List<Product>
            {
                new Product { CategoryId = catCopoes, Sku = "COP-VOD-01", Name = "Copão de Vodka", Type = ProductType.Standard, CostPrice = 5m, SellingPrice = 15m, MinimumStockAlert = 50m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catCopoes, Sku = "COP-GIN-01", Name = "Copão de Gin", Type = ProductType.Standard, CostPrice = 7m, SellingPrice = 20m, MinimumStockAlert = 50m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catCopoes, Sku = "COP-WHI-01", Name = "Copão Uísque Padrão", Type = ProductType.Standard, CostPrice = 10m, SellingPrice = 25m, MinimumStockAlert = 30m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catCopoes, Sku = "COP-WHI-JD", Name = "Copão Jack Daniels", Type = ProductType.Standard, CostPrice = 14m, SellingPrice = 30m, MinimumStockAlert = 20m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catCopoes, Sku = "COP-WHI-OP", Name = "Copão Old Parr", Type = ProductType.Standard, CostPrice = 16m, SellingPrice = 35m, MinimumStockAlert = 20m, IsActive = true, CreatedAt = now, CreatedBy = user },

                new Product { CategoryId = catCaipi, Sku = "CAI-300-01", Name = "Caipirinha 300ml", Type = ProductType.Standard, CostPrice = 4m, SellingPrice = 12m, MinimumStockAlert = 30m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catCaipi, Sku = "CAI-400-01", Name = "Caipirinha 400ml", Type = ProductType.Standard, CostPrice = 5.5m, SellingPrice = 14m, MinimumStockAlert = 30m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catCaipi, Sku = "CAI-1LT-01", Name = "Caipirinha 1 Litro", Type = ProductType.Standard, CostPrice = 10m, SellingPrice = 25m, MinimumStockAlert = 15m, IsActive = true, CreatedAt = now, CreatedBy = user },

                new Product { CategoryId = catNarguile, Sku = "NAR-SES-01", Name = "Sessão de Narguilé", Type = ProductType.Service, CostPrice = 0m, SellingPrice = 25m, MinimumStockAlert = 0m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catNarguile, Sku = "NAR-ESS-01", Name = "Essência Zomo 50g", Type = ProductType.Component, CostPrice = 4.5m, SellingPrice = 0m, MinimumStockAlert = 100m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catNarguile, Sku = "NAR-CAR-01", Name = "Carvão de Coco (Unidade)", Type = ProductType.Component, CostPrice = 0.5m, SellingPrice = 0m, MinimumStockAlert = 500m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catNarguile, Sku = "NAR-ALU-01", Name = "Folha de Alumínio", Type = ProductType.Component, CostPrice = 0.2m, SellingPrice = 0m, MinimumStockAlert = 500m, IsActive = true, CreatedAt = now, CreatedBy = user },

                new Product { CategoryId = catJogos, Sku = "JOG-SIN-01", Name = "Ficha de Sinuca", Type = ProductType.Service, CostPrice = 0m, SellingPrice = 5m, MinimumStockAlert = 0m, IsActive = true, CreatedAt = now, CreatedBy = user },

                new Product { CategoryId = catConv, Sku = "CON-HAL-01", Name = "Bala Halls", Type = ProductType.Standard, CostPrice = 1m, SellingPrice = 2m, MinimumStockAlert = 50m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catConv, Sku = "CON-FRE-01", Name = "Bala Freegels", Type = ProductType.Standard, CostPrice = 0.7m, SellingPrice = 1.5m, MinimumStockAlert = 50m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catConv, Sku = "CON-SAL-01", Name = "Salgadinho Elma Chips", Type = ProductType.Standard, CostPrice = 4m, SellingPrice = 8m, MinimumStockAlert = 20m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Product { CategoryId = catConv, Sku = "CON-CIG-01", Name = "Cigarro (Maço)", Type = ProductType.Standard, CostPrice = 8m, SellingPrice = 12m, MinimumStockAlert = 30m, IsActive = true, CreatedAt = now, CreatedBy = user }
            };

            await dbContext.Products.AddRangeAsync(products);
            await dbContext.SaveChangesAsync();
            return products;
        }

        private static async Task SeedCompositionsAsync(ApplicationDbContext dbContext, string user, List<Product> products)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            Product session = products.First(p => p.Sku == "NAR-SES-01");
            Product essence = products.First(p => p.Sku == "NAR-ESS-01");
            Product coal = products.First(p => p.Sku == "NAR-CAR-01");
            Product foil = products.First(p => p.Sku == "NAR-ALU-01");

            List<ProductComposition> boms = new List<ProductComposition>
            {
                new ProductComposition { ParentProductId = session.Id, ChildProductId = essence.Id, Quantity = 1m, CreatedAt = now, CreatedBy = user },
                new ProductComposition { ParentProductId = session.Id, ChildProductId = coal.Id, Quantity = 3m, CreatedAt = now, CreatedBy = user },
                new ProductComposition { ParentProductId = session.Id, ChildProductId = foil.Id, Quantity = 1m, CreatedAt = now, CreatedBy = user }
            };

            await dbContext.ProductCompositions.AddRangeAsync(boms);
            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedInventoryAsync(ApplicationDbContext dbContext, string user, int entryCount, List<Supplier> suppliers, List<Product> products)
        {
            Faker<StockEntry> entryFaker = new Faker<StockEntry>("pt_BR")
                .RuleFor(e => e.SupplierId, f => f.PickRandom(suppliers).Id)
                .RuleFor(e => e.InvoiceNumber, f => f.Random.AlphaNumeric(8).ToUpper())
                .RuleFor(e => e.EntryDate, f => f.Date.PastOffset(1).ToUniversalTime())
                .RuleFor(e => e.CreatedAt, (f, e) => e.EntryDate)
                .RuleFor(e => e.CreatedBy, user);

            List<StockEntry> entries = entryFaker.Generate(entryCount);
            Faker itemFaker = new Faker("pt_BR");
            List<StockMovement> movements = new();

            List<Product> storableProducts = products.Where(p => p.Type != ProductType.Service).ToList();

            foreach (StockEntry entry in entries)
            {
                int itemCount = itemFaker.Random.Int(3, 8);
                entry.Items = new List<StockEntryItem>();
                decimal totalCost = 0;

                for (int i = 0; i < itemCount; i++)
                {
                    Product randomProduct = itemFaker.PickRandom(storableProducts);
                    decimal quantity = itemFaker.Random.Decimal(50m, 500m);
                    decimal unitCost = randomProduct.CostPrice;

                    entry.Items.Add(new StockEntryItem
                    {
                        ProductId = randomProduct.Id,
                        Quantity = quantity,
                        UnitCost = unitCost,
                        CreatedAt = entry.EntryDate,
                        CreatedBy = user
                    });

                    totalCost += (quantity * unitCost);

                    movements.Add(new StockMovement
                    {
                        ProductId = randomProduct.Id,
                        Quantity = quantity,
                        Type = MovementType.Entry,
                        Reason = $"Reposição de Estoque via NFe {entry.InvoiceNumber}",
                        MovementDate = entry.EntryDate,
                        CreatedAt = entry.EntryDate,
                        CreatedBy = user
                    });

                    randomProduct.CurrentStock += quantity;
                }
                entry.TotalCost = totalCost;
            }

            await dbContext.StockEntries.AddRangeAsync(entries);
            await dbContext.StockMovements.AddRangeAsync(movements);
            dbContext.Products.UpdateRange(products);
            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedPromotionsAsync(ApplicationDbContext dbContext, string user, List<Product> products)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            Product hookah = products.First(p => p.Sku == "NAR-SES-01");
            Product caipi300 = products.First(p => p.Sku == "CAI-300-01");
            Product caipi400 = products.First(p => p.Sku == "CAI-400-01");

            List<Promotion> promotions = new List<Promotion>
            {
                new Promotion { Name = "Promo: 2 Narguilés por R$ 35", StartDate = now.AddDays(-30), EndDate = now.AddDays(30), DiscountType = DiscountType.FixedValue, DiscountValue = 7.5m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Promotion { Name = "Promo: 3 Caipirinhas 300ml por R$ 15", StartDate = now.AddDays(-30), EndDate = now.AddDays(30), DiscountType = DiscountType.FixedValue, DiscountValue = 7m, IsActive = true, CreatedAt = now, CreatedBy = user },
                new Promotion { Name = "Promo: 2 Caipirinhas 400ml por R$ 16", StartDate = now.AddDays(-30), EndDate = now.AddDays(30), DiscountType = DiscountType.FixedValue, DiscountValue = 6m, IsActive = true, CreatedAt = now, CreatedBy = user }
            };

            await dbContext.Promotions.AddRangeAsync(promotions);
            await dbContext.SaveChangesAsync();

            List<PromotionItem> promoItems = new List<PromotionItem>
            {
                new PromotionItem { PromotionId = promotions[0].Id, ProductId = hookah.Id, CreatedAt = now, CreatedBy = user },
                new PromotionItem { PromotionId = promotions[1].Id, ProductId = caipi300.Id, CreatedAt = now, CreatedBy = user },
                new PromotionItem { PromotionId = promotions[2].Id, ProductId = caipi400.Id, CreatedAt = now, CreatedBy = user }
            };

            await dbContext.PromotionItems.AddRangeAsync(promoItems);
            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedSalesAsync(ApplicationDbContext dbContext, string user, int count, List<Product> products)
        {
            Faker<Sale> historicalFaker = new Faker<Sale>("pt_BR")
                .RuleFor(s => s.ReceiptNumber, f => f.Random.AlphaNumeric(10).ToUpper())
                .RuleFor(s => s.CustomerIdentifier, f => f.Random.Bool(0.6f) ? "Mesa " + f.Random.Int(1, 15) : "Balcão")
                .RuleFor(s => s.SaleDate, f => f.Random.Bool(0.8f) ? f.Date.RecentOffset(30).ToUniversalTime() : f.Date.PastOffset(1).ToUniversalTime())
                .RuleFor(s => s.Status, f => f.Random.Bool(0.95f) ? SaleStatus.Completed : SaleStatus.Cancelled)
                .RuleFor(s => s.PaymentMethod, f => f.PickRandom(PaymentMethod.Pix, PaymentMethod.CreditCard, PaymentMethod.DebitCard, PaymentMethod.Cash))
                .RuleFor(s => s.CreatedAt, (f, s) => s.SaleDate)
                .RuleFor(s => s.CreatedBy, user);

            List<Sale> sales = historicalFaker.Generate(count);

            Faker<Sale> pendingFaker = new Faker<Sale>("pt_BR")
                .RuleFor(s => s.ReceiptNumber, f => f.Random.AlphaNumeric(10).ToUpper())
                .RuleFor(s => s.CustomerIdentifier, f => "Mesa " + f.Random.Int(1, 40))
                .RuleFor(s => s.SaleDate, f => DateTimeOffset.UtcNow)
                .RuleFor(s => s.Status, SaleStatus.Pending)
                .RuleFor(s => s.PaymentMethod, PaymentMethod.Pending)
                .RuleFor(s => s.CreatedAt, (f, s) => s.SaleDate)
                .RuleFor(s => s.CreatedBy, user);

            sales.AddRange(pendingFaker.Generate(8));

            Faker itemFaker = new Faker("pt_BR");
            List<StockMovement> movements = new();
            List<ProductComposition> boms = await dbContext.ProductCompositions.ToListAsync();

            foreach (Sale sale in sales)
            {
                int itemCount = itemFaker.Random.Int(1, 6);
                sale.Items = new List<SaleItem>();
                decimal subTotal = 0;
                decimal discountTotal = 0;

                for (int i = 0; i < itemCount; i++)
                {
                    Product randomProduct = itemFaker.PickRandom(products);
                    decimal quantity = itemFaker.Random.Int(1, 4);
                    decimal unitPrice = randomProduct.SellingPrice;

                    decimal discount = 0m;
                    if (randomProduct.Sku == "NAR-SES-01" && quantity >= 2) discount = 7.5m * quantity;
                    if (randomProduct.Sku == "CAI-300-01" && quantity >= 3) discount = 7m * quantity;
                    if (randomProduct.Sku == "CAI-400-01" && quantity >= 2) discount = 6m * quantity;

                    SaleItemStatus itemStatus = sale.Status == SaleStatus.Cancelled ? SaleItemStatus.Canceled :
                        (itemFaker.Random.Bool(0.02f) ? itemFaker.PickRandom(SaleItemStatus.Canceled, SaleItemStatus.Damaged) : SaleItemStatus.Active);

                    sale.Items.Add(new SaleItem
                    {
                        ProductId = randomProduct.Id,
                        Quantity = quantity,
                        UnitPrice = unitPrice,
                        DiscountAmount = discount,
                        Total = (unitPrice * quantity) - discount,
                        Status = itemStatus,
                        CreatedAt = sale.SaleDate,
                        CreatedBy = user
                    });

                    if (itemStatus == SaleItemStatus.Active)
                    {
                        subTotal += (unitPrice * quantity);
                        discountTotal += discount;
                    }

                    MovementType moveType = itemStatus == SaleItemStatus.Canceled ? MovementType.Entry : (itemStatus == SaleItemStatus.Damaged ? MovementType.Loss : MovementType.Sale);
                    decimal multiplier = itemStatus == SaleItemStatus.Canceled ? 1m : -1m;

                    if (randomProduct.Type == ProductType.Service)
                    {
                        IEnumerable<ProductComposition> productBoms = boms.Where(b => b.ParentProductId == randomProduct.Id);
                        foreach (ProductComposition bom in productBoms)
                        {
                            decimal compQuantity = bom.Quantity * quantity;
                            movements.Add(new StockMovement
                            {
                                ProductId = bom.ChildProductId,
                                Quantity = compQuantity,
                                Type = moveType,
                                Reason = $"Dedução BOM (Serviço) via Recibo {sale.ReceiptNumber}",
                                MovementDate = sale.SaleDate,
                                CreatedAt = sale.SaleDate,
                                CreatedBy = user
                            });

                            Product compProduct = products.First(p => p.Id == bom.ChildProductId);
                            compProduct.CurrentStock += (compQuantity * multiplier);
                        }
                    }
                    else
                    {
                        movements.Add(new StockMovement
                        {
                            ProductId = randomProduct.Id,
                            Quantity = quantity,
                            Type = moveType,
                            Reason = $"Venda direta via Recibo {sale.ReceiptNumber}",
                            MovementDate = sale.SaleDate,
                            CreatedAt = sale.SaleDate,
                            CreatedBy = user
                        });

                        randomProduct.CurrentStock += (quantity * multiplier);
                    }
                }

                sale.SubTotal = subTotal;
                sale.DiscountAmount = discountTotal;
                sale.TotalAmount = subTotal - discountTotal;
            }

            await dbContext.Sales.AddRangeAsync(sales);
            await dbContext.StockMovements.AddRangeAsync(movements);
            dbContext.Products.UpdateRange(products);
            await dbContext.SaveChangesAsync();
        }
        #endregion Methods
    }
    #endregion Mock Database Seeder
}