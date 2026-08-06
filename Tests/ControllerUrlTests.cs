using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urlencurtador.services;
using Xunit;

namespace urlencutador.Tests;

public class ControllerUrlTests : IDisposable
{
    private readonly DBurl _context;
    private readonly ServicesUrl _service;
    private readonly ControllerUrl _controller;

    public ControllerUrlTests()
    {
        var options = new DbContextOptionsBuilder<DBurl>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DBurl(options);
        _service = new ServicesUrl(_context);
        _controller = new ControllerUrl(_service);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateUrl_ShouldReturnCreatedResultWithModelUrl()
    {
        // Arrange
        var input = new CreateInput("https://www.example.com");

        // Act
        var result = await _controller.CreateUrl(input);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        var createdResult = result as CreatedResult;
        createdResult!.Value.Should().BeOfType<modelurl>();
        var model = createdResult.Value as modelurl;
        model!.Url.Should().Be("https://www.example.com");
        model.Code.Should().NotBeNullOrEmpty();

        var urls = await _context.Urls.ToListAsync();
        urls.Should().HaveCount(1);
        urls[0].Url.Should().Be("https://www.example.com");
    }

    [Fact]
    public async Task GetUrlById_WithValidCode_ShouldReturnOkResultWithUrlString()
    {
        // Arrange
        var model = await _service.Create("https://www.example.com");

        // Act
        var result = await _controller.GetUrlById(model.Code);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be("https://www.example.com");
    }

    [Fact]
    public async Task GetUrlById_WithInvalidCode_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.GetUrlById("non_existent_code");

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task RedirectToUrl_WithValidCode_ShouldReturnRedirectPermanent()
    {
        // Arrange
        var model = await _service.Create("https://www.example.com");

        // Act
        var result = await _controller.RedirectToUrl(model.Code);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult!.Url.Should().Be("https://www.example.com");
        redirectResult.Permanent.Should().BeTrue();
    }

    [Fact]
    public async Task RedirectToUrl_WithInvalidCode_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.RedirectToUrl("non_existent_code");

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAllUrls_ShouldReturnOkResultWithUrlList()
    {
        // Arrange
        await _service.Create("https://www.example1.com");
        await _service.Create("https://www.example2.com");
        await _service.Create("https://www.example3.com");

        // Act
        var result = await _controller.GetAllUrls();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var urls = okResult!.Value as List<modelurl>;
        urls.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllUrls_WithEmptyDatabase_ShouldReturnEmptyList()
    {
        // Act
        var result = await _controller.GetAllUrls();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var urls = okResult!.Value as List<modelurl>;
        urls.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateUrl_WithValidId_ShouldReturnOkResultAndEditUrl()
    {
        // Arrange
        var model = await _service.Create("https://www.example.com");
        var input = new CreateInput("https://www.updated.com");

        // Act
        var result = await _controller.UpdateUrl(model.Id, input);

        // Assert
        result.Should().BeOfType<OkResult>();
        var updatedModel = await _context.Urls.FindAsync(model.Id);
        updatedModel.Should().NotBeNull();
        updatedModel!.Url.Should().Be("https://www.updated.com");
    }

    [Fact]
    public async Task UpdateUrl_WithInvalidId_ShouldReturnOkResult()
    {
        // Arrange
        var input = new CreateInput("https://www.updated.com");

        // Act
        var result = await _controller.UpdateUrl(999999, input);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task DeleteUrlById_WithValidId_ShouldReturnOkResultAndRemoveUrl()
    {
        // Arrange
        var model = await _service.Create("https://www.example.com");

        // Act
        var result = await _controller.DeleteUrlById(model.Id);

        // Assert
        result.Should().BeOfType<OkResult>();
        var deletedModel = await _context.Urls.FindAsync(model.Id);
        deletedModel.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUrlById_WithInvalidId_ShouldReturnOkResult()
    {
        // Act
        var result = await _controller.DeleteUrlById(999999);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}
