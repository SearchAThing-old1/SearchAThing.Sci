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

using Novacode;
using SearchAThing.Sci;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

//
// extensions to the https://github.com/devel0/DocX
//

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// set margins to the given measure
        /// </summary>        
        public static void SetMargins(this DocX docx, Measure measure)
        {
            var margin_pts = docx.MeasureToPoints(measure);
            docx.MarginLeft = margin_pts;
            docx.MarginRight = margin_pts;
            docx.MarginTop = margin_pts;
            docx.MarginBottom = margin_pts;
        }

        /// <summary>
        /// convert given points to measure [inches] ( 1pt = 1/72 inch )
        /// </summary>        
        public static Measure PointsToMeasure(this DocX docx, int points)
        {
            return ((1.0 / 72.0) * MUCollection.Length.inch) * points;
        }

        /// <summary>
        /// convert given measure to points ( 1pt = 1/72 inch )
        /// </summary>        
        public static int MeasureToPoints(this DocX docx, Measure measure)
        {
            return (int)(measure.ConvertTo(MUCollection.Length.inch).Value * 72.0);
        }

        public static Picture SetSizeInches(this Picture pic, double wInch, double hInch)
        {
            pic.WidthInches = wInch;
            pic.HeightInches = hInch;

            return pic;
        }

        /// <summary>
        /// fit image 100% considering margin
        /// </summary>        
        public static Picture ImageFit(this DocX docx, string pathfilename, double factor = 1.0)
        {
            var page_width_in = docx.PointsToMeasure((int)docx.PageWidth).Value;
            var page_width_avail_in = page_width_in -
                docx.PointsToMeasure((int)docx.MarginLeft).Value -
                docx.PointsToMeasure((int)docx.MarginRight).Value;

            factor = factor * (page_width_avail_in / page_width_in);

            var img = docx.AddImage(pathfilename);
            var res = img.CreatePicture();
            var ratio_w_h = (double)res.Width / res.Height;            

            res.SetSizeInches(page_width_avail_in, page_width_avail_in / ratio_w_h);
            
            return res;
        }

    }

}
