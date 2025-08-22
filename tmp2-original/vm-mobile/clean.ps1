Get-ChildItem -Path . -Recurse -Directory -Filter bin | Remove-Item -Recurse -Force
Get-ChildItem -Path . -Recurse -Directory -Filter obj | Remove-Item -Recurse -Force

Stop-Process -Name devenv -Force -ErrorAction SilentlyContinue
# get current directory to change back to it later
$startDir = Get-Location
cd src/VisitTracker
dotnet nuget locals all --clear
dotnet workload restore
dotnet restore
# change back to the original directory
cd $startDir