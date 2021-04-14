using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs
{
    public class UserDTO
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "Password is limited to {2} to {1} characters", MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class UserViewDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
