using System.ComponentModel.DataAnnotations;

namespace DisasterAlleviationApplication.Models
{
    public class VolunteerAssignment
    {
        public int Id { get; set; }

        [Required]
        public int VolunteerId { get; set; }

        [Required]
        public int TaskId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public DateTime? StartDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;

        public int? HoursWorked { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? Feedback { get; set; }

        public int? Rating { get; set; } // 1-5 rating

        // Navigation properties
        public virtual Volunteer Volunteer { get; set; } = null!;
        public virtual VolunteerTask Task { get; set; } = null!;
    }

    public enum AssignmentStatus
    {
        Assigned,
        Accepted,
        InProgress,
        Completed,
        Cancelled,
        NoShow
    }
}
