using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace slls.DAO
{
    public interface IRepository : IDisposable
    {
        //IEnumerable<Models.Localization> Get();
        IQueryable<T> GetAll<T>() where T : class;
        void Insert<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        IQueryable<T> SearchFor<T>(Expression<Func<T, bool>> predicate) where T : class;
        T GetById<T>(int id) where T : class;
    }
}
