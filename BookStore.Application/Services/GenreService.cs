using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services;

public class GenreService(
    ICacheService cache, 
    IBookStoreDbContext context) : IGenreService
{
    // Constant prefix helps avoid typos
    private const string GenreCachePrefix = "genres";

    public async Task<ServiceResult<PaginatedList<GenreDto>>> GetGenres(PaginatedRequest request)
    {
        // 1. Generate a Unique Key based on the Request
        // Key looks like: "genres_p1_s10"
        var cacheKey = $"{GenreCachePrefix}_p{request.PageNumber}_s{request.PageSize}";

        // 2. Try to get from Cache (The "Happy Path")
        var cachedList = cache.Get<PaginatedList<GenreDto>>(cacheKey);
        if (cachedList != null)
        {
            return ServiceResult<PaginatedList<GenreDto>>.Success(cachedList);
        }

        // 3. Cache Miss - Fetch from Database
        // Use ProjectToType for performance (as taught in Mapster section)
        var query = context.Genres
            .AsNoTracking()
            .OrderBy(g => g.Id)
            .ProjectToType<GenreDto>();

        var paginatedResult = await PaginatedList<GenreDto>.CreateAsync(
            query, request.PageNumber, request.PageSize);

        // 4. Save to Cache
        // Expiration: Genres don't change often. 30 minutes is safe but for demo, we use few seconds.
        cache.Set(cacheKey, paginatedResult, TimeSpan.FromSeconds(30));

        return ServiceResult<PaginatedList<GenreDto>>.Success(paginatedResult);
    }

    public async Task<ServiceResult<GenreDto>> CreateGenre(GenreDto request)
    {
        var sanitizedName = request.Name.Trim();
        // 1. Validation (Check duplicate names)
        // We could check cache, but for uniqueness, ALWAYS check DB.
        var exists = await context.Genres.AnyAsync(g => g.Name == sanitizedName);
        if (exists)
        {
            return ServiceResult<GenreDto>.Failure(
                new ServiceError("Genre already exists", ServiceErrorType.Conflict));
        }
        

        // 2. Create Entity
        var genre = request.Adapt<Genre>();
        
        context.Genres.Add(genre);
        await context.SaveChangesAsync(default);

        // 3. Return Result
        var resultDto = genre.Adapt<GenreDto>();

        // 4. CACHE INVALIDATION (The Hard Part)
        // Since we added a new genre, the "List of Genres" on Page 1 might be outdated.
        // With our simple ICacheService, we can't easily say "Delete all keys starting with 'genres_'".
        // 
        // Strategy A (Simple): Do nothing. Let the cache expire in 30 mins.
        // Strategy B (Targeted): Remove the most popular page.
        cache.Remove($"{GenreCachePrefix}_p1_s10"); 
        
        // Note: For advanced systems, we would use Redis "Tags" or implement 
        // a "RemoveByPrefix" method in our ICacheService to clear all genre pages.

        return ServiceResult<GenreDto>.Success(resultDto);
    }
    public async Task<bool> DeleteGenre(int id)
    {
        var genre = await context.Genres.FindAsync(id);
        if (genre == null)
            return false;

        context.Genres.Remove(genre);
        await context.SaveChangesAsync(default);

        // Invalidate Cache
        // Simple strategy: remove the first page to ensure lists aren't totally stale.
        // A better full clearing strategy is out of scope for this specific task but recommended for future.
        cache.Remove($"{GenreCachePrefix}_p1_s10");
            
        return true;
    }
}