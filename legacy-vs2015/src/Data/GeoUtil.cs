using System;

namespace Kirana.WebApi.Data
{
    // Small geo helpers for delivery distance/ETA estimation.
    public static class GeoUtil
    {
        // Great-circle distance in kilometres between two lat/lng points.
        public static double DistanceKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371.0; // earth radius km
            var dLat = ToRad(lat2 - lat1);
            var dLng = ToRad(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                    + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                    * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        // Rough door-to-door ETA: 10 min prep + ~4 min per km (≈15 km/h city speed).
        public static int EtaMinutes(double distanceKm)
        {
            var eta = 10 + (int)Math.Ceiling(distanceKm * 4.0);
            if (eta < 15) eta = 15;
            if (eta > 180) eta = 180;
            return eta;
        }

        private static double ToRad(double deg) { return deg * Math.PI / 180.0; }
    }
}
