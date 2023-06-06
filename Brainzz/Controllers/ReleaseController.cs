using Brainzz.Models;
using Brainzz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Brainzz.Controllers
{
    public class ReleaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("Release/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var release = await GetRelease(id);

            return View(release);
        }

        public async Task<ReleaseModel> GetRelease(string releaseId)
        {
            var json = await Utilities.GetJson($"https://musicbrainz.org/ws/2/release/{releaseId}");

            var release = new ReleaseModel
            {
                Id = json.RootElement.GetProperty("id").GetString() ?? json.RootElement.GetProperty("id").ToString(),
                Title = json.RootElement.GetProperty("title").GetString() ?? json.RootElement.GetProperty("title").ToString(),
                Date = json.RootElement.GetProperty("date").GetString() ?? json.RootElement.GetProperty("date").ToString(),
                Country = Utilities.IsoCountryCodeToFlagEmoji(json.RootElement.GetProperty("country").GetString() ?? json.RootElement.GetProperty("country").ToString())
            };

            return release;
        }
    }
}
