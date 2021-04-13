using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.API.DTOs
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserViewDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
