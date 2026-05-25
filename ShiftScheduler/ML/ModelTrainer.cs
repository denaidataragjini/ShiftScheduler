using Microsoft.ML;

namespace ShiftScheduler.ML
{
    public class ModelTrainer
    {
        public void Train(string datasetPath)
        {
            var mlContext = new MLContext();

            var data = mlContext.Data.LoadFromTextFile<SchedulePredictionInput>(
                path: datasetPath,
                hasHeader: true,
                separatorChar: ',');

            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            var pipeline =
                mlContext.Transforms.Categorical.OneHotEncoding(
                    new[]
                    {
                    new InputOutputColumnPair("EmployeeEncoded", nameof(SchedulePredictionInput.EmployeeId)),
                    new InputOutputColumnPair("PositionEncoded", nameof(SchedulePredictionInput.PositionId))
                    })

                .Append(mlContext.Transforms.Concatenate(
                    "Features",
                    "EmployeeEncoded",
                    "PositionEncoded",
                    nameof(SchedulePredictionInput.ShiftType),
                    nameof(SchedulePredictionInput.DayOfWeek),
                    nameof(SchedulePredictionInput.IsWeekend),
                    nameof(SchedulePredictionInput.ContractHours),
                    nameof(SchedulePredictionInput.NightShift)))

                .Append(mlContext.BinaryClassification.Trainers.LightGbm(
                    labelColumnName: "Label",
                    featureColumnName: "Features"));

            Console.WriteLine("Training model...");

            var model = pipeline.Fit(split.TrainSet);

            Console.WriteLine("Evaluating model...");

            var predictions = model.Transform(split.TestSet);

            var metrics = mlContext.BinaryClassification.Evaluate(
                predictions,
                labelColumnName: "Label");

            Console.WriteLine($"Accuracy: {metrics.Accuracy}");
            Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve}");

            mlContext.Model.Save(
                model,
                data.Schema,
                "ML/model.zip");

            Console.WriteLine("Model saved.");
        }
    }
}