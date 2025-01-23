using CentralServer.Actor;

var app = Bootstrapper.Setup(WebApplication.CreateBuilder(args));
await app.RunAsync();