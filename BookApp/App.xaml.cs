using BookApp.Enums;
using BookApp.Utils;
using BookApp.Views;

namespace BookApp;

public partial class App : CurrentApplication
{
    public App()
    {
        InitializeComponent();
        ApplicationNavigator.Init();
        RegisterRoutes();
        
        Intent intent = new Intent();
        intent.AddValue(IntentName.Main, "test title from intent :D");
        MainPage = new NavigationPage(new LoginView(intent));
        (MainPage as NavigationPage).BarBackgroundColor = Colors.Chocolate;
        (MainPage as NavigationPage).BarTextColor = Colors.DarkGray;
    }

    private void RegisterRoutes()
    {
        NavigationManager.Register(NavigationWizardStep.Login, typeof(LoginView));
    }
}