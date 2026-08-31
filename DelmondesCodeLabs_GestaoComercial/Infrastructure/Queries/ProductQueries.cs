#region Usings
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using DelmondesCodeLabs_GestaoComercial.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Infrastructure.Queries
{
    #region Product Queries
    /// <summary>
    /// Implements high-performance read operations for the product domain using Dapper.
    /// </summary>
    public class ProductQueries(IConfiguration configuration) : IProductQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT *
                FROM ""Products""
                WHERE ""Id"" = @Id AND ""IsDeleted"" = false;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { Id = id },
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Product>(command);
        }

        /// <summary>
        /// Retrieves a paginated list of products, filtering by SKU, Barcode, or Name using unaccented matching.
        /// </summary>
        public async Task<PagedResult<Product>> SearchProductsAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            int offset = (page - 1) * pageSize;

            string countSql = @"
                SELECT COUNT(*) 
                FROM ""Products"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL 
                    OR ""Sku"" ILIKE '%' || @SearchTerm || '%' 
                    OR ""Barcode"" ILIKE '%' || @SearchTerm || '%' 
                    OR unaccent(""Name"") ILIKE unaccent('%' || @SearchTerm || '%'));";

            CommandDefinition countCommand = new CommandDefinition(
                commandText: countSql,
                parameters: new { SearchTerm = searchTerm },
                cancellationToken: cancellationToken);

            int totalCount = await connection.ExecuteScalarAsync<int>(countCommand);

            string dataSql = @"
                SELECT * 
                FROM ""Products"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL 
                    OR ""Sku"" ILIKE '%' || @SearchTerm || '%' 
                    OR ""Barcode"" ILIKE '%' || @SearchTerm || '%' 
                    OR unaccent(""Name"") ILIKE unaccent('%' || @SearchTerm || '%'))
                ORDER BY ""Name"" ASC
                LIMIT @Limit OFFSET @Offset;";

            CommandDefinition dataCommand = new CommandDefinition(
                commandText: dataSql,
                parameters: new { SearchTerm = searchTerm, Limit = pageSize, Offset = offset },
                cancellationToken: cancellationToken);

            IEnumerable<Product> items = await connection.QueryAsync<Product>(dataCommand);

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves a product by its unique stock keeping unit (SKU) or barcode.
        /// </summary>
        public async Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT * 
                FROM ""Products"" 
                WHERE ""Sku"" = @Code AND ""IsDeleted"" = false;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { Code = code },
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Product>(command);
        }

        #endregion Methods
    }
    #endregion Product Queries
}