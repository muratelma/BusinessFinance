# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 8 · Veri: disa aktarma, yedek ve yardimci araclar — tek icerik kaynagi."""

NO = 8
BASLIK = "Veri: dışa aktarma, yedek ve yardımcı araçlar"
ANA_SORU = "Kayıtlar uygulamanın dışına nasıl çıkıyor, nerede duruyor ve kaybolursa ne oluyor?"
EN_AZ_KARE = 13

GIRIS = [
    "Önceki yedi bölüm kayıtların içeride nasıl davrandığını izledi. Bu bölüm dışarı bakıyor: "
    "veri hangi biçimde çıkıyor, nereye yazılıyor ve cihaz kaybolursa ne oluyor.",
    "Sonunda bölümün ikinci yarısı var: ürünlerin finansal akışın dışında taşıdığı yardımcı "
    "araçlar. Küçük görünüyorlar ama kullanıcının uygulamadan ne beklediğini gösteriyorlar.",
]
GIRMEZ = [
    "Raporun içeriği ve dönem seçimi → Bölüm 7",
    "Silinen kaydın toplamlara etkisi → Bölüm 1",
    "Ekranların görsel düzeni → Belge 1 §10",
]
KAPSAM = [
    ("Hesap Defterim", ["canli"], "Dışa aktarma, klasör uyarısı ve yedek diyaloğu kareli."),
    ("Bluecoins", ["canli"], "İçe aktarma girişleri, çöp kutusu ve seyahat modu kareli."),
    ("Money Manager", ["canli"], "Yedekleme ve PC bağlantısı girişleri kareli."),
    ("Goodbudget", ["canli"], "Ek ve dosya yüzeyi menüde arandı."),
    ("Wallet", ["canli"], "Dışa aktarma formu çekmecenin katlanmış menüsünde kareli."),
    ("KolayBi", ["kaynak"], "Cari ekstresi ve PDF önizlemesi destek görselinde."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], None),
]
ACILIS_DAYANAK = (
    "Rozetler kanıt düzeyini gösterir. Bu bölümde çoğu iddia bir yüzeyin varlığına dayanıyor; "
    "dışa aktarılan dosyanın içeriği yalnız Hesap Defterim'de açıldı."
)

