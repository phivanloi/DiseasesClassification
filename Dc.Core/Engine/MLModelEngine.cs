using Microsoft.ML;
using Microsoft.Extensions.ObjectPool;
using System.IO;
using System;
using Microsoft.ML.Data;
using System.Linq;

namespace Dc.Core.Engine
{
    public class MLModelEngine<TData, TPrediction>
                    where TData : class
                    where TPrediction : class, new()
    {
        private readonly MLContext _mlContext;
        private readonly ObjectPool<PredictionEngine<TData, TPrediction>> _predictionEnginePool;
        private readonly int _maxObjectsRetained;

        /// <summary>
        /// Exposing the ML model allowing additional ITransformer operations such as Bulk predictions', etc.
        /// </summary>
        public ITransformer MLModel { get; }

        /// <summary>
        /// Constructor with modelFilePathName to load from
        /// </summary>
        public MLModelEngine(string modelFilePathName, int maxObjectsRetained = -1)
        {
            //Create the MLContext object to use under the scope of this class 
            _mlContext = new MLContext();

            //Load the ProductSalesForecast model from the .ZIP file
            using (var fileStream = File.OpenRead(modelFilePathName))
            {
                MLModel = _mlContext.Model.Load(fileStream, out var modelInputSchema);
            }

            _maxObjectsRetained = maxObjectsRetained;

            //Create PredictionEngine Object Pool
            _predictionEnginePool = CreatePredictionEngineObjectPool();
        }

        // Create the Object Pool based on the PooledPredictionEnginePolicy.
        // This method is only used once, from the cosntructor.
        private ObjectPool<PredictionEngine<TData, TPrediction>> CreatePredictionEngineObjectPool()
        {
            var predEnginePolicy = new PooledPredictionEnginePolicy<TData, TPrediction>(_mlContext, MLModel);

            DefaultObjectPool<PredictionEngine<TData, TPrediction>> pool;

            if (_maxObjectsRetained != -1)
            {
                pool = new DefaultObjectPool<PredictionEngine<TData, TPrediction>>(predEnginePolicy, _maxObjectsRetained);
            }
            else
            {
                //default maximumRetained is Environment.ProcessorCount * 2, if not explicitly provided
                pool = new DefaultObjectPool<PredictionEngine<TData, TPrediction>>(predEnginePolicy);
            }

            return pool;
        }

        /// <summary>
        /// The Predict() method performs a single prediction based on sample data provided (dataSample) and returning the Prediction.
        /// This implementation uses an object pool internally so it is optimized for scalable and multi-threaded apps.
        /// </summary>
        /// <param name="dataSample"></param>
        /// <returns></returns>
        public TPrediction Predict(TData dataSample)
        {
            PredictionEngine<TData, TPrediction> predictionEngine = _predictionEnginePool.Get();

            try
            {
                TPrediction prediction = predictionEngine.Predict(dataSample);
                return prediction;
            }
            finally
            {
                _predictionEnginePool.Return(predictionEngine);
            }
        }

        /// <summary>
        /// Get original label by perdiction index
        /// </summary>
        /// <param name="index">Prediction index</param>
        /// <returns>string</returns>
        public string GetOriginalLabel(uint index)
        {
            PredictionEngine<TData, TPrediction> predictionEngine = _predictionEnginePool.Get();
            VBuffer<ReadOnlyMemory<char>> keys = default;
            predictionEngine.OutputSchema["LabelAsKey"].GetKeyValues(ref keys);
            var originalLabels = keys.DenseValues().ToArray();
            return originalLabels[index].ToString();
        }

    }

}
