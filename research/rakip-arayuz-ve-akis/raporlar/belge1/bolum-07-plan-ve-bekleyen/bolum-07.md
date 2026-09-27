# Bölüm 7 · Plan, tekrar, taksit ve bekleyen

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Gelecekteki bir ödeme nasıl kuruluyor ve bekleyen işler nerede görünüyor?

Bu bölüm gelecekteki bir ödemenin arayüzde nasıl kurulduğunu ve bekleyen işlerin nerede göründüğünü izler: tekrar formu, taksit kurulumu, bekleyen liste, ana ekrandaki bekleyen alanı, onay adımı, bütçe ve hedef.

Kurulum ile gerçekleşme ayrı okunur. Planın gerçekleşince bakiyeye ve rapora etkisi bu bölümde anlatılmaz; ekranda görünen alan, etiket ve düğme anlatılır. Otomatik kolların çoğu denenmedi; denenmeyen kol için sonuç yazılmaz.

**Bu bölüme girmez**

- Kaydın gerçekleşince bakiyeye etkisi → Belge 2
- Borç ve fatura zinciri → 8
- Bütçe raporunun okunması → 9
- Tahmin formülü hakkında hüküm (bilinmiyor)

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare |  |
| Bluecoins | Canlı kare | Otomatik giriş kolu denenmedi. |
| Wallet | Canlı kare | Onay sorusunda No seçildi. |
| Hesap Defterim | Görülmedi | Plan ve taksit yüzeyi görülmedi. |
| Goodbudget | Canlı kare, Koşum kaydı | Kart hesabı açılamadı; sonraki örneğin üretimi bilinmiyor. |
| KolayBi | Kaynak görseli | Tekrarlı maaş formu; nakit akış raporu. |
| Paraşüt | Kaynak beyanı | Yalnız ana ekran anlatımı. |
| Logo İşbaşı | Görülmedi | Karşılaştırılabilir kaynak anlatımı bulunmadı. |
| QuickBooks Solopreneur | Görülmedi | Karşılaştırılabilir kaynak anlatımı bulunmadı. |


## 7.1 · Tekrar nasıl kuruluyor?

Üç üründe tekrar normal kayıt formunun içinden kuruluyor: bir rozet, bir alt sayfa ya da bir işaret kutusu. Wallet'ta planlı ödemenin kendi formu var; sıklık formun sonunda.

**Şekil 7.1 · Money Manager · Gider formu** (E0241)

1. Tarih satırının sağında Aylık rozeti.
2. Tekrarlı kayıtta yalnız Kaydet; Devam et yok.

**Şekil 7.2 · Bluecoins · planlı işlem alt sayfası** (E0039)

3. Sıklık çipleri: Bir Defa, Günlük, Haftalık, Aylık, Yıllık.
4. Bitiş: Asla, bir etkinlik sonra, Son Tarih.
5. Vade tarihinde otomatik kayıt: işaret kutusu.

**Şekil 7.3 · Wallet · Add Planned payment** (E0288)

6. Plana ad veriliyor (Name).
7. Frequency: Recurrent payment.

**Şekil 7.4 · Goodbudget · Add Transaction** (E0133)

8. Schedule this… kutusu işaretli.
9. Sıklık listesi: Once'tan Every 6 Months'a; sonuncusu kesik.

- Money Manager'da sıklık ayrı bir listede seçiliyor: Hiçbiri'den Yıllık'a 14 seçenek.
- Wallet'ın planlı ödeme tarih seçicisinde bugünden önceki günler soluk görünüyor.
- Goodbudget formunda sıklığın altında e-posta hatırlatma satırı var (… 3 days before); gönderim denenmedi.
- Goodbudget'ta kurulan planın sıklığı Every 2 Weeks.
- Money Manager tekrarlı kaydı kaydederken tek bir onay soruyor: "Tarihte tekrar eden işlemler uygulanır. Tekrarlı olarak kaydetmek istiyor musunuz?" (→ 7.5).

**Aynı soruda diğer ürünler**

