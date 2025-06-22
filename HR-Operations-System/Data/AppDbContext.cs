using HR_Operations_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HR_Operations_System.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<TimingPlan> TimingPlans { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmployeeAllow> EmployeeAllows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.ApplicationUser)
                .HasForeignKey<Employee>(e => e.UserId);

            modelBuilder.Entity<EmployeeAllow>()
                .HasOne(e => e.TimingPlan)
                .WithMany(t => t.EmployeeAllows)
                .HasForeignKey(e => e.TimingCode)
                .OnDelete(DeleteBehavior.NoAction); // 👈 Prevent cascade delete


        }

    }

}
