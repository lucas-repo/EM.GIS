using EM.IOC;

namespace EM.GIS.Data.MBTiles
{
    /// <summary>
    /// MBTiles驱动
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IDriver))]
    public class MBTilesDriver : Driver, IRasterDriver
    {
        public IRasterSet? Create(string filename, int width, int height, int bandCount, RasterType rasterType, string[]? options = null)
        {
            throw new NotImplementedException();
        }

        public override IDataSet? Open(string path)
        {
            throw new NotImplementedException();
        }

        IRasterSet? IRasterDriver.Open(string path)
        {
            throw new NotImplementedException();
        }
    }
}