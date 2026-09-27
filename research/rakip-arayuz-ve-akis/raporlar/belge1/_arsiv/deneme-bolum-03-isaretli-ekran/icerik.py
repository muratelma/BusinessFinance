# -*- coding: utf-8 -*-
"""Belge 1 - Bolum 3 - isaretli ekran anlatimi.

Bu dosya bolumun TEK icerik kaynagidir: metin, isaret koordinatlari, bolge
olculeri ve kanit baglari burada durur. uret.py hem PDF'i hem okunabilir
Markdown kopyasini bu veriden uretir; metin iki yerde tutulmaz.

Koordinat sistemi: her isaret ozgun karenin kendi piksel uzayindadir
(telefon kareleri 1080x2400, KolayBi 1412x960, Parasut 1207x892).
"""

BASLIK = "Gezinme ve ana ekran"
UST_BASLIK = "Belge 1 · Rakip arayüz yaklaşımları · Bölüm 3"
ALT_BASLIK = "İşaretli ekran anlatımı · 16 Eylül 2026 · taslak"

# --------------------------------------------------------------------------
# Kanit kayitlari: E kimligi -> arastirma kokune gore yol
# --------------------------------------------------------------------------
KANITLAR = {
    "E0228": ("money-manager/03-dolu-ana-ekran.png", "Money Manager · Faz 1, 10 Eylül 2026", "canli"),
    "E0227": ("money-manager/02-bos-ana-ekran.png", "Money Manager · Faz 1, 10 Eylül 2026", "canli"),
    "E0231": ("money-manager/07-rapor-agustos.png", "Money Manager · Faz 1, 10 Eylül 2026", "canli"),
    "E0236": ("money-manager/12-hesaplar-kart-borcu-bu-ay.png", "Money Manager · Faz 1, 10 Eylül 2026", "canli"),
    "E0254": ("money-manager/30-ayarlar-izgarasi.png", "Money Manager · ek koşum", "canli"),
    "E0017": ("bluecoins/01-ilk-acilis.png", "Bluecoins · Tur 1", "canli"),
    "E0103": ("bluecoins/f7-52-nav-check.png", "Bluecoins · Faz 7, 11 Eylül 2026", "canli"),
    "E0056": ("bluecoins/f7-05-hatirlaticilar.png", "Bluecoins · Faz 7, 11 Eylül 2026", "canli"),
    "E0026": ("bluecoins/10-fresh-bos-ana-ekran.png", "Bluecoins · Faz 3, 10 Eylül 2026", "canli"),
    "E0018": ("bluecoins/02-bos-ana-ekran.png", "Bluecoins · Tur 1", "canli"),
    "E0084": ("bluecoins/f7-33-menu2.png", "Bluecoins · Faz 7, 11 Eylül 2026", "canli"),
    "E0276": ("wallet-budgetbakers/03-dolu-ana-ekran.png", "Wallet · Tur 1", "canli"),
    "E0274": ("wallet-budgetbakers/00-magaza.png", "Wallet · Tur 1", "canli"),
    "E0376": ("wallet-budgetbakers/f7-55-menu-scroll.png", "Wallet · Faz 7, 11 Eylül 2026", "canli"),
    "E0135": ("hesap-defterim/01-ilk-acilis-hosgeldin.png", "Hesap Defterim · Tur 1, 10 Eylül 2026", "canli"),
    "E0137": ("hesap-defterim/03-dolu-ana-ekran.png", "Hesap Defterim · Tur 1, 10 Eylül 2026", "canli"),
    "E0136": ("hesap-defterim/02-bos-ana-ekran.png", "Hesap Defterim · Tur 1, 10 Eylül 2026", "canli"),
    "E0171": ("hesap-defterim/36-drawer-menu-ust.png", "Hesap Defterim · 12 Eylül 2026", "canli"),
    "E0115": ("goodbudget/08-envelopes-filled-home.png", "Goodbudget · 11 Eylül 2026", "canli"),
    "E0106": ("goodbudget/01-ilk-acilis.png", "Goodbudget · 11 Eylül 2026", "canli"),
    "E0211": ("kolaybi/d24-destek-guncel-durum-panosu.png", "KolayBi · destek sayfası görseli, 12 Eylül 2026 erişim", "kaynak"),
    "E0181": ("kolaybi/d33-video-guncel-durum-panosu.png", "KolayBi · tanıtım videosu karesi", "kaynak"),
    "E0262": ("parasut/04-video-cari-hesap-durumu.png", "Paraşüt · tanıtım videosu karesi", "cizim"),
}

# Isaretlemeden once uygulanan kirpma. KolayBi karesi siyah bir zemin uzerine
# yerlestirilmis bir tarayici penceresidir; zemin belgeye tasinmaz.
KIRPMA = {
    "E0211": (96, 112, 1320, 872),
}

# Gizlilik: yalnizca bu kutular karartilir. Ozgun kanit degismez.
KARARTMA = {
    "E0115": [(168, 170, 452, 248)],  # e-postadan tureyen hesap adi
}

