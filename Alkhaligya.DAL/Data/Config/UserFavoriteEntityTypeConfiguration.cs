using Alkhaligya.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alkhaligya.DAL.Data.Config
{
    public class UserFavoriteEntityTypeConfiguration : IEntityTypeConfiguration<UserFavorite>
    {
        public void Configure(EntityTypeBuilder<UserFavorite> builder)
        {
            builder.ToTable("UserFavorites");

            builder.HasKey(uf => uf.Id);

            builder.HasOne(uf => uf.User)
                .WithMany(u => u.UserFavorites)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uf => uf.Product)
                .WithMany(p => p.UserFavorites)
                .HasForeignKey(uf => uf.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(uf => uf.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(uf => uf.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}