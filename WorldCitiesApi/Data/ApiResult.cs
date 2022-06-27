namespace WorldCitiesAPI.Data;

using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

public class ApiResult<T> {
	/// <summary>
	///     Private constructor called by the CreateAsync method.
	/// </summary>
	private ApiResult(
		List<T> data,
		int     count,
		int     pageIndex,
		int     pageSize,

		//Added to enable sorting
		string? sortColumn,
		string? sortOrder,

		//Added to enable filtering
		string? filterColumn,
		string? filterQuery
	) {
		Data = data;
		PageIndex = pageIndex;
		PageSize = pageSize;
		TotalCount = count;
		TotalPages = ( int ) Math.Ceiling( count / ( double ) pageSize );

		//Added to enable sorting
		SortColumn = sortColumn;
		SortOrder = sortOrder;

		//Added to enable filtering
		FilterColumn = filterColumn;
		FilterQuery = filterQuery;
	}

	#region Methods

	/// <summary>
	///     Pages, sorts and/or filters a IQueryable source.
	/// </summary>
	/// <param name="source">
	///     An IQueryable source of generic
	///     type
	/// </param>
	/// <param name="pageIndex">
	///     Zero-based current page index
	///     (0 = first page)
	/// </param>
	/// <param name="pageSize">
	///     The actual size of each
	///     page
	/// </param>
	/// <param name="sortColumn">The sorting column name</param>
	/// <param name="sortOrder">The sorting order ("ASC" or "DESC")</param>
	/// <param name="filterColumn">The filtering column name</param>
	/// <param name="filterQuery">The filtering column query (value to lookup)</param>
	/// <returns>
	///     A object containing the paged/sorted/filtered result
	///     and all the relevant paging/sorting/filtering navigation info.
	/// </returns>
	public static async Task<ApiResult<T>> CreateAsync(
		IQueryable<T> source,
		int           pageIndex,
		int           pageSize,

		//Added to enable sorting
		string? sortColumn = null,
		string? sortOrder  = null,

		//Added to enable filtering
		string? filterColumn = null,
		string? filterQuery  = null
	) {
		//Added to enable filtering
		if ( !string.IsNullOrEmpty( filterColumn )
		  && !string.IsNullOrEmpty( filterQuery )
		  && IsValidProperty( filterColumn ) ) {
			source = source.Where( string.Format( "{0}.startsWith(@0)", filterColumn ), filterQuery );
		}

		var count = await source.CountAsync();

		//Added to enable sorting
		if ( !string.IsNullOrEmpty( sortColumn ) && IsValidProperty( sortColumn ) ) {
			sortOrder = !string.IsNullOrEmpty( sortOrder ) && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
			source = source.OrderBy( $"{sortColumn} {sortOrder}" );
		}

		source = Queryable.Take( Queryable.Skip( source, pageIndex * pageSize ), pageSize );

		var data = await source.ToListAsync();

		return new ApiResult<T>( data,
		                         count,
		                         pageIndex,
		                         pageSize,
		                         sortColumn,
		                         sortOrder,
		                         filterColumn,
		                         filterQuery
		);
	}

	#endregion

	#region Methods

	/// <summary>
	///     Checks if the given property name exists
	///     to protect against SQL injection attacks
	/// </summary>
	public static bool IsValidProperty(string propName, bool throwExceptionIfNotFound = true) {
		var prop = typeof(T).GetProperty( propName,
		                                  BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
		);
		if ( prop == null && throwExceptionIfNotFound )
			throw new NotSupportedException(
				string.Format( $"ERROR: Property {propName} not found in type {typeof(T).Name}" )
			);
		return prop != null;
	}

	#endregion

	#region Properties

	/// <summary>
	///     The data result.
	/// </summary>
	public List<T> Data { get; }

	/// <summary>
	///     Zero-based index of current page.
	/// </summary>
	public int PageIndex { get; }

	/// <summary>
	///     Number of items contained in each page.
	/// </summary>
	public int PageSize { get; }

	/// <summary>
	///     Total items count
	/// </summary>
	public int TotalCount { get; }

	/// <summary>
	///     Total pages count
	/// </summary>
	public int TotalPages { get; }

	/// <summary>
	///     TRUE if the current page has a previous page,
	///     FALSE otherwise.
	/// </summary>
	public bool HasPreviousPage => PageIndex > 0;

	/// <summary>
	///     TRUE if the current page has a next page, FALSE otherwise.
	/// </summary>
	public bool HasNextPage => PageIndex + 1 < TotalPages;

	/// <summary>
	///     Sorting Column name (or null if none set)
	/// </summary>
	public string? SortColumn { get; set; }

	/// <summary>
	///     Sorting Order ("ASC", "DESC" or null if none set)
	/// </summary>
	public string? SortOrder { get; set; }

	/// <summary>
	///     Filter Column name (or null if none set)
	/// </summary>
	public string? FilterColumn { get; set; }

	/// <summary>
	///     Filter Query string (to be used within given filterColumn)
	/// </summary>
	public string? FilterQuery { get; set; }

	#endregion
}