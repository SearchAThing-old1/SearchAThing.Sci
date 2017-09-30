﻿#region SearchAThing.Sci, Copyright(C) 2016-2017 Lorenzo Delana, License under MIT
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
using SearchAThing.Sci;
using System.Linq;
using SearchAThing;
using System;
using static System.Math;

namespace SearchAThing
{

    public static partial class Extensions
    {

        internal class BPolyPoint
        {
            public Vector3D point;
            /// <summary>
            /// distance from bbox side
            /// </summary>
            public double elevation;

            public override string ToString()
            {
                return $"{point} H={elevation}";
            }
        }

        /// <summary>
        /// compute bounding polygon
        /// </summary>        
        public static IReadOnlyList<Vector3D> BPoly(this IEnumerable<Vector3D> pts, double tol, netDxf.DxfDocument debug = null)
        {
            if (pts.Any(w => !w.Z.EqualsTol(tol, 0))) throw new System.Exception($"Z must zero for bounding poly");

            var pts_count = pts.Count();
            if (pts_count < 3) throw new System.Exception($"need at least 3 pts for bounding poly");
            if (pts_count == 3) return pts.ToList();

            var res = new List<Vector3D>();

            var bbox = pts.BBox();
            var bbox_perimeter = bbox.Coords2D.ToList().PolygonSegments(tol).Select(l => new
            {
                seg = l,
                is_vertical = l.V.X.EqualsTol(tol, 0),
                pts = new List<BPolyPoint>(),
                bounding_pts = new List<Vector3D>()
            }).ToList();

            // assign pts to bbox sides
            foreach (var p in pts)
            {
                // add pt to owner (nearest) bbox side and store elevation as bbox side distance
                bbox_perimeter.Select(w => new
                {
                    bbox_seg = w,
                    dst = p.Function((v) =>
                    {
                        if (w.is_vertical)
                            return Abs(v.X - w.seg.From.X);
                        else
                            return Abs(v.Y - w.seg.From.Y);
                    })
                })
                .OrderBy(w => w.dst).First().Action((bs) =>
                {
                    bs.bbox_seg.pts.Add(new BPolyPoint()
                    {
                        point = p,
                        elevation = bs.dst
                    });
                });

                foreach (var bp in bbox_perimeter)
                {
                    if (bp.seg.SegmentContainsPoint(tol, p) && !bp.pts.Any(w => w.point.EqualsTol(tol, p))) bp.pts.Add(new BPolyPoint()
                    {
                        point = p,
                        elevation = 0
                    });
                }
            }

            // process each bbox side
            foreach (var bs in bbox_perimeter)
            {
                var ord = bs.is_vertical ? OrdIdx.Y : OrdIdx.X;

                // start from side perimeter points
                var ordered_side_pts = bs.pts.Where(r => r.elevation.EqualsTol(tol, 0))
                    .OrderBy(w => w.point.GetOrd(ord)).ToList();

                Action<
                    Func<double, double, bool>,
                    Action<BPolyPoint>,
                    Func<IEnumerable<BPolyPoint>, BPolyPoint>> process_side =
                    (compare_off, list_add_element, ordered_list_get_pivot) =>
                {
                    while (true)
                    {
                        var pivot = ordered_list_get_pivot(ordered_side_pts);
                        var pivot_off = pivot.point.GetOrd(ord);
                        var predecessors = bs.pts.Where(r => compare_off(r.point.GetOrd(ord), pivot_off))
                            .OrderBy(w => w.point.GetOrd(ord)).ToList();
                        if (predecessors.Count == 0) break;

                        // sort and compute offset from pivot
                        var q = predecessors.Select(dp => new
                        {
                            dp = dp,
                            off = Abs(dp.point.GetOrd(ord) - pivot_off)
                        }).ToList();

                        // retrieve winner
                        var qwin = q.OrderBy(w => Abs(w.dp.elevation - pivot.elevation) / Abs(w.off - pivot_off)).First();

                        // update ordered side pts
                        list_add_element(qwin.dp);
                    }
                };

                // predecessors
                process_side(
                    // compare_off
                    (a, b) => a.LessThanTol(tol, b),
                    // list_add_element
                    (dp) => ordered_side_pts.Insert(0, dp),
                    // ordered_list_get_pivot)
                    (olst) => olst.First());

                // successors
                process_side(
                    // compare_off
                    (a, b) => a.GreatThanTol(tol, b),
                    // list_add_element
                    (dp) => ordered_side_pts.Add(dp),
                    // ordered_list_get_pivot)
                    (olst) => olst.Last());

                // transfer processed winner points to the side contanier
                bs.bounding_pts.AddRange(ordered_side_pts.Select(w => w.point));
            }

            // glue sides            
            var previous_bbox_seg = bbox_perimeter.First();

            foreach (var bp in bbox_perimeter)
            {
                if (debug != null)
                {
                    bp.bounding_pts.Segments(tol).Foreach(w => debug.AddEntity(w, new netDxf.Tables.Layer("debug")));
                }

                var bpts = bp.bounding_pts;
                if (bpts.Count == 0) throw new Exception("invalid bounding side pts count=0");

                var bpts_step = 1;
                var bpts_begin = 0;
                var bpts_end = bpts.Count - 1;

                if (res.Count > 0)
                {
                    var bbox_seg_cp = previous_bbox_seg.seg.CommonPoint(tol, bp.seg);

                    if (bbox_seg_cp.Distance(bpts[bpts_begin]).GreatThanTol(tol, bbox_seg_cp.Distance(bpts[bpts_end])))
                    {
                        bpts_step = -1;
                        bpts_begin = bpts.Count - 1;
                        bpts_end = 0;
                    }
                }

                var i = bpts_begin;
                while (true)
                {
                    // skip, if any, initial side point if already present in res
                    if (i == bpts_begin && res.Count > 0 && res.Last().EqualsTol(tol, bpts[i]))
                    {
                        if (i == bpts_end) break;
                        i += bpts_step;
                        continue;
                    }

                    res.Add(bpts[i]);

                    if (i == bpts_end) break;
                    i += bpts_step;
                }

                previous_bbox_seg = bp;
            }

            return res;
        }

    }

}
