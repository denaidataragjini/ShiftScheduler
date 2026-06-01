using ShiftScheduler.ML;
using ShiftScheduler.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IPredictionService, PredictionService>();
builder.Services.AddSingleton<IShiftPredictionService, ShiftPredictionService>();

var app = builder.Build();
var loader = new CsvLoaderService();

//var schedules =
//    loader.Load<Schedule>("Data/schedules.csv");
//var shiftTypes =
//    loader.Load<ShiftTypes>("Data/shift_types.csv");
//var generator =
//    new TrainingDataGeneratorService();

//var rows =
//    generator.GenerateShiftDataset(shiftTypes, schedules);

//generator.ExportShiftCsv(
//    rows,
//    "Output/shift_training_dataset.csv");

//Console.WriteLine(
//    $"Generated shift rows: {rows.Count}");


Console.WriteLine("Training model...");
var trainer = new ShiftModelTrainer();
trainer.Train("Output/Shift_training_dataset.csv");

Console.WriteLine("DONE");

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
//    var rows = generator.Generate(schedules, employees, requests);
//    Console.WriteLine($"Generated rows: {rows.Count}");

//    Directory.CreateDirectory("Output");

//    Console.WriteLine("Exporting CSV...");
//    generator.ExportCsv(rows, "Output/training_dataset.csv");
//    Console.WriteLine("CSV exported.");

//    Console.WriteLine("Training model...");
//    var trainer = new ModelTrainer();
//    trainer.Train("Output/training_dataset.csv");

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