namespace WorldCitiesApi.Controllers;

using Data;
using Data.DTOs;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;

[Route( "api/[controller]" ), ApiController]
public class CountriesController : ControllerBase {
	private readonly ApplicationDbContext _context;

	public CountriesController(ApplicationDbContext context) {
		_context = context;
	}

	// GET: api/Countries
	[HttpGet]
	public async Task<ActionResult<ApiResult<CountryDTO>>> GetCountries(
		int     pageIndex    = 0,
		int     pageSize     = 10,
		string? sortColumn   = null,
		string? sortOrder    = null,
		string? filterColumn = null,
		string? filterQuery  = null
	) => await ApiResult<CountryDTO>.CreateAsync( _context.Countries.AsNoTracking()
	                                                      .Select( c => new CountryDTO {
			                                                       Id = c.Id,
			                                                       Name = c.Name,
			                                                       ISO2 = c.ISO2,
			                                                       ISO3 = c.ISO3,
			                                                       TotalCities = c.Cities!.Count,
		                                                       }
	                                                       ),
	                                              pageIndex,
	                                              pageSize,
	                                              sortColumn,
	                                              sortOrder,
	                                              filterColumn,
	                                              filterQuery
	);

	// GET: api/Countries/5
	[HttpGet( "{id}" )]
	public async Task<ActionResult<Country>> GetCountry(int id) {
		if ( _context.Countries == null ) return NotFound();
		var country = await _context.Countries.FindAsync( id );

		if ( country == null ) return NotFound();

		return country;
	}

	// PUT: api/Countries/5
	// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[HttpPut( "{id}" )]
	public async Task<IActionResult> PutCountry(int id, Country country) {
		if ( id != country.Id ) return BadRequest();

		_context.Entry( country ).State = EntityState.Modified;

		try {
			await _context.SaveChangesAsync();
		}
		catch ( DbUpdateConcurrencyException ) {
			if ( !CountryExists( id ) ) return NotFound();
			throw;
		}

		return NoContent();
	}

	// POST: api/Countries
	// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[HttpPost]
	public async Task<ActionResult<Country>> PostCountry(Country country) {
		if ( _context.Countries == null ) return Problem( "Entity set 'ApplicationDbContext.Countries'  is null." );
		_context.Countries.Add( country );
		await _context.SaveChangesAsync();

		return CreatedAtAction( "GetCountry", new { id = country.Id }, country );
	}

	// DELETE: api/Countries/5
	[HttpDelete( "{id}" )]
	public async Task<IActionResult> DeleteCountry(int id) {
		if ( _context.Countries == null ) return NotFound();
		var country = await _context.Countries.FindAsync( id );
		if ( country == null ) return NotFound();

		_context.Countries.Remove( country );
		await _context.SaveChangesAsync();

		return NoContent();
	}

	private bool CountryExists(int id) => ( _context.Countries?.Any( e => e.Id == id ) ).GetValueOrDefault();


	[HttpPost, Route( "isDupeField" )]
	public bool IsDupeField(
		int    countryId,
		string fieldName,
		string fieldValue
	) {
		switch ( fieldName ) {
		case "name":
			return _context.Countries.Any( c => c.Id != countryId && c.Name == fieldValue );
		case "iso2":
			return _context.Countries.Any( c => c.ISO2 == fieldValue &&
			                                    c.Id != countryId
			);
		case "iso3":
			return _context.Countries.Any( c => c.ISO3 == fieldValue && c.Id != countryId );
		default:
			return false;
		}
	}
}