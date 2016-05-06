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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using SearchAThing.Core;
using static SearchAThing.Sci.MUCollection;

namespace SearchAThing.Sci.Examples
{

    [DataContract(IsReference = true)]
    class SampleProject : Project
    {

        [DataMember]
        public double A;

        [DataMember]
        public double B;

    };

    class Program
    {

        static void Main(string[] args)
        {            
            var la = 2 * mm;
            var lb = 5 * m;
            var lc = 2.4 * km;

            var km_to_m = Length.ConvertFactor(km, m);

            // write a project to xml
            {
                var binary = false;
                var knownTypes = new List<Type>() { typeof(Project), typeof(SampleProject) };

                {
                    var prj = new SampleProject();
                    prj.A = 1;
                    prj.B = 22;
                    prj.Save("test.xml", binary, knownTypes);
                }

                {
                    var prj = "test.xml".Deserialize<SampleProject>(binary, knownTypes);
                    Console.WriteLine($"prj read back data A={prj.A} B={prj.B}");
                }

            }

        }

    }

}
