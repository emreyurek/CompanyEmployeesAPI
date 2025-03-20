using AutoMapper;
using Contracts;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService;

        public ServiceManager(IRepositoryManager repository, IMapper mapper, IDataShaper<EmployeeDto> dataShaperEmployee, IDataShaper<CompanyDto> dataShaperCompany)
        {
            _companyService = new Lazy<ICompanyService>(() => new CompanyService(repository, mapper, dataShaperCompany));
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repository, mapper, dataShaperEmployee));
        }

        public ICompanyService CompanyService => _companyService.Value;

        public IEmployeeService EmployeeService => _employeeService.Value;
    }
}
