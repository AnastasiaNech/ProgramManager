var builder = WebApplication.CreateBuilder(args);

List<Thread> treads = new List<Thread>();
Object locker = new Object();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var logger = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("api/v1/commands", async (string nameLibraby) =>
{
        if (nameLibraby == null)
        {
            return Results.NotFound();
        }

    logger.LogInformation("Start {0}", nameLibraby);

    switch (nameLibraby.Trim())
        {
            case "CycleOfTenIterations1":
            treads.Add(CycleOfTenIterations1.CycleOfTenIterations.CycleOfTenIterationsLog(locker));
                break;
            case "CycleInfinitely2":
            treads.Add(CycleInfinitely2.CycleInfinitely.CycleInfinitelyLog(locker));
                break;
            case "CycleInfinitely3":
            treads.Add(CycleInfinitely3.CycleInfinitely.CycleInfinitelyLog(locker));
                break;
            default:
                return Results.NotFound();
    }

    return Results.Ok();
});

app.MapDelete("api/v1/commands", async (string nameLibraby) => {

    if (nameLibraby == null)
    {
        return Results.NotFound();
    }

    app.Logger.LogInformation("Stop {0}", nameLibraby);

    var thread = treads.FirstOrDefault(x => x.Name == nameLibraby);

    if (thread != null)
    {
        thread.Interrupt();
        treads.RemoveAll(x => x.Name == nameLibraby);
    }
    return Results.Ok();
});
app.Run();

