using Brainzz.Models;
using Brainzz.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Brainzz.Controllers
{
    public class ArtistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("Artist/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            var artist = await GetArtist(id);

            return View(artist);
        }

        public async Task<ArtistModel> GetArtist(string artistId)
        {
            var json = await Utilities.GetJson($"https://musicbrainz.org/ws/2/artist/{artistId}");

            var artist = new ArtistModel
            {
                Id = json.RootElement.GetProperty("id").GetString() ?? json.RootElement.GetProperty("id").ToString(),
                Name = json.RootElement.GetProperty("name").GetString() ?? json.RootElement.GetProperty("name").ToString(),
                Releases = await GetArtistReleases(artistId)
            };

            return artist;
        }

        public async Task<IEnumerable<ReleaseModel>> GetArtistReleases(string artistId)
        {
            var json = await Utilities.GetJson($"https://musicbrainz.org/ws/2/artist/{artistId}?inc=releases");

            var releases = json.RootElement.GetProperty("releases").EnumerateArray();

            var releaseList = releases.Select(release => new ReleaseModel
            {
                Id = release.GetProperty("id").GetString() ?? release.GetProperty("id").ToString(),
                Title = release.GetProperty("title").GetString() ?? release.GetProperty("title").ToString(),
                Date = release.GetProperty("date").GetString() ?? release.GetProperty("date").ToString(),
                Country = Utilities.IsoCountryCodeToFlagEmoji(release.GetProperty("country").GetString() ?? release.GetProperty("country").ToString())
            }).AsEnumerable();

            return releaseList ?? Enumerable.Empty<ReleaseModel>();
        }

    }
}
