using CETWebProject.Data;
using CETWebProject.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CETWebProject.Controllers
{
    public class EchelonController : Controller
    {
        private IEchelonRepository _echelonRepository;
        public EchelonController( IEchelonRepository echelonRepository)
        {
            _echelonRepository = echelonRepository;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var model = await _echelonRepository.GetAll();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ICollection<Echelons> model)
        {
            await _echelonRepository.UpdateEchelons(model);
            return View(model);
        }
    }
}
