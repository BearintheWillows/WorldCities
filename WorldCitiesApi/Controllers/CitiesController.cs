namespace WorldCitiesApi.Controllers;

using Data;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.DTOs;

[Route( "api/[controller]" ), ApiController]
public class CitiesController : ControllerBase {
	private readonly ApplicationDbContext _context;

	public CitiesController(ApplicationDbContext context) {
		_context = context;
	}


	// GET: api/Cities
	// GET: api/Cities/?pageIndex=0&pageSize=10
	// GET: api/Cities/?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc
	[HttpGet]
	public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(
		int     pageIndex    = 0,
		int     pageSize     = 10,
		string? sortColumn   = null,
		string? sortOrder    = null,
		string? filterColumn = null,
		string? filterQuery  = null
	) => await ApiResult<CityDTO>.CreateAsync( _context.Cities.AsNoTracking()
	                                                   .Select( c => new CityDTO {
			                                                    Id = c.Id,
			                                                    Name = c.Name,
			                                                    Lat = c.Lat,
			                                                    Lon = c.Lon,
			                                                    CountryId = c.Country!.Id,
			                                                    CountryName = c.Country!.Name,
		                                                    }
	                                                    ),
	                                           pageIndex,
	                                           pageSize,
	                                           sortColumn,
	                                           sortOrder,
	                                           filterColumn,
	                                           filterQuery
	);

// GET: api/Cities/5
[HttpGet( "{id}" )]
public async Task<ActionResult<City>> GetCity(int id) {
	if ( _context.Cities == null ) return NotFound();
	var city = await _context.Cities.FindAsync( id );

	if ( city == null ) return NotFound();

	return city;
}

// PUT: api/Cities/5
// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
[HttpPut( "{id}" )]
public async Task<IActionResult> PutCity(int id, City city) {
	if ( id != city.Id ) return BadRequest();

	_context.Entry( city ).State = EntityState.Modified;

	try {
		await _context.SaveChangesAsync();
	}
	catch ( DbUpdateConcurrencyException ) {
		if ( !CityExists( id ) ) return NotFound();
		throw;
	}

	return NoContent();
}

// POST: api/Cities
// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
[HttpPost]
public async Task<ActionResult<City>> PostCity(City city) {
	if ( _context.Cities == null ) return Problem( "Entity set 'ApplicationDbContext.Cities'  is null." );
	_context.Cities.Add( city );
	await _context.SaveChangesAsync();

	return CreatedAtAction( "GetCity", new { id = city.Id }, city );
}

// DELETE: api/Cities/5
[HttpDelete( "{id}" )]
public async Task<IActionResult> DeleteCity(int id) {
	if ( _context.Cities == null ) return NotFound();
	var city = await _context.Cities.FindAsync( id );
	if ( city == null ) return NotFound();

	_context.Cities.Remove( city );
	await _context.SaveChangesAsync();

	return NoContent();
}

private bool CityExists(int id) => ( _context.Cities?.Any( e => e.Id == id ) ).GetValueOrDefault();

[HttpPost, Route( "IsDupeCity" )]
public bool IsDupeCity(City city) => _context.Cities.Any( c => c.Name == city.Name
                                                            && c.Lat == city.Lat
                                                            && c.Lon == city.Lon
                                                            && c.CountryId == city.CountryId
                                                            && c.Id != city.Id
);
}