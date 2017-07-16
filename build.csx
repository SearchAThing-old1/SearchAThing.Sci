using System.Diagnostics;

var Configuration = System.Environment.GetEnvironmentVariable("Configuration");
var Targets = System.Environment.GetEnvironmentVariable("Targets");
var MsBuildExe = System.Environment.GetEnvironmentVariable("MsBuildExe");

if (string.IsNullOrEmpty(Configuration)) Configuration = "Release";
if (string.IsNullOrEmpty(Targets)) Targets = "Restore,Rebuild,Pack";
if (string.IsNullOrEmpty(MsBuildExe)) MsBuildExe = Path.Combine(System.Environment.GetEnvironmentVariable("ProgramFiles(x86)"), @"Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin\MSBuild.exe");

Action<string> hdr = (msg) =>
{
    Console.WriteLine($"------------------------------");
    Console.WriteLine($"   {msg}");
    Console.WriteLine($"------------------------------");
};

//-------------------------------BUILD
hdr("BUILD");

var psi = new ProcessStartInfo(MsBuildExe,
    $"SearchAThing.Sci.sln " +
    $"/t:{Targets} " +
    $"/p:Configuration={Configuration} " +
    $"/m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false");
psi.UseShellExecute = false;
var p = Process.Start(psi);
p.WaitForExit();

if (p.ExitCode != 0) System.Environment.Exit(p.ExitCode);
