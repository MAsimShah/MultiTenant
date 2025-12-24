var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// implement NSwag for API documentation
builder.Services.AddOpenApiDocument();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseOpenApi();      // /swagger/v1/swagger.json
app.UseSwaggerUi();    // /swagger

// Redoc HTML endpoint at /redoc
app.MapGet("/redoc", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(@"
<!DOCTYPE html>
<html>
  <head>
    <title>API Docs - Redoc</title>
  </head>
  <body>
    <redoc spec-url='/swagger/v1/swagger.json'></redoc>

    <!-- Load Redoc from CDN -->
    <script src=""https://cdn.redoc.ly/redoc/latest/bundles/redoc.standalone.js""></script>
  </body>
</html>");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();