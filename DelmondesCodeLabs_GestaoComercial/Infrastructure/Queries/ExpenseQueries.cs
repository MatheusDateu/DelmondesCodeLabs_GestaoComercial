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
    #region Expense Queries
    /// <summary>
    /// Implements high-performance read operations for the operational expenses using Dapper.
    /// </summary>
    public class ExpenseQueries(IConfiguration configuration) : IExpenseQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves a paginated list of expenses, optionally filtered by a search term.
        /// </summary>
        public async Task<PagedResult<Expense>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            int offset = (page - 1) * pageSize;

            string countSql = @"
                SELECT COUNT(*) 
                FROM ""Expenses"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR ""Description"" ILIKE '%' || @SearchTerm || '%');";

            CommandDefinition countCommand = new CommandDefinition(
                commandText: countSql,
                parameters: new { SearchTerm = searchTerm },
                cancellationToken: cancellationToken);

            int totalCount = await connection.ExecuteScalarAsync<int>(countCommand);

            string dataSql = @"
                SELECT * 
                FROM ""Expenses"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR ""Description"" ILIKE '%' || @SearchTerm || '%')
                ORDER BY ""DueDate"" ASC
                LIMIT @Limit OFFSET @Offset;";

            CommandDefinition dataCommand = new CommandDefinition(
                commandText: dataSql,
                parameters: new { SearchTerm = searchTerm, Limit = pageSize, Offset = offset },
                cancellationToken: cancellationToken);

            IEnumerable<Expense> items = await connection.QueryAsync<Expense>(dataCommand);

            return new PagedResult<Expense>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }
        #endregion Methods
    }
    #endregion Expense Queries
}