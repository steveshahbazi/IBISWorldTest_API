using IbisWorld_Web.Models;
using IbisWorld_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace IbisWorld_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGlossaryService _glossaryService;

        public HomeController(IGlossaryService glossaryService)
        {
            _glossaryService = glossaryService;
        }

        public async Task<IActionResult> Index()
        {
            List<TermDTO> list = new();
            var response = await _glossaryService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<TermDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }
    }
}