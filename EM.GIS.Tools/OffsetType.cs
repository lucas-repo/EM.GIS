using System.ComponentModel.DataAnnotations;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 偏移类型
    /// </summary>
    public enum OffsetType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Display(Name ="无")]
        None,
        /// <summary>
        /// GCJ-02坐标（火星坐标系）
        /// </summary>
        [Display(Name = "火星坐标系")]
        Gcj02,
        /// <summary>
        /// BD-09坐标（百度09坐标系）
        /// </summary>
        [Display(Name = "百度09坐标系")]
        Bd09
    }
}