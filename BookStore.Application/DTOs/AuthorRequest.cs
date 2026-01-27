using System.ComponentModel.DataAnnotations;

namespace BookStore.Application.DTOs;

public class AuthorRequest
{
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
    public string Name { get; set; }
}