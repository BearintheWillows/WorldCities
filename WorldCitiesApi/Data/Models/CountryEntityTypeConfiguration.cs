using Microsoft.EntityFrameworkCore;

namespace WorldCitiesApi.Data.Models;

public class CountryEntityTypeConfiguration : IEntityTypeConfiguration<CountryModel>
{
    public void Configure( Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CountryModel> builder )
    {
        builder.ToTable( "Countries" );
        builder.HasKey( c => c.Id );
        builder.Property( c => c.Id ).IsRequired();
    }
}
