using Xunit;

namespace SearchAThing.Sci.Tests
{
    public class Line3DTests
    {
        [Fact]
        public void Line3DTest()
        {
            var l = new Line3D(new Vector3D(1, 2, 3), new Vector3D(4, 5, 6));
            Assert.True(l.From.EqualsTol(1e-1, 1, 2, 3) && l.To.EqualsTol(1e-1, 4, 5, 6));
        }

        [Fact]
        public void Line3DTest1()
        {
            var l = new Line3D(new Vector3D(1, 2, 3), new Vector3D(4, 5, 6), Line3DConstructMode.PointAndVector);
            Assert.True(l.From.EqualsTol(1e-1, 1, 2, 3) && l.To.EqualsTol(1e-1, 1 + 4, 2 + 5, 3 + 6));
        }

        [Fact]
        public void Line3DTest2()
        {
            var l = new Line3D(1, 2, 3, 4);
            Assert.True(l.From.EqualsTol(1e-1, 1, 2, 0) && l.To.EqualsTol(1e-1, 3, 4, 0));
        }

        [Fact]
        public void Line3DTest3()
        {
            var l = new Line3D(1, 2, 3, 4, 5, 6);
            Assert.True(l.From.EqualsTol(1e-1, 1, 2, 3) && l.To.EqualsTol(1e-1, 4, 5, 6));
        }

        [Fact]
        public void EqualsTolTest()
        {
            var l = new Line3D(1, 2, 3, 4, 5, 6);
            Assert.True(l.EqualsTol(1e-1, new Line3D(new Vector3D(1, 2, 3), new Vector3D(4, 5, 6))));
        }

        [Fact]
        public void CommonPointTest()
        {
            var l = new Line3D(1, 2, 3, 4, 5, 6);
            var l2 = new Line3D(4, 5, 6, 7, 8, 9);
            Assert.True(l.CommonPoint(1e-1, l2).EqualsTol(1e-1, 4, 5, 6));
            var l3 = new Line3D(4.11, 5.11, 6.11, 7.11, 8.11, 9.11);
            // common point test only from,to
            Assert.True(l.CommonPoint(1e-1, l3) == null);
        }

        [Fact]
        public void ReverseTest()
        {
            var l = new Line3D(1, 2, 3, 4, 5, 6);
            var r = l.Reverse();
            Assert.True(r.From.EqualsTol(1e-1, 4, 5, 6) && r.To.EqualsTol(1e-1, 1, 2, 3));
        }

        [Fact]
        public void ScaleTest()
        {
            var l = new Line3D(0, 0, 0, 1, 2, 3);

            Assert.True(l.Scale(new Vector3D(0, 0, 0), .5).EqualsTol(1e-1,
                new Line3D(0, 0, 0, .5, 1, 1.5)));
            Assert.True(l.Scale(new Vector3D(0, 0, 0), -.5).EqualsTol(1e-1,
                new Line3D(0, 0, 0, -.5, -1, -1.5)));

            Assert.True(l.Scale(new Vector3D(.5, .5, .5), .5).EqualsTol(1e-1,
                new Line3D(.25, .25, .25, .75, 1.25, 1.75)));
            Assert.True(l.Scale(new Vector3D(.5, .5, .5), -.5).EqualsTol(1e-1,
                new Line3D(.75, .75, .75, .25, -.25, -.75)));
        }

        [Fact]
        public void LineContainsPointTest()
        {
            var l = new Line3D(1.1885, -.6908, 1.0009, 3.0186, 7.0544, 4.4160);
            var p = new Vector3D(2.1035, 3.1818, 2.7085);
            Assert.True(l.LineContainsPoint(1e-4, p.X, p.Y, p.Z));
            Assert.True(l.LineContainsPoint(1e-4, p));

            // line contains point consider infinite line
            p = new Vector3D(.2734, -4.5634, -.7066);
            Assert.True(l.LineContainsPoint(1e-4, p.X, p.Y, p.Z));
            Assert.True(l.LineContainsPoint(1e-4, p));
        }

        [Fact]
        public void SegmentContainsPointTest()
        {
            var l = new Line3D(1.1885, -.6908, 1.0009, 3.0186, 7.0544, 4.4160);
            var p = new Vector3D(2.1035, 3.1818, 2.7085);
            Assert.True(l.SegmentContainsPoint(1e-4, p.X, p.Y, p.Z));
            Assert.True(l.SegmentContainsPoint(1e-4, p));

            // line contains point consider infinite line
            p = new Vector3D(.2734, -4.5634, -.7066);
            Assert.False(l.SegmentContainsPoint(1e-4, p.X, p.Y, p.Z));
            Assert.False(l.SegmentContainsPoint(1e-4, p));
        }

        [Fact]
        public void IntersectTest()
        {
            {
                var l = new Line3D(0, 0, 0, 10, 0, 0);
                var l2 = new Line3D(5, 1e-1, 0, 5, 1e-1, 10); // vertical line dst=1e-1

                // default intersection behavior : midpoint
                var ip = l.Intersect(1e-1, l2);
                Assert.True(ip.EqualsTol(1e-2, l.Intersect(1e-1, l2, LineIntersectBehavior.MidPoint)));
                Assert.True(ip.EqualsTol(1e-2, 5, 1e-1 / 2, 0));

                ip = l.Intersect(1e-1, l2, LineIntersectBehavior.PointOnThis);
                Assert.True(ip.EqualsTol(1e-2, 5, 0, 0));

                ip = l.Intersect(1e-1, l2, LineIntersectBehavior.PointOnOther);
                Assert.True(ip.EqualsTol(1e-2, 5, 1e-1, 0));

                Assert.True(l.Intersect(5e-2, l2) == null);
            }

            {
                var l = new Line3D(0, 0, 0, 10, 20, 30);

                // cs around l=cs.z
                var cs = new CoordinateSystem3D(l.From, l.V, CoordinateSystem3DAutoEnum.AAA);

                // build a per line offsetted
                var lperp_off = new Line3D(l.MidPoint, cs.BaseX, Line3DConstructMode.PointAndVector)
                    .Move(cs.BaseY * 2e-1);

                var ip = l.Intersect(2e-1, lperp_off);
                Assert.True(ip.EqualsTol(1e-4, 4.9641, 9.9283, 15.0598));

                Assert.True(l.Intersect(1e-1, lperp_off) == null);
            }
        }

        [Fact]
        public void IntersectTest1()
        {

        }

        [Fact]
        public void PerpendicularTest()
        {

        }

        [Fact]
        public void PerpendicularToIntersectionTest()
        {

        }

        [Fact]
        public void ColinearTest()
        {

        }

        [Fact]
        public void IsParallelToTest()
        {

        }

        [Fact]
        public void IntersectTest2()
        {

        }

        [Fact]
        public void RotateAboutAxisTest()
        {

        }

        [Fact]
        public void SetLengthTest()
        {

        }

        [Fact]
        public void MoveTest()
        {

        }

        [Fact]
        public void MoveMidpointTest()
        {

        }

        [Fact]
        public void SplitTest()
        {

        }

        [Fact]
        public void EnsureFromTest()
        {

        }

        [Fact]
        public void ToStringTest()
        {

        }

        [Fact]
        public void ToStringTest1()
        {

        }

        [Fact]
        public void DivideTest()
        {

        }

        [Fact]
        public void CommonNodeTest()
        {

        }

        [Fact]
        public void BBoxTest()
        {

        }

        [Fact]
        public void BisectTest()
        {

        }
    }
}