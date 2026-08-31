#region Usings
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using DelmondesCodeLabs_GestaoComercial.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Infrastructure.Queries
{
    #region Product Composition Queries
    /// <summary>
    /// Implements high-performance read operations for the product composition domain using Dapper.
    /// </summary>
    public class ProductCompositionQueries(IConfiguration configuration) : IProductCompositionQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves all components linked to a specific parent service or kit, mapping the child product details.
        /// </summary>
        public async Task<IEnumerable<ProductComposition>> GetByParentProductIdAsync(Guid parentProductId, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT pc.*, cp.*
                FROM ""ProductCompositions"" pc
                INNER JOIN ""Products"" cp ON pc.""ChildProductId"" = cp.""Id""
                WHERE pc.""ParentProductId"" = @ParentProductId 
                AND pc.""IsDeleted"" = false;";

            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { ParentProductId = parentProductId },
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<ProductComposition, Product, ProductComposition>(
                command,
                (composition, childProduct) =>
                {
                    composition.ChildProduct = childProduct;
                    return composition;
                },
                splitOn: "Id");
        }
        #endregion Methods
    }
    #endregion Product Composition Queries
}