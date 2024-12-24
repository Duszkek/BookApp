using System.Collections.ObjectModel;
using BookApp.Enums;
using BookApp.Models;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BookApp.ViewModels;

public partial class LoginViewModel
    : ObservableObject
{
    #region Properties
    
    [ObservableProperty]
    private bool deleteMode;
    
    private Intent Intent { get; }

    public bool AnyUserIsVisible => UserList.Count > 0;

    public ObservableCollection<UserModel> UserList { get; set; } = [];
    
    public UserModel? SelectedItem { get; set; }
    
    #endregion
    
    #region Ctor

    public LoginViewModel(Intent intent)
    {
        Intent = intent;
    }
    
    #endregion
    
    #region Methods

    public void Refresh()
    {
        GetDataFromIntent();
        DeleteMode = false;
        SelectedItem = null;
    }
    
    public async Task LoadDataAsync()
    {
        List<UserModel> availableUsers = await MauiProgram.DataProviderService.GetUserModelListAsync();
        
        UserList.Clear();
        foreach (UserModel user in availableUsers)
        {
            UserList.Add(user);
        }
        
        OnPropertyChanged(nameof(UserList));
        OnPropertyChanged(nameof(AnyUserIsVisible));
    }

    public void GetDataFromIntent()
    {
        if (Intent.HasValue(IntentName.NewUser))
        {
            UserModel newUser = Intent.GetAndPopValue<UserModel>(IntentName.NewUser);
            UserList.Add(newUser);
            OnPropertyChanged(nameof(UserList));
            OnPropertyChanged(nameof(AnyUserIsVisible));
        }
            
    }
    
    public async Task ItemTapped(object selectedUser)
    {
        if (selectedUser is UserModel user && UserList.Contains(user) && !DeleteMode)
        {
            MauiProgram.CurrentUser = user;
            await ApplicationNavigator.GoToPage(Intent, NavigationWizardStep.SelectBookListType);
        }
    }
    
    #region RelayCommands
    
    [RelayCommand]
    private void ToggleDeleteMode()
    {
        DeleteMode = !DeleteMode;
    }
    
    [RelayCommand]
    private async Task OnAddUser()
    {
        await ApplicationNavigator.GoToPage(Intent, NavigationWizardStep.AddUser);
    }

    [RelayCommand]
    private async Task DeleteUser(int idUser)
    {
        bool isDeleted = await MauiProgram.DataProviderService.DeleteUserAsync(idUser);
        if (isDeleted)
        {
            UserModel? deletedUser = UserList.FirstOrDefault(u => u.UserId == idUser);
            if (deletedUser == null)
            {
                throw new NullReferenceException(nameof(deletedUser));
            }
            
            UserList.Remove(deletedUser);
        
            OnPropertyChanged(nameof(UserList));
            OnPropertyChanged(nameof(AnyUserIsVisible));

            if (UserList.Count == 0)
            {
                DeleteMode = false;
            }
        }
    }
    
    #endregion
    
    #endregion
}