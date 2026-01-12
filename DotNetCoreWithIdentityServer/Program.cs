using DAL.ApplicationContext;
using DAL.Interfaces;
using DAL.Repositories;
using DotNetCoreWithIdentityServer.Models;
using DTO;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var identityOptions = builder.Configuration
    .GetSection("IdentityServer")
    .Get<IdentityServerOptions>();

// Start - add Identity and Duende identity server
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;        // minimum length
    options.Password.RequireDigit = false;      // do NOT require digits
    options.Password.RequireLowercase = false;  // do NOT require lowercase letters
    options.Password.RequireUppercase = false;  // do NOT require uppercase letters
    options.Password.RequireNonAlphanumeric = false; // do NOT require symbols
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
{
    options.EmitStaticAudienceClaim = true;
    options.KeyManagement.Enabled = false;
})
.AddAspNetIdentity<ApplicationUser>()
.AddInMemoryApiScopes(IdentityConfig.ApiScopes)
.AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
.AddInMemoryClients(IdentityConfig.Clients)
.AddDeveloperSigningCredential()
.AddOperationalStore(options =>
{
    options.ConfigureDbContext = b => b.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
         sql => sql.MigrationsAssembly("DAL"));

    options.EnableTokenCleanup = true;
    options.TokenCleanupInterval = 3600; // clean every hour
});

// END - add Identity and Duende identity server

// implement jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = identityOptions?.Authority;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
});

builder.Services.AddAuthorization();

// end of implement jwt

// Add services to the container.
builder.Services.AddControllers();

// implement NSwag for API documentation
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "My API";
    config.Version = "v1";

    // Define the OAuth2 security scheme (keep your existing one)
    config.AddSecurity("oauth2", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.OAuth2,
        Flow = NSwag.OpenApiOAuth2Flow.Password,
        TokenUrl = identityOptions?.TokenEndpoint,
        Scopes = new Dictionary<string, string>
        {
            { "myapi", "Access My API" }
        }
    });

    // THIS is the key: use the built-in processor
    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("oauth2"));
});


// register custom services
builder.Services.Configure<IdentityServerOptions>(
    builder.Configuration.GetSection("IdentityServer"));
builder.Services.AddHttpClient<IAccountServices, AccountServices>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddTransient<IAccountServices, AccountServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseOpenApi();      // /swagger/v1/swagger.json
app.UseSwaggerUi(settings =>
{
    settings.OAuth2Client = new NSwag.AspNetCore.OAuth2ClientSettings
    {
        ClientId = "web-client",
        ClientSecret = "secret",
        AppName = "My API Swagger",
        UsePkceWithAuthorizationCodeGrant = false // ROPC flow
    };
});

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
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();