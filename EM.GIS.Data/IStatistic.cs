namespace EM.GIS.Data
{
    public interface IStatistic
    {
        /// <summary>
        /// Gets the maximum data value, not counting no-data values in the grid.
        /// </summary>
        double Maximum { get; }
        /// <summary>
        /// Gets the mean of the non-NoData values in this grid. If the data is not InRam, then
        /// the GetStatistics method must be called before these values will be correct.
        /// </summary>
        double Mean { get; }
        /// <summary>
        /// Gets the minimum data value that is not classified as a no data value in this raster.
        /// </summary>
        double Minimum { get; }
        /// <summary>
        /// Gets the standard deviation of all the Non-nodata cells. If the data is not InRam,
        /// then you will have to first call the GetStatistics() method to get meaningful values.
        /// </summary>
        double StdDeviation { get; }
    }
}