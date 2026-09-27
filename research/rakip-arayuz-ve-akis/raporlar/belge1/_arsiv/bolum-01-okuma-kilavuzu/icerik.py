# -*- coding: utf-8 -*-
"""Belge 1 · Bolum 1 · Okuma kilavuzu ve kanit duzeyleri — tek icerik kaynagi (plan §5)."""

NO = 1
BASLIK = "Okuma kılavuzu ve kanıt düzeyleri"
ANA_SORU = "Bu belge neyi, hangi kanıtla anlatıyor ve nasıl okunur?"
GIRIS = [
    "Belge 1, dokuz rakip ürünün arayüz yaklaşımlarını konu konu anlatır: ana ekran, kayıt formu, hesap ve kart, "
    "sınıflandırma, planlama, borç, rapor, veri aktarımı ve yardımcı araçlar. Ürünleri puanlamaz ve "
    "sıralamaz; her yaklaşımı ekranda görüldüğü gibi tarif eder.",
    "Bir kaydın bakiyeye ve rapora etkisi Belge 2'nin, bu bulguların BusinessFinance için ne anlama geldiği "
    "Belge 3'ün konusudur. Bu belgede \"→ Belge 2\" yazan yerler o sınırı gösterir.",
    "Canlı incelemeler 1–12 Eylül 2026 arasında Android emülatörde yapıldı; kullanıcı kontrolleri 14–15 Eylül'de "
    "eklendi. Test verisi sentetiktir: aynı senaryo (Ada Reklam geliri, market gideri, kart harcaması ve "
    "ödemesi, hesaplar arası aktarım, abonelik, taksit, alacak ve kısmi tahsilat) her ürüne, ürünün izin verdiği ölçüde girildi.",
]
GIRMEZ = [
    "Ürünlerin başarısı, hızı, memnuniyeti, erişilebilirliği",
    "Kaydın finansal sonucu → Belge 2",
    "BusinessFinance için tercih → Belge 3",
]
KAPSAM = [
    ("Money Manager", ["canli", "kosum"], None),
    ("Bluecoins", ["canli", "kosum"], None),
    ("Wallet", ["canli", "kosum"], None),
    ("Hesap Defterim", ["canli", "kosum"], None),
    ("Goodbudget", ["canli", "kosum"], None),
    ("KolayBi", ["kaynak", "beyan", "kosum"], None),
    ("Paraşüt", ["beyan", "kosum"], "Kareleri basılmıyor."),
    ("Logo İşbaşı", ["beyan", "kosum"], "Kareleri basılmıyor."),
    ("QuickBooks Solopreneur", ["beyan"], "Kareleri basılmıyor."),
]
ACILIS_DAYANAK = "Kanıt türlerinin anahtarı 1.2'dedir. Ürünlerin tek tek anlatımı Bölüm 2'dedir."

