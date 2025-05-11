using namespace System.IO
param([string]$InputFile, [string] $Version, [string] $DestinationFolder)

function PatchVsixManifest([string] $folder, [string] $newVersion){

    $file = "$folder/extension.vsixmanifest"
    if((Test-Path $file) -ne $true){ exit 1}

    [xml]$vsixXml = Get-Content "$folder/extension.vsixmanifest"

    $ns = New-Object System.Xml.XmlNamespaceManager $vsixXml.NameTable
    $ns.AddNamespace("ns", $vsixXml.DocumentElement.NamespaceURI) | Out-Null

    $attrVersion = ""

    if ($vsixXml.SelectSingleNode("//ns:Identity", $ns)){ # VS2012 format
        $attrVersion = $vsixXml.SelectSingleNode("//ns:Identity", $ns).Attributes["Version"]
    }
    elseif ($vsixXml.SelectSingleNode("//ns:Version", $ns)){ # VS2010 format
        $attrVersion = $vsixXml.SelectSingleNode("//ns:Version", $ns)
    }
    if($attrVersion -eq "") {exit 2}

    [Version]$version = New-Object Version $newVersion
    $attrVersion.InnerText = $version

    $vsixXml.Save("$file") | Out-Null
}

function PatchCatalogJson([string] $folder, [string] $newVersion){
    $file = "$folder/catalog.json"
    if((Test-Path $file) -ne $true){ exit 3}

    $a = Get-Content $file -raw | ConvertFrom-Json
    $idComposite = $a.info.id
    $match = [Regex]::Match($idComposite, "([^,]+),version=(.+)")
    $vsixId = $match.Groups[1].Value
    $a.info.id = "$vsixId,version=$newVersion"

    $a.packages | % {
        if($_.id.EndsWith($vsixId)){
            $_.version = "$newVersion"

            if($null -ne $_.dependencies.$vsixId){
                $_.dependencies.$vsixId = "$newVersion"
            }
        } elseif($_.id -eq $vsixId){
            $_.version = "$newVersion"
        }
    }
    #$a.version | % {if($_.name -eq 'test1'){$_.version=3.0}}
    $a | ConvertTo-Json -depth 32| set-content $file
}

function PatchManifestJson([string] $folder, [string] $newVersion){
    $file = "$folder/manifest.json"
    if((Test-Path $file) -ne $true){ exit 4}

    $a = Get-Content $file -raw | ConvertFrom-Json
    $a.version = $newVersion
    $a | ConvertTo-Json -depth 32| set-content $file
}

$tempDir = New-TemporaryFile | % { Remove-Item $_; New-Item -ItemType Directory -Path $_ }

Expand-Archive -Path $InputFile -DestinationPath $tempDir
PatchCatalogJson $tempDir $Version
PatchVsixManifest $tempDir $Version
PatchManifestJson $tempDir $Version

$fullName = [Path]::Combine($DestinationFolder, [Path]::GetFileName($InputFile))
if((Test-Path $DestinationFolder) -ne $true){
    New-Item -Type Directory $DestinationFolder | Out-Null
}

Compress-Archive -Path "$tempDir/*" -DestinationPath $fullName -Force
Remove-Item $tempDir -Recurse -Force

return $fullName