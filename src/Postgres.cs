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
using Npgsql;
using ClosedXML.Excel;
using CsvHelper;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// create an xlsx representation of a select
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="query"></param>
        /// <param name="reprocess_header">if not null allow you to transform header</param>
        /// <param name="convert_data">if not null allow you to custom parse to a type (int,double,datetime, etc ). for not processed header just return the functor argument string</param>
        /// <param name="header_width">if not null allow you to customize column width</param>
        /// <param name="auto_header_width">if not null allow you to autoset header width</param>
        /// <param name="autofilter">if sets to true allow you to enable the column autofilter</param>
        /// <param name="block_header">if sets to true allow you to block the header</param>
        /// <param name="bold_header">if sets to true enables columns header to be bold</param>        
        public static XLWorkbook ToXlsx(this NpgsqlConnection conn,
            string query,
            Func<string, string> reprocess_header = null,
            Func<string, Func<string, object>> convert_data = null,
            Func<string, double?> header_width = null,
            Func<string, bool> auto_header_width = null,
            bool autofilter = true,
            bool block_header = true,
            bool bold_header = true)
        {
            var wb = new XLWorkbook();

            IXLWorksheet ws = null;

            string[] header = null;

            using (var tr = conn.BeginTextExport($"COPY ({query}) TO STDOUT WITH DELIMITER ',' CSV HEADER"))
            {
                using (var csv = new CsvParser(tr, new CsvHelper.Configuration.CsvConfiguration() { Delimiter = "," }))
                {
                    header = csv.Read();
                    if (header == null) return null;

                    var rowdata = csv.Read();
                    if (rowdata == null) return null;

                    ws = wb.Worksheets.Add("data");

                    IXLCell cell = null;

                    var i_convert_data = new List<Func<string, object>>();

                    var row = 1;
                    var col = 1;
                    for (int i = 0; i < header.Length; ++i)
                    {
                        if (convert_data != null)
                            i_convert_data.Add(convert_data(header[i]));
                        else
                            i_convert_data.Add((s) => s);

                        cell = ws.Cell(row, col);
                        if (reprocess_header != null)
                            cell.Value = reprocess_header(header[i]);
                        else
                            cell.Value = header[i];

                        ++col;
                    }

                    ++row;
                    do
                    {
                        col = 1;
                        for (int i = 0; i < header.Length; ++i)
                        {
                            cell = ws.Cell(row, col);
                            cell.Value = i_convert_data[i](rowdata[i]);
                            ++col;
                        }

                        ++row;
                        rowdata = csv.Read();
                    } while (rowdata != null);
                }

            }

            for (int i = 0; i < header.Length; ++i)
            {
                var col = i + 1;

                if (header_width != null)
                {
                    var q = header_width(header[i]);
                    if (q != null) ws.Column(col).Width = q.Value;
                }
                if (auto_header_width != null)
                {
                    if (auto_header_width(header[i])) ws.Column(col).AdjustToContents();
                }
                if (bold_header) ws.Cell(1, col).Style.Font.Bold = true;
            }

            if (block_header) ws.SheetView.Freeze(1, 0);

            if (autofilter) ws.RangeUsed().SetAutoFilter();

            return wb;
        }

    }

}

