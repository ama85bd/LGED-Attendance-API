using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LGED.Core.Security
{
    public static class AppClaimTypes
    {
        //role claim type
        public const string Permission = "dhtt.permission";

        //user claim type
        public const string UserType = "dhtt.usertype";
        public const string StaffId = "dhtt.staffid";
        public const string IsMasterAdmin = "dhtt.master";
    }
}