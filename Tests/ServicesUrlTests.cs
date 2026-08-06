using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using urlencurtador.services;
using Xunit;

namespace urlencutador.Tests;

public class ServicesUrlTests : IDisposable
{
    private readonly DBurl _context;
    private readonly ServicesUrl _service;

    public ServicesUrlTests()
    {
        var options = new DbContextOptionsBuilder<DBurl>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DBurl(options);
        _service = new ServicesUrl(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task Create_ShouldCreateModelUrlAndSaveToDatabase()
    {
        // Arrange
        string testUrl = "https://www.example.com";

        // Act
        var result = await _service.Create(testUrl);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(testUrl);
        result.Code.Should().NotBeNullOrEmpty();

        var inDb = await _context.Urls.FirstOrDefaultAsync(u => u.Code == result.Code);
        inDb.Should().NotBeNull();
        inDb!.Url.Should().Be(testUrl);
    }

    [Fact]
    public async Task Get_WithValidCode_ShouldReturnUrl()
    {
        // Arrange
        var model = await _service.Create("https://www.example.com");

        // Act
        var result = await _service.Get(model.Code);

        // Assert
        result.Should().NotBeNull();
        result!.Url.Should().Be("https://www.example.com");
    }

    [Fact]
    public async Task Get_WithInvalidCode_ShouldReturnNull()
    {
        // Act
        var result = await _service.Get("invalid_code_999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task List_All_ShouldReturnAllUrls()
    {
        // Arrange
        await _service.Create("https://www.example1.com");
        await _service.Create("https://www.example2.com");
        await _service.Create("https://www.example3.com");

        // Act
        var result = await _service.List_All();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Update_WithValidId_ShouldUpdateUrl()
    {
        // Arrange
        var model = await _service.Create("https://www.example.com");

        // Act
        await _service.Update(model.Id, "https://www.updated.com");

        // Assert
        var result = await _context.Urls.FindAsync(model.Id);
        result.Should().NotBeNull();
        result!.Url.Should().Be("https://www.updated.com");
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldNotThrowException()
    {
        // Act
        var act = async () => await _service.Update(999999, "https://www.updated.com");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldRemoveUrl()
    {
        // Arrange
        var model = await _service.Create("https://www.example.com");

        // Act
        await _service.Delete(model.Id);

        // Assert
        var result = await _context.Urls.FindAsync(model.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldNotThrowException()
    {
        // Act
        var act = async () => await _service.Delete(999999);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
