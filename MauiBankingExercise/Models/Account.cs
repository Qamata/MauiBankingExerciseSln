// Models/Account.cs
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiBankingExercise.Models
{
    public class Account : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int AccountId { get; set; }

        public string AccountNumber { get; set; }

        [ForeignKey(typeof(AccountType))]
        public int AccountTypeId { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey(typeof(Customer))]
        public int CustomerId { get; set; }

        public DateTime DateOpened { get; set; }

        private decimal _accountBalance;
        public decimal AccountBalance
        {
            get => _accountBalance;
            set
            {
                if (_accountBalance != value)
                {
                    _accountBalance = value;
                    OnPropertyChanged();
                }
            }
        }

        [ManyToOne]
        public Customer Customer { get; set; }

        [ManyToOne]
        public AccountType AccountType { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Transaction> Transactions { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}