﻿using System.ComponentModel.DataAnnotations;

namespace NewsFeedAggregator.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? Interests { get; set; }
    }
}
