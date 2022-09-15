using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LGED.Domain.Commands.StudentCommand;
using LGED.Model.Entities.Student;

namespace LGED.Domain.AutoMapper
{
    public class AutoMapperConfigure : Profile
    {
        public AutoMapperConfigure()
        {
            CreateMap<CreateUpdateStudentCommand, Student>();
        }
    }
}