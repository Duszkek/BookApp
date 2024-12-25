using BookApp.Models;

namespace BookApp.Structs;

public class DbResponseStruct
{
    #region Properties
    
    public int ItemsFound { get; set; }
    
    public List<BookModel> Books { get; set; }
    
    #endregion
}