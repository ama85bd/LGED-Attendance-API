using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Data.Repository;
using LGED.Model.Context;

namespace LGED.Data.Base
{
    public interface IUnitOfWork: IDisposable, ICloneable
    {
        public LgedDbContext LgedDbContext { get; set; }

        StudentRepository StudentRepository { get; }

         bool Commit();
         Task<bool> CommitAsync();

         int ExecuteSqlCommand(string sql, object parameters = null);
         Task<int> ExecuteSqlCommandAsync(string sql, object parameters = null);

         IEnumerable<T> ExecuteSqlQuery<T>(string sql, object parameters = null) where T : class;

         Task<IEnumerable<T>> ExecuteSqlQueryAsync<T>(string sql, object parameters = null) where T : class;
    }
}