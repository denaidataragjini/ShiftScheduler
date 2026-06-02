using Microsoft.ML;
using Microsoft.ML.Data;

namespace ShiftScheduler.ML
{
    public class ModelTrainer
    {
        public void Train(string datasetPath)
        {
            var mlContext = new MLContext(seed: 42);

            var data = mlContext.Data.LoadFromTextFile<EmployeePredictionInput>(
                path: datasetPath,
                hasHeader: true,
                separatorChar: ',');

            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            var pipeline =
                mlContext.Transforms.Categorical.OneHotEncoding(
                    new[]
                    {
                        new InputOutputColumnPair("EmployeeEncoded", nameof(EmployeePredictionInput.EmployeeId)),
                        new InputOutputColumnPair("PositionEncoded", nameof(EmployeePredictionInput.PositionId))
                    })

                .Append(mlContext.Transforms.Conversion.ConvertType(
                    nameof(EmployeePredictionInput.IsWeekend),
                    nameof(EmployeePredictionInput.IsWeekend),
                    DataKind.Single))

                .Append(mlContext.Transforms.Conversion.ConvertType(
                    nameof(EmployeePredictionInput.NightShift),
                    nameof(EmployeePredictionInput.NightShift),
                    DataKind.Single))

                .Append(mlContext.Transforms.Conversion.ConvertType(
                    "Label", "Label", DataKind.Boolean))

                .Append(mlContext.Transforms.Concatenate(
                    "Features",
                    "EmployeeEncoded",
                    "PositionEncoded",
                    nameof(EmployeePredictionInput.ShiftType),
                    nameof(EmployeePredictionInput.DayOfWeek),
                    nameof(EmployeePredictionInput.Month),
                    nameof(EmployeePredictionInput.IsWeekend),
                    nameof(EmployeePredictionInput.ContractHours),
                    nameof(EmployeePredictionInput.NightShift)))

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