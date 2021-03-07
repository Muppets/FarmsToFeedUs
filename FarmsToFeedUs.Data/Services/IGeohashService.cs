using System.Collections.Generic;

namespace FarmsToFeedUs.Data
{
    public interface IGeohashService
    {
        long Encode(double latitude, double longitude);
        List<GeohashRange> GetRanges(double latitude, double longitude, int rangeBitDepth = 22);
    }

    public class GeohashRange
    {
        public long Min { get; set; }
        public long Max { get; set; }
    }
}
