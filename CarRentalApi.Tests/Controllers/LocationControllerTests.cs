using CarRentalApi.Services.Interfaces;
using NSubstitute;
using Xunit;
using CarRentalApi.Controllers;
using CarRentalApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarRentalApi.Controllers;

namespace CarRentalApi.Tests.Controllers;

public class LocationControllerTests
{
    private ILocationService _serviceMock;
    private LocationController _controller;

    public LocationControllerTests()
    {
        _serviceMock = Substitute.For<ILocationService>();
        _controller = new LocationController(_serviceMock);
    }

    private Location GetLocation(long id)
    {
        return new Location
        {
            Id = id,
            Name = "Test Location",
            PickUpAvailable = true,
            DropOffAvailable = true
        };
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public async Task GetLocations_ReturnsLocations(int locationCount)
    {
        // Arrange
        for (int i = 1; i <= locationCount; i++)
        {
            _serviceMock.GetAll().Returns(new List<Location> { GetLocation(i) });
        }

        // Act
        var result = await _controller.GetLocations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLocations = Assert.IsAssignableFrom<IEnumerable<Location>>(okResult.Value);
        Assert.Equal(locationCount, returnedLocations.Count());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(404)]
    public async Task GetLocation_ReturnsLocationOrNotFound(int locationId)
    {
        // Arrange
        var location = GetLocation(1);
        _serviceMock.GetById(1).Returns(location);
        _serviceMock.GetById(404).Returns((Location?)null);

        // Act
        var result = await _controller.GetLocation(locationId);

        // Assert
        if (locationId == 1)
        {
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedLocation = Assert.IsAssignableFrom<Location>(okResult.Value);
            Assert.Equal(location.Id, returnedLocation.Id);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public async Task GetDropOffLocations_ReturnsDropOffLocations(int locationCount)
    {
        // Arrange
        for (int i = 1; i <= locationCount; i++)
        {
            _serviceMock.GetAllDropOff().Returns(new List<Location> { GetLocation(i) });
        }

        // Act
        var result = await _controller.GetDropOffLocations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLocations = Assert.IsAssignableFrom<IEnumerable<Location>>(okResult.Value);
        Assert.Equal(locationCount, returnedLocations.Count());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public async Task GetPickUpLocations_ReturnsPickUpLocations(int locationCount)
    {
        // Arrange
        for (int i = 1; i <= locationCount; i++)
        {
            _serviceMock.GetAllPickUp().Returns(new List<Location> { GetLocation(i) });
        }

        // Act
        var result = await _controller.GetPickUpLocations();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedLocations = Assert.IsAssignableFrom<IEnumerable<Location>>(okResult.Value);
        Assert.Equal(locationCount, returnedLocations.Count());
    }

    [Theory]
    [InlineData("success")]
    [InlineData("not-found")]
    [InlineData("bad-request")]
    public async Task PutLocation_ReturnsBadRequestOrNoContent(string scenario)
    {
        // Arrange
        var location = GetLocation(1);
        var idToUpdate = scenario == "bad-request" ? 2 : 1;

        _serviceMock.Update(idToUpdate, location).Returns(Task.FromResult(scenario == "success"));

        // Act
        var result = await _controller.PutLocation(idToUpdate, location);

        // Assert
        switch (scenario)
        {
            case "success":
                Assert.IsType<NoContentResult>(result);
                break;
            case "not-found":
                Assert.IsType<NotFoundResult>(result);
                break;
            case "bad-request":
                Assert.IsType<BadRequestResult>(result);
                break;
        }
    }

    [Fact]
    public async Task PostLocation_ReturnsCreatedAtAction()
    {
        // Arrange
        var location = GetLocation(1);
        _serviceMock.Create(location).Returns(Task.FromResult(location));

        // Act
        var result = await _controller.PostLocation(location);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedLocation = Assert.IsAssignableFrom<Location>(createdAtActionResult.Value);
        Assert.Equal(location.Id, returnedLocation.Id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(404)]
    public async Task DeleteLocation_ReturnsNoContentOrNotFound(int locationId)
    {
        // Arrange
        _serviceMock.GetById(1).Returns(GetLocation(1));
        _serviceMock.GetById(404).Returns((Location?)null);
        _serviceMock.Delete(GetLocation(locationId)).Returns(Task.FromResult(locationId == 1));

        // Act
        var result = await _controller.DeleteLocation(locationId);

        // Assert
        if (locationId == 1)
        {
            Assert.IsType<NoContentResult>(result);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result);
        }
    }
}