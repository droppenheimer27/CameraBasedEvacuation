namespace RemoteCamera.Application.Settings;

public class CentralServerActorSystemSettings
{
    public const string Section = nameof(CentralServerActorSystemSettings);
    
    public string ActorSystemName { get; set; }
    
    public string Address { get; set; }
    
    public int Port { get; set; }
    
    public string ActorPath { get; set; }

    public string ToActorPath() => $"akka.tcp://{ActorSystemName}@{Address}:{Port}/{ActorPath}";
}