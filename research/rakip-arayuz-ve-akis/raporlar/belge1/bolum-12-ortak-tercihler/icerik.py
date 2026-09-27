# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 12 · Ortak tercihler ve ayrismalar — tek icerik kaynagi (plan §5).

Yeni kanit yok; onceki bolumlerin capraz okunmasi. Her satir kaynak bolumun alt sorusuna
gonderir; kanit kimlikleri o bolumdedir.
"""

NO = 12
BASLIK = "Ortak tercihler ve ayrışmalar"
ANA_SORU = "Önceki bölümler birlikte okunduğunda ürünler nerede birleşiyor, nerede ayrılıyor?"
GIRIS = [
    "Bu bölüm yeni kanıt taşımaz; Bölüm 2–11'in gözlemlerini üç başlıkta yan yana koyar: çoğunda aynı "
    "olanlar, ikiye ayrılanlar ve incelenen beş canlı üründen yalnız birinde görülenler.",
    "Her satır kaynağı olan alt soruya gönderir; kareler ve sınırlar oradadır. Satırlar bir değerlendirme ya da "
    "sıralama değildir. Bir tercihin çoğunlukta olması doğru olduğunu, tek üründe olması ayırt edici bir avantaj "
    "olduğunu göstermez.",
]
GIRMEZ = [
    "Hangi tercihin daha iyi olduğu",
    "Alma, uyarlama ya da reddetme kararı → Belge 3",
    "Yeni gözlem",
]
KAPSAM = None
ACILIS_DAYANAK = ("Nicelikler incelenen yüzeyle sınırlıdır: \"yalnız birinde\", incelenen beş canlı üründen "
                  "yalnız birinde görüldü demektir.")

SUTUNLAR = [("Gözlem", 1.6), ("Ayrıntı", 3.4), ("Ürünler", 2.0), ("Bak", 0.6)]


TEK_SUTUNLAR = [("Gözlem", 1.6), ("Ayrıntı", 5.0), ("Bak", 0.8)]


def _t(baslik, ayrinti, bak, tur="canli"):
    return [baslik, {"t": ayrinti, "tur": tur, "d": f"→ {bak}"}, bak]


def _s(baslik, ayrinti, urunler, bak, tur="canli"):
    return [baslik, {"t": ayrinti, "tur": tur, "d": f"→ {bak}"}, urunler, bak]


SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 12.1
    {
        "tur": "tablo", "no": "12.1", "baslik": "Çoğunda aynı olanlar",
        "giris": "Aşağıdaki tercihler incelenen canlı ürünlerin en az dördünde görüldü.",
        "sutunlar": SUTUNLAR,
        "boy": 7.3,
        "satirlar": [
            _s("Kayıt düğmesi", "Kayıt sağ altta tek bir düğmeyle başlıyor; yön sonra formda seçiliyor.",
               "Money Manager, Bluecoins, Wallet, Goodbudget. Ayrı: Hesap Defterim'de iki yön düğmesi.", "2.5"),
            _s("Gelir ve gider rengi", "Gelir ve gider renkle ayrılıyor; renklerin kendisi ürüne göre değişiyor.",
               "Beş canlı ürün.", "3.2"),
            _s("Seçili dönem", "Dönem ekranda görünür bir denetimle duruyor: çip, ay gezgini, aralık çipi ya da "
               "başlıkta yazılı aralık.", "Beş canlı ürün.", "3.4, 9.3"),
            _s("Transfer", "Transfer ayrı bir form ya da ayrı bir tür.", "Beş canlı ürün.", "5.6"),
            _s("Tutar girişi", "Tutar bir hesap makinesiyle girilebiliyor: tuş takımı, alandaki ikon ya da bir "
               "ayar.", "Beş canlı ürün (Goodbudget'ta ayarla).", "4.4"),
            _s("Silme onayı", "Kayıt silinirken bir onay diyaloğu çıkıyor.",
               "Beş canlı ürün (Hesap Defterim'de kalıcı silme adımında).", "4.8"),
            _s("İşletme/şahsi alanı", "Kayıt düzeyinde bir işletme/şahsi alanı görülmedi.",
               "Beş canlı ürün; KolayBi, Paraşüt, Logo İşbaşı kaynaklarında da. Ayrı: QuickBooks Solopreneur'ün "
               "yardım merkezi.", "6.5", tur="yok"),
        ],
        "notlar": [
            "\"Beş canlı ürün\" Money Manager, Bluecoins, Wallet, Hesap Defterim ve Goodbudget'tır.",
            "Kaynakla incelenen ürünler bu tabloda yalnız kanıtları o soruyu kapsadığında anılır.",
        ],
    },

    # ---------------------------------------------------------------- 12.2
    {
        "tur": "tablo", "no": "12.2", "baslik": "İkiye ayrılanlar",
        "giris": "Aynı soruya ürünlerin iki ya da üç farklı cevap verdiği yerler.",
        "sutunlar": SUTUNLAR,
        "boy": 7.1,
        "satirlar": [
            _s("Bölüm seçici", "Sabit sekme (altta ya da üstte) · çekmece.",
               "Sekme: Money Manager, Goodbudget · Çekmece: Wallet, Hesap Defterim · İkisi: Bluecoins.", "2.3"),
            _s("Kredi kartı", "Ekstre dönemli kart · dönemsiz, eksi bakiyeli hesap.",
               "Dönemli: Money Manager; dönem alanları: Bluecoins · Dönemsiz: Wallet; defter: Hesap Defterim.",
               "5.3, 5.4"),
            _s("Transfer listede", "Tek satır, iki hesap okla · iki satır, her hesap kendi satırında.",
               "Tek: Money Manager · İki: Bluecoins, Wallet, Hesap Defterim.", "5.6"),
            _s("Seçili tür", "Dolu renk ya da çerçeve · yalnız zemin tonu · açılır listede ad.",
               "Money Manager, Bluecoins, Hesap Defterim · Wallet · Goodbudget.", "4.3"),
            _s("Kaydettikten sonra", "Sessizce listeye dönüş · kısa bildirim ya da tebrik mesajı.",
               "Sessiz: Money Manager, Bluecoins, Wallet · Mesaj: Hesap Defterim, Goodbudget.", "4.6",
               tur="kosum"),
            _s("Hata mesajı", "Ekranın altında · alanın hemen altında kırmızı metin. Wallet iki formda ikisini "
               "de kullanıyor.", "Alt: Money Manager, Wallet hızlı form, Goodbudget · Alan: Wallet planlı ödeme.",
               "4.7"),
            _s("Kategori", "Seçici panel ya da liste · seçici yok.",
               "Seçici: Money Manager, Bluecoins, Wallet · Serbest metin: Hesap Defterim · Zarf: Goodbudget.",
               "6.1"),
            _s("Tekrar kurulumu", "Normal formun içinde · ayrı planlı ödeme formu.",
               "Form içi: Money Manager, Bluecoins, Goodbudget · Ayrı form: Wallet.", "7.1"),
            _s("Otomatik kayıt tercihi", "Kurulum formunda bir kutu · ilk onaydan sonra sorulan soru · genel "
               "bir ayar.", "Kutu: Bluecoins · Soru: Wallet · Ayar: Money Manager.", "7.5"),
            _s("Silineni geri almak", "Ayrı bir liste ya da çöp kutusu · incelenen menü ve ayarlarda böyle bir "
               "kalem yok.", "Liste: Hesap Defterim, Bluecoins · Yok: Money Manager, Wallet, Goodbudget.", "4.8"),
            _s("Alacak kaydı", "Normal formda cari hesaba gelir · ayrı borç kartı.",
               "Form: Bluecoins · Kart: Wallet.", "8.2"),
            _s("Kalanın yeri", "Hesabın bakiyesi · borç kartının üstü.",
               "Hesap: Bluecoins · Kart: Wallet.", "8.4"),
            _s("Boş durum", "İlk eylemi adıyla çağıran · yalnız veri olmadığını söyleyen.",
               "Çağıran: Bluecoins temiz kurulum, Wallet bölüm içi · Söyleyen: Money Manager, Goodbudget raporu.",
               "2.6, 3.5"),
        ],
    },

    # ---------------------------------------------------------------- 12.3
    {
        "tur": "tablo", "no": "12.3", "baslik": "Yalnız bir üründe görülenler",
        "giris": "İncelenen beş canlı üründen yalnız birinde görülen yaklaşımlar. Başka üründe görülmemesi, orada "
                 "bulunmadığı anlamına gelmez.",
        "sutunlar": TEK_SUTUNLAR,
        "boy": 7.3,
        "satirlar": [
            "Hesap Defterim",
            _t("Yön adlı düğmeler", "Kayıt, Alındı ve Ödendi adlı iki düğmeyle başlıyor; adlar kullanıcı "
               "tarafından değiştirilebiliyor.", "2.5, 4.5"),
            _t("Sabit toplam bandı", "Dönem toplamları ekranın altında sabit bir bantta.", "2.2, 9.2"),
            _t("Grafiksiz rapor", "Rapor grafik yerine tablo ve takvimle veriliyor.", "9.1"),
            _t("Kupür sayan araç", "Kupür × adet satırlarıyla nakit sayan hesap makinesi.", "11.2"),
            "Money Manager",
            _t("Kart borcu iki sütunda", "Kart borcu Bu Ay ve Gelecek Ay sütunlarında.", "5.4"),
            _t("Gider ödeme yöntemine göre", "Toplam sekmesinde nakit-banka gideri ile kart gideri ayrı satırda.",
               "9.1"),
            _t("Tekrarın uygulanma ayarı", "Tekrarlı kayıtların ne zaman uygulanacağı tek bir ayar: Tarihte ya "
               "da Her ayın ilk günü; örnek başına onay yok.", "7.5"),
            "Bluecoins",
            _t("Taksit oranı ve özet", "Taksit formunda oran alanı; kaydetmeden önce tek cümlelik özet.", "7.2"),
            _t("Seyahat modu", "Çekmecede seyahat modu anahtarı.", "11.2"),
        ],
    },
    {
        "tur": "tablo", "no": "12.3", "baslik": "Yalnız bir üründe görülenler (devam)", "haritada": False,
        "sutunlar": TEK_SUTUNLAR,
        "boy": 7.3,
        "satirlar": [
            "Wallet",
            _t("Ana ekranda bekleyen", "Ana ekranda bekleyen ödeme kartı ve kısayolu.", "7.4"),
            _t("Borca bağlı tahsilat", "Tahsilat belirli bir borç kartına bağlanıyor; aynı ada ait borçlar ayrı "
               "kart.", "8.3, 8.4"),
            _t("Soru başlıklı rapor", "Rapor kartının başlığı bir soru.", "3.3, 9.2"),
            "Goodbudget",
            _t("Ana ekran bütçe", "Ana ekran zarf listesi; hesap katmanı isteğe bağlı.", "2.2, 5.1"),
            _t("Tebrik mesajı", "Kayıttan sonra oyunlaştırılmış bir mesaj.", "4.6", tur="kosum"),
        ],
        "notlar": [
            "Kaynakla incelenen ürünlerde de tek üründe görülen yüzeyler var (proje ekseni, cari ekstre, KDV "
            "raporu); bunlar canlı ürünlerle aynı kanıt düzeyinde olmadığı için bu tabloya alınmadı (→ 6.3, 8.5, "
            "9.6).",
        ],
    },
]

EKSIKLER = []
