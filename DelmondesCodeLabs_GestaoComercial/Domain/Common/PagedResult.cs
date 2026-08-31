#region Usings
using System;
using System.Collections.Generic;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Common
{
    #region Paged Result
    /// <summary>
    /// Represents a paginated subset of data along with its metadata for UI rendering.
    /// </summary>
    public class PagedResult<T>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the collection of items for the current page.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = [];

        /// <summary>
        /// Gets or sets the total number of records across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the current page number (1-based index).
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Calculates the total number of pages based on TotalCount and PageSize.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        #endregion Properties
    }
    #endregion Paged Result
}