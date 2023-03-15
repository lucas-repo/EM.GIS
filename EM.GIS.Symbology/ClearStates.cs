namespace EM.GIS.Symbology
{
    /// <summary>
    /// 清空状态
    /// </summary>
    public enum ClearStates
    {
        /// <summary>
        /// 所选要素不会被清除
        /// </summary>
        False = 0,
        /// <summary>
        /// 选中的特性只有在SelectionEnabled为true时才会被清除
        /// </summary>
        True = 1,
        /// <summary>
        /// 即使SelectionEnabled为false，所选特性也将被清除
        /// </summary>
        Force = 2
    }
}