using MauiBankingExercise.Views.Pages;

namespace MauiBankingExercise
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Register routes for navigation
            Routing.RegisterRoute("dashboard", typeof(CustomerDashboardPage));
            Routing.RegisterRoute("accountdetails", typeof(AccountDetailsPage));
            Routing.RegisterRoute("transaction", typeof(TransactionPage));
        }
    }
}
