using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Upscript.Services.Catalog.API.ViewModel;
using Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceInterface;
using Upscript.Services.Employee.API.Model;

namespace Upscript.Services.Employee.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor of Employee Controller.
        /// </summary>
        /// <param name="employeeService">Service Class object of Employee</param>
        /// <param name="logger">Logging Object</param>
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        }

        /// <summary>
        /// This API is used for getting the all employees record.
        /// </summary>
        /// <param name="pageSize">No. of records on the page </param>
        /// <param name="pageIndex">Current Index of Page</param>
        /// <param name="ids">Employee Id List which can be comma separated</param>
        /// <returns>List of Employees</returns>
        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("employees")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<Employees>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<Employees>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EmployeesAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string ids = null)
        {
            long totalItems = 0;
            IEnumerable<Employees> itemsOnPage = null;
            PaginatedItemsViewModel<Employees> paginatedEmployees = null;
            if (!string.IsNullOrEmpty(ids))
            {
                itemsOnPage = await GetEmployeeByIdsAsync(ids);

                if (!itemsOnPage.Any())
                {
                    return BadRequest("ids value invalid. Must be comma-separated list of numbers");
                }

                return Ok(itemsOnPage);
            }
            totalItems = await _employeeService.CountEmployeeByIdsAsync(ids);
            itemsOnPage = await _employeeService.GetEmployeesAsync(pageSize, pageIndex, ids);
            paginatedEmployees = new PaginatedItemsViewModel<Employees>(pageIndex, pageSize, totalItems, itemsOnPage);


            return Ok(paginatedEmployees);
        }

        /// <summary>
        /// This API is used to get the employee by Id of the employee.
        /// </summary>
        /// <param name="id">Id of the employee</param>
        /// <returns>Employee Details</returns>
        /// GET api/v1/[controller]/employees/{id}]
        [HttpGet]
        [Route("employees/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Employees), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Employees>> EmployeeByIdAsync(int id)
        {
            Employees employee = null;
            if (id <= 0)
            {
                return BadRequest();
            }

            employee = await _employeeService.EmployeeByIdAsync(id);
            if (employee != null)
            {
                return Ok(employee);
            }
            else
            {
                return NotFound(new { Message = $"Employee with id {id} not found." });
            }

            return Ok(employee);
        }


        /// <summary>
        /// This API is used to get the Employees based on applied employee name filter.
        /// </summary>
        /// <param name="name">Name of the Employee</param>
        /// <param name="pageSize">No. of records on the page</param>
        /// <param name="pageIndex">Current Index of the Page</param>
        /// <returns>List of Employees</returns>
        /// GET api/v1/[controller]/employees/withname/{name}
        [HttpGet]
        [Route("employees/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<Employees>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<Employees>>> EmployeeWithNameAsync(string name, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            List<Employees> lstEmployees = null;
            PaginatedItemsViewModel<Employees> paginatedViewModel = null;
            long totalCount = 0;
            totalCount = await _employeeService.CountEmployeeWithNameAsync(name);
            lstEmployees = await _employeeService.EmployeeWithNameAsync(name, pageSize, pageIndex);
            paginatedViewModel = new PaginatedItemsViewModel<Employees>(pageIndex, pageSize, totalCount, lstEmployees);

            return Ok(paginatedViewModel);
        }

        /// <summary>
        /// This API is used to save new employee.
        /// </summary>
        /// <param name="employee">Employee Details</param>
        /// <returns>Saved Employee Detail</returns>
        //Post api/v1/[controller]/employees
        [Route("employees")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<Employees>> CreateEmployeeAsync([FromBody] Employees employee)
        {
            Employees savedEmployee = null;
            savedEmployee = await _employeeService.CreateEmployeeAsync(employee);


            return Ok(savedEmployee);
        }

        /// <summary>
        /// This API is used to update the existing employee.
        /// </summary>
        /// <param name="employeeToUpdate">Employee Details which need to be updated</param>
        /// <returns>Updated Employee Detail</returns>
        //PUT api/v1/[controller]/employees
        [Route("employees")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult<Employees>> UpdateEmployeeAsync([FromBody] Employees employeeToUpdate)
        {
            Employees employee = null;
            Employees updatedEmployee = null;

            employee = await _employeeService.EmployeeByIdAsync(employeeToUpdate.Id); ;

            if (employee == null)
            {
                return NotFound(new { Message = $"Employee with id {employeeToUpdate.Id} not found." });
            }
            updatedEmployee = await _employeeService.UpdateEmployeeAsync(employeeToUpdate);
            return Ok(updatedEmployee);
        }

        /// <summary>
        /// This API is used to delete employee.
        /// </summary>
        /// <param name="id">Id of the employee</param>
        /// <returns>Success/Failure Result</returns>
        //DELETE api/v1/[controller]/id
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteEmployeeAsync(int id)
        {
            var employee = _employeeService.EmployeeByIdAsync(id);
            if (employee == null)
                return NotFound(new { Message = $"Employee with id {id} not found." });

            await _employeeService.DeleteEmployeeAsync(id);
            return NoContent();
        }

        /// <summary>
        /// This method is used to get the employee by id of the employee.
        /// </summary>
        /// <param name="ids">Comma separated Employee Ids</param>
        /// <returns>List of Employees</returns>
        private async Task<IEnumerable<Employees>> GetEmployeeByIdsAsync(string ids)
        {
            var lstEmployee = await _employeeService.GetEmployeeByIdsAsync(ids);
            return lstEmployee;
        }

    }
}
