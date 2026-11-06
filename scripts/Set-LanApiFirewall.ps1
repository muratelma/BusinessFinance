[CmdletBinding()]
param(
    [switch]$Remove
)

# API'nin `lan` profiliyle ev ağından (iPhone ile cihaz testi) erişilebilmesi
# için gelen bağlantı kuralı. Kural yalnız Windows'un "özel" ağ profilinde ve
# yalnız yerel alt ağdan gelen bağlantıya açıktır; internete açmaz. SQL Server
# loopback'te kalır, ona kural yazılmaz. Yönetici PowerShell'inden çalıştırılır.

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$ruleName = 'BusinessFinance API (ev ağı, 5284)'

$existing = Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue
if ($existing) {
    Remove-NetFirewallRule -DisplayName $ruleName
}

if ($Remove) {
    Write-Output 'Ev ağı kuralı kaldırıldı.'
    exit 0
}

New-NetFirewallRule `
    -DisplayName $ruleName `
    -Direction Inbound `
    -Action Allow `
    -Protocol TCP `
    -LocalPort 5284 `
    -Profile Private `
    -RemoteAddress LocalSubnet | Out-Null

$privateNetworks = @(Get-NetConnectionProfile | Where-Object NetworkCategory -eq 'Private')
if ($privateNetworks.Count -eq 0) {
    Write-Warning 'Bağlı ağ "özel" değil; kural bu ağda çalışmaz. Ev ağını Ayarlar > Ağ ve İnternet içinden "Özel ağ" yapın.'
}

Write-Output 'Ev ağı kuralı eklendi. Kaldırmak için: .\scripts\Set-LanApiFirewall.ps1 -Remove'
