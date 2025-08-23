using MauiBankingExercise.Models;

namespace MauiBankingExercise.Services
{
    public class DataRefreshService : IDataRefreshService
    {
        public event EventHandler<AccountUpdatedEventArgs> AccountUpdated;
        public event EventHandler TransactionsUpdated;

        public void NotifyAccountUpdated(Account account)
        {
            AccountUpdated?.Invoke(this, new AccountUpdatedEventArgs(account));
        }

        public void NotifyTransactionsUpdated()
        {
            TransactionsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}