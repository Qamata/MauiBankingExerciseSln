using MauiBankingExercise.Models;

namespace MauiBankingExercise.Views.Controls;

public partial class TransactionCard : ContentView
{
    public static readonly BindableProperty TransactionProperty = BindableProperty.Create(
        nameof(Transaction), typeof(Transaction), typeof(TransactionCard), null);

    public Transaction Transaction
    {
        get => (Transaction)GetValue(TransactionProperty);
        set => SetValue(TransactionProperty, value);
    }

    public string TransactionIcon => Transaction?.TransactionType switch
    {
       /* "Deposit" => "?",
        "Withdrawal" => "?",
        "Transfer" => "?",
        _ => "??"*/
    };

    public Color IconBackgroundColor => Transaction?.Amount >= 0 ? Colors.Green : Colors.Red;

    public Color AmountTextColor => Transaction?.Amount >= 0
        ? Color.FromArgb("#2E7D32")
        : Color.FromArgb("#D32F2F");

    public TransactionCard()
    {
        InitializeComponent();
    }
}