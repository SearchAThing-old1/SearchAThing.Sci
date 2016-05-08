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

using sVector3D = System.Windows.Media.Media3D.Vector3D;
using sMatrix3D = System.Windows.Media.Media3D.Matrix3D;
using sQuaternion = System.Windows.Media.Media3D.Quaternion;

namespace SearchAThing.Sci
{

    public class Transform3D
    {

        sMatrix3D m;

        static sVector3D sXAxis = new sVector3D(1, 0, 0);
        static sVector3D sYAxis = new sVector3D(0, 1, 0);
        static sVector3D sZAxis = new sVector3D(0, 0, 1);

        public sMatrix3D TransformMatrix { get { return m; } }

        public Transform3D()
        {
            m = new sMatrix3D();            
        }

        public void RotateAboutXAxis(double angleDeg)
        {
            m.Rotate(new sQuaternion(sXAxis, angleDeg));
        }

        public void RotateAboutYAxis(double angleDeg)
        {
            m.Rotate(new sQuaternion(sYAxis, angleDeg));
        }

        public void RotateAboutZAxis(double angleDeg)
        {            
            m.Rotate(new sQuaternion(sZAxis, angleDeg));
        }

        public void RotateAboutAxis(Vector3D axis, double angleDeg)
        {
            m.Rotate(new sQuaternion(new sVector3D(axis.X, axis.Y, axis.Z), angleDeg));
        }

        public Vector3D Apply(Vector3D v)
        {
            return m.Transform(v.ToSystemVector3D()).ToVector3D();
        }

    }

}
