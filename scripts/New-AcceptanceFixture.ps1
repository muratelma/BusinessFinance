<#
.SYNOPSIS
    Kabul turu için bir hesabı uygulamanın bütün özelliklerini kapsayan
    sentetik veriyle doldurur.

.DESCRIPTION
    Her kabul turunda aynı veri elle giriliyordu: hesap aç, kategori seç, birkaç
    hareket yaz, kart tanımla, taksit kur. Bu hem uzun sürüyor hem de her tur
    farklı bir veri kümesiyle koşuluyor demek — bir turun bulduğu şey öteki turda
    aranmıyor bile.

    Betik veriyi **API üzerinden** yazar, doğrudan veritabanına değil. Nedeni
    tek: kurallar use case'lerde yaşıyor. Doğrudan SQL, aynı harcamayı iki kez
    saydıran ya da kapsamı çözülmemiş bir kayıt üretebilir ve o kayıt kabul
    turunda gerçek bir kusur gibi görünürdü.

    Adlandırılmış varlıklar (hesap, kategori, kart, karşı taraf, bütçe, hedef,
    tekrarlayan plan) **adına göre** aranır ve varsa yeniden kullanılır; betiği
    ikinci kez koşmak onları çoğaltmaz. Akış kayıtları (hareket, harcama,
    tahsilat, transfer) her koşuda yeniden yazılır — ikinci koşu geçmişi
    iki katına çıkarır. Bu bilinçli: fikstürün amacı tekrarlanabilir bir
    başlangıç değil, dolu bir hesap. Yalnız adlandırılmış varlıkları kurmak
    için `-SkipFlows` kullanılır.

    Veri sentetiktir: satıcı adları uydurma, vergi numaraları geçersiz
    aralıkta. Gerçek finansal veri Aşama 07'nin güvenlik kapısı geçilmeden
    kullanılmaz.

.PARAMETER BaseUrl
    API adresi. Varsayılan yerel API.

.PARAMETER Email
    Doldurulacak hesap. Hesap yoksa açılır; varsa mevcut parolasıyla girilir.

.PARAMETER Password
    Hesabın parolası. Hesap yeni açılıyorsa bu parola ile açılır.

.PARAMETER AsOf
    Fikstürün "bugün"ü. Bütün tarihler buna göre hesaplanır, böylece hangi ay
    koşulursa koşulsun veri güncel görünür.

.PARAMETER SkipFlows
    Yalnız adlandırılmış varlıkları kurar; hareket, harcama ve tahsilat yazmaz.

.PARAMETER DocumentPath
    Fiş/fatura örneklerinin klasörü. Bulunursa birkaç harekete ek olarak
    iliştirilir. Varsayılan `samples/documents`.

.EXAMPLE
    ./scripts/New-AcceptanceFixture.ps1 -Email vergi-kabul@example.test -Password '...'
#>
[CmdletBinding()]
param(
    [string]$BaseUrl = 'http://127.0.0.1:5284',
    [string]$Email = 'vergi-kabul@example.test',
    [Parameter(Mandatory = $true)][string]$Password,
    [datetime]$AsOf = (Get-Date),
    [switch]$SkipFlows,
    [string]$DocumentPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if (-not $DocumentPath) {
    $DocumentPath = Join-Path $repositoryRoot 'samples/documents'
}

$script:AccessToken = $null
$script:Created = [ordered]@{}

function Add-Count {
    param([string]$What, [int]$Delta = 1)
    if (-not $script:Created.Contains($What)) { $script:Created[$What] = 0 }
    $script:Created[$What] += $Delta
}

function Read-ErrorBody {
    param($ErrorRecord)
    try {
        $response = $ErrorRecord.Exception.Response
        if (-not $response) { return $ErrorRecord.Exception.Message }
        $stream = $response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream)
        try { return $reader.ReadToEnd() } finally { $reader.Dispose() }
    }
    catch { return $ErrorRecord.Exception.Message }
}

function Invoke-Api {
    param(
        [string]$Method,
        [string]$Path,
        $Body,
        [switch]$Anonymous,
        [switch]$Tolerate
    )

    $headers = @{}
    if (-not $Anonymous) { $headers['Authorization'] = "Bearer $script:AccessToken" }

    try {
        if ($null -ne $Body) {
            $json = $Body | ConvertTo-Json -Depth 8 -Compress
            $bytes = [System.Text.Encoding]::UTF8.GetBytes($json)
            return Invoke-RestMethod -Method $Method -Uri "$BaseUrl$Path" `
                -Headers $headers -Body $bytes `
                -ContentType 'application/json; charset=utf-8'
        }

        return Invoke-RestMethod -Method $Method -Uri "$BaseUrl$Path" -Headers $headers
    }
    catch {
        $detail = Read-ErrorBody $_
        if ($Tolerate) {
            Write-Warning "$Method $Path -> $detail"
            return $null
        }

        throw "$Method $Path basarisiz: $detail"
    }
}

