using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class NumberExtensions
    {
        /// <summary>
        /// 拉伸值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="factor">系数</param>
        /// <returns>拉伸后的值</returns>
        public static byte StretchToByteValue(this byte value, double maxValue, double minValue, double factor)
        {
            byte destValue;
            if (value >= maxValue)
            {
                destValue = (byte)maxValue;
            }
            else if (value <= minValue)
            {
                destValue = (byte)minValue;
            }
            else
            {
                destValue = (byte)((value - minValue) * factor);
            }
            return destValue;
        }
        /// <summary>
        /// 拉伸值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="factor">系数</param>
        /// <returns>拉伸后的值</returns>
        public static byte StretchToByteValue(this ushort value, double maxValue, double minValue, double factor)
        {
            byte destValue;
            if (value >= maxValue)
            {
                destValue = (byte)maxValue;
            }
            else if (value <= minValue)
            {
                destValue = (byte)minValue;
            }
            else
            {
                destValue = (byte)((value - minValue) * factor);
            }
            return destValue;
        }
        /// <summary>
        /// 拉伸值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="factor">系数</param>
        /// <returns>拉伸后的值</returns>
        public static byte StretchToByteValue(this short value, double maxValue, double minValue, double factor)
        {
            byte destValue;
            if (value >= maxValue)
            {
                destValue = (byte)maxValue;
            }
            else if (value <= minValue)
            {
                destValue = (byte)minValue;
            }
            else
            {
                destValue = (byte)((value - minValue) * factor);
            }
            return destValue;
        }
        /// <summary>
        /// 拉伸值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="factor">系数</param>
        /// <returns>拉伸后的值</returns>
        public static byte StretchToByteValue(this uint value, double maxValue, double minValue, double factor)
        {
            byte destValue;
            if (value >= maxValue)
            {
                destValue = (byte)maxValue;
            }
            else if (value <= minValue)
            {
                destValue = (byte)minValue;
            }
            else
            {
                destValue = (byte)((value - minValue) * factor);
            }
            return destValue;
        }
        /// <summary>
        /// 拉伸值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="factor">系数</param>
        /// <returns>拉伸后的值</returns>
        public static byte StretchToByteValue(this int value, double maxValue, double minValue, double factor)
        {
            byte destValue;
            if (value >= maxValue)
            {
                destValue = (byte)maxValue;
            }
            else if (value <= minValue)
            {
                destValue = (byte)minValue;
            }
            else
            {
                destValue = (byte)((value - minValue) * factor);
            }
            return destValue;
        }
        /// <summary>
        /// 拉伸值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="factor">系数</param>
        /// <returns>拉伸后的值</returns>
        public static byte StretchToByteValue(this float value, double maxValue, double minValue, double factor)
        {
            byte destValue;
            if (value >= maxValue)
            {
                destValue = (byte)maxValue;
            }
            else if (value <= minValue)
            {
                destValue = (byte)minValue;
            }
            else
            {
                destValue = (byte)((value - minValue) * factor);
            }
            return destValue;
        }
        /// <summary>
        /// 拉伸值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="factor">系数</param>
        /// <returns>拉伸后的值</returns>
        public static byte StretchToByteValue(this double value, double maxValue, double minValue, double factor)
        {
            byte destValue;
            if (value >= maxValue)
            {
                destValue = (byte)maxValue;
            }
            else if (value <= minValue)
            {
                destValue = (byte)minValue;
            }
            else
            {
                destValue = (byte)((value - minValue) * factor);
            }
            return destValue;
        }
    }
}
