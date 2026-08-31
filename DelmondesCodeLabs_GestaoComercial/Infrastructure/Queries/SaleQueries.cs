#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using DelmondesCodeLabs_GestaoComercial.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Infrastructure.Queries
{
    #region Sale Queries
    /// <summary>
    /// Implements high-performance read operations for the sales domain using Dapper.
    /// </summary>
    public class SaleQueries(IConfiguration configuration) : ISaleQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves a sale by its unique identifier, including all its line items and product details.
        /// </summary>
        public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT s.*, si.*, p.*
                FROM ""Sales"" s
                LEFT JOIN ""SaleItems"" si ON s.""Id"" = si.""SaleId"" AND si.""IsDeleted"" = false
                LEFT JOIN ""Products"" p ON si.""ProductId"" = p.""Id"" AND p.""IsDeleted"" = false
                WHERE s.""Id"" = @Id AND s.""IsDeleted"" = false;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { Id = id },
                cancellationToken: cancellationToken);

            Dictionary<Guid, Sale> saleDictionary = new Dictionary<Guid, Sale>();

            IEnumerable<Sale> result = await connection.QueryAsync<Sale, SaleItem, Product, Sale>(
                command,
                (sale, item, product) =>
                {
                    if (!saleDictionary.TryGetValue(sale.Id, out Sale? currentSale))
                    {
                        currentSale = sale;
                        currentSale.Items = new List<SaleItem>();
                        saleDictionary.Add(currentSale.Id, currentSale);
                    }

                    if (item != null)
                    {
                        item.Product = product;
                        currentSale.Items.Add(item);
                    }

                    return currentSale;
                },
                splitOn: "Id,Id");

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a paginated list of sales, optionally filtered by receipt number.
        /// </summary>
        public async Task<PagedResult<Sale>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            int offset = (page - 1) * pageSize;

            string countSql = @"
                SELECT COUNT(*) 
                FROM ""Sales"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR ""ReceiptNumber"" ILIKE '%' || @SearchTerm || '%');";

            CommandDefinition countCommand = new CommandDefinition(
                commandText: countSql,
                parameters: new { SearchTerm = searchTerm },
                cancellationToken: cancellationToken);

            int totalCount = await connection.ExecuteScalarAsync<int>(countCommand);

            string dataSql = @"
                SELECT * 
                FROM ""Sales"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR ""ReceiptNumber"" ILIKE '%' || @SearchTerm || '%')
                ORDER BY ""SaleDate"" DESC
                LIMIT @Limit OFFSET @Offset;";

            CommandDefinition dataCommand = new CommandDefinition(
                commandText: dataSql,
                parameters: new { SearchTerm = searchTerm, Limit = pageSize, Offset = offset },
                cancellationToken: cancellationToken);

            IEnumerable<Sale> items = await connection.QueryAsync<Sale>(dataCommand);

            return new PagedResult<Sale>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves all currently open tabs (pending sales), ordered by the most recently modified.
        /// </summary>
        public async Task<IEnumerable<Sale>> GetOpenTabsAsync(string? searchTerm, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT * 
                FROM ""Sales"" 
                WHERE ""IsDeleted"" = false AND ""Status"" = 1 
                AND (@SearchTerm IS NULL OR ""CustomerIdentifier"" ILIKE '%' || @SearchTerm || '%')
                ORDER BY COALESCE(""UpdatedAt"", ""CreatedAt"") DESC;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { SearchTerm = searchTerm },
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<Sale>(command);
        }

        #endregion Methods
    }
    #endregion Sale Queries
}