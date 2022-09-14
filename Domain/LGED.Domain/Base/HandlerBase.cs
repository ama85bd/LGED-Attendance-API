using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LGED.Core.Interfaces;
using LGED.Data.Base;
using MediatR;

namespace LGED.Domain.Base
{
    public abstract class HandlerBase<TCommand, TModel> : ServiceBase, IRequestHandler<TCommand, TModel> where TCommand : CommandBase<TModel>
    {
        public HandlerBase(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {
        }

        public HandlerBase(IUnitOfWork unitOfWork, IUserContext context, IMapper mapper) : base(unitOfWork, context, mapper)
        {
        }

        public abstract Task<TModel> Handle(TCommand command, CancellationToken cancellationToken);
    }
}