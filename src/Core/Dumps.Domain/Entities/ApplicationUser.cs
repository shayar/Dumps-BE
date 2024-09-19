using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Dumps.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }

        [StringLength(50)]
        [Required]
        public string LastName { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public bool IsDisabled { get; set; } = false;
        public string? DisabledBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdated { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDefault { get; set; }
        public bool IsEmailConfirmed { get; set;}
    }
}
