# Bölüm 5 · Hesap, kart ve transfer

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Para nerede duruyor, ürün bunu nasıl gösteriyor ve hesaplar arasında nasıl geçiriliyor?

Bu bölüm hesap listesini, hesap açarken sorulan açılış alanını, kredi kartının yüzeyini, kart borcunun gösterimini, kart ödemesinin nereden başladığını ve transferin formda ve listede nasıl göründüğünü izler.

Yokluk ifadeleri incelenen sürüm ve yüzeyle sınırlıdır. Bir kaydın bakiyeye ve toplamlara etkisi bu bölümde anlatılmaz; ekranda görünen alan, etiket ve düğme anlatılır.

**Bu bölüme girmez**

- Kart harcamasının rapora etkisi → 9 ve Belge 2
- Taksitli kart harcaması → 7
- Cari / karşı taraf hesabı → 8
- Transferin toplamlara etkisi → Belge 2
- Banka bağlantısı → 10

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare |  |
| Bluecoins | Canlı kare | Ekstre davranışı doğrulanmadı. |
| Wallet | Canlı kare | Yeni hesap ücretsiz pakette açılamıyor; açılış alanı düzenleme formunda yok. |
| Hesap Defterim | Canlı kare |  |
| Goodbudget | Canlı kare | Tek hesap sınırı; kart ve transfer denenemedi. |
| KolayBi | Kaynak görseli | Tablolar boş; davranış yok. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Görülmedi | Hesap ve kart yüzeyi görülmedi. |
| QuickBooks Solopreneur | Görülmedi | Hesap ve kart yüzeyi görülmedi. |


## 5.1 · Hesap listesi nasıl sunuluyor?

Money Manager, Bluecoins ve Goodbudget hesapları türe göre grupluyor; Wallet renkli kartlar kullanıyor, Hesap Defterim yalnız defter adlarını diziyor.

**Şekil 5.1 · Hesaplar** (E0235)

- Gruplar: Nakit, Banka Hesapları, Kredi Kartı.
- Üstte Varlıklar, Borçlar, Toplam.
- Kart grubunda iki dönem sütunu (→ 5.4).

**Şekil 5.2 · Hesaplar** (E0027)

- Üst başlıklar: VARLIKLAR, CARİ HESAP.
- Grup satırında grup toplamı.
- Hesabın altında para birimi.

**Şekil 5.3 · Home › Accounts** (E0283)

- Hesaplar iki sütunlu renkli kartlar.
- Kartta hesap adı ve bakiye.
- Toplam satırı bu karede yok.

**Şekil 5.4 · Hesaplar** (E0146)

- Yalnız defter adları; bakiye yok.
- Hesap türü yok.
- Varsayılan defter de listede.

**Şekil 5.5 · ACCOUNTS, ilk hâl** (E0111)

- Hesap katmanı kapalı geliyor.
- TURN ON ACCOUNTS ile açılıyor.

**Şekil 5.6 · Edit Accounts** (E0112)

- Gruplar: Checking, Savings, or Cash · Credit Card · Debt.
- Grup adında hesap sayısı.

- Money Manager ve Bluecoins liste başında ya da grup satırında toplam gösteriyor; Wallet ve Hesap Defterim'in bu karelerinde toplam satırı yok.
- Goodbudget'ta bütçe zarfları hesap olmadan çalışıyor; hesaplar isteğe bağlı bir katman.
- Bluecoins listesinin üstünde Nakit Akım Ayarı girişi var (→ 9.5).
- Money Manager'da hesap toplama dahil edilmeyince satırı gri görünüyor (→ 9.5).

## 5.2 · Hesap açma ve açılış bakiyesi alanı

Money Manager, Bluecoins ve Hesap Defterim açılışı hesap formunda soruyor, ama üç ayrı biçimde: tek tutar ve sonradan gelen bir soru, tutar ile tarih yan yana, isteğe bağlı tutar ile işaret ve tarih.

**Şekil 5.7 · Money Manager · hesap formu ve fark sorusu** (E0239)

1. Hesap formunda tek Tutar alanı.
2. Toplama Dahil Et anahtarı (→ 9.5).
3. Kaydedince soru: fark İşlemler'de gösterilsin mi?

**Şekil 5.8 · Money Manager · hesabın defteri** (E0240)

4. Fark defterde Bakiye Farkı satırı olarak.
5. Satır kaydın yapıldığı günün tarihiyle.

**Şekil 5.9 · Bluecoins · Yeni Hesap** (E0048)

