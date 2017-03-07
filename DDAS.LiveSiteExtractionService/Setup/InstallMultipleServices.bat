REM Run as Administrator
REM c:\windows\microsoft.net\framework\v4.0.30319\installutil.exe C:\Development\p926-ddas-admin\DDAS.LiveSiteExtractionService\bin\Debug\DDAS.LiveSiteExtractionService.exe
SC Create Service1  binpath=C:\Development\p926-ddas-admin\DDAS.LiveSiteExtractionService\bin\Debug\DDAS.LiveSiteExtractionService.exe
SC Create Service2  binpath=C:\Development\p926-ddas-admin\DDAS.LiveSiteExtractionService\bin\Debug\DDAS.LiveSiteExtractionService.exe
cmd /k
