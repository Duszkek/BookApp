namespace BookApp.Utils;

public class CurrentApplication : Application
{
    public new static CurrentApplication Current
    {
        get
        {
            if (Application.Current is CurrentApplication application)
            {
                return application;;
            }

            return null;
        }
    }

    public INavigation? ApplicationNavigation
    {
        get
        {
            if (Current.MainPage is NavigationPage navigationPage)
            {
                return navigationPage.Navigation;
            }

            return null;
        }
    }
}