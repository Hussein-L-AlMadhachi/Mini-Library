using BookStore.Application.DTOs;
using BookStore.Domain.Entities;
using Mapster;

namespace BookStore.Application.Mappers;

public class GenreMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Genre, GenreDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
        
        config.NewConfig<GenreDto, Genre>()
            .Map(dest => dest.Name, src => src.Name)
            .Ignore(dest => dest.Books)
            .Ignore(dest => dest.Id);
    }
}