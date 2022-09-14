using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LGED.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Add(IUserContext context, T entity);
        void Update(IUserContext context, T entity);
        void Update(T entity); // Update without context
        void Save(IUserContext context, T entity);
        void Delete(IUserContext context, T entity);
        void Delete(IUserContext context, Expression<Func<T, bool>> where);
        void Remove(T entity);
        bool Exist(Expression<Func<T, bool>> where);
        bool ExistById(Guid id);
        T GetById(Guid id);
        Task<T> GetByIdAsync(Guid id);
        T Get(Expression<Func<T, bool>> where);
        Task<T> GetAsync(Expression<Func<T, bool>> where);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where);
        //no user login
        IQueryable<T> GetQuery();
        //makes read-only queries faster
        IQueryable<T> GetQueryNoCached();
        IQueryable<T> GetQueryWithDeleted();
    }
}