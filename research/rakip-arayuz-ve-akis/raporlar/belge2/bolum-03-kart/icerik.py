# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 3 · Kart: harcama, taksit, borc, odeme — tek icerik kaynagi."""

NO = 3
BASLIK = "Kart: harcama, taksit, borç, ödeme"
ANA_SORU = "Kartla harcanan para nereye yazılıyor, ne zaman gider sayılıyor ve borç hangi sayıyla gösteriliyor?"
EN_AZ_KARE = 16

GIRIS = [
    "Kart, bu araştırmanın ürünleri en çok ayrıştıran konusu. Aynı 6.000'lik harcama bir üründe "
    "altı aya bölünmüş altı kayıt, bir üründe bir kayıt artı beş hatırlatıcı, bir üründe tek "
    "parça bir borç oluyor.",
    "Bölüm dört soruyu sırayla izliyor: kart üründe ne olarak kuruluyor, kartla harcayınca ne "
    "oluşuyor, taksit aylara nasıl dağılıyor ve ödeme yapıldığında hangi sayı düşüyor.",
]
GIRMEZ = [
    "Karşı tarafa borç ve tahsilat → Bölüm 4",
    "Tekrarlayan planın kurulması → Bölüm 5",
    "Kart ekranlarının görsel düzeni → Belge 1 §5",
    "Ay raporunun dönem seçimi → Bölüm 7",
]
KAPSAM = [
    ("Money Manager", ["canli"], "En derin ürün: kart formu, iki dönem sütunu, taksit zinciri ve kart defteri kareli."),
    ("Bluecoins", ["canli"], "Kart alanları, taksit kaydı ve kısmi ödeme kareli; dönem davranışı görülmedi."),
    ("Wallet", ["canli"], "Kart hesabı ve ayarları kareli; taksit alanı görünen bölümde yok."),
    ("Hesap Defterim", ["canli"], "Kart ayrı bir kavram değil; defter ve jenerik aktarım kareli."),
    ("Goodbudget", ["yok"], "Kart hesabı ücretsiz pakette açılamadı."),
    ("KolayBi", ["kaynak"], "Kredi kartı formu ve listesi destek görselinde."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], None),
]
ACILIS_DAYANAK = (
    "Rozetler kanıt düzeyini gösterir: ölçülen davranış Canlı kare, kanıttan çıkarılan neden "
    "Çıkarım. Kaynak görselleri yüzeyi kanıtlar, davranışı değil."
)

