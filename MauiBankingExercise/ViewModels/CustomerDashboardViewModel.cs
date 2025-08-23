// ViewModels/CustomerDashboardViewModel.cs
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiBankingExercise.ViewModels
{
    public class CustomerDashboardViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly IDataRefreshService _refreshService;

        private Customer _customer;
        public Customer Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                SetProperty(ref _selectedAccount, value);
                OnPropertyChanged(nameof(HasSelectedAccount));
            }
        }

        public bool HasSelectedAccount => SelectedAccount != null;

        private ObservableCollection<Account> _accounts = new ObservableCollection<Account>();
        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set
            {
                SetProperty(ref _accounts, value);
                OnPropertyChanged(nameof(TotalBalance)); // Notify when accounts change
            }
        }

        public decimal TotalBalance => Accounts.Sum(a => a.AccountBalance);

        public ICommand LoadAccountsCommand { get; }
        public ICommand ViewTransactionsCommand { get; }
        public ICommand MakeTransactionCommand { get; }
        public ICommand RefreshDataCommand { get; }

        public CustomerDashboardViewModel(IDatabaseService databaseService, INavigationService navigationService, IDataRefreshService refreshService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _refreshService = refreshService;

            LoadAccountsCommand = new Command(async () => await LoadAccounts());
            ViewTransactionsCommand = new Command(async () => await ViewTransactions(), () => HasSelectedAccount);
            MakeTransactionCommand = new Command(async () => await MakeTransaction(), () => HasSelectedAccount);
            RefreshDataCommand = new Command(async () => await RefreshData());

            // Subscribe to account updates
            _refreshService.AccountUpdated += OnAccountUpdated;
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

                // Notify that TotalBalance has changed
                OnPropertyChanged(nameof(TotalBalance));
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshData()
        {
            if (Customer == null) return;

            IsBusy = true;
            try
            {
                // Reload accounts to get updated balances
                var accounts = await _databaseService.GetCustomerAccountsAsync(Customer.CustomerId);

                // Update existing accounts with new balances
                foreach (var updatedAccount in accounts)
                {
                    var existingAccount = Accounts.FirstOrDefault(a => a.AccountId == updatedAccount.AccountId);
                    if (existingAccount != null)
                    {
                        existingAccount.AccountBalance = updatedAccount.AccountBalance;
                    }
                }

                // Notify that TotalBalance has changed
                OnPropertyChanged(nameof(TotalBalance));
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnAccountUpdated(object sender, AccountUpdatedEventArgs e)
        {
            // Update the account in our collection if it exists
            var accountToUpdate = Accounts.FirstOrDefault(a => a.AccountId == e.Account.AccountId);
            if (accountToUpdate != null)
            {
                accountToUpdate.AccountBalance = e.Account.AccountBalance;

                // Notify that TotalBalance has changed
                OnPropertyChanged(nameof(TotalBalance));

                Console.WriteLine($"Account {e.Account.AccountId} balance updated to {e.Account.AccountBalance:C}");
            }
        }

        private async Task ViewTransactions()
        {
            if (SelectedAccount != null)
            {
                await _navigationService.NavigateToAccountDetailsAsync(SelectedAccount);
            }
        }

        private async Task MakeTransaction()
        {
            if (SelectedAccount != null)
            {
                await _navigationService.NavigateToTransactionPageAsync(SelectedAccount);
            }
        }

        // Cleanup
        ~CustomerDashboardViewModel()
        {
            _refreshService.AccountUpdated -= OnAccountUpdated;
        }
    }
}