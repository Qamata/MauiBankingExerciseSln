// Views/Pages/TransactionPage.xaml.cs
using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views.Pages
{
    public partial class TransactionPage : ContentPage
    {
        public TransactionPage(TransactionViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}