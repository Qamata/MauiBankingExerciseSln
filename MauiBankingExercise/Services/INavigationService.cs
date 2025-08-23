// Services/INavigationService.cs
using MauiBankingExercise.Models;
using System.Threading.Tasks;

namespace MauiBankingExercise.Services
{
    public interface INavigationService
    {
        Task NavigateToDashboardAsync(Customer customer);
        Task NavigateToAccountDetailsAsync(Account account);
        Task NavigateToTransactionPageAsync(Account account);
        Task GoBackAsync();
    }
}