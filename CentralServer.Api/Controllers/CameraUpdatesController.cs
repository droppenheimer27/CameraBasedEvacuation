using System.Net.Mime;
using CameraBasedEvacuation.Shared.ValueObjects;
using CentralServer.Application;
using CentralServer.Application.CameraUpdates;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CentralServer.Actor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CameraUpdatesController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public CameraUpdatesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Retrieve the total number of people currently on-site.
    /// This endpoint aggregates the data from all camera updates and calculates
    /// the total number of people present across all zones.
    /// </summary>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PeopleCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<PeopleCountDto>> GetPeopleOnSiteCount() 
        => Ok(await _mediator.Send(new GetPeopleCountOnSiteQuery()));
    
    /// <summary>
    /// Retrieve the number of people currently reported by a specific camera.
    /// This endpoint filters the data to show the count from a single camera.
    /// </summary>
    /// <param name="cameraId">The unique identifier of the camera (e.g., "C1", "C2")</param>
    [HttpGet("{cameraId}")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PeopleCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<PeopleCountDto>> GetPeopleOnCameraCount([FromRoute] string cameraId) 
        => Ok(await _mediator.Send(new GetPeopleCountOnCameraQuery(new NotEmptyString(cameraId))));
}