using BookStore.Application.DTOs;
using FluentValidation;

namespace BookStore.Application.ValidationsAndAttributes;

public class BookRequestValidator : AbstractValidator<BookRequest>
{
    public BookRequestValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty()
            .WithMessage("Title must be provided")
            .Length(3, 20)
            .WithMessage("Title must be 3-20 chars");
    }
}