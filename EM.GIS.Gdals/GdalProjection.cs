﻿using EM.GIS.GdalExtensions;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 投影信息
    /// </summary>
    public class GdalProjection : Projection
    {
        /// <summary>
        /// 缓存的投影
        /// </summary>
        private ConcurrentDictionary<int, IProjection> projections = new ConcurrentDictionary<int, IProjection>();
        private SpatialReference spatialReference;

        /// <summary>
        /// 空间参考
        /// </summary>
        public SpatialReference SpatialReference
        {
            get { return spatialReference; }
            set
            {
                if (SpatialReferenceDisposable)
                {
                    SetProperty(ref spatialReference, value);
                }
                else
                {
                    SetProperty(ref spatialReference, value);
                }
            }
        }
        /// <summary>
        /// 空间参考是否可释放
        /// </summary>
        public bool SpatialReferenceDisposable { get; set; } = true;
        static GdalProjection()
        {
            GdalConfiguration.ConfigureGdal();
        }
        public GdalProjection(SpatialReference spatialReference)
        {
            SpatialReference = spatialReference;
        }
        public GdalProjection(string wkt)
        {
            SpatialReference = new SpatialReference(wkt);
        }
        public GdalProjection(int epsg)
        {
            SpatialReference = new SpatialReference(string.Empty);
            SpatialReference.ImportFromEPSG(epsg);
        }
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (projections.Count > 0)
                    {
                        foreach (var item in projections)
                        {
                            if (item.Value is IDisposable disposable)
                            {
                                disposable.Dispose();
                            }
                        }
                        projections.Clear();
                    }
                }
                if (SpatialReferenceDisposable && spatialReference != null)
                {
                    spatialReference.Dispose();
                    spatialReference = null;
                }
            }
            base.Dispose(disposing);
        }
        public override bool IsValid => SpatialReference.Validate() == 0;
        public override void ImportFromWkt(string wkt)
        {
            var destWkt = wkt;
            var ret = SpatialReference.ImportFromWkt(ref destWkt);
        }

        public override string ExportToWkt()
        {
            var ret = SpatialReference.ExportToWkt(out string wkt);
            return wkt;
        }

        public override void ImportFromESRI(string wkt)
        {
            string[] arry = { wkt };
            var ret = SpatialReference.ImportFromESRI(arry);
        }

        public override void ImportFromProj4(string proj4)
        {
            var ret = SpatialReference.ImportFromProj4(proj4);
        }

        public override string ExportToProj4()
        {
            var ret = SpatialReference.ExportToProj4(out string proj4);
            return proj4;
        }
        public override string Authority
        {
            get => SpatialReference?.GetAuthorityName(null);
            set
            {
                if (SpatialReference != null && EPSG.HasValue)
                {
                    SpatialReference.SetAuthority(null, value, EPSG.Value);
                }
            }
        }
        public override string AuthorityCode
        {
            get => SpatialReference?.GetAuthorityCode(null);
            set
            {
                if (SpatialReference != null && EPSG.HasValue)
                {
                    SpatialReference.SetAuthority(null, Authority, EPSG.Value);
                }
            }
        }
        public override bool IsLatLon { get => SpatialReference?.IsProjected() != 1; }
        public override double? CentralMeridian
        {
            get => SpatialReference?.GetProjParm("Central_Meridian", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("Central_Meridian", value.Value);
                }
            }
        }
        public override AuxiliarySphereType AuxiliarySphereType
        {
            get
            {
                AuxiliarySphereType auxiliarySphereType = AuxiliarySphereType.NotSpecified;
                double? auxType = SpatialReference?.GetProjParm("Auxiliary_Sphere_Type", double.NaN);
                if (auxType.HasValue && !double.IsNaN(auxType.Value))
                {
                    auxiliarySphereType = (AuxiliarySphereType)auxType;
                }
                return auxiliarySphereType;
            }
        }
        public override double? FalseEasting
        {
            get => SpatialReference?.GetProjParm("False_Easting", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("False_Easting", value.Value);
                }
            }
        }
        public override double? FalseNorthing
        {
            get => SpatialReference?.GetProjParm("False_Northing", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("False_Northing", value.Value);
                }
            }
        }
        public override double? LongitudeOfCenter
        {
            get => SpatialReference?.GetProjParm("Longitude_Of_Center", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("Longitude_Of_Center", value.Value);
                }
            }
        }
        public override double? StandardParallel1
        {
            get => SpatialReference?.GetProjParm("Standard_Parallel_1", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("Standard_Parallel_1", value.Value);
                }
            }
        }
        public override double? StandardParallel2
        {
            get => SpatialReference?.GetProjParm("Standard_Parallel_2", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("Standard_Parallel_2", value.Value);
                }
            }
        }
        public override double ScaleFactor
        {
            get
            {
                double scaleFactor = 0;
                if (SpatialReference != null)
                {
                    scaleFactor = SpatialReference.GetProjParm("Scale_Factor", 0);
                }
                return scaleFactor;
            }
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetProjParm("Scale_Factor", value);
                }
            }
        }
        public override double? alpha
        {
            get => SpatialReference?.GetProjParm("Azimuth", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("Azimuth", value.Value);
                }
            }
        }
        public override double? lon_1
        {
            get => SpatialReference?.GetProjParm("Longitude_Of_1st", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("Longitude_Of_1st", value.Value);
                }
            }
        }
        public override double? lon_2
        {
            get => SpatialReference?.GetProjParm("Longitude_Of_2nd", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("Longitude_Of_2nd", value.Value);
                }
            }
        }
        public override double? LatitudeOfOrigin
        {
            get => SpatialReference?.GetProjParm("Latitude_Of_Origin", double.NaN);
            set
            {
                if (SpatialReference != null && value.HasValue)
                {
                    SpatialReference.SetProjParm("Latitude_Of_Origin", value.Value);
                }
            }
        }
        public override GeographicInfo GeographicInfo
        {
            get
            {
                if (base.GeographicInfo == null)
                {
                    base.GeographicInfo = new GdalGeographicInfo(SpatialReference);
                }
                return base.GeographicInfo;
            }
            set => base.GeographicInfo = value;
        }
        public override string Name
        {
            get
            {
                string name = string.Empty;
                if (SpatialReference != null)
                {
                    if (SpatialReference.IsProjected() == 1)
                    {
                        name = SpatialReference.GetAttrValue("PROJCS", 0);
                    }
                    else
                    {
                        name = SpatialReference.GetAttrValue("GEOGCS", 0);
                    }
                }
                return name;
            }
            set
            {
                if (SpatialReference != null)
                {
                    if (SpatialReference.IsProjected() == 1)
                    {
                        var ret = SpatialReference.SetAttrValue("PROJCS", value);
                    }
                    else
                    {
                        var ret = SpatialReference.SetAttrValue("GEOGCS", value);
                    }
                }
            }
        }
        public override LinearUnit Unit
        {
            get
            {
                if (base.Unit == null)
                {
                    base.Unit = new GdalLinearUnit(SpatialReference);
                }
                return base.Unit;
            }
            set
            {
                base.Unit = value;
            }
        }

        public override int GetHashCode()
        {
            int hashCode = "GdalProjectionInfo".GetHashCode() ^ SpatialReference.GetHashCode();
            return hashCode;
        }

        public override void ReProject(IProjection destProjection, ICoordinate coordinate)
        {
            if (destProjection is GdalProjection gdalProjectionInfo && coordinate != null)
            {
                using (CoordinateTransformation ct = new CoordinateTransformation(SpatialReference, gdalProjectionInfo.SpatialReference))
                {
                    ct.Transform(coordinate);
                }
            }
        }

        public override void ReProject(IProjection destProjection, IGeometry geometry)
        {
            if (destProjection is GdalProjection gdalProjectionInfo && geometry != null)
            {
                using (CoordinateTransformation ct = new CoordinateTransformation(SpatialReference, gdalProjectionInfo.SpatialReference))
                {
                    ct.Transform(geometry);
                }
            }
        }

        public override void ReProject(IProjection destProjection, IList<ICoordinate> coordinates)
        {
            if (destProjection is GdalProjection gdalProjectionInfo && coordinates != null)
            {
                using (CoordinateTransformation ct = new CoordinateTransformation(SpatialReference, gdalProjectionInfo.SpatialReference))
                {
                    ct.Transform(coordinates, coordinates.Count);
                }
            }
        }

        public override void ReProject(IProjection destProjection, IExtent extent)
        {
            if (destProjection is GdalProjection gdalProjectionInfo && extent != null)
            {
                using (CoordinateTransformation ct = new CoordinateTransformation(SpatialReference, gdalProjectionInfo.SpatialReference))
                {
                    ct.Transform(extent);
                }
            }
        }
        protected override void OnCopy(object copy)
        {
            if (copy is GdalProjection gdalProjection)
            {
                gdalProjection.SpatialReference = new SpatialReference(string.Empty);
                if (EPSG.HasValue)
                {
                    gdalProjection.SpatialReference.ImportFromEPSG(EPSG.Value);
                }
            }
        }
        /// <inheritdoc/>
        public override void ReProject(int destProjectionEpsg, IExtent extent)
        {
            if (!projections.TryGetValue(destProjectionEpsg, out var projection))
            {
                projection = new GdalProjection(destProjectionEpsg);
                projections.TryAdd(destProjectionEpsg, projection);
            }
            ReProject(projection, extent);
        }
    }
}
