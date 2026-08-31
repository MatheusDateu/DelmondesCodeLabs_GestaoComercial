#region Usings
using DelmondesCodeLabs_GestaoComercial.Data;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using DelmondesCodeLabs_GestaoComercial.Tests.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Tests
{
    #region Database Seeder Tests
    /// <summary>
    /// Validates the mock data generation and the mathematical integrity of the inventory engine.
    /// </summary>
    public class DatabaseSeederTests
    {
        #region Setup
        /// <summary>
        /// Provisions an in-memory database context for isolated testing.
        /// </summary>
        private ApplicationDbContext CreateInMemoryContext()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Mocks the HttpContext to bypass the AuditableEntity interception user requirements
            IHttpContextAccessor mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();

            return new ApplicationDbContext(options, mockHttpContextAccessor);
        }
        #endregion Setup

        #region Tests
        /// <summary>
        /// Orchestrates the full Tier 1 to Tier 6 data seeding and asserts transactional integrity.
        /// </summary>
        [Fact]
        public async Task SeedAllTiers_ShouldPopulateDatabase_AndMaintainStockIntegrity()
        {
            // Arrange
            await using ApplicationDbContext dbContext = CreateInMemoryContext();
            MockDataSeeder seeder = new MockDataSeeder(dbContext);

            int expectedCategories = 5;
            int expectedSuppliers = 3;
            int expectedProducts = 20;
            int expectedStockEntries = 15;
            int expectedExpenses = 50;
            int expectedPromotions = 2;
            int expectedSales = 100;

            // Act - Tier 1
            List<Category> categories = await seeder.SeedTier1CategoriesAsync(expectedCategories);
            List<Supplier> suppliers = await seeder.SeedTier1SuppliersAsync(expectedSuppliers);

            // Act - Tier 2
            List<Product> products = await seeder.SeedTier2ProductsAsync(expectedProducts, categories);

            // Act - Tier 3
            await seeder.SeedTier3InventoryAsync(expectedStockEntries, suppliers, products);

            // Act - Tier 4
            await seeder.SeedTier4ExpensesAsync(expectedExpenses);

            // Act - Tier 5
            await seeder.SeedTier5PromotionsAsync(expectedPromotions, products);

            // Act - Tier 6
            await seeder.SeedTier6SalesAsync(expectedSales, products);

            // Assert - Row Counts
            dbContext.Categories.Count().Should().Be(expectedCategories);
            dbContext.Suppliers.Count().Should().Be(expectedSuppliers);
            dbContext.Products.Count().Should().Be(expectedProducts);
            dbContext.StockEntries.Count().Should().Be(expectedStockEntries);
            dbContext.Expenses.Count().Should().Be(expectedExpenses);
            dbContext.Promotions.Count().Should().Be(expectedPromotions);
            dbContext.Sales.Count().Should().Be(expectedSales);

            // Assert - Mathematical Integrity of the Inventory Engine
            foreach (Product product in dbContext.Products)
            {
                decimal totalEntries = dbContext.StockMovements
                    .Where(m => m.ProductId == product.Id && m.Type == MovementType.Entry)
                    .Sum(m => m.Quantity);

                decimal totalDeductions = dbContext.StockMovements
                    .Where(m => m.ProductId == product.Id && (m.Type == MovementType.Sale || m.Type == MovementType.Loss || m.Type == MovementType.InternalConsumption))
                    .Sum(m => m.Quantity);

                decimal expectedStock = totalEntries - totalDeductions;

                // Validates that the cached CurrentStock perfectly matches the sum of immutable movements
                product.CurrentStock.Should().Be(expectedStock);
            }

            // Assert - Financial Integrity of Sales
            foreach (Sale sale in dbContext.Sales.Include(s => s.Items))
            {
                // Only active items compute towards the final receipt totals
                decimal expectedSubTotal = sale.Items.Where(i => i.Status == SaleItemStatus.Active).Sum(i => i.UnitPrice * i.Quantity);
                decimal expectedDiscount = sale.Items.Where(i => i.Status == SaleItemStatus.Active).Sum(i => i.DiscountAmount);

                sale.SubTotal.Should().Be(expectedSubTotal);
                sale.TotalAmount.Should().Be(expectedSubTotal - expectedDiscount);
            }
        }
        #endregion Tests
    }
    #endregion Database Seeder Tests
}