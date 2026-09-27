# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 4 · Islem ekleme ve geri bildirim — tek icerik kaynagi (plan §3)."""

NO = 4
BASLIK = "İşlem ekleme ve geri bildirim"
ANA_SORU = "Kullanıcı yeni bir kaydı nasıl giriyor ve kaydettikten sonra ne görüyor?"
GIRIS = [
    "Bu bölüm kayıt düğmesine basıldıktan sonrasını izler: formun nasıl açıldığı, hangi alanları hangi "
    "sırayla sorduğu, tür ve tutarın nasıl girildiği, art arda kayıt kolaylıkları, kaydetme sonrası ve "
    "hata anında ekranda ne göründüğü, kaydın nasıl düzeltilip silindiği.",
    "Düğmenin ekrandaki yeri 2.5'tedir; bu bölüm oradan devam eder. Formlar yan yana dar basıldığında alan "
    "adları okunmadığı için beş form iki sayfaya bölündü.",
]
GIRMEZ = [
    "Kaydın bakiyeye ve rapora etkisi → Belge 2",
    "Tekrar ve taksit formu → 7",
    "Kategori ve etiketin anlamı → 6",
    "Fiş eki ve fiş okuma → 10",
    "Form renkleri ve tutar biçimi → 3",
]
KAPSAM = [
    ("Money Manager", ["canli", "kosum"], "Kayıttan sonra sessiz dönüş karesiz."),
    ("Bluecoins", ["canli", "kosum"], "Kayıttan sonra sessiz dönüş karesiz."),
    ("Wallet", ["canli", "kosum"], "Şablon ve split kaydedilmedi."),
    ("Hesap Defterim", ["canli", "kosum"], "Boş tutarın reddi ve deftere taşıma karesiz."),
    ("Goodbudget", ["canli", "kosum"], "Tebrik mesajı ve hesap makinesinin çalışması karesiz."),
    ("KolayBi", ["kaynak"], "Alan adları kanıtlı, davranış değil."),
    ("Paraşüt", ["beyan"], "Form ekranı görülmedi."),
    ("Logo İşbaşı", ["beyan"], "İç form görülmedi."),
    ("QuickBooks Solopreneur", ["beyan"], "Solopreneur formu görülmedi."),
]

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 4.1
    {
        "tur": "soru", "no": "4.1", "baslik": "Form nereden açılıyor, kaç adımda?",
        "giris": "Kayıt düğmesine basıldığında dört üründe form doğrudan açılıyor. Wallet araya bir menü "
                 "koyuyor: normal kayıt, transfer ve şablon üç ayrı giriş.",
        "yukseklik": 404,
        "sekiller": [
            {"k": "wl-menu", "e": "E0324", "etiket": "Wallet · kayıt düğmesinin açtığı menü",
             "isaretler": [
                 (1, 744, 1582, "Create first template: şablon girişi."),
                 (2, 618, 1854, "Transfer: ayrı giriş."),
                 (3, 500, 2050, "New record: normal kayıt formu."),
                 (4, 845, 2220, "Menü açıkken düğme kapatma işaretine dönüyor."),
             ]},
        ],
        "notlar": [
            "Wallet'ta normal bir kayda menüden bir seçim daha yapılarak ulaşılıyor.",
            ("Hesap Defterim'de yön ana ekrandaki düğmeyle seçildiği için form o yönün adıyla açılıyor "
             "(→ 4.2).", "E0138"),
        ],
        "urunler": [
            ("Money Manager, Bluecoins, Goodbudget", "kosum", "Düğme doğrudan kayıt formunu açıyor.",
             "E0010, E0005, E0006"),
            ("Hesap Defterim", "canli", "Alındı ve Ödendi düğmeleri doğrudan o yönün formunu açıyor.", "E0138"),
        ],
    },

    # ---------------------------------------------------------------- 4.2
    {
        "tur": "soru", "no": "4.2", "baslik": "Formda hangi alanlar var, hangi sırayla?",
        "giris": "Money Manager az alanı alt alta diziyor; Bluecoins bütün seçenekleri tek yoğun ekranda "
                 "topluyor. İşaretler alanların formdaki sırasını izliyor.",
        "yukseklik": 392,
        "sekiller": [
            {"k": "mm-form", "e": "E0229", "etiket": "Money Manager · gider formu ve kategori paneli",
             "isaretler": [
                 (1, 560, 190, "Tür segmenti formun en üstünde."),
                 (2, 780, 450, "Tekrar / Taksit girişi tarih satırında."),
                 (3, 1000, 700, "Beş alan alt alta: Tarih, Tutar, Kategori, Hesap, Not."),
                 (4, 700, 1150, "Detay ve kamera girişi."),
                 (5, 560, 1340, "Kategori seçimi alttaki panelde açılıyor."),
             ]},
            {"k": "bc-form", "e": "E0020", "etiket": "Bluecoins · Ekle formu",
             "isaretler": [
                 (6, 700, 320, "Ad alanı en üstte."),
                 (7, 1000, 545, "Tarih, saat ve Planlı İşlemler aynı satırda."),
                 (8, 1000, 860, "Tutar; hesap makinesi ve para birimi yanında."),
                 (9, 700, 995, "Kategori ve hesap alt alta."),
                 (10, 700, 1320, "Bölmek, Durum ve Etiket aynı ekranda."),
                 (11, 700, 1620, "Not alanı."),
                 (12, 400, 2105, "Tür seçici formun en altında."),
             ]},
            {"k": "bc-durum", "e": "E0448", "etiket": "Bluecoins · aynı form, Durum açık",
             "isaretler": [
                 (13, 504, 1278, "Durum alanı."),
                 (14, 540, 2040, "Dört değer: Yok, Kontrol, Mutabık, İptal edildi."),
                 (15, 780, 412, "Belge yalnız üstteki ataçla ekleniyor; formda fatura bağlama alanı yok."),
             ]},
        ],
        "notlar": [
            "Money Manager formunda kaydetme düğmesi, kategori paneli kapanınca görünüyor (→ 4.5).",
            ("Bluecoins'te hesap olarak kredi kartı seçildiğinde aynı formda taksit alanı çıkıyor (→ 7.2).",
             "E0028"),
        ],
    },
    {
        "tur": "soru", "no": "4.2", "baslik": "Formda hangi alanlar var? — Wallet, Hesap Defterim, Goodbudget",
        "haritada": False,
        "giris": "Wallet hızlı formda yalnız tutar, hesap ve kategori soruyor; Hesap Defterim'de kategori "
                 "seçicisi yok; Goodbudget tutarın yanına zarfı ve hesabı koyuyor.",
        "yukseklik": 350,
        "sekiller": [
            {"k": "wl-form", "e": "E0277", "etiket": "Wallet · hızlı form",
             "isaretler": [
                 (1, 560, 215, "Tür sekmeleri: INCOME, EXPENSE, TRANSFER."),
                 (2, 560, 960, "Tutar ekranın ortasında, büyük."),
                 (3, 560, 1150, "Yalnız hesap ve kategori."),
                 (4, 820, 1290, "Şablonlar girişi."),
             ]},
            {"k": "hd-form", "e": "E0138", "etiket": "Hesap Defterim · Alındı formu",
             "isaretler": [
                 (5, 800, 555, "Alındı / Ödendi seçici formda da var."),
                 (6, 700, 900, "Tutar; hesap makinesi ikonu alanın içinde."),
                 (7, 790, 1105, "Notlar; mikrofonla sesli giriş."),
                 (8, 560, 1350, "Fatura ekle ve Öğe eklemek aynı satırda."),
                 (9, 540, 2170, "İki kaydetme düğmesi: çık ve devam et."),
             ]},
            {"k": "gb-form", "e": "E0116", "etiket": "Goodbudget · Add Transaction",
             "isaretler": [
                 (10, 700, 330, "Payee en üstte."),
                 (11, 560, 600, "Tutar ve tür aynı satırda."),
                 (12, 700, 790, "Envelope: zarf seçimi."),
                 (13, 800, 990, "Account: hesap, bakiyesiyle."),
                 (14, 700, 1360, "Schedule this… kutusu."),
             ]},
        ],
        "notlar": [
            ("Wallet'ta not, etiket, karşı taraf, ödeme türü, durum, yer ve ek alanları kayıttan sonra açılan "
             "ayrıntı ekranında.", "E0294, E0313"),
            ("Hesap Defterim'de kategori yerine ayarlardan açılan serbest bir Açıklama / Kategori metin kutusu "
             "var (→ 6.1).", "E0151"),
            ("Goodbudget formunun alt kısmını klavye araç çubuğu örtüyor; Notes ve widget seçeneği kısmen "
             "okunuyor. Formda isteğe bağlı bir Check # alanı da var; formun menüsünde yalnız Help.",
             "E0116, E0128"),
        ],
    },

    # ---------------------------------------------------------------- 4.3
    {
        "tur": "serit", "no": "4.3", "baslik": "Tür ve yön nasıl seçiliyor, varsayılan ne?",
        "giris": "Tür seçicinin formdaki yeri ve seçili türün nasıl gösterildiği ürünler arasında ayrışıyor. "
                 "\"Form hangi türle açılıyor\" ile \"her kayıtta seçim gerekiyor mu\" ayrı sorulardır; ikincisi "
                 "hiçbir üründe doğrulanmadı.",
        "kirpinti_genislik": 360,
        "kirpintilar": [
            {"e": "E0229", "ad": "Money Manager", "kutu": (0, 270, 1080, 390), "konum": "Formun üstü",
             "aciklama": "Gelir / Gider / Havale segmenti; seçili olan çerçeve ve renkle. Tam ekran: [[mm-form]]."},
            {"e": "E0277", "ad": "Wallet · gider", "kutu": (0, 270, 1080, 400), "konum": "Formun üstü",
             "aciklama": "EXPENSE seçili; ayrım yalnız zemin tonunda. Tam ekran: [[wl-form]]."},
            {"e": "E0327", "ad": "Wallet · transfer", "kutu": (0, 270, 1080, 400), "konum": "Formun üstü",
             "aciklama": "TRANSFER seçili; yine yalnız ton farkı."},
            {"e": "E0138", "ad": "Hesap Defterim", "kutu": (0, 490, 1080, 620), "konum": "Formun üstü",
             "aciklama": "Alındı / Ödendi çipleri; seçili olan dolu renkle. Tam ekran: [[hd-form]]."},
            {"e": "E0116", "ad": "Goodbudget", "kutu": (0, 480, 1080, 650), "konum": "Tutar satırının sağı",
             "aciklama": "Tür açılır listeden (Credit). Tam ekran: [[gb-form]]."},
            {"e": "E0487", "ad": "Bluecoins", "kutu": (0, 2125, 860, 2290), "konum": "Formun altı",
             "aciklama": "GİDER / GELİR / TRANSFER; seçili olan dolu zemin. Yeni açılan form GİDER seçili; "
                         "formun tamamı: [[bc-form]]."},
        ],
        "notlar": [
            "Money Manager, Bluecoins ve Hesap Defterim seçili türü dolu renk ya da çerçeveyle gösteriyor; Wallet yalnız zemin tonuyla, Goodbudget açılır listede adıyla.",
            "Hesap Defterim'de yön önce ana ekrandaki düğmeyle seçiliyor, formda yeniden değiştirilebiliyor.",
            ("Wallet'ta gelir girilmek istenen bir kayıtta form gider türünde kaldı.", "E0277, E0338"),
        ],
        "urunler_baslik": "Varsayılan tür",
        "urunler": [
            ("Bluecoins", "canli", "Yeni form GİDER seçili açılmış; tutarın yanında kırmızı eksi.", "E0487"),
            ("Wallet", "canli", "Hızlı form Expense seçili açılmış.", "E0277"),
            ("Money Manager", "canli", "Yeni form Gider seçili açılmış.", "E0463"),
            ("Goodbudget", "canli", "Boş formda tür Expense; formun hangi türle açıldığı ayrıca doğrulanmadı.",
             "E0133"),
        ],
    },

    # ---------------------------------------------------------------- 4.4
    {
        "tur": "soru", "no": "4.4", "baslik": "Tutar nasıl giriliyor?",
        "giris": "Money Manager ve Wallet tutarı formun altındaki tuş takımıyla, Bluecoins ve Hesap Defterim "
                 "alanın içindeki hesap makinesi ikonuyla alıyor; Goodbudget'ta hesap makinesi bir ayar. "
                 "Wallet'ın planlı ödeme formunda hesap makinesi ayrı bir diyalog.",
        "yukseklik": 380,
        "sekiller": [
            {"k": "wl-tus", "e": "E0277", "etiket": "Wallet · hızlı formun tuş takımı",
             "isaretler": [
                 (1, 560, 1500, "Rakamlar ve işlemler aynı tuş takımında."),
                 (2, 820, 1620, "İşlem tuşları sağ sütunda."),
             ]},
            {"k": "wl-hesap", "e": "E0339", "etiket": "Wallet · planlı ödemede hesap makinesi diyaloğu",
             "isaretler": [
                 (3, 700, 785, "Diyalog formun üstünde açılıyor."),
                 (4, 440, 1675, "Sonuç Insert ile alana aktarılıyor."),
             ]},
        ],
        "kirpinti_k": "hesap-ikonlari",
        "kirpinti_baslik": "Hesap makinesi girişi",
        "kirpinti_etiket": "Bluecoins ve Hesap Defterim'de tutar alanının içinde; Money Manager'da tuş "
                           "takımının sağ sütununda.",
        "kirpintilar": [
            ("E0020", "Bluecoins", (700, 650, 840, 790)),
            ("E0138", "Hesap Defterim", (900, 830, 1040, 970)),
            ("E0463", "Money Manager", (874, 1862, 1014, 2002)),
        ],
        "notlar": [
            ("Hesap Defterim'de tutar alanı odaktayken ekranda klavye araç çubuğu görünüyor; tuş takımının "
             "kendisi karede yok.", "E0162"),
            "İncelenen karelerde tutarın cihaz klavyesiyle yazıldığını gösteren bir kare yok.",
        ],
        "urunler": [
            ("Money Manager", "canli", "Form açılınca altta rakam tuş takımı; sağ sütundaki simge tam ekran bir "
             "hesap makinesi açıyor (AC, ÷, ×, −, +, =).", "E0463, E0464"),
            ("Goodbudget", "kosum", "Hesap makinesi ayarlarda açılıp kapanıyor (Use calculator to enter "
             "amounts); açıkken tutar alanı dört işlemli bir hesap makinesi açıyor (kullanıcı kontrolü).",
             "E0453; GB-04"),
        ],
    },

    # ---------------------------------------------------------------- 4.5
    {
        "tur": "soru", "no": "4.5", "baslik": "Art arda kayıt ve kolaylıklar",
        "giris": "Beş üründe beş ayrı kolaylık kümesi görülüyor. Hesap Defterim en çoğunu ana formun içinde "
                 "taşıyor; Wallet kaydı bölmeyi kayıttan sonraya bırakıyor.",
        "yukseklik": 320, "not_genislik": 280,
        "sekiller": [
            {"k": "hd-oge", "e": "E0155", "etiket": "Hesap Defterim · Öğe eklemek",
             "isaretler": [
                 (1, 560, 505, "Kalem dökümü diyaloğu."),
                 (2, 560, 1010, "Öğe, miktar, birim ve fiyat girilip ekleniyor."),
                 (3, 560, 1140, "Kalemlerin toplamı üstte."),
             ]},
            {"k": "hd-yeniad", "e": "E0161", "etiket": "Hesap Defterim · yeniden adlandırılmış düğmeler",
             "isaretler": [
                 (4, 400, 515, "Sütun başlıkları yeni adla."),
                 (5, 700, 720, "Kalem dökümü kaydın notuna yazılmış."),
                 (6, 540, 1900, "Düğmeler de yeni adla."),
             ]},
            {"k": "wl-split", "e": "E0311", "etiket": "Wallet · Split record",
             "isaretler": [
                 (7, 700, 370, "Özgün kayıt üstte."),
                 (8, 700, 760, "Split: kaydı parçalara bölme."),
             ]},
        ],
        "kirpinti_k": "devam-dugmeleri",
        "kirpinti_baslik": "Art arda kayıt düğmeleri",
        "kirpinti_etiket": "Kaydet'in yanında formu açık tutan düğme (formun altı).",
        "kirpinti_h": 30,
        "kirpintilar": [
            ("E0232", "Money Manager", (0, 1100, 1080, 1240)),
            ("E0138", "Hesap Defterim", (0, 2215, 1080, 2335)),
        ],
        "notlar": [
            "Hesap Defterim'de düğme ve sütun adlarını kullanıcı değiştirebiliyor; yeni ad toplam etiketlerine de geçiyor.",
            "Wallet'ta split kaydedilmedi; parçaların sonucu görülmedi.",
        ],
        "urunler": [
            ("Bluecoins", "canli", "Planlama, bölme ve kartta taksit aynı formda. Bölünmüş kayıt listede tek "
             "satır: \"2 Kategoriler\".", "E0020, E0045, E0028, E0490"),
            ("Wallet", "canli", "Menüde şablon girişi; şablon kaydedilmedi.", "E0324, E0325"),
            ("Goodbudget", "canli", "Kaydederken konuma göre payee önerisi için izin istiyor.", "E0118"),
            ("Goodbudget", "canli", "Formda Quick Transactions widget'ına ekleme seçeneği.", "E0116"),
            ("Hesap Defterim", "canli", "Not önerileri; ayarda açık.", "E0178"),
        ],
    },

    # ---------------------------------------------------------------- 4.6
    {
        "tur": "soru", "no": "4.6", "baslik": "Kaydetme sonrası geri bildirim",
        "giris": "Kaydettikten sonra üç ürün sessizce listeye dönüyor, Hesap Defterim kısa bir bildirim "
                 "gösteriyor, Goodbudget bir tebrik mesajı veriyor.",
        "yukseklik": 404,
        "sekiller": [
            {"k": "hd-toast", "e": "E0141", "etiket": "Hesap Defterim · kayıttan hemen sonra",
             "isaretler": [
                 (1, 880, 2215, "İşlem Eklendi: kısa bildirim."),
                 (2, 700, 640, "Yeni kayıt listede; satırında denge."),
             ]},
        ],
        "notlar": [
            "Bildirim ekranın altında, toplam bandının üstünde beliriyor.",
        ],
        "urunler": [
            ("Money Manager, Wallet, Bluecoins", "kosum", "Mesaj yok; form kapanıp listeye dönülüyor.",
             "E0010, E0014, E0005"),
            ("Goodbudget", "kosum", "Kayıttan sonra oyunlaştırılmış bir tebrik mesajı.", "E0006"),
        ],
    },

    # ---------------------------------------------------------------- 4.7
    {
        "tur": "soru", "no": "4.7", "baslik": "Hata ve zorunlu alan",
        "giris": "Eksik bilgide mesajın nereye konduğu ayrışıyor: ekranın altında kısa bir mesaj, alanın hemen "
                 "altında kırmızı bir metin ya da hiç mesaj yok.",
        "yukseklik": 300, "not_genislik": 250,
        "sekiller": [
            {"k": "mm-hata", "e": "E0232", "etiket": "Money Manager · hesap seçilmedi",
             "isaretler": [
                 (1, 880, 2215, "Mesaj ekranın altında."),
                 (2, 700, 600, "Boş alanın yanında mesaj yok."),
             ]},
            {"k": "wl-hata", "e": "E0281", "etiket": "Wallet · sıfır tutar",
             "isaretler": [
                 (3, 560, 2110, "Mesaj ekranın altında."),
                 (4, 560, 960, "Tutar 0."),
             ]},
            {"k": "gb-hata", "e": "E0117", "etiket": "Goodbudget · zarf seçilmedi",
             "isaretler": [
                 (5, 540, 2130, "Mesaj ekranın altında."),
                 (6, 700, 790, "Zarf alanı boş."),
             ]},
            {"k": "wl-alan", "e": "E0347", "etiket": "Wallet · planlı ödemede kategori yok",
             "isaretler": [
                 (7, 700, 520, "Hata metni alanın hemen altında, kırmızı."),
             ]},
        ],
        "notlar": [
            "Alan dışı mesaj: Money Manager, Wallet hızlı formu, Goodbudget.",
            "Alan yanında mesaj: Wallet planlı ödeme formu.",
            "Aynı üründe (Wallet) iki formda iki ayrı yaklaşım görülüyor.",
        ],
        "urunler": [
            ("Hesap Defterim", "kosum", "Boş tutarla kaydetme mesaj vermeden sonuçsuz kaldı.", "E0007, E0162"),
            ("Hesap Defterim", "canli", "Sıfır tutarlı kayıt listeye yazıldı.", "E0163"),
            ("Bluecoins", "canli", "Sıfır tutarlı kayıt oluştu; detayı 4.8'de.", "E0024"),
            ("Money Manager", "canli", "Tutarı boş bırakılan kayıt listeye ₺0,00 olarak girdi; uyarı yok.",
             "E0465, E0434"),
        ],
    },

    # ---------------------------------------------------------------- 4.8
    {
        "tur": "soru", "no": "4.8", "baslik": "Düzeltme ve silme",
        "giris": "Silme onayının karesi beş üründe var; Hesap Defterim'deki onay kalıcı silme adımında. Silineni "
                 "geri almanın yüzeyi iki üründe görüldü: Hesap Defterim'in Silinmiş işlemler listesi ve "
                 "Bluecoins'in Çöp Kutusu. Money Manager, Wallet ve Goodbudget'ın incelenen menü ve "
                 "ayarlarında böyle bir kalem yok.",
        "yukseklik": 300, "not_genislik": 250,
        "sekiller": [
            {"k": "bc-sil", "e": "E0024", "etiket": "Bluecoins · kayıt detayı ve silme onayı",
             "isaretler": [
                 (1, 1000, 330, "Detayda yazdır ve sil simgeleri."),
                 (2, 700, 1220, "Benzerleri gösterme ve yineleme."),
                 (3, 560, 2230, "Silme onayı: İptal / TAMAM."),
             ]},
            {"k": "gb-sil", "e": "E0125", "etiket": "Goodbudget · silme onayı",
             "isaretler": [
                 (4, 1000, 300, "Düzenleme ekranında silme simgesi."),
                 (5, 300, 1305, "Onay: NO / YES."),
             ]},
            {"k": "hd-geri", "e": "E0167", "etiket": "Hesap Defterim · Silinmiş işlemler",
             "isaretler": [
                 (6, 620, 600, "Silinmiş kayıtta Geri Yükle ve Silme."),
             ]},
            {"k": "bc-cop", "e": "E0089", "etiket": "Bluecoins · Çöp Kutusu",
             "isaretler": [
                 (7, 560, 215, "Çekmeceden açılan ayrı ekran."),
                 (8, 560, 800, "Boş; yönlendirme metni yok."),
             ]},
        ],
        "notlar": [
            ("Hesap Defterim'de kalıcı silme ikinci bir onay istiyor.", "E0168"),
            ("Bluecoins'te geri yüklenen kaydın tutarı toplama döndü; satırın listeye gelmesi uygulama yeniden "
             "açılınca oldu (kullanıcı kontrolü).", "E0401, E0402"),
            ("Money Manager'ın onayı: \"Silmek istediğinize emin misiniz?\" HAYIR / EVET.", "E0468"),
            ("Wallet'ın onayı: \"Do you really want to delete this item?\" No / Yes. Silme kayıt ayrıntısının "
             "araç çubuğunda, bölme (split) ile yan yana.", "E0484"),
            ("Geri alma kalemi aranan yerler: Money Manager'ın Daha ızgarası, Wallet'ın çekmecesi ve ayarları, "
             "Goodbudget'ın ayarları.", "E0254, E0376, E0450, E0377, E0378, E0495, E0453"),
        ],
        "urunler": [
            ("Money Manager, Wallet", "kosum", "Satırdan ya da detaydan doğrudan düzenleme.", "E0010, E0014"),
            ("Hesap Defterim", "kosum", "Kaydı başka deftere taşıma ve kopyalama.", "E0007, E0159"),
        ],
    },

    # ---------------------------------------------------------------- 4.9
    {
        "tur": "soru", "no": "4.9", "baslik": "Kaynakta görülen formlar",
        "giris": "KolayBi'nin destek sayfalarındaki iki form, canlı ürünlerde görülmeyen alanları adıyla "
                 "gösteriyor. Alan adları kanıtlı; seçimlerin sonucu görülmedi.",
        "dikey": True,
        "sekiller": [
            {"k": "kb-gider", "e": "E0192", "etiket": "KolayBi · Yeni Genel Gider",
             "kirpma": (96, 112, 1305, 865), "genislik": 334,
             "isaretler": [
                 (1, 384, 273, "Cari Takibi: Yok / Var."),
                 (2, 724, 388, "Gider Tipi zorunlu."),
                 (3, 424, 445, "Ödeme Durumu: Ödenmedi / Ödendi."),
                 (4, 484, 498, "Son ödeme tarihi boş bırakılabiliyor."),
                 (5, 1104, 300, "Proje alanı; ayarlardan kapatılabileceği notu."),
                 (6, 1054, 490, "Açıklamayı şablon olarak kaydetme."),
                 (7, 1124, 583, "Dosya yükleme; türler ve 5 MB sınırı."),
                 (8, 624, 735, "Altta KDV bandı; iki ayrı Toplam KDV başlığı."),
             ]},
            {"k": "kb-fatura", "e": "E0200", "etiket": "KolayBi · Yeni Alış Faturası",
             "kirpma": (96, 112, 1305, 865), "genislik": 334,
             "isaretler": [
                 (9, 724, 278, "Cari zorunlu."),
                 (10, 604, 400, "Düzenleme saati zorunlu."),
                 (11, 424, 515, "Ödeme Durumu burada da var."),
                 (12, 484, 570, "Vade tarihi boş bırakılabiliyor."),
                 (13, 854, 701, "Altta ürün / hizmet kalem tablosu başlıyor."),
             ]},
        ],
        "notlar": [
            "Ödendi seçiminin kasaya, cariye ve rapora etkisi görülmedi (→ Belge 2 9.1).",
            "İki formun üst kısmı aynı: ödeme durumu, vade, proje, etiket, açıklama ve dosya.",
        ],
        "urunler_baslik": "Karesi basılmayan ürünler",
        "urunler": [
            ("Paraşüt", "beyan", "Beş gider türü: detaylı ve hızlı fiş/fatura, maaş/prim, vergi/SGK, banka "
             "gideri. Kayıt ile ödeme ayrı adım.", "E0011"),
            ("Paraşüt", "beyan", "Fiş fotoğrafından okuma (→ 10.3).", "E0011"),
            ("Logo İşbaşı", "beyan", "Sesle fatura kesme; çalışırken görülmedi.", "E0009"),
            ("QuickBooks Solopreneur", "beyan", "Kayıtlar ağırlıkla banka bağlantısından geliyor; kullanıcı "
             "türü ve kategoriyi inceliyor.", "E0012"),
        ],
        "dayanak": ["Kaynak beyanı: E0011, E0009, E0012"],
    },
]

EKSIKLER = [
    ("Goodbudget", "Tebrik mesajının ve hesap makinesinin karesi", "4.4, 4.6",
     "Yeni bir kayıt girip görüntü alın; mevcut kayıtlar silinmez", "Orta"),
    ("Goodbudget", "Formun varsayılan türü", "4.3", "Formu açıp ilk görüntüyü alın", "Düşük"),
    ("KolayBi / Paraşüt / Logo", "Formların çalışan hâli", "4.9", "Ücretli hesap gerekir", "Önerilmez"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0128", "E0133", "E0178", "E0254", "E0376", "E0377", "E0378", "E0401", "E0402", "E0434", "E0450", "E0453",
               "E0464", "E0465", "E0468", "E0484", "E0490", "E0495"]
