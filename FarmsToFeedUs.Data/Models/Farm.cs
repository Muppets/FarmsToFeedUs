using Amazon.DynamoDBv2.DataModel;

namespace FarmsToFeedUs.Data
{
    public class Farm
    {
        [DynamoDBHashKey]
        public string Name { get; set; } = "";

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }

        public string? Town { get; set; }

        public string? County { get; set; }

        public string? Postcode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string? Website { get; set; }

        public string? Instagram { get; set; }

        public string? Facebook { get; set; }

        //[DynamoDBIgnore]
        //public GeoCoordinate? Location => Latitude != null && Longitude != null ? new GeoCoordinate(Latitude.Value, Longitude.Value) : null;

        public override string ToString()
        {
            return Name;
        }
    }
}
