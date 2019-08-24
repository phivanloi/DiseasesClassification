using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DisDet.Web.Models;
using DisDet.Core;
using DisDet.Core.Engine;
using System.Linq;

namespace DisDet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly MLModelEngine<DiseasesTraining, DiseasesPerdiction> mLModelEngine;

        public HomeController(MLModelEngine<DiseasesTraining, DiseasesPerdiction> _mLModelEngine)
        {
            mLModelEngine = _mLModelEngine;
        }

        public IActionResult Index()
        {
            var model = new HomeIndexModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(HomeIndexModel homeIndexModel)
        {
            var diseasesTraining = new DiseasesTraining() { Syptom = homeIndexModel.Syptom };
            var perdictionResult = mLModelEngine.Predict(diseasesTraining);
            homeIndexModel.Name = perdictionResult.Name;
            homeIndexModel.PerdictionPercentage = Utility.CalculatePercentage(perdictionResult.Score.Max());
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
    }
}
