using System.ComponentModel.DataAnnotations;

namespace NewsFeedAggregator.Models
{
    public class NewsArticles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Summary { get; set; }

        [Required]
        public string Source { get; set; }

        public string? Url { get; set; }

        public DateTime PublishedAt { get; set; }

        public string? Category { get; set; }
    }
}
