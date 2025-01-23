namespace RemoteCamera.Application;

public record CameraUpdateDto
{
    public DateTime TimeStamp { get; init; }
    
    public int In { get; init; }
    
    public int Out { get; init; }
}