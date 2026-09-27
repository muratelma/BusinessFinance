# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 2 · Gezinme ve ana ekran — tek icerik kaynagi (plan §3)."""

NO = 2
BASLIK = "Gezinme ve ana ekran"
ANA_SORU = "Uygulama açıldığında ekran neyi önce söylüyor ve kullanıcıyı nereye çağırıyor?"
GIRIS = [
    "Bu bölüm açılıştan ilk kayda kadar olan yolu izler: ilk açılışta ne istendiği, ana ekranın "
    "ilk sırada ne gösterdiği, bölümlerin nerede durduğu, kaydın nereden başladığı ve veri yokken "
    "ekranda ne kaldığı.",
    "Anlatım ekranın üzerinden yürür. Karelerin üstündeki numaralı işaretler yalnız bu belgenin "
    "kopyalarına çizildi; araştırmanın özgün kareleri değişmedi.",
]
GIRMEZ = [
    "Form alanları ve kayıt sonrası geri bildirim → 4",
    "Hesap ve kart yüzeyi → 5",
    "Bekleyen işler ekranı → 7",
    "Rapor ekranı → 9",
    "Renk ve tutar biçimi → 3",
    "Kaydın finansal sonucu → Belge 2",
]
KAPSAM = [
    ("Money Manager", ["canli", "kosum"], "İlk açılışın karesi yok."),
    ("Bluecoins", ["canli"], None),
    ("Wallet", ["canli"], "İlk açılış ve boş ana ekran görülmedi."),
    ("Hesap Defterim", ["canli"], None),
    ("Goodbudget", ["canli"], "Boş ana ekran görülmedi."),
    ("KolayBi", ["kaynak"], "Web ana ekranı; 2026 videosunda daha yeni bir sürümü görüldü."),
    ("Paraşüt", ["kosum", "beyan"], "Giriş ekranı geçilemedi."),
    ("Logo İşbaşı", ["kosum"], "Yalnız kayıt yüzeyi görüldü."),
    ("QuickBooks Solopreneur", ["beyan"], "Solopreneur ekranı görülmedi."),
]

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 2.1
    {
        "tur": "soru", "no": "2.1", "baslik": "İlk açılışta ne isteniyor?",
        "giris": "İlk açılışta ekran bir şey isteyebilir, ne yapılacağını anlatabilir ya da doğrudan "
                 "çalışmaya başlayabilir. Canlı incelenen beş üründen üçünün ilk açılış yolu karede.",
        "yukseklik": 340, "not_genislik": 290,
        "sekiller": [
            {"k": "bc-ilk", "e": "E0017", "etiket": "Bluecoins · karşılama ekranı",
             "isaretler": [
                 (1, 540, 1416, "Karşılama cümlesi ne yapılacağını söylemiyor."),
                 (2, 930, 1758, "Tek ilerleme yolu: Hadi Başlayalım."),
                 (3, 880, 2184, "Dil ekranın altında seçiliyor."),
             ]},
            {"k": "hd-ilk", "e": "E0135", "etiket": "Hesap Defterim · ilk açılış",
             "isaretler": [
                 (4, 700, 1046, "Diyalog uygulamanın ne işe yaradığını anlatıyor."),
                 (5, 44, 1247, "Metin Ücretli ve Alınan düğmelerinden söz ediyor."),
                 (6, 60, 1970, "Ekrandaki düğmelerin adı Ödendi ve Alındı."),
             ]},
            {"k": "gb-ilk", "e": "E0109", "etiket": "Goodbudget · yeni haneden sonraki ilk ekran",
             "isaretler": [
                 (7, 672, 210, "Yeni haneden sonra ilk istenen: bütçe kurulumu."),
                 (8, 744, 486, "Ücretsiz pakette zarf sayacı, grup başına."),
                 (9, 672, 1980, "Tahmini aylık gelir ve dağıtılmadan kalan."),
                 (10, 540, 2262, "Adımlı kurulum: BACK ve NEXT."),
             ]},
        ],
        "notlar": [
            ("Goodbudget'ın ilk ekranı yalnız bir hane olup olmadığını soruyor: LOG IN ya da CREATE NEW "
             "HOUSEHOLD.", "E0106"),
            ("E-posta ve parolayla hane kaydı bütçe kurulup zarflar doldurulduktan sonra geliyor ve LATER "
             "düğmesi taşıyor; kaydın atlanması denenmedi.", "E0110"),
            "Hesap Defterim'in diyaloğu düğmeleri ekrandakinden farklı adla anıyor (işaret 5 ve 6).",
        ],
        "urunler": [
            ("Money Manager", "kosum", "Karşılama, kayıt veya giriş ekranı gelmedi; uygulama ana ekranla "
             "açıldı. Karesi yok, kullanıcı doğruladı.", "E0010 K00"),
            ("Wallet", "yok", "İlk açılış görülmedi; inceleme önceden açılmış hesapla yapıldı.", "E0014 K00"),
            ("Logo İşbaşı", "kosum", "Kayıt formu üç alan, SMS doğrulaması ve sözleşme onayı istiyor.", "E0009"),
            ("Paraşüt", "kosum", "Giriş öncesi tanıtım ekranları görüldü; giriş ekranı geçilemedi.", "E0011"),
            ("QuickBooks Solopreneur", "yok", "Solopreneur'ün ilk açılışı görülmedi.", "E0012"),
        ],
    },

    # ---------------------------------------------------------------- 2.2
    {
        "tur": "yanyana", "no": "2.2", "baslik": "Ana ekran neyi önce gösteriyor?",
        "giris": "Beş ürünün dolu ana ekranı aynı yükseklikte. Altındaki satırlar yalnız karede görüneni "
                 "yazar: ilk sırada ne var, alt çubuk var mı, tanıtım var mı, toplam satırı nerede.",
        "yukseklik": 262,
        "sekiller": [
            {"k": "mm-ana", "e": "E0228", "ad": "Money Manager", "etiket": "İşlemler › Gün",
             "satirlar": ["İlk sırada: seçili ayın Gelir, Gider ve Toplam satırı; altında gün gün liste.",
                          "Alt çubuk: var.", "Tanıtım: bu karede yok."]},
            {"k": "bc-ana", "e": "E0103", "ad": "Bluecoins", "etiket": "Hesaplar sekmesi",
             "satirlar": ["İlk sırada: Günlük Özet kartı; ardından Bütçe Özeti.",
                          "Alt çubuk: yok; sekmeler üstte.", "Tanıtım: iki kartın arasında reklam."]},
            {"k": "wl-ana", "e": "E0276", "ad": "Wallet", "etiket": "Home › Accounts",
             "satirlar": ["İlk sırada: hesap kartları ve bakiyeleri.",
                          "Alt çubuk: yok; çekmece ve iki ekran içi sekme.",
                          "Tanıtım: kartların hemen altında üç tanıtım kartı."]},
            {"k": "hd-ana", "e": "E0137", "ad": "Hesap Defterim", "etiket": "Ana Hesap › Herşey",
             "satirlar": ["İlk sırada: dönem çipleri ve seçili defterin hareket tablosu.",
                          "Alt çubuk: yok; altta iki kayıt düğmesi.",
                          "Toplam satırı: ekranın altında sabit bant."]},
            {"k": "gb-ana", "e": "E0115", "ad": "Goodbudget", "etiket": "ENVELOPES",
             "satirlar": ["İlk sırada: zarf listesi; üstte dağıtılmış toplam.",
                          "Alt çubuk: yok; dört sabit üst sekme.", "Tanıtım: bu karede yok."]},
        ],
        "notlar": [
            "Bu beş karede ilk sırada üç farklı şey duruyor: hesap bakiyeleri (Wallet), zamana göre "
            "sıralı hareketler (Money Manager, Hesap Defterim) ve bütçe dağılımı (Goodbudget). Bluecoins'in "
            "bu sekmesi özet kartlarıyla başlıyor.",
            "Beş kareden yalnız Hesap Defterim'inkinde toplam ekranın altında sabit bir bantta duruyor.",
        ],
        "sag_notlar": [
            ("Wallet'ın ana ekranı karenin altında devam ediyor; aşağısında bakiye eğilimi ve bekleyen ödeme "
             "kartları var (→ 7.4).", "E0469"),
            ("Bluecoins uygulama yeniden açıldığında Hesaplar sekmesiyle geliyor; bu kare aynı sekmeye "
             "gezinmeyle dönülerek alındı.", "E0486"),
            ("Hesap Defterim birden çok defter varken son kullanılan defterle açılıyor; arada defter seçimi "
             "yok.", "E0496"),
        ],
    },
    {
        "tur": "soru", "no": "2.2", "baslik": "Ana ekran neyi önce gösteriyor? — işaretli okuma",
        "haritada": False,
        "giris": "Üç ürünün bu ana ekran karelerinde bir toplam ya da özet duruyor; toplamın neyin "
                 "toplamı olduğu üçünde farklı.",
        "yukseklik": 360,
        "sekiller": [
            {"k": "mm-ana-i", "e": "E0228", "etiket": "Money Manager · İşlemler › Gün",
             "isaretler": [
                 (1, 420, 190, "Dönem seçici: ekrandaki her sayı bu aya ait."),
                 (2, 360, 425, "Gelir, Gider ve Toplam; üçü de seçili ayın."),
                 (3, 470, 520, "Gün başlığında o günün geliri ve gideri."),
                 (4, 700, 667, "Satırda kategori, not ve hesap birlikte."),
             ]},
            {"k": "bc-ana-i", "e": "E0103", "etiket": "Bluecoins · Hesaplar sekmesi",
             "isaretler": [
                 (5, 560, 240, "Sekmeler üstte; son sekmenin adı kesiliyor."),
                 (6, 640, 530, "İlk kart: Günlük Özet, gider grafiği ve gün ortalamaları."),
                 (7, 1005, 1400, "İki kartın arasında reklam."),
                 (8, 640, 1880, "Sonraki kart: Bütçe Özeti."),
             ]},
            {"k": "gb-ana-i", "e": "E0115", "etiket": "Goodbudget · ENVELOPES",
             "isaretler": [
                 (9, 520, 206, "Hane adı; bu kopyada karartıldı."),
                 (10, 690, 454, "Total: zarflara dağıtılmış tutar."),
                 (11, 650, 600, "Zarf satırında iki sayı; ekranda adları yazmıyor."),
                 (12, 540, 1500, "Zarflar bitince kare boş devam ediyor."),
             ]},
        ],
        "notlar": [
            ("Money Manager ve Goodbudget'ın bu karelerinde hesap bakiyesi yok; bakiyeler ayrı "
             "sekmede (→ 5.1).", "E0228, E0115, E0236"),
            ("Zarf satırındaki iki sayının anlamı koşum kaydına dayanır: üstteki kalan, alttaki bütçelenen. "
             "Kare tek başına ayırmıyor.", "E0006 K01; GB-03"),
            "Üç toplamın neyi saydığı → Belge 2 1.3, 1.4.",
        ],
        "dayanak": ["Koşum kaydı: E0006 K01 (zarf sayılarının anlamı)"],
    },

    # ---------------------------------------------------------------- 2.3
    {
        "tur": "serit", "no": "2.3", "baslik": "Bölüm seçici nerede duruyor?",
        "giris": "Gezinme bölgeleri aynı genişlikte kırpıldı. Kırpıntı tek başına bölgenin ekranın neresinde "
                 "olduğunu göstermediği için her birinin yanında konumu ve tam ekran şekli yazıyor.",
        "kirpintilar": [
            {"e": "E0228", "ad": "Money Manager", "kutu": (0, 2200, 1080, 2345), "konum": "Ekranın altı",
             "aciklama": "Dört sabit sekme. Tam ekran: [[mm-ana]]."},
            {"e": "E0115", "ad": "Goodbudget", "kutu": (0, 290, 1080, 410), "konum": "Ekranın üstü",
             "aciklama": "Dört sabit sekme. Tam ekran: [[gb-ana]]."},
            {"e": "E0103", "ad": "Bluecoins", "kutu": (0, 150, 1080, 440), "konum": "Ekranın üstü",
             "aciklama": "Çekmece düğmesi ve yana kaydırılan sekmeler; son sekmenin adı kesiliyor. "
                         "Tam ekran: [[bc-ana]]."},
            {"e": "E0276", "ad": "Wallet", "kutu": (0, 120, 1080, 430), "konum": "Ekranın üstü",
             "aciklama": "Çekmece düğmesi ve ekran içi iki sekme. Tam ekran: [[wl-ana]]."},
            {"e": "E0137", "ad": "Hesap Defterim", "kutu": (0, 140, 1080, 400), "konum": "Ekranın üstü",
             "aciklama": "Çekmece düğmesi, defter seçici ve dönem çipleri. Tam ekran: [[hd-ana]]."},
        ],
        "notlar": [
            "Money Manager ve Goodbudget'ta bölüm adları ekranda sürekli duruyor; Wallet ve Hesap "
            "Defterim'de bölüm listesi çekmece açılınca görünüyor.",
            "Bluecoins ikisini birlikte kullanıyor: üstte sekmeler, solda çekmece düğmesi (→ 2.4).",
            "Hesap Defterim'de başlıktaki açılır seçici bölüm değil, gösterilen defteri belirliyor.",
            "Wallet'ın ekran içi iki sekmesi çekmecedeki bölümlerden ayrı bir düzey.",
        ],
        "urunler_baslik": "Kaynakla incelenen ürünler",
        "urunler": [
            ("KolayBi", "kaynak", "Web panelinde solda sabit modül menüsü (→ 2.7). Mobil gezinme görülmedi.", "E0211"),
            ("Paraşüt, Logo İşbaşı, QuickBooks Solopreneur", "yok", "İç gezinme görülmedi.", "E0011, E0009, E0012"),
        ],
    },

    # ---------------------------------------------------------------- 2.4
    {
        "tur": "soru", "no": "2.4", "baslik": "Çekmecenin içinde ne var?",
        "giris": "Çekmece kullanan üç üründe çekmecenin içi tek düzeyde listeleniyor: finansal bölümler, "
                 "yardımcı araçlar ve ürün bakımı kalemleri aynı listede.",
        "yukseklik": 352, "not_genislik": 250,
        "sekiller": [
            {"k": "wl-cekmece", "e": "E0376", "etiket": "Wallet · çekmecenin alt kısmı",
             "isaretler": [
                 (1, 720, 170, "Debts: borç defteri."),
                 (2, 560, 330, "Araya başka bir ürünün tanıtımı giriyor."),
                 (3, 720, 700, "Alışveriş listesi, garanti ve sadakat kartı aynı listede."),
                 (4, 860, 1631, "Görünüm anahtarları da listede."),
             ]},
            {"k": "hd-cekmece", "e": "E0171", "etiket": "Hesap Defterim · çekmece",
             "isaretler": [
                 (5, 800, 368, "İlk kalem reklam kaldırma."),
                 (6, 800, 998, "Aktar: defterler arası para hareketi."),
                 (7, 800, 1628, "Nakit hesap makinesi ve not defteri aynı listede."),
                 (8, 800, 2006, "Silinmiş işlemler de aynı düzeyde."),
             ]},
            {"k": "bc-cekmece", "e": "E0084", "etiket": "Bluecoins · çekmece",
             "isaretler": [
                 (9, 640, 445, "İlk Hesaplar kalemi: kart panosunu açıyor."),
                 (10, 640, 700, "İkinci Hesaplar kalemi: hesap listesini açıyor."),
                 (11, 640, 1075, "Çöp Kutusu aynı listede."),
                 (12, 560, 1500, "Seyahat Modu anahtarı da listede."),
             ]},
        ],
        "notlar": [
            "Üç çekmecede de bölümler ile yardımcı araçlar arasında başlık, ayraç veya gruplama görünmüyor.",
            ("Bluecoins'in ikinci Hesaplar kalemi hesapları VARLIKLAR ve CARİ HESAP başlıkları altında "
             "listeliyor; ilki özet kartlarının panosu.", "E0488, E0489"),
            ("Wallet çekmecesinde katlanmış Others bölümü açılınca Imports, Exports ve Locations çıkıyor "
             "(→ 10.1).", "E0450"),
        ],
        "urunler": [
            ("Money Manager", "canli", "Çekmece görülmedi. Ayarlar, hesaplar, yedek ve yardımcı araçlar "
             "dördüncü sekme Daha'da tek ızgarada (→ 10.4).", "E0254"),
            ("Goodbudget", "yok", "Çekmece görülmedi; bölümler sabit sekmelerde (→ 2.3).", "E0115"),
        ],
    },

    # ---------------------------------------------------------------- 2.5
    {
        "tur": "soru", "no": "2.5", "baslik": "Kayıt nereden başlıyor?",
        "giris": "Dört üründe kayıt, sağ altta tek bir düğmeyle başlıyor ve paranın yönü sonra "
                 "açılan formda seçiliyor. Hesap Defterim yönü düğmenin adına taşımış.",
        "yukseklik": 404,
        "sekiller": [
            {"k": "hd-kayit", "e": "E0137", "etiket": "Hesap Defterim · ana ekran",
             "isaretler": [
                 (1, 62, 1970, "Alındı: para girişi."),
                 (2, 600, 1970, "Ödendi: para çıkışı."),
                 (3, 700, 720, "Her satırda o kayıttan sonraki denge."),
                 (4, 60, 2230, "Dönem toplamları ekranın altında sabit."),
             ]},
        ],
        "kirpinti_k": "kayit-dugmeleri",
        "kirpinti_baslik": "Diğer dört üründe kayıt başlatma",
        "kirpinti_etiket": "Dördünde de sağ altta tek düğme: Money Manager ve Goodbudget'ınki yuvarlak, "
                           "Bluecoins ve Wallet'ınki köşeleri yuvarlatılmış kare.",
        "kirpintilar": [
            ("E0228", "Money Manager", (884, 2008, 1044, 2168)),
            ("E0103", "Bluecoins", (884, 2150, 1044, 2300)),
            ("E0276", "Wallet", (884, 2140, 1044, 2300)),
            ("E0115", "Goodbudget", (884, 2140, 1044, 2300)),
        ],
        "notlar": [
            "Hesap Defterim'de iki yön ekranda; üçüncü yön olan aktarım çekmecedeki Aktar kaleminde ([[hd-cekmece]]).",
            ("Tek düğmeli dört üründe formda tür seçenekleri var; hangi türle açıldığı ve seçimin nasıl "
             "yapıldığı 4.3'te.", "E0229, E0020, E0277, E0116"),
            ("Hesap Defterim'de satırdaki denge ve alttaki önceki denge ayarla açılıp kapanıyor: \"Her işlemden "
             "sonra bakiyeyi göster\", \"Önceki denge\".", "E0150"),
            "Wallet'ta düğme önce bir ara menü açıyor (→ 4.1).",
        ],
    },

    # ---------------------------------------------------------------- 2.6
    {
        "tur": "soru", "no": "2.6", "baslik": "Veri yokken ekranda ne kalıyor?",
        "giris": "Üç kare üç ayrı duruma ait: kayıtsız bir ay, temiz kurulum ve boş defter. Üçünde de ekranın "
                 "yapısı büyük ölçüde duruyor; ayrışan, hangi sözcüklerin bulunduğu.",
        "yukseklik": 336,
        "sekiller": [
            {"k": "mm-bos", "e": "E0227", "etiket": "Money Manager · kayıtsız ay",
             "isaretler": [
                 (1, 420, 190, "Dönem seçici duruyor."),
                 (2, 300, 425, "Üç özet sayısı sıfır."),
                 (3, 760, 988, "Tek metin: Veri yok."),
                 (4, 760, 2080, "Kayıt düğmesi yerinde."),
                 (5, 560, 2120, "Alttaki mesaj çıkışla ilgili, ilk kayıt rehberi değil."),
             ]},
            {"k": "bc-bos", "e": "E0026", "etiket": "Bluecoins · temiz kurulum",
             "isaretler": [
                 (6, 560, 240, "Sekmeler duruyor."),
                 (7, 850, 1046, "Karşılama sözcüğü."),
                 (8, 850, 1236, "Sıfır bakiye, etiketiyle."),
                 (9, 850, 2003, "Para birimi yazıyor."),
                 (10, 900, 2234, "İlk eylem adıyla çağrılıyor: İlk İşlemi Ekle."),
             ]},
            {"k": "hd-bos", "e": "E0136", "etiket": "Hesap Defterim · boş defter",
             "isaretler": [
                 (11, 1010, 378, "Yedekleme daveti."),
                 (12, 730, 626, "Reklam kaldırma daveti."),
                 (13, 540, 904, "Sütun başlıkları duruyor."),
                 (14, 540, 1870, "İki yön düğmesi duruyor."),
                 (15, 960, 2295, "Altta banner reklam."),
             ]},
        ],
        "notlar": [
            "Bluecoins ilk eylemi adıyla çağırıyor ve para birimini yazıyor.",
            "Money Manager yalnız \"Veri yok.\" diyor.",
            "Hesap Defterim'in ekranında ilk kayda yönlendiren bir metin yok; görünen metinler yedekleme ve "
            "reklam davetleri.",
        ],
        "urunler": [
            ("Wallet, Goodbudget", "yok", "Boş ana ekran görülmedi; yeni hesap gerektirir.", "E0014, E0006"),
            ("Wallet", "canli", "Bölüm içi boş durumlar (Planned payments, Debts) ilk kaydı çağırıyor (→ 3.5).",
             "E0282, E0307"),
        ],
    },

    # ---------------------------------------------------------------- 2.7
    {
        "tur": "soru", "no": "2.7", "baslik": "Kaynakta görülen ana ekran",
        "giris": "İçine girilemeyen ürünlerde ana ekran yalnız ürünün kendi yayımladığı görsellerden ya da "
                 "anlatımından biliniyor. KolayBi'nin destek sayfasındaki görselde modül adları, sekmeler ve "
                 "sayaçlar okunabiliyor; yine de canlı bir inceleme değil.",
        "not_genislik": 250,
        "sekiller": [
            {"k": "kb-pano", "e": "E0211", "etiket": "KolayBi · destek sayfasındaki Güncel Durum ekranı",
             "kirpma": (96, 112, 1320, 872), "genislik": 500,
             "isaretler": [
                 (1, 84, 308, "Solda sabit modül menüsü."),
                 (2, 1009, 63, "Sağ üstte Hızlı İşlemler menüsü; seçenekleri kapalı."),
                 (3, 504, 110, "Ekranın içinde iş ortağı tanıtım bandı."),
                 (4, 464, 279, "Gelir ve gider için iki çizgili nakit akışı."),
                 (5, 1174, 473, "Günü Gelen İşlemler: Bugün, Yaklaşanlar, Tarihi Geçenler."),
                 (6, 804, 400, "Grafikteki demo verisi Ocak 2023 tarihli."),
             ]},
        ],
        "notlar": [
            "Görsel bir yayın görselidir; demo verisi taşıyor ve bugünkü sürümün ekranı olduğunu kanıtlamaz.",
            ("Aynı ekran eski bir tanıtım videosunun karesinde farklı düzenle görünüyor; kaynaklar tek bir ekran "
             "gibi birleştirilmedi.", "E0181"),
            ("2026'da yayımlanan bir videonun karesinde pano daha yeni: marka KolayBi Ofis, sol menüde ayrıca "
             "Fatura Ödeme, sağdaki listede BiLink yerine Müşavirini Davet Et. İskelet aynı: nakit akışı, Tahsilat "
             "ve Ödeme Özetleri, Günü Gelen İşlemler. Bugünkü sürüm olduğu doğrulanmadı. Video kareleri basılmadı.",
             "E0187"),
            "Ekran masaüstü genişliğinde; telefon kareleriyle aynı alan koşulunda karşılaştırılmaz.",
        ],
        "urunler_baslik": "Karesi basılmayan ürünler",
        "urunler": [
            ("Paraşüt", "beyan", "Ürünün tanıtım anlatımında ana ekran tahsilatı ve ödemeyi vade durumuna göre "
             "ayırıyor: tahsil edilecek, gecikmiş; ödenecek, planlanmış. Çalışan ana ekran görülmedi.",
             "E0011; video karesi E0262"),
            ("QuickBooks Solopreneur", "beyan", "Yardım merkezi anlatımı işlem listesini merkeze alıyor; Type ve "
             "Category ayrı sütun.", "E0012"),
            ("Logo İşbaşı", "yok", "Ana ekran görülmedi.", "E0009"),
        ],
        "dayanak": ["Kaynak beyanı: E0011, E0012"],
    },
]

EKSIKLER = [
    ("Money Manager", "İlk açılışın karesi yok; karşılama olmadığı kullanıcı beyanı", "2.1",
     "Temiz kurulum gerekir", "Önerilmez"),
    ("Wallet", "İlk açılış ve boş ana ekran görülmedi", "2.1, 2.6",
     "Oturum kapatma veya yeni hesap gerekir", "Önerilmez"),
    ("Goodbudget", "Boş ana ekran ve hane kaydını (LATER) atlamanın sonucu görülmedi", "2.1, 2.6",
     "Yeni hane gerekir", "Önerilmez"),
    ("KolayBi / Paraşüt", "Güncel sürüm ve çalışan ana ekran", "2.7", "Ücretli hesap gerekir", "Önerilmez"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0106", "E0110", "E0150", "E0181", "E0187", "E0254", "E0450", "E0469", "E0486", "E0488", "E0489", "E0496"]
