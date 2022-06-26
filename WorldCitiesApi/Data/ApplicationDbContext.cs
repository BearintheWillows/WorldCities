namespace WorldCitiesApi.Data;

using Microsoft.EntityFrameworkCore;
using Models;

public class ApplicationDbContext : DbContext {
	public ApplicationDbContext() { }

	public ApplicationDbContext(DbContextOptions options)
		: base( options ) { }


	public DbSet<City>    Cities    => Set<City>();
	public DbSet<Country> Countries => Set<Country>();

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating( modelBuilder );

		modelBuilder.ApplyConfigurationsFromAssembly( typeof(ApplicationDbContext).Assembly );

		/* modelBuilder.Entity<City>()
		     .ToTable( "Cities" );

		 modelBuilder.Entity<City>()
		     .HasKey( c => c.Id );

		 modelBuilder.Entity<City>().Property( p => p.Id ).IsRequired();

		 modelBuilder.Entity<City>()
		     .Property( p => p.Lat )
		     .HasColumnType( "decimal(7,4)" );

		 modelBuilder.Entity<City>()
		     .Property( p => p.Lon )
		     .HasColumnType( "decimal(7, 4" );

		 modelBuilder.Entity<Country>()
		     .ToTable( "Countries" );

		 modelBuilder.Entity<Country>()
		     .HasKey( c => c.Id );

		 modelBuilder.Entity<Country>()
		     .Property( p => p.Id ).IsRequired();

		 modelBuilder.Entity<City>()
		     .HasOne( c => c.Country )
		     .WithMany( y => y.Cities )
		     .HasForeignKey( x => x.Id );*/
	}
}