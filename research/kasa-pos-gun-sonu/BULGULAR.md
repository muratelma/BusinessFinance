# Kasa ve POS: esnafın gün sonu — bulgular (1. tur, masa başı)

**Tarih:** 28 Eylül 2026. **Kapsam:** planın A–G soruları ve H1–H16 hipotezleri (masa başı) ve
kendi uygulamamızın emülatör koşumu (H, §3.1). Kaynak erişim tarihi hepsi için 28 Eylül 2026.

Kanıt düzeyi: **R** resmî düzenleme · **B** banka / ödeme kuruluşunun kendi sayfası · **M** muhasebe
yayını / ürün blogu · **F** forum, şikâyet, haber · **K** kod (bizim).

---

## 1 · Hipotezlerin sonucu

| # | Hipotez | Sonuç | Kanıt |
|---|---|---|---|
| H1 | POS gün sonu fişinde komisyon ve net yazmaz; banka panelinde görünür | **Kısmen doğrulandı.** Banka ekstresinde işlem başına komisyon sütunu olmayabiliyor; brüt / komisyon / net / valör **üye işyeri ekstresinde** duruyor | M [5], B [4] |
| H2 | Gün sonu alınmazsa para geçmez ya da gecikir | **Doğrulandı.** "İşlemlerin hesabınıza aktarılması için … gün sonu işlemi yapmanız gerekmektedir." Birçok cihazda otomatik gün sonu var | B [3], M [9] |
| H3 | Üç model: ertesi gün, blokeli, ara | **Doğrulandı, dördüncüsüyle.** TEB üç modeli adlandırıyor; blokeli **takvim günü** ve **iş günü** ayrı seçenek olarak sunuluyor | B [1], M [2] |
| H4 | Blokeli çalışmada komisyon sıfırdır | **Doğrulandı** (saf blokelide). Ara modelde düşük komisyon var. Ek: bloke süresi dolmadan parayı çekmek için **bloke çözüm ücreti** alınabiliyor | B [1], R [6] |
| H5 | Oran ile süre ters orantılı | **Doğrulandı.** Bloke çözüm ücreti düzenlemede kalan gün / azami gün oranıyla bağlı | R [6] |
| H6 | Anlaşma esnafa özeldir; oran ve süre kullanıcıdan alınmalı | **Doğrulandı.** TCMB oranları **azami** sınırdır; banka "rekabetçi piyasa şartları ve üye işyerleri ile yaptıkları anlaşmalar çerçevesinde serbestçe" belirler | R [6] |
| H7 | Süre iş günü ya da takvim günüyle sayılır | **Doğrulandı.** İki ayrı ürün seçeneği | M [2] |
| H8 | Z raporu nakit ve kartlı satışı ayrı verir | **Doğrulandı.** Z raporunda TOPSAT, TOPKDV, KDV oranı kırılımı, NAKİT ve KREDİ tahsilat; kontrol "nakit + kredi = TOPSAT" | M [7] |
| H9 | Yatan tutar hesaplanan netten farklı olabilir | **Doğrulandı.** Azami oranlara BSMV dahil değil; BSMV komisyon üzerinden ayrıca kesiliyor. Ayrıca iade ve ters ibraz sonradan üye işyerinden tahsil ediliyor | R [6], M [8], F [11] |
| H10 | Günün satışları tek toplu hareketle yatar | **Doğrulandı** (sistem gün sonunda toplar); bazı anlaşmalarda satışlar ayın belirli günlerinde toplu aktarılıyor | B [3], M [5] |
| H11 | Taksitli satışta komisyon farklı, para taksit taksit geçebilir | **Doğrulandı, iki seçenekli.** Ya yüksek komisyonla tutarın tamamı ertesi gün (Garanti'de 2 taksit %5,34 … 18 taksit %33,82), ya her taksit ayrı ayrı bloke sonunda | B [4], R [6] |
| H12 | Komisyon kart türüne göre değişir | **Doğrulandı.** Azami: yurt içi kredi kartı %3,56, banka kartı %1,04, yurt dışı kart %1,90 (Ekim 2026). Bloke azamisi: kredi kartı 40 gün, banka kartı 15 gün | R [6] |
| H13 | Yemek kartı ayrı bir kanal gibi davranır | **Doğrulandı.** Yasal komisyon tavanı %6, ödeme süresi en fazla 30 gün; basında fiili kesintinin daha yüksek olduğu bildiriliyor | F [12] |
| H14 | Esnaf nakit satışı tek tek kaydetmez | **Doğrudan kanıt yok.** Muhasebe pratiği Z raporunu **günlük toplam** olarak kaydediyor (nakit kasaya, kart ara hesaba). Kasa defteri 1998'den beri zorunlu değil | M [7], M [10] |
| H15 | Hesap hareketi satış gününü ve brütü taşımaz | **Kısmen.** Hesap hareketi (ve açık bankacılık) yalnız yatışı görür. Ama bankalar ayrıca **üye işyeri entegrasyonu** (web servis / API) veriyor ve bu satış satış veri taşıyor | M [5], B [13] |
| H16 | Esnaf banka bakiyesini kayıtlarla karşılaştırmaya ihtiyaç duyar | **Dolaylı kanıt.** POS mutabakatı ayrı ürün olarak satılıyor ("her ay yeniden kurulan bir Excel kurgusu"); hedefi büyük işletme | B [13] |

## 2 · Soru soru bulgular

### A · Tahsilat kanalları
- **Banka POS'u** ve **yazar kasa POS (YN ÖKC)** aynı parayı taşır ama iki ayrı gün sonu vardır:
  Z raporu mali belgedir ve GİB'e gider; banka gün sonu ayrı bir mutabakattır [9].
- **Ödeme kuruluşları** (Ödeal, Paycell vb.) tek sözleşmeyle bütün bankaların kartını alıyor;
  "ertesi gün %0 komisyon" gibi farklı modeller satıyor [14]. Model bankadan farklı ama aynı
  eksenlerde: oran, süre, taksit.
- **Yemek kartları** ayrı bir kanal: kendi komisyonu (%6 tavan) ve kendi ödeme süresi (30 gün tavan) [12].
- Havale/EFT ve karekod bu turda incelenmedi.

### B · Komisyon ve süre
Bir tahsilatın hesaba geçişini belirleyen eksenler, kaynaklardan:

| Eksen | Değerler | Kaynak |
|---|---|---|
| Kart türü | yurt içi kredi · banka kartı · yurt dışı | R [6] |
| Taksit | peşin · 2…18 taksit | R [6], B [4] |
| Ödeme modeli | ertesi gün · blokeli · ara | B [1] |
| Gün sayımı | takvim günü · iş günü | M [2] |
| Ek kesinti | BSMV (komisyon üzerinden) · bloke çözüm ücreti · sabit aylık POS ücreti | R [6], B [4] |

TCMB azami oranları 2023'ten beri birkaç kez değişti; oranın koda gömülmemesi gerektiği bir kez daha
doğrulanıyor (ADR 0016 ilkesi).

### C · Gün sonu belgeleri
- Z raporu alanları ve muhasebe kaydı: nakit → 100 Kasa, kredi kartı slip alacağı → **108 Diğer
  Hazır Değerler** (ara hesap), satış → 600, KDV → 391 [7].
- **Muhasebede kart satışı bir ara hesapta bekler ve bankaya geçince oradan çıkar.** Bu bizim
  "yoldaki para" kurgumuzla (ADR 0015) aynı mantık; kurgu kaynakla desteklendi.

### D · Banka tarafı ve mutabakat
- Hesaba yatan tutar ile POS'tan geçen tutar "hiçbir zaman aynı değildir" (komisyon, valör, taksit, bloke) [5].
- İade gün sonundan sonra ayrı işlemdir; ters ibrazda tutar üye işyerinden tahsil edilir, itiraz
  süresi uzun olabilir (540 güne kadar) [11].

### E · Nakit ve kasa
- Kaynaklar gün sonunda kasanın sayılıp kayıtla karşılaştırılmasını öneriyor [10]. Kasa defteri
  zorunlu değil.
- Muhasebe günlük satışı Z raporundan **toplam** olarak kaydediyor; tek tek satış kaydı muhasebe
  pratiği değil [7].

### F · Entegrasyon ufku
İki ayrı entegrasyon türü var ve farklı veri getiriyorlar:

| Tür | Ne getirir | Kim sağlar | Kaynak |
|---|---|---|---|
| Hesap bilgisi (açık bankacılık) | Hesap hareketleri: yatış günü, net tutar, açıklama | Lisanslı hesap bilgisi hizmeti sağlayıcıları (TCMB, 6493) | R [15] |
| Üye işyeri entegrasyonu | Satış satış brüt, komisyon, net, valör | Banka, işletmenin talebiyle web servis / API | B [13] |

**Sonuç:** Kasa ve POS modeli "beklenen (satış günü) / gerçekleşen (yatış) / eşleştirme" ayrımıyla
kurulursa iki entegrasyon türü de modeli değiştirmeden bağlanabilir. Hesap bilgisi yalnız
"gerçekleşen"i, üye işyeri entegrasyonu "beklenen"i de doldurur.

### G · Başkaları nasıl yapıyor
- KolayBi: satış brüt hasılat, komisyon ayrı gider (780 veya banka komisyon alt hesabı); BSMV'nin KDV'li
  hizmet faturası gibi ele alınmaması uyarısı [8]. Bizim modelle aynı yön.
- Finteo: POS mutabakatını ayrı ürün olarak satıyor; hedefi perakende zincirleri ve çok bankalı
  finans ekipleri [13]. **Tek dükkânlı esnafa yönelik bir mutabakat aracı bu turda bulunmadı.**

## 3 · Kendi uygulamamız (koddan, 28 Eylül 2026)

| # | Bulgu | Önem |
|---|---|---|
| U1 | Kasa'nın beklenen bakiyesi girilen hareketlerden hesaplanıyor; nakit satış toplamı girilmezse sayım farkı günün satışı olur, açık/fazla değil | Yüksek — Kasa'nın gerçek hayatta çalışıp çalışmadığı buna bağlı |
| U2 | `Hesaba geçti` tutar almıyor; BSMV, iade, kesinti yüzünden farklı yatan tutar kaydedilemiyor (H9) | Orta |
| U3 | Her POS kaydı tek tek işaretleniyor; toplu yatış tek hareketle eşleşemiyor (H10) | Orta |
| U4 | Tek geçiş günü; taksitli satışın taksit taksit geçişi modellenemiyor (H11) | Sektöre göre değişir |
| U5 | Kart türü, BSMV, iş günü yok; oran her kayıtta elle | Düşük–orta (değer sınavına bağlı) |
| U6 | Formda canlı net önizlemesi yok | Düşük maliyet, kolay kazanç |
| U7 | Beklenen POS tahsilatları yaklaşanlar görünümünde yok | Orta |
| U8 | **Çifte sayım tuzağı:** banka ekstresi içe aktarımı her satırı gelir/gider (`BudgetTransaction`) olarak yazıyor (`ConfirmImportBatchUseCase`). Ekstredeki POS yatışı içe aktarılırsa satış bir kez POS'ta, bir kez içe aktarmada gelir sayılır | **Yüksek** — ADR 0014'ün önlediği hatayı bugün açık bırakıyor |
| U9 | Kasa ile POS birbirinden habersiz; Z raporu gibi tek bir gün sonu girişi ikisini birden besleyemiyor | Yüksek — sekmenin yapısını belirliyor |
| U10 | Kapanmış sayım, sonradan girilen geçmiş tarihli hareketlerden habersiz; ekran "oturdu" demeye devam ediyor (T1b) | Orta — ekran yanlış bir şey söylüyor |
| U11 | Nakit hesaba yazılan gelir/gider/transfer Kasa ekranını yenilemiyor (`FinancialDataChanges`) | Düşük maliyetli hata; düzeltilmeli |
| U12 | POS kaydı iptal edilemiyor, "hesaba geçti" geri alınamıyor; kod yorumu olmayan bir ekranı işaret ediyor | **Yüksek** — yanlış giriş kalıcı |
| U13 | POS formunda her akşam aynı beş seçim; beklenen gün varsayılanı bugün (T3) | Orta — POS tanımı bunu çözer |

### 3.1 · Emülatör koşumu (Pixel 8, 28 Eylül 2026)

**Veri:** `test@test.test` hesabına veritabanından sentetik bir hafta eklendi (21–27 Eylül): beş POS gün
sonu (%1,75, ertesi gün; 25 ve 26 Eylül yolda), kasadan iki gider, bankaya nakit yatırma ve şahsi
çekim (transfer). Nakit satış **bilerek girilmedi**. Kayıtlar açıklamada `[KP-test]` etiketi taşır;
mevcut veriye dokunulmadı. Kareler: `kareler/00`–`23`.

| # | Test | Sonuç | Kare |
|---|---|---|---|
| T1 | Nakit satış girilmeden sayım (31.200 TL) | **U1 doğrulandı:** "₺20.790,00 fazla". Bu tutar bir haftalık nakit satış. Fark kaydedilirse esnaf büyük ihtimalle "Satış geliri" seçer: bir haftanın satışı tek güne, KDV'siz, "kasa fazlası" olarak yazılır | 04, 05, 06 |
| T1b | Sayım kapandıktan sonra geçmiş tarihli hareket | **Yeni (U10):** sabah 24.000 TL'de "oturdu" diye kapanan sayım, sonra girilen dünkü giderlerden sonra da "kasa sayılan tutara oturdu" diyor; uygulamanın kendi kasası 10.410 TL. Aynı karede kart "Uygulamaya göre ₺23.300", panel "₺10.410" diyor | 01, 04 |
| T2 | Nakit satışı tek toplam gelir olarak gir, yeniden bak | Toplam girilince kasa "Tuttu" oldu; akış mantıklı. Ama: **(U11)** kayıttan sonra Kasa ekranı kendini yenilemedi, elle çekince düzeldi (`transactionsChanged` / `transferChanged` `cash` sinyali göndermiyor). Form, Kasa'dan ve Dükkân Kasası seçiliyken açılsa da hesabı önceden seçmiyor. Tutar alanı binlik ayırıcısız ("20790"), sayım panelinde ayırıcılı | 08–12 |
| T3 | Bugünün POS gün sonu (9.800 TL, %1,75) | Doğru kayıt (net 9.628,50). **~15 dokunuş + 2 klavye girişi.** Hesap, kategori, komisyon türü, komisyon kategorisi ve beklenen gün her akşam aynı olduğu hâlde her seferinde seçiliyor; beklenen gün varsayılanı **bugün**. Oran girilince net ya da komisyon tutarı görünmüyor (**U6 doğrulandı**) | 13–16 |
| T4 | 25–26 Eylül POS'ları tek yatışta, BSMV düşülmüş (23.362,67 yerine 23.383,50 beklenen) | **U2, U3 doğrulandı:** onay yalnız net tutarı gösterir, tutar ve gün sorulmaz (gün sessizce bugün). Kayıtlar tek tek işaretlenir. 20,83 TL fark hiçbir yere yazılamaz; banka bakiyesi uygulamada gerçekten 20,83 TL fazla kalır. Hafta sonu yüzünden "Gecikti" uyarısı yanlış alarm (H7). Detayda satış günü yazmıyor | 17, 18 |
| T5 | Yemek kartı tahsilatı | Ayrı yeri yok. POS tahsilatı olarak (30 gün, yüksek oran) girilebilir; ad ve açıklamalar "POS" dediği için yanıltıcı | — |
| T6 | Kartlı satış iadesi / yanlış POS kaydını düzeltme | **Yeni (U12):** POS için iptal ucu ve düğmesi **yok** (API: yalnız oluşturma ve "hesaba geçti"). Birleşik akış POS satırlarının iptalini "kaydın kendi ekranından yapılır" gerekçesiyle kapatıyor, ama o ekranda iptal yok. Yanlış girilen POS ve yanlışlıkla "hesaba geçti" denen kayıt düzeltilemiyor. İade için tek yol ayrı bir gider girmek | — |
| T7 | Yaklaşanlar ve Özet | **U7 doğrulandı:** "7 günde çıkacak" yalnız ödemeleri listeliyor; yarın geçecek 9.628,50 TL yok. Net varlık kartında "Yolda ₺9.628,50 · 29 Eylül" doğru görünüyor. Test sırasında Özet bir süre farklı değerler gösterdi; hangi aydan kaldığı doğrulanamadı (test artefaktı olabilir). Geçmiş ay görünümünde "Hesap bakiyeleri" bugünkü bakiyeyi gösteriyor | 20–23 |
| T8 | POS yatışı içeren ekstrenin içe aktarımı | Emülatörde koşulmadı; çifte sayım yolu koddan kesin (U8) | — |
| T9 | Kasa sekmesinin geneli | Sekme "bugün ne oldu" sorusunu cevaplamıyor: bugünkü satış, bugün yatan POS ve bugün bankada ne olduğu yok. Kasa seçicide şahsi cüzdan dükkân kasasıyla yan yana (tek havuz gereği doğru, ama esnafın "Kasa"sında şahsi cüzdanı görmesi kafa karıştırabilir). POS listesi satırları yalnız açıklamayla ayrışıyor | 01, 02 |

## 4 · Değer sınavı — ön değerlendirme

Karar belgesinde ayrıntılanacak; burada yalnız yön (plan §5.1, D1–D4).

| Olası özellik | D1 ihtiyaç | D2 en az detay | D3 doldurulur mu | Ön sonuç |
|---|---|---|---|---|
| Gün sonu girişi (Z raporu gibi: nakit + kart toplam) | Güçlü (muhasebe pratiği, Ç2) | Üç alan: nakit satış, kartlı satış, tarih | Her gün, Z raporundan kopyalanır | **Yap** |
| Banka mutabakatı (gerçek bakiye / yatış eşleştirme) | Güçlü (H9, H10, U8) | Yatan tutar + hangi yoldakileri kapattığı | Yatış günlerinde | **Yap**, sade sürümü |
| İçe aktarımda POS yatışını tanıma | Güçlü (U8 hatası) | Satırı "bu bir POS yatışı" diye işaretleyip gelir yazmamak | İçe aktarım yapan kullanıcıda | **Yap** (hata düzeltmesi) |
| POS tanımı (oran, süre, model) | Orta (H6) | Hesap + varsayılan oran + gün sayısı | Bir kez | **Sade sürümünü yap** |
| Canlı net önizlemesi | Orta | — | Her girişte | **Yap** (ucuz) |
| Kart türüne göre oran | Zayıf-orta: esnaf gün sonunda kart türü dağılımını ayrı girmez | — | Büyük ihtimalle doldurulmaz | **Yapma** (şimdilik) |
| Taksit taksit geçiş | Sektöre bağlı (giyim, beyaz eşya güçlü; kafe zayıf) | Taksit sayısı + model | Taksitli satış yapanda | Ertele / ayrı değerlendir |
| İş günü / tatil takvimi | Orta | Seçenek + elle düzeltme | Bir kez | Sade: yalnız "iş günü" seçeneği, tatil takvimi yok |
| BSMV ayrı alan | Zayıf: net tutarı tutturmak için yeterli olan "gerçekleşen tutar" farkı zaten yakalar | — | — | **Yapma**; U2 çözümü kapsar |
| Yemek kartı | Sektöre bağlı | POS tanımının bir türü olarak | — | POS tanımına tür olarak girer |

## 5 · Açık kalanlar (sonraki tur)

- T8'in emülatörde koşulması (gerekirse); T5 ve T6'nın akış olarak tasarlanması karar belgesine kalır.
- Havale/EFT ve karekodlu tahsilat.
- Esnafın nakit pratiğine doğrudan kanıt (H14): forum ve şikâyet kaynakları.
- Tek dükkânlı esnafa yönelik rakip bir mutabakat ya da gün sonu uygulaması var mı.
- Kasa sekmesinin yeri (plan I) karar belgesinde tartışılacak.

## Kaynaklar

1. TEB POS çalışma şekilleri — https://www.teb.com.tr/tebpos/calisma-sekilleri/ (B)
2. Ödeme modelleri özeti (takvim / iş günü blokeli, hibrit) — arama özeti, https://www.hesapkurdu.com/ticari/pos/karsilastirma ve https://paynkolay.com.tr/blog/pos-komisyon-oranlari-nasil-hesaplanir-2026-rehberi (M; birincil banka sayfasıyla ayrıca doğrulanmalı)
3. Garanti BBVA üye işyeri SSS — https://www.garantibbva.com.tr/sikca-sorulan-sorular/uye-isyeri-pos (B)
4. Garanti BBVA POS ücret ve komisyonlar — https://garantibbvapos.com.tr/urun-ve-hizmet-ucretleri (B)
5. Üye işyeri ekstresi ve toplu aktarım — https://finekra.com/hesap-hareketleri/ , https://ekstre.emonse.com/is-bankasi-ekstre-excel (M)
6. TCMB, Üye İşyerlerine Uygulanacak Azami Komisyon Oranları (Ekim 2026 tablosu) — https://www.tcmb.gov.tr/wps/wcm/connect/tr/tcmb+tr/main+menu/istatistikler/bankacilik+verileri/uye+isyerlerine+uygulanacak+azami+komisyon+oranlari ; bloke süresi ve bloke çözüm ücreti: Tebliğ 2020/4 (R)
7. Z raporu muhasebe kaydı — https://www.fisle.co/blog/z-raporu-muhasebe-kaydi (M)
8. KolayBi, POS komisyon giderleri — https://www.kolaybi.com/blog/pos-komisyon-giderleri ; BSMV %5: https://www.kobipratik.com/pratik-araclar/pos-komisyon-hesaplama (M)
9. Ödeal, gün sonu almayı unuttum — https://odeal.com/blog/gun-sonu-almayi-unuttum-ne-yapmaliyim/ (M)
10. Kasa hesabı / kasa defteri — https://isbasi.com/blog/kasa-defteri-nasil-tutulur , https://www.parasut.com/blog/kasa-hesabi-nedir (M)
11. İade ve ters ibraz — https://arifunal.com/yazi/iptal-ve-iade-islemi-nedir/ , https://www.iyzico.com/destek/yardim-merkezi/guvenli-odeme/ters-ibraz-chargeback (M/F)
12. Yemek kartı komisyon ve ödeme süresi — https://www.gzt.com/ekonomi/restoranlarda-yemek-karti-donemi-bitiyor-mu-yemek-karti-komisyon-oranlari-icin-esnaf-ne-istiyor-4020006 , https://kafe360.com/blog/yemek-karti-komisyon-oranlari-restoran-kafe (F; yönetmelik metniyle ayrıca doğrulanmalı)
13. Finteo POS — https://finteo.com.tr/finteo-pos (B)
14. Ödeal ürünleri — https://odeal.com/yazar-kasa-pos/ , https://odeal.com/odeal-sss/ (B)
15. TCMB, Ödeme Hizmetlerinde Veri Paylaşım Servislerine İlişkin Rehber — https://www.tcmb.gov.tr/wps/wcm/connect/d60cc679-ce04-4941-b310-b3788b6f3540/Odeme+Hizmetlerinde+Veri+Paylasim+Servislerine+Iliskin+Rehber.pdf?MOD=AJPERES (R)
