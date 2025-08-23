using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views.Pages;

public partial class CustomerDashboardPage : ContentPage
{
    public CustomerDashboardPage(CustomerDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}