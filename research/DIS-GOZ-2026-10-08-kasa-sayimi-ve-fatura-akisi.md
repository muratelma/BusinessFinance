**Kasa konusunda Claude’un ilk önerisinin yönü doğru, ikinci önerisi ise sorunu tam çözmüyor. Daha önemlisi, kodda bu iki öneriden önce ele alınması gereken bir hesaplama sorunu var.**

Mevcut kodu, ilgili ADR’leri, aktif aşama belgesini ve `faturasonucu.png` görselini inceledim. Aşağıdaki değerlendirme kod incelemesine dayanıyor; bu turda emülatör testi yapmadım, dosya değiştirmedim.

**1. En önemli sorun: eski sayım, yeni bakiyeyle karşılaştırılıyor.**

Farkı kaydetme kodu, sayım hangi güne ait olursa olsun **şimdiki hesap bakiyesini** alıyor, geçmişte sayılan tutardan çıkarıyor ve çıkan farkı sayım tarihine yazıyor. Aradan geçen süredeki hareketleri ayırmıyor. [İlgili kod](C:/Users/elma6/Documents/BusinessFinance/src/BusinessFinance.Application/Cash/CashCountUseCases.cs:257)

Örneğin:

| Olay | Uygulamadaki bakiye | Fiziksel nakit |
|---|---:|---:|
| 8 Ekim’de saydın; 100 TL eksik | 1.000 TL | 900 TL |
| Sonra 200 TL nakit satış yaptın ve kaydettin | 1.200 TL | 1.100 TL |

Şimdi 8 Ekim sayımının farkını kaydettirirsen kod **900 − 1.200 = −300 TL** hesaplıyor. Oysa açıklanması gereken eksik hâlâ **100 TL**.

Claude’un “eski sayım günler sonra API’den kaydedilebiliyor” bulgusu bu nedenle tek başına olumlu bir özellik değil. **Arada hareket varken yanlış düzeltme yazılabiliyor. Aynı sorun gün içinde de oluşabilir.**

Öncelik bunu düzeltmek olmalı. Bilgi satırının görünürlüğü daha sonra gelir.

**2. Senin “10 gün önce kendime aldım” senaryon bütünüyle mantıksız değil.**

Gerçekten 10 gün önce para aldıysan, bunu bugün hatırlayıp gerçek tarihine yazabilmelisin. İki gün önceki sayımın tutmuş olması, eski kaydı otomatik olarak yanlış yapmaz: önceki sayım yanlış olabilir veya başka bir eksik kayıt onu dengelemiş olabilir.

Ama şu sonuç da çıkarılamaz:

> “Bugünkü bakiye sayılan tutara eşitlendi; demek ki geçmiş de tamamen doğru.”

**Bugünkü tutarın eşitlenmesi, geçmişteki çelişkinin açıklanması değildir.**

Benim önerim:

- Normal **Kendime aldım** girişinde gerçek işlem tarihi seçilebilsin.
- Sayım farkını açıklayan giriş, hangi sayıma ait olduğunu bilsin.
- Sayımın eski gözlemi değişmesin: “O sırada uygulama 1.000 gösteriyordu, 1.000 sayılmıştı.”
- Sonradan geçmiş tarihli kayıt girildiyse, gerektiğinde “Bu sayımdan sonra geçmiş tarihli kayıt girildi” bilgisi gösterilsin.

Sadece tarih alanını kaldırmak, kullanıcıyı başka ekrandan aynı kaydı girmekten alıkoymaz; esas tutarlılık sorununu çözmez.

**3. Claude’un “Kendime aldım sayıma bağlansın” önerisine katılıyorum; tarihi ve tutarı kilitlemeyi koşullu görüyorum.**

Şu an fark panelindeki **Kendime aldım**, sıradan bir gider veya transfer yazıyor; sayımın farkını açıklayan kayıt olarak bağlanmıyor. Bu nedenle bakiye düzelirken sayımın durumu geride kalıyor.

Düzeltmede şu güvenceler gerekli:

- Gider/transfer ile sayım bağlantısı **birlikte** kaydedilmeli.
- Tekrar deneme ikinci kez para çıkarmamalı.
- Bağlı kayıt iptal edilirse sayım hâlâ “fark giderildi” dememeli.
- Kayıt zaten başka ekrandan girilmişse, aynı para için tekrar gider/transfer üretilmemeli.

**Tutarı kilitlemek**, düğmenin anlamı “farkın tamamını bununla açıkla” ise anlaşılır. Ama 100 TL eksiğin 60 TL’sini kendine aldığını hatırlayan kullanıcı da olabilir. Bu durumda tamamını açıklamış gibi davranılmamalı.

**Tarihi sayım gününe zorlamak** ise gerçek tarihi bilinen bir harcamayı yanlış güne yazdırabilir. Sayım günü, sebebi ve tarihi bilinmeyen farkın düzeltme tarihi olabilir; bilinen işlemin gerçek tarihinin yerine geçmemeli.

