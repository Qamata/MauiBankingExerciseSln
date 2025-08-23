using Microsoft.Maui.Controls;

namespace MauiBankingExercise.Views.Controls
{
    public partial class BalanceCard : ContentView
    {
        public static readonly BindableProperty BalanceProperty =
            BindableProperty.Create(nameof(Balance), typeof(decimal), typeof(BalanceCard), 0m);

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(BalanceCard), string.Empty);

        public static readonly BindableProperty SubtitleProperty =
            BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(BalanceCard), string.Empty);

        public decimal Balance
        {
            get => (decimal)GetValue(BalanceProperty);
            set => SetValue(BalanceProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public BalanceCard()
        {
            InitializeComponent();
        }
    }
}