# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 3 · Gorsel dil — tek icerik kaynagi (plan §4).

Bu bolumun kendi tema kaydi yok; mevcut kareler bes sabit soruyla yeniden tarandi.
Yeni emulator kosumu yapilmadi.
"""

NO = 3
BASLIK = "Görsel dil"
ANA_SORU = "Ürün sayıyı, rengi ve durumu nasıl gösteriyor?"
GIRIS = [
    "Bu bölüm önceki bölümlerin karelerini beş sabit soruyla yeniden okur: tutarın yazımı, rengin taşıdığı "
    "anlam, liste-kart-tablo-grafik seçimi, durum ve dönemin görünümü, boş, hata ve yükleme durumları.",
    "Çoğu sayfa aynı bölgenin ürünlerdeki kırpıntısını alt alta dizer. Kırpıntının geldiği tam ekran ilgili "
    "bölümdedir.",
]
GIRMEZ = [
    "Gezinme yapısı → 2",
    "Form alanlarının sırası → 4",
    "Raporun anlamı → 9",
    "Erişilebilirlik hükmü (ölçülmedi; hiçbir bölümde yazılmaz)",
]
KAPSAM = [
    ("Money Manager", ["canli"], None),
    ("Bluecoins", ["canli"], None),
    ("Wallet", ["canli"], "İki ayrı tutar yazımı; nedeni bilinmiyor."),
    ("Hesap Defterim", ["canli"], None),
    ("Goodbudget", ["canli"], None),
    ("KolayBi", ["kaynak"], None),
    ("Paraşüt, Logo İşbaşı, QuickBooks Solopreneur", ["yok"], "Görsel dil kanıtı basılmadı."),
]

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 3.1
    {
        "tur": "serit", "no": "3.1", "baslik": "Tutar nasıl yazılıyor?",
        "giris": "Para simgesi, ondalık ve binlik ayraç ürünler arasında farklı. Wallet aynı sürümün farklı "
                 "günlerde alınan karelerinde iki ayrı yazım gösteriyor.",
        "kirpinti_genislik": 330,
        "kirpintilar": [
            {"e": "E0235", "ad": "Money Manager", "kutu": (0, 450, 1080, 540), "konum": "Hesaplar listesi",
             "aciklama": "₺ önde, iki ondalık, Türkçe ayraçlar; tutar sağa hizalı."},
            {"e": "E0027", "ad": "Bluecoins", "kutu": (0, 560, 1080, 700), "konum": "Hesaplar listesi",
             "aciklama": "₺ önde, iki ondalık; grup tutarı renkli ve kalın."},
            {"e": "E0276", "ad": "Wallet · yazım 1", "kutu": (0, 550, 1080, 695), "konum": "Home, hesap kartları",
             "aciklama": "TRY 20,800.00: para kodu önde, İngilizce ayraçlar."},
            {"e": "E0283", "ad": "Wallet · yazım 2", "kutu": (0, 550, 1080, 695),
             "konum": "Home, hesap kartları",
             "aciklama": "₺20.800,00: simge önde, Türkçe ayraçlar."},
            {"e": "E0137", "ad": "Hesap Defterim", "kutu": (0, 2040, 1080, 2170), "konum": "Ekranın altı",
             "aciklama": "Tam sayı, binlik ayraç, simge yok."},
            {"e": "E0115", "ad": "Goodbudget", "kutu": (0, 570, 1080, 720), "konum": "Zarf satırı",
             "aciklama": "850.00: simge yok, İngilizce ayraçlar."},
            {"e": "E0116", "ad": "Goodbudget · tarih", "kutu": (0, 1180, 1080, 1260), "konum": "Kayıt formu",
             "aciklama": "09/11/2026: tarih ay/gün/yıl sırasıyla."},
        ],
        "notlar": [
            ("Wallet'taki iki yazımın nedeni (cihaz dili, hesap ayarı ya da sürüm içi değişiklik) doğrulanmadı. "
             "Ayarlardaki tek sayı biçimi seçeneği yalnız ondalığı açıp kapatıyor; kapalıyken simge ve ayraç "
             "aynı kalıyor.", "E0014; E0482, E0485"),
            "Money Manager kart borcunu Hesaplar'da işaretsiz ve kırmızı, kartın defterinde eksiyle yazıyor (→ 5.4).",
            ("Goodbudget'ın rapor başlığında ise Türkçe ay adları var; aynı üründe iki yerelleştirme. Tarih "
             "sırası, ondalık basamağı ve ondalık işareti ayarlardan değişiyor (Date Order, Decimal Digits, "
             "Decimal Mark; yalnız o cihazda).", "E0122; E0453"),
            ("Hesap Defterim'in ayarlarında bir \"Para birimi biçimi\" seçeneği var; seçenekleri açılmadı.",
             "E0150"),
        ],
        "urunler_baslik": "Kaynaktan",
        "urunler": [
            ("KolayBi", "kaynak", "₺19.543,53: simge önde, Türkçe ayraçlar.", "E0206"),
            ("Paraşüt, Logo İşbaşı, QuickBooks Solopreneur", "yok", "Tutar yazımı görülmedi; görülen kareler "
             "iç arayüzü temsil etmiyor.", "E0011, E0009, E0012"),
        ],
    },

    # ---------------------------------------------------------------- 3.2
    {
        "tur": "serit", "no": "3.2", "baslik": "Renk hangi anlamı taşıyor, yanında sözcük var mı?",
        "giris": "Gelir ve gider renkle ayrılıyor; renklerin kendisi ürüne göre değişiyor. Hesap "
                 "Defterim yönü rengin yanında sözcükle de yazıyor.",
        "kirpinti_genislik": 300,
        "kirpintilar": [
            {"e": "E0228", "ad": "Money Manager", "kutu": (0, 362, 1080, 470), "konum": "Ana ekran özet satırı",
             "aciklama": "Gelir mavi, gider turuncu-kırmızı, toplam siyah; etiketler üstte."},
            {"e": "E0230", "ad": "Money Manager · havale", "kutu": (0, 580, 1080, 700),
             "konum": "İşlem listesi",
             "aciklama": "Havale satırının tutarı siyah."},
            {"e": "E0030", "ad": "Bluecoins", "kutu": (0, 1400, 1080, 1740), "konum": "İşlem listesi",
             "aciklama": "Gider kırmızı, gelir yeşil; satır simgesi de renkli."},
            {"e": "E0030", "ad": "Bluecoins · transfer", "kutu": (0, 860, 1080, 1100), "konum": "İşlem listesi",
             "aciklama": "Transfer satırında mavi simge; bacakların tutarı gelir ve gider renginde."},
            {"e": "E0137", "ad": "Hesap Defterim", "kutu": (0, 1900, 1080, 2170), "konum": "Ekranın altı",
             "aciklama": "Alındı yeşil, Ödendi kırmızı; yön düğmede ve toplam etiketinde sözcükle de yazıyor."},
            {"e": "E0494", "ad": "Goodbudget", "kutu": (0, 912, 1080, 1168), "konum": "İşlem listesi",
             "aciklama": "Gelir yeşil ve artı işaretli; gider koyu ve işaretsiz. Gidere kırmızı kullanılmıyor."},
        ],
        "notlar": [
            "Wallet'ta formdaki seçili tür yalnız zemin tonuyla ayrılıyor (→ 4.3).",
            "Wallet ve Bluecoins'te transferin iki bacağı gelir ve gider renginde; Money Manager havaleyi nötr "
            "renkle tek satırda gösteriyor (→ 5.6).",
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Proje belge kartlarında iade başlıkları ters renkte ve −/+ işaretli.", "E0191"),
        ],
    },

    # ---------------------------------------------------------------- 3.3
    {
        "tur": "yanyana", "no": "3.3", "baslik": "Liste, kart, tablo ve grafik nerede kullanılıyor?",
        "giris": "Aynı türden bilgi ürünlerde farklı biçimlerde sunuluyor: sütun başlıklı tablo, takvim ızgarası, "
                 "kart panosu, soru başlıklı grafik kartı, pasta ve ilerleme çubuğu.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "hd-tablo", "e": "E0142", "ad": "Hesap Defterim", "etiket": "birleşik liste",
             "satirlar": ["Sütun başlıklı tablo.", "Altta sabit toplam bandı."]},
            {"k": "hd-takvim", "e": "E0144", "ad": "Hesap Defterim", "etiket": "Takvim",
             "satirlar": ["Takvim ızgarası; günün tutarı yön rengiyle."]},
            {"k": "bc-pano", "e": "E0103", "ad": "Bluecoins", "etiket": "Hesaplar sekmesi",
             "satirlar": ["Kart panosu; her kartın kendi başlığı ve menüsü."]},
            {"k": "wl-grafik", "e": "E0280", "ad": "Wallet", "etiket": "Statistics › Cash-flow",
             "satirlar": ["Kart başlığı bir soru.", "Gelir ve gider yatay çubukla; altında eğilim grafiği."]},
            {"k": "mm-pasta", "e": "E0231", "ad": "Money Manager", "etiket": "İstatistik",
             "satirlar": ["Pasta grafik; altında yüzdeli liste."]},
            {"k": "gb-cubuk", "e": "E0115", "ad": "Goodbudget", "etiket": "ENVELOPES",
             "satirlar": ["Zarf satırında ilerleme çubuğu."]},
        ],
        "notlar": [
            "Hesap Defterim'in incelenen yüzeylerinde grafik görülmedi; özet tablo ve takvimle veriliyor.",
            ("Bluecoins'te panodaki kartlar Ana Ekran ayarından tek tek açılıp kapanıyor.", "E0025"),
        ],
        "sag_notlar": [
            "Wallet'ın rapor kartlarında başlık bir soru olarak yazılmış; diğer ürünlerin incelenen raporlarında "
            "başlık ölçünün adı.",
        ],
    },

    # ---------------------------------------------------------------- 3.4
    {
        "tur": "serit", "no": "3.4", "baslik": "Durum ve dönem nasıl görünüyor?",
        "giris": "Bluecoins ve Wallet bekleyen kalemin durumunu sözcükle yazıyor. Seçili dönem ürünlerde çip, "
                 "sekme, ay gezgini ya da başlıkta yazılı aralık olarak görünüyor. Seçili filtrenin görünümü "
                 "9.4'te.",
        "kirpinti_genislik": 330,
        "kirpintilar": [
            {"e": "E0056", "ad": "Bluecoins · durum", "kutu": (0, 480, 1080, 860), "konum": "Hatırlatıcılar",
             "aciklama": "Dün bitti ve Bugün süresi doluyor: durum sözcükle ve renkle."},
            {"e": "E0056", "ad": "Bluecoins · sayaç", "kutu": (0, 1080, 1080, 1220), "konum": "Hatırlatıcılar",
             "aciklama": "Taksitli kayıtta kaçıncı taksit olduğu satırda (→ 7.3)."},
            {"e": "E0142", "ad": "Hesap Defterim", "kutu": (0, 280, 1080, 420), "konum": "Ekranın üstü",
             "aciklama": "Dönem çipleri; seçili olan çerçeveli, altında tarih aralığı ve oklar."},
            {"e": "E0228", "ad": "Money Manager", "kutu": (0, 130, 1080, 330), "konum": "Ekranın üstü",
             "aciklama": "Ay gezgini; görünüm sekmelerinde seçili olan alt çizgiyle."},
            {"e": "E0280", "ad": "Wallet", "kutu": (0, 2125, 1080, 2215), "konum": "Rapor ekranının altı",
             "aciklama": "Aralık çipleri; ücretli aralıklar kilitli."},
            {"e": "E0122", "ad": "Goodbudget", "kutu": (0, 120, 1080, 360), "konum": "Rapor ekranının üstü",
             "aciklama": "Dönem başlıkta yazılı; değiştirme simgesi üst çubukta."},
        ],
        "notlar": [
            "Bluecoins'teki durum sözcükleri rengin yanında anlamı da taşıyor.",
            ("Wallet'ın plan ayrıntısında da durum sözcükle yazıyor: Due today, Due in 30 days, Paid Today "
             "(→ 7.5).", "E0289, E0292"),
            ("Bluecoins'in kayıt formunda bundan ayrı bir Durum alanı var: Yok, Kontrol, Mutabık, İptal edildi "
             "(→ 4.2).", "E0448"),
        ],
    },

    # ---------------------------------------------------------------- 3.5
    {
        "tur": "soru", "no": "3.5", "baslik": "Boş, hata ve yükleme durumları",
        "giris": "2.6 ve 4.7'nin kareleri burada biçim açısından okunur. Boş durum ya ne yapılacağını söylüyor ya "
                 "da yalnız veri olmadığını; yükleme için tek kanıt Wallet'ın iskelet blokları.",
        "yukseklik": 340, "not_genislik": 250,
        "sekiller": [
            {"k": "wl-iskelet", "e": "E0274", "etiket": "Wallet · ana ekranın aşağısı, yüklenirken",
             "isaretler": [
                 (1, 800, 1000, "Kartın satırları yerine gri iskelet bloklar."),
             ]},
            {"k": "wl-bos", "e": "E0282", "etiket": "Wallet · Planned payments, boş",
             "isaretler": [
                 (2, 900, 1470, "Ne işe yaradığını söylüyor."),
                 (3, 960, 1720, "İlk kaydın yolunu gösteriyor."),
             ]},
            {"k": "gb-bos", "e": "E0122", "etiket": "Goodbudget · boş rapor",
             "isaretler": [
                 (4, 900, 1405, "Toplam 0.00."),
                 (5, 850, 1895, "Yalnız veri olmadığını söylüyor."),
             ]},
        ],
        "kirpinti_k": "hata-bicimleri",
        "kirpinti_baslik": "Hata mesajlarının biçimi (yerleri 4.7'de)",
        "kirpinti_etiket": "Üçü ekranın altında kısa mesaj kutusu; sonuncusu alanın altında kırmızı metin.",
        "kirpinti_h": 26,
        "kirpintilar": [
            ("E0232", "Money Manager", (220, 2170, 860, 2270)),
            ("E0281", "Wallet", (180, 2150, 1040, 2280)),
            ("E0117", "Goodbudget", (280, 2170, 800, 2270)),
            ("E0347", "Wallet · planlı ödeme", (0, 380, 1080, 560)),
        ],
        "notlar": [
            "Ne yapılacağını söyleyen boş durum: Wallet planlı ödemeler, Bluecoins temiz kurulum (→ 2.6).",
            ("Yalnız veri olmadığını söyleyen: Money Manager \"Veri yok.\", Goodbudget raporu, Bluecoins'in boş "
             "Çöp Kutusu.", "E0227, E0122, E0089"),
            "Mesaj kutularının üçünde simge var; Goodbudget'ınki uygulama simgesiyle.",
            ("Wallet'ın kartı yüklendiğinde iskeletin yerine sıradaki plan satırı geliyor (→ 7.4).", "E0469"),
        ],
    },
]

EKSIKLER = [
    ("Wallet", "İki tutar yazımının (para kodu ve ayraç) nedeni; sayı biçimi ayarı yalnız ondalığı değiştiriyor",
     "3.1", "Cihaz dili ve hesabın para birimi ayarı değiştirilip ana ekran okunur", "Düşük"),
    ("Hesap Defterim", "Para birimi biçimi seçenekleri", "3.1", "Ayarlarda seçeneği açmak", "Düşük"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0150", "E0289", "E0292", "E0448", "E0453", "E0469", "E0482", "E0485"]
