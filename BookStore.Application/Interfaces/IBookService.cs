using BookStore.Application.DTOs;
using BookStore.Application.Common;

namespace BookStore.Application.Interfaces;

public interface IBookService
{
    BookDto CreateBook(BookRequest request);
    BookDto? GetById(int id);
    Task<PaginatedList<BookDto>> GetBooks(PaginatedRequest request);
    Task<bool> UpdateBook(UpdateBookRequest book);
    Task<bool> DeleteBook(int id);
}