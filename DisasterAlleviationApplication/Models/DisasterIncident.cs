using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisasterAlleviationApplication.Models
{
    public class DisasterIncident
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DisasterType Type { get; set; }

        [Required]
        public IncidentSeverity Severity { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        public string PostalCode { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public DateTime IncidentDate { get; set; }
        public DateTime ReportedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public IncidentStatus Status { get; set; } = IncidentStatus.Reported;

        [StringLength(500)]
        public string? AdditionalNotes { get; set; }

        public int? EstimatedAffectedPeople { get; set; }

        // File attachment properties
        [StringLength(500)]
        public string? AttachmentFileName { get; set; }

        [StringLength(100)]
        public string? AttachmentContentType { get; set; }

        public long? AttachmentFileSize { get; set; }

        [StringLength(500)]
        public string? AttachmentFilePath { get; set; }

        public DateTime? AttachmentUploadedDate { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;

        // Not mapped - for file upload
        [NotMapped]
        public IFormFile? AttachmentFile { get; set; }
    }

    public enum DisasterType
    {
        Flood,
        Fire,
        Earthquake,
        Hurricane,
        Tornado,
        Drought,
        Pandemic,
        Other
    }

    public enum IncidentSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum IncidentStatus
    {
        Reported,
        UnderReview,
        InProgress,
        Resolved,
        Closed
    }
}
