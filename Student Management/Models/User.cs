using Microsoft.AspNetCore.Identity;

namespace Student_Management.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
