using MediatR;

namespace CentralServer.Application.CameraUpdates;

public record GetPeopleCountOnSiteQuery : IRequest<PeopleCountDto>;