- *Görülmedi* · **Hesap Defterim** — Çekmece, ayarlar ve form tarandı; finansal plan veya tekrar yüzeyi görülmedi. Not Defteri ayrı bir not yüzeyi.
- *Kaynak görseli* · **KolayBi** — Mevcut bir kayıttan tekrara dönüştürme (Şekil 7.5).

## 7.1 · Kaynakta görülen tekrar formu

KolayBi'de tekrar ayrı bir formdan değil, mevcut bir kaydın İşlemler menüsünden başlıyor. Destek sayfasındaki örnek bir çalışan maaşı.

**Şekil 7.5 · KolayBi · Çalışan Maaşı, İşlemler menüsü açık** (E0204)

1. İşlemler menüsünde Tekrarlı Maaşa Dönüştür.
2. Cari sekmelerinin sonunda Tekrarlı Maaşlar.

**Şekil 7.6 · KolayBi · Tekrarlı Maaş Oluştur** (E0205)

3. Oluşturma Periyodu, yıldızlı; değerleri görünmüyor.
4. Başlangıç Tarihi: kaynak maaşın tarihi.
5. Maaş Ödeme Tarihi: Belirsiz / Belirli.
6. Maaş Oluşturma Tekrar Sayısı, yıldızlı.

- Periyot ve tekrar sayısının zorunluluğu yalnız maaş formunda görüldü; diğer tekrarlı formlara genellenmez. Aynı alan dizisi (periyot, başlangıç tarihi, tekrar sayısı, bitiş Belirsiz / Belirli) Tekrarlı Proformaya ve Tekrarlı Genel Gidere Dönüştür formlarında da var.
- Formda onay adımı veya otomatik üretim tercihi görünmüyor; kayıtların nasıl üretildiği kaynakta anlatılmıyor.
- Görseller destek materyalidir; demo verisi 2023 tarihli ve güncel sürüm doğrulanmadı.

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Genel gider ayrıntısında da aynı menüde Tekrarlı Genel Gidere Dönüştür var.

## 7.2 · Taksit nasıl kuruluyor?

Money Manager taksiti formdaki bir rozetle, Bluecoins kart harcamasındaki bir alt sayfayla kuruyor. İkisinde de 6.000 altı aya bölündü.

**Şekil 7.7 · Money Manager · Gider formu, taksitli** (E0245)

1. Tarih satırında 6 Ay rozeti; Tutar'a toplam yazılıyor.

**Şekil 7.8 · Money Manager · Ağustos günlük listesi** (E0246)

2. Başlıkta (1/6); satırda tek taksidin tutarı.

**Şekil 7.9 · Bluecoins · Taksit şartlarını seçin** (E0034)

3. Taksit oranı alanı; bu kayıtta 0,00.
4. Taksit sayısı ve İlk ödeme tarihi.

**Şekil 7.10 · Bluecoins · formdaki özet** (E0035)

5. Kaydetmeden önce tek cümlelik özet; Değiştir ve Sıfırla.

- Money Manager'da sonraki taksit kart defterinde (2/6) başlığıyla görünüyor.
- Bluecoins'te kalan beş taksit Hatırlatıcılar listesinde sayaçla duruyor (→ 7.3). Taksitler kendiliğinden işlemlere girmiyor; her biri hatırlatıcıdan Kaydet ile kaydediliyor, ileri tarihli taksitte ödeme tarihi soruluyor: bugün ya da taksitin tarihi (kullanıcı kontrolü, karesi yok).
- Money Manager'ın ay sayısını soran alanı bu karelerde görünmüyor.
- Taksit oranının neyi ifade ettiği ekranda yazmıyor.
- İlk taksidin hangi aya yazıldığı → Belge 2 3.4.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Bluecoins** — Taksit alanı hesap olarak kredi kartı seçildiğinde formda çıkıyor (→ 4.2).
- *Görülmedi* · **Wallet** — İncelenen form ve kayıt ayrıntısında taksit alanı yok; 6.000 tek kart harcaması olarak yazıldı.
- *Koşum kaydı* · **Goodbudget** — Split into multiple Envelopes tutarı zarflara böler, zamana yaymaz. Kart hesabı ücretsiz pakette açılamadı.
- *Görülmedi* · **Hesap Defterim** — Taksit yüzeyi görülmedi.

