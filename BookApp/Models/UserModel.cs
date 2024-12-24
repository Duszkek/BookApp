using BookApp.Entities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookApp.Models;

public partial class UserModel
    : ObservableObject
{
    #region Properties

    [ObservableProperty]
    private int userId;
    
    [ObservableProperty]
    private string username;
    
    [ObservableProperty]
    private int booksCount;
    
    #endregion
    
    #region Ctor

    public UserModel(User user, int booksCount = 0)
    {
        UserId = user.IdUser;
        Username = user.Name;
        BooksCount = booksCount;
    }
    
    #endregion
}