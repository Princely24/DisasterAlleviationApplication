using System.ComponentModel.DataAnnotations;

namespace DisasterAlleviationApplication.Models
{
    public class Volunteer
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        public DateTime DateOfBirth { get; set; }

        [StringLength(50)]
        public string? EmergencyContactName { get; set; }

        [Phone]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(200)]
        public string? Skills { get; set; }

        [StringLength(200)]
        public string? Interests { get; set; }

        [StringLength(100)]
        public string? Availability { get; set; }

        public VolunteerStatus Status { get; set; } = VolunteerStatus.Pending;

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastActiveDate { get; set; }

        public bool HasBackgroundCheck { get; set; } = false;

        public DateTime? BackgroundCheckDate { get; set; }

        public int TotalHoursVolunteered { get; set; } = 0;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<VolunteerAssignment> Assignments { get; set; } = new List<VolunteerAssignment>();
    }

    public enum VolunteerStatus
    {
        Pending,
        Approved,
        Active,
        Inactive,
        Suspended
    }
}
