using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;
using LGED.Model.Entities.Student;

namespace LGED.Domain.Commands.StudentCommand
{
    public class CreateUpdateStudentCommand: CommandBase<bool>
    {
        public Student Student { get; set; }
    }
}