## 7.3 · Bekleyen işler hangi listede?

Bluecoins bekleyen bütün kalemleri tek bir tarih listesinde topluyor; Wallet her planı tek satırda sıradaki vadesiyle gösteriyor; Money Manager gelecek ayın listesinin üstüne bir satır koyuyor.

**Şekil 7.11 · Bluecoins · Hatırlatıcılar** (E0047)

1. Hatırlatıcılar, ana ekranın sekmelerinden biri.
2. Durum sözcükle: Bugün süresi doluyor.
3. Taksit satırında sayaç: 2 / 6.

**Şekil 7.12 · Bluecoins · Hatırlatıcılar, ertesi gün** (E0056)

4. Ertesi gün aynı kayıt: Dün bitti.

**Şekil 7.13 · Wallet · Planned payments** (E0306)

5. Plan başına tek satır: sıklık, tutar, sıradaki vade.
6. Alt sayfada All / Income / Expense / Transfer.

**Şekil 7.14 · Money Manager · Ekim günlük listesi** (E0244)

7. Tekrarlama satırı: 10/10, −600; ay toplamları 0.

- Bluecoins listesinde tek seferlik kira, aylık abonelik ve taksitler aynı tarih sırasında; ertesi günün kirası bir gün önce "Yarın borçlanacak" etiketini taşıyor.
- Bluecoins'te süresi geçen hatırlatıcı kendiliğinden kayda dönmüyor; kaydı kullanıcı yapıyor (→ 7.2).
- Wallet plan ayrıntısında bekleyen örnek Due today ve Confirm düğmesiyle duruyor (→ 7.5). Listenin araç çubuğunda arama ve sıralama var: vadeye ya da ada göre.
- Wallet Records menüsünde Show planned payments seçeneği var; sonucu denenmedi.
- Money Manager'da Ayarlar altında ayrı bir Tekrarlayan İşlemler listesi var; taksit planı bu listede yok.
- Planların hangi toplamda göründüğü → Belge 2 5.3, 5.4.

**Aynı soruda diğer ürünler**

- *Görülmedi* · **Goodbudget, Hesap Defterim** — Ayrı bir bekleyen liste görülmedi.

## 7.4 · Ana ekranda bekleyen iş

İncelenen beş canlı üründen yalnız Wallet'ın ana ekranında bekleyen ödemelere ayrılmış bir alan görüldü: üstte bir kısayol, aşağıda bir kart. Diğer dört ürün için bu, ana ekran karesiyle sınırlı bir gözlemdir.

**Şekil 7.15 · Wallet · Home, üst kısım** (E0293)

1. Hesap kartlarının altında Cash-flow ve Planned payments kısayolları.
2. Kısayolların altında tanıtım kartları.

**Şekil 7.16 · Wallet · Home, aşağısı** (E0469)

3. Upcoming planned payments kartı.
4. Satırda plan adı, kategori, tutar ve vade.
5. Add more cards: ana ekrana kart ekleme.

- İki kare aynı ana ekranın üstü ve aşağısıdır; farklı anlarda çekildi.
- Kart yüklenirken satırların yerinde iskelet bloklar duruyor (→ 3.5).
- Kısayolun açtığı liste → 7.3.

**Aynı soruda diğer ürünler**

- *Görülmedi* · **Money Manager, Hesap Defterim, Goodbudget** — İncelenen ana ekran karelerinde bekleyen iş alanı görünmüyor.
- *Görülmedi* · **Bluecoins** — İncelenen Hesaplar sekmesi karesinde görünmüyor; bekleyenler aynı sekme çubuğundaki Hatırlatıcılar'da (→ 7.3).
- *Kaynak görseli* · **KolayBi** — Güncel Durum ekranında Günü Gelen İşlemler: Bugün, Yaklaşanlar, Tarihi Geçenler (→ 2.7).
- *Kaynak beyanı* · **Paraşüt** — Tanıtım anlatımında ana ekran tahsilatı ve ödemeyi vade durumuna göre ayırıyor (→ 2.7).

