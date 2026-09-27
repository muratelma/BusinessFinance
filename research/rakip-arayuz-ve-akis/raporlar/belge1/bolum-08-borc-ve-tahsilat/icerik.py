# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 8 · Borc, fatura ve tahsilat — tek icerik kaynagi (plan §4)."""

NO = 8
BASLIK = "Borç, fatura ve tahsilat"
ANA_SORU = "Bir alacak nasıl kaydediliyor, tahsilat nasıl işleniyor ve kalan nerede görünüyor?"
GIRIS = [
    "Bu bölüm karşı tarafın nerede listelendiğini, alacağın hangi ekrandan yazıldığını, tahsilatın hangi "
    "ekrandan işlendiğini ve kalan tutarın nerede göründüğünü izler.",
    "İki üründe aynı senaryo girildi: Ada Reklam'a 12.000'lik hizmet alacağı ve 5.000'lik kısmi "
    "tahsilat. İki ürün bunu farklı ekranlarla kaydediyor; tutarların bakiyeye ve rapora etkisi bu bölümde "
    "anlatılmaz.",
]
GIRMEZ = [
    "Tutarların bakiyeye etkisi ve zinciri → Belge 2",
    "E-belge ve entegrasyon → 10",
    "Rapor toplamları → 9",
    "Tekrar eden fatura → 7",
    "Kendi modelimizle eşdeğerlik hükmü → Belge 3",
]
KAPSAM = [
    ("Money Manager", ["yok"], "Cari zinciri kurulmadı."),
    ("Bluecoins", ["canli"], "Cari hesap ve transfer."),
    ("Wallet", ["canli"], "Debts yüzeyi."),
    ("Hesap Defterim", ["yok"], "Ayrı cari yüzeyi görülmedi."),
    ("Goodbudget", ["kosum"], "Debt grubu denenmedi."),
    ("KolayBi", ["kaynak"], "Cari detayı, açılış, ekstre."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], None),
]
KB = (96, 112, 1305, 865)
# PDF onizlemesindeki dosya adi, adres, telefon ve e-posta; icerige gerekmiyor.
KARARTMA = {
    "E0197": [(410, 355, 588, 382), (615, 444, 812, 488)],
}
KAYNAK_NOTLARI = [
    "E0197'deki PDF önizlemesinde dosya adı, adres, telefon ve e-posta bu bölümün kopyasında karartıldı; "
    "özgün kanıt değişmedi.",
]

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 8.1
    {
        "tur": "soru", "no": "8.1", "baslik": "Karşı taraf nerede listeleniyor?",
        "giris": "Bluecoins karşı tarafı bir hesap olarak, hesap listesinin kendi grubunda tutuyor. Wallet borçları "
                 "ayrı bir Debts yüzeyinde topluyor.",
        "yukseklik": 384,
        "sekiller": [
            {"k": "bc-cari-grup", "e": "E0072", "etiket": "Bluecoins · hesap seçici",
             "isaretler": [
                 (1, 700, 1767, "Hesap grupları arasında Cari hesap."),
                 (2, 700, 1925, "Karşı taraf bir hesap satırı; bakiyesi altında."),
             ]},
            {"k": "wl-debts", "e": "E0307", "etiket": "Wallet · Debts, borç girilmeden önce",
             "isaretler": [
                 (3, 900, 520, "Active / Closed sekmeleri."),
                 (4, 540, 1850, "Boş durumda yönlendirme: Track what you lent and borrowed."),
             ]},
        ],
        "notlar": [
            ("Wallet'ta Debts çekmeceden açılan ayrı bir bölüm (→ 2.4).", "E0376"),
            "Bluecoins'te cari hesap, banka ve kredi kartı hesaplarıyla aynı seçicide duruyor.",
            ("Bluecoins'in cari hesap formu başlangıç bakiyesi, son bakiye, açılış tarihi, hesap tipi ve iki "
             "anahtar taşıyor; vade alanı yok. KolayBi'nin cari formu vade soruyor (→ 8.5).", "E0449"),
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Cari listesi; Müşteri, Tedarikçi, ikisi birden ve yurt dışı tipleri (→ 6.4). "
             "Cari detayı → 8.5.", "E0194"),
            ("Hesap Defterim", "yok", "Ayrı bir cari listesi görülmedi. Çekmecede ayrı bir Veresiye Defteri "
             "uygulamasına bağlantı var; o uygulama incelenmedi.", "E0177, E0007"),
            ("Goodbudget", "kosum", "Hesap türlerinde Debt grubu var; içeriği denenmedi.", "E0006"),
            ("Money Manager", "yok", "Cari veya karşı taraf yüzeyi görülmedi; kart borcu → 5.", "E0010"),
        ],
    },

    # ---------------------------------------------------------------- 8.2
    {
        "tur": "soru", "no": "8.2", "baslik": "Alacak hangi ekrandan yazılıyor?",
        "giris": "Bluecoins'te alacak normal kayıt formunda yazılıyor: tür GELİR, hesap cari hesap. Wallet'ta ayrı "
                 "bir borç formu var; ürün formdan önce ve kaydederken iki soru soruyor.",
        "sekiller": [
            {"k": "bc-borclandir", "e": "E0074", "etiket": "Bluecoins · Ekle, cari hesaba gelir",
             "isaretler": [
                 (1, 800, 1110, "Hesap alanında cari hesap."),
                 (2, 540, 1960, "Tür seçicide GELİR."),
             ]},
            {"k": "wl-bagla", "e": "E0308", "etiket": "Wallet · borç formundan önce",
             "isaretler": [
                 (3, 540, 1300, "Mevcut bir kaydı borca bağlama sorusu; Yes / No, skip."),
             ]},
            {"k": "wl-ilent", "e": "E0364", "etiket": "Wallet · I Lent formu",
             "isaretler": [
                 (4, 700, 460, "Başlıkta tür: I Lent. Ad, açıklama, hesap, tutar."),
             ]},
            {"k": "wl-record-soru", "e": "E0365", "etiket": "Wallet · kaydederken",
             "isaretler": [
                 (5, 300, 1362, "Record oluşturulsun mu? Oluşturulursa bakiye değişir."),
             ]},
        ],
        "notlar": [
            ("Wallet'ta alacak I Lent türüyle girildi; ürün bu kartı fatura diye adlandırmıyor.", "E0364"),
            ("Borç eklemek iki yönle başlıyor: I Lent ve I Borrowed. I Lent formunda Date ve Due date var; "
             "vadenin varsayılanı bir yıl sonrası.", "E0358, E0362"),
            ("Aynı adla ikinci borç girilirken uyarı görülmedi.", "E0364"),
            ("Yes kolunda borca Loan, interests kategorili bir kayıt bağlandı; bağlama listesi mevcut kayıtları "
             "etiketleriyle gösteriyor.", "E0317, E0441"),
            ("Bluecoins'te kaydın adı serbest metin; adındaki fatura sözcüğü ayrı bir fatura nesnesi değil.",
             "E0068"),
            "Alacağın iki üründe hangi toplama yazıldığı → Belge 2 4.1–4.3.",
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Cari detayında Borç/Alacak Ekle ve Fatura Ekle ayrı düğmeler (→ 8.5).", "E0196"),
        ],
    },

    # ---------------------------------------------------------------- 8.3
    {
        "tur": "soru", "no": "8.3", "baslik": "Tahsilat hangi ekrandan işleniyor?",
        "giris": "Bluecoins'te tahsilat, cari hesaptan bankaya bir TRANSFER. Wallet'ta tahsilat aynı borç kartından "
                 "başlıyor ve karta bağlı bir kayıt olarak yazılıyor.",
        "sekiller": [
            {"k": "bc-tahsilat", "e": "E0078", "etiket": "Bluecoins · Ekle, TRANSFER",
             "isaretler": [
                 (1, 800, 870, "Kaynak cari hesap; hedef banka."),
                 (2, 800, 1260, "Transfer ücreti, Durum, Etiket; fatura seçen alan görünmüyor."),
             ]},
            {"k": "wl-add-record", "e": "E0367", "etiket": "Wallet · Add Record",
             "isaretler": [
                 (3, 540, 1800, "Mevcut kaydı bağla ya da yeni kayıtla öde veya borcu artır."),
             ]},
            {"k": "wl-repay", "e": "E0369", "etiket": "Wallet · Create Debt Record",
             "isaretler": [
                 (4, 700, 470, "Debt action: Repay debt."),
                 (5, 700, 930, "Kısmi tutar: 5.000."),
             ]},
        ],
        "notlar": [
            ("Tutar boşken yer tutucu kalan tutarı gösteriyor: ₺12.000,00 to Repay debt.", "E0368"),
            ("Debt action iki değer taşıyor: Repay debt ve Increase debt.", "E0451"),
            ("Bluecoins'in kayıt formunda da fatura ya da belge seçen alan yok; belge yalnız formun üstündeki "
             "ataçla ekleniyor (→ 4.2).", "E0448"),
            "Tahsilatın hangi alacağı kapattığı → Belge 2 4.3.",
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Cari detayında Ödeme/Tahsilat Ekle ayrı bir düğme (→ 8.5).", "E0196"),
        ],
    },

    # ---------------------------------------------------------------- 8.4
    {
        "tur": "soru", "no": "8.4", "baslik": "Kalan tutar nerede yazıyor?",
        "giris": "Bluecoins'te kalan, cari hesabın bakiyesi olarak kayıt satırında görünüyor. Wallet'ta kalan borç "
                 "kartının üstünde; aynı ada ait iki borç iki ayrı kart.",
        "sekiller": [
            {"k": "bc-kalan", "e": "E0079", "etiket": "Bluecoins · İşlemler, tahsilattan sonra",
             "isaretler": [
                 (1, 80, 600, "Satırın sağında hesabın o anki bakiyesi: cari 7.000."),
                 (2, 80, 822, "Alacak satırında o günkü bakiye 12.000; bugünkü kalan değil."),
             ]},
            {"k": "wl-iki-kart", "e": "E0366", "etiket": "Wallet · Debts, tahsilattan önce",
             "isaretler": [
                 (3, 250, 590, "Aynı ada iki ayrı kart; toplam satırı yok."),
                 (4, 700, 1000, "Add Record kartın içinde."),
             ]},
            {"k": "wl-kalan", "e": "E0370", "etiket": "Wallet · Debts, tahsilattan sonra",
             "isaretler": [
                 (5, 650, 870, "Yalnız bağlı kart 7.000'e indi; öteki kart değişmedi."),
             ]},
        ],
        "notlar": [
            ("Wallet'ta borcun kendi kayıt listesinde bir Total satırı var; işareti karttakinin tersi.", "E0317"),
            ("Tahsilattan sonra borcun listesinde yalnız tahsilat satırı var (Ada Reklam → Me, +5.000); Total "
             "₺5.000,00, altta Add Record.", "E0478"),
            "Kalanın hangi alacağa düştüğü → Belge 2 4.3, 4.4.",
            "Bluecoins'te 7.000 cari hesabın toplamıdır; hangi alacağın kalanı olduğu ekranda yazmıyor.",
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Cari detayında Toplam Bakiye kartı; hareket tablosunda Bakiye kolonu (→ 8.5).",
             "E0196"),
            ("Hesap Defterim", "yok", "Karşı taraf başına kalan görülmedi; defterin kendi bakiyesi var.", "E0007"),
        ],
    },

    # ---------------------------------------------------------------- 8.5
    {
        "tur": "soru", "no": "8.5", "baslik": "Kaynakta görülen cari akışı",
        "giris": "KolayBi'nin destek görsellerinde cari detayı borçlandırma, fatura ve tahsilatı üç ayrı düğmeyle "
                 "ayırıyor. Cari oluşturma formu açılış bakiyesini ve vadeyi soruyor.",
        "dikey": True,
        "sekiller": [
            {"k": "kb-cari-detay", "e": "E0196", "etiket": "KolayBi · cari detayı (önde ekstre diyaloğu → 10.1)",
             "kirpma": KB, "genislik": 350,
             "isaretler": [
                 (1, 954, 128, "Borç/Alacak Ekle, Fatura Ekle, Ödeme/Tahsilat Ekle."),
                 (2, 974, 218, "İşlemler: Mahsuplaştır, Cari Ekstresi Oluştur, pasifleştir, sil."),
                 (3, 1169, 403, "Toplam Bakiye kartı."),
                 (4, 704, 653, "Cari Hareketleri: vade, borç, alacak, bakiye, banka/kasa, proje."),
             ]},
            {"k": "kb-cari-form", "e": "E0195", "etiket": "KolayBi · Cari Detay Bilgileri",
             "kirpma": (96, 112, 1305, 795), "genislik": 350,
             "isaretler": [
                 (5, 324, 138, "Vade Günü: Yok / Var."),
                 (6, 324, 295, "Açılış Bakiyesi: tutar, para birimi, durum, proje, tarih, vade."),
                 (7, 324, 538, "Oluştururken Borç Alacak Ekle."),
             ]},
        ],
        "notlar": [
            "Düğmelerin ayrı olması, arkadaki kayıt modelinin nasıl işlediğini göstermez.",
            "Kısmi kapanışın faturaya nasıl yazıldığı → Belge 2 4.5.",
            "Görseller destek materyalidir; demo verisi 2023 tarihli ve güncel sürüm doğrulanmadı.",
        ],
        "urunler": [
            ("KolayBi", "kaynak", "Gider formunda Cari Takibi Yok/Var ile Ödeme Durumu ayrı alanlar (→ 4.9).",
             "E0192"),
        ],
    },
    {
        "tur": "soru", "no": "8.5", "baslik": "Ekstre önizlemesi ve kaynak beyanları", "haritada": False,
        "giris": "Cari ekstre bir PDF önizlemesi olarak açılıyor. Karesi basılmayan üç ürünün borç ve fatura akışı "
                 "yalnız kendi anlatımlarından biliniyor; kaynaktan kurulan akış modeli Belge 2 Bölüm 9'dadır.",
        "not_genislik": 300,
        "sekiller": [
            {"k": "kb-ekstre", "e": "E0197", "etiket": "KolayBi · Cari Hesap Ekstresi önizlemesi (kişisel "
                                                      "bilgiler karartıldı)",
             "kirpma": KB, "genislik": 420,
             "isaretler": [
                 (1, 604, 498, "Borç, alacak ve bakiye toplamı."),
                 (2, 409, 583, "Yeni Sekmede Görüntüle, E-Posta ile Gönder, Kapat."),
             ]},
        ],
        "notlar": [
            "E-Posta ile Gönder düğmesi gönderim kanıtı değildir (→ 10).",
        ],
        "urunler_baslik": "Karesi basılmayan ürünler",
        "urunler": [
            ("Paraşüt", "beyan", "Kılavuza göre gider veya fatura önce, ödeme sonra yazılıyor; kısmi ödeme ve "
             "avans var; tahsilat en gecikmiş açık faturadan başlayarak eşleniyor.", "E0011"),
            ("Logo İşbaşı", "beyan", "Anlatımda müşteri seçilirken bakiyesi görünüyor; tahsilat ve ödeme cari "
             "içinden nakit veya banka ile yapılıyor.", "E0009"),
            ("QuickBooks Solopreneur", "beyan", "Ürün sayfasında fatura sırası: müşteri, kalem, ödeme yöntemi, "
             "gönderim ve hatırlatma.", "E0012"),
        ],
        "dayanak": ["Kaynak beyanı: E0011, E0009, E0012"],
    },
]

EKSIKLER = [
    ("Wallet", "Kalandan büyük tahsilatın sonucu", "8.3", "Kalanı aşan bir Repay debt girmek", "Orta"),
    ("KolayBi", "Mahsuplaştır menüsünün sonucu", "8.5", "Canlı deneme hesabı gerekir", "Önerilmez"),
    ("Goodbudget", "Debt hesap grubu içeriği", "8.1", "Ücretli paket gerekir", "Önerilmez"),
    ("Paraşüt / Logo İşbaşı", "Cari ve tahsilat ekranları", "8.5", "Ücretli hesap gerekir", "Önerilmez"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0358", "E0362", "E0441", "E0448", "E0449", "E0451", "E0478"]