# --------------------------------------------------------------------------
# Kanit turu rozetleri
# --------------------------------------------------------------------------
ROZETLER = [
    ("canli", "Canlı kare", "Emülatörde açılan ekranın görüntüsü."),
    ("kayit", "Koşum kaydı", "Gözlem formunda yazılı; karesi yok veya kare tek başına göstermiyor."),
    ("kaynak", "Kaynak görseli", "Ürünün kendi destek sayfası veya tanıtım videosu. Davranış denenmedi."),
    ("cizim", "Temsili çizim", "Ürünün pazarlama görseli. Çalışan ekran olduğu doğrulanmadı."),
    ("yok", "Görülmedi", "Kanıt yok. Özelliğin bulunmadığı anlamına gelmez."),
]

# --------------------------------------------------------------------------
# SAYFA 1 - kapak
# --------------------------------------------------------------------------
GIRIS = [
    "Bu bölüm tek bir soruyu izler: uygulama açıldığında ekran neyi önce söylüyor ve kullanıcıyı nereye çağırıyor?",
    "Anlatım ekranın kendisi üzerinden yürür. Her karenin üzerine numaralı işaretler konmuştur ve metin o numaralara "
    "konuşur. İşaretler yalnız bu bölümün kopyaları üzerine çizildi; araştırmanın özgün kareleri değişmedi.",
    "Kanıt niteliği ürüne değil ifadeye bağlıdır. Bir ürünün canlı incelenmiş olması, o ürün hakkındaki her cümlenin "
    "sınanmış olduğu anlamına gelmez.",
]

KAPSAM = [
    ("Canlı koşumla incelendi", "Money Manager · Bluecoins · Wallet · Hesap Defterim · Goodbudget", "Android"),
    ("Yalnız kaynaktan görüldü", "KolayBi (destek sayfası, tanıtım videosu) · Paraşüt (tanıtım videosu)", "web / masaüstü"),
    ("Ana ekranı hiç görülmedi", "Logo İşbaşı · QuickBooks Solopreneur", "—"),
]

OLCMEZ = (
    "Bu bölüm başarı, hız, memnuniyet ve erişilebilirlik ölçmez; bunların hiçbiri için test yapılmadı. "
    "Kaydın finansal sonucu Belge 2'nin, BusinessFinance için tercihler Belge 3'ün konusudur."
)

# Sayfa haritasi (kapak sayfasinin altinda basilir)
BOLUM_HARITASI = [
    ("2", "Beş ekran, beş farklı bölüşüm", "Aynı yükseklikte yan yana; bölge ölçüleri"),
    ("3", "İlk açılışta ne isteniyor?", "Bluecoins · Hesap Defterim · Goodbudget"),
    ("4", "Ana ekran önce neyi söylüyor?", "Money Manager · Goodbudget"),
    ("5", "Bölümler nerede duruyor?", "Beş üründe bölüm seçici"),
    ("6", "Çekmecenin içinde ne var?", "Wallet · Hesap Defterim"),
    ("7", "Kayıt nereden başlıyor?", "Hesap Defterim ve dört yuvarlak düğme"),
    ("8", "Bekleyen işler nerede görünüyor?", "Wallet · Bluecoins"),
    ("9", "Veri yokken ekranda ne kalıyor?", "Üç farklı boşluk durumu"),
    ("10", "Kaynaktan görülen pano: KolayBi", "Destek sayfası görseli"),
    ("11", "Temsili çizim: Paraşüt", "Neden davranış kanıtı değil"),
    ("12", "Kanıt eki", "Şekil dizini · çıkarılmayan sonuçlar · eksikler"),
]

# Sayfa 2'nin altina dusulen not
S2_NOT_DUGME = (
    "Yüzen kayıt düğmesi bir bant kaplamadığı için oranlara girmiyor. Money Manager, Bluecoins, Wallet ve "
    "Goodbudget'ın dördünde de var (Şekil 6.2); tabloda yalnız Hesap Defterim'in sabit bandı görünüyor."
)

# Sayfa 4'un sag sutunu
SERIT_NOTLARI = [
    "Beş yerleşimin üçünde bölüm adları ekranda sürekli duruyor (Money Manager, Goodbudget, Bluecoins); "
    "ikisinde bölümler ancak çekmece açılınca görünüyor (Wallet, Hesap Defterim).",
    "Bluecoins'in sekme şeridi ekrana sığmıyor: son sekmenin adı kesiliyor. Kaç sekme olduğu bu kareden "
    "sayılamıyor.",
    "Hesap Defterim'de başlıktaki açılır seçici bölüm değil, hangi defterin gösterildiğini belirliyor; "
    "çekmeceyle aynı yerde duran ama farklı iş yapan iki kontrol.",
    "Wallet'ta ekran içi iki sekme (Accounts, Budgets & Goals) çekmecedeki hedeflerden ayrı bir düzey.",
    "Görülmedi: Logo İşbaşı ve QuickBooks Solopreneur'ün iç gezinmesi; Paraşüt çiziminde menü alanları boş.",
]

