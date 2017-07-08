@echo Off

echo "argument 1 = [%1]"

set config=%1
if "%config%" == "" (
   set config=Release
)

echo "config = [%config%]"

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

echo "version = [%version%]"

REM Build
"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" SearchAThing.Sci.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Unit tests
call %nuget% install xunit.runner.console -Version 2.2.0 -OutputDirectory packages
packages\xunit.runner.console\2.2.0\tools\xunit.console.exe test\bin\%config%\SearchAThing.SciTests.dll
if not "%errorlevel%"=="0" goto failure

REM Package
mkdir Build
call %nuget% pack "src\SearchAThing.Sci.csproj" -symbols -o Build -p Configuration=%config% %version%
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1