namespace UpScript.Services.Admin.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Upscript.Services.Admin.API.Model;
    using Upscript.Services.Employee.API.Infrastructure.EntityConfigurations;

    public class AdminContext : DbContext
    {
        public AdminContext(DbContextOptions<AdminContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AdminEntityTypeConfiguration());
        }
    }
}