# --------------------------------------------------------------------------
# SAYFA 2 - bes ekranin bolusumu
# --------------------------------------------------------------------------
# Bolgeler: (baslangic_px, bitis_px, tur) - tur: gezinme | para | tanitim | bos | kayit | notr
BOLGE_RENKLERI = {
    "gezinme": (0x6E, 0x5A, 0x9E),
    "para": (0x1F, 0x6F, 0xB2),
    "tanitim": (0x8A, 0x93, 0xA0),
    "kayit": (0x2E, 0x8B, 0x57),
    "bos": (0xDD, 0xE2, 0xE8),
    "notr": (0xF2, 0xF4, 0xF7),
}
BOLGE_ADLARI = [
    ("gezinme", "Gezinme ve denetim"),
    ("para", "Para bilgisi"),
    ("tanitim", "Tanıtım / reklam"),
    ("kayit", "Kayıt başlatma"),
    ("bos", "Boş alan"),
]

BOLUSUM = [
    ("E0228", "Money Manager", [
        (0, 108, "notr"), (108, 228, "gezinme"), (228, 342, "gezinme"),
        (342, 470, "para"), (470, 1660, "para"), (1660, 2210, "bos"),
        (2210, 2340, "gezinme"), (2340, 2400, "notr"),
    ]),
    ("E0103", "Bluecoins", [
        (0, 108, "notr"), (108, 300, "gezinme"), (300, 430, "gezinme"),
        (430, 1068, "para"), (1068, 1110, "bos"), (1110, 1758, "tanitim"),
        (1758, 1800, "bos"), (1800, 2340, "para"), (2340, 2400, "notr"),
    ]),
    ("E0276", "Wallet", [
        (0, 108, "notr"), (108, 330, "gezinme"), (330, 426, "gezinme"),
        (426, 840, "para"), (840, 1110, "gezinme"), (1110, 2340, "tanitim"),
        (2340, 2400, "notr"),
    ]),
    ("E0137", "Hesap Defterim", [
        (0, 108, "notr"), (108, 300, "gezinme"), (300, 390, "gezinme"),
        (390, 460, "gezinme"), (460, 1548, "para"), (1548, 1912, "bos"),
        (1912, 2028, "kayit"), (2028, 2180, "para"), (2180, 2340, "bos"),
        (2340, 2400, "notr"),
    ]),
    ("E0115", "Goodbudget", [
        (0, 108, "notr"), (108, 300, "gezinme"), (300, 400, "gezinme"),
        (400, 480, "para"), (480, 1212, "para"), (1212, 2340, "bos"),
        (2340, 2400, "notr"),
    ]),
]

S2_GIRIS = (
    "Beş ürünün açılışta görünen karesi aynı yükseklikte yan yana konduğunda ekranın nasıl bölündüğü doğrudan "
    "okunabiliyor. Her karenin solundaki şerit, o karenin dikey bölgelerini gösterir."
)

S2_BULGULAR = [
    "Wallet'ın karesinde hesap kartları dikey alanın yaklaşık altıda birini tutuyor; ürün, üyelik ve banka "
    "bağlantısı tanıtımları yarısından fazlasını kaplıyor.",
    "Bluecoins'te iki veri kartının arasına yaklaşık dörtte bir yükseklikte bir reklam giriyor.",
    "Goodbudget'ın karesinde zarflar bittikten sonra kalan alan boş; bu karede aşağıda başka bir blok yok.",
    "Hesap Defterim kayıt başlatmayı yüzen bir düğme olarak değil, ekranın altında sabit bir bant olarak taşıyor.",
    "Money Manager ve Goodbudget'ın bu karelerinde tanıtım alanı yok. Hesap Defterim'in dolu karesinde de yok; "
    "boş defter karesinde alt banner reklamı görünüyor (Şekil 8.3).",
]

S2_SINIR = (
    "Ölçü sınırı: oranlar yalnız açılışta görünen kareden, dikey piksel bölgelerinden okundu. Kaydırıldığında "
    "görünen bölümler ölçüye girmiyor. Bluecoins ve Wallet karelerinde içerik alt kenarda kesiliyor; bu iki ürünün "
    "gerçek sayfa uzunluğu bu kareden bilinmiyor. Oran bir tasarım değerlendirmesi değil, karenin geometrisidir."
)

# --------------------------------------------------------------------------
# SORU SAYFALARI
# --------------------------------------------------------------------------
# sekil: (E kimligi, basligi, isaretler[(no, x, y, metin)], vurgular[(x0,y0,x1,y1)])

