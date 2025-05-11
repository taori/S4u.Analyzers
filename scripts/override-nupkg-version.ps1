using namespace System.IO
param([string]$InputFile, [string] $Version, [string] $DestinationFolder)

function PatchNuspec([string] $folder, [string] $newVersion){

    $file = Get-ChildItem -Path $folder -Filter "*.nuspec" | Select-Object -ExpandProperty FullName -First 1
    if((Test-Path $file) -ne $true){ exit 1}

    [xml]$vsixXml = Get-Content $file

    $ns = New-Object System.Xml.XmlNamespaceManager $vsixXml.NameTable
    $ns.AddNamespace("ns", $vsixXml.DocumentElement.NamespaceURI) | Out-Null

    $node = $vsixXml.SelectSingleNode("//ns:package/ns:metadata/ns:version", $ns)
    if($null -eq $node) { exit 2 }

    $oldVersion = $node.InnerText
    $node.InnerText = $newVersion

    $vsixXml.Save("$file") | Out-Null
    return @($oldVersion, $newVersion)
}

$tempDir = New-TemporaryFile | % { Remove-Item $_; New-Item -ItemType Directory -Path $_ }

Write-Host "Patching $InputFile using tmp folder $tempDir" -ForegroundColor Green

Expand-Archive -Path $InputFile -DestinationPath $tempDir
$suffixSwap = PatchNuspec $tempDir $Version

$fn = [Path]::GetFileName($InputFile).Replace($suffixSwap[0], $suffixSwap[1])

$fullName = [Path]::Combine($DestinationFolder, $fn)
if((Test-Path $DestinationFolder) -ne $true){
    New-Item -Type Directory $DestinationFolder | Out-Null
}

Compress-Archive -Path "$tempDir/*" -DestinationPath $fullName -Force
Remove-Item $tempDir -Recurse -Force

return $fullName