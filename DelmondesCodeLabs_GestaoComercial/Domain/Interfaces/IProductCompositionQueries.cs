#region Usings
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Product Composition Queries Interface
    /// <summary>
    /// Defines the high-performance read operations for product compositions (Bill of Materials) using Dapper.
    /// </summary>
    public interface IProductCompositionQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves all components linked to a specific parent service or kit.
        /// </summary>
        Task<IEnumerable<ProductComposition>> GetByParentProductIdAsync(Guid parentProductId, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Product Composition Queries Interface
}