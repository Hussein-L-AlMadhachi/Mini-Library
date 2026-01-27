using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookStore.Api.Common;
using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[Route("api/loans")]
[Authorize] // All loan endpoints require authentication
public class LoansController(ILoanService loanService) : BaseController
{
    /// <summary>
    /// Borrow a book - UserId is extracted from JWT token, not from request body
    /// </summary>
    [HttpPost("borrow")]
    public async Task<ActionResult<ApiResponse<LoanDto>>> BorrowBook([FromBody] LoanRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new ApiResponse<LoanDto>(new List<string> { "User not authenticated" }));

            var result = await loanService.BorrowBookAsync(userId.Value, request);
            return Ok(new ApiResponse<LoanDto>(result, "Book borrowed successfully"));
        }
        catch (Exception e)
        {
            return BadRequest(new ApiResponse<LoanDto>(new List<string> { e.Message }));
        }
    }

    /// <summary>
    /// Return a borrowed book
    /// </summary>
    [HttpPost("return/{loanId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> ReturnBook(int loanId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<bool>(new List<string> { "User not authenticated" }));

        var result = await loanService.ReturnBookAsync(userId.Value, loanId);
        if (!result)
            return BadRequest(new ApiResponse<bool>(new List<string> { "Loan not found or already returned" }));

        return Ok(new ApiResponse<bool>(true, "Book returned successfully"));
    }

    /// <summary>
    /// Get current user's loans (paginated)
    /// </summary>
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<PaginatedList<LoanDto>>>> GetMyLoans(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse<PaginatedList<LoanDto>>(new List<string> { "User not authenticated" }));

        var result = await loanService.GetUserLoansAsync(userId.Value, pageIndex, pageSize);
        return Ok(new ApiResponse<PaginatedList<LoanDto>>(result));
    }

    /// <summary>
    /// Get all loans (Admin only, paginated)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PaginatedList<LoanDto>>>> GetAllLoans(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await loanService.GetAllLoansAsync(pageIndex, pageSize);
        return Ok(new ApiResponse<PaginatedList<LoanDto>>(result));
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub) 
            ?? User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            return userId;
        
        return null;
    }
}
