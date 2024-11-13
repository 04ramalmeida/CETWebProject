using System;
using System.Threading.Tasks;
using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CETWebProject.Controllers
{
    public class WaterMetersController : Controller
    {
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IUserTempRepository _userTempRepository;
        private readonly IUserHelper _userHelper;
        
        public WaterMetersController(IWaterMeterRepository waterMeterRepository,
            IUserHelper userHelper,
            IUserTempRepository userTempRepository)
        {
            _waterMeterRepository = waterMeterRepository;
            _userTempRepository = userTempRepository;
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
            var meter = await _waterMeterRepository.GetWaterMeterWithUser((int)id);
            var user = await _userHelper.GetUserByEmailAsync(meter.Username);
            if (meter == null)
            {
                return NotFound();
            }
            await _waterMeterRepository.DeleteWaterMeterAsync(id.Value);
            return RedirectToAction("Index", new {id = user.Id});
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
                return RedirectToAction("Readings", new { id = model.WaterMeterId });
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
            try
            {
                await _waterMeterRepository.DeleteReadingAsync(reading);
                return RedirectToAction($"Details", new { id = meterId });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = "Error in deleting reading";
                    ViewBag.ErrorMessage = "We were unable to delete this reading, this usually happens if an invoice associated with it has already been issued.";
                }
                return View("Error");
            }
        }

        public async Task<IActionResult> RequestMeterByUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            if (user != null)
            {
                await _waterMeterRepository.RequestMeter(new RequestMeterViewModel
                {
                    Username = user.UserName,
                    Date = DateTime.Now,
                });
            }
            //TODO make this work somehow ViewBag.Message("Your request has been sent.");
            return RedirectToAction($"UserIndex");
        }

        public async Task<IActionResult> MeterRequests()
        {
            var requests = await _waterMeterRepository.GetAllMeterTemp();
            return View(requests);
        }

        public async Task<IActionResult> Deliver (int id)
        {
            var request = await _waterMeterRepository.GetRequestById(id);
            //await _waterMeterRepository.AddWaterMeterAsync(request.User.Email);
            //var userId = request.User.Id;
            await _waterMeterRepository.RemoveRequest(request);
            await _waterMeterRepository.ApproveMeterRequest(request.Username);
            return RedirectToAction("MeterRequests");
        }

        public async Task<IActionResult> Deny (int id)
        {
            var request = await _waterMeterRepository.GetRequestById(id);
            var user = await _userHelper.GetUserByEmailAsync(request.Username);
            if (user.Id == null)
            {
                return RedirectToAction("MeterRequests", new { id = user.Id });
            }
            var userId = user.Id;
            await _waterMeterRepository.RemoveRequest(request);
            return RedirectToAction("MeterRequests", new {id = userId});
        }
    }
}