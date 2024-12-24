using BookApp.Enums;
using BookApp.Utils;

namespace BookApp.Base;

public abstract class AppPage
    : ContentPage
{
    #region Properties

    public abstract PageName Name { get; }
    protected Intent Intent { get; private set; }
    protected new bool IsLoaded { get; set; }

    #endregion

    #region Ctor

    public AppPage(Intent intent)
    {
        Intent = intent;
    }

    #endregion
}