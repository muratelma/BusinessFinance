# Karar denetimi — dış kaynak eki (29 Eylül 2026)

**Ne:** `DENETIM-2026-09-29.md`'deki bulgular için "başkaları bu sorunu nasıl çözmüş, mevzuat ne diyor"
araştırması. Denetim belgesi **değiştirilmedi** (kullanıcı inceliyor); bu ek onu düzeltir ve güçlendirir.
Çelişki olursa bu ekteki kaynaklı bilgi geçerlidir.
**Kanıt düzeyi** (kasa araştırmasıyla aynı): **R** resmî düzenleme · **B** ürünün kendi sayfası / yardım merkezi ·
**M** muhasebe yayını, ürün blogu · **F** forum. Erişim tarihi hepsi için 29 Eylül 2026.
**Tarafsızlık:** rakip çözümleri ölçüt değil; her biri "ne kazandırıyor / ne kaybettiriyor" ile yazıldı.

---

## 0 · Özet — denetimde ne değişiyor

| Bulgu | Değişiklik |
|---|---|
| **B9, senaryo (g)** | **Denetimde hata: gerekçenin nakit tarafı yanlış.** Faturalı satış Z'nin TOPSAT'ına girmez ama **NAKİT/KREDİ satırlarında görünür** [3]. KP4 geliri TOPSAT'tan değil NAKİT ve KART satırlarından okuduğu için, gün içinde ayrıca girilmiş faturalı satış **iki tarafta da** gün sonunun içindedir. Doğru varsayılan "işaretsiz" değil, **işaretli (düşülür)**. Sonuç sadeleşir: gün içinde tek tek girilmiş **her** satış varsayılan olarak "dahil" gelir (§1) |
| **B1** | **Güçlendi, ve çözümü kolaylaştı.** Yazar kasada veresiye tahsilatı ayrı bir belgeyle yapılır ("Cari Hesap Tahsilatı" bilgi fişi, *mali değeri yoktur*) ve Z raporunda **kendi sayacı** vardır: "Cari Hesap Tahsilatı Sayısı / Tutarı" [1][2]. Z okuyan kullanıcıda düşülecek tutar Z'nin üstünde yazıyor |
| **B10** | **Yeniden çerçevelendi, ciddiyeti arttı.** Sorun yanlış alarm değil, **sessiz eksik**: ödemenin tamamı yemek kartıyla alınan satış "mali değeri olmayan bilgi fişi"yle belgelenir ve Z'de ayrı "Yemek Fişi/Kartı İşlem Tutarı" sayacında durur [1]. KP4 yalnız NAKİT ve KART okursa lokantanın yemek kartı satışı **hiç gelir yazılmaz**. Orta → **yüksek** öneriyorum |
| **B2** | **Güçlendi.** İncelediğimiz Türk rakiplerin hepsi muhasebeciye veri vermeyi ürünün çekirdeğinde tutuyor; tek işi bu olan ürün de var (Fisle) (§2.2) |
| **B4** | Paraşüt'ün toplu ödemesi bir desen öneriyor: **tek tutar, kendiliğinden eşleşme** (en gecikmişten başlayarak) [5] |
| **B5** | Karşılaştırılan bütçe uygulamalarında (YNAB, Monarch) gerçekleşmiş plan kaydı **sıradan bir kayıttır**: düzenlenir, silinir, geri alınır [7][8]. Bizim kilidimiz alışılmışın dışında |
| **B8** | Üçüncü bir yol çıktı: **muhasebenin kendi yöntemi** — kasa her sayımda sayılan tutara eşitlenir; nedeni bilinmeyen fark gelir/gider olmadan **geçici bir kovada** bekler [10] (§2.4) |
| **Yeni — Y1** | Birden çok yazar kasası olan işletme günde **birden çok Z** alır [12]; KP3'ün "(kullanıcı, gün, kasa)" anahtarı ikinci Z'yi "tekrar kapatma" sayıp yok sayar |
| **Yeni — Y2** | Vergi takviminde iki tarih düzeltmesi: muhtasarın SGK kısmı ve Bağkur'un **dönemi** (§3) |

---

## 1 · Denetime düzeltme: faturalı satış ve B9

