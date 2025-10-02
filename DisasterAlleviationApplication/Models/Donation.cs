using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisasterAlleviationApplication.Models
{
    public class Donation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Unit { get; set; } = string.Empty; // e.g., "pieces", "kg", "liters", "boxes"

        public decimal? EstimatedValue { get; set; }

        [Required]
        public DonationType Type { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        [StringLength(200)]
        public string? PickupLocation { get; set; }

        public DateTime? PickupDate { get; set; }

        [StringLength(200)]
        public string? DistributionLocation { get; set; }

        public DateTime? DistributionDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsUrgent { get; set; } = false;

        public DateTime? ExpiryDate { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual DonationCategory Category { get; set; } = null!;
    }

    public enum DonationType
    {
        Physical,
        Financial,
        Service
    }

    public enum DonationStatus
    {
        Pending,
        Approved,
        Collected,
        InTransit,
        Distributed,
        Rejected
    }
}
