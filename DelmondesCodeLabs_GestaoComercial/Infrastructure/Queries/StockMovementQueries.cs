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
    #region Stock Movement Queries
    /// <summary>
    /// Implements high-performance read operations for the stock movement ledger using Dapper.
    /// </summary>
    public class StockMovementQueries(IConfiguration configuration) : IStockMovementQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves a paginated history of stock movements for a specific product.
        /// </summary>
        public async Task<PagedResult<StockMovement>> GetByProductIdAsync(Guid productId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            int offset = (page - 1) * pageSize;

            string countSql = @"
                SELECT COUNT(*) 
                FROM ""StockMovements"" 
                WHERE ""ProductId"" = @ProductId AND ""IsDeleted"" = false;";

            CommandDefinition countCommand = new CommandDefinition(
                commandText: countSql,
                parameters: new { ProductId = productId },
                cancellationToken: cancellationToken);

            int totalCount = await connection.ExecuteScalarAsync<int>(countCommand);

            string dataSql = @"
                SELECT * 
                FROM ""StockMovements"" 
                WHERE ""ProductId"" = @ProductId AND ""IsDeleted"" = false
                ORDER BY ""MovementDate"" DESC
                LIMIT @Limit OFFSET @Offset;";

            CommandDefinition dataCommand = new CommandDefinition(
                commandText: dataSql,
                parameters: new { ProductId = productId, Limit = pageSize, Offset = offset },
                cancellationToken: cancellationToken);

            IEnumerable<StockMovement> items = await connection.QueryAsync<StockMovement>(dataCommand);

            return new PagedResult<StockMovement>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }
        #endregion Methods
    }
    #endregion Stock Movement Queries
}