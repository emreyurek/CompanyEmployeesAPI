using Entitites.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var list = new List<Link>
            {
                new() {
                    Href = $"{baseUrl}/api",
                    Rel = "self",
                    Method = "GET"
                },
                new() {
                    Href = $"{baseUrl}/api/companies",
                    Rel = "companies",
                    Method = "GET"
                },
                new() {
                    Href = $"{baseUrl}/api/companies",
                    Rel = "create_company",
                    Method = "POST"
                }
            };
            return Ok(list);
        }
    }
}
