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
        var test = query.Single();
        var result = PaginatedList<AuthorDto>.CreateAsync(query, request.PageNumber, request.PageSize);
        return result;
        
    }
}