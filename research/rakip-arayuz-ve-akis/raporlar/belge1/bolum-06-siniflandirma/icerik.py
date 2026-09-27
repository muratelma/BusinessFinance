# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 6 · Siniflandirma yuzeyleri — tek icerik kaynagi (plan §4)."""

NO = 6
BASLIK = "Sınıflandırma yüzeyleri"
ANA_SORU = "Kayıt hangi eksenlerde sınıflandırılıyor ve bu arayüzde nasıl görünüyor?"
GIRIS = [
    "Bu bölüm kategoriyi, etiketi, proje eksenini, karşı tarafı ve işletme/şahsi ayrımını arayüzdeki "
    "görünümüyle izler: formda nasıl seçildikleri ve hangi ekranlarda göründükleri.",
    "Bir eksenin üründe bulunması, onun hangi amaçla kullanıldığını göstermez. Demo verisindeki adlardan "
    "kullanım amacı çıkarılmadı.",
]
GIRMEZ = [
    "Kapsamın BusinessFinance için anlamı → Belge 3",
    "Rapor kırılımı → 9",
    "Karşı taraf hesabının borç ve tahsilat akışı → 8",
    "Demo proje ve cari adlarından kullanım amacı",
]
KAPSAM = [
    ("Money Manager", ["canli"], None),
    ("Bluecoins", ["canli"], None),
    ("Wallet", ["canli"], None),
    ("Hesap Defterim", ["canli"], "Kategori seçicisi yok."),
    ("Goodbudget", ["canli"], "Kategori yerine zarf."),
    ("KolayBi", ["kaynak"], "Proje ve cari ekseni."),
    ("Paraşüt, Logo İşbaşı", ["yok"], "Kaynakta anlatılmıyor."),
    ("QuickBooks Solopreneur", ["beyan"], "İşlem başına Business / Personal."),
]
# Destek gorsellerindeki demo cari adlari icerige gerekmedigi icin karartildi.
KARARTMA = {
    "E0194": [(455, 475, 700, 850)],
    "E0201": [(465, 420, 600, 640)],
}
KAYNAK_NOTLARI = [
    "E0194 ve E0201'de destek materyalinin demo cari adları (Unvan sütunu) bu bölümün kopyalarında karartıldı; "
    "özgün kanıt değişmedi.",
]

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 6.1
    {
        "tur": "soru", "no": "6.1", "baslik": "Kategori formda nasıl seçiliyor?",
        "giris": "Üç üründe kategori ayrı bir seçicide: simgeli ızgara, üst başlıklara gruplu liste ya da sık "
                 "kullanılanların öne alındığı liste.",
        "yukseklik": 380,
        "sekiller": [
            {"k": "mm-kategori", "e": "E0229", "etiket": "Money Manager · formun altındaki kategori paneli",
             "isaretler": [
                 (1, 560, 1340, "Panel formun altında açılıyor; başlıkta düzenleme."),
                 (2, 920, 1950, "Simge ve addan oluşan 11 kutu."),
             ]},
            {"k": "bc-kategori", "e": "E0071", "etiket": "Bluecoins · kategori seçici",
             "isaretler": [
                 (3, 700, 470, "Arama ve Yeni aynı satırda."),
                 (4, 800, 730, "Kategoriler üst başlık altında gruplu."),
                 (5, 700, 1150, "Alt kategoriler simgeyle."),
             ]},
            {"k": "wl-kategori", "e": "E0349", "etiket": "Wallet · Category",
             "isaretler": [
                 (6, 700, 355, "Sık kullanılanlar üstte."),
                 (7, 800, 857, "Altında bütün ana kategoriler."),
                 (8, 900, 1170, "Ana kategori alt kategorilere açılıyor."),
             ]},
        ],
        "notlar": [
            ("Wallet'ta gider formunda gelir türünde bir kategori (Sale) seçili durabiliyor.", "E0277"),
            ("Bluecoins'in Türkçe arayüzünde grup adları Türkçe (Araba, Eve Ait, Eğlence), alt kategori adları "
             "İngilizce (Fuel, Grocery, Others).", "E0071, E0087"),
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Genel Gider Yönetimi'nde kategori kartları, altlarında tipler; Kategoriyi "
             "Düzenle, Yeni Tip Ekle ve Yeni Kategori Ekle düğmeleri. Kategoriler demo verisidir.", "E0193"),
        ],
    },
    {
        "tur": "soru", "no": "6.1", "baslik": "Kategori seçicisi olmayan iki ürün", "haritada": False,
        "giris": "Hesap Defterim'de kategori listesi yok: ayarlardan açılan serbest bir metin kutusu ve düğme "
                 "adlarını değiştiren bir diyalog var. Goodbudget kaydı kategori yerine zarfa bağlıyor.",
        "yukseklik": 380,
        "sekiller": [
            {"k": "hd-metin", "e": "E0151", "etiket": "Hesap Defterim · Alındı formu",
             "isaretler": [
                 (1, 800, 1290, "Açıklama / Kategori: serbest metin, seçici yok."),
             ]},
            {"k": "hd-islemadlari", "e": "E0139", "etiket": "Hesap Defterim · İşlem adları",
             "isaretler": [
                 (2, 700, 440, "Düğme adı seçenekleri: Ödendi Alındı, Gelir Gider, Özel."),
             ]},
            {"k": "gb-zarf", "e": "E0116", "etiket": "Goodbudget · Add Transaction",
             "isaretler": [
                 (3, 700, 790, "Kategori yerine zarf."),
             ]},
        ],
        "notlar": [
            "Hesap Defterim'in İşlem adları yalnız düğme ve sütun adlarını değiştiriyor (→ 4.5); kayda bir "
            "sınıf eklemiyor.",
            ("Goodbudget'ta zarf hem bütçe kovası hem sınıf işi görüyor (→ 7.6).", "E0115"),
        ],
    },

    # ---------------------------------------------------------------- 6.2
    {
        "tur": "soru", "no": "6.2", "baslik": "Etiket yüzeyi",
        "giris": "Bluecoins'te etiketlerin kendi ekranı var ve etiket kayıt satırında çip olarak görünüyor; "
                 "Wallet'ta etiket raporda kategoriyle aynı düzeyde bir kırılım.",
        "yukseklik": 404,
        "sekiller": [
            {"k": "bc-etiket", "e": "E0088", "etiket": "Bluecoins · Etiketler",
             "isaretler": [
                 (1, 800, 370, "Etiket araması."),
                 (2, 700, 850, "Listede İş ve Kişisel adlı değerler."),
             ]},
            {"k": "bc-etiketli", "e": "E0491", "etiket": "Bluecoins · İşlemler, etiketli kayıt",
             "isaretler": [
                 (3, 330, 694, "Kayda verilen İş etiketi satırın altında çip."),
             ]},
            {"k": "wl-etiket", "e": "E0278", "etiket": "Wallet · Statistics › Spending",
             "isaretler": [
                 (4, 700, 800, "Rapor kategoriye ya da etikete göre bölünüyor."),
             ]},
        ],
        "notlar": [
            ("Bluecoins'te etiket formda çoklu seçimle veriliyor; seçicide beş değer var (Doğum günü, Film, İş, "
             "Kişisel, Tatil). Filtre panelinde de ayrı bir boyut (→ 9.4). İş ve Kişisel değerlerinin kapsam "
             "boyutu olarak kullanıldığı iddia edilmez.", "E0426, E0491, E0082"),
            ("Wallet'ta etiketi kullanıcı oluşturuyor: formda ad, renk ve Auto assign to new records anahtarı. "
             "İki kayda Isletme ve Sahsi etiketi verildi; Labels görünümü yalnız etiketli kayıtları gösteriyor "
             "(toplamın anlamı → Belge 2 6.2).", "E0497, E0425"),
            ("Wallet'ta etiket, kayıttan sonra açılan ayrıntı ekranında seçiliyor (→ 4.2).", "E0294"),
        ],
        "urunler": [
            ("Money Manager, Hesap Defterim, Goodbudget", "yok", "Etiket yüzeyi görülmedi.", "E0010, E0007, E0006"),
            ("KolayBi", "kaynak", "Formlarda Etiketler alanı (→ 4.9).", "E0192"),
        ],
    },

    # ---------------------------------------------------------------- 6.3
    {
        "tur": "soru", "no": "6.3", "baslik": "Proje ekseni",
        "giris": "KolayBi kayıtları kullanıcının tanımladığı projelere bağlıyor. Proje listesi her projenin "
                 "gelirini, giderini ve netini gösteriyor; proje sayfasında toplamlar sekmelere ayrılıyor.",
        "dikey": True,
        "sekiller": [
            {"k": "kb-projeler", "e": "E0188", "etiket": "KolayBi · Projeler",
             "kirpma": (96, 112, 1305, 865), "genislik": 360,
             "isaretler": [
                 (1, 554, 187, "Proje oluşturma, içe ve dışa aktarma."),
                 (2, 684, 218, "Durum: Aktif / Pasif."),
                 (3, 964, 188, "Listede Gelir, Gider ve Net sütunları."),
             ]},
            {"k": "kb-proje", "e": "E0190", "etiket": "KolayBi · proje sayfası",
             "kirpma": (96, 112, 1305, 620), "genislik": 360,
             "isaretler": [
                 (4, 864, 228, "Durum Değiştir: Aktif / Pasif; yanında silme simgesi."),
                 (5, 904, 298, "Toplam gelir, gider, tahsilat ve ödeme sekmeleri."),
                 (6, 424, 351, "\"Kar / Zarar\" ve \"Nakit Durumu\" ayrı alt sekmeler."),
             ]},
        ],
        "notlar": [
            "Formlarda Proje alanı ve \"Ayarlar sayfasından Proje Takip seçeneğini kapatabilirsiniz\" notu var "
            "(→ 4.9).",
            "Proje kodu, adı, para birimi, başlangıç ve bitiş tarihleri listede ayrı sütunlar.",
            "Listedeki proje adları destek materyalinin örnek verisidir; kullanım amacı bu adlardan çıkarılmaz.",
            "Liste ile proje sayfası aynı projeye ait değil; iki ekranın toplamları karşılaştırılmadı.",
            "Proje ekseninin işletme/şahsi ayrımıyla ilişkisi → Belge 2 6.3.",
        ],
    },

    # ---------------------------------------------------------------- 6.4
    {
        "tur": "soru", "no": "6.4", "baslik": "Karşı taraf bir eksen olarak",
        "giris": "KolayBi'de cariler tipine göre ayrılıyor: müşteri, tedarikçi, ikisi birden, yurt dışı. "
                 "Personel ve ortaklar ayrı sekmelerde.",
        "dikey": True,
        "sekiller": [
            {"k": "kb-cariler", "e": "E0194", "etiket": "KolayBi · Genel Cari Hesapları (demo adlar karartıldı)",
             "kirpma": (96, 112, 1305, 865), "genislik": 360,
             "isaretler": [
                 (1, 784, 63, "Sekmeler: Genel, Potansiyel, Personel, Ortaklar, Tekrarlı Maaşlar."),
                 (2, 664, 308, "Cari Tipi: Müşteri, Tedarikçi, ikisi birden, Yurt Dışı."),
                 (3, 1119, 328, "Yerel bakiye işaretli ve renkli."),
             ]},
            {"k": "kb-personel", "e": "E0201", "etiket": "KolayBi · Personel Cari Hesapları (demo adlar karartıldı)",
             "kirpma": (96, 112, 1305, 680), "genislik": 360,
             "isaretler": [
                 (4, 604, 98, "Personel Carileri sekmesi."),
                 (5, 584, 268, "Çalışma tipi cari tipinde."),
                 (6, 1054, 268, "Personelin de yerel bakiyesi var."),
             ]},
        ],
        "notlar": [
            ("Ortaklar sekmesinin kendi görseli destek sayfasında yayımlanmamış. Destek metnine göre ortak, cari "
             "oluşturmaya benzer biçimde tanımlanıyor ve ortağa maaş, prim gibi ödemeler yapılıyor: cari "
             "kartının bir alt türü.", "E0008 (kolaybi.com/destek/cari-hesaplar)"),
            "Bu cari türlerinin patronun şahsi harcaması için kullanıldığı kaynakta anlatılmıyor.",
            "Borçlandırma ve tahsilat → 8. Cari tiplerinin işletme/şahsi ayrımıyla ilişkisi → Belge 2 6.5.",
        ],
        "urunler": [
            ("Bluecoins", "canli", "Hesaplar arasında CARİ HESAP başlığı; karşı taraf bir hesap olarak (→ 8.1).",
             "E0072"),
            ("Wallet", "canli", "Borçlar ayrı bir yüzeyde, kişi adıyla (→ 8.1).", "E0307"),
        ],
    },

    # ---------------------------------------------------------------- 6.5
    {
        "tur": "tablo", "no": "6.5", "baslik": "İşletme/şahsi ayrımı nerede var?",
        "giris": "Kayıt düzeyinde bir işletme/şahsi alanı yalnız QuickBooks Solopreneur'ün yardım merkezinde "
                 "anlatılıyor. Canlı incelenen beş üründe ve kaynakla incelenen üç Türk ön muhasebe ürününde böyle "
                 "bir alan görülmedi; yokluk ifadeleri incelenen sürüm, yüzey ve kaynakla sınırlıdır. Tablo her "
                 "ürünün bu ayrıma en yakın aracını gösteriyor.",
        "sutunlar": [("Ürün", 1.3), ("Ayrıma en yakın araç", 5.7)],
        "boy": 7.8,
        "satirlar": [
            "Canlı incelenen ürünler",
            ["Money Manager",
             {"t": "Kategori listesi kişisel; etiket yüzeyi görülmedi.", "tur": "canli", "d": "E0229"}],
            ["Bluecoins",
             {"t": "Kategori ve etiket; bir kayda İş etiketi verilip listede görüldü (→ 6.2).",
              "tur": "canli", "d": "E0088, E0491"}],
            ["Wallet",
             {"t": "Labels; Isletme ve Sahsi etiketiyle denendi (→ 6.2). Ayarlarda Automatic rules: kategori "
                   "ve etiketi kurala göre atama (kurulmadı).", "tur": "canli", "d": "E0278, E0425, E0378"}],
            ["Hesap Defterim",
             {"t": "Ayrı bir defter açmak; düğme adlarını değiştirmek. İkinci defterle ayrım denenmedi.",
              "tur": "canli", "d": "E0146, E0139"}],
            ["Goodbudget",
             {"t": "Zarflar (→ 6.1).", "tur": "canli", "d": "E0115"}],
            "Kaynakla incelenen ürünler",
            ["KolayBi",
             {"t": "Kullanıcı tanımlı proje; Ortaklar ve Personel carileri (→ 6.3, 6.4).", "tur": "kaynak",
              "d": "E0188, E0194"}],
            ["Paraşüt", {"t": "Kaynakta anlatılmıyor.", "tur": "yok", "d": "E0011"}],
            ["Logo İşbaşı", {"t": "Kaynakta anlatılmıyor.", "tur": "yok", "d": "E0009"}],
            ["QuickBooks Solopreneur",
             {"t": "Alanın kendisi: işlem başına tek Type alanı, Business / Personal; üçüncü değer yok. Type ve "
                   "Category ayrı sütun; listede Type'a göre süzme. Ayrım ABD vergi formuna hizalı. Yakın araçlar: "
                   "Split, tutarı parçalara bölüp her parçaya ayrı tür ve kategori; Rules ile otomatik etiketleme; "
                   "Exclude.", "tur": "beyan", "d": "E0012"}],
        ],
        "notlar": [
            "QuickBooks Solopreneur'ün hiçbir ekranı görülmedi; satır yalnız yardım merkezi anlatımıdır.",
            "Etiketin ve öteki araçların kapsam ayrımına ne kadar yaklaştığı → Belge 2 6.2.",
        ],
        "dayanak": ["Canlı kare: E0229, E0088, E0491, E0278, E0146, E0139, E0115. Kaynak görseli: E0188, E0194. "
                    "Kaynak beyanı: E0011, E0009, E0012"],
    },
]

EKSIKLER = [
    ("KolayBi", "Ortaklar sekmesinin ekranı (destek sayfasında yayımlanmamış)", "6.4",
     "Canlı deneme hesabı gerekir", "Önerilmez"),
    ("Hesap Defterim", "Serbest Açıklama / Kategori metninin raporda sınıf gibi kullanılıp kullanılmadığı", "6.1",
     "Aynı metinle iki kayıt girip rapora bakmak", "Orta"),
    ("Bluecoins", "Etiketin nasıl oluşturulduğu (hazır mı, kullanıcı mı ekliyor)", "6.2",
     "Etiketler ekranında yeni etiket eklemek", "Orta"),
    ("QuickBooks Solopreneur", "Type alanının ekranı", "6.5", "ABD hesabı gerekir", "Önerilmez"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0082", "E0087", "E0193", "E0378", "E0425", "E0426", "E0497"]
