using CameraBasedEvacuation.Shared.ValueObjects;

namespace CameraBasedEvacuation.Domain.Shared.Tests.ValueObjects;

[TestFixture]
public class NonNegativeIntTests
{
    [Test]
    public void Constructor_GivenValueIsZeroOrPositive_ShouldCreateObject()
    {
        // Arrange & Act
        var validInt = new NonNegativeInt(0);
        var anotherValidInt = new NonNegativeInt(5);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(validInt.Value, Is.EqualTo(0));
            Assert.That(anotherValidInt.Value, Is.EqualTo(5));
        });
    }

    [Test]
    public void Constructor_GivenValueIsNegative_ShouldThrowException()
    {
        // Arrange & Act
        var exception = Assert.Throws<ArgumentException>(() => new NonNegativeInt(-1));
        
        // Assert
        Assert.That(exception.Message, Is.EqualTo("The value cannot be negative. (Parameter 'value')"));
    }
}