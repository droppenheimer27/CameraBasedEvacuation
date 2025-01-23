using CentralServer.Application.CameraUpdates;
using CentralServer.Domain.CameraUpdates;
using CentralServer.Domain.CameraUpdates.Exceptions;
using Moq;

namespace CentralServer.Application.Tests.CameraUpdates;

[TestFixture]
public class GetPeopleOnSiteHandlerTests
{
    private Mock<ICameraUpdatesRepository> _cameraUpdatesMock;
    private GetPeopleCountOnSiteHandler _handler;

    [SetUp]
    public void Setup()
    {
        _cameraUpdatesMock = new Mock<ICameraUpdatesRepository>();
        _handler = new GetPeopleCountOnSiteHandler(_cameraUpdatesMock.Object);
    }
    
    [Test]
    public async Task Handle_GivenCameraUpdatesInPersistence_ShouldReturnPeopleOnCameraCount()
    {
        // Arrange
        var query = new GetPeopleCountOnSiteQuery();
        var expectedCameraUpdates = new CameraUpdatesCollection(new List<CameraUpdate>
        {
            new() { Timestamp = new DateTime(2023, 1, 1, 10, 0, 0), In = 5, Out = 3 },
            new() { Timestamp = new DateTime(2023, 1, 1, 11, 0, 0), In = 7, Out = 2 },
            new() { Timestamp = new DateTime(2023, 1, 1, 12, 0, 0), In = 10, Out = 4 }
        });
        
        _cameraUpdatesMock
            .Setup(cameraUpdates => cameraUpdates.GetAll())
            .Returns(expectedCameraUpdates);
        
        // Act
        var peopleCountDto = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(peopleCountDto, Is.Not.Null);
            Assert.That(peopleCountDto.Count, Is.EqualTo(13)); 
        });
        
        _cameraUpdatesMock.Verify(cameraUpdates => cameraUpdates.GetAll(), Times.Once);
    }
    
    [Test]
    public void Handle_WhenNoCameraUpdates_ShouldThrowException()
    {
        // Arrange
        var query = new GetPeopleCountOnSiteQuery();
        
        _cameraUpdatesMock
            .Setup(cameraUpdates => cameraUpdates.GetAll())
            .Returns(new CameraUpdatesCollection(new List<CameraUpdate>()));
        
        // Act & Assert
        var exception = Assert.ThrowsAsync<CameraUpdateException>(
            async () => await _handler.Handle(query, CancellationToken.None)
        );
    
        // Assert
        Assert.That(exception.Message, Is.EqualTo("No available camera updates"));
        _cameraUpdatesMock.Verify(cameraUpdates => cameraUpdates.GetAll(), Times.Once);
    }
}