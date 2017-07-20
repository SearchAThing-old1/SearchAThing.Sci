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

using System;

//using sVector3D = System.Windows.Media.Media3D.Vector3D;
using System.Globalization;
using System.Collections.Generic;
using netDxf;

namespace SearchAThing
{

    namespace Sci
    {

        public partial class Vector3D
        {

            public string CadScript
            {
                get
                {
                    return string.Format(CultureInfo.InvariantCulture, "_POINT {0},{1},{2}\r\n", X, Y, Z);
                }
            }

            public string CadScriptLineFrom
            {
                get
                {
                    return string.Format(CultureInfo.InvariantCulture, "_LINE {0},{1},{2}\r\n", X, Y, Z);
                }
            }

            public override BBox3D BBox(double tol_len, double tol_rad)
            {
                return new BBox3D(new[] { this });
            }

            public override IEnumerable<Vector3D> Divide(int cnt, bool include_endpoints = false)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// convert given Vector2 to a Vector3D ( with z=0 )
            /// </summary>            
            public static implicit operator Vector3D(Vector2 v)
            {
                return new Vector3D(v.X, v.Y, 0);
            }

            /// <summary>
            /// Convert given Vector3 to Vector3D
            /// </summary>            
            public static implicit operator Vector3D(Vector3 v)
            {
                return new Vector3D(v.X, v.Y, v.Z);
            }

            /// <summary>
            /// Convert given Vector3D to Vector3
            /// </summary>            
            public static implicit operator Vector3(Vector3D v)
            {
                return new Vector3(v.X, v.Y, v.Z);
            }

        }

    }

}