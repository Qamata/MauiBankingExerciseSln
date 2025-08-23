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

        public decimal TotalBalance => Accounts.Sum(a => a.AccountBalance);

        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();

        public ICommand LoadAccountsCommand { get; }
        public ICommand ViewTransactionsCommand { get; }
        public ICommand MakeTransactionCommand { get; }

        public CustomerDashboardViewModel(IDatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            LoadAccountsCommand = new Command(async () => await LoadAccounts());
            ViewTransactionsCommand = new Command(async () => await ViewTransactions(), () => HasSelectedAccount);
            MakeTransactionCommand = new Command(async () => await MakeTransaction(), () => HasSelectedAccount);
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
    }
}