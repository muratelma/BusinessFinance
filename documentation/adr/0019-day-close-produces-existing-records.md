# ADR 0019 — Gün sonu var olan kayıtları üretir; POS tanımı, yatış ve kartla tahsil

- Durum: **Kabul edildi** (29 Eylül 2026, kullanıcı onayı). Kararlar 28–29 Eylül
  2026'da alındı. Aşama 06.3 Grup 4–7'nin karar kapısıdır.
- **Nasıl okunur:** bu ADR iki katmanlıdır. **§İlkeler bağlayıcıdır**; değişmesi
  kullanıcı kararı ve bu ADR'nin güncellenmesini ister. **§Başlangıç tasarımı**
  uygulamanın ilk hâlidir; kod yazılırken daha iyi ya da gerekli bir yol
  bulunursa ilkeleri bozmadan değişebilir (`AGENTS.md` "Kararlardan sapma").
  Sık değişecek ayrıntı (gün sonu varsayılanları, Z okuma kuralları)
  `research/kasa-pos-gun-sonu/KAPANIS.md`'dedir ve Grup 7'de gerçek Z
  örnekleriyle yeniden incelenir.
- Bağlam: Kasa ve POS'un gerçek hayatta çalışır hâle getirilmesi
- **Genişlettiği:** ADR 0015 — §2–3'ün yoldaki para tanımı (İ6) ve §1'in kelime
  kuralı (T7). ADR 0015'in geri kalanı aynen geçerlidir.
- İlgili: ADR 0014 (tanır / taşır — bu ADR onun uygulamasıdır, istisnası değil),
  ADR 0013 (tek havuz), ADR 0004 (birleşik okuma modelleri), ADR 0011 (belge
  okuma bir öneridir), ADR 0009 (oran paradan çözülür)
- Gerekçe belgeleri: `research/kasa-pos-gun-sonu/KAPANIS.md` (KP1–KP22),
  `KARAR.md`, `BULGULAR.md`, `GUN-SONU-BELGELERI.md`, `research/para-akisi/HARITA.md`,
  `research/DENETIM-2026-09-29.md` ve dış kaynak eki

## Bağlam

Aşama 04 Kasa'yı ve POS tahsilatını kurdu. 28 Eylül 2026'da uygulama emülatörde
bir haftalık esnaf senaryosuyla denendi; bulguların (U1–U13) dört kök nedeni çıktı:

- **"Gün" kavramı yok.** Nakit satış tek tek girilmezse kasa sayımı bir haftanın
  satışını "fazla" diye gösterdi (U1). Kasa ile POS birbirinden habersiz (U9).
- **Banka tarafında kontrol noktası yok.** Yatan tutar beklenenden farklı olunca
  fark hiçbir yere yazılamadı; toplu yatış tek tek işaretlendi (U2, U3).
- **POS girişi tekrarlı ve düzeltilemez.** Her akşam aynı beş seçim (~15 dokunuş,
  U13); POS kaydı iptal edilemiyor, "hesaba geçti" geri alınamıyor (U12).
- **Görünürlük.** Beklenen tahsilat Yaklaşanlar'da yok (U7), nakit kayıt Kasa'yı
  yenilemiyor (U11).

Kullanıcının çerçevesi: "tek tek girmesini beklemek hiç mantıklı değil"; POS
parasının bankaya geçişi için "para işlemler kısmına girer ama gider gelir olmaz";
ürün bir **işletme bütçe uygulamasıdır** — muhasebe defteri, stok, adisyon ve
satış satış kayıt kapsam dışıdır. Muhasebe pratiği de Z raporunu **günlük toplam**
olarak kaydeder; kart satışı bankaya geçene kadar bir ara hesapta bekler (bizim
"yoldaki para"mız).

## İlkeler (bağlayıcı)

**İ1 — Tanır / taşır** (ADR 0014). Gün sonu satışı **tanır**. POS parasının
bankaya geçişi (yatış) ve kartla tahsil **taşır**: bakiyeyi değiştirir, gelir
yazmaz. Beklenen ile gerçekleşen arasındaki fark ayrı bir kayıttır.

