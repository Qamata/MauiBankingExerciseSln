// Helpers/Constants.cs
using SQLite;

namespace MauiBankingExercise.Helpers
{
    public static class Constants
    {
        public const string DatabaseFilename = "bank.db";

        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}