using System.Diagnostics;

var Configuration = System.Environment.GetEnvironmentVariable("Configuration");
var Targets = System.Environment.GetEnvironmentVariable("Targets");
var MsBuildExe = System.Environment.GetEnvironmentVariable("MsBuildExe");
var NuGet = System.Environment.GetEnvironmentVariable("NuGet");

var testFramework = "net461";

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

Action<bool, string, string> run = (shell, file, args) =>
{
    ProcessStartInfo psi = null;
    if (shell)
    {
        psi = new ProcessStartInfo()
        {
            FileName = $"cmd",
            Arguments = $"/c {file} {args}",
            UseShellExecute = false            
        };
    }
    else
    {
        psi = new ProcessStartInfo()
        {
            FileName = file,
            Arguments = args,
            UseShellExecute = false         
        };
    }
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
    foreach (var prj in new[] { "src/SearchAThing.Sci.csproj", "tests/SearchAThing.Sci.Tests.csproj" })
    {
        run(false, MsBuildExe, $"{prj} " +
            $"/t:{Targets} " +
            $"/p:Configuration={Configuration} " +
            $"/m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false");
    }
}

//-------------------------------UNIT TEST
{
    hdr("UNIT TEST");

    //
    // install xunit.runner.console
    //
    run(false, NuGet, "install xunit.runner.console -Version 2.2.0 -OutputDirectory packages");

    //
    // install opencover
    //
    run(false, NuGet, "install OpenCover -Version 4.6.519 -OutputDirectory packages");

    //
    // run codecover
    //
    run(false, @"packages\OpenCover.4.6.519\tools\OpenCover.Console.exe",
        @"-register:user -target:""packages\xunit.runner.console.2.2.0\tools\xunit.console.exe"" " +
        $@"-targetargs:"".\tests\bin\Release\{testFramework}\SearchAThing.Sci.Tests.dll -noshadow"" " +
        @"-output:"".\coverage.xml"" " +
        "\"-filter:+[*]* -[*]Microsoft.Xna.*\"");
}

//-------------------------------COVERAGE
{    
    hdr("COVERAGE");

    //
    // ensure codecov
    //
    run(true, "npm", "install codecov -g");

    //
    // run codecov
    //
    run(true, "codecov", "-f coverage.xml");

}