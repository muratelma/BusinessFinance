<#
.SYNOPSIS
    Flutter bağımlılıklarını bilinen güvenlik duyuruları için tarar
    (Aşama 06.1 Grup 2).

.DESCRIPTION
    Backend tarafında karşılığı `dotnet list package --vulnerable`'dır ve CI'da
    koşar. Pub tarafında karşılığı `pub outdated`'in JSON çıktısıdır: pub.dev
    her paket sürümü için güvenlik duyurusu (advisory) ve geri çekilme
    (retraction) bilgisini oradan verir.

    İki bulgu türü kapıyı kırar:

      * isCurrentAffectedByAdvisory — kilit dosyasındaki sürüm bilinen bir
        güvenlik duyurusunun kapsamında.
      * isCurrentRetracted — sürüm yayıncısı tarafından geri çekilmiş.

    Kullanımdan kaldırılmış (discontinued) paket bulgu sayılmaz ve kapıyı
    kırmaz: bakım riskidir, zafiyet değil. Yine de rapora yazılır, çünkü
    görülmeden karar verilemez.

.PARAMETER ReportPath
    `pub outdated --json` çıktısını taşıyan hazır bir dosya. Verilmezse komut
    çalıştırılır. Kapının gerçekten kırıldığını denemek için vardır: sentetik
    bir rapor verilip betiğin 1 döndüğü görülür.
#>
[CmdletBinding()]
param(
    [string]$ReportPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$flutterProject = Join-Path $repositoryRoot 'mobile/business_finance_mobile'

if ($ReportPath) {
    $json = Get-Content -LiteralPath $ReportPath -Raw
}
else {
    Push-Location $flutterProject
    try {
        # `pub outdated` güncellenebilir paket bulunca sıfırdan farklı dönebilir;
        # burada ölçülen şey güncellik değil duyuru olduğu için çıkış kodu değil
        # çıktı okunur.
        $json = & flutter pub outdated --json --show-all 2>$null | Out-String
    }
    finally { Pop-Location }
}

if (-not $json.Trim()) {
    throw 'Bağımlılık raporu boş; tarama çalışmıyor demektir.'
}

$report = $json | ConvertFrom-Json

if (-not $report.packages) {
    throw 'Bağımlılık raporu hiçbir paket görmedi; tarama çalışmıyor demektir.'
}

Write-Output "Bağımlılık duyuruları taranıyor ($($report.packages.Count) paket)..."

$findings = New-Object System.Collections.Generic.List[string]
$notes = New-Object System.Collections.Generic.List[string]

foreach ($package in $report.packages) {
    $version = if ($package.current) { $package.current.version } else { '?' }

    if ($package.isCurrentAffectedByAdvisory) {
        $findings.Add("$($package.package) $version -> güvenlik duyurusu")
    }

    if ($package.isCurrentRetracted) {
        $findings.Add("$($package.package) $version -> sürüm geri çekilmiş")
    }

    if ($package.isDiscontinued) {
        $notes.Add("$($package.package) -> paket kullanımdan kaldırılmış")
    }
}

if ($notes.Count -gt 0) {
    Write-Output ''
    Write-Output 'Not (kapıyı kırmaz, bakım riski):'
    $notes | Sort-Object -Unique | ForEach-Object { Write-Output "  $_" }
}

if ($findings.Count -gt 0) {
    Write-Output ''
    Write-Output 'Bağımlılık taraması bulgu üretti:'
    $findings | Sort-Object -Unique | ForEach-Object { Write-Output "  $_" }
    Write-Output ''
    Write-Output 'Her bulgu bir sonuca bağlanır: yükselt, değiştir veya gerekçesiyle kabul et.'
    exit 1
}

Write-Output 'Bağımlılık taraması temiz.'
exit 0
