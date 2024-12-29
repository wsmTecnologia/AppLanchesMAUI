using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSM.AppLanches.UI.Models;

namespace WSM.AppLanches.UI.Services
{
    public class FavoritosService
    {
        private readonly SQLiteAsyncConnection _database;
        public FavoritosService() {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "favorito.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ProdutoFavorito>().Wait();
        }

        public async Task<ProdutoFavorito> ReadAsync(int id) 
        {
            try
            {
                return await _database.Table<ProdutoFavorito>().Where(f => f.ProdutoId == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {

                throw;
            }            
        }

        public async Task<List<ProdutoFavorito>> ReadAllAsync()
        {
            try
            {
                return await _database.Table<ProdutoFavorito>().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public async Task<int> CreateAsync(ProdutoFavorito produtoFavorito)
        {
            try
            {
                return await _database.InsertAsync(produtoFavorito);
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public async Task<int> DeleteAsync(ProdutoFavorito produtoFavorito)
        {
            try
            {
                return await _database.DeleteAsync(produtoFavorito);
            }
            catch (Exception)
            {
                throw;
            }           
        }
    }
}
