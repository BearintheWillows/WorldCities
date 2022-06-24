namespace WorldCitiesApi.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Countries")]
[Index( nameof( Name ))]
[Index( nameof( Lat ) )]
[Index( nameof( Lon ) )]
public class CityModel
{
    #region Properties

    /// <summary>
    /// The unique id and primary key for this city
    /// </summary>	

    [Key]
    [Required]
    public int Id { get; set; }

    /// <summary>
    /// City name (in UTF8 format)
    /// </summary>

    public string Name { get; set; }

    /// <summary>
    /// City Latitude
    /// </summary>

    [Column( TypeName = "decimal(7,4" )]
    public decimal Lat { get; set; }

    /// <summary>
    /// City Longitude
    /// </summary>

    [Column( TypeName = "decimal(7,4" )]
    public decimal Lon { get; set; }

    ///<summary>
    /// Country ID
    /// </summary

    [ForeignKey( nameof( Country ) )]
    public int CountryId { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// The Country related to this city.
    /// </summary>

    public CountryModel? Country { get; set; } = null;

    #endregion


}