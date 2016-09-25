#region SearchAThing.Sci, Copyright(C) 2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using DotSpatial.Projections;
using System;
using System.Collections.Generic;
using static System.FormattableString;

namespace SearchAThing.Sci
{

    /// <summary>
    /// Hold data for Coordinate Reference System transformations
    /// </summary>
    public class CRSData
    {

        /// <summary>
        /// DotSpatial ProjectionInfo
        /// </summary>
        public ProjectionInfo ProjectionInfo { get; private set; }

        /// <summary>
        /// non null if this CRS convert using custom function
        /// </summary>
        public CustomCRSInfo CustomCRSInfo { get; private set; }

        public string Name
        {
            get
            {
                if (CustomCRSInfo != null)
                    return CustomCRSInfo.Name;
                else
                    return ProjectionInfo.Name;
            }
        }

        /// <summary>
        /// true if this CRS convert using custom function
        /// </summary>
        public bool IsCustom
        {
            get
            {
                return CustomCRSInfo != null;
            }
        }

        /// <summary>
        /// true if this is a latlon world system
        /// </summary>
        public bool IsGeocentric
        {
            get
            {
                if (IsCustom)
                    return CustomCRSInfo.IsGeoCentric;
                else
                    return ProjectionInfo.IsGeocentric;
            }
        }

        /// <summary>
        /// true if this is a latlon world system
        /// </summary>
        public bool IsLatLon
        {
            get
            {
                if (IsCustom)
                    return CustomCRSInfo.IsGeoCentric;
                else
                    return ProjectionInfo.IsLatLon;
            }
        }

