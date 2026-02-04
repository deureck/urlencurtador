using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
        _service = new ServicesUrl(_context, new Base62Converter());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void Create_Model_ShouldCreateModelUrlWithCorrectUrl()
    {
        // Arrange
        string testUrl = "https://www.example.com";

        // Act
        var result = _service.Create_Model(testUrl);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be(testUrl);
    }

    [Fact]
    public async Task Add_ShouldAddUrlToDatabase()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");

        // Act
        await _service.Add(model);

        // Assert
        var urls = await _context.Urls.ToListAsync();
        urls.Should().HaveCount(1);
        urls[0].Url.Should().Be("https://www.example.com");
    }

    [Fact]
    public async Task Get_WithValidId_ShouldReturnUrl()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");
        _context.Urls.Add(model);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.Get(model.Id);

        // Assert
        result.Should().NotBeNull();
        result.Url.Should().Be("https://www.example.com");
    }

    [Fact]
    public async Task Get_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.Get(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task List_All_ShouldReturnAllUrls()
    {
        // Arrange
        _context.Urls.Add(new modelurl("https://www.example1.com"));
        _context.Urls.Add(new modelurl("https://www.example2.com"));
        _context.Urls.Add(new modelurl("https://www.example3.com"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.List_All();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public void GetEncode62_ShouldEncodeIdWithOffset()
    {
        // Arrange
        long id = 1;
        long expectedEncodedId = 1000001; // id + IDOFFSET (1000000)

        // Act
        var result = _service.GetEncode62(id);

        // Assert
        var decoded = Base62Converter.Decode(result);
        decoded.Should().Be(expectedEncodedId);
    }

    [Fact]
    public async Task SetEncode62_WithValidHash_ShouldReturnUrl()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");
        _context.Urls.Add(model);
        await _context.SaveChangesAsync();

        var hash = _service.GetEncode62(model.Id);

        // Act
        var result = await _service.SetEncode62(hash);

        // Assert
        result.Should().Be("https://www.example.com");
    }

    [Fact]
    public async Task SetEncode62_WithInvalidHash_ShouldReturnNull()
    {
        // Arrange
        var invalidHash = Base62Converter.Encode(999999); // ID that doesn't exist

        // Act
        var result = await _service.SetEncode62(invalidHash);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetEncode62_WithHashResultingInNegativeId_ShouldReturnNull()
    {
        // Arrange
        var hashForSmallId = Base62Converter.Encode(100); // Less than IDOFFSET

        // Act
        var result = await _service.SetEncode62(hashForSmallId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_WithValidId_ShouldUpdateUrl()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");
        _context.Urls.Add(model);
        await _context.SaveChangesAsync();

        var updatedModel = new modelurl("https://www.updated.com");

        // Act
        await _service.Update(model.Id, updatedModel);

        // Assert
        var result = await _context.Urls.FindAsync(model.Id);
        result.Should().NotBeNull();
        result!.Url.Should().Be("https://www.updated.com");
    }

    [Fact]
    public async Task Update_WithInvalidId_ShouldNotThrowException()
    {
        // Arrange
        var updatedModel = new modelurl("https://www.updated.com");

        // Act
        var act = async () => await _service.Update(999, updatedModel);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldRemoveUrl()
    {
        // Arrange
        var model = new modelurl("https://www.example.com");
        _context.Urls.Add(model);
        await _context.SaveChangesAsync();

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
        var act = async () => await _service.Delete(999);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
