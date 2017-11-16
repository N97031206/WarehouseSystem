using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace WarehouseSystem.Models.Interface
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        void Create(TEntity instance);
        void Update(TEntity instance);
        void Update(TEntity instance, Dictionary<string, object> DicPropertyValue);
        void Delete(TEntity instance);
        TEntity Get(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAll();
        void SaveChanges();
    }
}