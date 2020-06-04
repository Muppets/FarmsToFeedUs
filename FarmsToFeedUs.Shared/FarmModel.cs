namespace FarmsToFeedUs.Shared
{
    public class FarmModel
    {
        public string Name { get; set; } = "";

        public string? Town { get; set; }

        public string? County { get; set; }

        public string? Postcode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? InstagramUrl { get; set; }

        public string? FacebookUrl { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