function Connect-Fixture {
    $login = @{ email = $Email; password = $Password }
    try {
        $session = Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/v1/auth/login" `
            -Body ([System.Text.Encoding]::UTF8.GetBytes(($login | ConvertTo-Json -Compress))) `
            -ContentType 'application/json; charset=utf-8'
        Write-Output "Mevcut hesaba girildi: $Email"
    }
    catch {
        $register = @{ email = $Email; password = $Password; hasBusiness = $true }
        $session = Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/v1/auth/register" `
            -Body ([System.Text.Encoding]::UTF8.GetBytes(($register | ConvertTo-Json -Compress))) `
            -ContentType 'application/json; charset=utf-8'
        Write-Output "Hesap acildi: $Email"
        Add-Count 'hesap (kullanici)'
    }

    $script:AccessToken = $session.accessToken
}

# --- tarih yardimcilari -------------------------------------------------------
# Butun tarihler AsOf'a gore. Fikstur hangi ay kosulursa kosulsun guncel gorunur.

$today = $AsOf.Date

function Day { param([int]$Delta) ($today.AddDays($Delta)).ToString('yyyy-MM-dd') }

function MonthDay {
    param([int]$MonthsBack, [int]$DayOfMonth)
    $anchor = $today.AddDays(-($today.Day - 1)).AddMonths(-$MonthsBack)
    $day = [Math]::Min($DayOfMonth, [DateTime]::DaysInMonth($anchor.Year, $anchor.Month))
    (Get-Date -Year $anchor.Year -Month $anchor.Month -Day $day).ToString('yyyy-MM-dd')
}

function Money { param([decimal]$Value) $Value.ToString('0.0000', [cultureinfo]::InvariantCulture) }

# --- adlandirilmis varlik arama ----------------------------------------------

function Get-Existing {
    param([string]$Path, [string]$Name)
    $list = Invoke-Api -Method Get -Path $Path
    if ($null -eq $list) { return $null }
    $items = if ($list.PSObject.Properties.Name -contains 'items') { $list.items } else { $list }
    foreach ($item in $items) {
        if ($item.name -eq $Name) { return $item }
    }
    return $null
}

function New-Account {
    param([string]$Name, [string]$Type, [decimal]$Opening, [string]$Scope)
    $existing = Get-Existing -Path '/api/v1/accounts?pageSize=100' -Name $Name
    if ($existing) { return $existing }
    $created = Invoke-Api -Method Post -Path '/api/v1/accounts' -Body @{
        name           = $Name
        type           = $Type
        currency       = 'TRY'
        openingBalance = (Money $Opening)
        defaultScope   = $Scope
    }
    Add-Count 'hesap'
    return $created
}

function New-Category {
    param([string]$Name, [string]$Type, [string]$Scope, $Deductible)
    $existing = Get-Existing -Path '/api/v1/categories?pageSize=200' -Name $Name
    if ($existing) { return $existing }
    $created = Invoke-Api -Method Post -Path '/api/v1/categories' -Body @{
        name                   = $Name
        type                   = $Type
        defaultScope           = $Scope
        defaultIsTaxDeductible = $Deductible
    }
    Add-Count 'kategori'
    return $created
}

function Get-Category {
    param([string]$Name)
    $found = Get-Existing -Path '/api/v1/categories?pageSize=200' -Name $Name
    if (-not $found) { throw "Kategori bulunamadi: $Name" }
    return $found
}

function New-Card {
    param([string]$Name, [decimal]$Limit, [int]$Closing, [int]$Due, [string]$Scope)
    $existing = Get-Existing -Path '/api/v1/credit-cards?pageSize=50' -Name $Name
    if ($existing) { return $existing }
    $created = Invoke-Api -Method Post -Path '/api/v1/credit-cards' -Body @{
        name                = $Name
        limit               = (Money $Limit)
        currency            = 'TRY'
        statementClosingDay = $Closing
        paymentDueDay       = $Due
        minimumPaymentRate  = '0.2000'
        defaultScope        = $Scope
    }
    Add-Count 'kredi karti'
    return $created
}

function New-Counterparty {
    param([string]$Name, [string]$Note)
    $existing = Get-Existing -Path '/api/v1/counterparties?pageSize=100' -Name $Name
    if ($existing) { return $existing }
    $created = Invoke-Api -Method Post -Path '/api/v1/counterparties' -Body @{
        name = $Name
        note = $Note
    }
    Add-Count 'karsi taraf'
    return $created
}

function New-Transaction {
    param(
        [string]$AccountId, [string]$CategoryId, [decimal]$Amount, [string]$Type,
        [string]$Date, [string]$Scope, [string]$Description,
        [string]$VatRate, [string]$VatAmount, $Deductible
    )
    $body = @{
        accountId       = $AccountId
        categoryId      = $CategoryId
        amount          = (Money $Amount)
        currency        = 'TRY'
        type            = $Type
        scope           = $Scope
        transactionDate = $Date
        description     = $Description
    }
    if ($VatRate) { $body['vatRate'] = $VatRate }
    if ($VatAmount) { $body['vatAmount'] = $VatAmount }
    if ($null -ne $Deductible) { $body['isTaxDeductible'] = $Deductible }
    $created = Invoke-Api -Method Post -Path '/api/v1/transactions' -Body $body
    Add-Count 'hareket'
    return $created
}

# --- fikstur ------------------------------------------------------------------

Connect-Fixture

Write-Output ''
Write-Output 'Hesaplar ve kategoriler...'

$cash = New-Account -Name 'Dukkan Kasasi' -Type 'cash' -Opening 5000 -Scope 'business'
$bank = New-Account -Name 'Ziraat Vadesiz' -Type 'bank' -Opening 38500 -Scope 'business'
$wallet = New-Account -Name 'Sahsi Cuzdan' -Type 'cash' -Opening 2400 -Scope 'personal'
$savings = New-Account -Name 'Birikim Hesabi' -Type 'bank' -Opening 60000 -Scope 'personal'

# Kategori seti kayit sirasinda kuruldu; bunlar setin disindaki iki ornek.
# Biri indirilebilirligin varsayilanini tasiyor, oteki sahsi tarafta duruyor.
$suppliesCategory = New-Category -Name 'Kirtasiye sarfi' -Type 'expense' -Scope 'business' -Deductible $true
$null = New-Category -Name 'Cocuk okul gideri' -Type 'expense' -Scope 'personal' -Deductible $null

$sales = Get-Category -Name 'Satış geliri'
$service = Get-Category -Name 'Hizmet geliri'
$goods = Get-Category -Name 'Ticari mal alımı'
$rent = Get-Category -Name 'İşyeri kirası'
$utilities = Get-Category -Name 'Elektrik, su, doğalgaz'
$payroll = Get-Category -Name 'Personel ücreti'
$accountant = Get-Category -Name 'Muhasebeci ve danışmanlık'
$vehicle = Get-Category -Name 'Araç ve yakıt'
$commission = Get-Category -Name 'Banka ve POS komisyonu'
$taxCategory = Get-Category -Name 'SGK ve vergi ödemesi'
$otherBusiness = Get-Category -Name 'Diğer işletme gideri'
$market = Get-Category -Name 'Market Alışverişi'
$dining = Get-Category -Name 'Yeme-içme'
$transport = Get-Category -Name 'Ulaşım'
$health = Get-Category -Name 'Sağlık'
$subscription = Get-Category -Name 'Abonelikler'
$rentIncome = Get-Category -Name 'Kira geliri'

Write-Output 'Kartlar...'
$businessCard = New-Card -Name 'Ticari Kart' -Limit 60000 -Closing 25 -Due 10 -Scope 'business'
$personalCard = New-Card -Name 'Sahsi Kart' -Limit 20000 -Closing 15 -Due 5 -Scope 'personal'

Write-Output 'Karsi taraflar...'
$school = New-Counterparty -Name 'Okul Kooperatifi' -Note 'Aylik toplu kirtasiye alimi'
$wholesaler = New-Counterparty -Name 'Ornek Toptan Kagit' -Note 'Vadeli alim, 30 gun'
$neighbour = New-Counterparty -Name 'Mahalle Kirtasiye' -Note 'Kucuk bakiye'
$dormant = New-Counterparty -Name 'Eski Musteri' -Note 'Artik calisilmiyor'

if ($SkipFlows) {
    Write-Output ''
    Write-Output 'Akis kayitlari atlandi (-SkipFlows).'
    $script:Created.GetEnumerator() | ForEach-Object { Write-Output ("  {0,-22} {1}" -f $_.Key, $_.Value) }
    return
}

Write-Output 'Gelir ve giderler...'

# Isletme geliri: KDV tasiyor, tutar brut. KDV tutari kayit tutarini
# degistirmez - tasinir, hesaplanmaz (ADR 0016).
New-Transaction -AccountId $cash.id -CategoryId $sales.id -Amount 8450 -Type 'income' `
    -Date (MonthDay 2 6) -Scope 'business' -Description 'Okul donemi acilis satisi' `
    -VatRate '0.2000' -VatAmount '1408.3300' -Deductible $null | Out-Null
New-Transaction -AccountId $cash.id -CategoryId $sales.id -Amount 11200 -Type 'income' `
    -Date (MonthDay 1 4) -Scope 'business' -Description 'Hafta sonu satisi' `
    -VatRate '0.2000' -VatAmount '1866.6700' -Deductible $null | Out-Null
New-Transaction -AccountId $cash.id -CategoryId $sales.id -Amount 9750 -Type 'income' `
    -Date (Day -12) -Scope 'business' -Description 'Gunluk perakende satis' `
    -VatRate '0.2000' -VatAmount '1625.0000' -Deductible $null | Out-Null
New-Transaction -AccountId $bank.id -CategoryId $service.id -Amount 6400 -Type 'income' `
    -Date (Day -20) -Scope 'business' -Description 'Fotokopi ve ciltleme hizmeti' `
    -VatRate '0.2000' -VatAmount '1066.6700' -Deductible $null | Out-Null

# Isletme gideri: indirilebilirlik acik olarak tasiniyor.
New-Transaction -AccountId $bank.id -CategoryId $goods.id -Amount 14800 -Type 'expense' `
    -Date (MonthDay 2 9) -Scope 'business' -Description 'Toptan defter ve kalem alimi' `
    -VatRate '0.2000' -VatAmount '2466.6700' -Deductible $true | Out-Null
New-Transaction -AccountId $bank.id -CategoryId $rent.id -Amount 18500 -Type 'expense' `
    -Date (MonthDay 1 5) -Scope 'business' -Description 'Isyeri kirasi' `
    -VatRate '0.2000' -VatAmount '3083.3300' -Deductible $true | Out-Null
New-Transaction -AccountId $bank.id -CategoryId $utilities.id -Amount 3260 -Type 'expense' `
    -Date (Day -26) -Scope 'business' -Description 'Elektrik faturasi' `
    -VatRate '0.2000' -VatAmount '543.3300' -Deductible $true | Out-Null
New-Transaction -AccountId $bank.id -CategoryId $payroll.id -Amount 22000 -Type 'expense' `
    -Date (Day -15) -Scope 'business' -Description 'Yarim zamanli personel ucreti' `
    -VatRate $null -VatAmount $null -Deductible $true | Out-Null
New-Transaction -AccountId $bank.id -CategoryId $taxCategory.id -Amount 4120 -Type 'expense' `
    -Date (Day -7) -Scope 'business' -Description 'SGK primi' `
    -VatRate $null -VatAmount $null -Deductible $true | Out-Null
New-Transaction -AccountId $cash.id -CategoryId $suppliesCategory.id -Amount 980 -Type 'expense' `
    -Date (Day -4) -Scope 'business' -Description 'Toner ve zimba teli' `
    -VatRate '0.2000' -VatAmount '163.3300' -Deductible $true | Out-Null

# Indirilemeyen isletme gideri: alan iki durumlu, kismi oran yok.
New-Transaction -AccountId $cash.id -CategoryId $otherBusiness.id -Amount 1450 -Type 'expense' `
    -Date (Day -9) -Scope 'business' -Description 'Belgesiz kucuk gider' `
    -VatRate $null -VatAmount $null -Deductible $false | Out-Null

# Sahsi taraf: indirilebilirlik hic sorulmaz.
New-Transaction -AccountId $wallet.id -CategoryId $market.id -Amount 2340 -Type 'expense' `
    -Date (Day -18) -Scope 'personal' -Description 'Haftalik market' `
    -VatRate $null -VatAmount $null -Deductible $null | Out-Null
New-Transaction -AccountId $wallet.id -CategoryId $dining.id -Amount 680 -Type 'expense' `
    -Date (Day -11) -Scope 'personal' -Description 'Aile yemegi' `
    -VatRate $null -VatAmount $null -Deductible $null | Out-Null
New-Transaction -AccountId $wallet.id -CategoryId $transport.id -Amount 420 -Type 'expense' `
    -Date (Day -6) -Scope 'personal' -Description 'Toplu tasima yuklemesi' `
    -VatRate $null -VatAmount $null -Deductible $null | Out-Null
New-Transaction -AccountId $wallet.id -CategoryId $health.id -Amount 1250 -Type 'expense' `
    -Date (Day -3) -Scope 'personal' -Description 'Dis hekimi' `
    -VatRate $null -VatAmount $null -Deductible $null | Out-Null
New-Transaction -AccountId $bank.id -CategoryId $rentIncome.id -Amount 9000 -Type 'income' `
    -Date (MonthDay 1 1) -Scope 'personal' -Description 'Daire kirasi' `
    -VatRate $null -VatAmount $null -Deductible $null | Out-Null

Write-Output 'Transferler...'
# Transfer gelir/gider raporuna sifir etki eder ve kapsam tasimaz.
Invoke-Api -Method Post -Path '/api/v1/transfers' -Body @{
    sourceAccountId      = $cash.id
    destinationAccountId = $bank.id
    amount               = (Money 12000)
    currency             = 'TRY'
    transferDate         = (Day -14)
    description          = 'Kasadan bankaya yatirma'
} | Out-Null
Add-Count 'transfer'

Invoke-Api -Method Post -Path '/api/v1/transfers' -Body @{
    sourceAccountId      = $bank.id
    destinationAccountId = $savings.id
    amount               = (Money 5000)
    currency             = 'TRY'
    transferDate         = (Day -5)
    description          = 'Vergi karsiligi ayirma'
} | Out-Null
Add-Count 'transfer'

Write-Output 'Kart harcamalari ve odeme...'
# Kart harcamasi kart borcunu VE aylik gideri artirir; kart odemesi ise
# yalnizca bakiye hareketidir - ayni harcama iki kez sayilmaz.
Invoke-Api -Method Post -Path "/api/v1/credit-cards/$($businessCard.id)/charges" -Body @{
    categoryId      = $goods.id
    amount          = (Money 6750)
    currency        = 'TRY'
    scope           = 'business'
    chargeDate      = (Day -22)
    description     = 'Raf ve teshir malzemesi'
    vatRate         = '0.2000'
    vatAmount       = '1125.0000'
    isTaxDeductible = $true
} | Out-Null
Add-Count 'kart harcamasi'

Invoke-Api -Method Post -Path "/api/v1/credit-cards/$($businessCard.id)/charges" -Body @{
    categoryId      = $vehicle.id
    amount          = (Money 2260)
    currency        = 'TRY'
    scope           = 'business'
    chargeDate      = (Day -10)
    description     = 'Akaryakit'
    vatRate         = '0.2000'
    vatAmount       = '376.6700'
    isTaxDeductible = $true
} | Out-Null
Add-Count 'kart harcamasi'

Invoke-Api -Method Post -Path "/api/v1/credit-cards/$($personalCard.id)/charges" -Body @{
    categoryId  = $subscription.id
    amount      = (Money 429)
    currency    = 'TRY'
    scope       = 'personal'
    chargeDate  = (Day -8)
    description = 'Dijital abonelik'
} | Out-Null
Add-Count 'kart harcamasi'

Invoke-Api -Method Post -Path "/api/v1/credit-cards/$($businessCard.id)/payments" -Body @{
    accountId   = $bank.id
    amount      = (Money 5000)
    currency    = 'TRY'
    paymentDate = (Day -2)
    description = 'Kart ekstresi kismi odeme'
} | Out-Null
Add-Count 'kart odemesi'

Write-Output 'Taksitli alisveris...'
# Plan yalniz niyettir; yalnizca realize edilen taksit gercek harcama uretir.
$plan = Invoke-Api -Method Post -Path '/api/v1/installment-plans' -Body @{
    creditCardId         = $businessCard.id
    categoryId           = $goods.id
    clientRequestId      = [guid]::NewGuid().ToString()
    totalAmount          = (Money 24000)
    currency             = 'TRY'
    scope                = 'business'
    installmentCount     = 6
    firstInstallmentDate = (MonthDay 2 15)
    description          = 'Vitrin dolabi ve raf sistemi'
}
Add-Count 'taksit plani'
foreach ($sequence in 1..3) {
    Invoke-Api -Method Post -Path "/api/v1/installment-plans/$($plan.id)/items/$sequence/realize" -Tolerate | Out-Null
    Add-Count 'gerceklesen taksit'
}

Write-Output 'Tekrarlayan planlar...'
function New-Recurring {
    param(
        [string]$Description, [string]$SourceType, [string]$SourceId,
        [string]$CategoryId, [decimal]$Amount, [string]$Kind, [string]$Scope,
        [string]$StartDate
    )
    $body = @{
        categoryId       = $CategoryId
        amount           = (Money $Amount)
        currency         = 'TRY'
        kind             = $Kind
        scope            = $Scope
        frequency        = 'monthly'
        startDate        = $StartDate
        monthEndBehavior = 'clamp-to-last-day'
        description      = $Description
        sourceType       = $SourceType
    }
    if ($SourceType -eq 'credit-card') { $body['creditCardId'] = $SourceId }
    else { $body['accountId'] = $SourceId }
    $created = Invoke-Api -Method Post -Path '/api/v1/recurring-transactions' -Body $body
    Add-Count 'tekrarlayan plan'
    return $created
}

$null = New-Recurring -Description 'Isyeri kirasi' -SourceType 'account' -SourceId $bank.id `
    -CategoryId $rent.id -Amount 18500 -Kind 'expense' -Scope 'business' -StartDate (MonthDay 2 5)
$null = New-Recurring -Description 'Muhasebeci ucreti' -SourceType 'account' -SourceId $bank.id `
    -CategoryId $accountant.id -Amount 3500 -Kind 'expense' -Scope 'business' -StartDate (MonthDay 2 10)
$null = New-Recurring -Description 'Daire kirasi tahsilati' -SourceType 'account' -SourceId $bank.id `
    -CategoryId $rentIncome.id -Amount 9000 -Kind 'income' -Scope 'personal' -StartDate (MonthDay 2 1)
$null = New-Recurring -Description 'Dijital abonelik' -SourceType 'credit-card' -SourceId $personalCard.id `
    -CategoryId $subscription.id -Amount 429 -Kind 'expense' -Scope 'personal' -StartDate (MonthDay 2 20)

# Pasif plan para uretmez: bekleyen kayitlari listelerden duser, gerceklestirme
# reddedilir. Kabul turunda bunun gorulmesi icin bir tane pasife alinir.
$dormantPlan = New-Recurring -Description 'Eski depo kirasi' -SourceType 'account' -SourceId $bank.id `
    -CategoryId $rent.id -Amount 4200 -Kind 'expense' -Scope 'business' -StartDate (MonthDay 2 12)
Invoke-Api -Method Patch -Path "/api/v1/recurring-transactions/$($dormantPlan.id)/active" `
    -Body @{ isActive = $false } | Out-Null

Invoke-Api -Method Post -Path '/api/v1/recurring-transactions/occurrences/generate' -Body @{
    throughDate = ($today.AddDays(45)).ToString('yyyy-MM-dd')
} | Out-Null

# Gecmis donemlerin bir kismi gerceklestirilir, gelecek donemler planli kalir -
# planlanan gorunumun dolu olmasi icin.
$occurrences = Invoke-Api -Method Get -Path '/api/v1/recurring-transactions/occurrences?pageSize=100'
$realized = 0
foreach ($occurrence in $occurrences.items) {
    if ($realized -ge 6) { break }
    if ($occurrence.status -ne 'planned') { continue }
    if ([datetime]$occurrence.scheduledDate -ge $today) { continue }
    $result = Invoke-Api -Method Post `
        -Path "/api/v1/recurring-transactions/occurrences/$($occurrence.id)/realize" `
        -Body @{ amount = $null } -Tolerate
    if ($null -ne $result) { $realized++; Add-Count 'gerceklesen tekrar' }
}

Write-Output 'Cari hesap...'
# Borclandirma TANIR (gelir/gider yazar, bakiye kipirdamaz); tahsilat TASIR
# (bakiyeyi degistirir, gelir/gider uretmez) - ADR 0014.
$schoolCharge = Invoke-Api -Method Post -Path "/api/v1/counterparties/$($school.id)/charges" -Body @{
    direction       = 'receivable'
    amount          = (Money 14200)
    currency        = 'TRY'
    categoryId      = $sales.id
    chargeDate      = (Day -24)
    scope           = 'business'
    description     = 'Donem basi toplu kirtasiye'
    dueDate         = (Day 6)
    vatRate         = '0.2000'
    vatAmount       = '2366.6700'
}
Add-Count 'cari borclandirma'
$null = $schoolCharge

Invoke-Api -Method Post -Path "/api/v1/counterparties/$($school.id)/payments" -Body @{
    direction   = 'receivable'
    amount      = (Money 9400)
    currency    = 'TRY'
    accountId   = $bank.id
    paymentDate = (Day -13)
    description = 'Kismi tahsilat'
} | Out-Null
Add-Count 'cari tahsilat'

Invoke-Api -Method Post -Path "/api/v1/counterparties/$($wholesaler.id)/charges" -Body @{
    direction       = 'payable'
    amount          = (Money 15757.20)
    currency        = 'TRY'
    categoryId      = $goods.id
    chargeDate      = (Day -13)
    scope           = 'business'
    description     = 'Vadeli kagit ve kirtasiye alimi'
    dueDate         = (Day 17)
    vatRate         = '0.2000'
    vatAmount       = '2626.2000'
    isTaxDeductible = $true
} | Out-Null
Add-Count 'cari borclandirma'

Invoke-Api -Method Post -Path "/api/v1/counterparties/$($wholesaler.id)/payments" -Body @{
    direction   = 'payable'
    amount      = (Money 7500)
    currency    = 'TRY'
    accountId   = $bank.id
    paymentDate = (Day -5)
    description = 'Agustos cari hesap odemesi'
} | Out-Null
Add-Count 'cari tahsilat'

# Fazla tahsilat kirpilmaz: bakiye ters yone gecer ve oyle gorunur.
Invoke-Api -Method Post -Path "/api/v1/counterparties/$($neighbour.id)/charges" -Body @{
    direction   = 'receivable'
    amount      = (Money 2800)
    currency    = 'TRY'
    categoryId  = $sales.id
    chargeDate  = (Day -19)
    scope       = 'business'
    description = 'Veresiye satis'
} | Out-Null
Add-Count 'cari borclandirma'

Invoke-Api -Method Post -Path "/api/v1/counterparties/$($neighbour.id)/payments" -Body @{
    direction   = 'receivable'
    amount      = (Money 3300)
    currency    = 'TRY'
    accountId   = $cash.id
    paymentDate = (Day -3)
    description = 'Fazla odeme yapildi'
} | Out-Null
Add-Count 'cari tahsilat'

# Pasif karsi tarafa yeni borclandirma yazilamaz, tahsilat yazilabilir.
Invoke-Api -Method Put -Path "/api/v1/counterparties/$($dormant.id)" -Body @{
    name     = $dormant.name
    isActive = $false
    note     = 'Artik calisilmiyor'
} | Out-Null

Write-Output 'Borclar...'
# Borc acilisi bir kaynak tasir: nakit girdi mi, bir sey mi tuketildi.
$payable = Invoke-Api -Method Post -Path '/api/v1/debts' -Body @{
    counterpartyName  = 'Ornek Ekipman'
    direction         = 'payable'
    scope             = 'business'
    principal         = (Money 18000)
    totalRepayment    = (Money 19800)
    currency          = 'TRY'
    sourceType        = 'expense'
    categoryId        = $goods.id
    startDate         = (MonthDay 2 10)
    firstDueDate      = (MonthDay 1 10)
    installmentCount  = 6
    description       = 'Vitrin dolabi - senetli alim'
    asOfDate          = (Day 0)
}
Add-Count 'borc'
foreach ($sequence in 1..2) {
    Invoke-Api -Method Post -Path "/api/v1/debts/$($payable.id)/installments/$sequence/pay" -Body @{
        accountId   = $bank.id
        paymentDate = (MonthDay (2 - $sequence) 10)
        asOfDate    = (Day 0)
    } -Tolerate | Out-Null
    Add-Count 'borc taksidi odemesi'
}

$receivable = Invoke-Api -Method Post -Path '/api/v1/debts' -Body @{
    counterpartyName  = 'Ornek Alici'
    direction         = 'receivable'
    scope             = 'business'
    principal         = (Money 9000)
    totalRepayment    = (Money 9600)
    currency          = 'TRY'
    sourceType        = 'income'
    categoryId        = $sales.id
    startDate         = (MonthDay 1 14)
    firstDueDate      = (Day 9)
    installmentCount  = 3
    description       = 'Ikinci el fotokopi makinesi satisi'
    asOfDate          = (Day 0)
}
Add-Count 'borc'
$null = $receivable

Write-Output 'Yukumlulukler...'
# Yukumluluk vadesi olan, henuz odenmemis bir borctur; odendiginde kapanir.
$openObligation = Invoke-Api -Method Post -Path '/api/v1/obligations' -Body @{
    direction       = 'payable'
    amount          = (Money 3950.88)
    currency        = 'TRY'
    categoryId      = $utilities.id
    issueDate       = (Day -2)
    dueDate         = (Day 12)
    scope           = 'business'
    description     = 'Isyeri elektrik faturasi'
    vatRate         = '0.2000'
    vatAmount       = '658.4800'
    isTaxDeductible = $true
}
Add-Count 'yukumluluk'
$null = $openObligation

$overdue = Invoke-Api -Method Post -Path '/api/v1/obligations' -Body @{
    direction       = 'payable'
    amount          = (Money 1180)
    currency        = 'TRY'
    categoryId      = $otherBusiness.id
    issueDate       = (Day -35)
    dueDate         = (Day -11)
    scope           = 'business'
    description     = 'Gecikmis internet faturasi'
    isTaxDeductible = $true
}
Add-Count 'yukumluluk (gecikmis)'
$null = $overdue

$settled = Invoke-Api -Method Post -Path '/api/v1/obligations' -Body @{
    direction       = 'payable'
    amount          = (Money 2400)
    currency        = 'TRY'
    categoryId      = $accountant.id
    issueDate       = (Day -30)
    dueDate         = (Day -16)
    scope           = 'business'
    description     = 'Muhasebeci temmuz ucreti'
    isTaxDeductible = $true
}
Add-Count 'yukumluluk'
Invoke-Api -Method Post -Path "/api/v1/obligations/$($settled.id)/settlement" -Body @{
    accountId      = $bank.id
    settlementDate = (Day -16)
} -Tolerate | Out-Null

Write-Output 'POS tahsilatlari...'
# Yoldaki para: kullanilabilir bakiye ile net varlik arasindaki fark tam olarak
# bu tutardir.
$transferred = Invoke-Api -Method Post -Path '/api/v1/pos-settlements' -Body @{
    accountId             = $bank.id
    categoryId            = $sales.id
    grossAmount           = (Money 12750)
    currency              = 'TRY'
    settlementDate        = (Day -16)
    expectedTransferDate  = (Day -14)
    commissionAmount      = (Money 223.13)
    commissionCategoryId  = $commission.id
    scope                 = 'business'
    description           = 'Gun sonu POS'
    vatRate               = '0.2000'
    vatAmount             = '2125.0000'
}
Add-Count 'POS tahsilati'
Invoke-Api -Method Post -Path "/api/v1/pos-settlements/$($transferred.id)/transfer" -Body @{
    transferDate = (Day -14)
} -Tolerate | Out-Null

$inTransit = Invoke-Api -Method Post -Path '/api/v1/pos-settlements' -Body @{
    accountId             = $bank.id
    categoryId            = $sales.id
    grossAmount           = (Money 22650)
    currency              = 'TRY'
    settlementDate        = (Day -2)
    expectedTransferDate  = (Day 2)
    commissionRate        = '0.0175'
    commissionCategoryId  = $commission.id
    scope                 = 'business'
    description           = 'Gun sonu POS - yolda'
    vatRate               = '0.2000'
    vatAmount             = '3775.0000'
}
Add-Count 'POS tahsilati (yolda)'
$null = $inTransit

Write-Output 'Kasa sayimi...'
# Farkli bir sayim: fark onaylanana kadar hicbir kayit uretmez.
# Sayilan tutar hesabin o anki bakiyesinden turetilir. Sabit bir sayi vermek,
# fikstur buyudukce bakiyeden uzaklasir ve kasadan kaybolmus binlerce liralik
# hayali bir fark uretirdi.
$cashState = Invoke-Api -Method Get -Path "/api/v1/accounts/$($cash.id)"
$countedAmount = [decimal]$cashState.balance - 85
$count = Invoke-Api -Method Post -Path '/api/v1/cash-counts' -Body @{
    accountId     = $cash.id
    countedAmount = (Money $countedAmount)
    countDate     = (Day -1)
    scope         = 'business'
    note          = 'Gun sonu sayimi'
} -Tolerate
if ($null -ne $count) {
    Add-Count 'kasa sayimi'
    Invoke-Api -Method Post -Path "/api/v1/cash-counts/$($count.id)/adjustment" -Body @{
        categoryId = $otherBusiness.id
    } -Tolerate | Out-Null
}

Write-Output 'Butceler...'
# Butce ilerlemesi kategori + kapsam ciftiyle toplanir.
function New-Budget {
    param([string]$CategoryId, [decimal]$Limit, [string]$Scope)
    $body = @{
        categoryId = $CategoryId
        limit      = (Money $Limit)
        currency   = 'TRY'
        scope      = $Scope
        year       = $today.Year
        month      = $today.Month
    }
    $created = Invoke-Api -Method Post -Path '/api/v1/budgets' -Body $body -Tolerate
    if ($null -ne $created) { Add-Count 'butce' }
}

New-Budget -CategoryId $goods.id -Limit 25000 -Scope 'business'
New-Budget -CategoryId $vehicle.id -Limit 2000 -Scope 'business'   # bilerek asilir
New-Budget -CategoryId $market.id -Limit 6000 -Scope 'personal'
New-Budget -CategoryId $dining.id -Limit 2500 -Scope 'personal'

Write-Output 'Hedefler...'
$goal = Invoke-Api -Method Post -Path '/api/v1/goals' -Body @{
    name         = 'Yeni vitrin'
    targetAmount = (Money 30000)
    currency     = 'TRY'
    targetDate   = ($today.AddMonths(5)).ToString('yyyy-MM-dd')
    trackingMode = 'manual-contributions'
    description  = 'Elle katki ile takip'
    asOfDate     = (Day 0)
    scope        = 'business'
} -Tolerate
if ($null -ne $goal) {
    Add-Count 'hedef'
    foreach ($offset in @(-40, -20, -6)) {
        Invoke-Api -Method Post -Path "/api/v1/goals/$($goal.id)/contributions" -Body @{
            amount           = (Money 2500)
            currency         = 'TRY'
            contributionDate = (Day $offset)
            clientRequestId  = [guid]::NewGuid().ToString()
            note             = 'Aylik ayirma'
            asOfDate         = (Day 0)
        } -Tolerate | Out-Null
        Add-Count 'hedef katkisi'
    }
}

$balanceGoal = Invoke-Api -Method Post -Path '/api/v1/goals' -Body @{
    name         = 'Vergi karsiligi'
    targetAmount = (Money 50000)
    currency     = 'TRY'
    targetDate   = ($today.AddMonths(3)).ToString('yyyy-MM-dd')
    trackingMode = 'account-balance'
    accountId    = $savings.id
    description  = 'Birikim hesabinin bakiyesiyle takip'
    asOfDate     = (Day 0)
    scope        = 'personal'
} -Tolerate
if ($null -ne $balanceGoal) { Add-Count 'hedef' }

Write-Output 'Belge ekleri...'
# Belge rastgele bir harekete degil, anlattigi harekete baglanir: kabul turunda
# eki acan kisi fisin ustundeki tutarla kaydin tutarini karsilastirabilsin.
$documentMap = [ordered]@{
    'fis-market-01.jpg'          = 'Haftalik market'
    'fatura-elektrik-02.jpg'     = 'Elektrik faturasi'
    'fis-kirtasiye-03-soluk.jpg' = 'Toner ve zimba teli'
    'dekont-eft-01.jpg'          = 'SGK primi'
}

if (Test-Path -LiteralPath $DocumentPath) {
    $recent = Invoke-Api -Method Get -Path '/api/v1/transactions?pageSize=100'
    foreach ($entry in $documentMap.GetEnumerator()) {
        $document = Join-Path $DocumentPath $entry.Key
        if (-not (Test-Path -LiteralPath $document)) {
            Write-Warning "Belge yok: $($entry.Key)"
            continue
        }

        $target = $recent.items | Where-Object { $_.description -eq $entry.Value } | Select-Object -First 1
        if (-not $target) {
            Write-Warning "Eklenecek hareket bulunamadi: $($entry.Value)"
            continue
        }

        # `Invoke-RestMethod -Form` PowerShell 7 ile geldi; betik Windows
        # PowerShell 5.1'de de kosuyor. curl.exe her iki kabukta da var.
        $uploaded = & curl.exe --silent --show-error --fail-with-body `
            --request POST `
            --header "Authorization: Bearer $script:AccessToken" `
            --form "file=@$document" `
            "$BaseUrl/api/v1/transactions/$($target.id)/attachments" 2>&1
        if ($LASTEXITCODE -eq 0) { Add-Count 'belge eki' }
        else { Write-Warning "Ek yuklenemedi ($($entry.Key)): $uploaded" }
    }
}
else {
    Write-Warning "Belge klasoru bulunamadi: $DocumentPath"
}

Write-Output ''
Write-Output 'Fikstur yazildi:'
$script:Created.GetEnumerator() | ForEach-Object { Write-Output ("  {0,-26} {1}" -f $_.Key, $_.Value) }
Write-Output ''
Write-Output "Hesap: $Email"
