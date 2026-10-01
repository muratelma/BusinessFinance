# ADR 0018 — Vergi bir nakit planıdır: KDV, indirilebilirlik ve muhasebeci paketi kalkar

- Durum: **Kabul edildi** (29 Eylül 2026, kullanıcı onayı). Kararlar 28–29 Eylül
  2026'da alındı. Aşama 06.3 Grup 2 ve Grup 3'ün karar kapısıdır.
- **Nasıl okunur:** bu ADR iki katmanlıdır. **§İlkeler bağlayıcıdır**; değişmesi
  kullanıcı kararı ve bu ADR'nin güncellenmesini ister. **§Başlangıç tasarımı**
  uygulamanın ilk hâlidir; kod yazılırken daha iyi ya da gerekli bir yol
  bulunursa ilkeleri bozmadan değişebilir (`AGENTS.md` "Kararlardan sapma").
  Sık değişecek ayrıntı (hazır türler tablosu) `research/vergi/YENI-YAKLASIM.md`
  §6.6'dadır.
- Bağlam: Vergi temasının yeniden ele alınması; kaldırmalar ve vergi ekranı
- **Yerini aldığı:** ADR 0016'nın §2'si (KDV alanları), §3'ü (indirilebilirlik),
  §4'ün muhasebeci paketi kısmı ve "Sonuçları"ndaki ilgili satırlar. ADR 0016'nın
  §1'i (oran ve tarih kullanıcınındır; öneri kurulduğu an kullanıcının verisi
  olur) ve **"uygulama hiçbir vergi tutarını hesaplamaz"** ilkesi yürürlükte kalır.
- **Dokunduğu:** ADR 0005 — başlangıç tasarımında vergi türündeki planın kaynağı
  isteğe bağlıdır (§T4).
- İlgili: ADR 0013 (tek havuz, kapsam zinciri), ADR 0014 (tanır / taşır),
  ADR 0004 (birleşik okuma modelleri), ADR 0011 (fiş okuma bir öneridir)
- Gerekçe belgeleri: `research/vergi/YENI-YAKLASIM.md` (§6.6), `research/vergi/BULGULAR.md`,
  `research/DENETIM-2026-09-29.md`

## Bağlam

Aşama 05 vergiyi **taşınan alanlar** olarak kurdu: beş kayıt türünde KDV oranı ve
tutarı, giderlerde indirilebilirlik, dört kalemlik bir takvim ve ay sonu
muhasebeci paketi. 28 Eylül 2026'da kendi uygulamamızın denetimi şunu gösterdi:

- **KDV alanları rejime bakmıyor.** Hedef kullanıcının bir kısmı basit usuldedir
  ve KDV'si yoktur; bu kullanıcı her formda "Vergi bilgisi" görüyor. 2026'da bir
  kısım esnaf gerçek usule geçti; kullanıcı KDV açısından ikiye bölünmüş durumda.
- **Paketteki KDV, girilen KDV'lerin toplamıdır.** Ağustos'ta 9 kaydın 6'sında
  KDV yoktu; paket gerçek KDV'nin bir parçasını gösteriyordu. Z raporu, e-fatura
  ve e-arşiv zaten GİB'e elektronik gidiyor; muhasebeci KDV'yi oradan kuruyor.
- **Takvim bir durum ekranı değildi** ve planlarda **sabit tahmini tutar**
  gösteriyordu; kullanıcı bunu gerçek tutar sanıyordu.

Kullanıcının çerçevesi: ürün bir **işletme bütçe uygulamasıdır, ön muhasebe
değildir**; esnaf için vergi bir **nakit çıkışıdır** ve tutarı çoğu zaman ödeme
gününe kadar bilinmez ("parayı muhasebeciye ödeyince tutarı öğrenmiş oluyoruz").
Bir kısım kullanıcı yaklaşan ödemeleri hiç izlemez; yalnız cebinden çıkan vergiyi
**tek seferde, toplu** yazıp bütçesini doğru tutmak ister. Muhasebeciye veri
hazırlamak bu ürünün işi değildir; kullanıcı muhasebecisinden aldığı bilgiyi
kendisi girer.

## İlkeler (bağlayıcı)

