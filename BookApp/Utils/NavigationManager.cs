using System;
using System.Collections.Concurrent;
using BookApp.Enums;
using BookApp.Extensions;

namespace BookApp.Utils;

public static class NavigationManager
{
    #region Members

    private static ConcurrentDictionary<NavigationWizardStep, Type> Routes;

    #endregion
    
    #region Ctor

    static NavigationManager()
    {
        Routes = new();
    }
    
    #endregion

    public static void Register(NavigationWizardStep step, Type page)
    {
        Routes[step] = page;
    }

    public static Type? GetRoute(NavigationWizardStep step)
    {
        return Routes.TryGetValue(step);
    }

    public static AppPage? GetPage(Intent intent, NavigationWizardStep step)
    {
        try
        {
            Type routeType = GetRoute(step);
            object? instance = null;

            System.Reflection.ConstructorInfo routeCtor = routeType.GetConstructor(new Type[] {typeof(Intent)});

            if (routeCtor is not null)
            {
                instance = routeCtor.Invoke(new object[] {intent});
            }

            AppPage? result = instance as AppPage;

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}