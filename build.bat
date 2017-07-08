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

REM variables
rem EDIT: set your msbuild for local debug
rem set msbuild="c:\Program Files (x86)\Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin\MSBuild.exe"
set msbuild="%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"

if "%nuget%" == "" (
  set nuget="c:\nuget\nuget.exe"
)

REM Build
echo
echo "---> Build"

%msbuild% SearchAThing.Sci.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Unit tests
echo
echo "---> Unit tests"

call %nuget% install xunit.runner.console -Version 2.2.0 -OutputDirectory packages
packages\xunit.runner.console.2.2.0\tools\xunit.console.exe tests\bin\%config%\SearchAThing.Sci.Tests.dll
if not "%errorlevel%"=="0" goto failure

REM Package
echo
echo "---> Package"

mkdir Build
call %nuget% pack "src\SearchAThing.Sci.csproj" -symbols -o Build -p Configuration=%config% %version%
if not "%errorlevel%"=="0" goto failure

REM EDIT: commen follows exit for local debug

:success
exit 0

:failure
exit -1