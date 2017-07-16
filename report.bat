@echo Off

"c:\nuget\nuget.exe" install ReportGenerator

c:\nuget-packages\ReportGenerator.2.5.10\tools\ReportGenerator.exe -reports:coverage.xml -targetdir:report