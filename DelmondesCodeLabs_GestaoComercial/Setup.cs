#region Usings
using DelmondesCodeLabs_GestaoComercial.Components;
using DelmondesCodeLabs_GestaoComercial.Components.Account;
using DelmondesCodeLabs_GestaoComercial.Data;
using DelmondesCodeLabs_GestaoComercial.Domain.Interfaces;
using DelmondesCodeLabs_GestaoComercial.Infrastructure.Queries;
using DelmondesCodeLabs_GestaoComercial.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MudBlazor.Services;
using System.Globalization;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Extensions
{
    #region Application Setup
    /// <summary>
    /// Extension methods for configuring services and the HTTP request pipeline.
    /// Encapsulates application startup logic to keep Program.cs clean.
    /// </summary>
    public static class Setup
    {
        #region Service Registrations
        /// <summary>
        /// Registers core UI components, MudBlazor, and FluentValidation services.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddRazorComponents()
                    .AddInteractiveServerComponents();

            services.AddMudServices();

            services.AddValidatorsFromAssemblyContaining<Program>();

            services.AddHttpContextAccessor();

            services.AddHealthChecks();

            return services;
        }

        /// <summary>
        /// Configures authentication, authorisation, Identity services, and the PostgreSQL database context.
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Other Infrastructure Services
            services.AddCascadingAuthenticationState();
            services.AddScoped<IdentityRedirectManager>();
            services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            });

            services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/keys"))
                    .SetApplicationName("DelmondesCodeLabs_GestaoComercial");

            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

            services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
            services.AddLocalization(options => options.ResourcesPath = "");

            #endregion

            #region CQRS - Write Repositories
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<ISupplierRepository, SupplierRepository>();
            services.AddTransient<IStockEntryRepository, StockEntryRepository>();
            services.AddTransient<IStockMovementRepository, StockMovementRepository>();
            services.AddTransient<IProductCompositionRepository, ProductCompositionRepository>();
            services.AddTransient<IExpenseRepository, ExpenseRepository>();
            services.AddTransient<IPromotionRepository, PromotionRepository>();
            services.AddTransient<ISaleRepository, SaleRepository>();
            #endregion CQRS - Write Repositories

            #region CQRS - Read Repositories
            services.AddScoped<IProductQueries, ProductQueries>();
            services.AddScoped<ICategoryQueries, CategoryQueries>();
            services.AddScoped<ISupplierQueries, SupplierQueries>();
            services.AddScoped<IStockEntryQueries, StockEntryQueries>();
            services.AddScoped<IStockMovementQueries, StockMovementQueries>();
            services.AddScoped<IProductCompositionQueries, ProductCompositionQueries>();
            services.AddScoped<IExpenseQueries, ExpenseQueries>();
            services.AddScoped<IPromotionQueries, PromotionQueries>();
            services.AddScoped<ISaleQueries, SaleQueries>();
            services.AddScoped<IBIDashboardQueries, BIDashboardQueries>();
            #endregion CQRS - Read Repositories
            return services;
        }
        #endregion

        #region Pipeline Configuration
        /// <summary>
        /// Configures the HTTP request pipeline middleware for routing, security, and error handling.
        /// </summary>
        public static void ConfigureHttpRequestPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

            app.MapStaticAssets();

            app.MapHealthChecks("/health");

            app.UseAntiforgery();

            app.MapRazorComponents<App>()
               .AddInteractiveServerRenderMode();

            app.MapAdditionalIdentityEndpoints();

            #region Localisation
            string[] supportedCultures = ["pt-BR"];
            RequestLocalizationOptions localisationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localisationOptions);

            // Forces the default culture for all threads in the application to ensure consistent formatting and parsing of dates, numbers, and other culture-sensitive data.
            CultureInfo defaultCulture = new CultureInfo(supportedCultures[0]);
            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
            #endregion
        }
        #endregion
    }
    #endregion
}