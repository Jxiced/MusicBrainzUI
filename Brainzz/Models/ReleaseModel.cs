namespace Brainzz.Models
{
    public class ReleaseModel
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Date { get; set; }
        public string? Country { get; set; }
        public IEnumerable<ReleaseModel> Releases { get; set; }
    }
}
