namespace WorldCitiesApi.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CountryEntityTypeConfiguration : IEntityTypeConfiguration<Country> {
	public void Configure(EntityTypeBuilder<Country> builder) {
		builder.ToTable( "Countries" );
		builder.HasKey( c => c.Id );
		builder.Property( c => c.Id ).IsRequired();
	}
}