        /// <summary>
        /// measure unit used by this crs
        /// </summary>
        public MeasureUnit Unit
        {
            get
            {
                if (IsCustom)
                    return CustomCRSInfo.MU;
                else
                {
                    if (IsGeocentric || IsLatLon)
                        return MUCollection.PlaneAngle.grad;
                    else
                    {
                        var unit = ProjectionInfo.Unit;

                        switch (unit.Name)
                        {
                            case "Meter": return MUCollection.Length.m;
                            case "Foot": return MUCollection.Length.ft;
                            case "Yard": return MUCollection.Length.yard;
                            case "Link": return MUCollection.Length.links;
                            default: throw new NotImplementedException($"unsupported linear unit [{unit.Name}]");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve proj4 string
        /// </summary>
        public string Proj4String
        {
            get
            {
                if (ProjectionInfo != null)
                    return ProjectionInfo.ToProj4String();
                else
                    return "";
            }
        }

        /// <summary>
        /// Retrieve esri string
        /// </summary>
        public string EsriString
        {
            get
            {
                if (ProjectionInfo != null)
                    return ProjectionInfo.ToEsriString();
                else
                    return "";
            }
        }

        /// <summary>
        /// create a new instance of CRS from given DotSpatial ProjectionInfo
        /// </summary>        
        public CRSData(ProjectionInfo projectionInfo)
        {
            ProjectionInfo = projectionInfo;
        }

        /// <summary>
        /// create a new instance of CRS from given custom transformation function
        /// </summary>        
        public CRSData(CustomCRSInfo customCRSInfo)
        {
            CustomCRSInfo = customCRSInfo;
        }

        /// <summary>
        /// create a new instance of CRS from given esri WKT string
        /// </summary>        
        public CRSData(string name, string esriString)
        {
            var prj = new ProjectionInfo();
            prj.ParseEsriString(esriString);
            ProjectionInfo = prj;
        }

        /// <summary>
        /// Project given vector from this CRS to the given one
        /// </summary>        
        public Vector3D Project(Vector3D v, CRSData to)
        {
            if (IsCustom)
                return CustomCRSInfo.CustomProject(v, this, to);
            else
            {
                var xy = new double[] { v.X, v.Y };
                var z = new double[] { v.Z };
                Reproject.ReprojectPoints(xy, z, ProjectionInfo, to.ProjectionInfo, 0, 1);
                return new Vector3D(xy[0], xy[1], z[0]);
            }
        }

    }

    /// <summary>
    /// holds CRS Custom info
    /// </summary>
    public class CustomCRSInfo
    {

        public string Name { get; private set; }
        public bool IsGeoCentric { get; private set; }
        public MeasureUnit MU { get; private set; }
        public CustomProject CustomProject { get; private set; }

        public CustomCRSInfo(string name, bool isGeoCentric, MeasureUnit mu, CustomProject customProjectFn)
        {
            Name = name;
            IsGeoCentric = isGeoCentric;
            MU = mu;
            CustomProject = customProjectFn;
        }

    }

    /// <summary>
    /// delegate for custom CRS transformation
    /// </summary>    
    public delegate Vector3D CustomProject(Vector3D v, CRSData from, CRSData to);

    /// <summary>
    /// Catalogue of all available CRS
    /// </summary>
    public static class CRSCatalog
    {

        static CRSData _WGS84;
        /// <summary>
        /// Well known WGS84 world latlon system
        /// </summary>
        public static CRSData WGS84
        {
            get
            {
                if (_WGS84 == null) _WGS84 = CRSList["EPSG:4326"];

                return _WGS84;
            }
        }

        static Dictionary<string, CRSData> _CRSList;
        /// <summary>
        /// List of all available CRS, included registered custom
        /// </summary>
        public static IReadOnlyDictionary<string, CRSData> CRSList
        {
            get
            {
                if (_CRSList == null)
                {
                    _CRSList = new Dictionary<string, CRSData>();
                    foreach (var x in DotSpatial.Projections.AuthorityCodes.AuthorityCodeHandler.Instance.AllProjectionInfo)
                    {
                        _CRSList.Add(x.Name, new CRSData(x));
                    }
                }
                return _CRSList;
            }
        }

        /// <summary>
        /// Add a custom crs to the catalogue dictionary
        /// </summary>        
        public static void AddCustom(CustomCRSInfo custom)
        {
            _CRSList.Add(custom.Name, new CRSData(custom));
        }

    }

    /// <summary>
    /// Describe a zone of validity for a CRS system
    /// </summary>
    public class CRSAreaOfUse
    {

        public double WestBoundLongitudeDeg { get; private set; }
        public double SouthBoundLatitudeDeg { get; private set; }
        public double EastBoundLongitudeDeg { get; private set; }
        public double NorthBoundLatitudeDeg { get; private set; }

        public CRSAreaOfUse(double westBoundLongitudeDeg, double southBoundLatitudeDeg,
            double eastBoundLongitudeDeg, double northBoundLatitudeDeg)
        {
            WestBoundLongitudeDeg = westBoundLongitudeDeg;
            SouthBoundLatitudeDeg = southBoundLatitudeDeg;
            EastBoundLongitudeDeg = eastBoundLongitudeDeg;
            NorthBoundLatitudeDeg = northBoundLatitudeDeg;

            if (WestBoundLongitudeDeg > EastBoundLongitudeDeg ||
                SouthBoundLatitudeDeg > NorthBoundLatitudeDeg) throw new Exception($"invalid bound coords [{ToString()}] given"); 

        }

        /// <summary>
        /// check if given longitude, latitude is valid within this area of use
        /// </summary>        
        public bool Contains(double longitudeDeg, double latitudeDeg)
        {
            // longitude range [-180,180]
            // latitude range [-90,90]
            return
                longitudeDeg >= WestBoundLongitudeDeg && longitudeDeg <= EastBoundLongitudeDeg &&
                latitudeDeg >= SouthBoundLatitudeDeg && latitudeDeg <= NorthBoundLatitudeDeg;            
        }

        public override string ToString()
        {
            return Invariant($"west[{WestBoundLongitudeDeg}], south[{SouthBoundLatitudeDeg}], east[{EastBoundLongitudeDeg}], north[{NorthBoundLatitudeDeg}]");
        }

    }

    public static partial class Extensions
    {

        /// <summary>
        /// Project this vector from the given CRS to the other one
        /// </summary>        
        public static Vector3D Project(this Vector3D v, CRSData from, CRSData to)
        {
            return from.Project(v, to);
        }

        /// <summary>
        /// check if given x=longitude, y=latitude is valid within given area of use
        /// </summary>        
        public static bool IsValid(this Vector3D v, CRSAreaOfUse areaOfUse)
        {
            return areaOfUse.Contains(v.X, v.Y);
        }

    }

}
