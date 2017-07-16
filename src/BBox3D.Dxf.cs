#region SearchAThing.Sci, Copyright(C) 2016-2017 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016-2017 Lorenzo Delana, https://searchathing.com
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

using System.Linq;
using System.Collections.Generic;
using netDxf.Entities;
using netDxf;
using netDxf.Tables;
using SearchAThing.Sci;
using System;
using System.Text;

namespace SearchAThing
{

    public static partial class Extensions
    {

        public static string CadScript(this BBox3D bbox)
        {
            var sb = new StringBuilder();

            foreach (var x in bbox.ToFace3DList())
            {
                sb.AppendLine(x.CadScript());
            }

            return sb.ToString();
        }

        public static IEnumerable<Face3d> ToFace3DList(this BBox3D bbox)
        {
            var d = bbox.Max - bbox.Min;
            return DxfKit.Cuboid((bbox.Max + bbox.Min) / 2, d);
        }

        public static IEnumerable<Face3d> DrawCuboid(this BBox3D bbox, DxfObject dxfObj, Layer layer = null)
        {
            var ents = bbox.ToFace3DList().ToList();

            dxfObj.AddEntities(ents, layer);

            return ents;
        }

        public static IEnumerable<Vector3D> Points(this EntityObject eo)
        {
            switch (eo.Type)
            {
                case EntityType.Line:
                    {
                        var line = (Line)eo;
                        yield return line.StartPoint;
                        yield return line.EndPoint;
                    }
                    break;

                case EntityType.LightWeightPolyline:
                    {
                        var lw = (LwPolyline)eo;
                        foreach (var x in lw.Vertexes) yield return x.Position.ToVector3D();
                    }
                    break;

                case EntityType.Text:
                    {
                        var txt = (Text)eo;
                        yield return txt.Position;
                    }
                    break;

                case EntityType.MText:
                    {
                        var mtxt = (MText)eo;
                        yield return mtxt.Position;
                    }
                    break;

                case EntityType.Point:
                    {
                        var pt = (Point)eo;
                        yield return pt.Position;
                    }
                    break;

                case EntityType.Insert:
                    {
                        var ins = (Insert)eo;
                        var insPt = ins.Position;
                        var pts = ins.Block.Entities.SelectMany(w => w.Points());

                        pts = pts.Select(w => w.ScaleAbout(Vector3D.Zero, ins.Scale));

                        var N = ins.Normal;
                        var ocs = new CoordinateSystem3D(insPt, N).Rotate(N, ins.Rotation.ToRad());

                        pts = pts.Select(w => w.ToWCS(ocs));

                        foreach (var x in pts) yield return x;
                    };
                    break;

                case EntityType.Hatch:
                    {
                    }
                    break;

                case EntityType.Circle:
                    {
                        var circleLw = ((Circle)eo).ToPolyline(4);
                        foreach (var x in circleLw.Vertexes) yield return x.Position.ToVector3D();
                    }
                    break;

                default:
                    throw new NotImplementedException($"bbox not implemented for dxf entity type [{eo.Type.ToString()}]");
            }
        }

        public static BBox3D BBox(this EntityObject eo)
        {
            switch (eo.Type)
            {
                // TODO consider text width
                case EntityType.Text:
                // TODO consider text width
                case EntityType.MText:
                case EntityType.Line:
                case EntityType.Point:
                case EntityType.Insert:
                    return eo.Points().BBox();

                case EntityType.Arc:
                    {
                        var arc = (eo as Arc).ToArc3D();
                        return new BBox3D(new[] { arc.From, arc.To, arc.MidPoint });
                    }                    

                case EntityType.Circle: return ((Circle)eo).ToPolyline(4).BBox();

                case EntityType.LightWeightPolyline:
                    {
                        var lwpoly = (LwPolyline)eo;

                        var N = lwpoly.Normal;
                        var ocs = new CoordinateSystem3D(N * lwpoly.Elevation, N);

                        return new BBox3D(eo.Points().Select(k => k.ToWCS(ocs)));
                    }


                case EntityType.Hatch: return new BBox3D();

                default:
                    throw new NotImplementedException($"bbox not implemented for dxf entity type [{eo.Type.ToString()}]");
            }
        }

        public static BBox3D BBox(this IEnumerable<EntityObject> ents)
        {
            var bbox = new BBox3D();

            foreach (var x in ents)
            {
                bbox = bbox.Union(x.BBox());
            }

            return bbox;
        }

    }

}

