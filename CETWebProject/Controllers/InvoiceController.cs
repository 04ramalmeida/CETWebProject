﻿using CETWebProject.Data;
using CETWebProject.Data.Entities;
using CETWebProject.Helpers;
using CETWebProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SQLitePCL;
using System;
using System.Threading.Tasks;

namespace CETWebProject.Controllers
{
    public class InvoiceController : Controller
    {
        private IMailHelper _mailHelper;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IUserHelper _userHelper;

        public InvoiceController(IInvoiceRepository invoiceRepository,
            IWaterMeterRepository waterMeterRepository,
            IMailHelper mailHelper,
            IUserHelper userHelper)
        {
            _invoiceRepository = invoiceRepository;
            _waterMeterRepository = waterMeterRepository;
            _mailHelper = mailHelper;
            _userHelper = userHelper;
        }


        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> InvoiceIndex()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var model = await _invoiceRepository.GetInvoicesByUserAsync(user.Id);
            return View(model);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> EmployeeInvoiceIndex(string userId)
        {
            var model = await _invoiceRepository.GetInvoicesByUserAsync(userId);
            var user = await _userHelper.GetUserById(userId);
            ViewBag.UserId = userId;    
            return View("InvoiceIndex",model);
        }

        /*[Authorize(Roles = "Employee")]
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
                return RedirectToAction("InvoiceIndex", new { userId = model.UserId });
            }
            return View(model);
        }*/

        public async Task<IActionResult> EmployeeIssueInvoice (int id)
        {
            var reading = await _waterMeterRepository.GetReadingByIdAsync(id);
            await _invoiceRepository.AddInvoiceAsync(reading.WaterMeter.User.Id, reading.UsageAmount);
            string tokenLink = Url.Action("InvoiceIndex", "Invoice","", protocol: HttpContext.Request.Scheme);

            Response response = _mailHelper.SendEmail(reading.WaterMeter.User.Email,
                "New Invoice",
                "A new invoice has been issued. To see the details, please click on the link below." +
                "</br>" +
                $"<a href=\"{tokenLink}\">Check my invoices</a>");
            return RedirectToAction("EmployeeInvoiceIndex", new { userId = reading.WaterMeter.User.Id });
        }

        public async Task<IActionResult> Delete (int id)
        {
            var invoice = _invoiceRepository.GetInvoiceWithUser(id);
            await _invoiceRepository.DeleteAsync(invoice);
            return RedirectToAction("InvoiceIndex", new { userId = invoice.User.Id });
        }
    }
}
