# Bölüm 4 · İşlem ekleme ve geri bildirim

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Kullanıcı yeni bir kaydı nasıl giriyor ve kaydettikten sonra ne görüyor?

Bu bölüm kayıt düğmesine basıldıktan sonrasını izler: formun nasıl açıldığı, hangi alanları hangi sırayla sorduğu, tür ve tutarın nasıl girildiği, art arda kayıt kolaylıkları, kaydetme sonrası ve hata anında ekranda ne göründüğü, kaydın nasıl düzeltilip silindiği.

Düğmenin ekrandaki yeri 2.5'tedir; bu bölüm oradan devam eder. Formlar yan yana dar basıldığında alan adları okunmadığı için beş form iki sayfaya bölündü.

**Bu bölüme girmez**

- Kaydın bakiyeye ve rapora etkisi → Belge 2
- Tekrar ve taksit formu → 7
- Kategori ve etiketin anlamı → 6
- Fiş eki ve fiş okuma → 10
- Form renkleri ve tutar biçimi → 3

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare, Koşum kaydı | Kayıttan sonra sessiz dönüş karesiz. |
| Bluecoins | Canlı kare, Koşum kaydı | Kayıttan sonra sessiz dönüş karesiz. |
| Wallet | Canlı kare, Koşum kaydı | Şablon ve split kaydedilmedi. |
| Hesap Defterim | Canlı kare, Koşum kaydı | Boş tutarın reddi ve deftere taşıma karesiz. |
| Goodbudget | Canlı kare, Koşum kaydı | Tebrik mesajı ve hesap makinesinin çalışması karesiz. |
| KolayBi | Kaynak görseli | Alan adları kanıtlı, davranış değil. |
| Paraşüt | Kaynak beyanı | Form ekranı görülmedi. |
| Logo İşbaşı | Kaynak beyanı | İç form görülmedi. |
| QuickBooks Solopreneur | Kaynak beyanı | Solopreneur formu görülmedi. |


## 4.1 · Form nereden açılıyor, kaç adımda?

Kayıt düğmesine basıldığında dört üründe form doğrudan açılıyor. Wallet araya bir menü koyuyor: normal kayıt, transfer ve şablon üç ayrı giriş.

**Şekil 4.1 · Wallet · kayıt düğmesinin açtığı menü** (E0324)

1. Create first template: şablon girişi.
2. Transfer: ayrı giriş.
3. New record: normal kayıt formu.
4. Menü açıkken düğme kapatma işaretine dönüyor.

- Wallet'ta normal bir kayda menüden bir seçim daha yapılarak ulaşılıyor.
- Hesap Defterim'de yön ana ekrandaki düğmeyle seçildiği için form o yönün adıyla açılıyor (→ 4.2).

**Aynı soruda diğer ürünler**

- *Koşum kaydı* · **Money Manager, Bluecoins, Goodbudget** — Düğme doğrudan kayıt formunu açıyor.
- *Canlı kare* · **Hesap Defterim** — Alındı ve Ödendi düğmeleri doğrudan o yönün formunu açıyor.

## 4.2 · Formda hangi alanlar var, hangi sırayla?

Money Manager az alanı alt alta diziyor; Bluecoins bütün seçenekleri tek yoğun ekranda topluyor. İşaretler alanların formdaki sırasını izliyor.

**Şekil 4.2 · Money Manager · gider formu ve kategori paneli** (E0229)

1. Tür segmenti formun en üstünde.
2. Tekrar / Taksit girişi tarih satırında.
3. Beş alan alt alta: Tarih, Tutar, Kategori, Hesap, Not.
4. Detay ve kamera girişi.
5. Kategori seçimi alttaki panelde açılıyor.

**Şekil 4.3 · Bluecoins · Ekle formu** (E0020)

