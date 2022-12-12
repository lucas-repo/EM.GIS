using EM.IOC;
using System.ComponentModel;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据工厂
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IDataFactory))]
    public class DataFactory : IDataFactory
    {
        //private static readonly object _lockObj = new object();
        //private static IDataFactory _default;
        ///// <summary>
        ///// 默认数据工厂
        ///// </summary>
        //public static IDataFactory Default
        //{
        //    get
        //    {
        //        lock (_lockObj)
        //        {
        //            if (_default == null)
        //            {
        //                _default = new DataFactory();
        //            }
        //            return _default;
        //        }
        //    }
        //}
        private ProgressDelegate _progress;

        [Category("Handlers")]
        [Description("Gets or sets the object that implements the IProgressHandler interface for recieving status messages.")]
        public ProgressDelegate Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                if (DriverFactory != null)
                {
                    DriverFactory.Progress = value;
                }
            }
        }

        public IDriverFactory DriverFactory { get; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IGeometryFactory GeometryFactory { get; set; }

        //public DataFactory()
        //{
        //    DriverFactory = new DriverFactory()
        //    {
        //        Progress = Progress
        //    };
        //}
    }
}
