using MauiBankingExercise.Models;

namespace MauiBankingExercise.Services
{
    public interface IBalanceVerificationService
    {
        Task<bool> VerifyAndCorrectAccountBalanceAsync(int accountId);
        Task VerifyAllAccountBalancesAsync();
        Task<Account> VerifyBalanceAfterTransactionAsync(int accountId);
    }
}