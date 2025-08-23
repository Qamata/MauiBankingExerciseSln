using MauiBankingExercise.Models;

namespace MauiBankingExercise.Services
{
    public interface IDataRefreshService
    {
        event EventHandler<AccountUpdatedEventArgs> AccountUpdated;
        event EventHandler TransactionsUpdated;
        void NotifyAccountUpdated(Account account);
        void NotifyTransactionsUpdated();
    }

    public class AccountUpdatedEventArgs : EventArgs
    {
        public Account Account { get; set; }
        public AccountUpdatedEventArgs(Account account)
        {
            Account = account;
        }
    }
}