using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlleviationApplication.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DisasterIncident> DisasterIncidents { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<VolunteerTask> VolunteerTasks { get; set; }
        public DbSet<VolunteerAssignment> VolunteerAssignments { get; set; }
        public DbSet<DonationCategory> DonationCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<DisasterIncident>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Donation>()
                .HasOne(d => d.Category)
                .WithMany(c => c.Donations)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Volunteer>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VolunteerTask>()
                .HasOne(t => t.AssignedVolunteer)
                .WithMany()
                .HasForeignKey(t => t.AssignedVolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VolunteerAssignment>()
                .HasOne(va => va.Volunteer)
                .WithMany(v => v.Assignments)
                .HasForeignKey(va => va.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VolunteerAssignment>()
                .HasOne(va => va.Task)
                .WithMany(t => t.Assignments)
                .HasForeignKey(va => va.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data for donation categories
            builder.Entity<DonationCategory>().HasData(
                new DonationCategory { Id = 1, Name = "Food", Description = "Non-perishable food items" },
                new DonationCategory { Id = 2, Name = "Clothing", Description = "Clothes and personal items" },
                new DonationCategory { Id = 3, Name = "Medical Supplies", Description = "Medical equipment and supplies" },
                new DonationCategory { Id = 4, Name = "Emergency Supplies", Description = "Emergency response equipment" },
                new DonationCategory { Id = 5, Name = "Financial", Description = "Monetary donations" }
            );
        }
    }
}
