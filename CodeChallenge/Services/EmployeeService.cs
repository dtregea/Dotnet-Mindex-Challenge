using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
        
        public List<Employee> GetAll()
        {
            return _employeeRepository.GetAll();
        }
        
        /// <summary>
        /// Get an employee and a count of all subordinates
        /// </summary>
        /// <param name="id">ID of employee</param>
        /// <returns>Reporting structure of Employee</returns>
        public ReportingStructure GetReportingStructure(string id)
        {
            var topEmployee = _employeeRepository.GetById(id);
            // Map all employee ID's to a list of subordinate ID's
            var hierarchy = GetHierarchy();
            var totalEmployees = 0;

            var queue = new Queue<string>();
            var seen = new HashSet<string>();
            queue.Enqueue(id);
            seen.Add(id);

            // Traverse down the hierarchy, counting each employee
            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                if (hierarchy.ContainsKey(currentId))
                {
                    foreach (var subordinateId in hierarchy[currentId])
                    {
                        if (!seen.Contains(subordinateId))
                        {
                            queue.Enqueue(subordinateId);
                            seen.Add(subordinateId);
                            totalEmployees++;
                        }
                    }
                }
            }

            return new ReportingStructure
            {
                Employee = topEmployee,
                NumberOfReports = totalEmployees
            };
        }
        
        /// <summary>
        /// Map each employeeID to a list of their subordinates ID's
        /// </summary>
        /// <returns>Mapping of ID to list of subordinate ID's</returns>
        private Dictionary<String, List<String>> GetHierarchy()
        {
            // New "GetAll" method used to build entire employee hierarchy in advance to avoid
            // excessive database calls during BFS search, if in case the database is not in memory but over network
            var employees = GetAll();
            var idToSubordinates = new Dictionary<String, List<String>>();
            foreach (var employee in employees)
            {
                if (!idToSubordinates.ContainsKey(employee.EmployeeId))
                {
                    idToSubordinates.Add(employee.EmployeeId, new List<String>());
                }

                if (employee.DirectReports != null)
                {
                    idToSubordinates[employee.EmployeeId].AddRange(employee.DirectReports.Select(e => e.EmployeeId));
                }
            }

            return idToSubordinates;
        }
    }
}
