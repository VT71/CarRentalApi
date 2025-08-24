using NSubstitute;
using CarRentalApi.Controllers;
using CarRentalApi.Models;
using Microsoft.AspNetCore.Mvc;
using CarRentalApi.Services.Interfaces;
using NSubstitute.ExceptionExtensions;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Tests.Controllers;

public class MakeControllerTests
{
    private IMakeService _serviceMock;
    private MakeController _controller;

    public MakeControllerTests()
    {
        _serviceMock = Substitute.For<IMakeService>();
        _controller = new MakeController(_serviceMock);
    }

    private Make GetMake(long id)
    {
        return new Make
        {
            Id = id,
            Name = "Test Make"
        };
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public async Task GetMakes_ReturnsMakes(int makeCount)
    {
        // Arrange
        for (int i = 1; i <= makeCount; i++)
        {
            _serviceMock.GetAll().Returns(new List<Make> { GetMake(i) });
        }

        // Act
        var result = await _controller.GetMakes();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedMakes = Assert.IsAssignableFrom<IEnumerable<Make>>(okResult.Value);
        Assert.Equal(makeCount, returnedMakes.Count());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(404)]
    public async Task GetMake_ReturnsMakeOrNotFound(long id)
    {
        // Arrange
        _serviceMock.GetById(id).Returns(id != 404 ? GetMake(id) : (Make?)null);

        // Act
        var result = await _controller.GetMake(id);

        // Assert
        if (id != 404)
        {
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedMake = Assert.IsType<Make>(okResult.Value);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task PostMake_CreatesMake()
    {
        // Arrange
        var makeToCreate = new Make { Name = "New Make" };
        var createdMake = new Make { Id = 1, Name = "New Make" };
        _serviceMock.Create(makeToCreate).Returns(createdMake);

        // Act
        var result = await _controller.PostMake(makeToCreate);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedMake = Assert.IsType<Make>(createdAtActionResult.Value);
        Assert.Equal(createdMake.Id, returnedMake.Id);
        Assert.Equal(createdMake.Name, returnedMake.Name);
    }

    [Theory]
    [InlineData("no-content")]
    [InlineData("bad-request")]
    [InlineData("not-found")]
    public async Task PutMake_UpdatesMake_ReturnsAppropriateResponse(string scenario)
    {
        // Arrange
        var make = GetMake(1);
        _serviceMock.Update(1, make).Returns(scenario == "no-content" ? true : false);
        if (scenario == "not-found")
        {
            _serviceMock.Update(1, make).Throws(new DbUpdateConcurrencyException());
            _serviceMock.GetById(1).Returns((Make?)null);
        }

        // Act
        var result = await _controller.PutMake(1, make);

        // Assert
        switch (scenario)
        {
            case "no-content":
                Assert.IsType<NoContentResult>(result);
                break;
            case "bad-request":
                Assert.IsType<BadRequestResult>(result);
                break;
            case "not-found":
                Assert.IsType<NotFoundResult>(result);
                break;
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(404)]
    public async Task DeleteMake_ReturnsNoContentOrNotFound(int makeId)
    {
        // Arrange
        _serviceMock.GetById(1).Returns(GetMake(1));
        _serviceMock.GetById(404).Returns((Make?)null);
        _serviceMock.Delete(GetMake(makeId).Id).Returns(Task.FromResult(makeId == 1));

        // Act
        var result = await _controller.DeleteMake(makeId);

        // Assert
        if (makeId == 1)
        {
            Assert.IsType<NoContentResult>(result);
        }
        else
        {
            Assert.IsType<NotFoundResult>(result);
        }
    }
}