## 7.5 · Onay ve otomatik tercihi

Wallet'ta bekleyen örnek Confirm ile kayda dönüşüyor. İlk onaydan sonra ürün, sonraki örneklerin otomatik mi onayla mı kaydedileceğini soruyor.

**Şekil 7.17 · Wallet · Payment summary** (E0290)

1. Onaydan önce tarih, hesap ve tutar açılır alan olarak.

**Şekil 7.18 · Wallet · ilk onaydan sonra** (E0291)

2. Yes (Recommended) önceden seçili; yalnız Confirm.

**Şekil 7.19 · Wallet · aynı soru, sonradan açıldı** (E0318)

3. Kayıtlı seçim No; Cancel eklenmiş.

**Şekil 7.20 · Wallet · onaydan sonra plan ayrıntısı** (E0292)

4. Sıradaki örnek: Due in 30 days, Confirm.
5. Gerçekleşen örnek: Paid Today.

- Bekleyen örneğin üç nokta menüsünde Postpone ve Dismiss var; sonuçları denenmedi.
- No seçildi; Yes kolunun sonucu, vadeden önce onay ve değiştirilmiş tutarla onay görülmedi.
- Sorunun plan başına mı hesap başına mı geçerli olduğu metinde yazmıyor.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Bluecoins** — Otomatik kayıt kurulum formunda bir işaret kutusu (→ 7.1). Kutu kapalıyken gecikmiş hatırlatıcı kaydedilirken tarih soruluyor: bugün mü, vade tarihi mi.
- *Canlı kare* · **Money Manager** — Onay kurulumda bir kez (→ 7.1); örnek başına onay yok. Ne zaman uygulanacağını bir ayar belirliyor: Tarihte ya da Her ayın ilk günü. Vadesi gelmemiş tekrarlar ay listesinde ayrı bir Tekrarlama bloğunda (→ 7.3).
- *Koşum kaydı* · **Goodbudget** — Sonraki örneğin ne zaman ve onayla mı üretildiği bilinmiyor.
- *Görülmedi* · **KolayBi** — Görülen formda onay veya otomatik tercihi yok (→ 7.1).

## 7.6 · Bütçe ve hedef yüzeyi

Wallet bütçe ile hedefi aynı sekmede tutuyor. Bütçe ayrıntısı harcananı, bir tahmini ve günlük ortalamayı gösteriyor; hedef formu tutar ve tarih soruyor.

**Şekil 7.21 · Wallet · bütçe ayrıntısı** (E0302)

1. Harcanan 6.600; bütçe 5.000; kırmızı çubuk.
2. Forecasted Spend: 30 gün için 6.600.
3. Günlük ortalama harcama.
4. Geçen dönemin aynı günlerine göre +222%.

**Şekil 7.22 · Wallet · New goal** (E0304)

5. Target amount, Saved already, Desired date.
6. Desired date varsayılanı bugün.

**Şekil 7.23 · Wallet · Budgets & Goals** (E0305)

7. Bütçe kartı: −1.600,00 / 5.000,00, Over Budget.
8. Hedef satırı: ₺0 ve %0.

- Bütçe formu ad, dönem ve tutar soruyor; kategori, hesap ve etiketle daraltılabiliyor.
- Bütçe aşılınca alt bantta aşım uyarısı çıktı.
- Forecasted Spend bu karede harcanan tutara eşit; hesaplama yöntemi bilinmiyor.
- Hedefin ayrıntısında 0 / 20.000 ₺ ve %0; altında Add saved amount ve Set goal as reached.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Bluecoins** — Bütçe Özeti kartının ayrıntısı kategori başına güncel tutarı ve bütçeyi gösteriyor; incelenen veride hiçbir kategoriye bütçe kurulmamış. Kurulumun girişi Kategoriler ekranındaki Bütçe Kur düğmesi; kurulum formu açılmadı.
- *Canlı kare* · **Money Manager** — Bütçe kategori başına; Toplam sekmesinde bir blok olarak görünüyor. Bütçe Ayarları varsayılan tutar ve ay ay değişiklik alıyor; değişiklik önümüzdeki aydan geçerli.
- *Görülmedi* · **Hesap Defterim** — Bütçe veya hedef yüzeyi görülmedi.

