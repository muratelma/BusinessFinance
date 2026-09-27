# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 1 · Gelir ve gider kaydi — tek icerik kaynagi."""

NO = 1
BASLIK = "Gelir ve gider kaydı"
ANA_SORU = "Aynı parayı beş ürüne yazdığımızda hangi sayılar değişiyor, hangileri değişmiyor?"
EN_AZ_KARE = 12

GIRIS = [
    "Beş üründe aynı çekirdek veri var: 22.000 açılış, 25.000 hizmet geliri, 850 ve 1.200 "
    "gider, 3.000 hesap aktarımı, 1.200 kart ödemesi. Bu bölüm tek soruyu izliyor — kayıt "
    "tuşuna basıldıktan sonra ekrandaki hangi sayı kıpırdıyor.",
    "Kaydın kendisi beş üründe de sorunsuz oluşuyor. Ayrışma kayıttan sonra başlıyor ve iki "
    "yerde toplanıyor: ayın gelir/gider toplamının neyi saydığı, ve kaydın kaç deftere birden "
    "yazıldığı.",
]
GIRMEZ = [
    "Kart harcaması ve taksit → Bölüm 3",
    "Hesaplar arası aktarımın kendisi → Bölüm 2",
    "Raporun dönem ve filtre davranışı → Bölüm 7",
    "Formun alan yerleşimi ve görsel dili → Belge 1 §4",
]
KAPSAM = [
    ("Money Manager", ["canli"], "Form, liste, istatistik ve hesap toplamı kareli."),
    ("Bluecoins", ["canli"], "Form, liste ve dönem raporu kareli."),
    ("Wallet", ["canli"], "Hızlı form, Records ve Cash-flow kareli."),
    ("Hesap Defterim", ["canli"], "Defter modeli ve birleşik rapor kareli."),
    ("Goodbudget", ["canli"], "Gelir yolu ayrı bir koşumda uçtan uca izlendi."),
    ("KolayBi", ["kaynak"], "Fatura formu destek görselinde; davranış görülmedi."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], None),
]
ACILIS_DAYANAK = (
    "Rozetler kanıt düzeyini gösterir: ölçülen davranış Canlı kare, kanıttan çıkarılan neden "
    "Çıkarım. Tutarlar ürünler arasında farklı günlerde koşuldu; farkın sonucu değiştirdiği "
    "yerde ayrıca yazılıdır."
)

