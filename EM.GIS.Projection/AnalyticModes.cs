using System;

namespace EM.GIS.Projection
{
    [Flags]
    public enum AnalyticModes
    {
        /// <summary>
        /// Derivatives of lon analytic
        /// </summary>
        IsAnalXlYl = 0x1,

        /// <summary>
        /// Derivatives of lat analytic
        /// </summary>
        IsAnalXpYp = 0x2,

        /// <summary>
        /// h and k are analytic
        /// </summary>
        IsAnalHk = 0x4,

        /// <summary>
        /// convergence analytic
        /// </summary>
        IsAnalConv = 0x8,
    }
}