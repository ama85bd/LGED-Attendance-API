using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LGED.Core.Interfaces;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.StudentCommand;
using LGED.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.StudentHandler
{
    public class GetStudentListHandler : HandlerBase<GetStudentListCommand, List<GetStudentModel>>
    {
        public GetStudentListHandler(IUnitOfWork unitOfWork, IUserContext context, IMapper mapper) : base(unitOfWork, context, mapper)
        {
        }

        public override async Task<List<GetStudentModel>> Handle(GetStudentListCommand command, CancellationToken cancellationToken)
        {
            var result = (from gi in _unitOfWork.StudentRepository.GetQuery()                        
                          orderby gi.FirstName
                          select new GetStudentModel
                          {
                              StudentId = gi.StudentId,
                              FirstName = gi.FirstName,
                              LastName = gi.LastName,
                              Email = gi.Email,
                              City = gi.City,
                              MobileNumber = gi.MobileNumber

                          }).Distinct().ToListAsync(cancellationToken);

                          return await result;
        }
    }
}