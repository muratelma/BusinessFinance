# Fiş analizi ölçüm koşumu

Bu araç fiş analizi geliştirmesindeki **manuel veri toplama**
kapısıdır. Üretim uygulamasına bağlanmaz, gerçek finansal veri kabul etmez ve
yalnız kodla çizilen 30 sentetik fişi kullanır.

## Koşum

```powershell
dotnet run --project src/BusinessFinance.ReceiptMeasurement `
  --configuration Release -- dry-run

dotnet run --project src/BusinessFinance.ReceiptMeasurement `
  --configuration Release -- preprocessing --confirm-calls 60

dotnet run --project src/BusinessFinance.ReceiptMeasurement `
  --configuration Release -- models --confirm-calls 30 `
  --preprocessor noop --retry-failures

dotnet run --project src/BusinessFinance.ReceiptMeasurement `
  --configuration Release -- summary

dotnet run --project src/BusinessFinance.ReceiptMeasurement `
  --configuration Release -- field --path artifacts/receipt-field-test/images
```

`field`, sentetik setin ölçemediğini ölçer: **gerçek** fotoğrafları üretim boru
hattının tamamından (istemci JPEG'i, ön işleme, `AnalyzeReceiptUseCase`, belge
türü kapısı, doğrulayıcı) geçirir. Anahtarı `BUSINESS_FINANCE_GEMINI_TEST_KEY`
yoksa API user-secrets'tan okur ve hiçbir yere yazmaz. İlk koşumun sonucu:
2026-08-19 saha koşumu kaydında; belge önceki repoda (`Kisisel-Butce-Mobil`).

Gerçek iki koşu `BUSINESS_FINANCE_GEMINI_TEST_KEY` environment değişkenini
zorunlu tutar. Anahtar URL'ye, konsola veya sonuç dosyasına yazılmaz. Üretim
adapter'ı sağlayıcıya `store=false` gönderir.

Sonuçlar varsayılan olarak Git tarafından yok sayılan
`artifacts/receipt-measurement/results.jsonl` dosyasına her logical call'dan
sonra eklenir. Aynı komut tekrar çalıştırıldığında başarılı satırlar atlanır;
`--retry-failures` yalnız başarısız/eksik işi tekrar dener. 429 veya sağlayıcı
erişim hatası kalan işleri durdurur.

Ön işleme turunda çağrılar arasında en az 4 saniye, model turunda en az 12
saniye vardır. 3.7 Flash için üretim adapter'ının tek retry'ı da fiziksel çağrı
sayılır; araç her işten önce iki slot ayırır ve günlük 20 fiziksel çağrı
sınırını aşmadan durur. Sayaç eklenmeden önce yazılmış satırlar güvenli tarafta
kalmak için ikişer çağrı sayılır.

## Referans ve puanlama

Her fiş için işletme, tarih, ara toplam, KDV ve genel toplam kaynak kodda sabit
referanstır. Karar metriği işletme, tarih, KDV ve genel toplamdan oluşan dört
alandır; ara toplam ham sonuçta L4 tutarlılık incelemesi için saklanır.

İşletme başlığındaki son iki rakam sentetik vaka kimliğidir (`01…30`), işletme
adının parçası değildir. Prompt şube bilgisini çıkar dediği için puanlama bu
eki hem beklenen hem gerçek değerde yok sayar. Ham değerler JSONL'de aynen
kalır; özet güncel puanlama kuralıyla yeniden hesaplanır.

## 18 Ağustos 2026 sonucu

### A — ön işleme (tamamlandı)

| Varyant | Fiş | Başarılı | Alan doğruluğu | Ortalama gecikme | Ortalama token |
|---|---:|---:|---:|---:|---:|
| NoOp | 30 | 30 | 120/120 — %100 | 7.321 ms | 1.561,4 |
| Normalize | 30 | 30 | 120/120 — %100 | 8.246 ms | 1.561,2 |

