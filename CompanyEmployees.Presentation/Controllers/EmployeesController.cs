using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController: ControllerBase
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
        [HttpGet("{id:guid}")]
        public IActionResult GetEmployeeForCompany(Guid companyid, Guid id)
        {
            var employee = serviceManager.EmployeeService.GetEmployee(companyid, id, false);
            return Ok(employee);
        }
    }
}
