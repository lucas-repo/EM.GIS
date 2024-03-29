﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Projections
{
    /// <summary>
    /// 转换接口
    /// </summary>
    public interface ITransformation
    {
        /// <summary>
        /// 源投影
        /// </summary>
        Projection SrcProjection { get; set; }
        /// <summary>
        /// 目标投影
        /// </summary>
        Projection DestProjection { get; set; }
        /// <summary>
        /// 转换点
        /// </summary>
        /// <param name="inout"></param>
        void TransformPoint(double[] inout);
        /// <summary>
        /// 转换点
        /// </summary>
        /// <param name="argout"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void TransformPoint(double[] argout, double x, double y, double z);
        /// <summary>
        /// 转换多个点
        /// </summary>
        /// <param name="nCount"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void TransformPoints(int nCount, double[] x, double[] y, double[] z);
    }
}
