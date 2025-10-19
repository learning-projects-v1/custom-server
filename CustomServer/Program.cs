// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CustomServeer;

Console.WriteLine("Hello, World!");
var builder = new ApplicationBuilder();

// var router = new Router();
var router = new TrieBasedRouter();
router.MapControllers(Assembly.GetExecutingAssembly());

builder.Use(async (context, next) =>
{
    await router.Build()(context);
});


var pipeline = builder.Build();
var server = new CustomServer(pipeline);


await server.StartServer();

