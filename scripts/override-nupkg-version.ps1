using namespace System.IO
param([string]$File, [string] $Version)

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

$tempDir = Join-Path $env:TEMP $(New-Guid) | %{ mkdir $_ }

Expand-Archive -Path $File -DestinationPath $tempDir
$suffixSwap = PatchNuspec $tempDir $Version

$fn = [Path]::GetFileName($File).Replace($suffixSwap[0], $suffixSwap[1])
$fullName = [Path]::Combine([Path]::GetDirectoryName($File), $fn)

Compress-Archive -Path "$tempDir/*" -DestinationPath $fullName -Force
return $fullName