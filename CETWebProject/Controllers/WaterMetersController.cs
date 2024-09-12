using System.Threading.Tasks;
using CETWebProject.Data;
using CETWebProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace CETWebProject.Controllers
{
    public class WaterMetersController : Controller
    {
        private readonly IWaterMeterRepository _waterMeterRepository;
        
        public WaterMetersController(IWaterMeterRepository waterMeterRepository)
        {
            _waterMeterRepository = waterMeterRepository;
        }
        // GET
        public async Task<IActionResult> Index()
        {
            var model = await _waterMeterRepository.GetWaterMetersByUserAsync("email@email.com");
            return View(model);
        }

        public IActionResult Create() 
        {
            return View();
        }

        
        public async Task<IActionResult> AddMeter()
        {
            await _waterMeterRepository.AddWaterMeterAsync("email@email.com");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) 
            {
               return NotFound();
            }
            await _waterMeterRepository.DeleteWaterMeterAsync(id.Value);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)//Unfinished
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = await _waterMeterRepository.GetWaterMeterWithCities(id.Value);
            return View(model);
        }

        public async Task<IActionResult> AddReading(int? id)//To Be Done
        {
            if (id == null)
            {
                return NotFound();
            }
            var meter = await _waterMeterRepository.GetWaterMeterWithCities(id.Value);
            if (meter == null)
            {
                return NotFound();
            }
            var model = new AddReadingViewModel { WaterMeterId = meter.Id };
            return View(model);
        }
    }
}