6. Başlangıç bakiyesi.
7. Açılış tarihi aynı formda.

**Şekil 5.10 · Hesap Defterim · Hesap Eklem** (E0145)

8. Açılış bilançosu isteğe bağlı.
9. Artı / eksi seçimi.
10. Açılışın tarihi.

- Açılışın hangi toplama girdiği bu bölümün konusu değildir (→ Belge 2 2.1).

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Wallet** — Hesap eklerken önce tür soruluyor: Bank Sync, Investments, File Import, Manual Input. Ücretsiz pakette dördüncü hesap premium duvarına çarpıyor. Mevcut hesabın düzenleme formunda açılış alanı yok; hesap detayında bir bakiye düzenleme girişi var (→ Belge 2 2.1).
- *Görülmedi* · **Goodbudget** — Hesap ekleme formu görülmedi.

## 5.3 · Kredi kartı yüzeyi

Beş canlı üründe beş farklı kart sunumu ve erişim durumu var: ekstre dönemli form, dönem alanları olan form, dönemsiz form, kart kavramı olmayan defter ve açılamayan kart hesabı.

**Şekil 5.11 · Money Manager · kredi kartı hesabı** (E0233)

1. Tür: Kredi Kartı.
2. Kaynak: ödemenin yapılacağı hesap.
3. Hesap Kesim Tarihi ve Son Ödeme Tarihi.
4. Bu Ay ve Gelecek Ay dönemleri, ödeme tarihleriyle.

**Şekil 5.12 · Wallet · Edit account** (E0298)

5. Type: Credit card.
6. Limit alanı; bu kartta 0.
7. Available Credit gösterimi.
8. Tek Payment Due Date; Not set.

**Şekil 5.13 · Goodbudget · hesap sınırı** (E0114)

9. Credit Card grubu var, boş.
10. Hesap sınırına ulaşıldı.
11. Ücretsiz pakette tek hesap; kart hesabı açılamadı.

- Wallet formunda kesim günü ya da ekstre alanı görülmedi. Payment Due Date ayın gününü bir tekerlekle seçtiriyor; neyi tetiklediği görülmedi.
- Goodbudget'a erişememek ürünün kart yüzeyi hakkında bir hüküm değildir.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Bluecoins** — Kart hesabı formunda Kredi Limiti, Hesap Kesim Günü ve Bitiş tarihi (Şekil 5.9). Ekstre davranışı doğrulanmadı.
- *Canlı kare* · **Hesap Defterim** — Hesap formunda tür alanı yok; kart, eksiye giden bir defter olarak tutuluyor.

## 5.4 · Kart borcu nasıl gösteriliyor?

Money Manager kart borcunu iki dönem sütununa ayırıyor ve kartın defterinde satır başına bakiye tutuyor. Wallet kartı eksi bakiyeli bir hesap olarak gösteriyor ve eşiği geçince uyarıyor.

**Şekil 5.14 · Money Manager · Hesaplar** (E0236)

1. Borçlar üst satırda ayrı.
2. Kart grubunda Bu Ay ve Gelecek Ay sütunları.
3. Borç Bu Ay sütununda.

**Şekil 5.15 · Money Manager · kartın defteri** (E0255)

4. Faturalama dönemi başlıkta; altında Para Yatırma, Çekme, Toplam, Bakiye.
5. Satırda o kayıttan sonraki bakiye.
6. Ödeme düğmesi (→ 5.5).

**Şekil 5.16 · Wallet · Home** (E0296)

7. Kart eksi bakiyeyle, diğer hesaplarla aynı düzende.
8. Bakiye eşiğin altına inince uyarı.

- İncelenen beş canlı üründen yalnız Money Manager kart borcunu iki dönem sütununda gösteriyor.
- Uyarının metni "minimum threshold" diyor. Hesap düzenleme formunda "Minimum balance — Get notified when balance drops below this amount" anahtarı var (açık, tutar 0.00); formun hangi hesaba ait olduğu karede okunmuyor, bağ kurulmadı.
- Bu Ay tutarının ödenmesi gereken tutarla ilişkisi → Belge 2 3.7.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Bluecoins** — Kart hesabı eksi bakiyeyle listeleniyor; ekstre görünümü görülmedi.
- *Canlı kare* · **Hesap Defterim** — Kartın defteri eksi dengeyle.
- *Görülmedi* · **Goodbudget** — Kart hesabı açılamadı (→ 5.3).

## 5.5 · Kart ödemesi nereden başlıyor?

