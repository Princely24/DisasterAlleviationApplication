using System.ComponentModel.DataAnnotations;

namespace DisasterAlleviationApplication.Models
{
    public class VolunteerTask
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TaskCategory Category { get; set; }

        [Required]
        public TaskPriority Priority { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public TaskStatus Status { get; set; } = TaskStatus.Open;

        public int? AssignedVolunteerId { get; set; }

        public int RequiredVolunteers { get; set; } = 1;

        public int CurrentVolunteers { get; set; } = 0;

        [StringLength(200)]
        public string? RequiredSkills { get; set; }

        [StringLength(200)]
        public string? Equipment { get; set; }

        public int? EstimatedHours { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual Volunteer? AssignedVolunteer { get; set; }
        public virtual ICollection<VolunteerAssignment> Assignments { get; set; } = new List<VolunteerAssignment>();
    }

    public enum TaskCategory
    {
        ReliefDistribution,
        SearchAndRescue,
        MedicalSupport,
        Logistics,
        Communication,
        Fundraising,
        Administrative,
        Cleanup,
        Transportation,
        Other
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum TaskStatus
    {
        Open,
        Assigned,
        InProgress,
        Completed,
        Cancelled
    }
}
