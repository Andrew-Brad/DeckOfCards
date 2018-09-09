#Before running this script, change the migration name parameter:
$migrationName = 'WidgetClassificationWithValueConversion'

#Capture necessary file paths in the project
$startupProject = '../../../Web/ApiKickstart.WebApi/' #relative to EF data project
$relativeDataDirectory = '/../src/Domain/ApiKickstart.DataModel/ApiKickstart.DataModel' #relative to scripts directory
$relativeStartupDirectory = '/../src/Web/ApiKickstart.WebApi/' #relative to scripts directory

#Capture where this script is running
$PSScriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition

#Prep environment variable since EF doesn't call Main() - more can be found at https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-2.1
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:ASPNETCORE_CONTENTROOT = Join-Path -Path $PSScriptRoot -ChildPath $relativeStartupDirectory -Resolve

#Changing to data project directory for dotnet ef CLI
Set-Location $PSScriptRoot$relativeDataDirectory

#For projects that aren't 'flat', a startup project isn't assumed by EF, and has to be specified, otherwise the IDesignTimeContext exception gets thrown
dotnet ef migrations add $migrationName --startup-project $startupProject

#These lines below are for auto generating the associated sql script for the given variables
$migrationsList = dotnet ef migrations list --startup-project $startupProject

#echo "Last migration:" $migrationsList[$migrationsList.Length - 1]
dotnet ef migrations script $migrationsList[$migrationsList.Length - 2] $migrationsList[$migrationsList.Length - 1] --startup-project $startupProject --idempotent --output .\Migrations\$migrationName.sql
#dotnet ef migrations script <from_migration> <to_migration> -s ../../../Web/ApiKickstart.WebApi/ --idempotent

