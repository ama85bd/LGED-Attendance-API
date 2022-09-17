using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LGED.Core.Helper
{
    public static class AppConfigs
    {
        public static class InternalIdServer
        {
            public const string ValidAudience = "InternalIdServer:ValidAudience";
            public const string ValidIssuer = "InternalIdServer:ValidIssuer";
            public const string Secret = "InternalIdServer:Secret";
        }
    }
}