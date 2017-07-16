call %NuGet% install scriptcs -Version 0.17.1 -Source https://www.myget.org/F/scriptcsnightly/api/v3/index.json

packages\scriptcs.0.17.1\tools\scriptcs.exe -V

scriptcs build.csx
