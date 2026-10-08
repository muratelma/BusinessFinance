**Benim seçimim: 1’de B’nin biraz tamamlanmış hâli, 2’de önerilen davranış, 3’te dört isim değişikliğinin çekirdeğe alınması.** C’ye bugün geçmezdim.

**1 · Kişi yükümlülükte kalsın, cari bakiyeye katılmasın — B**

Bu, mevcut ihtiyaç için en dengeli seçenek. Faturanın kime ait olduğu, vadesi, Planlananlar’daki yeri ve kendine bağlı kapanışı korunuyor. Şahsi faturaya da doğal bir yer bırakıyor.

Ancak B’yi şu ayrımla kabul ederdim:

> Kişiye ait borçların birlikte gösterilmesi, hepsinin aynı ödeme işlemiyle kapanmasını gerektirmez.

Bu nedenle **“kişinin tek toplam borcu olamaz” B’nin zorunlu kaybı değil.** İstersen kişinin ayrıntısında toplam borç özeti gösterilebilir; altında cari borç, açık faturalar ve borç planları ayrı durur. Toplam bilgi verir; ödeme düğmeleri kendi kayıtlarını kapatır. Örneğinizde kullanıcı şunu anlayabilmeli:

| Tedarikçi A | Tutar |
|---|---:|
| Cari borç | 500 TL |
| Açık fatura | 1.000 TL |
| Toplam borç | 1.500 TL |

Cari ödeme **500 TL’yi**, faturanın `Öde ve kapat` işlemi **1.000 TL’yi** karşılar. Toplamın yanında hangi borcu kapattığı belirsiz bir `Öde` düğmesi olmamalı.

Uygulamaya ilişkin üç şartım var:

- Eski kişili yükümlülükler ve geri yüklenen yedekler de aynı kuralla okunmalı.
- Cari bakiyeden çıkan faturalar **net varlıktan ve ödeme planından kaybolmamalı**; ilgili hesaplamalar birlikte kontrol edilmeli.
- “İki yoldan ödeme tamamen engellendi” denmemeli. Cari formunun faturayı ödeme önerisine katması çözülür; kullanıcının elle cari ödemeye fazla tutar yazması hâlâ mümkün olabilir.

**C ne zaman gerekir?** Kullanıcı gerçekten “Tedarikçiye tek seferde 1.200 TL gönderdim; bunun hangi faturaları ne kadar kapattığını takip etmek istiyorum” diyorsa. Bu, B’nin karşılamadığı gerçek bir ihtiyaçtır. Ancak bugün bu ihtiyacın zorunlu olduğuna dair yeterli dayanak görmüyorum. A ise daha önce konuştuğumuz işlev kayıpları nedeniyle tercihim olmaz.

**2 · İki tarafa açık satış kategorisi POS ve gün sonunda kalsın**

Öneriye katılıyorum. Burada belirleyici bilgi yalnız kategori değil, **girilen olayın işletme satışı olması**.

Kuralı şöyle yazardım:

| Kategorinin izin verdiği taraf | POS / gün sonu |
|---|---|
| Yalnız İşletme | Seçilebilir; İşletme yazılır |
| İşletme ve Şahsi | Seçilebilir; İşletme yazılır |
| Yalnız Şahsi | Seçilemez; sunucu da reddeder |

Genel `Gelir ekle` formunda iki tarafa açık kategori için taraf sorulması bununla çelişmez: genel form, gelirin işletme satışı olduğunu baştan bilmiyor.

Burada **Y1’in ifadesini de netleştirirdim:** “Kategori kaydın tarafını söyler” yerine, “Kategori izin verilen tarafları belirler; iki tarafa açıksa olayın bağlamı veya kullanıcının seçimi tarafı belirler.” Böylece her yeni özel akışta ayrı bir istisna uydurmak gerekmez.

**3 · Dört isim değişikliği çekirdeğe girsin**

Önceki tablodaki şu dört değişikliği kastediyorsanız, evet:

- `Elektrik, su, doğalgaz` → `İşyeri faturaları`
- `Faturalar` → `Ev faturaları`
- `Konut` → `Ev kirası`
- `Personel ücreti` → `Personel giderleri`

Taraf artık kategoriyle bağlayıcı hâle geliyorsa, kategorinin neyi kapsadığı anlaşılır olmalı. Bu isimler yalnız görünüşü iyileştirmiyor; yanlış taraf seçilmesini azaltıyor.

Yalnız **`Konut → Ev kirası` bir kapsam daraltmasıdır.** Konut adıyla daha önce kira dışındaki ev giderleri de tutulmuş olabilir. Yeni kategori setinde `Ev kirası` doğru; mevcut kayıtların kategori adını değiştirirken onları sessizce kira olarak yeniden yorumlamamak gerekir. Sentetik veriyi yeniden kurma kararı burada uygulanabilir.

Dolayısıyla seçilecek paket bence şu: **B + anlaşılır kişi borç özeti; bağlama göre İşletme yazılan ortak satış kategorileri; anlamı netleştiren dört isim değişikliği.** Yeni kategoriler, C9 ve iki taraflı bütçe ise kendi gerekçeleriyle ayrıca ele alınabilir.
