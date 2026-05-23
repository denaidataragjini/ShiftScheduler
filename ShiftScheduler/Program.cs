using ShiftScheduler.Models;
using ShiftScheduler.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//try
//{
//    Console.WriteLine("STARTING...");

//    var loader = new CsvLoaderService();

//    Console.WriteLine("Loading schedules...");
//    var schedules = loader.Load<Schedule>("Data/schedules.csv");
//    Console.WriteLine($"Schedules loaded: {schedules.Count}");

//    Console.WriteLine("Loading employees...");
//    var employees = loader.Load<Employee>("Data/employees.csv");
//    Console.WriteLine($"Employees loaded: {employees.Count}");

//    Console.WriteLine("Loading requests...");
//    var requests = loader.Load<Request>("Data/requests.csv");
//    Console.WriteLine($"Requests loaded: {requests.Count}");

//    Console.WriteLine("Generating dataset...");

//    var generator = new TrainingDataGeneratorService();

//    var rows = generator.Generate(
//        schedules,
//        employees,
//        requests);

//    Console.WriteLine($"Generated rows: {rows.Count}");

//    if (!Directory.Exists("Output"))
//    {
//        Directory.CreateDirectory("Output");
//    }

//    Console.WriteLine("Exporting CSV...");

//    generator.ExportCsv(rows, "Output/training_dataset.csv");

//    Console.WriteLine("DONE");
//}
//catch (Exception ex)
//{
//    Console.WriteLine("ERROR:");
//    Console.WriteLine(ex.Message);
//    Console.WriteLine(ex.StackTrace);
//}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();