**Denetimde yazılan (B9, senaryo g):** faturalı satış Z TOPSAT'ında yok, bu yüzden nakit tarafta işaretsiz
bırakmak doğru; yalnız kart tarafında (banka gün sonu) sorun var.

**Kaynağın söylediği:** "Bilgi fişi kesilir ve satış faturaya bağlanır. Bu tutar Z raporunun **TOPSAT toplamına
girmez ama kasa/kredi satırlarında görünür**" [3]. GİB kılavuzu da bilgi fişlerinin X ve Z raporlarında ayrı
sayaçlarda gösterildiğini söylüyor: "Faturalı Satış Sayısı / Toplam Tutarı", "Cari Hesap Tahsilatı
Sayısı / Tutarı", "Yemek Fişi/Kartı İşlem Sayısı / Tutarı", avans, otopark [1].

**Sonuçlar:**
1. KP4 geliri NAKİT ve KART'tan okuyor → faturalı satışın parası o satırlarda. Aynı satış gün içinde ayrıca
   girildiyse **iki kez** gelir. Doğru davranış: gün içi faturalı satış **düşülür** (işaretli).
2. B9'un önerisi sadeleşir: **gün içinde tek tek girilmiş her satış varsayılan olarak "Z'de var" gelir.**
   Denetimdeki "faturalı satış işaretsiz" istisnası kalkar. Kayıt başına ayrı nakit/kart çözümü de
   gerekmez: kayıt hangi yolla ödendiyse o satırdan düşülür.
3. KP4'ün kontrolü "nakit + kart = TOP" faturalı satış olan her gün tutmaz. Fisle aynı şeyi söylüyor: denklem
   tutmuyorsa "ya ödeme kırılımı yanlış okunmuş ya da raporda faturalı satış/nakit giriş vardır" [3].
   Doğru kontrol: **NAKİT + KART = TOP + faturalı + cari tahsilat (+ avans)**. Ayrı sayaçlar okunursa kontrol
   gerçek bir hata yakalayıcı olur.
4. `GUN-SONU-BELGELERI.md` §1 ve §3 ("faturalı satışlar … TOPSAT'a dahil değil") yanlış değil ama **eksik**:
   ödeme satırlarına dahil oldukları yazılmamış. KP7'nin gerekçesi bu eksik bilgiye dayanıyor.

**Kanıt gücü:** [3] ürün blogu (M); [1] GİB teknik kılavuzu (R) ayrı sayaçları doğruluyor ama bilgi fişi
tutarının NAKİT/KREDİ satırına girdiğini **açıkça yazmıyor**. §5'teki bekleyen girdi (kartla ödenmiş faturalı
satışlı gerçek bir Z) bunu kesinleştirir.

## 2 · Bulgu bulgu dış kaynak

### 2.1 · B1 — kartla veresiye tahsilatı

