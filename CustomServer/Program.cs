// See https://aka.ms/new-console-template for more information

using CustomServeer;

Console.WriteLine("Hello, World!");
var builder = new ApplicationBuilder();

var router = new Router();
builder.Use(async (context, next) =>
{
    await router.Build()(context);
});


var pipeline = builder.Build();
var server = new CustomServer(pipeline);


await server.StartServer();

