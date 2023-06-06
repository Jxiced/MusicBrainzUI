namespace Brainzz.Models
{
    public class ArtistModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public IEnumerable<ReleaseModel> Releases { get; set; }
    }
}
