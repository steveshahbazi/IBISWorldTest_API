using IbisWorld_Web.Models;
using IbisWorld_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IbisWorld_Web.Controllers
{
    public class GlossaryController : Controller
    {
        private readonly IGlossaryService _glossaryService;

        public GlossaryController(IGlossaryService glossaryService)
        {
            _glossaryService = glossaryService;
        }

        public async Task<IActionResult> IndexGlossary()
        {
            List<TermDTO> list = new();
            var response = await _glossaryService.GetAllAsync<APIResponse>();
            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<TermDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        public async Task<IActionResult> CreateTerm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTerm(TermDTO model)
        {
            if (ModelState.IsValid)
            {
                var response = await _glossaryService.CreateAsync<APIResponse>(model);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("IndexGlossary");
                }
            }

            return View(model);
        }
    }
}
