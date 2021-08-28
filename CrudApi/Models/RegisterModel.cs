using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrudApi.Models
{
    public class RegisterModel
    {
        [Required] 
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
        
        public string Gender { get; set; }
         
        public DateTime DateOfBirth { get; set; }
        
        public string Address { get; set; }
    }
}
