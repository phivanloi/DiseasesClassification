using Microsoft.ML.Data;

namespace Dc.Core
{
    public class DiseasesSymptomTraining
    {
        /// <summary>
        /// Diseases Name
        /// </summary>
        [LoadColumn(1)]
        public string Name { get; set; }

        /// <summary>
        /// Diseases Syptom
        /// </summary>
        [LoadColumn(0)]
        public string Syptom { get; set; }
    }
}
