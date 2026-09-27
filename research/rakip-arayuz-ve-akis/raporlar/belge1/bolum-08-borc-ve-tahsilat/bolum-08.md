# Bölüm 8 · Borç, fatura ve tahsilat

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Bir alacak nasıl kaydediliyor, tahsilat nasıl işleniyor ve kalan nerede görünüyor?

Bu bölüm karşı tarafın nerede listelendiğini, alacağın hangi ekrandan yazıldığını, tahsilatın hangi ekrandan işlendiğini ve kalan tutarın nerede göründüğünü izler.

İki üründe aynı senaryo girildi: Ada Reklam'a 12.000'lik hizmet alacağı ve 5.000'lik kısmi tahsilat. İki ürün bunu farklı ekranlarla kaydediyor; tutarların bakiyeye ve rapora etkisi bu bölümde anlatılmaz.

**Bu bölüme girmez**

- Tutarların bakiyeye etkisi ve zinciri → Belge 2
- E-belge ve entegrasyon → 10
- Rapor toplamları → 9
- Tekrar eden fatura → 7
- Kendi modelimizle eşdeğerlik hükmü → Belge 3

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Görülmedi | Cari zinciri kurulmadı. |
| Bluecoins | Canlı kare | Cari hesap ve transfer. |
| Wallet | Canlı kare | Debts yüzeyi. |
| Hesap Defterim | Görülmedi | Ayrı cari yüzeyi görülmedi. |
| Goodbudget | Koşum kaydı | Debt grubu denenmedi. |
| KolayBi | Kaynak görseli | Cari detayı, açılış, ekstre. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı |  |


## 8.1 · Karşı taraf nerede listeleniyor?

Bluecoins karşı tarafı bir hesap olarak, hesap listesinin kendi grubunda tutuyor. Wallet borçları ayrı bir Debts yüzeyinde topluyor.

**Şekil 8.1 · Bluecoins · hesap seçici** (E0072)

1. Hesap grupları arasında Cari hesap.
2. Karşı taraf bir hesap satırı; bakiyesi altında.

**Şekil 8.2 · Wallet · Debts, borç girilmeden önce** (E0307)

3. Active / Closed sekmeleri.
4. Boş durumda yönlendirme: Track what you lent and borrowed.

- Wallet'ta Debts çekmeceden açılan ayrı bir bölüm (→ 2.4).
- Bluecoins'te cari hesap, banka ve kredi kartı hesaplarıyla aynı seçicide duruyor.
- Bluecoins'in cari hesap formu başlangıç bakiyesi, son bakiye, açılış tarihi, hesap tipi ve iki anahtar taşıyor; vade alanı yok. KolayBi'nin cari formu vade soruyor (→ 8.5).

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Cari listesi; Müşteri, Tedarikçi, ikisi birden ve yurt dışı tipleri (→ 6.4). Cari detayı → 8.5.
- *Görülmedi* · **Hesap Defterim** — Ayrı bir cari listesi görülmedi. Çekmecede ayrı bir Veresiye Defteri uygulamasına bağlantı var; o uygulama incelenmedi.
- *Koşum kaydı* · **Goodbudget** — Hesap türlerinde Debt grubu var; içeriği denenmedi.
- *Görülmedi* · **Money Manager** — Cari veya karşı taraf yüzeyi görülmedi; kart borcu → 5.

## 8.2 · Alacak hangi ekrandan yazılıyor?

Bluecoins'te alacak normal kayıt formunda yazılıyor: tür GELİR, hesap cari hesap. Wallet'ta ayrı bir borç formu var; ürün formdan önce ve kaydederken iki soru soruyor.

**Şekil 8.3 · Bluecoins · Ekle, cari hesaba gelir** (E0074)

1. Hesap alanında cari hesap.
2. Tür seçicide GELİR.

**Şekil 8.4 · Wallet · borç formundan önce** (E0308)

3. Mevcut bir kaydı borca bağlama sorusu; Yes / No, skip.

**Şekil 8.5 · Wallet · I Lent formu** (E0364)

4. Başlıkta tür: I Lent. Ad, açıklama, hesap, tutar.

**Şekil 8.6 · Wallet · kaydederken** (E0365)

5. Record oluşturulsun mu? Oluşturulursa bakiye değişir.