6. Ad alanı en üstte.
7. Tarih, saat ve Planlı İşlemler aynı satırda.
8. Tutar; hesap makinesi ve para birimi yanında.
9. Kategori ve hesap alt alta.
10. Bölmek, Durum ve Etiket aynı ekranda.
11. Not alanı.
12. Tür seçici formun en altında.

**Şekil 4.4 · Bluecoins · aynı form, Durum açık** (E0448)

13. Durum alanı.
14. Dört değer: Yok, Kontrol, Mutabık, İptal edildi.
15. Belge yalnız üstteki ataçla ekleniyor; formda fatura bağlama alanı yok.

- Money Manager formunda kaydetme düğmesi, kategori paneli kapanınca görünüyor (→ 4.5).
- Bluecoins'te hesap olarak kredi kartı seçildiğinde aynı formda taksit alanı çıkıyor (→ 7.2).

## 4.2 · Formda hangi alanlar var? — Wallet, Hesap Defterim, Goodbudget

Wallet hızlı formda yalnız tutar, hesap ve kategori soruyor; Hesap Defterim'de kategori seçicisi yok; Goodbudget tutarın yanına zarfı ve hesabı koyuyor.

**Şekil 4.5 · Wallet · hızlı form** (E0277)

1. Tür sekmeleri: INCOME, EXPENSE, TRANSFER.
2. Tutar ekranın ortasında, büyük.
3. Yalnız hesap ve kategori.
4. Şablonlar girişi.

**Şekil 4.6 · Hesap Defterim · Alındı formu** (E0138)

5. Alındı / Ödendi seçici formda da var.
6. Tutar; hesap makinesi ikonu alanın içinde.
7. Notlar; mikrofonla sesli giriş.
8. Fatura ekle ve Öğe eklemek aynı satırda.
9. İki kaydetme düğmesi: çık ve devam et.

**Şekil 4.7 · Goodbudget · Add Transaction** (E0116)

10. Payee en üstte.
11. Tutar ve tür aynı satırda.
12. Envelope: zarf seçimi.
13. Account: hesap, bakiyesiyle.
14. Schedule this… kutusu.

- Wallet'ta not, etiket, karşı taraf, ödeme türü, durum, yer ve ek alanları kayıttan sonra açılan ayrıntı ekranında.
- Hesap Defterim'de kategori yerine ayarlardan açılan serbest bir Açıklama / Kategori metin kutusu var (→ 6.1).
- Goodbudget formunun alt kısmını klavye araç çubuğu örtüyor; Notes ve widget seçeneği kısmen okunuyor. Formda isteğe bağlı bir Check # alanı da var; formun menüsünde yalnız Help.

## 4.3 · Tür ve yön nasıl seçiliyor, varsayılan ne?

Tür seçicinin formdaki yeri ve seçili türün nasıl gösterildiği ürünler arasında ayrışıyor. "Form hangi türle açılıyor" ile "her kayıtta seçim gerekiyor mu" ayrı sorulardır; ikincisi hiçbir üründe doğrulanmadı.

- **Şekil 4.8 · Money Manager** (E0229) — *Formun üstü.* Gelir / Gider / Havale segmenti; seçili olan çerçeve ve renkle. Tam ekran: Şekil 4.2.
- **Şekil 4.9 · Wallet · gider** (E0277) — *Formun üstü.* EXPENSE seçili; ayrım yalnız zemin tonunda. Tam ekran: Şekil 4.5.
- **Şekil 4.10 · Wallet · transfer** (E0327) — *Formun üstü.* TRANSFER seçili; yine yalnız ton farkı.
- **Şekil 4.11 · Hesap Defterim** (E0138) — *Formun üstü.* Alındı / Ödendi çipleri; seçili olan dolu renkle. Tam ekran: Şekil 4.6.
- **Şekil 4.12 · Goodbudget** (E0116) — *Tutar satırının sağı.* Tür açılır listeden (Credit). Tam ekran: Şekil 4.7.
- **Şekil 4.13 · Bluecoins** (E0487) — *Formun altı.* GİDER / GELİR / TRANSFER; seçili olan dolu zemin. Yeni açılan form GİDER seçili; formun tamamı: Şekil 4.3.