Doğruluk ve token maliyeti eşit kaldı; normalize hattı sentetik sette ölçülen
bir kazanç göstermedi ve ortalama 925 ms daha yavaştı. B koşusunda daha az
dönüşüm yapan **NoOp** sabitlendi. Bu seçim yalnız ölçümün B girdisidir;
üretim DI ayarı bu turdan ötürü değişmedi (gerekçesi aşağıdaki kapsam
bölümündedir).

#### Sonucun kapsamı: neyi ölçtü, neyi ölçmedi

Set kusursuz fişlerden ibaret değildir; R11–R30 bilerek bozulmuş görsellerdir
(`SyntheticReceiptSet.Colours` ve `DrawPaperTexture`/`DrawForegroundDefects`):
koyu zemin + gri mürekkep, soluk termal baskı, düşük kontrast, 1.400 noktalık
gürültü, genişliğin %42'sini kaplayan gölge, kırışık çizgileri, 2,8° eğim,
1800×3000 büyük görsel ve 900×2600 uzun fiş.

Buna göre ön işlemecinin üç işinden **ikisi gerçekten sınandı**:

| Ön işleme adımı | Sette karşılığı | Sonuç |
|---|---|---|
| Karanlık/düşük kontrast fotoğrafı kaldırma | `Dark`, `Faded`, `LowContrast` fişleri | Sınandı — doğrulukta fark yok |
| Büyük fotoğrafı ölçekleme | `Oversized` 1800×3000, `Long` 900×2600 | Sınandı — doğrulukta fark yok |
| **EXIF rotasyonunu piksele işleme** | **Yok** | **Hiç sınanmadı** |

Yani kontrast ve ölçekleme için bu **gerçek bir sonuçtur**: bozulmuş fişlerde
bile model ön işleme olmadan da 120/120 okudu.

Sınanmayan tek adım EXIF rotasyonudur ve sebebi yapısaldır: `Tilted` varyantı
eğimi **piksele çizerek** üretir, EXIF etiketi yazmaz — Skia EXIF yazmaz. Oysa
`AutoOrient`'in düzelttiği şey, telefonun kendi çektiği fotoğrafa koyduğu
90°/180°'lik yön etiketidir. 2,8°'lik çizili eğim onun yerine geçmez.

Bu yüzden:

- NoOp'un B turunda kullanılması doğrudur (daha az dönüşüm, daha az karışan
  değişken).
- **Üretim varsayılanı `ReceiptImagePreprocessor` olarak kalır** — kontrast ve
  ölçekleme kazanç göstermemiş olsa da zarar da göstermedi, ve rotasyon adımı
  hâlâ sınanmamış bir kazançtır. Kanıtsız kapatmak, kanıtsız açık tutmaktan
  daha riskli olurdu.
- Rotasyonun değeri ancak **telefonla çekilmiş** fişlerle ölçülebilir. O ölçüm
  Grup 5 sonrası Pixel 8 kabul turuna aittir; `ReceiptImagePreprocessorTests`
  rotasyonun teknik olarak çalıştığını zaten kanıtlıyor, eksik olan doğruluğa
  katkısının ölçüsüdür.

### B — model karşılaştırması (6/15 tam çiftte kullanıcı kararıyla kapatıldı)

İlk altı zor vaka iki modelde de tamamlandı:

| Model | Tam çift | Başarılı alan | Ortalama gecikme | Ortalama token |
|---|---:|---:|---:|---:|
| Gemini 3.7 Flash | 6/15 | 24/24 — %100 | 28.547 ms | 1.790,3 |
| Gemini 3.5 Flash Lite | 6/15 | 24/24 — %100 | 10.086 ms | 1.560,5 |

