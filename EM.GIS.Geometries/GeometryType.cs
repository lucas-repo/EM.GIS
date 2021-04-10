namespace EM.GIS.Geometries
{
    /// <summary>
    /// 要素类型
    /// </summary>
    public enum GeometryType
    {
        Unknown,
        Point,
        Point25D,
        PointM,
        PointZM,
        LineString,
        LineString25D,
        LineStringM,
        LineStringZM,
        LinearRing,
        Polygon,
        Polygon25D,
        PolygonM,
        PolygonZM,

        MultiPoint,
        MultiPoint25D,
        MultiPointM,
        MultiPointZM,
        MultiLineString,
        MultiLineString25D,
        MultiLineStringM,
        MultiLineStringZM,
        MultiPolygon,
        MultiPolygon25D,
        MultiPolygonM,
        MultiPolygonZM,
    }
}