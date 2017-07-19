using Xunit;
using System.Linq;

namespace SearchAThing.Sci.Tests
{
    public class Vector3DTests
    {

        double rad_tol;

        public Vector3DTests()
        {
            rad_tol = (1e-1).ToRad();
        }

        [Fact]
        public void AngleRadTest()
        {
            var v1 = new Vector3D(10, 0, 0);
            var v2 = new Vector3D(2, 5, 0);
            var angv1v2 = v1.AngleRad(1e-4, v2);
            var angv2v1 = v2.AngleRad(1e-4, v1);
            Assert.True(angv1v2.EqualsTol(rad_tol, angv2v1));
            Assert.True(angv1v2.EqualsTol(rad_tol, 68.2d.ToRad()));
        }

        [Fact]
        public void AngleTowardTest()
        {
            var v1 = new Vector3D(10, 0, 0);
            var v2 = new Vector3D(2, 5, 0);

            var angv1v2_zplus = v1.AngleToward(1e-4, v2, Vector3D.ZAxis);
            var angv1v2_zminus = v1.AngleToward(1e-4, v2, -Vector3D.ZAxis);

            var angv2v1_zplus = v2.AngleToward(1e-4, v1, Vector3D.ZAxis);
            var angv2v1_zminus = v2.AngleToward(1e-4, v1, -Vector3D.ZAxis);

            Assert.True(angv1v2_zplus.EqualsTol(rad_tol, angv2v1_zminus));
            Assert.True(angv1v2_zplus.EqualsTol(rad_tol, 68.1d.ToRad()));

            Assert.True(angv2v1_zplus.EqualsTol(rad_tol, angv1v2_zminus));
            Assert.True(angv2v1_zplus.EqualsTol(rad_tol, 291.8d.ToRad()));
        }

        [Fact]
        public void AxisTest()
        {

        }

        [Fact]
        public void BBoxTest()
        {

        }

        [Fact]
        public void ColinearTest()
        {

        }

        [Fact]
        public void ConcordantTest()
        {

        }

        [Fact]
        public void ConvertTest()
        {

        }

        [Fact]
        public void CrossProductTest()
        {

        }

        [Fact]
        public void DistanceTest()
        {

        }

        [Fact]
        public void Distance2DTest()
        {

        }

        [Fact]
        public void DivideTest()
        {

        }

        [Fact]
        public void DotProductTest()
        {

        }

        [Fact]
        public void EqualsTol1Test()
        {

        }

        [Fact]
        public void EqualsTol2Test()
        {

        }

        [Fact]
        public void EqualsTol3Test()
        {

        }

        [Fact]
        public void From2DCoordsTest()
        {

        }

        [Fact]
        public void From3DCoordsTest()
        {

        }

        [Fact]
        public void FromStringTest()
        {

        }

        [Fact]
        public void FromStringArrayTest()
        {

        }

        [Fact]
        public void GetOrdTest()
        {

        }

        [Fact]
        public void IsParallelToTest()
        {

        }

        [Fact]
        public void IsPerpendicularTest()
        {

        }

        [Fact]
        public void MirrorTest()
        {

        }

        [Fact]
        public void NormalizedTest()
        {

        }

        [Fact]
        public void Project1Test()
        {

        }

        [Fact]
        public void Project2Test()
        {

        }

        [Fact]
        public void Random1Test()
        {

        }

        [Fact]
        public void Random2Test()
        {

        }

        [Fact]
        public void RelTest()
        {

        }

        [Fact]
        public void RotateAboutAxis1Test()
        {

        }

        [Fact]
        public void RotateAboutAxis2Test()
        {

        }

        [Fact]
        public void RotateAboutXAxisTest()
        {

        }

        [Fact]
        public void RotateAboutYAxisTest()
        {

        }

        [Fact]
        public void RotateAboutZAxisTest()
        {

        }

        [Fact]
        public void RotateAsTest()
        {

        }

        [Fact]
        public void ScalarTest()
        {

        }

        [Fact]
        public void ScaleAbout1Test()
        {

        }

        [Fact]
        public void ScaleAbout2Test()
        {

        }

        [Fact]
        public void SetTest()
        {

        }

        [Fact]
        public void StringRepresentationTest()
        {

        }

        [Fact]
        public void ToString1Test()
        {

        }

        [Fact]
        public void ToString2Test()
        {

        }

        [Fact]
        public void ToString3Test()
        {

        }

        [Fact]
        public void ToSystemVector3DTest()
        {

        }

        [Fact]
        public void ToUCSTest()
        {

        }

        [Fact]
        public void ToWCSTest()
        {

        }

        [Fact]
        public void Vector3D1Test()
        {

        }

        [Fact]
        public void Vector3D2Test()
        {

        }

        [Fact]
        public void Vector3D3Test()
        {

        }

        [Fact]
        public void Vector3D4Test()
        {

        }

        [Fact]
        public void OperatorSub1Test()
        {

        }

        [Fact]
        public void OperatorSub2Test()
        {

        }

        [Fact]
        public void OperatorScalarMul1Test()
        {

        }

        [Fact]
        public void OperatorScalarMul2Test()
        {

        }

        [Fact]
        public void OperatorMulTest()
        {

        }

        [Fact]
        public void OperatorDivide1Test()
        {

        }

        [Fact]
        public void OperatorDivide2Test()
        {

        }

        [Fact]
        public void OperatorSumTest()
        {

        }

    }

}
