param(
    [string]$OutDir="D:/tmp/roslyn-script-deploy",
    [string]$Prefix="0.1.0",
    [string]$Suffix="internal-$([DateTime]::Now.ToString('yyyyMMddHHmmss'))"
)

$r = $PSScriptRoot
$projectName = "Logging"
$tempDir = Join-Path $env:TEMP $(New-Guid) | %{ mkdir $_ }

$sln = Resolve-Path "$r/../$projectName.slnx"

"Cleaning Project" | Write-Host -Foreground Green
dotnet clean $sln

$projects = @(
    Resolve-Path "$r/../$projectName.Analyzers/$projectName.Analyzers.csproj"
    Resolve-Path "$r/../$projectName.Codefixes/$projectName.Codefixes.csproj"    
)

$msbuildPath = &"${env:ProgramFiles(x86)}/Microsoft Visual Studio/Installer/vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild/**/Bin/MSBuild.exe
Write-Host "The following path is used for MSBuild: $msbuildPath"

dotnet restore $sln

$vsixProj = Resolve-Path "$r/../$projectName.Vsix/$projectName.Vsix.csproj"

&$msbuildPath $vsixProj -verbosity:m -p:OutputPath=$tempDir

$vsixFile = Get-ChildItem -Path $tempDir -Filter "*.vsix" | Select-Object -First 1 -ExpandProperty FullName

Copy-Item -Path $vsixFile -Destination $OutDir
Remove-Item -Recurse -Force -Path $tempDir