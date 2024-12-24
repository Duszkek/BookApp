using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp.Entities;

public class UserReadBooks
{
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    [ForeignKey("Book")]
    public int BookId { get; set; }
    public User User { get; set; }
    public Book Book { get; set; }
}