// ViewModels/TransactionViewModel.cs
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Windows.Input;

namespace MauiBankingExercise.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;

        private Account _account;
        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private int _selectedTransactionTypeId = 1; // Default to deposit
        public int SelectedTransactionTypeId
        {
            get => _selectedTransactionTypeId;
            set => SetProperty(ref _selectedTransactionTypeId, value);
        }

        public ICommand SubmitTransactionCommand { get; }

        public TransactionViewModel(IDatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            SubmitTransactionCommand = new Command(async () => await SubmitTransaction(), () => !IsBusy);
        }

        public void Initialize(Account account)
        {
            Account = account;
            Amount = 0;
            Description = string.Empty;
            SelectedTransactionTypeId = 1; // Reset to deposit
        }

        private async Task SubmitTransaction()
        {
            if (Amount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Amount must be greater than zero", "OK");
                return;
            }

            if (SelectedTransactionTypeId == 2 && Amount > Account.AccountBalance) // Withdrawal
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Insufficient funds", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var transaction = new Transaction
                {
                    AccountId = Account.AccountId,
                    TransactionTypeId = SelectedTransactionTypeId,
                    Amount = Amount,
                    Description = Description,
                    TransactionDate = DateTime.Now
                };

                var success = await _databaseService.MakeTransactionAsync(transaction);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Transaction completed successfully", "OK");
                    await _navigationService.GoBackAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Transaction failed", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}