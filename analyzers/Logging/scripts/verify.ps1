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
    $nupkgName = Get-ChildItem "*.nupkg" -Path .\libs\ `
        | Select-Object -ExpandProperty Name `
        | Select-String -Pattern "^.+(?=\.\d{1,3}\.\d{1,3}\.\d{1,3})" `
        | ForEach-Object { $_.Matches[0].Value } | Select-Object -First 1
    Write-Host "Installing $nupkgName" -ForegroundColor Green
    dotnet add TestProject package $nupkgName --prerelease
    dotnet add TestProject package "Arc4u.Standard.Diagnostics.Serilog" -v "8.2.1"
    dotnet build "./TestSolution.sln"
    rider "$tempDir/TestSolution.sln"
}
finally
{
    Pop-Location
}