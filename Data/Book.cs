using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.API.Data
{
    [Table("Books")]
    public partial class Book
    {
        // An alternative approach to manually creating classes to match the tables in an existing DB,
        // Run the following command in the Package Manager Console in VS:
        // Scaffold-DbContext [-Connection] [-Provider] [-OutputDir] [-Context] [-Schemas>] [-Tables>] 
        // [-DataAnnotations] [-Force] [-Project] [-StartupProject] [<CommonParameters>]
        // Scaffold-DbContext "Server=localhost;Database=BookStore;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Data
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Isbn { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public double? Price { get; set; }

        public int? AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }
}