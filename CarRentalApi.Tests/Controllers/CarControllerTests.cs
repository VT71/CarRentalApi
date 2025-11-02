using CarRentalApi.Services.Interfaces;
using NSubstitute;
using Xunit;
using CarRentalApi.Controllers;
using CarRentalApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApi.Tests.Controllers;

public class CarControllerTests
{
    private ICarService _serviceMock;
    // Add _contextMock if needed for integration tests

    public CarControllerTests()
    {
        _serviceMock = Substitute.For<ICarService>();
    }

    private Car GetCar(long id)
    {
        return new Car
        {
            Id = id,
            Model = "Test Car",
            MakeId = 1,
            Make = new Make { Id = 1, Name = "Test Make" },
            Description = "Test Description",
            PictureUrl = "http://example.com/car.jpg",
            TransmissionType = TransmissionType.Auto
        };
    }

    [Fact]
    public void Controller_RequiresAuthorizationAttribute()
    {
        // Arrange
        var controllerType = typeof(CarController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(authorizeAttribute);
    }

    [Fact]
    public async Task GetCars_WhenCarsExist_ReturnsCars()
    {
        // Arrange
        var cars = new List<Car>
        {
            GetCar(1),
            GetCar(2)
        };
        _serviceMock.GetAll().Returns(cars);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.GetCars();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCars = Assert.IsAssignableFrom<IEnumerable<Car>>(okResult.Value);
        Assert.Equal(2, returnedCars.Count());
    }

    [Fact]
    public async Task GetCars_WhenNoCarsExist_ReturnsEmptyList()
    {
        // Arrange
        var cars = new List<Car>();
        _serviceMock.GetAll().Returns(cars);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.GetCars();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedCars = Assert.IsAssignableFrom<IEnumerable<Car>>(okResult.Value);
        Assert.Empty(returnedCars);
    }

    [Fact]
    public async Task GetAvailableCars_WithValidParams_ReturnsAvailableCars()
    {
        // Arrange
        var availableCars = new List<Car>
        {
            GetCar(1),
            GetCar(2)
        };
        _serviceMock.GetAvailableCars(1, 2, "2023-10-01T10:00:00Z", "2023-10-02T10:00:00Z")
            .Returns(availableCars);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.GetAvailableCars(1, 2, "2023-10-01T10:00:00Z", "2023-10-02T10:00:00Z");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAvailableCars_WhenNoCarsAvailable_ReturnsEmptyList()
    {
        // Arrange
        var availableCars = new List<Car>();
        _serviceMock.GetAvailableCars(1, 2, "2023-10-01T10:00:00Z", "2023-10-02T10:00:00Z")
            .Returns(availableCars);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.GetAvailableCars(1, 2, "2023-10-01T10:00:00Z", "2023-10-02T10:00:00Z");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCar_WhenCarExists_ReturnsCar()
    {
        // Arrange
        var car = GetCar(1);
        _serviceMock.GetById(1).Returns(car);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.GetCar(1);

        // Assert
        Assert.IsType<ActionResult<Car>>(result);
        Assert.NotNull(result.Value);
        Assert.Equal(car.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetCar_WhenCarDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.GetById(1).Returns((Car?)null);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.GetCar(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Put_WhenCarIdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var car = GetCar(1);
        _serviceMock.GetById(1).Returns(car);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.Put(2, car);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Put_WhenMakeNotFound_ReturnsBadRequest()
    {
        // Arrange
        var car = GetCar(1);
        car.MakeId = 999; // Assuming this make does not exist
        _serviceMock.GetById(1).Returns(car);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.Put(1, car);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Put_WhenCarNotFound_ReturnsNotFound()
    {
        // Arrange
        var car = GetCar(1);
        _serviceMock.GetById(1).Returns((Car?)null);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.Put(1, car);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Put_WhenCarUpdated_ReturnsNoContent()
    {
        // Arrange
        var car = GetCar(1);
        _serviceMock.GetById(1).Returns(car);
        _serviceMock.Update(1, car).Returns(true);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.Put(1, car);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PostCar_WhenCarCreated_ReturnsCreatedAtAction()
    {
        // Arrange
        var car = GetCar(1);
        _serviceMock.Create(car).Returns(car);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.PostCar(car);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetCar", createdResult.ActionName);
        Assert.Equal(car.Id, createdResult.RouteValues["id"]);
        Assert.Equal(car, createdResult.Value);
    }

    [Fact]
    public async Task PostCar_WhenCarCreationFails_ReturnsBadRequest()
    {
        // Arrange
        var car = GetCar(1);
        _serviceMock.Create(car).Returns((Car?)null);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.PostCar(car);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task DeleteCar_WhenCarExists_ReturnsNoContent()
    {
        // Arrange
        var car = GetCar(1);
        _serviceMock.GetById(1).Returns(car);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.DeleteCar(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCar_WhenCarDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.GetById(1).Returns((Car?)null);

        // Act
        var controller = new CarController(null, _serviceMock);
        var result = await controller.DeleteCar(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}