**İ1 — Vergi tutarı türetilmez.** Uygulama hiçbir vergi tutarını hesaplamaz,
türetmez veya tahmin etmez; tutar kullanıcıdan gelir. Oran ve tarih
kullanıcınındır (ADR 0016 §1).

**İ2 — Ön muhasebe yükü kalkar.** KDV alanları, indirilebilirlik ve muhasebeci
paketi kaldırılır; yerlerine başka bir ön muhasebe özelliği eklenmez.

**İ3 — Vergi bir nakit çıkışıdır.** Ödendiği gün, ödendiği hesaptan (ya da
karttan) etkiler. Ödenmemiş vergi net varlığı, işletme netini ve bütçeyi
etkilemez.

**İ4 — Tanımlamadan ödenebilir.** Kullanıcı hiçbir vergiyi tanımlamadan, tek bir
tutarla "ödediğim vergiler" yazabilir. Tanımlı vergi kalemleri yalnız
hatırlatma ve takip içindir; ödemenin ön koşulu değildir.

**İ5 — Tutarı bilinmeyen vergi meşrudur.** Uygulama onu toplamlara tahminle
katmaz; bilinmediğini söyler.

**İ6 — Vergi ayrı bir kayıt türü değildir.** Tanımlı vergi, tekrarlayan planın bir
biçimidir ve Yaklaşanlar'a var olan planlanan projection'dan düşer (ADR 0004);
ödenen vergi var olan gider kayıtlarıdır. Raporlara ikinci bir yol açılmaz.

**İ7 — Vergi ödemesi düzeltilebilir.** Yanlış girilen ödeme silme yerine
iptalle geri alınır. Gerçekleşen bir plan kalemi tam olarak tek sonuç taşır.

**İ8 — Kimlik ada bağlanmaz.** Bir kaydın ya da planın vergi olduğu, kullanıcının
değiştirebileceği bir ad ya da kategori adı üzerinden tanınmaz.

**İ9 — Kapsam kullanıcının seçimidir, yoksa profilin tarafıdır.** Vergi kaydının
kapsamı kullanıcının açık seçiminden, seçim yoksa profilin tarafından (işletmesi
olan kullanıcıda işletme, olmayanda şahsi) gelir. **Ödeme kaynağının etiketi
kapsamı belirlemez:** işletme vergisini şahsi kartla ödemek olağandır. Sunucu
kapsam uydurmaz. *(30 Eylül 2026, kullanıcı kararıyla değişti; önceki metin
"vergi kayıtları kapsamı ADR 0013 zinciriyle alır" idi ve zincirin hesap/kart
etiketi basamağını içeriyordu.)*

Reddedilenler (ilke düzeyinde):

| Reddedilen | Neden |
|---|---|
| KDV alanlarını basit usulde gizleyip KDV mükellefinde tutmak ("vergi durumu" sorusu) | Ön muhasebe yaklaşımı; kullanıcı reddetti. Alanı dolduran azınlık, dolduranın bile verisi eksik (V-U2) |
| Paketin KDV'siz hâlini "ay sonu dökümü + belgeler" olarak tutmak | Muhasebeciye veri vermek ön muhasebenin işi; bu ürün bütçe uygulaması (kullanıcı, 29 Eylül). İşlem CSV'si dışa aktarımı yerinde kalır |
| Eski KDV verisini gizli tutmak | Okunmayan kolon, şema ile ürünün ayrışmasıdır; veri sentetik |
| Vergi için ayrı kayıt türü | Yaklaşanlar, İşlemler, yedek ve raporlara ikinci bir yol; kazancı yok |
| Tanımda tahmini tutar zorunlu (bugünkü davranış) | Kullanıcı tahmini gerçek tutar sanıyor (V-U7); "vergi hesaplamaz" sınırına en yakın yanlış |

## Başlangıç tasarımı (uygulamada değişebilir)

Aşağıdakiler 06.3 Grup 2–3'ün ilk hâlidir. Kod yazılırken değişirse sapma aşama
belgesinin "Sapmalar" tablosuna yazılır; kullanıcının gördüğü davranışı
değiştiren sapma uygulanmadan önce kullanıcıya sorulur (`AGENTS.md`).

### T1. Kaldırmalar (İ2)