SORULAR = [
    {
        "no": "3",
        "baslik": "İlk açılışta ne isteniyor?",
        "giris": "Uygulama ilk kez açıldığında ekran kullanıcıdan bir şey isteyebilir, ne yapılacağını "
                 "anlatabilir veya doğrudan çalışmaya başlayabilir. İncelenen beş üründen üçünün ilk karesi "
                 "var; ikisinde ilk açılış görülmedi.",
        "sekiller": [
            {
                "e": "E0017", "etiket": "Bluecoins · karşılama ekranı",
                "isaretler": [
                    (1, 880, 1302, "Mağaza rozeti uygulamanın kendi ekranında tekrar ediyor."),
                    (2, 540, 1416, "Karşılama cümlesi; ne yapılacağını söylemiyor."),
                    (3, 930, 1758, "Tek ilerleme yolu: Hadi Başlayalım."),
                    (4, 880, 2184, "Dil ekranın altında seçilebiliyor."),
                ],
            },
            {
                "e": "E0135", "etiket": "Hesap Defterim · ilk açılış",
                "isaretler": [
                    (5, 700, 1046, "Açıklama diyaloğu uygulamanın ne işe yaradığını anlatıyor."),
                    (6, 44, 1247, "Metin Ücretli ve Alınan düğmelerinden söz ediyor."),
                    (7, 60, 1970, "Ekrandaki düğmelerin adı Ödendi ve Alındı."),
                    (8, 600, 1483, "Diyalogdan tek çıkış: TAMAM MI."),
                ],
            },
            {
                "e": "E0106", "etiket": "Goodbudget · ilk açılış",
                "isaretler": [
                    (9, 930, 876, "Hesabı olan giriş yapıyor."),
                    (10, 930, 1164, "Hesabı olmayan için tek yol yeni hane açmak."),
                    (11, 130, 1366, "Şifreleme notu ekranda."),
                    (12, 850, 1501, "Sürüm numarası ekranda: 2.24.26013 (180)."),
                ],
            },
        ],
        "notlar": [
            "Money Manager'ın ilk açılış karesi yok. Koşum kaydına göre kayıt, giriş veya karşılama ekranı "
            "gelmedi; uygulama doğrudan ana ekranla açıldı (E0010 K00, kullanıcı doğruladı).",
            "Wallet'ın ilk açılışı görülmedi; koşumlar önceden açılmış bir hesapla yapıldı (E0014 K00).",
            "Goodbudget üçü içinde hesap isteyen tek ürün. İzlenen yolda yeni haneden sonra bütçe kurulumu "
            "geliyor; atlanabilir olup olmadığı denenmedi.",
            "Hesap Defterim'in diyaloğu ekrandaki düğme adlarını farklı yazıyor — metin Ücretli/Alınan, "
            "düğmeler Ödendi/Alındı. İkisi aynı karede görünüyor (işaret 6 ve 7).",
            "Bu üç kare kurulumun tamamını göstermiyor; yalnız ilk görülen ekranı gösteriyor.",
        ],
        "dayanak": "Canlı kare: E0017, E0135, E0106. Koşum kaydı: E0010 K00, E0014 K00. Görülmedi: Money "
                   "Manager ve Wallet'ın ilk açılış karesi. Çıkarılmayan sonuç: adımların zorunlu veya "
                   "atlanabilir olduğu; ilk açılışın kullanıcıyı kayda yöneltmedeki etkisi.",
    },
    {
        "no": "4",
        "baslik": "Ana ekran önce neyi söylüyor?",
        "giris": "İki ürün de açılışta bir toplam gösteriyor. Toplamın neyin toplamı olduğu farklı: birinde seçili "
                 "ayın gelir ve gideri, diğerinde bütçe kovalarına dağıtılmış tutar.",
        "sekiller": [
            {
                "e": "E0228", "etiket": "Money Manager · İşlemler › Gün",
                "isaretler": [
                    (1, 330, 190, "Dönem seçici. Ekrandaki her sayı bu aya ait."),
                    (2, 55, 408, "Gelir, Gider ve Toplam; üçü de seçili ayın."),
                    (3, 700, 524, "Gün başlığında o günün geliri ve gideri ayrıca yazıyor."),
                    (4, 700, 667, "Satırda kategori, açıklama ve hesap adı birlikte."),
                ],
            },
            {
                "e": "E0115", "etiket": "Goodbudget · ENVELOPES",
                "isaretler": [
                    (5, 520, 206, "Hesap adı. Bu kopyada karartıldı."),
                    (6, 690, 454, "Total: zarflara dağıtılmış tutar. Hesap bakiyesi değil."),
                    (7, 700, 630, "Zarf satırında iki sayı var; hangisinin ne olduğu ekranda yazmıyor."),
                    (8, 540, 1500, "Zarflar bitince kare boş devam ediyor."),
                ],
            },
        ],
        "notlar": [
            "Money Manager'ın karesinde hesap bakiyesi yok; bakiyeler ayrı bir yüzeyde (E0236).",
            "Goodbudget'ın karesinde de hesap bakiyesi yok; ACCOUNTS ayrı sekmede.",
            "Zarf satırındaki iki sayının anlamı koşum kaydına dayanır; kare tek başına ayırmıyor.",
        ],
        "diger": [
            ("Bluecoins", "Hesaplar sekmesi hesap listesi değil, düzenlenebilir özet kartları gösteriyor (Şekil 2.2)."),
            ("Wallet", "İlk blok doğrudan hesap kartları ve bakiyeleri (Şekil 2.3)."),
            ("Hesap Defterim", "Seçili defterin hareketleri; her satırda o anki denge, altta sabit toplamlar (Şekil 2.4)."),
        ],
        "dayanak": "Canlı kare: E0228, E0115. Koşum kaydı: E0006 K01 (zarf sayılarının anlamı). "
                   "Çıkarılmayan sonuç: iki toplamın finansal olarak denk olduğu.",
    },
    {
        "no": "5", "duzen": "serit",
        "baslik": "Bölümler nerede duruyor?",
        "giris": "Beş üründe bölüm seçici beş ayrı yerde: ekranın altında sabit, üstünde sabit, üstte yana kaydırılan, "
                 "yandan açılan çekmecede veya çekmece ile defter seçicinin birleşiminde. Çekmeceyi kullanan iki "
                 "üründe çekmecenin içi tek düzeyde listeleniyor.",
        "serit": [
            ("E0228", "Money Manager", (0, 2200, 1080, 2345), "Altta dört sabit sekme"),
            ("E0115", "Goodbudget", (0, 290, 1080, 410), "Üstte dört sabit sekme"),
            ("E0103", "Bluecoins", (0, 300, 1080, 440), "Üstte kaydırılan sekmeler; son sekme kesiliyor"),
            ("E0276", "Wallet", (0, 120, 1080, 430), "Çekmece düğmesi ve ekran içi iki sekme"),
            ("E0137", "Hesap Defterim", (0, 140, 1080, 400), "Çekmece düğmesi ve defter seçici"),
        ],
        "sekiller": [
            {
                "e": "E0376", "etiket": "Wallet · çekmecenin alt kısmı",
                "isaretler": [
                    (1, 52, 116, "Debts: borç defteri."),
                    (2, 52, 410, "Araya başka bir ürünün tanıtımı giriyor."),
                    (3, 52, 558, "Alışveriş listesi, garanti ve sadakat kartı aynı listede."),
                    (4, 860, 1631, "Görünüm anahtarları da aynı listede."),
                ],
            },
            {
                "e": "E0171", "etiket": "Hesap Defterim · çekmece",
                "isaretler": [
                    (5, 52, 368, "Listenin ilk kalemi reklam kaldırma."),
                    (6, 52, 998, "Aktar: hesaplar arası para hareketi burada."),
                    (7, 52, 1628, "Nakit hesap makinesi ve not defteri aynı listede."),
                    (8, 52, 2006, "Silinmiş işlemler de aynı düzeyde."),
                ],
            },
        ],
        "notlar": [
            "İki çekmecede de finansal bölümler ile yardımcı araçlar arasında görsel bir ayrım yok: başlık, ayraç "
            "veya gruplama görünmüyor.",
            "Bir bölümün var olup olmadığı çekmece açılmadan ekrandan anlaşılmıyor.",
            "Bluecoins çekmecesinde farklı simgeli iki 'Hesaplar' kalemi var; hedefleri açılmadı (E0084).",
        ],
        "dayanak": "Canlı kare: E0228, E0115, E0103, E0276, E0137, E0376, E0171. "
                   "Çıkarılmayan sonuç: menü uzunluğundan bulunabilirlik, öğrenme süresi veya işlem hızı.",
    },
    {
        "no": "6", "duzen": "kayit",
        "baslik": "Kayıt nereden başlıyor?",
        "giris": "Dört üründe kayıt tek bir yuvarlak düğmeyle başlıyor ve paranın yönü sonra açılan formda seçiliyor. "
                 "Hesap Defterim yönü düğmenin adına taşımış: ekranda iki düğme var ve hangisine basıldığı yönü "
                 "belirliyor.",
        "sekiller": [
            {
                "e": "E0137", "etiket": "Hesap Defterim · ana ekran",
                "isaretler": [
                    (1, 62, 1970, "Alındı: para girişi."),
                    (2, 600, 1970, "Ödendi: para çıkışı."),
                    (3, 700, 720, "Her satırda o kayıttan sonraki denge yazıyor."),
                    (4, 44, 2106, "Ekranın altında dönem toplamları sabit duruyor."),
                ],
            },
        ],
        "serit2": [
            ("E0228", "Money Manager", (884, 2008, 1044, 2168)),
            ("E0103", "Bluecoins", (884, 2150, 1044, 2300)),
            ("E0276", "Wallet", (884, 2158, 1044, 2308)),
            ("E0115", "Goodbudget", (884, 2140, 1044, 2300)),
        ],
        "notlar": [
            "Hesap Defterim'de iki yön ekranda; üçüncü yön olan hesaplar arası aktarım ekranda değil, çekmecedeki "
            "Aktar kaleminde (Şekil 5.7).",
            "Dört + düğmeli üründe formda tür seçenekleri görülüyor (E0229, E0074, E0277, E0116). Formun hangi türle "
            "açıldığı ve her kayıtta seçimin değiştirilmesi gerekip gerekmediği bu bölümde doğrulanmadı.",
            "Form alanlarının sırası ve kaydetme sonrası geri bildirim Bölüm 5'in konusu.",
        ],
        "dayanak": "Canlı kare: E0137, E0171, E0228, E0103, E0276, E0115. "
                   "Çıkarılmayan sonuç: adım sayısı, kayıt süresi, tür seçiminin zorunlu olduğu.",
    },
    {
        "no": "7",
        "baslik": "Bekleyen işler nerede görünüyor?",
        "giris": "Bekleyen iş iki üründe iki ayrı yerde duruyor: ana ekranın aşağısında bir kart ya da kendi "
                 "sekmesinde tarih gruplu bir liste. İkisi de gelecekteki veya vadesi geçmiş kayıtların görünür bir "
                 "yerini gösteriyor; gerçekleşme kuralları bu bölümün dışında.",
        "sekiller": [
            {
                "e": "E0274", "etiket": "Wallet · ana ekranın aşağısı",
                "isaretler": [
                    (1, 640, 228, "Bakiye eğilimi kartı."),
                    (2, 820, 1072, "Upcoming planned payments: bekleyen ödemeler kartı."),
                    (3, 540, 1380, "Kartın satırları bu karede yükleniyor; içerik görünmüyor."),
                    (4, 880, 1880, "Ana ekrana kart ekleme davetiyesi."),
                ],
            },
            {
                "e": "E0056", "etiket": "Bluecoins · Hatırlatıcılar sekmesi",
                "isaretler": [
                    (5, 180, 574, "Durum sözcükle yazılıyor: Dün bitti."),
                    (6, 180, 830, "Bugün süresi doluyor; renk tek başına taşımıyor."),
                    (7, 700, 757, "Tarih grubunun başlığında o günün toplamı."),
                    (8, 330, 1188, "Taksitli kayıtta kaçıncı taksit olduğu satırda."),
                ],
            },
        ],
        "notlar": [
            "\"Canlı incelenen ürünlerin hiçbirinde ana ekranda bekleyen iş yok\" cümlesi yazılamaz; Şekil 7.1 bunu "
            "çürütür. Bu, kapalı bir karşılaştırmanın açık kanıtla sınanmasıdır.",
            "Wallet kartının yüklenmiş hâli görülmedi: satır içeriği, sayısı ve tutarları bu kareden bilinmiyor.",
            "Money Manager, Hesap Defterim ve Goodbudget'ın incelenen ana ekran karelerinde bekleyen iş alanı "
            "görünmüyor. Bu, ürün genelinde böyle bir yüzey olmadığı anlamına gelmez.",
        ],
        "dayanak": "Canlı kare: E0274, E0056. Görülmedi: Wallet kartının yüklenmiş içeriği. "
                   "Çıkarılmayan sonuç: bekleyen kaydın gerçekleşmesi, bakiyeye etkisi, hatırlatma davranışı.",
    },
    {
        "no": "8",
        "baslik": "Veri yokken ekranda ne kalıyor?",
        "giris": "Boş ekranda iki ayrı şey korunabilir: ekranın yapısı ve ne yapılacağını söyleyen sözcükler. Üç kare "
                 "üçünü de aynı anda vermiyor ve üçü aynı boşluk durumuna ait değil.",
        "sekiller": [
            {
                "e": "E0227", "etiket": "Money Manager · kayıtsız ay",
                "isaretler": [
                    (1, 330, 190, "Dönem bağlamı duruyor: Eylül 2026."),
                    (2, 55, 408, "Üç özet sayısı sıfır olarak kalıyor."),
                    (3, 760, 988, "Tek metin: Veri yok."),
                    (4, 990, 2242, "Alttaki mesaj çıkışla ilgili, ilk kayıt rehberi değil."),
                ],
            },
            {
                "e": "E0026", "etiket": "Bluecoins · temiz kurulum",
                "isaretler": [
                    (5, 850, 1046, "Karşılama sözcüğü."),
                    (6, 850, 1236, "Sıfır bakiye, etiketiyle birlikte."),
                    (7, 850, 2003, "Para birimi ekranda yazıyor."),
                    (8, 900, 2234, "İlk eylem adıyla çağrılıyor: İlk İşlemi Ekle."),
                ],
            },
            {
                "e": "E0136", "etiket": "Hesap Defterim · boş defter",
                "isaretler": [
                    (9, 940, 380, "Yedekleme daveti."),
                    (10, 730, 626, "Reklam kaldırma daveti."),
                    (11, 540, 904, "Sütun başlıkları duruyor."),
                    (12, 540, 1970, "İki yön düğmesi duruyor."),
                    (13, 540, 2256, "Altta banner reklam."),
                ],
            },
        ],
        "notlar": [
            "Hesap Defterim yapıyı koruyor, sözcük vermiyor: başlıklar, düğmeler ve sıfır toplamlar yerinde, ilk "
            "kayıt için yönlendirme metni yok.",
            "Bluecoins tersini yapıyor: liste yapısı yok, ama ilk eylem ve para birimi sözcükle söyleniyor.",
            "Money Manager ikisini de vermiyor; koruduğu tek şey dönem bağlamı ve sıfırlanmış özet satırı.",
            "Üç kare üç farklı durum: kayıtsız bir ay, temiz kurulum ve boş defter. Aynı soruya cevap veriyorlar ama "
            "aynı başlangıç noktasından değil.",
        ],
        "dayanak": "Canlı kare: E0227, E0026, E0136. Görülmedi: Wallet ve Goodbudget'ın boş ana ekranı (yeni hesap "
                   "gerektirir, mevcut test verisi silinmez). Çıkarılmayan sonuç: boş ekranın kullanıcıyı ilk kayda "
                   "yöneltmedeki etkisi.",
    },
]

