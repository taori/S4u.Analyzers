param(
    [string]$OutDir="D:\tmp\roslyn-script-deploy", 
    [string]$Prefix="0.2.0", 
    [string]$Suffix="internal-$([DateTime]::Now.ToString('yyyyMMddHHmmss'))"
)

$sln = Resolve-Path "$PSScriptRoot\..\Logging.slnx"
$projects = @(
    Resolve-Path "$PSScriptRoot\..\Logging.Analyzers\Logging.Analyzers.csproj"
    Resolve-Path "$PSScriptRoot\..\Logging.Codefixes\Logging.Codefixes.csproj"
    
)

$nupkgProj = Resolve-Path "$PSScriptRoot\..\Logging.Package\Logging.Package.csproj"

dotnet restore $sln

#foreach ($proj in $projects){
#    $outputDir = Resolve-Path "$nupkgProj\..\bin\Release\netstandard2.0"
#    dotnet build "$proj" -c Release -o $outputDir
#}

if($Suffix -eq "" -and $Prefix -eq ""){
    dotnet pack "$nupkgProj" -o "$OutDir"    
}
else
{
    if($Suffix -ne "" -and $Prefix -ne ""){
        dotnet pack "$nupkgProj" -o "$OutDir" /p:VersionSuffix=$Suffix /p:VersionPrefix=$Prefix    
    } else {
        if($Suffix -ne "")
        {
            dotnet pack "$nupkgProj" -o "$OutDir" /p:VersionSuffix=$Suffix
        }
        else
        {
            dotnet pack "$nupkgProj" -o "$OutDir" /p:VersionPrefix=$Prefix            
        }
    }   
}