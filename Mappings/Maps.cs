using AutoMapper;
using BookStore.API.Data;
using BookStore.API.DTOs;
using Microsoft.AspNetCore.Identity;

namespace BookStore.API.Mappings
{
    // Configuration class
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap(); // mapping in both directions
            CreateMap<Author, AuthorCreateDTO>().ReverseMap();
            CreateMap<Author, AuthorUpdateDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();
            CreateMap<Book, BookCreateDTO>().ReverseMap();
            CreateMap<Book, BookUpdateDTO>().ReverseMap();
            CreateMap<IdentityUser, UserViewDTO>();
        }
        
    }
}
