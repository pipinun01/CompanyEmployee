﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
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
        public IActionResult GetEmployeesForCompany(Guid companyid)
        {
            var employees = serviceManager.EmployeeService.GetEmployees(companyid, false);
            return Ok(employees);
        }
        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployeeForCompany(Guid companyid, Guid id)
        {
            var employee = serviceManager.EmployeeService.GetEmployee(companyid, id, false);
            return Ok(employee);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreation)
        {
            if (employeeForCreation is null)
            {
                return BadRequest("EmployeeForCreationDto object is null");
            }
            if(!ModelState.IsValid) 
                return UnprocessableEntity(ModelState);

            var employeeToReturn = serviceManager.EmployeeService.CreateEmployeeForCompany(companyId, employeeForCreation, false);
            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyid, Guid id)
        {
            serviceManager.EmployeeService.DeleteEmployeeForCompany(companyid, id, false);
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employeeUpdate)
        {
            if (employeeUpdate is null)
                return BadRequest("EmployeeForUpdateDto object is null");
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            serviceManager.EmployeeService.UpdateEmployeeForCompany(companyId,id,employeeUpdate, false, true);
            return NoContent();
        }
        [HttpPatch("{id:guid}")]
        public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if(patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");
            var result = serviceManager.EmployeeService.GetEmployeeForPatch(companyId, id, false, true);
            patchDoc.ApplyTo(result.employeeToPatch, ModelState);

            TryValidateModel(result.employeeToPatch);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            serviceManager.EmployeeService.SaveChangesForPatch(result.employeeToPatch, result.employeeEntity);
            return NoContent();
        }
    }
}