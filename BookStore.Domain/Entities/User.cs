using System.ComponentModel.DataAnnotations;
using BookStore.Domain.Entities.Common;
using BookStore.Domain.Enums;
using BookStore.Domain.Interfaces;

namespace BookStore.Domain.Entities;

public class User : BaseEntity, IAuditable, ISoftDeletable
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    [MaxLength(45)]
    public string FirstName { get; set; } = string.Empty;
    
    [MaxLength(45)]
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    
    // Navigation property for loans
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    
    // IAuditable
    public int? UpdatedBy { get; set; }
    public string? Changes { get; set; }
    
    // ISoftDeletable
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}