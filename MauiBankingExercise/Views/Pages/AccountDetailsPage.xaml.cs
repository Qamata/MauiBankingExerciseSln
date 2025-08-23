// Views/Pages/AccountDetailsPage.xaml.cs
using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views.Pages
{
    public partial class AccountDetailsPage : ContentPage
    {
        public AccountDetailsPage(AccountDetailsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}