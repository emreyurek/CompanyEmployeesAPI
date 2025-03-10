using AutoMapper;
using Contracts;
using Service.Contracts;

namespace Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mappger;

        public EmployeeService(IRepositoryManager repository, IMapper mappger)
        {
            _repository = repository;
            _mappger = mappger;
        }
    }
}
