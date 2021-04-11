using AutoMapper;
using BookStore.API.Data;
using BookStore.API.DTOs;

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
        }
        
    }
}