İncelenen beş canlı üründen yalnız Money Manager'da karta özel bir ödeme düğmesi var (Şekil 5.15). Bluecoins ve Wallet'ta kart ödemesi hesaplar arası transferle yapılıyor.

**Şekil 5.17 · Money Manager · Ödeme'nin açtığı form** (E0237)

1. Form Havale türünde açılıyor.
2. Tutar o anki borçla aynı.
3. Kaynak Ana Hesap, Giriş kart.
4. Not kendiliğinden: Ödeme Bilgisi.

**Şekil 5.18 · Money Manager · tutarı değiştirilmiş ödeme** (E0248)

5. Tutar 400 olarak değiştirilmiş.

**Şekil 5.19 · Bluecoins · işlem listesi** (E0050)

6. Ödeme bağlı iki transfer satırı.
7. İkinci satırda kartın yeni bakiyesi.

**Şekil 5.20 · Wallet · transfer formu** (E0333)

8. Tutar serbest.
9. From → To.
10. Hedef hesaptaki tutar ayrıca yazıyor.

- Money Manager'da ödeme formu borç tutarıyla ön doldurulmuş açılıyor ve tutar değiştirilebiliyor.
- Ödemenin hangi dönemin borcuna uygulandığı → Belge 2 3.6.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Hesap Defterim** — Kart ödemesi de defterler arası Aktar ile (→ 5.6).
- *Görülmedi* · **Goodbudget** — Kart hesabı olmadığı için denenmedi.

## 5.6 · Transfer formu

Transfer beş canlı üründe ayrı bir form ya da ayrı bir tür. Money Manager'ın Havale formu 5.5'te (Şekil 5.17).

**Şekil 5.21 · Bluecoins · TRANSFER formu** (E0029)

1. Kaynak ve hedef hesap alt alta.
2. Yön değiştirme simgesi.
3. Transfer ücreti alanı.
4. TRANSFER seçili.

**Şekil 5.22 · Hesap Defterim · Aktar** (E0147)

5. Miktar.
6. Kimden / Kime.
7. Notlar; kategori alanı yok.

**Şekil 5.23 · Goodbudget · Account Transfer** (E0119)

8. From ve To.
9. Açıklama kendiliğinden: Account Transfer.
10. Schedule this… burada da var.

**Şekil 5.24 · Wallet · hızlı form, TRANSFER** (E0327)

11. From → To.

- Bluecoins'te ücret açılınca ayrı bir blok çıkıyor: kendi tutarı, kendi hesabı ve kendi kategorisi (varsayılan Diğer).
- Wallet'ın hedef hesap seçicisinde dördüncü seçenek "…outside of Wallet"; kullanılmadı.
- Goodbudget'ın transfer formunda zarf alanı görülmüyor.
- Goodbudget'ta tek hesap olduğu için gerçek bir transfer yapılamadı.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Money Manager** — Havale formunda tutarın yanında Harç düğmesi: aktarım ücreti formda ayrı bir giriş.

## 5.6 · Transfer listede nasıl görünüyor?

Money Manager transferi tek bir satırda gösteriyor. Bluecoins, Wallet ve Hesap Defterim iki satır yazıyor: biri çıkan, biri giren hesap için.

**Şekil 5.25 · Money Manager · İşlemler** (E0230)

1. Tek satır: Ana Hesap → Ortak Cuzdan.
2. Gün başlığında gelir ve gider 0.

**Şekil 5.26 · Bluecoins · İşlemler** (E0030)

3. Gün başlığı ₺0,00.
4. İki bağlı satır: eksi ve artı.
5. Satırda işlem sonrası hesap bakiyesi.

**Şekil 5.27 · Wallet · Records** (E0285)

6. Hafta toplamı ₺0.
7. Kart ödemesi iki satır: artı ve eksi.
8. İki satır da Transfer, withdraw adıyla.

**Şekil 5.28 · Hesap Defterim · İşlemler-Bütün Hesaplar** (E0140)

9. Hesaplar sütunu satırın defterini yazıyor.
10. Aktar iki satır: Kime ve Kimden.

- Tek satırlı gösterimde iki hesap aynı satırda okla yazılıyor; iki satırlı gösterimde her hesap kendi satırında.
- Transferin toplamlara etkisi → Belge 2 2.3, 2.4.

**Aynı soruda diğer ürünler**

- *Görülmedi* · **Goodbudget** — Transfer yapılamadığı için listede görünümü görülmedi.

## 5.7 · Kaynakta görülen hesap ve kart yüzeyleri

