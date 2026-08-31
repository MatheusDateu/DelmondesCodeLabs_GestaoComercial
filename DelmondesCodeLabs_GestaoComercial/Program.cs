#region Usings
using DelmondesCodeLabs_GestaoComercial.Components;
using DelmondesCodeLabs_GestaoComercial.Data;
using DelmondesCodeLabs_GestaoComercial.Data.Seeders;
using DelmondesCodeLabs_GestaoComercial.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
#endregion

namespace DelmondesCodeLabs_GestaoComercial
{
    #region Entry Point
    /// <summary>
    /// The main entry point for the Delmondes CodeLabs Gestão Comercial application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Configures and starts the web application host.
        /// </summary>
        public static async Task Main(string[] args)
        {
            #region WebApplicationBuilder Configuration
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            WebApplication app = builder.Build();
            #endregion WebApplicationBuilder Configuration

            #region Pipeline Execution & Data Seeding
            app.ConfigureHttpRequestPipeline();

            using (IServiceScope scope = app.Services.CreateScope())
            {
                ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await dbContext.Database.MigrateAsync();

                IServiceProvider services = scope.ServiceProvider;
                await IdentityDbSeeder.SeedRolesAsync(services);

                if (app.Environment.IsDevelopment())
                {
                    await MockDbSeeder.SeedDevelopmentDataAsync(dbContext);
                }
            }
            
            await app.RunAsync();
            #endregion Pipeline Execution & Data Seeding
        }
    }
    #endregion
}