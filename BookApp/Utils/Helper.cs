namespace BookApp.Utils;

public static class Helper
{
    public static bool IsInternetAvailable()
    {
        IConnectivity current = Connectivity.Current;

        return current.NetworkAccess == NetworkAccess.Internet;
    }
}