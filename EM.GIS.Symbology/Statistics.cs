using System;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public class Statistics
    {
        #region Properties
        
        public int Count { get; protected set; }
        
        public double Maximum { get; protected set; }
        
        public double Mean { get; protected set; }
        
        public double Median { get; protected set; }
        
        public double Minimum { get; protected set; }
        
        public double StandardDeviation { get; protected set; }
        
        public double Sum { get; protected set; }

        #endregion

        #region Methods
        
        public void Calculate(List<double> values)
        {
            values.Sort(); 

            if (values.Count == 0)
            {
                Clear();
                return;
            }

            if (values.Count % 2 == 0)
            {
                int lowIndex = (values.Count - 1) / 2; 
                Median = (values[lowIndex] + values[lowIndex + 1]) / 2;
            }
            else
            {
                int index = values.Count / 2;
                Median = values[index];
            }

            Count = values.Count;
            Minimum = values[0];
            Maximum = values[values.Count - 1];

            double total = 0;
            double sqrTotal = 0;
            foreach (double val in values)
            {
                total += val;
                sqrTotal += val * val;
            }

            Sum = total;
            Mean = total / Count;
            StandardDeviation = Math.Sqrt((sqrTotal / Count) - ((total / Count) * (total / Count)));
        }
        
        public void Clear()
        {
            Count = 0;
            Minimum = 0;
            Maximum = 0;
            Mean = 0;
            Median = 0;
            Sum = 0;
            StandardDeviation = 0;
        }

        #endregion
    }
}