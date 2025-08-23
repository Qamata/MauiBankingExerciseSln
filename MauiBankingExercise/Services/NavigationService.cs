// Services/NavigationService.cs
using MauiBankingExercise.Models;
using MauiBankingExercise.ViewModels;
using MauiBankingExercise.Views.Pages;
using System.Threading.Tasks;

namespace MauiBankingExercise.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task NavigateToDashboardAsync(Customer customer)
        {
            var dashboardPage = _serviceProvider.GetService<CustomerDashboardPage>();
            var viewModel = _serviceProvider.GetService<CustomerDashboardViewModel>();
            await viewModel.Initialize(customer);
            dashboardPage.BindingContext = viewModel;
            await Application.Current.MainPage.Navigation.PushAsync(dashboardPage);
        }

        public async Task NavigateToAccountDetailsAsync(Account account)
        {
            var accountDetailsPage = _serviceProvider.GetService<AccountDetailsPage>();
            var viewModel = _serviceProvider.GetService<AccountDetailsViewModel>();
            await viewModel.Initialize(account);
            accountDetailsPage.BindingContext = viewModel;
            await Application.Current.MainPage.Navigation.PushAsync(accountDetailsPage);
        }

        public async Task NavigateToTransactionPageAsync(Account account)
        {
            var transactionPage = _serviceProvider.GetService<TransactionPage>();
            var viewModel = _serviceProvider.GetService<TransactionViewModel>();
            viewModel.Initialize(account);
            transactionPage.BindingContext = viewModel;
            await Application.Current.MainPage.Navigation.PushAsync(transactionPage);
        }

        public async Task GoBackAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}