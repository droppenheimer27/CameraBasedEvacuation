namespace CentralServer.Domain.CameraUpdates.Exceptions;

public class CameraUpdateException : InvalidOperationException
{
    public CameraUpdateException(string message) : base(message)
    { }
}