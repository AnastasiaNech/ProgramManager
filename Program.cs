using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

List<Thread> treads = new List<Thread>();
Object locker = new Object();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var logger = builder.Logging.Services.BuildServiceProvider()
    .GetRequiredService<ILogger<Program>>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Метод подключения библиотеки по названию в отдельном потоке
app.MapPost("api/v1/cycles", (string nameLibraby) =>
{
        if (nameLibraby == null)
        {
            return Results.NotFound();
        }

    switch (nameLibraby.Trim())
        {
            case "CycleOfTenIterations1": 
            Assembly asm = Assembly.LoadFrom("CycleOfTenIterations1.dll");
            Type? t = asm.GetType("CycleOfTenIterations1.CycleOfTenIterations");
            MethodInfo? thead = t?.GetMethod ("CycleOfTenIterationsLog", BindingFlags.Public | BindingFlags.Static);
            treads.Add((Thread)(thead?.Invoke(null, new object[] { locker })));
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

    logger.LogInformation("Start {0}", nameLibraby);
    return Results.Ok();
});

//Метод отключения потока по названию библиотеки
app.MapDelete("api/v1/cycles", (string nameLibraby) => {

    if (nameLibraby == null)
    {
        return Results.NotFound();
    }

    var thread = treads.FirstOrDefault(x => x.Name == nameLibraby);
    if (thread != null)
    {
        thread.Interrupt();
        treads.RemoveAll(x => x.Name == nameLibraby);
    }

    app.Logger.LogInformation("Stop {0}", nameLibraby);
    return Results.Ok();
});

app.Run();

