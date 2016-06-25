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

using System.Linq;
using System.Collections.Generic;
using netDxf.Entities;
using netDxf;
using netDxf.Tables;
using SearchAThing.Sci;
using System;

namespace SearchAThing
{

    public static partial class Extensions
    {

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

        public static BBox3D BBox(this EntityObject eo)
        {
            switch (eo.Type)
            {
                case EntityType.Line:
                    return new BBox3D(((Line)eo).ToLine3D().Points);

                case EntityType.LightWeightPolyline:                    
                    return new BBox3D(((LwPolyline)eo).Vertexes.Select(k => k.Position.ToVector3D()));

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
