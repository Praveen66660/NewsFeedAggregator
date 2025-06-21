using System.ComponentModel.DataAnnotations;

namespace NewsFeedAggregator.Dtos
{
    public class RegisterUserRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? Interests { get; set; }
    }
}
