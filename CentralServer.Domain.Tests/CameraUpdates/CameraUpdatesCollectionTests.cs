using CentralServer.Domain.CameraUpdates;

namespace CentralServer.Domain.Tests.CameraUpdates;

[TestFixture]
public class CameraUpdatesCollectionTests
{
    [Test]
    public void Constructor_GivenNullCameraUpdates_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new CameraUpdatesCollection(null));

        Assert.That(exception.ParamName, Is.EqualTo("cameraUpdates"));
    }
    
    [Test]
    public void CurrentCountPeopleOnSite_GivenNoUpdates_ReturnsZero()
    {
        // Arrange
        var updates = new List<CameraUpdate>();
        var collection = new CameraUpdatesCollection(updates);

        // Act
        var result = collection.CurrentCountPeopleOnSite(DateTime.Now);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }
    
    [Test]
    public void CurrentCountPeopleOnSite_GivenOrderedUpdates_ReturnsCorrectCount()
    {
        // Arrange
        var updates  = new List<CameraUpdate>
        {
            new() { Timestamp = new DateTime(2025, 1, 1, 10, 0, 0), In = 5, Out = 3 },
            new() { Timestamp = new DateTime(2025, 1, 1, 11, 0, 0), In = 7, Out = 2 }
        };
        
        var latestUpdate = new DateTime(2025, 1, 1, 12, 0, 0);
        var collection  = new CameraUpdatesCollection(updates );
        
        // Act
        var peopleCount = collection.CurrentCountPeopleOnSite(latestUpdate);
        
        // Assert
        Assert.That(peopleCount, Is.EqualTo(7));
    }
    
    [Test]
    public void CurrentCountPeopleOnSite_Given_OutOfOrderUpdates_ReturnsCorrectCount()
    {
        // Arrange
        var updates = new List<CameraUpdate>
        {
            new() { Timestamp = new DateTime(2025, 1, 1, 11, 0, 0), In = 7, Out = 4 },
            new() { Timestamp = new DateTime(2025, 1, 1, 10, 0, 0), In = 5, Out = 3 }
        };
        
        var collection = new CameraUpdatesCollection(updates);
        var latestTimestamp = new DateTime(2025, 1, 1, 12, 0, 0);

        // Act
        var result = collection.CurrentCountPeopleOnSite(latestTimestamp);

        // Assert
        Assert.That(result, Is.EqualTo(5));
    }
    
    [Test]
    public void CurrentCountPeopleOnSite_GivenSomeUpdatesAfterTimestamp_IgnoresLateUpdates()
    {
        // Arrange
        var updates = new List<CameraUpdate>
        {
            new() { Timestamp = new DateTime(2025, 1, 1, 10, 0, 0), In = 5, Out = 3 },
            new() { Timestamp = new DateTime(2025, 1, 1, 11, 0, 0), In = 7, Out = 4 },
            new() { Timestamp = new DateTime(2025, 1, 1, 13, 0, 0), In = 10, Out = 2 }
        };
        var collection = new CameraUpdatesCollection(updates);
        var latestTimestamp = new DateTime(2025, 1, 1, 12, 0, 0);

        // Act
        var result = collection.CurrentCountPeopleOnSite(latestTimestamp);

        // Assert
        Assert.That(result, Is.EqualTo(5)); // Late update (13:00) is ignored
    }
    
    [Test]
    public void CurrentCountPeopleOnSite_GivenAllUpdatesAfterTimestamp_ReturnsZero()
    {
        // Arrange
        var updates = new List<CameraUpdate>
        {
            new() { Timestamp = new DateTime(2025, 1, 1, 13, 0, 0), In = 10, Out = 2 },
            new() { Timestamp = new DateTime(2025, 1, 1, 14, 0, 0), In = 8, Out = 6 }
        };
        var collection = new CameraUpdatesCollection(updates);
        var latestTimestamp = new DateTime(2025, 1, 1, 12, 0, 0);

        // Act
        var result = collection.CurrentCountPeopleOnSite(latestTimestamp);

        // Assert
        Assert.That(result, Is.EqualTo(0)); // No updates are before the timestamp
    }
    
    [Test]
    public void CurrentCountPeopleOnSite_GivenDuplicateUpdates_RemovesDuplicates()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var updates = new List<CameraUpdate>
        {
            new() { Timestamp = timestamp.AddMinutes(-5), In = 5, Out = 3 },
            new() { Timestamp = timestamp.AddMinutes(-5), In = 5, Out = 3 } // Duplicate
        };

        var collection = new CameraUpdatesCollection(updates);

        // Act
        var result = collection.CurrentCountPeopleOnSite(timestamp);

        // Assert
        Assert.That(result, Is.EqualTo(2)); // (5 - 3), counted once
    }
    
    [Test]
    public void CurrentCountPeopleOnSite_GivenOutOfOrderUpdates_ProcessesChronologically()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var updates = new List<CameraUpdate>
        {
            new() { Timestamp = timestamp.AddMinutes(-1), In = 3, Out = 1 },
            new() { Timestamp = timestamp.AddMinutes(-5), In = 5, Out = 3 }
        };

        var collection = new CameraUpdatesCollection(updates);

        // Act
        var result = collection.CurrentCountPeopleOnSite(timestamp);

        // Assert
        Assert.That(result, Is.EqualTo(4)); // (5 - 3) + (3 - 1), processed in order
    }
    
    [Test]
    public void CurrentCountPeopleOnSite_GivenDuplicateUpdates_CorrectCalculation()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var updates = new List<CameraUpdate>
        {
            new() { Timestamp = timestamp.AddMinutes(-9), In = 10, Out = 2 },
            new() { Timestamp = timestamp.AddMinutes(-5), In = 5, Out = 3 },
            new() { Timestamp = timestamp.AddMinutes(-5), In = 4, Out = 4 },
            new() { Timestamp = timestamp.AddMinutes(-5), In = 5, Out = 3 }, // Duplicate
            new() { Timestamp = timestamp.AddMinutes(-1), In = 7, Out = 2 }
        };

        var collection = new CameraUpdatesCollection(updates);

        // Act
        var result = collection.CurrentCountPeopleOnSite(timestamp);

        // Assert
        Assert.That(result, Is.EqualTo(15)); // (10 - 2) + (5 - 3) + (0) + (7 - 2)
    }
    
    [Test]
    public void Count_ReturnsCorrectCount()
    {
        // Arrange
        var updates = new List<CameraUpdate>
        {
            new() { Timestamp = new DateTime(2025, 1, 1, 10, 0, 0), In = 5, Out = 3 },
            new() { Timestamp = new DateTime(2025, 1, 1, 11, 0, 0), In = 7, Out = 4 }
        };
        var collection = new CameraUpdatesCollection(updates);

        // Act
        var result = collection.Count;

        // Assert
        Assert.That(result, Is.EqualTo(2));
    }
}