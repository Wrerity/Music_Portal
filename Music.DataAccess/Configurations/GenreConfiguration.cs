using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.DataAccess.Models;

namespace Music.DataAccess.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        // 6. Уникальность имени жанра
        builder.HasIndex(g => g.Name).IsUnique();
        builder.Property(g => g.Name).IsRequired().HasMaxLength(100);
    }
}
