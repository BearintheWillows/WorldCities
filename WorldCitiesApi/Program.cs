using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WorldCitiesApi.Data;

var builder = WebApplication.CreateBuilder( args );

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add ApplicationDbContext and SQL server support
builder.Services.AddDbContext<ApplicationDbContext>(
	options => options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() ) {
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();