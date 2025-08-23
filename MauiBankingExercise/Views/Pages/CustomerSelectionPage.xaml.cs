// Views/Pages/CustomerSelectionPage.xaml.cs
using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views.Pages
{
    public partial class CustomerSelectionPage : ContentPage
    {
        private readonly CustomerSelectionViewModel _viewModel;

        public CustomerSelectionPage(CustomerSelectionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Clear selection when returning to this page
            if (collectionView != null)
            {
                collectionView.SelectedItem = null;
            }

            // Load customers if the collection is empty
            if (_viewModel.Customers.Count == 0)
            {
                await _viewModel.LoadCustomers();
            }
        }
    }
}