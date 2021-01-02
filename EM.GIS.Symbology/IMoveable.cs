namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可移动元素的接口
    /// </summary>
    public interface IMoveable
    {
        /// <summary>
        /// 移动元素
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        void Move(int oldIndex, int newIndex);
    }
}