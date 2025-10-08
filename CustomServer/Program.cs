// See https://aka.ms/new-console-template for more information

using CustomServeer;

Console.WriteLine("Hello, World!");
var builder = new ApplicationBuilder();

var router = new Router();
// router.Register("/api/demo", async context =>
// {
//     // do something
//     context.Response.StatusCode = 202;
//     context.Response.Headers.Add("Content-Type", "text/plain");
//     context.Response.Body = "My first respone!";
//     Console.WriteLine($"At route: request body-{context.Request.RequestBody}");
// });

builder.Use(async (context, next) =>
{
    await router.Build()(context);
});


var pipeline = builder.Build();
var server = new CustomServer(pipeline);


await server.StartServer();

