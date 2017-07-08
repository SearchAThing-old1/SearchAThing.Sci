using Xunit;

namespace SearchAThing.Sci.Tests
{
    public class Line3DTests
    {
        [Fact]
        public void Line3DTest()
        {
            var l = new Line3D(new Vector3D(1, 2, 3), new Vector3D(4, 5, 6));
            Assert.True(l.From.EqualsTol(1e-1, 1, 2, 3));
            Assert.True(l.To.EqualsTol(1e-1, 4, 5, 6));
        }

        [Fact]
        public void Line3DTest1()
        {
            var l = new Line3D(new Vector3D(1, 2, 3), new Vector3D(4, 5, 6), Line3DConstructMode.PointAndVector);
            Assert.True(l.From.EqualsTol(1e-1, 1, 2, 3));
            Assert.True(l.To.EqualsTol(1e-1, 1 + 4, 2 + 5, 3 + 6));
        }

        [Fact]
        public void Line3DTest2()
        {
            var l = new Line3D(1, 2, 3, 4);
            Assert.True(l.From.EqualsTol(1e-1, 1, 2, 0));
            Assert.True(l.To.EqualsTol(1e-1, 3, 4, 0));
        }

        [Fact]
        public void Line3DTest3()
        {
            var l = new Line3D(1, 2, 3, 4, 5, 6);
            Assert.True(l.From.EqualsTol(1e-1, 1, 2, 3));
            Assert.True(l.To.EqualsTol(1e-1, 4, 5, 6));
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
            
        }

        [Fact]
        public void ReverseTest()
        {
            
        }

        [Fact]
        public void ScaleTest()
        {
            
        }

        [Fact]
        public void LineContainsPointTest()
        {
            
        }

        [Fact]
        public void LineContainsPointTest1()
        {
             
        }

        [Fact]
        public void SegmentContainsPointTest()
        {
             
        }

        [Fact]
        public void SegmentContainsPointTest1()
        {
             
        }

        [Fact]
        public void IntersectTest()
        {
             
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