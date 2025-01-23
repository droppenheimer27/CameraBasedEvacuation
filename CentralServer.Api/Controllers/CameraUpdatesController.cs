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
    
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PeopleCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<PeopleCountDto>> GetPeopleOnSiteCount() 
        => Ok(await _mediator.Send(new GetPeopleCountOnSiteQuery()));
    
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