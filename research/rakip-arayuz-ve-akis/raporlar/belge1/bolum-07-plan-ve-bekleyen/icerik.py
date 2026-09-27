# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 7 · Plan, tekrar, taksit ve bekleyen — tek icerik kaynagi (plan §4)."""

NO = 7
BASLIK = "Plan, tekrar, taksit ve bekleyen"
ANA_SORU = "Gelecekteki bir ödeme nasıl kuruluyor ve bekleyen işler nerede görünüyor?"
GIRIS = [
    "Bu bölüm gelecekteki bir ödemenin arayüzde nasıl kurulduğunu ve bekleyen işlerin nerede göründüğünü izler: "
    "tekrar formu, taksit kurulumu, bekleyen liste, ana ekrandaki bekleyen alanı, onay adımı, bütçe ve hedef.",
    "Kurulum ile gerçekleşme ayrı okunur. Planın gerçekleşince bakiyeye ve rapora etkisi bu bölümde anlatılmaz; "
    "ekranda görünen alan, etiket ve düğme anlatılır. Otomatik kolların çoğu denenmedi; denenmeyen kol için "
    "sonuç yazılmaz.",
]
GIRMEZ = [
    "Kaydın gerçekleşince bakiyeye etkisi → Belge 2",
    "Borç ve fatura zinciri → 8",
    "Bütçe raporunun okunması → 9",
    "Tahmin formülü hakkında hüküm (bilinmiyor)",
]
KAPSAM = [
    ("Money Manager", ["canli"], None),
    ("Bluecoins", ["canli"], "Otomatik giriş kolu denenmedi."),
    ("Wallet", ["canli"], "Onay sorusunda No seçildi."),
    ("Hesap Defterim", ["yok"], "Plan ve taksit yüzeyi görülmedi."),
    ("Goodbudget", ["canli", "kosum"], "Kart hesabı açılamadı; sonraki örneğin üretimi bilinmiyor."),
    ("KolayBi", ["kaynak"], "Tekrarlı maaş formu; nakit akış raporu."),
    ("Paraşüt", ["beyan"], "Yalnız ana ekran anlatımı."),
    ("Logo İşbaşı", ["yok"], "Karşılaştırılabilir kaynak anlatımı bulunmadı."),
    ("QuickBooks Solopreneur", ["yok"], "Karşılaştırılabilir kaynak anlatımı bulunmadı."),
]
KAYNAK_NOTLARI = [
    "Goodbudget E0115'in üst çubuğundaki hane adı belge genelinde karartıldı; özgün kanıt değişmedi.",
    "E0399 ve E0400 kullanıcı kontrolü kareleridir (15 Eylül); basılmadı, metinde kimlikle anıldı.",
]
KB = (96, 112, 1305, 865)      # KolayBi destek gorsellerinin ic penceresi

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 7.1
    {
        "tur": "soru", "no": "7.1", "baslik": "Tekrar nasıl kuruluyor?",
        "giris": "Üç üründe tekrar normal kayıt formunun içinden kuruluyor: bir rozet, bir alt sayfa ya da bir işaret "
                 "kutusu. Wallet'ta planlı ödemenin kendi formu var; sıklık formun sonunda.",
        "sekiller": [
            {"k": "mm-tekrar", "e": "E0241", "etiket": "Money Manager · Gider formu",
             "isaretler": [
                 (1, 985, 580, "Tarih satırının sağında Aylık rozeti."),
                 (2, 540, 1500, "Tekrarlı kayıtta yalnız Kaydet; Devam et yok."),
             ]},
            {"k": "bc-tekrar", "e": "E0039", "etiket": "Bluecoins · planlı işlem alt sayfası",
             "isaretler": [
                 (3, 800, 555, "Sıklık çipleri: Bir Defa, Günlük, Haftalık, Aylık, Yıllık."),
                 (4, 820, 1760, "Bitiş: Asla, bir etkinlik sonra, Son Tarih."),
                 (5, 1000, 1940, "Vade tarihinde otomatik kayıt: işaret kutusu."),
             ]},
            {"k": "wl-tekrar", "e": "E0288", "etiket": "Wallet · Add Planned payment",
             "isaretler": [
                 (6, 750, 620, "Plana ad veriliyor (Name)."),
                 (7, 750, 2090, "Frequency: Recurrent payment."),
             ]},
            {"k": "gb-tekrar", "e": "E0133", "etiket": "Goodbudget · Add Transaction",
             "isaretler": [
                 (8, 440, 1430, "Schedule this… kutusu işaretli."),
                 (9, 960, 1700, "Sıklık listesi: Once'tan Every 6 Months'a; sonuncusu kesik."),
             ]},
        ],
        "notlar": [
            ("Money Manager'da sıklık ayrı bir listede seçiliyor: Hiçbiri'den Yıllık'a 14 seçenek.", "E0238"),
            ("Wallet'ın planlı ödeme tarih seçicisinde bugünden önceki günler soluk görünüyor.", "E0287"),
            ("Goodbudget formunda sıklığın altında e-posta hatırlatma satırı var (… 3 days before); gönderim "
             "denenmedi.", "E0133"),
            ("Goodbudget'ta kurulan planın sıklığı Every 2 Weeks.", "E0126"),
            ("Money Manager tekrarlı kaydı kaydederken tek bir onay soruyor: \"Tarihte tekrar eden işlemler "
             "uygulanır. Tekrarlı olarak kaydetmek istiyor musunuz?\" (→ 7.5).", "E0430"),
        ],
        "urunler": [
            ("Hesap Defterim", "yok", "Çekmece, ayarlar ve form tarandı; finansal plan veya tekrar yüzeyi "
             "görülmedi. Not Defteri ayrı bir not yüzeyi.", "E0171, E0178, E0007"),
            ("KolayBi", "kaynak", "Mevcut bir kayıttan tekrara dönüştürme ([[kb-donustur]]).", "E0204, E0205"),
        ],
    },
    {
        "tur": "soru", "no": "7.1", "baslik": "Kaynakta görülen tekrar formu", "haritada": False,
        "giris": "KolayBi'de tekrar ayrı bir formdan değil, mevcut bir kaydın İşlemler menüsünden başlıyor. "
                 "Destek sayfasındaki örnek bir çalışan maaşı.",
        "dikey": True,
        "sekiller": [
            {"k": "kb-donustur", "e": "E0204", "etiket": "KolayBi · Çalışan Maaşı, İşlemler menüsü açık",
             "kirpma": (96, 112, 1305, 700), "genislik": 350,
             "isaretler": [
                 (1, 1014, 198, "İşlemler menüsünde Tekrarlı Maaşa Dönüştür."),
                 (2, 784, 63, "Cari sekmelerinin sonunda Tekrarlı Maaşlar."),
             ]},
            {"k": "kb-tekrar-form", "e": "E0205", "etiket": "KolayBi · Tekrarlı Maaş Oluştur",
             "kirpma": (96, 112, 1305, 800), "genislik": 350,
             "isaretler": [
                 (3, 319, 283, "Oluşturma Periyodu, yıldızlı; değerleri görünmüyor."),
                 (4, 319, 348, "Başlangıç Tarihi: kaynak maaşın tarihi."),
                 (5, 319, 406, "Maaş Ödeme Tarihi: Belirsiz / Belirli."),
                 (6, 319, 466, "Maaş Oluşturma Tekrar Sayısı, yıldızlı."),
             ]},
        ],
        "notlar": [
            ("Periyot ve tekrar sayısının zorunluluğu yalnız maaş formunda görüldü; diğer tekrarlı formlara "
             "genellenmez. Aynı alan dizisi (periyot, başlangıç tarihi, tekrar sayısı, bitiş Belirsiz / Belirli) "
             "Tekrarlı Proformaya ve Tekrarlı Genel Gidere Dönüştür formlarında da var.", "E0205; E0008 (WB-02)"),
            "Formda onay adımı veya otomatik üretim tercihi görünmüyor; kayıtların nasıl üretildiği kaynakta "
            "anlatılmıyor.",
            "Görseller destek materyalidir; demo verisi 2023 tarihli ve güncel sürüm doğrulanmadı.",
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Genel gider ayrıntısında da aynı menüde Tekrarlı Genel Gidere Dönüştür var.",
             "E0199"),
        ],
    },

    # ---------------------------------------------------------------- 7.2
    {
        "tur": "soru", "no": "7.2", "baslik": "Taksit nasıl kuruluyor?",
        "giris": "Money Manager taksiti formdaki bir rozetle, Bluecoins kart harcamasındaki bir alt sayfayla "
                 "kuruyor. İkisinde de 6.000 altı aya bölündü.",
        "sekiller": [
            {"k": "mm-taksit-form", "e": "E0245", "etiket": "Money Manager · Gider formu, taksitli",
             "isaretler": [
                 (1, 985, 580, "Tarih satırında 6 Ay rozeti; Tutar'a toplam yazılıyor."),
             ]},
            {"k": "mm-taksit-liste", "e": "E0246", "etiket": "Money Manager · Ağustos günlük listesi",
             "isaretler": [
                 (2, 760, 880, "Başlıkta (1/6); satırda tek taksidin tutarı."),
             ]},
            {"k": "bc-taksit", "e": "E0034", "etiket": "Bluecoins · Taksit şartlarını seçin",
             "isaretler": [
                 (3, 600, 1560, "Taksit oranı alanı; bu kayıtta 0,00."),
                 (4, 750, 1985, "Taksit sayısı ve İlk ödeme tarihi."),
             ]},
            {"k": "bc-taksit-ozet", "e": "E0035", "etiket": "Bluecoins · formdaki özet",
             "isaretler": [
                 (5, 300, 1330, "Kaydetmeden önce tek cümlelik özet; Değiştir ve Sıfırla."),
             ]},
        ],
        "notlar": [
            ("Money Manager'da sonraki taksit kart defterinde (2/6) başlığıyla görünüyor.", "E0256"),
            ("Bluecoins'te kalan beş taksit Hatırlatıcılar listesinde sayaçla duruyor (→ 7.3). Taksitler "
             "kendiliğinden işlemlere girmiyor; her biri hatırlatıcıdan Kaydet ile kaydediliyor, ileri tarihli "
             "taksitte ödeme tarihi soruluyor: bugün ya da taksitin tarihi (kullanıcı kontrolü, karesi yok).",
             "E0037; BC-X2"),
            ("Money Manager'ın ay sayısını soran alanı bu karelerde görünmüyor.", "E0245"),
            "Taksit oranının neyi ifade ettiği ekranda yazmıyor.",
            "İlk taksidin hangi aya yazıldığı → Belge 2 3.4.",
        ],
        "urunler": [
            ("Bluecoins", "canli", "Taksit alanı hesap olarak kredi kartı seçildiğinde formda çıkıyor (→ 4.2).",
             "E0028"),
            ("Wallet", "yok", "İncelenen form ve kayıt ayrıntısında taksit alanı yok; 6.000 tek kart harcaması "
             "olarak yazıldı.", "E0294, E0296"),
            ("Goodbudget", "kosum", "Split into multiple Envelopes tutarı zarflara böler, zamana yaymaz. Kart "
             "hesabı ücretsiz pakette açılamadı.", "E0006"),
            ("Hesap Defterim", "yok", "Taksit yüzeyi görülmedi.", "E0007"),
        ],
    },

    # ---------------------------------------------------------------- 7.3
    {
        "tur": "soru", "no": "7.3", "baslik": "Bekleyen işler hangi listede?",
        "giris": "Bluecoins bekleyen bütün kalemleri tek bir tarih listesinde topluyor; Wallet her planı tek "
                 "satırda sıradaki vadesiyle gösteriyor; Money Manager gelecek ayın listesinin üstüne bir satır "
                 "koyuyor.",
        "sekiller": [
            {"k": "bc-bekleyen", "e": "E0047", "etiket": "Bluecoins · Hatırlatıcılar",
             "isaretler": [
                 (1, 540, 215, "Hatırlatıcılar, ana ekranın sekmelerinden biri."),
                 (2, 200, 570, "Durum sözcükle: Bugün süresi doluyor."),
                 (3, 600, 1190, "Taksit satırında sayaç: 2 / 6."),
             ]},
            {"k": "bc-bekleyen-ertesi", "e": "E0056", "etiket": "Bluecoins · Hatırlatıcılar, ertesi gün",
             "isaretler": [
                 (4, 200, 570, "Ertesi gün aynı kayıt: Dün bitti."),
             ]},
            {"k": "wl-bekleyen", "e": "E0306", "etiket": "Wallet · Planned payments",
             "isaretler": [
                 (5, 540, 750, "Plan başına tek satır: sıklık, tutar, sıradaki vade."),
                 (6, 400, 1930, "Alt sayfada All / Income / Expense / Transfer."),
             ]},
            {"k": "mm-onizleme", "e": "E0244", "etiket": "Money Manager · Ekim günlük listesi",
             "isaretler": [
                 (7, 540, 650, "Tekrarlama satırı: 10/10, −600; ay toplamları 0."),
             ]},
        ],
        "notlar": [
            ("Bluecoins listesinde tek seferlik kira, aylık abonelik ve taksitler aynı tarih sırasında; ertesi "
             "günün kirası bir gün önce \"Yarın borçlanacak\" etiketini taşıyor.", "E0047"),
            ("Bluecoins'te süresi geçen hatırlatıcı kendiliğinden kayda dönmüyor; kaydı kullanıcı yapıyor (→ 7.2).",
             "E0056; BC-X2"),
            ("Wallet plan ayrıntısında bekleyen örnek Due today ve Confirm düğmesiyle duruyor (→ 7.5). Listenin "
             "araç çubuğunda arama ve sıralama var: vadeye ya da ada göre.", "E0289, E0498"),
            ("Wallet Records menüsünde Show planned payments seçeneği var; sonucu denenmedi.", "E0399"),
            ("Money Manager'da Ayarlar altında ayrı bir Tekrarlayan İşlemler listesi var; taksit planı bu "
             "listede yok.", "E0416"),
            "Planların hangi toplamda göründüğü → Belge 2 5.3, 5.4.",
        ],
        "urunler": [
            ("Goodbudget, Hesap Defterim", "yok", "Ayrı bir bekleyen liste görülmedi.", "E0006, E0007"),
        ],
    },

    # ---------------------------------------------------------------- 7.4
    {
        "tur": "soru", "no": "7.4", "baslik": "Ana ekranda bekleyen iş",
        "giris": "İncelenen beş canlı üründen yalnız Wallet'ın ana ekranında bekleyen ödemelere ayrılmış bir alan "
                 "görüldü: üstte bir kısayol, aşağıda bir kart. Diğer dört ürün için bu, ana ekran karesiyle "
                 "sınırlı bir gözlemdir.",
        "yukseklik": 384,
        "sekiller": [
            {"k": "wl-ana-ust", "e": "E0293", "etiket": "Wallet · Home, üst kısım",
             "isaretler": [
                 (1, 900, 1075, "Hesap kartlarının altında Cash-flow ve Planned payments kısayolları."),
                 (2, 900, 1455, "Kısayolların altında tanıtım kartları."),
             ]},
            {"k": "wl-ana-alt", "e": "E0469", "etiket": "Wallet · Home, aşağısı",
             "isaretler": [
                 (3, 850, 1320, "Upcoming planned payments kartı."),
                 (4, 760, 1545, "Satırda plan adı, kategori, tutar ve vade."),
                 (5, 200, 1885, "Add more cards: ana ekrana kart ekleme."),
             ]},
        ],
        "notlar": [
            "İki kare aynı ana ekranın üstü ve aşağısıdır; farklı anlarda çekildi.",
            ("Kart yüklenirken satırların yerinde iskelet bloklar duruyor (→ 3.5).", "E0274"),
            ("Kısayolun açtığı liste → 7.3.", "E0306"),
        ],
        "urunler": [
            ("Money Manager, Hesap Defterim, Goodbudget", "yok", "İncelenen ana ekran karelerinde bekleyen iş "
             "alanı görünmüyor.", "E0228, E0137, E0115"),
            ("Bluecoins", "yok", "İncelenen Hesaplar sekmesi karesinde görünmüyor; bekleyenler aynı sekme "
             "çubuğundaki Hatırlatıcılar'da (→ 7.3).", "E0103"),
            ("KolayBi", "kaynak", "Güncel Durum ekranında Günü Gelen İşlemler: Bugün, Yaklaşanlar, Tarihi "
             "Geçenler (→ 2.7).", "E0211"),
            ("Paraşüt", "beyan", "Tanıtım anlatımında ana ekran tahsilatı ve ödemeyi vade durumuna göre "
             "ayırıyor (→ 2.7).", "E0011"),
        ],
    },

    # ---------------------------------------------------------------- 7.5
    {
        "tur": "soru", "no": "7.5", "baslik": "Onay ve otomatik tercihi",
        "giris": "Wallet'ta bekleyen örnek Confirm ile kayda dönüşüyor. İlk onaydan sonra ürün, sonraki örneklerin "
                 "otomatik mi onayla mı kaydedileceğini soruyor.",
        "sekiller": [
            {"k": "wl-ozet", "e": "E0290", "etiket": "Wallet · Payment summary",
             "isaretler": [
                 (1, 540, 1560, "Onaydan önce tarih, hesap ve tutar açılır alan olarak."),
             ]},
            {"k": "wl-soru", "e": "E0291", "etiket": "Wallet · ilk onaydan sonra",
             "isaretler": [
                 (2, 960, 1235, "Yes (Recommended) önceden seçili; yalnız Confirm."),
             ]},
            {"k": "wl-soru-ayar", "e": "E0318", "etiket": "Wallet · aynı soru, sonradan açıldı",
             "isaretler": [
                 (3, 960, 1400, "Kayıtlı seçim No; Cancel eklenmiş."),
             ]},
            {"k": "wl-sonra", "e": "E0292", "etiket": "Wallet · onaydan sonra plan ayrıntısı",
             "isaretler": [
                 (4, 530, 790, "Sıradaki örnek: Due in 30 days, Confirm."),
                 (5, 560, 1040, "Gerçekleşen örnek: Paid Today."),
             ]},
        ],
        "notlar": [
            ("Bekleyen örneğin üç nokta menüsünde Postpone ve Dismiss var; sonuçları denenmedi.", "E0400"),
            "No seçildi; Yes kolunun sonucu, vadeden önce onay ve değiştirilmiş tutarla onay görülmedi.",
            ("Sorunun plan başına mı hesap başına mı geçerli olduğu metinde yazmıyor.", "E0291"),
        ],
        "urunler": [
            ("Bluecoins", "canli", "Otomatik kayıt kurulum formunda bir işaret kutusu (→ 7.1). Kutu kapalıyken "
             "gecikmiş hatırlatıcı kaydedilirken tarih soruluyor: bugün mü, vade tarihi mi.", "E0039, E0043"),
            ("Money Manager", "canli", "Onay kurulumda bir kez (→ 7.1); örnek başına onay yok. Ne zaman "
             "uygulanacağını bir ayar belirliyor: Tarihte ya da Her ayın ilk günü. Vadesi gelmemiş tekrarlar ay "
             "listesinde ayrı bir Tekrarlama bloğunda (→ 7.3).", "E0430, E0429, E0416, E0407"),
            ("Goodbudget", "kosum", "Sonraki örneğin ne zaman ve onayla mı üretildiği bilinmiyor.", "E0006"),
            ("KolayBi", "yok", "Görülen formda onay veya otomatik tercihi yok (→ 7.1).", "E0205"),
        ],
    },

    # ---------------------------------------------------------------- 7.6
    {
        "tur": "soru", "no": "7.6", "baslik": "Bütçe ve hedef yüzeyi",
        "giris": "Wallet bütçe ile hedefi aynı sekmede tutuyor. Bütçe ayrıntısı harcananı, bir tahmini ve günlük "
                 "ortalamayı gösteriyor; hedef formu tutar ve tarih soruyor.",
        "sekiller": [
            {"k": "wl-butce", "e": "E0302", "etiket": "Wallet · bütçe ayrıntısı",
             "isaretler": [
                 (1, 700, 820, "Harcanan 6.600; bütçe 5.000; kırmızı çubuk."),
                 (2, 700, 1030, "Forecasted Spend: 30 gün için 6.600."),
                 (3, 980, 1380, "Günlük ortalama harcama."),
                 (4, 500, 1660, "Geçen dönemin aynı günlerine göre +222%."),
             ]},
            {"k": "wl-hedef", "e": "E0304", "etiket": "Wallet · New goal",
             "isaretler": [
                 (5, 700, 640, "Target amount, Saved already, Desired date."),
                 (6, 700, 1100, "Desired date varsayılanı bugün."),
             ]},
            {"k": "wl-butce-hedef", "e": "E0305", "etiket": "Wallet · Budgets & Goals",
             "isaretler": [
                 (7, 700, 520, "Bütçe kartı: −1.600,00 / 5.000,00, Over Budget."),
                 (8, 700, 1215, "Hedef satırı: ₺0 ve %0."),
             ]},
        ],
        "notlar": [
            ("Bütçe formu ad, dönem ve tutar soruyor; kategori, hesap ve etiketle daraltılabiliyor.", "E0299"),
            ("Bütçe aşılınca alt bantta aşım uyarısı çıktı.", "E0301"),
            "Forecasted Spend bu karede harcanan tutara eşit; hesaplama yöntemi bilinmiyor.",
            ("Hedefin ayrıntısında 0 / 20.000 ₺ ve %0; altında Add saved amount ve Set goal as reached.",
             "E0470"),
        ],
        "urunler": [
            ("Bluecoins", "canli", "Bütçe Özeti kartının ayrıntısı kategori başına güncel tutarı ve bütçeyi "
             "gösteriyor; incelenen veride hiçbir kategoriye bütçe kurulmamış. Kurulumun girişi Kategoriler "
             "ekranındaki Bütçe Kur düğmesi; kurulum formu açılmadı.", "E0018, E0442, E0087"),
            ("Money Manager", "canli", "Bütçe kategori başına; Toplam sekmesinde bir blok olarak görünüyor. "
             "Bütçe Ayarları varsayılan tutar ve ay ay değişiklik alıyor; değişiklik önümüzdeki aydan "
             "geçerli.", "E0433, E0431, E0432"),
            ("Hesap Defterim", "yok", "Bütçe veya hedef yüzeyi görülmedi.", "E0007"),
        ],
    },
    {
        "tur": "soru", "no": "7.6", "baslik": "Zarf satırı ve dönem sonu tahmini", "haritada": False,
        "giris": "Goodbudget'ta bütçe ana ekranın kendisi: her zarf satırı bir tutar ve bir ilerleme çubuğu "
                 "taşıyor. KolayBi'nin nakit akış raporu güncel bakiyenin yanında bir dönem sonu tahmini veriyor.",
        "not_genislik": 190, "yukseklik": 330,
        "sekiller": [
            {"k": "gb-zarf", "e": "E0115", "etiket": "Goodbudget · ENVELOPES",
             "isaretler": [
                 (1, 690, 455, "Üstte dağıtılan toplam."),
                 (2, 540, 600, "Zarf satırı: iki tutar ve ilerleme çubuğu."),
                 (3, 540, 1000, "[Available]: dağıtılmamış tutar satırı."),
             ]},
            {"k": "kb-nakit", "e": "E0216", "etiket": "KolayBi · Nakit Akış Raporu",
             "kirpma": (96, 112, 1305, 640), "genislik": 400,
             "isaretler": [
                 (4, 399, 308, "Güncel Bakiye, Tahsilatlar, Ödemeler."),
                 (5, 1174, 288, "Tahmini Dönem Sonu Bakiyesi."),
                 (6, 234, 408, "Belirsiz ve Geçmiş kovaları, ardından on iki ay."),
             ]},
        ],
        "notlar": [
            ("Goodbudget'ta hesap katmanı varsayılan olarak kapalı; zarf kalanı hesap bakiyesinden ayrı bir "
             "gösterge (→ 5.1).", "E0111"),
            ("Goodbudget satırındaki iki tutarın hangisinin kalan, hangisinin bütçelenen olduğu karede yazmıyor; "
             "koşum kaydına göre üstteki kalan, alttaki bütçelenen.", "E0115; E0006 K01, GB-03"),
            "KolayBi karesinde bütün tutar Belirsiz kovasında; aylık kovalar boş. Tahminin formülü kaynakta "
            "tanımlanmıyor.",
            "KolayBi demo verisi 2023–2024 tarihli; güncel sürüm doğrulanmadı.",
        ],
    },

    # ---------------------------------------------------------------- ozet
    {
        "tur": "tablo", "no": "", "baslik": "Özet: hangi yüzey nerede görüldü?",
        "giris": "Yeni kanıt yok; önceki sayfaların satır satır dökümü. Yokluk ifadeleri incelenen sürüm, yüzey ve "
                 "kaynakla sınırlıdır.",
        "sutunlar": [("Ürün", 1.2), ("Tekrar", 1.5), ("Taksit", 1.5), ("Bekleyen liste", 1.5),
                     ("Ana ekranda", 1.4), ("Onay / otomatik", 1.6), ("Bütçe / hedef", 1.5)],
        "boy": 7.2,
        "satirlar": [
            "Canlı incelenen ürünler",
            ["Money Manager",
             {"t": "Rozet; 14 sıklık (7.1).", "tur": "canli", "d": "E0241, E0238"},
             {"t": "Rozet; başlıkta (1/6) (7.2).", "tur": "canli", "d": "E0245, E0246"},
             {"t": "Gelecek ayda Tekrarlama satırı; ayarlarda tekrar listesi (7.3).", "tur": "canli",
              "d": "E0244, E0416"},
             {"t": "Karede yok (7.4).", "tur": None, "d": "E0228"},
             {"t": "Kurulumda tek onay; uygulanma zamanı ayarda (7.5).", "tur": "canli", "d": "E0430, E0429"},
             {"t": "Kategori başına bütçe (7.6).", "tur": "canli", "d": "E0431, E0433"}],
            ["Bluecoins",
             {"t": "Alt sayfa; bitiş seçenekleri (7.1).", "tur": "canli", "d": "E0039"},
             {"t": "Oran, sayı, ilk ödeme; özet (7.2).", "tur": "canli", "d": "E0034, E0035"},
             {"t": "Hatırlatıcılar; sözcükle durum, sayaç (7.3).", "tur": "canli", "d": "E0047, E0056"},
             {"t": "Karede yok; ayrı sekme (7.4).", "tur": None, "d": "E0103"},
             {"t": "Otomatik kutusu; tarih sorusu (7.5).", "tur": "canli", "d": "E0039, E0043"},
             {"t": "Bütçe Özeti kartı; Bütçe Kur girişi (7.6).", "tur": "canli", "d": "E0018, E0442, E0087"}],
            ["Wallet",
             {"t": "Planned payment formu (7.1).", "tur": "canli", "d": "E0288"},
             {"t": "İncelenen yollarda yok (7.2).", "tur": None, "d": "E0294"},
             {"t": "Planned payments; sıradaki vade (7.3).", "tur": "canli", "d": "E0306"},
             {"t": "Kısayol ve kart (7.4).", "tur": "canli", "d": "E0293, E0469"},
             {"t": "Özet, otomatik/onaylı sorusu (7.5).", "tur": "canli", "d": "E0290, E0291, E0318"},
             {"t": "Bütçe ayrıntısı; hedef formu (7.6).", "tur": "canli", "d": "E0302, E0304"}],
            ["Hesap Defterim",
             {"t": "Çekmece, ayarlar, form tarandı (7.1).", "tur": None, "d": "E0171"},
             {"t": "Taksit planı kurulamadı (7.2).", "tur": None, "d": "E0007"},
             {"t": "Ayrı liste bulunmadı (7.3).", "tur": None, "d": "E0007"},
             {"t": "Karede yok (7.4).", "tur": None, "d": "E0137"},
             {"t": "—", "tur": None},
             {"t": "Yüzey bulunmadı (7.6).", "tur": None, "d": "E0007"}],
            ["Goodbudget",
             {"t": "Schedule this…; e-posta satırı (7.1).", "tur": "canli", "d": "E0133"},
             {"t": "Zarflara bölme, zamana değil (7.2).", "tur": "kosum", "d": "E0006"},
             {"t": "Ayrı liste bulunmadı (7.3).", "tur": None, "d": "E0006"},
             {"t": "Karede yok (7.4).", "tur": None, "d": "E0115"},
             {"t": "Bilinmiyor (7.5).", "tur": "kosum", "d": "E0006"},
             {"t": "Zarf satırı ve çubuk (7.6).", "tur": "canli", "d": "E0115"}],
            "Kaynakla incelenen ürünler",
            ["KolayBi",
             {"t": "Kayıttan dönüştürme (7.1).", "tur": "kaynak", "d": "E0204, E0205"},
             {"t": "—", "tur": None},
             {"t": "—", "tur": None},
             {"t": "Günü Gelen İşlemler (7.4).", "tur": "kaynak", "d": "E0211"},
             {"t": "Formda yok (7.5).", "tur": None, "d": "E0205"},
             {"t": "Dönem sonu tahmini (7.6).", "tur": "kaynak", "d": "E0216"}],
            ["Paraşüt",
             {"t": "—", "tur": None},
             {"t": "—", "tur": None},
             {"t": "—", "tur": None},
             {"t": "Vadeye göre ayrım (7.4).", "tur": "beyan", "d": "E0011"},
             {"t": "—", "tur": None},
             {"t": "—", "tur": None}],
        ],
        "notlar": [
            "Logo İşbaşı ve QuickBooks Solopreneur için bu bölümün sorularında karşılaştırılabilir kaynak "
            "anlatımı bulunmadı; bu, ürünlerde özellik olmadığı anlamına gelmez.",
            "Boş hücre (—): o soru bu ürün için incelenmedi ya da önceki sütunlarda yüzey bulunmadığı için sorulamadı.",
        ],
    },
]

EKSIKLER = [
    ("Wallet", "Postpone / Dismiss sonucu", "7.5", "Plan verisini değiştirir", "Önerilmez"),
    ("Bluecoins", "Otomatik giriş açıkken vade günü", "7.5", "Gelecek vadeyi beklemek gerekir", "Önerilmez"),
    ("Goodbudget", "Sonraki tekrar örneğinin üretimi (B06)", "7.5", "Gelecek vadeyi beklemek gerekir",
     "Önerilmez"),
    ("KolayBi", "Oluşturma Periyodu değerleri (resmî görsellerde liste kapalı)", "7.1",
     "Canlı deneme hesabı gerekir", "Önerilmez"),
    ("Bluecoins", "Sıfır dışı taksit oranının kayda etkisi", "7.2", "Oranlı yeni bir taksitli kayıt girmek",
     "Orta"),
    ("Bluecoins", "Bütçe kurulum formu (Bütçe Kur)", "7.6", "Kategoriler ekranında Bütçe Kur'a dokunmak", "Orta"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0087", "E0274", "E0399", "E0400", "E0407", "E0416", "E0429", "E0430", "E0431", "E0432",
               "E0433", "E0442", "E0470", "E0498"]
