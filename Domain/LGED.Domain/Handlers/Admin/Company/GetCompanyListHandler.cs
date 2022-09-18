using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Admin.Company;
using LGED.Domain.Models.Company;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.Company
{
    public class GetCompanyListHandler : HandlerBase<GetCompanyListCommand, List<GetCompanyModel>>
    {
        public GetCompanyListHandler(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {
        }
        public override async Task<List<GetCompanyModel>> Handle(GetCompanyListCommand command, CancellationToken cancellationToken)
        {
            var result = await (from comp in _unitOfWork.CompanyRepository.GetQueryNoCached()                        
                        where comp.IsDeleted == false && comp.IsActive == true
                          orderby comp.Name
                          select new GetCompanyModel
                          {
                              CompanyId = comp.Id,
                              CompanyName = comp.Name

                          }).Distinct().ToListAsync(cancellationToken);

                          return result;
        }
    }
}