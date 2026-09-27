# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 7 · Raporun neyi saydigi — tek icerik kaynagi."""

NO = 7
BASLIK = "Raporun neyi saydığı"
ANA_SORU = "Rapordaki sayı hangi kayıtları, hangi dönemi ve hangi tanımı içeriyor?"
EN_AZ_KARE = 14

GIRIS = [
    "Önceki bölümler kayıtların nasıl oluştuğunu izledi. Bu bölüm o kayıtların toplandığı "
    "yere bakıyor ve tek bir şeyi sorguluyor: ekrandaki sayı tam olarak neyin toplamı.",
    "Üç ayrı yerde ayrışma çıkıyor. Raporun sorduğu soru, varsayılan dönemi, ve aynı görünen "
    "iki etiketin aynı şeyi saymaması.",
]
GIRMEZ = [
    "Toplamların içine neyin girdiği → Bölüm 1 ve 2",
    "Kart borcunun iki ayrı sayıyla görünmesi → Bölüm 3",
    "Kapsam kırılımı → Bölüm 6",
    "Raporun dosyaya aktarılması → Bölüm 8",
]
KAPSAM = [
    ("Money Manager", ["canli"], "İstatistik, Toplam sekmesi ve filtre paneli kareli."),
    ("Bluecoins", ["canli"], "Net Kazançlar ve net varlık ekranları kareli."),
    ("Wallet", ["canli"], "Cash-flow ve Spending ekranları kareli."),
    ("Goodbudget", ["canli"], "İki rapor ekranı ve varsayılan dönem kareli."),
    ("Hesap Defterim", ["canli"], "Okuma yüzeyi dönem filtreli liste olarak kareli."),
    ("KolayBi", ["kaynak"], "Nakit akış ve gelir/gider raporları destek görselinde."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], None),
]
ACILIS_DAYANAK = (
    "Rozetler kanıt düzeyini gösterir. Bu bölümdeki tutarlar farklı koşum anlarına ait "
    "olabilir; karşılaştırılan şey tutarların kendisi değil, toplamların tanımıdır."
)

