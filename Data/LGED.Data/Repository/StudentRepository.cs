using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Data.Base;
using LGED.Model.Context;
using LGED.Model.Entities.Student;

namespace LGED.Data.Repository
{
    public class StudentRepository : RepositoryBase<Student>
    {
        public StudentRepository(LgedDbContext context) : base(context)
        {
        }
    }
}