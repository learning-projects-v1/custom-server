// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CustomHttp;
using CustomMvc;
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


var router = new TrieRouter();
router.MapControllers(Assembly.GetExecutingAssembly());
builder.Use(async (context, next) =>
{
    await router.UseRouting()(context);
    await next(context);
});

var pipeline = builder.Build();
var server = new CustomServer.CustomServer(pipeline);

server.Start();
