using System;
using System.Drawing;

namespace EM.GIS.Symbology
{
    public static class RandomExtension
    {
        #region Methods

        public static bool NextBool(this Random generator)
        {
            return generator.Next(0, 1) == 1;
        }

        public static bool[] NextBoolArray(this Random generator, int minLength, int maxLength)
        {
            var len = generator.Next(minLength, maxLength);
            var result = new bool[len];
            for (var i = 0; i < len; i++)
            {
                result[i] = generator.Next(0, 1) == 1;
            }

            return result;
        }

        public static Color NextColor(this Random generator)
        {
            //byte a = (byte)generator.Next(0, 255);
            byte r = (byte)generator.Next(0, 255);
            byte g = (byte)generator.Next(0, 255);
            byte b = (byte)generator.Next(0, 255);
            Color color = Color.FromArgb( r, g, b);
            return color;
        }

        public static T NextEnum<T>(this Random generator) where T : Enum
        {
            var names = Enum.GetNames(typeof(T));
            string name = names[generator.Next(0, names.Length - 1)];
            T t = (T)Enum.Parse(typeof(T), name);
            return t;
        }

        /// <summary>
        /// Generates a floating point value from 0 to 1
        /// </summary>
        /// <param name="generator">The random class to extend</param>
        /// <returns>A new randomly created floating point value from 0 to 1</returns>
        public static float NextFloat(this Random generator)
        {
            return (float)generator.NextDouble();
        }

        /// <summary>
        /// Generates a random floating point value from 0 to the specified extremeValue, which can
        /// be either positive or negative.
        /// </summary>
        /// <param name="generator">This random class</param>
        /// <param name="extremeValue">The floating point maximum for the number being calculated</param>
        /// <returns>A value ranging from 0 to ma</returns>
        public static float NextFloat(this Random generator, float extremeValue)
        {
            return generator.NextFloat() * extremeValue;
        }

        /// <summary>
        /// Calculates a random floating point value that ranges between (inclusive) the specified minimum and maximum values.
        /// </summary>
        /// <param name="generator">The random class to generate the random value</param>
        /// <param name="minimum">The floating point maximum</param>
        /// <param name="maximum">The floating point minimum</param>
        /// <returns>A floating point value that is greater than or equal to the minimum and less than or equal to the maximum</returns>
        public static float NextFloat(this Random generator, float minimum, float maximum)
        {
            var dbl = generator.NextDouble();
            var spread = maximum - (double)minimum;
            return (float)((dbl * spread) + minimum);
        }

        #endregion
    }
}