# -*- coding: utf-8 -*-
"""Belge 2 · Bolum 10 · Kapanis: urunlerin ayrildigi yer — tek icerik kaynagi."""

NO = 10
BASLIK = "Ürünlerin ayrıldığı yer"
ANA_SORU = "Dokuz bölümde ayrı ayrı görülen farklar tek bir yere mi çıkıyor?"
EN_AZ_KARE = 0

GIRIS = [
    "Bu bölüm yeni kanıt getirmez. Önceki dokuz bölümde ayrı ayrı ölçülen davranışları yan "
    "yana koyup üç tekrar eden deseni gösterir ve belgenin bıraktığı soruları toplar.",
    "Her cümlenin dayanağı ilgili bölümdedir; burada tekrar kare basılmaz.",
]
GIRMEZ = [
    "Yeni gözlem veya yeni kare → yok",
    "Kararlar ve öneriler → Belge 3",
]
KAPSAM = [
    ("Money Manager", ["canli"], "Dokuz bölümün sekizinde ölçüldü."),
    ("Bluecoins", ["canli"], "Dokuz bölümün sekizinde ölçüldü."),
    ("Wallet", ["canli"], "Dokuz bölümün sekizinde ölçüldü."),
    ("Hesap Defterim", ["canli"], "Dokuz bölümün yedisinde ölçüldü."),
    ("Goodbudget", ["canli"], "Ücretsiz paket sınırı üç bölümü kapattı."),
    ("KolayBi", ["kaynak"], "Sekiz bölümde yüzey olarak; Bölüm 9'da modeliyle."),
    ("Paraşüt", ["beyan"], "Model olarak Bölüm 9'da."),
    ("Logo İşbaşı", ["beyan"], "Model olarak Bölüm 9'da."),
    ("QuickBooks Solopreneur", ["beyan"], "Kendi sayfası Bölüm 6'da."),
]
ACILIS_DAYANAK = (
    "Bu bölümdeki her cümle önceki bölümlerde kareyle ya da kaynakla gösterilmiş bir "
    "gözleme dayanır; burada yeni bir iddia kurulmaz."
)

