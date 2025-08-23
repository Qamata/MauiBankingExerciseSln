// ViewModels/AccountDetailsViewModel.cs
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiBankingExercise.ViewModels
{
    public class AccountDetailsViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;

        private Account _account;
        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        public ObservableCollection<Transaction> Transactions { get; } = new ObservableCollection<Transaction>();

        public ICommand LoadTransactionsCommand { get; }

        public AccountDetailsViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadTransactionsCommand = new Command(async () => await LoadTransactions());
        }

        public async Task Initialize(Account account)
        {
            Account = account;
            await LoadTransactions();
        }

        private async Task LoadTransactions()
        {
            if (Account == null) return;

            IsBusy = true;
            try
            {
                Transactions.Clear();
                var transactions = await _databaseService.GetAccountTransactionsAsync(Account.AccountId);
                foreach (var transaction in transactions.OrderByDescending(t => t.TransactionDate))
                {
                    Transactions.Add(transaction);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}