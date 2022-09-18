using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Models.Company
{
    public class GetCompanyModel: ViewModelBase
    {
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }
}