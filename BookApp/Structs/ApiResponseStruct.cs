using BookApp.Models;

namespace BookApp.Structs;

public struct ApiResponseStruct
{
    #region Properties
    
    public int ItemsFound { get; set; }
    
    public List<BookModel> Books { get; set; }
    
    #endregion
}