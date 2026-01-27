using BookStore.Domain.Entities.Common;

namespace BookStore.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; }
    
    public ICollection<BookGenre> Books { get; set; }
}