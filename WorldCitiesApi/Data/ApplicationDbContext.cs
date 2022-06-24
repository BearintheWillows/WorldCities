using Microsoft.EntityFrameworkCore;
using WorldCitiesApi.Data.Models;

namespace WorldCitiesApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext() : base()
    {

    }

    public ApplicationDbContext( DbContextOptions options )
        : base( options )
    {

    }



    public DbSet<CityModel> Cities => Set<CityModel>();
    public DbSet<CountryModel> Countries => Set<CountryModel>();

    protected override void OnModelCreating( ModelBuilder modelBuilder )
    {
        base.OnModelCreating( modelBuilder );

        modelBuilder.ApplyConfigurationsFromAssembly( typeof( ApplicationDbContext ).Assembly );

        /* modelBuilder.Entity<CityModel>()
             .ToTable( "Cities" );

         modelBuilder.Entity<CityModel>()
             .HasKey( c => c.Id );

         modelBuilder.Entity<CityModel>().Property( p => p.Id ).IsRequired();

         modelBuilder.Entity<CityModel>()
             .Property( p => p.Lat )
             .HasColumnType( "decimal(7,4)" );

         modelBuilder.Entity<CityModel>()
             .Property( p => p.Lon )
             .HasColumnType( "decimal(7, 4" );

         modelBuilder.Entity<CountryModel>()
             .ToTable( "Countries" );

         modelBuilder.Entity<CountryModel>()
             .HasKey( c => c.Id );

         modelBuilder.Entity<CountryModel>()
             .Property( p => p.Id ).IsRequired();

         modelBuilder.Entity<CityModel>()
             .HasOne( c => c.Country )
             .WithMany( y => y.Cities )
             .HasForeignKey( x => x.Id );*/

    }
}