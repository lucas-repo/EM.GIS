using BruTile;
using EM.GIS.Geometries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 瓦片数据集
    /// </summary>
    public class TileSet : RasterSet, ITileSet
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<TileIndex, IRasterSet> Tiles { get; } = new ConcurrentDictionary<TileIndex, IRasterSet>();
        /// <inheritdoc/>
        public override int ByteSize => GetByteSize(default(byte));

        /// <inheritdoc/>
        protected override void Dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    if (Tiles.Count > 0)
                    {
                        foreach (var item in Tiles)
                        {
                            item.Value?.Dispose();
                        }
                        Tiles.Clear();
                    }
                }
            }
            base.Dispose(disposeManagedResources);
        }
        public override Image GetImage(IExtent envelope, Rectangle window, Action<int> progressAction = null, Func<bool> cancelFunc = null)
        {
            throw new NotImplementedException();
        }
    }
}
