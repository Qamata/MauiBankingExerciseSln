using MauiBankingExercise.Models;
using SQLite;
using MauiBankingExercise.Helpers;



namespace MauiBankingExercise.Services
{
    public class BalanceVerificationService : IBalanceVerificationService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IDataRefreshService _refreshService;
        private readonly SQLiteConnection _dbConnection;

        public BalanceVerificationService(IDatabaseService databaseService, IDataRefreshService refreshService)
        {
            _databaseService = databaseService;
            _refreshService = refreshService;
            _dbConnection = new SQLiteConnection(Constants.DatabasePath, Constants.Flags);
        }

        public async Task<bool> VerifyAndCorrectAccountBalanceAsync(int accountId)
        {
            try
            {
                Console.WriteLine($"=== VERIFYING ACCOUNT BALANCE ===");
                Console.WriteLine($"Account ID: {accountId}");

                // Get transactions for this account
                var transactions = _dbConnection.Table<Transaction>()
                    .Where(t => t.AccountId == accountId)
                    .ToList();

                decimal calculatedBalance = 0;

                foreach (var transaction in transactions)
                {
                    if (transaction.TransactionTypeId == 1) // Deposit
                    {
                        calculatedBalance += transaction.Amount;
                    }
                    else if (transaction.TransactionTypeId == 2) // Withdrawal
                    {
                        calculatedBalance -= transaction.Amount;
                    }
                    else if (transaction.TransactionTypeId == 3) // Transfer
                    {
                        calculatedBalance -= transaction.Amount;
                    }
                }

                // Get the account
                var account = _dbConnection.Table<Account>()
                    .FirstOrDefault(a => a.AccountId == accountId);

                if (account != null)
                {
                    Console.WriteLine($"Database balance: {account.AccountBalance:C}");
                    Console.WriteLine($"Calculated balance: {calculatedBalance:C}");

                    if (account.AccountBalance != calculatedBalance)
                    {
                        Console.WriteLine($"CORRECTING BALANCE: {account.AccountBalance:C} -> {calculatedBalance:C}");

                        // Update the account balance
                        account.AccountBalance = calculatedBalance;
                        _dbConnection.Update(account);

                        // Notify about the correction
                        _refreshService.NotifyAccountUpdated(account);

                        return true;
                    }

                    Console.WriteLine("Balance is correct");
                    return false;
                }

                Console.WriteLine("Account not found");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying account balance: {ex.Message}");
                return false;
            }
        }

        public async Task VerifyAllAccountBalancesAsync()
        {
            try
            {
                Console.WriteLine("=== VERIFYING ALL ACCOUNT BALANCES ===");

                var allAccounts = _dbConnection.Table<Account>().ToList();
                Console.WriteLine($"Found {allAccounts.Count} accounts to verify");

                foreach (var account in allAccounts)
                {
                    await VerifyAndCorrectAccountBalanceAsync(account.AccountId);
                }

                Console.WriteLine("=== ALL ACCOUNT BALANCES VERIFIED ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verifying all account balances: {ex.Message}");
            }
        }

        public async Task<Account> VerifyBalanceAfterTransactionAsync(int accountId)
        {
            var wasCorrected = await VerifyAndCorrectAccountBalanceAsync(accountId);
            if (wasCorrected)
            {
                Console.WriteLine("Balance was corrected, returning fresh account data");
                return _dbConnection.Table<Account>().FirstOrDefault(a => a.AccountId == accountId);
            }
            return null;
        }
    }
}