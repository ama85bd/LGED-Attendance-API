using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Commands.Admin.LocationChanges
{
    public class AssignCustomLocationToUserCommand : CommandBase<bool>
    {
        public Guid UserId { get; set; }
        public double Latitude {get; set;}
        public double Longitude {get; set;}
        
    }
}