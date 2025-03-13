using System.ComponentModel.DataAnnotations;
using System;

namespace AMDT_UserManagement.Models
{
    public class RoleDto
    {

        [Required]
        public string RoleName { get; set; }

        [Required]
        public int Status { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}
