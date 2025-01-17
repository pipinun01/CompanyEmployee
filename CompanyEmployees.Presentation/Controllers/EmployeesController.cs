using CompanyEmployees.Presentation.ActionFilters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager serviceManager;
        public EmployeesController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyid, [FromQuery]EmployeeParameters employeeParameters)
        {
            var employees = await serviceManager.EmployeeService.GetEmployeesAsync(companyid, employeeParameters, false);
            return Ok(employees);
        }
        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyid, Guid id)
        {
            var employee = await serviceManager.EmployeeService.GetEmployeeAsync(companyid, id, false);
            return Ok(employee);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreation)
        {
            //if (employeeForCreation is null)
            //{
            //    return BadRequest("EmployeeForCreationDto object is null");
            //}
            //if(!ModelState.IsValid) 
            //    return UnprocessableEntity(ModelState);

            var employeeToReturn = await serviceManager.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employeeForCreation, false);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyid, Guid id)
        {
            await serviceManager.EmployeeService.DeleteEmployeeForCompanyAsync(companyid, id, false);
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employeeUpdate)
        {
            //if (employeeUpdate is null)
            //    return BadRequest("EmployeeForUpdateDto object is null");
            //if (!ModelState.IsValid)
            //    return UnprocessableEntity(ModelState);

            await serviceManager.EmployeeService.UpdateEmployeeForCompanyAsync(companyId,id,employeeUpdate, false, true);
            return NoContent();
        }
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if(patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");
            var result = await serviceManager.EmployeeService.GetEmployeeForPatchAsync(companyId, id, false, true);
            patchDoc.ApplyTo(result.employeeToPatch, ModelState);

            TryValidateModel(result.employeeToPatch);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            await serviceManager.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);
            return NoContent();
        }
    }
}