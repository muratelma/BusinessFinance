# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 2 · Urun kimligi — tek icerik kaynagi (plan §4)."""

NO = 2
BASLIK = "Ürün kimliği"
ANA_SORU = "İncelenen dokuz ürün nedir, neresi ve ne kadar derin incelendi?"
GIRIS = [
    "Dokuz ürün kartı. Her kartta aynı alanlar var: incelenen platform ve sürüm, kaynaklarda belirtilen kullanım "
    "odağı, gözlenen temel arayüz yaklaşımı, inceleme kapsamı ve bilinmeyenler. Kanıtı az olan ürünün kartı "
    "sayfayı doldurmak için uzatılmadı.",
    "Bu bölüm ürünleri tek tek anlatan tek bölümdür; sonraki bölümler konuya göre ilerler ve kartlardaki "
    "okların gösterdiği yerlerde aynı ürüne döner.",
]
GIRMEZ = [
    "Yaygınlık, indirme sayısı ve puan",
    "Memnuniyet",
    "Fiyat karşılaştırması",
    "Ürünlerin BusinessFinance'e yakınlığı → Belge 3",
]
KAPSAM = [
    ("Money Manager", ["canli", "kosum"], "Android · 30 kare"),
    ("Bluecoins", ["canli", "kosum"], "Android · 92 kare"),
    ("Wallet", ["canli", "kosum"], "Android · 109 kare"),
    ("Hesap Defterim", ["canli", "kosum"], "Android · 45 kare"),
    ("Goodbudget", ["canli", "kosum"], "Android · 31 kare; ücretsiz paket sınırlı"),
    ("KolayBi", ["kaynak", "kosum"], "Web · 31 destek görseli basılıyor"),
    ("Paraşüt", ["beyan", "kosum"], "Kareleri basılmıyor"),
    ("Logo İşbaşı", ["beyan", "kosum"], "Kareleri basılmıyor"),
    ("QuickBooks Solopreneur", ["beyan"], "Yalnız yardım merkezi"),
]
ACILIS_DAYANAK = ("Kare sayıları kanıt envanterindendir. Kanıt türleri ve bazı ürünlerin karesinin neden "
                  "basılmadığı Bölüm 1'dedir.")


def _kart(urun, sekil, incelenen, odak, yaklasim, kapsam):
    return {
        "urun": urun, "sekil": sekil,
        "alanlar": [
            ("İncelenen",) + incelenen,
            ("Kaynaklarda belirtilen odak",) + odak,
            ("Gözlenen arayüz yaklaşımı",) + yaklasim,
            ("Kapsam ve bilinmeyenler",) + kapsam,
        ],
    }


