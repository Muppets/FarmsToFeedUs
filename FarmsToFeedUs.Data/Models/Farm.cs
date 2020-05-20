using Amazon.DynamoDBv2.DataModel;
using GeoCoordinatePortable;

namespace FarmsToFeedUs.Data
{
    public class Farm
    {
        [DynamoDBHashKey]
        public string Name { get; set; } = "";

        [DynamoDBVersion]
        public int? VersionNumber { get; set; }

        public string Town { get; set; } = "";

        public string County { get; set; } = "";

        public string Postcode { get; set; } = "";

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [DynamoDBIgnore]
        public GeoCoordinate? Location => Latitude != null && Longitude != null ? new GeoCoordinate(Latitude.Value, Longitude.Value) : null;
    }
}
