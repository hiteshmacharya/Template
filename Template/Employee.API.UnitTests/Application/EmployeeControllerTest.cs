using EventBus.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Upscript.Services.Employee.API.Controllers;
using Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceInterface;
using Upscript.Services.Employee.API.Model;
using Xunit;

namespace UnitTest.Employee.Application
{
    public class EmployeeControllerTest
    {
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly Mock<ILogger<EmployeeController>> _loggerMock;
        private readonly Mock<IEventBus> _eventBusMock;

        public EmployeeControllerTest()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _loggerMock = new Mock<ILogger<EmployeeController>>();
            _eventBusMock = new Mock<IEventBus>();
        }
        [Fact]
        public async Task Get_Employee_success()
        {
            //Arrange
            int fakeEmployeeId = 1;
            var fakeEmployee = GetEmployeeFake(fakeEmployeeId);

            _employeeServiceMock.Setup(x => x.EmployeeByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fakeEmployee));


            //Act
            var employeeController = new EmployeeController(
                _employeeServiceMock.Object,
                _loggerMock.Object);

            var actionResult = await employeeController.EmployeeByIdAsync(fakeEmployeeId);

            //Assert
            Assert.Equal((actionResult.Result as OkObjectResult).StatusCode, (int)System.Net.HttpStatusCode.OK);
            Assert.Equal((((ObjectResult)actionResult.Result).Value as Employees).Id, fakeEmployeeId);
        }

        private Employees GetEmployeeFake(int fakeEmployeeId)
        {
            return new Employees()
            {
                Id = fakeEmployeeId
            };
        }
    }
}
