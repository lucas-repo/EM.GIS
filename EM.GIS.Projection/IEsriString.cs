namespace EM.GIS.Projection
{
    /// <summary>
    /// 与Esri字符串互转
    /// </summary>
    public interface IEsriString
    {
        /// <summary>
        /// 转成EsriString
        /// </summary>
        /// <returns></returns>
        string ToEsriString();
        /// <summary>
        /// 读取EsriString
        /// </summary>
        /// <param name="esriString"></param>
        void ParseEsriString(string esriString);
    }
}