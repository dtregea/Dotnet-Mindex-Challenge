using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;
using ICompensationRepository_;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository, IEmployeeService employeeService)
        {
            _compensationRepository = compensationRepository;
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Create a compensation
        /// </summary>
        /// <param name="compensation">Compensation to Create</param>
        /// <returns></returns>
        /// <exception cref="Exception">Indicates an invalid Employee ID</exception>
        public Compensation Create(Compensation compensation)
        {
            if (compensation == null) return null;
            var employeeID = compensation.Employee != null ? compensation.Employee.EmployeeId : compensation.EmployeeId;
            var employee = _employeeService.GetById(employeeID);
            if (employee == null)
            {
                throw new Exception($"Employee {compensation.EmployeeId} Not Found");
            }
            _compensationRepository.Add(compensation);
            _compensationRepository.SaveAsync().Wait();

            return compensation;
        }

        /// <summary>
        /// Get a compensation by ID
        /// </summary>
        /// <param name="id">ID of compensation</param>
        /// <returns>Compensation of ID parameter</returns>
        public Compensation GetById(string id)
        {
            return !String.IsNullOrEmpty(id) ? _compensationRepository.GetById(id) : null;
        }
        
    }
}
