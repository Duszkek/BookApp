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
            // apparently in maui you can also do single page apps :) just like in blazor hybrid
            if (Current.MainPage is NavigationPage navigationPage)
            {
                return navigationPage.Navigation;
            }

            return null;
        }
    }
}