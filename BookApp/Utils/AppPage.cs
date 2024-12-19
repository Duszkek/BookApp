using BookApp.Enums;
using Microsoft.Maui.Controls;

namespace BookApp.Utils;

public abstract class AppPage
    : ContentPage
{
    #region Properties

    public abstract PageName Name { get; }

    protected Intent Intent { get; private set; }

    #endregion

    #region Ctor

    public AppPage(Intent intent)
    {
        Intent = intent;
    }

#endregion
}