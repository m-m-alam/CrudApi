using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudApi.Models
{
    public class UserModel
    {
        
        public string FullName { get; set; }
        
        public string Email { get; set; }
        
        public string UserName { get; set; }
        
        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }
    }
}
