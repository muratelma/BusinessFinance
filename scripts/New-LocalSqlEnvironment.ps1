[CmdletBinding()]
param(
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$environmentFile = Join-Path $repositoryRoot '.env'

if ((Test-Path -LiteralPath $environmentFile) -and -not $Force) {
    Write-Output 'Local .env already exists; no changes were made.'
    exit 0
}

$randomBytes = New-Object byte[] 24
$randomNumberGenerator = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$randomNumberGenerator.GetBytes($randomBytes)
$randomNumberGenerator.Dispose()
$randomPart = [Convert]::ToBase64String($randomBytes)
$password = "Aa1!$randomPart"
if ($password.Length -lt 20) {
    throw 'Generated password did not satisfy the minimum length.'
}

$content = "MSSQL_SA_PASSWORD=$password`nMSSQL_HOST_PORT=14334`n"
$utf8WithoutBom = [System.Text.UTF8Encoding]::new($false)

[System.IO.File]::WriteAllText($environmentFile, $content, $utf8WithoutBom)
Write-Output 'Created ignored local .env with a generated SQL Server password.'
