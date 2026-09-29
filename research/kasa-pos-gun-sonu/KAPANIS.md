# Kasa ve para yolları — kapanış listesi

**Durum: KAPANDI (28 Eylül 2026, kullanıcı: "bu kısım tamam"); 29 Eylül karar denetimiyle güncellendi**
(`research/DENETIM-2026-09-29.md` ve dış kaynak eki; kullanıcı: "en uygun çözümleri uygulayabilirsin").
Bu liste temanın **geçerli** kararlarıdır; `KARAR.md`, `para-akisi/HARITA.md` §7, `GUN-SONU-BELGELERI.md` ve
denetim belgeleri gerekçe için okunur. `research/` belgeleri arasında çelişki
olursa bu liste geçerlidir; ADR 0019 ya da aşama belgesiyle çelişirse onlar
geçerlidir ve çelişki kullanıcıya sorulur.
**Statü (29 Eylül):** bağlayıcı ilkeler ADR 0019'un "İlkeler" bölümündedir; bu
listenin ayrıntıları **başlangıç tasarımıdır** ve `AGENTS.md` "Kararlardan sapma"
kuralıyla değişebilir (sapma `stages/06.3-butunsel-duzenleme.md` "Sapmalar"
tablosuna yazılır). **Z okumanın ayrıntıları (KP4, KP7'nin
varsayılanları) kod yazılırken gerçek Z örnekleriyle yeniden incelenir** (kullanıcı, 29 Eylül).

