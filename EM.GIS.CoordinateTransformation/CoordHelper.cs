using System;
using System.Collections.Generic;

namespace EM.GIS.CoordinateTransformation
{
    /// <summary>
    /// 坐标工具类
    /// </summary>
    public class CoordHelper
    {
        const double pi = 3.1415926535897932384626;
        const double x_pi = 3.14159265358979324 * 3000.0 / 180.0;
        const double a = 6378245.0;
        const double ee = 0.00669342162296594323;

        private static double TransformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        private static double TransformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
            return ret;
        }

        private static double[] Transform(double lat, double lon)
        {
            if (OutOfChina(lat, lon)) { return new double[] { lat, lon }; }
            double dLat = TransformLat(lon - 105.0, lat - 35.0);
            double dLon = TransformLon(lon - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = lat + dLat;
            double mgLon = lon + dLon;
            return new double[] { mgLat, mgLon };
        }
        /// <summary>
        /// 是否超出中国范围
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <returns>超出中国范围为true，反之false</returns>
        public static bool OutOfChina(double lat, double lon)
        {
            if (lon < 72.004 || lon > 137.8347) return true;
            if (lat < 0.8293 || lat > 55.8271) return true;
            return false;
        }
        /// <summary>
        /// WGS84坐标转GCJ-02坐标（火星坐标系）
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <returns>坐标（纬度、经度）</returns>
        public static (double Lat, double Lon) Wgs84ToGcj02(double lat, double lon)
        {
            if (OutOfChina(lat, lon)) { return (lat, lon); }
            double dLat = TransformLat(lon - 105.0, lat - 35.0);
            double dLon = TransformLon(lon - 105.0, lat - 35.0);
            double radLat = lat / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / (a * (1 - ee) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
            double mgLat = lat + dLat;
            double mgLon = lon + dLon;
            return (mgLat, mgLon);
        }

        /// <summary>
        /// GCJ-02坐标（火星坐标系）转WGS84坐标
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <returns>坐标（纬度、经度）</returns>
        public static (double Lat, double Lon) Gcj02ToWgs84(double lat, double lon)
        {
            double[] gps = Transform(lat, lon);
            double lontitude = lon * 2 - gps[1];
            double latitude = lat * 2 - gps[0];
            return (latitude, lontitude);
        }

        /// <summary>
        /// GCJ-02坐标（火星坐标系）转BD-09坐标（百度09坐标系）
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <returns>坐标（纬度、经度）</returns>
        public static (double Lat, double Lon) Gcj02ToBd09(double lat, double lon)
        {
            double x = lon, y = lat;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
            double tempLon = z * Math.Cos(theta) + 0.0065;
            double tempLat = z * Math.Sin(theta) + 0.006;
            return (tempLat, tempLon);
        }

        /// <summary>
        /// 将BD-09坐标（百度09坐标系）转换成GCJ-02坐标（火星坐标系）
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <returns>坐标（纬度、经度）</returns>
        public static (double Lat, double Lon) Bd09ToGcj02(double lat, double lon)
        {
            double x = lon - 0.0065, y = lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
            double tempLon = z * Math.Cos(theta);
            double tempLat = z * Math.Sin(theta);
            return (tempLat, tempLon);
        }

        /// <summary>
        /// WGS84坐标转BD-09坐标（百度09坐标系）
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <returns>坐标（纬度、经度）</returns>
        public static (double Lat, double Lon) Wgs84ToBd09(double lat, double lon)
        {
            var gcj02 = Wgs84ToGcj02(lat, lon);
            var bd09 = Gcj02ToBd09(gcj02.Lat, gcj02.Lon);
            return bd09;
        }
        /// <summary>
        /// BD-09坐标（百度09坐标系）转WGS84坐标
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <returns>坐标（纬度、经度）</returns>
        public static (double Lat, double Lon) Bd09ToWgs84(double lat, double lon)
        {
            var gcj02 = Bd09ToGcj02(lat, lon);
            var Wgs84 = Gcj02ToWgs84(gcj02.Lat, gcj02.Lon);
            return Wgs84;
        }
    }

    /// <summary>
    /// 最新坐标转换算法
    /// </summary>
    public class CoordConvert
    {
        const double a = 6378245.0;
        const double f = 1 / 298.3;
        const double b = a * (1 - f);
        const double ee = 1 - (b * b) / (a * a);
        const double PI = Math.PI;

        bool OutOfChina(double lng, double lat)
        {
            if (!(72.004 <= lng && lng <= 137.8347) && (0.8293 <= lat && lat <= 55.8271))
                return true;
            return false;
        }

        double geohey_transformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret = ret + (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0;
            ret = ret + (20.0 * Math.Sin(y * PI) + 40.0 * Math.Sin(y / 3.0 * PI)) * 2.0 / 3.0;
            ret = ret + (160.0 * Math.Sin(y / 12.0 * PI) + 320.0 * Math.Sin(y * PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        double geohey_transformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret = ret + (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0;
            ret = ret + (20.0 * Math.Sin(x * PI) + 40.0 * Math.Sin(x / 3.0 * PI)) * 2.0 / 3.0;
            ret = ret + (150.0 * Math.Sin(x / 12.0 * PI) + 300.0 * Math.Sin(x * PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        public (double lng, double lat) wgs2gcj(double wgsLon, double wgsLat)
        {
            if (OutOfChina(wgsLon, wgsLat))
                return (wgsLon, wgsLat);
            double dLat = geohey_transformLat(wgsLon - 105.0, wgsLat - 35.0);
            double dLon = geohey_transformLon(wgsLon - 105.0, wgsLat - 35.0);
            double radLat = wgsLat / 180.0 * PI;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * PI);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * PI);
            double gcjLat = wgsLat + dLat;
            double gcjLon = wgsLon + dLon;
            return (gcjLon, gcjLat);
        }

        (double lng, double lat) gcj2wgs(double gcjLon, double gcjLat)
        {
            var g0 = new List<double>() { gcjLon, gcjLat };
            var w0 = g0;
            var g1 = wgs2gcj(w0[0], w0[1]);
            var w1 = IterCal1(w0, new List<double>() { g1.lng, g1.lat }, g0);
            var delta = IterCal2(w0, new List<double>() { w1[0], w1[1] });
            while (Math.Abs(delta[0]) >= 1e-6 || Math.Abs(delta[1]) >= 1e-6)
            {
                w0 = w1;
                g1 = wgs2gcj(w0[0], w0[1]);
                w1 = IterCal1(w0, new List<double>() { g1.lng, g1.lat }, g0);
                delta = IterCal2(w0, new List<double>() { w1[0], w1[1] });
            }
            return (w1[0], w1[1]);
        }

        List<double> IterCal1(List<double> w0, List<double> g1, List<double> g0)
        {
            var w1 = new List<double>();
            for (int i = 0; i < w0.Count(); i++)
            {
                double result = w0[i] - (g1[i] - g0[i]);
                w1.Add(result);
            }
            return w1;
        }

        List<double> IterCal2(List<double> w0, List<double> w1)
        {
            var delta = new List<double>();
            for (int i = 0; i < w1.Count(); i++)
            {
                double result = w1[i] - w0[i];
                delta.Add(result);
            }
            return delta;
        }

        (double lng, double lat) gcj2bd(double gcjLon, double gcjLat)
        {
            var z = Math.Sqrt(gcjLon * gcjLon + gcjLat * gcjLat) + 0.00002 * Math.Sin(gcjLat * PI * 3000.0 / 180.0);
            var theta = Math.Atan2(gcjLat, gcjLon) + 0.000003 * Math.Cos(gcjLon * PI * 3000.0 / 180.0);
            var bdLon = z * Math.Cos(theta) + 0.0065;
            var bdLat = z * Math.Sin(theta) + 0.006;
            return (bdLon, bdLat);
        }

        (double lng, double lat) bd2gcj(double bdLon, double bdLat)
        {
            var x = bdLon - 0.0065;
            var y = bdLat - 0.006;
            var z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * PI * 3000.0 / 180.0);
            var theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * PI * 3000.0 / 180.0);
            var gcjLon = z * Math.Cos(theta);
            var gcjLat = z * Math.Sin(theta);
            return (gcjLon, gcjLat);
        }

        (double lng, double lat) wgs2bd(double wgsLon, double wgsLat)
        {
            var gcj = wgs2gcj(wgsLon, wgsLat);
            return gcj2bd(gcj.lng, gcj.lat);
        }

        (double lng, double lat) bd2wgs(double bdLon, double bdLat)
        {
            var gcj = bd2gcj(bdLon, bdLat);
            return gcj2wgs(gcj.lng, gcj.lat);
        }

    }
}
