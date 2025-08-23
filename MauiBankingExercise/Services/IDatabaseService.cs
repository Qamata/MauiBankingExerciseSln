// Services/IDatabaseService.cs
using MauiBankingExercise.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiBankingExercise.Services
{
    public interface IDatabaseService
    {
        Task InitializeDatabaseAsync();
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int customerId);
        Task<List<Account>> GetCustomerAccountsAsync(int customerId);
        Task<Account> GetAccountByIdAsync(int accountId);
        Task<List<Transaction>> GetAccountTransactionsAsync(int accountId);
        Task<bool> MakeTransactionAsync(Transaction transaction);
        bool HasData();
    }
}