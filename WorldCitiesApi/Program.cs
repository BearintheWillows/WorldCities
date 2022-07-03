using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using WorldCitiesApi.Data;

var builder = WebApplication.CreateBuilder( args );

//Adds serilog support
builder.Host.UseSerilog( (ctx, lc) => lc.ReadFrom.Configuration( ctx.Configuration )
                                        .WriteTo.MSSqlServer( connectionString:
                                                              ctx.Configuration.GetConnectionString( "DefaultConnection"
                                                              ),
                                                              restrictedToMinimumLevel: LogEventLevel.Information,
                                                              sinkOptions: new MSSqlServerSinkOptions {
                                                                  TableName = "LogEvents", AutoCreateSqlTable = true
                                                              }
                                         )
                                        .WriteTo.Console()
);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.

builder.Services.AddControllers();
//.AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add ApplicationDbContext and SQL server support
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer( builder.Configuration.GetConnectionString( "DefaultConnection" ) )
);

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();