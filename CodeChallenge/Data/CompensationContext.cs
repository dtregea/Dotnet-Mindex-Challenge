using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data
{
    public class CompensationContext : DbContext
    {
        public CompensationContext(DbContextOptions<CompensationContext> options) : base(options)
        {

        }

        public DbSet<Compensation> Compensations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Compensation>()
                .HasOne(c => c.Employee)
                .WithMany()
                .HasForeignKey(c => c.EmployeeId);
        }
    }
}