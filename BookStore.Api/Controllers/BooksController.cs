using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

public class BooksController(IBookService bookService, ICacheService cacheService) : BaseController
{
    private const string BooksCacheKey = "books_all";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    [HttpGet]
    public ActionResult<ApiResponse<List<BookDto>>> GetBooks()
    {
        // Try to get from cache first
        var cachedBooks = cacheService.Get<List<BookDto>>(BooksCacheKey);
        if (cachedBooks != null)
        {
            return Ok(new ApiResponse<List<BookDto>>(cachedBooks, "Retrieved from cache"));
        }

        var result = bookService.GetBooks();
        
        // Cache the result
        cacheService.Set(BooksCacheKey, result, CacheDuration);
        
        return Ok(new ApiResponse<List<BookDto>>(result));
    }

    [HttpGet("{id:int}")]
    public ActionResult<ApiResponse<Book?>> GetBookById(int id)
    {
        var result = bookService.GetById(id);
        if (result is null)
            return NotFound(new ApiResponse<Book?>(new List<string> { "Book was not found" }));
        return Ok(new ApiResponse<Book?>(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse<Book>> CreateBook([FromBody] BookRequest request)
    {
        var result = bookService.CreateBook(request);
        
        // Invalidate cache when a new book is added
        cacheService.Remove(BooksCacheKey);
        
        return Ok(new ApiResponse<Book>(result, "Book created successfully"));
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateBook([FromBody] UpdateBookRequest request)
    {
        var result = await bookService.UpdateBook(request);
        if (result)
        {
            // Invalidate cache when a book is updated
            cacheService.Remove(BooksCacheKey);
            return Ok(new ApiResponse<bool>(true, "Book updated successfully"));
        }
        return BadRequest(new ApiResponse<bool>(new List<string> { "Book was not found or data provided is invalid" }));
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteBook(int id)
    {
        var result = await bookService.DeleteBook(id);
        if (!result)
            return NotFound(new ApiResponse<bool>(new List<string> { "Book not found" }));
        
        // Invalidate cache when a book is deleted
        cacheService.Remove(BooksCacheKey);
        
        return Ok(new ApiResponse<bool>(true, "Book deleted successfully"));
    }
}