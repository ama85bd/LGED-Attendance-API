using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LGED.Core.Interfaces;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.StudentCommand;
using LGED.Model.Entities.Student;

namespace LGED.Domain.Handlers.StudentHandler
{
    public class CreateUpdateStudentHandler : HandlerBase<CreateUpdateStudentCommand, bool>
    {
        public CreateUpdateStudentHandler(IUnitOfWork unitOfWork, IUserContext context, IMapper mapper) : base(unitOfWork, context, mapper)
        {
        }

        public override async Task<bool> Handle(CreateUpdateStudentCommand command, CancellationToken cancellationToken)
        {
            var CaseItem = _unitOfWork.StudentRepository.GetQueryNoCached().Where(x => x.StudentId == command.Student.StudentId )
                .FirstOrDefault();

            if(CaseItem == null)
            {
                Student student = new Student
                {
                    StudentId = command.Student.StudentId,
                    FirstName = command.Student.FirstName,
                    LastName = command.Student.LastName,
                    City = command.Student.City,
                    MobileNumber = command.Student.MobileNumber,
                    Email = command.Student.Email,
                };
                _unitOfWork.StudentRepository.Add(_context, student);
            }
            else
            {
                CaseItem.StudentId = command.Student.StudentId;
                    CaseItem.FirstName = command.Student.FirstName;
                    CaseItem.LastName = command.Student.LastName;
                    CaseItem.City = command.Student.City;
                    CaseItem.MobileNumber = command.Student.MobileNumber;
                    CaseItem.Email = command.Student.Email;

                    _unitOfWork.StudentRepository.Update(_context, CaseItem);
            }
            return await _unitOfWork.CommitAsync();
        }
    }
}