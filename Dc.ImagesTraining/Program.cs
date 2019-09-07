using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using static Microsoft.ML.DataOperationsCatalog;
using System.Linq;
using Microsoft.ML.Data;
using Dc.Core;

namespace Dc.ImagesTraining
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start input data");
            var traningDataPath = Utility.GetAbsolutePath(typeof(Program).Assembly.Location, "../../../TrainingData");
            MLContext mLContext = new MLContext(seed: 1);
            IEnumerable<ImageData> images = LoadImagesFromDirectory(directory: traningDataPath, true).ToList();
            IDataView fullImagesDataset = mLContext.Data.LoadFromEnumerable(images);
            IDataView shuffledFullImagesDataset = mLContext.Data.ShuffleRows(fullImagesDataset);

            Console.WriteLine("Split test data and transformed validation data");
            TrainTestData trainTestData = mLContext.Data.TrainTestSplit(shuffledFullImagesDataset, testFraction: 0.2);
            IDataView trainDataView = trainTestData.TrainSet;
            IDataView testDataView = trainTestData.TestSet;
            IDataView transformedValidationDataView = mLContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelAsKey", inputColumnName: "Label", keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                .Fit(testDataView)
                .Transform(testDataView);

            Console.WriteLine("Create leaning pipeline");
            var pipeline = mLContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelAsKey", inputColumnName: "Label", keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                .Append(mLContext.Model.ImageClassification("ImagePath", "LabelAsKey",
                                arch: ImageClassificationEstimator.Architecture.ResnetV2101,
                                epoch: 100,
                                batchSize: 2,
                                metricsCallback: (metrics) => Console.WriteLine(metrics),
                                validationSet: transformedValidationDataView));

            Console.WriteLine("*** Training data ***");
            ITransformer trainedModel = pipeline.Fit(trainDataView);

            Console.WriteLine("*** Evaluate data ***");
            EvaluateModel(mLContext, testDataView, trainedModel);

            mLContext.Model.Save(trainedModel, trainDataView.Schema, "DiseasesImagesModel.zip");
            Console.WriteLine($"Model saved to: DiseasesImagesModel.zip");

            Console.WriteLine("Press any key to finish");
            Console.ReadKey();

            Console.WriteLine("Hello World!");
        }

        /// <summary>
        /// Get list image data in a folder
        /// </summary>
        /// <param name="directory">Path to folder</param>
        /// <param name="useFolderNameasLabel">Set folder name as label</param>
        /// <returns>IEnumerable ImageData</returns>
        public static IEnumerable<ImageData> LoadImagesFromDirectory(string directory, bool useFolderNameasLabel = true)
        {
            var files = Directory.GetFiles(directory, "*",
                searchOption: SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png"))
                    continue;

                var label = Path.GetFileName(file);
                if (useFolderNameasLabel)
                    label = Directory.GetParent(file).Name;
                else
                {
                    for (int index = 0; index < label.Length; index++)
                    {
                        if (!char.IsLetter(label[index]))
                        {
                            label = label.Substring(0, index);
                            break;
                        }
                    }
                }

                yield return new ImageData()
                {
                    ImagePath = file,
                    Label = label
                };

            }
        }

        /// <summary>
        /// Evaluate accuracy of model
        /// </summary>
        /// <param name="mlContext">Machine leaning context</param>
        /// <param name="testDataset">Test DataView</param>
        /// <param name="trainedModel">Trained DataView</param>
        private static void EvaluateModel(MLContext mlContext, IDataView testDataset, ITransformer trainedModel)
        {
            Console.WriteLine("Making predictions in bulk for evaluating model's quality...");

            IDataView predictionsDataView = trainedModel.Transform(testDataset);

            var metrics = mlContext.MulticlassClassification.Evaluate(predictionsDataView, labelColumnName: "LabelAsKey", predictedLabelColumnName: "PredictedLabel");
            ConsoleHelper.PrintMultiClassClassificationMetrics("TensorFlow DNN Transfer Learning", metrics);

            Console.WriteLine("*** Showing all the predictions ***");
            VBuffer<ReadOnlyMemory<char>> keys = default;
            predictionsDataView.Schema["LabelAsKey"].GetKeyValues(ref keys);
            var originalLabels = keys.DenseValues().ToArray();

            List<ImagePredictionEx> predictions = mlContext.Data.CreateEnumerable<ImagePredictionEx>(predictionsDataView, false, true).ToList();
            predictions.ForEach(pred => ConsoleWriteImagePrediction(pred.ImagePath, pred.Label, (originalLabels[pred.PredictedLabel]).ToString(), pred.Score.Max()));

        }

        /// <summary>
        /// Write prediction result
        /// </summary>
        /// <param name="ImagePath">Full image path</param>
        /// <param name="Label">Curent label</param>
        /// <param name="PredictedLabel">Prediction label</param>
        /// <param name="Probability">% probability</param>
        public static void ConsoleWriteImagePrediction(string ImagePath, string Label, string PredictedLabel, float Probability)
        {
            var defaultForeground = Console.ForegroundColor;
            var labelColor = ConsoleColor.Magenta;
            var probColor = ConsoleColor.Blue;

            Console.Write("Image File: ");
            Console.ForegroundColor = labelColor;
            Console.Write($"{Path.GetFileName(ImagePath)}");
            Console.ForegroundColor = defaultForeground;
            Console.Write(" original labeled as ");
            Console.ForegroundColor = labelColor;
            Console.Write(Label);
            Console.ForegroundColor = defaultForeground;
            Console.Write(" predicted as ");
            Console.ForegroundColor = labelColor;
            Console.Write(PredictedLabel);
            Console.ForegroundColor = defaultForeground;
            Console.Write(" with score ");
            Console.ForegroundColor = probColor;
            Console.Write(Probability);
            Console.ForegroundColor = defaultForeground;
            Console.WriteLine("");
        }

    }
}
