namespace BookStore.Application.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public AuthorDto Author { get; set; }
    
}