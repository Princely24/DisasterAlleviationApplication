using Microsoft.AspNetCore.Identity;

namespace DisasterAlleviationApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<DisasterIncident> DisasterIncidents { get; set; } = new List<DisasterIncident>();
        public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public virtual ICollection<Volunteer> VolunteerProfiles { get; set; } = new List<Volunteer>();
    }
}
