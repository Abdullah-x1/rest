using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSAR.Models
{
    public class User : IdentityUser
    {

        public string FullName { get; set; }
        public int UserId { get; set; }

    }
    
}
