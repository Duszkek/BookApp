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
    
    protected override async void OnAppearing()
    {
        if (!IsLoaded)
        {
            base.OnAppearing();
            await ViewModel?.LoadDataAsync();
            IsLoaded = true;
        }

        ViewModel?.GetDataFromIntent();

        ViewModel.DeleteMode = false; // force delete mode to off
    }
}