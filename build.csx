using System.Diagnostics;

var Configuration = System.Environment.GetEnvironmentVariable("Configuration");
var Targets = System.Environment.GetEnvironmentVariable("Targets");
var MsBuildExe = System.Environment.GetEnvironmentVariable("MsBuildExe");
var NuGet = System.Environment.GetEnvironmentVariable("NuGet");

var testFramework = "net462";

if (string.IsNullOrEmpty(Configuration)) Configuration = "Release";
if (string.IsNullOrEmpty(Targets)) Targets = "Restore,Rebuild";
if (string.IsNullOrEmpty(MsBuildExe)) MsBuildExe = Path.Combine(System.Environment.GetEnvironmentVariable("ProgramFiles(x86)"), @"Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin\MSBuild.exe");
if (string.IsNullOrEmpty(NuGet)) NuGet = @"c:\nuget\nuget.exe";

Action<string> hdr = (msg) =>
{
    Console.WriteLine($"------------------------------");
    Console.WriteLine($"   {msg}");
    Console.WriteLine($"------------------------------");
};

Action<string, string> run = (file, args) =>
{
    var psi = new ProcessStartInfo()
    {
        FileName = file,
        Arguments = args,
        UseShellExecute = false
    };
    var p = Process.Start(psi);
    p.WaitForExit();
    if (p.ExitCode != 0) System.Environment.Exit(p.ExitCode);
};

//-------------------------------BUILD
{
    hdr("BUILD");

    //
    // msbuild targets
    //
    run(MsBuildExe, $"SearchAThing.Sci.sln " +
        $"/t:{Targets} " +
        $"/p:Configuration={Configuration} " +
        $"/m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false");
}

//-------------------------------COVERAGE
{
    hdr("COVERAGE");

    //
    // install xunit.runner.console
    //
    run(NuGet, "install xunit.runner.console -Version 2.2.0 -OutputDirectory packages");

    //
    // install opencover
    //
    run(NuGet, "install OpenCover -Version 4.6.519 -OutputDirectory packages");

    //
    // run codecover
    //
    run(@"packages\OpenCover.4.6.519\tools\OpenCover.Console.exe",
        @"-register:user -target:packages\xunit.runner.console.2.2.0\tools\xunit.console.exe " +
        $@"-targetargs:"".\tests\bin\Release\{testFramework}\SearchAThing.Sci.Tests.dll -noshadow"" " +
        @"-output:.\coverage.xml ""-filter:+[*]* -[*]Microsoft.Xna.*"" ");

    Console.WriteLine("codecov");

    //
    // ensure codecov
    //
    run("npm", "install codecov -g");

    //
    // run codecov
    //
    run("codecov", "-f coverage.xml");

}