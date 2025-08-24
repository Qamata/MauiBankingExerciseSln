using MauiBankingExercise.Services;
using MauiBankingExercise.Views.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace MauiBankingExercise
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBalanceVerificationService _balanceVerificationService;

        public App(IServiceProvider serviceProvider, IBalanceVerificationService balanceVerificationService)
        {
            _serviceProvider = serviceProvider;
            _balanceVerificationService = balanceVerificationService;

            InitializeComponent();

            // Initialize database and verify balances
            InitializeApp();
        }

        private async void InitializeApp()
        {
            try
            {
                // Initialize database
                var databaseService = _serviceProvider.GetService<IDatabaseService>();
                if (databaseService != null)
                {
                    await databaseService.InitializeDatabaseAsync();
                    Console.WriteLine("Database initialized successfully");
                }

                // Verify all account balances on app start
                if (_balanceVerificationService != null)
                {
                    await _balanceVerificationService.VerifyAllAccountBalancesAsync();
                    Console.WriteLine("Account balances verified successfully");
                }

                // Set the main page after initialization is complete
                MainPage = new NavigationPage(_serviceProvider.GetService<CustomerSelectionPage>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during app initialization: {ex.Message}");

                // Fallback: still set the main page even if initialization fails
                MainPage = new NavigationPage(_serviceProvider.GetService<CustomerSelectionPage>());
            }
        }
    }
}