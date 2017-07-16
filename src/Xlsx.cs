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

using System.Collections.Generic;
using System.Text;
using System.Linq;
using ClosedXML.Excel;
using System.Dynamic;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// Parse xlsx sheet data into a list of dynamic objects
        /// it will search on given sheetname ( it not null ) or on any sheets ( if null )
        /// it will list for given columnNames ( if not null ) or for all columns ( if null )
        /// if columnNamesIgnoreCase result object will contains lowercase properties
        /// </summary>        
        public static IEnumerable<ImportXlsxDataSheet> ParseXlsxData(this string xlsxPathfilename,
            string _sheetName = null,
            bool sheetNameIgnoreCase = true,
            HashSet<string> columnNames = null,
            bool columnNamesIgnoreCase = true,
            string[] valid_sheetnames = null)
        {
            var wb = new XLWorkbook(xlsxPathfilename);
            string sheetName = _sheetName;

            IXLWorksheet ws = null;

            if (sheetName == null)
            {
                foreach (var _ws in wb.Worksheets)
                {
                    if (valid_sheetnames != null && !valid_sheetnames.Any(r => r.ToLower() == _ws.Name.ToLower())) continue;

                    yield return new ImportXlsxDataSheet(_ws.Name, _ws.ParseXlsxData(columnNames, columnNamesIgnoreCase));
                }
            }
            else
            {
                if (sheetNameIgnoreCase)
                {
                    sheetName = _sheetName.ToLower();
                    ws = wb.Worksheets.FirstOrDefault(w => w.Name.ToLower() == sheetName);
                }
                else
                    ws = wb.Worksheets.FirstOrDefault(w => w.Name == sheetName);

                yield return new ImportXlsxDataSheet(sheetName, ws.ParseXlsxData(columnNames, columnNamesIgnoreCase));
            }
        }

        /// <summary>
        /// Parse xlsx sheet data into a list of dynamic objects
        /// it will list for given columnNames ( if not null ) or for all columns ( if null )
        /// if columnNamesIgnoreCase result object will contains lowercase properties
        /// </summary>        
        public static List<dynamic> ParseXlsxData(this IXLWorksheet ws, HashSet<string> _columnNames = null, bool columnNamesIgnoreCase = true)
        {
            HashSet<string> columnNames = null;

            if (columnNamesIgnoreCase && _columnNames != null)
                columnNames = _columnNames.Select(w => w.ToLower()).ToHashSet();
            else
                columnNames = _columnNames;

            var res = new List<dynamic>();

            var columnDict = new Dictionary<string, int>();

            var row = ws.FirstRow();

            var lastCol = row.LastCellUsed().Address.ColumnNumber;

            for (int ci = 1; ci <= lastCol; ++ci)
            {
                var cname = (string)row.Cell(ci).Value;
                if (string.IsNullOrEmpty((string)cname)) continue;

                if (columnNamesIgnoreCase) cname = cname.ToLower();

                if (columnNames == null || columnNames.Contains(cname))
                {
                    columnDict.Add(cname, ci);
                }
            }

            var lastRow = ws.LastRowUsed().RowNumber();

            for (int ri = 2; ri <= lastRow; ++ri)
            {
                row = ws.Row(ri);

                IDictionary<string, object> eo = new ExpandoObject();

                foreach (var c in columnDict)
                {
                    var cell = row.Cell(c.Value);

                    eo.Add(c.Key, cell.Value);
                }

                res.Add(eo);
            }

            return res;
        }

    }

    public class ImportXlsxDataSheet
    {

        public string SheetName { get; private set; }
        public IEnumerable<dynamic> Rows { get; private set; }

        public ImportXlsxDataSheet(string sheetName, IEnumerable<dynamic> rows)
        {
            SheetName = sheetName;
            Rows = rows;
        }

    }

}