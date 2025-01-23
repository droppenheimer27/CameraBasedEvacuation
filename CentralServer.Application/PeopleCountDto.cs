namespace CentralServer.Application;

public record PeopleCountDto
{
    public int Count { get; init; }

    public PeopleCountDto(int count)
    {
        Count = count;
    }
}