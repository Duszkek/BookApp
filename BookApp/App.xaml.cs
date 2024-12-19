using BookApp.Enums;
using BookApp.Utils;

namespace BookApp;

public partial class App : CurrentApplication
{
    public App()
    {
        InitializeComponent();
        ApplicationNavigator.Init();
        RegisterRoutes();
        
        Intent intent = new Intent();
        intent.AddValue(IntentName.Test, "test title from intent :D");
        MainPage = new NavigationPage(new MainPage(intent));
        (MainPage as NavigationPage).BarBackgroundColor = Colors.Chocolate;
        (MainPage as NavigationPage).BarTextColor = Colors.DarkGray;
    }

    private void RegisterRoutes()
    {
        NavigationManager.Register(NavigationWizardStep.Main, typeof(MainPage));
    }
}