// ViewModels/CustomerDashboardViewModel.cs
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiBankingExercise.ViewModels
{
    public class CustomerDashboardViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;

        private Customer _customer;
        public Customer Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();

        public ICommand LoadAccountsCommand { get; }
        public ICommand SelectAccountCommand { get; }
        public ICommand MakeTransactionCommand { get; }

        public CustomerDashboardViewModel(IDatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            LoadAccountsCommand = new Command(async () => await LoadAccounts());
            SelectAccountCommand = new Command<Account>(async (account) => await SelectAccount(account));
            MakeTransactionCommand = new Command<Account>(async (account) => await MakeTransaction(account));
        }

        public async Task Initialize(Customer customer)
        {
            Customer = customer;
            await LoadAccounts();
        }

        private async Task LoadAccounts()
        {
            if (Customer == null) return;

            IsBusy = true;
            try
            {
                Accounts.Clear();
                var accounts = await _databaseService.GetCustomerAccountsAsync(Customer.CustomerId);
                foreach (var account in accounts)
                {
                    Accounts.Add(account);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SelectAccount(Account account)
        {
            if (account != null)
            {
                await _navigationService.NavigateToAccountDetailsAsync(account);
            }
        }

        private async Task MakeTransaction(Account account)
        {
            if (account != null)
            {
                await _navigationService.NavigateToTransactionPageAsync(account);
            }
        }
    }
}