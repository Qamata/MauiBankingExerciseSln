using MauiBankingExercise.Models;

namespace MauiBankingExercise.Views.Controls
{
    public partial class TransactionCard : ContentView
    {
        public static readonly BindableProperty TransactionProperty =
            BindableProperty.Create(nameof(Transaction), typeof(Transaction), typeof(TransactionCard), null);

        public Transaction Transaction
        {
            get => (Transaction)GetValue(TransactionProperty);
            set => SetValue(TransactionProperty, value);
        }

        public TransactionCard()
        {
            InitializeComponent();
        }
    }
}