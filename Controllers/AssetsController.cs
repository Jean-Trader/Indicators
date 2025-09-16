using Application.DTOs;
using Application.Interface;
using Application.Services;
using Application.ViewModels;
using Indicators.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
namespace Indicators.Controllers
{
    public class AssetsController : Controller
    {
       
       private readonly AssetsService _assetsService = new AssetsService();

        public AssetsController()
        {
           
        }

        public IActionResult Index()
        {
            ViewBag.AssetNames = _assetsService.GetAllAssets().Select(a => a.Name).ToList();
            return View(new List<CalculatedAssetsViewModel>()); 
        }

        [HttpPost]        
        public IActionResult Index(string indicatorType)
        {
           if(!string.IsNullOrWhiteSpace(indicatorType)) 
            {

                List<CalculatedAssetsDto> prediction = IndicatorsPrediccion.Prediccion(indicatorType, _assetsService.GetAllAssets());

                ViewBag.AssetNames = _assetsService.GetAllAssets().Select(a => a.Name).ToList();

                List<CalculatedAssetsViewModel> model = prediction.Select(a => new CalculatedAssetsViewModel
                {
                    AssetsName = a.AssetsName,
                    IndicatorName = a.IndicatorName,
                    Price = a.Price,
                    Trend = a.Trend,
                    Details = a.Details
                }).ToList();

                return View(model);
            }
            else { 
                return View();
            }

        }

        public IActionResult AddAssets()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddAssets(AddAssetsModelView addmodel)
        {
            try
            {

                Console.WriteLine("POST recibido"); // Debug
                Console.WriteLine($"ModelState isValid: {ModelState.IsValid}"); // Debug
                Console.WriteLine($"Nombre recibido: {addmodel?.Name}"); // Debug
                Console.WriteLine($"Precios recibidos: {addmodel?.PriceHistory?.Count}"); // Debug

                if (ModelState.IsValid)
                {



                    List<PriceHistoryDto> priceHistory = addmodel.PriceHistory.Select(ph => new PriceHistoryDto
                    {
                        Date = ph.Date,
                        Price = ph.Price
                    }).ToList();

                    AssetsDto assets = new AssetsDto()
                    {
                        Name = addmodel.Name,
                        PriceHistory = priceHistory
                    };

                    _assetsService.AddAsset(assets);

                    return RedirectToRoute(new { controller = "Assets", action = "Index", message = "Asset created successful", messageType = "alert-success" });
                }
                else
                {
                    return View(addmodel);
                }
            }
            catch (Exception ex)
            {
                return RedirectToRoute(new { controller = "Assets", action = "Index", message = $"Error created asset", messageType = "alert-danger" });
            }
            
            
        }
    }
}
