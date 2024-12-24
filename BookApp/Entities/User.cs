using System.ComponentModel.DataAnnotations;

namespace BookApp.Entities;

public class User
{
    [Key]
    public int IdUser { get; set; }
    public string Name { get; set; }
    public ICollection<UserReadBooks> UserReadBooks { get; set; }
}