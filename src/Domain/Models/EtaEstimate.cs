namespace SmartDrone.Domain.Models;

public sealed record EtaEstimate(double DistanceKm, double EstimatedMinutes);

public static class GeoDistance
{
    private const double EarthRadiusKm = 6371.0;

    public static double HaversineKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        var p1 = ToRadians((double)lat1);
        var p2 = ToRadians((double)lat2);
        var dLat = p2 - p1;
        var dLon = ToRadians((double)(lon2 - lon1));

        var a = Math.Pow(Math.Sin(dLat / 2), 2)
              + Math.Cos(p1) * Math.Cos(p2) * Math.Pow(Math.Sin(dLon / 2), 2);

        return EarthRadiusKm * 2 * Math.Asin(Math.Sqrt(Math.Clamp(a, 0, 1)));
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