## 7.6 · Zarf satırı ve dönem sonu tahmini

Goodbudget'ta bütçe ana ekranın kendisi: her zarf satırı bir tutar ve bir ilerleme çubuğu taşıyor. KolayBi'nin nakit akış raporu güncel bakiyenin yanında bir dönem sonu tahmini veriyor.

**Şekil 7.24 · Goodbudget · ENVELOPES** (E0115)

1. Üstte dağıtılan toplam.
2. Zarf satırı: iki tutar ve ilerleme çubuğu.
3. [Available]: dağıtılmamış tutar satırı.

**Şekil 7.25 · KolayBi · Nakit Akış Raporu** (E0216)

4. Güncel Bakiye, Tahsilatlar, Ödemeler.
5. Tahmini Dönem Sonu Bakiyesi.
6. Belirsiz ve Geçmiş kovaları, ardından on iki ay.

- Goodbudget'ta hesap katmanı varsayılan olarak kapalı; zarf kalanı hesap bakiyesinden ayrı bir gösterge (→ 5.1).
- Goodbudget satırındaki iki tutarın hangisinin kalan, hangisinin bütçelenen olduğu karede yazmıyor; koşum kaydına göre üstteki kalan, alttaki bütçelenen.
- KolayBi karesinde bütün tutar Belirsiz kovasında; aylık kovalar boş. Tahminin formülü kaynakta tanımlanmıyor.
- KolayBi demo verisi 2023–2024 tarihli; güncel sürüm doğrulanmadı.

## Özet: hangi yüzey nerede görüldü?

Yeni kanıt yok; önceki sayfaların satır satır dökümü. Yokluk ifadeleri incelenen sürüm, yüzey ve kaynakla sınırlıdır.

| Ürün | Tekrar | Taksit | Bekleyen liste | Ana ekranda | Onay / otomatik | Bütçe / hedef |
|---|---|---|---|---|---|---|
| **Canlı incelenen ürünler** | | | | | | |
| Money Manager | *Canlı kare* · Rozet; 14 sıklık (7.1). | *Canlı kare* · Rozet; başlıkta (1/6) (7.2). | *Canlı kare* · Gelecek ayda Tekrarlama satırı; ayarlarda tekrar listesi (7.3). | Karede yok (7.4). | *Canlı kare* · Kurulumda tek onay; uygulanma zamanı ayarda (7.5). | *Canlı kare* · Kategori başına bütçe (7.6). |
| Bluecoins | *Canlı kare* · Alt sayfa; bitiş seçenekleri (7.1). | *Canlı kare* · Oran, sayı, ilk ödeme; özet (7.2). | *Canlı kare* · Hatırlatıcılar; sözcükle durum, sayaç (7.3). | Karede yok; ayrı sekme (7.4). | *Canlı kare* · Otomatik kutusu; tarih sorusu (7.5). | *Canlı kare* · Bütçe Özeti kartı; Bütçe Kur girişi (7.6). |
| Wallet | *Canlı kare* · Planned payment formu (7.1). | İncelenen yollarda yok (7.2). | *Canlı kare* · Planned payments; sıradaki vade (7.3). | *Canlı kare* · Kısayol ve kart (7.4). | *Canlı kare* · Özet, otomatik/onaylı sorusu (7.5). | *Canlı kare* · Bütçe ayrıntısı; hedef formu (7.6). |
| Hesap Defterim | Çekmece, ayarlar, form tarandı (7.1). | Taksit planı kurulamadı (7.2). | Ayrı liste bulunmadı (7.3). | Karede yok (7.4). | — | Yüzey bulunmadı (7.6). |
| Goodbudget | *Canlı kare* · Schedule this…; e-posta satırı (7.1). | *Koşum kaydı* · Zarflara bölme, zamana değil (7.2). | Ayrı liste bulunmadı (7.3). | Karede yok (7.4). | *Koşum kaydı* · Bilinmiyor (7.5). | *Canlı kare* · Zarf satırı ve çubuk (7.6). |
| **Kaynakla incelenen ürünler** | | | | | | |
| KolayBi | *Kaynak görseli* · Kayıttan dönüştürme (7.1). | — | — | *Kaynak görseli* · Günü Gelen İşlemler (7.4). | Formda yok (7.5). | *Kaynak görseli* · Dönem sonu tahmini (7.6). |
| Paraşüt | — | — | — | *Kaynak beyanı* · Vadeye göre ayrım (7.4). | — | — |

