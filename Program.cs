var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Register in-memory user repository
builder.Services.AddSingleton<UserManagementAPI.Services.IUserRepository, UserManagementAPI.Services.InMemoryUserRepository>();
// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();   
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Minimal APIs OpenAPI mapping (if present) and Swagger UI for Swashbuckle
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ensure Swagger is available in non-development if desired (optional)
// app.UseSwagger();
// app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
