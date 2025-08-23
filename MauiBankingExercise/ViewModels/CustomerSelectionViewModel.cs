
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MauiBankingExercise.ViewModels
{
    public class CustomerSelectionViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly IDataRefreshService _refreshService;

        public ObservableCollection<Customer> Customers { get; } = new ObservableCollection<Customer>();

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                SetProperty(ref _selectedCustomer, value);
                if (value != null)
                {
                    NavigateToDashboard(value);
                }
            }
        }
        

        public ICommand LoadCustomersCommand { get; }

        public CustomerSelectionViewModel(IDatabaseService databaseService, INavigationService navigationService,
                                IDataRefreshService refreshService)
        {
            _refreshService = refreshService;
            _databaseService = databaseService;
            _navigationService = navigationService;
            LoadCustomersCommand = new Command(async () => await LoadCustomers());
        }

        public async Task Initialize()
        {
            await LoadCustomers();
        }

        public async Task LoadCustomers()
        {
            IsBusy = true;
            try
            {
                Customers.Clear();
                var customers = await _databaseService.GetAllCustomersAsync();
                foreach (var customer in customers)
                {
                    Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                // Handle error - you might want to add error properties to BaseViewModel
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load customers: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void NavigateToDashboard(Customer customer)
        {
            await _navigationService.NavigateToDashboardAsync(customer);
        }
    }
}