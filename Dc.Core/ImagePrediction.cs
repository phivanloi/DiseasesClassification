using Microsoft.ML.Data;
using System;

namespace Dc.Core
{
    public class ImagePrediction
    {
        [ColumnName("Score")]
        public float[] Score;

        [ColumnName("PredictedLabel")]
        public UInt32 PredictedLabel;
    }
}
