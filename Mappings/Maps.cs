using AutoMapper;
using BookStore.API.Data;
using BookStore.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.API.Mappings
{
    // Configuration class
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<Author, AuthorDTO>().ReverseMap(); // mapping in both directions
            CreateMap<Book, BookDTO>().ReverseMap();
        }
        
    }
}
