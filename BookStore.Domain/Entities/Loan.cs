using BookStore.Domain.Entities.Common;
using BookStore.Domain.Interfaces;

namespace BookStore.Domain.Entities;

public class Loan : BaseEntity, IAuditable
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    
    public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
    public DateTime? ReturnDate { get; set; }
    
    // IAuditable
    public int? UpdatedBy { get; set; }
    public string? Changes { get; set; }
}
