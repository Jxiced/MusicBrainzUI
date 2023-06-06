using Brainzz.Models;
using Brainzz.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Brainzz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Query))
                return View("Index", await SearchArtists(model.Query));

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Artists()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Username) || !string.IsNullOrWhiteSpace(model.Password))
            {
                await LoginUser(model);
            }

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Username) || !string.IsNullOrWhiteSpace(model.Password) || !string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                if (model.Password == model.ConfirmPassword)
                    await CreateUser(model);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("ReleaseGroup/{id}")]
        public async Task<ActionResult> ReleaseGroup(string id)
        {
            var releaseGroup = await GetReleaseGroup(id);

            return View(releaseGroup);
        }

        public async Task<IEnumerable<ArtistModel>> SearchArtists(string name)
        {
            var json = await Utilities.GetJson($"https://musicbrainz.org/ws/2/artist?query={name}");

            var releases = json.RootElement.GetProperty("artists").EnumerateArray();

            var artistList = releases.Select(release => new ArtistModel
            {
                Id = release.GetProperty("id").GetString() ?? release.GetProperty("id").ToString(),
                Name = release.GetProperty("name").GetString() ?? release.GetProperty("name").ToString()
            }).AsEnumerable();

            return artistList ?? Enumerable.Empty<ArtistModel>();
        }

        public async Task<ReleaseModel> GetReleaseGroup(string releaseGroupId)
        {
            var json = await Utilities.GetJson($"https://musicbrainz.org/ws/2/release-group/{releaseGroupId}");

            var release = new ReleaseModel
            {
                Id = json.RootElement.GetProperty("id").GetString() ?? json.RootElement.GetProperty("id").ToString(),
                Title = json.RootElement.GetProperty("title").GetString() ?? json.RootElement.GetProperty("title").ToString(),
                Date = json.RootElement.GetProperty("first-release-date").GetString() ?? json.RootElement.GetProperty("first-release-date").ToString()
            };

            return release;
        }

        public async Task CreateUser(RegisterModel model)
        {
            try
            {
                string connectionString = "Data Source=(local);Initial Catalog=Brainz;Integrated Security=True";

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                SqlCommand cmd = new($"INSERT INTO Users (Username, Password) VALUES(@username, @password)", connection);
                cmd.Parameters.Add("@username", System.Data.SqlDbType.NVarChar);
                cmd.Parameters.Add("@password", System.Data.SqlDbType.NVarChar);

                cmd.Parameters["@username"].Value = model.Username;
                cmd.Parameters["@password"].Value = model.Password;

                var res = await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task LoginUser(LoginModel model)
        {
            try
            {
                string connectionString = "Data Source=(local);Initial Catalog=Brainz;Integrated Security=True";

                using SqlConnection connection = new(connectionString);
                await connection.OpenAsync();

                SqlCommand cmd = new($"SELECT Username, Password FROM Users WHERE Username=@username AND Password=@password", connection);
                cmd.Parameters.Add("@username", System.Data.SqlDbType.NVarChar);
                cmd.Parameters.Add("@password", System.Data.SqlDbType.NVarChar);

                cmd.Parameters["@username"].Value = model.Username;
                cmd.Parameters["@password"].Value = model.Password;


                using SqlDataReader reader = cmd.ExecuteReader();
                string username = "";
                string password = "";

                while (reader.Read())
                {
                    username = reader.GetString(0);
                    password = reader.GetString(1);
                }

                if (model.Username == username && model.Password == password)
                {
                    TempData["Login"] = true;
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
