using BookApp.Entities;

namespace BookApp.Models;

public class BookModel
{
    #region Properties
    
    public int? LocalId { get; set; }
    public string ApiId { get; set; } 
    public string Title { get; set; }
    public string Authors { get; set; }
    public string PublishedDate { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public int PageCount { get; set; } 
    public bool IsSaved => LocalId != null;

    #endregion
    
    #region Ctor
    
    public BookModel()
    {
        
    }
    
    public BookModel(Book book)
        : this()
    {
        LocalId = book.IdBook;
        ApiId = book.ApiId;
        Title = book.Title;
        Authors = book.Authors;
        PublishedDate = book.PublishedDate;
        Description = book.Description;
        Thumbnail = book.Thumbnail;
        PageCount = book.PageCount;
    }
    
    #endregion
}