using Akka.Actor;
using Akka.Hosting;
using Akka.Pattern;
using CameraBasedEvacuation.Shared.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using RemoteCamera.Application;
using RemoteCamera.Application.Actors;

namespace RemoteCamera.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CamerasController : ControllerBase
{
    private readonly IActorRef _cameraActor;
    
    public CamerasController(IRequiredActor<Application.Actors.RemoteCamera> actor)
    {
        _cameraActor = actor.ActorRef;
    }

    /// <summary>
    /// Add a new update from a camera.
    /// This endpoint accepts data about people entering and leaving from a specific camera
    /// and forwards it to the camera actor for processing.
    /// </summary>
    /// <returns>
    /// Returns `201 Created` on successful submission of the update.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public ActionResult AddNewCameraState([FromBody] CameraUpdateDto dto)
    {
        _cameraActor.Tell(ConvertToMessage(dto));
        return Created();
    }

    private static CameraUpdateMessage ConvertToMessage(CameraUpdateDto dto)
        => new()
        {
            Timestamp = new ValidDateTime(dto.TimeStamp),
            In = new NonNegativeInt(dto.In),
            Out = new NonNegativeInt(dto.Out)
        };
}