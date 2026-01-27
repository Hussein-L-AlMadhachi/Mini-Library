using BookStore.Application.DTOs;
using BookStore.Domain.Entities;
using Mapster;

namespace BookStore.Application.Mappers;

public class AuthorMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Author, AuthorDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name);
    }
}