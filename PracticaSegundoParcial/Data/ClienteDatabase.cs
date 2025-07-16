using PracticaSegundoParcial.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaSegundoParcial.Data
{
    public class ClienteDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public ClienteDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Cliente>().Wait();
        }

        public Task<int> SaveClienteAsync(Cliente cliente)
        {
            return _database.InsertAsync(cliente);
        }

        public Task<List<Cliente>> GetClientesAsync()
        {
            return _database.Table<Cliente>().ToListAsync();
        }
    }
}