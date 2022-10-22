using Microsoft.EntityFrameworkCore;
using Users.DataAccess;
using Users.Models;

var builder = WebApplication.CreateBuilder(args);

// Default Connection Builder
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

// Get All Users
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

}).WithName("getUsers");

// Get User By Id
app.MapGet("/userById/{id}", async (Guid? id, UserDBContext db) =>
{
    if (id.HasValue)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
        return Results.Ok(user);
    }
    else
    {
        return Results.BadRequest("Error! Please enter a valid id.");
    }

}).WithName("getUser");


// Get User Details

app.MapGet("/userDetails", async (UserDBContext db) =>
{
    var usersDetail = await db.Users.Include(ud => ud.ContactInfos).ToListAsync();
    if(usersDetail == null)
    {
        return Results.BadRequest("Error! User Detail Not Found.");
    }

    return Results.Ok(usersDetail);

}).WithName("usersDetails");

// Get User Detail

app.MapGet("/userDetailById/{id}", async (Guid? id, UserDBContext db) =>
{
    if (id.HasValue)
    {
        var userDetail = await db.Users.Include(ud => ud.ContactInfos).FirstOrDefaultAsync(ud => ud.Id == id);

        return Results.Ok(userDetail);
    }
    else
    {
        return Results.BadRequest("Error! Please enter a valid id.");
    }

}).WithName("userDetail");

// Add User

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

}).WithName("addUser");

// Add User Detail
app.MapPost("/userDetailAdd/{id}", async (Guid id, ContactInfo info, UserDBContext db) =>
{
    var user = await db.Users.FindAsync(id);

    if (user == null)
    {
        return Results.BadRequest("Error! User not found.");
    }

    info.UId = id;
    await db.AddAsync(info);
    if (await db.SaveChangesAsync() > 0)
    {
        return Results.Ok(info);
    }
    return Results.BadRequest("User contact information could not be added.");
}).WithName("addUserDetail");


// Delete User

app.MapDelete("/userDelete/{id}", async (Guid? id, UserDBContext db) =>
{
    User user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
    ContactInfo contactInfo = await db.ContactInfos.FirstOrDefaultAsync(c=>c.UId == id);
    if (user == null)
    {
        return Results.BadRequest("Error! User not found.");
    }
    else
    {
        db.ContactInfos.Remove(contactInfo);
        db.Users.Remove(user);
        db.SaveChanges();
        return Results.Ok("User Deleted.");
    }

}).WithName("deleteUser");

// Delete User Detail
app.MapDelete("/userDetailDelete/{id}", async (Guid id, UserDBContext db) =>
{
    var userInfo = await db.ContactInfos.FindAsync(id);
    if (userInfo == null)
    {
        return Results.BadRequest("Contact Information not found");
    }
    else
    {
        db.ContactInfos.Remove(userInfo);
        await db.SaveChangesAsync();
        return Results.Ok("User information was deleted");
    }

    
}).WithName("deleteUserDetail");

app.Run();



