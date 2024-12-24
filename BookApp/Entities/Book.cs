using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BookApp.Entities;

public class Book
{
    [Key]
    public int IdBook { get; set; }
    
    public string ApiId { get; set; } 
    public string Title { get; set; }
    public string Authors { get; set; }
    public string Description { get; set; }
    public string PublishedDate { get; set; }
    public string Thumbnail { get; set; }
    public int PageCount { get; set; }
    public ICollection<UserReadBooks> UserReadBooks { get; set; }
}