- Money Manager, Bluecoins ve Hesap Defterim seçili türü dolu renk ya da çerçeveyle gösteriyor; Wallet yalnız zemin tonuyla, Goodbudget açılır listede adıyla.
- Hesap Defterim'de yön önce ana ekrandaki düğmeyle seçiliyor, formda yeniden değiştirilebiliyor.
- Wallet'ta gelir girilmek istenen bir kayıtta form gider türünde kaldı.

**Varsayılan tür**

- *Canlı kare* · **Bluecoins** — Yeni form GİDER seçili açılmış; tutarın yanında kırmızı eksi.
- *Canlı kare* · **Wallet** — Hızlı form Expense seçili açılmış.
- *Canlı kare* · **Money Manager** — Yeni form Gider seçili açılmış.
- *Canlı kare* · **Goodbudget** — Boş formda tür Expense; formun hangi türle açıldığı ayrıca doğrulanmadı.

## 4.4 · Tutar nasıl giriliyor?

Money Manager ve Wallet tutarı formun altındaki tuş takımıyla, Bluecoins ve Hesap Defterim alanın içindeki hesap makinesi ikonuyla alıyor; Goodbudget'ta hesap makinesi bir ayar. Wallet'ın planlı ödeme formunda hesap makinesi ayrı bir diyalog.

**Şekil 4.14 · Wallet · hızlı formun tuş takımı** (E0277)

1. Rakamlar ve işlemler aynı tuş takımında.
2. İşlem tuşları sağ sütunda.

**Şekil 4.15 · Wallet · planlı ödemede hesap makinesi diyaloğu** (E0339)

3. Diyalog formun üstünde açılıyor.
4. Sonuç Insert ile alana aktarılıyor.

**Şekil 4.16 · Bluecoins ve Hesap Defterim'de tutar alanının içinde; Money Manager'da tuş takımının sağ sütununda.** — Bluecoins (E0020), Hesap Defterim (E0138), Money Manager (E0463)

- Hesap Defterim'de tutar alanı odaktayken ekranda klavye araç çubuğu görünüyor; tuş takımının kendisi karede yok.
- İncelenen karelerde tutarın cihaz klavyesiyle yazıldığını gösteren bir kare yok.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Money Manager** — Form açılınca altta rakam tuş takımı; sağ sütundaki simge tam ekran bir hesap makinesi açıyor (AC, ÷, ×, −, +, =).
- *Koşum kaydı* · **Goodbudget** — Hesap makinesi ayarlarda açılıp kapanıyor (Use calculator to enter amounts); açıkken tutar alanı dört işlemli bir hesap makinesi açıyor (kullanıcı kontrolü).

## 4.5 · Art arda kayıt ve kolaylıklar

Beş üründe beş ayrı kolaylık kümesi görülüyor. Hesap Defterim en çoğunu ana formun içinde taşıyor; Wallet kaydı bölmeyi kayıttan sonraya bırakıyor.

**Şekil 4.17 · Hesap Defterim · Öğe eklemek** (E0155)

1. Kalem dökümü diyaloğu.
2. Öğe, miktar, birim ve fiyat girilip ekleniyor.
3. Kalemlerin toplamı üstte.

**Şekil 4.18 · Hesap Defterim · yeniden adlandırılmış düğmeler** (E0161)

4. Sütun başlıkları yeni adla.
5. Kalem dökümü kaydın notuna yazılmış.
6. Düğmeler de yeni adla.

**Şekil 4.19 · Wallet · Split record** (E0311)

7. Özgün kayıt üstte.
8. Split: kaydı parçalara bölme.

**Şekil 4.20 · Kaydet'in yanında formu açık tutan düğme (formun altı).** — Money Manager (E0232), Hesap Defterim (E0138)