- Wallet'ta alacak I Lent türüyle girildi; ürün bu kartı fatura diye adlandırmıyor.
- Borç eklemek iki yönle başlıyor: I Lent ve I Borrowed. I Lent formunda Date ve Due date var; vadenin varsayılanı bir yıl sonrası.
- Aynı adla ikinci borç girilirken uyarı görülmedi.
- Yes kolunda borca Loan, interests kategorili bir kayıt bağlandı; bağlama listesi mevcut kayıtları etiketleriyle gösteriyor.
- Bluecoins'te kaydın adı serbest metin; adındaki fatura sözcüğü ayrı bir fatura nesnesi değil.
- Alacağın iki üründe hangi toplama yazıldığı → Belge 2 4.1–4.3.

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Cari detayında Borç/Alacak Ekle ve Fatura Ekle ayrı düğmeler (→ 8.5).

## 8.3 · Tahsilat hangi ekrandan işleniyor?

Bluecoins'te tahsilat, cari hesaptan bankaya bir TRANSFER. Wallet'ta tahsilat aynı borç kartından başlıyor ve karta bağlı bir kayıt olarak yazılıyor.

**Şekil 8.7 · Bluecoins · Ekle, TRANSFER** (E0078)

1. Kaynak cari hesap; hedef banka.
2. Transfer ücreti, Durum, Etiket; fatura seçen alan görünmüyor.

**Şekil 8.8 · Wallet · Add Record** (E0367)

3. Mevcut kaydı bağla ya da yeni kayıtla öde veya borcu artır.

**Şekil 8.9 · Wallet · Create Debt Record** (E0369)

4. Debt action: Repay debt.
5. Kısmi tutar: 5.000.

- Tutar boşken yer tutucu kalan tutarı gösteriyor: ₺12.000,00 to Repay debt.
- Debt action iki değer taşıyor: Repay debt ve Increase debt.
- Bluecoins'in kayıt formunda da fatura ya da belge seçen alan yok; belge yalnız formun üstündeki ataçla ekleniyor (→ 4.2).
- Tahsilatın hangi alacağı kapattığı → Belge 2 4.3.

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Cari detayında Ödeme/Tahsilat Ekle ayrı bir düğme (→ 8.5).

## 8.4 · Kalan tutar nerede yazıyor?

Bluecoins'te kalan, cari hesabın bakiyesi olarak kayıt satırında görünüyor. Wallet'ta kalan borç kartının üstünde; aynı ada ait iki borç iki ayrı kart.

**Şekil 8.10 · Bluecoins · İşlemler, tahsilattan sonra** (E0079)

1. Satırın sağında hesabın o anki bakiyesi: cari 7.000.
2. Alacak satırında o günkü bakiye 12.000; bugünkü kalan değil.

**Şekil 8.11 · Wallet · Debts, tahsilattan önce** (E0366)

3. Aynı ada iki ayrı kart; toplam satırı yok.
4. Add Record kartın içinde.

**Şekil 8.12 · Wallet · Debts, tahsilattan sonra** (E0370)

5. Yalnız bağlı kart 7.000'e indi; öteki kart değişmedi.

- Wallet'ta borcun kendi kayıt listesinde bir Total satırı var; işareti karttakinin tersi.
- Tahsilattan sonra borcun listesinde yalnız tahsilat satırı var (Ada Reklam → Me, +5.000); Total ₺5.000,00, altta Add Record.
- Kalanın hangi alacağa düştüğü → Belge 2 4.3, 4.4.
- Bluecoins'te 7.000 cari hesabın toplamıdır; hangi alacağın kalanı olduğu ekranda yazmıyor.

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Cari detayında Toplam Bakiye kartı; hareket tablosunda Bakiye kolonu (→ 8.5).
- *Görülmedi* · **Hesap Defterim** — Karşı taraf başına kalan görülmedi; defterin kendi bakiyesi var.

## 8.5 · Kaynakta görülen cari akışı

KolayBi'nin destek görsellerinde cari detayı borçlandırma, fatura ve tahsilatı üç ayrı düğmeyle ayırıyor. Cari oluşturma formu açılış bakiyesini ve vadeyi soruyor.

**Şekil 8.13 · KolayBi · cari detayı (önde ekstre diyaloğu → 10.1)** (E0196)

