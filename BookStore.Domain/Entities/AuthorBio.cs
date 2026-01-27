using BookStore.Domain.Entities.Common;

namespace BookStore.Domain.Entities;

public class AuthorBio : BaseEntity
{
    public string Biography { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}