- Hesap Defterim'de düğme ve sütun adlarını kullanıcı değiştirebiliyor; yeni ad toplam etiketlerine de geçiyor.
- Wallet'ta split kaydedilmedi; parçaların sonucu görülmedi.

**Aynı soruda diğer ürünler**

- *Canlı kare* · **Bluecoins** — Planlama, bölme ve kartta taksit aynı formda. Bölünmüş kayıt listede tek satır: "2 Kategoriler".
- *Canlı kare* · **Wallet** — Menüde şablon girişi; şablon kaydedilmedi.
- *Canlı kare* · **Goodbudget** — Kaydederken konuma göre payee önerisi için izin istiyor.
- *Canlı kare* · **Goodbudget** — Formda Quick Transactions widget'ına ekleme seçeneği.
- *Canlı kare* · **Hesap Defterim** — Not önerileri; ayarda açık.

## 4.6 · Kaydetme sonrası geri bildirim

Kaydettikten sonra üç ürün sessizce listeye dönüyor, Hesap Defterim kısa bir bildirim gösteriyor, Goodbudget bir tebrik mesajı veriyor.

**Şekil 4.21 · Hesap Defterim · kayıttan hemen sonra** (E0141)

1. İşlem Eklendi: kısa bildirim.
2. Yeni kayıt listede; satırında denge.

- Bildirim ekranın altında, toplam bandının üstünde beliriyor.

**Aynı soruda diğer ürünler**

- *Koşum kaydı* · **Money Manager, Wallet, Bluecoins** — Mesaj yok; form kapanıp listeye dönülüyor.
- *Koşum kaydı* · **Goodbudget** — Kayıttan sonra oyunlaştırılmış bir tebrik mesajı.

## 4.7 · Hata ve zorunlu alan

Eksik bilgide mesajın nereye konduğu ayrışıyor: ekranın altında kısa bir mesaj, alanın hemen altında kırmızı bir metin ya da hiç mesaj yok.

**Şekil 4.22 · Money Manager · hesap seçilmedi** (E0232)

1. Mesaj ekranın altında.
2. Boş alanın yanında mesaj yok.

**Şekil 4.23 · Wallet · sıfır tutar** (E0281)

3. Mesaj ekranın altında.
4. Tutar 0.

**Şekil 4.24 · Goodbudget · zarf seçilmedi** (E0117)

5. Mesaj ekranın altında.
6. Zarf alanı boş.

**Şekil 4.25 · Wallet · planlı ödemede kategori yok** (E0347)

7. Hata metni alanın hemen altında, kırmızı.

- Alan dışı mesaj: Money Manager, Wallet hızlı formu, Goodbudget.
- Alan yanında mesaj: Wallet planlı ödeme formu.
- Aynı üründe (Wallet) iki formda iki ayrı yaklaşım görülüyor.

**Aynı soruda diğer ürünler**

- *Koşum kaydı* · **Hesap Defterim** — Boş tutarla kaydetme mesaj vermeden sonuçsuz kaldı.
- *Canlı kare* · **Hesap Defterim** — Sıfır tutarlı kayıt listeye yazıldı.
- *Canlı kare* · **Bluecoins** — Sıfır tutarlı kayıt oluştu; detayı 4.8'de.
- *Canlı kare* · **Money Manager** — Tutarı boş bırakılan kayıt listeye ₺0,00 olarak girdi; uyarı yok.

## 4.8 · Düzeltme ve silme

Silme onayının karesi beş üründe var; Hesap Defterim'deki onay kalıcı silme adımında. Silineni geri almanın yüzeyi iki üründe görüldü: Hesap Defterim'in Silinmiş işlemler listesi ve Bluecoins'in Çöp Kutusu. Money Manager, Wallet ve Goodbudget'ın incelenen menü ve ayarlarında böyle bir kalem yok.

**Şekil 4.26 · Bluecoins · kayıt detayı ve silme onayı** (E0024)

