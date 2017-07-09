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
set msbuild=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe

rem EDIT: set your msbuild for local debug
rem set msbuild=%ProgramFiles(x86)%\Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin\MSBuild.exe

echo "msbuild = [%msbuild%]"

if "%nuget%" == "" (
  set nuget="c:\nuget\nuget.exe"
)

REM Build
echo
echo "====================> Build"

call "%msbuild%" SearchAThing.Sci.sln /t:Restore,Rebuild /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Code Coverage
echo
echo "====================> Coverage"

call %nuget% install xunit.runner.console -Version 2.2.0 -OutputDirectory packages
if not "%errorlevel%"=="0" goto failure

call %nuget% install OpenCover -Version 4.6.519 -OutputDirectory packages
if not "%errorlevel%"=="0" goto failure

packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -target:"packages\xunit.runner.console.2.2.0\tools\xunit.console.exe" -targetargs:".\tests\bin\Release\SearchAThing.Sci.Tests.dll -noshadow" -output:".\coverage.xml"
rem if not "%errorlevel%"=="0" goto failure

echo "---> ensuring codecov"
rem call %nuget% install Codecov -Version 1.0.1 -OutputDirectory packages
rem if not "%errorlevel%"=="0" goto failure
npm install codecov -g > null

rem packages\Codecov.1.0.1\tools\codecov.exe -f coverage.xml -t %CODECOV_TOKEN%
echo "---> running codecov -f coverage.xml"
codecov -f coverage.xml
if not "%errorlevel%"=="0" goto failure

REM Package
echo
echo "====================> Package"

mkdir Build
call %nuget% pack "src\SearchAThing.Sci.csproj" -symbols -o Build -p Configuration=%config% %version%
if not "%errorlevel%"=="0" goto failure

REM EDIT: uncommen follows exit for local debug
rem goto debugend

:success
exit 0

:failure
exit -1

:debugend