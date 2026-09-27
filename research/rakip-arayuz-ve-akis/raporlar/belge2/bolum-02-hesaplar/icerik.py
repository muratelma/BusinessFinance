# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 2 · Hesaplar ve para aktarimi — tek icerik kaynagi."""

NO = 2
BASLIK = "Hesaplar ve para aktarımı"
ANA_SORU = "Para hesaplar arasında gezdiğinde ne üretiyor — bir kayıt mı, iki mi, hiç mi?"
EN_AZ_KARE = 12

GIRIS = [
    "Bu bölüm iki soruyu izliyor. Birincisi hesabın başlangıç parasının nereye yazıldığı; "
    "ikincisi aynı paranın bir hesaptan diğerine geçerken kaç satır ürettiği ve o satırların "
    "ayın toplamına ne yaptığı.",
    "İkisi birbirine bağlı: açılış bakiyesini bir kayıt olarak yazan ürün, o kaydı ayın "
    "toplamına da sokma riskini alır. Bölüm 1'de görülen 51.200'ün kaynağı burasıdır.",
]
GIRMEZ = [
    "Kart borcu ve kart ödemesi → Bölüm 3",
    "Karşı tarafa borç ve tahsilat → Bölüm 4",
    "Hesap listesinin görsel düzeni → Belge 1 §5",
    "Raporun dönem seçimi → Bölüm 7",
]
KAPSAM = [
    ("Money Manager", ["canli"], "Açılış diyaloğu, defter karşılığı ve toplam anahtarı kareli."),
    ("Bluecoins", ["canli"], "Aktarım formu ve listedeki iki bacak kareli."),
    ("Wallet", ["canli"], "Aktarımın listedeki izi kareli; açılış alanı bulunamadı."),
    ("Hesap Defterim", ["canli", "kosum"], "Aktar formu kareli; bacak bağımsızlığı koşum kaydı."),
    ("Goodbudget", ["canli"], "Hesap kurulumu kareli; aktarım ücretli pakette."),
    ("KolayBi", ["kaynak"], "Finans sekmeleri destek görselinde."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], None),
]
ACILIS_DAYANAK = (
    "Rozetler kanıt düzeyini gösterir: ölçülen davranış Canlı kare, kareye alınmamış koşum "
    "gözlemi Koşum kaydı, kanıttan çıkarılan neden Çıkarım."
)