# --------------------------------------------------------------------------
# SAYFA 8 - kaynaktan gorulen panolar
# --------------------------------------------------------------------------
KAYNAK_SAYFALARI = [
    {
        "no": "9",
        "baslik": "Kaynaktan görülen pano: KolayBi",
        "giris": "İçine girilemeyen ürünlerde ana ekran yalnız ürünün kendi yayımladığı görsellerden görülüyor. "
                 "KolayBi'nin destek sayfasındaki bu görselde modül adları, sekmeler ve sayaçlar okunabiliyor; "
                 "yine de canlı bir koşum değil.",
        "sekil": {
            "e": "E0211", "etiket": "KolayBi · destek sayfası panosu (kaynak görseli)",
            "isaretler": [
                (1, 84, 308, "Solda sabit modül paneli: satış, satın alma, gider, cari, finans, projeler, raporlar."),
                (2, 1009, 63, "Sağ üstte Hızlı İşlemler menüsü; kapalı olduğu için seçenekleri görünmüyor."),
                (3, 504, 110, "Panonun içinde iş ortağı tanıtım bandı."),
                (4, 464, 279, "Gelir ve gider için iki çizgili nakit akışı."),
                (5, 1174, 473, "Günü Gelen İşlemler: Bugün, Yaklaşanlar, Tarihi Geçenler."),
                (6, 804, 400, "Baloncuktaki demo verisi 5.1.2023 tarihli."),
            ],
        },
        "notlar": [
            "Görsel bir tarayıcı penceresi olarak çerçevelenmiş ve gölgelendirilmiş; ham ekran görüntüsü değil, "
            "destek sayfası için hazırlanmış bir yayın görselidir.",
            "İçindeki veri demo verisidir ve grafiğin baloncuğu Ocak 2023'ü gösteriyor. Bu kare, bugünkü sürümün "
            "ekranı olduğunu kanıtlamaz.",
            "Panonun bir kısmı alt kenarda kesiliyor: Tahsilat Ve Ödeme Özetleri başlığı görünüyor, içeriği bu "
            "karede yok.",
            "İkinci bir video karesi (E0181) aynı panoyu farklı bir düzenle gösteriyor; iki kaynak tek bir güncel "
            "ekran gibi birleştirilmedi.",
            "Pano masaüstü genişliğinde. Telefon ekranlarıyla aynı alan koşullarında karşılaştırılamaz; bölüm 2'nin "
            "oranları bu görsele uygulanmadı.",
        ],
        "dayanak": "Kaynak görseli: E0211 (destek sayfası), E0181 (tanıtım videosu). Görülmedi: mobil iç yüzey, "
                   "canlı davranış, güncel sürüm. Çıkarılmayan sonuç: bu düzenin bugün çalışan ekran olduğu.",
    },
    {
        "no": "10",
        "baslik": "Temsili çizim: Paraşüt",
        "giris": "Paraşüt'ün ana ekranı yalnız bir tanıtım videosu karesinden görülüyor. Bu kare bir ekran görüntüsü "
                 "değil, çizimdir. Aşağıdaki dört işaret bunu ayrı ayrı gösteriyor; bu yüzden kareden hiçbir "
                 "davranış veya tutar sonucu çıkarılmadı.",
        "sekil": {
            "e": "E0262", "etiket": "Paraşüt · tanıtım videosu karesi (temsili çizim)",
            "isaretler": [
                (1, 72, 460, "Sol menü kutuları boş çizilmiş; hiçbirinde etiket yok."),
                (2, 308, 332, "Tahsil edilecek halkası: 138.89,20 ₺."),
                (3, 524, 332, "Gecikmiş halkası birebir aynı tutarı gösteriyor."),
                (4, 896, 742, "Uyarı kutusundaki 19.989,00 ₺ bu iki halkanın hiçbiriyle uyuşmuyor."),
                (5, 1047, 300, "Sağ sütun da etiketsiz boş dikdörtgenlerden oluşuyor."),
            ],
        },
        "notlar": [
            "Dört işaret birlikte çizim olduğunu gösteriyor: etiketsiz menü kutuları, iki halkada yinelenen aynı "
            "tutar, uyarı kutusuyla uyuşmayan üçüncü tutar ve geçerli bir Türkçe biçime uymayan sayı yazımı "
            "(138.89,20).",
            "Yine de kareden okunabilen bir şey var: ürünün kendi anlatımında ana ekran tahsilat ve ödemeyi vade "
            "durumuna göre ayırıyor — tahsil edilecek, gecikmiş, fatura yok; ödenecek, ödeme yok, planlanmış.",
            "Bu bir ürün iddiasıdır, gözlem değildir. Paraşüt'ün gerçek ana ekranı hiçbir kaynakta görülmedi.",
            "Mobil uygulamada yalnız giriş öncesi tanıtım ekranları görüldü; giriş ekranı geçilemedi.",
        ],
        "dayanak": "Temsili çizim: E0262. Koşum kaydı: E0011 (giriş ekranı geçilemedi). Görülmedi: çalışan ana "
                   "ekran, mobil iç yüzey, her türlü davranış. Çıkarılmayan sonuç: karedeki tutarların bir senaryo "
                   "sonucu olduğu.",
    },
]

