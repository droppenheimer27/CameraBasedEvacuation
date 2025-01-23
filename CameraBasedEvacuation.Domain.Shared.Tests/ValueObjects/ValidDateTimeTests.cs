using CameraBasedEvacuation.Shared.ValueObjects;

namespace CameraBasedEvacuation.Domain.Shared.Tests.ValueObjects;


[TestFixture]
public class ValidDateTimeTests
{
    [Test]
    public void Constructor_GivenValidDate_AssignsValue()
    {
        // Arrange
        var validDate = DateTime.UtcNow.AddHours(-1); // A past valid date

        // Act
        var validDateTime = new ValidDateTime(validDate);

        // Assert
        Assert.That(validDateTime.Value, Is.EqualTo(validDate));
    }

    [Test]
    public void Constructor_GivenMinDate_ThrowsArgumentException()
    {
        // Arrange
        var invalidDate = DateTime.MinValue;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new ValidDateTime(invalidDate));
        Assert.Multiple(() =>
        {
            Assert.That(exception.ParamName, Is.EqualTo("value"));
            Assert.That(exception.Message, Does.Contain("Date cannot be the minimum DateTime value"));
        });
    }

    [Test]
    public void Constructor_GivenFutureDate_ThrowsArgumentException()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddMinutes(1); // A date in the future

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new ValidDateTime(futureDate));
        Assert.Multiple(() =>
        {
            Assert.That(exception.ParamName, Is.EqualTo("value"));
            Assert.That(exception.Message, Does.Contain("Date cannot be in the future"));
        });
    }
}