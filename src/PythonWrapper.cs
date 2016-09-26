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
using SearchAThing.Core;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using static System.Math;
using static System.FormattableString;

namespace SearchAThing
{

    public class PythonWrapper
    {
        ProcessTask processTask = null;
        Action<string> debug = null;

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

        public PythonWrapper(string args = "-i -q", Action<string> _debug = null)
        {
            debug = _debug;
            processTask = new ProcessTask("python.exe", ProcessTaskPathMode.SeachInEnvironment, "PYTHONPATH");
            debug?.Invoke($" py: running [{processTask.Pathfilename}] [{processTask.Arguments}]");
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
            if (!processTask.IsActive)
            {
                debug?.Invoke($" py: process not active");
                throw new Exception($"process not active. Use Start()");
            }

            debug?.Invoke($" py: write [{str}]");

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
                debug?.Invoke($" py: waiting output...");
                var s = await processTask.ReadOutput();

                if (s == TerminateExpectedOutput) break;

                debug?.Invoke($" py: read [{s}]");
                sb.AppendLine(s);
            }

            return sb.ToString();
        }

        public static MultiArray ParseArray(string res)
        {
            var r = res.Trim().StripBegin("array(").StripEnd(")");

            return new MultiArray(r);
        }

        public class MultiArray
        {

            public string[] Elements { get; private set; }
            public List<MultiArray> Children { get; private set; }

            public IEnumerable<double> ElementsAsDouble
            {
                get
                {
                    string[] ee = Elements;

                    if (Elements == null && Children.Count == 1 && Children.First().Elements != null)
                        ee = Children.First().Elements;

                    foreach (var x in ee)
                    {
                        yield return double.Parse(x, CultureInfo.InvariantCulture);
                    }
                }
            }

            public IEnumerable<int> ElementsAsInt
            {
                get
                {
                    string[] ee = Elements;

                    if (Elements == null && Children.Count == 1 && Children.First().Elements != null)
                        ee = Children.First().Elements;

                    foreach (var x in ee)
                    {
                        yield return int.Parse(x);
                    }
                }
            }

            public MultiArray(string s)
            {
                if (s.StartsWith(" ") || s.EndsWith(" ")) s = s.Trim();
                if (s.Length == 0)
                {
                    Elements = new string[] { };
                    return;
                }

                if (s.StartsWith("["))
                {
                    Children = new List<MultiArray>();
                    var q = s.StripBegin("[").StripEnd("]").Trim();
                    if (!q.StartsWith("["))
                        Children.Add(new MultiArray(q));
                    else
                    {
                        var inp = false;
                        for (int i = 0; i < q.Length; ++i)
                        {
                            if (!inp && q[i] == '[')
                            {
                                var sb = new StringBuilder();
                                while (i < q.Length && q[i] != ']')
                                {
                                    sb.Append(q[i]);
                                    ++i;
                                }
                                if (i < q.Length) sb.Append(q[i]);

                                Children.Add(new MultiArray(sb.ToString()));

                                while (i < q.Length && q[i] != ',') ++i;
                            }
                        }
                    }
                }
                else
                {
                    var nrs = s.Split(',');

                    Elements = nrs
                        .Where(r => !r.Trim().StartsWith("dtype"))
                        .Select(w =>
                    {
                        var str = w.Replace('\r', ' ').Replace('\n', ' ').StripEnd("]").Trim();
                        return str;
                    }
                    ).ToArray();
                }

            }

            public override string ToString()
            {
                if (Elements != null || Children.Count == 1 && Children.First().Elements != null)
                {
                    var dbls = Elements;
                    if (Children.Count == 1) dbls = Children.First().Elements;

                    return $"({string.Join(",", dbls.Select(w => Invariant($"{w}")))})";
                }
                else
                    return $"array of {Children.Count} elements";
            }

        }

    }

}
