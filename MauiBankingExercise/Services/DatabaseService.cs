// Services/DatabaseService.cs
using MauiBankingExercise.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MauiBankingExercise.Services
{
    public class DatabaseService : IDatabaseService
    {
        private static DatabaseService _instance;
        private SQLiteConnection _dbConnection;

        public static DatabaseService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DatabaseService();
            }
            return _instance;
        }

        public string GetDatabasePath()
        {
            string filename = "bank.db";
            string pathToDb = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(pathToDb, filename);
        }

        // In DatabaseService.cs - Update ExtractDbEmbeddedResource method
        private void ExtractDbEmbeddedResource()
        {
            try
            {
                Console.WriteLine("=== EXTRACTING EMBEDDED DATABASE ===");
                var assembly = typeof(DatabaseService).GetTypeInfo().Assembly;

                // List all embedded resources to verify the database is there
                var resourceNames = assembly.GetManifestResourceNames();
                Console.WriteLine("Available embedded resources:");
                foreach (var name in resourceNames)
                {
                    Console.WriteLine($"- {name}");
                }

                Stream stream = assembly.GetManifestResourceStream("MauiBankingExercise.EmbeddedDb.bank.db");

                if (stream == null)
                {
                    Console.WriteLine("ERROR: Embedded database resource not found!");
                    Console.WriteLine("Looking for: MauiBankingExercise.EmbeddedDb.bank.db");
                    return;
                }

                Console.WriteLine($"Embedded database found, length: {stream.Length} bytes");

                var path = GetDatabasePath();
                Console.WriteLine($"Extracting to: {path}");

                using (BinaryReader br = new BinaryReader(stream))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        BinaryWriter bw = new BinaryWriter(fs);
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        bw.Write(bytes);
                        Console.WriteLine($"Database extracted successfully. File size: {new FileInfo(path).Length} bytes");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public DatabaseService()
        {
            Console.WriteLine("Initializing DatabaseService...");
            Console.WriteLine($"Database path: {GetDatabasePath()}");

            // Example: Always overwrite for debugging
            if (File.Exists(GetDatabasePath()))
                File.Delete(GetDatabasePath());
            ExtractDbEmbeddedResource();

            _dbConnection = new SQLiteConnection(GetDatabasePath());
            Console.WriteLine("SQLite connection created successfully");

            DebugDatabaseContents();
        }

        public async Task InitializeDatabaseAsync()
        {
            // This method is kept for interface compatibility
            Console.WriteLine("Database initialization completed");
            await Task.CompletedTask;
        }

        // In DatabaseService.cs - Update the HasData method
        public bool HasData()
        {
            try
            {
                Console.WriteLine("=== CHECKING DATABASE DATA ===");
                string dbPath = GetDatabasePath();
                Console.WriteLine($"Database path: {dbPath}");
                Console.WriteLine($"Database exists: {File.Exists(dbPath)}");

                if (!File.Exists(dbPath))
                {
                    Console.WriteLine("Database file does not exist!");
                    return false;
                }

                // Check file size
                var fileInfo = new FileInfo(dbPath);
                Console.WriteLine($"Database file size: {fileInfo.Length} bytes");

                if (fileInfo.Length == 0)
                {
                    Console.WriteLine("Database file is empty!");
                    return false;
                }

                // Try to access the Customer table
                try
                {
                    var tableInfo = _dbConnection.GetTableInfo("Customer");
                    Console.WriteLine($"Customer table exists: {tableInfo.Any()}");

                    if (tableInfo.Any())
                    {
                        var customerCount = _dbConnection.Table<Customer>().Count();
                        Console.WriteLine($"Number of customers in database: {customerCount}");
                        return customerCount > 0;
                    }
                    else
                    {
                        Console.WriteLine("Customer table does not exist in database");
                        return false;
                    }
                }
                catch (Exception tableEx)
                {
                    Console.WriteLine($"Error accessing Customer table: {tableEx.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HasData: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            try
            {
                Console.WriteLine("Getting customers with raw SQL...");

                // Use raw SQL query - this bypasses all the relationship mapping issues
                var customers = _dbConnection.Query<Customer>("SELECT * FROM Customer");
                Console.WriteLine($"Raw SQL query returned: {customers.Count} customers");

                return await Task.FromResult(customers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in raw SQL query: {ex.Message}");
                return await Task.FromResult(new List<Customer>());
            }
        }

        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                var customer = _dbConnection.Table<Customer>()
                    .FirstOrDefault(c => c.CustomerId == customerId);
                return await Task.FromResult(customer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customer by ID: {ex.Message}");
                return await Task.FromResult<Customer>(null);
            }
        }

        public async Task<List<Account>> GetCustomerAccountsAsync(int customerId)
        {
            try
            {
                var accounts = _dbConnection.Table<Account>()
                    .Where(a => a.CustomerId == customerId && a.IsActive)
                    .ToList();
                return await Task.FromResult(accounts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting customer accounts: {ex.Message}");
                return await Task.FromResult(new List<Account>());
            }
        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            try
            {
                var account = _dbConnection.Table<Account>()
                    .FirstOrDefault(a => a.AccountId == accountId);
                return await Task.FromResult(account);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting account by ID: {ex.Message}");
                return await Task.FromResult<Account>(null);
            }
        }

        public async Task<List<Transaction>> GetAccountTransactionsAsync(int accountId)
        {
            try
            {
                var transactions = _dbConnection.Table<Transaction>()
                    .Where(t => t.AccountId == accountId)
                    .ToList();
                return await Task.FromResult(transactions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting account transactions: {ex.Message}");
                return await Task.FromResult(new List<Transaction>());
            }
        }

        // In DatabaseService.cs - Add this comprehensive debug method
        private void DebugDatabaseContents()
        {
            try
            {
                Console.WriteLine("=== COMPREHENSIVE DATABASE DEBUG ===");

                // 1. Check if database file exists and its size
                string dbPath = GetDatabasePath();
                Console.WriteLine($"Database path: {dbPath}");
                Console.WriteLine($"Database exists: {File.Exists(dbPath)}");

                if (File.Exists(dbPath))
                {
                    var fileInfo = new FileInfo(dbPath);
                    Console.WriteLine($"Database size: {fileInfo.Length} bytes");
                }

                // 2. List all tables in the database
                Console.WriteLine("\n=== DATABASE TABLES ===");
                var tables = _dbConnection.Query<dynamic>("SELECT name FROM sqlite_master WHERE type='table'");
                foreach (var table in tables)
                {
                    Console.WriteLine($"Table: {table.name}");
                }

                // 3. Check Customer table structure
                Console.WriteLine("\n=== CUSTOMER TABLE STRUCTURE ===");
                var customerColumns = _dbConnection.Query<dynamic>("PRAGMA table_info(Customer)");
                foreach (var column in customerColumns)
                {
                    Console.WriteLine($"Column: {column.name} ({column.type}) PK: {column.pk}");
                }

                // 4. Try different ways to query customers
                Console.WriteLine("\n=== QUERYING CUSTOMERS ===");

                // Method 1: Raw SQL
                Console.WriteLine("Method 1: Raw SQL query");
                var rawCustomers = _dbConnection.Query<dynamic>("SELECT * FROM Customer");
                Console.WriteLine($"Raw SQL returned: {rawCustomers.Count} rows");

                if (rawCustomers.Count > 0)
                {
                    Console.WriteLine("First customer raw data:");
                    var firstCustomer = rawCustomers[0];
                    var dict = (IDictionary<string, object>)firstCustomer;
                    foreach (var key in dict.Keys)
                    {
                        Console.WriteLine($"  {key}: {dict[key]}");
                    }
                }

                // Method 2: Table<T> with basic Customer class (no relationships)
                Console.WriteLine("\nMethod 2: Table<Customer>");
                try
                {
                    var tableCustomers = _dbConnection.Table<Customer>().ToList();
                    Console.WriteLine($"Table<Customer> returned: {tableCustomers.Count} customers");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Table<Customer> error: {ex.Message}");
                }

                // Method 3: Manual mapping
                Console.WriteLine("\nMethod 3: Manual mapping from raw SQL");
                var manualCustomers = _dbConnection.Query<dynamic>("SELECT * FROM Customer")
                    .Select(row => new Customer
                    {
                        CustomerId = Convert.ToInt32(row.CustomerId),
                        FirstName = row.FirstName?.ToString(),
                        LastName = row.LastName?.ToString(),
                        Email = row.Email?.ToString(),
                        PhoneNumber = row.PhoneNumber?.ToString(),
                        // Add other properties as needed
                    }).ToList();
                Console.WriteLine($"Manual mapping returned: {manualCustomers.Count} customers");

                // 5. Check if there are any records at all
                Console.WriteLine("\n=== RECORD COUNTS ===");
                var tableCounts = _dbConnection.Query<dynamic>("SELECT name FROM sqlite_master WHERE type='table'");
                foreach (var table in tableCounts)
                {
                    string tableName = table.name;
                    try
                    {
                        var count = _dbConnection.ExecuteScalar<int>($"SELECT COUNT(*) FROM {tableName}");
                        Console.WriteLine($"{tableName}: {count} records");
                    }
                    catch
                    {
                        Console.WriteLine($"{tableName}: Could not count records");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Debug error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task<bool> MakeTransactionAsync(Transaction transaction)
        {
            try
            {
                _dbConnection.RunInTransaction(() =>
                {
                    // Insert the transaction
                    _dbConnection.Insert(transaction);

                    // Update the account balance
                    var account = _dbConnection.Table<Account>()
                        .FirstOrDefault(a => a.AccountId == transaction.AccountId);

                    if (account != null)
                    {
                        if (transaction.TransactionTypeId == 1) // Deposit
                        {
                            account.AccountBalance += transaction.Amount;
                        }
                        else if (transaction.TransactionTypeId == 2) // Withdrawal
                        {
                            account.AccountBalance -= transaction.Amount;
                        }

                        _dbConnection.Update(account);
                    }
                });

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error making transaction: {ex.Message}");
                return await Task.FromResult(false);
            }
        }
    }
}