- Beş kayıt türündeki (`BudgetTransaction`, `CreditCardCharge`,
  `CounterpartyCharge`, `Obligation`, `PosSettlement`) KDV alanları, giderlerdeki
  indirilebilirlik ve kategorinin indirilebilirlik varsayılanı kaldırılır.
  Kolonlar ve girilmiş veri silinir (veri sentetik; veri kaybettiren karar
  kullanıcıdan alındı ve `docs/project-status.md`'ye yazıldı).
- Muhasebeci paketi tamamen kalkar: ekran, okuma ucu ve zip dışa aktarımı.
- Fiş okuma belgedeki KDV'yi okuyabilir ama forma yazmaz.

### T2. Vergi ekranı ve vergi türü (İ6, İ8)

- Vergi kendi ekranında tanımlanır, düzenlenir, duraklatılır, silinir ve ödenir.
  Arka planda planda ayrı bir **vergi türü** alanı bulunur (hazır tür anahtarı ya
  da kullanıcının kendi türü). Vergi türü dolu planlar Tekrarlayanlar listesinde
  görünmez, Yaklaşanlar'da görünür.
- Hazır türler yalnız sıklık ve gün önerir; öneri kurulduğu an kullanıcının
  verisidir.
- Yeni sıklık **"seçilen aylarda"** (ör. emlak Mayıs ve Kasım, gelir vergisi Mart
  ve Temmuz); genel tekrarlayan formda da kullanılabilir; "üç aylık" kalır.
- Bugün var olan planların vergi türü **boş** kalır (hangisinin vergi olduğu
  geçmişte bilinmiyor; uydurulmaz).

### T3. Tutarsız kalem (İ5)

- Planın ve bekleyen kalemin tutarı boş olabilir; tanımda tutar isteğe bağlıdır.
- Tutar önceden belli olursa bekleyen kaleme yazılır ("tutar belli oldu"); plan
  değişmez.
- Toplamlar ("7 günde çıkacak") tutarsız kalemi saymaz, sayısını ayrıca söyler.

### T4. "Ödedim" ve geri alma (İ3, İ7)

- "Ödedim" tutar, gün ve hesap ya da kart ister; kayıt **ödeme gününe** yazılır,
  vade gününe değil. Kartla ödeme kart harcaması yazar.
- Tanımda hesap/kart isteğe bağlıdır: ADR 0005'in "plan kaynağı tam olarak biri
  dolu" kuralı yalnız vergi türündeki planda "en çok biri dolu"ya gevşer ve
  kaynak ödemede verilir. (Uygulamada daha iyi bir yol bulunursa — ör. tanımda
  varsayılan hesap zorunlu — ADR 0005 aynen kalabilir.)
- Gerçekleşmiş ödeme vergi ekranından geri alınır: ürettiği kayıt iptal edilir,
  kalem bekleyene döner. Birleşik akıştaki köken kilidi (`*.cancel_origin_locked`)
  kalır; geri alma kaynağın kendi ekranından yapılır.

### T5. Toplu vergi ödemesi (İ4)

- "Vergi ödemesi ekle": tutar, gün, hesap/kart, isteğe bağlı not. Tanımlı bir
  vergiye bağlı olmak zorunda değildir. Hiç vergi tanımlamamış kullanıcıda vergi
  ekranının ana eylemi budur; "vergi tanımla" onu gölgelemez.
- Tanımlı bekleyenler varsa listelenir, vadesi gelmiş ve geçmiş olanlar seçili
  gelir; seçilenler yeni bir **"kapatıldı"** durumuna geçer (sonuç kaydı yok,
  kapatan ödemenin kimliği var). Ödeme geri alınırsa kapattıkları bekleyene döner.
- Tutarla kendiliğinden eşleştirme yapılmaz (çoğu kalemin tutarı boş). Gecikme
  zammı hesaplanmaz.
- Vergi, gider formundan vergi işaretli bir kategoriyle de girilebilir; sonuç
  aynıdır.

### T6. "Vergi" işareti (İ8)

- Kategoride bir "vergi" işareti bulunur; iki varsayılan sette birer kategori
  işaretli gelir (işletme: "SGK ve vergi ödemesi", kişisel: "Vergi ve harç");
  kullanıcı başka kategorileri de işaretleyebilir. Mevcut kullanıcılarda bu iki
  adlı kategori bir kez işaretlenir.
- Vergi ekranının "Ödenenler" listesi işaretli kategorilerdeki giderlerdir: tek
  gerçek, iki kapı. Kategori yine bir raporlama kovasıdır (PRD §4); işaret
  kovanın türünü söyler, tek tek kaydın kimliğini değil.

### T7. Kapsam (İ9)

- **(30 Eylül 2026'da değişti.)** Her vergi tanımında `İşletme · Şahsi` seçimi
  sorulur: formun en üstünde, başlıksız, varsayılanı profilin tarafı; hesap ya da
  kart seçimi onu değiştirmez. İşletmesi olmayan kullanıcıda seçim görünmez ve
  vergi şahsidir. Tanımsız toplu ödemede seçim yoktur; kapsam profilin
  tarafıdır (şahsi yazmak isteyen gider formunu kullanır). İlk metin "kapsam
  sorulmaz; yalnız MTV ve emlakta şahsi seçilebilir" idi.
- İşletme kapsamlı vergi işletme netini düşürür. Özet'e bunu söyleyen bir
  açıklama **eklenmedi** (kullanıcı kararı, 30 Eylül 2026).

Reddedilen tasarımlar:

| Reddedilen | Neden |
|---|---|
| İki taksitli vergiyi iki yıllık kalemle ifade etmek | Tek vergi iki satır olur; Mart–Temmuz gibi ritim "6 ayda bir"e sığmıyor |
| Tutar sıfır = bilinmiyor | "Sıfır" ile "bilinmiyor"u karıştırır; toplamlar sessizce yanlış olur |
| Bir gideri birden çok kaleme sonuç olarak bağlamak | "Tek sonuç" kuralını (İ7) ve iptali bozar |
| Kapatılacakları işaretlemeyi tamamen isteğe bağlı bırakmak | İşaretlenmeyen kalem sahte "gecikti" üretir |
| Tutarı en gecikmişten başlayarak dağıtmak (Paraşüt deseni) | Tutarsız kalemlerde dağıtılacak tutar yok |

## Sonuçları

- **Veri kaybettiren migration:** KDV ve indirilebilirlik kolonları ve verisi
  kalkar. Kullanıcı onayı `docs/project-status.md`'dedir.
- Başlangıç tasarımıyla tekrarlayan plan genişler: vergi türü alanı; plan ve kalem
  tutarı nullable (mevcut satırlar dolu kalır, backfill yok); kaynak vergi
  türünde isteğe bağlı; "seçilen aylarda" sıklığı; kalem durumu "kapatıldı".
  İlgili CHECK kısıtları (`[Amount] > 0`, kaynak kuralı) yeniden yazılır;
  `AGENTS.md` migration kuralları geçerlidir.
- Gerçekleştirme isteği tutar + gün + kaynak alır; geri alma ve toplu ödeme uçları
  eklenir; kategoriye "vergi" işareti eklenir.
- Planlanan projection, "7 günde çıkacak" toplamı ve API sözleşmesi tutarsız
  kalemi taşır.
- Muhasebeci paketinin uçları (`/api/v1/accountant-package`,
  `/api/v1/exports/accountant-package.zip`) kalkar.
- Yedek şeması Aşama 06.3'te **tek sürümle** ilerler (`PROJECT-ROADMAP.md`).
- Arayüz metinleri "vergi hesabı", "matrah", "beyanname" gibi hesaplama iddiası
  taşıyan kelimeleri kullanmaz. Hazır türlerin ipuçları "genelde" diliyle yazılır
  ve kullanıcıyı muhasebecisine yönlendirir.

## Bu ADR neyi karara bağlamaz

- Muhasebecinin gönderdiği tahakkuk fişinden tutar okumak (`research/fikir-kaydi.md`
  F17): örnek görülünce ayrı karar.
- Özet'te vergi raporu / widget (F22), vergi kalemine karşılık ayırma kısayolu
  (F15), yapılandırılan vergi borcunun borç sözleşmesiyle izlenmesi (F20).
- Muhasebecinin esnaf adına ödediği vergi dışı masraflar: cari hesapla karşılanır
  (F18, F21); vergi ekranı cari hesaba bağlanmaz.
- Basit usul had takibi (F16): kapsam dışı önerildi.