**4. “Önceki farkı yalnız bugün de fark varsa gösterelim” önerisini kalıcı çözüm olarak seçmezdim.**

Bu öneri bazı yanlış uyarıları gizliyor ama eski farkın açıklanıp açıklanmadığını belirlemiyor.

Örneğin eski 100 TL eksiği unuttuğun gideri girerek açıkladın. Bugün de tamamen başka sebeple 40 TL eksik çıktı. Claude’un önerisinde eski 100 TL satırı yeniden görünebilir. Kullanıcı yine iki ayrı eksik olduğunu düşünebilir.

Bence ekran şunları ayırmalı:

| Bilgi | Anlamı |
|---|---|
| **8 Ekim sayımında 100 TL eksik çıktı** | Tarihsel gözlem |
| **Bu fark için kayıt oluşturuldu** | Sayıma bağlı işlem var |
| **Son sayımda kasa tutuyor** | Yeni fiziksel kontrolün sonucu |
| **Sayımdan sonra kayıtlar değişti** | Eski gözlemle bugünkü durumu doğrudan karşılaştıramıyoruz |

“Kaydedilmemiş fark” teknik olarak doğru olsa bile kullanıcıya **“kasanda hâlâ bu kadar eksik var”** anlamını veriyor. Sorunun bir kısmı bu.

Senin üç gün sonraki senaryonda doğru akış şu olur: **sayım geçmişte kalır, bakiye kendiliğinden değişmez, ana eylem yeniden kasayı saymaktır.** Yeni sayım yapılmadan bugünkü fiziksel farkın hâlâ 100 TL olduğunu bilemeyiz.

**Benim bu ürün için tercih edeceğim sade çözüm:** geçmiş gözlemleri korumak; fark açıklamasını işleme bağlamak; araya hareket girdiyse eski sayımdan sessizce yeni düzeltme tutarı üretmemek. Zaman ilişkisi belirsizse yeniden sayım istemek. İleride daha ayrıntılı eşleştirme gerekirse bunun üzerine eklenebilir.

**5. Fatura tarafında da birkaç net bulgu var.**

- **Geri dönüş sorunun gerçek.** Hem okumadan karar ekranına hem karar ekranından forma geçerken `pushReplacement` kullanılıyor; önceki adım geçmişten çıkarılıyor. Formdan geri dönünce okunan fatura korunarak **Ödedim / Henüz ödemedim** ekranına dönmeli. Yeniden okutmak gerekmemeli. [Yönlendirme kodu](C:/Users/elma6/Documents/BusinessFinance/mobile/business_finance_mobile/lib/core/routing/app_router.dart:1099)
- **“İşyeri faturaları” faturadan okunmuş bir alan değil, modelin kategori önerisi.** Modele kullanıcının kategori adları gönderiliyor. Ekranda bunun altında “Fişten okundu” yazması yanıltıcı; **“Önerilen kategori — kontrol edin”** daha doğru. Bu seçim kapsamı da İşletme yaptığı için ayrım önemli.
- **Karşı tarafın gelmemesi, satıcı adının okunmadığı anlamına gelmiyor.** Görselde ad okunmuş. Mevcut kod, okunan adı kayıtlı kişi adıyla eşleştiriyor; anlam veya şirket unvanı benzerliği aramıyor. Ancak bu özel başarısızlığın nedenini, analiz cevabı ve kayıtlı kişi birlikte görülmeden kesinleştiremeyiz.
- **Yükümlülük işlevi gerekli; ayrı ve anlaşılmaz bir menü olması zorunlu değil.** Fotoğrafsız da girilebilen, henüz ödenmemiş/tahsil edilmemiş tek seferlik kayıt ihtiyacını karşılıyor. İsim adayım **“Faturalar ve alacaklar”**, içeride **Ödenecek / Tahsil edilecek** ayrımı. Tasarım notunda yalnız adın anlaşılmadığı yazılmış; kesin yeni ad seçilmemiş. Ayrıca aynı nottaki “kişisiz fatura” ifadesi güncel kararla uyuşmuyor: kişi isteğe bağlı kalıyor. [Tasarım notu](C:/Users/elma6/Documents/BusinessFinance/stages/06.3-butunsel-duzenleme.md:1333)

**Sıradaki tek iş olarak, sayımdan sonra hareket girildiğinde fark kaydının nasıl davranacağını netleştirip yukarıdaki 100 TL → yanlışlıkla 300 TL düzeltme senaryosunu teste bağlayarak düzeltirdim.** Ardından sayıma bağlı “Kendime aldım” ve durum metinleri aynı kurala oturtulmalı. Bu tur inceleme olduğu için build/test/analyze çalıştırmadım.
