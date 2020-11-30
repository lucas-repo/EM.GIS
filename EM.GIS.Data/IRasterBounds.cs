using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 栅格范围接口
    /// </summary>
    public interface IRasterBounds:ICloneable
    {
        /// <summary>
        /// 仿射系数
        /// X' = [0] + [1] * Column + [2] * Row
        /// Y' = [3] + [4] * Column + [5] * Row
        /// </summary>
        double[] AffineCoefficients { get; set; }
        /// <summary>
        /// 像素高度
        /// </summary>
        double CellHeight { get; set; }
        /// <summary>
        /// 像素宽度
        /// </summary>
        double CellWidth { get; set; }

        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        int NumColumns { get; }

        /// <summary>
        /// 高度
        /// </summary>
        int NumRows { get; }

        /// <summary>
        /// 世界范围文件路径
        /// </summary>
        string WorldFile { get;  }

        /// <summary>
        /// 左上角X坐标
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// 左上角Y坐标
        /// </summary>
        double Y { get; set; }

        /// <summary>
        /// 打开世界文件
        /// </summary>
        /// <param name="worldFileName"></param>
        void OpenWorldFile(string worldFileName);

        /// <summary>
        /// 保存
        /// </summary>
        void Save();
        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="worldFileName"></param>
        void SaveAs(string worldFileName);
    }
}
