using CETWebProject.Data;
using CETWebProject.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CETWebProject.Controllers
{
    public class AlertController : Controller
    {
        private readonly IAlertRepository _alertRepository;

        public AlertController(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = await _alertRepository.GetByUserAsync(this.User.Identity.Name);
            return View(model);
        }
    }
}