1. Detayda yazdır ve sil simgeleri.
2. Benzerleri gösterme ve yineleme.
3. Silme onayı: İptal / TAMAM.

**Şekil 4.27 · Goodbudget · silme onayı** (E0125)

4. Düzenleme ekranında silme simgesi.
5. Onay: NO / YES.

**Şekil 4.28 · Hesap Defterim · Silinmiş işlemler** (E0167)

6. Silinmiş kayıtta Geri Yükle ve Silme.

**Şekil 4.29 · Bluecoins · Çöp Kutusu** (E0089)

7. Çekmeceden açılan ayrı ekran.
8. Boş; yönlendirme metni yok.

- Hesap Defterim'de kalıcı silme ikinci bir onay istiyor.
- Bluecoins'te geri yüklenen kaydın tutarı toplama döndü; satırın listeye gelmesi uygulama yeniden açılınca oldu (kullanıcı kontrolü).
- Money Manager'ın onayı: "Silmek istediğinize emin misiniz?" HAYIR / EVET.
- Wallet'ın onayı: "Do you really want to delete this item?" No / Yes. Silme kayıt ayrıntısının araç çubuğunda, bölme (split) ile yan yana.
- Geri alma kalemi aranan yerler: Money Manager'ın Daha ızgarası, Wallet'ın çekmecesi ve ayarları, Goodbudget'ın ayarları.

**Aynı soruda diğer ürünler**

- *Koşum kaydı* · **Money Manager, Wallet** — Satırdan ya da detaydan doğrudan düzenleme.
- *Koşum kaydı* · **Hesap Defterim** — Kaydı başka deftere taşıma ve kopyalama.

## 4.9 · Kaynakta görülen formlar

KolayBi'nin destek sayfalarındaki iki form, canlı ürünlerde görülmeyen alanları adıyla gösteriyor. Alan adları kanıtlı; seçimlerin sonucu görülmedi.

**Şekil 4.30 · KolayBi · Yeni Genel Gider** (E0192)

1. Cari Takibi: Yok / Var.
2. Gider Tipi zorunlu.
3. Ödeme Durumu: Ödenmedi / Ödendi.
4. Son ödeme tarihi boş bırakılabiliyor.
5. Proje alanı; ayarlardan kapatılabileceği notu.
6. Açıklamayı şablon olarak kaydetme.
7. Dosya yükleme; türler ve 5 MB sınırı.
8. Altta KDV bandı; iki ayrı Toplam KDV başlığı.

**Şekil 4.31 · KolayBi · Yeni Alış Faturası** (E0200)

9. Cari zorunlu.
10. Düzenleme saati zorunlu.
11. Ödeme Durumu burada da var.
12. Vade tarihi boş bırakılabiliyor.
13. Altta ürün / hizmet kalem tablosu başlıyor.

- Ödendi seçiminin kasaya, cariye ve rapora etkisi görülmedi (→ Belge 2 9.1).
- İki formun üst kısmı aynı: ödeme durumu, vade, proje, etiket, açıklama ve dosya.

**Karesi basılmayan ürünler**

- *Kaynak beyanı* · **Paraşüt** — Beş gider türü: detaylı ve hızlı fiş/fatura, maaş/prim, vergi/SGK, banka gideri. Kayıt ile ödeme ayrı adım.
- *Kaynak beyanı* · **Paraşüt** — Fiş fotoğrafından okuma (→ 10.3).
- *Kaynak beyanı* · **Logo İşbaşı** — Sesle fatura kesme; çalışırken görülmedi.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Kayıtlar ağırlıkla banka bağlantısından geliyor; kullanıcı türü ve kategoriyi inceliyor.

*Dayanak.* Kaynak beyanı: E0011, E0009, E0012.

## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 4.1 | E0324 | Wallet | Canlı kare | Wallet · kayıt düğmesinin açtığı menü |
| 4.2 | E0229 | Money Manager | Canlı kare | Money Manager · gider formu ve kategori paneli |
| 4.3 | E0020 | Bluecoins | Canlı kare | Bluecoins · Ekle formu |
| 4.4 | E0448 | Bluecoins | Canlı kare | Bluecoins · aynı form, Durum açık |
| 4.5 | E0277 | Wallet | Canlı kare | Wallet · hızlı form |
| 4.6 | E0138 | Hesap Defterim | Canlı kare | Hesap Defterim · Alındı formu |
| 4.7 | E0116 | Goodbudget | Canlı kare | Goodbudget · Add Transaction |
| 4.8 | E0229 | Money Manager | Canlı kare | Money Manager · Formun üstü |
| 4.9 | E0277 | Wallet | Canlı kare | Wallet · gider · Formun üstü |
| 4.10 | E0327 | Wallet | Canlı kare | Wallet · transfer · Formun üstü |
| 4.11 | E0138 | Hesap Defterim | Canlı kare | Hesap Defterim · Formun üstü |
| 4.12 | E0116 | Goodbudget | Canlı kare | Goodbudget · Tutar satırının sağı |
| 4.13 | E0487 | Bluecoins | Canlı kare | Bluecoins · Formun altı |
| 4.14 | E0277 | Wallet | Canlı kare | Wallet · hızlı formun tuş takımı |
| 4.15 | E0339 | Wallet | Canlı kare | Wallet · planlı ödemede hesap makinesi diyaloğu |
| 4.16 | E0020 | Bluecoins | Canlı kare | Bluecoins ve Hesap Defterim'de tutar alanının içinde; Money Manager'da tuş takımının sağ sütununda. |
| 4.16 | E0138 | Hesap Defterim | Canlı kare | Bluecoins ve Hesap Defterim'de tutar alanının içinde; Money Manager'da tuş takımının sağ sütununda. |
| 4.16 | E0463 | Money Manager | Canlı kare | Bluecoins ve Hesap Defterim'de tutar alanının içinde; Money Manager'da tuş takımının sağ sütununda. |
| 4.17 | E0155 | Hesap Defterim | Canlı kare | Hesap Defterim · Öğe eklemek |
| 4.18 | E0161 | Hesap Defterim | Canlı kare | Hesap Defterim · yeniden adlandırılmış düğmeler |
| 4.19 | E0311 | Wallet | Canlı kare | Wallet · Split record |
| 4.20 | E0232 | Money Manager | Canlı kare | Kaydet'in yanında formu açık tutan düğme (formun altı). |
| 4.20 | E0138 | Hesap Defterim | Canlı kare | Kaydet'in yanında formu açık tutan düğme (formun altı). |
| 4.21 | E0141 | Hesap Defterim | Canlı kare | Hesap Defterim · kayıttan hemen sonra |
| 4.22 | E0232 | Money Manager | Canlı kare | Money Manager · hesap seçilmedi |
| 4.23 | E0281 | Wallet | Canlı kare | Wallet · sıfır tutar |
| 4.24 | E0117 | Goodbudget | Canlı kare | Goodbudget · zarf seçilmedi |
| 4.25 | E0347 | Wallet | Canlı kare | Wallet · planlı ödemede kategori yok |
| 4.26 | E0024 | Bluecoins | Canlı kare | Bluecoins · kayıt detayı ve silme onayı |
| 4.27 | E0125 | Goodbudget | Canlı kare | Goodbudget · silme onayı |
| 4.28 | E0167 | Hesap Defterim | Canlı kare | Hesap Defterim · Silinmiş işlemler |
| 4.29 | E0089 | Bluecoins | Canlı kare | Bluecoins · Çöp Kutusu |
| 4.30 | E0192 | KolayBi | Kaynak görseli | KolayBi · Yeni Genel Gider |
| 4.31 | E0200 | KolayBi | Kaynak görseli | KolayBi · Yeni Alış Faturası |
