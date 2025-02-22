using CoriCore.Models; // Importing the Employee model
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoriCore.Controllers
{
    [Route("api/[controller]")] // Setting the route
    [ApiController] // Should behave as an ApiController

    // Inheriting from the ControllerBase class
    public class EmployeeController : ControllerBase
    {
        // Temporary list of employees
        private static List<Employee> employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "John Doe", Email = "john.doe@example.com" },
            new Employee { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com" },
            new Employee { Id = 3, Name = "Jim Beam", Email = "jim.beam@example.com" }
        };

        // GET: api/Employee
        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetEmployees()
        {
            return Ok(employees);
        }

        // GET: api/Employee/5
        /// <summary>
        /// Retrieves an employee by their ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Employee> Get(int id)
        {
            var employee = employees.Find(e => e.Id == id);
            return Ok(employee);
        }

        // POST: api/Employee
        /// <summary>
        /// Creates a new employee.
        /// </summary>
        [HttpPost]
        public ActionResult<Employee> CreateEmployee(Employee employee)
        {
            employee.Id = employees.Count + 1; // Set ID of new employee
            employees.Add(employee); // Adding new employee to list
            return CreatedAtAction(nameof(GetEmployees), new { id = employee.Id }, employee);
        }
    }
}
