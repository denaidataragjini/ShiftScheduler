using Microsoft.ML;
using Microsoft.ML.Data;

namespace ShiftScheduler.ML
{
    public class ModelTrainer
    {
        public void Train(string datasetPath)
        {
            var mlContext = new MLContext(seed: 42);

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

                .Append(mlContext.Transforms.Conversion.ConvertType(
                    nameof(SchedulePredictionInput.IsWeekend),
                    nameof(SchedulePredictionInput.IsWeekend),
                    DataKind.Single))

                .Append(mlContext.Transforms.Conversion.ConvertType(
                    nameof(SchedulePredictionInput.NightShift),
                    nameof(SchedulePredictionInput.NightShift),
                    DataKind.Single))

                .Append(mlContext.Transforms.Conversion.ConvertType(
                    "Label", "Label", DataKind.Boolean))

                .Append(mlContext.Transforms.Concatenate(
                    "Features",
                    "EmployeeEncoded",
                    "PositionEncoded",
                    nameof(SchedulePredictionInput.ShiftType),
                    nameof(SchedulePredictionInput.DayOfWeek),
                    nameof(SchedulePredictionInput.Month),
                    nameof(SchedulePredictionInput.IsWeekend),
                    nameof(SchedulePredictionInput.ContractHours),
                    nameof(SchedulePredictionInput.NightShift)))

                .Append(mlContext.BinaryClassification.Trainers.LightGbm(
                    labelColumnName: "Label",
                    featureColumnName: "Features",
                    numberOfIterations: 200,
                    learningRate: 0.05,
                    numberOfLeaves: 31));

            Console.WriteLine("Training model...");
            var model = pipeline.Fit(split.TrainSet);

            Console.WriteLine("Evaluating model...");
            var predictions = model.Transform(split.TestSet);
            var metrics = mlContext.BinaryClassification.Evaluate(
                predictions,
                labelColumnName: "Label");

            Console.WriteLine($"Accuracy:  {metrics.Accuracy:P2}");
            Console.WriteLine($"AUC:       {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1 Score:  {metrics.F1Score:P2}");

            Directory.CreateDirectory("ML");
            mlContext.Model.Save(model, data.Schema, "ML/model.zip");
            Console.WriteLine("Model saved to ML/model.zip");
        }
    }
}