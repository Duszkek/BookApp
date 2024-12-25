using System.Collections.ObjectModel;
using BookApp.Enums;
using BookApp.Models;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookApp.ViewModels;

public partial class AddUserViewModel
    : ObservableObject
{
    #region Members
    
    [ObservableProperty]
    private string username;

    private Intent Intent { get; set; }
    
    #endregion
    
    #region Properties

    public bool AnyUserIsVisible => UserList.Count > 0;
    
    public ObservableCollection<UserModel> UserList { get; set; }
    
    #endregion
    
    #region Ctor

    public AddUserViewModel(Intent intent)
    {
        Intent = intent;
    }
    
    #endregion
    
    #region Methods
    
    [RelayCommand]
    private async Task SaveUser()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return;
        }

        UserModel newUser = await MauiProgram.DataProviderService.CreateUserAsync(Username);
        Intent.AddValue(IntentName.NewUser, newUser);
        await ApplicationNavigator.GoBack();
    }
    
    #endregion
}