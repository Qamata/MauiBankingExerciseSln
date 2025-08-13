using MauiBankingExercise.Models;
using SQLite;
using System;
using System.Collections.Generic;

namespace MauiBankingExercise.Services
{


    public static class BankingSeeder
    {
        public static void Seed(SQLiteConnection db)
        {
            // Ensure tables are created  
            db.CreateTable<Bank>();
            db.CreateTable<CustomerType>();
            db.CreateTable<Customer>();
            db.CreateTable<AccountType>();
            db.CreateTable<Account>();
            db.CreateTable<TransactionType>();
            db.CreateTable<Transaction>();
            db.CreateTable<AuthType>();
            db.CreateTable<Auth>();
            db.CreateTable<AssetType>();
            db.CreateTable<Asset>();

            // Seed CustomerTypes  
            var customerTypes = new List<CustomerType>
        {
            new CustomerType { Name = "Individual" },
            new CustomerType { Name = "Business" }
        };
            foreach (var ct in customerTypes) db.Insert(ct);

            // Seed AccountTypes  
            var accountTypes = new List<AccountType>
        {
            new AccountType { Name = "Savings" },
            new AccountType { Name = "Checking" }
        };
            foreach (var at in accountTypes) db.Insert(at);

            // Seed TransactionTypes  
            var transactionTypes = new List<TransactionType>
        {
            new TransactionType { Name = "Deposit" },
            new TransactionType { Name = "Withdrawal" }
        };
            foreach (var tt in transactionTypes) db.Insert(tt);

            // Seed AuthTypes  
            var authTypes = new List<AuthType>
        {
            new AuthType { Name = "Password" },
            new AuthType { Name = "OTP" }
        };
            foreach (var autht in authTypes) db.Insert(autht);

            // Seed AssetTypes  
            var assetTypes = new List<AssetType>
        {
            new AssetType { Name = "Property" },
            new AssetType { Name = "Vehicle" }
        };
            foreach (var ast in assetTypes) db.Insert(ast);

            // Seed Banks  
            var banks = new List<Bank>
        {
            new Bank
            {
                BankName = "Alpha Bank",
                BankAddress = "123 Main St, Metropolis",
                BranchCode = "AB001",
                ContactPhoneNumber = "555-1000",
                ContactEmail = "contact@alphabank.com",
                IsActive = true,
                OperatingHours = "9am-5pm"
            },
            new Bank
            {
                BankName = "Beta Bank",
                BankAddress = "456 Oak Ave, Gotham",
                BranchCode = "BB002",
                ContactPhoneNumber = "555-2000",
                ContactEmail = "info@betabank.com",
                IsActive = true,
                OperatingHours = "8am-4pm"
            }
        };
            foreach (var bank in banks) db.Insert(bank);

            // Seed Customers  
            var customers = new List<Customer>
        {
            new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@email.com",
                PhoneNumber = "555-1234",
                PhysicalAddress = "101 Maple Street",
                IdentityNumber = "A123456789",
                CustomerTypeId = customerTypes[0].CustomerTypeId,
                GenderTypeId = 1,
                RaceTypeId = 1,
                Nationality = "USA",
                MaritalStatusId = 2,
                EmploymentStatusId = 1,
                BankId = banks[0].BankId
            },
            new Customer
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@email.com",
                PhoneNumber = "555-5678",
                PhysicalAddress = "202 Pine Avenue",
                IdentityNumber = "B987654321",
                CustomerTypeId = customerTypes[1].CustomerTypeId,
                GenderTypeId = 2,
                RaceTypeId = 2,
                Nationality = "USA",
                MaritalStatusId = 1,
                EmploymentStatusId = 2,
                BankId = banks[1].BankId
            }
        };
            foreach (var cust in customers) db.Insert(cust);

            // Seed Accounts  
            var accounts = new List<Account>
        {
            new Account
            {
                AccountNumber = "111111",
                AccountTypeId = accountTypes[0].AccountTypeId,
                IsActive = true,
                CustomerId = customers[0].CustomerId,
                DateOpened = DateTime.Now.AddYears(-2),
                AccountBalance = 2500.50m
            },
            new Account
            {
                AccountNumber = "222222",
                AccountTypeId = accountTypes[1].AccountTypeId,
                IsActive = true,
                CustomerId = customers[1].CustomerId,
                DateOpened = DateTime.Now.AddYears(-1),
                AccountBalance = 3500.75m
            }
        };
            foreach (var acc in accounts) db.Insert(acc);

            // Seed Transactions  
            var transactions = new List<Transaction>
        {
            new Transaction
            {
                TransactionTypeId = transactionTypes[0].TransactionTypeId,
                AccountId = accounts[0].AccountId,
                TransactionDate = DateTime.Now.AddDays(-10),
                Amount = 1000m,
                Description = "Initial deposit"
            },
            new Transaction
            {
                TransactionTypeId = transactionTypes[1].TransactionTypeId,
                AccountId = accounts[1].AccountId,
                TransactionDate = DateTime.Now.AddDays(-5),
                Amount = -500m,
                Description = "ATM withdrawal"
            }
        };
            foreach (var trans in transactions) db.Insert(trans);

            // Seed Auths  
            var auths = new List<Auth>
        {
            new Auth
            {
                CustomerId = customers[0].CustomerId,
                UserName = "john.doe@email.com",
                Password = "pass123",
                AuthTypeId = authTypes[0].AuthTypeId
            },
            new Auth
            {
                CustomerId = customers[1].CustomerId,
                UserName = "jane.smith@email.com",
                Password = "smith456",
                AuthTypeId = authTypes[1].AuthTypeId
            }
        };
            foreach (var auth in auths) db.Insert(auth);

            // Seed Assets  
            var assets = new List<Asset>
        {
            new Asset
            {
                AssetTypeId = assetTypes[0].AssetTypeId,
                CustomerId = customers[0].CustomerId,
                Value = 100000m,
                Name = "Home"
            },
            new Asset
            {
                AssetTypeId = assetTypes[1].AssetTypeId,
                CustomerId = customers[1].CustomerId,
                Value = 20000m,
                Name = "Car"
            }
        };
            foreach (var asset in assets) db.Insert(asset);

            // Done!  
            Console.WriteLine("Database seeded with fake banking data.");
        }
    }


}
