# Custom HTTP Server & MVC Framework (C#)
This project is a from-scratch implementation of a minimal HTTP server and MVC-style framework in C#,
inspired by ASP.NET Core and Kestrel. The goal of this project is learning and demonstrating deep 
understanding of how web servers work internally, including TCP, HTTP parsing, routing, middleware pipelines, and model binding — without using ASP.NET Core.

## Project Goals
- Understand how HTTP works over raw TCP
- Build a minimal web server without Kestrel
- Implement an MVC-style framework using reflection
- Recreate core ASP.NET Core concepts:
  - HttpContext
  - middleware pipeline
  - routing
  - controller invocation
  - model binding (JSON body → DTO)
- Maintain clean separation of concerns between transport, framework, and application code
This is a learning framework, not a production web server.

## Key Features
### HTTP Server (CustomServer)
- Built directly on TcpListener and NetworkStream
- Manual HTTP parsing:
  - request line
  - headers
  - body (Content-Length)
- Correct handling of:
  - partial reads
  - leftover bytes
  - blocking reads
- Proper protocol-level error responses (400, 500)
- Raw HTTP response writing (no ASP.NET abstractions)

## Supported HTTP Verbs
- GET
- POST
- PUT
- DELETE
```
GET    /users/{id}
POST   /users
PUT    /users/{id}
DELETE /users/{id}
```
## MVC Framework (CustomMvc)
- Attribute-based routing:
```
[HttpPost("/users")]
public IActionResult Create(UserDto dto)
```
- Trie-based route matching
- Reflection-based controller discovery
- Automatic model binding:
  - route parameters
  - query parameters
  - JSON request body → DTO
```
public class UserController : ControllerBase
{
    [HttpPost("/users")]
    public IActionResult Create(UserDto dto)
    {
        return Ok(dto);
    }
}
```
Request:
```
POST /users HTTP/1.1
Content-Type: application/json
Content-Length: 27

{"name":"Zakir","age":30}
```
DTO is automatically deserialized and injected.
