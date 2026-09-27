# Bölüm 3 · Görsel dil

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Ürün sayıyı, rengi ve durumu nasıl gösteriyor?

Bu bölüm önceki bölümlerin karelerini beş sabit soruyla yeniden okur: tutarın yazımı, rengin taşıdığı anlam, liste-kart-tablo-grafik seçimi, durum ve dönemin görünümü, boş, hata ve yükleme durumları.

Çoğu sayfa aynı bölgenin ürünlerdeki kırpıntısını alt alta dizer. Kırpıntının geldiği tam ekran ilgili bölümdedir.

**Bu bölüme girmez**

- Gezinme yapısı → 2
- Form alanlarının sırası → 4
- Raporun anlamı → 9
- Erişilebilirlik hükmü (ölçülmedi; hiçbir bölümde yazılmaz)

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare |  |
| Bluecoins | Canlı kare |  |
| Wallet | Canlı kare | İki ayrı tutar yazımı; nedeni bilinmiyor. |
| Hesap Defterim | Canlı kare |  |
| Goodbudget | Canlı kare |  |
| KolayBi | Kaynak görseli |  |
| Paraşüt, Logo İşbaşı, QuickBooks Solopreneur | Görülmedi | Görsel dil kanıtı basılmadı. |


## 3.1 · Tutar nasıl yazılıyor?

Para simgesi, ondalık ve binlik ayraç ürünler arasında farklı. Wallet aynı sürümün farklı günlerde alınan karelerinde iki ayrı yazım gösteriyor.

- **Şekil 3.1 · Money Manager** (E0235) — *Hesaplar listesi.* ₺ önde, iki ondalık, Türkçe ayraçlar; tutar sağa hizalı.
- **Şekil 3.2 · Bluecoins** (E0027) — *Hesaplar listesi.* ₺ önde, iki ondalık; grup tutarı renkli ve kalın.
- **Şekil 3.3 · Wallet · yazım 1** (E0276) — *Home, hesap kartları.* TRY 20,800.00: para kodu önde, İngilizce ayraçlar.
- **Şekil 3.4 · Wallet · yazım 2** (E0283) — *Home, hesap kartları.* ₺20.800,00: simge önde, Türkçe ayraçlar.
- **Şekil 3.5 · Hesap Defterim** (E0137) — *Ekranın altı.* Tam sayı, binlik ayraç, simge yok.
- **Şekil 3.6 · Goodbudget** (E0115) — *Zarf satırı.* 850.00: simge yok, İngilizce ayraçlar.
- **Şekil 3.7 · Goodbudget · tarih** (E0116) — *Kayıt formu.* 09/11/2026: tarih ay/gün/yıl sırasıyla.

- Wallet'taki iki yazımın nedeni (cihaz dili, hesap ayarı ya da sürüm içi değişiklik) doğrulanmadı. Ayarlardaki tek sayı biçimi seçeneği yalnız ondalığı açıp kapatıyor; kapalıyken simge ve ayraç aynı kalıyor.
- Money Manager kart borcunu Hesaplar'da işaretsiz ve kırmızı, kartın defterinde eksiyle yazıyor (→ 5.4).
- Goodbudget'ın rapor başlığında ise Türkçe ay adları var; aynı üründe iki yerelleştirme. Tarih sırası, ondalık basamağı ve ondalık işareti ayarlardan değişiyor (Date Order, Decimal Digits, Decimal Mark; yalnız o cihazda).
- Hesap Defterim'in ayarlarında bir "Para birimi biçimi" seçeneği var; seçenekleri açılmadı.

**Kaynaktan**

- *Kaynak görseli* · **KolayBi** — ₺19.543,53: simge önde, Türkçe ayraçlar.
- *Görülmedi* · **Paraşüt, Logo İşbaşı, QuickBooks Solopreneur** — Tutar yazımı görülmedi; görülen kareler iç arayüzü temsil etmiyor.

## 3.2 · Renk hangi anlamı taşıyor, yanında sözcük var mı?

Gelir ve gider renkle ayrılıyor; renklerin kendisi ürüne göre değişiyor. Hesap Defterim yönü rengin yanında sözcükle de yazıyor.

- **Şekil 3.8 · Money Manager** (E0228) — *Ana ekran özet satırı.* Gelir mavi, gider turuncu-kırmızı, toplam siyah; etiketler üstte.
- **Şekil 3.9 · Money Manager · havale** (E0230) — *İşlem listesi.* Havale satırının tutarı siyah.
- **Şekil 3.10 · Bluecoins** (E0030) — *İşlem listesi.* Gider kırmızı, gelir yeşil; satır simgesi de renkli.
- **Şekil 3.11 · Bluecoins · transfer** (E0030) — *İşlem listesi.* Transfer satırında mavi simge; bacakların tutarı gelir ve gider renginde.
- **Şekil 3.12 · Hesap Defterim** (E0137) — *Ekranın altı.* Alındı yeşil, Ödendi kırmızı; yön düğmede ve toplam etiketinde sözcükle de yazıyor.
- **Şekil 3.13 · Goodbudget** (E0494) — *İşlem listesi.* Gelir yeşil ve artı işaretli; gider koyu ve işaretsiz. Gidere kırmızı kullanılmıyor.

