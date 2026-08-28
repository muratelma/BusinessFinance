<#
.SYNOPSIS
    Repo geçmişini ve üretilen APK'yı secret için tarar (Aşama 06.1 Grup 1).

.DESCRIPTION
    Çalışma ağacının taraması bir testtir (SecretScanTests) ve her `dotnet test`
    koşusunda çalışır. Bir teste sığmayan iki ayak bu betikte yaşar:

      * Repo geçmişi: silinmiş bir dosyadaki anahtar commit'te durmaya devam
        eder; tarama HEAD'i değil bütün commit'lerin ağacını gezer.
      * Üretilen APK: --dart-define ile giren değerler, gömülü yapılandırma ve
        asset'ler paketin içine girer.

    Bulgu varsa çıkış kodu 1'dir. Eşleşen değerin kendisi hiçbir koşulda ekrana
    basılmaz: rapor bulgunun türünü ve nerede görüldüğünü taşır, içeriğini değil.

.PARAMETER ApkPath
    Taranacak APK. Verilmezse debug çıktısı aranır; o da yoksa APK ayağı
    atlanır ve bu bir hata değildir (CI APK üretmiyor).

.PARAMETER SkipHistory
    Geçmiş taramasını atlar. Yalnız yerelde hızlı bir APK kontrolü için vardır;
    kalite kapısında kullanılmaz.
#>
[CmdletBinding()]
param(
    [string]$ApkPath,
    [switch]$SkipHistory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot

# Desenler SecretScanTests ile ortak bir kaynaktan gelmez; ikisi bilerek ayrıdır.
# Test kaynak ağacını, betik geçmişi ve ikili paketi tarar. Aynı ifadeyi üç farklı
# girdi türüne uygulamak için ortak bir soyutlama kurmak, kazandırdığından
# fazlasını karmaşıklaştırırdı.
$patterns = @(
    @{ Name = 'Google API anahtarı'; Pattern = 'AIza[0-9A-Za-z_\-]{30,}' }
    @{ Name = 'Brevo API anahtarı'; Pattern = 'xkeysib-[0-9a-f]{16,}' }
    @{ Name = 'OpenAI benzeri anahtar'; Pattern = '\bsk-[A-Za-z0-9]{32,}' }
    @{ Name = 'GitHub token'; Pattern = '\bgh[pousr]_[A-Za-z0-9]{30,}' }
    @{ Name = 'AWS erişim anahtarı'; Pattern = '\bAKIA[0-9A-Z]{16}\b' }
    @{ Name = 'Özel anahtar bloğu'; Pattern = '-----BEGIN [A-Z ]*PRIVATE KEY-----' }
    @{ Name = 'Kodlanmış JWT'; Pattern = '\beyJhbGciOi[A-Za-z0-9._\-]{40,}' }
    @{ Name = 'Parolalı connection string'; Pattern = '(?i)(Server|Data Source)=[^;]{1,80};[^;]{0,120}?(Password|Pwd)=(?![$%{]|Replace-With)[^;]{4,}' }
)

$findings = New-Object System.Collections.Generic.List[string]

if (-not $SkipHistory) {
    $commits = & git -C $repositoryRoot rev-list --all
    if (-not $commits) {
        throw 'Geçmiş taraması hiçbir commit görmedi; tarama çalışmıyor demektir.'
    }

    Write-Output "Repo geçmişi taranıyor ($($commits.Count) commit)..."

    foreach ($pattern in $patterns) {
        # `--name-only` bilerek: eşleşen satırın kendisi ekrana basılmamalı.
        # `--perl-regexp`: desenler lookahead ve `` kullanıyor, ERE yetmez.
        $hits = & git -C $repositoryRoot grep --name-only --perl-regexp --text -e $pattern.Pattern $commits 2>$null

        foreach ($hit in $hits) {
            $findings.Add("geçmiş $hit -> $($pattern.Name)")
        }
    }
}

if (-not $ApkPath) {
    $default = Join-Path $repositoryRoot 'mobile/business_finance_mobile/build/app/outputs/flutter-apk/app-debug.apk'
    if (Test-Path -LiteralPath $default) { $ApkPath = $default }
}

if ($ApkPath -and (Test-Path -LiteralPath $ApkPath)) {
    Write-Output "APK taranıyor: $(Split-Path -Leaf $ApkPath)"

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $archive = [System.IO.Compression.ZipFile]::OpenRead(
        (Resolve-Path -LiteralPath $ApkPath).Path)

    try {
        foreach ($entry in $archive.Entries) {
            if ($entry.Length -eq 0) { continue }

            $stream = $entry.Open()
            try {
                # ISO-8859-1 (28591): ikili içerik kayıpsız okunur ve ASCII
                # desenleri kod çözme hatasına takılmadan aranabilir.
                # `::Latin1` kısayolu Windows PowerShell 5.1'de yok.
                $reader = New-Object System.IO.StreamReader(
                    $stream, [System.Text.Encoding]::GetEncoding(28591))
                try {
                    $text = $reader.ReadToEnd()
                }
                finally { $reader.Dispose() }
            }
            finally { $stream.Dispose() }

            foreach ($pattern in $patterns) {
                if ($text -match $pattern.Pattern) {
                    $findings.Add("apk $($entry.FullName) -> $($pattern.Name)")
                }
            }
        }
    }
    finally { $archive.Dispose() }
}
else {
    Write-Output 'APK bulunamadı; bu ayak atlandı.'
}

if ($findings.Count -gt 0) {
    Write-Output ''
    Write-Output 'Secret taraması bulgu üretti:'
    $findings | Sort-Object -Unique | ForEach-Object { Write-Output "  $_" }
    Write-Output ''
    Write-Output 'Bulunan değer DÖNDÜRÜLMELİDİR; kaynağı silmek yetmez.'
    exit 1
}

Write-Output 'Secret taraması temiz.'
exit 0
