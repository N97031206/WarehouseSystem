using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WarehouseSystem.Models.DbContextFactory
{
    public class DbContextFactory : IDbContextFactory
    {
        private string _connectionString = string.Empty;

        public DbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        private DbContext _dbContext;
        private DbContext dbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    Type T = typeof(DbContext);
                    this._dbContext = (DbContext)Activator.CreateInstance(T, this._connectionString);
                }
                return _dbContext;
            }
        }

        public DbContext GetDbContext()
        {
            return this.dbContext;
        }
    }
}