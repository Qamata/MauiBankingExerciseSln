// App.xaml.cs
using MauiBankingExercise.Services;
using MauiBankingExercise.Views.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace MauiBankingExercise
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Initialize database
            var databaseService = serviceProvider.GetService<IDatabaseService>();
            databaseService.InitializeDatabaseAsync();

            MainPage = new NavigationPage(serviceProvider.GetService<CustomerSelectionPage>());
        }
    }
}