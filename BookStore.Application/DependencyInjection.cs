using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILoanService, LoanService>();
        return services;
    }
}