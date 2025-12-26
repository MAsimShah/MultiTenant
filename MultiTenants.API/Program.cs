using DAL.ApplicationContext;
using Duende.IdentityServer.Models;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Start - add Identity and Duende identity server
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
{
    options.EmitStaticAudienceClaim = true;
})
.AddAspNetIdentity<ApplicationUser>()
.AddInMemoryApiScopes(new[]
{
    new ApiScope("api.read", "Read Access")
})
.AddInMemoryClients(new[]
{
    new Client
    {
        ClientId = "client-app",
        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
        ClientSecrets = { new Secret("secret".Sha256()) },
        AllowedScopes = { "api.read" }
    }
})
.AddDeveloperSigningCredential();

// END - add Identity and Duende identity server

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