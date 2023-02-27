using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 范围类型
    /// </summary>
    public enum BoundType
    {
        /// <summary>
        /// 选择的要素
        /// </summary>
        [Display(Name ="选择的要素")]
        SelectedFeatures,
        /// <summary>
        /// 中国
        /// </summary>
        [Display(Name = "中国")]
        China
    }
}
