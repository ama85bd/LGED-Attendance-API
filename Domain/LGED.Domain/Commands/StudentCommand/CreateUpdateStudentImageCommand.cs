using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;
using LGED.Model.Entities.Student;

namespace LGED.Domain.Commands.StudentCommand
{
    public class CreateUpdateStudentImageCommand : CommandBase<bool>
    {
        public StudentImage StudentImage { get; set; }
    }
}