using BookStore.Api.ActionFilters;
using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

public class GenresController(IGenreService genreService) : BaseController
{
    [HttpGet]
    [ServiceFilter(typeof(LogActivityAttribute))]
    public async Task<IActionResult> Get([FromQuery]PaginatedRequest request)
    {
        return HandleResult(await genreService.GetGenres(request));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GenreDto request)
    {
        return HandleResult(await genreService.CreateGenre(request));
    }
}