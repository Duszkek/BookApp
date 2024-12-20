using BookApp.Enums;
using BookApp.Utils;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BookApp.ViewModels;

public partial class LoginViewModel
    : ObservableObject
{
    #region Members

    private Intent Intent { get; set; }

    #endregion
    
    #region Properties

    [ObservableProperty] private string title;

    #endregion
    
    #region Ctor

    public LoginViewModel(Intent intent)
    {
        Intent = intent;
        Title = intent.GetValue<string>(IntentName.Test);
    }
    
    #endregion
    
    #region Methods

    public async void OnButtonClicked()
    {
        ApplicationNavigator.GoToPage(Intent, NavigationWizardStep.Main);
    }
    
    #endregion
}