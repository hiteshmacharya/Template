namespace UpScript.Services.Employee.API.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using Upscript.Services.Employee.API.Infrastructure.EntityConfigurations;
    using Upscript.Services.Employee.API.Model;

    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {
        }
        public DbSet<Employees> Employees { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new EmployeeEntityTypeConfiguration());
        }
    }
}
