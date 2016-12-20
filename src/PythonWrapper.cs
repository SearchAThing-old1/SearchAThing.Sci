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
using System.Threading;
using System.Diagnostics;

namespace SearchAThing
{

    #region python pipe
    public class PythonPipe
    {

        #region python path
        static string _PythonExePathfilename;

        internal static string PythonExePathfilename
        {
            get
            {
                if (_PythonExePathfilename == null)
                {
                    var searchFor =
                        (
                        Environment.OSVersion.Platform == PlatformID.Unix
                        ||
                        Environment.OSVersion.Platform == PlatformID.MacOSX
                        )
                        ? "python" : "python.exe";

                    _PythonExePathfilename = Path.SearchInPath(searchFor);
                    if (_PythonExePathfilename == null) _PythonExePathfilename = "";
                }
                return _PythonExePathfilename;
            }
        }
        #endregion

        object wrapper_initialized = new object();
        Action<string> debug = null;

        Process process = null;

        public PythonPipe(string initial_imports = "", Action<string> _debug = null)
        {
            debug = _debug;

            var th = new Thread(() =>
            {
                debug?.Invoke("initializing python");
                lock (wrapper_initialized)
                {
                    var guid = Guid.NewGuid().ToString();

                    process = new Process();
                    process.StartInfo.FileName = PythonExePathfilename;
                    process.StartInfo.Arguments = "-i -q";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.ErrorDialog = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    var started = process.Start();

                    var finished = false;
                    process.OutputDataReceived += (a, b) =>
                    {
                        finished = true;
                    };
                    process.BeginOutputReadLine();

                    process.StandardInput.AutoFlush = false;
                    process.StandardInput.WriteLine($"{initial_imports}\r\nprint('{guid}')\r\n");
                    process.StandardInput.Flush();

                    while (!finished)
                    {
                        Thread.Sleep(250);
                    }

                    process.CancelOutputRead();
                }
                process.WaitForExit();
            });
            th.Start();
        }

        internal const int win32_string_len_safe = 3000;
        internal const int win32_max_string_len = 4000;

        public string Exec(string code)
        {
            if (Extensions.win32.Value)
            {
                var lines = code.Lines(removeEmptyLines: false);
                var q = lines.FirstOrDefault(r => r.Length > win32_max_string_len);
                if (q != null)
                    throw new PythonException($"string too much large, use backslash to split. Line [{q}]", "");
            }

            var sw = new Stopwatch();
            sw.Start();

            string res = "";

            debug?.Invoke("python exec");

            lock (wrapper_initialized)
            {
                var guid = Guid.NewGuid().ToString();

                var sberr = new StringBuilder();
                var sbout = new StringBuilder();

                bool finished = false;
                var hasErr = false;

                process.ErrorDataReceived += (a, b) =>
                {
                    var s = b.Data;
                    while (s.StartsWith(">>> ")) s = s.Substring(4);
                    sberr.AppendLine(s);
                    hasErr = true;
                };

                process.OutputDataReceived += (a, b) =>
                {
                    if (b.Data == guid) finished = true;
                    sbout.AppendLine(b.Data);
                };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.StandardInput.Write(code + $"\r\nprint('{guid}')\r\n");
                process.StandardInput.Flush();

                while (!finished)
                {
                    Thread.Sleep(250);
                    if (hasErr)
                    {
                        Thread.Sleep(250); // gather other errors
                        break;
                    }
                }

                process.CancelErrorRead();
                process.CancelOutputRead();

                if (hasErr) throw new PythonException($"{sberr.ToString()}", sbout.ToString());

                res = sbout.ToString();
            }

            sw.Stop();
            debug?.Invoke($"python took [{sw.Elapsed}]");

            return res;
        }

    }
    #endregion


    public class PythonException : Exception
    {

        public PythonException(string errmsg, string output) : base(errmsg)
        {
            Error = errmsg;
            Output = output;
        }

        public string Error { get; private set; }
        public string Output { get; private set; }

        public override string ToString()
        {
            return $"output [{Output}] error [{Error}]";
        }

    }

    public static partial class Extensions
    {

        static bool? _win32;
        internal static bool? win32
        {
            get
            {
                if (!_win32.HasValue)
                {
                    _win32 =
                        Environment.OSVersion.Platform != PlatformID.Unix;
                }
                return _win32.Value;
            }
        }

        /// <summary>
        /// split given string using backslash if larger than 8k for win32 platform
        /// </summary>        
        public static string PythonSafeString(this string large_string)
        {
            var split_size = PythonPipe.win32_string_len_safe;

            if (win32.Value && large_string.Length > split_size)
            {
                var sb = new StringBuilder();
                int i = 0;
                var len = large_string.Length;
                while (i < len)
                {
                    if (sb.Length > 0) sb.Append("\\\r\n");
                    var size = Min(split_size, len - i);
                    sb.Append(large_string.Substring(i, size));
                    i += split_size;
                }
                return sb.ToString();
            }
            else
                return large_string;
        }

    }

}
