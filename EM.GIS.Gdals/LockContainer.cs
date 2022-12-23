using System.Collections.Concurrent;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 多个对象的容器字典
    /// </summary>
    public class LockContainer : ConcurrentDictionary<object, object>
    {
        /// <summary>
        /// 获取或创建指定对象的锁对象
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>锁对象</returns>
        public object GetOrCreateLock(object key)
        {
            object lockObj = null;
            if (key == null)
            {
                return lockObj;
            }

            if (!ContainsKey(key))
            {
                TryAdd(key, new object());
            }

            TryGetValue(key, out lockObj);
            return lockObj;
        }
    }
}