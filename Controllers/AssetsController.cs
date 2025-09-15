using System.Diagnostics;
using Indicators.Models;
using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Application.DTOs;
using Application.Interface;
using Application.ViewModels;
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
            return View();
        }

        [HttpPost]        
        public IActionResult Index(string indicatorType)
        {
           if(string.IsNullOrWhiteSpace(indicatorType)) 
            {

                List<CalculatedAssetsDto> prediction = IndicatorsPrediccion.Prediccion(indicatorType, _assetsService.GetAllAssets());


                List<CalculatedAssetsViewModel> model = prediction.Select(a => new CalculatedAssetsViewModel
                {
                    AssetsName = a.AssetsName,
                    IndicatorName = a.IndicatorName,
                    Price = a.Price,
                    Trend = a.Trend
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
        public IActionResult AddAssets(List<AddAssetsModelView> addmodel)
        {
            try{

                if (ModelState.IsValid)
                {
                    List<PriceHistoryDto> priceHistory = new List<PriceHistoryDto>();

                    foreach (var M in addmodel)
                    {
                        PriceHistoryDto price = new PriceHistoryDto()
                        {
                            Date = M.Date,
                            Price = M.Price
                        };

                        priceHistory.Add(price);
                    }
                    AssetsDto assets = new AssetsDto()
                    {
                        Name = addmodel[0].Name,
                        PriceHistory = priceHistory
                    };

                    _assetsService.AddAsset(assets);

                    return RedirectToRoute(new { controller = "Assets", action = "Index", message = "Asset created successful", messageType = "alert-success" });
                }
                else
                {
                    return View();
                }
                }catch(Exception ex) 
                {
                 return RedirectToRoute(new { controller = "Assets", action = "Index", message = "Error creating assets", messageType = "alert-success" });
                }
            
        }
    }
}
