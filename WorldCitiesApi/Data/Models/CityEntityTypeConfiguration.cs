using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorldCitiesApi.Data.Models;

public class CityEntityTypeConfiguration : IEntityTypeConfiguration<CityModel>
{
    public void Configure( EntityTypeBuilder<CityModel> builder )
    {
        builder.ToTable( "Cities" );
        builder.HasKey( c => c.Id );
        builder.Property( c => c.Id ).IsRequired();
        builder
            .HasOne( c => c.Country )
            .WithMany( c => c.Cities )
            .HasForeignKey( c => c.CountryId );
        builder.Property( p => p.Lat ).HasColumnType( "decimal(7,4)" );
        builder.Property( p => p.Lon ).HasColumnType( "decimal(7,4)" );

    }
}
