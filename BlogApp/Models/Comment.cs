using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }

        public string? Author { get; set; }

        public string? UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int BlogId { get; set; }

        public Blog? Blog { get; set; } 
    }

}
