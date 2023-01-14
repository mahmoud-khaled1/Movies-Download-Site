using Microsoft.AspNetCore.Identity;

namespace Cinema_Download_Site.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string Country { get; set; }
    }
}
