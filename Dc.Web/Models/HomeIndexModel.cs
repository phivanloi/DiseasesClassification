using System.ComponentModel.DataAnnotations;

namespace Dc.Web.Models
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
        [Required(ErrorMessage = "Yêu cầu nhập các chiệu chứng chuẩn đoán")]
        [StringLength(512, MinimumLength = 64, ErrorMessage = "Triệu chứng bệnh không được nhỏn hơn 64 và lớn hơn 512 ký tự.")]
        public string Syptom { get; set; }

        /// <summary>
        /// % Perdiction
        /// </summary>
        public float PerdictionPercentage { get; set; }
    }
}
