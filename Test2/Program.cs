// See https://aka.ms/new-console-template for more information

using System.Reflection;
using CustomServerHttp;
using CustomServerMvc;
using CustomServer;

var builder = new ApplicationBuilder();
var router = new TrieRouter();

router.MapControllers(Assembly.GetExecutingAssembly());
builder.Use(async (context, next) =>
{
    await router.UseRouting()(context);
    await next(context);
});

var pipeline = builder.Build();
var server = new HttpServer(pipeline);

server.Start();