KolayBi'nin destek sayfalarında Finans modülü altı sekmeyle görünüyor ve kredi kartı formu canlı ürünlerde görülmeyen alanlar taşıyor. Alan adlarıyla kanıtlı; tablolar boş ya da demo.

**Şekil 5.29 · KolayBi · Finans › Banka Hesapları** (E0206)

1. Finans altında altı sekme.
2. Hesap ekleme ve dışa aktarma.
3. Listede Açılış Tarihi ve Bakiye kolonları.
4. Toplam ve para birimi bakiyesi üstte.

**Şekil 5.30 · KolayBi · Yeni Kredi Kartı** (E0208)

5. Kart numarası zorunlu.
6. Hesap Kesim Günü ve Son Ödeme Günü zorunlu.
7. Kart Limiti ve Minimum Ödeme Oranı zorunlu.
8. Detay ekle kapalı.

- Kredi Kartları listesinde Kalan Limit kolonu var; tablo boş, hesaplanışı görülmedi.
- Bir tanıtım videosu karesinde Finans altında dört sekme vardı; ötekilerin ekleniş zamanı bilinmiyor. Video karesi basılmadı.

**Karesi basılmayan ürünler**

- *Kaynak beyanı* · **Paraşüt** — Kasa ve bankalar tek listede; IBAN ve döviz sütunu ve banka hesabı bağlama girişi.
- *Görülmedi* · **Logo İşbaşı, QuickBooks Solopreneur** — Hesap ve kart yüzeyi görülmedi.

*Dayanak.* Kaynak beyanı: E0011.

## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 5.1 | E0235 | Money Manager | Canlı kare | Hesaplar |
| 5.2 | E0027 | Bluecoins | Canlı kare | Hesaplar |
| 5.3 | E0283 | Wallet | Canlı kare | Home › Accounts |
| 5.4 | E0146 | Hesap Defterim | Canlı kare | Hesaplar |
| 5.5 | E0111 | Goodbudget | Canlı kare | ACCOUNTS, ilk hâl |
| 5.6 | E0112 | Goodbudget | Canlı kare | Edit Accounts |
| 5.7 | E0239 | Money Manager | Canlı kare | Money Manager · hesap formu ve fark sorusu |
| 5.8 | E0240 | Money Manager | Canlı kare | Money Manager · hesabın defteri |
| 5.9 | E0048 | Bluecoins | Canlı kare | Bluecoins · Yeni Hesap |
| 5.10 | E0145 | Hesap Defterim | Canlı kare | Hesap Defterim · Hesap Eklem |
| 5.11 | E0233 | Money Manager | Canlı kare | Money Manager · kredi kartı hesabı |
| 5.12 | E0298 | Wallet | Canlı kare | Wallet · Edit account |
| 5.13 | E0114 | Goodbudget | Canlı kare | Goodbudget · hesap sınırı |
| 5.14 | E0236 | Money Manager | Canlı kare | Money Manager · Hesaplar |
| 5.15 | E0255 | Money Manager | Canlı kare | Money Manager · kartın defteri |
| 5.16 | E0296 | Wallet | Canlı kare | Wallet · Home |
| 5.17 | E0237 | Money Manager | Canlı kare | Money Manager · Ödeme'nin açtığı form |
| 5.18 | E0248 | Money Manager | Canlı kare | Money Manager · tutarı değiştirilmiş ödeme |
| 5.19 | E0050 | Bluecoins | Canlı kare | Bluecoins · işlem listesi |
| 5.20 | E0333 | Wallet | Canlı kare | Wallet · transfer formu |
| 5.21 | E0029 | Bluecoins | Canlı kare | Bluecoins · TRANSFER formu |
| 5.22 | E0147 | Hesap Defterim | Canlı kare | Hesap Defterim · Aktar |
| 5.23 | E0119 | Goodbudget | Canlı kare | Goodbudget · Account Transfer |
| 5.24 | E0327 | Wallet | Canlı kare | Wallet · hızlı form, TRANSFER |
| 5.25 | E0230 | Money Manager | Canlı kare | Money Manager · İşlemler |
| 5.26 | E0030 | Bluecoins | Canlı kare | Bluecoins · İşlemler |
| 5.27 | E0285 | Wallet | Canlı kare | Wallet · Records |
| 5.28 | E0140 | Hesap Defterim | Canlı kare | Hesap Defterim · İşlemler-Bütün Hesaplar |
| 5.29 | E0206 | KolayBi | Kaynak görseli | KolayBi · Finans › Banka Hesapları |
| 5.30 | E0208 | KolayBi | Kaynak görseli | KolayBi · Yeni Kredi Kartı |
