using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VKApp.Services;
using VKApp.DB;
using VKApp.models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationContext>(options =>
options.UseNpgsql(
    builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.TryAdd(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());
builder.Services.AddTransient<UserService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
dbContext.Database.Migrate();

// Получить всех пользователей или страницу пользователей (если pageSize = 0, то вернется все)
app.MapGet("/api/users", getGroupUser);

async Task<IResult> getGroupUser(ApplicationContext context, UserService userService, int pageNumber = 1, int pageSize = 0)
{
    var users = await userService.GetAllUsersAsync(pageNumber, pageSize);
    return Results.Ok(users);
}

// Получить пользователя по ID
app.MapGet("api/users/{id}", async (ApplicationContext context, int id, UserService userService) =>
{
    var user = await userService.GetUserByIdAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(user);
});

// Добавить нового пользователя
app.MapPost("/api/users", async (ApplicationContext context, User user, UserService userService) =>
{
    var registeredUser = await userService.RegisterUser(user);
    if (registeredUser != null)
    {
        return Results.Ok();
    }
    return Results.BadRequest("can't register user");
});

// Удалить пользователя
app.MapDelete("/api/users/{id}", async (ApplicationContext context, int id, UserService userService) =>
{
    var user = await userService.DeleteUserByIdAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }
    return Results.Ok();
});

app.Run();
