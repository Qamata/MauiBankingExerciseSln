// MauiProgram.cs
using MauiBankingExercise.Services;
using MauiBankingExercise.ViewModels;
using Microsoft.Extensions.Logging;
using MauiBankingExercise.Views.Pages;

namespace MauiBankingExercise
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Register Services
            builder.Services.AddSingleton<IDatabaseService>(_ => DatabaseService.GetInstance());
            builder.Services.AddSingleton<INavigationService, NavigationService>();

            // Register ViewModels
            builder.Services.AddTransient<CustomerSelectionViewModel>();
            builder.Services.AddTransient<CustomerDashboardViewModel>();
            builder.Services.AddTransient<AccountDetailsViewModel>();
            builder.Services.AddTransient<TransactionViewModel>();

            // Register Pages
            builder.Services.AddTransient<CustomerSelectionPage>();
            builder.Services.AddTransient<CustomerDashboardPage>();
            builder.Services.AddTransient<AccountDetailsPage>();
            builder.Services.AddTransient<TransactionPage>();
            // MauiProgram.cs
            builder.Services.AddSingleton<IDataRefreshService, DataRefreshService>();

            return builder.Build();
        }
    }
}