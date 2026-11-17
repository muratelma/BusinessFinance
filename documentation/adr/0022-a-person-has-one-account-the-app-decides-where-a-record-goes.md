# ADR 0022 — Kişiyle olan her şey kişinin hesabında yürür; nereye yazılacağını uygulama bilir

- Durum: **Kabul edildi** (16 Kasım 2026, kullanıcı onayı). Aşama 06.3 Grup 8
  "Cari, borç ve alacak"ın karar kapısıdır; ADR 0021'in yanında durur.
- **Nasıl okunur:** iki katmanlıdır. **§İlkeler bağlayıcıdır**; değişmesi
  kullanıcı kararı ve bu ADR'nin güncellenmesini ister. **§Başlangıç tasarımı**
  ilkeleri bozmadan değişebilir (`AGENTS.md` "Kararlardan sapma").
- **Değiştirdiği:**
  - ADR 0020 İ9 ("cari işletmeye özeldir"): kişinin hesabı işletmeye özel
    değildir; cari, `İşletme` kişilerinin hesabıdır.
  - ADR 0020 İ10 ve ADR 0021 İ3: mal ya da hizmet borcunun cariye mi ayrı
    borca mı yazılacağı **sorulmaz**; kural sabittir.
  - ADR 0021 İ8: `Borç ve alacaklar` ve `Cari hesap` iki liste değildir; tek
    sayfadır ve cari onun `İşletme` görünümüdür.
  - ADR 0021 T7 ve T8: giriş akışları ve okumalar bu ADR'ye göre kurulur.
- **Değiştirmediği:** ADR 0021'in tek borç modeli (`Debt`), kısmi ödeme, istek
  kimliği, bağlı etkilerin birlikte geri alınması, uygulamadan önceki borç,
  bedel düzeltmesi / alınmayacak para / para hareketi ayrımı; ADR 0014; ADR
  0013 ve ADR 0020'nin kayıt kapsamı kuralı (kategori sınırlar).
- Gerekçe ve ayrıntı: `stages/06.3-borc-alacak/01-ARASTIRMA.md` (dış
  araştırma, üç yolun karşılaştırması), `02-TASARIM.md` (kararlar K1–K18,
  ekranlar, senaryolar), `03-PLAN.md` (fazlar).

## Bağlam