SAYFALAR = [
    {"tur": "acilis"},

    # ----------------------------------------------------------------- 10.1
    {
        "tur": "tablo", "no": "10.1", "baslik": "Dokuz bölüm, dokuz ayrım noktası",
        "giris": "Her bölümün bulduğu asıl ayrım tek cümleyle. Sağdaki sütun o ayrımın hangi "
                 "ürünü diğerlerinden en çok uzaklaştırdığını gösteriyor.",
        "sutunlar": [("Bölüm", 16), ("Ayrımın çıktığı yer", 46), ("En uçtaki ürün", 38)],
        "boy": 8.6,
        "satirlar": [
            ["1 · Gelir ve gider",
             {"t": "Ürünün kaydı kaç deftere yazdığı ve o defterlerin toplamına ne ad verdiği", "tur": "canli", "d": "E0142"},
             {"t": "Hesap Defterim — ayın gideri 6.250 okunuyor, gerçek gider 2.050", "tur": "canli", "d": "E0142"}],
            ["2 · Hesaplar ve aktarım",
             {"t": "Açılış parasının kayıt mı alan mı olduğu; aktarımın iki ucunun bağlı olup olmadığı", "tur": "canli", "d": "E0140"},
             {"t": "Hesap Defterim — bacaklar bağımsız, biri silinince net varlık sessizce bozuluyor", "tur": "kosum", "d": "E0140"}],
            ["3 · Kart",
             {"t": "Kartın bir dönemi olup olmadığı", "tur": "canli", "d": "E0233"},
             {"t": "Money Manager — aynı kart için iki ekranda 1.600 ve 5.600", "tur": "canli", "d": "E0413"}],
            ["4 · Borç ve tahsilat",
             {"t": "Alacağın bir hesap mı ayrı bir nesne mi olduğu", "tur": "canli", "d": "E0316"},
             {"t": "Wallet — borcun bakiyeye dokunup dokunmayacağını kullanıcıya soruyor", "tur": "canli", "d": "E0315"}],
            ["5 · Zaman ve plan",
             {"t": "Tanım ile kayıt arasındaki kapının kime ait olduğu", "tur": "canli", "d": "E0291"},
             {"t": "Hesap Defterim — kapı yok, çünkü plan kavramı hiç yok", "tur": "yok", "d": "E0171"}],
            ["6 · Sınıflandırma",
             {"t": "Kayıt düzeyinde işletme/şahsi alanının olup olmadığı", "tur": "canli", "d": "E0294"},
             {"t": "QuickBooks — dokuz üründe böyle bir alanı olan tek ürün", "tur": "beyan", "d": "Intuit yardım merkezi"}],
            ["7 · Rapor",
             {"t": "Toplamın tanımının ve döneminin ekranda yazılıp yazılmadığı", "tur": "canli", "d": "E0405"},
             {"t": "Goodbudget — harcama raporunun toplamı bir gelir kaydını içeriyor", "tur": "canli", "d": "E0123"}],
            ["8 · Veri",
             {"t": "Verinin cihazda mı hesapta mı durduğu ve bunun söylenip söylenmediği", "tur": "canli", "d": "E0149"},
             {"t": "Hesap Defterim — \"yedek kapalıysa geri yükleyemeyiz\" diye yazıyor", "tur": "canli", "d": "E0149"}],
            ["9 · Ön muhasebe tarafı",
             {"t": "Kaydın bir satır mı belge mi olduğu — ve belgenin yanında vergi, kalem ve müşaviri de getirip getirmediği", "tur": "kaynak", "d": "E0200"},
             {"t": "Üç ön muhasebe ürünü — kayıt ile ödeme ayrı anlar, vergi belgenin üstünde", "tur": "kaynak", "d": "E0214"}],
        ],
        "notlar": [
            "Dokuz ayrımın yedisi tek bir soruya bağlanıyor: ürün bir kaydı kaç yere yazıyor ve "
            "o yerlerin her birine ne ad veriyor.",
            "Kalan ikisi kullanıcıya sorulan sorularla ilgili — ve ikisi de aynı üründen "
            "çıkıyor.",
        ],
    },

    # ----------------------------------------------------------------- 10.2
    {
        "tur": "metin", "no": "10.2",
        "baslik": "Üç tekrar eden desen",
        "giris": "Dokuz bölüm boyunca aynı üç şey farklı konularda tekrar etti. Üçü de tek tek "
                 "küçük görünüyor ama birlikte ürünlerin karakterini belirliyor.",
        "sutun": 3,
        "bloklar": [
            {"baslik": "1 · Aynı şeyin iki sayısı", "tur": "cikarim", "d": "E0413, E0317, E0032",
             "metin": [
                 "Üç ayrı bölümde aynı desen çıktı. Kart bölümünde aynı kart için Hesaplar "
                 "ekranı 1.600, kart defteri 5.600 gösterdi. Borç bölümünde borç kartı "
                 "\"OWES ME 5.000\" derken kayıt listesi \"Total −5.000\" dedi. Rapor bölümünde "
                 "benzer adlı iki blok biri dönem akışını, öteki varlığı saydı.",
                 "Üçünde de iki sayı da doğru. Eksik olan şey aynı: her sayının hangi soruyu "
                 "cevapladığı ekranda yazmıyor.",
             ]},
            {"baslik": "2 · Kaç defter, o kadar kural", "tur": "cikarim", "d": "E0142, E0421, isbasi.com",
             "metin": [
                 "Bir kaydı tek deftere yazan ürünün toplamı kendiliğinden tutuyor. İkinci bir "
                 "defter açan ürün, iki defterin nasıl uyuşacağını ayrıca çözmek zorunda.",
                 "Goodbudget zarf ile hesabı ayırdı ve ikisi tutmadı: fark her zarf doldurmada "
                 "büyüdü. Hesap Defterim her şeyi tek deftere yazdı ve bu kez toplamların "
                 "tanımı kirlendi.",
                 "Kaynak taraf üç defteri birden besliyor ve bunu ürünün kapsamı olarak sunuyor "
                 "— cari, kasa ve stok.",
             ]},
            {"baslik": "3 · Kararı kim veriyor", "tur": "cikarim", "d": "E0315, E0291, E0043, E0430",
             "metin": [
                 "Üç yerde bir ürün kararı kendisi vermek yerine kullanıcıya sordu ve sorunun "
                 "sonucunu aynı cümlede yazdı: borç bakiyeye dokunsun mu, gelecek ödemeler "
                 "kendiliğinden kayda dönüşsün mü.",
                 "Bir başka üründe de benzer bir soru var: plan kayda çevrilirken hangi tarihe "
                 "yazılsın — bugüne mi, planlanan güne mi. Money Manager ise tekrarlı kaydı "
                 "kurarken sonucunu yazan bir soru soruyor, ama sorduğu şey kurulumun kendisi; "
                 "gerçekleşmeyi kullanıcıya bırakmıyor.",
                 "Diğer yerlerde bu kararlar verilmiş ve yazılmamış. Kullanıcı sonucu ancak "
                 "toplamlara bakarak anlıyor.",
             ]},
            {"baslik": "Desenlerin ortak kökü", "tur": "cikarim", "d": "E0405",
             "metin": [
                 "Üç desen de aynı yere çıkıyor: ürünün kendi tanımını kullanıcıya gösterip "
                 "göstermediği.",
                 "Money Manager bunun tersini yapan tek örnek: toplamı tek sayı yerine ödeme "
                 "kaynağına göre bölüp satır adına yazıyor. Bedeli okuma yükü — kullanıcı tek "
                 "sayı yerine dört satır okuyor.",
             ]},
            {"baslik": "Ortak olan az şey", "tur": "cikarim", "d": "E0405, E0244",
             "metin": [
                 "Dokuz bölümde yalnız iki davranış bütün ölçülen ürünlerde aynı çıktı.",
                 "Birincisi: kart ödemesi hiçbir üründe ayın giderine girmiyor — harcama zaten "
                 "kart kullanıldığı gün gider yazıldı.",
                 "İkincisi: gerçekleşmemiş plan hiçbir üründe ayın toplamına girmiyor. Plan "
                 "görünür kalıyor ama sayılmıyor.",
             ]},
            {"baslik": "Bu belgenin sınırı", "tur": "beyan", "d": "—",
             "metin": [
                 "Beş ürün canlı koşuldu, biri destek görselleriyle, üçü yalnız kendi "
                 "anlatımıyla incelendi. Kanıt düzeyi her sayfada rozetle yazılı.",
                 "Ücretsiz paket sınırları üç konuyu tamamen kapattı: Goodbudget'ta kart, "
                 "aktarım ve ikinci hesap. Bunlar üründe yokluk değil, erişim eksikliğidir.",
             ]},
        ],
        "notlar": [
            "Üç desenin hiçbiri bir ürünün hatası değil; her biri bir tasarım tercihinin "
            "görünen yüzü. Ürünlerin nasıl çalıştığı 10.3'te, tercihlerin bedeli 10.4'te.",
        ],
    },

    # ----------------------------------------------------------------- 10.3
    {
        "tur": "tablo", "no": "10.3", "baslik": "Beş ürünün motoru, aynı beş soruyla",
        "giris": "Dokuz bölüm boyunca ürünler konu konu okundu. Bu tablo aynı kanıtı ürün "
                 "ürün okuyor: beş soru beşine de aynı biçimde soruluyor, böylece bir sütun "
                 "baştan aşağı tek bir ürünün modeli olarak okunabiliyor. Yeni kanıt yok; her "
                 "hücre ilgili bölüme dayanıyor.",
        "sutunlar": [("", 15), ("Money Manager", 17), ("Bluecoins", 17), ("Wallet", 17),
                     ("Hesap Defterim", 17), ("Goodbudget", 17)],
        "boy": 7.8,
        "satirlar": [
            ["Para nerede yaşıyor",
             {"t": "Hesap defterlerinde; kartın kesim günü olan kendi dönemi var", "tur": "canli", "d": "E0233"},
             {"t": "Hesap gruplarında: banka, nakit, cari ve kart aynı listede", "tur": "canli", "d": "E0049"},
             {"t": "Hesaplarda; borç ayrı bir nesne ve kendi ekranında", "tur": "canli", "d": "E0316"},
             {"t": "Tek defterde; kart, taksit, plan ve cari kavramı yok", "tur": "canli", "d": "E0140"},
             {"t": "İki defterde: zarf ve hesap; ikisi tutmuyor", "tur": "canli", "d": "E0421"}],
            ["Kayıt ile ödeme ayrılıyor mu",
             {"t": "Kartta evet: harcama gider yazıyor, ödeme ayrı yüzeyden geçiyor ve gidere girmiyor", "tur": "canli", "d": "E0405"},
             {"t": "Cari hesapta evet: borçlandırma banka değişmeden gelir yazıyor", "tur": "canli", "d": "E0075"},
             {"t": "Kullanıcıya soruluyor: borç kayıtla mı kayıtsız mı oluşsun", "tur": "canli", "d": "E0315"},
             {"t": "Hayır; kayıt yazıldığı anda denge değişiyor", "tur": "canli", "d": "E0142"},
             {"t": "Ölçülemedi; ücretsiz pakette kart ve aktarım kapalı", "tur": "yok", "d": "E0120"}],
            ["Gelecek nasıl tutuluyor",
             {"t": "İki ayrı biçim: taksit gelecek ayın gider toplamına giriyor, tekrarlayan plan önizlemede kalıyor ve vadesi gelince sorulmadan kayda dönüşüyor", "tur": "canli", "d": "E0244 · E0430"},
             {"t": "Hatırlatıcı listesi; hiçbir vade kendiliğinden gerçekleşmiyor, kaydederken hangi tarihe yazılacağı soruluyor", "tur": "kosum", "d": "E0043 · kullanıcı kontrolü"},
             {"t": "Planned payments; kapının açık kalıp kalmayacağı soruluyor ve sonradan değişebiliyor", "tur": "canli", "d": "E0318"},
             {"t": "Gelecek diye bir yer yok; unutulan ödeme iz bırakmıyor", "tur": "yok", "d": "E0171"},
             {"t": "İşlem formunda Schedule kutusu var; gerçekleşmesi ölçülmedi", "tur": "canli", "d": "E0133"}],
            ["Ayın toplamı neyi sayıyor",
             {"t": "Kaynağına göre bölünmüş: nakit-banka gideri, kart harcaması ve ödeme ayrı satırlar", "tur": "canli", "d": "E0405"},
             {"t": "Dönem akışını; ama benzer adlı ikinci bir blok varlığı sayıyor", "tur": "canli", "d": "E0053"},
             {"t": "Kayan bir pencereyi — son 30 gün ya da 12 hafta; takvim ayı ayrıca seçiliyor", "tur": "canli", "d": "E0284 · E0280 · E0439"},
             {"t": "Her satırı: açılış ve aktarım bacakları dâhil; ayın gideri 6.250 okunurken gerçek gider 2.050", "tur": "canli", "d": "E0142"},
             {"t": "Zarf başına kırılım; harcama raporunun toplamı bir gelir kaydını içeriyor", "tur": "canli", "d": "E0123"}],
            ["Toplamın tanımı ekranda yazıyor mu",
             {"t": "Evet — satır adlarında yazılı; dokuz üründe tek örnek", "tur": "canli", "d": "E0405"},
             {"t": "Hayır; iki blok aynı ekranda, hangisinin neyi saydığı yazmıyor", "tur": "canli", "d": "E0053"},
             {"t": "Hayır; ama kararı sorarken sonucunu aynı cümlede yazıyor", "tur": "canli", "d": "E0315"},
             {"t": "Hayır; sütun adları değişiyor, tanım değişmiyor", "tur": "canli", "d": "E0161"},
             {"t": "Hayır", "tur": "canli", "d": "E0123"}],
        ],
        "notlar": [
            "Sütunlar baştan aşağı okunduğunda her ürünün kendi mantığı çıkıyor; satırlar yan "
            "yana okunduğunda aynı sorunun beş cevabı. Tablonun işi bu iki okumayı aynı anda "
            "mümkün kılmak.",
            "Dördüncü satır belgenin en çok tekrarlanan bulgusunu taşıyor: beş üründe \"ayın "
            "toplamı\" beş ayrı şeyi sayıyor ve beşi de kendi içinde tutarlı.",
            "Kaynaktan incelenen dört ürün bu tabloda yok; onların modeli Bölüm 9'da kendi "
            "bölümünde kuruldu ve kanıt düzeyi farklı olduğu için yan yana konmadı.",
        ],
    },

    # ----------------------------------------------------------------- 10.4
    {
        "tur": "tablo", "no": "10.4", "baslik": "Ürün ürün: neyi iyi yapıyor, neyi bırakıyor",
        "giris": "Her ürün için iki satır: dokuz bölümde en güçlü olduğu yer ve en çok "
                 "kaybettirdiği yer.",
        "sutunlar": [("Ürün", 14), ("En güçlü olduğu yer", 43), ("En çok kaybettirdiği yer", 43)],
        "boy": 8.6,
        "satirlar": [
            ["Money Manager",
             {"t": "Toplamın tanımını ekrana yazan tek ürün: gider ödeme kaynağına göre bölünmüş, aktarım kendi satırında; kart dönemli ve taksit aylara dağılıyor", "tur": "canli", "d": "E0405"},
             {"t": "Aynı kart için iki ekran iki farklı borç gösteriyor ve hangisinin hangi dönemi kapsadığı yazmıyor", "tur": "canli", "d": "E0413"}],
            ["Bluecoins",
             {"t": "Cari bir hesap türü olduğu için tahsilat doğal bir aktarım; aktarımın iki bacağı günde netleşiyor; plan kayda çevrilirken tarih soruluyor", "tur": "canli", "d": "E0079"},
             {"t": "Benzer adlı iki toplam aynı ekranda: biri dönem akışı, öteki varlık", "tur": "canli", "d": "E0032"}],
            ["Wallet",
             {"t": "Bir kaydın bakiyeye ve kapının davranışına dair kararı kullanıcıya soran ve sonucunu yazan tek ürün; borç ayrı bir nesne, vadesi var; bütçe tahminle birlikte geliyor", "tur": "canli", "d": "E0315 · E0291"},
             {"t": "Rapor aralığı takvim ayı değil kayan bir pencere; kart dönemsiz ve taksit bölünmüyor", "tur": "canli", "d": "E0284 · E0280"}],
            ["Hesap Defterim",
             {"t": "Defter mantığı tek sayıda doğru neti veriyor; verinin cihazda durduğunu açıkça yazıyor; silinen kayıt için ayrı bir liste var", "tur": "canli", "d": "E0149"},
             {"t": "Ayın gelir ve gider toplamı kullanılamıyor; kart, taksit, plan ve cari kavramlarının hiçbiri yok", "tur": "canli", "d": "E0142"}],
            ["Goodbudget",
             {"t": "Para amacına göre bölünmüş; bütçe ile bakiye aynı şey olduğu için ayrı bir bütçe kavramı gerekmiyor", "tur": "canli", "d": "E0417"},
             {"t": "Zarf ile hesap tutmuyor ve fark her doldurmada büyüyor; harcama raporunun toplamı bir gelir kaydını içeriyor", "tur": "canli", "d": "E0123"}],
            ["KolayBi",
             {"t": "Cari, proje, çek-senet ve personel carisi birinci sınıf kavramlar; stok birimi varyant ile deponun çifti; vergi oran başına matrah ve tutar olarak raporlanıyor", "tur": "kaynak", "d": "E0214"},
             {"t": "Yalnız yüzey görüldü; hiçbir alanın kasaya, cariye veya rapora etkisi ölçülemedi", "tur": "yok", "d": "E0192"}],
            ["QuickBooks Solopreneur",
             {"t": "Kayıt başına işletme/şahsi alanı ve kısmen işletme gider için bölme; şahsi kayıt silinmeden toplamdan çıkıyor", "tur": "beyan", "d": "Intuit yardım merkezi"},
             {"t": "Ürünün hiçbir iç ekranı görülmedi; ayrım ülkeye özgü bir vergi formuna bağlı", "tur": "yok", "d": "Intuit yardım merkezi"}],
            ["Paraşüt",
             {"t": "Kaynağa göre gider beş türe ayrılmış, tahsilat en gecikmiş açık faturadan başlayarak otomatik mahsuplaşıyor, vergi ay bazında net KDV'ye kadar gidiyor ve stok ek ücretsiz", "tur": "beyan", "d": "parasut.com kılavuzu"},
             {"t": "İç arayüz hiç görülmedi; elde yalnız mağaza ve tanıtım videosu kareleri var", "tur": "yok", "d": "parasut.com kılavuzu"}],
            ["Logo İşbaşı",
             {"t": "Kaynağa göre tek kayıt cari, kasa-banka ve stoku birlikte güncelliyor; fatura sesle kesilebiliyor; müşavir portalı dosya alışverişini kaldırıyor", "tur": "beyan", "d": "isbasi.com"},
             {"t": "İç arayüz hiç görülmedi; müşavirin yetki sınırı ve iz katmanı arandı, kaynakta yok. Ürünün kendi ifadesiyle oluşturulan fatura kayıtlarının resmî değeri de yok", "tur": "yok", "d": "isbasi.com"}],
        ],
        "notlar": [
            "Sağdaki sütun bir puanlama değil: her satır bir tasarım tercihinin bedeli. İki "
            "sütun da aynı tercihten doğuyor.",
            "Son dört ürünün kaybettirdiği yer aynı: erişim. Bu, ürünün değil araştırmanın "
            "sınırı.",
        ],
    },

    # ----------------------------------------------------------------- 10.5
    {
        "tur": "akis", "no": "10.5", "baslik": "Bir paranın belgedeki yolu",
        "giris": "Dokuz bölümün akış sayfaları tek bir yolda birleştiğinde şu çıkıyor. Her "
                 "adımda ürünlerin ayrıldığı nokta, ilgili bölümde kareyle gösterilmişti.",
        "adimlar": [
            {
                "baslik": "Olay oluyor, kayda geçiyor",
                "dallar": [
                    {"urunler": "Canlı beş ürün",
                     "metin": "Bir satır. Yazıldığı anda hesap bakiyesi değişiyor ve ayın "
                              "toplamına giriyor.", "d": "E0142"},
                    {"urunler": "Üç ön muhasebe ürünü", "vurgu": True,
                     "metin": "Bir belge. Cari borç oluşuyor, kasa kıpırdamıyor; ödeme ayrı "
                              "bir adım.", "d": "E0200"},
                ],
            },
            {
                "baslik": "Kaç deftere yazılıyor",
                "dallar": [
                    {"urunler": "Dört canlı ürün",
                     "metin": "Tek defter. Toplam kendiliğinden tutuyor.", "d": "E0235"},
                    {"urunler": "Goodbudget", "vurgu": True,
                     "metin": "İki defter: zarf ve hesap. İkisi tutmuyor ve fark büyüyor.",
                     "d": "E0421"},
                    {"urunler": "Logo İşbaşı",
                     "metin": "Üç defter: cari, kasa-banka ve stok birlikte.", "d": "isbasi.com"},
                ],
            },
            {
                "baslik": "Gelecekteki para ne oluyor",
                "dallar": [
                    {"urunler": "Money Manager · Wallet · Bluecoins",
                     "metin": "Plan görünür kalıyor ama ayın toplamına girmiyor; gerçekleşme "
                              "ürüne göre kendiliğinden, onayla ya da elle.",
                     "d": "E0244 · E0291 · E0430"},
                    {"urunler": "Hesap Defterim", "vurgu": True,
                     "metin": "Gelecek diye bir yer yok. Unutulan ödeme hiç iz bırakmıyor.",
                     "d": "E0171"},
                    {"urunler": "Ön muhasebe tarafı",
                     "metin": "Ödenmemiş belgeler cari ve vade kavramlarında bekliyor.",
                     "d": "parasut.com kılavuzu"},
                ],
            },
            {
                "baslik": "Toplamda okunuyor",
                "dallar": [
                    {"urunler": "Money Manager", "vurgu": True,
                     "metin": "Toplamın tanımı satır adlarında yazılı; aktarım kendi satırında.",
                     "d": "E0405"},
                    {"urunler": "Bluecoins · Wallet · Goodbudget",
                     "metin": "Tek sayı veriliyor; neyi saydığı ekranda yazmıyor.",
                     "d": "E0032"},
                    {"urunler": "Hesap Defterim",
                     "metin": "Toplam her satırı içeriyor: açılış ve aktarım bacakları dâhil.",
                     "d": "E0142"},
                ],
            },
        ],
        "notlar": [
            "Bu yol dokuz bölümün akış sayfalarının birleşimidir; her dalın kanıtı ilgili "
            "bölümdedir.",
            "İlk adımdaki çatallanma diğer üçünü de belirliyor: kaydı bir satır mı yoksa bir "
            "belge mi sayıyorsunuz.",
        ],
    },

    # ----------------------------------------------------------------- 10.6
    {
        "tur": "cikarim", "no": "10.6", "baslik": "Belgenin bıraktığı yer",
        "giris": "Bu belge rakipleri anlattı ve puanlamadı. Ortaya çıkan şey bir sıralama değil, "
                 "bir soru listesi — ve o liste Belge 3'ün girdisi. Bölümlerde 37 soru var; aşağıdaki "
                 "liste her bölümü en az bir soruyla temsil ediyor.",
        "mekanizma": [
            {
                "baslik": "Ayrışmanın tek bir kökü var",
                "metin": [
                    "Dokuz bölümde ölçülen farkların çoğu — 10.1'de dokuz ayrımın yedisi — tek bir "
                    "soruya bağlanıyor: ürün bir kaydı kaç yere yazıyor ve o yerlerin her birine "
                    "ne ad veriyor.",
                    "Tek deftere yazan üründe toplam kendiliğinden tutuyor ama defterin tanımı "
                    "kirlenebiliyor. İki deftere yazanda tanım temiz kalıyor ama iki defterin "
                    "uyuşması ayrıca çözülmesi gereken bir iş oluyor.",
                    "Üç deftere yazan taraf bunu bir kapsam genişlemesi olarak sunuyor — cari, "
                    "kasa ve stok birlikte — ve karşılığında kaydın bir karşı tarafı olmasını "
                    "zorunlu kılıyor.",
                ],
                "dayanak": "E0142, E0421, isbasi.com, E0200",
            },
            {
                "baslik": "Ürünler sayıyı veriyor, tanımı vermiyor",
                "metin": [
                    "Üç ayrı bölümde aynı sonuç çıktı: aynı şeyin iki ekranda iki sayısı var ve "
                    "ikisi de doğru. Eksik olan tanım — hangi sayının hangi soruyu "
                    "cevapladığı.",
                    "Bunu ekranda yazan tek örnek, toplamı ödeme kaynağına göre bölüp satır "
                    "adına yazan yüzey. Bedeli okuma yükü; kazancı yanlış okumanın "
                    "zorlaşması.",
                    "Bu belgenin en çok tekrarlanan bulgusu bu: kart, borç ve rapor "
                    "bölümlerinde üç ayrı konuda çıktı.",
                ],
                "dayanak": "E0413, E0317, E0032, E0405",
            },
            {
                "baslik": "Kararı kullanıcıya sormak nadir ama mümkün",
                "metin": [
                    "İki üründe üç ayrı yerde ürün kendi kararını vermek yerine kullanıcıya "
                    "sordu: borç bakiyeye dokunsun mu, gelecek ödemeler kendiliğinden kayda "
                    "dönüşsün mü, plan hangi tarihe yazılsın.",
                    "Üçünde de soru, cevabın sonucunu aynı cümlede söylüyor. Bu, tanımı ekranda "
                    "yazmanın bir başka biçimi: ürün kendi davranışını kullanıcıya açıklıyor.",
                    "Money Manager'ın tekrarlı kayıt sorusu da sonucunu yazıyor — \"Tarihte tekrar "
                    "eden işlemler uygulanır\" — ama kullanıcıya bir kapı vermiyor, kapının "
                    "olmadığını haber veriyor.",
                    "Diğer yerlerde bu kararlar verilmiş ve yazılmamış; kullanıcı sonucu ancak "
                    "toplamlara bakarak anlıyor.",
                ],
                "dayanak": "E0315, E0291, E0043, E0430",
            },
        ],
        "kazanc": [
            ("Tek defter",
             "Toplam kendiliğinden tutuyor; ikinci bir uyuşma kuralı gerekmiyor",
             "Defterin tanımı kirlenebiliyor: açılış ve aktarım bacakları aynı toplama giriyor"),
            ("İki defter",
             "Amaç ile para ayrı okunabiliyor; kullanıcı ne kadar harcayabileceğini doğrudan "
             "görüyor",
             "İki defterin uyuşması ayrı bir iş; uyuşmazsa fark sessizce büyüyor"),
            ("Üç defter · belge tabanlı",
             "Cari, stok ve kasa tek kayıttan besleniyor; vade ve karşı taraf kendiliğinden "
             "geliyor",
             "Kaydın bir karşı tarafı olmak zorunda ve kayıt iki adıma bölünüyor"),
            ("Tanımı yazan yüzey",
             "Yanlış okuma zorlaşıyor: toplamın neyi içerdiği satır adında yazılı",
             "Okuma yükü artıyor: tek sayı yerine birkaç satır"),
            ("Kararı soran yüzey",
             "Ürünün davranışı kullanıcıya açıklanıyor ve iki yol da açık kalıyor",
             "Her kararda bir soru daha; akış uzuyor"),
        ],
        "soru": [
            "Kayıt kaç deftere yazılmalı ve o defterlerin uyuşması nasıl garanti edilmeli?",
            "Ayın gelir ve gider toplamı hangi kayıtları içermeli? Açılış bakiyesi, aktarım "
            "bacakları ve kart ödemesi üç ayrı sınav ve ürünler üç ayrı cevap veriyor.",
            "Aynı şeyin iki sayısı olabilir mi? Üç bölümde de olabildiği görüldü; sorun sayılar "
            "değil, aralarındaki farkın görünmemesi.",
            "Bir sayının neyi ve hangi dönemi kapsadığı ekranda yazmalı mı? Yazmanın bedeli "
            "okuma yükü, yazmamanın bedeli yanlış okuma.",
            "Bir kaydın bakiyeye dokunup dokunmayacağı kullanıcıya sorulmalı mı, yoksa ürünün "
            "kararı mı olmalı?",
            "Ekonomik olayın kaydı ile para hareketi ayrı iki adım mı olmalı? Ayrıldığında cari "
            "ve vade kavramları gerekiyor; ayrılmadığında henüz gelmemiş para kayda giremiyor.",
            "İşletme ile şahsi ayrımı sabit bir alan mı, kullanıcı tanımlı bir eksen mi olmalı? "
            "Dokuz üründe yalnız biri sabit alan kullanıyor ve o da hiç görülemedi.",
            "Plan gerçek kayda hangi kapıdan ve hangi tarihle dönmeli? Ürünler planı "
            "kendiliğinden, onayla ya da elle dönüştürüyor; seçilen tarih ayın toplamını "
            "değiştiriyor.",
            "Veri kimin sorumluluğunda durmalı: cihazın mı, hesabın mı? İki model iki ayrı "
            "sorumluluk dağılımı üretiyor ve kullanıcı bunu ancak ürün ekranda yazarsa öğreniyor.",
            "Uygulama vergi tutarını hesaplamalı mı, yoksa kullanıcının girdiğini taşıyıp "
            "raporlamalı mı? Hesaplayan ürün ayın en zahmetli işini üstleniyor; karşılığında "
            "oran listesini güncel tutma sorumluluğunu ve yanlış sayının sonucunu alıyor.",
            "Kaydın bir kalemi olmalı mı? Kalem taşıyan üründe \"ne kadar kaldı\" sorusu "
            "kendiliğinden cevaplanıyor; taşımayanda kayıt daha kısa ve ne alındığı serbest "
            "metinde kalıyor.",
        ],
    },
]

EKSIKLER = [
    ("—", "Bu bölüm yeni ölçüm içermez; eksikler ilgili bölümlerin listelerindedir", "—",
     "Bölüm 1–9 eksik listeleri", "—"),
]
