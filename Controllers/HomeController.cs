using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarFinderApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://vpic.nhtsa.dot.gov/api/vehicles/getallmakes?format=json");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                var makes = doc.RootElement.GetProperty("Results").Clone();
                ViewBag.Makes = makes;
            }
            else
            {
                ViewBag.Makes = null;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleTypes(int makeId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://vpic.nhtsa.dot.gov/api/vehicles/GetVehicleTypesForMakeId/{makeId}?format=json");
            
            if (!response.IsSuccessStatusCode) return BadRequest();

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetModels(int makeId, int year)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}?format=json");
            
            if (!response.IsSuccessStatusCode) return BadRequest();

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}