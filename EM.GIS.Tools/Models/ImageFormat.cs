using System.ComponentModel.DataAnnotations;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 图片格式
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// JPG
        /// </summary>
        [Display(Name = "JPG")]
        JPG,
        /// <summary>
        /// PNG
        /// </summary>
        [Display(Name = "PNG")]
        PNG,
        /// <summary>
        /// TIF
        /// </summary>
        [Display(Name = "TIF")]
        TIF
    }
}
