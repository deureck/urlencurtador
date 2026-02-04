using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace urlencutador.Tests;

public class ControllerUrlTests : IDisposable
{
    private readonly DBurl _context;
    private readonly ServicesUrl _service;
    private readonly ControllerUrl _controller;

    public ControllerUrlTests()
    {
        // Create an in-memory database for testing
        var options = new DbContextOptionsBuilder<DBurl>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DBurl(options);
        _service = new ServicesUrl(_context, new Base62Converter());
        _controller = new ControllerUrl(_service);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task CreateUrl_ShouldReturnCreatedResult()
    {
        // Arrange
        var input = new CreateInput("https://www.example.com");

        // Act
        var result = await _controller.CreateUrl(input);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        
        // Verify the URL was actually added to the database
        var urls = await _context.Urls.ToListAsync();
        urls.Should().HaveCount(1);
        urls[0].Url.Should().Be("https://www.example.com");
    }

    [Fact]
    public async Task GetUrlById_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");
        _context.Urls.Add(model);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetUrlById(model.Id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(model);
    }

    [Fact]
    public async Task GetUrlById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.GetUrlById(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateHashById_ShouldReturnOkResultWithHash()
    {
        // Arrange
        long id = 1;

        // Act
        var result = await _controller.CreateHashById(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        
        // Verify the hash is not null or empty
        var hashObject = okResult!.Value;
        hashObject.Should().NotBeNull();
        
        // Use reflection to get the hash property
        var hashProperty = hashObject!.GetType().GetProperty("hash");
        var hashValue = hashProperty!.GetValue(hashObject) as string;
        hashValue.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RedirectToUrl_WithValidHash_ShouldReturnRedirectPermanent()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");
        _context.Urls.Add(model);
        await _context.SaveChangesAsync();

        var hash = _service.GetEncode62(model.Id);

        // Act
        var result = await _controller.RedirectToUrl(hash);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult!.Url.Should().Be("https://www.example.com");
        redirectResult.Permanent.Should().BeTrue();
    }

    [Fact]
    public async Task RedirectToUrl_WithInvalidHash_ShouldReturnNotFound()
    {
        // Arrange
        string invalidHash = Base62Converter.Encode(100); // Hash that results in invalid ID

        // Act
        var result = await _controller.RedirectToUrl(invalidHash);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAllUrls_ShouldReturnOkResultWithUrlList()
    {
        // Arrange
        _context.Urls.Add(new modelurl("https://www.example1.com"));
        _context.Urls.Add(new modelurl("https://www.example2.com"));
        _context.Urls.Add(new modelurl("https://www.example3.com"));
        await _context.SaveChangesAsync();

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
    public async Task UpdateUrl_ShouldReturnOkResult()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");
        _context.Urls.Add(model);
        await _context.SaveChangesAsync();

        var input = new CreateInput("https://www.updated.com");

        // Act
        var result = await _controller.UpdateUrl(model.Id, input);

        // Assert
        result.Should().BeOfType<OkResult>();
        
        // Verify the URL was actually updated
        var updatedModel = await _context.Urls.FindAsync(model.Id);
        updatedModel.Should().NotBeNull();
        updatedModel!.Url.Should().Be("https://www.updated.com");
    }

    [Fact]
    public async Task UpdateUrl_WithInvalidId_ShouldReturnOkButNotUpdate()
    {
        // Arrange
        var input = new CreateInput("https://www.updated.com");

        // Act
        var result = await _controller.UpdateUrl(999, input);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task DeleteUrlById_ShouldReturnOkResult()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");
        _context.Urls.Add(model);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DeleteUrlById(model.Id);

        // Assert
        result.Should().BeOfType<OkResult>();
        
        // Verify the URL was actually deleted
        var deletedModel = await _context.Urls.FindAsync(model.Id);
        deletedModel.Should().BeNull();
    }

    [Fact]
    public async Task DeleteUrlById_WithInvalidId_ShouldReturnOkWithoutError()
    {
        // Act
        var result = await _controller.DeleteUrlById(999);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}
