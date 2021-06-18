using Employee.API.IntegrationEvents;
using EventBus.Abstractions;
using IntegrationEventLogEF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Upscript.Services.Employee.API;
using Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceImplementation;
using Upscript.Services.Employee.API.Model;
using UpScript.Services.Employee.API.Infrastructure;
using Xunit;

namespace UnitTest.Employee.Application
{
    public class EmployeeServiceTest
    {

        private readonly DbContextOptions<EmployeeContext> _dbOptions;
        private readonly Mock<IEventBus> _eventBusMock;
        private readonly Mock<IEmployeeIntegrationEventService> _employeeIntegrationEventService;


        public EmployeeServiceTest()
        {
            _eventBusMock = new Mock<IEventBus>();
            _employeeIntegrationEventService = new Mock<IEmployeeIntegrationEventService>();
            _dbOptions = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(databaseName: "in-memory")
                .Options;

            using (var dbContext = new EmployeeContext(_dbOptions))
            {
                var employees = dbContext.Employees.ToListAsync();
                if (employees.Result.Count > 0)
                {
                    foreach (var employee in employees.Result)
                    {
                        dbContext.Remove(employee);
                        dbContext.SaveChanges();
                    }
                }
                dbContext.AddRange(GetFakeEmployee());
                dbContext.SaveChanges();
            }
        }

        [Fact]
        public async Task Get_employee_by_id_success()
        {
            //Arrange
            var employeeContext = new EmployeeContext(_dbOptions);
            var employeeSettings = new Mock<IOptionsSnapshot<EmployeeSettings>>();



            //Act
            var employeeService = new EmployeeService(employeeContext, employeeSettings.Object, _eventBusMock.Object, _employeeIntegrationEventService.Object);
            var employees = await employeeService.EmployeeByIdAsync(1);

            //Assert
            Assert.Equal(1, employees.Id);
        }

        [Fact]
        public async Task Get_employee_by_name_success()
        {
            //Arrange
            var employeeContext = new EmployeeContext(_dbOptions);
            var employeeSettings = new Mock<IOptionsSnapshot<EmployeeSettings>>();

            //Act
            var employeeService = new EmployeeService(employeeContext, employeeSettings.Object, _eventBusMock.Object, _employeeIntegrationEventService.Object);
            var employees = await employeeService.EmployeeWithNameAsync("fakeItemA");

            //Assert
            Assert.True(1 == employees.Count);
        }

        [Fact]
        public async Task Get_employee_totalCount()
        {
            //Arrange
            var employeeContext = new EmployeeContext(_dbOptions);
            var employeeSettings = new Mock<IOptionsSnapshot<EmployeeSettings>>();

            //Act
            var employeeService = new EmployeeService(employeeContext, employeeSettings.Object, _eventBusMock.Object, _employeeIntegrationEventService.Object);
            var count = await employeeService.CountEmployeeByIdsAsync();

            //Assert
           Assert.True(5 == count);
        }
        private List<Employees> GetFakeEmployee()
        {
            return new List<Employees>()
            {
                new Employees()
                {
                    Id = 1,
                    Name = "fakeItemA",
                    Description = "fakeItemA",
                    Email = "test@test.com"
                },
                new Employees()
                {
                    Id = 2,
                    Name = "fakeItemB",
                    Description = "fakeItemB",
                    Email = "test@test.com"
                },
                new Employees()
                {
                    Id = 3,
                    Name = "fakeItemC",
                    Description = "fakeItemC",
                    Email = "test@test.com"
                },
                new Employees()
                {
                    Id = 4,
                    Name = "fakeItemD",
                    Description = "fakeItemD",
                    Email = "test@test.com"
                },
                new Employees()
                {
                    Id = 5,
                    Name = "fakeItemE",
                    Description = "fakeItemE",
                    Email = "test@test.com"
                }
            };
        }
    }
}
