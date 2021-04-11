using System.ComponentModel.DataAnnotations;

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
        public string Image { get; set; } // Path to Image (on say CDN)
        public decimal? Price { get; set; } // 'money' type in SQL === decimal in c#

        public int? AuthorId { get; set; }
        public virtual AuthorDTO Author { get; set; } // DTOs interact with DTOs
    }

    public class BookCreateDTO
    {
        [Required]
        public string Title { get; set; }
        public int? Year { get; set; }
        [Required]
        public string Isbn { get; set; }
        [StringLength(500)]
        public string Summary { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        [Required]
        public int AuthorId { get; set; }
    }

    public class BookUpdateDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int? Year { get; set; }        
        [StringLength(500)]
        public string Summary { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        [Required]
        public int AuthorId { get; set; }
    }

}
