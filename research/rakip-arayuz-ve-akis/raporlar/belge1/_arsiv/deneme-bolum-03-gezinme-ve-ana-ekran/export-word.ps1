$ErrorActionPreference='Stop'
$word=$null
$document=$null
$inputPath=[string](Resolve-Path -LiteralPath (Join-Path $PSScriptRoot 'bolum-03.docx')).Path
$outputPath=[System.IO.Path]::ChangeExtension($inputPath,'.pdf')
try {
    Write-Output 'Word baslatiliyor'
    $word=New-Object -ComObject Word.Application
    $word.Visible=$false
    $word.DisplayAlerts=0
    $word.Options.UpdateLinksAtOpen=$false
    $document=$word.Documents.Open($inputPath,$false,$true)
    Write-Output 'Belge acildi'
    $document.ExportAsFixedFormat($outputPath,17)
    Write-Output 'PDF hazir'
} finally {
    if($null -ne $document){$document.Close(0)}
    if($null -ne $word){$word.Quit()}
}
