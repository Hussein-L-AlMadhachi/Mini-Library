using System.Reflection;
using BookStore.Application.DTOs;
using BookStore.Application.Mappers;
using BookStore.Domain.Entities;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure;

public static class DependencyInjection
{
    public static void RegisterMapsterConfiguration(this IServiceCollection services)
    {
        // Global Settings
        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

        // Specific Rule: Map Author.Name -> BookDto.AuthorName
        TypeAdapterConfig<Book, BookDto>
            .NewConfig()
            .Map(dest => dest.Author.Name, src => src.Author.Name);
        
        

        // Scan for other configs if you split them classes
        TypeAdapterConfig.GlobalSettings.Scan(typeof(AuthorMappingConfig).Assembly);
    }
}