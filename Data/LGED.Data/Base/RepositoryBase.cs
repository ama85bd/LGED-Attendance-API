using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace LGED.Data.Base
{
    public class RepositoryBase<T> : IRepository<T> where T : class, IEntityBase
    {
        readonly LgedDbContext DbContext;
        readonly DbSet<T> DbSet;
        public RepositoryBase(LgedDbContext context)
        {
            DbContext = context;
            DbSet = DbContext.Set<T>();
        }

        public void Add(IUserContext context, T entity)
        {
            var currentDateTime = DateTime.Now;
            entity.InsertedBy = context.UserId;
            entity.InsertedAt = currentDateTime;
            entity.UpdatedBy = context.UserId;
            entity.UpdatedAt = currentDateTime;
            entity.IsDeleted = false;

            //no need to use async method here because all Add request will be wrapped by a Repository.Commit
            //https://stackoverflow.com/questions/47135262/addasync-vs-add-in-ef-core
            //should use Repository.CommitAsync in case of asynchronous save
            DbSet.Add(entity);
        }

         public void Delete(IUserContext context, T entity)
        {
            entity.UpdatedBy = context.UserId;
            entity.UpdatedAt = DateTime.Now;
            entity.IsDeleted = true;
            DbSet.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(IUserContext context, Expression<Func<T, bool>> where)
        {
            var entries = DbSet.Where(where).AsEnumerable();
            foreach (var entity in entries)
            {
                entity.UpdatedBy = context.UserId;
                entity.UpdatedAt = DateTime.Now;
                entity.IsDeleted = true;

                DbSet.Attach(entity);
                DbContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public void Remove(T entity)
        {
            DbContext.Remove(entity);
        }

        public bool Exist(Expression<Func<T, bool>> where)
        {
            try
            {
                return DbSet.Where(t => t.IsDeleted == false).Any(where);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool ExistById(Guid id)
        {
            try
            {
                return DbSet.Where(t => t.IsDeleted == false).Any(c => c.Id == id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(t => t.IsDeleted == false).Where(where).FirstOrDefault();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.Where(t => t.IsDeleted == false).Where(where).FirstOrDefaultAsync();
        }
        public T GetById(Guid id)
        {
            return DbSet.Find(id);
        }

        public IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(t => t.IsDeleted == false).Where(where).ToList();
        }

        public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.Where(t => t.IsDeleted == false).Where(where).ToListAsync();
        }

        public IQueryable<T> GetQuery()
        {
            return DbSet.Where(t => t.IsDeleted == false);
        }

          //use this function for query read-only data (like in search, sort, filter...)
        public IQueryable<T> GetQueryNoCached()
        {
            return DbSet.Where(t => t.IsDeleted == false).AsNoTracking();
        }

        public IQueryable<T> GetQueryWithDeleted()
        {
            return DbSet.AsQueryable<T>();
        }

        public void Save(IUserContext context, T entity)
        {
            try
            {
                if (ExistById(entity.Id))
                {
                    Update(context, entity);
                }
                else
                {
                    Add(context, entity);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Update(IUserContext context, T entity)
        {
            try
            {
                entity.UpdatedBy = context.UserId;
                entity.UpdatedAt = DateTime.Now;
                DbSet.Attach(entity);
                DbContext.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // Update without context, not save update by and update at
        public void Update(T entity)
        {
            try
            {
                DbSet.Attach(entity);
                DbContext.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}