3.7 Flash'ın R13 çağrısı önce iki denemede sağlayıcı erişim hatası verdi,
resume'da geçti. R17 çağrısı `receipt.provider_rate_limited` döndürünce araç
durdu; Lite'ın R17 eşi çağrılmadı. 3.7'nin ilk başarılı çağrısı 102.473 ms,
Lite'ın R16 çağrısı 46.970 ms sürdü; ortalamalar bu iki uç değeri içerir.

#### Karar: `Gemini 3.5 Flash Lite` (18 Ağustos 2026, kullanıcı kararı)

Tur 15/15'e ulaşmadan kapatıldı. Bu, örneklem hedefinden bilerek verilmiş bir
ödündür ve gerekçesi kararın **hangi yöne** bakmasıdır: eksik 9 çift yalnız
3.7 Flash lehine bir itiraz taşıyabilirdi, Lite lehine olan tabloyu
değiştiremezdi. Aşamanın baştan yazılı karar kuralı zaten şudur — *3.7 yalnız
alan bazlı doğrulukta belirgin ve tekrarlanabilir bir üstünlük gösterirse
seçilir; fark küçükse RPD 500 kazanır.* Ölçülen altı çiftte 3.7'nin üstünlüğü
sıfırdır.

İki modelin arkasındaki ölçüm hacmi de eşit değildir:

| Model | Ölçülen çağrı | Doğru alan | Kapsanan fiş |
|---|---:|---:|---|
| Gemini 3.5 Flash Lite | 66 | 264/264 — %100 | 30 fişin tamamı (A, iki varyant) + 6 zor vaka |
| Gemini 3.7 Flash | 6 | 24/24 — %100 | 6 zor vaka |

Lite, bilerek bozulmuş R11–R30 görselleri dahil **setin tamamında** iki kez
okundu ve tek alan düşürmedi. Yani seçilen model az ölçülen değil, çok ölçülen
taraftır; eksik kalan ölçüm reddedilen modelin ölçümüdür.

Kota tarafı bunu pekiştirir: RPD 20, Pixel 8'de tek bir elle kabul oturumunun
günlük bütçeyi bitirmesi demektir — B turunun kendisi tam da bu duvara çarparak
durdu. RPD 500 bu özelliği günde 20 fişe hapsetmeyen tek seçenektir.

**Üretim yapılandırması değişmedi**: `GeminiOptions.Model` zaten
`gemini-3.5-flash-lite` idi ve öyle kalıyor. `TimeoutSeconds = 60` ölçülen
Lite uç değerine (46.970 ms) göre doğrudur; 3.7'ye göre yeniden bakma şartı
düştü. Ön işleme varsayılanı `ReceiptImagePreprocessor` olarak kalır (yukarıdaki
kapsam bölümü).

#### Bu kararın ölçmediği, ileride ölçülecek olan

- **Kalan 9 zor vaka (R17–R25) hiçbir modelde okunmadı.** Lite'ın bu fişlerdeki
  doğruluğu A turundan biliniyor (aynı görseller, tek çağrı), B'deki ikinci
  okuma tekrarlanabilirliği sınayacaktı; sınanmadı.
- **3.7 Flash'ın 6 çiftten sonrası bilinmiyor.** "3.7 daha kötü" denmiyor;
  "daha iyi olduğu gösterilmedi" deniyor. Karar kuralı zaten kanıt yükünü
  3.7'ye vermişti.
- **Sentetik fiş gerçek fotoğraf değildir.** Bütün set kodla çizilmiştir; asıl
  sınav Grup 5 sonrası Pixel 8 ile çekilmiş fişlerdir.

**Yeniden karşılaştırma tetikleyicileri.** Şunlardan biri olursa çift yeniden
ölçülür, tercihle değil ölçümle: gerçek telefon fişlerinde Lite'ın alan
doğruluğu düşerse; ücretli katmana geçilip RPD 20 kısıtı kalkarsa; ya da
sağlayıcı yeni bir Lite kuşağı yayımlarsa. Araç, JSONL ve puanlama aynı
komutla durduğu yerden devam eder.
