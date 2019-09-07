using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Dc.Web.Models
{
    public class HomeImageModel
    {
        /// <summary>
        /// Diseases Name
        /// </summary>
        public string Name { get; set; }

        public IFormFile FileUpload { get; set; }

        /// <summary>
        /// % Perdiction
        /// </summary>
        public float PerdictionPercentage { get; set; }
    }
}
