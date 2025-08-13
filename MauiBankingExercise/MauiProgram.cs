using Microsoft.Extensions.Logging;

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

            builder.Services.AddSingleton<PokemonViewModel>();
            builder.Services.AddTransient<PokemonView>();
            builder.Services.AddSingleton<BankDatabaseService>();
            builder.Services.AddSingleton<AllPokemonViewModel>();
            builder.Services.AddTransient<AllPokemonView>();

            return builder.Build();
        }
    }
}