SAYFALAR = [
    {"tur": "acilis"},

    # ---------------------------------------------------------------- 1.1
    {
        "tur": "tablo", "no": "1.1", "baslik": "Ne incelendi?",
        "giris": "Beş ürün emülatörde canlı açıldı; dört ürün yalnız kendi yayımladıkları kaynaklardan incelendi. "
                 "Kare sayıları kanıt envanterindendir.",
        "sutunlar": [("Ürün", 1.3), ("Platform ve sürüm", 2.2), ("Erişim", 2.4), ("Tarih", 1.2),
                     ("Kare", 0.6), ("Belgede basılan", 1.6)],
        "boy": 7.3,
        "satirlar": [
            "Canlı incelenen ürünler",
            ["Money Manager", "Android 4.12.8, Türkçe arayüz", "Canlı; kayıt ve giriş yok", "1, 10 Eylül",
             "30", "Kareler"],
            ["Bluecoins", "Android 13.1.45, ağırlıkla Türkçe", "Canlı, derin; kayıt ve giriş yok",
             "1, 10, 11 Eylül", "92", "Kareler"],
            ["Wallet (BudgetBakers)", "Android 9.3.6, İngilizce arayüz", "Canlı, derin; bulut hesabıyla",
             "1, 10, 11 Eylül", "109", "Kareler; hesap sahibinin adını taşıyan kare hariç"],
            ["Hesap Defterim", "Android, Türkçe arayüz", "Canlı; kayıt ve giriş yok", "10–12 Eylül", "45",
             "Kareler"],
            ["Goodbudget", "Android 2.24.26013, İngilizce arayüz", "Canlı; ücretsiz paket sınırlı",
             "11–12 Eylül", "31", "Kareler; hane adı karartılarak"],
            "Kaynakla incelenen ürünler",
            ["KolayBi", "Web paneli; mobilde yalnız giriş ekranı", "Destek sayfası görselleri ve tanıtım videosu",
             "1–12 Eylül", "39", "Yalnız 31 destek görseli"],
            ["Paraşüt", "Android giriş ekranları; kılavuz", "Hesap açılamadı; kılavuz ve tanıtım videosu",
             "1–12 Eylül", "9", "Hiçbiri"],
            ["Logo İşbaşı", "Android giriş ve kayıt ekranları", "Ürün sayfaları ve tanıtım videosu",
             "1–12 Eylül", "6", "Hiçbiri"],
            ["QuickBooks Solopreneur", "—", "Yalnız yardım merkezi; ürün ABD dışına kapalı", "1–12 Eylül", "4",
             "Hiçbiri"],
        ],
        "notlar": [
            "Toplam 365 kare incelendi ve envantere işlendi. Kullanıcı kontrolüyle eklenen kareler de bu sayıya "
            "dahildir.",
            "Sürüm ve dil ayrıntısı ile her ürünün bilinmeyenleri Bölüm 2'deki kartlardadır.",
        ],
        "dayanak": ["Koşum kaydı: E0010, E0005, E0014, E0007, E0006, E0008, E0011, E0009, E0012 (gözlem formları)"],
    },

    # ---------------------------------------------------------------- 1.2
    {
        "tur": "metin", "no": "1.2", "baslik": "Kanıt türleri",
        "giris": "Her cümlenin yanında ya da sayfanın altındaki dayanak bandında beş türden biri yazar. Tür ürüne "
                 "değil ifadeye bağlıdır: canlı incelenen bir ürün hakkındaki her cümle sınanmış değildir.",
        "sutun": 3,
        "bloklar": [
            {"baslik": "Canlı kare", "tur": "canli", "d": "—", "metin": [
                "Emülatörde açılan ekranın görüntüsü. Belgede basılır; üstündeki numaralı işaretler metinle "
                "eşleşir."]},
            {"baslik": "Koşum kaydı", "tur": "kosum", "d": "—", "metin": [
                "Gözlem formunda yazılı; karesi yok ya da kare tek başına göstermiyor. Kullanıcının sonradan "
                "yaptığı ekran kontrolleri de bu türdedir."]},
            {"baslik": "Kaynak görseli", "tur": "kaynak", "d": "—", "metin": [
                "Ürünün kendi arayüzünü anlatmak için yayımladığı ekran görüntüsü. Bu belgede yalnız KolayBi'nin "
                "destek sayfası görselleri. Demo verisi taşır; güncel sürümü gösterdiği doğrulanmadı."]},
            {"baslik": "Kaynak beyanı", "tur": "beyan", "d": "—", "metin": [
                "Ürünün kılavuzu, yardım merkezi veya ürün sayfası metni. Görsel yok; anlatılan davranış "
                "çalışırken görülmedi."]},
            {"baslik": "Görülmedi", "tur": "yok", "d": "—", "metin": [
                "Kanıt yok. Özelliğin bulunmadığı anlamına gelmez: yalnız incelenen sürümde, incelenen yüzeyde "
                "ve incelenen kaynakta görülmediğini söyler."]},
            {"baslik": "\"Görülmedi\" ile \"yok\" ayrıdır", "metin": [
                "\"Yok\" ancak karede ya da koşumda yokluk gözlendiyse ve kapsamıyla birlikte yazılır: \"incelenen "
                "form ve kayıt ayrıntısında taksit alanı yok\" gibi.",
                "\"Her ekranda\", \"tek ürün\", \"hiçbirinde\" gibi nicelikler yalnız o kapsamı taşıyan kanıt "
                "gösterildiğinde kullanılır; aksi hâlde cümle gözlenen kapsamla sınırlanır."]},
        ],
    },

    # ---------------------------------------------------------------- 1.3
    {
        "tur": "metin", "no": "1.3", "baslik": "Neden bazı kareler basılmıyor?",
        "giris": "İncelenen her kare belgeye girmez. Basılmayan bir kareye dayanan cümle kaynak niteliğini "
                 "kaybetmez; kimliği dayanak bandında ve bölümün kaynaklar dosyasında kalır.",
        "dayanak": ["Koşum kaydı: E0011, E0009, E0012, E0008. Paraşüt tanıtım videosu karesi: E0262 (basılmadı)"],
        "sutun": 3,
        "bloklar": [
            {"baslik": "Paraşüt ve Logo İşbaşı", "metin": [
                "Paraşüt'ün 9 karesinin 2'si, Logo İşbaşı'nın 6 karesinin 4'ü canlı yakalanmış giriş ve kayıt "
                "ekranıdır; geri kalanlar tanıtım videosu karesidir.",
                "Hiçbiri ürünün iç arayüzünü temsil etmiyor. Bu yüzden kareler basılmıyor; gözlemler ve kaynak "
                "beyanları metinde kalıyor."]},
            {"baslik": "Bir tanıtım karesi ne kadar uzaktır?", "metin": [
                ("Paraşüt'ün tanıtım videosundaki cari hesap ekranında sol menü kutuları etiketsiz çizilmiş; iki "
                 "halka aynı tutarı gösteriyor, uyarı kutusundaki üçüncü tutar ikisiyle de uyuşmuyor ve tutarların "
                 "yazımı geçerli bir Türkçe biçime uymuyor.", "kosum", "E0262"),
                "Böyle bir kare, ürünün bir ekranı olduğunu değil, ürünün kendini nasıl anlattığını gösterir."]},
            {"baslik": "QuickBooks Solopreneur", "metin": [
                "Görülen 4 kare başka bir paketin kayıt ve plan ekranıdır. Solopreneur'ün hiçbir ekranı görülmedi; "
                "anlatım yalnız yardım merkezine dayanır."]},
            {"baslik": "KolayBi", "metin": [
                "Destek sayfası görselleri basılır. Tanıtım videosu kareleri basılmaz, gerektiğinde anılır: aynı "
                "ekranın kaynaklarda iki farklı hâlde görünmesi, güncel sürümün doğrulanmadığını gösterir."]},
            {"baslik": "Kişisel veri", "metin": [
                "Goodbudget'ın hane adını taşıyan kareleri karartılarak basılır. Wallet'ın çekmecesinin üst kısmı "
                "hesap sahibinin adını taşıdığı için hiç basılmaz; yerine çekmecenin alt kısmı kullanılır.",
                "KolayBi destek görsellerindeki demo kişi adları ve iletişim bilgileri, içerik için gerekmediğinde "
                "bölüm kopyasında karartılır. Karartma yalnız kopyaya uygulanır; özgün kanıt değişmez."]},
        ],
    },

    # ---------------------------------------------------------------- 1.4
    {
        "tur": "metin", "no": "1.4", "baslik": "Sayfalar nasıl okunur ve belge neyi ölçmez?",
        "sutun": 3,
        "bloklar": [
            {"baslik": "Ekran ve işaret", "metin": [
                "Soru sayfalarında kare büyük basılır ve üstüne numaralı işaretler konur. Sağ sütundaki numaralı "
                "satırlar o işaretleri açıklar. İşaret gösterdiği şeyin üstüne değil, yanındaki boş alana konur.",
                "Kareler aynı anın görüntüsü değildir; farklı ürünlerin sayıları birbiriyle karşılaştırılmaz."]},
            {"baslik": "Notlar ve diğer ürünler", "metin": [
                "Numaralı satırların altındaki madde işaretli notlar, karede okunmayan bilgiyi ya da kareden çıkan sonucun "
                "sınırını yazar. \"Aynı soruda diğer ürünler\" listesi, karesi basılmayan ürünlerin aynı sorudaki durumunu "
                "kanıt türüyle verir."]},
            {"baslik": "Dayanak bandı", "metin": [
                "Her sayfanın altında sayfanın kanıt kimlikleri ve \"çıkarılmayan sonuç\" yazar: sayfadaki "
                "gözlemlerden çıkarılmaması gereken sonuç."]},
            {"baslik": "Göndermeler", "metin": [
                "\"→ 8.4\" başka bir bölümün alt sorusuna gönderir. Bir konunun ayrıntısı tek bir bölümdedir; "
                "öteki bölümler kısa bağlam verip oraya gönderir.",
                "Ekranda görünen etiketler ürünün kendi dilinde ve yazımıyla aktarılır (ENVELOPES, Alındı, "
                "Hızlı İşlemler)."]},
            {"baslik": "Bu belge neyi ölçmez?", "metin": [
                "Başarıyı, hızı, memnuniyeti ve erişilebilirliği ölçmez; kullanılabilirlik testi değildir.",
                "Ölçülmemiş bir etki için cümle kurulmaz. Ürünlerin iyi veya kötü olduğuna dair bir sıralama "
                "yapılmaz."]},
            {"baslik": "Sıradaki belgeler", "metin": [
                "Belge 2 aynı akışların arkasındaki olay modelini ve toplamların içeriğini anlatır. Belge 3, iki "
                "belgenin bulgularını BusinessFinance için alma, uyarlama veya yeniden sorma kararlarına çevirir."]},
        ],
    },
]

CIKARILMAYAN = [
    "Kanıt türünün ürünün kalitesini gösterdiği",
]
EKSIKLER = []
EK_KANITLAR = ["E0262"]
