using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistance.EntitiesConfigurations;

public class BookGenresConfiguration : IEntityTypeConfiguration<BookGenre>
{
    public void Configure(EntityTypeBuilder<BookGenre> builder)
    {
        builder.HasKey(bg => new { bg.BookId, bg.GenreId });
        builder.HasOne(bg => bg.Book)
            .WithMany(b => b.Genres)
            .HasForeignKey(bg => bg.BookId);
        
        builder.HasOne(bg => bg.Genre)
            .WithMany(b => b.Books)
            .HasForeignKey(bg => bg.GenreId);
    }
}