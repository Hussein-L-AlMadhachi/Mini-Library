using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services;

public class AuthorService(IBookStoreDbContext dbContext) : IAuthorService
{
    public Author CreateAuthor(AuthorRequest request)
    {
        var newAuthor = new Author()
        {
            Name = request.Name,
        };
        dbContext.Authors.Add(newAuthor);

        dbContext.SaveChanges();
        return newAuthor;
    }

    public Task<PaginatedList<AuthorDto>> GetAuthors(PaginatedRequest request)
    {
        var query =  dbContext.Authors
            .OrderBy(a => a.Id)
            // .Select(author => new AuthorDto()
            // {
            //     Name = author.Name,
            //     Id = author.Id
            // })
            .ProjectToType<AuthorDto>();

        var result = PaginatedList<AuthorDto>.CreateAsync(query, request.PageNumber, request.PageSize);
        return result;
        
    }

    public async Task<bool> DeleteAuthor(int id)
    {
        var author = await dbContext.Authors.FindAsync(id);
        if (author == null)
            return false;

        dbContext.Authors.Remove(author);
        await dbContext.SaveChangesAsync(default);
        return true;
    }
}