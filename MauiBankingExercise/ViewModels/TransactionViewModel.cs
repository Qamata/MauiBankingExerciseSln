using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiBankingExercise.ViewModels
{
    public class TransactionViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly IDataRefreshService _refreshService;
        private readonly IBalanceVerificationService _balanceVerificationService;

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

        private TransactionType _selectedTransactionType;
        public TransactionType SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                SetProperty(ref _selectedTransactionType, value);
                // Update the ID when selection changes
                if (value != null)
                {
                    SelectedTransactionTypeId = value.TransactionTypeId;
                }
            }
        }

        public int SelectedTransactionTypeId { get; private set; }

        public ObservableCollection<TransactionType> TransactionTypes { get; } = new ObservableCollection<TransactionType>();

        public ICommand SubmitTransactionCommand { get; }
        public ICommand LoadTransactionTypesCommand { get; }

        public TransactionViewModel(
            IDatabaseService databaseService,
            INavigationService navigationService,
            IDataRefreshService refreshService,
            IBalanceVerificationService balanceVerificationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _refreshService = refreshService;
            _balanceVerificationService = balanceVerificationService;

            SubmitTransactionCommand = new Command(async () => await SubmitTransaction(), () => !IsBusy);
            LoadTransactionTypesCommand = new Command(async () => await LoadTransactionTypes());
        }

        public async void Initialize(Account account)
        {
            Account = account;
            Amount = 0;
            Description = string.Empty;
            SelectedTransactionType = null;
            SelectedTransactionTypeId = 0;

            await LoadTransactionTypes();
        }

        private async Task LoadTransactionTypes()
        {
            IsBusy = true;
            try
            {
                Console.WriteLine("=== LOADING TRANSACTION TYPES ===");
                TransactionTypes.Clear();

                // Get transaction types from database
                var transactionTypes = await GetTransactionTypesFromDatabase();
                Console.WriteLine($"Retrieved {transactionTypes.Count} transaction types from database");

                foreach (var type in transactionTypes)
                {
                    Console.WriteLine($"- {type.TransactionTypeId}: {type.Name}");
                    TransactionTypes.Add(type);
                }

                // If no types found in database, use fallback
                if (TransactionTypes.Count == 0)
                {
                    Console.WriteLine("No transaction types found in database, using fallback");
                    // Add fallback types
                    var fallbackTypes = new List<TransactionType>
                    {
                        new TransactionType { TransactionTypeId = 1, Name = "Deposit" },
                        new TransactionType { TransactionTypeId = 2, Name = "Withdrawal" },
                        new TransactionType { TransactionTypeId = 3, Name = "Transfer" }
                    };

                    foreach (var type in fallbackTypes)
                    {
                        TransactionTypes.Add(type);
                    }
                }

                // Set default selection if types are available
                if (TransactionTypes.Count > 0 && SelectedTransactionType == null)
                {
                    SelectedTransactionType = TransactionTypes[0];
                    Console.WriteLine($"Default selection set to: {SelectedTransactionType.Name} (ID: {SelectedTransactionType.TransactionTypeId})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transaction types: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Fallback to hardcoded types on error
                TransactionTypes.Clear();
                var fallbackTypes = new List<TransactionType>
                {
                    new TransactionType { TransactionTypeId = 1, Name = "Deposit" },
                    new TransactionType { TransactionTypeId = 2, Name = "Withdrawal" },
                    new TransactionType { TransactionTypeId = 3, Name = "Transfer" }
                };

                foreach (var type in fallbackTypes)
                {
                    TransactionTypes.Add(type);
                }

                if (TransactionTypes.Count > 0)
                {
                    SelectedTransactionType = TransactionTypes[0];
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<List<TransactionType>> GetTransactionTypesFromDatabase()
        {
            try
            {
                // Try to get from database first
                var types = await _databaseService.GetAllTransactionTypesAsync();

                // If database is empty, insert default types using the service
                if (types.Count == 0)
                {
                    Console.WriteLine("TransactionTypes table is empty, inserting default types...");
                    var defaultTypes = new List<TransactionType>
                    {
                        new TransactionType { Name = "Deposit" },
                        new TransactionType { Name = "Withdrawal" },
                        new TransactionType { Name = "Transfer" }
                    };

                    // Insert using the service method
                    foreach (var type in defaultTypes)
                    {
                        await _databaseService.InsertTransactionTypeAsync(type);
                    }

                    // Re-fetch from database
                    types = await _databaseService.GetAllTransactionTypesAsync();
                }

                return types;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transaction types from database: {ex.Message}");
                // Fallback to hardcoded types
                return new List<TransactionType>
                {
                    new TransactionType { TransactionTypeId = 1, Name = "Deposit" },
                    new TransactionType { TransactionTypeId = 2, Name = "Withdrawal" },
                    new TransactionType { TransactionTypeId = 3, Name = "Transfer" }
                };
            }
        }

        private async Task SubmitTransaction()
        {
            // Validation
            if (Amount <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Amount must be greater than zero", "OK");
                return;
            }

            if (SelectedTransactionType == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please select a transaction type", "OK");
                return;
            }

            // Get fresh account data for validation to ensure we have current balance
            var freshAccount = await _databaseService.GetAccountByIdAsync(Account.AccountId);
            if (freshAccount == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Account not found", "OK");
                return;
            }

            // Check for sufficient funds for withdrawals
            if (SelectedTransactionType.TransactionTypeId == 2 && Amount > freshAccount.AccountBalance)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Insufficient funds", "OK");
                return;
            }

            IsBusy = true;
            ((Command)SubmitTransactionCommand).ChangeCanExecute();

            try
            {
                var transaction = new Transaction
                {
                    AccountId = Account.AccountId,
                    TransactionTypeId = SelectedTransactionType.TransactionTypeId,
                    Amount = Amount,
                    Description = Description,
                    TransactionDate = DateTime.Now
                };

                Console.WriteLine($"Submitting transaction: {transaction.Amount:C} for account {transaction.AccountId}, type {transaction.TransactionTypeId}");
                Console.WriteLine($"Current account balance: {freshAccount.AccountBalance:C}");

                var success = await _databaseService.MakeTransactionAsync(transaction);

                if (success)
                {
                    // Use the balance verification service to ensure data integrity
                    var updatedAccount = await _balanceVerificationService.VerifyBalanceAfterTransactionAsync(Account.AccountId);

                    if (updatedAccount != null)
                    {
                        // Balance was corrected, use the corrected account
                        _refreshService.NotifyAccountUpdated(updatedAccount);
                        await Application.Current.MainPage.DisplayAlert("Success",
                            $"Transaction completed successfully. New balance: {updatedAccount.AccountBalance:C}", "OK");
                    }
                    else
                    {
                        // Balance was already correct, just notify about transactions
                        _refreshService.NotifyTransactionsUpdated();
                        await Application.Current.MainPage.DisplayAlert("Success",
                            "Transaction completed successfully", "OK");
                    }

                    await _navigationService.GoBackAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Transaction failed. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting transaction: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                await Application.Current.MainPage.DisplayAlert("Error", $"Transaction failed: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                ((Command)SubmitTransactionCommand).ChangeCanExecute();
            }
        }

        // Helper method to update command executability
        private void UpdateCommandStates()
        {
            ((Command)SubmitTransactionCommand).ChangeCanExecute();
        }

        protected void OnViewModelPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);

            // Update command states when properties change that affect them
            if (propertyName == nameof(IsBusy))
            {
                UpdateCommandStates();
            }
        }
    }
}