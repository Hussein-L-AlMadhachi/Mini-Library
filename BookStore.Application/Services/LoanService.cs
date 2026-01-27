using BookStore.Application.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Application.Services;

public class LoanService(IBookStoreDbContext dbContext) : ILoanService
{
    public async Task<LoanDto> BorrowBookAsync(int userId, LoanRequest request)
    {
        // Verify book exists and is not deleted
        var book = await dbContext.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BookId);
        
        if (book == null)
            throw new Exception("Book not found");
        
        // Verify user exists
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null)
            throw new Exception("User not found");
        
        // Check if book is already borrowed (not returned)
        var existingLoan = await dbContext.Loans
            .FirstOrDefaultAsync(l => l.BookId == request.BookId && l.ReturnDate == null);
        
        if (existingLoan != null)
            throw new Exception("Book is already borrowed");
        
        // Create new loan
        var loan = new Loan
        {
            UserId = userId,
            BookId = request.BookId,
            BorrowDate = DateTime.UtcNow
        };
        
        dbContext.Loans.Add(loan);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        
        return new LoanDto
        {
            Id = loan.Id,
            UserId = userId,
            UserName = user.Name,
            BookId = book.Id,
            BookTitle = book.Title,
            BorrowDate = loan.BorrowDate,
            ReturnDate = null
        };
    }

    public async Task<bool> ReturnBookAsync(int userId, int loanId)
    {
        var loan = await dbContext.Loans
            .FirstOrDefaultAsync(l => l.Id == loanId && l.UserId == userId);
        
        if (loan == null)
            return false;
        
        if (loan.ReturnDate.HasValue)
            return false; // Already returned
        
        loan.ReturnDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(CancellationToken.None);
        
        return true;
    }

    public async Task<PaginatedList<LoanDto>> GetUserLoansAsync(int userId, int pageIndex, int pageSize)
    {
        var query = dbContext.Loans
            .AsNoTracking()
            .Include(l => l.User)
            .Include(l => l.Book)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.BorrowDate)
            .Select(l => new LoanDto
            {
                Id = l.Id,
                UserId = l.UserId,
                UserName = l.User.Name,
                BookId = l.BookId,
                BookTitle = l.Book.Title,
                BorrowDate = l.BorrowDate,
                ReturnDate = l.ReturnDate
            });
        
        return await PaginatedList<LoanDto>.CreateAsync(query, pageIndex, pageSize);
    }

    public async Task<PaginatedList<LoanDto>> GetAllLoansAsync(int pageIndex, int pageSize)
    {
        var query = dbContext.Loans
            .AsNoTracking()
            .Include(l => l.User)
            .Include(l => l.Book)
            .OrderByDescending(l => l.BorrowDate)
            .Select(l => new LoanDto
            {
                Id = l.Id,
                UserId = l.UserId,
                UserName = l.User.Name,
                BookId = l.BookId,
                BookTitle = l.Book.Title,
                BorrowDate = l.BorrowDate,
                ReturnDate = l.ReturnDate
            });
        
        return await PaginatedList<LoanDto>.CreateAsync(query, pageIndex, pageSize);
    }
}