| # | Karar | Kaynak |
|---|---|---|
| KP1 | Kasa = dükkânın nakdi; POS = kart/banka tarafı. Satışlar tek tek değil **gün sonu toplamıyla** girilir; tek tek giriş de mümkün kalır | Kullanıcı |
| KP2 | **Gün sonu paneli:** nakit satış + her POS için kartlı satış (yemek kartı kendi cihazının gün sonu tutarıyla, kendi POS satırında). Elle ya da Z raporu fotoğrafından. **Elle girişte nakit, kart ve toplamdan ikisi yeterli; üçüncüsü hesaplanır.** Kapılar: "+" menüsü (işletme profilinde "POS tahsilatı"nın yerine, H1) ve Kasa'daki "Gün sonunu gir" | H1, K1; kullanıcı 29 Eyl |
| KP3 | Gün sonu **ayrı bir kayıt değil**: var olan gelir (nakit, kasaya) ve POS kayıtlarını üretir; bağlayıcı kimlik taşır. **Tekrar kontrolü Z no ile**; Z no'suz elle girişte gün başına bir gün sonu, aynı güne ikincisi yalnız açıkça "ek gün sonu" (ikinci cihaz) olarak ve "bu gün kapatıldı" uyarısıyla. **Düzeltme:** gün sonu bir bütün olarak geri alınır, ürettiği kayıtlar birlikte iptal olur; tek tek kayıtları köken kilidiyle korunur | K3; denetim B12, Y1 |
| KP4 | **Z okuma:** okunur — tarih ya da aralık, Z no, NAKİT, KART, TOP. Okunmaz — KÜMSAT, KDV, BRÜT, iptal satırı. Basılmayan satır = 0. **Kayıtlar NAKİT ve KART'tan yazılır; TOP kayıt üretmez, açıklar:** NAKİT + KART ile TOP farkı gösterilir ("genelde faturalı satış ya da veresiye tahsilatıdır"). Z'de faturalı satış, cari hesap tahsilatı, yemek kartı ve iptal için ayrı satırlar var (GİB kılavuzu); bunlar P7'de gerçek örnekle ele alınır. Birden çok POS tanımı varsa kartlı tutar bölüştürülür (varsayılan ana POS) | GÜN-SONU §5–7; dış kaynak §1, §2.5 |
| KP5 | **Toplu Z** (birkaç gün): okunan aralık gösterilir, toplam son güne yazılır, kullanıcı değiştirir; **aralıktaki bütün günler "kapalı" sayılır**; aralık ay dönümünü geçerse uyarı verilir, tutar iki parçaya bölünebilir | K1a; denetim B11 |
| KP6 | **Z numarası** yalnız fotoğraftan okunur; elle girişte sorulmaz. Atlanan Z uyarısı yalnız Z no'lu günler arasında; elle kapatılmış gün "kapalı" sayılır; hiç Z no'su olmayan kullanıcı kontrolü görmez. Uyarı metni kaydı denetler ("Z 3142 girilmedi"), esnafın cihazını değil | K1b |
| KP7 | **Zaten girilmiş kayıtlar:** gün sonu paneli o günün tek tek girilmiş **satışlarını** (gelir ve POS; faturalı dahil), **kartla tahsilatları** (KP13) ve **nakit cari tahsilatları** listeler, "gün sonu tutarında var mı?" diye sorar. **Varsayılan:** satışlar ve kartla tahsilatlar işaretli (düşülür), nakit cari tahsilat işaretsiz (çoğu zaman yazar kasadan geçmez). Yazılan nakit satış = nakit − işaretli nakit kayıtlar; kart satış = kart − işaretli kartlı kayıtlar. Kart toplamdan hesaplandığında (KP2) düşme kuralı P5'te yeniden incelenir | H3, F24; denetim B1, B9; dış kaynak §1, §2.1 |
| KP8 | **Aylık toplu giriş** (mali hafıza raporu + banka dökümü): kabul, **sonraya** (belge örnekleri yok) | K1c |
| KP9 | **Kasa sayımı gün sonundan ayrılır:** Kasa'da bağımsız "Kasayı say". **Beklenen = kasanın hesap bakiyesi** (tek gerçek); kaydedilmemiş son fark ayrı bir satırda görünür ve sonraki sayımda yeniden sorulmaz. Sıklığı kullanıcı seçer; fark kaydı isteğe bağlı, sebep sorulur (girilmemiş gider · kendime aldım · bilmiyorum). **Geliştirirken yeniden düşünülecek** | F23; denetim B8 |
| KP10 | **POS tanımı ("POS'larım"):** ad, geçeceği hesap, oran, komisyon kategorisi, geçiş gün sayısı, iş günü seçeneği. Yemek kartı ayrı özellik değil, bir POS tanımı. Form tanımdan dolar; canlı net görünür | K1, D5 |
| KP11 | **Yatış:** yoldaki POS'lar toplu seçilip "Hesaba geçenleri işaretle"; gerçek yatan tutar yazılır; fark **kesinti** olarak POS tanımının komisyon kategorisine gider yazılır ve **İşlemler'de yatışın detayında** görünür; yatış geri alınabilir; POS kaydı iptal edilebilir — **yatışa bağlanmış POS kaydı, önce yatış geri alınmadan iptal edilemez** | K1, K4, D3; denetim B12 |
| KP12 | Eski "hesaba geçti" kayıtları yeni yatış yapısına taşınır (test verisi) | K6 |
| KP13 | **Kartla tahsil:** cari tahsilatta **ve tek seferlik alacağın (yükümlülük) kapatılmasında** "kartla (POS)" ödeme yolu; para yolda görünür, gelir ikinci kez yazılmaz, bankaya geçişi KP11 ile işaretlenir. **Yoldaki para iki kaynaktan beslenir** (POS satışı + kartla tahsilat); net varlık ikisini de içerir; kesintinin kapsamı POS tanımından (varsayılan İşletme). Gün sonunda düşülmesi KP7 | H2; denetim B15, B18 |
| KP14 | **Kasa sekmesi:** Bugün (satış, gün sonu durumu) · Nakit ("Kasayı say", Kasadan öde, Bankaya yatır, Kendime aldım) · Kartla gelecek · Son günler. Adı "Kasa". Ayrıntı Claude Design'da | K2 |
| KP15 | Kasa'da **işletme kapsamlı ve kapsamsız** nakit hesaplar; yalnız şahsi etiketliler (şahsi cüzdan) gizlenir | H5, D4; denetim B13 |
| KP16 | "Kendime aldım": her seferinde sorulur (şahsi hesaba transfer · şahsi gider), varsayılan transfer; **şahsi hesap yoksa varsayılan şahsi gider**, "şahsi cüzdan aç" seçeneğiyle | H4; denetim B19 |
| KP17 | "POS" kelimesi yalnız satış tarafında; esnafın kendi kartıyla ödediği yerde "kartla ödedim" | H6 |
| KP18 | Beklenen POS tahsilatı **Yaklaşanlar'da** görünür ("girecek" satırıyla; "7 günde çıkacak" toplamına girmez) | U7 |
| KP19 | Kapsam dışı bu tur: banka sayımı · CSV (ayrı tur) · dekont kapısı · gün sonunda KDV · kart türüne göre oran · ayrı BSMV alanı · tatil takvimi · satış tarafında taksit · banka entegrasyonu | D2, D7, F10 |
| KP20 | **Elle kasa defteri tutan esnaf:** hiçbir akış fotoğrafa ya da POS'a bağlı değil; gün sonu elle, Z no'suz çalışır | GÜN-SONU §6 |
| KP21 | Hatalar (tema dışı, P1): **hesap bakiyesini değiştiren her olayın** Kasa'yı yenilemesi (U11; bugün cari tahsilat, yükümlülük kapatma, tekrarlayan gerçekleşme, borç taksidi ve içe aktarım da yenilemiyor), POS'un iptal edilememesi (U12), sayımdan sonra değişen kasanın "oturdu" demesi (U10), **içe aktarımda çifte sayım uyarısı** ("POS yatışı, kart ödemesi ya da transfer olabilir" + satırı atla; U8'in asıl çözümü F05) | Bulgular; denetim B14, B17 |
| KP22 | Kayıtta isteğe bağlı **Z no** (fiş okuma doldurur; KP6 ve KP7 için) | Denetim B23 |
