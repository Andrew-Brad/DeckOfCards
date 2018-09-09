Echo "Updating Database."

#Capture necessary file paths in the project
$startupProject = '../../../Web/ApiKickstart.WebApi/' #relative to EF data project
$relativeDataDirectory = '/../src/Domain/ApiKickstart.DataModel/ApiKickstart.DataModel' #relative to scripts directory
$relativeStartupDirectory = '/../src/Web/ApiKickstart.WebApi/' #relative to scripts directory

#Prep environment variable since EF doesn't call Main() - more can be found at https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-2.1
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:ASPNETCORE_CONTENTROOT = Join-Path -Path $PSScriptRoot -ChildPath $relativeStartupDirectory -Resolve

$PSScriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
Set-Location $PSScriptRoot/../src/Domain/ApiKickstart.DataModel/ApiKickstart.DataModel
#Echo $PSScriptRoot
#For projects that aren't 'flat', a startup project isn't assumed by EF, and has to be specified, otherwise the IDesignTimeContext exception gets thrown
dotnet ef database update -s ../../../Web/ApiKickstart.WebApi/