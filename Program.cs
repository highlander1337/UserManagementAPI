using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Disable automatic 400 responses so controllers can return custom ValidationProblemDetails
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

// Register in-memory user repository
builder.Services.AddSingleton<UserManagementAPI.Services.IUserRepository, UserManagementAPI.Services.InMemoryUserRepository>();
// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

// 1) Error handling must be first
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2) Authentication middleware next
app.UseMiddleware<TokenAuthenticationMiddleware>();

// 3) Logging middleware last (of our custom middlewares)
app.UseMiddleware<RequestResponseLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{   
    // Enable Swagger UI in development
    app.UseSwagger();
    app.UseSwaggerUI();
    // Map minimal API OpenAPI endpoints if present
    app.MapOpenApi();
}

// Common middleware ordering
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