1. Borç/Alacak Ekle, Fatura Ekle, Ödeme/Tahsilat Ekle.
2. İşlemler: Mahsuplaştır, Cari Ekstresi Oluştur, pasifleştir, sil.
3. Toplam Bakiye kartı.
4. Cari Hareketleri: vade, borç, alacak, bakiye, banka/kasa, proje.

**Şekil 8.14 · KolayBi · Cari Detay Bilgileri** (E0195)

5. Vade Günü: Yok / Var.
6. Açılış Bakiyesi: tutar, para birimi, durum, proje, tarih, vade.
7. Oluştururken Borç Alacak Ekle.

- Düğmelerin ayrı olması, arkadaki kayıt modelinin nasıl işlediğini göstermez.
- Kısmi kapanışın faturaya nasıl yazıldığı → Belge 2 4.5.
- Görseller destek materyalidir; demo verisi 2023 tarihli ve güncel sürüm doğrulanmadı.

**Aynı soruda diğer ürünler**

- *Kaynak görseli* · **KolayBi** — Gider formunda Cari Takibi Yok/Var ile Ödeme Durumu ayrı alanlar (→ 4.9).

## 8.5 · Ekstre önizlemesi ve kaynak beyanları

Cari ekstre bir PDF önizlemesi olarak açılıyor. Karesi basılmayan üç ürünün borç ve fatura akışı yalnız kendi anlatımlarından biliniyor; kaynaktan kurulan akış modeli Belge 2 Bölüm 9'dadır.

**Şekil 8.15 · KolayBi · Cari Hesap Ekstresi önizlemesi (kişisel bilgiler karartıldı)** (E0197)

1. Borç, alacak ve bakiye toplamı.
2. Yeni Sekmede Görüntüle, E-Posta ile Gönder, Kapat.

- E-Posta ile Gönder düğmesi gönderim kanıtı değildir (→ 10).

**Karesi basılmayan ürünler**

- *Kaynak beyanı* · **Paraşüt** — Kılavuza göre gider veya fatura önce, ödeme sonra yazılıyor; kısmi ödeme ve avans var; tahsilat en gecikmiş açık faturadan başlayarak eşleniyor.
- *Kaynak beyanı* · **Logo İşbaşı** — Anlatımda müşteri seçilirken bakiyesi görünüyor; tahsilat ve ödeme cari içinden nakit veya banka ile yapılıyor.
- *Kaynak beyanı* · **QuickBooks Solopreneur** — Ürün sayfasında fatura sırası: müşteri, kalem, ödeme yöntemi, gönderim ve hatırlatma.

*Dayanak.* Kaynak beyanı: E0011, E0009, E0012.

## Şekil dizini

| Şekil | Kimlik | Ürün | Tür | Etiket |
|---|---|---|---|---|
| 8.1 | E0072 | Bluecoins | Canlı kare | Bluecoins · hesap seçici |
| 8.2 | E0307 | Wallet | Canlı kare | Wallet · Debts, borç girilmeden önce |
| 8.3 | E0074 | Bluecoins | Canlı kare | Bluecoins · Ekle, cari hesaba gelir |
| 8.4 | E0308 | Wallet | Canlı kare | Wallet · borç formundan önce |
| 8.5 | E0364 | Wallet | Canlı kare | Wallet · I Lent formu |
| 8.6 | E0365 | Wallet | Canlı kare | Wallet · kaydederken |
| 8.7 | E0078 | Bluecoins | Canlı kare | Bluecoins · Ekle, TRANSFER |
| 8.8 | E0367 | Wallet | Canlı kare | Wallet · Add Record |
| 8.9 | E0369 | Wallet | Canlı kare | Wallet · Create Debt Record |
| 8.10 | E0079 | Bluecoins | Canlı kare | Bluecoins · İşlemler, tahsilattan sonra |
| 8.11 | E0366 | Wallet | Canlı kare | Wallet · Debts, tahsilattan önce |
| 8.12 | E0370 | Wallet | Canlı kare | Wallet · Debts, tahsilattan sonra |
| 8.13 | E0196 | KolayBi | Kaynak görseli | KolayBi · cari detayı (önde ekstre diyaloğu → 10.1) |
| 8.14 | E0195 | KolayBi | Kaynak görseli | KolayBi · Cari Detay Bilgileri |
| 8.15 | E0197 | KolayBi | Kaynak görseli | KolayBi · Cari Hesap Ekstresi önizlemesi (kişisel bilgiler karartıldı) |
