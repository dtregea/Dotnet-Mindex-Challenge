﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Insert an Employee record
        /// </summary>
        /// <param name="employee">Employee record to insert</param>
        /// <returns>Inserted Employee record</returns>
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        /// <summary>
        /// Get an Employee by id
        /// </summary>
        /// <param name="id">ID of Employee to get</param>
        /// <returns>Employee of ID provided</returns>
        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        /// <summary>
        /// Replace an Employee record
        /// </summary>
        /// <param name="id">Id of employee to replace</param>
        /// <param name="newEmployee">New employee record</param>
        /// <returns>Newly updated Employee</returns>
        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }
        
        /// <summary>
        /// Get the number of subordinates of an Employee
        /// </summary>
        /// <param name="id">Employee to count subordinates of</param>
        /// <returns>Reporting Structure of Employee</returns>
        [HttpGet("{id}/reportingStructure")]
        public IActionResult GetReportingStructure(String id)
        {
            _logger.LogDebug($"Received employee reporting structure get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            var reportingStructure = _employeeService.GetReportingStructure(id);

            return Ok(reportingStructure);
        }
    }
}
