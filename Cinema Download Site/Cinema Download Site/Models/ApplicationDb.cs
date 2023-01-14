using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Download_Site.Models
{
    public class ApplicationDb : IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public ApplicationDb(DbContextOptions<ApplicationDb>options )
            :base(options){}

        // if i want to rename the tables of identity or remove any properites in specific table 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //make Country Property in AspNetUser Table allow null 
            builder.Entity<ApplicationUser>().Property(u=>u.Country).IsRequired(false);
        }

    }
}
