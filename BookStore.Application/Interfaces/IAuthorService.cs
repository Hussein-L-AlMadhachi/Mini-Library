using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces;

public interface IAuthorService
{
    Author CreateAuthor(AuthorRequest request);
    Task<PaginatedList<AuthorDto>> GetAuthors(PaginatedRequest request);
    Task<bool> DeleteAuthor(int id);
}