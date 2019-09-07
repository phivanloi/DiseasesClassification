using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dc.Web.Models;
using Dc.Core;
using Dc.Core.Engine;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using System;

namespace Dc.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MLModelEngine<DiseasesSymptomTraining, DiseasesSymptomPerdiction> mLSymptomEngine;
        private readonly MLModelEngine<ImageData, ImagePrediction> mLImageEngine;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(
            IHostingEnvironment _hostingEnvironment,
            MLModelEngine<ImageData, ImagePrediction> _mLImageEngine,
            MLModelEngine<DiseasesSymptomTraining, DiseasesSymptomPerdiction> _mLSymptomEngine)
        {
            mLSymptomEngine = _mLSymptomEngine;
            mLImageEngine = _mLImageEngine;
            hostingEnvironment = _hostingEnvironment;
        }

        public IActionResult Index()
        {
            var model = new HomeIndexModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(HomeIndexModel homeIndexModel)
        {
            if (ModelState.IsValid)
            {
                var diseasesTraining = new DiseasesSymptomTraining() { Syptom = homeIndexModel.Syptom };
                var perdictionResult = mLSymptomEngine.Predict(diseasesTraining);
                homeIndexModel.Name = perdictionResult.Name;
                homeIndexModel.PerdictionPercentage = Utility.CalculatePercentage(perdictionResult.Score.Max());
            }
            return View(homeIndexModel);
        }

        public IActionResult Image()
        {
            var model = new HomeImageModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Image(HomeImageModel homeIndexModel)
        {
            if (homeIndexModel.FileUpload?.Length > 0 && homeIndexModel.FileUpload.Length < 1024000 && ((Path.GetExtension(homeIndexModel.FileUpload.FileName).ToLower() == ".jpg") || (Path.GetExtension(homeIndexModel.FileUpload.FileName).ToLower() == ".png")))
            {
                var filePath = $"{hostingEnvironment.WebRootPath}\\uploads\\{homeIndexModel.FileUpload.FileName}";
                var saveFilePath = Utility.IdentityFileName(filePath);
                using (var stream = new FileStream(saveFilePath, FileMode.Create))
                {
                    await homeIndexModel.FileUpload.CopyToAsync(stream);
                }
                ImageData imageToPredict = new ImageData
                {
                    ImagePath = saveFilePath
                };
                var perdictionResult = mLImageEngine.Predict(imageToPredict);
                var index = perdictionResult.PredictedLabel;
                homeIndexModel.Name = GetDiseasesNameForImageClassification(mLImageEngine.GetOriginalLabel(index));
                homeIndexModel.PerdictionPercentage = Utility.CalculatePercentage(perdictionResult.Score[index]);
            }
            else
            {
                ModelState.AddModelError("FileUpload", "Dữ Liệu không hợp lệ.");
            }
            return View(homeIndexModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetDiseasesNameForImageClassification(string label)
        {
            switch (label)
            {
                case "HacLao":
                    return "Hắc lào";
                case "VayNen":
                    return "Vảy nến";
                case "ViemDa":
                    return "Viêm da";
                default:
                    return "";
            }
        }
    }
}
