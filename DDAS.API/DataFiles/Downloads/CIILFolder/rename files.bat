@echo off
for %%F in (*.zip) do (
  set "name=%%~nF"
  call ren "%%F" "ClinicalInvestigatorInspectionPage%%name%%:~7%.zip")