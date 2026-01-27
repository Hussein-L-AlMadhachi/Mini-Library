using System.ComponentModel.DataAnnotations;
using BookStore.Application.ValidationsAndAttributes;

namespace BookStore.Application.DTOs;

public class BookRequest
{
    public string Title { get; set; }
    [Required]
    public int? AuthorId { get; set; }
}