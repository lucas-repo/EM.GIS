namespace EM.GIS.Geometries
{
    /// <summary>
    /// 要素类型
    /// </summary>
    public enum GeometryType
    {
        Unknown,
        Point,
        LineString,
        LinearRing,
        Polygon,
        MultiPoint,
        MultiLineString,
        MultiPolygon,
        GeometryCollection
    }
}