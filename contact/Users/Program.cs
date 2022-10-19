using Microsoft.EntityFrameworkCore;
using Users.DataAccess;
using Users.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<UserDBContext>(
    opt =>
    {
        opt.EnableSensitiveDataLogging();
        opt.EnableDetailedErrors();
        opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }, ServiceLifetime.Transient
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/users", async ( UserDBContext db) =>
{
    if (db.Users.ToList() != null)
    {
        var users = await db.Users.ToListAsync();
        return Results.Ok(users);
    }
    else
    {
        return Results.BadRequest("Error! Users not found.");
    }

});

app.MapGet("/userById/{id}", async (Guid? id, UserDBContext db) =>
{
    if (id.HasValue)
    {
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        return Results.Ok(user);
    }
    else
    {
        return Results.BadRequest("Error! Please enter a valid id.");
    }

});

app.MapPost("/userAdd", async (User user,UserDBContext db) =>
{
    await db.Users.AddAsync(user);
    if (db.SaveChanges()>0)
    {
        return Results.Ok(user);
    }
    else
    {
        return Results.BadRequest("Error! Could not add user.");
    }

});

app.MapDelete("/userDelete/{id}", async (Guid? id, UserDBContext db) =>
{
    User? getUser = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
    if (getUser == null)
    {
        return Results.BadRequest("Error! User not found.");
    }
    else
    {
        db.Users.Remove(getUser);
        db.SaveChanges();
        return Results.Ok("User Deleted.");
    }

});




//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//       new WeatherForecast
//       (
//           DateTime.Now.AddDays(index),
//           Random.Shared.Next(-20, 55),
//           summaries[Random.Shared.Next(summaries.Length)]
//       ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

//app.Run();

//internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}