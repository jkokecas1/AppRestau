using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
namespace AppResta.Data
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _connection;

        public Database(string path) { 
            _connection =  new SQLiteAsyncConnection(path);
            
            _connection.CreateTableAsync<Model.Platillos>();
            _connection.CreateTableAsync<Model.Empleado>();
            _connection.CreateTableAsync<Model.Ordenes>();
        }

        #region PLATILLOS
        public Task<List<Model.Platillos>> GetPlatilloAsync() { 
            return _connection.Table<Model.Platillos>().ToListAsync();
        }

        public Task<int> SavePlatilloAsync(Model.Platillos platillo)
        {
            return _connection.InsertAsync(platillo);
        }
        
        #endregion

        #region EMPLEADOS
        public Task<List<Model.Empleado>> GetEmpleadoAsync()
        {
            return _connection.Table<Model.Empleado>().ToListAsync();
        }

        public Task<int> SaveEmpleadoAsync(Model.Empleado empelado)
        {
            return _connection.InsertAsync(empelado);
        }

        
        #endregion


        #region ORDEN
        public Task<List<Model.Ordenes>> GetOrdenesAsync()
        {
            return _connection.Table<Model.Ordenes>().ToListAsync();
        }

        public Task<int> SaveOrdenesAsync(Model.Ordenes orden)
        {
            return _connection.InsertAsync(orden);
        }

        #endregion


        #region CART
        public Task<List<Model.Cart>> GetCartAsync()
        {
            return _connection.Table<Model.Cart>().ToListAsync();
        }

        public Task<int> SaveCartAsync(Model.Cart carrito)
        {
            return _connection.InsertAsync(carrito);
        }
        #endregion


        #region CARTITEMS
        public Task<List<Model.CartItems>> GetCartItemsAsync()
        {
            return _connection.Table<Model.CartItems>().ToListAsync();
        }

        public Task<int> SaveCartItemsAsync(Model.CartItems itsm)
        {
            return _connection.InsertAsync(itsm);
        }
        #endregion

        #region MESAS
        public Task<List<Model.Mesas>> GetMesaAsync()
        {
            return _connection.Table<Model.Mesas>().ToListAsync();
        }

        public Task<int> SaveMesaAsync(Model.Mesas mesa)
        {
            return _connection.InsertAsync(mesa);
        }
        #endregion
        #region P

        #endregion
    }
}
