using BookApp.Entities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookApp.Models;

public partial class BookModel
    : ObservableObject
{
    #region Properties

    [ObservableProperty]
    private int? localId;

    [ObservableProperty] 
    private string apiId;

    [ObservableProperty]
    private string title;

    [ObservableProperty] 
    private string authors;

    [ObservableProperty] 
    private string publishedDate;

    [ObservableProperty] 
    private string description;

    [ObservableProperty] 
    private string thumbnail;

    [ObservableProperty] 
    private string smallThumbnail;

    [ObservableProperty] 
    private int? pageCount;

    [ObservableProperty] 
    private bool isSaved;
    public bool HasThumbnailImage => !string.IsNullOrEmpty(Thumbnail);
    public bool HasSmallThumbnailImage => !string.IsNullOrEmpty(SmallThumbnail);

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
        Authors = book.Authors ?? string.Empty;
        PublishedDate = book.PublishedDate ?? string.Empty;
        Description = book.Description ?? string.Empty;
        Thumbnail = book.Thumbnail ?? string.Empty;
        PageCount = book.PageCount;
        IsSaved = false;
    }
    
    #endregion
}