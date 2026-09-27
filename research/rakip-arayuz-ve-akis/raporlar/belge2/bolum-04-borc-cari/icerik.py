# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 4 · Borc, cari ve tahsilat — tek icerik kaynagi."""

NO = 4
BASLIK = "Borç, cari ve tahsilat"
ANA_SORU = "Henüz tahsil edilmemiş bir alacak nerede duruyor ve hesap bakiyesine dokunuyor mu?"
EN_AZ_KARE = 18

GIRIS = [
    "Bir iş yapıldı, fatura kesildi, para henüz gelmedi. Bu an ürünler arasındaki en keskin "
    "ayrımı ortaya çıkarıyor: kimi üründe alacak bir hesap bakiyesi, kiminde ayrı bir nesne, "
    "kiminde hiç yok.",
    "Bölüm üç adımı izliyor: alacak nasıl doğuyor, doğduğu anda hangi sayıyı değiştiriyor, ve "
    "tahsilat yapıldığında para nereden nereye geçiyor.",
]
GIRMEZ = [
    "Kart borcu ve kart ödemesi → Bölüm 3",
    "Vadeli ödemenin takvimde görünmesi → Bölüm 5",
    "Cari ekstresinin belge olarak dışa aktarılması → Bölüm 8",
    "Cari ekranlarının görsel düzeni → Belge 1 §8",
]
KAPSAM = [
    ("Bluecoins", ["canli"], "Alacağın doğması ve tahsilat zinciri uçtan uca kareli."),
    ("Wallet", ["canli"], "Borç nesnesi, yön seçimi ve bakiye sorusu kareli."),
    ("Money Manager", ["canli"], "Cari kavramı hesap listesinde bulunamadı."),
    ("Hesap Defterim", ["canli"], "Veresiye ayrı bir uygulamaya yönlendiriliyor."),
    ("Goodbudget", ["yok"], "Debt hesap türü var ama ücretsiz pakette açılamadı."),
    ("KolayBi", ["kaynak"], "Cari formu ve personel carisi destek görselinde."),
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

    # ------------------------------------------------------------------ 4.1
    {
        "tur": "yanyana", "no": "4.1",
        "baslik": "Alacak nerede yaşıyor",
        "giris": "İki canlı üründe iki bambaşka cevap: birinde alacak bir hesap ve bakiyesi "
                 "listede diğer hesapların yanında duruyor; ötekinde alacak ayrı bir nesne ve "
                 "kendi ekranında yaşıyor. Üçüncüsünde kavram ürünün dışına çıkarılmış.",
        "yukseklik": 310,
        "sekiller": [
            {"k": "a-bc", "e": "E0049", "ad": "Bluecoins", "etiket": "Alacak bir hesap türü",
             "kirpma": (0, 120, 1080, 2090),
             "satirlar": ["Cari hesap, hesap listesinde kendi grubunda.",
                          "Grubu, kredi kartıyla aynı CARİ HESAP bölümünde.",
                          "Özel bir alanı veya fatura bağı görülmedi."]},
            {"k": "a-wl1", "e": "E0316", "ad": "Wallet", "etiket": "Borç ayrı bir nesne",
             "kirpma": (0, 120, 1080, 2300),
             "satirlar": ["Debts / Active sekmesinde bir kart.",
                          "\"ADA REKLAM OWES ME\" 5.000,00.",
                          "Kartın altında Add Record girişi."]},
            {"k": "a-wl2", "e": "E0358", "ad": "Wallet", "etiket": "Borç yönle başlıyor",
             "kirpma": (0, 120, 1080, 2300),
             "satirlar": ["Ekleme yalnız iki yönle başlıyor:",
                          "I Lent ve I Borrowed.",
                          "Fatura veya alacak türü seçeneği yok."]},
            {"k": "a-hd", "e": "E0177", "ad": "Hesap Defterim", "etiket": "Veresiye ayrı uygulamada",
             "kirpma": (0, 120, 1080, 2300),
             "satirlar": ["Menüde \"Diğer uygulamalar\" bölümü.",
                          "Veresiye Defteri ayrı bir indirme.",
                          "Bu üründe cari kavramı yok."]},
        ],
        "notlar": [
            "Money Manager'ın hesap listesinde Nakit, Banka Hesapları ve Kredi Kartı grupları "
            "var; cari veya karşı taraf türünde bir grup bulunamadı. Alacak bu üründe ancak "
            "sıradan bir hesap açılarak taklit edilebilir.",
            "Goodbudget'ın hesap türleri arasında bir Debt grubu var, ama ücretsiz pakette tek "
            "hesap sınırı dolduğu için bu türde hesap açılamadı; borç modeli görülemedi.",
        ],
        "sag_notlar": [
            ("Bluecoins'te cari, hesap seçicide kendi grubu olarak duruyor: Banka, Nakit, "
             "Cari hesap, Kredi Kartı.", "E0072"),
            ("Goodbudget'ın hesap grupları arasında Debt başlığı kareli.", "E0112"),
        ],
    },

    # ------------------------------------------------------------------ 4.2
    {
        "tur": "yanyana", "no": "4.2",
        "baslik": "Wallet kullanıcıya soruyor: bu borç bakiyeye dokunsun mu",
        "giris": "Bu araştırmanın en açık sözlü ekranı. Ürün, borcun hesap bakiyesini değiştirip "
                 "değiştirmeyeceğini kendi kararı yapmıyor — kullanıcıya soruyor ve sonucunu "
                 "cümleyle yazıyor.",
        "yukseklik": 300,
        "sekiller": [
            {"k": "w-bag", "e": "E0308", "ad": "Önce bağlama sorusu", "etiket": "Mevcut kayda bağla ya da atla",
             "kirpma": (0, 700, 1080, 2300),
             "satirlar": ["\"Do you already have this debt record in Wallet?\"",
                          "Yes, select record · No, skip."]},
            {"k": "w-form", "e": "E0309", "ad": "I Lent formu", "etiket": "Alanlar ve varsayılan vade",
             "kirpma": (0, 120, 1080, 1700),
             "satirlar": ["Name, Description, Account, Amount.",
                          "Due date varsayılanı bir yıl sonrası.",
                          "Hesap alanı formda zorunlu duruyor."]},
            {"k": "w-soru", "e": "E0315", "ad": "Kritik soru", "etiket": "Kayıt oluşturulsun mu",
             "kirpma": (0, 120, 1080, 1560),
             "satirlar": ["\"Do you want to create a Record for this Debt?\"",
                          "\"If you create a Record your balance will change.\"",
                          "No · Yes, create record."]},
            {"k": "w-kayit", "e": "E0317", "ad": "Evet kolunun sonucu", "etiket": "Debt Records",
             "kirpma": (0, 120, 1080, 1000),
             "satirlar": ["Ana Hesap'ta −5.000 kayıt oluştu.",
                          "Kategori: Loan, interests.",
                          "Liste toplamı −5.000,00."]},
        ],
        "notlar": [
            "Ürün iki yolu da açık tutuyor: borç yalnız bir takip kaydı olarak kalabilir, ya da "
            "hesap bakiyesini değiştiren gerçek bir işlem üretebilir. Seçim kullanıcıya bırakılmış "
            "ve sonucu tek cümleyle yazılmış.",
            ("Bağlama kolu da işletildi: Select Record bütün mevcut kayıtları etiketleriyle "
             "listeliyor ve borç o kayıtlardan birine bağlanabiliyor.", "E0441 · E0367"),
        ],
        "sag_notlar": [
            ("Borç kartı \"OWES ME 5.000,00\" derken kayıt listesi \"Total −5.000,00\" diyor — "
             "aynı borç, iki ekranda ters işaret.", "E0317"),
            ("Borç eklemenin başlangıcı yalnız iki yön: I Lent / I Borrowed.", "E0358"),
            ("Aynı soru ikinci borç kurulurken de soruldu.", "E0365"),
        ],
    },

    # ------------------------------------------------------------------ 4.3
    {
        "tur": "yanyana", "no": "4.3",
        "baslik": "Tahsilat: alacak hesabından bankaya",
        "giris": "Bluecoins'te zincir uçtan uca koşuldu. Alacak bir hesap olduğu için tahsilat "
                 "da iki hesap arasında bir aktarım — ve tam da bu yüzden gelir/gider "
                 "toplamlarına hiç dokunmuyor.",
        "yukseklik": 290,
        "sekiller": [
            {"k": "b-form", "e": "E0074", "ad": "Alacak doğuyor", "etiket": "Cari hesaba gelir formu",
             "kirpma": (0, 120, 1080, 1560),
             "satirlar": ["Gelir kaydı cari hesaba yazılıyor.",
                          "Durum ve fatura bağı bu karede açık değil."]},
            {"k": "b-d2", "e": "E0075", "ad": "Fatura sonrası", "etiket": "Cari 12.000",
             "kirpma": (0, 120, 1080, 1250),
             "satirlar": ["Cari bakiyesi 12.000.",
                          "Ana Hesap 29.700 — banka kıpırdamadı."]},
            {"k": "b-d3", "e": "E0079", "ad": "Tahsilat sonrası", "etiket": "Cari 7.000",
             "kirpma": (0, 120, 1080, 1250),
             "satirlar": ["Cari 12.000 → 7.000.",
                          "Banka 29.700 → 34.700.",
                          "Aktarım neti sıfır."]},
            {"k": "b-tum", "e": "E0055", "ad": "Bütün hesaplar", "etiket": "Dönem karşılaştırması",
             "kirpma": (0, 120, 1080, 2130),
             "satirlar": ["VARLIKLAR 22.350 → 43.850.",
                          "CARİ HESAP −1.000 → −500: cari 0, kart −500.",
                          "Cari, varlıklarda değil, kartla aynı bölümde."]},
        ],
        "notlar": [
            "Tahsilat 5.000'lik kısmi bir ödemeydi: alacağın tamamı kapanmadan cari bakiyesi "
            "7.000'de kaldı. Kısmi tahsilat ayrı bir kavram değil, sıradan bir aktarım tutarı.",
            "Alacağın doğduğu an banka bakiyesi hiç değişmedi; para ancak tahsilat adımında "
            "bankaya geçti. Gelir ise fatura anında yazıldı.",
        ],
        "sag_notlar": [
            ("Geçmiş bir satırdaki 12.000 güncel cari bakiyesi değildir; liste satırı o anın "
             "tutarını taşıyor.", "E0079"),
            ("Formun Durum alanı dört değer taşıyor: Yok, Kontrol, Mutabık, İptal edildi. "
             "Fatura bağlama alanı yok; belge yalnız formun üstündeki ataçla ekleniyor.", "E0448"),
        ],
    },

    # ----------------------------------------------------------------- 4.3b
    {
        "tur": "yanyana", "no": "4.3b",
        "baslik": "Wallet'ta tahsilat ve borcun rapora düşen izi",
        "giris": "Aynı zincir bu üründe de uçtan uca koşuldu ve sonucu diğerinden farklı çıktı. "
                 "Tahsilat borcu düşürüyor — ama borç kayıtları gelir ve gider toplamlarına da "
                 "giriyor.",
        "yukseklik": 290,
        "sekiller": [
            {"k": "wt-nasil", "e": "E0367", "ad": "İki yol soruluyor", "etiket": "Kayıt nasıl eklensin",
             "kirpma": (0, 120, 1080, 1620),
             "satirlar": ["Select Record: mevcut bir kaydı borca bağla.",
                          "Create new Record: borcu öde ya da artır.",
                          "Borcu artırma da bir seçenek."]},
            {"k": "wt-form", "e": "E0368", "ad": "Repay debt", "etiket": "Kalan tutar yer tutucuda",
             "kirpma": (0, 120, 1080, 1050),
             "satirlar": ["Debt action: Repay debt.",
                          "Yer tutucu: 12.000,00 to Repay debt.",
                          "Kalan tutar formun içinde yazılı."]},
            {"k": "wt-sonuc", "e": "E0370", "ad": "Kısmi tahsilat sonrası", "etiket": "12.000 → 7.000",
             "kirpma": (0, 120, 1080, 1480),
             "satirlar": ["5.000 girildi; borç 12.000'den 7.000'e indi.",
                          "Diğer kart 5.000'de değişmeden kaldı.",
                          "Tahsilat yalnız bağlandığı borcu düşürdü."]},
            {"k": "wt-rapor", "e": "E0398", "ad": "Cash-flow · son 30 gün", "etiket": "Borç kayıtları toplamda",
             "kirpma": (0, 120, 1080, 1180),
             "satirlar": ["Income 5.000 · Expenses −21.600 · net −16.600.",
                          "Record taşıyan borç kayıtları gelir/gidere dâhil.",
                          "Tahsilat gelire, borç verme gidere yazıldı."]},
        ],
        "notlar": [
            "Kayıt listesinde iki satır bu iki yönü taşıyor: tahsilat Lending, renting "
            "kategorisiyle +5.000, borç verme Loan, interests kategorisiyle −5.000. Yani borç, "
            "kategori sisteminin içinden geçip gelir/gider toplamına giriyor.",
            "Bu, önceki sayfadaki ürünle taban tabana zıt bir sonuç: orada tahsilat iki hesap "
            "arasında bir aktarımdı ve gelir/gider toplamına hiç dokunmuyordu.",
            ("Ödeme formundaki Debt action tam iki değer taşıyor: Repay debt ve Increase debt.",
             "E0451"),
        ],
        "sag_notlar": [
            ("Aynı karşı tarafa iki ayrı borç kartı açılabiliyor ve ikisi birleştirilmiyor; "
             "karşı taraf başına toplam gösterilmiyor.", "E0366"),
            ("Hizmet faturası alacağı da I Lent formuyla giriliyor; ürün alacak ile borç vermeyi "
             "ayrı kavramlar olarak ayırmıyor.", "E0364"),
        ],
    },

    # ------------------------------------------------------------------ 4.4
    {
        "tur": "tablo", "no": "4.4", "baslik": "Alacak ve tahsilat, ürün ürün",
        "giris": "Aynı üç soru: alacak nerede duruyor, doğduğu an hangi sayı değişiyor, tahsilat "
                 "ne üretiyor.",
        "sutunlar": [("", 16), ("Bluecoins", 21), ("Wallet", 21), ("Money Manager", 21),
                     ("Hesap Defterim", 21)],
        "boy": 8.4,
        "satirlar": [
            [
                "Alacak nerede",
                {"t": "Bir hesap; listede kendi grubunda", "tur": "canli", "d": "E0072"},
                {"t": "Ayrı bir nesne; Debts ekranında", "tur": "canli", "d": "E0316"},
                {"t": "Cari türü bulunamadı; sıradan hesapla taklit edilir", "tur": "canli", "d": "E0235"},
                {"t": "Ürün içinde yok; ayrı uygulamaya yönlendiriyor", "tur": "canli", "d": "E0177"},
            ],
            [
                "Doğduğu an bakiye değişir mi",
                {"t": "Cari hesabın bakiyesi artar; banka değişmez", "tur": "canli", "d": "E0075"},
                {"t": "Kullanıcıya soruluyor: kayıt oluşturulursa değişir", "tur": "canli", "d": "E0315"},
                {"t": "Ölçülemedi — kavram yok", "tur": "yok", "d": "E0235"},
                {"t": "Ölçülemedi — kavram yok", "tur": "yok", "d": "E0177"},
            ],
            [
                "Alacak gelir sayılıyor mu",
                {"t": "Evet — fatura anında gelir yazıldı", "tur": "canli", "d": "E0075"},
                {"t": "Evet — Record taşıyan borç kayıtları gelir/gidere giriyor", "tur": "canli", "d": "E0398"},
                {"t": "Ölçülemedi", "tur": "yok", "d": "E0235"},
                {"t": "Ölçülemedi", "tur": "yok", "d": "E0177"},
            ],
            [
                "Tahsilat ne üretiyor",
                {"t": "Aktarım: cari −5.000, banka +5.000", "tur": "canli", "d": "E0079"},
                {"t": "Repay debt kaydı; borç 12.000 → 7.000", "tur": "canli", "d": "E0370"},
                {"t": "Ölçülemedi", "tur": "yok", "d": "E0235"},
                {"t": "Ölçülemedi", "tur": "yok", "d": "E0177"},
            ],
            [
                "Vade kavramı var mı",
                {"t": "Yok — cari hesap formu tarandı, vade alanı yok", "tur": "canli", "d": "E0449"},
                {"t": "Evet — Due date, varsayılanı bir yıl sonrası", "tur": "canli", "d": "E0309"},
                {"t": "Yok", "tur": "yok", "d": "E0235"},
                {"t": "Yok", "tur": "yok", "d": "E0177"},
            ],
        ],
        "notlar": [
            "Goodbudget tabloda yok: hesap türleri arasında Debt grubu bulunmasına rağmen "
            "ücretsiz pakette ikinci hesap açılamadığı için hiçbir satır ölçülemedi.",
            "İki canlı üründe iki ayrı model olduğu için satırların çoğu yan yana "
            "karşılaştırılamıyor; ayrışma cevaplarda değil, sorunun ürüne uyup uymamasında.",
        ],
    },

    # ------------------------------------------------------------------ 4.5
    {
        "tur": "soru", "no": "4.5", "baslik": "Kaynakta cari: vadesi ve iskontosu olan bir taraf",
        "giris": "KolayBi'de cari birinci sınıf bir nesne ve formu canlı iki üründe bulunmayan "
                 "alanlar taşıyor: vade günü, sabit iskonto ve durumu olan bir açılış bakiyesi. "
                 "Alanların varlığı kanıtlı; kaydın sonucu görülmedi.",
        "dikey": True, "not_genislik": 300,
        "sekiller": [
            {"k": "kb-cari", "e": "E0195", "etiket": "KolayBi · Cari Detay Bilgileri",
             "kirpma": (96, 112, 1305, 800), "genislik": 350,
             "satirlar": ["Vade Günü ve Sabit İskonto: Yok / Var.",
                          "Açılış Bakiyesi tutar, para birimi, durum, proje ve tarih taşıyor.",
                          "Formda Borç Alacak Ekle ve Banka Ekle girişleri var."]},
            {"k": "kb-liste", "e": "E0194", "etiket": "KolayBi · Cari listesi (demo adlar karartıldı)",
             "kirpma": (96, 112, 1305, 700), "genislik": 350,
             "satirlar": ["Cariler beş sekmeye ayrılmış.",
                          "Her satırda renkli yerel bakiye ve cari tipi.",
                          "Toplu seçim ve favori işaretleme var."]},
        ],
        "notlar": [
            "Vade günü cari tarafında tanımlanıyor: taraf başına bir ödeme beklentisi. Wallet "
            "vadeyi borcun kendisine, KolayBi ise karşı tarafa bağlamış.",
            "Personel carileri ayrı bir sekme ve maaş ödemesi bu cari üzerinden yürüyor; "
            "listede bakiyeler hem artı hem eksi olabiliyor. Çalışan da bir karşı taraf.",
        ],
        "sag_notlar": [
            ("Personel carilerinde tipler Serbest / Yarı Zamanlı / Tam Zamanlı.", "E0201"),
        ],
        "urunler_baslik": "Aynı soruda kaynaktan okunan üç ürün",
        "urunler": [
            ("Paraşüt", "beyan",
             "Kaynak üç borçlandırma yolu sayıyor: müşteri kaydında açılış bakiyesi, satış "
             "faturası, ve borç yokken ödeme ekleme (avans). Cari bakiye otomatik hesaplanıyor "
             "ve tahsilat en gecikmiş açık faturadan başlayarak mahsuplaşıyor. Bölüm 9'da kurulur.",
             "parasut.com kılavuzu"),
            ("Logo İşbaşı", "beyan",
             "Kaynağa göre tahsilat, borç-alacak ve ödeme işlemleri cari hesap içinde toplanıyor "
             "ve fatura kesilirken müşterinin bakiyesi anında görünüyor. Bölüm 9'da kurulur.",
             "isbasi.com"),
            ("QuickBooks Solopreneur", "beyan",
             "Kaynakta karşı taraf cari hesabı anlatılmıyor; ürün fatura kesip ödeme hatırlatması "
             "gönderiyor ama alacak bakiyesi kavramı belgelenmemiş.", "Intuit yardım merkezi"),
        ],
    },

    # ------------------------------------------------------------------ 4.6
    {
        "tur": "akis", "no": "4.6", "baslik": "Alacağın yolu ve ayrıldığı noktalar",
        "giris": "Alacak doğduğu andan tahsil edildiği ana kadar üç adım. İlk çatallanma daha "
                 "alacağın nereye yazılacağında oluyor ve sonraki her şeyi belirliyor.",
        "adimlar": [
            {
                "baslik": "Alacak doğuyor",
                "dallar": [
                    {"urunler": "Bluecoins", "vurgu": True,
                     "metin": "Cari bir hesap türü. Gelir kaydı o hesaba yazılıyor ve hesabın "
                              "bakiyesi alacağı gösteriyor.", "d": "E0075"},
                    {"urunler": "Wallet", "vurgu": True,
                     "metin": "Borç ayrı bir nesne. Yönle başlıyor (I Lent / I Borrowed) ve "
                              "kendi ekranında yaşıyor.", "d": "E0316"},
                    {"urunler": "Money Manager · Hesap Defterim",
                     "metin": "Kavram yok. Biri sıradan hesapla taklit gerektiriyor, diğeri "
                              "ayrı bir uygulamaya yönlendiriyor.", "d": "E0177"},
                ],
            },
            {
                "baslik": "Hesap bakiyesi kıpırdıyor mu",
                "dallar": [
                    {"urunler": "Bluecoins",
                     "metin": "Banka kıpırdamıyor; hareket eden şey cari hesabın bakiyesi. "
                              "Gelir ise fatura anında yazılıyor.", "d": "E0075"},
                    {"urunler": "Wallet", "vurgu": True,
                     "metin": "Ürün karar vermiyor, soruyor: kayıt oluşturulursa bakiye "
                              "değişecek. İki yol da açık bırakılmış.", "d": "E0315"},
                ],
            },
            {
                "baslik": "Tahsilat yapılıyor",
                "dallar": [
                    {"urunler": "Bluecoins",
                     "metin": "İki hesap arası aktarım: cari −5.000, banka +5.000. Gelir/gider "
                              "toplamlarına dokunmuyor, çünkü gelir zaten yazılmıştı.",
                     "d": "E0079"},
                    {"urunler": "Wallet", "vurgu": True,
                     "metin": "Repay debt kaydı borcu düşürüyor — ama aynı kayıt gelir/gider "
                              "toplamına da giriyor.", "d": "E0398"},
                ],
            },
        ],
        "notlar": [
            "İki üründe iki ayrı model olduğu için yol tek bir çizgide birleşmiyor. Ortak olan "
            "tek şey şu: alacağın doğması ile paranın gelmesi iki ayrı an, ve ikisi aynı sayıyı "
            "değiştirmiyor.",
            "Vade iki üründe iki ayrı yere bağlanmış: Wallet borcun kendisine, KolayBi karşı "
            "tarafa. Bluecoins'in cari hesap formunda vade alanı yok.",
        ],
    },

    # ------------------------------------------------------------------ 4.7
    {
        "tur": "cikarim", "no": "4.7", "baslik": "Neden ayrışıyorlar",
        "giris": "Bu bölümde ürünler aynı soruya farklı cevap vermiyor — soruyu farklı "
                 "soruyorlar. Alacağı bir hesap sayan ürünle ayrı bir nesne sayan ürün, "
                 "sonrasındaki her adımı da farklı kurmak zorunda kalıyor.",
        "mekanizma": [
            {
                "baslik": "Alacağı hesap yapmak, tahsilatı aktarıma çeviriyor",
                "metin": [
                    "Bluecoins'te cari bir hesap türü olduğu için alacak bir hesap bakiyesi, "
                    "tahsilat da iki hesap arasındaki bir aktarım oluyor. Fatura anında gelir "
                    "yazıldı ve banka kıpırdamadı; tahsilat anında banka arttı ve gelir "
                    "kıpırdamadı.",
                    "Bu yapı aynı parayı iki kez gelir saymayı kendiliğinden engelliyor: para "
                    "iki kez hareket ediyor ama yalnız biri gelir.",
                    "Karşılığında alacak hesap listesinde duruyor, ama VARLIKLAR bölümünde değil: "
                    "kredi kartı ve ipotekle aynı CARİ HESAP bölümünde, yani borç tarafında. "
                    "Net değere giriyor, varlık toplamına girmiyor; \"tahsil edeceğim para\" "
                    "elimdeki paranın değil, kartın yanında okunuyor.",
                ],
                "dayanak": "E0074, E0075, E0079, E0055, E0049, E0072, kullanıcı kontrolü (Bluecoins)",
            },
            {
                "baslik": "Wallet kararı kullanıcıya bırakıyor ve sonucunu yazıyor",
                "metin": [
                    "Borç kaydedilirken ürün şunu soruyor: \"Do you want to create a Record for "
                    "this Debt? If you create a Record your balance will change.\" İki seçenek "
                    "de açık; borç yalnız bir takip kaydı olarak kalabiliyor ya da hesap "
                    "bakiyesini değiştiren gerçek bir işleme dönüşebiliyor.",
                    "Bu, incelenen dokuz ürün içinde bir kaydın bakiyeye dokunup dokunmayacağını "
                    "kullanıcıya açıkça soran tek yüzey. Soru, cevabın sonucunu da aynı cümlede "
                    "söylüyor.",
                    "Evet kolu seçildiğinde oluşan kayıt Loan, interests kategorisine düşüyor — "
                    "yani borç, kategori sisteminin içinden geçiyor.",
                ],
                "dayanak": "E0308, E0309, E0315, E0317",
            },
            {
                "baslik": "İki model, raporda taban tabana zıt iki sonuç",
                "metin": [
                    "Alacağı hesap yapan üründe tahsilat bir aktarım ve gelir/gider toplamına "
                    "hiç dokunmuyor. Borcu ayrı bir nesne yapan üründe ise tam tersi: borç "
                    "kayıtları kategori sisteminin içinden geçiyor ve dönem raporuna giriyor.",
                    "Kayıt listesinde iki yön iki ayrı kategoriyle duruyor — tahsilat Lending, "
                    "renting ile artı, borç verme Loan, interests ile eksi. Son otuz günün "
                    "raporunda Income 5.000 ve Expenses −21.600 okunuyor.",
                    "Sonuç: aynı ekonomik olay bir üründe ayın gelirini değiştiriyor, ötekinde "
                    "değiştirmiyor. İkisi de kendi modeline göre tutarlı; farkı üreten şey "
                    "alacağın hesap mı yoksa ayrı bir nesne mi olduğu.",
                ],
                "dayanak": "E0398, E0372, E0079, E0370",
            },
            {
                "baslik": "Aynı borç, iki ekranda ters işaret",
                "metin": [
                    "Borç kartı \"ADA REKLAM OWES ME 5.000,00\" derken aynı borcun kayıt listesi "
                    "\"Total −5.000,00\" gösteriyor. İkisi de kendi bakış açısından doğru: kart "
                    "alacağı, liste o alacağı doğuran kaydın hesaptan çıkışını yazıyor.",
                    "Bu, kart bölümündeki iki borç sayısıyla aynı desen: bir ekran sonucu, "
                    "öteki o sonucu üreten hareketi gösteriyor ve hangisinin hangisi olduğu "
                    "ekranda yazmıyor.",
                ],
                "dayanak": "E0316, E0317",
            },
            {
                "baslik": "Kavramın yokluğu da bir tasarım kararı",
                "metin": [
                    "Hesap Defterim carileri ürünün dışına çıkarmış ve menüsünde ayrı bir "
                    "uygulamaya yönlendiriyor. Money Manager'ın hesap listesinde böyle bir grup "
                    "hiç yok; alacak ancak sıradan bir hesap açılarak taklit edilebilir.",
                    "İkisi de defter mantığını korumayı seçmiş: her satır gerçekleşmiş bir para "
                    "hareketi. Henüz gelmemiş para bu mantığa girmiyor, bu yüzden ya dışarı "
                    "çıkarılıyor ya hiç yok.",
                ],
                "dayanak": "E0177, E0235",
            },
        ],
        "kazanc": [
            ("Bluecoins",
             "Alacak bir hesap olduğu için tahsilat doğal bir aktarım; gelir bir kez yazılıyor "
             "ve kısmi tahsilat ayrı bir kavram gerektirmiyor",
             "Tahsil edilmemiş alacak kartla aynı bölümde, borç tarafında duruyor; varlık "
             "toplamı onu göstermiyor ve cari hesap formunda vade alanı yok"),
            ("Wallet",
             "Borç ayrı bir nesne, yönü ve vadesi var; bakiyeye dokunup dokunmayacağını "
             "kullanıcıya soruyor ve sonucunu aynı cümlede yazıyor",
             "Aynı borç iki ekranda ters işaretle görünüyor; borç kayıtları ayın gelir ve gider "
             "toplamına giriyor ve aynı karşı tarafa açılan iki borç birleştirilmiyor"),
            ("Money Manager",
             "—",
             "Cari kavramı yok; alacak ancak sıradan bir hesap açılarak taklit edilebilir"),
            ("Hesap Defterim",
             "Defter mantığı bozulmuyor: her satır gerçekleşmiş bir para hareketi",
             "Veresiye ürünün dışına çıkarılmış; kullanıcı ikinci bir uygulama kurmaya "
             "yönlendiriliyor"),
            ("KolayBi",
             "Cari birinci sınıf: vade günü, sabit iskonto ve durumu olan açılış bakiyesi "
             "taşıyor; personel de bir karşı taraf",
             "Cari kartı çok alanlı: vade, iskonto, durumu olan açılış bakiyesi ve proje tek "
             "formda soruluyor; karşı taraf açmak canlı ürünlerdeki bir hesap açmaktan uzun bir "
             "iş"),
        ],
        "soru": [
            "Tahsil edilmemiş alacak, elimdeki parayla aynı listede mi durmalı? Bluecoins ikisini "
            "aynı listede tutuyor ama alacağı kartla aynı bölüme koyuyor; Wallet ayrı bir "
            "ekrana alıyor.",
            "Bir kaydın bakiyeye dokunup dokunmayacağı kullanıcıya sorulmalı mı? Wallet bunu "
            "soruyor ve sonucunu yazıyor — dokuz üründe tek örnek.",
            "Vade kime ait olmalı: borcun kendisine mi, karşı tarafa mı? İki üründe iki ayrı yer.",
            "Alacak ve tahsilat ayın gelir/gider toplamına girmeli mi? İki ürün iki zıt cevap "
            "veriyor ve ikisi de kendi modeline göre tutarlı.",
            "Aynı tutarın iki ekranda ters işaretle görünmesi kaçınılabilir mi? Kart bölümündeki "
            "iki borç sayısıyla aynı desen burada da çıkıyor.",
        ],
    },
]

EKSIKLER = [
    ("Wallet", "Borcun tamamen kapanması ve Closed sekmesine düşmesi", "4.3b",
     "Kalan 7.000'i kapatmak mevcut test verisini bozar; ayrı bir kontrol borcu gerekir",
     "Önerilmez"),
    ("Wallet", "Borcu artırma kolunun sonucu (Increase debt)", "4.3b",
     "Kolun varlığı görüldü; artırılan borcun karta ve rapora etkisi okunmadı", "Orta"),
    ("Goodbudget", "Debt hesap türünün tamamı", "4.1",
     "Ücretli paket; ikinci hesap açılamıyor", "Önerilmez"),
]
