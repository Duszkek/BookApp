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
        MainPage = new NavigationPage(new LoginView(intent));
    }
    private void RegisterRoutes()
    {
        NavigationManager.Register(NavigationWizardStep.Login, typeof(LoginView));
        NavigationManager.Register(NavigationWizardStep.AddUser, typeof(AddUserView));
        NavigationManager.Register(NavigationWizardStep.SelectBookListType, typeof(SelectBookListTypeView));
        NavigationManager.Register(NavigationWizardStep.SearchBooksList, typeof(SearchBooksListView));
        NavigationManager.Register(NavigationWizardStep.SearchReadBooksList, typeof(SearchReadBooksListView));
        NavigationManager.Register(NavigationWizardStep.BookDetails, typeof(BookDetailsView));
    }
}