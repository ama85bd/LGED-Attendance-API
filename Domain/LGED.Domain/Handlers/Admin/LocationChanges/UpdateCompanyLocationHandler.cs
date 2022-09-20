using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Core.Model;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Admin.LocationChanges;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace LGED.Domain.Handlers.Admin.LocationChanges
{
    public class UpdateCompanyLocationHandler : HandlerBase<UpdateCompanyLocationCommand, bool>
    {
        public UpdateCompanyLocationHandler(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {
            
        }
        public override async Task<bool> Handle(UpdateCompanyLocationCommand command, CancellationToken cancellationToken)
        {
            var companyId = _context.CurrentCompanyId;
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId);
            var companyOldLocation = company.Location;
            var users = _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.CompanyId == companyId);
            foreach(var item in users)
            {
            var user =  _unitOfWork.UserRepository.GetQueryNoCached().Where(r => r.Id == item.UserId);
            System.Console.WriteLine("item item item ==================================system   "+user);
            }
            
            if(company != null)
            {
                    company.Latitude = command.Latitude;
                    company.Longitude = command.Longitude;
                    company.SRID = 4326;
                    company.Location = new Point(command.Longitude, command.Latitude) { SRID = 4326 };
                    
                    _unitOfWork.CompanyRepository.Update(_context, company);
                    foreach(var item in users)
                    {
                    var user = await _unitOfWork.UserRepository.GetByIdAsync(item.UserId);
                        user.Location = user.Location == companyOldLocation ? new Point(command.Longitude, command.Latitude) { SRID = 4326 } : user.Location;
                        _unitOfWork.UserRepository.Update(_context, user);
                    }
                    return await _unitOfWork.CommitAsync();
             }else
             {
                throw new ApiException("The company not found", (int)HttpStatusCode.NotFound);
             }
                    
            
        
        }
    }
}