SAYFALAR = [
    {"tur": "acilis"},
    {
        "tur": "kartlar", "no": "2.1", "baslik": "Canlı incelenen ürünler",
        "dayanak": ["Koşum kaydı: E0010, E0005 (gözlem formları)"],
        "kartlar": [
            _kart("Money Manager",
                  {"k": "mm-kimlik", "e": "E0228", "etiket": "ana ekran", "yukseklik": 250},
                  ("Android, ücretsiz reklamlı sürüm 4.12.8, Türkçe arayüz. 1 ve 10 Eylül 2026.", "kosum", "E0010"),
                  ("Kişisel ve gündelik bütçe defteri; hızlı giriş.", "kosum", "E0010"),
                  ("Alt çubuk ve her ekranda aynı Gelir / Gider / Toplam satırı; kart hesabında ekstre dönemi ve "
                   "karta özel ödeme düğmesi (→ 3.2, 6.3, 6.5).", "canli", "E0228, E0233, E0255"),
                  ("Kayıt ve giriş yok; veri cihazda. İlk turun İngilizce kareleri diskte olmadığı için Türkçe "
                   "koşum yeniden yapıldı. Bütçe kurulumu, yedek ve Excel çıktısı inceleme dışında bırakıldı.",
                   "kosum", "E0010")),
            _kart("Bluecoins",
                  {"k": "bc-kimlik", "e": "E0103", "etiket": "Hesaplar sekmesi", "yukseklik": 250},
                  ("Android, ücretsiz yerel sürüm 13.1.45; ağırlıkla Türkçe, bazı tarih ve kategori adları "
                   "İngilizce. 1, 10 ve 11 Eylül 2026.", "kosum", "E0005"),
                  ("İleri düzey kişisel bütçe: çok hesap, kredi, ipotek, rapor.", "kosum", "E0005"),
                  ("Üstte kaydırılan sekmeler ve çekmece; düzenlenebilir özet kartları; tek yoğun formda planlama, "
                   "bölme ve taksit (→ 3.2, 5.2, 8.2).", "canli", "E0103, E0020, E0034"),
                  ("Kayıt ve giriş yok. En derin incelenen iki üründen biri. Kart ekstre davranışı ve dosya "
                   "çıktısının son adımı denenmedi.", "kosum", "E0005")),
        ],
    },
    {
        "tur": "kartlar", "no": "2.1", "baslik": "Canlı incelenen ürünler — 2", "haritada": False,
        "dayanak": ["Koşum kaydı: E0014, E0007 (gözlem formları)"],
        "kartlar": [
            _kart("Wallet (BudgetBakers)",
                  {"k": "wl-kimlik", "e": "E0276", "etiket": "Home", "yukseklik": 250},
                  ("Android 9.3.6, arayüz İngilizce; bulut hesabıyla giriş yapıldı. 1, 10 ve 11 Eylül 2026.",
                   "kosum", "E0014"),
                  ("Kişisel ve ortak hane finansı; ücretli pakette banka bağlantısı.", "kosum", "E0014"),
                  ("Çekmece ve ana ekranda hesap kartları; kayıttan sonra açılan ayrıntı ekranı; borç kartları "
                   "ve planlı ödeme onayı (→ 3.2, 5.2, 8.5, 9.2).", "canli", "E0276, E0294, E0366, E0290"),
                  ("En derin incelenen iki üründen biri. İlk açılış ve boş ana ekran görülmedi; banka bağlantısı "
                   "ve grup paylaşımı denenmedi.", "kosum", "E0014")),
            _kart("Hesap Defterim",
                  {"k": "hd-kimlik", "e": "E0137", "etiket": "ana ekran", "yukseklik": 250},
                  ("Android, ücretsiz reklamlı sürüm, Türkçe arayüz. 10–12 Eylül 2026.", "kosum", "E0007"),
                  ("Yürüyen bakiyeli tek sütunlu defter. Veresiye ve kategorili gelir-gider için aynı geliştiricinin "
                   "ayrı uygulamalarına yönlendiriyor.", "canli", "E0177"),
                  ("Yön adını taşıyan iki kayıt düğmesi, ekranın altında sabit toplam bandı, kategori yerine serbest "
                   "metin; çekmecede yardımcı araçlar (→ 3.5, 5.2, 12).", "canli", "E0137, E0151, E0171"),
                  ("Kayıt ve giriş yok. Uygulama kayıtları sunucuda saklamadığını söylüyor (→ 11.4). Drive yedeği ve "
                   "otomatik e-posta denenmedi.", "kosum", "E0007, E0149")),
        ],
    },
    {
        "tur": "kartlar", "no": "2.1", "baslik": "Canlı ve kaynakla incelenen ürünler", "haritada": False,
        "dayanak": ["Koşum kaydı: E0006, E0008. Kaynak beyanı: E0008"],
        "kartlar": [
            _kart("Goodbudget",
                  {"k": "gb-kimlik", "e": "E0115", "etiket": "ENVELOPES (hane adı karartıldı)", "yukseklik": 250},
                  ("Android 2.24.26013, arayüz İngilizce, tarih biçimi ABD; ücretsiz hane hesabı. 11–12 Eylül 2026.",
                   "kosum", "E0006, E0106"),
                  ("Zarf bütçeleme: parayı harcamadan önce kovalara dağıtma.", "kosum", "E0006"),
                  ("Dört sabit üst sekme; ana ekran zarf listesi; hesap katmanı isteğe bağlı (→ 3.2, 6.1).",
                   "canli", "E0115, E0111"),
                  ("Ücretsiz pakette 10 zarf ve 1 hesap sınırı; kart hesabı ve transfer denenemedi. Dışa aktarma ve "
                   "yedek inceleme dışında.", "kosum", "E0006, E0114")),
            _kart("KolayBi",
                  {"k": "kb-kimlik", "e": "E0211", "etiket": "destek sayfasındaki Güncel Durum ekranı",
                   "kirpma": (96, 112, 1320, 872), "genislik": 377},
                  ("Web paneli: destek sayfası görselleri ve tanıtım videosu. Mobil uygulamada (3.3.1) yalnız giriş "
                   "ekranı; mobilde kayıt yok, web kaydı ücretli. 1–12 Eylül 2026.", "kosum", "E0008"),
                  ("Bulut ön muhasebe ve e-dönüşüm; hedef kitlede şahıs şirketleri adıyla anılıyor.", "beyan", "E0008"),
                  ("Solda sabit modül menüsü; formlarda ödeme durumu, vade ve proje; cari detayda ayrı borçlandırma "
                   "ve tahsilat eylemleri (→ 3.7, 5.9, 9.5).", "kaynak", "E0211, E0192, E0196"),
                  ("Görseller demo verisi taşıyor; güncel sürüm ve davranış doğrulanmadı.", "kosum", "E0008")),
        ],
    },
    {
        "tur": "kartlar", "no": "2.2", "baslik": "Yalnız kaynaktan incelenen ürünler",
        "dayanak": ["Koşum kaydı ve kaynak beyanı: E0011, E0009, E0012 (gözlem formları)"],
        "giris": "Bu üç ürünün karesi basılmıyor: görülen kareler giriş ve kayıt yüzeyi ya da tanıtım videosu "
                 "karesi, iç arayüzü temsil etmiyor (Bölüm 1). Kartlar yalnız metin.",
        "kartlar": [
            {"urun": "Paraşüt", "sekil": None, "alanlar": [
                ("İncelenen", "Android 5.25.0'ın giriş öncesi ekranları, kullanım kılavuzu ve tanıtım videosu. "
                 "Hesap açılamadı.", "kosum", "E0011"),
                ("Kaynaklarda belirtilen odak", "Bulut ön muhasebe ve e-dönüşüm; KOBİ ve e-ticaret.", "beyan", "E0011"),
                ("Kaynağın anlattığı yaklaşım", "Ana ekran tahsilat ve ödemeyi vade durumuna göre ayırıyor; gider ve "
                 "fatura önce, ödeme sonra kaydediliyor (→ 3.7, 9.5).", "beyan", "E0011"),
                ("Kapsam ve bilinmeyenler", "Mobilde kayıt yok; web kaydı şirket bilgisi istiyor. İç arayüzün hiçbir "
                 "ekranı görülmedi.", "kosum", "E0011"),
            ]},
            {"urun": "Logo İşbaşı", "sekil": None, "alanlar": [
                ("İncelenen", "Android 3.20.0'ın giriş ve kayıt ekranları, ürün sayfaları ve tanıtım videosu. "
                 "Ekran etiketleri İngilizce, içerik Türkçe.", "kosum", "E0009"),
                ("Kaynaklarda belirtilen odak", "Mobil fatura ve ön muhasebe; mikro işletme.", "beyan", "E0009"),
                ("Kayıt akışı", "Form şirket adı, sektör ve telefon istiyor; ardından SMS doğrulaması ve sözleşme "
                 "onayı geliyor. İncelenen kayıtta hesap hemen kullanıma açılmadı; firmaya özel hazırlanma, satış ve "
                 "hazırlık sürecine girdi. Bu tek koşumun gözlemidir; ürünün bütün kullanıcılarına genellenmez.",
                 "kosum", "E0009"),
                ("Kaynağın anlattığı yaklaşım", "Sesle fatura kesme ve mali müşavir portalı (→ 5.9, 11.5).",
                 "beyan", "E0009"),
            ]},
            {"urun": "QuickBooks Solopreneur", "sekil": None, "alanlar": [
                ("İncelenen", "Yalnız Intuit yardım merkezi. Mobil uygulamada kayıt yapıldı, başka bir paketin plan "
                 "ekranında durdu; Solopreneur ABD dışına kapalı.", "kosum", "E0012"),
                ("Kaynaklarda belirtilen odak", "ABD'de tek kişilik işletme; işlem başına Business / Personal "
                 "etiketi vergi formuna hizalı.", "beyan", "E0012"),
                ("Kaynağın anlattığı yaklaşım", "Kayıtlar ağırlıkla banka bağlantısından geliyor; kullanıcı türü ve "
                 "kategoriyi inceliyor; kurallarla otomatik etiketleme (→ 7.5, 11.5).", "beyan", "E0012"),
                ("Kapsam ve bilinmeyenler", "Solopreneur'ün hiçbir ekranı görülmedi; görülen dört kare başka bir "
                 "paketin kaydıdır.", "kosum", "E0012"),
            ]},
        ],
    },
]

CIKARILMAYAN = [
    "Ürünlerin pazar payı, yaygınlığı veya kullanıcı memnuniyeti",
    "Fiyat veya paket karşılaştırması",
    "Bir ürünün BusinessFinance'e en yakın ürün olduğu",
]
EKSIKLER = [
    ("Paraşüt, Logo İşbaşı", "İç arayüz", "2.2", "Ücretli veya hazırlanmış hesap gerekir", "Önerilmez"),
    ("QuickBooks Solopreneur", "Solopreneur ekranı", "2.2", "ABD hesabı gerekir", "Önerilmez"),
]
