using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchAThing.Sci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchAThing.Sci.Tests
{
    [TestClass()]
    public class Line3DTests
    {
        [TestMethod()]
        public void Line3DTest()
        {
            var l = new Line3D(new Vector3D(1, 2, 3), new Vector3D(4, 5, 6));
            Assert.IsTrue(l.From.EqualsTol(1e-1, 1, 2, 3));
            Assert.IsTrue(l.From.EqualsTol(1e-1, 4, 5, 6));
        }

        [TestMethod()]
        public void Line3DTest1()
        {
            var l = new Line3D(new Vector3D(1, 2, 3), new Vector3D(4, 5, 6), Line3DConstructMode.PointAndVector);
            Assert.IsTrue(l.From.EqualsTol(1e-1, 1, 2, 3));
            Assert.IsTrue(l.To.EqualsTol(1e-1, 1 + 4, 2 + 5, 3 + 6));
        }

        [TestMethod()]
        public void Line3DTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void Line3DTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EqualsTolTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CommonPointTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReverseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ScaleTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LineContainsPointTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void LineContainsPointTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SegmentContainsPointTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SegmentContainsPointTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IntersectTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IntersectTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PerpendicularTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PerpendicularToIntersectionTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ColinearTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IsParallelToTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void IntersectTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void RotateAboutAxisTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SetLengthTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MoveTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MoveMidpointTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SplitTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void EnsureFromTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToStringTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DivideTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CommonNodeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BBoxTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void BisectTest()
        {
            Assert.Fail();
        }
    }
}