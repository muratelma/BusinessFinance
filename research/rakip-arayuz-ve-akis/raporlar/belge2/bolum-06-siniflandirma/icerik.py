# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 6 · Siniflandirma: isletme mi, sahsi mi — tek icerik kaynagi."""

NO = 6
BASLIK = "Sınıflandırma: işletme mi, şahsi mi"
ANA_SORU = "Bir kaydın işletmeye mi şahsi hayata mı ait olduğu nerede yazıyor?"
EN_AZ_KARE = 11

GIRIS = [
    "Bu bölüm bir yokluğun bölümü. Canlı koşulan beş üründe kayıt düzeyinde işletme/şahsi "
    "ayrımı yapan bir alan arandı ve hiçbirinde bulunamadı. Bulunan şey, aynı işi dolaylı "
    "yoldan yapmaya çalışan dört ayrı araç.",
    "Yokluğun kendisi de bir bulgu, ama tek başına az şey söyler. Bu yüzden bölüm iki yöne "
    "gidiyor: canlı ürünlerde bu işe en yakın duran araçlar, ve kaynaktan okunan iki ayrı "
    "çözüm — biri kullanıcının tanımladığı bir eksen, öteki kayıt başına iki değerli bir alan.",
]
GIRMEZ = [
    "Kategorinin rapordaki kırılımı → Bölüm 7",
    "Zarf modelinin para üzerindeki etkisi → Bölüm 1",
    "Kategori ekranlarının görsel düzeni → Belge 1 §6",
]
KAPSAM = [
    ("Money Manager", ["canli"], "Form ve kategori paneli kareli; kapsam alanı bulunamadı."),
    ("Bluecoins", ["canli"], "Form alanları ve kategori ağacı kareli."),
    ("Wallet", ["canli"], "Kayıt ayrıntısı ve Labels katmanı kareli."),
    ("Hesap Defterim", ["canli"], "Serbest metin kategorisi ve yeniden adlandırma kareli."),
    ("Goodbudget", ["canli"], "Zarf amaç ekseni olarak kareli."),
    ("KolayBi", ["kaynak"], "Proje ekseni destek görselinde."),
    ("QuickBooks Solopreneur", ["beyan"], "Kayıt başına Business/Personal alanı; yalnız kaynak metni."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
]
ACILIS_DAYANAK = (
    "Yokluk ifadeleri incelenen sürüm ve taranan yüzeylerle sınırlıdır. Rozetler kanıt "
    "düzeyini gösterir; kaynak beyanı ölçülmüş davranış değildir."
)

SAYFALAR = [
    {"tur": "acilis"},

    # ------------------------------------------------------------------ 6.1
    {
        "tur": "yanyana", "no": "6.1",
        "baslik": "Kayıt formunda böyle bir alan var mı",
        "giris": "Dört üründe kayıt formunun ve kayıt ayrıntısının tamamı tarandı. Tutar, tarih, "
                 "kategori, hesap, not, etiket, durum — hepsi var. İşletme ile şahsiyi ayıran "
                 "bir alan yok.",
        "yukseklik": 290,
        "sekiller": [
            {"k": "y-mm", "e": "E0229", "ad": "Money Manager", "etiket": "Form ve kategori paneli",
             "kirpma": (0, 120, 1080, 2050),
             "satirlar": ["Tutar, Kategori, Hesap, Not.",
                          "On bir kategori: Yiyecek, Eğlence, Taşıma…",
                          "Hepsi kişisel gider kategorisi."]},
            {"k": "y-bc", "e": "E0020", "ad": "Bluecoins", "etiket": "Formun bütün alanları",
             "kirpma": (0, 120, 1080, 1615),
             "satirlar": ["Kategori, hesap, Planlı İşlemler, Bölmek.",
                          "Durum ve Etiket ayrı alanlar.",
                          "Kapsam alanı yok."]},
            {"k": "y-wl", "e": "E0294", "ad": "Wallet", "etiket": "Kayıt ayrıntısı",
             "kirpma": (0, 120, 1080, 2300),
             "satirlar": ["Note, Labels, Payee, Date, Time.",
                          "Payment Type, Warranty, Status, Place.",
                          "Görünen bölümde kapsam alanı yok."]},
            {"k": "y-hd", "e": "E0151", "ad": "Hesap Defterim", "etiket": "Açıklama / Kategori",
             "kirpma": (0, 120, 1080, 1615),
             "satirlar": ["Kategori bir seçici değil, serbest metin.",
                          "Kullanıcı ne yazarsa o.",
                          "İki değerli bir ayrım alanı yok."]},
        ],
        "notlar": [
            "Goodbudget'ta da kayıt formunda böyle bir alan yok; kaydı sınıflayan tek şey hangi "
            "zarfa yazıldığı. Zarf ise amaç ekseni, kapsam ekseni değil.",
            "Bu bölümdeki yokluk ifadeleri incelenen sürümler ve taranan yüzeylerle sınırlı: "
            "kayıt formu, kayıt ayrıntısı, kategori yönetimi ve ayarlar. Ücretli paketlerde "
            "açılan yüzeyler bu taramaya girmedi.",
        ],
        "sag_notlar": [
            ("Money Manager'ın kategori paneli on bir kutunun hepsini kişisel gider başlığıyla "
             "dolduruyor; işletme tarafına ayrılmış bir grup yok.", "E0229"),
            ("Hesap Defterim'de kategori alanı ayarlardan kapatılabiliyor.", "E0178"),
        ],
    },

    # ------------------------------------------------------------------ 6.2
    {
        "tur": "yanyana", "no": "6.2",
        "baslik": "Aynı işe en yakın duran dört araç",
        "giris": "Kapsam alanı olmayınca kullanıcı eldekiyle idare ediyor. Dört üründe dört ayrı "
                 "araç bu işe en yakın duran şey — ama dördü de başka bir soru için yapılmış.",
        "yukseklik": 250,
        "sekiller": [
            {"k": "z-wl", "e": "E0425", "ad": "Wallet · Labels", "etiket": "Yalnız etiketli para",
             "kirpma": (0, 120, 1080, 1825),
             "satirlar": ["Rapor ekranında Categories / Labels geçişi.",
                          "Labels seçili: bu ay ₺150.",
                          "Etiketsiz kayıtlar bu görünümde yok."]},
            {"k": "z-bc2", "e": "E0088", "ad": "Bluecoins · Etiketler", "etiket": "Etiket kendi listesinde",
             "kirpma": (0, 120, 1080, 1615),
             "satirlar": ["Etiketlerin kendi yönetim ekranı var.",
                          "Arama ve silme eylemleri burada.",
                          "Listede İş ve Kişisel de var."]},
            {"k": "z-bc", "e": "E0087", "ad": "Bluecoins · Kategori ağacı", "etiket": "İki katmanlı kategori",
             "kirpma": (0, 120, 1080, 1800),
             "satirlar": ["Üst başlıklar: Araba, Eve Ait, Eğlence.",
                          "Altlarında alt kategoriler.",
                          "Üst katman kapsam gibi kullanılabilir."]},
            {"k": "z-gb", "e": "E0417", "ad": "Goodbudget · Zarf", "etiket": "Amaç ekseni",
             "kirpma": (0, 120, 1080, 1245),
             "satirlar": ["Para amacına göre bölünmüş.",
                          "Zarf adı kaydın ne için olduğunu söylüyor.",
                          "Kimin parası olduğunu söylemiyor."]},
            {"k": "z-hd", "e": "E0161", "ad": "Hesap Defterim · Adlandırma", "etiket": "Sütun adları değişti",
             "kirpma": (0, 120, 1080, 2200),
             "satirlar": ["Alındı/Ödendi yerine Tahsilat/FaturaOdemesi.",
                          "Bütün ekran başlıkları birlikte değişti.",
                          "Ayrım değil, dil değişikliği."]},
        ],
        "notlar": [
            "Dördü de kısmi çözüm. Etiket serbest ve çok değerli — iki değerli bir ayrım için "
            "kullanıldığında kullanıcının disiplinine bağlı. Kategori ağacının üst katmanı "
            "kapsam gibi kullanılabilir ama o zaman kategori kırılımı kaybediliyor.",
            ("Disiplinin bedeli Wallet'ta ölçüldü: koşumda iki kayda Isletme ve Sahsi etiketi "
             "verildi. Labels görünümü yalnız bu ikisini, ₺150'yi saydı; aynı ayın ₺21.750'lik "
             "giderinin etiketlenmemiş kısmı bu görünümde hiç yok.", "E0425 · E0439"),
            "Hesap Defterim'inki bir sınıflandırma aracı değil: düğme ve sütun adlarının "
            "kullanıcının diline çevrilmesi. Ayrımı değil, ayrımın adını değiştiriyor.",
        ],
        "sag_notlar": [
            ("Wallet'ın ayarlarında bir kural motoru var: kayıtlara kategori ve etiket atayan Automatic rules. Kapsam için kullanılıp kullanılamayacağı denenmedi.", "E0378"),
            ("Bir başka dolaylı yol hesabı ayırmak: işletme parası için ayrı hesap açıp kapsamı "
             "hesap düzeyine taşımak. Bu koşumda denenmedi.", "E0027"),
            ("Bluecoins'in etiket listesi, araştırmada hiç etiket eklenmemişken de İş ve Kişisel "
             "değerlerini taşıyordu. Etiket çoklu seçiliyor, filtre panelinde ayrı bir boyut ve "
             "bölünmüş kaydın her parçası kendi etiketini alabiliyor.", "E0088 · E0426 · E0443"),
        ],
    },

    # ------------------------------------------------------------------ 6.3
    {
        "tur": "soru", "no": "6.3", "baslik": "Kaynakta birinci çözüm: kullanıcının tanımladığı eksen",
        "giris": "KolayBi kapsamı sabit bir alan yapmıyor — kullanıcının açtığı bir proje ekseni "
                 "sunuyor. Her belge bir projeye bağlanıyor ve proje kendi gelir, gider ve net "
                 "toplamını taşıyor. Alanların varlığı kanıtlı; davranış görülmedi.",
        "dikey": True, "not_genislik": 300,
        "sekiller": [
            {"k": "kb-proje", "e": "E0188", "etiket": "KolayBi · Proje listesi",
             "kirpma": (96, 112, 1305, 700), "genislik": 350,
             "satirlar": ["Kolonlar: kod, ad, etiket, para birimi, durum, tarih.",
                          "Ve üç toplam: gelir, gider, net.",
                          "Proje adları kullanıcının koyduğu adlar."]},
            {"k": "kb-detay", "e": "E0190", "etiket": "KolayBi · Proje özeti",
             "kirpma": (96, 112, 1305, 845), "genislik": 350,
             "satirlar": ["Kâr/Zarar ve Nakit Durumu iki ayrı sekme.",
                          "Toplam, tahsil edilen, ödenen, bekleyen kırılımı.",
                          "Aktif/Pasif menüsü ve ayrıca silme var."]},
        ],
        "notlar": [
            "Eksen kullanıcı tanımlı olduğu için \"işletme\" ve \"şahsi\" iki proje olarak "
            "açılabilir — ama ürün bunu önermiyor; proje bir iş kalemi olarak kurgulanmış ve "
            "listedeki adlar da öyle.",
            "Kullanıcı tanımlı eksenin bedeli: iki proje mi, yirmi proje mi olacağı kullanıcıya "
            "kalıyor ve raporun anlamı kullanıcının disiplinine bağlanıyor.",
        ],
        "sag_notlar": [
            ("Projenin belge kırılımı satış, alış, iade, para girişi ve genel gider kartlarına "
             "ayrılmış.", "E0191"),
            ("Yeni proje formunda kod, ad ve para birimi zorunlu; başlangıç-bitiş tarihi var.",
             "E0189"),
        ],
    },

    # ------------------------------------------------------------------ 6.4
    {
        "tur": "metin", "no": "6.4",
        "baslik": "Kaynakta ikinci çözüm: kayıt başına iki değerli bir alan",
        "giris": "QuickBooks Solopreneur, incelenen dokuz ürün içinde kayıt düzeyinde "
                 "işletme/şahsi ayrımı yapan tek ürün. Ürünün hiçbir iç ekranı görülmedi; "
                 "aşağıdakilerin tamamı yardım merkezi metninden okunmuştur ve ölçülmüş "
                 "davranış değildir.",
        "sutun": 3,
        "bloklar": [
            {"baslik": "Alan: Type", "tur": "beyan", "d": "Intuit yardım merkezi",
             "metin": [
                 "İşlem başına tek bir alan: Type sütunu Business veya Personal değerini alıyor. "
                 "Üçüncü bir değer yok.",
                 "İşlem listesi bu başlıktan süzülebiliyor; şahsi işaretlenen kayıt işletme "
                 "raporuna girmiyor ama silinmiyor.",
             ]},
            {"baslik": "Kısmen işletme: Split", "tur": "beyan", "d": "Intuit yardım merkezi",
             "metin": [
                 "Bir gider kısmen işletme kısmen şahsiyse kayıt bölünüyor: Edit → Split "
                 "transaction → tutar parçalara ayrılıyor ve her parça ayrı ayrı işaretleniyor.",
                 "Parçaların toplamı özgün tutara eşitleniyor; yani kapsam kayıt düzeyinde değil, "
                 "kalem düzeyinde tanımlanabiliyor.",
                 "Kaynak bir istisna da yazıyor: araç ve yakıt gideri bölünmüyor, tamamı işletme "
                 "işaretleniyor ve oranı muhasebeci hesaplıyor.",
             ]},
            {"baslik": "Silme yerine hariç tutma", "tur": "beyan", "d": "Intuit yardım merkezi",
             "metin": [
                 "Yinelenen veya ilgisiz kayıt silinmiyor, Exclude ile hariç tutuluyor. Kayıt "
                 "veride kalıyor, toplamlardan çıkıyor.",
                 "Bu, şahsi işaretlemenin de mantığı: kayıt duruyor, yalnız hangi toplama "
                 "gireceği değişiyor.",
             ]},
            {"baslik": "Sınıflamayı kim yapıyor", "tur": "beyan", "d": "Intuit yardım merkezi",
             "metin": [
                 "Kayıtlar bağlı banka ve kart hesaplarından otomatik iniyor; ürün geçmişe ve "
                 "başka kullanıcıların davranışına bakarak kategori ve tür öneriyor.",
                 "Kullanıcının işi kaydı oluşturmak değil, inen kaydı gözden geçirip düzeltmek.",
                 "Tekrar eden düzeltmeler bir kural motoruna dönüşüyor: en çok otuz kural, "
                 "benzer işlemleri otomatik sınıflıyor.",
             ]},
            {"baslik": "Kategoriler nereye bağlı", "tur": "beyan", "d": "Intuit yardım merkezi",
             "metin": [
                 "Kategoriler ülkeye özgü bir vergi formunun kalemleriyle hizalı; kategorize "
                 "edilen her işlem o formda bir satıra eşleniyor.",
                 "Yani sınıflandırmanın amacı raporlama değil, yıl sonunda üretilecek belge. "
                 "Ayrımın keskinliği ve üçüncü değerin olmaması buradan geliyor.",
             ]},
            {"baslik": "Bu sayfanın sınırı", "tur": "beyan", "d": "Intuit yardım merkezi",
             "metin": [
                 "Ürünün iç arayüzü hiç görülmedi: elde yalnız açılış ekranları ve ödeme duvarı "
                 "var. Yukarıdaki her cümle ürünün kendi anlatımıdır.",
                 "Alanın gerçekte nasıl davrandığı, şahsi kaydın hangi toplamlardan çıktığı ve "
                 "bölmenin sonucunun raporda nasıl göründüğü ölçülmedi.",
             ]},
        ],
        "notlar": [
            "Bu sayfa bölümün tek karesiz sayfası ve bilinçli olarak öyle: basılacak bir ekran "
            "yok. Kaynak metninin kendisi bulgu, çünkü canlı beş üründe karşılığı hiç çıkmadı.",
        ],
    },

    # ------------------------------------------------------------------ 6.5
    {
        "tur": "tablo", "no": "6.5", "baslik": "Dokuz üründe kapsam ayrımı",
        "giris": "Aynı soru dokuz ürüne soruldu: bir kaydın işletmeye mi şahsi hayata mı ait "
                 "olduğu nerede yazıyor?",
        "sutunlar": [("Ürün", 18), ("Kapsam alanı", 22), ("En yakın araç", 30), ("Ayrımın sonucu", 30)],
        "boy": 8.4,
        "satirlar": [
            ["Money Manager",
             {"t": "Yok", "tur": "canli", "d": "E0229"},
             {"t": "Kategori; on bir kutunun hepsi kişisel", "tur": "canli", "d": "E0229"},
             {"t": "Kapsam raporda hiç görünmüyor", "tur": "canli", "d": "E0231"}],
            ["Bluecoins",
             {"t": "Yok", "tur": "canli", "d": "E0020"},
             {"t": "Etiket (listede İş ve Kişisel) ve iki katmanlı kategori ağacı", "tur": "canli", "d": "E0087 · E0088"},
             {"t": "Üst kategori kapsam gibi kullanılırsa kırılım kayboluyor", "tur": "cikarim", "d": "E0087"}],
            ["Wallet",
             {"t": "Yok", "tur": "canli", "d": "E0294"},
             {"t": "Labels — kategoriden bağımsız ikinci eksen", "tur": "canli", "d": "E0278"},
             {"t": "Rapor Categories/Labels olarak ayrılıyor; Labels yalnız etiketli parayı sayıyor", "tur": "canli", "d": "E0278 · E0425"}],
            ["Hesap Defterim",
             {"t": "Yok", "tur": "canli", "d": "E0151"},
             {"t": "Serbest metin kategori; düğme adlarını değiştirme", "tur": "canli", "d": "E0161"},
             {"t": "Raporda kategori kırılımı da yok", "tur": "canli", "d": "E0142"}],
            ["Goodbudget",
             {"t": "Yok", "tur": "canli", "d": "E0417"},
             {"t": "Zarf — amaç ekseni", "tur": "canli", "d": "E0417"},
             {"t": "Zarf ne için olduğunu söylüyor, kimin parası olduğunu değil", "tur": "cikarim", "d": "E0417"}],
            ["KolayBi",
             {"t": "Proje ekseni", "tur": "kaynak", "d": "E0188"},
             {"t": "Kullanıcının tanımladığı proje; gelir/gider/net proje başına", "tur": "kaynak", "d": "E0188"},
             {"t": "Eksenin anlamı kullanıcının disiplinine bağlı", "tur": "cikarim", "d": "E0188"}],
            ["QuickBooks Solopreneur",
             {"t": "Var — Type: Business / Personal", "tur": "beyan", "d": "Intuit yardım merkezi"},
             {"t": "Kayıt başına iki değerli alan; kısmen işletme için Split", "tur": "beyan", "d": "Intuit yardım merkezi"},
             {"t": "Şahsi kayıt silinmiyor, toplamdan çıkıyor", "tur": "beyan", "d": "Intuit yardım merkezi"}],
            ["Paraşüt",
             {"t": "Yok", "tur": "beyan", "d": "parasut.com kılavuzu"},
             {"t": "Ortak/personel carisi üzerinden dolaylı", "tur": "beyan", "d": "parasut.com kılavuzu"},
             {"t": "Şahsi harcama bir alacak-borç kalemine dönüşüyor", "tur": "cikarim", "d": "parasut.com kılavuzu"}],
            ["Logo İşbaşı",
             {"t": "Yok", "tur": "beyan", "d": "isbasi.com"},
             {"t": "Ortak carisi veya çekilen para", "tur": "beyan", "d": "isbasi.com"},
             {"t": "Klasik firma defteri; şahsi taraf ürünün dışında", "tur": "cikarim", "d": "isbasi.com"}],
        ],
        "notlar": [
            "Dokuz üründe kayıt düzeyinde kapsam alanı yalnız birinde var ve o ürünün hiçbir iç "
            "ekranı görülmedi. Canlı koşulan beş üründe sıfır.",
            "Üç Türk ön muhasebe ürününde de alan yok; ikisinde şahsi harcamanın dolaylı yolu "
            "ortak veya personel carisinden geçiyor — yani şahsi harcama bir borç kalemine "
            "dönüşüyor.",
        ],
    },

    # ------------------------------------------------------------------ 6.6
    {
        "tur": "akis", "no": "6.6", "baslik": "Kapsam sorusunun yolu ve ayrıldığı noktalar",
        "giris": "Aynı soru dokuz üründe üç ayrı cevapla karşılaşıyor: alan yok, eksen "
                 "kullanıcıya bırakılmış, ya da alan sabit ve iki değerli.",
        "adimlar": [
            {
                "baslik": "Kayıt giriliyor",
                "ortak": "Dokuz üründe de kayıt tutar, tarih ve bir sınıflandırma taşıyor. "
                         "Ayrışma o sınıflandırmanın hangi soruyu cevapladığında.",
            },
            {
                "baslik": "Kapsam nerede yazılıyor",
                "dallar": [
                    {"urunler": "Canlı beş ürün", "vurgu": True,
                     "metin": "Hiçbir yerde. Kayıt formunda, kayıt ayrıntısında ve kategori "
                              "yönetiminde böyle bir alan bulunamadı.", "d": "E0294"},
                    {"urunler": "KolayBi",
                     "metin": "Kullanıcının açtığı bir proje ekseninde. Eksen sabit değil, "
                              "adları kullanıcı koyuyor.", "d": "E0188"},
                    {"urunler": "QuickBooks Solopreneur",
                     "metin": "Kayıt başına sabit bir alanda: Business ya da Personal. Üçüncü "
                              "değer yok.", "d": "Intuit yardım merkezi"},
                ],
            },
            {
                "baslik": "Kısmen işletme olan gider",
                "dallar": [
                    {"urunler": "Wallet",
                     "metin": "Kaydı bölmek mümkün (Split record) ama parçalara kapsam "
                              "verilemiyor; bölme kategori içindir.", "d": "E0311"},
                    {"urunler": "Bluecoins",
                     "metin": "Her parça kendi etiketini taşıyabiliyor; İş ve Kişisel "
                              "etiketleriyle gider parça parça işaretlenebilir. Raporun parçaları "
                              "etikete göre ayırdığı ölçülmedi.", "d": "E0443 · E0088"},
                    {"urunler": "QuickBooks Solopreneur", "vurgu": True,
                     "metin": "Kayıt tutara göre bölünüyor ve her parça ayrı işaretleniyor; "
                              "kapsam kalem düzeyine iniyor.", "d": "Intuit yardım merkezi"},
                ],
            },
            {
                "baslik": "Raporda ne oluyor",
                "dallar": [
                    {"urunler": "Canlı beş ürün",
                     "metin": "Rapor kapsamdan habersiz. Ayrım ancak etiket veya kategori "
                              "disipliniyle taklit edilebiliyor; Wallet'ta etiketlenmeyen kayıt "
                              "etiket raporundan tamamen düşüyor.", "d": "E0278 · E0425"},
                    {"urunler": "KolayBi",
                     "metin": "Her proje kendi gelir, gider ve net toplamını taşıyor.",
                     "d": "E0188"},
                    {"urunler": "QuickBooks Solopreneur",
                     "metin": "Şahsi işaretlenen kayıt işletme raporundan çıkıyor ama veride "
                              "kalıyor — silinmiyor, hariç tutuluyor.", "d": "Intuit yardım merkezi"},
                ],
            },
        ],
        "notlar": [
            "Yolun ilk adımı ortak, ikincisinden sonrası tamamen ayrışıyor. Alanı olmayan beş "
            "üründe sonraki iki adım da boş kalıyor — kavram olmayınca ne bölme ne raporlama "
            "sorusu oluşuyor.",
            "Bu akışın iki dalı kaynak metnine dayanıyor ve ölçülmüş davranış değil.",
        ],
    },

    # ------------------------------------------------------------------ 6.7
    {
        "tur": "cikarim", "no": "6.7", "baslik": "Neden ayrışıyorlar",
        "giris": "Bu bölümde ayrışmanın kaynağı ürünlerin kime hizmet ettiği. Kişisel finans "
                 "defterinde kapsam sorusu hiç oluşmuyor; ön muhasebede şahsi taraf ürünün "
                 "dışında; vergi ekseninde ise ayrım ürünün asıl işi.",
        "mekanizma": [
            {
                "baslik": "Kapsam alanının yokluğu ürünlerin hedef kitlesinden geliyor",
                "metin": [
                    "Canlı beş ürün kişisel finans uygulaması ve kayıt düzeyinde tek bir cep "
                    "varsayıyor. Money Manager'ın on bir kategorisinin hepsi kişisel gider "
                    "başlığı taşıyor — işletme tarafına ayrılmış tek bir grup bile yok. Bluecoins'in "
                    "etiket listesinde İş ve Kişisel'in bulunması, ayrımın akla geldiğini ama bir "
                    "alana dönüşmediğini gösteriyor.",
                    "Üç Türk ön muhasebe ürününde de alan yok, ama nedeni tersi: onlar firma "
                    "defteri tutuyor ve şahsi harcama zaten ürünün konusu değil. Kaynakların "
                    "gösterdiği dolaylı yol, şahsi harcamayı ortak veya personel carisine "
                    "yazmak — yani onu bir borç kalemine çevirmek.",
                    "İki uçta da kapsam sorulmuyor, ama iki ayrı nedenle: birinde ikinci cep "
                    "yok sayılıyor, ötekinde ikinci cep başka bir kavrama dönüştürülüyor.",
                ],
                "dayanak": "E0229, E0020, E0294, E0151, parasut.com kılavuzu, isbasi.com",
            },
            {
                "baslik": "Yerine kullanılan araçların hepsi başka bir soru için yapılmış",
                "metin": [
                    "Etiket çok değerli ve serbest; iki değerli bir ayrım için kullanıldığında "
                    "sonucun doğruluğu tamamen kullanıcının disiplinine kalıyor. Wallet'ta bunun "
                    "sonucu ölçüldü: etiket raporu yalnız etiketli ₺150'yi saydı, etiketlenmemiş "
                    "her kayıt o rapordan düştü. Kategori ağacının "
                    "üst katmanı kapsam gibi kullanılabilir, ama o zaman kategorinin asıl işi — "
                    "harcamayı türüne göre kırmak — kayboluyor.",
                    "Zarf amacı söylüyor, sahibi değil: \"Market\" zarfı hem işletmenin hem evin "
                    "marketi olabilir. Hesap Defterim'in sunduğu şey ise bir ayrım değil, "
                    "ayrımın adını değiştirme imkânı.",
                    "Ortak nokta şu: kategori, zarf ve adlandırma tek eksenli. Kapsam ikinci bir "
                    "eksen istiyor ve canlı ürünlerde bu ekseni taşıyan tek araç etiket — "
                    "Wallet'ta raporun kendi görünümü, Bluecoins'te bir filtre boyutu ve etiket "
                    "listesinde İş ile Kişisel.",
                ],
                "dayanak": "E0278, E0425, E0087, E0088, E0417, E0161",
            },
            {
                "baslik": "Sabit alan, ayrımın amacından doğuyor",
                "metin": [
                    "QuickBooks'un alanı iki değerli ve üçüncü değeri yok. Kaynağa göre bunun "
                    "nedeni kategorilerin bir vergi formunun kalemleriyle hizalı olması: "
                    "kategorize edilen her işlem o formda bir satıra eşleniyor.",
                    "Ayrımın amacı raporlama değil, yıl sonunda üretilecek bir belge. Belge "
                    "\"kısmen\" kabul etmediği için alan da kabul etmiyor — kısmi durum kaydı "
                    "bölerek çözülüyor, alana üçüncü bir değer eklenerek değil.",
                    "KolayBi'nin çözümü bunun tersi: eksen sabit değil, kullanıcı tanımlı. "
                    "Esneklik kazanılıyor, ama raporun anlamı kullanıcının kaç proje açtığına "
                    "ve neyi hangi projeye yazdığına bağlanıyor.",
                ],
                "dayanak": "Intuit yardım merkezi, E0188, E0190",
            },
            {
                "baslik": "Şahsi kayıt silinmiyor, hariç tutuluyor",
                "metin": [
                    "Kaynağa göre şahsi işaretlenen kayıt işletme raporuna girmiyor ama veride "
                    "kalıyor. Aynı mantık ilgisiz kayıtlar için de geçerli: silme değil, hariç "
                    "tutma.",
                    "Bu, kapsamı bir filtre olarak kurmak demek — kayıt tek, toplam çok. Kaydın "
                    "kendisiyle o kaydın hangi toplama gireceği birbirinden ayrılıyor.",
                    "Canlı ürünlerde bu ayrımın karşılığı hesap düzeyinde duruyor: Money "
                    "Manager'ın Toplama Dahil Et ve Wallet'ın Exclude from stats anahtarı bütün "
                    "bir hesabı toplamdan çıkarıyor, hesabı silmeden. Kayıt düzeyinde bir "
                    "karşılığı görülmedi.",
                ],
                "dayanak": "Intuit yardım merkezi, E0252, E0253, E0437",
            },
        ],
        "kazanc": [
            ("Wallet",
             "Labels kategoriden bağımsız ikinci bir eksen ve rapor bu eksende de okunabiliyor",
             "Etiket serbest ve çok değerli; etiketlenmeyen kayıt etiket raporundan tamamen "
             "düşüyor"),
            ("Bluecoins",
             "Kategori iki katmanlı; etiket listesinde İş ve Kişisel var ve bölünmüş kaydın her "
             "parçası kendi etiketini alabiliyor",
             "Üst katman kapsama ayrılırsa kategorinin asıl kırılımı kayboluyor"),
            ("Goodbudget",
             "Zarf kaydın amacını net söylüyor ve para zaten o amaca ayrılmış durumda",
             "Amaç ekseni sahibi söylemiyor; aynı zarf iki cebe de ait olabilir"),
            ("Money Manager · Hesap Defterim",
             "—",
             "Kapsam için kullanılabilecek ikinci bir eksen yok; biri yalnız kategori, diğeri "
             "yalnız serbest metin sunuyor"),
            ("KolayBi",
             "Eksen kullanıcı tanımlı; her proje kendi gelir, gider ve net toplamını taşıyor",
             "Esnekliğin bedeli belirsizlik: raporun anlamı kullanıcının eksen disiplinine bağlı"),
            ("QuickBooks Solopreneur",
             "Kayıt başına sabit, iki değerli bir alan; kısmen işletme olan gider bölünebiliyor "
             "ve şahsi kayıt silinmeden toplamdan çıkıyor",
             "Ayrım ülkeye özgü bir vergi formuna bağlı; alan iki değerli olduğu için kısmi "
             "durum ancak kaydı bölerek çözülüyor"),
        ],
        "soru": [
            "Kapsam kayıt düzeyinde sabit bir alan mı olmalı, yoksa kullanıcının tanımladığı bir "
            "eksen mi? İki kaynak iki ayrı cevap veriyor ve ikisinin bedeli farklı.",
            "Kısmen işletme olan bir gider ne olmalı — bölünmeli mi, tek kapsamda mı kalmalı? "
            "Kaynakta Split kapsamı kalem düzeyine indiriyor; canlı tarafta Bluecoins'in parça "
            "başına etiketi aynı işe yarayabilir ama sonucu ölçülmedi.",
            "Şahsi işaretlenen kayıt silinmeli mi, toplamdan mı çıkmalı? Kaynaktaki model "
            "kaydı tutup filtreliyor.",
            "İkinci eksen kategoriden ayrı mı olmalı? Canlı beş üründe kategoriden bağımsız "
            "ikinci eksen yalnız ikisinde var ve ikisi de etiket.",
        ],
    },
]

EKSIKLER = [
    ("Bluecoins", "Etiket filtresinin toplamlara etkisi ve bölünmüş kaydın parça etiketleriyle raporu", "6.2",
     "Etiket filtre boyutu görüldü; filtre uygulanmış toplam okunmadı", "Orta"),
    ("Beş canlı ürün", "Ücretli paketlerde açılan yüzeylerin taranması", "6.1",
     "Ücretli paket gerekir", "Önerilmez"),
    ("QuickBooks Solopreneur", "Type alanının ve Split akışının gerçek davranışı", "6.4",
     "Ücretli paket; ürünün içi görülemiyor", "Önerilmez"),
    ("KolayBi", "Projenin kayda ve rapora etkisi", "6.3",
     "Canlı erişim yok; yalnız destek görselleri", "Önerilmez"),
]
