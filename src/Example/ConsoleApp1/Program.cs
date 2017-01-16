using SearchAThing.Sci;
using SearchAThing;
using netDxf;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var dxf = new DxfDocument();

            var line = new Line3D(new Vector3D(0, 0), new Vector3D(100, 50));
            dxf.AddEntity(line.ToLine().SetColor(AciColor.Red));

            dxf.Save(@"output.dxf");
        }
    }
}
