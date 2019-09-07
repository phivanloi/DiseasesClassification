using Dc.Core;
using Microsoft.ML;
using System;

namespace Dc.SymptomTraining
{
    class Program
    {
        static void Main()
        {
            MLContext mLContext = new MLContext(1);

            var traningDataPath = Utility.GetAbsolutePath(typeof(Program).Assembly.Location, "../../../SysptomTraining.txt");
            var trainingDataView = mLContext.Data.LoadFromTextFile<DiseasesSymptomTraining>(traningDataPath, hasHeader: false, separatorChar: '|');

            var dataProcessPipeline = mLContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Label", inputColumnName: nameof(DiseasesSymptomTraining.Name))
                            .Append(mLContext.Transforms.Text.FeaturizeText(outputColumnName: "Syptom", inputColumnName: nameof(DiseasesSymptomTraining.Syptom)))
                            .Append(mLContext.Transforms.Concatenate(outputColumnName: "Features", "Syptom"))
                            .AppendCacheCheckpoint(mLContext);

            IEstimator<ITransformer> trainer = mLContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer).Append(mLContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mLContext.MulticlassClassification.CrossValidate(data: trainingDataView, estimator: trainingPipeline, numberOfFolds: 6, labelColumnName: "Label");
            ConsoleHelper.PrintMulticlassClassificationFoldsAverageMetrics(trainer.ToString(), crossValidationResults);

            Console.WriteLine("=============== Training the model ===============");
            var trainedModel = trainingPipeline.Fit(trainingDataView);

            Console.WriteLine("=============== Saving the model to a file ===============");
            mLContext.Model.Save(trainedModel, trainingDataView.Schema, "../../../DiseasesSysptomModel.zip");

            ConsoleHelper.ConsoleWriteHeader("Training process finalized");

            ConsoleHelper.ConsolePressAnyKey();
        }
    }
}
