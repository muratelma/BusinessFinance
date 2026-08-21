# ADR 0006 — Finansal Renkler `ColorScheme`'de Değil, Ayrı Bir `ThemeExtension`'da

- Durum: Kabul edildi; Aşama 12.6 Grup 1'de uygulandı
- Tarih: 2026-08-15
- Kapsam: `lib/core/theme/app_finance_colors.dart`, `AppTheme`, tutar ve durum
  gösteren bütün Flutter bileşenleri

## Bağlam

Uygulama gelir, gider ve nötr hareketi renkle ayırt ediyor. Bu ayrım ürün
kararıdır ve Aşama 12.5'te açıkça sabitlendi: **yeşil yalnız gelirdir**, nötr
hareket (transfer, kart borcu ödemesi) gelirle karışmayacak kendi tonunu
taşır.

On iki aşama boyunca bu renkler ekran ekran, ham Material paletinden seçildi:

```dart
ActivityEffect.income => Colors.green.shade700,
ActivityEffect.neutral => Colors.deepPurple.shade400,
```

Aşama 12.6'da kontrast kapısı kurulunca bu değerlerin hiçbirinin WCAG AA
eşiğini geçmediği ölçüldü:

| Renk | Zemin | Ölçülen | Gereken |
|---|---|---|---|
| `Colors.green.shade700` | karanlık kart | 4,16:1 | 4,5:1 |
| `Colors.green.shade700` | aydınlık kart | 3,72:1 | 4,5:1 |
| `Colors.deepPurple.shade400` | karanlık kart | 3,28:1 | 4,5:1 |
| `Colors.green` | aydınlık yüzey | 2,65:1 | 4,5:1 |

Sabit renkler tanım gereği temaya tepki vermez: aydınlık yüzey için seçilen
bir ton karanlık yüzeyde okunmaz, tersi de geçerlidir.

## Değerlendirilen seçenekler

### 1. Material rollerine bindirmek

`income → colorScheme.tertiary`, `neutral → colorScheme.secondary` gibi.

Reddedildi. `ColorScheme` rolleri **görsel hiyerarşiyi** anlatır (birincil
eylem, ikincil vurgu), finansal anlamı değil. Seed rengi değiştiği anda
`tertiary` başka bir renge kayar ve "gelir" sessizce başka bir şey olur — üstelik
hiçbir test bunu yakalamaz, çünkü kod hâlâ "tertiary" diyordur. Ayrıca
`colorScheme.error` gider için doğal görünse de `error` bir **hata** durumudur;
gider hata değildir ve ikisinin ileride ayrışması gerekebilir.

### 2. Ham sabitleri korumak, yalnız değerleri düzeltmek

Reddedildi. Kontrast sorununu bir kez çözer ama yapıyı çözmez: değerler yine
çağrı yerlerine dağılmış kalır, karanlık tema için ikinci bir set gerekir ve
bir sonraki ekran yine kendi tonunu seçer.

### 3. Ayrı `ThemeExtension` (seçilen)

`AppFinanceColors extends ThemeExtension<AppFinanceColors>`; aydınlık ve
karanlık için iki tam palet, `AppTheme` tarafından ilgili temaya kaydedilir.

## Karar

Finansal anlam renkleri `ColorScheme`'den bağımsız, kendi tema uzantısında
yaşar.

```text
AppFinanceColors
├── income     + incomeContainer     + onIncomeContainer
├── expense    + expenseContainer    + onExpenseContainer
├── neutral    + neutralContainer    + onNeutralContainer
├── planned    + plannedContainer    + onPlannedContainer
└── cancelled  + cancelledContainer  + onCancelledContainer

AppTheme.light() -> AppFinanceColors.light
AppTheme.dark()  -> AppFinanceColors.dark
```

- `*` alanları yüzey üzerindeki metin/ikon rengidir.
- `*Container` + `on*Container` çiftleri rozet ve chip içindir.
- `AppFinanceColors.of(context)` uzantı yoksa **sessiz varsayılana düşmez**,
  `FlutterError` atar. Tema kurulumu atlanırsa renkler sessizce yanlış olmak
  yerine görünür biçimde patlar.

## Sonuçlar

### Kazanılan

- Kontrast **ölçülebilir** hale geldi: her token, üç yüzeye karşı iki
  parlaklıkta birim testle doğrulanıyor. Renk seçimi artık gözle onaylanan bir
  zevk meselesi değil, geçmesi gereken bir kapı.
- "Yeşil yalnız gelirdir" kuralı token adına gömüldü; `income` alanını başka
  bir anlam için kullanmak okunduğu anda yanlış görünür.
- Karanlık tema ayrı bir palet aldı; aynı paleti iki temada kullanma
  regresyonu testle kapatıldı.
- Yeni ekranlar renk uydurmaz: yapısal kapı (`design_tokens_test.dart`) ham
  `Colors.*` kullanımını kaynak taramasıyla reddediyor.

### Ödenen bedel

- Tema kuran her test harness'ı `AppTheme.light()` ya da `AppTheme.dark()`
  vermek zorunda. Bu bir maliyet gibi görünse de aslında sapmayı düzeltti:
  testler temasız `MaterialApp` kuruyordu, uygulama ise her zaman temalı.
- Beş rol × üç renk × iki tema = 30 değer elle bakımlanıyor. Material'ın
  ton üreticisinden türetmek mümkündü ama üretilen tonların kontrast eşiğini
  geçtiği garanti değil; açık değerler ve ölçen bir test daha güvenli.

### Bu kararın dışında kalan

- Renk körlüğü. Kontrast oranı parlaklık farkını ölçer, ton farkını değil ve
  yeşil/kırmızı ayrımı hiçbir sayısal ölçütle garanti edilemez. Koruma
  renkten değil, **her zaman ikon ve metin** kuralından gelir; bu kural
  `AppStatusChip`'in imzasına (ikon ve etiket zorunlu alan) gömüldü.
- Kullanıcının tema seçmesi. `ThemeMode.system` olduğu gibi kalır.
- Marka paleti veya özel font. Material 3 `ColorScheme.fromSeed` temeli
  değişmedi; bu ADR yalnız finansal anlam renklerini kapsar.
