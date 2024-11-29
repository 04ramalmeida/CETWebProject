using System;
using System.Threading.Tasks;
using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Index(string id)
        {
            var user = await _userHelper.GetUserById(id);
            if (user == null) 
            {
                return new NotFoundViewResult("UserNotFound");
            }
            var model = await _waterMeterRepository.GetWaterMetersByUserAsync(user.Email);
            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UserIndex()
        {
            var model = await _waterMeterRepository.GetWaterMetersByUserAsync(this.User.Identity.Name);
            return View(model);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            

            if (id == null) 
            {
                return new NotFoundViewResult("MeterNotFound");
            }
            var meter = await _waterMeterRepository.GetWaterMeterWithUser((int)id);
            var user = await _userHelper.GetUserByEmailAsync(meter.Username);
            if (meter == null)
            {
                return new NotFoundViewResult("MeterNotFound");
            }
            await _waterMeterRepository.DeleteWaterMeterAsync(id.Value);
            return RedirectToAction("Index", new {id = user.Id});
        }

        [Authorize]
        public async Task<IActionResult> Readings(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("MeterNotFound");
            }

            var model = await _waterMeterRepository.GetWaterMeterWithReadings(id.Value);
            if (model == null)
            {
                return new NotFoundViewResult("MeterNotFound");
            }
            return View(model);
        }

        // TODO: Fix this
        [Authorize(Roles = "Employee,Customer")]
        public async Task<IActionResult> AddReading(int? id)
        {
            

            if (id == null)
            {
                return new NotFoundViewResult("MeterNotFound");
            }
            var meter = await _waterMeterRepository.GetWaterMeterWithReadings(id.Value);
            if (meter == null)
            {
                return new NotFoundViewResult("MeterNotFound");
            }
            if (User.IsInRole("Customer"))
            {
                if (meter.Username != this.User.Identity.Name)
                {
                    return RedirectToAction("NotAuthorized", "Account", null);
                }
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
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> EditReading(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ReadingNotFound");
            }
            
            var reading = await _waterMeterRepository.GetReadingByIdAsync(id.Value);

            if (reading == null)
            {
                return new NotFoundViewResult("ReadingNotFound");
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

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteReading(int? id)
        {
            
            
            if (id == null) 
            {
                return new NotFoundViewResult("MeterNotfound");
            }

            var reading = await _waterMeterRepository.GetReadingByIdAsync(id.Value);
            if (reading == null)
            {
                return new NotFoundViewResult("ReadingNotFound");
            }
            var meterId = _waterMeterRepository.GetMeterIdByReading(reading);
            
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

        [Authorize(Roles = "Customer")]
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
            else
            {
                return new NotFoundViewResult("UserNotFound");
            }
            //TODO make this work somehow ViewBag.Message("Your request has been sent.");
            return RedirectToAction($"UserIndex");
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> MeterRequests()
        {
            var requests = await _waterMeterRepository.GetAllMeterTemp();
            return View(requests);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Deliver (int id)
        {
            var request = await _waterMeterRepository.GetRequestById(id);
            if (request == null)
            {
                return new NotFoundViewResult("MeterRequestNotFound");
            }
            //await _waterMeterRepository.AddWaterMeterAsync(request.User.Email);
            //var userId = request.User.Id;
            await _waterMeterRepository.RemoveRequest(request);
            await _waterMeterRepository.ApproveMeterRequest(request.Username);
            return RedirectToAction("MeterRequests");
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Deny (int id)
        {
            var request = await _waterMeterRepository.GetRequestById(id);
            if (request == null)
            {
                return new NotFoundViewResult("MeterRequestNotFound");
            }
            var user = await _userHelper.GetUserByEmailAsync(request.Username);
            if (user == null)
            {
                return new NotFoundViewResult("UserNotFound");
            }
            var userId = user.Id;
            await _waterMeterRepository.RemoveRequest(request);
            return RedirectToAction("MeterRequests", new {id = userId});
        }
    }
}