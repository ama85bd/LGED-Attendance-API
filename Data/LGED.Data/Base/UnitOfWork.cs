using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using LGED.Data.Repository;
using LGED.Data.Repository.Profile;
using LGED.Model.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LGED.Data.Base
{
    public class UnitOfWork: IUnitOfWork
    {
        

        public LgedDbContext LgedDbContext { get; set; }
        public UnitOfWork(LgedDbContext context)
        {
            LgedDbContext=context;
        }

        private StudentRepository _studentRepository;

         public StudentRepository StudentRepository
        {
            get { return _studentRepository ??= new StudentRepository(LgedDbContext); }
        }
        private UserRepository _userRepository;
         public UserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(LgedDbContext); }
        }
        private UserRolesRepository _userRolesRepository;
          public UserRolesRepository UserRolesRepository
        {
            get { return _userRolesRepository ??= new UserRolesRepository(LgedDbContext); }
        }
        private RoleRepository _roleRepository;
          public RoleRepository RoleRepository
        {
            get { return _roleRepository ??= new RoleRepository(LgedDbContext); }
        }
        private CompanyRepository _companyRepository;
          public CompanyRepository CompanyRepository
        {
            get { return _companyRepository ??= new CompanyRepository(LgedDbContext); }
        }


        public bool Commit()
        {
            return LgedDbContext.SaveChanges() > 0;
        }

        public async Task<bool> CommitAsync()
        {
            return await LgedDbContext.SaveChangesAsync() > 0;
        }

        public int ExecuteSqlCommand(string sql, object parameters = null)
        {
            using var connection = new SqlConnection(LgedDbContext.Database.GetConnectionString());
            connection.Open();
            return connection.Execute(sql, parameters);
        }
        
        public async Task<int> ExecuteSqlCommandAsync(string sql, object parameters = null)
        {
            await using var connection = new SqlConnection(LgedDbContext.Database.GetConnectionString());
            connection.Open();
            return await connection.ExecuteAsync(sql, parameters);
        }
        
        public IEnumerable<T> ExecuteSqlQuery<T>(string sql, object parameters = null) where T : class
        {
            using var connection = new SqlConnection(LgedDbContext.Database.GetConnectionString());
            connection.Open();
            return connection.Query<T>(sql, parameters);
        }

        public async Task<IEnumerable<T>> ExecuteSqlQueryAsync<T>(string sql, object parameters = null) where T : class
        {
            await using var connection = new SqlConnection(LgedDbContext.Database.GetConnectionString());
            connection.Open();
            return await connection.QueryAsync<T>(sql, parameters);
        }

        public void Dispose()
        {
            LgedDbContext?.Dispose();
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}