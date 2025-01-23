using CameraBasedEvacuation.Shared.ValueObjects;
using CentralServer.Application.CameraUpdates;
using CentralServer.Domain.CameraUpdates;
using CentralServer.Domain.CameraUpdates.Exceptions;
using Moq;

namespace CentralServer.Application.Tests.CameraUpdates;

[TestFixture]
public class GetPeopleOnCameraHandlerTests
{
    private Mock<ICameraUpdatesRepository> _cameraUpdatesMock;
    private GetPeopleCountOnCameraHandler _handler;

    [SetUp]
    public void Setup()
    {
        _cameraUpdatesMock = new Mock<ICameraUpdatesRepository>();
        _handler = new GetPeopleCountOnCameraHandler(_cameraUpdatesMock.Object);
    }
    
    [Test]
    public async Task Handle_GivenCameraUpdatesInPersistence_ShouldReturnPeopleOnCameraCount()
    {
        // Arrange
        var query = new GetPeopleCountOnCameraQuery(new NotEmptyString("C1"));
        var expectedCameraUpdates = new CameraUpdatesCollection(new List<CameraUpdate>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CameraId = new NotEmptyString("C1"),
                Timestamp = new DateTime(2023, 1, 1, 10, 0, 0),
                In = 5,
                Out = 3
            },
            new()
            {
                Id = Guid.NewGuid(),
                CameraId = new NotEmptyString("C1"),
                Timestamp = new DateTime(2023, 1, 1, 11, 0, 0),
                In = 7,
                Out = 2
            }
        });
        
        _cameraUpdatesMock
            .Setup(cameraUpdates => cameraUpdates.GetByCameraId(query.CameraId))
            .Returns(expectedCameraUpdates);
        
        // Act
        var peopleCountDto = await _handler.Handle(query, CancellationToken.None);

        // Assert
        
        Assert.Multiple(() =>
        {
            Assert.That(peopleCountDto, Is.Not.Null);
            Assert.That(peopleCountDto.Count, Is.EqualTo(7));
            
        });
        
        _cameraUpdatesMock.Verify(cameraUpdates => 
                cameraUpdates.GetByCameraId(
                    It.Is<NotEmptyString>(cameraId => cameraId == query.CameraId)
                ), 
            Times.Once
        );
    }
    
    [Test]
    public void Handle_WhenNoCameraUpdates_ShouldThrowException()
    {
        // Arrange
        const string cameraId = "C1";
        var query = new GetPeopleCountOnCameraQuery(new NotEmptyString(cameraId));
        
        _cameraUpdatesMock
            .Setup(cameraUpdates => cameraUpdates.GetByCameraId(query.CameraId))
            .Returns(new CameraUpdatesCollection(new List<CameraUpdate>()));
        
        // Act & Assert
        var exception = Assert.ThrowsAsync<CameraUpdateException>(
            async () => await _handler.Handle(query, CancellationToken.None)
        );

        // Assert
        Assert.That(exception.Message, Is.EqualTo($"No available updates for camera '{cameraId}'"));
        
        _cameraUpdatesMock.Verify(cameraUpdates => 
                cameraUpdates.GetByCameraId(
                    It.Is<NotEmptyString>(id => id == query.CameraId)
                ), 
            Times.Once
        );
    }
}