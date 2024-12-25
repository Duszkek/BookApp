using BookApp.Base;
using BookApp.Enums;
using BookApp.Utils;
using BookApp.ViewModels;

namespace BookApp.Views;

public partial class LoginView
    : AppPage
{
    #region Members
    
    private readonly LoginViewModel ViewModel;
    
    #endregion
    
    #region Properties
    
    public override PageName Name => PageName.Login;

    #endregion

    public LoginView(Intent intent)
        : base(intent)
    {
        InitializeComponent();
        BindingContext = ViewModel = new LoginViewModel(intent);
    }

    private async void UserListView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
        {
            return;
        }
        
        if (Helper.IsInternetAvailable())
        {
            if (e.CurrentSelection?.Count > 0)
            {
                await ViewModel?.ItemTapped(e.CurrentSelection.FirstOrDefault());
            }
        }
        else
        {
            await DisplayAlert("No Internet", "Internet connection is not available. Please check your connection and try again.", "OK");
        }
        
        if (sender is CollectionView userCollectionView)
        {
            userCollectionView.SelectedItem = null;
        }
    }
    
    protected override async void OnAppearing()
    {
        if (!IsLoaded)
        {
            base.OnAppearing();
            await ViewModel?.LoadDataAsync();
            IsLoaded = true;
        }

        ViewModel.Refresh();
    }
}