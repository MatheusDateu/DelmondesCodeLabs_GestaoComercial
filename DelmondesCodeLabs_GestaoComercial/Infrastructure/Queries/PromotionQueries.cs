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
    #region Promotion Queries
    /// <summary>
    /// Implements high-performance read operations for the promotion domain using Dapper.
    /// </summary>
    public class PromotionQueries(IConfiguration configuration) : IPromotionQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves a promotion by its unique identifier, including its associated items.
        /// </summary>
        public async Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT p.*, pi.*, prod.*, cat.*
                FROM ""Promotions"" p
                LEFT JOIN ""PromotionItems"" pi ON p.""Id"" = pi.""PromotionId"" AND pi.""IsDeleted"" = false
                LEFT JOIN ""Products"" prod ON pi.""ProductId"" = prod.""Id"" AND prod.""IsDeleted"" = false
                LEFT JOIN ""Categories"" cat ON pi.""CategoryId"" = cat.""Id"" AND cat.""IsDeleted"" = false
                WHERE p.""Id"" = @Id AND p.""IsDeleted"" = false;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { Id = id },
                cancellationToken: cancellationToken);

            Dictionary<Guid, Promotion> promotionDictionary = new Dictionary<Guid, Promotion>();

            IEnumerable<Promotion> result = await connection.QueryAsync<Promotion, PromotionItem, Product, Category, Promotion>(
                command,
                (promotion, item, product, category) =>
                {
                    if (!promotionDictionary.TryGetValue(promotion.Id, out Promotion? currentPromotion))
                    {
                        currentPromotion = promotion;
                        currentPromotion.Items = new List<PromotionItem>();
                        promotionDictionary.Add(currentPromotion.Id, currentPromotion);
                    }

                    if (item != null)
                    {
                        item.Product = product;
                        item.Category = category;
                        currentPromotion.Items.Add(item);
                    }

                    return currentPromotion;
                },
                splitOn: "Id,Id,Id,Id");

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a paginated list of promotions, optionally filtered by a search term.
        /// </summary>
        public async Task<PagedResult<Promotion>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            int offset = (page - 1) * pageSize;

            string countSql = @"
                SELECT COUNT(*) 
                FROM ""Promotions"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR to_tsvector('portuguese', ""Name"") @@ plainto_tsquery('portuguese', @SearchTerm));";

            CommandDefinition countCommand = new CommandDefinition(
                commandText: countSql,
                parameters: new { SearchTerm = searchTerm },
                cancellationToken: cancellationToken);

            int totalCount = await connection.ExecuteScalarAsync<int>(countCommand);

            string dataSql = @"
                SELECT * 
                FROM ""Promotions"" 
                WHERE ""IsDeleted"" = false 
                AND (@SearchTerm IS NULL OR to_tsvector('portuguese', ""Name"") @@ plainto_tsquery('portuguese', @SearchTerm))
                ORDER BY ""StartDate"" DESC
                LIMIT @Limit OFFSET @Offset;";

            CommandDefinition dataCommand = new CommandDefinition(
                commandText: dataSql,
                parameters: new { SearchTerm = searchTerm, Limit = pageSize, Offset = offset },
                cancellationToken: cancellationToken);

            IEnumerable<Promotion> items = await connection.QueryAsync<Promotion>(dataCommand);

            return new PagedResult<Promotion>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Retrieves all currently active and valid promotions.
        /// </summary>
        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync(CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT * 
                FROM ""Promotions"" 
                WHERE ""IsDeleted"" = false 
                AND ""IsActive"" = true 
                AND ""StartDate"" <= @Now 
                AND ""EndDate"" >= @Now
                ORDER BY ""EndDate"" ASC;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { Now = DateTimeOffset.UtcNow },
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<Promotion>(command);
        }

        #endregion Methods
    }
    #endregion Promotion Queries
}