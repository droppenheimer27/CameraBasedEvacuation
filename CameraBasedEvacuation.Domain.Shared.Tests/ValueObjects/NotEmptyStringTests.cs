using CameraBasedEvacuation.Shared.ValueObjects;

namespace CameraBasedEvacuation.Domain.Shared.Tests.ValueObjects;

[TestFixture]
public class NotEmptyStringTests
{
    [Test]
    [TestCase("cool string")]
    [TestCase("   a    lot   of      spaces          ")]
    public void Constructor_GivenValidStrings_ShouldCreate(string str)
    {
        // Act & Assert
        Assert.DoesNotThrow(() => new NotEmptyString(str));
    }
    
    [Test]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("\t")]
    [TestCase("\n")]
    [TestCase(null)]
    public void Constructor_GivenNotValidStrings_ShouldThrowException(string? str)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new NotEmptyString(str));
    }
}