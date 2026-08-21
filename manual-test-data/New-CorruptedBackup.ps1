param(
    [Parameter(Mandatory = $true)]
    [string] $InputPath,

    [Parameter(Mandatory = $true)]
    [string] $OutputPath
)

$envelope = Get-Content -Raw -LiteralPath $InputPath | ConvertFrom-Json
$payload = [string] $envelope.payload
if ($payload.Length -lt 32) {
    throw 'Yedek payload alanı test için beklenenden kısa.'
}

$index = [Math]::Floor($payload.Length / 2)
while ($index -lt $payload.Length -and $payload[$index] -eq '=') {
    $index++
}
if ($index -ge $payload.Length) {
    throw 'Değiştirilebilecek base64 karakteri bulunamadı.'
}

$replacement = if ($payload[$index] -eq 'A') { 'B' } else { 'A' }
$envelope.payload = $payload.Substring(0, $index) + $replacement + $payload.Substring($index + 1)
$json = $envelope | ConvertTo-Json -Compress -Depth 8
[System.IO.File]::WriteAllText(
    $OutputPath,
    $json,
    [System.Text.UTF8Encoding]::new($false))
