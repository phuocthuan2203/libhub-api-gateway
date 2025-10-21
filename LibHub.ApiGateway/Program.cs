using LibHub.ApiGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- START: Add services to the container ---

// 1. Configure CORS to allow the Angular frontend to communicate with the gateway.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Configure Service Discovery
builder.Services.AddServiceDiscovery();

// 3. Add and Configure YARP with Service Discovery.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

// --- END: Add services to the container ---

var app = builder.Build();

// --- START: Configure the HTTP request pipeline ---

// Use the Correlation ID middleware on every request.
app.UseMiddleware<CorrelationIdMiddleware>();

// Use the CORS policy we defined.
app.UseCors("AllowAngularApp");

// Add a simple health check endpoint for the gateway itself.
app.MapGet("/health", () => Results.Ok("API Gateway is healthy."));

// Enable the YARP reverse proxy.
app.MapReverseProxy();

// --- END: Configure the HTTP request pipeline ---

app.Run();
