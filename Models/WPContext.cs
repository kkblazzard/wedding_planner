using Microsoft.EntityFrameworkCore;

namespace wedding_planner.Models
{
    public class WPContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public WPContext(DbContextOptions<WPContext> options) : base(options) { }
        public DbSet<User> Users {get; set;}
        public DbSet<Wedding> Wedding {get; set;}
        public DbSet<RSVP> RSVP {get; set;}
    }
}
