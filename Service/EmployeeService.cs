using System.Dynamic;
using AutoMapper;
using Contracts;
using Entitites.Exceptions;
using Entitites.LinkModels;
using Entitites.Models;
using Service.Contracts;
using Shared;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;


namespace Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IDataShaper<EmployeeDto> _shapper;
        public EmployeeService(IRepositoryManager repository, IMapper mapper, IDataShaper<EmployeeDto> shapper)
        {
            _repository = repository;
            _mapper = mapper;
            _shapper = shapper;
        }
        private List<Link> CreateLinksForEmployee(Guid companyId, Guid id)
        {
            var links = new List<Link>
             {
                new Link(
                    href: $"http://localhost:5297/api/companies/{companyId}/employees/{id}",
                    rel: "self",
                    method: "GET"
                ),
                new Link(
                    href: $"http://localhost:5297/api/companies/{companyId}/employees/{id}",
                    rel: "update-employee",
                    method: "PUT"
                ),
                new Link(
                    href: $"http://localhost:52971/api/companies/{companyId}/employees/{id}",
                    rel: "delete-employee",
                    method: "DELETE"
                )
            };

            return links;
        }
        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }
        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeDb is null)
                throw new EmployeeNotFoundException(id);
            return employeeDb;
        }
        public async Task<(IEnumerable<ExpandoObject> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            if (!employeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();

            await CheckIfCompanyExists(companyId, trackChanges);

            var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);
            var shapedData = _shapper.ShapeData(employeesDto, employeeParameters.Fields);

            // For Hateos
            var employeesWithLinks = shapedData.Select(e =>
            {
                var employeeDict = (IDictionary<string, object>)e;
                var employeeId = (Guid)employeeDict["Id"];
                employeeDict.Add("Links", CreateLinksForEmployee(companyId, employeeId));
                return e;
            });

            return (employees: employeesWithLinks, metaData: employeesWithMetaData.MetaData);
        }
        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

            var employeeDto = _mapper.Map<EmployeeDto>(employeeDb);
            return employeeDto;
        }
        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeEntity = _mapper.Map<Employee>(employeeForCreationDto);

            _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repository.SaveAsync();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

            return employeeToReturn;
        }
        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);

            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);

            _repository.Employee.DeleteEmployee(employeeDb);
            await _repository.SaveAsync();
        }
        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdateDto, bool compTrackChanges, bool empTrackChanges)
        {
            await CheckIfCompanyExists(companyId, compTrackChanges);

            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

            _mapper.Map(employeeForUpdateDto, employeeDb);
            await _repository.SaveAsync();
        }
    }
}
