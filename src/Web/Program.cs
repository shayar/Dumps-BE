using Application.Services;
using Core.Interfaces;
using Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add cors configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:3000") // Allow specific origins
            .AllowAnyHeader()                 // Allow any headers
            .AllowAnyMethod();                // Allow any HTTP methods
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy"); // Use cors policy
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
