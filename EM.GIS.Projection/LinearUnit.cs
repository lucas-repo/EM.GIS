﻿using EM.Bases;
using System;
using System.Globalization;

namespace EM.GIS.Projections
{
    /// <summary>
    /// 线性单位
    /// </summary>
    public class LinearUnit : BaseCopy
    {
        /// <summary>
        /// Gets or sets the name of this
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the constant to multiply with maps distances to get the distances in meters
        /// </summary>
        public virtual double Meters { get; set; }

    }
}