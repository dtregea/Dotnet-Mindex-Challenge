using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        /// <summary>
        /// Insert a Compensation record
        /// </summary>
        /// <param name="compensation">Compensation record to insert</param>
        /// <returns>Newly inserted Compensation record</returns>
        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for '{compensation.CompensationId}'");
            try
            {
                compensation = _compensationService.Create(compensation);
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }

            return CreatedAtRoute("getCompensationById", new { id = compensation.CompensationId }, compensation);
        }

        /// <summary>
        /// Get a Compensation by ID
        /// </summary>
        /// <param name="id">ID of compensation to get</param>
        /// <returns>Compensation of ID provided</returns>
        [HttpGet("{id}", Name = "getCompensationById")]
        public IActionResult GetCompensationById(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var compensation = _compensationService.GetById(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }
        
    }
}
