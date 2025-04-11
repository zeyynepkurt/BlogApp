using BlogApp.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Blog
{
    public int Id { get; set; }

    public string? UserId { get; set; } // kullanıcıya ait
    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; } // ilişki


    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public string Author { get; set; }

    public DateTime PublishDate { get; set; }

    [Required]
    public int CategoryId { get; set; }

    // Bu alan ZORUNLU OLMAMALI
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public string? ImagePath { get; set; }
}
