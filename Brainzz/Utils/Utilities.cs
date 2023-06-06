using System.Data.SqlClient;
using System.Text.Json;

namespace Brainzz.Utils
{
    public class Utilities
    {
        public static string IsoCountryCodeToFlagEmoji(string country)
        {
            return string.Concat(country.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
        }

        public static async Task<SqlConnection> ConnectToDatabase()
        {
            string connectionString = "Data Source=(local);Initial Catalog=Brainz;Integrated Security=True";

            using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            return connection;
        }

        public static async Task<JsonDocument> GetJson(string requestUrl)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var json = await client.GetStringAsync(requestUrl);

            return JsonDocument.Parse(json);
        }
    }
}
