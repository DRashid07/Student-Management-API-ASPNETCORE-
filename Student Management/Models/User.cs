using Microsoft.AspNetCore.Identity;

namespace Student_Management.Models
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