- **Mevzuat:** yazar kasayla cari hesaba tahsilat yapılırsa "Cari Hesap Tahsilatı" bilgi fişi (9 no'lu örnek)
  iki nüsha düzenlenir. Fişte "MALİ DEĞERİ YOKTUR – CARİ HESAP TAHSİLATI" yazar, kartla ödemede kart hamili
  cari hesap sahibiyle aynı kişi olmalıdır [1] §9.6; özelge 23.01.2018, 11395140-105[VUK-1-20450]-74016 [2].
  Z'de "Cari Hesap Tahsilatı Sayısı / Tutarı" alanı var [1] §8.
- **Ne demek:** esnafın kendi mevzuatı veresiye tahsilatını satıştan ayırıyor; bizim KP13'ümüz ile aynı yön.
  B1'in çözümü (denetimde öneri A) Z okuyan kullanıcıda **okunan bir alanla** yapılabilir: KP4'ün okunacak
  alanlarına "Cari Hesap Tahsilatı Tutarı" eklenir; bu tutar karta (ya da nakde) göre düşülür ve o günün
  uygulamaya girilmiş cari tahsilatlarıyla karşılaştırılır.
- **Hâlâ açık:** yalnız banka POS'u olan (yazar kasasız) esnafta banka gün sonu fişi tahsilatı satıştan ayırmaz
  ([4], banka fişi yalnız kart işlemlerini listeler). Orada B1-A'nın "uygulamaya girilmiş kartlı cari
  tahsilatları varsayılan düş" kuralı gerekli kalır.

### 2.2 · B2 — muhasebeciye veri

| Ürün | Muhasebeciye ne veriyor | Kazandırdığı | Kaybettirdiği | Kaynak |
|---|---|---|---|---|
| Paraşüt | Mali müşavir hesaba eklenir, veriyi canlı görür | Ay sonu dosya alışverişi yok | Çok taraflı erişim, yeni yetki katmanı | `gozlemler/parasut.md:66, 212` (B) |
| Logo İşbaşı | Müşavir Portal; "gider fişlerinizi muhasebecinize kolayca"; fiş okuma görselleri saklanır, "mali müşaviriniz fişlerinizi kolaylıkla görüntüleyebilir" | Belge fotoğrafı doğrudan müşavire | Aynı yetki maliyeti | `gozlemler/logo-isbasi.md:108-132`, [9] (B) |
| KolayBi | Çok müşterili muhasebeci erişimi | — | — | `gozlemler/kolaybi.md:94` (B) |
| Hesap Defterim | PDF/Excel dışa aktarma + "işlem dökümünü e-posta ile otomatik gönder" | Yetki katmanı olmadan düzenli teslim | Belge görseli yok | `gozlemler/hesap-defterim.md:271` (gözlem) |
| Fisle | Ürünün tamamı: fiş ve **Z raporu** fotoğrafından muhasebe satırı, muhasebe programına Excel | Muhasebecinin elle giriş işini kaldırır | Muhasebe ürünü; bizim sınırımızın dışında | [3], [11] (M/B) |

**Ne demek:** bu ihtiyacı karşılamayan tek ürün sınıfı kişisel bütçe uygulamaları. İşletme tarafındaki bütün
Türk rakipler bunu çekirdekte tutuyor. Hedef kullanıcımız (şahıs şirketi, esnaf) muhasebecisiz yaşamıyor.
Denetimdeki **B2-B** önerisi (KDV'siz "ay sonu dökümü + belgeler") Hesap Defterim'in yetki katmanı
gerektirmeyen yoluna denk düşüyor: canlı erişimin maliyetini almadan ihtiyacın çoğunu karşılıyor. Karar yine
kullanıcının; bu tablo yalnız "paket kalkarsa rakiplerin hepsinin verdiği bir şeyi vermeyen tek işletme ürünü
oluruz" bilgisini ekliyor.

### 2.3 · B3, B4, B5 — vergi planı

| Ürün | Tutarsız / değişken kalem | Birden çok kalemi tek ödemeyle kapatma | Gerçekleşmişi düzeltme | Kaynak |
|---|---|---|---|---|
| Paraşüt (Vergi/SGK primi) | Kayıtta **tutar, vergi dönemi ve vade zorunlu**; durum "Ödenecek / Ödendi". Ödendiyse hesap ve gün sorulur; ödeme sonradan tarih + tutarla eklenir | Tedarikçi giderlerinde: toplam görünür, kısmi ödeme "en gecikmişten başlayarak" kendiliğinden eşleşir | Belgelenmemiş | [5], [6] (B) |
| YNAB | Planlı kayıt ("scheduled") girilmeden önce tutarı, günü düzenlenir; yalnız bu sefer ya da bundan sonrakiler | — | Girilen kayıt sıradan bir kayıttır: düzenlenir, silinir | [7] (B) |
| Monarch | Değişken tutarlı faturayı kaçırabildiğini kendisi yazıyor; elle "ödendi" işaretlenir | — | — | [8] (B) |

**Ne demek:**
- **B3 (tutarsız plan):** Paraşüt tutarsız vergi kaydına izin **vermiyor**; tahakkuk gelince kaydı açıyor.
  Kazandırdığı: tutarsız kalem sorunu hiç doğmuyor. Kaybettirdiği: tutar gelmeden takvim görünmüyor, yani
  bizim "vergi = nakit planı" fikrinin tam tersi. Kullanıcının "tutarı ödemede öğreniyoruz" sözü tutarsız kalemi
  gerekli kılıyor; rakip bunu çözmüş değil, **kaçınmış**. Denetimdeki B3-A (nullable tutar) geçerli.
- **B4 (toplu ödeme):** Paraşüt deseni: kullanıcı tek tutar yazar, uygulama en gecikmişten başlayarak
  kendiliğinden eşler. Bu, denetimdeki B4-A'nın "hepsi seçili gelir" önerisinden **daha az dokunuşlu** ikinci
  seçenek: seçtirmek yerine kendiliğinden kapat, kalan olursa göster. Vergi türleri karışıksa (KDV + Bağkur)
  kendiliğinden eşleşme yanlış kalemi kapatabilir; o yüzden öneri: **aynı türde kendiliğinden, farklı türde
  seçili gelir.**
- **B5 (geri alma):** karşılaştırılan ürünlerde gerçekleşmiş planlı kayıt sıradan kayıttır. Bizim
  `transactions.cancel_origin_locked` kilidimiz planın geçmişini korumak için var ve bunun bir karşılığı
  yok değil; ama "düzeltme yolu hiç yok" durumu rakiplerde görülmedi. B5-A (vergi ekranından geri al,
  occurrence bekleyene döner) bu farkı kapatır.

### 2.4 · B8 — kasada beklenen tutar

| Yöntem | Nasıl | Kazandırdığı | Kaybettirdiği | Kaynak |
|---|---|---|---|---|
| POS sistemleri (Square, Loyverse) | Her oturum **sayılarak girilen açılış nakdiyle** başlar; beklenen = açılış + nakit satış − iade ± kasaya giren/çıkan; fark gösterilir | Eski fark ertesi gün yeniden sorulmaz | Bir defter tutmaz; fark hiçbir hesaba yazılmaz | [13], [14] (B) |
| Muhasebe (Tek Düzen) | Kasa her sayımda **fiili sayıma eşitlenir**; nedeni bulunana kadar fark 197 (noksan) / 397 (fazla) **geçici hesaplarında** bekler, nedeni bulununca ilgili hesaba aktarılır | Tek gerçek korunur **ve** sebep sonradan yazılabilir | Kullanıcıya bir kayıt daha | [10] (M) |

**Ne demek:** KP9 ("son sayımdan bu yana") POS sistemlerinin modeli. Ama POS sistemleri bir defter tutmuyor.
Bizde kasa bir hesap ve bakiyesi net varlığa giriyor; bu yüzden KP9 ile iki gerçek doğuyor (B8). Muhasebenin
yöntemi üçüncü bir seçenek veriyor:

- **B8-D (yeni seçenek):** sayımda fark her zaman kaydedilir (kasa sayılan tutara eşitlenir). Sebep sorusu
  isteğe bağlı kalır. "Bilmiyorum" seçilirse fark "Kasa farkı (açıklanmadı)" kategorisinde bekler, sonradan
  gerçek kategorisine taşınabilir. Bu yol KP9'un amacını (eski farkı her gün yeniden sormamak) ve tek gerçeği
  birlikte sağlar. Bedeli: "fark kaydı isteğe bağlı" kararından vazgeçmek. Sebep sorusu yine isteğe bağlı
  olduğu için kullanıcıya ek soru getirmiyor; getirdiği şey fazladan bir kayıt.

### 2.5 · B10 — yemek kartı

- Ödemenin **tamamı** yemek kartı/çekiyle alınırsa "mali değeri olmayan bilgi fişi" düzenlenir (satış sonra
  yemek kartı firmasına faturalanır); **kısmen** alınırsa mali fiş düzenlenir, yemek kartı kısmına KDV
  hesaplanmaz [1] §9.2. Z'de "Yemek Fişi/Kartı İşlem Sayısı / Tutarı" ayrı alan [1] §8.
- **Ne demek:** KP4 yalnız NAKİT ve KART okursa yemek kartı satışı kaybolur. KP10'un "yemek kartı = POS
  tanımı" kararı doğru; eksik olan, o POS'a giden tutarın **Z'nin yemek kartı alanından** okunması. Lokanta ve
  kafeler (2026'da gerçek usule geçen sektörlerden biri) için bu, gün sonunun işe yarayıp yaramaması demek.
  **Ciddiyet önerisi: yüksek.**

### 2.6 · B11 — toplu Z ve Y1 — birden çok yazar kasa

- Z raporu her gün alınmalı; ay sonunda hepsini tek fişle deftere geçirmek mevzuata uygun görülmüyor. Birden
  çok cihaz varsa her birinin Z'si günlük bir icmale bağlanabilir [12].
- **Y1 (yeni):** iki yazar kasalı bir dükkân günde iki Z getirir. KP3 gün sonunu "(kullanıcı, gün, kasa)"
  anahtarıyla tekrarsız yazıyor. Buradaki "kasa" uygulamadaki nakit hesap, cihaz değil. İkinci Z "aynı günü
  tekrar kapatmak" sayılır ve yazılmaz. **Öneri:** tekrar kontrolü **Z no + cihaz** üzerinden yapılsın
  (Fisle'nin yöntemi: "aynı cihaz + Z no daha önce yüklendiyse mükerrer uyarısı" [3]). Z no'suz elle girişte
  gün başına tek gün sonu kalır. Aynı gün ikinci bir gün sonu "ikinci cihaz" olarak eklenebilir.
- Ay dönümünü geçen toplu Z için kaynak bulunamadı (§4).

## 3 · Vergi takvimi tarihleri (hazır türler)

| Tür | Hazır türde | Kaynak | Sonuç |
|---|---|---|---|
| Muhtasar ve prim hizmet | Her ay 26 | Beyan izleyen ayın 26'sı; tatile denk gelirse ilk iş günü. Muhtasar ile SGK bildirimi tek beyannamede birleşik; süre uzatmaları olabiliyor (ör. Nisan 2026 dönemi: ödeme 5 Haziran'a uzatıldı) [15][16] | Gün doğru. `vergi/BULGULAR.md` §1.3'teki "ay sonuna kadar ödeme" notu SGK primi kısmı için olabilir; iki ödeme tek beyannameden çıkıyor. **Uzatmalar B5'i destekliyor:** plandaki tarih bir beklentidir, "Ödedim"de gün değişebilmeli |
| Bağkur (4/b) | Her ay, ay sonu | Kaynaklar **çelişkili**: bir kaynak "ait olduğu ayı takip eden ayın son günü", aynı sayfadaki örnek "Temmuz primi 31 Temmuz" [17] | Ödeme günü her iki okumada da **ay sonu** (hazır tür doğru). Çelişki yalnız **dönem** etiketinde: Eylül sonunda ödenen Bağkur Eylül'ün mü Ağustos'un mu? §6.5'in "Dönem" etiketi (B22) bu yüzden yanlış yazılabilir. SGK'nın kendi sayfasından doğrulanmalı |
| Geçici vergi, yıllık gelir, emlak, MTV, tabela | — | Bu turda yeniden aranmadı; 28 Eylül'de kaynaklandı ya da kullanıcı doğruladı | — |

## 4 · Doğrulanamayanlar (bekleyen girdi)

1. Bilgi fişli (faturalı ya da cari tahsilat) **kartla** ödenen işlemin Z'nin KREDİ satırına girdiği: [3]
   "kasa/kredi satırlarında görünür" diyor (M), GİB kılavuzu açıkça yazmıyor. **Kartlı faturalı satışı ve bir
   veresiye tahsilatı olan gerçek bir Z** bunu kesinleştirir. `YOL-HARITASI.md` §5'e eklenmeli.
2. Tamamı yemek kartıyla ödenen satışın Z'de TOPSAT'a girip girmediği (mali değeri olmayan bilgi fişi →
   girmemesi beklenir). Lokantadan bir Z gerekir.
3. Ay dönümünü geçen toplu Z'nin muhasebede hangi aya yazıldığı: kaynak bulunamadı; kullanıcının
   muhasebeciye sorabileceği bir soru.
4. Paraşüt'te vergi ödemesinin silinip silinemediği: yardım sayfasında yok.

## Kaynaklar

1. GİB, *YN ÖKC Bilgi Fişleri Düzenlenmesine Dair Usul ve Esaslara İlişkin Teknik Kılavuz*, sürüm 3.0 (24.07.2020), §8, §9.2, §9.6 — https://ynokc.gib.gov.tr/UploadedFiles/Files/Bilgi_Fisi_Teknik_Kilavuzu_24072020.pdf (R)
2. Cari hesaba istinaden yapılan tahsilatlarda YN ÖKC kullanımı (özelge özeti, 23.01.2018) — https://nelsus.com.tr/cari-hesaba-istinaden-yapilan-tahsilatlarda-yeni-nesil-odeme-kaydedici-cihazlarin-kullanimi-hk/ ; özelge: https://gib.gov.tr/mevzuat/kanun/443/ozelge/26622 (sayfa içeriği araçla okunamadı) (M/R)
3. Fisle, *Z Raporu Muhasebe Kaydı* — https://www.fisle.co/blog/z-raporu-muhasebe-kaydi (M)
4. Kasa araştırması, `kasa-pos-gun-sonu/GUN-SONU-BELGELERI.md` §1 ve kaynakları [4], [5]
5. Paraşüt, *Yaptığınız ödemeleri kaydetmek* — https://www.parasut.com/kullanim-kilavuzu/yaptiginiz-odemeleri-kaydetmek (B)
6. Paraşüt, *Vergi/SGK primi takibi* — https://www.parasut.com/kullanim-kilavuzu/vergi-sgk-primi-olusturmak (B)
7. YNAB, *Scheduled Transactions* ve *Editing and Deleting Scheduled Transactions* — https://support.ynab.com/en_us/scheduled-transactions-a-guide-BygrAIFA9 , https://support.ynab.com/en_us/editing-and-deleting-scheduled-transactions-a-guide-Skru9yNJo (B)
8. Monarch, *Tracking Recurring Expenses and Bills* — https://help.monarch.com/hc/en-us/articles/4890751141908-Tracking-Recurring-Expenses-and-Bills (B)
9. Logo İşbaşı, fiş okuma — https://isbasi.com/fis-okuma-programi (B)
10. Kasa sayım farkı, 197/397 hesapları — https://www.muhasebetr.com/yazarlarimiz/erdogan/012/ , https://vergidosyasi.com/2017/10/29/kasa-noksanliginin-aciginin-muhasebelestirilmesi-197-sayim-ve-tesellum-noksanlari-hesabinin-isleyisi/ (M)
11. Fisle ürün açıklaması (arama özeti; Z okuma, faturalı satış ayrımı, mükerrer uyarısı) — https://www.fisle.co/blog/z-raporu-muhasebe-kaydi (M)
12. Z raporlarının toplu defter kaydı — https://www.muhasebenews.com/gunu-sonu-z-raporlari-tek-bir-gune-toplanarak-defter-kayitlarina-alinir-mi/ , https://www.verginet.net/dtt/11/ozelge-2020-3.aspx?ozID=2198 (M/R)
13. Square, *Start and end a cash drawer session* / *View cash drawer reports* — https://squareup.com/help/us/en/article/8344-start-and-end-a-cash-drawer-session , https://squareup.com/help/us/en/article/8358-view-cash-drawer-reports (B)
14. Loyverse, *Shift management* — https://help.loyverse.com/help/shift-management-loyverse-pos (B)
15. Müşavirler Kulübü, *Muhtasar ve Prim Hizmet Beyannamesi 2026* — https://musavirlerkulubu.com.tr/beyanname/muhtasar-beyannamesi (M)
16. SGK, *2026/Nisan dönemi MUHSGK süre uzatımı* — https://www.sgk.gov.tr/duyuru/detay/2026Nisan-AyiDonemi-Muhtasar-ve-Prim-Hizmet-Beyannamelerinin-ve-Aylik-Prim-ve-Hizmet-Belgelerinin-Verilme-ve-Odeme-Suresinin-Uzatilmasi-Hakkinda-2026-05-20-02-33-45 (R)
17. Bağkur prim ödeme günü (arama özeti; çelişkili) — https://mukellef.co/blog/bag-kur-primi-nedir-nasil-odenir/ , https://www.uyumsoft.com/blog/bag-kur-4b-borcu-sorgulama-odeme-yapilandirma (M)
