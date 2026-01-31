using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistance.EntitiesConfigurations;

public class LoansConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.Id);

        // Usage of IsRequired(false) suppresses the warning about Global Query Filters 
        // on the related 'Book' and 'User' entities, which are SoftDeletable.
        // It tells EF that the navigation property can be null (e.g. if the related entity is filtered out),
        // even though the Foreign Key (BookId/UserId) is strictly required in the DB (int).
        
        builder.HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .IsRequired(false); 

        builder.HasOne(l => l.User)
            .WithMany(u => u.Loans)
            .HasForeignKey(l => l.UserId)
            .IsRequired(false);
    }
}
