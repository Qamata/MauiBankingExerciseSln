using MauiBankingExercise.Models;

namespace MauiBankingExercise.Views.Controls;

public partial class CustomerCard : ContentView
{
    public static readonly BindableProperty CustomerProperty = BindableProperty.Create(
        nameof(Customer), typeof(Customer), typeof(CustomerCard), null);

    public Customer Customer
    {
        get => (Customer)GetValue(CustomerProperty);
        set => SetValue(CustomerProperty, value);
    }

    public CustomerCard()
    {
        InitializeComponent();
    }
}