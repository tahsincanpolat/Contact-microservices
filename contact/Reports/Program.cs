using MassTransit;
using Microsoft.EntityFrameworkCore;
using Reports.DataAccess;
using Reports.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ReportsDBContext>(
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


// Get All Reports

app.MapGet("/getReports", async (ReportsDBContext db) =>
{
    var reports  = await db.AllReports.ToListAsync();
    if(reports == null)
    {
        return Results.BadRequest("Report not found");
    }
    else
    {
        return Results.Ok(reports);
    }

});

// Create Report

app.MapPost("/createReport", async (ReportsDBContext db, ISendEndpointProvider sendEndpointProivder) =>
{
    AllReports report = new AllReports();
});

app.Run();
