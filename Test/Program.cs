// See https://aka.ms/new-console-template for more information

using CustomHttp;
using CustomServer;


var builder = new ApplicationBuilder();
builder.Use(async (context, next) =>
{
    Console.WriteLine("First middleware");
    await next(context);
});

builder.Use(async (context, next) =>
{
    Console.WriteLine("Second middleware");
    await next(context);
});

builder.Use(async (context, next) =>
{
    context.Response.StatusCode = 200;
    context.Response.Headers["Content-Type"] = "text/plain";
    context.Response.Headers["Content-Length"] = "50";
    context.Response.Body = "<h1>Ki ase jibone<h1></br><p>Kisu nai:(</p>";
    await next(context);
});

builder.Build();
var pipeline = builder.Build();
var server = new CustomServer.CustomServer(pipeline);

server.Start();
