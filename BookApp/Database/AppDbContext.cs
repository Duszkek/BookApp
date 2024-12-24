using BookApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Database;

public class AppDbContext : DbContext
{
    #region Const
    
    private readonly string DbPath;

    #endregion
    
    #region Properties
    
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<UserReadBooks> UserReadBooks { get; set; }
    
    #endregion
    
    #region Ctor
    
    public AppDbContext(string dbFileName)
    {
        if (string.IsNullOrWhiteSpace(dbFileName))
        {
            throw new ArgumentNullException(nameof(dbFileName));
        }

        string appDataPath = FileSystem.AppDataDirectory;

        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        DbPath = Path.Combine(appDataPath, dbFileName);
    }
    
    #endregion
    
    #region Methods

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Filename={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.IdUser);

        modelBuilder.Entity<Book>()
            .HasKey(b => b.IdBook);

        modelBuilder.Entity<UserReadBooks>()
            .HasKey(urb => urb.Id);

        modelBuilder.Entity<UserReadBooks>()
            .HasOne(urb => urb.User)
            .WithMany(u => u.UserReadBooks)
            .HasForeignKey(urb => urb.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<UserReadBooks>()
            .HasOne(urb => urb.Book)
            .WithMany(b => b.UserReadBooks)
            .HasForeignKey(urb => urb.BookId)
            .OnDelete(DeleteBehavior.NoAction);
    }
    
    public async Task InitializeDatabaseAsync()
    {
        await this.Database.EnsureCreatedAsync();
    }
    
    #endregion
}
