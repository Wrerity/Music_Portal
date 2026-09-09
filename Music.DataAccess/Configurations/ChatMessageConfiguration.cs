using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Music.DataAccess.Models;

namespace Music.DataAccess.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> b)
    {
        b.Property(x => x.Content).IsRequired().HasMaxLength(2000);
        b.HasOne(x => x.Sender).WithMany(u => u.SentMessages).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Receiver).WithMany().HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Room).WithMany(r => r.Messages).HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.Timestamp);
        b.HasIndex(x => new { x.RoomId, x.Timestamp });
    }
}
