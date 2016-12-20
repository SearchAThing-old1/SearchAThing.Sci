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

using Novacode;
using SearchAThing.Sci;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

//
// extensions to the http://docx.codeplex.com/
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

            Picture res = null;

            using (var ms = new MemoryStream())
            {
                var _img = System.Drawing.Image.FromFile(pathfilename);

                var horiz_scale = 96f / _img.HorizontalResolution;
                var vert_scale = 96f / _img.VerticalResolution;

                var resized_img = new Bitmap(_img, new Size((int)(_img.Width * horiz_scale), (int)(_img.Height * vert_scale)));
                resized_img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                var img = docx.AddImage(ms);
                res = img.CreatePicture();

                res.Width = (int)(horiz_scale * factor * _img.Width);
                res.Height = (int)(vert_scale * factor * _img.Height);
            }

            return res;
        }

    }

}
