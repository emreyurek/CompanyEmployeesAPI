using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mappger;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mappger)
        {
            _repository = repository;
            _logger = logger;
            _mappger = mappger;
        }
    }
}