**İ2 — Aynı satış iki kez gelir sayılmaz.** Gün sonu, o gün zaten girilmiş
kayıtları hesaba katar; kartla tahsil ve yatış gelir üretmez.

**İ3 — Gün sonu yeni bir finansal kayıt türü değildir.** Var olan kayıtları
(gelir, POS tahsilatı) üretir; raporlara, işletme netine, bütçeye ve birleşik
akışa ikinci bir kaynak açılmaz.

**İ4 — Toplamla giriş, tek tek giriş ikisi de mümkündür; hiçbir akış fotoğrafa ya
da POS'a bağlı değildir.** Elle kasa defteri tutan, POS'u olmayan esnaf aynı
akışı elle kullanır.

**İ5 — Belge okuma bir öneridir** (ADR 0011). Okunan değer formu doldurur,
kullanıcı onaylar; günlük olmayan bir değer (cihazın kümülatif toplamı gibi)
günlük satış sayılmaz.

**İ6 — Yoldaki para bir projection'dır, hesap değildir** (ADR 0015 §2). Kaynağı
ne olursa olsun (POS satışı ya da kartla tahsil) kullanılabilir bakiye ile net
varlık arasındaki fark **tam olarak** yoldaki tutardır; bu bir test kapısıdır.

**İ7 — Kasada tek gerçek hesap bakiyesidir.** Kasa ekranı ikinci bir bakiye
kaynağı üretmez; sayım bir gözlemdir.

**İ8 — Yanlış girilen her kayıt düzeltilebilir.** Gün sonu, POS kaydı ve yatış
silme yerine iptal ya da geri almayla düzeltilir.

**İ9 — Ürün sınırı.** Banka hesabı sayımı / mutabakatı, satış satış kayıt,
adisyon, stok ve banka/POS/üye işyeri entegrasyonu yoktur.

Reddedilenler (ilke düzeyinde):

| Reddedilen | Neden |
|---|---|
| Ayrı bir `DailyClose` tablosu (Z toplamı, not, durum) | Raporlara, işletme netine, bütçeye, birleşik akışa ve yedeğe ikinci bir kaynak ekler (İ3) |
| Kartla veresiye tahsilatında "POS'a girmeyin" uyarısıyla yetinmek | Esnafın en olağan işlerinden birini cevapsız bırakır; kullanıcı POS'a girerse gelir iki kez sayılır |
| Banka hesabı için sayım ("banka mutabakatı") | Kullanıcının bütün hareketleri %100 girmesini bekler; pratikte olmaz (kullanıcı) |
| POS tahsilatını bir hesap türü yapmak | ADR 0015 §2'nin reddi aynen geçerlidir |

## Başlangıç tasarımı (uygulamada değişebilir)

Aşağıdakiler 06.3 Grup 4–7'nin ilk hâlidir. Kod yazılırken değişirse sapma aşama
belgesinin "Sapmalar" tablosuna yazılır; kullanıcının gördüğü davranışı
değiştiren sapma uygulanmadan önce kullanıcıya sorulur (`AGENTS.md`).

### T1. Gün sonu paneli (İ3, İ4, İ8)

- Nakit satış ve her POS tanımı için kartlı satış alır (yemek kartı kendi
  cihazının gün sonu tutarıyla, kendi POS satırında). Nakit satış bir gelir
  kaydıdır (kasaya), kartlı satış bir POS tahsilatıdır; kayıtlar bir **bağlayıcı
  kimlik** taşır ve tek `SaveChanges` sınırında yazılır.
- **Tekrar kontrolü Z numarasıyla** yapılır. Z no'suz elle girişte gün başına bir
  gün sonu vardır; aynı güne ikincisi yalnız açıkça "ek gün sonu" (ikinci cihaz)
  olarak ve "bu gün kapatıldı" uyarısıyla girilir. İstek düzeyindeki idempotency
  ayrıca korunur.
- Gün sonu **bir bütün olarak geri alınır**: ürettiği kayıtlar birlikte iptal
  olur, gün yeniden açılır. Tek tek kayıtları birleşik akışta köken kilidiyle
  korunur.
