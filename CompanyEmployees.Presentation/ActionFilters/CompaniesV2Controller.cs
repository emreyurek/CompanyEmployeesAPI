using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared;

namespace CompanyEmployees.Presentation.ActionFilters
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IServiceManager _service;

        public CompaniesV2Controller(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companiesParamaters)
        {
            var companies = await _service.CompanyService.GetAllCompaniesAsync
            (companiesParamaters, trackChanges: false);

            return Ok(companies.Count());
        }
    }
}
