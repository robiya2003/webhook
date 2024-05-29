using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using webhook.models;

namespace webhook
{
    public class AppBotDbContext : DbContext
    {
        public AppBotDbContext(DbContextOptions<AppBotDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        public DbSet<User> Users { get; set; }
    }
}