- Kapılar: "+" menüsü (işletme profilinde "POS tahsilatı"nın yerini alır) ve
  Kasa'daki "Gün sonunu gir".

### T2. Zaten girilmiş kayıtlar (İ2)

- Panel o günün zaten girilmiş kayıtlarını listeler ve "gün sonu tutarında var
  mı?" diye sorar: tek tek girilmiş satışlar (gelir ve POS; faturalı dahil),
  kartla tahsilatlar (T5) ve nakit cari tahsilatlar.
- Varsayılan: satışlar ve kartla tahsilatlar işaretli (düşülür); nakit cari
  tahsilat işaretsiz (çoğu zaman yazar kasadan geçmez).
- Yazılan nakit satış = nakit − işaretli nakit kayıtlar; kart satış = kart −
  işaretli kartlı kayıtlar.
- Elle girişte nakit, kart ve toplamdan ikisi yeterlidir; üçüncüsü hesaplanır.
- **Toplu Z** (birkaç gün): okunan aralık gösterilir, toplam son güne yazılır,
  kullanıcı değiştirebilir; aralıktaki bütün günler "kapalı" sayılır; aralık ay
  dönümünü geçerse uyarı verilir ve tutar iki parçaya bölünebilir.
- Z numarası yalnız fotoğraftan okunur; atlanan Z uyarısı yalnız Z no'lu günler
  arasında gösterilir ve kaydı denetler, esnafın cihazını değil.

### T3. Z raporu okuma (İ5)

- Okunur: tarih ya da aralık, Z no, NAKİT, KART, TOP. Okunmaz: KÜMSAT, KDV, BRÜT,
  iptal satırı. Basılmayan satır 0'dır.
