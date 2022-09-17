using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LGED.Core.Security
{
    public static class AppClaimTypes
    {
        //role claim type
        public const string Permission = "lged.permission";

        //user claim type
        public const string UserType = "lged.usertype";
        public const string StaffId = "lged.staffid";
        public const string IsMasterAdmin = "lged.master";
    }
}