#region Usings
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using DelmondesCodeLabs_GestaoComercial.Domain.DTOs;
using DelmondesCodeLabs_GestaoComercial.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Infrastructure.Queries
{
    #region BI Dashboard Queries
    /// <summary>
    /// Implements high-performance read operations for the Business Intelligence dashboards using Dapper.
    /// </summary>
    public class BIDashboardQueries(IConfiguration configuration) : IBIDashboardQueries
    {
        #region Fields
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        #endregion Fields

        #region Methods
        /// <summary>
        /// Retrieves the aggregated financial summary for a specific date range.
        /// </summary>
        public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT 
                    COALESCE((SELECT SUM(""TotalAmount"") FROM ""Sales"" WHERE ""Status"" = 2 AND ""IsDeleted"" = false AND ""SaleDate"" >= @StartDate AND ""SaleDate"" <= @EndDate), 0) AS GrossRevenue,
                    COALESCE((SELECT SUM(""DiscountAmount"") FROM ""Sales"" WHERE ""Status"" = 2 AND ""IsDeleted"" = false AND ""SaleDate"" >= @StartDate AND ""SaleDate"" <= @EndDate), 0) AS TotalDiscounts,
                    COALESCE((SELECT SUM(si.""Quantity"" * p.""CostPrice"") FROM ""SaleItems"" si INNER JOIN ""Sales"" s ON si.""SaleId"" = s.""Id"" INNER JOIN ""Products"" p ON si.""ProductId"" = p.""Id"" WHERE s.""Status"" = 2 AND s.""IsDeleted"" = false AND si.""Status"" = 1 AND si.""IsDeleted"" = false AND s.""SaleDate"" >= @StartDate AND s.""SaleDate"" <= @EndDate), 0) AS CostOfGoodsSold,
                    COALESCE((SELECT SUM(""Amount"") FROM ""Expenses"" WHERE ""IsPaid"" = true AND ""IsDeleted"" = false AND ""PaymentDate"" >= @StartDate AND ""PaymentDate"" <= @EndDate), 0) AS TotalExpenses,
                    COALESCE((SELECT SUM(sm.""Quantity"" * p.""CostPrice"") FROM ""StockMovements"" sm INNER JOIN ""Products"" p ON sm.""ProductId"" = p.""Id"" WHERE sm.""Type"" = 3 AND sm.""IsDeleted"" = false AND sm.""MovementDate"" >= @StartDate AND sm.""MovementDate"" <= @EndDate), 0) AS TotalLosses;";

            // O Npgsql exige que o DateTimeOffset para "timestamp with time zone" tenha Offset 0 (UTC).
            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { StartDate = startDate.ToUniversalTime(), EndDate = endDate.ToUniversalTime() },
                cancellationToken: cancellationToken);

            return await connection.QuerySingleAsync<FinancialSummaryDto>(command);
        }

        /// <summary>
        /// Retrieves the top-performing products based on sales revenue for the ABC curve analysis.
        /// </summary>
        public async Task<IEnumerable<AbcProductDto>> GetAbcSalesCurveAsync(DateTimeOffset startDate, DateTimeOffset endDate, int topN = 10, CancellationToken cancellationToken = default)
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(_connectionString);

            string sql = @"
                SELECT 
                    p.""Name"" AS ProductName,
                    SUM(si.""Quantity"") AS TotalQuantitySold,
                    SUM(si.""Total"") AS TotalRevenue
                FROM ""SaleItems"" si
                INNER JOIN ""Sales"" s ON si.""SaleId"" = s.""Id""
                INNER JOIN ""Products"" p ON si.""ProductId"" = p.""Id""
                WHERE s.""Status"" = 2 
                  AND s.""IsDeleted"" = false 
                  AND si.""Status"" = 1 
                  AND si.""IsDeleted"" = false
                  AND s.""SaleDate"" >= @StartDate 
                  AND s.""SaleDate"" <= @EndDate
                GROUP BY p.""Name""
                ORDER BY TotalRevenue DESC
                LIMIT @TopN;";

            // O Npgsql exige que o DateTimeOffset para "timestamp with time zone" tenha Offset 0 (UTC).
            CommandDefinition command = new CommandDefinition(
                commandText: sql,
                parameters: new { StartDate = startDate.ToUniversalTime(), EndDate = endDate.ToUniversalTime(), TopN = topN },
                cancellationToken: cancellationToken);

            return await connection.QueryAsync<AbcProductDto>(command);
        }
        #endregion Methods
    }
    #endregion BI Dashboard Queries
}