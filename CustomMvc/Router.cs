using System.ComponentModel;
using System.Reflection;
using CustomHttp;

namespace CustomMvc;

public class RouteItem
{
    public RequestDelegate Handler { get; set; }
    public Dictionary<string, RouteItem> Children { get; set; } = new Dictionary<string, RouteItem>();
    public bool IsLeaf { get; set; }
    public RouteItem? ParameterChild { get; set; }
}

public interface IRouter
{
    void Build();
    void MapControllers(Assembly assembly);
}

public class TrieRouter : IRouter
{
    private RouteItem root = new RouteItem();
    private void AddRoute(string[] pathSegment, RequestDelegate handler)
    {
        var cur = root;
        foreach (var path in pathSegment)
        {
            if (path.StartsWith('{'))
            {
                if(cur.ParameterChild == null) cur.ParameterChild = new RouteItem();
                cur = cur.ParameterChild;
                return;
            }
            var routeItem = new RouteItem();
            if (!cur.Children.ContainsKey(path))
            { 
                cur.Children.Add(path, routeItem);
            }
            cur = cur.Children[path];
        }
        cur.Handler = handler;
        cur.IsLeaf = true;
    }

    private RouteItem? GetRoute(string[] pathSegment)
    {
        var cur = root;
        foreach (var path in pathSegment)
        {
            if (cur.Children.TryGetValue(path, out var child))
            {
                cur = child;
            }
            else if (cur.ParameterChild != null)
            {
                cur = cur.ParameterChild;
            }
            else return null;
        }

        if (cur.IsLeaf) return cur;
        return null;
    }
    
    public void MapControllers(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(t => t.IsPublic && t.IsAssignableTo(typeof(AControllerBase))))
        {
            var methods = type.GetMethods().Where(m => !m.IsAbstract);
            foreach (var method in methods)
            {
               var attributes = method.GetCustomAttributes<AHttpAttribute>();
               foreach (var attribute in attributes)
               {
                   var path = attribute._path.ToLower();
                   var pathSegments = path.Split('/');
                   RequestDelegate handler = (HttpContext httpContext) =>
                   {
                       var parameters = method.GetParameters();
                       var allParameters = GetParameters(pathSegments, httpContext.Request.PathSegments, httpContext.Request.QueryParams, parameters);
                       var controllerInstance = Activator.CreateInstance(type);
                       method.Invoke(controllerInstance, allParameters);
                       return Task.CompletedTask;
                   };
                   AddRoute(pathSegments, handler);
               }
            }
        }
    }
    
    

    private object[] GetParameters(string[] controllerPathSegments, string[] requestPathSegments, Dictionary<string, string> requestQueryParams, ParameterInfo[] parameters)
    {
        var routeParams = ParseRouteParameters(controllerPathSegments, requestPathSegments);
        foreach (var keyValuePair in routeParams)
        {
            requestQueryParams.Add(keyValuePair.Key, keyValuePair.Value);
        }
        var result = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var propName = parameters[i].Name;
            var type = parameters[i].ParameterType;
            if (requestQueryParams.TryGetValue(propName, out var value))
            {
                result[i] = Convert.ChangeType(value, type);
            }
        }
        return result;
    }

    private Dictionary<string, string> ParseRouteParameters(string[] controllerPathSegments, string[] requestPathSegments)
    {
        var routeParams = new Dictionary<string, string>();
        if (controllerPathSegments.Length != requestPathSegments.Length) return new();
        for(int i = 0; i < controllerPathSegments.Length; i++)
        {
            var controllerPathSegment = controllerPathSegments[i];
            var pathSegment = requestPathSegments[i];
            if (controllerPathSegment.StartsWith("{") && controllerPathSegment.EndsWith("}"))
            {
                var propertyName = controllerPathSegment.Trim('{', '}');
                routeParams.Add(propertyName, pathSegment);
            }
        }
        return routeParams;
    }

    public void Build()
    {
        
    }
}