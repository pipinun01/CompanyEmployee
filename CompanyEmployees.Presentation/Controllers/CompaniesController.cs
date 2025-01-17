    using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;


namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController: ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CompaniesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _serviceManager.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }
        [HttpGet("{companyGuid:guid}", Name ="CompanyById")]
        public async Task<IActionResult> GetCompany(Guid companyGuid)
        {
            var company = await _serviceManager.CompanyService.GetCompanyAsync(companyGuid, false);
            return Ok(company);
        }
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType =typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var companies = await _serviceManager.CompanyService.GetByIdsAsync(ids, false);
            return Ok(companies);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            //if(company == null)
            //{
            //    return BadRequest("CompanyForCreationDto object is null");
            //}
            var createdCompany = await _serviceManager.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new { companyGuid = createdCompany.id }, createdCompany);
        }
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody]IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await _serviceManager.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        }
        [HttpDelete("{comapnyId:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid comapnyId)
        {
            await _serviceManager.CompanyService.DeleteCompanyAsync(comapnyId, false);
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            //if(company is null)
            //    return BadRequest("CompanyForUpdateDto object is null");
            await _serviceManager.CompanyService.UpdateCompanyAsync(id, company, true);
            return NoContent();
        }
    }
}
