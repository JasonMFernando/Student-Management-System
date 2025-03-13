using System.ComponentModel.DataAnnotations;
using System;

namespace AMDT_UserManagement.Models
{
    public class Role
    {
        [Required]
        public int RoleID { get; set; }

        [Required]
        public string RoleName { get; set; }


        public Status Status { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}
