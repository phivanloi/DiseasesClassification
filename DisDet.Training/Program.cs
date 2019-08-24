using DisDet.Core;
using Microsoft.ML;
using System;

namespace DisDet.Training
{
    class Program
    {
        static void Main()
        {
            MLContext mLContext = new MLContext(1);

            // STEP 1: Common data loading configuration
            var trainingDataView = mLContext.Data.LoadFromTextFile<DiseasesTraining>("training.txt", hasHeader: false, separatorChar: '|');

            // STEP 2: Common data process configuration with pipeline data transformations
            var dataProcessPipeline = mLContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Label", inputColumnName: nameof(DiseasesTraining.Name))
                            .Append(mLContext.Transforms.Text.FeaturizeText(outputColumnName: "Syptom", inputColumnName: nameof(DiseasesTraining.Syptom)))
                            .Append(mLContext.Transforms.Concatenate(outputColumnName: "Features", "Syptom"))
                            .AppendCacheCheckpoint(mLContext);

            // STEP 3: Create the selected training algorithm/trainer
            IEstimator<ITransformer> trainer = mLContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features");

            //Set the trainer/algorithm and map label to value (original readable state)
            var trainingPipeline = dataProcessPipeline.Append(trainer).Append(mLContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // STEP 4: Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
            // in order to evaluate and get the model's accuracy metrics

            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mLContext.MulticlassClassification.CrossValidate(data: trainingDataView, estimator: trainingPipeline, numberOfFolds: 6, labelColumnName: "Label");
            ConsoleHelper.PrintMulticlassClassificationFoldsAverageMetrics(trainer.ToString(), crossValidationResults);

            // STEP 5: Train the model fitting to the DataSet
            Console.WriteLine("=============== Training the model ===============");
            var trainedModel = trainingPipeline.Fit(trainingDataView);

            // STEP 6: Save/persist the trained model to a .ZIP file
            Console.WriteLine("=============== Saving the model to a file ===============");
            mLContext.Model.Save(trainedModel, trainingDataView.Schema, "DiseasesModel.zip");

            ConsoleHelper.ConsoleWriteHeader("Training process finalized");

            ConsoleHelper.ConsolePressAnyKey();
        }
    }
}
