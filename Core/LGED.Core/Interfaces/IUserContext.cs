using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LGED.Core.Interfaces
{
    public interface IUserContext
    {
        public abstract Guid UserId { get; }
        public abstract string UserName { get; }
        public abstract Guid CurrentCompanyId { get; }
        public abstract string UserType { get; }
    }
}