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
    
    // For production, a custom BackoffSupervisor should replace the generic one to better handle retries, failure conditions, 
    // and specific business logic related to camera actor recovery. However, for demonstration purposes, 
    // the generic BackoffSupervisor suffices to provide basic supervision and fault tolerance.
    public CamerasController(IRequiredActor<BackoffSupervisor> actor)
    {
        _cameraActor = actor.ActorRef;
    }

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

    private CameraUpdateMessage ConvertToMessage(CameraUpdateDto dto)
        => new()
        {
            Timestamp = new ValidDateTime(dto.TimeStamp),
            In = dto.In,
            Out = dto.Out
        };
}