using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsFeedAggregator.Models
{
    public class UserPreference
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public string Category { get; set; } // e.g., tech, health, sports

        public virtual User User { get; set; }
    }
}
