namespace BookStore.Application.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required AuthorDto Author { get; set; }
    public List<GenreDto> Genres { get; set; } = [];
    public string? Borrower { get; set; }
}