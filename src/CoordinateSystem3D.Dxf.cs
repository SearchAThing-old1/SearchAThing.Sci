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

using System.Globalization;
using System.Text;
using static System.Math;

namespace SearchAThing
{

    namespace Sci
    {

        public partial class CoordinateSystem3D
        {

            public string ToCadString(double axisLen)
            {
                var sb = new StringBuilder();

                sb.Append(string.Format("-COLOR 1\r\n"));
                sb.Append(new Line3D(Origin, BaseX * axisLen, Line3DConstructMode.PointAndVector).CadScript);
                sb.Append("\r\n");

                sb.Append(string.Format("-COLOR 2\r\n"));
                sb.Append(new Line3D(Origin, BaseY * axisLen, Line3DConstructMode.PointAndVector).CadScript);
                sb.Append("\r\n");

                sb.Append(string.Format("-COLOR 3\r\n"));
                sb.Append(new Line3D(Origin, BaseZ * axisLen, Line3DConstructMode.PointAndVector).CadScript);
                sb.Append("\r\n");

                return sb.ToString();
            }

            public string CadScript
            {
                get
                {
                    return ToCadString(1.0);
                }
            }

        }

    }

}