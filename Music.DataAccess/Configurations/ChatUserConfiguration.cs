using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.DataAccess.Models;

namespace Music.DataAccess.Configurations;

public class ChatUserConfiguration : IEntityTypeConfiguration<ChatUser>
{
    public void Configure(EntityTypeBuilder<ChatUser> b)
    {
        b.HasIndex(x => x.Username).IsUnique();
        b.HasIndex(x => x.ConnectionId);
        b.Property(x => x.Username).IsRequired().HasMaxLength(50);
    }
}
