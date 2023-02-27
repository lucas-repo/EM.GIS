using System.ComponentModel.DataAnnotations;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 输出文件类型
    /// </summary>
    public enum OutType
    {
        /// <summary>
        /// 拼接图像
        /// </summary>
        [Display(Name = "拼接图像")]
        Splice,
        /// <summary>
        /// 谷歌瓦片
        /// </summary>
        [Display(Name = "谷歌瓦片")]
        GoogleTiles,
        /// <summary>
        /// 瓦片库
        /// </summary>
        [Display(Name = "瓦片库")]
        MBTiles,
    }
}