SAYFALAR = [
    {"tur": "acilis"},

    # ------------------------------------------------------------------ 1.1
    {
        "tur": "yanyana", "no": "1.1",
        "baslik": "Form ne soruyor: aynı kaydı beş ayrı biçimde yazmak",
        "giris": "Üç üründe form aynı üç şeyi soruyor — tutar, hesap, kategori. Dördüncüsü "
                 "kategoriyi hiç sormuyor, beşincisi hesabın yerine zarf soruyor. Ayrışma "
                 "buradan değil, bu formların sonucundan çıkıyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "f-mm", "e": "E0229", "ad": "Money Manager", "etiket": "İşlem formu",
             "satirlar": ["Üstte Gelir / Gider / Havale seçici; karede Gider açık.",
                          "Tutar · Kategori · Hesap · Not.",
                          "Kategori 11 kutuluk panelden seçiliyor."]},
            {"k": "f-bc", "e": "E0020", "ad": "Bluecoins", "etiket": "İşlem formu, GELİR seçili",
             "satirlar": ["+25.000 TRY, 3 Ağustos 2026.",
                          "Kategori Diğer/Diğer, hesap Banka/Ana Hesap.",
                          "Altta Planlı İşlemler · Bölmek · Durum · Etiket."]},
            {"k": "f-wl", "e": "E0277", "ad": "Wallet", "etiket": "Hızlı form",
             "satirlar": ["Tür bir sekme; karede Expense seçili ve zeminle birleşik.",
                          "Tutar, Account ANA HESAP, Category SALE.",
                          "Ayrıntı kaydettikten sonra açılıyor."]},
            {"k": "f-hd", "e": "E0138", "ad": "Hesap Defterim", "etiket": "Alındı formu",
             "satirlar": ["Alındı / Ödendi seçici; kategori alanı yok.",
                          "Tutar 25.000 ve serbest metin Notlar.",
                          "Kaydın adı yalnız bu nottan geliyor."]},
            {"k": "f-gb", "e": "E0419", "ad": "Goodbudget", "etiket": "From New Income, dolu",
             "satirlar": ["Received from · Amount · Account · Date.",
                          "Hesabın yanında How to fill Envelopes.",
                          "Zarf seçilmeden kayıt tamamlanmıyor."]},
        ],
        "notlar": [
            "Money Manager ve Bluecoins türü bir seçiciyle, Wallet bir sekmeyle, Hesap Defterim "
            "iki adlandırılmış düğmeyle soruyor. Hesap Defterim'de bu iki düğmenin adı "
            "kullanıcı tarafından değiştirilebiliyor — koşumda Alındı/Ödendi, Tahsilat/FaturaOdemesi "
            "oldu ve bütün ekrandaki başlıklar onunla birlikte değişti.",
            "Goodbudget'ta hesap alanı formda var ama kaydı tamamlayan şey zarf seçimi; zarf "
            "seçilmeden gelir reddediliyor.",
        ],
        "sag_notlar": [
            ("Hesap Defterim kategoriyi ayrı bir serbest metin kutusunda soruyor (Açıklama / "
             "Kategori) ve bu alan seçici değil, yazılan metin.", "E0151"),
            ("Bluecoins'in Bölmek alanı tek kaydı birden çok satıra bölüyor: her parça kendi "
             "tutarını, kategorisini, hesabını, durumunu ve etiketini taşıyor. Koşumda yazılan "
             "50 + 60'lık kayıt listede tek satır olarak, \"2 Kategoriler\" diye duruyor.",
             "E0443 · E0444"),
            ("Wallet'ta tür ile kategori birbirini denetlemiyor: Expense seçiliyken kategori "
             "SALE duruyor; yanlış tür seçilmiş kayıt formda kendini ele vermiyor.", "E0277"),
        ],
    },

    # ------------------------------------------------------------------ 1.2
    {
        "tur": "tablo", "no": "1.2", "baslik": "Kayıttan sonra hangi sayı kıpırdadı",
        "giris": "Aynı çekirdek veri girildikten sonra her üründe okunan değerler. Beş üründen "
                 "dördü aynı iki sayıda buluşuyor; ayrışan iki ürün iki ayrı nedenle ayrışıyor.",
        "sutunlar": [("", 15), ("Money Manager", 17), ("Bluecoins", 17), ("Wallet", 17),
                     ("Hesap Defterim", 17), ("Goodbudget", 17)],
        "boy": 8.4,
        "satirlar": [
            [
                "Hesap bakiyesi",
                {"t": "Varlıklar 44.950", "tur": "canli", "d": "E0235"},
                {"t": "Net 44.950", "tur": "canli", "d": "E0032"},
                {"t": "Üç hesap toplamı 22.950 (kart öncesi)", "tur": "canli", "d": "E0283"},
                {"t": "Denge 44.950", "tur": "canli", "d": "E0142"},
                {"t": "41.734 — gelir kaydı bakiyeyi değiştirmedi", "tur": "canli", "d": "E0421"},
            ],
            [
                "Ayın gelir toplamı",
                {"t": "Gelir 25.000 (Ağu)", "tur": "canli", "d": "E0231"},
                {"t": "25.000 (Ağu)", "tur": "canli", "d": "E0022"},
                {"t": "Income 25.000", "tur": "canli", "d": "E0280"},
                {"t": "Toplam Alındı 51.200 — açılış ve aktarım bacakları dâhil", "tur": "canli", "d": "E0142"},
                {"t": "Income 3.284 (Eyl) — zarfa giren para", "tur": "canli", "d": "E0423"},
            ],
            [
                "Ayın gider toplamı",
                {"t": "Gider 2.050", "tur": "canli", "d": "E0231"},
                {"t": "GİDER −2.050", "tur": "canli", "d": "E0023"},
                {"t": "Expenses −2.050", "tur": "canli", "d": "E0280"},
                {"t": "Toplam Ödendi 6.250", "tur": "canli", "d": "E0142"},
                {"t": "Spending 0 (Eyl)", "tur": "canli", "d": "E0423"},
            ],
            [
                "Kategori kırılımı",
                {"t": "Pasta: Diğer %58,5 · Yiyecek %41,5", "tur": "canli", "d": "E0231"},
                {"t": "Kategori ve alt kategori satırları", "tur": "canli", "d": "E0023"},
                {"t": "Donut: Communication, PC, Food & Drinks", "tur": "canli", "d": "E0278"},
                {"t": "Yok — okuma yüzeyi dönem filtreli liste, kırılım taşımıyor", "tur": "canli", "d": "E0142"},
                {"t": "Zarf başına kalan", "tur": "canli", "d": "E0417"},
            ],
            [
                "Aktarım ay toplamına giriyor mu",
                {"t": "Hayır — aktarım satırının gün başlığı 0/0", "tur": "canli", "d": "E0230"},
                {"t": "Hayır — iki bacak aynı günde netleşiyor", "tur": "canli", "d": "E0022"},
                {"t": "Hayır — çift satır, net etki 0", "tur": "canli", "d": "E0279"},
                {"t": "Evet — iki bacak da Alındı/Ödendi'ye giriyor", "tur": "canli", "d": "E0142"},
                {"t": "Transfer ücretli pakette; tek hesapla denenemedi", "tur": "yok", "d": "E0120"},
            ],
            [
                "Silinen kayıt geri alınabiliyor mu",
                {"t": "Hayır — silme onaylı; menü ızgarasında çöp kutusu yok", "tur": "canli", "d": "E0415"},
                {"t": "Evet — çöp kutusundan geri yükleniyor; kayıt listeye yeniden açılınca dönüyor", "tur": "kosum", "d": "E0089 · E0401 · E0402 · kullanıcı kontrolü"},
                {"t": "Hayır — silme onaylı; çekmece ve ayarlarda çöp kutusu yok", "tur": "canli", "d": "E0452"},
                {"t": "Silinmiş işlemler listesi + Geri Yükle", "tur": "canli", "d": "E0167"},
                {"t": "Hayır — tek onayla siliniyor; ayarlarda geri alma yok", "tur": "canli", "d": "E0125 · E0453"},
            ],
            [
                "Kaydın adı nereden geliyor",
                {"t": "Not alanı", "tur": "canli", "d": "E0229"},
                {"t": "Not alanı", "tur": "canli", "d": "E0020"},
                {"t": "Note / Payee", "tur": "canli", "d": "E0294"},
                {"t": "Notlar (tek kimlik kaynağı)", "tur": "canli", "d": "E0138"},
                {"t": "Received from", "tur": "canli", "d": "E0419"},
            ],
        ],
        "notlar": [
            "Money Manager, Bluecoins ve Wallet'ın satırları Ağustos 2026 dönemine, Goodbudget'ınki "
            "Eylül 2026'ya ait; bu ürünün gelir yolu ayrı bir koşumda izlendi. Dönem farkı yalnız "
            "bu satırda sonucu değiştirdiği için yazıldı.",
            "Wallet'ın hesap toplamı kart ödemesi öncesi karede okundu; diğer dördü koşum sonu "
            "değerleridir.",
            ("Tutar düzenlemesi yalnız Money Manager'da ölçüldü: sonradan eklenen bir kaydın "
             "tutarı 0'dan 580'e çıkarılınca ayın gideri aynı anda 580 arttı; onay veya yeniden "
             "hesaplama adımı yok. Diğer dört üründe denenmedi.", "E0434 · koşum kaydı"),
        ],
    },

    # ------------------------------------------------------------------ 1.3
    {
        "tur": "yanyana", "no": "1.3",
        "baslik": "Aynı ay, aynı veri, iki ayrı gider toplamı",
        "giris": "Ağustos'un gideri üç üründe 2.050, birinde 6.250. İkisi de kendi tanımına göre "
                 "doğru; aradaki 4.200 tam olarak hesaplar arası aktarımın ve kart ödemesinin "
                 "iki bacağı.",
        "yukseklik": 262,
        "sekiller": [
            {"k": "r-mm", "e": "E0231", "ad": "Money Manager", "etiket": "İstatistik · Ağustos",
             "satirlar": ["Gelir 25.000 · Gider 2.050.",
                          "Aktarım ve kart ödemesi toplamda yok."]},
            {"k": "r-bc", "e": "E0023", "ad": "Bluecoins", "etiket": "Net Kazançlar · Ağustos",
             "satirlar": ["GİDER −2.050; aynı iki gider.",
                          "Aktarımın iki bacağı gün netini 0 yapıyor."]},
            {"k": "r-hd", "e": "E0142", "ad": "Hesap Defterim", "etiket": "Bütün Hesaplar · Aylık",
             "satirlar": ["Toplam Alındı 51.200 · Ödendi 6.250.",
                          "Denge 44.950 — diğer ürünlerle aynı net.",
                          "Kategori kırılımı yok."]},
        ],
        "notlar": [
            "Hesap Defterim'in 6.250'si şu toplam: gerçek gider 2.050 + aktarımın çıkış bacağı "
            "3.000 + kart ödemesinin çıkış bacağı 1.200 — dördü de karede okunuyor. 51.200'ün "
            "içindeki 22.000'lik açılış satırları ise listenin kesilen alanında kalıyor ve "
            "aritmetikle bulunuyor: 25.000 gelir + 4.200 giriş bacağı + 22.000 açılış.",
            "Hesap Defterim yine de doğru nete varıyor: 51.200 − 6.250 = 44.950. Money "
            "Manager'ın Toplam satırı ve Bluecoins'in neti de aynı sayıyı veriyor. Ayrışan şey "
            "net değil, netin iki bileşeni.",
        ],
        "sag_notlar": [
            ("Wallet aynı dönemde Income 25.000 / Expenses −2.050 okuyor; üç ürün bu ikilide "
             "birebir aynı.", "E0280"),
            ("Hesap Defterim'in gözlem formu bu toplamı kendi ölçüsünde \"karışık\" diye "
             "işaretliyor: açılış + transfer + gerçek gelir/gider.", "E0142"),
        ],
    },

    # ------------------------------------------------------------------ 1.4
    {
        "tur": "yanyana", "no": "1.4",
        "baslik": "Goodbudget: gelir girdi, rapora yazıldı, hesap kıpırdamadı",
        "giris": "Bu ürünün gelir yolu ayrı bir koşumda uçtan uca izlendi: tek bir kayıt girildi ve "
                 "sonucu her katmanda ayrı ayrı okundu. Kayıt 1.234, Beta Tasarim, hesap Ana Hesap, "
                 "tamamı Tasarim Yazilimi zarfına. Dört kare kaydın önce ve sonrasını gösteriyor.",
        "yukseklik": 190,
        "sekiller": [
            {"k": "gb-z1", "e": "E0417", "ad": "Zarflar · önce", "etiket": "Total 43.784,00",
             "kirpma": (0, 120, 1080, 1245),
             "satirlar": ["Tasarim Yazilimi zarfı 0,00.", "Available 20.000,00."]},
            {"k": "gb-z2", "e": "E0420", "ad": "Zarflar · sonra", "etiket": "Total 45.018,00",
             "kirpma": (0, 120, 1080, 1245),
             "satirlar": ["Tasarim Yazilimi 1.234,00.", "Available değişmedi."]},
            {"k": "gb-h1", "e": "E0418", "ad": "Hesap · önce", "etiket": "All Accounts 41.734,00",
             "kirpma": (0, 120, 1080, 1245),
             "satirlar": ["Tek hesap: Ana Hesap.", "Zarf toplamıyla farkı 2.050."]},
            {"k": "gb-h2", "e": "E0421", "ad": "Hesap · sonra", "etiket": "All Accounts 41.734,00",
             "kirpma": (0, 120, 1080, 1245),
             "satirlar": ["Değişmedi.", "Fark 2.050'den 3.284'e çıktı."]},
        ],
        "notlar": [
            "İşlem listesi kaydı hesaba atfedilmiş gösteriyor: 09/20 Beta Tasarim +1.234,00 "
            "Ana Hesap. Rapor da geliri sayıyor: Eylül Income 2.050 → 3.284.",
            "Uygulama kapatılıp yeniden açıldıktan sonra hesap yine 41.734,00. Bunun nedeni — "
            "senkronizasyon, ücretsiz paket sınırı veya tasarım tercihi — bu koşumdan çıkmıyor.",
        ],
        "sag_notlar": [
            ("Zarf toplamı ile hesap arasındaki fark tam olarak kaydın tutarı kadar büyüdü: "
             "2.050 → 3.284.", "E0420"),
            ("Eylül raporu Income 3.284 · Spending 0 · Net Total 3.284.", "E0423"),
            ("Sıradan gider kayıtları hesaba işliyor: açılış 20.000 → 25.000 gelir → −850 → "
             "−1.200 zinciri hesap ekranında doğrulanmıştı.", "E0130"),
        ],
    },

    # ------------------------------------------------------------------ 1.5
    {
        "tur": "yanyana", "no": "1.5",
        "baslik": "Kayıt yanlışsa: ürün önce mi durduruyor, sonra mı geri alıyor",
        "giris": "Hatalı kayıt iki ayrı yerde yakalanabilir — kaydetmeden önce formda, veya "
                 "kaydettikten sonra geri almada. Beş ürün bu ikisini farklı dağıtmış.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "h-wl", "e": "E0281", "ad": "Wallet", "etiket": "Sıfır tutar reddediliyor",
             "satirlar": ["0 TRY'de \"Please fill in the amount.\"",
                          "Uyarı alan dışında, snackbar olarak."]},
            {"k": "h-mm", "e": "E0232", "ad": "Money Manager", "etiket": "Hesap boşken toast",
             "satirlar": ["Hesap seçilmeden kaydetme durduruluyor.",
                          "Uyarı yine alan dışında."]},
            {"k": "h-mm2", "e": "E0434", "ad": "Money Manager", "etiket": "Sıfır tutar kabul edildi",
             "satirlar": ["Tutarı boş kayıt ₺0,00 olarak listeye düştü.",
                          "Uyarı yok; ayın toplamı değişmedi."]},
            {"k": "h-hd", "e": "E0163", "ad": "Hesap Defterim", "etiket": "Sıfır tutar kabul edildi",
             "satirlar": ["Boş tutar sessizce reddediliyor…",
                          "…ama 0 kabul ediliyor ve başlıksız satır düşüyor."]},
            {"k": "h-hd2", "e": "E0167", "ad": "Hesap Defterim", "etiket": "Silinmiş işlemler",
             "satirlar": ["Silinen kayıt ayrı bir listede duruyor.",
                          "Bağlam menüsü: Geri Yükle / Silme."]},
        ],
        "notlar": [
            ("Geri alma yalnız iki üründe var: Hesap Defterim'in Silinmiş işlemler listesi ve "
             "Bluecoins'in çöp kutusu. Wallet, Money Manager ve Goodbudget'ta silme bir onay "
             "diyaloğundan geçiyor; menüleri ve ayarları tarandı, kurtarma yüzeyi bulunamadı.",
             "E0167 · E0089 · E0415 · E0452 · E0453"),
            ("Money Manager ve Wallet'ta hata mesajı formun alanının yanında değil, ekranın "
             "altında beliriyor. Ama sıfır tutarda ikisi ayrılıyor: Wallet kaydı durduruyor, "
             "Money Manager tutarı boş kaydı ₺0 olarak kabul ediyor. Hesap Defterim de sıfırı "
             "kabul ediyor ve hiç mesaj vermiyor.", "E0281 · E0434 · E0163"),
        ],
        "sag_notlar": [
            ("Bluecoins'te sıfır tutarlı kayıt ve silme onayı aynı karede; kaydetme sırasında "
             "hiç uyarı çıkmaması koşum notudur.", "E0024"),
            ("Goodbudget tek onaylı silme kullanıyor.", "E0125"),
            ("Hesap Defterim'de kalıcı silme ikinci bir onay istiyor.", "E0168"),
        ],
    },

    # ------------------------------------------------------------------ 1.6
    {
        "tur": "yanyana", "no": "1.6",
        "baslik": "Aynı soru ön muhasebe tarafında: kayıt ile ödeme ayrı iki adım",
        "giris": "KolayBi'nin destek görselleri ürünün gerçek ekranlarıdır; davranışı "
                 "ölçülmedi, yalnız yüzeyi okunabiliyor. Gider formunun alanları Belge 1'de "
                 "tek tek gösterilmişti ve oradaki not şunu devretmişti: Ödendi seçiminin "
                 "kasaya ve rapora etkisi görülmedi. Soldaki kare o kaydın sonrasını, sağdaki "
                 "ise kaydın kategorisinin nereden geldiğini gösteriyor.",
        "yukseklik": 238,
        "sekiller": [
            {"k": "kb-detay", "e": "E0199", "ad": "KolayBi", "etiket": "Gider detayı",
             "kirpma": (96, 112, 1305, 865),
             "satirlar": ["Kayıt oluştuktan sonra Ödeme Ekle ayrı bir eylem.",
                          "Durum rozetleri: Bedelsiz · Yeni · Tahsilata Kapalı.",
                          "Tekrarlı Genel Gidere Dönüştür aynı menüde."]},
            {"k": "kb-tipler", "e": "E0193", "ad": "KolayBi", "etiket": "Gider tipleri",
             "kirpma": (96, 112, 1305, 865),
             "satirlar": ["Kategori kartları: Ulaşım/Konaklama, Temel Giderler, Vergi, Diğer.",
                          "Her kartın altında tip satırları — kategori iki katmanlı.",
                          "Kullanıcı hem yeni tip hem yeni kategori ekleyebiliyor."]},
        ],
        "notlar": [
            "Canlı beş üründe kayıt ile ödeme tek adımdır: kaydettiğiniz anda bakiye değişir. "
            "Burada iki ayrı adım var ve ikisi arasında kayıt bir durum rozetiyle bekliyor. "
            "Bunun para akışında ne anlama geldiği Bölüm 9'da kurulur.",
            "Kategori burada iki katmanlı: üstte kategori kartı, altında tip satırları. Canlı "
            "beşte kategori düz bir listedir — Money Manager'ın Ağustos pastasındaki tek "
            "\"Diğer\" başlığının altında iki ayrı gider birleşiyor.",
        ],
        "urunler_baslik": "Aynı soruda kaynaktan okunan üç ürün",
        "urunler": [
            ("Paraşüt", "beyan",
             "Kaynak beş gider kaydı türü sayıyor ve kaydın ödemeden ayrı olduğunu yazıyor: "
             "önce gider oluşturulur, sonra ödeme eklenir; kasa/banka ancak ödeme adımında "
             "azalır. Kısmi ödeme destekleniyor. Bölüm 9'da kurulur.", "parasut.com kılavuzu"),
            ("Logo İşbaşı", "beyan",
             "Kaynak gelir ve giderin fatura ve fişlerle kaydedildiğini, tek kaydın cari, "
             "kasa-banka ve stok defterlerini birlikte güncellediğini yazıyor. "
             "Bölüm 9'da kurulur.", "isbasi.com"),
            ("QuickBooks Solopreneur", "beyan",
             "Kaynağa göre kayıt elle değil, bağlı banka ve kart hesaplarından otomatik "
             "iniyor; kullanıcının işi kaydı oluşturmak değil, inen kaydı sınıflamak. "
             "Bölüm 6'da kurulur.", "Intuit yardım merkezi"),
        ],
    },

    # ------------------------------------------------------------------ 1.7
    {
        "tur": "akis", "no": "1.7", "baslik": "Kaydın yolu ve ayrıldığı noktalar",
        "giris": "Beş üründe kayıt aynı yolda başlıyor. Yol dört adımda ilerliyor ve ilk adımdan "
                 "sonra üç kez çatallanıyor. Aşağıdaki her dal, önceki sayfalarda kareyle "
                 "gösterilen davranışın yoldaki yeridir.",
        "adimlar": [
            {
                "baslik": "Kayıt giriliyor",
                "ortak": "Tutar, tarih ve tür beş üründe de soruluyor. Kategori üçünde "
                         "seçiciden, birinde serbest metinden geliyor, birinde yerini zarf "
                         "alıyor. Yol buraya kadar ortak.",
            },
            {
                "baslik": "Kayıt kaç deftere yazılıyor",
                "dallar": [
                    {"urunler": "Money Manager · Bluecoins · Wallet · Hesap Defterim",
                     "metin": "Tek defter. Kayıt hesabın defterine düşüyor, bakiye aynı anda "
                              "değişiyor.", "d": "E0235"},
                    {"urunler": "Goodbudget", "vurgu": True,
                     "metin": "İki defter: zarf ve hesap. Zarfı dolduran kayıt hesap adını "
                              "taşısa bile hesap defterine girmiyor; aradaki fark her "
                              "doldurmada büyüyor.", "d": "E0421"},
                ],
            },
            {
                "baslik": "Ayın toplamına ne giriyor",
                "dallar": [
                    {"urunler": "Money Manager · Bluecoins · Wallet",
                     "metin": "Yalnız gelir ve gider. Aktarım ile kart ödemesinin iki bacağı "
                              "netleşiyor: Ağustos gideri 2.050.", "d": "E0231"},
                    {"urunler": "Hesap Defterim", "vurgu": True,
                     "metin": "Deftere giren her satır. Açılış bakiyesi ve aktarım bacakları "
                              "da toplama giriyor: aynı ayın gideri 6.250.", "d": "E0142"},
                    {"urunler": "Goodbudget", "vurgu": True,
                     "metin": "Zarfa giren para gelir sayılıyor; hesap bakiyesi bu toplamın "
                              "dışında kalıyor.", "d": "E0423"},
                ],
            },
            {
                "baslik": "Kayıt yanlışsa ne oluyor",
                "dallar": [
                    {"urunler": "Bluecoins · Hesap Defterim",
                     "metin": "Silinen kayıt ikinci bir listede bekliyor ve geri "
                              "yüklenebiliyor.", "d": "E0167"},
                    {"urunler": "Wallet",
                     "metin": "Form sıfır tutarı durduruyor; silinen kaydın kurtarma "
                              "yüzeyi yok.", "d": "E0281 · E0452"},
                    {"urunler": "Money Manager",
                     "metin": "Boş hesabı durduruyor ama sıfır tutarı kabul ediyor; kurtarma "
                              "yüzeyi yok.", "d": "E0434 · E0415"},
                    {"urunler": "Goodbudget",
                     "metin": "Tek onayla siliniyor; ayarlarda kurtarma yüzeyi yok.",
                     "d": "E0125 · E0453"},
                ],
            },
        ],
        "notlar": [
            "Üç çatallanmanın üçü de aynı yerde toplanıyor: ürünün kaydı kaç deftere yazdığı, "
            "sonraki iki adımı da belirliyor. Tek defter tutan dört üründen üçü ayın toplamında "
            "da buluşuyor; ayrışan ikisi kendi defter tanımını toplamlara taşıyor.",
            "Bu sayfa yeni bir kanıt getirmez; 1.1–1.5'te kareyle gösterilen davranışları "
            "sürecin üzerine yerleştirir.",
        ],
    },

    # ------------------------------------------------------------------ 1.7
    {
        "tur": "cikarim", "no": "1.8", "baslik": "Neden ayrışıyorlar",
        "giris": "Beş üründe form neredeyse aynı şeyi soruyor. Ayrışma formda değil, ürünün "
                 "kaydı kaç deftere yazdığında ve o defterlerin toplamına ne ad verdiğinde.",
        "mekanizma": [
            {
                "baslik": "Toplamın adı aynı, tanımı değil",
                "metin": [
                    "Hesap Defterim'in Alındı/Ödendi'si gelir ve gider değil, deftere giren ve "
                    "çıkan her satırdır. Açılış bakiyesi, aktarımın iki bacağı ve kart ödemesi de "
                    "bu toplamlara giriyor. Bu yüzden Ağustos'un gideri 6.250 okunuyor, oysa "
                    "gerçek gider 2.050.",
                    "Aradaki fark tesadüfi değil: 6.250 − 2.050 = 4.200, ve bu tam olarak iki "
                    "çıkış bacağıdır — 3.000 aktarım + 1.200 kart ödemesi. Aynı iki hareketin "
                    "giriş bacakları da 51.200'ün içinde duruyor. Ürün yanlış toplamıyor — "
                    "farklı bir şey topluyor ve ona aynı adı veriyor.",
                    "Netin kendisi doğru kalıyor: 51.200 − 6.250 = 44.950, ve bu sayı Money "
                    "Manager ile Bluecoins'in net değerine birebir eşit. Kullanılamaz hâle gelen "
                    "şey net değil, netin iki bileşeni.",
                ],
                "dayanak": "E0142, E0140, E0235, E0032",
            },
            {
                "baslik": "Goodbudget'ta zarf ve hesap iki ayrı defter",
                "metin": [
                    "Zarf katmanı ile hesap katmanı arasındaki fark, hesaba yazılmayan kayıtların "
                    "toplamına eşit. Gelir kaydından önce 2.050'ydi ve karşılığı 09/11 tarihli "
                    "Initial Envelope Fill +2.050 satırıydı; yeni kayıttan sonra tam 1.234 "
                    "büyüyerek 3.284 oldu.",
                    "Sıradan gider kayıtları hesaba işliyor — açılıştan başlayan zincir hesap "
                    "ekranında doğrulanıyor. Ayrışan şey kayıt türü: zarf dolduran kayıt, formda "
                    "hesap adını taşısa bile hesap defterine girmiyor.",
                    "Bunun nedeni koşumdan çıkmıyor ve ürün kusuru olarak yazılmıyor; ölçülen "
                    "şey sonucun kendisi.",
                ],
                "dayanak": "E0417, E0418, E0420, E0421, E0422, E0130",
            },
            {
                "baslik": "Kaydın kimliği kategoriden değil, kullanıcının yazdığından geliyor",
                "metin": [
                    "Seçicisi olan dört üründe kategori — Goodbudget'ta zarf — paylaşılan bir "
                    "kovadır: Money Manager'ın Ağustos pastasında Mavi Yazılım gideri yalnız "
                    "\"Diğer %58,5\" olarak okunuyor; aynı \"Diğer\" adını Ada Reklam geliri de "
                    "taşıyor. Kaydı ayırt eden şey Not, Payee veya "
                    "Received from alanına yazılan metin.",
                    "Hesap Defterim bu ayrımı tersinden kuruyor: kimlik Notlar alanında, kategori "
                    "ise seçici değil, adı \"Açıklama / Kategori\" olan ikinci bir serbest metin "
                    "kutusu. Açıklama ile kovayı tek kutuda birleştirmenin sonucu, kovanın "
                    "kaybolması: aynı harcama her seferinde başka yazılabiliyor ve okuma "
                    "yüzeyinde kategori kırılımı hiç yok.",
                ],
                "dayanak": "E0231, E0138, E0142, E0151",
            },
        ],
        "kazanc": [
            ("Money Manager",
             "Gelir/gider toplamı ile net varlık iki ayrı ekranda ve ikisi de tutarlı; aktarım "
             "toplamlara karışmıyor",
             "Kategori kırılımı tek kovada toplanınca aynı kategorideki iki farklı gider "
             "ayırt edilemiyor"),
            ("Bluecoins",
             "Aktarımın iki bacağını aynı günde netleyip gün netini 0 gösteriyor; dönem "
             "raporu gerçek gideri veriyor",
             "Form yoğun: tek ekranda kategori, hesap, durum, etiket, bölme ve planlama "
             "birlikte soruluyor"),
            ("Wallet",
             "Hızlı form tutar ve hesapla kaydı bitiriyor, ayrıntı sonraya kalıyor; "
             "Cash-flow sorusu kullanıcının dilinde",
             "Tür bir sekme olduğu için seçili sekme zeminle birleşiyor ve yanlış tür "
             "seçmek kolay"),
            ("Hesap Defterim",
             "Defter mantığı tek sayıda doğru neti veriyor ve satır başına yürüyen denge "
             "gösteriyor; düğme adları kullanıcının diline çevrilebiliyor",
             "Ayın gelir ve gider toplamı kullanılamıyor: açılış ve aktarım bacakları aynı "
             "toplamın içinde"),
            ("Goodbudget",
             "Para amaca bağlanıyor; kullanıcı \"ne kadar harcayabilirim\" sorusunu zarftan "
             "doğrudan okuyor",
             "\"Param nerede\" sorusunun iki cevabı var ve ikisi tutmuyor; fark her zarf "
             "doldurmada büyüyor"),
        ],
        "soru": [
            "Bir ayın \"gelir\" ve \"gider\" toplamı hangi kayıtları içermeli? Hesap Defterim'in "
            "örneği, açılış bakiyesi ve aktarım bacaklarının toplama girmesinin neti bozmadığını "
            "ama iki bileşeni kullanılamaz kıldığını gösteriyor.",
            "Bütçe ilerlemesi ile hesap bakiyesi aynı kaynaktan mı beslenmeli? Goodbudget bu ikisini "
            "ayırdığında kullanıcıya iki farklı \"toplam paran\" sayısı kalıyor.",
            "Kaydın adı nereden gelmeli — kategoriden mi, kullanıcının yazdığı metinden mi? "
            "Seçicisi olan dört üründe kategori kovadır, kimlik değildir.",
        ],
    },
]

EKSIKLER = [
    ("Bluecoins, Wallet, Hesap Defterim, Goodbudget",
     "Bir kaydın tutarını değiştirmenin ay toplamlarına etkisi (Money Manager'da ölçüldü)", "1.2",
     "Mevcut veriyle: bir kaydı düzenleyip ay raporunun önce/sonrasını alın", "Orta"),
    ("Goodbudget", "Keep Available kipi ve FROM AVAILABLE sekmesinin sonucu", "1.4",
     "Mevcut veriye dokunmadan ikinci bir doldurma kipi koşulabilir", "Orta"),
    ("KolayBi, Paraşüt, Logo, QuickBooks", "Kaydın sonucu", "—",
     "Ücretli paket; erişim yok", "Önerilmez"),
]

EK_NOTLAR = (
    "Çıkarım satırları kanıttan türetilmiştir ve dayanakları 1.8'de her blokta yazılıdır; "
    "gözlemden ayrı bir rozet taşırlar.",
)
