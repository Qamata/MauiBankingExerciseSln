using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBankingExercise.Services
{
    public class BankDatabaseService
    {
        private static BankDatabaseService _instance;

        public static BankDatabaseService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new BankDatabaseService();
            }
            return _instance;
        }



        private SQLiteConnection _dbConnection;

        public string GetDatabasePath()
        {
            string filename = "pokemon.db";
            string pathToDb = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(pathToDb, filename);
        }

        private void ExtractDbEmbeddedResource()
        {
            var assembly = typeof(BankDatabaseService).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MauiPokemon.EmbeddedDb.pokemon.db");

            var path = GetDatabasePath();

            if (stream != null)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        BinaryWriter bw = new BinaryWriter(fs);
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        bw.Write(bytes);
                    }
                }
            }
        }

        public BankDatabaseService()
        {
            if (!File.Exists(GetDatabasePath()))
                ExtractDbEmbeddedResource();

            _dbConnection = new SQLiteConnection(GetDatabasePath());
        }

        public List<Bank> GetAllPokemon()
        {

            return _dbConnection.Table<Bank>().ToList();
        }

        public Ability GetAbility(int id)
        {
            return _dbConnection.Table<Ability>().Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
