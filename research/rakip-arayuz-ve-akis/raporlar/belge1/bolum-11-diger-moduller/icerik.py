# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 11 · Diger moduller ve yardimci araclar — tek icerik kaynagi (plan §5)."""

NO = 11
BASLIK = "Diğer modüller ve yardımcı araçlar"
ANA_SORU = "Ürünler kayıt ve rapor dışında hangi modülleri sunuyor?"
GIRIS = [
    "Bu bölüm rakiplerin özellik yüzeyini haritalar: işletme modülleri ve günlük yardımcı araçlar. Bir modülün "
    "rakipte bulunması bir ihtiyaç olduğu anlamına gelmez; bu değerlendirme Belge 3'tedir.",
    "Tablodaki \"Var\", modülün girişinin ya da kaynaktaki anlatımının görüldüğünü söyler; modüllerin iç "
    "işleyişi denenmedi. Boş hücreler kanıt olmadığını gösterir, modülün bulunmadığını değil.",
]
GIRMEZ = [
    "Modüllerin iç işleyişi ve hesaplamaları",
    "Belge 3'ün alma kararı",
]
KAPSAM = [
    ("Money Manager", ["canli"], None),
    ("Bluecoins", ["canli"], None),
    ("Wallet", ["canli"], "Çekmece girişleri."),
    ("Hesap Defterim", ["canli"], "Not defteri, kupür aracı."),
    ("Goodbudget", ["yok"], "Ek modül görülmedi."),
    ("KolayBi", ["kaynak"], "İşletme modülleri."),
    ("Paraşüt", ["beyan"], None),
    ("Logo İşbaşı", ["beyan"], None),
    ("QuickBooks Solopreneur", ["beyan"], "Km takibi."),
]
ACILIS_DAYANAK = ("\"Var\" girişin veya anlatımın görüldüğünü söyler; \"Görülmedi\", modülün bulunmadığı anlamına "
                  "gelmez.")

URUNLER = ["Money Manager", "Bluecoins", "Wallet", "Hesap Defterim", "Goodbudget", "KolayBi", "Paraşüt",
           "Logo İşbaşı", "QuickBooks"]
SUTUNLAR = [("Modül", 1.55)] + [(u, 1.0) for u in URUNLER]


def _var(tur, d):
    return {"t": f"Var · {d}", "tur": tur, "d": d}


def _satir(modul, **hucreler):
    """hucreler: urun kisaltmasi -> (tur, dayanak). Verilmeyen urun 'gorulmedi'."""
    anahtar = ["mm", "bc", "wl", "hd", "gb", "kb", "ps", "lg", "qb"]
    satir = [modul]
    for a in anahtar:
        if a in hucreler:
            satir.append(_var(*hucreler[a]))
        else:
            satir.append({"t": " ", "tur": "yok", "d": "—"})
    return satir


SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 11.1
    {
        "tur": "tablo", "no": "11.1", "baslik": "İşletme modülleri",
        "giris": "Stok, personel, kıymetli evrak ve ürün kartı yalnız kaynakla incelenen ön muhasebe ürünlerinde "
                 "görüldü. Hücredeki kimlik, dayanağın kanıt kimliğidir.",
        "sutunlar": SUTUNLAR,
        "boy": 7.0,
        "satirlar": [
            _satir("Stok ve depo", kb=("kaynak", "E0218"), ps=("beyan", "E0011"), lg=("beyan", "E0009")),
            _satir("Personel ve maaş", kb=("kaynak", "E0201"), ps=("beyan", "E0011")),
            _satir("Çek ve senet", kb=("kaynak", "E0209"), ps=("beyan", "E0011"), lg=("beyan", "E0009")),
            _satir("Ürün ve hizmet kartı", kb=("kaynak", "E0218"), ps=("beyan", "E0011"), lg=("beyan", "E0009")),
        ],
        "notlar": [
            ("KolayBi'nin ürün sayfasında Tümü, Ürünler, Hizmetler, Depolar ve Varyantlar sekmeleri var; "
             "varyant ve depo işleyişi görülmedi.", "E0218"),
            ("KolayBi personel carilerinde çalışma tipi ve bakiye var; maaş ve prim oluşturma, ödeme ve avans "
             "girişleri ayrı (→ 6.4, 7.1).", "E0201, E0202, E0203"),
            ("KolayBi çek listesinde keşideci, hamil ve vade; senet listesinde kefil, ödenen ve kalan tutar "
             "kolonları var. İki tablo da boş.", "E0209, E0210"),
            ("Logo İşbaşı'nın anlatımında teklif, sipariş ve sesli fatura girişi de geçiyor.", "E0009"),
            ("Hesap Defterim'in Öğe eklemek diyaloğu kalemlerden tutar ve not üretiyor; bir ürün kataloğu "
             "değil.", "E0155"),
        ],
        "dayanak": ["Canlı kare: E0155. Kaynak görseli: E0201, E0202, E0203, E0209, E0210, E0218. "
                    "Kaynak beyanı: E0011, E0009"],
    },

    # ---------------------------------------------------------------- 11.2
    {
        "tur": "tablo", "no": "11.2", "baslik": "Yardımcı araçlar",
        "giris": "Günlük araçlar canlı incelenen ürünlerde, çoğunlukla çekmece veya ayar girişi olarak görüldü.",
        "sutunlar": SUTUNLAR,
        "boy": 7.0,
        "satirlar": [
            _satir("Not defteri", hd=("canli", "E0165"), kb=("kaynak", "E0212")),
            _satir("Takvim", mm=("canli", "E0228"), bc=("canli", "E0098"), hd=("canli", "E0144")),
            _satir("Kupür sayan hesap makinesi", hd=("canli", "E0166")),
            _satir("Alışveriş listesi", wl=("canli", "E0376")),
            _satir("Garanti", wl=("canli", "E0376")),
            _satir("Sadakat kartı", wl=("canli", "E0376")),
            _satir("Seyahat modu", bc=("canli", "E0100")),
            _satir("Kilometre takibi", qb=("beyan", "E0012")),
        ],
        "notlar": [
            ("Hesap Defterim'in Not Defteri tarih, onay kutusu, arama ve tamamlandı sayaçları taşıyor; finansal "
             "bir vade değil (→ 7.1).", "E0165"),
            ("KolayBi notlarında hatırlatıcı ve özel/şirket notu seçimi var; bildirim denenmedi.", "E0212"),
            ("Money Manager'ın takvimi kayıt listesinin sekmelerinden biri.", "E0228"),
            ("Wallet kayıt ayrıntısında Warranty alanı da var. Shopping lists'te bir liste ve Share list; "
             "Warranties ve Loyalty cards boş, ikisi de ilk kaydı çağırıyor.", "E0294, E0480, E0479, E0481"),
            ("Wallet çekmecesinde Investments ve Currency rates girişleri de var; Investments'ın görüldüğü kare "
             "hesap sahibinin adını taşıdığı için basılmadı.", "E0375, E0376"),
            ("Money Manager'ın CalcBox ve PC'den Yönet girişleri uygulama içi modül değil: CalcBox başka bir "
             "ürünün mağaza sayfasını açıyor, PC'den Yönet ücretli sürüm ekranına gidiyor.", "E0254, E0455, E0456"),
            ("Bluecoins'in seyahat modu anahtarına dokununca bir etiket seçici açılıyor; seçim yapılmadığı için "
             "modun ne yaptığı görülmedi. Gelişmiş ayarlarda döviz kuru tercihleri var.", "E0100, E0454, E0095"),
        ],
        "dayanak": ["Canlı kare: E0095, E0098, E0100, E0144, E0165, E0166, E0228, E0254, E0294, E0376. "
                    "Kaynak görseli: E0212. Kaynak beyanı: E0012"],
    },
]

EKSIKLER = [
    ("Bluecoins", "Seyahat modunun etkisi", "11.2", "Bir etiket seçip listeye bakmak", "Orta"),
    ("Paraşüt / Logo İşbaşı", "Stok ve çek ekranları", "11.1", "Ücretli hesap gerekir", "Önerilmez"),
    ("QuickBooks Solopreneur", "Km takibi ekranı", "11.2", "ABD hesabı gerekir", "Önerilmez"),
]
# Bolumde kare basilmiyor; tabloda anilan kanitlar hash denetimine yine girer.
EK_KANITLAR = ["E0095", "E0098", "E0100", "E0144", "E0155", "E0165", "E0166", "E0201", "E0202", "E0203",
               "E0209", "E0210", "E0212", "E0218", "E0228", "E0254", "E0294", "E0376", "E0454", "E0455",
               "E0456", "E0479", "E0480", "E0481"]
