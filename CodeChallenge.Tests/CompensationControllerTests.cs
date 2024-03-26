using System;
using System.Net;
using System.Net.Http;
using System.Text;
using CodeChallenge.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Create and Insert Employee
            var employee = new Employee();
            var employeeContent = new JsonSerialization().ToJson(employee);
            var employeePost = _httpClient.PostAsync("api/employee",
                new StringContent(employeeContent, Encoding.UTF8, "application/json"));
            var employeeResponse = employeePost.Result.DeserializeContent<Employee>();
            Assert.IsNotNull(employeeResponse);
            
            // Create and Insert Compensation
            var compensation = new Compensation
            {
                EmployeeId = employeeResponse.EmployeeId,
                Salary = 9001,
                EffectiveDate = DateTime.Now
            };

            var compensationContent = new JsonSerialization().ToJson(compensation);
            var compensationPost = _httpClient.PostAsync("api/compensation",
                new StringContent(compensationContent, Encoding.UTF8, "application/json"));
            var response = compensationPost.Result;
            var newCompensation = response.DeserializeContent<Compensation>();

            // Assert Success
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(newCompensation.CompensationId);
            Assert.AreEqual(compensation.EmployeeId, newCompensation.EmployeeId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
        }
        
        [TestMethod]
        public void GetCompensationById_Returns_NotFound()
        {
            // Attempt to retrieve a compensation that doesn't exist
            var invalidCompensationId = "Invalid_Id";

            var getRequestTask = _httpClient.GetAsync($"api/compensation/{invalidCompensationId}");
            var response = getRequestTask.Result;

            // Assert Not Found
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [TestMethod]
        public void GetCompensationById_Returns_BadRequest()
        {
            // Create Compensation With Invalid Employee ID
            var compensation = new Compensation
            {
                EmployeeId = "not-a-real-employee",
                Salary = 9001,
                EffectiveDate = DateTime.Now
            };

            var compensationContent = new JsonSerialization().ToJson(compensation);
            var compensationPost = _httpClient.PostAsync("api/compensation",
                new StringContent(compensationContent, Encoding.UTF8, "application/json"));
            var response = compensationPost.Result;

            // Assert Bad Request
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
