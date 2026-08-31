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
    #region Stock Entry Queries
    /// <summary>
    /// Implements high-performance read operations for the stock entry domain using Dapper.
    /// </summary>
    public class StockEntryQueries(IConfiguration configuration) : IStockEntryQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves a stock entry by its unique identifier, including its line items and supplier details.
        /// </summary>
        public async Task<StockEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT se.*, s.*, sei.*, p.*
                FROM ""StockEntries"" se
                INNER JOIN ""Suppliers"" s ON se.""SupplierId"" = s.""Id""
                LEFT JOIN ""StockEntryItems"" sei ON se.""Id"" = sei.""StockEntryId"" AND sei.""IsDeleted"" = false
                LEFT JOIN ""Products"" p ON sei.""ProductId"" = p.""Id""
                WHERE se.""Id"" = @Id AND se.""IsDeleted"" = false;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { Id = id },
                cancellationToken: cancellationToken);

            Dictionary<Guid, StockEntry> entryDictionary = new Dictionary<Guid, StockEntry>();

            IEnumerable<StockEntry> result = await connection.QueryAsync<StockEntry, Supplier, StockEntryItem, Product, StockEntry>(
                command,
                (entry, supplier, item, product) =>
                {
                    if (!entryDictionary.TryGetValue(entry.Id, out StockEntry? currentEntry))
                    {
                        currentEntry = entry;
                        currentEntry.Supplier = supplier;
                        currentEntry.Items = new List<StockEntryItem>();
                        entryDictionary.Add(currentEntry.Id, currentEntry);
                    }

                    if (item != null)
                    {
                        item.Product = product;
                        currentEntry.Items.Add(item);
                    }

                    return currentEntry;
                },
                splitOn: "Id,Id,Id");

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a paginated list of stock entries, optionally filtered by invoice number or supplier name.
        /// </summary>
        public async Task<PagedResult<StockEntry>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            int offset = (page - 1) * pageSize;

            string countSql = @"
                SELECT COUNT(*) 
                FROM ""StockEntries"" se
                INNER JOIN ""Suppliers"" s ON se.""SupplierId"" = s.""Id""
                WHERE se.""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR se.""InvoiceNumber"" ILIKE '%' || @SearchTerm || '%' OR to_tsvector('portuguese', s.""Name"") @@ plainto_tsquery('portuguese', @SearchTerm));";

            CommandDefinition countCommand = new CommandDefinition(
                commandText: countSql,
                parameters: new { SearchTerm = searchTerm },
                cancellationToken: cancellationToken);

            int totalCount = await connection.ExecuteScalarAsync<int>(countCommand);

            string dataSql = @"
                SELECT se.*, s.*
                FROM ""StockEntries"" se
                INNER JOIN ""Suppliers"" s ON se.""SupplierId"" = s.""Id""
                WHERE se.""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR se.""InvoiceNumber"" ILIKE '%' || @SearchTerm || '%' OR to_tsvector('portuguese', s.""Name"") @@ plainto_tsquery('portuguese', @SearchTerm))
                ORDER BY se.""EntryDate"" DESC
                LIMIT @Limit OFFSET @Offset;";

            CommandDefinition dataCommand = new CommandDefinition(
                commandText: dataSql,
                parameters: new { SearchTerm = searchTerm, Limit = pageSize, Offset = offset },
                cancellationToken: cancellationToken);

            IEnumerable<StockEntry> items = await connection.QueryAsync<StockEntry, Supplier, StockEntry>(
                dataCommand,
                (entry, supplier) =>
                {
                    entry.Supplier = supplier;
                    return entry;
                },
                splitOn: "Id");

            return new PagedResult<StockEntry>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }
        #endregion Methods
    }
    #endregion Stock Entry Queries
}