- Logo İşbaşı ve QuickBooks Solopreneur için bu bölümün sorularında karşılaştırılabilir kaynak anlatımı bulunmadı; bu, ürünlerde özellik olmadığı anlamına gelmez.
- Boş hücre (—): o soru bu ürün için incelenmedi ya da önceki sütunlarda yüzey bulunmadığı için sorulamadı.

## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 7.1 | E0241 | Money Manager | Canlı kare | Money Manager · Gider formu |
| 7.2 | E0039 | Bluecoins | Canlı kare | Bluecoins · planlı işlem alt sayfası |
| 7.3 | E0288 | Wallet | Canlı kare | Wallet · Add Planned payment |
| 7.4 | E0133 | Goodbudget | Canlı kare | Goodbudget · Add Transaction |
| 7.5 | E0204 | KolayBi | Kaynak görseli | KolayBi · Çalışan Maaşı, İşlemler menüsü açık |
| 7.6 | E0205 | KolayBi | Kaynak görseli | KolayBi · Tekrarlı Maaş Oluştur |
| 7.7 | E0245 | Money Manager | Canlı kare | Money Manager · Gider formu, taksitli |
| 7.8 | E0246 | Money Manager | Canlı kare | Money Manager · Ağustos günlük listesi |
| 7.9 | E0034 | Bluecoins | Canlı kare | Bluecoins · Taksit şartlarını seçin |
| 7.10 | E0035 | Bluecoins | Canlı kare | Bluecoins · formdaki özet |
| 7.11 | E0047 | Bluecoins | Canlı kare | Bluecoins · Hatırlatıcılar |
| 7.12 | E0056 | Bluecoins | Canlı kare | Bluecoins · Hatırlatıcılar, ertesi gün |
| 7.13 | E0306 | Wallet | Canlı kare | Wallet · Planned payments |
| 7.14 | E0244 | Money Manager | Canlı kare | Money Manager · Ekim günlük listesi |
| 7.15 | E0293 | Wallet | Canlı kare | Wallet · Home, üst kısım |
| 7.16 | E0469 | Wallet | Canlı kare | Wallet · Home, aşağısı |
| 7.17 | E0290 | Wallet | Canlı kare | Wallet · Payment summary |
| 7.18 | E0291 | Wallet | Canlı kare | Wallet · ilk onaydan sonra |
| 7.19 | E0318 | Wallet | Canlı kare | Wallet · aynı soru, sonradan açıldı |
| 7.20 | E0292 | Wallet | Canlı kare | Wallet · onaydan sonra plan ayrıntısı |
| 7.21 | E0302 | Wallet | Canlı kare | Wallet · bütçe ayrıntısı |
| 7.22 | E0304 | Wallet | Canlı kare | Wallet · New goal |
| 7.23 | E0305 | Wallet | Canlı kare | Wallet · Budgets & Goals |
| 7.24 | E0115 | Goodbudget | Canlı kare | Goodbudget · ENVELOPES |
| 7.25 | E0216 | KolayBi | Kaynak görseli | KolayBi · Nakit Akış Raporu |
