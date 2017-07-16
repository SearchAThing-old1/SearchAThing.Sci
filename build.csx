using System.Diagnostics;

var Configuration = System.Environment.GetEnvironmentVariable("Configuration");
var Targets = System.Environment.GetEnvironmentVariable("Targets");
var MsBuildExe = System.Environment.GetEnvironmentVariable("MsBuildExe");

// EDIT : change to true for local debug
var local_mode = false;

var msbuild_cmd = local_mode ?
Path.Combine(System.Environment.GetEnvironmentVariable("ProgramFiles(x86)"), @"Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin\MSBuild.exe")
: MsBuildExe;

Action<string> hdr = (msg) =>
{
    Console.WriteLine($"------------------------------");
    Console.WriteLine($"   {msg}");
    Console.WriteLine($"------------------------------");
};

//-------------------------------BUILD
hdr("BUILD");

var cmd = new ProcessStartInfo(msbuild_cmd,
    $"SearchAThing.Sci.sln " +
    $"/t:{Targets} " +
    $"/p:Configuration={Configuration} " +
    $"/m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false");
var p = Process.Start(cmd);
p.WaitForExit();
if (p.ExitCode != 0) System.Environment.Exit(p.ExitCode);

