using System.ComponentModel.DataAnnotations;

namespace DisDet.Web.Models
{
    public class HomeIndexModel
    {
        /// <summary>
        /// Diseases Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Diseases Syptom
        /// </summary>
        [Required]
        [StringLength(150, ErrorMessage = "Triệu chứng bệnh không được nhỏn hơn 50 và lớn hơn 150 ký tự.")]
        public string Syptom { get; set; }

        /// <summary>
        /// % Perdiction
        /// </summary>
        public float PerdictionPercentage { get; set; }
    }
}