SAYFALAR = [
    {"tur": "acilis"},

    # ------------------------------------------------------------------ 7.1
    {
        "tur": "yanyana", "no": "7.1",
        "baslik": "Rapor hangi soruyu soruyor",
        "giris": "Dört ürünün rapor ekranı dört ayrı soruyla açılıyor. Biri doğrudan kullanıcının "
                 "cümlesiyle konuşuyor, biri kategoriye bölüyor, biri dönemleri yan yana koyuyor, "
                 "biri geliri ve harcamayı karşılaştırıyor.",
        "yukseklik": 280,
        "sekiller": [
            {"k": "r-wl", "e": "E0280", "ad": "Wallet", "etiket": "Cash-flow",
             "kirpma": (0, 120, 1080, 1200),
             "satirlar": ["Başlık bir soru: \"Am I spending less than I make?\"",
                          "Net 22.950; Income 25.000; Expenses −2.050.",
                          "Rapor kullanıcının diliyle konuşuyor."]},
            {"k": "r-mm", "e": "E0231", "ad": "Money Manager", "etiket": "İstatistik · Ay",
             "kirpma": (0, 120, 1080, 1400),
             "satirlar": ["Gelir ve Gider iki ayrı sekme.",
                          "Pasta: Diğer %58,5 · Yiyecek %41,5.",
                          "Soru kategoriye bölünmüş harcama."]},
            {"k": "r-bc", "e": "E0023", "ad": "Bluecoins", "etiket": "Net Kazançlar",
             "kirpma": (0, 120, 1080, 1450),
             "satirlar": ["İki dönem yan yana sütun olarak.",
                          "Kategori ve alt kategori satırları.",
                          "Soru dönemler arası karşılaştırma."]},
            {"k": "r-gb", "e": "E0423", "ad": "Goodbudget", "etiket": "Reports",
             "kirpma": (0, 120, 1080, 1340),
             "satirlar": ["İki rapor alt alta: zarf harcaması ve",
                          "gelir-harcama karşılaştırması.",
                          "Net Total tek satırda veriliyor."]},
        ],
        "notlar": [
            "Wallet raporu bir soru cümlesiyle açıyor ve cevabı tek satırda veriyor. Diğer üçü "
            "sayıyı önce gösterip yorumu kullanıcıya bırakıyor.",
            "Hesap Defterim'de ayrı bir rapor ekranı yok: \"Aylık\" seçildiğinde açılan şey "
            "dönem filtresi uygulanmış aynı liste. Adı rapor olsa da içeriği liste.",
        ],
        "sag_notlar": [
            ("Money Manager'da aynı veri üçüncü bir yüzeyde de okunuyor: takvim görünümü günleri "
             "renklendirip gün başına tutar yazıyor.", "E0234"),
            ("Bluecoins'te de takvim günlük gelir, gider ve net ayrımı gösteriyor.", "E0098"),
        ],
    },

    # ------------------------------------------------------------------ 7.2
    {
        "tur": "yanyana", "no": "7.2",
        "baslik": "\"Bu dönem\" her üründe başka bir dönem",
        "giris": "Rapor açıldığında hangi aralığın seçili geldiği küçük bir ayrıntı gibi "
                 "duruyor ama sonucu doğrudan değiştiriyor. Dört üründe dört ayrı varsayılan.",
        "yukseklik": 300,
        "sekiller": [
            {"k": "d-wl", "e": "E0284", "ad": "Wallet", "etiket": "Son 12 hafta",
             "kirpma": (0, 120, 1080, 2260),
             "satirlar": ["Seçili aralık 12W / LAST 12 WEEKS.",
                          "Takvim ayı değil, kayan bir pencere.",
                          "6M ve 1Y ücretli pakette."]},
            {"k": "d-gb", "e": "E0121", "ad": "Goodbudget", "etiket": "Cari takvim ayı",
             "kirpma": (0, 120, 1080, 1340),
             "satirlar": ["Rapor içinde bulunulan ayda açılıyor.",
                          "Dönem seçimi üst bardaki takvim simgesinden."]},
            {"k": "d-mm", "e": "E0250", "ad": "Money Manager", "etiket": "Toplam · ay",
             "kirpma": (0, 120, 1080, 1340),
             "satirlar": ["Dönem bir takvim ayı: 1.08 ~ 31.08.",
                          "Gider ödeme kaynağına göre bölünmüş.",
                          "Altta Excel olarak gönderme girişi."]},
            {"k": "d-hd", "e": "E0148", "ad": "Hesap Defterim", "etiket": "Özet · gün gün",
             "kirpma": (0, 120, 1080, 1000),
             "satirlar": ["Çipler: Herşey / Haftalık / Aylık / Yıllık.",
                          "Günlük çipi yok.",
                          "Gün gün Alındı, Ödendi ve Tasarruf."]},
        ],
        "notlar": [
            ("Wallet'ın raporu takvim ayıyla değil kayan bir pencereyle açılıyor: bu karede "
             "son 12 hafta, Şekil 7.1'de son 30 gün. Aynı veriyle \"bu ay ne harcadım\" "
             "sorusunun cevabı, kullanıcı aralığa bakmazsa bir aydan uzun bir toplam "
             "olabiliyor.", "E0284 · E0280"),
            "Bluecoins'in dönem seçicisi 30 Gün gibi kayan pencereler de sunuyor; aynı ekranda "
            "hem takvim ayı hem gün sayısı seçilebiliyor.",
        ],
        "sag_notlar": [
            ("Hesap Defterim'in \"Aylık\" görünümü ayrı bir rapor değil, dönem filtresi "
             "uygulanmış liste.", "E0142"),
            ("Bluecoins'in dönem ve işlem tipi seçicileri ana ekranın üstünde.", "E0021"),
            ("Hesap Defterim'in Haftalık ve Yıllık çipleri yalnız kendi aralığının hareketlerini "
             "topluyor; öncesi ayrı bir \"Önceki denge\" satırıyla taşınıyor. Hareketsiz bir "
             "haftada Ana Hesap defterinin geliri ve gideri 0, dengesi yine 43.150.", "E0458 · E0447"),
        ],
    },

    # ------------------------------------------------------------------ 7.3
    {
        "tur": "yanyana", "no": "7.3",
        "baslik": "Aynı görünen iki etiket, iki ayrı toplam",
        "giris": "Bu bölümün en sessiz tuzağı. İki ekran neredeyse aynı adı taşıyor ama biri "
                 "dönem içindeki akışı, öteki elde kalan varlığı sayıyor. Hangisinin hangisi "
                 "olduğu ekranda yazmıyor.",
        "yukseklik": 300,
        "sekiller": [
            {"k": "e-bc1", "e": "E0032", "ad": "Bluecoins", "etiket": "Net Kazanç · varlık",
             "kirpma": (0, 800, 1080, 2200),
             "satirlar": ["Benzer iki etiket, iki ayrı toplam.",
                          "Biri dönem akışı, öteki varlık.",
                          "Ayrım ekranda yazılı değil."]},
            {"k": "e-bc2", "e": "E0053", "ad": "Bluecoins", "etiket": "İkisi aynı ekranda",
             "kirpma": (0, 750, 1080, 2100),
             "satirlar": ["Net Kazançlar: dönem akışı 21.350.",
                          "Net Kazanç: Varlıklar 43.850, Cari hesap −500.",
                          "İki blok alt alta duruyor."]},
            {"k": "e-mm1", "e": "E0251", "ad": "Money Manager", "etiket": "Filtre paneli",
             "kirpma": (0, 120, 1080, 1720),
             "satirlar": ["Sütunlar: Gelir / Giden Havale · Gider / Gelen Havale.",
                          "Aktarım gelir ve giderden ayrı kolonlarda.",
                          "Hesap başına dört sayı."]},
            {"k": "e-mm2", "e": "E0405", "ad": "Money Manager", "etiket": "Gider ödeme kaynağına göre",
             "kirpma": (0, 120, 1080, 1530),
             "satirlar": ["Gider (Nakit, Banka Hesapları) ayrı.",
                          "Gider (Kredi Kartı, Ödeme) ayrı ve parantezli.",
                          "Havale kendi satırında, 0,00."]},
        ],
        "notlar": [
            "Money Manager aynı tuzağı tersine çeviriyor: toplamı tek bir sayı olarak "
            "göstermek yerine, neyin dâhil olduğunu satır adlarıyla yazıyor. Aktarım kendi "
            "satırında duruyor ve sıfır olduğu görülüyor.",
            "Bluecoins'te ise iki blok alt alta ve adları birbirine çok yakın. Kullanıcının "
            "hangi bloğa baktığını ad değil, blokta görünen kalemler belli ediyor.",
        ],
        "sag_notlar": [
            ("Money Manager'ın Toplam sekmesi aynı ayrımı Ağustos için de yapıyor: nakit gideri "
             "ayrı, kart gideri ayrı ve ödenen tutar parantezde.", "E0250"),
            ("Bluecoins'in tümünü göster raporu iki tarihi yan yana koyarak VARLIKLAR ve CARİ "
             "HESAP bölümlerini, altındaki grup ve hesaplarla ayrı ayrı veriyor.", "E0055"),
            ("Money Manager'ın filtre paneli toplamı anında yeniden kuruyor: yalnız kart "
             "seçilince gider o kartın harcamasına, 1.000'e iniyor ve payı %45 yazılıyor; "
             "havale yine ayrı bir satırda.", "E0457"),
        ],
    },

    # ------------------------------------------------------------------ 7.4
    {
        "tur": "yanyana", "no": "7.4",
        "baslik": "Raporun kendi içinde tutmadığı bir an",
        "giris": "Goodbudget'ın iki rapor ekranı aynı anda çekildi. Harcama raporu negatif bir "
                 "toplam gösteriyor ve o negatif tutarın içinde gelir var — yani bir gelir kaydı "
                 "harcama raporuna düşmüş.",
        "yukseklik": 320,
        "sekiller": [
            {"k": "g-spend", "e": "E0123", "ad": "Spending by Envelope", "etiket": "Toplam negatif",
             "kirpma": (0, 120, 1080, 1580),
             "satirlar": ["Harcama raporunun toplamı eksi çıkıyor.",
                          "−22.950 = 1.200 + 850 − 25.000.",
                          "25.000'lik gelir bu toplamın içinde."]},
            {"k": "g-inc", "e": "E0124", "ad": "Income vs Spending", "etiket": "Aynı andaki tablo",
             "kirpma": (0, 120, 1080, 1720),
             "satirlar": ["Aynı dönem, ikinci rapor ekranı.",
                          "Grafikteki çubuk ile tablodaki sayı",
                          "görsel olarak örtüşmüyor."]},
        ],
        "notlar": [
            "Aritmetik tutuyor: harcama raporunun toplamı, iki gideri ve bir geliri aynı kovaya "
            "koyduğunda tam olarak −22.950 veriyor. Yani ekran bir hesap hatası yapmıyor; "
            "kaydı yanlış kovaya koyuyor.",
            "Bunun bir ürün kusuru mu yoksa zarf modelinin bir sonucu mu olduğu bu koşumdan "
            "çıkmıyor. Ölçülen şey sonucun kendisi: harcama raporu gelir içeriyor.",
        ],
        "sag_notlar": [
            ("Aynı dönemin Income vs Spending tablosunda da aynı −22.950 duruyor; iki ekran "
             "birbiriyle tutarlı.", "E0124"),
            ("Bölüm 1'de görülen iki defterli yapı bu ürünün raporunu da etkiliyor: rapor "
             "zarf katmanından besleniyor.", "E0423"),
        ],
    },

    # ------------------------------------------------------------------ 7.5
    {
        "tur": "tablo", "no": "7.5", "baslik": "Rapor yüzeyleri, ürün ürün",
        "giris": "Raporun sorduğu soru, varsayılan dönemi, aktarımı nasıl gösterdiği ve "
                 "kategori kırılımı.",
        "sutunlar": [("", 15), ("Money Manager", 17), ("Bluecoins", 17), ("Wallet", 17),
                     ("Goodbudget", 17), ("Hesap Defterim", 17)],
        "boy": 8.4,
        "satirlar": [
            [
                "Ayrı bir rapor ekranı var mı",
                {"t": "Evet — İstatistik ve Toplam sekmeleri", "tur": "canli", "d": "E0231"},
                {"t": "Evet — Net Kazançlar ve net varlık", "tur": "canli", "d": "E0023"},
                {"t": "Evet — Cash-flow ve Spending", "tur": "canli", "d": "E0280"},
                {"t": "Evet — iki ayrı rapor ekranı", "tur": "canli", "d": "E0124"},
                {"t": "Hayır — dönem filtreli liste", "tur": "canli", "d": "E0142"},
            ],
            [
                "Varsayılan dönem",
                {"t": "Takvim ayı", "tur": "canli", "d": "E0250"},
                {"t": "Kayan pencere ve takvim ayı birlikte", "tur": "canli", "d": "E0021"},
                {"t": "Kayan pencere: son 30 gün ya da 12 hafta", "tur": "canli", "d": "E0284 · E0280"},
                {"t": "Cari takvim ayı", "tur": "canli", "d": "E0121"},
                {"t": "Çip seçimi: Herşey / Haftalık / Aylık / Yıllık", "tur": "canli", "d": "E0148"},
            ],
            [
                "Aktarım raporda nasıl görünüyor",
                {"t": "Kendi satırında ve kendi kolonlarında", "tur": "canli", "d": "E0251"},
                {"t": "Gün içinde netleşiyor; toplamda yok", "tur": "canli", "d": "E0022"},
                {"t": "Çift satır; dönem toplamına etkisi yok", "tur": "canli", "d": "E0279"},
                {"t": "Aktarım koşulamadı", "tur": "yok", "d": "E0120"},
                {"t": "Alındı ve Ödendi toplamlarının içinde", "tur": "canli", "d": "E0142"},
            ],
            [
                "Kategori kırılımı",
                {"t": "Pasta ve yüzde listesi", "tur": "canli", "d": "E0231"},
                {"t": "Kategori ve alt kategori satırları", "tur": "canli", "d": "E0023"},
                {"t": "Donut; Categories/Labels geçişi", "tur": "canli", "d": "E0278"},
                {"t": "Zarf başına", "tur": "canli", "d": "E0123"},
                {"t": "Yok", "tur": "canli", "d": "E0142"},
            ],
            [
                "Dikkat gerektiren yer",
                {"t": "Gider ödeme kaynağına göre bölünmüş; okumadan önce satır adı okunmalı", "tur": "canli", "d": "E0405"},
                {"t": "Benzer adlı iki blok: dönem akışı ve varlık", "tur": "canli", "d": "E0032"},
                {"t": "Varsayılan aralık takvim ayı değil", "tur": "canli", "d": "E0284"},
                {"t": "Harcama raporunun toplamı gelir içeriyor", "tur": "canli", "d": "E0123"},
                {"t": "Toplamlar açılış ve aktarım bacaklarını içeriyor", "tur": "canli", "d": "E0142"},
            ],
        ],
        "notlar": [
            "Son satır bu bölümün özeti: beş ürünün beşinde de rapordaki sayıyı doğru okumak "
            "için ekranda yazmayan bir şeyi bilmek gerekiyor.",
        ],
    },

    # ------------------------------------------------------------------ 7.6
    {
        "tur": "soru", "no": "7.6", "baslik": "Kaynakta rapor: nakit akışı ve dönem kovaları",
        "giris": "KolayBi'nin rapor yüzeyi on sekmeye ayrılmış. Nakit akış raporu canlı beş "
                 "üründe bulunmayan bir şey yapıyor: gelecek dönemleri kova kova gösterip "
                 "tahmini dönem sonu bakiyesini yazıyor. Alanların varlığı kanıtlı; davranış "
                 "görülmedi.",
        "dikey": True, "not_genislik": 300,
        "sekiller": [
            {"k": "kb-nakit", "e": "E0216", "etiket": "KolayBi · Nakit Akış Raporu",
             "kirpma": (96, 112, 1305, 700), "genislik": 350,
             "satirlar": ["Güncel Bakiye, Tahsilatlar, Ödemeler üstte.",
                          "Tahmini Dönem Sonu Bakiyesi ayrı bir sayı.",
                          "Tabloda aylık kovalar ve bir Belirsiz kovası."]},
            {"k": "kb-gg", "e": "E0215", "etiket": "KolayBi · Gelir/Gider Raporu",
             "kirpma": (96, 112, 1305, 870), "genislik": 350,
             "satirlar": ["Gelirler ve Giderler satır, dört para birimi kolon.",
                          "Kolonlar arasında Ödeme Yöntemi ve Banka da var.",
                          "Gelirler ve Giderler ayrı alt sekmeler."]},
        ],
        "notlar": [
            "\"Belirsiz\" kovası dikkat çekici: vadesi belli olmayan tahsilat ayrı bir kovada "
            "duruyor ve aylık kovalara dağılmıyor. Canlı ürünlerde vadesi belirsiz para için "
            "böyle bir yer yok.",
            "Gelir/gider raporunun kolonları arasında ödeme yöntemi ve banka bulunuyor; rapor "
            "yalnız tutarı değil, paranın hangi yoldan geçtiğini de taşıyor.",
        ],
        "sag_notlar": [
            ("Rapor yüzeyi on sekmeye bölünmüş ve filtreler arasında cari, proje, etiket, şube "
             "ve vade aralığı var.", "E0213"),
        ],
        "urunler_baslik": "Aynı soruda kaynaktan okunan üç ürün",
        "urunler": [
            ("Paraşüt", "beyan",
             "Kaynak nakit akışı ve yaşlandırma raporlarından söz ediyor; tahsilat kaydedilince "
             "nakit akışı ve yaşlandırma raporunun güncellendiği yazılı. Bölüm 9'da kurulur.",
             "parasut.com kılavuzu"),
            ("Logo İşbaşı", "beyan",
             "Kaynak tarih aralığına göre kategorize raporlar ve görsel grafik vaat ediyor; "
             "rapor içeriği anlatılmıyor. Bölüm 9'da kurulur.", "isbasi.com"),
            ("QuickBooks Solopreneur", "beyan",
             "Kaynağa göre kâr-zarar, nakit akışı geçmişi ve genel bakış panosu kategorize "
             "edilen kayıtlardan türetiliyor; ayrıca üç aylık tahmini vergi hesaplanıyor.",
             "Intuit yardım merkezi"),
        ],
    },

    # ------------------------------------------------------------------ 7.7
    {
        "tur": "akis", "no": "7.7", "baslik": "Sayının rapora gelene kadarki yolu",
        "giris": "Bir kayıt yazıldıktan sonra rapordaki sayıya dönüşene kadar üç eşikten "
                 "geçiyor. Her eşikte ürünler ayrı kararlar vermiş.",
        "adimlar": [
            {
                "baslik": "Kayıt yazılıyor",
                "ortak": "Beş üründe de kayıt bir tutar, bir tarih ve bir sınıflandırma "
                         "taşıyor. Buraya kadar rapor devrede değil.",
            },
            {
                "baslik": "Hangi dönemin içinde sayılıyor",
                "dallar": [
                    {"urunler": "Money Manager · Goodbudget",
                     "metin": "Takvim ayı. Rapor içinde bulunulan ayda açılıyor.", "d": "E0121"},
                    {"urunler": "Wallet", "vurgu": True,
                     "metin": "Kayan bir pencere: son 30 gün ya da son 12 hafta. Takvim ayı "
                              "ayrı bir sayfadan seçiliyor.", "d": "E0284 · E0280 · E0439"},
                    {"urunler": "Bluecoins · Hesap Defterim",
                     "metin": "Kullanıcı seçiyor: kayan pencere ile takvim dönemi aynı ekranda.",
                     "d": "E0148"},
                ],
            },
            {
                "baslik": "Hangi toplama giriyor",
                "dallar": [
                    {"urunler": "Money Manager", "vurgu": True,
                     "metin": "Gider ödeme kaynağına göre bölünüyor ve aktarım kendi satırında "
                              "duruyor; neyin dâhil olduğu satır adında yazılı.", "d": "E0405"},
                    {"urunler": "Bluecoins · Wallet",
                     "metin": "Aktarım netleşiyor, gelir/gider toplamına girmiyor.", "d": "E0279"},
                    {"urunler": "Hesap Defterim", "vurgu": True,
                     "metin": "Her satır toplama giriyor: açılış bakiyesi ve aktarım bacakları "
                              "dâhil.", "d": "E0142"},
                    {"urunler": "Goodbudget", "vurgu": True,
                     "metin": "Bir gelir kaydı harcama raporunun toplamına girmiş; toplam eksi "
                              "çıkıyor.", "d": "E0123"},
                ],
            },
            {
                "baslik": "Ekranda okunuyor",
                "dallar": [
                    {"urunler": "Wallet",
                     "metin": "Sayının yanında soru cümlesi: \"Am I spending less than I make?\"",
                     "d": "E0280"},
                    {"urunler": "Bluecoins", "vurgu": True,
                     "metin": "Benzer adlı iki blok alt alta: biri dönem akışı, öteki varlık. "
                              "Ayrım ekranda yazılı değil.", "d": "E0032"},
                    {"urunler": "Money Manager · Hesap Defterim",
                     "metin": "Sayı önce, yorum kullanıcıda. Money Manager satır adlarıyla "
                              "kapsamı yazıyor.", "d": "E0250"},
                ],
            },
        ],
        "notlar": [
            "İkinci ve üçüncü eşik bu bölümün ayrım noktaları: aynı kayıt farklı dönemlere "
            "düşebiliyor ve aynı adlı toplam farklı şeyleri sayabiliyor.",
            "Dördüncü eşikte tek fark okunabilirlik: hiçbir ürün yanlış hesaplamıyor, ama "
            "hesabın tanımını yazan ürün sayısı az.",
        ],
    },

    # ------------------------------------------------------------------ 7.8
    {
        "tur": "cikarim", "no": "7.8", "baslik": "Neden ayrışıyorlar",
        "giris": "Rapor bir hesap makinesi değil, bir tanım. Ürünler aynı kayıtları farklı "
                 "tanımlarla topluyor ve o tanımı ekranda yazıp yazmamakta da ayrışıyorlar.",
        "mekanizma": [
            {
                "baslik": "Varsayılan dönem sessizce sonucu değiştiriyor",
                "metin": [
                    "Wallet'ın raporu takvim ayıyla değil kayan bir pencereyle açılıyor — "
                    "karelerde son 30 gün ve son 12 hafta; takvim ayı ancak ayrı bir sayfadan "
                    "seçiliyor. Goodbudget ve Money Manager takvim ayıyla açılıyor. Aynı veriyle "
                    "\"bu ay ne harcadım\" sorusunun cevabı, kullanıcı aralığa bakmazsa üründen "
                    "ürüne değişiyor.",
                    "Fark ekranda görünüyor ama okunmayı bekliyor: aralık bir çip ya da bir "
                    "başlık. Sayıya bakan kullanıcı aralığa bakmayabilir.",
                ],
                "dayanak": "E0284, E0280, E0439, E0438, E0121, E0250, E0148",
            },
            {
                "baslik": "Aynı adlı iki toplam aynı şeyi saymıyor",
                "metin": [
                    "Bluecoins'te iki blok alt alta duruyor ve adları birbirine çok yakın. Biri "
                    "dönem içindeki akışı — gelir eksi gider — öteki elde kalan varlığı sayıyor. "
                    "Koşumda biri 21.350, öteki 43.350 okundu: Varlıklar 43.850 ile \"Cari hesap\" −500'ün toplamı. \"Cari hesap\" satırı "
                    "Bluecoins'in cariyi ve kartı birlikte topladığı bölüm; o anda içindeki tek "
                    "tutar kart borcuydu.",
                    "İkisi de doğru ve ikisi de gerekli; sorun adların ayırt edici olmaması. "
                    "Kullanıcı hangi bloğa baktığını addan değil, bloğun içindeki kalemlerden "
                    "anlıyor.",
                    "Bu, kart bölümündeki iki borç sayısı ve borç bölümündeki ters işaretle aynı "
                    "desen: ürün iki farklı soruyu cevaplıyor, hangisinin hangisi olduğunu "
                    "yazmıyor.",
                ],
                "dayanak": "E0032, E0053, E0052",
            },
            {
                "baslik": "Money Manager tanımı ekrana yazmayı seçmiş",
                "metin": [
                    "Aynı tuzağın tersi de var. Money Manager toplamı tek bir sayı olarak "
                    "göstermek yerine, gideri ödeme kaynağına göre bölüyor: nakit ve banka "
                    "gideri ayrı satır, kart gideri ayrı satır ve ödenen tutar parantezde. "
                    "Aktarım da kendi satırında duruyor ve sıfır olduğu okunuyor.",
                    "Filtre panelinde aynı ilke daha da açık: sütunlar \"Gelir / Giden Havale\" "
                    "ve \"Gider / Gelen Havale\" olarak adlandırılmış, yani aktarımın hangi "
                    "tarafta sayıldığı adın içinde.",
                    "Bedeli okuma yükü: kullanıcı tek bir sayı yerine dört satır okuyor.",
                ],
                "dayanak": "E0405, E0251, E0250",
            },
            {
                "baslik": "Bir raporda gelir, harcama toplamının içine girmiş",
                "metin": [
                    "Goodbudget'ın harcama raporu negatif bir toplam gösteriyor ve aritmetik "
                    "bunun nedenini veriyor: iki gider ile bir gelir aynı kovada toplanınca "
                    "−22.950 çıkıyor. Aynı dönemin ikinci rapor ekranı da aynı sayıyı "
                    "gösteriyor, yani iki ekran kendi aralarında tutarlı.",
                    "Ürün hesap hatası yapmıyor; kaydı yanlış kovaya koyuyor. Bunun bir kusur mu "
                    "yoksa zarf modelinin bir sonucu mu olduğu koşumdan çıkmıyor — ölçülen şey "
                    "sonucun kendisi.",
                ],
                "dayanak": "E0123, E0124",
            },
        ],
        "kazanc": [
            ("Money Manager",
             "Toplamın tanımını satır adlarına yazıyor: gider ödeme kaynağına göre bölünmüş, "
             "aktarım kendi satırında ve sıfır olduğu görülüyor",
             "Tek bakışta okunan bir sayı yok; kullanıcı dört satır okumak zorunda"),
            ("Bluecoins",
             "Dönem akışı ile varlığı ayrı ayrı veriyor ve iki dönemi yan yana koyup "
             "karşılaştırma yapıyor",
             "İki bloğun adı birbirine çok yakın; hangisinin akış hangisinin varlık olduğu "
             "ekranda yazmıyor"),
            ("Wallet",
             "Rapor kullanıcının cümlesiyle açılıyor ve cevabı tek satırda veriyor; kategori "
             "ile etiket iki ayrı kırılım olarak okunabiliyor",
             "Rapor aralığı takvim ayı değil kayan bir pencere; aralığa bakmayan kullanıcı "
             "son 30 günü ya da 12 haftayı \"bu ay\" sanabilir"),
            ("Goodbudget",
             "Gelir ile harcamayı tek grafikte karşılaştırıyor ve zarf başına kırılım veriyor",
             "Harcama raporunun toplamı bir gelir kaydını içeriyor ve eksi çıkıyor"),
            ("Hesap Defterim",
             "Gün gün Alındı, Ödendi ve Tasarruf veren bir özet var",
             "Ayrı bir rapor ekranı yok; toplamlar açılış ve aktarım bacaklarını içeriyor ve "
             "kategori kırılımı hiç bulunmuyor"),
        ],
        "soru": [
            "Raporun varsayılan dönemi takvim ayı mı olmalı, kayan bir pencere mi? İki seçenek "
            "aynı veriyle iki ayrı sayı veriyor.",
            "Bir toplamın neyi içerdiği ekranda yazmalı mı? Money Manager yazıyor ve okuma yükü "
            "artıyor; diğerleri yazmıyor ve yanlış okuma riski kalıyor.",
            "Akış ile varlık aynı ekranda durabilir mi? Duruyorsa adları nasıl ayrışmalı?",
            "Rapor ekranları birbiriyle tutarlı olmalı mı, yoksa her ekran kendi tanımını mı "
            "taşımalı? Bir üründe iki ekran tutarlı ama ikisi de aynı kovalama sorununu "
            "taşıyor.",
        ],
    },
]

EKSIKLER = [
    ("Wallet", "Son 12 hafta ile takvim ayının farklı sayı verdiği bir anın karesi", "7.2",
     "Koşumda kayıtların hepsi son 30 gündeydi; takvim ayı ile kayan pencere aynı toplamı verdi", "Düşük"),
    ("Goodbudget", "Harcama raporundaki gelir kaydının nedeni", "7.4",
     "Sonraki gelir kaydı harcama raporuna girmedi (Spending 0); 25.000'lik gelirin neden girdiği açık", "Orta"),
    ("Bluecoins", "Dönem seçicinin iki bloğa etkisi", "7.3",
     "Blokların adı kesinleşti (Net Kazançlar · Net Kazanç); dönem değiştirip ikisini birlikte okumak kaldı", "Düşük"),
    ("KolayBi", "Raporların gerçek veriyle davranışı", "7.6",
     "Canlı erişim yok; destek görselleri boş demo verisiyle", "Önerilmez"),
]
