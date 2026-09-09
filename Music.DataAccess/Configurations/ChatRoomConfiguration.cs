using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.DataAccess.Models;

namespace Music.DataAccess.Configurations;

public class ChatRoomConfiguration : IEntityTypeConfiguration<ChatRoom>
{
    public void Configure(EntityTypeBuilder<ChatRoom> b)
    {
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.HasData(new ChatRoom { Id = 1, Name = "General", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
    }
}
