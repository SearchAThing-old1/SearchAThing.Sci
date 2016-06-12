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

using System;
using System.Linq;
using SearchAThing.Core;
using static System.Math;
using System.Globalization;
using System.Collections.Generic;
using netDxf.Entities;
using SearchAThing.Sci;
using netDxf;
using netDxf.Blocks;
using netDxf.Tables;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// add entity to the given dxf object ( it can be Dxfdocument or Block )
        /// optionally set layer
        /// </summary>        
        public static EntityObject AddEntity(this DxfObject dxfObj, EntityObject eo, Layer layer = null)
        {
            if (dxfObj is DxfDocument) (dxfObj as DxfDocument).AddEntity(eo);
            else if (dxfObj is Block) (dxfObj as Block).Entities.Add(eo);
            else throw new ArgumentException($"dxfObj must DxfDocument or Block");

            if (layer != null) eo.Layer = layer;

            return eo;
        }

        /// <summary>
        /// add entity to the given dxf object ( it can be Dxfdocument or Block )
        /// optionally set layer
        /// </summary>        
        public static void AddEntities(this DxfObject dxfObj, IEnumerable<EntityObject> ents, Layer layer = null)
        {           
            foreach (var ent in ents) dxfObj.AddEntity(ent, layer);            
        }

        /// <summary>
        /// Set layer of given set of dxf entities
        /// </summary>        
        public static IEnumerable<EntityObject> SetLayer(this IEnumerable<EntityObject> ents, Layer layer)
        {
            foreach (var x in ents) x.Layer = layer;
            return ents;
        }

        /// <summary>
        /// Creates and add dxf entities for a 3 axis of given length centered in given center point.
        /// </summary>        
        public static IEnumerable<EntityObject> DrawStar(this DxfObject dxfObj, Vector3D center, double L, Layer layer = null)
        {
            var q = DxfKit.Star(center, L).ToList();

            foreach (var line in q) dxfObj.AddEntity(line, layer);

            return q;
        }

        /// <summary>
        /// Creates and add dxf entities for a 6 faces of a cube
        /// </summary>        
        public static IEnumerable<EntityObject> DrawCube(this DxfObject dxfObj, Vector3D center, double L, Layer layer = null)
        {
            var ents = DxfKit.Cuboid(center, new Vector3D(L, L, L)).ToList();

            dxfObj.AddEntities(ents, layer);

            return ents;
        }

        /// <summary>
        /// Creates and add dxf entities for 6 faces of a cuboid
        /// </summary>        
        public static IEnumerable<EntityObject> DrawCuboid(this DxfObject dxfObj, Vector3D center, Vector3D size, Layer layer = null)
        {
            var ents = DxfKit.Cuboid(center, size).ToList();

            dxfObj.AddEntities(ents, layer);

            return ents;
        }

    }

    namespace Sci
    {

        public static partial class DxfKit
        {

            /// <summary>
            /// Creates dxf entities for a 3 axis of given length centered in given center point.
            /// </summary>        
            public static IEnumerable<Line> Star(Vector3D center, double L)
            {
                yield return new Line((center - L / 2 * Vector3D.XAxis).ToVector3(), (center + L / 2 * Vector3D.XAxis).ToVector3());
                yield return new Line((center - L / 2 * Vector3D.YAxis).ToVector3(), (center + L / 2 * Vector3D.YAxis).ToVector3());
                yield return new Line((center - L / 2 * Vector3D.ZAxis).ToVector3(), (center + L / 2 * Vector3D.ZAxis).ToVector3());
            }

            /// <summary>
            /// Creates dxf entities for a 6 faces of a cube
            /// </summary>        
            public static IEnumerable<Face3d> Cube(Vector3D center, double L)
            {
                return Cuboid(center, new Vector3D(L, L, L));
            }

            /// <summary>
            /// Creates dxf entities for 6 faces of a cuboid
            /// </summary>        
            public static IEnumerable<Face3d> Cuboid(Vector3D center, Vector3D size)
            {
                var corner = center - size / 2;

                // is this a cuboid ? :)
                //
                //       011------------111
                //      / .            / |
                //   001------------101  |      z
                //    |   .          |   |      |    y
                //    |   .          |   |      |  /
                //    |  010.........|. 110     | /
                //    | .            | /        |/
                //   000------------100         ---------x
                //
                var m = new Vector3[2, 2, 2];
                for (int xi = 0; xi < 2; ++xi)
                {
                    for (int yi = 0; yi < 2; ++yi)
                    {
                        for (int zi = 0; zi < 2; ++zi)
                        {
                            m[xi, yi, zi] = (corner + size.Scalar(xi, yi, zi)).ToVector3();
                        }
                    }
                }

                yield return new Face3d(m[0, 0, 0], m[1, 0, 0], m[1, 0, 1], m[0, 0, 1]); // front
                yield return new Face3d(m[0, 1, 0], m[0, 1, 1], m[1, 1, 1], m[1, 1, 0]); // back
                yield return new Face3d(m[0, 0, 0], m[0, 0, 1], m[0, 1, 1], m[0, 1, 0]); // left
                yield return new Face3d(m[1, 0, 0], m[1, 1, 0], m[1, 1, 1], m[1, 0, 1]); // right
                yield return new Face3d(m[0, 0, 0], m[0, 1, 0], m[1, 1, 0], m[1, 0, 0]); // bottom
                yield return new Face3d(m[0, 0, 1], m[1, 0, 1], m[1, 1, 1], m[0, 1, 1]); // top
            }

        }

    }

}