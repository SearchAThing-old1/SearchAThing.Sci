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

using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace SearchAThing.Sci
{

    public static partial class Extensions
    {

        /// <summary>
        /// start excel (interop mode) to execute a bunch of calc
        /// - the excel need to be enabled to load macro ( eg. Developer tab / Macro Security / Trust access to VBA project object model )
        /// - module_name is the name where it place the macros
        /// - input_csv_pathfilename is the pathfilename of the csv to load data from
        /// - output_csv_pathfilename is the pathfilename where output results are places
        /// 
        /// example input_csv
        /// cells_to_write,INPUT,H8,I8,J8
        /// cells_to_read,OUTPUT,D109,D110
        /// input_data_set_follow        
        /// 1;2;3
        /// 4;5;6
        /// 7;8;9
        /// 1;2;3        
        /// 
        /// this will compute 4 times the worksheet by setting values in INPUT sheet cells "H8,I8,J8"
        /// and reading back results from the OUTPUT sheet cells "D109,D110" that will be written in the `output_csv_pathfilename`
        /// </summary>        
        public static void AutomaticCalc(this Application app, Workbook wb,
            string module_name,
            string input_csv_pathfilename,
            string output_csv_pathfilename)
        {
            Microsoft.Vbe.Interop.VBComponent module = null;

            // check if module already exists
            var module_cnt = wb.VBProject.VBComponents.Count;
            for (int i = 1; i <= module_cnt; ++i)
            {
                var _module = wb.VBProject.VBComponents.Item(i);
                if (_module.Name == module_name)
                {
                    module = _module;
                    break;
                }
            }

            // if not exists, create with AutomaticCalc code
            if (module == null)
            {
                module = wb.VBProject.VBComponents.Add(Microsoft.Vbe.Interop.vbext_ComponentType.vbext_ct_StdModule);
                module.Name = module_name;
                var ass = Assembly.GetExecutingAssembly();
                var vba_code = ass.GetResourceTextFile("SearchAThing.Sci.excel_interop_automatic_calc.vba");
                module.CodeModule.AddFromString(vba_code);
            }            

            app.Run($"{module_name}.AutomaticCalc", input_csv_pathfilename, output_csv_pathfilename);            
        }

    }

}
