# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 5 · Zaman: tekrar, plan ve butce — tek icerik kaynagi."""

NO = 5
BASLIK = "Zaman: tekrar, plan ve bütçe"
ANA_SORU = "Gelecekte olacak bir ödeme ne zaman gerçek bir kayda dönüşüyor ve o ana kadar hangi toplamda görünüyor?"
EN_AZ_KARE = 18

GIRIS = [
    "Tekrarlayan bir ödeme kurulduğunda ortada iki şey oluyor: bir tanım ve o tanımın "
    "üreteceği kayıtlar. Ürünler bu ikisinin arasına koydukları kapıyla ayrışıyor — kapı "
    "otomatik mi açılıyor, kullanıcı onayı mı istiyor, yoksa hiç kapı yok mu.",
    "Bölüm üç soruyu izliyor: plan nasıl kuruluyor, gerçek kayda hangi anda dönüşüyor ve "
    "dönüşene kadar ayın toplamında görünüyor mu. Sonunda aynı zaman ekseninin ikinci "
    "yüzü var: bütçe.",
]
GIRMEZ = [
    "Taksitin aylara dağılması → Bölüm 3",
    "Vadeli alacağın takibi → Bölüm 4",
    "Raporun dönem seçimi → Bölüm 7",
    "Plan ekranlarının görsel düzeni → Belge 1 §7",
]
KAPSAM = [
    ("Money Manager", ["canli"], "Plan kurulumu, gerçekleşme ve gelecek önizlemesi kareli."),
    ("Wallet", ["canli"], "Onay kapısı, otomatik/onaylı seçimi ve bütçe kareli."),
    ("Bluecoins", ["canli"], "Hatırlatıcı listesi ve kayda çevirme adımı kareli."),
    ("Goodbudget", ["canli"], "Tekrar sıklığı kareli; bütçe zarfın kendisi."),
    ("Hesap Defterim", ["yok"], "Tekrar, plan ve hatırlatıcı beş yüzeyde arandı, bulunamadı."),
    ("KolayBi", ["kaynak"], "Tekrarlı maaş formu destek görselinde."),
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

    # ------------------------------------------------------------------ 5.1
    {
        "tur": "yanyana", "no": "5.1",
        "baslik": "Plan nasıl kuruluyor",
        "giris": "Üç üründe plan kaydın bir özelliği: aynı forma bir tekrar rozeti ekleniyor. "
                 "Bir üründe ise plan ayrı bir nesne ve kendi formu var. Sıklık listeleri de "
                 "aynı zenginlikte değil.",
        "yukseklik": 310,
        "sekiller": [
            {"k": "p-mm1", "e": "E0241", "ad": "Money Manager", "etiket": "Aylık rozetli gider formu",
             "kirpma": (0, 120, 1080, 1315),
             "satirlar": ["Sıradan gider formu; üstte \"Aylık\" rozeti.",
                          "600, Ana Hesap, \"Bulut yazilim aboneligi\".",
                          "Plan kaydın bir özelliği, ayrı nesne değil."]},
            {"k": "p-mm2", "e": "E0238", "ad": "Money Manager", "etiket": "Tekrarlama seçenekleri",
             "kirpma": (0, 120, 1080, 1950),
             "satirlar": ["On dört seçenek: Günlük'ten Yıllık'a.",
                          "Haftanın günleri, Haftasonu, Ayın Son Günü de var."]},
            {"k": "p-bc", "e": "E0039", "ad": "Bluecoins", "etiket": "Planlı işlem sayfası",
             "kirpma": (0, 120, 1080, 2300),
             "satirlar": ["Plan işlem formundan açılıyor.",
                          "Otomatik kol seçenek olarak duruyor.",
                          "Çalışması bu koşumda denenmedi."]},
            {"k": "p-wl", "e": "E0288", "ad": "Wallet", "etiket": "Add Planned payment",
             "kirpma": (0, 120, 1080, 2200),
             "satirlar": ["Plan ayrı bir nesne; kendi formu var.",
                          "Income / Expense / Transfer seçimi.",
                          "Frequency: Recurrent payment."]},
        ],
        "notlar": [
            "Hesap Defterim'de tekrar, plan, hatırlatıcı veya abonelik kavramı beş ayrı yüzeyde "
            "arandı — işlem formu, formun menüsü, ana ekran menüsü, çekmecenin on yedi kalemi "
            "ve ayarların yirmi bir kalemi — ve hiçbirinde bulunamadı. Kullanıcı her ay eliyle "
            "giriyor.",
            "Goodbudget'ta tekrar işlem formunun bir kutusu: \"Schedule this…\" işaretlenince "
            "sıklık açılıyor. Seçenekler arasında Once, Weekly ve Every 2 Weeks görüldü.",
        ],
        "sag_notlar": [
            ("Wallet'ta planın tarih seçicisinde geçmiş günler soluk; plan ileri tarihli "
             "kuruluyor.", "E0287"),
            ("Goodbudget'ta kurulan plan Every 2 Weeks sıklığıyla iki tarihe düştü.", "E0126"),
        ],
    },

    # ------------------------------------------------------------------ 5.2
    {
        "tur": "yanyana", "no": "5.2",
        "baslik": "Plan gerçek kayda hangi anda dönüşüyor",
        "giris": "Bölümün ayrım noktası burası. Bir üründe kapı tek dokunuşluk bir onay, ve "
                 "ürün o kapının hep açık mı kalacağını kullanıcıya soruyor. Bir üründe her vade "
                 "elle kayda çevriliyor ve hangi tarihin yazılacağı soruluyor. Bir üründe kapı "
                 "hiç yok: vade gelince kayıt kendiliğinden oluşuyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "g-wl1", "e": "E0289", "ad": "Wallet", "etiket": "Bekleyen örnek: Due today",
             "kirpma": (0, 120, 1080, 1315),
             "satirlar": ["Plan detayında turuncu \"Due today\".",
                          "Tek dokunuşluk Confirm düğmesi.",
                          "Gerçekleşme kullanıcının onayıyla."]},
            {"k": "g-wl2", "e": "E0291", "ad": "Wallet", "etiket": "Kapı hep açık mı kalsın",
             "kirpma": (0, 120, 1080, 1765),
             "satirlar": ["\"…create transactions automatically…?\"",
                          "Yes (Recommended) önceden seçili.",
                          "No: \"will wait for your approving.\""]},
            {"k": "g-bc", "e": "E0043", "ad": "Bluecoins", "etiket": "Hangi tarihe yazılsın",
             "kirpma": (0, 1560, 1080, 2400),
             "satirlar": ["Kayda çevirirken iki tarih seçeneği.",
                          "Bugün mü, planlanan gün mü.",
                          "Hangisine basıldığı karede görünmüyor."]},
            {"k": "g-wl3", "e": "E0318", "ad": "Wallet", "etiket": "Kapı sonradan değişebiliyor",
             "kirpma": (0, 120, 1080, 1765),
             "satirlar": ["Aynı soru yeniden açılıyor.",
                          "Bu kez No önceden seçili.",
                          "Seçilen mod saklanıyor."]},
            {"k": "g-mm", "e": "E0430", "ad": "Money Manager", "etiket": "Kurulumda tek soru",
             "kirpma": (0, 120, 1080, 1515),
             "satirlar": ["\"Tarihte tekrar eden işlemler uygulanır.\"",
                          "Tekrarlı kaydedilsin mi: Hayır / Evet.",
                          "Soru kurulumda, vadede değil."]},
        ],
        "notlar": [
            "Wallet kapının davranışını kullanıcıya soruyor ve iki seçeneğin sonucunu da "
            "yazıyor: otomatik kolda gelecek ödemeler kendiliğinden kayda dönüşüyor, diğerinde "
            "onay bekliyor. Varsayılan otomatik tarafta.",
            ("Money Manager'da kapı yok: tekrarlı kayıt kurulurken ürün bir kez soruyor ve "
             "sonucunu aynı cümlede yazıyor; vadesi gelen örnek onay beklemeden gerçek kayıt "
             "oluyor ve ayın giderine giriyor. Ne zaman uygulanacağı bir ayar: tarihinde ya da "
             "her ayın ilk günü.", "E0430 · E0429 · E0243"),
        ],
        "sag_notlar": [
            ("Bekleyen bir örnek ertelenebiliyor ya da atılabiliyor: plan menüsünde Postpone ve Dismiss girişleri var; sonuçları denenmedi.", "E0400"),
            ("Wallet'ta onay sonrası örnek \"Paid Today\" olarak geçmişe düşüyor ve sıradaki "
             "vade 30 gün sonrasına kayıyor.", "E0292"),
            ("Bluecoins'te tekrar ve taksitler hatırlatıcı listesinde bekliyor ve kendiliğinden "
             "gerçekleşmiyor; her biri Kaydet ile elle kayda dönüşüyor. Bu bir kullanıcı "
             "kontrolüyle kayıtlı. Planlı işlem sayfasındaki otomatik kolun açık olduğu bir plan "
             "izlenmedi.", "kullanıcı kontrolü · E0039 · E0047"),
        ],
    },

    # ------------------------------------------------------------------ 5.3
    {
        "tur": "yanyana", "no": "5.3",
        "baslik": "Gerçekleşmemiş plan hangi toplamda görünüyor",
        "giris": "Gelecekteki bir ödeme ayın giderine girmemeli — ama görünmemeli de değil. "
                 "Dört ürün bu dengeyi dört ayrı yerde kuruyor: ayrı bir bölüm, ayrı bir liste, "
                 "ayrı bir ekran veya ortak bir hatırlatıcı listesi.",
        "yukseklik": 290,
        "sekiller": [
            {"k": "s-mm1", "e": "E0244", "ad": "Money Manager", "etiket": "Gelecek ay önizlemesi",
             "kirpma": (0, 120, 1080, 1315),
             "satirlar": ["Ekim toplamı 0 / 0 / 0 — \"Veri yok\".",
                          "Üstte ayrı \"Tekrarlama\" satırı −600.",
                          "Plan görünür ama toplama girmiyor."]},
            {"k": "s-mm2", "e": "E0416", "ad": "Money Manager", "etiket": "Tekrarlayan İşlemler listesi",
             "kirpma": (0, 120, 1080, 1315),
             "satirlar": ["Ayarlar altında planların kendi listesi.",
                          "Sıradaki vade, sıklık, hesap ve kategori.",
                          "Taksit planı bu listede yok."]},
            {"k": "s-wl", "e": "E0306", "ad": "Wallet", "etiket": "Planned payments",
             "kirpma": (0, 120, 1080, 2260),
             "satirlar": ["Planlar kendi ekranında listeleniyor.",
                          "Yalnız sıradaki vade görünüyor.",
                          "All / Income / Expense / Transfer filtresi."]},
            {"k": "s-bc", "e": "E0047", "ad": "Bluecoins", "etiket": "Hatırlatıcı listesi",
             "kirpma": (0, 120, 1080, 1315),
             "satirlar": ["Üç tür aynı listede:",
                          "bağımsız hatırlatıcı, tekrar ve taksit.",
                          "Gerçekleşen kayıt listeden çıkıyor."]},
        ],
        "notlar": [
            "Dördü de aynı ilkede buluşuyor: gerçekleşmemiş plan ayın gelir/gider toplamına "
            "girmiyor. Ayrışan şey planın nerede görüneceği — Money Manager aynı ekranın üst "
            "bölümünde, Wallet ve Money Manager ayrıca kendi listelerinde, Bluecoins bütün plan "
            "türlerini tek bir hatırlatıcı listesinde topluyor.",
            "Bluecoins'in ortak listesi üç ayrı kavramı yan yana koyuyor: bir kez olacak "
            "hatırlatıcı, tekrarlayan plan ve taksit. Üçünün tek listede durması ürünün bunları "
            "aynı şey saydığı anlamına gelmiyor; yalnız aynı yerde gösteriyor.",
        ],
        "sag_notlar": [
            ("Money Manager'ın \"Tekrar ne zaman uygulanır?\" ayarı iki değer taşıyor: Tarihte "
             "ve Her ayın ilk günü. Kayıt başına onay seçeneği yok.", "E0429 · E0416"),
            ("Bluecoins'te bağımsız bir hatırlatıcı \"Bir Defa\" sıklığıyla da kurulabiliyor.",
             "E0046"),
        ],
    },

    # ------------------------------------------------------------------ 5.4
    {
        "tur": "tablo", "no": "5.4", "baslik": "Plan ve gerçekleşme, ürün ürün",
        "giris": "Aynı abonelik planı beş üründe kuruldu ya da kurulamadı. Tabloda planın "
                 "tanımı, kapısı ve toplamlardaki yeri.",
        "sutunlar": [("", 15), ("Money Manager", 17), ("Wallet", 17), ("Bluecoins", 17),
                     ("Goodbudget", 17), ("Hesap Defterim", 17)],
        "boy": 8.4,
        "satirlar": [
            [
                "Plan nerede tanımlanıyor",
                {"t": "Kaydın bir özelliği; forma rozet ekleniyor", "tur": "canli", "d": "E0241"},
                {"t": "Ayrı bir nesne; kendi formu ve ekranı", "tur": "canli", "d": "E0288"},
                {"t": "İşlem formundan açılan planlı işlem sayfası", "tur": "canli", "d": "E0039"},
                {"t": "İşlem formunda Schedule this… kutusu", "tur": "canli", "d": "E0133"},
                {"t": "Beş yüzeyde arandı, bulunamadı", "tur": "yok", "d": "E0171"},
            ],
            [
                "Sıklık seçenekleri",
                {"t": "On dört seçenek; Ayın Son Günü dâhil", "tur": "canli", "d": "E0238"},
                {"t": "Every 1 month koşuldu; liste açılmadı", "tur": "canli", "d": "E0288"},
                {"t": "Aylık koşuldu; tam liste görülmedi", "tur": "canli", "d": "E0039"},
                {"t": "Once, Weekly, Every 2 Weeks görüldü", "tur": "canli", "d": "E0133"},
                {"t": "Kavram yok", "tur": "yok", "d": "E0171"},
            ],
            [
                "Gerçekleşme kapısı",
                {"t": "Kapı yok — vade gelince kendiliğinden kayıt; zamanı bir ayar", "tur": "canli", "d": "E0430 · E0429"},
                {"t": "Tek dokunuşluk Confirm; kapının hep açık kalması sorularak seçiliyor", "tur": "canli", "d": "E0291"},
                {"t": "Her vade elle kaydediliyor; kaydederken tarih soruluyor", "tur": "kosum", "d": "E0043 · kullanıcı kontrolü"},
                {"t": "Ölçülmedi", "tur": "yok", "d": "E0126"},
                {"t": "Kavram yok", "tur": "yok", "d": "E0171"},
            ],
            [
                "Gerçekleşmemiş plan toplamda mı",
                {"t": "Hayır — ayrı bölümde önizleme", "tur": "canli", "d": "E0244"},
                {"t": "Hayır — ayrı ekranda liste", "tur": "canli", "d": "E0306"},
                {"t": "Hayır — hatırlatıcı listesinde", "tur": "canli", "d": "E0047"},
                {"t": "Ölçülmedi", "tur": "yok", "d": "E0126"},
                {"t": "Kavram yok", "tur": "yok", "d": "E0171"},
            ],
            [
                "Bütçe var mı",
                {"t": "Evet — kategori başına; kurulu tek bütçe Yiyecek 1.400", "tur": "canli", "d": "E0431"},
                {"t": "Evet — dönemli bütçe, aşım uyarısı ve tahmin", "tur": "canli", "d": "E0301"},
                {"t": "Bütçe Özeti var; hiçbir kategoriye bütçe kurulmamış", "tur": "canli", "d": "E0442"},
                {"t": "Bütçe zarfın kendisi", "tur": "canli", "d": "E0417"},
                {"t": "Bulunamadı", "tur": "yok", "d": "E0171"},
            ],
        ],
        "notlar": [
            "Gerçekleşmemiş planın ayın toplamına girmemesi ölçülebilen üç üründe de aynı. "
            "Ayrışan şey planın nerede durduğu, sayılıp sayılmadığı değil.",
        ],
    },

    # ------------------------------------------------------------------ 5.5
    {
        "tur": "yanyana", "no": "5.5",
        "baslik": "Bütçe: aynı zaman ekseninin ikinci yüzü",
        "giris": "Plan geleceğe yazılmış bir ödeme; bütçe ise geleceğe konmuş bir sınır. İki "
                 "üründe bütçe dönemli bir üst sınır, birinde paranın kendisi zaten zarflara "
                 "bölünmüş durumda.",
        "yukseklik": 300,
        "sekiller": [
            {"k": "bu-wl1", "e": "E0301", "ad": "Wallet", "etiket": "Bütçe aşıldı",
             "kirpma": (0, 120, 1080, 2280),
             "satirlar": ["Aylık bütçe 5.000; kalan −1.600.",
                          "Kırmızı çubuk ve Over Budget etiketi.",
                          "Ayrıca aşım uyarısı çıkıyor."]},
            {"k": "bu-wl2", "e": "E0302", "ad": "Wallet", "etiket": "Bütçe detayı ve tahmin",
             "kirpma": (0, 120, 1080, 1720),
             "satirlar": ["Harcanan 6.600; günlük ortalama 660.",
                          "Forecasted Spend 6.600 / 30 gün.",
                          "Geçen döneme göre +%222."]},
            {"k": "bu-mm", "e": "E0405", "ad": "Money Manager", "etiket": "Toplam sekmesi · bütçe bloğu",
             "kirpma": (0, 120, 1080, 1315),
             "satirlar": ["Bütçe bloğu Toplam sekmesinin içinde.",
                          "Kurulu bütçe yalnız Yiyecek 1.400.",
                          "Yiyecek'te harcama yok: %0."]},
            {"k": "bu-gb", "e": "E0417", "ad": "Goodbudget", "etiket": "Zarflar",
             "kirpma": (0, 120, 1080, 1245),
             "satirlar": ["Her zarfın altında bütçe tutarı.",
                          "Market 23.784 / bütçe 850.",
                          "Bütçe ayrı bir ekran değil, paranın kendisi."]},
        ],
        "notlar": [
            "Wallet bütçeyi bir tahmin motoruna bağlamış: harcanan tutarın yanında günlük "
            "ortalama, dönem sonu tahmini ve önceki dönemle karşılaştırma duruyor. Bütçe yalnız "
            "bir sınır değil, bir gidişat göstergesi.",
            "Goodbudget'ta bütçe ile bakiye aynı şey: zarfın içindeki para hem harcanabilir "
            "tutar hem bütçe sınırı. Bölüm 1'de görülen iki defterli yapının kaynağı da bu.",
        ],
        "sag_notlar": [
            ("Money Manager'ın bütçesi kurulu: kategori başına, bir varsayılan tutar ve ay ay "
             "değerle; ekran değişikliğin önümüzdeki aydan geçerli olduğunu söylüyor. %0'ın "
             "nedeni ayın bütün giderinin Diğer'de olması, bütçeli tek kategori Yiyecek'te "
             "harcama olmaması.", "E0431 · E0432 · E0404"),
            ("Bluecoins'in Bütçe Özeti hiçbir kategoriye bütçe kurulmadığı için ayın bütün "
             "giderini tek dilimde, Others olarak gösteriyor; bütçe sütunu 0.", "E0442"),
        ],
    },

    # ------------------------------------------------------------------ 5.6
    {
        "tur": "soru", "no": "5.6", "baslik": "Kaynakta tekrar: mevcut kaydı tekrarlıya çevirmek",
        "giris": "KolayBi'de tekrar, sıfırdan kurulan bir plan değil; var olan bir kaydın "
                 "menüsünden çıkan bir dönüştürme. Canlı ürünlerde plan kayıttan önce "
                 "kuruluyor, burada kayıttan sonra.",
        "yukseklik": 330, "not_genislik": 260,
        "sekiller": [
            {"k": "kb-tekrar", "e": "E0205", "etiket": "KolayBi · Tekrarlı Maaş Oluştur",
             "kirpma": (96, 112, 1305, 865), "genislik": 420,
             "satirlar": ["Arkada İşlemler menüsünde \"Tekrarlı Maaşa Dönüştür\" vurgulu.",
                          "Oluşturma Periyodu ve Maaş Oluşturma Tekrar Sayısı zorunlu.",
                          "Maaş Ödeme Tarihi: Belirsiz / Belirli."]},
        ],
        "notlar": [
            "Tekrar sayısı burada bir alan: plan sonsuza kadar sürmüyor, kaç kez oluşacağı "
            "baştan yazılıyor. Canlı dört üründe böyle bir alan görülmedi.",
            "Aynı dönüştürme genel gider tarafında da var: gider detayının İşlemler menüsünde "
            "\"Tekrarlı Genel Gidere Dönüştür\" duruyor. Tekrar, kayıt türünden bağımsız bir "
            "eylem olarak kurgulanmış.",
        ],
        "sag_notlar": [
            ("Notlar sayfasında ayrıca hatırlatıcısı olan not kavramı var: başlık, not, "
             "hatırlatıcı aktif/pasif ve görünürlük.", "E0212"),
        ],
        "urunler_baslik": "Aynı soruda kaynaktan okunan üç ürün",
        "urunler": [
            ("Paraşüt", "beyan",
             "Kaynak tekrarlayan gideri ve tekrarlayan faturayı iki ayrı akış sayıyor; abonelik "
             "faturalaması için otomatik oluşturma anlatılıyor. Maaş ve prim ise kendi başına "
             "otomatik tekrarlayan bir gider türü. Bölüm 9'da kurulur.", "parasut.com kılavuzu"),
            ("Logo İşbaşı", "beyan",
             "Kaynakta tekrarlayan kayıt ayrı bir başlık olarak anlatılmıyor; vurgu fatura ve "
             "tahsilat takvimine veriliyor. Bölüm 9'da kurulur.", "isbasi.com"),
            ("QuickBooks Solopreneur", "beyan",
             "Kaynağa göre tekrar bir plan değil, bir kural: en çok otuz kural tanımlanıyor ve "
             "kurallar inen benzer işlemleri otomatik sınıflıyor. Kural kayıt üretmiyor, inen "
             "kaydı etiketliyor. Bölüm 6'da kurulur.", "Intuit yardım merkezi"),
        ],
    },

    # ------------------------------------------------------------------ 5.7
    {
        "tur": "akis", "no": "5.7", "baslik": "Planın yolu ve ayrıldığı noktalar",
        "giris": "Plan kurulduğu andan ayın toplamına girdiği ana kadar dört adım. Asıl "
                 "çatallanma üçüncü adımda: tanım ile kayıt arasındaki kapı.",
        "adimlar": [
            {
                "baslik": "Plan tanımlanıyor",
                "dallar": [
                    {"urunler": "Money Manager · Bluecoins · Goodbudget",
                     "metin": "Plan kaydın bir özelliği: aynı forma bir tekrar rozeti ya da "
                              "kutusu ekleniyor.", "d": "E0241"},
                    {"urunler": "Wallet",
                     "metin": "Plan ayrı bir nesne; kendi formu, kendi ekranı ve kendi "
                              "listesi var.", "d": "E0288"},
                    {"urunler": "Hesap Defterim", "vurgu": True,
                     "metin": "Kavram yok. Beş ayrı yüzey tarandı; tekrar, plan veya "
                              "hatırlatıcı bulunamadı.", "d": "E0171"},
                ],
            },
            {
                "baslik": "Vade geliyor",
                "ortak": "Ölçülebilen üç üründe de gerçekleşmemiş plan ayın gelir/gider "
                         "toplamına girmiyor; ayrı bir bölümde, ayrı bir listede ya da "
                         "hatırlatıcı listesinde bekliyor.",
            },
            {
                "baslik": "Kayda dönüşüyor",
                "dallar": [
                    {"urunler": "Wallet", "vurgu": True,
                     "metin": "Tek dokunuşluk Confirm. Ürün ayrıca kapının hep açık mı "
                              "kalacağını soruyor: otomatik kol mu, onay kolu mu.", "d": "E0291"},
                    {"urunler": "Bluecoins", "vurgu": True,
                     "metin": "Her vade elle kaydediliyor; kaydederken hangi tarihin yazılacağı "
                              "soruluyor: bugün mü, planlanan gün mü.",
                     "d": "E0043 · kullanıcı kontrolü"},
                    {"urunler": "Money Manager", "vurgu": True,
                     "metin": "Kapı yok. Soru yalnız kurulumda soruluyor; vade gelince kayıt "
                              "kendiliğinden oluşuyor.", "d": "E0430 · E0243"},
                ],
            },
            {
                "baslik": "Bütçeyle karşılaşıyor",
                "dallar": [
                    {"urunler": "Wallet",
                     "metin": "Dönemli bütçe: aşım uyarısı, günlük ortalama ve dönem sonu "
                              "tahmini birlikte geliyor.", "d": "E0302"},
                    {"urunler": "Goodbudget", "vurgu": True,
                     "metin": "Bütçe ayrı bir sınır değil, paranın kendisi: zarfın içindeki "
                              "tutar hem harcanabilir para hem sınır.", "d": "E0417"},
                    {"urunler": "Money Manager",
                     "metin": "Kategori başına bütçe, Toplam sekmesinin içinde; bütçeli "
                              "kategoride harcama olmayınca ilerleme %0.", "d": "E0405 · E0431"},
                ],
            },
        ],
        "notlar": [
            "Üçüncü adımdaki kapı bu bölümün ayrım noktası ve üç ürün üç uçta duruyor: biri "
            "kapının hep açık kalıp kalmayacağını kullanıcıya soruyor, biri her vadeyi "
            "kullanıcıya bırakıyor, biri kapıyı hiç kurmuyor.",
            "Dördüncü adımda Goodbudget yolu tamamen kısaltıyor: zarf hem plan hem bütçe hem "
            "bakiye olduğu için ayrı bir bütçe kavramına ihtiyaç duymuyor.",
        ],
    },

    # ------------------------------------------------------------------ 5.8
    {
        "tur": "cikarim", "no": "5.8", "baslik": "Neden ayrışıyorlar",
        "giris": "Tekrarlayan bir ödemede iki nesne var: tanım ve kayıt. Ürünler bu ikisinin "
                 "arasına koydukları kapıyla, ve o kapının kime ait olduğuyla ayrışıyor.",
        "mekanizma": [
            {
                "baslik": "Kapının kime ait olduğu ürünün en açık tercihi",
                "metin": [
                    "Wallet kapıyı kullanıcıya veriyor ve iki kez soruyor: önce her vadede tek "
                    "dokunuşluk bir Confirm, sonra \"bu kapı hep açık mı kalsın\" sorusu. İki "
                    "seçeneğin sonucu da ekranda yazılı — otomatik kolda gelecek ödemeler "
                    "kendiliğinden kayda dönüşüyor, diğerinde onay bekliyor.",
                    "Bluecoins kapıyı her vadede kullanıcıya bırakıyor: tekrar ve taksitler "
                    "hatırlatıcı listesinde bekliyor, kendiliğinden gerçekleşmiyor ve Kaydet'e "
                    "basılınca hangi tarihin yazılacağını soruyor. Bugün mü, planlanan gün mü — "
                    "aynı kayıt iki ayrı aya düşebilir ve bu seçim ayın toplamını değiştirir.",
                    "Money Manager kapıyı hiç kurmuyor: kurulumda bir kez \"Tarihte tekrar eden "
                    "işlemler uygulanır\" diyor ve vadesi gelen örnek onaysız kayda dönüşüyor. "
                    "Kullanıcıya kalan tek seçim zamanlama: tarihinde ya da ayın ilk günü.",
                    "Üç uç, aynı sorunun üç cevabı. Bluecoins'te unutulan onay gideri eksik "
                    "bırakıyor; Money Manager'da hiç sorulmadan yazılmış bir kayıt duruyor; "
                    "Wallet hangisinin olacağını kullanıcının seçimine bağlıyor.",
                ],
                "dayanak": "E0291, E0289, E0043, E0430, E0429, E0243, kullanıcı kontrolü (Bluecoins)",
            },
            {
                "baslik": "Gerçekleşmemiş plan ölçülen hiçbir üründe toplama girmiyor",
                "metin": [
                    "Ölçülebilen üç üründe de aynı sonuç: gelecekteki bir ödeme ayın gelir/gider "
                    "toplamına dâhil değil. Money Manager'ın Ekim listesi toplamı 0/0/0 "
                    "gösterirken üst bölümde plan satırı duruyor.",
                    "Ayrışan şey planın nerede görüneceği: aynı ekranın üst bölümünde, ayrı bir "
                    "ekranda, ya da bütün plan türlerinin toplandığı ortak bir hatırlatıcı "
                    "listesinde. Üçü de planı görünür tutuyor ama sayıya katmıyor.",
                    "Bu ortak davranış, planın bir niyet olduğunun ürünler arası bir kabul "
                    "olduğunu gösteriyor.",
                    "Taksit bu kuralın dışında kalıyor: Money Manager gelecek taksitleri plan "
                    "değil, önceden yazılmış kayıt olarak tutuyor ve her biri kendi ayının "
                    "giderine giriyor (Bölüm 3). Aynı ürün tekrarlayan planı toplama katmıyor, "
                    "taksidi katıyor.",
                ],
                "dayanak": "E0244, E0306, E0047, E0416, E0407",
            },
            {
                "baslik": "Bütçe iki ayrı şey olarak kurulmuş",
                "metin": [
                    "Wallet'ta bütçe dönemli bir üst sınır ve kendi ekranında yaşıyor: aşıldığında "
                    "kırmızı çubuk, uyarı, günlük ortalama ve dönem sonu tahmini birlikte geliyor. "
                    "Bütçe bir sınırın yanında bir gidişat göstergesi.",
                    "Money Manager da aynı tarafta ama gidişat göstergesi olmadan: bütçe "
                    "kategori başına bir sınır, Toplam sekmesinde bir ilerleme çubuğuyla "
                    "ölçülüyor.",
                    "Goodbudget'ta bütçe ayrı bir nesne değil — zarfın içindeki para hem "
                    "harcanabilir tutar hem sınır. Bu yüzden aşımın ayrı bir bütçe ekranı yok; "
                    "sınır zarfın kendi bakiyesi. Zarfı aşan bir harcama bu koşumda denenmedi.",
                    "İki yaklaşım aynı soruyu farklı yerde cevaplıyor: biri harcamayı sonradan "
                    "ölçüyor, öteki parayı baştan bölüyor.",
                ],
                "dayanak": "E0301, E0302, E0405, E0431, E0417, E0424",
            },
            {
                "baslik": "Kavramın hiç olmaması da bir sonuç üretiyor",
                "metin": [
                    "Hesap Defterim'de tekrar beş ayrı yüzeyde arandı ve bulunamadı. Bunun "
                    "sonucu kullanıcının her ay aynı kaydı eliyle girmesi — ve girilmediği ay "
                    "defterde hiçbir iz kalmaması.",
                    "Defter mantığı burada da kendini koruyor: yalnız olmuş olan yazılıyor. "
                    "Olacak olanı gösterecek bir yer olmadığı için, unutulan ödeme sessizce "
                    "kayboluyor.",
                ],
                "dayanak": "E0171, E0150, E0178",
            },
        ],
        "kazanc": [
            ("Money Manager",
             "On dört sıklık seçeneği ve planların kendi listesi var; gelecek ay planı aynı "
             "ekranda ayrı bir bölümde gösterip toplama katmıyor",
             "Vadesi gelen tekrar sorulmadan kayda dönüşüyor; bütçe bloğu Toplam sekmesinin "
             "içinde saklı ve %0'ın nedeni ekranda yazmıyor"),
            ("Wallet",
             "Kapı kullanıcının: her vadede tek dokunuşluk onay, ve kapının hep açık kalıp "
             "kalmayacağı sorularak seçiliyor; bütçe aşım uyarısı ve dönem sonu tahminiyle "
             "birlikte geliyor",
             "Planlar yalnız sıradaki vadeyle listeleniyor; ileri dönemlerin toplu görünümü yok"),
            ("Bluecoins",
             "Bağımsız hatırlatıcı, tekrar ve taksit tek listede toplanıyor; kayda çevirirken "
             "hangi tarihe yazılacağı soruluyor",
             "Tekrar ve taksitler kendiliğinden gerçekleşmiyor, her vade bir dokunuş istiyor; "
             "üç ayrı kavramın tek listede durması hangisinin ne olduğunu okumayı zorlaştırıyor"),
            ("Goodbudget",
             "Bütçe ile zarfın bakiyesi aynı şey olduğu için ayrı bir bütçe ekranı "
             "gerekmiyor",
             "Zarf katmanı hesap katmanıyla uyuşmuyor: bütçenin bıraktığı para ile hesaptaki "
             "para iki ayrı sayı"),
            ("Hesap Defterim",
             "—",
             "Tekrar, plan, hatırlatıcı ve bütçe kavramlarının hiçbiri yok; unutulan ödeme "
             "defterde hiç iz bırakmıyor"),
        ],
        "soru": [
            "Plan gerçek kayda dönerken onay istenmeli mi? Wallet bunu bir tercih yapıp sonucunu "
            "yazıyor; varsayılanı otomatik tarafta.",
            "Gerçekleşme hangi tarihe yazılmalı — bugüne mi, planlanan güne mi? Bluecoins soruyor "
            "ve cevap ayın toplamını değiştiriyor.",
            "Gerçekleşmemiş plan nerede görünmeli? Üç ürün üç ayrı yer seçmiş ama üçü de aynı "
            "kuralda birleşiyor: toplama girmiyor. Money Manager'ın taksitleri bunun dışında.",
            "Bütçe harcamayı sonradan mı ölçmeli, parayı baştan mı bölmeli? İki model iki ayrı "
            "kullanıcı davranışı üretiyor.",
        ],
    },
]

EKSIKLER = [
    ("Bluecoins", "Otomatik kolun çalışması", "5.2",
     "Elle gerçekleşme kullanıcı kontrolüyle görüldü; otomatik kolu açık bir plan kurup vadesini geçirmek gerekir", "Orta"),
    ("Goodbudget", "Planın gerçekleşmesi ve sıklık listesinin tamamı", "5.4",
     "Kurulu planın vadesini geçirip işlem listesini okuyun", "Orta"),
]
