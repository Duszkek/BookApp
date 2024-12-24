using BookApp.Database;
using BookApp.Services;
using BookApp.Utils;
using BookApp.ViewModels;
using BookApp.Views;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace BookApp;

public static class MauiProgram
{
    #region Properties
    
    public static DataProviderService DataProviderService { get; set; }
    
    #endregion
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .RegisterPages()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        DataProviderService = new DataProviderService(new AppDbContext(Path.Combine(FileSystem.AppDataDirectory, "database.db3")));
        
#if DEBUG
        builder.Logging.AddDebug();
#endif
        MauiApp app = builder.Build();

        using IServiceScope scope = app.Services.CreateScope();
        DataProviderService.InitializeDatabaseAsync().Wait();

        return app;
    }

    private static MauiAppBuilder RegisterPages(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<LoginView>();
        builder.Services.AddSingleton<LoginViewModel>();
        
        builder.Services.AddSingleton<AddUserView>();
        builder.Services.AddSingleton<AddUserViewModel>();

        return builder;
    }
}