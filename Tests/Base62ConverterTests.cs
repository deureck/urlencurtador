using FluentAssertions;
using Xunit;

namespace urlencutador.Tests;

public class Base62ConverterTests
{
    [Theory]
    [InlineData(0, "0")]
    [InlineData(1, "1")]
    [InlineData(10, "A")]
    [InlineData(61, "z")]
    [InlineData(62, "10")]
    [InlineData(123456789, "8M0kX")]
    [InlineData(1000000, "4C92")]
    public void Encode_ShouldConvertNumberToBase62String(long number, string expected)
    {
        // Act
        var result = Base62Converter.Encode(number);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("1", 1)]
    [InlineData("A", 10)]
    [InlineData("z", 61)]
    [InlineData("10", 62)]
    [InlineData("8M0kX", 123456789)]
    [InlineData("4C92", 1000000)]
    public void Decode_ShouldConvertBase62StringToNumber(string base62, long expected)
    {
        // Act
        var result = Base62Converter.Decode(base62);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    [InlineData(10000000)]
    [InlineData(long.MaxValue)]
    public void EncodeAndDecode_ShouldBeReversible(long originalNumber)
    {
        // Act
        var encoded = Base62Converter.Encode(originalNumber);
        var decoded = Base62Converter.Decode(encoded);

        // Assert
        decoded.Should().Be(originalNumber);
    }

    [Fact]
    public void Encode_WithLargeNumber_ShouldReturnValidBase62String()
    {
        // Arrange
        long largeNumber = 9999999999;

        // Act
        var result = Base62Converter.Encode(largeNumber);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex("^[0-9A-Za-z]+$");
    }

    [Fact]
    public void Decode_WithValidBase62String_ShouldReturnPositiveNumber()
    {
        // Arrange
        string validBase62 = "AbCdEf";

        // Act
        var result = Base62Converter.Decode(validBase62);

        // Assert
        result.Should().BeGreaterThan(0);
    }
}
