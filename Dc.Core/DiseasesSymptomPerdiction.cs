using Microsoft.ML.Data;

namespace Dc.Core
{
    public class DiseasesSymptomPerdiction
    {
        /// <summary>
        /// Diseases Name
        /// </summary>
        [ColumnName("PredictedLabel")]
        public string Name { get; set; }

        public float[] Score;
    }
}
