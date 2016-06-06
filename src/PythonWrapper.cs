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
using System.Collections.Generic;
using SearchAThing.Sci;
using ClipperLib;
using System.Threading.Tasks;
using System.Text;

namespace SearchAThing
{

    public class PythonWrapper
    {
        ProcessTask processTask = null;

        string _TerminatingFn = "print('>>>')";
        public string TerminatingFn
        {
            get { return _TerminatingFn; }
            set { _TerminatingFn = value; }
        }

        string _TerminateExpectedOutput = ">>>";
        public string TerminateExpectedOutput
        {
            get { return _TerminateExpectedOutput; }
            set { _TerminateExpectedOutput = value; }
        }

        public PythonWrapper(string args = "-i -q")
        {
            processTask = new ProcessTask("python.exe", ProcessTaskPathMode.SeachInEnvironment, "PYTHONPATH");
            processTask.Arguments = args;
            processTask.RedirectStandardInput = true;
            processTask.RedirectStandardOutput = true;
            processTask.RedirectStandardError = true;
        }

        public void Start()
        {
            processTask.Start();
        }

        /// <summary>
        /// restart the interpreter
        /// </summary>
        public void Recycle()
        {
            processTask.Recycle();
        }

        public void Kill()
        {
            processTask.Kill();
        }

        /// <summary>
        /// Writes to the interpreter given command using terminating special function
        /// in order to allow state the pipeline output end
        /// </summary>       
        public void Write(string str)
        {
            if (!processTask.IsActive) throw new Exception($"process not active. Use Start()");

            var sb = new StringBuilder(str);

            sb.AppendLine();
            sb.AppendLine(TerminatingFn);

            processTask.Write(sb.ToString());
        }

        public async Task<string> Read()
        {
            var sb = new StringBuilder();

            while (true)
            {
                var s = await processTask.ReadOutput();                

                if (s == TerminateExpectedOutput) break;

                sb.AppendLine(s);
            }

            return sb.ToString();
        }

    }

}
