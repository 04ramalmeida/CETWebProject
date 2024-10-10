using System;
using System.Threading.Tasks;
using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CETWebProject.Controllers
{
    public class WaterMetersController : Controller
    {
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IUserHelper _userHelper;
        
        public WaterMetersController(IWaterMeterRepository waterMeterRepository,
            IUserHelper userHelper)
        {
            _waterMeterRepository = waterMeterRepository;
            _userHelper = userHelper;
        }
        // GET
        public async Task<IActionResult> Index(string id)
        {
            var user = await _userHelper.GetUserById(id);
            var model = await _waterMeterRepository.GetWaterMetersByUserAsync(user.Email);
            return View(model);
        }

        public async Task<IActionResult> UserIndex()
        {
            var model = await _waterMeterRepository.GetWaterMetersByUserAsync(this.User.Identity.Name);
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

        public async Task<IActionResult> Readings(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _waterMeterRepository.GetWaterMeterWithReadings(id.Value);
            return View(model);
        }

        public async Task<IActionResult> AddReading(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var meter = await _waterMeterRepository.GetWaterMeterWithReadings(id.Value);
            if (meter == null)
            {
                return NotFound();
            }
            var model = new AddReadingViewModel { WaterMeterId = meter.Id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReading(AddReadingViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _waterMeterRepository.AddReadingAsync(model.WaterMeterId, model);
                return RedirectToAction("Details", new { id = model.WaterMeterId });
            }
            return View(model);
        }

        public async Task<IActionResult> EditReading(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var reading = await _waterMeterRepository.GetReadingByIdAsync(id.Value);

            if (reading == null)
            {
                return NotFound();
            }
            
            return View(reading);
        }

        [HttpPost]
        public async Task<IActionResult> EditReading(Reading reading)
        {
            var meter = _waterMeterRepository.GetMeterIdByReading(reading);
            if (this.ModelState.IsValid)
            {
                await _waterMeterRepository.UpdateReading(reading);
                if (meter != 0)
                {
                    return RedirectToAction($"Readings", new { id = meter });
                }
            }

            return View(reading);
        }
        
        public async Task<IActionResult> DeleteReading(int? id)
        {
            
            
            if (id == null) 
            {
                return NotFound();
            }

            var reading = await _waterMeterRepository.GetReadingByIdAsync(id.Value);
            var meterId = _waterMeterRepository.GetMeterIdByReading(reading);
            if (reading == null)
            {
                return NotFound();
            }
            await _waterMeterRepository.DeleteReadingAsync(reading);
            return RedirectToAction($"Details", new { id = meterId});
        }

        public async Task<IActionResult> RequestMeterByUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user != null)
            {
                await _waterMeterRepository.RequestMeter(new RequestMeterViewModel
                {
                    User = user,
                    Date = DateTime.Now,
                });
            }
            //TODO make this work somehow ViewBag.Message("Your request has been sent.");
            return RedirectToAction($"UserIndex");
        }
    }
}