SAYFALAR = [
    {"tur": "acilis"},

    # ------------------------------------------------------------------ 3.1
    {
        "tur": "yanyana", "no": "3.1",
        "baslik": "Kart üründe ne olarak kuruluyor",
        "giris": "Üç canlı üründe de kartın kendi formu var ve üçü de kesim günü ya da limit "
                 "soruyor. Ayrışma alanların varlığında değil, o alanların bir dönem üretip "
                 "üretmediğinde.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "k-mm", "e": "E0233", "ad": "Money Manager", "etiket": "Kredi Kartı hesap formu",
             "satirlar": ["Hesap Kesim Tarihi ve Son Ödeme Tarihi.",
                          "Altta iki dönem kutusu: Bu Ay 01/08 ~ 31/08",
                          "(Ödeme 01/09) ve Gelecek Ay."]},
            {"k": "k-bc", "e": "E0048", "ad": "Bluecoins", "etiket": "Kart hesabı alanları",
             "satirlar": ["Hesap kesim günü ve limit alanları var.",
                          "Bitiş tarihi etiketinin anlamı karede açılmıyor.",
                          "Dönemin nasıl hesaplandığı görülmedi."]},
            {"k": "k-wl", "e": "E0298", "ad": "Wallet", "etiket": "Edit account · Is Karti",
             "satirlar": ["Type Credit card; limit alanı 0.",
                          "Balance Display Options: Available Credit.",
                          "Payment Due Date: Not set."]},
        ],
        "notlar": [
            "Hesap Defterim'de kart ayrı bir kavram değil. Kart da diğerleri gibi bir defter; "
            "kesim günü, limit veya son ödeme tarihi alanı yok ve kart ödemesi jenerik bir "
            "aktarım olarak yazılıyor.",
            "Goodbudget'ta kredi kartı hesabı ücretsiz pakette açılamadı: tek hesap sınırı "
            "dolduğu için kart türünde hesap oluşturulamadı. Bu üründe kart modeli hiç "
            "görülmedi.",
        ],
        "sag_notlar": [
            ("Money Manager'ın iki dönem kutusu bu bölümün en ayırt edici alanı: kartın borcu "
             "tek sayı değil, iki dönem olarak tutuluyor.", "E0233"),
            ("Wallet'ta kartın limiti 0 bırakıldı; Credit sekmesi borç varken kullanımı %0 "
             "gösteriyor. Ödeme günü ayın bir günü olarak seçiliyor ve koşumda boş kaldı.",
             "E0298 · E0461"),
        ],
    },

    # ----------------------------------------------------------------- 3.1b
    {
        "tur": "soru", "no": "3.1b", "baslik": "Kaynakta görünen kart formu",
        "giris": "KolayBi'nin destek görseli kart formunun alanlarını adıyla gösteriyor. "
                 "Canlı üç üründe bulunmayan iki alan burada var: kart limiti ve minimum "
                 "ödeme oranı. Alanların varlığı kanıtlı; kaydın sonucu görülmedi.",
        "yukseklik": 330, "not_genislik": 260,
        "sekiller": [
            {"k": "k-kb", "e": "E0208", "etiket": "KolayBi · Yeni Kredi Kartı formu",
             "kirpma": (96, 112, 1305, 865), "genislik": 420,
             "satirlar": ["Ad, Kart Numarası, Hesap Kesim Günü ve Son Ödeme Günü zorunlu.",
                          "Kart Limiti ve Minimum Ödeme Oranı (%) ayrı alanlar.",
                          "Açılış Tarihi ve Para Birimi de zorunlu."]},
        ],
        "notlar": [
            "Minimum ödeme oranı canlı dört ürünün hiçbirinde bulunmadı. Kart listesinde "
            "ayrıca Kalan Limit kolonu var; limitten kullanılan tutarın düşülmesiyle "
            "hesaplandığı çıkarımdır, davranış görülmedi.",
            "Kart burada finans yüzeyinin altı sekmesinden biri: Banka Hesapları, Kasalar, "
            "Kredi Kartları, Online Banka Hesapları, Çekler, Senetler.",
        ],
        "sag_notlar": [
            ("Kart listesinin kolonları: Kredi Kartı Adı, Etiketler, Kart Numarası, Hesap "
             "Kesim Günü, Son Ödeme Günü, Kart Limiti, Açılış Tarihi, Kalan Limit.", "E0207"),
        ],
        "urunler_baslik": "Aynı soruda kaynaktan okunan üç ürün",
        "urunler": [
            ("Paraşüt", "beyan",
             "Kaynak kasa ve bankanın yanında ayrı bir kart kavramından söz etmiyor; ödeme "
             "adımı kasa/banka bakiyesini azaltıyor.", "parasut.com kılavuzu"),
            ("Logo İşbaşı", "beyan",
             "Kaynak kasa-banka yönetimi ve çek giriş/çıkışı sayıyor; kart dönemi veya ekstre "
             "anlatılmıyor.", "isbasi.com"),
            ("QuickBooks Solopreneur", "beyan",
             "Kaynağa göre kart bir bağlı hesap: işlemler karttan otomatik iniyor ve "
             "sınıflanıyor; kart borcu veya ekstre modeli belgelenmemiş.", "Intuit yardım merkezi"),
        ],
    },

    # ------------------------------------------------------------------ 3.2
    {
        "tur": "yanyana", "no": "3.2",
        "baslik": "Kartla harcayınca borç nereye yazılıyor",
        "giris": "Üç üründe üç ayrı yer: dönemli bir borç sütunu, eksiye inen bir hesap "
                 "bakiyesi ve defterin yürüyen dengesi. Harcamanın ayın giderine de girdiği "
                 "Money Manager, Bluecoins ve Hesap Defterim'de ölçüldü.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "h-mm", "e": "E0236", "ad": "Money Manager", "etiket": "Hesaplar · ödeme öncesi",
             "satirlar": ["Is Karti: Bu Ay 1.200 (kırmızı) / Gelecek Ay 0.",
                          "Borçlar 1.200; Varlıklar 46.150; Toplam 44.950.",
                          "Borç iki dönem sütununda duruyor."]},
            {"k": "h-wl", "e": "E0297", "ad": "Wallet", "etiket": "Account Detail · Is Karti",
             "satirlar": ["Kart bir hesap; TODAY −6.000,00.",
                          "Grafik bugün −6k'ya iniyor.",
                          "Dönem yok; borç tek parça bakiye."]},
            {"k": "h-hd", "e": "E0153", "ad": "Hesap Defterim", "etiket": "Is Karti defteri",
             "satirlar": ["Kart sıradan bir defter; Denge −1.000.",
                          "Toplamlar 1.600 / 2.200 / −600.",
                          "Kart ödemesi jenerik aktarım bacağı."]},
        ],
        "notlar": [
            ("Wallet kart bakiyesi eksiye inince bir uyarı gösterdi: \"Your balance on Is Karti "
             "dropped below the minimum threshold.\" Eşik hesabın kendi formunda: en az bakiye "
             "bildirimi açık ve tutarı 0. Uyarı kart mantığından değil, her hesapta bulunan bu "
             "bildirimden geliyor.", "E0296 · E0437"),
            "Bluecoins'te kart harcaması dönem raporunun giderini 1.000 artırdı; kart borcu ile "
            "gider aynı anda yazılıyor. Kartın dönem davranışı bu koşumda görülmedi.",
        ],
        "sag_notlar": [
            ("Money Manager'da kart harcaması ayın gider toplamına da giriyor: Ağustos'un "
             "2.050'sinin 1.200'ü karttan.", "E0231"),
            ("Bluecoins'te taksit kaydından sonra dönem gideri 3.050 okundu.", "E0038"),
        ],
    },

    # ------------------------------------------------------------------ 3.3
    {
        "tur": "yanyana", "no": "3.3",
        "baslik": "Taksit: bir plan mı, altı kayıt mı, hiç mi",
        "giris": "Aynı 6.000'lik taksitli harcama üç üründe üç ayrı şeye dönüşüyor. Ayrışma "
                 "formda başlıyor: ikisinde taksit sayısı sorulan bir alan, birinde kaydın "
                 "görünen bölümünde böyle bir alan yok.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "x-mm1", "e": "E0245", "ad": "Money Manager", "etiket": "Gider formu · 6 Ay rozeti",
             "satirlar": ["6.000, Is Karti, \"6 Ay\" rozeti formda.",
                          "Tekrar/Taksit menüsünden seçiliyor."]},
            {"k": "x-mm2", "e": "E0246", "ad": "Money Manager", "etiket": "Listede ilk taksit",
             "satirlar": ["\"Tasarim ekipmani (1/6)\" 1.000 olarak düşüyor.",
                          "Ağustos gideri 2.050 → 3.650.",
                          "Kayıt 6.000 değil, 1.000 yazıyor."]},
            {"k": "x-bc", "e": "E0037", "ad": "Bluecoins", "etiket": "Kalan beş taksit",
             "satirlar": ["İlk taksit gerçek kayıt oldu.",
                          "Kalan beşi hatırlatıcı listesinde bekliyor.",
                          "Liste niyeti gösteriyor, kaydı değil."]},
            {"k": "x-wl", "e": "E0294", "ad": "Wallet", "etiket": "Kayıt ayrıntısı",
             "satirlar": ["6.000 tek kayıt olarak durdu.",
                          "Görünen bölümde taksit alanı yok.",
                          "Payment Type Cash olarak kalmış."]},
        ],
        "notlar": [
            "Money Manager taksidi kaydın bir özelliği yapıyor: tek form, altı ayrı satır. "
            "Bluecoins ilk taksidi kayda, kalanını hatırlatıcıya yazıyor — yani yarısı gerçek, "
            "yarısı niyet. Wallet'ta 6.000 bölünmeden tek parça kaldı.",
            "Hesap Defterim'de taksit planı kavramı yok; kullanıcı ya tek 6.000 kaydı ya da elle "
            "altı ayrı kayıt açıyor ve \"6 taksit\" bilgisi hiçbir yere taşınmıyor.",
        ],
        "sag_notlar": [
            ("Bluecoins'in taksit şartları sayfasında oran alanı var; oranın faiz mi yüzde mi "
             "olduğu karede açılmıyor.", "E0033"),
            ("Bluecoins'te kalan taksitler kendiliğinden kayda dönüşmüyor: hatırlatıcıya "
             "dokununca Kaydet ve Düzenle çıkıyor, taksit ancak Kaydet ile gerçekleşiyor; ileri "
             "tarihli taksitte bugüne mi taksit gününe mi yazılacağı soruluyor. Bu bir kullanıcı "
             "kontrolüyle kayıtlı; karesi yok.", "kullanıcı kontrolü · E0037 · E0043"),
        ],
    },

    # ------------------------------------------------------------------ 3.4
    {
        "tur": "yanyana", "no": "3.4",
        "baslik": "Taksit aylara nasıl düşüyor (Money Manager)",
        "giris": "Taksit zinciri tek üründe uçtan uca izlenebildi. Dört ay ileri gidildiğinde "
                 "her ayın gideri tam olarak o ayın taksidi kadar; zincir altıncı taksitte "
                 "bitiyor ve sonraki ay boşalıyor.",
        "yukseklik": 187,
        "sekiller": [
            {"k": "t-eki", "e": "E0407", "ad": "Ekim", "etiket": "Taksit 3/6",
             "kirpma": (0, 120, 1080, 1215),
             "satirlar": ["Gider 1.000,00.", "15 Ekim: Tasarim ekipmani (3/6)."]},
            {"k": "t-ara", "e": "E0409", "ad": "Aralık", "etiket": "Taksit 5/6",
             "kirpma": (0, 120, 1080, 1215),
             "satirlar": ["Gider 1.000,00.", "Aynı tutar, aynı gün."]},
            {"k": "t-oca", "e": "E0410", "ad": "Ocak", "etiket": "Taksit 6/6",
             "kirpma": (0, 120, 1080, 1215),
             "satirlar": ["Zincirin son taksidi.", "Gider yine 1.000,00."]},
            {"k": "t-sub", "e": "E0411", "ad": "Şubat", "etiket": "Zincir bitti",
             "kirpma": (0, 120, 1080, 1215),
             "satirlar": ["Gelir/Gider/Toplam 0,00 — Veri yok.",
                          "Yalnız tekrarlayan plan önizlemesi kalıyor."]},
        ],
        "notlar": [
            "Altı taksit altı aya eşit dağılıyor ve her ay yalnız kendi taksidini gider yazıyor. "
            "Harcamanın tamamı ilk aya yazılmıyor; 6.000'in aylık görünümü 1.000.",
            "Ekim ve sonrasındaki aylarda listenin üstünde ayrı bir tekrarlayan plan bölümü "
            "duruyor ve o bölüm ayın toplamına girmiyor — plan ile gerçekleşmiş kayıt ayrı "
            "tutuluyor. Bu ayrımın kendisi Bölüm 5'te.",
        ],
    },

    # ------------------------------------------------------------------ 3.5
    {
        "tur": "tablo", "no": "3.5", "baslik": "Aynı kart olayları, beş üründe sonuç",
        "giris": "Kartla harcama, taksit ve ödeme üç ayrı olay; her biri farklı bir sayıyı "
                 "kıpırdatıyor. Tabloda ürünlerin bu üç olaya verdiği cevaplar yan yana.",
        "sutunlar": [("", 15), ("Money Manager", 17), ("Bluecoins", 17), ("Wallet", 17),
                     ("Hesap Defterim", 17), ("Goodbudget", 17)],
        "boy": 8.4,
        "satirlar": [
            [
                "Kartın dönemi var mı",
                {"t": "Evet — Bu Ay / Gelecek Ay iki sütun", "tur": "canli", "d": "E0233"},
                {"t": "Alanlar var; dönem davranışı görülmedi", "tur": "canli", "d": "E0048"},
                {"t": "Hayır — tek parça bakiye", "tur": "canli", "d": "E0297"},
                {"t": "Hayır — kart kavramı yok", "tur": "canli", "d": "E0153"},
                {"t": "Kart hesabı açılamadı", "tur": "yok", "d": "E0114"},
            ],
            [
                "Kart harcaması gider sayılıyor mu",
                {"t": "Evet — Ağustos giderinin 1.200'ü karttan", "tur": "canli", "d": "E0231"},
                {"t": "Evet — dönem gideri 1.000 arttı", "tur": "canli", "d": "E0038"},
                {"t": "Evet — 30 günlük gider 21.600'ün 6.000'i kart harcaması", "tur": "cikarim", "d": "E0440 · E0441 · E0398"},
                {"t": "Evet — defterde Ödendi satırı", "tur": "canli", "d": "E0153"},
                {"t": "Ölçülemedi", "tur": "yok", "d": "E0114"},
            ],
            [
                "Taksit ne üretiyor",
                {"t": "Altı ayrı kayıt, aylara bölünmüş", "tur": "canli", "d": "E0410"},
                {"t": "Bir kayıt + beş hatırlatıcı", "tur": "canli", "d": "E0037"},
                {"t": "Bölünmüyor; 6.000 tek parça", "tur": "canli", "d": "E0294"},
                {"t": "Taksit kavramı yok; elle girilir", "tur": "canli", "d": "E0153"},
                {"t": "Ölçülemedi", "tur": "yok", "d": "E0114"},
            ],
            [
                "Kısmi ödeme neyi düşürüyor",
                {"t": "Yalnız Bu Ay: 1.000 → 600", "tur": "canli", "d": "E0249"},
                {"t": "Kart bakiyesi −1.000 → −500", "tur": "canli", "d": "E0050"},
                {"t": "Tek parça bakiye: −5.600 → −5.300", "tur": "canli", "d": "E0438"},
                {"t": "Denge −1.000 → −600", "tur": "canli", "d": "E0153"},
                {"t": "Ölçülemedi", "tur": "yok", "d": "E0114"},
            ],
            [
                "Ödeme gider sayılıyor mu",
                {"t": "Hayır — ayrı satır: Gider (Kredi Kartı, Ödeme)", "tur": "canli", "d": "E0405"},
                {"t": "Hayır — aktarım; net 43.350 korunuyor", "tur": "canli", "d": "E0050"},
                {"t": "Hayır — aktarım; 30 günlük gider 21.600'de kaldı", "tur": "canli", "d": "E0438"},
                {"t": "Hayır — jenerik aktarım bacağı", "tur": "canli", "d": "E0153"},
                {"t": "Ölçülemedi", "tur": "yok", "d": "E0114"},
            ],
        ],
        "notlar": [
            "Ödemenin gider sayılmaması ölçülen dört üründe de aynı: ödeme parayı taşıyor, harcama zaten "
            "kart kullanıldığı gün gider yazılmıştı. İkisini birden saymak aynı harcamayı iki "
            "kez gider yapardı.",
        ],
    },

    # ------------------------------------------------------------------ 3.6
    {
        "tur": "yanyana", "no": "3.6",
        "baslik": "Kısmi ödeme: hangi sayı düşüyor, hangisi durur",
        "giris": "Kart borcunun bir kısmı ödendiğinde dört üründe de net varlık değişmiyor — para "
                 "yer değiştiriyor. Ayrışma hangi sayının düştüğünde: bir üründe yalnız kesilmiş "
                 "dönem, üçünde tek parça bakiye.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "o-mm1", "e": "E0248", "ad": "Money Manager", "etiket": "Ödeme formu",
             "satirlar": ["Tutar 400; Kaynak Ana Hesap, Giriş Is Karti.",
                          "Ödeme düğmesi formu ön dolduruyor,",
                          "tutar serbestçe değiştirilebiliyor."]},
            {"k": "o-mm2", "e": "E0249", "ad": "Money Manager", "etiket": "Ödeme sonrası hesaplar",
             "satirlar": ["Is Karti Bu Ay 1.000 → 600.",
                          "Gelecek Ay 1.000 değişmedi.",
                          "Toplam 41.750 ödeme öncesiyle aynı."]},
            {"k": "o-bc", "e": "E0050", "ad": "Bluecoins", "etiket": "500 kısmi ödeme",
             "satirlar": ["Kart −1.000 → −500; banka 40.200 → 39.700.",
                          "Net 43.350 korunuyor."]},
            {"k": "o-mm3", "e": "E0405", "ad": "Money Manager", "etiket": "Toplam sekmesi · Eylül",
             "satirlar": ["Gider (Nakit, Banka Hesapları) 600,00.",
                          "Gider (Kredi Kartı, Ödeme) 1.000,00 (400,00).",
                          "Ödenen tutar parantezde, gidere girmiyor."]},
        ],
        "notlar": [
            "Money Manager ödemeyi yalnız kesilmiş döneme yazıyor: Bu Ay 1.000'den 600'e indi, "
            "Gelecek Ay'daki 1.000 yerinde kaldı. Ödemenin hangi döneme sayılacağı sorusuna "
            "ürün açık bir cevap vermiş.",
            "Aynı ürünün Toplam sekmesi gideri ödeme kaynağına göre ayırıyor ve kart ödemesini "
            "gider satırının içine değil, parantezine koyuyor.",
        ],
        "sag_notlar": [
            ("Hesap Defterim'de kısmi ödeme jenerik bir aktarım: Denge −1.000'den −600'e indi ve "
             "ekstre veya asgari tutar kavramı devreye girmedi.", "E0153"),
            ("Wallet'ta kısmi ödeme hesaptan karta bir aktarım: 300 ödendi, kart −5.600'den "
             "−5.300'e indi, 30 günlük gider yerinde kaldı. Kart bakiyesi tek parça olduğu için "
             "ödemenin hangi döneme yazılacağı sorusu üründe oluşmuyor.", "E0297"),
        ],
    },

    # ------------------------------------------------------------------ 3.7
    {
        "tur": "yanyana", "no": "3.7",
        "baslik": "Aynı kart, aynı an, iki farklı borç sayısı",
        "giris": "Money Manager'ın iki ekranı aynı kart için iki ayrı borç gösteriyor: Hesaplar "
                 "1.600 derken kart defterinin Ocak satırı 5.600 diyor. İkisi de doğru — ama "
                 "hangisinin ne anlattığı ekranda yazmıyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "i-hes", "e": "E0414", "ad": "Hesaplar ekranı", "etiket": "Borçlar 1.600",
             "kirpma": (0, 120, 1080, 1215),
             "satirlar": ["Is Karti: Bu Ay 600 / Gelecek Ay 1.000.",
                          "Borçlar 1.600; Toplam 42.350."]},
            {"k": "i-eki", "e": "E0412", "ad": "Kart defteri · Ekim", "etiket": "Bakiye 2.600",
             "kirpma": (0, 120, 1080, 1215),
             "satirlar": ["Çekme 1.000; dönem toplamı −1.000.",
                          "Yürüyen bakiye ileriye uzuyor."]},
            {"k": "i-oca", "e": "E0413", "ad": "Kart defteri · Ocak", "etiket": "Bakiye −5.600",
             "kirpma": (0, 120, 1080, 1215),
             "satirlar": ["Zincirin son taksidi işlenmiş hâli.",
                          "Aynı kart, aynı an, farklı sayı."]},
        ],
        "notlar": [
            "Aradaki 4.000 tam olarak Ekim, Kasım, Aralık ve Ocak taksitlerinin toplamı. "
            "Hesaplar ekranı kesilmiş dönem ile bir sonraki dönemi topluyor; kart defteri ise "
            "gelecekte yazılmış bütün taksitleri yürüyen bakiyeye katıyor.",
            "İki yüzey farklı dönem kapsamı taşıyor ve bu kapsam hiçbir ekranda yazılı değil. "
            "Kullanıcı \"kartıma ne kadar borcum var\" sorusuna baktığı ekrana göre iki ayrı "
            "cevap alıyor.",
        ],
    },

    # ------------------------------------------------------------------ 3.8
    {
        "tur": "akis", "no": "3.8", "baslik": "Kart parasının yolu ve ayrıldığı noktalar",
        "giris": "Kart kurulduğu andan borç ödendiği ana kadar dört adım. İlk adımdaki tercih "
                 "sonraki üç adımın hepsini belirliyor.",
        "adimlar": [
            {
                "baslik": "Kart kuruluyor",
                "dallar": [
                    {"urunler": "Money Manager", "vurgu": True,
                     "metin": "Dönemli hesap: kesim günü ve son ödeme günü iki dönem kutusu "
                              "üretiyor — Bu Ay ve Gelecek Ay.", "d": "E0233"},
                    {"urunler": "Bluecoins",
                     "metin": "Alanlar var (kesim günü, limit) ama dönem davranışı bu koşumda "
                              "görülmedi.", "d": "E0048"},
                    {"urunler": "Wallet · Hesap Defterim",
                     "metin": "Dönemsiz: kart sıradan bir hesap veya sıradan bir defter.",
                     "d": "E0297"},
                ],
            },
            {
                "baslik": "Kartla harcanıyor",
                "ortak": "Dört üründe de harcama aynı anda iki şey yapıyor: kartın borcunu "
                         "artırıyor ve ayın giderine giriyor. Gider harcandığı gün yazılıyor, "
                         "ödendiği gün değil.",
            },
            {
                "baslik": "Taksitse ne oluyor",
                "dallar": [
                    {"urunler": "Money Manager", "vurgu": True,
                     "metin": "Altı aya bölünüp altı kayıt olarak yazılıyor; her ay yalnız kendi "
                              "taksidini gider sayıyor.", "d": "E0410"},
                    {"urunler": "Bluecoins",
                     "metin": "İlk taksit kayıt, kalan beşi hatırlatıcı — yarısı gerçekleşmiş, "
                              "yarısı niyet.", "d": "E0037"},
                    {"urunler": "Wallet · Hesap Defterim",
                     "metin": "Bölünmüyor. Tutarın tamamı tek kayıt olarak tek aya yazılıyor.",
                     "d": "E0294"},
                ],
            },
            {
                "baslik": "Borç ödeniyor",
                "dallar": [
                    {"urunler": "Money Manager", "vurgu": True,
                     "metin": "Yalnız kesilmiş dönem düşüyor: Bu Ay 1.000 → 600, Gelecek Ay "
                              "yerinde kalıyor.", "d": "E0249"},
                    {"urunler": "Bluecoins · Wallet · Hesap Defterim",
                     "metin": "Tek parça bakiye düşüyor; ödeme jenerik bir aktarım.",
                     "d": "E0050 · E0438"},
                    {"urunler": "Ölçülen dört üründe de",
                     "metin": "Ödeme gider sayılmıyor — aynı harcama iki kez gidere girmiyor.",
                     "d": "E0405"},
                ],
            },
        ],
        "notlar": [
            "Zincirin tamamı tek üründe koşulabildi. Diğerlerinde adımların bir kısmı ürünün "
            "kavramı olmadığı için değil, o kavram hiç bulunmadığı için boş kalıyor.",
            "Goodbudget bu yolun hiçbir adımında yok: ücretsiz pakette kart hesabı açılamadı.",
        ],
    },

    # ------------------------------------------------------------------ 3.9
    {
        "tur": "cikarim", "no": "3.9", "baslik": "Neden ayrışıyorlar",
        "giris": "Kartta ayrışmanın kaynağı tek bir soruya verilen cevap: kartın bir dönemi var "
                 "mı. Dönemi olan ürün borcu ikiye bölmek, taksidi aylara yazmak ve ödemeyi bir "
                 "döneme saymak zorunda; dönemi olmayan ürün bunların hiçbirini yapmıyor.",
        "mekanizma": [
            {
                "baslik": "Dönem varsa borç tek sayı olmaktan çıkıyor",
                "metin": [
                    "Money Manager kesim günü ve son ödeme gününü bir dönem kutusuna çeviriyor "
                    "ve kartın borcunu Bu Ay ile Gelecek Ay olarak ikiye bölüyor. Taksitli "
                    "harcamada iki taksit iki döneme düştü, kalan dördü borçta hiç görünmedi.",
                    "Wallet ve Hesap Defterim'de dönem yok; borç tek parça. Wallet'ta 6.000 "
                    "tek seferde kart bakiyesine yazıldı, Hesap Defterim'de defterin yürüyen "
                    "dengesine. İkisinde de \"bu ay ne ödemem gerekiyor\" sorusunun ekranda "
                    "karşılığı yok.",
                    "Bluecoins ikisinin arasında: alanlar var, davranış bu koşumda görülmedi.",
                ],
                "dayanak": "E0233, E0247, E0297, E0153, E0048",
            },
            {
                "baslik": "Aynı kart için iki borç sayısı, ikisi de doğru",
                "metin": [
                    "Money Manager'ın Hesaplar ekranı 1.600 derken aynı kartın defteri Ocak "
                    "satırında 5.600 gösteriyor. Aradaki 4.000 tam olarak Ekim, Kasım, Aralık "
                    "ve Ocak taksitlerinin toplamı.",
                    "Yani iki ekran iki farklı soruyu cevaplıyor: Hesaplar \"şu an ve bir "
                    "sonraki dönemde ne borçlusun\", defter \"bugüne kadar yazılmış bütün "
                    "taksitlerle nereye varıyorsun\". İkisi de kendi tanımına göre doğru.",
                    "Sorun sayıların yanlışlığı değil, kapsamın hiçbir ekranda yazmaması. "
                    "Kullanıcı hangi ekrana baktığına göre iki ayrı cevap alıyor ve ikisinin "
                    "neden farklı olduğunu ekrandan anlayamıyor.",
                ],
                "dayanak": "E0414, E0412, E0413, E0410",
            },
            {
                "baslik": "Taksit yalnız bir üründe kendiliğinden aylık gider eğrisine dönüşüyor",
                "metin": [
                    "Aynı 6.000, Money Manager'da altı ayın her birine 1.000 olarak yazıldı ve "
                    "altıncı taksitten sonraki ay boşaldı. Bluecoins'te yalnız ilk 1.000 gider "
                    "oldu; kalan beşi hatırlatıcıda bekliyor ve ancak kullanıcı her birini "
                    "kaydettikçe gidere giriyor. Wallet ve Hesap Defterim'de 6.000'in tamamı tek "
                    "aya yazıldı.",
                    "Sonuç: aynı harcama üç üründe üç ayrı aylık gider eğrisi üretiyor. Bir "
                    "üründe altı ay boyunca kendiliğinden 1.000; birinde kullanıcı her ay "
                    "kaydettiği sürece 1.000, kaydetmediği ay sıfır; birinde bir ay 6.000 sonra "
                    "sıfır.",
                    "Bu fark yalnız görünümü değil, ayın gider toplamıyla alınan her kararı "
                    "etkiliyor.",
                ],
                "dayanak": "E0246, E0407, E0410, E0411, E0037, E0294, kullanıcı kontrolü (Bluecoins)",
            },
            {
                "baslik": "Ödeme dört üründe de gider değil",
                "metin": [
                    "Ödemenin sonucu ölçülebilen dört üründe de aynı: kart ödemesi ayın "
                    "giderine girmedi. Money Manager bunu ekranda açıkça ayırıyor — gider "
                    "satırı ödeme kaynağına göre bölünmüş ve ödenen tutar gider rakamının "
                    "içine değil, parantezine yazılmış.",
                    "Gerekçe dördünde de aynı: harcama kart kullanıldığı gün zaten gider yazıldı. "
                    "Ödemeyi de gider saymak aynı parayı iki kez saymak olurdu.",
                ],
                "dayanak": "E0405, E0050, E0153, E0249, E0438",
            },
        ],
        "kazanc": [
            ("Money Manager",
             "Kartın dönemi var: borç Bu Ay / Gelecek Ay olarak bölünüyor, taksit aylara "
             "dağılıyor ve ödeme yalnız kesilmiş döneme yazılıyor; gider ödeme kaynağına göre "
             "ayrı okunabiliyor",
             "Aynı kart için iki ekran iki farklı borç gösteriyor ve hangisinin hangi dönemi "
             "kapsadığı hiçbir yerde yazmıyor"),
            ("Bluecoins",
             "Taksit formda bir alan ve kalan taksitler hatırlatıcı listesinde görünür "
             "kalıyor; kısmi ödeme net varlığı bozmuyor",
             "Taksitin yalnız ilki gerçek kayıt; kalan beşi kendiliğinden gerçekleşmiyor ve "
             "her biri elle kaydediliyor — kaydedilmeyen taksit gidere hiç girmiyor"),
            ("Wallet",
             "Kart bir hesap olduğu için borç tek bakışta okunuyor; Available Credit gösterimi "
             "ve ödeme günü alanı kart hesabına özgü",
             "Dönem yok ve taksit bölünmüyor: 6.000 tek aya yazılıyor, \"bu ay ne ödemeliyim\" "
             "sorusunun karşılığı bulunmuyor"),
            ("Hesap Defterim",
             "Kart da diğer defterler gibi çalıştığı için öğrenilecek ayrı bir kavram yok; "
             "kısmi ödeme doğal olarak serbest tutarlı",
             "Kesim günü, limit, ekstre ve taksit kavramlarının hiçbiri yok; kartın diğer "
             "hesaplardan farkı yalnız adı"),
            ("Goodbudget",
             "—",
             "Ücretsiz pakette tek hesap açılabiliyor; banka hesabının yanına kart eklenemediği "
             "için kart kullanan biri bu pakette kart borcunu ayrı izleyemiyor"),
        ],
        "soru": [
            "Kart borcu tek sayı mı olmalı, dönemlere bölünmüş mü? Dönem, \"bu ay ne ödemeliyim\" "
            "sorusunu cevaplıyor ama iki ekranın iki farklı sayı göstermesi riskini getiriyor.",
            "Bir sayının hangi dönemi kapsadığı ekranda yazmalı mı? Money Manager'ın 1.600 ile "
            "5.600'ü aynı anda doğru; ayrımı kullanıcı göremiyor.",
            "Taksit bir plan mı, önceden yazılmış kayıtlar mı? Üç ürün üç ayrı cevap veriyor ve "
            "cevap doğrudan aylık gider eğrisini değiştiriyor.",
            "Kart ödemesi hiçbir üründe gider sayılmıyor — bu ortak davranışın istisnası var mı, "
            "yoksa kural mı?",
        ],
    },
]

EKSIKLER = [
    ("Bluecoins", "Kısmi ödeme sonrası dönem raporunun önce/sonrası", "3.6",
     "Koşumdaki rapor karesi ödemenin önce/sonrasını değil, ayın bugünkü hâlini gösteriyor", "Orta"),
    ("Bluecoins", "Kart dönemi ve Hesap Kesim Günü davranışı", "3.1",
     "Kart ayarlarında kesim günü girili bir dönemi geçirmek gerekir", "Orta"),
    ("Wallet", "Kart harcamasının dönem giderine girdiğinin hesap filtreli karesi", "3.5",
     "Şu an aritmetikle kurulu (çıkarım); Spending'de hesap filtresi Is Karti seçilip okunabilir", "Düşük"),
    ("Dört ürün", "Kart borcunun tamamen ödenmesi ve ekstre belgesi", "3.5",
     "Kurulu bakiyeleri bozar; ayrı bir kontrol kartıyla denenmeli", "Önerilmez"),
    ("Goodbudget", "Kart modelinin tamamı", "3.1",
     "Ücretli paket; kart hesabı açılamıyor", "Önerilmez"),
]
