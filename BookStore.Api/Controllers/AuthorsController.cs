using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;


public class AuthorsController(IAuthorService authorService) : BaseController
{
    [HttpPost]
    public ActionResult<Author> CreateAuthor(AuthorRequest request)
    {
        return Ok(authorService.CreateAuthor(request));
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PaginatedList<AuthorDto>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<object>))]
    public async Task<IActionResult> GetAuthors([FromQuery] PaginatedRequest request)
    {
        return Ok(await authorService.GetAuthors(request));
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await authorService.DeleteAuthor(id);
        if (!result) return NotFound();
        return NoContent();
    }
}