using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController(IServiceManager service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetEmployeesForCompany(Guid companyId)
    {
        var employees = service.EmployeeService.GetEmployees(companyId, trackChanges: false);
        return Ok(employees);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = service.EmployeeService.GetEmployee(companyId, id, trackChanges: false);
        return Ok(employee);
    }

    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        if (employee is null)
            return BadRequest("EmployeeForCreationDto object is null");

        var employeeToReturn = service.EmployeeService.CreateEmployeeForCompany(companyId, employee, trackChanges: false);
        return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
    }
}

