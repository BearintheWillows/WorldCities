namespace WorldCitiesApi.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table( "Cities" ), Index( nameof(Name) ), Index( nameof(ISO2) ), Index( nameof(ISO3) )]
public class Country {
	#region Navigation Properties

    /// <summary>
    ///     A collection of all cities related to this
    ///     country
    /// </summary>

    public ICollection<City>? Cities { get; set; } = null;

	#endregion

	#region Properties

    /// <summary>
    ///     The unique id and primary key for this country
    /// </summary

    [Key, Required]
	public int Id { get; set; }

    /// <summary>
    ///     Country Name (in UTF8 format)
    /// </summary>

    public string Name { get; set; } = null;

    /// <summary>
    ///     Country Code (in ISO 3166-1 ALPHA-2 format
    /// </summary

    public string ISO2 { get; set; } = null;

    /// <summary>
    ///     Country Code (in ISO 2166-1 ALPHA-3 format)
    /// </summary>

    public string ISO3 { get; set; } = null;

	#endregion
}