SAYFALAR = [
    {"tur": "acilis"},

    # ------------------------------------------------------------------ 2.1
    {
        "tur": "yanyana", "no": "2.1",
        "baslik": "Açılış bakiyesi bir alan mı, bir kayıt mı",
        "giris": "Hesabın başlangıç parası dört üründe dört ayrı yere yazılıyor. Money Manager "
                 "bunu kullanıcıya açıkça soruyor; Hesap Defterim defterin ilk satırı yapıyor; "
                 "Goodbudget hesabın bir özelliği olarak tutuyor. Wallet'ta böyle bir alan "
                 "aranıp bulunamadı.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "a-mm1", "e": "E0239", "ad": "Money Manager", "etiket": "Hesap formu ve sorulan soru",
             "satirlar": ["Tutar 2.000 ve Toplama Dahil Et anahtarı.",
                          "Kaydedince çıkan diyalog soruyor:",
                          "fark İşlemler'de gösterilsin mi?"]},
            {"k": "a-mm2", "e": "E0240", "ad": "Money Manager", "etiket": "Aynı açılışın defterdeki karşılığı",
             "satirlar": ["Para Yatırma 2.000 / Çekme 0.",
                          "Açılış defterde bir satır olarak duruyor."]},
            {"k": "a-hd", "e": "E0145", "ad": "Hesap Defterim", "etiket": "Hesap Eklem",
             "satirlar": ["Açılış bilançosu [İsteğe bağlı].",
                          "+ / − radyosu: açılış eksi de olabiliyor.",
                          "Tarih alanı açılışa ait."]},
            {"k": "a-gb", "e": "E0113", "ad": "Goodbudget", "etiket": "Ana Hesap kuruldu",
             "satirlar": ["Açılış 20.000 hesabın kendi değeri.",
                          "İşlem listesinde ayrı bir satır olarak görünmüyor."]},
        ],
        "notlar": [
            "Bluecoins hesap açılışlarını işlem listesine ayrı satırlar olarak yazmış; açılış "
            "satırlarının düştüğü günün başlığı bu yüzden sıfır çıkmıyor.",
            ("Wallet'ta açılış bakiyesi alanı yok. Dördüncü hesap ücretsiz pakette açılamadığı "
             "için mevcut hesabın düzenleme formu tarandı: renk, içe aktarma e-postası, "
             "istatistikten çıkarma, arşiv, en az ve en çok bakiye bildirimi var; açılış bakiyesi "
             "yok.", "E0437 · E0283"),
        ],
        "sag_notlar": [
            ("Hesap Defterim'de açılış defterin ilk satırıdır ve Ağustos listesinde 20.000 olarak "
             "görünüyor; ayın Alındı toplamına da bu yüzden giriyor.", "E0137"),
            ("Money Manager'ın sorduğu soru bir tercih: açılış farkı isteğe bağlı olarak işlem "
             "listesinde gösteriliyor.", "E0239"),
        ],
    },

    # ------------------------------------------------------------------ 2.2
    {
        "tur": "yanyana", "no": "2.2",
        "baslik": "Aktarım formu: aynı iş, beş ayrı form",
        "giris": "Beş üründe de aktarım birinci sınıf: üçünde işlem formunun üçüncü türü, "
                 "ikisinde kendi ekranı. Ayrışma formun sorduğu alanlarda — ikisi aktarım ücreti "
                 "soruyor, biri zarf sormuyor, ikisi yalnız iki hesap ve tutar istiyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "t-mm", "e": "E0462", "ad": "Money Manager", "etiket": "Havale formu",
             "satirlar": ["Havale, Gelir ve Gider'in yanında üçüncü tür.",
                          "Kaynak ve Giriş; tutarın yanında Harç.",
                          "Kategori alanı yok."]},
            {"k": "t-bc", "e": "E0435", "ad": "Bluecoins", "etiket": "Transfer formu",
             "satirlar": ["Kaynak ve hedef hesap.",
                          "Transfer ücreti ayrı blok:",
                          "kendi tutarı, hesabı ve kategorisi."]},
            {"k": "t-wl", "e": "E0436", "ad": "Wallet", "etiket": "Aktarım formu",
             "satirlar": ["Income / Expense / Transfer tek form.",
                          "From account → To account.",
                          "Ücret veya kategori alanı yok."]},
            {"k": "t-hd", "e": "E0147", "ad": "Hesap Defterim", "etiket": "Aktar formu",
             "satirlar": ["Miktar, Kimden, Kime, tarih, notlar.",
                          "Tek form; sonuç iki ayrı defter satırı.",
                          "Ayrı bir Aktar menü girişi var."]},
            {"k": "t-gb", "e": "E0119", "ad": "Goodbudget", "etiket": "Account Transfer",
             "satirlar": ["From · To · Amount · Description · Date.",
                          "Açıklama otomatik dolduruluyor.",
                          "Zarf alanı yok — aktarım zarfa uğramıyor."]},
        ],
        "notlar": [
            "Goodbudget'ta aktarım birinci sınıf ve ayrı bir ekran, ama formda zarf alanı "
            "bulunmuyor. Bölüm 1'de zarf ile hesabın iki ayrı defter olduğu görülmüştü; aktarım "
            "bu iki defterden yalnız hesabı ilgilendiriyor.",
            "Ücretsiz pakette tek hesap açılabildiği için gerçek aktarım koşulamadı: Kimden ve "
            "Kime açılırları aynı tek kaydı listeliyor.",
        ],
        "sag_notlar": [
            ("Bluecoins'te ücret aktarımın parçası değil, kendi hesabı ve kategorisi olan ayrı "
             "bir gider olarak soruluyor. Ücretli bir aktarım kaydedilmedi; iki bakiyeye "
             "etkisi ölçülmedi.", "E0435"),
            ("Goodbudget'ta aktarımın engellendiği an kareyle kayıtlı.", "E0120"),
        ],
    },

    # ------------------------------------------------------------------ 2.3
    {
        "tur": "tablo", "no": "2.3", "baslik": "3.000 bir hesaptan diğerine geçince ne oldu",
        "giris": "Aynı aktarımın dört üründeki sonucu; beşincisinde aktarım hiç "
                 "çalıştırılamadı. Toplam para dördünde de aynı kalıyor — ayrışma satır "
                 "sayısında ve ayın toplamında.",
        "sutunlar": [("", 15), ("Money Manager", 17), ("Bluecoins", 17), ("Wallet", 17),
                     ("Hesap Defterim", 17), ("Goodbudget", 17)],
        "boy": 8.4,
        "satirlar": [
            [
                "Toplam paraya etkisi",
                {"t": "Yok — Toplam 44.950", "tur": "canli", "d": "E0235"},
                {"t": "Yok — net 44.950", "tur": "canli", "d": "E0032"},
                {"t": "Yok — dönem toplamı 22.950 sabit", "tur": "canli", "d": "E0285"},
                {"t": "Yok — Denge 44.950", "tur": "canli", "d": "E0142"},
                {"t": "Aktarım koşulamadı; tek hesap sınırı", "tur": "yok", "d": "E0120"},
            ],
            [
                "Listede kaç satır",
                {"t": "Tek Havale satırı, gün başlığı 0/0", "tur": "canli", "d": "E0228"},
                {"t": "İki bacak, gün neti 0", "tur": "canli", "d": "E0022"},
                {"t": "İki satır: biri yeşil artı, biri kırmızı eksi", "tur": "canli", "d": "E0279"},
                {"t": "İki satır: Kime X / Kimden Y", "tur": "canli", "d": "E0140"},
                {"t": "Tek hesap sınırı; aktarım kurulamadı", "tur": "yok", "d": "E0120"},
            ],
            [
                "Ayın gelir/gider toplamı",
                {"t": "Etkilenmedi", "tur": "canli", "d": "E0231"},
                {"t": "Etkilenmedi", "tur": "canli", "d": "E0023"},
                {"t": "Etkilenmedi", "tur": "canli", "d": "E0280"},
                {"t": "Her iki bacak da toplama girdi: +3.000 ve −3.000", "tur": "canli", "d": "E0142"},
                {"t": "Tek hesap sınırı; ölçülemedi", "tur": "yok", "d": "E0120"},
            ],
            [
                "Aktarım ayrı bir tür mü",
                {"t": "Evet — Havale, Gelir/Gider'in yanında", "tur": "canli", "d": "E0229"},
                {"t": "Evet — TRANSFER sekmesi", "tur": "canli", "d": "E0029"},
                {"t": "Evet — ekleme menüsünde ayrı giriş; formun Transfer sekmesi", "tur": "canli", "d": "E0324 · E0436"},
                {"t": "Evet — ayrı Aktar ekranı", "tur": "canli", "d": "E0147"},
                {"t": "Evet — ayrı Account Transfer ekranı", "tur": "canli", "d": "E0119"},
            ],
            [
                "Form ne soruyor",
                {"t": "Kaynak · Giriş · tutar · Harç · not", "tur": "canli", "d": "E0462"},
                {"t": "Kaynak · hedef · tutar · transfer ücreti", "tur": "canli", "d": "E0435"},
                {"t": "From · To · tutar", "tur": "canli", "d": "E0436"},
                {"t": "Kimden · Kime · miktar · not", "tur": "canli", "d": "E0147"},
                {"t": "From · To · tutar · açıklama — zarf yok", "tur": "canli", "d": "E0119"},
            ],
        ],
        "notlar": [
            "Money Manager aktarımı listede tek bir Havale satırı olarak gösteriyor ve o günün "
            "gelir/gider başlığını 0/0 yazıyor; Bluecoins ve Wallet iki bacağı ayrı ayrı gösterip "
            "aynı günde netliyor. Üç ürün de aynı sonuca farklı yoldan varıyor.",
        ],
    },

    # ------------------------------------------------------------------ 2.4
    {
        "tur": "yanyana", "no": "2.4",
        "baslik": "İki bacak listede: netleşen üç ürün, bağımsız duran bir ürün",
        "giris": "Aktarımın listedeki izi ürünün toplamı nasıl koruduğunu da gösteriyor. Üç "
                 "üründe iki bacak aynı günde birbirini götürüyor; birinde iki satır birbirinden "
                 "habersiz duruyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "b-bc", "e": "E0022", "ad": "Bluecoins", "etiket": "İşlem listesi · Ağustos",
             "satirlar": ["3.000 aktarım ve 1.200 kart ödemesi ikişer bacak.",
                          "İki bacağın olduğu günlerin neti 0."]},
            {"k": "b-wl", "e": "E0285", "ad": "Wallet", "etiket": "Records",
             "satirlar": ["Aynı tarih ve notla iki satır.",
                          "Biri yeşil artı, biri kırmızı eksi.",
                          "Dönem toplamı 22.950 değişmiyor."]},
            {"k": "b-hd", "e": "E0140", "ad": "Hesap Defterim", "etiket": "Bütün Hesaplar",
             "satirlar": ["Kime Is Karti / Kimden Ana Hesap iki ayrı satır.",
                          "İkisi de Alındı ve Ödendi toplamlarına giriyor.",
                          "Toplam Alındı 51.200 / Ödendi 6.250."]},
        ],
        "notlar": [
            "Hesap Defterim'de iki bacak birbirine bağlı değil. Koşumda bir bacak silindi, karşı "
            "bacak silinmeden kaldı ve net varlık sessizce yanlışa döndü. Bu bir koşum kaydıdır: "
            "kare yalnız tek defteri gösteriyor, silme anı kareye alınmadı.",
            "Aynı şekilde bir bacağın tutarı veya tarihi düzenlendiğinde diğerinin güncellenmediği "
            "koşumda görüldü; bu da kareyle değil koşumla kayıtlı.",
        ],
        "urunler_baslik": "Aynı soruda kaynaktan okunan dört ürün",
        "urunler": [
            ("KolayBi", "kaynak",
             "Finans altı sekmeye bölünmüş: Banka Hesapları, Kasalar, Kredi Kartları, Online "
             "Banka Hesapları, Çekler, Senetler. Kasa ile banka ayrı iki kavram; canlı beşte "
             "ikisi de aynı hesap listesinde durur.", "E0206"),
            ("Paraşüt", "beyan",
             "Kaynağa göre kasa ve banka ayrı tutuluyor ve ödeme adımı kasa/banka bakiyesini "
             "azaltıyor; banka entegrasyonu mutabakatı otomatikleştiriyor.", "parasut.com kılavuzu"),
            ("Logo İşbaşı", "beyan",
             "Kaynağa göre tek kayıt cari, kasa-banka ve stok defterlerini birlikte güncelliyor; "
             "nakit tahsilat ve banka bilgisi telefondan eklenebiliyor.", "isbasi.com"),
            ("QuickBooks Solopreneur", "beyan",
             "Kaynakta hesaplar arası aktarım için ayrı bir model belgelenmemiş; ürün çift "
             "taraflı muhasebe yapmıyor.", "Intuit yardım merkezi"),
        ],
    },

    # ------------------------------------------------------------------ 2.5
    {
        "tur": "yanyana", "no": "2.5",
        "baslik": "Hesabı toplamdan çıkarmak: aynı para, iki net varlık",
        "giris": "Money Manager hesap formunda bir anahtar taşıyor. Anahtar kapatıldığında hesap "
                 "listede kalıyor ama net varlığa girmiyor — aynı veriyle iki farklı toplam "
                 "okunuyor.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "d-mm1", "e": "E0252", "ad": "Money Manager", "etiket": "Toplama Dahil Et açık",
             "satirlar": ["Hesap Bilgisi formunda iki anahtar.",
                          "Toplama Dahil Et ve Göster/Gizle ayrı şeyler.",
                          "Ortak Cuzdan tutarı 4.150."]},
            {"k": "d-mm2", "e": "E0253", "ad": "Money Manager", "etiket": "Anahtar kapalıyken hesaplar",
             "satirlar": ["Varlıklar 39.800 / Toplam 38.200.",
                          "Nakit grubu 0,00 siyah; Ortak Cuzdan 4.150 gri.",
                          "Hesap listede duruyor ama toplamda yok."]},
        ],
        "notlar": [
            "Anahtar açıkken aynı ekran Varlıklar 44.950 / Toplam 44.950 diyordu. Fark tam olarak "
            "dışarıda bırakılan hesabın 4.150'si ve kart borcunun 1.600'ü kadar; iki sayı da aynı "
            "veriden okunuyor.",
            ("Bluecoins'te hesap listesinin üstündeki Nakit Akım Ayarı hesap başına bir anahtar. "
             "Ekranın kendi cümlesi: \"Nakit akışı hesaplarken kullanılacak nakit hesapları "
             "seçiniz.\" Anahtarı değiştirip raporun değiştiği ölçülmedi.", "E0446 · E0027"),
        ],
        "sag_notlar": [
            ("Gri gösterim hesabın pasif olduğunu değil, toplama girmediğini anlatıyor; hesap "
             "silinmemiş, listede duruyor.", "E0253"),
        ],
    },

    # ------------------------------------------------------------------ 2.6
    {
        "tur": "akis", "no": "2.6", "baslik": "Paranın hesaplar arasındaki yolu",
        "giris": "Hesap kurulduğu andan aktarımın toplamlara yansıdığı ana kadar dört adım. "
                 "İlk çatallanma daha hesap açılırken oluyor.",
        "adimlar": [
            {
                "baslik": "Hesap açılıyor",
                "ortak": "Beş üründe de hesabın adı ve türü soruluyor. Ayrışma bir sonraki "
                         "adımda, başlangıç parasının nereye yazıldığında başlıyor.",
            },
            {
                "baslik": "Açılış parası nereye yazılıyor",
                "dallar": [
                    {"urunler": "Money Manager · Bluecoins · Hesap Defterim", "vurgu": True,
                     "metin": "Deftere bir kayıt olarak. Money Manager bunu kullanıcıya soruyor; "
                              "Hesap Defterim listenin ilk satırı yapıyor.", "d": "E0239"},
                    {"urunler": "Goodbudget",
                     "metin": "Hesabın kendi değeri olarak; işlem listesinde ayrı satır yok.",
                     "d": "E0113"},
                    {"urunler": "Wallet",
                     "metin": "Hesap formunda böyle bir alan yok; düzenleme formu tarandı.",
                     "d": "E0437"},
                ],
            },
            {
                "baslik": "Aktarım kaç satır üretiyor",
                "dallar": [
                    {"urunler": "Money Manager",
                     "metin": "Tek Havale satırı; iki hesabı da o satır taşıyor.", "d": "E0228"},
                    {"urunler": "Bluecoins · Wallet",
                     "metin": "İki bacak, aynı gün içinde birbirini götürüyor.", "d": "E0279"},
                    {"urunler": "Hesap Defterim", "vurgu": True,
                     "metin": "İki bağımsız satır; biri silinince diğeri kalıyor.", "d": "E0140"},
                ],
            },
            {
                "baslik": "Ayın toplamına ne oluyor",
                "dallar": [
                    {"urunler": "Money Manager · Bluecoins · Wallet",
                     "metin": "Hiçbir şey. Aktarım gelir/gider toplamına girmiyor.", "d": "E0231"},
                    {"urunler": "Hesap Defterim", "vurgu": True,
                     "metin": "Her iki bacak da giriyor: Alındı +3.000, Ödendi −3.000.",
                     "d": "E0142"},
                ],
            },
        ],
        "notlar": [
            "İki çatallanma da aynı tercihten doğuyor: ürün defterine ne kadar çok şeyi kayıt "
            "olarak yazarsa, o kayıtları toplamların dışında tutmak için o kadar çok kurala "
            "ihtiyaç duyuyor.",
            "Goodbudget bu yolun üçüncü adımında hiç görünmüyor; ücretsiz pakette tek hesap "
            "açılabildiği için aktarım koşulamadı.",
        ],
    },

    # ------------------------------------------------------------------ 2.7
    {
        "tur": "cikarim", "no": "2.7", "baslik": "Neden ayrışıyorlar",
        "giris": "Bu bölümün iki sorusu tek bir tercihte birleşiyor: ürün neyi kayıt sayıyor. "
                 "Kayıt sayılan her şey deftere girer, ve deftere giren her şeyin toplamlardan "
                 "nasıl çıkarılacağı ayrı bir karar hâline gelir.",
        "mekanizma": [
            {
                "baslik": "Açılış bakiyesi bir kayıt olunca ayın toplamına da giriyor",
                "metin": [
                    "Üç üründe açılış parası deftere bir satır olarak yazılıyor. Money Manager "
                    "bunu kullanıcıya açıkça soruyor ve hesabın defterinde Para Yatırma 2.000 "
                    "olarak gösteriyor; Hesap Defterim Ağustos listesinin ilk satırı yapıyor.",
                    "Fark burada değil, sonrasında: Money Manager açılışı ayın gelir toplamına "
                    "sokmuyor, Hesap Defterim sokuyor. Bölüm 1'deki 51.200'ün içindeki 22.000 "
                    "tam olarak budur. Aynı tasarım tercihi, iki ayrı sonuç.",
                ],
                "dayanak": "E0239, E0240, E0137, E0142, E0231",
            },
            {
                "baslik": "Aktarım bir üründe tek kayıt, üçünde iki satır",
                "metin": [
                    "Money Manager aktarımı tek bir Havale kaydı olarak tutuyor: iki hesabı aynı "
                    "satır taşıyor ve günün başlığı 0/0 kalıyor. Bluecoins ve Wallet iki bacak "
                    "gösteriyor ve ikisini aynı günde netliyor. Toplamı koruyan şey ya tek kayıt "
                    "olması ya da iki bacağın birlikte yazılması.",
                    "Hesap Defterim'de bu netleme yok: iki satır birbirinden habersiz. Koşumda "
                    "bir bacak silindiğinde karşı bacak kaldı ve net varlık sessizce yanlışa "
                    "döndü. Bu gözlem kareyle değil koşumla kayıtlı; kare yalnız tek defteri "
                    "gösteriyor.",
                    "Sessizlik burada asıl sorun: yanlış sayı bir hata mesajı vermiyor, yalnız "
                    "toplamda duruyor.",
                ],
                "dayanak": "E0228, E0140, E0022, E0279, koşum kaydı (Hesap Defterim A testi)",
            },
            {
                "baslik": "Goodbudget'ta aktarım zarf katmanına hiç uğramıyor",
                "metin": [
                    "Aktarım formu birinci sınıf ve ayrı bir ekran, ama zarf alanı taşımıyor. "
                    "Bölüm 1'de zarf ile hesabın iki ayrı defter olduğu görülmüştü; aktarım bu "
                    "iki defterden yalnız hesabı ilgilendiriyor.",
                    "Bu tutarlı bir tasarım: para amacını değiştirmeden yer değiştiriyorsa zarf "
                    "kalanının değişmesi için bir neden yok. Karşılığında kullanıcı, paranın "
                    "hangi hesapta durduğunu zarf ekranından hiçbir zaman göremiyor.",
                ],
                "dayanak": "E0119, E0120, E0417, E0421",
            },
        ],
        "kazanc": [
            ("Money Manager",
             "Aktarımı tek satırda gösterip günün toplamını 0/0 yazıyor; hesabı toplamdan "
             "çıkarma anahtarı hesabı silmeden raporu sadeleştiriyor",
             "Aynı veriden iki farklı net varlık okunabiliyor ve hangisinin geçerli olduğu "
             "ekranda yazmıyor"),
            ("Bluecoins",
             "İki bacağı da gösterip günde netliyor — kullanıcı paranın nereden nereye "
             "gittiğini satırdan okuyabiliyor; aktarım ücreti için ayrı alan var",
             "Ücret aktarımın içinde değil, kendi hesabı ve kategorisi olan ayrı bir gider "
             "olarak soruluyor; aktarımın maliyeti aktarımın kendisinde görünmüyor"),
            ("Wallet",
             "Aktarımın iki satırı renk ve yönle ayrışıyor, dönem toplamı bozulmuyor",
             "Hesap formunda açılış bakiyesi alanı yok; başlangıç parası ilk kayıtla girilmek "
             "zorunda"),
            ("Hesap Defterim",
             "Aktar ayrı bir ekran ve tek formla iki deftere yazıyor; satır başına yürüyen "
             "denge paranın izini kolay okutuyor",
             "İki bacak bağımsız: biri silinince veya düzenlenince diğeri güncellenmiyor ve "
             "net varlık sessizce bozuluyor"),
            ("Goodbudget",
             "Aktarım birinci sınıf bir işlem türü ve zarf kalanına dokunmuyor — para amacını "
             "değiştirmeden yer değiştiriyor",
             "Ücretsiz pakette tek hesap açılabildiği için aktarım hiç çalıştırılamıyor"),
        ],
        "soru": [
            "Açılış bakiyesi bir kayıt mı olmalı, hesabın bir özelliği mi? Kayıt olursa ayın "
            "toplamından çıkarmak için ayrı bir kural gerekiyor.",
            "Aktarımın iki ucu tek bir nesne mi olmalı? Money Manager tek kayıt tutuyor; Hesap "
            "Defterim'in örneği iki bağımsız satırın sessizce tutarsızlaşabildiğini gösteriyor.",
            "Bir hesabı toplamdan çıkarma anahtarı gerekli mi? Gerekliyse, hangi toplamın "
            "geçerli olduğu ekranda nasıl yazılmalı?",
        ],
    },
]

EKSIKLER = [
    ("Wallet", "Hesap oluşturma formunun kendisi", "2.1",
     "Dördüncü hesap ücretsiz pakette açılamıyor; düzenleme formu tarandı, açılış alanı yok", "Önerilmez"),
    ("Bluecoins", "Nakit Akım Ayarı'nın hesabı nakit akışından çıkarıp çıkarmadığı", "2.5",
     "Ekranın cümlesi okundu; ayarı açıp kapatıp nakit akışı raporunu karşılaştırmak kaldı", "Orta"),
    ("Bluecoins", "Transfer ücretinin kayda ve iki bakiyeye etkisi", "2.2",
     "Formdaki yeri görüldü; ücretli bir aktarım kaydedip iki bakiyeyi okumak kaldı", "Orta"),
    ("Hesap Defterim", "Bacak silmenin birleşik net varlığa etkisinin karesi", "2.4",
     "Mevcut veriyi bozar; ayrı bir kontrol defteriyle denenmeli", "Önerilmez"),
    ("Goodbudget", "Aktarımın kendisi", "2.3", "Ücretli paket; ikinci hesap açılamıyor", "Önerilmez"),
]