- Wallet'ta formdaki seçili tür yalnız zemin tonuyla ayrılıyor (→ 4.3).
- Wallet ve Bluecoins'te transferin iki bacağı gelir ve gider renginde; Money Manager havaleyi nötr renkle tek satırda gösteriyor (→ 5.6).

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Proje belge kartlarında iade başlıkları ters renkte ve −/+ işaretli.

## 3.3 · Liste, kart, tablo ve grafik nerede kullanılıyor?

Aynı türden bilgi ürünlerde farklı biçimlerde sunuluyor: sütun başlıklı tablo, takvim ızgarası, kart panosu, soru başlıklı grafik kartı, pasta ve ilerleme çubuğu.

**Şekil 3.14 · birleşik liste** (E0142)

- Sütun başlıklı tablo.
- Altta sabit toplam bandı.

**Şekil 3.15 · Takvim** (E0144)

- Takvim ızgarası; günün tutarı yön rengiyle.

**Şekil 3.16 · Hesaplar sekmesi** (E0103)

- Kart panosu; her kartın kendi başlığı ve menüsü.

**Şekil 3.17 · Statistics › Cash-flow** (E0280)

- Kart başlığı bir soru.
- Gelir ve gider yatay çubukla; altında eğilim grafiği.

**Şekil 3.18 · İstatistik** (E0231)

- Pasta grafik; altında yüzdeli liste.

**Şekil 3.19 · ENVELOPES** (E0115)

- Zarf satırında ilerleme çubuğu.

- Hesap Defterim'in incelenen yüzeylerinde grafik görülmedi; özet tablo ve takvimle veriliyor.
- Bluecoins'te panodaki kartlar Ana Ekran ayarından tek tek açılıp kapanıyor.
- Wallet'ın rapor kartlarında başlık bir soru olarak yazılmış; diğer ürünlerin incelenen raporlarında başlık ölçünün adı.

## 3.4 · Durum ve dönem nasıl görünüyor?

Bluecoins ve Wallet bekleyen kalemin durumunu sözcükle yazıyor. Seçili dönem ürünlerde çip, sekme, ay gezgini ya da başlıkta yazılı aralık olarak görünüyor. Seçili filtrenin görünümü 9.4'te.

- **Şekil 3.20 · Bluecoins · durum** (E0056) — *Hatırlatıcılar.* Dün bitti ve Bugün süresi doluyor: durum sözcükle ve renkle.
- **Şekil 3.21 · Bluecoins · sayaç** (E0056) — *Hatırlatıcılar.* Taksitli kayıtta kaçıncı taksit olduğu satırda (→ 7.3).
- **Şekil 3.22 · Hesap Defterim** (E0142) — *Ekranın üstü.* Dönem çipleri; seçili olan çerçeveli, altında tarih aralığı ve oklar.
- **Şekil 3.23 · Money Manager** (E0228) — *Ekranın üstü.* Ay gezgini; görünüm sekmelerinde seçili olan alt çizgiyle.
- **Şekil 3.24 · Wallet** (E0280) — *Rapor ekranının altı.* Aralık çipleri; ücretli aralıklar kilitli.
- **Şekil 3.25 · Goodbudget** (E0122) — *Rapor ekranının üstü.* Dönem başlıkta yazılı; değiştirme simgesi üst çubukta.

- Bluecoins'teki durum sözcükleri rengin yanında anlamı da taşıyor.
- Wallet'ın plan ayrıntısında da durum sözcükle yazıyor: Due today, Due in 30 days, Paid Today (→ 7.5).
- Bluecoins'in kayıt formunda bundan ayrı bir Durum alanı var: Yok, Kontrol, Mutabık, İptal edildi (→ 4.2).

## 3.5 · Boş, hata ve yükleme durumları

2.6 ve 4.7'nin kareleri burada biçim açısından okunur. Boş durum ya ne yapılacağını söylüyor ya da yalnız veri olmadığını; yükleme için tek kanıt Wallet'ın iskelet blokları.

**Şekil 3.26 · Wallet · ana ekranın aşağısı, yüklenirken** (E0274)

1. Kartın satırları yerine gri iskelet bloklar.

**Şekil 3.27 · Wallet · Planned payments, boş** (E0282)

