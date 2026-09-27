# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 10 · Veri aktarimi, yedek ve paylasim — tek icerik kaynagi (plan §5)."""

NO = 10
BASLIK = "Veri aktarımı, yedek ve paylaşım"
ANA_SORU = "Veri üründen nasıl çıkıyor, ürüne nasıl giriyor ve kimlerle paylaşılıyor?"
GIRIS = [
    "Bu bölüm çıktı seçimini, içe aktarma girişlerini, kayda eklenen dosyaları, yedek ve veri konumunu ve "
    "üçüncü kişilere açılan girişleri izler.",
    "Bölümün ortak sınırı bir kez yazılır: bir seçeneği görmek dosya üretmek değildir; dosya üretmek de "
    "dosyayı alıcıya teslim etmek değildir. Ürün satırlarında yalnız o satıra ait sınır yazılır.",
]
GIRMEZ = [
    "Kaydın finansal sonucu → Belge 2",
    "Rapor ekranı → 9",
    "Diğer modüller → 11",
    "Veri merkezi, şifreleme ve güvenlik hükmü",
]
KAPSAM = [
    ("Money Manager", ["canli"], "Yedek ve Excel çıktısı incelenmedi."),
    ("Bluecoins", ["canli"], None),
    ("Wallet", ["canli", "kosum"], "Bulut hesabının oturumlar arası sürekliliği koşum kaydında."),
    ("Hesap Defterim", ["canli"], "PDF'in içi karede; Excel dosyasının içi görülmedi."),
    ("Goodbudget", ["canli", "kosum"], "Dışa aktarma incelenmedi; yedek göstergesi görüldü, ortak hane karesiz."),
    ("KolayBi", ["kaynak", "beyan"], None),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], None),
]
KB = (96, 112, 1305, 865)

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 10.1
    {
        "tur": "soru", "no": "10.1", "baslik": "Çıktı nereden seçiliyor?",
        "giris": "Hesap Defterim'de çıktı iki yerden başlıyor: bütün hesaplar için dönem soran bir diyalog ve defter "
                 "başına dönem sormayan bir liste. Bluecoins üç biçimi tek bir alt sayfada sunuyor.",
        "sekiller": [
            {"k": "hd-bildiri", "e": "E0169", "etiket": "Hesap Defterim · Bildiri-Bütün Hesaplar",
             "isaretler": [
                 (1, 880, 1065, "Dönem: Herşey ya da tarih aralığı."),
                 (2, 880, 1250, "Biçim: PDF ya da EXCEL."),
             ]},
            {"k": "hd-pdf", "e": "E0427", "etiket": "Hesap Defterim · üretilen PDF",
             "isaretler": [
                 (3, 816, 468, "Başlıkta defter adı ve dönem."),
                 (4, 216, 625, "Önceki denge satırı; kolonlar Tarih, Notlar, Açıklama / Kategori, Gelir, Gider, Denge."),
                 (5, 480, 824, "Aktarım bacağı Gider sütununda düz bir satır."),
                 (6, 180, 1254, "Altta özet: Toplam Gelir, Toplam Gider, Denge."),
             ]},
            {"k": "hd-paylas", "e": "E0176", "etiket": "Hesap Defterim · çıktı sonrası",
             "isaretler": [
                 (7, 300, 1390, "Ürün dosyaların kasadefteri klasörüne kaydedildiğini söylüyor (→ 10.4)."),
                 (8, 700, 1662, "Android paylaşım sayfası: 2 dosya paylaşılıyor."),
             ]},
            {"k": "bc-cikti", "e": "E0105", "etiket": "Bluecoins · İşlemi seçin",
             "isaretler": [
                 (9, 700, 1827, "PDF veya Yazıcıya gönder, Excel (.csv), HTML."),
             ]},
        ],
        "notlar": [
            ("Hesap Defterim'in defter başına Bildiri'si yalnız PDF ve Excel soruyor; dönem sormuyor. Excel "
             "dosyasının içi ve alıcıya teslim görülmedi.", "E0175, E0007"),
            ("Bluecoins'te Excel seçeneği .csv uzantısı taşıyor. Ürünün kendi cümlesi: \"Tüm raporları PDF, "
             "Excel (cvs) veya Html'e aktarmak için soldaki yazıcı simgesinin olduğu her yerde bulunur.\" "
             "Dosya üretilmedi.", "E0105, E0493"),
            ("Wallet'ta dışa aktarma Records menüsünde yok; çekmecedeki katlanmış Others bölümünde (Imports, "
             "Exports, Locations). Exports formu hesap, tür, ödeme türü ve tarih aralığı soruyor; biçimler PDF, "
             "XLS, CSV. Dosya üretilmedi.", "E0399, E0450, E0428"),
        ],
        "urunler": [
            ("Money Manager", "canli", "Toplam sekmesinde Excel(.xlsx) e-posta olarak gönder (→ 9.1).", "E0250"),
            ("KolayBi", "kaynak", "Cari ekstrede tarih, para birimi ve yedi isteğe bağlı kolon; PDF önizleme "
             "(→ 8.5).", "E0196, E0197"),
            ("Paraşüt", "beyan", "KDV dökümünden Excel'e aktarma anlatılıyor.", "E0011"),
            ("Goodbudget", "yok", "Dışa aktarma incelenmedi.", "E0006"),
        ],
    },

    # ---------------------------------------------------------------- 10.2
    {
        "tur": "soru", "no": "10.2", "baslik": "İçe aktarma nereden başlıyor?",
        "giris": "Bluecoins'te içe aktarma, veri yönetimi ekranında iki biçimle yer alıyor. Wallet dosyadan içe "
                 "aktarmayı hesap eklerken bir hesap türü olarak sunuyor.",
        "yukseklik": 384,
        "sekiller": [
            {"k": "bc-ice", "e": "E0091", "etiket": "Bluecoins · Veri Yönetimi",
             "isaretler": [
                 (1, 700, 360, "Aynı ekranda yedekleme ve geri yükleme (→ 10.4)."),
                 (2, 700, 640, "Verileri İçe Aktar: Excel (.csv) ve QIF."),
             ]},
            {"k": "wl-hesap-turu", "e": "E0471", "etiket": "Wallet · hesap türü seçimi",
             "isaretler": [
                 (3, 480, 414, "Bank Sync: banka bağlantısı (denenmedi)."),
                 (4, 480, 1200, "File Import: CSV, Excel, OFX; dosyalar Wallet'a e-postayla gönderiliyor."),
                 (5, 516, 1590, "Manual Input: elle; içe aktarma ya da banka sonradan bağlanabiliyor."),
             ]},
        ],
        "notlar": [
            "İçe aktarılan dosyanın şeması, eşleme ve sonucu denenmedi.",
            ("Wallet'ta ayrıca çekmecede Others › Imports, hesap düzenleme formunda içe aktarma e-postası ve "
             "Enable automatic imports anahtarı var.", "E0450, E0437"),
            "Bluecoins içe aktarmada yalnız CSV ve QIF okuyor; banka ekstresi ayrıştırması → Belge 2 8.1.",
        ],
        "urunler": [
            ("Money Manager, Hesap Defterim, Goodbudget", "yok", "İçe aktarma yüzeyi görülmedi.",
             "E0010, E0007, E0006"),
        ],
    },
    {
        "tur": "soru", "no": "10.2", "baslik": "İçe aktarma nereden başlıyor? — kaynakta", "haritada": False,
        "giris": "KolayBi'nin destek görsellerinde listelerin üstünde İçe Aktar ve Dışarıya Aktar düğmeleri yan "
                 "yana; kayıt satırında e-Fatura durumu ayrı bir sütun.",
        "not_genislik": 250,
        "sekiller": [
            {"k": "kb-ice", "e": "E0198", "etiket": "KolayBi · Genel Giderler",
             "kirpma": KB, "genislik": 520,
             "isaretler": [
                 (6, 554, 288, "İçe Aktar ve Dışarıya Aktar."),
                 (7, 224, 358, "e-Fatura Durumu sütunu: Aktarıldı / Aktarılmadı."),
             ]},
        ],
        "notlar": [
            "e-Fatura sütunu dış bir sistemle bağ olduğunu gösteriyor; bağın işleyişi görülmedi.",
            "Görseller destek materyalidir; demo verisi taşıyor ve güncel sürüm doğrulanmadı.",
        ],
        "urunler_baslik": "Karesi basılmayan ürünler",
        "urunler": [
            ("Paraşüt, Logo İşbaşı, QuickBooks Solopreneur", "yok", "İçe aktarma yüzeyi kaynakta anlatılmıyor ya "
             "da görülmedi; kaynaktan kurulan veri akışı → Belge 2 Bölüm 9.", "E0011, E0009, E0012"),
        ],
    },

    # ---------------------------------------------------------------- 10.3
    {
        "tur": "soru", "no": "10.3", "baslik": "Kayda dosya eklemek ve dosyadan okumak",
        "giris": "Hesap Defterim ve Wallet kayda dosya veya fotoğraf eklemeye izin veriyor. İki üründe de incelenen "
                 "yolda dosyadan tutar veya tarih okunmadı; ek dosya saklamak, dosyadan veri okumak değildir.",
        "yukseklik": 384,
        "sekiller": [
            {"k": "hd-ek", "e": "E0157", "etiket": "Hesap Defterim · Ödendi formu",
             "isaretler": [
                 (1, 850, 1700, "Fatura ekle: Kamera, Fotoğraf Galerisi, PDF."),
             ]},
            {"k": "hd-atac", "e": "E0158", "etiket": "Hesap Defterim · kayıttan sonra",
             "isaretler": [
                 (2, 140, 705, "Kayıt satırında ataç simgesi."),
             ]},
            {"k": "wl-ek", "e": "E0313", "etiket": "Wallet · Record detail",
             "isaretler": [
                 (3, 900, 1165, "Add receipt: Pick a file / Take a picture."),
                 (4, 800, 1915, "Attachments bölümü."),
             ]},
        ],
        "notlar": [
            ("Hesap Defterim'de ek tam ekran açılıp silinebiliyor.", "E0159"),
            "Fiş okumanın bu belgedeki tek yeri bu sorudur.",
        ],
        "urunler": [
            ("Paraşüt", "beyan", "Fiş fotoğrafından okuma anlatılıyor; çalışırken görülmedi.", "E0011"),
            ("Logo İşbaşı", "beyan", "Fiş fotoğrafından okuma anlatılıyor; çalışırken görülmedi.", "E0009"),
            ("KolayBi", "beyan", "Ürün özellikleri arasında fiş okuma (Fiş OCR) sayılıyor; incelenen web gider "
             "formunda fiş okumaya dair iz yok.", "E0008"),
            ("KolayBi", "kaynak", "Gider formunda dosya alanı: kabul edilen türler ve 5 MB sınırı (→ 4.9).",
             "E0192"),
            ("Money Manager", "canli", "Formun Detay satırında kamera simgesi; denenmedi.", "E0229"),
            ("Bluecoins", "canli", "Formun üstünde ataç düğmesi; denenmedi.", "E0035"),
            ("Goodbudget", "canli", "İşlem formunda dosya eki girişi görülmedi; form menüsünde yalnız Help.",
             "E0128"),
        ],
    },

    # ---------------------------------------------------------------- 10.4
    {
        "tur": "soru", "no": "10.4", "baslik": "Yedek ve veri konumu",
        "giris": "Hesap Defterim yedeklemeyi bir diyalogla hatırlatıyor ve kayıtları sunucusunda saklamadığını "
                 "söylüyor. Money Manager ve Bluecoins'te yedek, ayarlardan açılan girişler.",
        "sekiller": [
            {"k": "hd-yedek", "e": "E0149", "etiket": "Hesap Defterim · Yedekleme kapalı",
             "isaretler": [
                 (1, 700, 850, "Kayıtlar sunucuda saklanmıyor; Drive'a kendi kopyası öneriliyor."),
             ]},
            {"k": "mm-yedek", "e": "E0254", "etiket": "Money Manager · Ayarlar",
             "isaretler": [
                 (2, 980, 1260, "Yedekle; yanında PC'den Yönet."),
             ]},
            {"k": "bc-bulut", "e": "E0090", "etiket": "Bluecoins · Ayarlar",
             "isaretler": [
                 (3, 700, 1695, "Veri Yönetimi ve Bulut Ayarları."),
             ]},
        ],
        "notlar": [
            ("Hesap Defterim'de Drive yedek ve geri yükleme yolu gerçek bir Google hesabıyla denenmedi.", "E0007"),
            ("Money Manager ve Bluecoins'in araştırılan kurulumları yerel ve girişsiz kullanıldı. Bluecoins'in "
             "Veri Yönetimi ekranında yedeğin yeri Telefon hafızası.", "E0010, E0005, E0445"),
            ("Hesap Defterim'in çıktı uyarısı dosyaların kasadefteri adlı bir klasöre kaydedildiğini söylüyor; "
             "dosya sisteminde bu adla bir klasör bulunmadı. PDF'ler Documents altındaki Hesap Defterim "
             "klasörüne, Excel uygulamanın kendi dış dizinine yazıldı (koşum kaydı, karesiz).",
             "E0447; E0007"),
            "Yedek menüsünü görmek geri yükleme denemesi değildir.",
        ],
        "urunler": [
            ("Wallet", "kosum", "Bulut hesabıyla kullanıldı; kayıtlar sonraki oturumda korundu. Çok cihaz ve geri "
             "yükleme denenmedi.", "E0014"),
            ("Goodbudget", "canli", "Zarflar ekranının üstünde son yedeğin zamanı yazıyor (Last Backup); yedeğin "
             "kendisi ve geri yükleme incelenmedi.", "E0115"),
            ("KolayBi, Paraşüt, Logo İşbaşı", "beyan", "Bulut ve web ifadeleri ürün beyanı; veri konumu ve yedek "
             "bütünlüğü ölçülmedi.", "E0008, E0011, E0009"),
        ],
    },

    # ---------------------------------------------------------------- 10.5
    {
        "tur": "soru", "no": "10.5", "baslik": "Üçüncü kişiye açılan girişler",
        "giris": "Canlı incelenen ürünlerde başka bir kişiye erişim veren girişler görüldü ama hiçbiri kullanılmadı. "
                 "Muhasebeci erişimi yalnız ürün anlatımlarında geçiyor.",
        "yukseklik": 404, "not_genislik": 420,
        "sekiller": [
            {"k": "wl-grup", "e": "E0376", "etiket": "Wallet · çekmecenin alt kısmı",
             "isaretler": [
                 (1, 650, 1125, "Group sharing."),
             ]},
        ],
        "notlar": [
            ("Bank Sync hesap eklerken de bir seçenek (→ 10.2). Çekmecenin üst kısmındaki Bank Sync girişinin "
             "karesi hesap sahibinin adını taşıdığı için basılmadı.", "E0471, E0375"),
            "Grup paylaşımı ve banka bağlantısı denenmedi.",
            ("KolayBi'nin 2026'da yayımlanan bir videosunun karesinde panoda Müşavirini Davet Et bağlantısı var; bağlantının açtığı akış görülmedi.", "E0187"),
            "Muhasebecinin ürünlerde nerede durduğu → Belge 2 9.7.",
        ],
        "urunler": [
            ("Goodbudget", "kosum", "Hesap bir household olarak açılıyor; ortak düzenleme ve yetki denenmedi. "
             "Kaynakta ücretli pakette banka senkronizasyonu anlatılıyor.", "E0006"),
            ("KolayBi", "beyan", "Banka, e-belge, pazaryeri, sanal POS, geliştirici API ve çok müşterili "
             "muhasebeci erişimi.", "E0008"),
            ("Paraşüt", "beyan", "Banka, e-ticaret, online tahsilat, e-belge; muhasebecinin hesabı canlı "
             "görüntülemesi.", "E0011"),
            ("Logo İşbaşı", "beyan", "Müşterinin eklediği mali müşavir müşteri adına işlem yapabiliyor.", "E0009"),
            ("QuickBooks Solopreneur", "beyan", "Banka ve kart hesabından gelen kayıtlar kategori ve Type ile "
             "inceleniyor; kural tanımlanabiliyor.", "E0012"),
        ],
        "dayanak": ["Kaynak beyanı: E0008, E0011, E0009, E0012"],
    },

    # ---------------------------------------------------------------- ozet
    {
        "tur": "tablo", "no": "", "baslik": "Özet: hangi yol nerede görüldü?",
        "giris": "Yeni kanıt yok; önceki sayfaların satır satır dökümü. Dosya eki, içe aktarma, çıktı, yedek ve "
                 "üçüncü kişi erişimi ayrı yollardır; birinin görülmesi ötekinin çalıştığını göstermez.",
        "sutunlar": [("Ürün", 1.3), ("Çıktı", 1.7), ("İçe aktarma", 1.5), ("Ek dosya / okuma", 1.6),
                     ("Yedek / konum", 1.6), ("Üçüncü kişi", 1.6)],
        "boy": 7.2,
        "satirlar": [
            "Canlı incelenen ürünler",
            ["Money Manager",
             {"t": "Excel(.xlsx) e-posta düğmesi (10.1).", "tur": "canli", "d": "E0250"},
             {"t": "Giriş bulunmadı (10.2).", "tur": None, "d": "E0010"},
             {"t": "Kamera simgesi (10.3).", "tur": "canli", "d": "E0229"},
             {"t": "Yedekle girişi; yerel kurulum (10.4).", "tur": "canli", "d": "E0254"},
             {"t": "—", "tur": None}],
            ["Bluecoins",
             {"t": "PDF/Yazıcı, Excel (.csv), HTML (10.1).", "tur": "canli", "d": "E0105"},
             {"t": "Excel (.csv), QIF (10.2).", "tur": "canli", "d": "E0091"},
             {"t": "Ataç düğmesi (10.3).", "tur": "canli", "d": "E0035"},
             {"t": "Bulut Ayarları; yedek telefon hafızasında (10.4).", "tur": "canli", "d": "E0090, E0445"},
             {"t": "—", "tur": None}],
            ["Wallet",
             {"t": "Çekmece › Others › Exports: PDF, XLS, CSV (10.1).", "tur": "canli", "d": "E0450, E0428"},
             {"t": "Hesap türü File Import; Imports; e-postayla (10.2).", "tur": "canli",
              "d": "E0471, E0450, E0437"},
             {"t": "Dosya veya fotoğraf (10.3).", "tur": "canli", "d": "E0313"},
             {"t": "Bulut hesabı (10.4).", "tur": "kosum", "d": "E0014"},
             {"t": "Group sharing, Bank Sync (10.5).", "tur": "canli", "d": "E0376"}],
            ["Hesap Defterim",
             {"t": "Dönemli PDF/Excel; PDF'in içi (10.1).", "tur": "canli", "d": "E0169, E0427"},
             {"t": "Giriş bulunmadı (10.2).", "tur": None, "d": "E0007"},
             {"t": "Kamera, galeri, PDF; ataç (10.3).", "tur": "canli", "d": "E0157, E0158"},
             {"t": "Sunucuda saklamama beyanı (10.4).", "tur": "canli", "d": "E0149"},
             {"t": "—", "tur": None}],
            ["Goodbudget",
             {"t": "İncelenmedi (10.1).", "tur": None, "d": "E0006"},
             {"t": "Giriş bulunmadı (10.2).", "tur": None, "d": "E0006"},
             {"t": "Form menüsünde ek yok (10.3).", "tur": "canli", "d": "E0128"},
             {"t": "Last Backup göstergesi (10.4).", "tur": "canli", "d": "E0115"},
             {"t": "Household (10.5).", "tur": "kosum", "d": "E0006"}],
            "Kaynakla incelenen ürünler",
            ["KolayBi",
             {"t": "Cari ekstre PDF (10.1).", "tur": "kaynak", "d": "E0196, E0197"},
             {"t": "Listelerde İçe Aktar (10.2).", "tur": "kaynak", "d": "E0198"},
             {"t": "Dosya türleri, 5 MB (10.3).", "tur": "kaynak", "d": "E0192"},
             {"t": "Bulut beyanı (10.4).", "tur": "beyan", "d": "E0008"},
             {"t": "Muhasebeci, banka, e-belge (10.5).", "tur": "beyan", "d": "E0008"}],
            ["Paraşüt",
             {"t": "KDV dökümü → Excel (10.1).", "tur": "beyan", "d": "E0011"},
             {"t": "—", "tur": None},
             {"t": "Fiş okuma (10.3).", "tur": "beyan", "d": "E0011"},
             {"t": "Bulut beyanı (10.4).", "tur": "beyan", "d": "E0011"},
             {"t": "Muhasebeci, banka, e-belge (10.5).", "tur": "beyan", "d": "E0011"}],
            ["Logo İşbaşı",
             {"t": "—", "tur": None},
             {"t": "—", "tur": None},
             {"t": "Fiş okuma (10.3).", "tur": "beyan", "d": "E0009"},
             {"t": "Bulut beyanı (10.4).", "tur": "beyan", "d": "E0009"},
             {"t": "Müşavir işlem yetkisi (10.5).", "tur": "beyan", "d": "E0009"}],
            ["QuickBooks Solopreneur",
             {"t": "—", "tur": None},
             {"t": "—", "tur": None},
             {"t": "—", "tur": None},
             {"t": "—", "tur": None},
             {"t": "Banka ve kart akışı (10.5).", "tur": "beyan", "d": "E0012"}],
        ],
        "notlar": [
            "Boş hücre (—): o soru bu ürün için incelenmedi ya da kaynakta anlatılmıyor.",
        ],
    },
]

EKSIKLER = [
    ("Hesap Defterim", "Üretilen Excel dosyasının içeriği", "10.1", "Dosya yöneticisinden açmak", "Orta"),
    ("Wallet", "İçe aktarma akışının kendisi (dosya gönderimi, eşleme, sonuç)", "10.2",
     "Kişisel e-posta ve gerçek dosya gerekir", "Önerilmez"),
    ("Bluecoins", "CSV/HTML çıktısının üretimi", "10.1", "Seçeneği denemek", "Orta"),
    ("Bluecoins", "CSV/QIF içe aktarma", "10.2", "Yeni kayıt yaratır; veri değişir", "Önerilmez"),
    ("Hesap Defterim", "Drive yedek ve geri yükleme", "10.4", "Gerçek Google hesabı gerekir", "Önerilmez"),
    ("Wallet", "Group sharing ve Bank Sync", "10.5", "Üçüncü kişi ve banka hesabı gerekir", "Önerilmez"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0115", "E0128", "E0175", "E0187", "E0399", "E0428", "E0437", "E0445", "E0447", "E0450", "E0493"]
