Fotoğraf net. Aşama belgesindeki tam metni de okudum. **Öncekinden daha sağlam bir değerlendirme; fakat G3 ve G4’ü bu haliyle kabul etmeni önermiyorum.** Sorunun bir bölümünü hâlâ kullanıcının uyarıyı okuyup doğru kutuyu seçmesine bırakıyor.

| Karar | Değerlendirmem |
|---|---|
| **G1 — Toplamdan nakit/kart hesaplamayı kaldırmak** | **Katılıyorum.** Genel toplamın başka ödeme türlerini içermediği bilinmiyor. |
| **G2 — Vadeli satışları listelemek** | **Katılıyorum, ancak kısmi ödeme ve hangi tutarın düşüleceği çözülmeli.** |
| **G3 — İkisini de düşüp uyarmak** | **Yetersiz.** Olası çifte düşümü tespit edip yine de belirsiz bırakıyor. |
| **G4 — Tahsilatlar işaretsiz kalsın** | **Doğrulanmamış genel varsayılan olarak önermiyorum.** Belgede bile bazı olağan senaryolarda yanlış başladığı yazıyor. |
| **G5 — Soruyu ve hesabı açıklamak** | **Katılıyorum.** Bazı önerilen cümleler düzeltilmeli. |
| **G6 — Farkın nedenini tahmin etmemek** | **Katılıyorum.** |
| **G7 — Önce bu hataları düzeltmek** | **Katılıyorum.** |

**Önce örneklerdeki hesap hatası düzeltilmeli.**

Belgedeki S6 şöyle: 500 TL satışın 200 TL’si alınmış, 300 TL’si veresiye; nakit toplamına yalnız **200 TL** girmiş.

Buna ayrıca 1.000 TL normal nakit satış ekleyelim. Uygulamada 500 TL satış ve 200 TL tahsilat zaten kayıtlı. Gün sonuna girilen nakit **1.200 TL**:

| Gün sonunda düşülen | Yeni yazılan satış | Günün toplam geliri | Günün toplam kasa girişi |
|---|---:|---:|---:|
| Hiçbiri | 1.200 | 1.700 | 1.400 |
| **Yalnız 200 TL tahsilat** | **1.000** | **1.500** | **1.200** |
| Yalnız 500 TL satış | 700 | 1.200 | 900 |
| 500 TL satış + 200 TL tahsilat | 500 | 1.000 | 700 |

**Bu senaryoda ikisini düşmek 200 değil, 500 TL eksik gelir üretir.** “200 TL eksik” sonucu, satışın tamamının nakit toplamına yazıldığı başka bir senaryoda çıkabilir.

Bu ayrıntı önemli: yanlış örnek üzerinden yazılacak test, yanlış davranışı doğru diye sabitleyebilir.

**G3 için çözüm, aynı kişiye ait iki kaydı koşulsuz engellemek de değil.**

Agent’ın şu itirazı doğru: Müşteri bugün eski borcunu ödeyip yeni veresiye alışveriş yapabilir. Aynı kişi ve tarih, iki kaydın aynı satışı temsil ettiğini kanıtlamaz.

Ama buradan “uyaralım, ikisini de düşelim” sonucu çıkmıyor. Önerim:

- Uygulamada satış–tahsilat bağlantısı biliniyorsa bu bilgi kullanılsın.
- Bağlantı bilinmiyorsa kişi ve tarih yalnız **olası çakışmayı** göstersin.
- Çakışma görüldüğünde kullanıcı, bunların girilen toplamda **ayrı tutarlar mı, aynı tutarın iki kaydı mı** olduğunu açıklığa kavuştursun.
- Aynı tutar iki kayıtta temsil ediliyorsa bir kez düşülsün. Gerçekten ayrı tutarlarsa ikisinin düşülmesine izin verilsin.

Burada gereken şey bütün cari sistemi faturalara ödeme dağıtan bir yapıya çevirmek değil; **gün sonunda hangi tutarın neden düşüldüğünü belirlemek.** Yalnız “uyarıyı gördüm” onayı bunu belirlemiyor.

**G4’te her satırı tek tek sordurmakla hepsini işaretsiz bırakmak arasında bir seçenek daha var.**

Tahsilat bulunan günlerde bölüm düzeyinde şu soru sorulabilir:

> “Bu tahsilatlar yazdığınız nakit tutarına dahil mi?”

Seçenekler:

- Hepsi dahil.
- Hiçbiri dahil değil.
- Bazıları dahil — kayıtları seç.

Böylece on tahsilat için on ayrı cevap gerekmeyebilir. Bu bir tasarım önerisi; henüz kullanılabilirliği doğrulanmış değil. Ancak **cevap verilmemiş olmasını “hiçbiri dahil değil” diye yorumlamaktan daha açık.** Yanlış kullanıcı cevabını tamamen önleyemez; uygulamanın sessizce cevap uydurmasını önler.

Doğrulanmış bir rapor biçimi bu bilgiyi sağlıyorsa öneri otomatik hazırlanabilir. Fakat yalnız Z’de “Cari Hesap Tahsilatı” sayacını okumak, tutarın nakit satırına dahil olduğunu kanıtlamaz.

**G1 ve G5’te iki küçük ama önemli düzeltme gerekiyor.**

G1’de “kullanıcı iki tutarı da yazar” gereksiz zorunluluğa dönüşmesin. Yalnız nakitle çalışan esnaf POS tanımlamak veya kart tutarı girmek zorunda kalmamalı. Kaldırılacak şey **belirsiz toplamdan ödeme türü çıkarımı**.

G5’te önerilen “işaretsiz: ayrıca alındı sayılır” cümlesi de aynı karışıklığı yeniden yaratabilir. Tahsilat zaten alınmış ve kaydedilmiş. Daha doğru anlatım:

> “Bu tahsilat zaten kayıtlı. Girilen nakit tutarına dahilse yeni satıştan düşülür.”

Vadeli satışın altında koşulsuz “parası alınmadı” yazmak da kısmen ödenmiş satışta yanlış olur. Metin kaydın gerçek durumunu anlatmalı.

**Kaynak yorumunda da bir ayrım korunmalı.** GİB’in §9.1.1.2 bölümündeki sonradan tahsilat yolunda düzenlenen belge **fatura bilgi fişi** olarak tarif ediliyor. §9.6’daki **cari hesap tahsilatı bilgi fişi** ise ayrıca koşullara bağlı. Dolayısıyla bütün veresiye tahsilatlarının tek bir “cari tahsilat” sayacından bulunabileceği varsayılmamalı. [GİB teknik kılavuzu](https://ynokc.gib.gov.tr/UploadedFiles/Files/Bilgi_Fisi_Teknik_Kilavuzu_24072020.pdf)

**Benim tavsiyem:** G1, G5, G6 ve G7’nin yönünü kabul et; G2–G4’ü birlikte yeniden netleştirt. Sıradaki tek iş, kısmi ödeme ve çakışma senaryolarını **girilen tutar → düşülen tutar → yeni satış → toplam gelir → kasa girişi** şeklinde sayısallaştırmak olsun. Bu tablo doğru kurulmadan kodlamaya geçilmesin.

Dosya değiştirmedim; örneklerin aritmetiğini kontrol ettim, uygulama testlerini çalıştırmadım.