SAYFALAR = [
    {"tur": "acilis"},

    # ------------------------------------------------------------------ 8.1
    {
        "tur": "yanyana", "no": "8.1",
        "baslik": "Veri dışarı nasıl çıkıyor",
        "giris": "Çıkış yolları ürüne göre değişiyor: dosya olarak cihaza, e-posta eki olarak, "
                 "ya da üç biçimden birinde. İçeri alma tarafında ise yalnız dosya biçimleri "
                 "var; Bluecoins'in içe aktarması iki biçimle sınırlı.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "c-hd1", "e": "E0427", "ad": "Hesap Defterim", "etiket": "Üretilen PDF",
             "kirpma": (0, 120, 1080, 1515),
             "satirlar": ["Tarih · Notlar · Gelir · Gider · Denge.",
                          "Açılış 20.000 Gelir sütununda.",
                          "Aktarım bacakları Gider satırı."]},
            {"k": "c-hd2", "e": "E0176", "ad": "Hesap Defterim", "etiket": "Dosya nereye yazıldı",
             "kirpma": (0, 700, 1080, 2330),
             "satirlar": ["\"Veriler … kasadefteri adlı bir klasöre kaydedilir.\"",
                          "Uyarı bir klasör adı veriyor.",
                          "Altında Android paylaşma sayfası."]},
            {"k": "c-mm", "e": "E0254", "ad": "Money Manager", "etiket": "Ayarlar ızgarası",
             "kirpma": (0, 120, 1080, 1480),
             "satirlar": ["Yedekle ve PC'den Yönet ayrı girişler.",
                          "CalcBox ve Giriş Kodu da burada.",
                          "Dışa aktarma rapor ekranından e-postayla."]},
            {"k": "c-wl", "e": "E0428", "ad": "Wallet", "etiket": "Dışa aktarma formu",
             "kirpma": (0, 120, 1080, 1800),
             "satirlar": ["Hesap · tür · ödeme tipi · tarih aralığı.",
                          "Aktarımlar dahil edilsin mi ayrıca soruluyor.",
                          "PDF / XLS / CSV."]},
            {"k": "c-bc", "e": "E0445", "ad": "Bluecoins", "etiket": "Veri Yönetimi",
             "kirpma": (0, 120, 1080, 1415),
             "satirlar": ["Yedek: telefon hafızası.",
                          "İçe aktarma yalnız Excel (.csv) ve QIF.",
                          "Banka ekstresi okuyan bir giriş yok."]},
        ],
        "notlar": [
            "Hesap Defterim iki ayrı çıkış yüzeyi taşıyor: bütün hesapları kapsayan dönemli bir "
            "dışa aktarma ve defter başına dönemsiz bir tane. İkisi de PDF ve Excel sunuyor.",
            "Money Manager'ın rapor ekranında ayrıca \"Excel(.xlsx) e-posta olarak gönder\" "
            "girişi var; dosya cihaza yazılmak yerine doğrudan paylaşılıyor.",
            ("Hesap Defterim'in PDF'i ekrandaki tanımı dosyaya da taşıyor: Ana Hesap defterinde "
             "açılış bakiyesi Gelir sütununda, iki aktarım bacağı Gider satırı olarak duruyor; "
             "Toplam Gelir 47.500 açılışı da içeriyor. Bölüm 1'deki toplam dosyada da aynı.",
             "E0427"),
            ("Ürünün söylediği klasör ise oluşmuyor: cihazın dosya sisteminde kasadefteri adlı bir "
             "klasör bulunamadı; PDF'ler Documents altındaki Hesap Defterim klasörüne, Excel "
             "uygulamanın kendi veri dizinine yazılmış. Bu bir koşum kaydıdır; dosya sistemi "
             "araması kareye alınmadı.", "E0447 · koşum kaydı"),
        ],
        "sag_notlar": [
            ("Hesap Defterim'de defter başına dışa aktarmada dönem seçimi yok, yalnız PDF ve "
             "Excel satırları var.", "E0175"),
            ("Wallet'ta dışa aktarma Ayarlar'da ve kayıt listesinin menüsünde yok; çekmecenin "
             "katlanmış Others bölümünde duruyor.", "E0450"),
            ("Bluecoins'in yazıcı simgesi üç çıktı sunuyor: PDF veya yazıcı, Excel (.csv) ve "
             "HTML. Dosya üretilmedi.", "E0444 · E0105"),
        ],
    },

    # ------------------------------------------------------------------ 8.2
    {
        "tur": "yanyana", "no": "8.2",
        "baslik": "Veri nerede duruyor, kaybolursa ne oluyor",
        "giris": "Bir ürün bu soruyu kendisi soruyor ve cevabı açıkça yazıyor. Diğerlerinde "
                 "cevap ayarların içinde bir giriş olarak duruyor; silinen kaydın ikinci bir "
                 "şansı olup olmadığı da ürüne göre değişiyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "v-hd1", "e": "E0149", "ad": "Hesap Defterim", "etiket": "Yedekleme kapalı uyarısı",
             "kirpma": (0, 120, 1080, 1760),
             "satirlar": ["\"Kayıtlarınızı sunucularımızda saklamıyoruz…\"",
                          "Telefon değişiminde kayıt kaybı uyarısı.",
                          "Atla / Yedeklemeyi Aç."]},
            {"k": "v-hd2", "e": "E0143", "ad": "Hesap Defterim", "etiket": "Silinmiş işlemler",
             "kirpma": (0, 120, 1080, 1415),
             "satirlar": ["Silinen kayıt ayrı bir listede.",
                          "Geri yükleme bu listeden.",
                          "Kalıcı silme ikinci onay istiyor."]},
            {"k": "v-bc", "e": "E0089", "ad": "Bluecoins", "etiket": "Çöp kutusu",
             "kirpma": (0, 120, 1080, 1415),
             "satirlar": ["Ayrı bir çöp kutusu yüzeyi var.",
                          "Karede kutu boş.",
                          "Boş durumda yönlendirme yok."]},
            {"k": "v-gb", "e": "E0128", "ad": "Goodbudget", "etiket": "Kayıt menüsü",
             "kirpma": (0, 120, 1080, 1415),
             "satirlar": ["Kaydın menüsünde yalnız Help var.",
                          "Bu yüzeyde dosya veya fotoğraf eki yok."]},
        ],
        "notlar": [
            "Hesap Defterim'in uyarısı bu bölümün en açık cümlesi: ürün verinin yalnız cihazda "
            "durduğunu ve yedek kapalıysa geri yüklemenin mümkün olmadığını kendi ağzıyla "
            "söylüyor. Bulut bir varsayım değil, bir tercih olarak sunuluyor.",
            ("Geri alma iki üründe ayrı bir yüzey: Hesap Defterim'in Silinmiş işlemler listesi ve "
             "Bluecoins'in çöp kutusu. Money Manager, Wallet ve Goodbudget'ta menüler ve ayarlar "
             "tarandı; böyle bir katman yok.", "E0143 · E0089 · E0415 · E0452 · E0453"),
            ("Bluecoins'in çöp kutusu çalışıyor: silinen kayıt oraya düşüyor ve geri "
             "yükleniyor, ama geri yüklenen kayıt işlem listesine ancak uygulama yeniden "
             "açılınca geliyor. Bu iki ayrı kullanıcı kontrolüyle kayıtlı; rapor toplamı ise "
             "geri yüklemeyi hemen sayıyor.", "E0401 · E0402 · kullanıcı kontrolü"),
        ],
        "sag_notlar": [
            ("Hesap Defterim'de kalıcı silme ikinci bir onay diyaloğu açıyor.", "E0168"),
            ("Money Manager'ın ayarlarında Yedekle girişi var; içeriği açılmadı.", "E0415"),
        ],
    },

    # ------------------------------------------------------------------ 8.3
    {
        "tur": "soru", "no": "8.3", "baslik": "Kaynakta çıktı: dosya değil, belge",
        "giris": "KolayBi'de dışa aktarma bir yedek alma işi değil, bir belge üretme işi. "
                 "Ekstre oluşturmadan önce hangi sütunların görüneceği seçiliyor, sonra çıktı "
                 "önizleniyor ve e-postayla gönderilebiliyor. Alanların varlığı kanıtlı; "
                 "çıktının gerçek içeriği ölçülmedi.",
        "dikey": True, "not_genislik": 300,
        "sekiller": [
            {"k": "kb-ekstre", "e": "E0196", "etiket": "KolayBi · Cari Ekstresi Oluştur",
             "kirpma": (96, 112, 1305, 740), "genislik": 350,
             "satirlar": ["Tarih aralığı, para birimi ve açıklama.",
                          "Yedi isteğe bağlı sütun anahtarı.",
                          "Kapat / Yazdır / Oluştur."]},
            {"k": "kb-pdf", "e": "E0197", "etiket": "KolayBi · PDF önizlemesi",
             "kirpma": (96, 112, 1305, 740), "genislik": 350,
             "satirlar": ["Tek sayfalık Cari Hesap Ekstresi.",
                          "Borç, alacak ve bakiye kolonları.",
                          "Yeni sekme, e-posta ve kapat."]},
        ],
        "notlar": [
            "Sütun anahtarları çıktının biçimini kullanıcıya bırakıyor: aynı ekstre farklı "
            "alıcılar için farklı görünebiliyor. Canlı beş üründe dışa aktarmanın biçimi "
            "sabit — yalnız PDF mi Excel mi ve hangi dönem sorulabiliyor.",
            "Çek ve senet listelerinde de ayrı bir Dışarıya Aktar girişi ve bordro kavramı var; "
            "belge üretme bu üründe tek bir yerde değil, yüzeylere dağılmış.",
        ],
        "sag_notlar": [
            ("Çekler listesinde Toplu Çek Ekle, Bordrolar ve Dışarıya Aktar girişleri birlikte "
             "duruyor.", "E0209"),
        ],
        "urunler_baslik": "Aynı soruda kaynaktan okunan üç ürün",
        "urunler": [
            ("Paraşüt", "beyan",
             "Kaynak KDV raporunun Excel'e aktarılabildiğini yazıyor; ayrıca muhasebeciye canlı "
             "erişim veriliyor — dosya alışverişi yerine aynı veriye ortak erişim. "
             "Bölüm 9'da kurulur.", "parasut.com kılavuzu"),
            ("Logo İşbaşı", "beyan",
             "Kaynak bir müşavir portalından söz ediyor: müşavir müşterinin verisine erişiyor ve "
             "müşteri onu eklediğinde adına işlem yapabiliyor. Bölüm 9'da kurulur.", "isbasi.com"),
            ("QuickBooks Solopreneur", "beyan",
             "Kaynağa göre veri buluta bağlı hesaplardan geliyor; muhasebeci erişimi ve dışa "
             "aktarma yardım merkezinde net belgelenmemiş.", "Intuit yardım merkezi"),
        ],
    },

    # ------------------------------------------------------------------ 8.4
    {
        "tur": "tablo", "no": "8.4", "baslik": "Veri yüzeyleri, ürün ürün",
        "giris": "Dışa aktarma biçimi, verinin nerede durduğu, geri alma katmanı ve içe "
                 "aktarma.",
        "sutunlar": [("", 15), ("Hesap Defterim", 17), ("Money Manager", 17), ("Bluecoins", 17),
                     ("Goodbudget", 17), ("Wallet", 17)],
        "boy": 8.4,
        "satirlar": [
            [
                "Dışa aktarma biçimi",
                {"t": "PDF ve Excel; dönemli ve dönemsiz iki yol", "tur": "canli", "d": "E0169 · E0427"},
                {"t": "Excel, e-posta eki olarak", "tur": "canli", "d": "E0250"},
                {"t": "PDF veya yazıcı · Excel (.csv) · HTML; dosya üretilmedi", "tur": "canli", "d": "E0444"},
                {"t": "Bu taramada bulunamadı", "tur": "yok", "d": "E0128"},
                {"t": "PDF · XLS · CSV; katlanmış bir menüde", "tur": "canli", "d": "E0428 · E0450"},
            ],
            [
                "Dosya nereye yazılıyor",
                {"t": "Ürün kasadefteri klasörünü söylüyor, ama bu klasör oluşmuyor", "tur": "kosum", "d": "E0176 · E0447 · koşum kaydı"},
                {"t": "Doğrudan paylaşılıyor; cihaza yazma görülmedi", "tur": "canli", "d": "E0250"},
                {"t": "Ölçülmedi", "tur": "yok", "d": "E0091"},
                {"t": "Ölçülmedi", "tur": "yok", "d": "E0128"},
                {"t": "Ölçülmedi", "tur": "yok", "d": "E0275"},
            ],
            [
                "Veri nerede duruyor",
                {"t": "Yalnız cihazda; yedek kapalıysa geri yükleme yok", "tur": "canli", "d": "E0149"},
                {"t": "Yedekle ve PC'den Yönet girişleri var", "tur": "canli", "d": "E0254"},
                {"t": "Ayarlarda yedek girişleri var; denenmedi", "tur": "canli", "d": "E0095"},
                {"t": "Hesap tabanlı; Last Backup satırı ekranda", "tur": "canli", "d": "E0417"},
                {"t": "Hesap tabanlı; menüde Bank Sync girişi", "tur": "canli", "d": "E0275"},
            ],
            [
                "Silinen kaydın ikinci şansı",
                {"t": "Silinmiş işlemler listesi + Geri Yükle", "tur": "canli", "d": "E0143"},
                {"t": "Yok — menü ızgarasında çöp kutusu kalemi yok", "tur": "canli", "d": "E0415"},
                {"t": "Çöp kutusu çalışıyor; geri yüklenen kayıt listeye yeniden açılınca geliyor", "tur": "kosum", "d": "E0089 · E0401 · E0402 · kullanıcı kontrolü"},
                {"t": "Tek onayla siliniyor; ayarlarda geri alma yok", "tur": "canli", "d": "E0125 · E0453"},
                {"t": "Yok — çekmece ve ayarlarda çöp kutusu yok", "tur": "canli", "d": "E0452"},
            ],
            [
                "Kayda dosya eklenebiliyor mu",
                {"t": "Evet — Kamera / Galeri / PDF; okuma yok", "tur": "canli", "d": "E0157"},
                {"t": "Formda kamera simgesi var", "tur": "canli", "d": "E0229"},
                {"t": "Formun üstünde ataç simgesi; okuma görülmedi", "tur": "canli", "d": "E0448"},
                {"t": "Bu yüzeyde bulunamadı", "tur": "yok", "d": "E0128"},
                {"t": "Kayıt ayrıntısında Add receipt girişi", "tur": "canli", "d": "E0294"},
            ],
        ],
        "notlar": [
            "İki ürün verinin cihazda mı bulutta mı durduğunu ekranda söylüyor: biri yedek "
            "uyarısıyla, öteki hesap ekranındaki Last Backup satırıyla. Diğer üçünde bu bilgi "
            "ayarların içinde kalıyor.",
            "Dosya eki dört üründe var ama hiçbirinde okunan bir şey değil: ek yalnız saklanıyor, "
            "tutar veya tarih ondan çıkarılmıyor.",
        ],
    },

    # ------------------------------------------------------------------ 8.5
    {
        "tur": "yanyana", "no": "8.5",
        "baslik": "Finansal akışın dışındaki araçlar",
        "giris": "Üç ürün, para akışıyla doğrudan ilgisi olmayan küçük araçlar taşıyor. Bunlar "
                 "ürünün kullanıcı hakkındaki varsayımını gösteriyor: kim, nerede, hangi "
                 "koşulda kullanıyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "a-hd1", "e": "E0165", "ad": "Hesap Defterim", "etiket": "Not Defteri",
             "kirpma": (0, 120, 1080, 2200),
             "satirlar": ["Dönem çipli bir yapılacaklar listesi.",
                          "Tamamlandı / Beklemede / Toplam sayaçları.",
                          "Finansal kayıtla bağı yok."]},
            {"k": "a-hd2", "e": "E0166", "ad": "Hesap Defterim", "etiket": "Nakit Hesap Makinesi",
             "kirpma": (0, 120, 1080, 1415),
             "satirlar": ["Kupür × adet tablosu; canlı toplam.",
                          "200 × 5 = 1.000 satırı görünüyor.",
                          "Kasa sayan kullanıcı için."]},
            {"k": "a-bc", "e": "E0100", "ad": "Bluecoins", "etiket": "Seyahat modu",
             "kirpma": (0, 120, 1080, 1915),
             "satirlar": ["Çekmecede bir anahtar; Ayarlar ayrı bir kalem. Koşumda kapalı.",
                          "Açıldığında etiket seçici çıkıyor.",
                          "Yolculukta ayrı bir kip varsayımı."]},
        ],
        "notlar": [
            "Nakit Hesap Makinesi bu araçların en anlatıcısı: kupürleri tek tek sayan bir "
            "kullanıcı varsayıyor. Gün sonunda kasa sayan esnafın işi, finansal kayıt "
            "uygulamasının değil — ama ürün ikisini aynı yere koymuş.",
            ("Money Manager'ın CalcBox ve PC'den Yönet girişleri uygulamanın içinde değil: "
             "CalcBox ayrı bir ürünün mağaza sayfasını açıyor, PC'den Yönet ücretli sürüme "
             "yükseltme ekranına gidiyor.", "E0455 · E0456"),
            ("Bluecoins'in seyahat modu anahtarı açılınca bir etiket seçici çıkıyor. Etiket "
             "seçildikten sonra ürünün ne yaptığı görülmedi.", "E0454"),
        ],
        "sag_notlar": [
            ("Hesap Defterim menüsünde ayrıca iki ayrı ürüne çapraz tanıtım var: Veresiye "
             "Defteri ve Gelir Gider.", "E0177"),
            ("KolayBi'de de finansal olmayan bir araç var: hatırlatıcısı ve görünürlük ayarı "
             "olan notlar.", "E0212"),
        ],
    },

    # ------------------------------------------------------------------ 8.6
    {
        "tur": "akis", "no": "8.6", "baslik": "Verinin uygulamadan çıkış yolu",
        "giris": "Kayıt yazıldıktan sonra uygulamadan çıkana kadar üç adım. İlk adımda ürünler "
                 "zaten ayrışmış oluyor: veri nerede duruyor.",
        "adimlar": [
            {
                "baslik": "Veri bir yerde duruyor",
                "dallar": [
                    {"urunler": "Hesap Defterim", "vurgu": True,
                     "metin": "Yalnız cihazda. Ürün bunu kendisi yazıyor: yedek kapalıysa geri "
                              "yükleme yok.", "d": "E0149"},
                    {"urunler": "Goodbudget · Wallet",
                     "metin": "Hesap tabanlı; veri bir hesaba bağlı ve ekranda yedek ya da "
                              "senkron izi görünüyor.", "d": "E0417"},
                    {"urunler": "Money Manager · Bluecoins",
                     "metin": "Ayarlarda yedek girişleri var; nereye yedeklendiği bu koşumda "
                              "açılmadı.", "d": "E0254"},
                ],
            },
            {
                "baslik": "Dışarı çıkarılıyor",
                "dallar": [
                    {"urunler": "Hesap Defterim",
                     "metin": "Dosya olarak cihaza: PDF veya Excel, dönemli ya da dönemsiz. "
                              "Ürün bir klasör adı söylüyor ama dosya başka yere yazılıyor.",
                     "d": "E0427 · E0447"},
                    {"urunler": "Money Manager",
                     "metin": "E-posta eki olarak Excel; dosya cihaza yazılmadan paylaşılıyor.",
                     "d": "E0250"},
                    {"urunler": "Bluecoins · Wallet",
                     "metin": "Üç biçim sunuluyor; Wallet'ta form katlanmış bir menüde. Dosya "
                              "üretilmedi.", "d": "E0444 · E0428"},
                ],
            },
            {
                "baslik": "Geri alınabiliyor mu",
                "dallar": [
                    {"urunler": "Hesap Defterim · Bluecoins",
                     "metin": "Silinen kayıt ayrı bir listede bekliyor; geri yükleme oradan.",
                     "d": "E0143"},
                    {"urunler": "Goodbudget", "vurgu": True,
                     "metin": "Tek onayla siliniyor; ayarlarda kurtarma katmanı yok.",
                     "d": "E0125 · E0453"},
                ],
            },
        ],
        "notlar": [
            "Bu bölümün akışı diğerlerinden kısa, çünkü ölçülen şey çoğunlukla davranış değil "
            "yüzeyin varlığı: dışa aktarılan dosya yalnız bir üründe açıldı.",
            "Kaynak taraf bu yolu bambaşka kuruyor: dosya çıkarmak yerine muhasebeciye aynı "
            "veriye erişim veriyor. Bu Bölüm 9'da kurulur.",
        ],
    },

    # ------------------------------------------------------------------ 8.7
    {
        "tur": "cikarim", "no": "8.7", "baslik": "Neden ayrışıyorlar",
        "giris": "Bu bölümde ayrışmanın kaynağı ürünün verinin kime ait olduğu hakkındaki "
                 "varsayımı: cihazdaki bir defter mi, bir hesabın içeriği mi, yoksa "
                 "paylaşılacak bir belge mi.",
        "mekanizma": [
            {
                "baslik": "Cihazdaki defter, verinin sahibini kullanıcı yapıyor",
                "metin": [
                    "Hesap Defterim verinin yalnız cihazda durduğunu ekranda yazıyor ve yedek "
                    "kapalıysa geri yüklemenin mümkün olmadığını söylüyor. Dışa aktarma da bu "
                    "mantığın devamı: dosya cihaza yazılıyor ve ürün bir klasör adı veriyor — "
                    "ama o klasör oluşmuyor, dosya başka bir yere yazılıyor.",
                    "Bu, kullanıcıyı tam sahibi yapıyor — ve tam sorumlu. Telefon kaybolursa "
                    "kayıt da kayboluyor ve ürünün yapabileceği bir şey yok. Dosyasını bulmak da "
                    "kullanıcıya kalıyor: ürünün gösterdiği yerde dosya yok.",
                    "Aynı ürünün Silinmiş işlemler listesi ve iki aşamalı kalıcı silme onayı bu "
                    "sorumluluğun karşılığı: veri kolay kaybolmasın diye ürün içinde iki kapı "
                    "var.",
                ],
                "dayanak": "E0149, E0176, E0447, E0143, E0168, koşum kaydı (dosya sistemi)",
            },
            {
                "baslik": "Hesap tabanlı ürünlerde soru hiç sorulmuyor",
                "metin": [
                    "Goodbudget ve Wallet'ta veri bir hesaba bağlı; ekranda yedek zamanı ya da "
                    "senkron girişi görünüyor. Kullanıcı verinin nerede durduğunu sormuyor, "
                    "çünkü cevap ürünün kendisinde.",
                    "Bedeli şu: dışa aktarma bu iki üründe öne çıkan bir yüzey değil. "
                    "Goodbudget'ta bu taramada hiç bulunamadı; Wallet'ta var ama Ayarlar'da ve "
                    "kayıt menüsünde değil, çekmecenin katlanmış bir bölümünde. Veri ürünün "
                    "içinde güvende olduğu için dışarı çıkarmanın aciliyeti azalıyor.",
                ],
                "dayanak": "E0417, E0275, E0128, E0450, E0428",
            },
            {
                "baslik": "Kaynakta çıktı bir dosya değil, bir belge",
                "metin": [
                    "KolayBi'de ekstre üretmeden önce hangi sütunların görüneceği seçiliyor, "
                    "sonra çıktı önizleniyor ve e-postayla gönderilebiliyor. Yani çıktı bir "
                    "yedek değil, karşı tarafa gidecek bir belge.",
                    "Canlı ürünlerde dışa aktarma bir süzgeç: biçim, dönem ve en çok hesap ile "
                    "kayıt türü soruluyor. Çıktının kime gideceği ürünün sorusu değil.",
                    "Kaynak taraf bunu bir adım daha ileri götürüyor: iki üründe muhasebeciye "
                    "dosya göndermek yerine aynı veriye erişim veriliyor.",
                ],
                "dayanak": "E0196, E0197, E0209, E0169, E0428, parasut.com kılavuzu, isbasi.com",
            },
            {
                "baslik": "Dosya eki her yerde var, okunan hiçbir yerde",
                "metin": [
                    "Dört üründe kayda dosya veya fotoğraf eklenebiliyor: kamera simgesi, "
                    "galeri seçimi, PDF ekleme, Add receipt girişi. Hiçbirinde ekten tutar ya da "
                    "tarih okunmuyor.",
                    "Ek bu ürünlerde bir kanıt saklama yeri: kaydı doğrulayan belge duruyor ama "
                    "kaydı o belge üretmiyor. Fişten kayıt üreten tek yol kaynak metinlerinde "
                    "anlatılıyor ve canlı hiçbir üründe görülmedi.",
                ],
                "dayanak": "E0157, E0229, E0294, E0448, E0128",
            },
        ],
        "kazanc": [
            ("Hesap Defterim",
             "Verinin nerede durduğunu ve kaybolabileceğini açıkça yazıyor; iki ayrı dışa "
             "aktarma yolu, ekranla aynı tanımı taşıyan bir PDF ve silinen kayıt için ayrı bir "
             "liste var",
             "Veri yalnız cihazda: yedek kapalıysa geri yükleme yok ve sorumluluk tamamen "
             "kullanıcıda; dışa aktarma uyarısı oluşmayan bir klasörü gösteriyor"),
            ("Money Manager",
             "Dışa aktarma rapor ekranından e-posta ekiyle tek adımda",
             "Masaüstünden bağlanma ücretli sürümde; yedek girişinin içeriği yüzeyden "
             "okunmuyor"),
            ("Bluecoins",
             "Üç biçimde çıktı ve çalışan bir çöp kutusu var",
             "İçe aktarma yalnız CSV ve QIF, banka ekstresi okumuyor; geri yüklenen kayıt "
             "listeye ancak uygulama yeniden açılınca geliyor"),
            ("Goodbudget",
             "Veri hesaba bağlı; yedek zamanı zarf ekranının üstünde yazılı",
             "Dışa aktarma bu taramada bulunamadı ve silme tek onayla yapılıyor"),
            ("KolayBi",
             "Çıktı bir belge: sütunları seçilebiliyor, önizleniyor ve e-postayla "
             "gönderilebiliyor",
             "Çıktının biçimi her seferinde soruluyor: tarih aralığı, para birimi, açıklama ve "
             "yedi sütun anahtarı"),
        ],
        "soru": [
            "Veri cihazda mı durmalı, hesapta mı? İki model iki ayrı sorumluluk dağılımı "
            "üretiyor ve kullanıcı bunu ancak ürün yazarsa öğreniyor.",
            "Dışa aktarma bir yedek mi, bir belge mi? Kaynak taraf ikisini ayırıyor; canlı "
            "ürünlerde çıktı yalnız bir dosya.",
            "Silinen kaydın ikinci bir şansı olmalı mı? İki üründe var, üçünde taranan "
            "yüzeylerde yok.",
            "Kayda eklenen belge okunmalı mı, yoksa yalnız saklanmalı mı? Canlı beş üründe "
            "okuma yok; ek bir kanıt, bir girdi değil.",
        ],
    },
]

EKSIKLER = [
    ("Hesap Defterim", "Excel dosyasının içeriği ve dosya sistemindeki yerin karesi", "8.1",
     "PDF açıldı; Excel açılmadı, klasör araması karesiz bir koşum kaydı", "Orta"),
    ("Wallet, Bluecoins", "Dışa aktarılan dosyanın içeriği", "8.1",
     "Formlar görüldü; bir dosya üretip açmak gerekir", "Orta"),
    ("Bluecoins", "Seyahat modunda etiket seçildikten sonrası", "8.5",
     "Anahtar etiket seçiciyi açıyor; bir etiket seçip ana ekranı okumak kaldı", "Düşük"),
]
