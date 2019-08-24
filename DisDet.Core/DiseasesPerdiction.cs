using Microsoft.ML.Data;

namespace DisDet.Core
{
    public class DiseasesPerdiction
    {
        /// <summary>
        /// Diseases Name
        /// </summary>
        [ColumnName("PredictedLabel")]
        public string Name { get; set; }

        public float[] Score;
    }
}
