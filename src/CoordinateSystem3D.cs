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

namespace SearchAThing.Sci
{

    public class CoordinateSystem3D
    {

        Matrix3D m;
        Matrix3D mInv;

        public Vector3D Origin { get; private set; }
        public Vector3D BaseX { get; private set; }
        public Vector3D BaseY { get; private set; }
        public Vector3D BaseZ { get; private set; }

        public static CoordinateSystem3D WCS = new CoordinateSystem3D(Vector3D.Zero, Vector3D.XAxis, Vector3D.YAxis, Vector3D.ZAxis);

        /// <summary>
        /// Construct a coordinate system with the given origin and orthonormal bases
        /// </summary>        
        public CoordinateSystem3D(Vector3D o, Vector3D baseX, Vector3D baseY, Vector3D baseZ)
        {
            Origin = o;
            BaseX = baseX;
            BaseY = baseY;
            BaseZ = baseZ;

            m = Matrix3D.FromVectorsAsColumns(BaseX, BaseY, BaseZ);
            mInv = m.Inverse();
        }

        /// <summary>
        /// Construct a right-hand coordinate system with the given origin and bases such as:
        /// BaseX = v1
        /// BaseZ = v1 x BaseY
        /// BaseY = BaseZ x BaseX
        /// </summary>        
        public CoordinateSystem3D(Vector3D o, Vector3D v1, Vector3D v2)
        {
            Origin = o;
            BaseX = v1.Normalized();
            BaseZ = v1.CrossProduct(v2).Normalized();
            BaseY = BaseZ.CrossProduct(BaseX).Normalized();

            m = Matrix3D.FromVectorsAsColumns(BaseX, BaseY, BaseZ);
            mInv = m.Inverse();
        }

        /// <summary>
        /// Transform given wcs vector into this ucs
        /// </summary>        
        public Vector3D ToUCS(Vector3D p)
        {
            return mInv * (p - Origin);
        }

        public Vector3D ToWCS(Vector3D p)
        {
            return m * p + Origin;
        }

        public bool IsParallelTo(double tol, CoordinateSystem3D other)
        {
            return BaseZ.IsParallelTo(tol, other.BaseZ);
        }

    }

}