# --------------------------------------------------------------------------
# SAYFA 9 - kanit eki
# --------------------------------------------------------------------------
CIKARILMAYAN = [
    ("Bölüm geneli", "Ürünlerin aynı derinlikte incelendiği"),
    ("Bölüm 2 oranları", "Oranların bir tasarım kalitesi ölçüsü olduğu; kaydırılan sayfanın tamamını anlattığı"),
    ("Bölüm 3", "İlk açılış adımlarının zorunlu veya atlanabilir olduğu"),
    ("Bölüm 5", "Bu yerleşimlerden bulunabilirlik, öğrenme süresi veya işlem hızı"),
    ("Bölüm 4", "Money Manager ile Goodbudget toplamlarının finansal olarak denk olduğu"),
    ("Bölüm 6", "Menü uzunluğundan bulunabilirlik, öğrenme süresi veya işlem hızı"),
    ("Bölüm 7", "Kayıt adımlarının sayısı; tür seçiminin her kayıtta zorunlu olduğu"),
    ("Bölüm 8", "Bekleyen kaydın gerçekleşmesi ve bakiyeye etkisi; ürün genelinde yokluk"),
    ("Bölüm 9", "Boş ekranın kullanıcıyı ilk kayda yöneltmedeki etkisi"),
    ("Bölüm 10", "KolayBi panosunun bugün çalışan ekran olduğu; masaüstü ile mobil yoğunluğun kıyaslanabilirliği"),
    ("Bölüm 11", "Paraşüt çizimindeki tutarların bir senaryo sonucu olduğu"),
]

