using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace EM.GIS.GdalExtensions
{
    public static class DatasetExtensions
    {
        /// <summary>
        /// 尝试以读写打开数据，若失败则以只读打开
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Dataset Open(string fileName)
        {
            Dataset dataset = null;
            if (File.Exists(fileName))
            {
                try
                {
                    dataset = Gdal.Open(fileName, Access.GA_Update);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"以读写打开“{fileName}”失败，将尝试以只读打开，错误信息：{e}");
                    try
                    {
                        dataset = Gdal.Open(fileName, Access.GA_ReadOnly);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"以只读打开“{fileName}”失败，错误信息：{ex}");
                    }
                }
            }
            return dataset;
        }
        /// <summary>
        /// 创建金字塔
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="resampling"></param>
        /// <param name="overviewlist"></param>
        /// <returns></returns>
        public static int BuildOverviews(this Dataset dataset, Resampling resampling = Resampling.NEAREST, int[] overviewlist = null)
        {
            int value = -1;
            if (dataset == null || dataset.RasterCount <= 0)
            {
                return value;
            }

            if (overviewlist == null)
            {
                List<int> intList = new List<int>();
                int width = dataset.RasterXSize;
                int height = dataset.RasterYSize;
                int k = 1;
                while (width > 256 && height > 256)
                {
                    k *= 2;
                    intList.Add(k);
                    width /= 2;
                    height /= 2;
                }

                overviewlist = intList.ToArray();
            }

            value = dataset.BuildOverviews(resampling.ToString(), overviewlist);
            return value;
        }
        /// <summary>
        /// 获取tiff的可选参数
        /// </summary>
        /// <returns>可选参数</returns>
        public static string[] GetTiffOptions()
        {
            string[] options = { "TILED=YES", "COMPRESS=DEFLATE", "BIGTIFF=YES" };
            return options;
        }
        /// <summary>
        /// 获取MBTiles的可选参数
        /// </summary>
        /// <returns>可选参数</returns>
        public static string[] GetMBTilesOptions(string name=null,string description=null,LayerType type= LayerType.overlay,string version="1.1",int blockSize=256,TileFormat tileFormat= TileFormat.PNG,int quality=75,int zLevel=6,YesNo dither= YesNo.NO,ZoomLevelStrategy zoomLevelStrategy= ZoomLevelStrategy.AUTO,Resampling resampling= Resampling.BILINEAR, YesNo writeBounds = YesNo.YES)
        {
            List<string> options = new List<string>();
            if (!string.IsNullOrEmpty(name)) options.Add($"NAME={name}");
            if (!string.IsNullOrEmpty(description)) options.Add($"DESCRIPTION={description}");
            options.Add($"TYPE={type}");
            if (!string.IsNullOrEmpty(version)) options.Add($"VERSION={version}");
            options.Add($"BLOCKSIZE={blockSize}");
            options.Add($"TILE_FORMAT={tileFormat}");
            options.Add($"QUALITY={quality}");
            options.Add($"ZLEVEL={zLevel}");
            options.Add($"DITHER={dither}");
            options.Add($"ZOOM_LEVEL_STRATEGY={zoomLevelStrategy}");
            options.Add($"RESAMPLING={resampling}");
            options.Add($"WRITE_BOUNDS={writeBounds}");
            return options.ToArray();
        }
        /// <summary>
        /// 创建金字塔
        /// </summary>
        /// <param name="dataset">数据集</param>
        /// <param name="resampling">重采样方式</param>
        /// <param name="overviewlist">金字塔集合</param>
        /// <returns></returns>
        public static int CreateOverview(this Dataset dataset, string resampling = "NEAREST", int[]? overviewlist = null)
        {
            int value = -1;
            if (dataset == null || dataset.RasterCount <= 0)
            {
                return value;
            }

            if (overviewlist == null)
            {
                List<int> intList = new List<int>();
                int width = dataset.RasterXSize;
                int height = dataset.RasterYSize;
                int k = 1;
                while (width > 256 && height > 256)
                {
                    k *= 2;
                    intList.Add(k);
                    width /= 2;
                    height /= 2;
                }

                overviewlist = intList.ToArray();
            }

            value = dataset.BuildOverviews(resampling, overviewlist);
            return value;
        }
    }
}
