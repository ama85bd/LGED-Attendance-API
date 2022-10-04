using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Data.Base;
using LGED.Model.Context;
using LGED.Model.Entities.Attendance;

namespace LGED.Data.Repository.Attendance
{
    public class AttendanceWithImageRepository : RepositoryBase<AttendanceWithImage>
    {
        public AttendanceWithImageRepository(LgedDbContext context) : base(context)
        {
        }
    }
}