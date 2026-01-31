using BookStore.Application.Common;
using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface IGenreService
{
    Task<ServiceResult<PaginatedList<GenreDto>>> GetGenres(PaginatedRequest request);
    Task<ServiceResult<GenreDto>> CreateGenre(GenreDto request);
    Task<bool> DeleteGenre(int id);
}