ADR 0021 iki modeli (cari defter, tek borç) ayrı tuttu ve aralarındaki seçimi
girişte kullanıcıya sordu. Soru bir muhasebe yöntemi sorusuydu ("yürüyen
bakiye mi, kendi başına kapanan kayıt mı") ve her ekrana sızdı: `Nasıl takip
edilsin?`, `Hangi alacağa kaydedeceksiniz?`, dört liste, kişi sayfasının
içinde ayrı bir cari sayfası. Tasarım turları 14–29 Ekim arasında bu yüzden
kapanmadı; 29 Ekim'de denenen "her alım ayrı borç, ödeme dağıtılır" modelini
kullanıcı reddetti.

Dış inceleme (veresiye uygulamaları, ön muhasebe ürünleri, kişisel finans
uygulamaları, Apple Card) aynı sonucu verdi: yürüyen açık hesap ile planlı
borç gerçekten iki ayrı şeydir, ama hangisinin kullanılacağını hiçbir ürün
kullanıcıya sormaz; kuralı ürün koyar.

## İlkeler (bağlayıcı)

**İ1 — Kayıt nereye yazılacağını uygulama bilir.** Taksitli olan ya da kişisi
olmayan borç ayrı bir kayıttır (`Debt`); kişiyle olan taksitsiz her şey
kişinin hesabına yazılır. Kullanıcıya takip biçimi sorulmaz.

**İ2 — Kişinin hesabı yürüyen bakiyedir.** Tahsilat ve ödeme kişinin
bakiyesini düşürür, hiçbir kayda bağlanmaz ve hedef sormaz. Hangi kaydın açık
ya da gecikmiş olduğu okunurken en eski vadeden başlanarak hesaplanır; kalıcı
bir dağıtım kaydı tutulmaz.

**İ3 — Kişinin hesabı borç parayı da tutar.** Verilen ve alınan borç para
hesaba yazılır; para hareket eder, gelir ya da gider yazılmaz (ADR 0014).

**İ4 — Fazla ödeme reddedilmez ve kırpılmaz.** Karşı yönde bakiye olur; ekran
kaydetmeden önce söyler.

**İ5 — Kapsamı listede kişi, raporda kategori taşır.** Kişinin bir türü vardır
(`İşletme` / `Şahsi`) ve `Borç ve alacak` bu türle bölünür; `İşletme`
kişilerinin hesabı cari hesaptır. Gelir ya da gider yazan kaydın rapordaki
kapsamı her kayıt gibi kategori kuralından gelir (ADR 0020). Kişinin türünü
değiştirmek hiçbir tutarı, geliri ya da gideri değiştirmez. Taksitli ve
kişisiz kayıt kendi kapsamını taşır.

**İ6 — Tek sayfa, tek kişi sayfası.** Borç, alacak ve cari tek sayfadadır;
kişi nereden açılırsa açılsın aynı sayfa açılır. Bir tutar listede iki satırda
yer almaz ve toplamlar sunucudan gelir.

**İ7 — Taksitli kaydın ödemesi kendi sayfasından yapılır.** Kişinin toplamı
taksitli kayıtların kalanını içerir; kişinin `Tahsilat` ve `Ödeme` düğmeleri
yalnız hesabına yazar.

**İ8 — Uygulama faiz hesaplamaz.** Kredide tutarları kullanıcı bankanın
planından girer.

Reddedilenler:

| Reddedilen | Neden |
|---|---|
| Girişte takip biçimini sormak (ADR 0021 İ3) | Muhasebe yöntemi sorusu; her ekrana sızıyor |
| Her alımın ayrı borç olması ve ödemenin dağıtılması | Dağıtım kaydı, geri alma sırası, bağlanmamış ödeme; ön muhasebe yükü; kullanıcı 29 Ekim'de reddetti |
| Ödemeyi seçilen kayda bağlamak (ilk sürümde) | Her ödemeye bir adım ve yeni bir bağ kaydı ekler; isteğe bağlı işaret olarak sonra eklenebilir |
| Kişi başına iki hesap (işletme ve şahsi) | Toptancıdan eve alınan malı iki hesaba böler, tek ödeme iki kayıt ister, hedef sorusunu geri getirir |
| Kapsamı kayıt kayıt tutup bakiyeyi hesapla bölmek | Ödeme kapsam taşımıyor; bölme varsayıma dayanır |
| Ayrı `Cari hesap` listesi ve sayfası | Aynı kişi dört listede görünüyordu |

## Başlangıç tasarımı (uygulamada değişebilir)

Ayrıntı `stages/06.3-borc-alacak/02-TASARIM.md` içindedir; burada yalnız
çerçeve durur.

- **Hesap kayıtları:** veresiye satış, vadeli alım, borç verdim, borç aldım,
  avans, tahsilat, ödeme, açılış bakiyesi, indirim, vazgeçme.
- **Ekranlar:** `Borç ve alacak` (özet, `Alacaklar · Borçlar · Kişiler`,
  `Geciken · Yaklaşan · Diğer`), kişi sayfası, kayıt sayfası, tek adımlı
  tahsilat/ödeme paneli, `Ne oldu?` ile açılan yeni kayıt formu.
- **Sunucu farkı:** hesapta borç para, hesaba kredi kartıyla ödeme,
  borçlandırmada istek kimliği, kişide tür, vazgeçme ve kalanı indirim sayma,
  iki okuma (kişi sayfası, liste), fişin "henüz ödemedim" yönlendirmesi,
  yükümlülük uçlarının kalkması, yedek sürümü.
- **İlk sürümde olmayanlar:** seçerek ödeme, çek ve senet takibi (vade olarak
  kalır), mahsup, iade alacağı, kişi birleştirme, hatırlatma mesajı ve ekstre
  paylaşımı, gider ve gelir formunda kişi alanı, altın ve döviz borcu.
- **Eski veri:** kişiye bağlı taksitsiz `Debt` kayıtları taşınmaz ve
  uydurulmaz; okunur ve kişi sayfasında ayrı satır olarak görünür. Eski
  kişinin türü bilinmiyorsa uydurulmaz.

## Bilinen bedeller

- "Yeni faturayı ödedim, eskisi dursun" denemez; toplam her zaman doğrudur,
  etkilenen yalnız hangi kaydın gecikmiş göründüğüdür.
- Hem müşteri olan hem şahsen borç verilen kişide iki tutar tek hesapta
  toplanır; ayrı tutmak isteyen ayırt edici adla ikinci bir kişi açar.
- Kişiye bağlı eski taksitsiz borçlar bir süre ayrı satır olarak yaşar.

## Kabul

`02-TASARIM.md` §9'daki senaryolar; kapanışta gerçek API ve emülatörde denenir.
