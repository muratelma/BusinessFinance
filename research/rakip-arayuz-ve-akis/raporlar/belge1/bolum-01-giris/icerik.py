# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 1 · Giris ve incelenen urunler — tek icerik kaynagi (duzeltme plani §3, karar 6).

Eski Bolum 1 (okuma kilavuzu) ve Bolum 2 (urun kimligi) burada birlesir; ikisi _arsiv/ altindadir.
Acilis sayfasi girisin kendisidir (K11); kartlar 1.1 ve 1.2. Kare sayilari KANIT-ENVANTERI.md'nin
son halindendir (461 kare).
"""

NO = 1
BASLIK = "Giriş ve incelenen ürünler"
ANA_SORU = "Bu belge neyi, hangi kanıtla anlatıyor ve hangi ürünler incelendi?"
GIRIS = [
    "Belge 1, dokuz rakip ürünün arayüz yaklaşımlarını konu konu anlatır: ana ekran, görsel dil, kayıt formu, "
    "hesap ve kart, sınıflandırma, planlama, borç, rapor, veri aktarımı ve yardımcı araçlar. Her yaklaşımı "
    "ekranda görüldüğü gibi tarif eder; ürünleri puanlamaz ve sıralamaz. Ekrandaki etiketler ürünün kendi "
    "dilinde ve yazımıyla aktarılır.",
    "Beş ürün Android emülatörde canlı açıldı; dördü yalnız kendi yayımladıkları kaynaklardan incelendi. Test "
    "verisi sentetiktir: aynı senaryo (Ada Reklam geliri, market gideri, kart harcaması ve ödemesi, hesaplar "
    "arası aktarım, abonelik, taksit, alacak ve kısmi tahsilat) her ürüne, ürünün izin verdiği ölçüde girildi. "
    "Kareler aynı anın görüntüsü değildir; sayılar birbiriyle karşılaştırılmaz.",
    "\"Görülmedi\" ile \"yok\" ayrıdır. \"Yok\" yalnız karede ya da incelemede yokluk gözlendiyse ve kapsamıyla "
    "birlikte yazılır. \"Her ekranda\", \"hiçbirinde\" gibi nicelikler yalnız o kapsamı taşıyan kanıt varsa "
    "kullanılır; yoksa cümle gözlenen kapsamla sınırlanır.",
    "Belge başarıyı, hızı, memnuniyeti ve erişilebilirliği ölçmez; ölçülmemiş bir etki için cümle kurulmaz. "
    "Bir kaydın bakiyeye ve rapora etkisi Belge 2'nin, bu bulguların BusinessFinance için ne anlama geldiği "
    "Belge 3'ün konusudur; \"→ Belge 2\" yazan yerler o sınırı gösterir.",
]
GIRMEZ = [
    "Ürünlerin başarısı, hızı, memnuniyeti, erişilebilirliği",
    "Yaygınlık, puan ve fiyat karşılaştırması",
    "Kaydın finansal sonucu → Belge 2",
    "BusinessFinance için tercih → Belge 3",
]
KAPSAM = [
    ("Money Manager", ["canli", "kosum"], "Android · 60 kare"),
    ("Bluecoins", ["canli", "kosum"], "Android · 111 kare"),
    ("Wallet", ["canli", "kosum"], "Android · 141 kare"),
    ("Hesap Defterim", ["canli", "kosum"], "Android · 49 kare"),
    ("Goodbudget", ["canli", "kosum"], "Android · 42 kare; ücretsiz paket sınırlı"),
    ("KolayBi", ["kaynak", "beyan", "kosum"], "Web · 31 destek görselinin 17'si basılıyor"),
    ("Paraşüt", ["beyan", "kosum"], "Kareleri basılmıyor"),
    ("Logo İşbaşı", ["beyan", "kosum"], "Kareleri basılmıyor"),
    ("QuickBooks Solopreneur", ["beyan", "kosum"], "Yalnız yardım merkezi"),
]
ACILIS_DAYANAK = ("Kare sayıları kanıt envanterindendir. \"Görülmedi\", özelliğin incelenen sürümde ve yüzeyde "
                  "görülmediğini söyler; bulunmadığı anlamına gelmez.")


def _kart(urun, alt, sekil, incelenen, odak, yaklasim, kapsam, odak_adi="Kaynaklarda belirtilen odak"):
    return {
        "urun": urun, "alt": alt, "sekil": sekil,
        "alanlar": [
            ("İncelenen",) + incelenen,
            (odak_adi,) + odak,
            ("Gözlenen arayüz yaklaşımı",) + yaklasim,
            ("Kapsam ve bilinmeyenler",) + kapsam,
        ],
    }


SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 1.1
    {
        "tur": "kartlar", "no": "1.1", "baslik": "Canlı incelenen ürünler",
        "giris": "Beş ürün Android emülatörde açıldı. Her kartta aynı alanlar var; kanıtı az olan ürünün kartı "
                 "sayfayı doldurmak için uzatılmadı.",
        "dayanak": ["Koşum kaydı: E0010, E0005 (gözlem formları)"],
        "kartlar": [
            _kart("Money Manager", None,
                  {"k": "mm-kimlik", "e": "E0228", "etiket": "ana ekran", "yukseklik": 250},
                  ("Android, ücretsiz reklamlı sürüm 4.12.8, Türkçe arayüz.", "kosum", "E0010"),
                  ("Kişisel ve gündelik bütçe defteri; hızlı giriş.", "kosum", "E0010"),
                  ("Alt çubuk ve her ekranda aynı Gelir / Gider / Toplam satırı; kart hesabında ekstre dönemi ve "
                   "karta özel ödeme düğmesi (→ 2.2, 5.3, 5.5).", "canli", "E0228, E0233, E0255"),
                  ("Kayıt ve giriş yok; veri cihazda. Yedek ve Excel çıktısı incelenmedi.", "kosum", "E0010")),
            _kart("Bluecoins", None,
                  {"k": "bc-kimlik", "e": "E0103", "etiket": "Hesaplar sekmesi", "yukseklik": 250},
                  ("Android, ücretsiz yerel sürüm 13.1.45; ağırlıkla Türkçe, bazı tarih ve kategori adları "
                   "İngilizce.", "kosum", "E0005"),
                  ("İleri düzey kişisel bütçe: çok hesap, kredi, ipotek, rapor.", "kosum", "E0005"),
                  ("Üstte kaydırılan sekmeler ve çekmece; düzenlenebilir özet kartları; tek yoğun formda planlama, "
                   "bölme ve taksit (→ 2.2, 4.2, 7.2).", "canli", "E0103, E0020, E0034"),
                  ("Kayıt ve giriş yok. En derin incelenen iki üründen biri. Kart ekstre davranışı ve dosya "
                   "çıktısının son adımı denenmedi.", "kosum", "E0005")),
        ],
    },
    {
        "tur": "kartlar", "no": "1.1", "baslik": "Canlı incelenen ürünler (devam)", "haritada": False,
        "dayanak": ["Koşum kaydı: E0014, E0007 (gözlem formları)"],
        "kartlar": [
            _kart("Wallet (BudgetBakers)", None,
                  {"k": "wl-kimlik", "e": "E0276", "etiket": "Home", "yukseklik": 250},
                  ("Android 9.3.6, arayüz İngilizce; bulut hesabıyla giriş yapıldı.", "kosum", "E0014"),
                  ("Kişisel ve ortak hane finansı; ücretli pakette banka bağlantısı.", "kosum", "E0014"),
                  ("Çekmece ve ana ekranda hesap kartları; kayıttan sonra açılan ayrıntı ekranı; borç kartları "
                   "ve planlı ödeme onayı (→ 2.2, 4.2, 7.5, 8.2).", "canli", "E0276, E0294, E0366, E0290"),
                  ("En derin incelenen iki üründen biri. İlk açılış ve boş ana ekran görülmedi; banka bağlantısı "
                   "ve grup paylaşımı denenmedi.", "kosum", "E0014")),
            _kart("Hesap Defterim", None,
                  {"k": "hd-kimlik", "e": "E0137", "etiket": "ana ekran", "yukseklik": 250},
                  ("Android, ücretsiz reklamlı sürüm, Türkçe arayüz.", "kosum", "E0007"),
                  ("Yürüyen bakiyeli tek sütunlu defter. Veresiye ve kategorili gelir-gider için aynı geliştiricinin "
                   "ayrı uygulamalarına yönlendiriyor.", "canli", "E0177"),
                  ("Yön adını taşıyan iki kayıt düğmesi, ekranın altında sabit toplam bandı, kategori yerine serbest "
                   "metin; çekmecede yardımcı araçlar (→ 2.5, 4.2, 11).", "canli", "E0137, E0151, E0171"),
                  ("Kayıt ve giriş yok. Uygulama kayıtları sunucuda saklamadığını söylüyor (→ 10.4). Drive yedeği "
                   "ve otomatik e-posta denenmedi.", "kosum", "E0007, E0149"),
                  odak_adi="Gözlenen odak"),
        ],
    },
    {
        "tur": "kartlar", "no": "1.1", "baslik": "Canlı ve kaynakla incelenen ürünler", "haritada": False,
        "dayanak": ["Koşum kaydı: E0006, E0008. Kaynak beyanı: E0008"],
        "kartlar": [
            _kart("Goodbudget", None,
                  {"k": "gb-kimlik", "e": "E0115", "etiket": "ENVELOPES (hane adı karartıldı)", "yukseklik": 250},
                  ("Android 2.24.26013, arayüz İngilizce, tarih ay/gün/yıl sırasında (bir ayar, → 3.1); ücretsiz "
                   "hane hesabı.", "kosum", "E0006, E0106, E0453"),
                  ("Zarf bütçeleme: parayı harcamadan önce kovalara dağıtma.", "kosum", "E0006"),
                  ("Dört sabit üst sekme; ana ekran zarf listesi; hesap katmanı isteğe bağlı (→ 2.2, 5.1).",
                   "canli", "E0115, E0111"),
                  ("Ücretsiz pakette 10 zarf (Setup Budget: 8 of 10 free Envelopes left) ve 1 hesap sınırı; kart "
                   "hesabı ve transfer denenemedi. Zarflar ekranı son yedeğin zamanını gösteriyor (Last Backup); "
                   "dışa aktarma ve yedeğin kendisi incelenmedi.", "kosum", "E0006, E0109, E0114, E0115")),
            _kart("KolayBi", None,
                  {"k": "kb-kimlik", "e": "E0211", "etiket": "destek sayfasındaki Güncel Durum ekranı",
                   "kirpma": (96, 112, 1320, 872), "genislik": 377},
                  ("Web paneli: destek sayfası görselleri ve tanıtım videosu. Mobil uygulamada (3.3.1) yalnız giriş "
                   "ekranı; mobilde kayıt yok, web kaydı ücretli.", "kosum", "E0008"),
                  ("Bulut ön muhasebe ve e-dönüşüm; hedef kitlede şahıs şirketleri adıyla anılıyor.", "beyan",
                   "E0008"),
                  ("Solda sabit modül menüsü; formlarda ödeme durumu, vade ve proje; cari detayda ayrı borçlandırma "
                   "ve tahsilat eylemleri (→ 2.7, 4.9, 8.5).", "kaynak", "E0211, E0192, E0196"),
                  ("Görseller demo verisi taşıyor; güncel sürüm ve davranış doğrulanmadı. Aynı ekran tanıtım "
                   "videosunda farklı düzenle görünüyor (→ 2.7).", "kosum", "E0008, E0181")),
        ],
    },

    # ---------------------------------------------------------------- 1.2
    {
        "tur": "kartlar", "no": "1.2", "baslik": "Yalnız kaynaktan incelenen ürünler",
        "giris": "Dört ürünün içine girilemedi. KolayBi yalnız destek sayfası görselleriyle görülüyor. Bu ürünlerin "
                 "kaynaktan kurulan akış modeli Belge 2 Bölüm 9'dadır.",
        "dayanak": ["Koşum kaydı ve kaynak beyanı: E0011, E0009, E0012 (gözlem formları). Paraşüt tanıtım "
                    "videosu karesi: E0262 (basılmadı)"],
        "kartlar": [
            {"urun": "Paraşüt",
             "sekil": None, "alanlar": [
                ("İncelenen", "Android 5.25.0'ın giriş öncesi ekranları, kullanım kılavuzu ve tanıtım videosu. "
                 "Hesap açılamadı.", "kosum", "E0011"),
                ("Kaynaklarda belirtilen odak", "Bulut ön muhasebe ve e-dönüşüm; KOBİ ve e-ticaret.", "beyan",
                 "E0011"),
                ("Kaynağın anlattığı yaklaşım", "Ana ekran tahsilat ve ödemeyi vade durumuna göre ayırıyor; gider ve "
                 "fatura önce, ödeme sonra kaydediliyor (→ 2.7, 8.5).", "beyan", "E0011"),
                ("Tanıtım karesi", "Videodaki cari hesap ekranında menü kutuları etiketsiz; iki halka aynı tutarı "
                 "gösteriyor, uyarıdaki üçüncü tutar ikisiyle de uyuşmuyor. Böyle bir kare ürünün bir ekranını "
                 "değil, kendini nasıl anlattığını gösterir.", "kosum", "E0262"),
                ("Kapsam ve bilinmeyenler", "Mobilde kayıt yok; web kaydı şirket bilgisi istiyor. İç arayüzün hiçbir "
                 "ekranı görülmedi.", "kosum", "E0011"),
            ]},
            {"urun": "Logo İşbaşı",
             "sekil": None, "alanlar": [
                ("İncelenen", "Android 3.20.0'ın giriş ve kayıt ekranları, ürün sayfaları ve tanıtım videosu. "
                 "Ekran etiketleri İngilizce, içerik Türkçe.", "kosum", "E0009"),
                ("Kaynaklarda belirtilen odak", "Mobil fatura ve ön muhasebe; mikro işletme.", "beyan", "E0009"),
                ("Kayıt akışı", "Form şirket adı, sektör ve telefon istiyor; ardından SMS doğrulaması ve sözleşme "
                 "onayı geliyor. İncelenen kayıtta hesap hemen kullanıma açılmadı; firmaya özel hazırlanma, satış ve "
                 "hazırlık sürecine girdi. Bu tek bir kaydın gözlemidir; ürünün bütün kullanıcılarına genellenmez.",
                 "kosum", "E0009"),
                ("Kaynağın anlattığı yaklaşım", "Sesle fatura kesme ve mali müşavir portalı (→ 4.9, 10.5).",
                 "beyan", "E0009"),
            ]},
            {"urun": "QuickBooks Solopreneur",
             "sekil": None, "alanlar": [
                ("İncelenen", "Yalnız Intuit yardım merkezi. Mobil uygulamada kayıt yapıldı, başka bir paketin plan "
                 "ekranında durdu; Solopreneur ABD dışına kapalı.", "kosum", "E0012"),
                ("Kaynaklarda belirtilen odak", "ABD'de tek kişilik işletme; işlem başına Business / Personal "
                 "etiketi vergi formuna hizalı.", "beyan", "E0012"),
                ("Kaynağın anlattığı yaklaşım", "Kayıtlar ağırlıkla banka bağlantısından geliyor; kullanıcı türü ve "
                 "kategoriyi inceliyor; kurallarla otomatik etiketleme (→ 6.5, 10.5).", "beyan", "E0012"),
                ("Kapsam ve bilinmeyenler", "Solopreneur'ün hiçbir ekranı görülmedi; görülen dört kare başka bir "
                 "paketin kaydıdır.", "kosum", "E0012"),
            ]},
        ],
    },
]

EKSIKLER = [
    ("Paraşüt, Logo İşbaşı", "İç arayüz", "1.2", "Ücretli veya hazırlanmış hesap gerekir", "Önerilmez"),
    ("QuickBooks Solopreneur", "Solopreneur ekranı", "1.2", "ABD hesabı gerekir", "Önerilmez"),
]

# Basilmayan ama metinde kimligiyle anilan kareler: hash denetimine girer.
EK_KANITLAR = ["E0020", "E0034", "E0106", "E0109", "E0111", "E0114", "E0149", "E0151", "E0171", "E0177",
               "E0181", "E0192", "E0196", "E0233", "E0255", "E0262", "E0290", "E0294", "E0366", "E0453"]