- Kayıtlar NAKİT ve KART'tan yazılır; TOP kayıt üretmez, açıklar: NAKİT + KART
  ile TOP farklıysa panel farkı gösterir ("genelde faturalı satış ya da veresiye
  tahsilatıdır"). Z'de faturalı satış, cari hesap tahsilatı ve yemek kartı için
  ayrı sayaçlar vardır (GİB bilgi fişi teknik kılavuzu).
- Birden çok POS tanımı varsa kart tutarı bölüştürülür (varsayılan ana POS).
- NAKİT/KART satırlarının faturalı satış ve tahsilatı içerip içermediği gerçek
  örnekle 06.3 Grup 7'de doğrulanır; T2'nin varsayılanları buna göre değişebilir.

### T4. POS tanımı

- Kullanıcının bir kez girdiği ayar: ad, geçeceği hesap, varsayılan oran,
  komisyon kategorisi, geçiş gün sayısı, iş günü seçeneği (hafta sonunu atlar;
  tatil takvimi yok, gün her zaman elle düzeltilir). Yemek kartı bir POS
  tanımıdır.
- Form tanımdan dolar ve canlı net gösterir. Kayıt tutarı saklar, oranı saklamaz
  (`PosSettlement` kuralı, ADR 0009).

### T5. Yatış ve kartla tahsil (İ1, İ6, İ8)

- Yatış "hesaba geçti"nin yerini alır: yoldaki kayıtlar toplu seçilir, gerçek
  yatan tutar ve gün yazılır; fark **kesinti** olarak POS tanımının komisyon
  kategorisine ayrı bir gider kaydıyla yazılır ve İşlemler'de yatışın detayında
  görünür. Ayrı BSMV alanı yoktur.
- Yatış geri alınabilir; kapattığı kayıtlar yeniden yolda olur. Yatışa bağlı POS
  kaydı, önce yatış geri alınmadan iptal edilemez. POS kaydının kendisi iptal
  edilebilir.
- Mevcut `TransferredOn` verisi yatış yapısına taşınır; iki yol yan yana yaşamaz.
- Cari tahsilatta ve tek seferlik alacağın (yükümlülük) kapatılmasında
  **"kartla (POS)"** ödeme yolu: alacak kapanır, gelir yazılmaz, para yola çıkar;
  bankaya geçişi yatışla işaretlenir. Kesintinin kapsamı POS tanımından gelir
  (varsayılan İşletme).
- Yoldaki para = gerçekleşmemiş POS satışlarının neti + gerçekleşmemiş kartlı
  tahsilatlar.

### T6. Kasa (İ7)

- Kasa sayımı gün sonundan ayrıdır ("Kasayı say"); sıklığı kullanıcı seçer.
- Beklenen = kasanın hesap bakiyesi. Kaydedilmemiş son fark ayrı bir satırda
  görünür ve sonraki sayımda yeniden sorulmaz. Fark kaydı isteğe bağlıdır; sebep
  sorulur.
- Kasa sekmesinde işletme kapsamlı ve kapsamsız nakit hesaplar görünür; yalnız
  şahsi etiketliler gizlenir (görünüm kuralı; ADR 0013'ün tek havuzuna dokunmaz).
- "Kendime aldım": şahsi hesaba transfer ya da şahsi gider, her seferinde sorulur;
  varsayılan transfer, şahsi hesap yoksa şahsi gider.

### T7. Kelimeler (ADR 0015 §1'in genişlemesi)

- Satış tarafında "kartla satış", "Kartla gelecek" ve "POS" kullanılır. ADR 0015'in
  "tahsilat tarafında `kart` kelimesi tek başına kullanılmaz" kuralı, kelime bir
  yönle birlikte geçtiğinde karışıklık doğurmadığı için bu kadarıyla gevşer.
- Harcama tarafında "kartla ödedim" ve "kredi kartı"; "POS" kelimesi harcama
  tarafında geçmez.

Reddedilen tasarımlar:

| Reddedilen | Neden |
|---|---|
| Gün sonunun yalnız "girilmemiş kısmı" sorması | Esnaf Z'deki sayıyı olduğu gibi yazar; çıkarma işini ona bırakmak hata üretir |
| Aynı günü tekrar kapatmanın üzerine yazması | "Tekrar basmak etkisizdir" güvencesini zayıflatır; düzeltme açık bir geri almayla yapılır |
| Kart türüne göre oran, ayrı BSMV alanı | Esnaf gün sonunda bu dağılımı girmez; yatıştaki fark satırı yakalar |
| Kasada beklenen = son sayım + o günden beri hareket | Farkı kaydedilmemiş bir sayımdan sonra Kasa ile hesap bakiyesi iki ayrı sayı söyler (İ7) |

## Sonuçları

- Başlangıç tasarımıyla yeni kayıtlar: POS tanımı, yatış, gün sonunun bağlayıcı
  kimliği, kartla tahsilin yoldaki parası; kayıtta isteğe bağlı Z no. Ayrıntılı
  model Aşama 06.3 belgesindedir.
- Birleşik akışa yeni satır ve köken değerleri girer (yatış, gün sonu kökenli
  satış, kartla tahsil); ADR 0004'ün beş boyutuna eklenir ve `canCancel` kuralı
  bunları kapsar.
- Planlanan projection'a beklenen POS girişi katılır; Yaklaşanlar'da "girecek"
  satırıyla görünür, "7 günde çıkacak" toplamına girmez.
  `IUpcomingPaymentRepository` ikinci sorgu tutmaz kuralı korunur.
- `FinancialDataChanges`: hesap bakiyesini değiştiren her olay Kasa'yı yeniler;
  yatış ve gün sonu yeni olaylardır.
- Yedek şeması Aşama 06.3'te **tek sürümle** ilerler (`PROJECT-ROADMAP.md`).
- CSV içe aktarım bu turda değişmez; yalnız çifte sayım uyarısı ve "satırı atla"
  eklenir (asıl çözüm ayrı tur, `research/fikir-kaydi.md` F05).

## Bu ADR neyi karara bağlamaz

- Mali hafıza raporu + banka dökümüyle aylık toplu giriş (KP8): belge örnekleri
  gelince.
- CSV içe aktarımının POS yatışını tanıması (F05), dekont kapısı.
- Satış tarafında taksit, tatil takvimi, bloke çözüm hesabı.
- Offline yazma: PRD §6.13'teki karar kapısı geçerlidir.
