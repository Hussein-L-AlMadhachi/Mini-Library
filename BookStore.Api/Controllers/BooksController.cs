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
    private const string BooksCacheKeyPrefix = "books_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedList<BookDto>>>> GetBooks([FromQuery] PaginatedRequest request)
    {
        var cacheKey = $"{BooksCacheKeyPrefix}{request.PageNumber}_{request.PageSize}";
        
        // Try to get from cache first
        var cachedBooks = cacheService.Get<PaginatedList<BookDto>>(cacheKey);
        if (cachedBooks != null)
        {
            return Ok(new ApiResponse<PaginatedList<BookDto>>(cachedBooks, "Retrieved from cache"));
        }

        var result = await bookService.GetBooks(request);
        
        // Cache the result
        cacheService.Set(cacheKey, result, CacheDuration);
        
        return Ok(new ApiResponse<PaginatedList<BookDto>>(result));
    }

    [HttpGet("{id:int}")]
    public ActionResult<ApiResponse<BookDto?>> GetBookById(int id)
    {
        var result = bookService.GetById(id);
        if (result is null)
            return NotFound(new ApiResponse<BookDto?>(new List<string> { "Book was not found" }));
        return Ok(new ApiResponse<BookDto?>(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<ApiResponse<BookDto>> CreateBook([FromBody] BookRequest request)
    {
        var result = bookService.CreateBook(request);
        
        // Invalidate all book caches
        cacheService.RemoveByPrefix(BooksCacheKeyPrefix);
        
        return Ok(new ApiResponse<BookDto>(result, "Book created successfully"));
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateBook([FromBody] UpdateBookRequest request)
    {
        var result = await bookService.UpdateBook(request);
        if (result)
        {
            // Invalidate all book caches
            cacheService.RemoveByPrefix(BooksCacheKeyPrefix);
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
        
        // Invalidate all book caches
        cacheService.RemoveByPrefix(BooksCacheKeyPrefix);
        
        return Ok(new ApiResponse<bool>(true, "Book deleted successfully"));
    }
}