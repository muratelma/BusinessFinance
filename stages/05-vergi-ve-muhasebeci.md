# Aşama 05 — Vergi ve muhasebeci

## Belge durumu

- Durum: **Aktif** (26 Ağustos 2026'da kullanıcı onayıyla açıldı)
- Ön koşul: Aşama 04 — Kasa, POS ve gezinme
- Sonraki aşama: Aşama 06 — Bulut güvenli beta
- Karar kapısı: `documentation/adr/0016-tax-fields-carry-they-do-not-calculate.md`
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/flows.md`, `documentation/permissions.md`,
  `documentation/tests.md`, `documentation/variables.md`,
  `documentation/restore-runbook.md`, **yeni ADR** (aşağıda)
- Doğrulanmış ilerleme: `docs/project-status.md`

## Amaç

Esnafın ay sonunda yaptığı iş belgeleri toplayıp muhasebeciye göndermek. Bugün
uygulama işletme hareketlerini biliyor ama KDV'yi taşımıyor, hangi giderin
indirilebilir olduğunu bilmiyor ve muhasebeciye giden paketi üretmiyor.

Bu aşama o boşluğu kapatıyor — **hesaplamadan**. Ürün beyanname üretmez ve
vergi hesaplamaz; taşır ve raporlar.

## Kullanıcıya katkı

- Faturadaki KDV'yi kaydın üstünde tutmak ve ay sonunda toplamını görmek
- İndirilemeyen giderleri işaretleyip muhasebeciye yanlış rakam göndermemek
- Beyan ve ödeme tarihlerini uygulamada görmek, kaçırmamak
- Ay sonunda tek dosyayla muhasebeciye göndermek
- KDV için kenara para ayırmak ve ne kadar biriktiğini görmek

## Değiştirilmeyecek mimari kararlar

- **Ürün vergi hesaplamaz ve beyanname üretmez.** Vergiye dair her alan taşıyan
  ve raporlayan alandır.
- **Kapsam ile indirilebilirlik ayrı alanlardır** (ADR 0013 madde 5). Tek alanda
  birleştirilmez.
- Ekonomik olay tanır, ödeme taşır (Aşama 02 ADR'si).
- Kapsam boyutu ve türetme zinciri (ADR 0013).
- Bakiye ve türetilen tutarlar kalıcı kolon değildir.
- "Kâr" kelimesi kullanılmaz; hesaplanan nakit esaslı işletme netidir.

## Bu aşamanın karar kapısı: mevzuat takibi ürünün işi değildir

Aşama başlamadan **bir ADR yazılır.** Vergi oranları ve beyan tarihleri değişir;
uygulama bunları takip edemez ve etmeye çalışması kullanıcıyı yanıltır.

ADR'nin karara bağlayacağı üç şey:

1. **Oranlar ve tarihler koda gömülmez.** Tek bir yapılandırma kaynağında
   durur, kullanıcı düzenleyebilir ve uygulama "bu bilgi sizin girdiğinizdir"
   der.
2. **Uygulama hiçbir vergi tutarı hesaplamaz.** KDV tutarı kullanıcıdan veya
   belgeden gelir; uygulama yalnız toplar ve gruplar.
3. **Takvim bir hatırlatmadır, yükümlülük beyanı değildir.** Kaçırılan tarihin
   sorumluluğu kullanıcıdadır ve bu ekranda yazılıdır.

## Kapsam

### Dahil

- Hareket başına KDV oranı ve tutarı (taşınan alan)
- İndirilebilirlik bayrağı; kategori bazlı varsayılan
- Vergi ve SGK takvimi (kullanıcı tarafından düzenlenebilir)
- Ay sonu muhasebeci paketi
- Tasarruf hedefinin "karşılık" olarak konumlanması ve hedefe kapsam eklenmesi
- Yedek şeması v10

### Açıkça kapsam dışında

- **Beyanname üretme, vergi hesaplama, tevkifat, istisna kuralları**
- KDV oranının belgeden otomatik türetilmesi ve doğrulanması
- e-Fatura / e-Arşiv / e-Defter entegrasyonu
- Muhasebeci hesabı, çok kullanıcılı erişim, paylaşım linki
- Amortisman, kıst dönem, geçici vergi hesaplaması
- Personel bordrosu ve SGK prim hesaplaması

## Çalışma grupları

### Grup 1 — ADR: vergi alanları taşır, hesaplamaz

- Yukarıdaki üç karar; reddedilen seçenekler (oranı koda gömmek, KDV'yi
  tutardan hesaplamak, takvimi mevzuata bağlamak) gerekçeleriyle yazılır.
- ADR yazıldı: `documentation/adr/0016-tax-fields-carry-they-do-not-calculate.md`
  (26 Ağustos 2026). Üç kararın yanına dördüncüsü eklendi: takvim bir
  hatırlatmadır ve muhasebeci paketi ikinci bir hesaplama yolu değil, aynı ayın
  işletme raporunun okumasıdır.
- Ölçüt: ADR kabul edildi ve `CLAUDE.md` belge haritasına eklendi.
- **Tamamlandı** (26 Ağustos 2026): ADR kullanıcı tarafından kabul edildi.

### Grup 2 — Domain: KDV taşıyan alanlar

- Gider/gelir üreten kayıtlara **nullable** KDV oranı ve KDV tutarı: her
  harekette KDV yoktur ve boş bırakmak meşru bir cevaptır.
- **Tutar hesaplanmaz.** Kullanıcı brüt tutarı ve KDV tutarını girer; uygulama
  ikisi arasında tutarlılık **uyarısı** verebilir ama değeri düzeltmez.
- Para hassasiyeti kuralı aynen geçerli: `decimal(19,4)`, API'de string.
- Ölçüt: KDV alanı boş bırakılabiliyor; hiçbir yerde otomatik bölme yok.
- **Tamamlandı** (26 Ağustos 2026): `VatDetails` değer nesnesi yazıldı ve
  gelir/gider **tanıyan** beş kayıt onu taşıyor (`BudgetTransaction`,
  `CreditCardCharge`, `CounterpartyCharge`, `Obligation`, `PosSettlement`).
  Parayı yalnız taşıyan kayıtlar KDV taşımıyor. Oran ve tutar bağımsız iki
  nullable alan; biri diğerinden türetilmiyor ve uyuşmazlık reddedilmiyor.
  `ImpliedAmount` yalnız uyarı içindir ve hiçbir alanı doldurmuyor. Beş tabloya
  iki nullable kolon (`AddVatFields`) ve iki CHECK kısıtı eklendi; sözleşmede
  isteğe bağlı `vatRate`/`vatAmount` alanları ve cevapta tek `vat` nesnesi var.
  KDV hiçbir toplamı değiştirmiyor — rapor brüt kalıyor.

### Grup 3 — Domain: indirilebilirlik

- **Kapsamdan ayrı**, kendi alanı. Yalnız işletme kapsamlı kayıtlarda anlamlı;
  şahsi kayıtta sorulmaz.
- Kategori bazlı varsayılan taşır: trafik cezası indirilemez, ticari mal alımı
  indirilir. Kullanıcı her kayıtta düzeltebilir.
- Kısmi indirilebilirlik **bu aşamada modellenmez** — oran girmek hesaplamaya
  giden ilk adımdır. Alan iki durumludur; kısmi durumlar muhasebecinin işidir
  ve pakette not olarak taşınır.
- Ölçüt: işletme neti indirilebilirlikten etkilenmiyor; yalnız paket etkileniyor.
- **Tamamlandı** (26 Ağustos 2026): `IsTaxDeductible` gider **tanıyan** dört
  kayda eklendi (`BudgetTransaction` yalnız gider, `CreditCardCharge`,
  `CounterpartyCharge` ve `Obligation` yalnız borç yönünde). Kapsamdan ayrı, iki
  durumlu; şahsi kayıtta ve gelirde soru sorulmaz ve açık cevap **reddedilir**.
  `Category.DefaultIsTaxDeductible` ve `TaxDeductibilityResolution` zinciri
  (açık seçim → kategori varsayılanı) eklendi; soru sorulmayan kayıtta
  kategorinin varsayılanı sessizce düşüyor. İşletme kategori seti gider
  kalemlerini `true` önerisiyle açıyor, `SGK ve vergi ödemesi` boş kalıyor.
  `AddTaxDeductibility` migration'ı beş tabloya nullable kolon ve beş CHECK
  kısıtı ekledi. İşletme neti etkilenmiyor: indirilemeyen gider de gider.

### Grup 4 — Vergi ve SGK takvimi

- Tekrarlayan yükümlülük altyapısı üzerine kurulur; yeni bir zamanlayıcı
  yazılmaz.
- Hazır kalemler kullanıcıya **öneri olarak** sunulur ve tek dokunuşla kurulur:
  KDV beyanı, geçici vergi, Bağkur/SGK, muhtasar.
- Tarihler ve tutarlar kullanıcıya aittir; uygulama mevzuat takibi yapmaz ve
  bunu ekranda yazar.
- Ölçüt: takvim kalemleri yaklaşanlar listesine düşüyor; kullanıcı düzenleyip
  silebiliyor.
- **Tamamlandı** (26 Ağustos 2026): takvim kalemi tekrarlayan bir plandır — yeni
  tablo, yeni zamanlayıcı ve ikinci bir yaklaşanlar kaynağı yok.
  `GET /api/v1/tax-calendar/suggestions` dört hazır kalemi (KDV beyanı,
  muhtasar, SGK/Bağkur, geçici vergi) **tutarsız** olarak öneriyor ve hiçbir şey
  yazmıyor; kalem mevcut tekrarlayan plan ucundan kuruluyor. Geçici verginin
  ritmi için `RecurrenceFrequency.Quarterly` eklendi (`AddQuarterlyRecurrence`).
  Gerçekleştirme isteği isteğe bağlı bir tutar taşıyor: plandaki tutar bir
  beklentidir, kayda kullanıcının yazdığı geçiyor ve planın tutarı değişmiyor.

### Grup 5 — Ay sonu muhasebeci paketi

- Tek eylemle üretilen paket: seçilen ayın **yalnız işletme** kapsamlı gelir ve
  giderleri, KDV özeti, indirilemeyen kalemlerin ayrı listesi ve kayıtlara bağlı
  belge ekleri.
- Şahsi hiçbir kayıt pakete girmez — bu, aşamanın en kritik testidir.
- Paketin toplamları aynı ayın işletme raporuyla **birebir** tutmalıdır; iki
  ayrı hesaplama yolu bırakılmaz.
- Mevcut dışa aktarma altyapısı genişletilir; ikinci bir dışa aktarma yolu
  yazılmaz.
- Ölçüt: paket toplamları rapor toplamlarıyla eşit; şahsi kayıt sızmıyor.
- **Tamamlandı** (26 Ağustos 2026): `GET /api/v1/accountant-package` önizlemeyi,
  `GET /api/v1/exports/accountant-package.zip` tek dosyayı veriyor. Toplamlar
  aynı ayın işletme raporundan okunuyor — ikinci hesaplama yolu yok; satırların
  toplamı da ona eşit (birim + gerçek SQL testi). Kapsam parametresi yok,
  `Business` sabit. Dosyada `summary.csv`, `lines.csv`, `attachments.csv` ve
  `attachments/` altında kayda bağlı belgeler var; boyut tavanını aşan ek
  listede kalıyor ama dosyası konmuyor.

### Grup 6 — Karşılık olarak hedefler

- `SavingsGoal` kapsam alanı kazanır.
- "Vergi karşılığı" hazır bir hedef türü olarak sunulur; **yeni modül
  yazılmaz** — mevcut manuel katkı ve bakiye izleme mekanizması kullanılır.
- Ölçüt: işletme kapsamlı hedef, şahsi hedeflerden ayrı raporlanıyor.
- **Tamamlandı** (26 Ağustos 2026): `SavingsGoal` nullable bir kapsam kazandı
  (`AddSavingsGoalScope`). `GET /api/v1/goals?scope=` filtreli okuma kapsamsız
  hedefleri de eliyor; filtresiz okuma üç kovalı bir kırılım
  (`scopeBreakdown`) döndürüyor — işletme, şahsi ve etiketsiz. Yeni modül
  yazılmadı: karşılık, mevcut manuel katkı mekanizmasının kapsam etiketli
  hâlidir; "Vergi karşılığı" istemcinin ön dolduracağı bir addır.

### Grup 7 — Flutter

- İşlem formunda KDV alanı: isteğe bağlı, katlanmış, dolu değilse görünmez
  şekilde sade.
- İndirilebilirlik yalnız işletme kapsamlı kayıtta görünen bir anahtar.
- Vergi takvimi ekranı ve hazır kalem kurulumu.
- Ay sonu paketi ekranı: önizleme, kayıt sayısı, toplamlar, dosya boyutu; sonra
  paylaş.
- Ölçüt: zorunlu ekran durumları; KDV alanı boşken form sadeliğini bozmuyor.

### Grup 8 — Yedek v10 ve belgeler

- Şema **v10** yazar, yalnız v10 okur.
- `documentation/variables.md` yeni yapılandırma kaynağını kaydeder.
- Ölçüt: KDV, indirilebilirlik ve takvim kalemleri kayıpsız geri yükleniyor.

## Zorunlu testler

### Domain / Application

- KDV alanı boş bırakılabilir; girildiğinde hiçbir tutar otomatik türetilmez.
- İndirilebilirlik yalnız işletme kapsamlı kayıtta anlamlı; şahsi kayıtta
  reddedilir veya yok sayılır (davranış testle sabitlenir).
- İndirilebilirlik işletme netini **değiştirmez**.
- Takvim kalemi tekrarlayan yükümlülük olarak idempotent üretilir.

### API ve gerçek SQL

- Kullanıcı izolasyonu; pozitif **ve** negatif senaryo.
- Ay sonu paketi yalnız işletme kapsamını içerir; şahsi kayıt sızmaz.
- Paket toplamları aynı ayın rapor toplamlarıyla birebir eşit.
- Yedek v10 yazma/okuma; v9 reddi.

### Flutter

- KDV alanı boşken form sade; doluyken değer korunuyor.
- İndirilebilirlik anahtarı şahsi kapsamda görünmüyor.
- Paket önizlemesi hassas içeriği açmadan özet gösteriyor.

## Kalite komutları

```bash
dotnet build BusinessFinance.slnx --configuration Release --no-restore
dotnet test BusinessFinance.slnx --configuration Release --no-restore
dotnet format BusinessFinance.slnx --verify-no-changes --no-restore
```

```bash
flutter analyze
flutter test
dart format --set-exit-if-changed lib test
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

## Belge güncellemeleri

- Yeni ADR: vergi alanları taşır, hesaplamaz
- `documentation/architecture.md`: KDV ve indirilebilirlik alanları, paket üretimi
- `documentation/flows.md`: KDV girişi, takvim kurulumu, ay sonu paketi
- `documentation/permissions.md`: yeni endpoint'ler
- `documentation/variables.md`: takvim yapılandırma kaynağı
- `documentation/tests.md`, `documentation/restore-runbook.md`
- `PRD-BusinessFinance.md`: ürün sınırı bölümü, alan eklendikçe teyit edilir
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler

## Güvenlik ve veri sınırları

- Bütün sorgular current-user kapsamlıdır.
- Muhasebeci paketi kullanıcının kendi cihazından paylaşılır; sunucu üzerinden
  üçüncü kişiye gönderilmez.
- Paket gerçek finansal veri taşıyabilir; bulut güvenlik kapısı (Aşama 06)
  tamamlanmadan gerçek veri kullanılmaz.
- Yalnız sentetik veri kullanılır.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| Vergi alanının hesaplayan alana dönüşmesi | ADR ve "otomatik bölme yok" testi |
| Şahsi kaydın muhasebeci paketine sızması | Paket kapsam testi, çıkış koşulu |
| Paket ile raporun farklı toplam vermesi | Tek hesaplama yolu; eşitlik testi |
| Takvim tarihlerinin eskimesi | Tarihler kullanıcıya ait; ekranda açıkça yazılı |
| KDV alanının formu şişirmesi | İsteğe bağlı ve katlanmış alan; boşken görünmez |
| Kısmi indirilebilirliğin oran alanına dönüşmesi | İki durumlu alan; kısmi durum pakette not |

## Çıkış koşulları

- [ ] ADR yazıldı ve kabul edildi.
- [ ] Bütün çalışma grupları tamamlandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test, format ve debug build kontrolleri geçti.
- [ ] Kullanıcı izolasyonu negatif senaryolarla kanıtlandı.
- [ ] Bir ayın paketi üretildi; toplamları aynı ayın işletme raporuyla birebir
      tutuyor ve şahsi hiçbir kayıt içermiyor.
- [ ] Hiçbir vergi tutarının uygulama tarafından hesaplanmadığı testle
      kanıtlandı.
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Kullanıcı Aşama 06'yı açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında burada: hangi commit'lerle bitti, hangi kontroller geçti,
belge `docs/archive/stages/` altına taşındı mı.
