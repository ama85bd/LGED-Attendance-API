using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LGED.Core.Interfaces;
using LGED.Data.Base;

namespace LGED.Domain.Base
{
    public class ServiceBase
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserContext _context;
        protected readonly IMapper _mapper;
        protected ServiceBase(IUnitOfWork unitOfWork, IUserContext context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        protected ServiceBase(IUnitOfWork unitOfWork, IUserContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _unitOfWork = unitOfWork;
        }
    }
}