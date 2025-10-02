using System.ComponentModel.DataAnnotations;

namespace DisasterAlleviationApplication.Models
{
    public class DonationCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
    }
}
