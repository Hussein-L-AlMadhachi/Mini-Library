using BookStore.Domain.Entities.Common;

namespace BookStore.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
    public AuthorBio AuthorBio { get; set; }
}