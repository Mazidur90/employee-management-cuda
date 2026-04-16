using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using System.Collections.Generic;

namespace EmployeeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EmployeeRecord>> GetEmployees()
        {
            return Ok(_employeeService.GetActiveEmployees());
        }

        [HttpGet("{id}")]
        public ActionResult<EmployeeRecord> GetEmployee(string id)
        {
            var employee = _employeeService.GetActiveEmployees().Find(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPost]
        public ActionResult<EmployeeRecord> PostEmployee(EmployeeRecord employee)
        {
            _employeeService.Save(employee, null);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
        }

        [HttpPut("{id}")]
        public IActionResult PutEmployee(string id, EmployeeRecord employee)
        {
            if (id != employee.EmployeeId)
            {
                return BadRequest();
            }
            _employeeService.Save(employee, null);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(string id)
        {
            _employeeService.SoftDelete(id);
            return NoContent();
        }
    }
}