# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 9 · Rapor ve donem secimi — tek icerik kaynagi (plan §3)."""

NO = 9
BASLIK = "Rapor ve dönem seçimi"
ANA_SORU = "Rapor ekranı neyi nasıl gösteriyor; dönem ve filtre nasıl seçiliyor?"
GIRIS = [
    "Bu bölüm rapor ekranlarının biçimini, toplamların ekranda hangi etiketle yazıldığını, dönemin ve filtrenin "
    "nereden seçildiğini ve bir hesabı toplamın dışında bırakan denetimleri izler.",
    "Toplamların neyi içerdiği ve doğruluğu bu bölümde anlatılmaz; ekranda görünen etiket, düğme ve alan "
    "anlatılır. Kareler farklı anlara ve veri durumlarına aittir; sayılar birbiriyle karşılaştırılmaz.",
]
GIRMEZ = [
    "Dosya çıktısı ve teslim → 10",
    "Toplamların içeriği ve doğruluğu → Belge 2",
    "Vergi mevzuatı yorumu",
]
KAPSAM = [
    ("Money Manager", ["canli"], None),
    ("Bluecoins", ["canli"], "Kaydedilen filtrenin geri açılması görülmedi."),
    ("Wallet", ["canli"], "Filtre formu görüldü; filtre kaydedilmedi."),
    ("Hesap Defterim", ["canli"], "Grafik yerine tablo ve takvim."),
    ("Goodbudget", ["canli"], None),
    ("KolayBi", ["kaynak"], "Rapor ailesi ve KDV raporu."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], None),
]
KB = (96, 112, 1305, 865)
KAYNAK_NOTLARI = [
    "Goodbudget E0121'in üst çubuğundaki hane adı belge genelinde karartıldı; özgün kanıt değişmedi.",
]

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 9.1
    {
        "tur": "yanyana", "no": "9.1", "baslik": "Rapor ekranı ne gösteriyor?",
        "giris": "Dört üründe rapor grafikle açılıyor: pasta, dönem sütunları, çubuklar ya da rapor kartları. "
                 "Hesap Defterim'de grafik görülmedi; rapor, dönem çipli bir liste.",
        "yukseklik": 262,
        "sekiller": [
            {"k": "mm-rapor", "e": "E0231", "ad": "Money Manager", "etiket": "İstatistik",
             "satirlar": ["Gelir / Gider sekmesi, pasta ve kategori listesi."]},
            {"k": "bc-rapor", "e": "E0023", "ad": "Bluecoins", "etiket": "Net Kazançlar",
             "satirlar": ["İki dönem sütunu; kategori satırları."]},
            {"k": "wl-rapor", "e": "E0280", "ad": "Wallet", "etiket": "Statistics › Cash-flow",
             "satirlar": ["Soru başlıklı kart; gelir ve gider çubukları."]},
            {"k": "gb-rapor", "e": "E0121", "ad": "Goodbudget", "etiket": "REPORTS",
             "satirlar": ["İki rapor kartı: Spending by Envelope, Income vs Spending."]},
            {"k": "hd-rapor", "e": "E0142", "ad": "Hesap Defterim", "etiket": "İşlemler-Bütün Hesaplar",
             "satirlar": ["Grafik yok; dönem çipli liste, altta sabit toplam."]},
        ],
        "notlar": [
            "Beş kare aynı anın veya aynı veri durumunun görüntüsü değildir.",
            ("Wallet'ta 6M ve 1Y aralıkları kilitli.", "E0280"),
        ],
        "sag_notlar": [
            ("Wallet'ın Statistics ekranında Balance, Outlook, Cash-flow, Spending ve Credit sekmeleri var.",
             "E0280"),
            ("Bluecoins raporunda Net Kazançlar, Öğeler Özeti ve Etiketler sekmeleri var.", "E0023"),
            ("Money Manager'da pasta dilimine dokununca kategori ayrıntısı açılıyor: kategori toplamı, aylar "
             "boyunca çizgi grafik ve o kategorinin kayıtları.", "E0466"),
        ],
    },
    {
        "tur": "soru", "no": "9.1", "baslik": "Tablo, takvim ve ayrı satırlar", "haritada": False,
        "giris": "Money Manager'ın Toplam sekmesi gideri ödeme yöntemine göre iki satıra ayırıyor. Money Manager, "
                 "Bluecoins ve Hesap Defterim'de dönem bir takvim olarak da okunuyor.",
        "sekiller": [
            {"k": "mm-toplam", "e": "E0250", "etiket": "Money Manager · Toplam sekmesi, Ağustos",
             "isaretler": [
                 (1, 540, 770, "Hesaplar kutusu: nakit-banka gideri 850; kredi kartı gideri 2.200 (1.200)."),
                 (2, 540, 1420, "Excel(.xlsx) e-posta olarak gönder (→ 10.1)."),
             ]},
            {"k": "bc-takvim", "e": "E0098", "etiket": "Bluecoins · Takvim, bir gün seçili",
             "isaretler": [
                 (3, 950, 363, "Ay takvimi; kayıtlı günlerde renkli noktalar."),
                 (4, 600, 1880, "Seçili günün GİDER, GELİR ve NET KAZANÇLAR kırılımı."),
             ]},
            {"k": "hd-takvim", "e": "E0144", "etiket": "Hesap Defterim · Takvim, Ağustos",
             "isaretler": [
                 (5, 540, 1800, "Gün hücrelerinde yeşil giriş, kırmızı çıkış tutarı."),
                 (6, 540, 2000, "Altta sabit bant: Toplam Alındı, Toplam Ödendi, Denge (→ 9.2)."),
             ]},
            {"k": "mm-takvim", "e": "E0234", "etiket": "Money Manager · İşlemler › Takvim, Ağustos",
             "isaretler": [
                 (7, 390, 960, "Gün hücresinde gelir mavi, gider kırmızı tutar; üstte ayın toplamları."),
             ]},
        ],
        "notlar": [
            ("Money Manager'ın kart satırındaki parantezli 1.200, dönem içindeki kart ödemesi; kartın dönem sonu "
             "borcu değil.", "E0250, E0256"),
            ("Hesap Defterim takvimi seçili defterin toplamını gösteriyor.", "E0144"),
            "Hesap Defterim karesinin altındaki bant bir reklam.",
        ],
    },

    # ---------------------------------------------------------------- 9.2
    {
        "tur": "soru", "no": "9.2", "baslik": "Toplamın etiketi ne söylüyor?",
        "giris": "Hesap Defterim toplamları üç etiketle veriyor: Toplam Alındı, Toplam Ödendi, Denge; etiket, açılış "
                 "bakiyesini veya aktarımı kapsayıp kapsamadığını yazmıyor. Bluecoins iki ayrı ölçüye bir harfle "
                 "ayrılan adlar veriyor: Net Kazançlar ve Net Kazanç. Neyin dahil olduğu → Belge 2 1.3, 7.3.",
        "yukseklik": 384,
        "sekiller": [
            {"k": "hd-etiket", "e": "E0142", "etiket": "Hesap Defterim · İşlemler-Bütün Hesaplar, Aylık",
             "isaretler": [
                 (1, 400, 720, "Aktarım iki satır: Kime ve Kimden."),
                 (2, 700, 2005, "Toplam Alındı, Toplam Ödendi, Denge."),
             ]},
            {"k": "hd-tasarruf", "e": "E0148", "etiket": "Hesap Defterim · Is Karti, gün gün özet",
             "isaretler": [
                 (3, 700, 780, "Üçüncü sütunun adı Tasarruf."),
                 (4, 540, 2000, "Aynı sütun altta Denge adıyla."),
             ]},
            {"k": "bc-bloklar", "e": "E0053", "etiket": "Bluecoins · Hesaplar panosu, iki blok",
             "isaretler": [
                 (5, 520, 822, "Birinci blok: Net Kazançlar."),
                 (6, 400, 1375, "Satırları Gelir, Gider, Net Kazançlar."),
                 (7, 420, 1535, "İkinci blok: Net Kazanç."),
                 (8, 390, 2087, "Satırları Varlıklar, Cari hesap, Net Kazanç."),
             ]},
        ],
        "notlar": [
            "Hesap Defterim'de aynı ölçü iki farklı adla (Tasarruf, Denge) görünüyor.",
            "Bluecoins'te ayrı iki ölçünün adları bir harfle ayrılıyor; blok başlığı dışında ölçüyü anlatan bir "
            "metin karede yok.",
        ],
        "urunler": [
            ("Money Manager", "canli", "Üst bantta Gelir, Gider, Toplam.", "E0250"),
            ("Wallet", "canli", "Cash-flow kartının başlığı bir soru: Am I spending less than I make?", "E0280"),
            ("Goodbudget", "canli", "Income vs Spending kartında Net Total.", "E0121"),
        ],
    },

    # ---------------------------------------------------------------- 9.3
    {
        "tur": "serit", "no": "9.3", "baslik": "Dönem nereden seçiliyor?",
        "giris": "Beş üründe dönem denetimi farklı yerde duruyor: başlıkta ay gezgini, çip satırı, rapor kartının "
                 "içi, rapor başlığının yanı ya da ekranın altı.",
        "kirpintilar": [
            {"k": "mm-donem", "e": "E0231", "kutu": (0, 140, 1080, 240), "ad": "Money Manager",
             "konum": "Ekranın üstü",
             "aciklama": "Oklu ay gezgini; sağda Ay seçicisi. Tam ekran: [[mm-rapor]]."},
            {"k": "hd-donem", "e": "E0142", "kutu": (0, 300, 1080, 480), "ad": "Hesap Defterim",
             "konum": "Başlık çubuğunun altı",
             "aciklama": "Çipler: Herşey, Günlük, Haftalık, Aylık, Yıllık; altında oklu tarih aralığı. Tam ekran: [[hd-rapor]]."},
            {"k": "bc-donem", "e": "E0023", "kutu": (0, 480, 1080, 590), "ad": "Bluecoins",
             "konum": "Rapor başlığının sağı",
             "aciklama": "Dönem düğmesi: Bu Ay. Tam ekran: [[bc-rapor]]."},
            {"k": "gb-donem", "e": "E0121", "kutu": (0, 480, 1080, 600), "ad": "Goodbudget",
             "konum": "Rapor kartının içi",
             "aciklama": "Kart içinde dönem yazıyor; rapor açılışta içinde bulunulan ayı gösterdi. Tam ekran: [[gb-rapor]]."},
            {"k": "wl-donem", "e": "E0280", "kutu": (0, 2120, 1080, 2230), "ad": "Wallet",
             "konum": "Ekranın altı",
             "aciklama": "Aralık çipleri: 7D, 30D, 12W, 6M, 1Y; son ikisi kilitli. Tam ekran: [[wl-rapor]]."},
        ],
        "notlar": [
            ("Hesap Defterim ayarlarında ayın, haftanın ve yılın ilk günü ile varsayılan süre ayarlanıyor.",
             "E0178"),
            ("Wallet'ın gelişmiş ayarlarında muhasebe döneminin başladığı gün: 1.", "E0379"),
            ("Money Manager'ın Toplam görünümü dönemi tarih aralığıyla yazıyor.", "E0250"),
            ("Hesap Defterim'in Haftalık ve Yıllık çiplerinde listenin üstünde bir Önceki denge satırı çıkıyor; "
             "önceki denge ayarla açılıp kapanıyor.", "E0458, E0150"),
            ("Wallet'ın dönem seçicisi üç sayfalı: göreli çipler, adlandırılmış dönem (Today, This week, This "
             "month; This year kilitli) ve özel tarih aralığı.", "E0473, E0475, E0474"),
            "Dönem ayarlarının raporlara etkisi denenmedi.",
            ("Goodbudget'ın her açılışta içinde bulunulan ayla açıldığı tek açılışla sınırlı bir gözlem.",
             "E0121"),
        ],
    },

    # ---------------------------------------------------------------- 9.4
    {
        "tur": "soru", "no": "9.4", "baslik": "Filtre yüzeyi",
        "giris": "Bluecoins filtreyi tek bir alt sayfada topluyor. Money Manager filtreyi rapor üstündeki bir panelde "
                 "sekmelerle veriyor. Wallet'ta özel filtreler ayarlardan tanımlanıyor.",
        "sekiller": [
            {"k": "bc-filtre", "e": "E0082", "etiket": "Bluecoins · filtre alt sayfası",
             "isaretler": [
                 (1, 300, 250, "Sıfırla, kaydet, aç simgeleri."),
                 (2, 1000, 660, "Metin araması ve tutar aralığı."),
                 (3, 1000, 1100, "Tarih, işlem tipi, kategori, hesap, etiket, durum; yanlarında süzgeç."),
             ]},
            {"k": "mm-filtre", "e": "E0251", "etiket": "Money Manager · filtre paneli",
             "isaretler": [
                 (4, 800, 790, "GELİR, GİDER, HESAP sekmeleri."),
                 (5, 470, 1450, "Hesap başına iki sütun çifti: gelir/havale ve gider/havale."),
             ]},
            {"k": "wl-filtre", "e": "E0476", "etiket": "Wallet · Settings › Filters › Add filter",
             "isaretler": [
                 (6, 700, 386, "Filtreye ad veriliyor: kaydedilen bir süzgeç."),
                 (7, 700, 855, "Kaydın onay durumu da bir süzgeç."),
                 (8, 700, 1325, "Etikete göre süzme."),
                 (9, 700, 2250, "Transfers: dahil ya da hariç."),
             ]},
        ],
        "notlar": [
            ("Bluecoins'te Ada araması iki transfer bacağını ve geliri buldu. Kaydet simgesi ad soran bir "
             "sayfa açıyor; kaydedilen filtrenin geri açılması görülmedi.", "E0081, E0492"),
            ("Money Manager'da panelde bir hesap seçilince paneldeki toplam hemen değişiyor; Filtre düğmesi "
             "aynı süzmeyi listeye uyguluyor ve seçili filtre listenin altında yazıyor.", "E0457, E0467"),
            ("Hesap Defterim'de arama çubuğu alt toplamları süzülen kayıtlara göre yeniden yazıyor.", "E0164"),
            ("Money Manager filtre panelinde havale sütun başlıkları verinin yönüyle ters görünüyor.", "E0251"),
            ("Wallet formunun devamında Debts (dahil / hariç) ve metin araması var; filtre Settings › Filters'tan "
             "açılıyor, kaydedilmedi.", "E0477, E0377"),
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Alış/satış raporunda cari, ödeme durumu, para birimi, şube, tarih ve vade "
             "filtreleri.", "E0213"),
            ("Goodbudget", "yok", "Filtre yüzeyi görülmedi.", "E0006"),
            ("Goodbudget", "canli", "İşlem araması var: Transaction Search.", "E0127"),
        ],
    },

    # ---------------------------------------------------------------- 9.5
    {
        "tur": "soru", "no": "9.5", "baslik": "Bir hesabı toplamın dışında bırakmak",
        "giris": "İki üründe bir hesabı bir toplamın dışında bırakan denetim var ve ikisi farklı ölçülere bağlı. "
                 "Money Manager'ın anahtarı hesap formunda, Bluecoins'inki nakit akışı ayarında.",
        "sekiller": [
            {"k": "mm-dahil", "e": "E0252", "etiket": "Money Manager · Hesap Bilgisi",
             "isaretler": [
                 (1, 700, 925, "Toplama Dahil Et anahtarı; altında Göster/Gizle."),
             ]},
            {"k": "mm-gri", "e": "E0253", "etiket": "Money Manager · Hesaplar, anahtar kapalı",
             "isaretler": [
                 (2, 700, 430, "Üst bantta Varlıklar, Borçlar, Toplam."),
                 (3, 600, 493, "Grup satırında 0,00."),
                 (4, 600, 598, "Kapalı hesabın satırı gri."),
             ]},
            {"k": "bc-nakit", "e": "E0086", "etiket": "Bluecoins · Nakit Akım Ayarı",
             "isaretler": [
                 (5, 600, 700, "Nakit akışına katılan hesaplar tek tek seçiliyor."),
             ]},
        ],
        "notlar": [
            "İki denetim eşdeğer sayılmaz: biri hesap listesinin toplamına, öteki nakit akışı raporuna bağlı.",
            ("Bluecoins ayarının kendi cümlesi: \"Nakit akışı hesaplarken kullanılacak nakit hesapları "
             "seçiniz.\" Görülen seçimlerin ürün varsayılanı olup olmadığı doğrulanmadı.", "E0086, E0446"),
            "Kapalı anahtarın toplamlara etkisi → Belge 2 2.5.",
        ],
        "urunler": [
            ("Wallet", "canli", "Hesap ayarında Exclude from stats anahtarı (→ 5.3).", "E0298"),
            ("Hesap Defterim, Goodbudget", "yok", "Bu tür bir denetim görülmedi.", "E0007, E0006"),
        ],
    },

    # ---------------------------------------------------------------- 9.6
    {
        "tur": "soru", "no": "9.6", "baslik": "Boş rapor ve kaynaktaki rapor aileleri",
        "giris": "Goodbudget'ta kaydı olmayan dönemin raporu boş bir grafik ve tek satırlık bir metin gösteriyor. "
                 "KolayBi'nin rapor sayfasında on rapor üst barda yan yana.",
        "not_genislik": 230, "yukseklik": 300,
        "sekiller": [
            {"k": "gb-bos", "e": "E0122", "etiket": "Goodbudget · Spending by Envelope, boş dönem",
             "isaretler": [
                 (1, 950, 340, "Başlıkta dönem; üst çubukta takvim simgesi."),
                 (2, 540, 1700, "Gri pasta, Total Spending 0.00, No transactions found."),
             ]},
            {"k": "kb-kdv", "e": "E0214", "etiket": "KolayBi · KDV Raporu",
             "kirpma": KB, "genislik": 370,
             "isaretler": [
                 (3, 604, 93, "Üst barda on rapor."),
                 (4, 804, 238, "Fatura başlangıç ve bitiş tarihi; Filtrele."),
                 (5, 1054, 364, "Matrahlı anahtarı."),
                 (6, 254, 388, "Oran sütunlarında matrah ve tutar; satırlarda belge türleri."),
             ]},
        ],
        "notlar": [
            "Karedeki matrah ile tutarın oranı, ürünün vergiyi kendisinin hesapladığını göstermez.",
            ("Aynı tarih aralığında KolayBi Gelir/Gider Raporu boş; iki karenin veri anı bilinmiyor.", "E0215"),
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Alış/satış raporunda cari, proje, ürün ve etiket kırılımları.", "E0213"),
            ("Paraşüt", "beyan", "Gelir/Gider ile Kasa/Banka raporları ayrı.", "E0011"),
            ("Logo İşbaşı", "beyan", "Tarih aralıklı, kategorize raporlar ve grafikler.", "E0009"),
            ("QuickBooks Solopreneur", "beyan", "İşletme/şahsi ayrımı vergi raporuna bağlanıyor (→ 6.5).", "E0012"),
        ],
        "dayanak": ["Kaynak beyanı: E0011, E0009, E0012"],
    },
]

EKSIKLER = [
    ("Wallet", "Filtrenin kaydedilip bir raporda uygulanması", "9.4", "Filtre tanımlamak veriyi değiştirmez",
     "Orta"),
    ("Bluecoins", "Kaydedilen filtrenin geri açılması", "9.4", "Kaydet sayfasında ad verip Aç simgesini denemek",
     "Orta"),
    ("Hesap Defterim", "Dönem başlangıç ayarının rapora etkisi", "9.3", "Ayarı değiştirmek", "Orta"),
    ("Paraşüt / Logo İşbaşı", "Çalışan rapor ekranı", "9.6", "Ücretli hesap gerekir", "Önerilmez"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0127", "E0150", "E0377", "E0446", "E0457", "E0458", "E0466", "E0467", "E0473", "E0474",
               "E0475", "E0477", "E0492"]
