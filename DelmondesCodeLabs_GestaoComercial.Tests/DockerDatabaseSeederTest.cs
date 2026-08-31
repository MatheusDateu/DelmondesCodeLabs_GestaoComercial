#region Usings
using DelmondesCodeLabs_GestaoComercial.Data;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using DelmondesCodeLabs_GestaoComercial.Tests.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Tests
{
    #region Docker E2E Database Seeder
    /// <summary>
    /// Utility test designed to populate the local Docker PostgreSQL database with months of historical data.
    /// This prepares the environment for manual QA and automated E2E UI testing via Playwright.
    /// </summary>
    public class DockerDatabaseSeederTest
    {
        #region Tests
        [Fact]
        public async Task SeedDockerPostgres_ForE2ETesting()
        {
            // Atacando a porta 5433 diretamente para desviar do PostgreSQL nativo do Windows
            string connectionString = "Host=127.0.0.1;Port=5433;Database=GestaoComercialDb;Username=superuser;Password=L0c4l.S3cr3t..Sup3rus3r";

            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            IHttpContextAccessor mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();

            await using ApplicationDbContext dbContext = new ApplicationDbContext(options, mockHttpContextAccessor);

            if (await dbContext.Categories.AnyAsync())
            {
                return;
            }

            MockDataSeeder seeder = new MockDataSeeder(dbContext);

            int expectedCategories = 8;
            int expectedSuppliers = 5;
            int expectedProducts = 40;
            int expectedStockEntries = 25;
            int expectedExpenses = 80;
            int expectedPromotions = 3;
            int expectedSales = 500;

            List<Category> categories = await seeder.SeedTier1CategoriesAsync(expectedCategories);
            List<Supplier> suppliers = await seeder.SeedTier1SuppliersAsync(expectedSuppliers);

            List<Product> products = await seeder.SeedTier2ProductsAsync(expectedProducts, categories);

            await seeder.SeedTier3InventoryAsync(expectedStockEntries, suppliers, products);
            await seeder.SeedTier4ExpensesAsync(expectedExpenses);
            await seeder.SeedTier5PromotionsAsync(expectedPromotions, products);
            await seeder.SeedTier6SalesAsync(expectedSales, products);
        }
        #endregion Tests
    }
    #endregion Docker E2E Database Seeder
}