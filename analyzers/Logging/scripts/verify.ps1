param(
    [string]$Prefix="0.1.0",
    [string]$Suffix="internal-$([DateTime]::Now.ToString('yyyyMMddHHmmss'))"
)


$tempDir = Join-Path $env:TEMP $(New-Guid) | %{ mkdir $_ }
try
{
    Push-Location $tempDir
    dotnet new sln -n "TestSolution"
    dotnet new classlib -n "TestProject"
    dotnet sln add TestProject
    dotnet new nugetconfig
    mkdir libs
    dotnet nuget add source "./libs" --name "local.dir"
    $nupkgScript = Resolve-Path "$PSScriptRoot/build-nupkg.ps1"
    & $nupkgScript -OutDir "$tempDir/libs" -Prefix "$Prefix" -Suffix "$Suffix"
    dotnet add TestProject package "Logging" --prerelease
    dotnet build "./TestSolution.sln"
    explorer $tempDir
}
finally
{
    Pop-Location
}