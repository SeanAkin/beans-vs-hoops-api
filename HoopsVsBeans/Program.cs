using HoopsVsBeans.Data;
using HoopsVsBeans.Data.Models;
using HoopsVsBeans.Middleware;
using HoopsVsBeans.ServiceCollectionExtensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerApiKeySupport();

builder.Services.AddSecrets(builder.Configuration);
builder.Services.AddDbContext<HoopsVsBeansContext>(opt 
    => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
    app.UseMiddleware<IpRestrictionMiddleware>();
}

app.UseMiddleware<ApiKeyMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HoopsVsBeansContext>();
    context.Database.Migrate();
}
app.UsePathBase("/hoops-vs-beans-api");
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPut("/Vote/{option}", async (HoopsVsBeansContext dbContext, string option) =>
{
    option = option.ToLower();
    if (option != "beans" && option != "hoops")
    {
        return Results.BadRequest("Invalid voting option. Choose 'Hoops' or 'Beans'.");
    }

    var column = option == "hoops" ? "Hoops" : "Beans";
    var sql = $"UPDATE VoteOptions SET {column} = {column} + 1 WHERE Id = 1;";

    await dbContext.Database.ExecuteSqlRawAsync(sql);
    dbContext.Votes.Add(new Vote() { VoteTime = DateTime.UtcNow, OptionVoted = option });
    await dbContext.SaveChangesAsync();

    return Results.Ok(await dbContext.VoteOptions.FirstAsync());
});

app.MapGet("/VoteCount", async (HoopsVsBeansContext dbContext) => await dbContext.VoteOptions.FirstAsync());

app.Run();