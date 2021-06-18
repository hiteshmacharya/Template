using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Upscript.Services.Employee.API.Model;

namespace Upscript.Services.Employee.API.Infrastructure.EntityConfigurations
{
    class EmployeeEntityTypeConfiguration
        : IEntityTypeConfiguration<Employees>
    {
        public void Configure(EntityTypeBuilder<Employees> builder)
        {
            builder.ToTable("employees");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               .IsRequired();

            builder.Property(cb => cb.Name)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(cb => cb.Description)
                .IsRequired(false)
                .HasMaxLength(250);

            builder.Property(cb => cb.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cb => cb.IsActive)
                .IsRequired();
        }
    }
}
