using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.API.DTOs
{
    // DTOs can enforce validations; users will interact with DTOs    
    public class BookDTO
    {
        // Prop names same as in Data class, for AutoMapper to map them
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Isbn { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public double? Price { get; set; }

        public int? AuthorId { get; set; }
        public virtual AuthorDTO Author { get; set; } // DTOs interact with DTOs
    }
}
