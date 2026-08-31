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
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Infrastructure.Queries
{
    #region Supplier Queries
    /// <summary>
    /// Implements high-performance read operations for the supplier domain using Dapper.
    /// </summary>
    public class SupplierQueries(IConfiguration configuration) : ISupplierQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves a supplier by its unique identifier.
        /// </summary>
        public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT * 
                FROM ""Suppliers"" 
                WHERE ""Id"" = @Id AND ""IsDeleted"" = false;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { Id = id },
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Supplier>(command);
        }

        /// <summary>
        /// Retrieves a paginated list of suppliers, optionally filtered by a search term.
        /// </summary>
        public async Task<PagedResult<Supplier>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            int offset = (page - 1) * pageSize;

            string countSql = @"
                SELECT COUNT(*) 
                FROM ""Suppliers"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR to_tsvector('portuguese', ""Name"") @@ plainto_tsquery('portuguese', @SearchTerm) OR ""Cnpj"" ILIKE '%' || @SearchTerm || '%');";

            CommandDefinition countCommand = new CommandDefinition(
                commandText: countSql,
                parameters: new { SearchTerm = searchTerm },
                cancellationToken: cancellationToken);

            int totalCount = await connection.ExecuteScalarAsync<int>(countCommand);

            string dataSql = @"
                SELECT * 
                FROM ""Suppliers"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR to_tsvector('portuguese', ""Name"") @@ plainto_tsquery('portuguese', @SearchTerm) OR ""Cnpj"" ILIKE '%' || @SearchTerm || '%')
                ORDER BY ""Name""
                LIMIT @Limit OFFSET @Offset;";

            CommandDefinition dataCommand = new CommandDefinition(
                commandText: dataSql,
                parameters: new { SearchTerm = searchTerm, Limit = pageSize, Offset = offset },
                cancellationToken: cancellationToken);

            IEnumerable<Supplier> items = await connection.QueryAsync<Supplier>(dataCommand);

            return new PagedResult<Supplier>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }
        #endregion Methods
    }
    #endregion Supplier Queries
}