EKSIKLER = [
    ("Money Manager", "İlk açılışın karesi yok; karşılama olmadığı kullanıcı beyanı", "3, 9",
     "Temiz kurulum gerekir; mevcut veriyi etkiler", "Önerilmez"),
    ("Wallet", "İlk açılış ve boş ana ekran görülmedi", "3, 9",
     "Oturum kapatma veya yeni hesap gerekir; bulut hesabı etkilenir", "Önerilmez"),
    ("Goodbudget", "Boş ana ekran ve kurulumu atlama yolu görülmedi", "3, 9",
     "Yeni hane gerekir", "Önerilmez"),
    ("Wallet", "Bekleyen ödeme kartının yüklenmiş hâli görülmedi", "8",
     "Ana ekranı açıp kart yüklenene kadar bekleyip görüntü alın", "Yüksek"),
    ("Bluecoins", "Dolu hesapta açılışta hangi sekmenin seçili geldiği bilinmiyor", "2",
     "Uygulamayı son uygulamalardan tamamen kapatıp açın; ilk ekranın görüntüsü", "Orta"),
    ("Hesap Defterim", "Birden fazla defter varken açılışta seçili defter bilinmiyor", "2, 4",
     "Uygulamayı tamamen kapatıp açın; ilk ekranın görüntüsü", "Orta"),
    ("KolayBi / Paraşüt", "Güncel sürüm ve canlı pano davranışı", "10, 11",
     "Ücretli hesap gerekir", "Önerilmez"),
]

ENVANTER_NOTU = (
    "İki karede dosya adı ile içerik uyuşmuyor; ikisi de bu turda açılarak doğrulandı ve adlar "
    "değiştirilmedi, çünkü E kimlikleri sabittir. E0231'in adı 07-rapor-agustos.png, ekranın alt çubuğunda "
    "seçili sekme İstatistik. E0274'ün özgün dosya adı 00-magaza.png; içeriği mağaza görseli değil, canlı emülatör karesidir (durum çubuğu, "
    "iskelet yükleme blokları ve uygulamanın kendi kayıt düğmesi görünür). Ad ile içerik uyuşmazlığı burada kayda "
    "geçirildi; dosya adı değiştirilmedi, çünkü E kimlikleri sabittir."
)

VERI_KURALI = (
    "Mevcut test verisi silinmez veya sıfırlanmaz. Yeni kurulum, oturum kapatma veya yeni hesap gerektiren eksikler "
    "bu nedenle Önerilmez olarak işaretlidir."
)