2. Ne işe yaradığını söylüyor.
3. İlk kaydın yolunu gösteriyor.

**Şekil 3.28 · Goodbudget · boş rapor** (E0122)

4. Toplam 0.00.
5. Yalnız veri olmadığını söylüyor.

**Şekil 3.29 · Üçü ekranın altında kısa mesaj kutusu; sonuncusu alanın altında kırmızı metin.** — Money Manager (E0232), Wallet (E0281), Goodbudget (E0117), Wallet · planlı ödeme (E0347)

- Ne yapılacağını söyleyen boş durum: Wallet planlı ödemeler, Bluecoins temiz kurulum (→ 2.6).
- Yalnız veri olmadığını söyleyen: Money Manager "Veri yok.", Goodbudget raporu, Bluecoins'in boş Çöp Kutusu.
- Mesaj kutularının üçünde simge var; Goodbudget'ınki uygulama simgesiyle.
- Wallet'ın kartı yüklendiğinde iskeletin yerine sıradaki plan satırı geliyor (→ 7.4).

## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 3.1 | E0235 | Money Manager | Canlı kare | Money Manager · Hesaplar listesi |
| 3.2 | E0027 | Bluecoins | Canlı kare | Bluecoins · Hesaplar listesi |
| 3.3 | E0276 | Wallet | Canlı kare | Wallet · yazım 1 · Home, hesap kartları |
| 3.4 | E0283 | Wallet | Canlı kare | Wallet · yazım 2 · Home, hesap kartları |
| 3.5 | E0137 | Hesap Defterim | Canlı kare | Hesap Defterim · Ekranın altı |
| 3.6 | E0115 | Goodbudget | Canlı kare | Goodbudget · Zarf satırı |
| 3.7 | E0116 | Goodbudget | Canlı kare | Goodbudget · tarih · Kayıt formu |
| 3.8 | E0228 | Money Manager | Canlı kare | Money Manager · Ana ekran özet satırı |
| 3.9 | E0230 | Money Manager | Canlı kare | Money Manager · havale · İşlem listesi |
| 3.10 | E0030 | Bluecoins | Canlı kare | Bluecoins · İşlem listesi |
| 3.11 | E0030 | Bluecoins | Canlı kare | Bluecoins · transfer · İşlem listesi |
| 3.12 | E0137 | Hesap Defterim | Canlı kare | Hesap Defterim · Ekranın altı |
| 3.13 | E0494 | Goodbudget | Canlı kare | Goodbudget · İşlem listesi |
| 3.14 | E0142 | Hesap Defterim | Canlı kare | birleşik liste |
| 3.15 | E0144 | Hesap Defterim | Canlı kare | Takvim |
| 3.16 | E0103 | Bluecoins | Canlı kare | Hesaplar sekmesi |
| 3.17 | E0280 | Wallet | Canlı kare | Statistics › Cash-flow |
| 3.18 | E0231 | Money Manager | Canlı kare | İstatistik |
| 3.19 | E0115 | Goodbudget | Canlı kare | ENVELOPES |
| 3.20 | E0056 | Bluecoins | Canlı kare | Bluecoins · durum · Hatırlatıcılar |
| 3.21 | E0056 | Bluecoins | Canlı kare | Bluecoins · sayaç · Hatırlatıcılar |
| 3.22 | E0142 | Hesap Defterim | Canlı kare | Hesap Defterim · Ekranın üstü |
| 3.23 | E0228 | Money Manager | Canlı kare | Money Manager · Ekranın üstü |
| 3.24 | E0280 | Wallet | Canlı kare | Wallet · Rapor ekranının altı |
| 3.25 | E0122 | Goodbudget | Canlı kare | Goodbudget · Rapor ekranının üstü |
| 3.26 | E0274 | Wallet | Canlı kare | Wallet · ana ekranın aşağısı, yüklenirken |
| 3.27 | E0282 | Wallet | Canlı kare | Wallet · Planned payments, boş |
| 3.28 | E0122 | Goodbudget | Canlı kare | Goodbudget · boş rapor |
| 3.29 | E0232 | Money Manager | Canlı kare | Üçü ekranın altında kısa mesaj kutusu; sonuncusu alanın altında kırmızı metin. |
| 3.29 | E0281 | Wallet | Canlı kare | Üçü ekranın altında kısa mesaj kutusu; sonuncusu alanın altında kırmızı metin. |
| 3.29 | E0117 | Goodbudget | Canlı kare | Üçü ekranın altında kısa mesaj kutusu; sonuncusu alanın altında kırmızı metin. |
| 3.29 | E0347 | Wallet | Canlı kare | Üçü ekranın altında kısa mesaj kutusu; sonuncusu alanın altında kırmızı metin. |
