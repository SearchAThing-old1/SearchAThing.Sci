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

using SearchAThing.Sci;
using System.Collections.Generic;
using System.Linq;

namespace SearchAThing
{

    namespace Sci
    {

        public enum GeometryType
        {
            Vector3D,
            Line3D,
            Circle3D,
            Arc3D
        }

        public abstract class Geometry
        {

            public Geometry(GeometryType type) { Type = type; }

            public GeometryType Type { get; protected set; }

            public abstract IEnumerable<Vector3D> Vertexes { get; }
            public abstract Vector3D GeomFrom { get; }
            public abstract Vector3D GeomTo { get; }

            public abstract netDxf.Entities.EntityObject DxfEntity { get; }

        }

    }

    public static partial class Extensions
    {

        public static IEnumerable<Geometry> ToGeometryBlock(this netDxf.Entities.LwPolyline lwpolyline, double tolLen)
        {
            var geoms = new List<Geometry>();

            var els = lwpolyline.Explode();

            foreach (var el in els)
            {
                if (el.Type == netDxf.Entities.EntityType.Arc)
                {
                    yield return (el as netDxf.Entities.Arc).ToArc3D();
                }
                else if (el.Type == netDxf.Entities.EntityType.Line)
                {
                    var line = (el as netDxf.Entities.Line);
                    if (((Vector3D)line.StartPoint).EqualsTol(tolLen, line.EndPoint)) continue;

                    yield return line.ToLine3D();
                }
            }
        }

        /// <summary>
        /// segments representation of given geometries
        /// if arc found a segment between endpoints returns
        /// </summary>        
        public static IEnumerable<Line3D> Segments(this IReadOnlyList<Geometry> geometry_block)
        {
            foreach (var geom in geometry_block)
            {
                switch (geom.Type)
                {
                    case GeometryType.Line3D: yield return geom as Line3D; break;
                    case GeometryType.Arc3D: yield return (geom as Arc3D).Segment; break;
                    default: throw new System.Exception($"unsupported type [{geom.Type}] on Segments function");
                }
            }
        }

        public static IEnumerable<Vector3D> Vertexes(this IReadOnlyList<Geometry> geometry_block, double tolLen)
        {
            Vector3D last = null;
            for (int i = 0; i < geometry_block.Count; ++i)
            {
                var geom = geometry_block[i];
                var from = geom.GeomFrom;

                if (last != null)
                {
                    if (last.EqualsTol(tolLen, from))
                    {
                        last = geom.GeomTo;
                        yield return from;
                    }
                    else
                    {
                        last = geom.GeomFrom;
                        yield return geom.GeomTo;
                    }
                }
                else
                {
                    last = geom.GeomTo;
                    yield return from;
                }
            }
        }

        public static Vector3D GeomCentroid(this IReadOnlyList<Geometry> geometry_block, double tolLen)
        {
            var segs = geometry_block.Vertexes(tolLen).ToList();

            // TODO centroid with polyline and arcs

            if (geometry_block.Count(r => r.Type == GeometryType.Arc3D) > 1)
            {
                var arcs = geometry_block.Where(r => r.Type == GeometryType.Arc3D).Take(2).Cast<Arc3D>().ToList();
                return (arcs[0].MidPoint + arcs[1].MidPoint) / 2;
            }
            else
            {
                var A = Area(segs, tolLen);
                var centroid = Centroid(segs, tolLen, A);

                // search for arcs
                foreach (var geom in geometry_block)
                {
                    if (geom.Type == GeometryType.Arc3D)
                    {
                        var arc = geom as Arc3D;
                        var arc_sign = segs.ContainsPoint(tolLen, arc.MidPoint) ? -1.0 : 1.0;
                        var arc_A = 0.0;
                        var arc_centre_of_mass = arc.CentreOfMass(out arc_A);

                        var new_centroid_x = (centroid.X * A + arc_centre_of_mass.X * arc_A * arc_sign) / (A + arc_A * arc_sign);
                        var new_centroid_y = (centroid.Y * A + arc_centre_of_mass.Y * arc_A * arc_sign) / (A + arc_A * arc_sign);

                        A += arc_A * arc_sign;
                        centroid = new Vector3D(new_centroid_x, new_centroid_y, 0);
                    }
                }

                return centroid;
            }
        }

    }

}
