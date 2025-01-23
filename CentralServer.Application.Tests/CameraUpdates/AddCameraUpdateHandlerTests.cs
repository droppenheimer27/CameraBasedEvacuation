using CameraBasedEvacuation.Shared.ValueObjects;
using CentralServer.Application.CameraUpdates;
using CentralServer.Domain.CameraUpdates;
using Moq;

namespace CentralServer.Application.Tests.CameraUpdates;

[TestFixture]
public class AddCameraUpdateHandlerTests
{
    private Mock<ICameraUpdatesRepository> _cameraUpdatesMock;
    private AddCameraUpdateHandler _handler;

    [SetUp]
    public void Setup()
    {
        _cameraUpdatesMock = new Mock<ICameraUpdatesRepository>();
        _handler = new AddCameraUpdateHandler(_cameraUpdatesMock.Object);
    }

    [Test]
    public async Task Handle_WhenInputCommandIsValid_ShouldSaveMessageToPersistence()
    {
        // Arrange
        var command = new AddCameraUpdateCommand
        {
            CameraId = new NotEmptyString("C1"),
            Timestamp = DateTime.UtcNow,
            In = 1,
            Out = 0
        };

        var cameraUpdate = new CameraUpdate
        {
            CameraId = command.CameraId,
            Timestamp = command.Timestamp,
            In = command.In,
            Out = command.Out
        };

        _cameraUpdatesMock.Setup(cameraUpdates => cameraUpdates.Save(cameraUpdate));
        

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _cameraUpdatesMock.Verify(cameraUpdates => 
                cameraUpdates.Save(
                    It.Is<CameraUpdate>(message => 
                        message.CameraId == command.CameraId &&
                        message.Timestamp == command.Timestamp &&
                        message.In == command.In &&
                        message.Out == command.Out
                    )
                ), 
            Times.Once
        );
    }
}