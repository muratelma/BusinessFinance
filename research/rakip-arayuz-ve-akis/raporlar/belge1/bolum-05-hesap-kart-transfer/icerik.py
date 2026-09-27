# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 5 · Hesap, kart ve transfer — tek icerik kaynagi (plan §3)."""

NO = 5
BASLIK = "Hesap, kart ve transfer"
ANA_SORU = "Para nerede duruyor, ürün bunu nasıl gösteriyor ve hesaplar arasında nasıl geçiriliyor?"
GIRIS = [
    "Bu bölüm hesap listesini, hesap açarken sorulan açılış alanını, kredi kartının yüzeyini, kart borcunun "
    "gösterimini, kart ödemesinin nereden başladığını ve transferin formda ve listede nasıl göründüğünü izler.",
    "Yokluk ifadeleri incelenen sürüm ve yüzeyle sınırlıdır. Bir kaydın bakiyeye ve toplamlara etkisi bu "
    "bölümde anlatılmaz; ekranda görünen alan, etiket ve düğme anlatılır.",
]
GIRMEZ = [
    "Kart harcamasının rapora etkisi → 9 ve Belge 2",
    "Taksitli kart harcaması → 7",
    "Cari / karşı taraf hesabı → 8",
    "Transferin toplamlara etkisi → Belge 2",
    "Banka bağlantısı → 10",
]
KAPSAM = [
    ("Money Manager", ["canli"], None),
    ("Bluecoins", ["canli"], "Ekstre davranışı doğrulanmadı."),
    ("Wallet", ["canli"], "Yeni hesap ücretsiz pakette açılamıyor; açılış alanı düzenleme formunda yok."),
    ("Hesap Defterim", ["canli"], None),
    ("Goodbudget", ["canli"], "Tek hesap sınırı; kart ve transfer denenemedi."),
    ("KolayBi", ["kaynak"], "Tablolar boş; davranış yok."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["yok"], "Hesap ve kart yüzeyi görülmedi."),
    ("QuickBooks Solopreneur", ["yok"], "Hesap ve kart yüzeyi görülmedi."),
]

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 5.1
    {
        "tur": "yanyana", "no": "5.1", "baslik": "Hesap listesi nasıl sunuluyor?",
        "giris": "Money Manager, Bluecoins ve Goodbudget hesapları türe göre grupluyor; Wallet renkli kartlar "
                 "kullanıyor, Hesap Defterim yalnız defter adlarını diziyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "mm-hesaplar", "e": "E0235", "ad": "Money Manager", "etiket": "Hesaplar",
             "satirlar": ["Gruplar: Nakit, Banka Hesapları, Kredi Kartı.",
                          "Üstte Varlıklar, Borçlar, Toplam.",
                          "Kart grubunda iki dönem sütunu (→ 5.4)."]},
            {"k": "bc-hesaplar", "e": "E0027", "ad": "Bluecoins", "etiket": "Hesaplar",
             "satirlar": ["Üst başlıklar: VARLIKLAR, CARİ HESAP.",
                          "Grup satırında grup toplamı.",
                          "Hesabın altında para birimi."]},
            {"k": "wl-hesaplar", "e": "E0283", "ad": "Wallet", "etiket": "Home › Accounts",
             "satirlar": ["Hesaplar iki sütunlu renkli kartlar.",
                          "Kartta hesap adı ve bakiye.",
                          "Toplam satırı bu karede yok."]},
            {"k": "hd-hesaplar", "e": "E0146", "ad": "Hesap Defterim", "etiket": "Hesaplar",
             "satirlar": ["Yalnız defter adları; bakiye yok.",
                          "Hesap türü yok.",
                          "Varsayılan defter de listede."]},
            {"k": "gb-kapali", "e": "E0111", "ad": "Goodbudget", "etiket": "ACCOUNTS, ilk hâl",
             "satirlar": ["Hesap katmanı kapalı geliyor.",
                          "TURN ON ACCOUNTS ile açılıyor."]},
            {"k": "gb-gruplar", "e": "E0112", "ad": "Goodbudget", "etiket": "Edit Accounts",
             "satirlar": ["Gruplar: Checking, Savings, or Cash · Credit Card · Debt.",
                          "Grup adında hesap sayısı."]},
        ],
        "notlar": [
            "Money Manager ve Bluecoins liste başında ya da grup satırında toplam gösteriyor; Wallet ve Hesap "
            "Defterim'in bu karelerinde toplam satırı yok.",
            "Goodbudget'ta bütçe zarfları hesap olmadan çalışıyor; hesaplar isteğe bağlı bir katman.",
        ],
        "sag_notlar": [
            ("Bluecoins listesinin üstünde Nakit Akım Ayarı girişi var (→ 9.5).", "E0027"),
            ("Money Manager'da hesap toplama dahil edilmeyince satırı gri görünüyor (→ 9.5).", "E0253"),
        ],
    },

    # ---------------------------------------------------------------- 5.2
    {
        "tur": "soru", "no": "5.2", "baslik": "Hesap açma ve açılış bakiyesi alanı",
        "giris": "Money Manager, Bluecoins ve Hesap Defterim açılışı hesap formunda soruyor, ama üç ayrı biçimde: "
                 "tek tutar ve sonradan gelen bir soru, tutar ile tarih yan yana, isteğe bağlı tutar ile işaret ve tarih.",
        "yukseklik": 300, "not_genislik": 250,
        "sekiller": [
            {"k": "mm-acilis", "e": "E0239", "etiket": "Money Manager · hesap formu ve fark sorusu",
             "isaretler": [
                 (1, 700, 600, "Hesap formunda tek Tutar alanı."),
                 (2, 700, 930, "Toplama Dahil Et anahtarı (→ 9.5)."),
                 (3, 540, 1300, "Kaydedince soru: fark İşlemler'de gösterilsin mi?"),
             ]},
            {"k": "mm-fark", "e": "E0240", "etiket": "Money Manager · hesabın defteri",
             "isaretler": [
                 (4, 700, 830, "Fark defterde Bakiye Farkı satırı olarak."),
                 (5, 410, 705, "Satır kaydın yapıldığı günün tarihiyle."),
             ]},
            {"k": "bc-hesap", "e": "E0048", "etiket": "Bluecoins · Yeni Hesap",
             "isaretler": [
                 (6, 700, 865, "Başlangıç bakiyesi."),
                 (7, 700, 1120, "Açılış tarihi aynı formda."),
             ]},
            {"k": "hd-acilis", "e": "E0145", "etiket": "Hesap Defterim · Hesap Eklem",
             "isaretler": [
                 (8, 850, 1160, "Açılış bilançosu isteğe bağlı."),
                 (9, 850, 1320, "Artı / eksi seçimi."),
                 (10, 850, 1455, "Açılışın tarihi."),
             ]},
        ],
        "notlar": [
            "Açılışın hangi toplama girdiği bu bölümün konusu değildir (→ Belge 2 2.1).",
        ],
        "urunler": [
            ("Wallet", "canli", "Hesap eklerken önce tür soruluyor: Bank Sync, Investments, File Import, Manual "
             "Input. Ücretsiz pakette dördüncü hesap premium duvarına çarpıyor. Mevcut hesabın düzenleme "
             "formunda açılış alanı yok; hesap detayında bir bakiye düzenleme girişi var (→ Belge 2 2.1).",
             "E0471, E0472, E0437, E0297"),
            ("Goodbudget", "yok", "Hesap ekleme formu görülmedi.", "E0006"),
        ],
    },

    # ---------------------------------------------------------------- 5.3
    {
        "tur": "soru", "no": "5.3", "baslik": "Kredi kartı yüzeyi",
        "giris": "Beş canlı üründe beş farklı kart sunumu ve erişim durumu var: ekstre dönemli form, dönem alanları "
                 "olan form, dönemsiz form, kart kavramı olmayan defter ve açılamayan kart hesabı.",
        "yukseklik": 350,
        "sekiller": [
            {"k": "mm-kart", "e": "E0233", "etiket": "Money Manager · kredi kartı hesabı",
             "isaretler": [
                 (1, 700, 365, "Tür: Kredi Kartı."),
                 (2, 700, 700, "Kaynak: ödemenin yapılacağı hesap."),
                 (3, 700, 1075, "Hesap Kesim Tarihi ve Son Ödeme Tarihi."),
                 (4, 360, 1390, "Bu Ay ve Gelecek Ay dönemleri, ödeme tarihleriyle."),
             ]},
            {"k": "wl-kart", "e": "E0298", "etiket": "Wallet · Edit account",
             "isaretler": [
                 (5, 700, 940, "Type: Credit card."),
                 (6, 700, 1170, "Limit alanı; bu kartta 0."),
                 (7, 700, 1410, "Available Credit gösterimi."),
                 (8, 700, 1645, "Tek Payment Due Date; Not set."),
             ]},
            {"k": "gb-sinir", "e": "E0114", "etiket": "Goodbudget · hesap sınırı",
             "isaretler": [
                 (9, 700, 675, "Credit Card grubu var, boş."),
                 (10, 900, 1100, "Hesap sınırına ulaşıldı."),
                 (11, 300, 1360, "Ücretsiz pakette tek hesap; kart hesabı açılamadı."),
             ]},
        ],
        "notlar": [
            ("Wallet formunda kesim günü ya da ekstre alanı görülmedi. Payment Due Date ayın gününü bir "
             "tekerlekle seçtiriyor; neyi tetiklediği görülmedi.", "E0298, E0483"),
            "Goodbudget'a erişememek ürünün kart yüzeyi hakkında bir hüküm değildir.",
        ],
        "urunler": [
            ("Bluecoins", "canli", "Kart hesabı formunda Kredi Limiti, Hesap Kesim Günü ve Bitiş tarihi "
             "([[bc-hesap]]). Ekstre davranışı doğrulanmadı.", "E0048"),
            ("Hesap Defterim", "canli", "Hesap formunda tür alanı yok; kart, eksiye giden bir defter olarak "
             "tutuluyor.", "E0145, E0141"),
        ],
    },

    # ---------------------------------------------------------------- 5.4
    {
        "tur": "soru", "no": "5.4", "baslik": "Kart borcu nasıl gösteriliyor?",
        "giris": "Money Manager kart borcunu iki dönem sütununa ayırıyor ve kartın defterinde satır başına "
                 "bakiye tutuyor. Wallet kartı eksi bakiyeli bir hesap olarak gösteriyor ve eşiği geçince uyarıyor.",
        "yukseklik": 360,
        "sekiller": [
            {"k": "mm-borc", "e": "E0236", "etiket": "Money Manager · Hesaplar",
             "isaretler": [
                 (1, 700, 345, "Borçlar üst satırda ayrı."),
                 (2, 450, 975, "Kart grubunda Bu Ay ve Gelecek Ay sütunları."),
                 (3, 450, 1130, "Borç Bu Ay sütununda."),
             ]},
            {"k": "mm-kart-defter", "e": "E0255", "etiket": "Money Manager · kartın defteri",
             "isaretler": [
                 (4, 620, 455, "Faturalama dönemi başlıkta; altında Para Yatırma, Çekme, Toplam, Bakiye."),
                 (5, 650, 830, "Satırda o kayıttan sonraki bakiye."),
                 (6, 560, 1960, "Ödeme düğmesi (→ 5.5)."),
             ]},
            {"k": "wl-borc", "e": "E0296", "etiket": "Wallet · Home",
             "isaretler": [
                 (7, 820, 490, "Kart eksi bakiyeyle, diğer hesaplarla aynı düzende."),
                 (8, 560, 2080, "Bakiye eşiğin altına inince uyarı."),
             ]},
        ],
        "notlar": [
            "İncelenen beş canlı üründen yalnız Money Manager kart borcunu iki dönem sütununda gösteriyor.",
            ("Uyarının metni \"minimum threshold\" diyor. Hesap düzenleme formunda \"Minimum balance — Get "
             "notified when balance drops below this amount\" anahtarı var (açık, tutar 0.00); formun hangi "
             "hesaba ait olduğu karede okunmuyor, bağ kurulmadı.", "E0296, E0437"),
            "Bu Ay tutarının ödenmesi gereken tutarla ilişkisi → Belge 2 3.7.",
        ],
        "urunler": [
            ("Bluecoins", "canli", "Kart hesabı eksi bakiyeyle listeleniyor; ekstre görünümü görülmedi.",
             "E0050, E0072"),
            ("Hesap Defterim", "canli", "Kartın defteri eksi dengeyle.", "E0141"),
            ("Goodbudget", "yok", "Kart hesabı açılamadı (→ 5.3).", "E0114"),
        ],
    },

    # ---------------------------------------------------------------- 5.5
    {
        "tur": "soru", "no": "5.5", "baslik": "Kart ödemesi nereden başlıyor?",
        "giris": "İncelenen beş canlı üründen yalnız Money Manager'da karta özel bir ödeme düğmesi var "
                 "([[mm-kart-defter]]). Bluecoins ve Wallet'ta kart ödemesi hesaplar arası transferle yapılıyor.",
        "yukseklik": 300, "not_genislik": 240,
        "sekiller": [
            {"k": "mm-odeme", "e": "E0237", "etiket": "Money Manager · Ödeme'nin açtığı form",
             "isaretler": [
                 (1, 560, 190, "Form Havale türünde açılıyor."),
                 (2, 700, 580, "Tutar o anki borçla aynı."),
                 (3, 700, 760, "Kaynak Ana Hesap, Giriş kart."),
                 (4, 750, 925, "Not kendiliğinden: Ödeme Bilgisi."),
             ]},
            {"k": "mm-kismi", "e": "E0248", "etiket": "Money Manager · tutarı değiştirilmiş ödeme",
             "isaretler": [
                 (5, 700, 580, "Tutar 400 olarak değiştirilmiş."),
             ]},
            {"k": "bc-odeme", "e": "E0050", "etiket": "Bluecoins · işlem listesi",
             "isaretler": [
                 (6, 620, 590, "Ödeme bağlı iki transfer satırı."),
                 (7, 620, 700, "İkinci satırda kartın yeni bakiyesi."),
             ]},
            {"k": "wl-odeme", "e": "E0333", "etiket": "Wallet · transfer formu",
             "isaretler": [
                 (8, 560, 960, "Tutar serbest."),
                 (9, 560, 1060, "From → To."),
                 (10, 850, 1290, "Hedef hesaptaki tutar ayrıca yazıyor."),
             ]},
        ],
        "notlar": [
            "Money Manager'da ödeme formu borç tutarıyla ön doldurulmuş açılıyor ve tutar değiştirilebiliyor.",
            "Ödemenin hangi dönemin borcuna uygulandığı → Belge 2 3.6.",
        ],
        "urunler": [
            ("Hesap Defterim", "canli", "Kart ödemesi de defterler arası Aktar ile (→ 5.6).", "E0147"),
            ("Goodbudget", "yok", "Kart hesabı olmadığı için denenmedi.", "E0006"),
        ],
    },

    # ---------------------------------------------------------------- 5.6
    {
        "tur": "soru", "no": "5.6", "baslik": "Transfer formu",
        "giris": "Transfer beş canlı üründe ayrı bir form ya da ayrı bir tür. Money Manager'ın Havale formu 5.5'te "
                 "([[mm-odeme]]).",
        "yukseklik": 300, "not_genislik": 240,
        "sekiller": [
            {"k": "bc-transfer", "e": "E0029", "etiket": "Bluecoins · TRANSFER formu",
             "isaretler": [
                 (1, 700, 890, "Kaynak ve hedef hesap alt alta."),
                 (2, 800, 1040, "Yön değiştirme simgesi."),
                 (3, 700, 1150, "Transfer ücreti alanı."),
                 (4, 400, 2110, "TRANSFER seçili."),
             ]},
            {"k": "hd-aktar", "e": "E0147", "etiket": "Hesap Defterim · Aktar",
             "isaretler": [
                 (5, 400, 405, "Miktar."),
                 (6, 400, 560, "Kimden / Kime."),
                 (7, 560, 995, "Notlar; kategori alanı yok."),
             ]},
            {"k": "gb-transfer", "e": "E0119", "etiket": "Goodbudget · Account Transfer",
             "isaretler": [
                 (8, 700, 420, "From ve To."),
                 (9, 700, 990, "Açıklama kendiliğinden: Account Transfer."),
                 (10, 700, 1275, "Schedule this… burada da var."),
             ]},
            {"k": "wl-transfer", "e": "E0327", "etiket": "Wallet · hızlı form, TRANSFER",
             "isaretler": [
                 (11, 560, 1060, "From → To."),
             ]},
        ],
        "notlar": [
            ("Bluecoins'te ücret açılınca ayrı bir blok çıkıyor: kendi tutarı, kendi hesabı ve kendi kategorisi "
             "(varsayılan Diğer).", "E0435"),
            ("Wallet'ın hedef hesap seçicisinde dördüncü seçenek \"…outside of Wallet\"; kullanılmadı.", "E0328"),
            "Goodbudget'ın transfer formunda zarf alanı görülmüyor.",
            ("Goodbudget'ta tek hesap olduğu için gerçek bir transfer yapılamadı.", "E0120"),
        ],
        "urunler": [
            ("Money Manager", "canli", "Havale formunda tutarın yanında Harç düğmesi: aktarım ücreti formda "
             "ayrı bir giriş.", "E0462"),
        ],
    },
    {
        "tur": "soru", "no": "5.6", "baslik": "Transfer listede nasıl görünüyor?", "haritada": False,
        "giris": "Money Manager transferi tek bir satırda gösteriyor. Bluecoins, Wallet ve Hesap Defterim iki "
                 "satır yazıyor: biri çıkan, biri giren hesap için.",
        "yukseklik": 300, "not_genislik": 240,
        "sekiller": [
            {"k": "mm-transfer-liste", "e": "E0230", "etiket": "Money Manager · İşlemler",
             "isaretler": [
                 (1, 720, 665, "Tek satır: Ana Hesap → Ortak Cuzdan."),
                 (2, 470, 520, "Gün başlığında gelir ve gider 0."),
             ]},
            {"k": "bc-transfer-liste", "e": "E0030", "etiket": "Bluecoins · İşlemler",
             "isaretler": [
                 (3, 560, 820, "Gün başlığı ₺0,00."),
                 (4, 720, 860, "İki bağlı satır: eksi ve artı."),
                 (5, 600, 1112, "Satırda işlem sonrası hesap bakiyesi."),
             ]},
            {"k": "wl-transfer-liste", "e": "E0285", "etiket": "Wallet · Records",
             "isaretler": [
                 (6, 700, 470, "Hafta toplamı ₺0."),
                 (7, 700, 640, "Kart ödemesi iki satır: artı ve eksi."),
                 (8, 700, 1200, "İki satır da Transfer, withdraw adıyla."),
             ]},
            {"k": "hd-transfer-liste", "e": "E0140", "etiket": "Hesap Defterim · İşlemler-Bütün Hesaplar",
             "isaretler": [
                 (9, 450, 510, "Hesaplar sütunu satırın defterini yazıyor."),
                 (10, 450, 700, "Aktar iki satır: Kime ve Kimden."),
             ]},
        ],
        "notlar": [
            "Tek satırlı gösterimde iki hesap aynı satırda okla yazılıyor; iki satırlı gösterimde her hesap kendi "
            "satırında.",
            "Transferin toplamlara etkisi → Belge 2 2.3, 2.4.",
        ],
        "urunler": [
            ("Goodbudget", "yok", "Transfer yapılamadığı için listede görünümü görülmedi.", "E0120"),
        ],
    },

    # ---------------------------------------------------------------- 5.7
    {
        "tur": "soru", "no": "5.7", "baslik": "Kaynakta görülen hesap ve kart yüzeyleri",
        "giris": "KolayBi'nin destek sayfalarında Finans modülü altı sekmeyle görünüyor ve kredi kartı formu canlı "
                 "ürünlerde görülmeyen alanlar taşıyor. Alan adlarıyla kanıtlı; tablolar boş ya da demo.",
        "dikey": True,
        "sekiller": [
            {"k": "kb-banka", "e": "E0206", "etiket": "KolayBi · Finans › Banka Hesapları",
             "kirpma": (96, 112, 1305, 520), "genislik": 470,
             "isaretler": [
                 (1, 834, 63, "Finans altında altı sekme."),
                 (2, 504, 211, "Hesap ekleme ve dışa aktarma."),
                 (3, 864, 233, "Listede Açılış Tarihi ve Bakiye kolonları."),
                 (4, 984, 173, "Toplam ve para birimi bakiyesi üstte."),
             ]},
            {"k": "kb-kart", "e": "E0208", "etiket": "KolayBi · Yeni Kredi Kartı",
             "kirpma": (96, 112, 1305, 600), "genislik": 470,
             "isaretler": [
                 (5, 504, 245, "Kart numarası zorunlu."),
                 (6, 904, 245, "Hesap Kesim Günü ve Son Ödeme Günü zorunlu."),
                 (7, 804, 305, "Kart Limiti ve Minimum Ödeme Oranı zorunlu."),
                 (8, 354, 404, "Detay ekle kapalı."),
             ]},
        ],
        "notlar": [
            ("Kredi Kartları listesinde Kalan Limit kolonu var; tablo boş, hesaplanışı görülmedi.", "E0207"),
            ("Bir tanıtım videosu karesinde Finans altında dört sekme vardı; ötekilerin ekleniş zamanı "
             "bilinmiyor. Video karesi basılmadı.", "E0186"),
        ],
        "urunler_baslik": "Karesi basılmayan ürünler",
        "urunler": [
            ("Paraşüt", "beyan", "Kasa ve bankalar tek listede; IBAN ve döviz sütunu ve banka hesabı bağlama "
             "girişi.", "E0011; video karesi E0263"),
            ("Logo İşbaşı, QuickBooks Solopreneur", "yok", "Hesap ve kart yüzeyi görülmedi.", "E0009, E0012"),
        ],
        "dayanak": ["Kaynak beyanı: E0011"],
    },
]

EKSIKLER = [
    ("Wallet", "Yeni hesap formu (ücretsiz pakette açılamıyor)", "5.2", "Ücretli paket gerekir", "Önerilmez"),
    ("Goodbudget", "Hesap ekleme formu, kart hesabı ve transfer", "5.2, 5.3, 5.6",
     "Ücretli paket gerekir", "Önerilmez"),
    ("Bluecoins", "Kart ekstre davranışı", "5.3", "Gelecek dönemi beklemek gerekir", "Önerilmez"),
    ("Wallet", "Payment Due Date'in neyi tetiklediği (hatırlatma, ekstre)", "5.3",
     "Bir gün seçip o günü beklemek gerekir", "Önerilmez"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0328", "E0435", "E0437", "E0462", "E0471", "E0472", "E0483"]
