using System.ComponentModel.DataAnnotations;
using System;

namespace AMDT_UserManagement.Models
{
    public class StatusDto
    {
        [Required]
        public string StatusName { get; set; }



        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}
