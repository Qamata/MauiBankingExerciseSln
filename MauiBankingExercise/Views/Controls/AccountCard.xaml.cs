using MauiBankingExercise.Models;
using Microsoft.Maui.Controls;

namespace MauiBankingExercise.Views.Controls
{
    public partial class AccountCard : ContentView
    {
        public static readonly BindableProperty AccountProperty =
            BindableProperty.Create(nameof(Account), typeof(Account), typeof(AccountCard), null);

        public Account Account
        {
            get => (Account)GetValue(AccountProperty);
            set => SetValue(AccountProperty, value);
        }

        public AccountCard()
        {
            InitializeComponent();
        }
    }
}