using NGeoHash;
using System;
using System.Collections.Generic;

namespace FarmsToFeedUs.Data
{
    public class GeohashService : IGeohashService
    {
        public long Encode(double latitude, double longitude)
        {
            return GeoHash.EncodeInt(latitude, longitude);
        }

        public List<GeohashRange> GetRanges(double latitude, double longitude, int rangeBitDepth = 16) // 156.5km
        {
            long geohash = GeoHash.EncodeInt(latitude, longitude, bitDepth: rangeBitDepth);
            long[] neighbours = GeoHash.NeighborsInt(geohash, bitDepth: rangeBitDepth);
            var ranges = new List<GeohashRange>(neighbours.Length);
            var rangeLength = geohash.ToString().Length;

            foreach (var neighbour in neighbours)
            {
                var latLong = GeoHash.DecodeInt(neighbour, bitDepth: rangeBitDepth);
                long geoHash = GeoHash.EncodeInt(latLong.Coordinates.Lat, latLong.Coordinates.Lon);
                var padAmount = geoHash.ToString().Length;

                // Now produce the min / max value of this based on the range length
                // So 21345 becomes 213450000000 to 21345999999
                var leftPart = geoHash.ToString().Substring(0, rangeLength);
                var min = Convert.ToInt64(leftPart.PadRight(padAmount, '0'));
                var max = Convert.ToInt64(leftPart.PadRight(padAmount, '9'));

                ranges.Add(new GeohashRange { Min = min, Max = max });
            }

            return ranges;
        }

        //HashBits, Radius Meters
        //52, 597cm
        //50, 1.1m
        //48, 2.3m
        //46, 4.7m
        //44, 9.5m
        //42, 19.1m
        //40, 38.2m
        //38, 76.4m
        //36, 152.8m
        //34, 305.7m
        //32, 611.5m
        //30, 1.2km
        //28, 2.4km
        //26, 4.8km
        //24, 9.7km
        //22, 19.5km
        //20, 39.1km
        //18, 78.2km
        //16, 156.5km
        //14, 313km
        //12, 626km
        //10, 1252km
        //8, 2,504km
        //6, 5,009km
        //4, 10,018km
    }
}
