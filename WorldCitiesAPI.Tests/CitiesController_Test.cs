using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WorldCitiesApi.Data.Models;
using WorldCitiesApi.Data;
using WorldCitiesApi.Controllers;
using Xunit;

namespace WorldCitiesAPI.Tests;

	public class CitiesControllerTests
	{
		/// <summary>
		/// Test the GetCity() method
		/// </summary>
		[Fact]
		public async Task GetCity()
		{
			// Arrange
			
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
			   .UseInMemoryDatabase(databaseName: "WorldCities")
			             .Options;
			
			using var context = new ApplicationDbContext(options);

			context.Add( new City {
				             Id = 1,
				             CountryId = 1,
				             Lat = 1,
				             Lon = 1,
				             Name = "TestCity"
			             }
			);
			context.SaveChanges();
			
			var controller = new CitiesController(context);
			City? cityExisting = null;
			City? cityNonExisting = null;
			


			// Act
			cityExisting = (await controller.GetCity(1)).Value;
			cityNonExisting = (await controller.GetCity(2)).Value;
			// Assert
			Assert.NotNull( cityExisting );
			Assert.Null( cityNonExisting );
		}
	}