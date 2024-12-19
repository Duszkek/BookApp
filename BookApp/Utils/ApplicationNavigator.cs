using BookApp.Enums;

namespace BookApp.Utils;

public static class ApplicationNavigator
{
    #region Methods

    public static void Init()
    {
        
    }
    
    //TO DO : Could do with some sort of fade or animation xd its too fast!
    public static async Task GoToPage(Intent intent, NavigationWizardStep? step)
    {
        try
        {
            INavigation? navigation = CurrentApplication.Current.ApplicationNavigation;

            AppPage page = null;
                
            if (navigation == null)
            {
                throw new NullReferenceException();
            }

            if (page == null && step.HasValue)
            {
                page = NavigationManager.GetPage(intent, step.Value);
            }

            if (page == null)
            {
                throw new NullReferenceException();
            }

            await navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public static async Task GoBack(INavigation? navigation = null)
    {
        try
        {
            navigation ??= CurrentApplication.Current.ApplicationNavigation;

            if (navigation == null)
            {
                throw new NullReferenceException();
            }

            await navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    
    #endregion
}