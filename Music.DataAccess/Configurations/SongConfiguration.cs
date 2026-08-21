using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.DataAccess.Models;

namespace Music.DataAccess.Configurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.HasMany(s => s.Genres)
            .WithMany(g => g.Songs)
            .UsingEntity<Dictionary<string, object>>(
                "SongGenres",
                j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
                j => j.HasOne<Song>().WithMany().HasForeignKey("SongId"),
                j => j.HasKey("SongId", "GenreId"));

        builder.HasMany(s => s.Authors)
            .WithMany(a => a.Songs)
            .UsingEntity<Dictionary<string, object>>(
                "SongAuthors",
                j => j.HasOne<Author>().WithMany().HasForeignKey("AuthorId"),
                j => j.HasOne<Song>().WithMany().HasForeignKey("SongId"),
                j => j.HasKey("SongId", "AuthorId"));
    }
}