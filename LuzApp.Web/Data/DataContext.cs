using LuzApp.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuzApp.Web.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<Neighborhood> Neighborhoods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>(cou =>
            {
                cou.HasIndex("Name").IsUnique();
                cou.HasMany(c => c.Cities).WithOne(d => d.Department).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<City>(dep =>
            {
                dep.HasIndex("Name", "DepartmentId").IsUnique();
                dep.HasOne(d => d.Department).WithMany(c => c.Cities).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Neighborhood>(cit =>
            {
                cit.HasIndex("Name", "CityId").IsUnique();
                cit.HasOne(c => c.City).WithMany(d => d.Neighborhoods).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}