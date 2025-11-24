using FluentAssertions;
using SyncTrip.Core.Entities;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité Brand.
/// </summary>
public class BrandTests
{
    private const int ValidId = 1;
    private const string ValidName = "Renault";
    private const string ValidLogoUrl = "https://example.com/renault-logo.png";

    #region Create - Success Cases

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var brand = Brand.Create(ValidId, ValidName, ValidLogoUrl);

        // Assert
        brand.Should().NotBeNull();
        brand.Id.Should().Be(ValidId);
        brand.Name.Should().Be(ValidName);
        brand.LogoUrl.Should().Be(ValidLogoUrl);
        brand.Vehicles.Should().NotBeNull();
        brand.Vehicles.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithWhitespaceInName_ShouldTrimName()
    {
        // Arrange
        const string nameWithSpaces = "  Renault  ";

        // Act
        var brand = Brand.Create(ValidId, nameWithSpaces, ValidLogoUrl);

        // Assert
        brand.Name.Should().Be("Renault");
    }

    [Fact]
    public void Create_WithWhitespaceInLogoUrl_ShouldTrimLogoUrl()
    {
        // Arrange
        const string logoUrlWithSpaces = "  https://example.com/logo.png  ";

        // Act
        var brand = Brand.Create(ValidId, ValidName, logoUrlWithSpaces);

        // Assert
        brand.LogoUrl.Should().Be("https://example.com/logo.png");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(999)]
    public void Create_WithDifferentIds_ShouldSucceed(int id)
    {
        // Arrange & Act
        var brand = Brand.Create(id, ValidName, ValidLogoUrl);

        // Assert
        brand.Id.Should().Be(id);
    }

    [Theory]
    [InlineData("Yamaha")]
    [InlineData("Honda")]
    [InlineData("Toyota")]
    [InlineData("Kawasaki")]
    [InlineData("Peugeot")]
    public void Create_WithDifferentNames_ShouldSucceed(string name)
    {
        // Arrange & Act
        var brand = Brand.Create(ValidId, name, ValidLogoUrl);

        // Assert
        brand.Name.Should().Be(name);
    }

    [Theory]
    [InlineData("https://example.com/logo1.png")]
    [InlineData("https://cdn.example.com/logos/brand.svg")]
    [InlineData("https://storage.example.com/brands/logo.jpg")]
    public void Create_WithDifferentLogoUrls_ShouldSucceed(string logoUrl)
    {
        // Arrange & Act
        var brand = Brand.Create(ValidId, ValidName, logoUrl);

        // Assert
        brand.LogoUrl.Should().Be(logoUrl);
    }

    #endregion

    #region Create - Validation Failures

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange & Act
        var act = () => Brand.Create(ValidId, invalidName!, ValidLogoUrl);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*nom de la marque*")
            .WithParameterName("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidLogoUrl_ShouldThrowArgumentException(string? invalidLogoUrl)
    {
        // Arrange & Act
        var act = () => Brand.Create(ValidId, ValidName, invalidLogoUrl!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*URL du logo*")
            .WithParameterName("logoUrl");
    }

    [Fact]
    public void Create_WithEmptyNameAfterTrim_ShouldThrowArgumentException()
    {
        // Arrange
        const string nameWithOnlySpaces = "     ";

        // Act
        var act = () => Brand.Create(ValidId, nameWithOnlySpaces, ValidLogoUrl);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*nom de la marque*")
            .WithParameterName("name");
    }

    [Fact]
    public void Create_WithEmptyLogoUrlAfterTrim_ShouldThrowArgumentException()
    {
        // Arrange
        const string logoUrlWithOnlySpaces = "     ";

        // Act
        var act = () => Brand.Create(ValidId, ValidName, logoUrlWithOnlySpaces);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*URL du logo*")
            .WithParameterName("logoUrl");
    }

    #endregion

    #region Collections Initialization

    [Fact]
    public void Create_ShouldInitializeVehiclesCollectionAsEmpty()
    {
        // Arrange & Act
        var brand = Brand.Create(ValidId, ValidName, ValidLogoUrl);

        // Assert
        brand.Vehicles.Should().NotBeNull();
        brand.Vehicles.Should().BeOfType<List<Vehicle>>();
        brand.Vehicles.Should().HaveCount(0);
    }

    #endregion
}
