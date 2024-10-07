using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CETWebProject.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserHelper _userHelper;

        public InvoiceController(IInvoiceRepository invoiceRepository,
            IUserHelper userHelper)
        {
            _invoiceRepository = invoiceRepository;
            _userHelper = userHelper;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EmployeeIndex(string userId)
        {
            var model = await _invoiceRepository.GetInvoicesByUserAsync(userId);
            var user = await _userHelper.GetUserById(userId);
            ViewBag.UserId = userId;    
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EmployeeAddInvoice(string userId) 
        {
            var model = new AddInvoiceViewModel
            {
                UserId = userId,
                Date = DateTime.Now,
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EmployeeAddInvoice(AddInvoiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _invoiceRepository.AddInvoiceAsync(model.UserId, model.Value);
                return RedirectToAction("EmployeeIndex", new { userId = model.UserId });
            }
            return View(model);
        }
    }
}
