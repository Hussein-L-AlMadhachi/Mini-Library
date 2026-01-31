using BookStore.Application.DTOs;
using BookStore.Application.Common;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services;

public class BookService(IBookStoreDbContext dbContext) : IBookService
{
    public BookDto CreateBook(BookRequest request)
    {
        var newBook = new Book()
        {
            Title = request.Title,
            AuthorId = request.AuthorId.Value
        };
        
        if (request.GenreIds.Count != 0)
        {
            var genres = dbContext.Genres.Where(g => request.GenreIds.Contains(g.Id)).ToList();
            if (genres.Count != request.GenreIds.Count)
                throw new Exception("One or more genres not found");
                
            foreach (var genre in genres)
            {
                newBook.Genres.Add(new BookGenre
                {
                    Book = newBook,
                    GenreId = genre.Id
                });
            }
        }
        
        dbContext.Books.Add(newBook);
        dbContext.SaveChanges();

        // Fetch the complete book with relationships to map to DTO
        var createdBook = dbContext.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.Genres)
                .ThenInclude(bg => bg.Genre)
            .ProjectToType<BookDto>()
            .First(b => b.Id == newBook.Id);

        return createdBook;
    }
    
    public async Task<PaginatedList<BookDto>> GetBooks(PaginatedRequest request)
    {
        var query = dbContext.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.Genres)
                .ThenInclude(bg => bg.Genre)
            .Include(b => b.Loans)
                .ThenInclude(l => l.User)
            .ProjectToType<BookDto>();

        return await PaginatedList<BookDto>.CreateAsync(query, request.PageNumber, request.PageSize);
    }

    public async Task<bool> UpdateBook(UpdateBookRequest request)
    {
        var bookToUpdate = dbContext.Books.FirstOrDefault(b => b.Id == request.Id);
        if (bookToUpdate is null)
            return false;
        if (!string.IsNullOrWhiteSpace(request.Title) && bookToUpdate.Title.Trim() != request.Title.Trim())
            bookToUpdate.Title = request.Title.Trim();
        if (request.AuthorId.HasValue && bookToUpdate.AuthorId != request.AuthorId.Value)
        {
            var author = dbContext.Authors.Any(a => a.Id == request.AuthorId.Value);
            if (!author)
                return false;
            bookToUpdate.AuthorId = request.AuthorId.Value;
        }

        if (request.GenreIds != null)
        {
             var genres = dbContext.Genres.Where(g => request.GenreIds.Contains(g.Id)).ToList();
             if (genres.Count != request.GenreIds.Count)
                 throw new Exception("One or more genres not found");
             
             // Remove existing genres
             var existingGenres = dbContext.BookGenres.Where(bg => bg.BookId == bookToUpdate.Id).ToList();
             dbContext.BookGenres.RemoveRange(existingGenres);
             
             // Add new genres
             foreach (var genre in genres)
             {
                 dbContext.BookGenres.Add(new BookGenre
                 {
                     BookId = bookToUpdate.Id,
                     GenreId = genre.Id
                 });
             }
        }

        await dbContext.SaveChangesAsync(CancellationToken.None);
        return true;
    }

    public BookDto? GetById(int id)
    {
        var book = dbContext.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .Include(b => b.Genres)
            .Include(b => b.Loans)
                .ThenInclude(l => l.User)
            .ProjectToType<BookDto>()
            .FirstOrDefault(b => b.Id == id);
            
        return book;
    }


    public async Task<bool> DeleteBook(int id)
    {
        var book = dbContext.Books.FirstOrDefault(b => b.Id == id);
        if (book is null)
            return false;
        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        return true;
    }
}