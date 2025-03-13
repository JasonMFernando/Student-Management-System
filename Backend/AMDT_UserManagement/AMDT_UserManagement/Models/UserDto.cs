using System.ComponentModel.DataAnnotations;
using System;

namespace AMDT_UserManagement.Models
{
    public class UserDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int RoleType { get; set; }

        [Required]
        public int Status { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}
