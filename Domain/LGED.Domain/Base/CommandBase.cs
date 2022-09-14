using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace LGED.Domain.Base
{
    public class CommandBase<TModel> : IRequest<TModel>
    {
        
    }
}