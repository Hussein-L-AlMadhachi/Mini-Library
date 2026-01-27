using BookStore.Application.Common;
using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface ILoanService
{
    Task<LoanDto> BorrowBookAsync(int userId, LoanRequest request);
    Task<bool> ReturnBookAsync(int userId, int loanId);
    Task<PaginatedList<LoanDto>> GetUserLoansAsync(int userId, int pageIndex, int pageSize);
    Task<PaginatedList<LoanDto>> GetAllLoansAsync(int pageIndex, int pageSize);
}
