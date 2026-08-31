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
    #region Category Queries
    /// <summary>
    /// Implements high-performance read operations for the category domain using Dapper.
    /// </summary>
    public class CategoryQueries(IConfiguration configuration) : ICategoryQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves a category by its unique identifier.
        /// </summary>
        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT * 
                FROM ""Categories"" 
                WHERE ""Id"" = @Id AND ""IsDeleted"" = false;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { Id = id },
                cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<Category>(command);
        }

        /// <summary>
        /// Retrieves all active categories, typically used to populate POS filter buttons.
        /// </summary>
        public async Task<IEnumerable<Category>> GetAllActiveAsync(CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT * 
                FROM ""Categories"" 
                WHERE ""IsDeleted"" = false 
                ORDER BY ""Name"";";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<Category>(command);
        }

        /// <summary>
        /// Retrieves a paginated list of categories for the administration dashboard.
        /// </summary>
        public async Task<PagedResult<Category>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            int offset = (page - 1) * pageSize;

            string countSql = @"
                SELECT COUNT(*) 
                FROM ""Categories"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR to_tsvector('portuguese', ""Name"") @@ plainto_tsquery('portuguese', @SearchTerm));";

            CommandDefinition countCommand = new CommandDefinition(
                commandText: countSql,
                parameters: new { SearchTerm = searchTerm },
                cancellationToken: cancellationToken);

            int totalCount = await connection.ExecuteScalarAsync<int>(countCommand);

            string dataSql = @"
                SELECT * 
                FROM ""Categories"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR to_tsvector('portuguese', ""Name"") @@ plainto_tsquery('portuguese', @SearchTerm))
                ORDER BY ""Name""
                LIMIT @Limit OFFSET @Offset;";

            CommandDefinition dataCommand = new CommandDefinition(
                commandText: dataSql,
                parameters: new { SearchTerm = searchTerm, Limit = pageSize, Offset = offset },
                cancellationToken: cancellationToken);

            IEnumerable<Category> items = await connection.QueryAsync<Category>(dataCommand);

            return new PagedResult<Category>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }
        #endregion Methods
    }
    #endregion Category Queries
}