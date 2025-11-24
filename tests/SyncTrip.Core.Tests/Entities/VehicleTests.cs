using FluentAssertions;
using SyncTrip.Core.Entities;
using SyncTrip.Core.Enums;
using Xunit;

namespace SyncTrip.Core.Tests.Entities;

/// <summary>
/// Tests unitaires pour l'entité Vehicle.
/// </summary>
public class VehicleTests
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private const int ValidBrandId = 1;
    private const string ValidModel = "Clio";
    private const VehicleType ValidType = VehicleType.Car;

    #region Create - Success Cases

    [Fact]
    public void Create_WithValidMinimalData_ShouldSucceed()
    {
        // Arrange & Act
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);

        // Assert
        vehicle.Should().NotBeNull();
        vehicle.Id.Should().NotBe(Guid.Empty);
        vehicle.UserId.Should().Be(_validUserId);
        vehicle.BrandId.Should().Be(ValidBrandId);
        vehicle.Model.Should().Be(ValidModel);
        vehicle.Type.Should().Be(ValidType);
        vehicle.Color.Should().BeNull();
        vehicle.Year.Should().BeNull();
        vehicle.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithAllFields_ShouldSucceed()
    {
        // Arrange
        const string color = "Rouge";
        const int year = 2020;

        // Act
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, color, year);

        // Assert
        vehicle.Should().NotBeNull();
        vehicle.Color.Should().Be(color);
        vehicle.Year.Should().Be(year);
    }

    [Fact]
    public void Create_WithWhitespaceInModel_ShouldTrimModel()
    {
        // Arrange
        const string modelWithSpaces = "  Clio  ";

        // Act
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, modelWithSpaces, ValidType);

        // Assert
        vehicle.Model.Should().Be("Clio");
    }

    [Fact]
    public void Create_WithWhitespaceInColor_ShouldTrimColor()
    {
        // Arrange
        const string colorWithSpaces = "  Rouge  ";

        // Act
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, colorWithSpaces);

        // Assert
        vehicle.Color.Should().Be("Rouge");
    }

    [Fact]
    public void Create_WithEmptyColor_ShouldSetColorToNull()
    {
        // Arrange & Act
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, "   ");

        // Assert
        vehicle.Color.Should().BeNull();
    }

    [Theory]
    [InlineData(VehicleType.Car)]
    [InlineData(VehicleType.Motorcycle)]
    [InlineData(VehicleType.Truck)]
    [InlineData(VehicleType.Van)]
    [InlineData(VehicleType.Motorhome)]
    public void Create_WithDifferentVehicleTypes_ShouldSucceed(VehicleType vehicleType)
    {
        // Arrange & Act
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, vehicleType);

        // Assert
        vehicle.Type.Should().Be(vehicleType);
    }

    [Theory]
    [InlineData(1900)]
    [InlineData(1950)]
    [InlineData(2000)]
    [InlineData(2024)]
    public void Create_WithValidYear_ShouldSucceed(int year)
    {
        // Arrange & Act
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, year: year);

        // Assert
        vehicle.Year.Should().Be(year);
    }

    [Fact]
    public void Create_WithNextYear_ShouldSucceed()
    {
        // Arrange
        var nextYear = DateTime.UtcNow.Year + 1;

        // Act
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, year: nextYear);

        // Assert
        vehicle.Year.Should().Be(nextYear);
    }

    #endregion

    #region Create - Validation Failures

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => Vehicle.Create(Guid.Empty, ValidBrandId, ValidModel, ValidType);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*identifiant utilisateur*")
            .WithParameterName("userId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithInvalidBrandId_ShouldThrowArgumentException(int invalidBrandId)
    {
        // Arrange & Act
        var act = () => Vehicle.Create(_validUserId, invalidBrandId, ValidModel, ValidType);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*identifiant de la marque*")
            .WithParameterName("brandId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidModel_ShouldThrowArgumentException(string? invalidModel)
    {
        // Arrange & Act
        var act = () => Vehicle.Create(_validUserId, ValidBrandId, invalidModel!, ValidType);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*modèle*")
            .WithParameterName("model");
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(1000)]
    [InlineData(500)]
    public void Create_WithYearTooOld_ShouldThrowArgumentException(int year)
    {
        // Arrange & Act
        var act = () => Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, year: year);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*entre 1900 et*")
            .WithParameterName("year");
    }

    [Fact]
    public void Create_WithYearTooFarInFuture_ShouldThrowArgumentException()
    {
        // Arrange
        var yearTooFar = DateTime.UtcNow.Year + 2;

        // Act
        var act = () => Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, year: yearTooFar);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*entre 1900 et*")
            .WithParameterName("year");
    }

    #endregion

    #region Update - Success Cases

    [Fact]
    public void Update_WithNewModel_ShouldUpdateModel()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        const string newModel = "Megane";

        // Act
        vehicle.Update(model: newModel);

        // Assert
        vehicle.Model.Should().Be(newModel);
    }

    [Fact]
    public void Update_WithNewColor_ShouldUpdateColor()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        const string newColor = "Bleu";

        // Act
        vehicle.Update(color: newColor);

        // Assert
        vehicle.Color.Should().Be(newColor);
    }

    [Fact]
    public void Update_WithNewYear_ShouldUpdateYear()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        const int newYear = 2022;

        // Act
        vehicle.Update(year: newYear);

        // Assert
        vehicle.Year.Should().Be(newYear);
    }

    [Fact]
    public void Update_WithAllFields_ShouldUpdateAllFields()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        const string newModel = "Megane";
        const string newColor = "Bleu";
        const int newYear = 2022;

        // Act
        vehicle.Update(newModel, newColor, newYear);

        // Assert
        vehicle.Model.Should().Be(newModel);
        vehicle.Color.Should().Be(newColor);
        vehicle.Year.Should().Be(newYear);
    }

    [Fact]
    public void Update_WithWhitespaceInModel_ShouldTrimModel()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        const string modelWithSpaces = "  Megane  ";

        // Act
        vehicle.Update(model: modelWithSpaces);

        // Assert
        vehicle.Model.Should().Be("Megane");
    }

    [Fact]
    public void Update_WithWhitespaceInColor_ShouldTrimColor()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        const string colorWithSpaces = "  Bleu  ";

        // Act
        vehicle.Update(color: colorWithSpaces);

        // Assert
        vehicle.Color.Should().Be("Bleu");
    }

    [Fact]
    public void Update_WithEmptyColor_ShouldSetColorToNull()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, "Rouge");

        // Act
        vehicle.Update(color: "   ");

        // Assert
        vehicle.Color.Should().BeNull();
    }

    [Fact]
    public void Update_WithNullModel_ShouldNotUpdateModel()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        var originalModel = vehicle.Model;

        // Act
        vehicle.Update(model: null);

        // Assert
        vehicle.Model.Should().Be(originalModel);
    }

    [Fact]
    public void Update_WithEmptyModel_ShouldNotUpdateModel()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        var originalModel = vehicle.Model;

        // Act
        vehicle.Update(model: "   ");

        // Assert
        vehicle.Model.Should().Be(originalModel);
    }

    [Fact]
    public void Update_WithNullYear_ShouldNotUpdateYear()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType, year: 2020);
        var originalYear = vehicle.Year;

        // Act
        vehicle.Update(year: null);

        // Assert
        vehicle.Year.Should().Be(originalYear);
    }

    #endregion

    #region Update - Validation Failures

    [Theory]
    [InlineData(1899)]
    [InlineData(1000)]
    public void Update_WithYearTooOld_ShouldThrowArgumentException(int invalidYear)
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);

        // Act
        var act = () => vehicle.Update(year: invalidYear);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*entre 1900 et*")
            .WithParameterName("year");
    }

    [Fact]
    public void Update_WithYearTooFarInFuture_ShouldThrowArgumentException()
    {
        // Arrange
        var vehicle = Vehicle.Create(_validUserId, ValidBrandId, ValidModel, ValidType);
        var yearTooFar = DateTime.UtcNow.Year + 2;

        // Act
        var act = () => vehicle.Update(year: yearTooFar);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*entre 1900 et*")
            .WithParameterName("year");
    }

    #endregion
}
