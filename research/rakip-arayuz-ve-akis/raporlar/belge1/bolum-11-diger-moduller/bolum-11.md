# Bölüm 11 · Diğer modüller ve yardımcı araçlar

Belge 1 · Rakip arayüz yaklaşımları

> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.
> İşaretler ve sayfa düzeni yalnız PDF'te görünür.

**Ana soru.** Ürünler kayıt ve rapor dışında hangi modülleri sunuyor?

Bu bölüm rakiplerin özellik yüzeyini haritalar: işletme modülleri ve günlük yardımcı araçlar. Bir modülün rakipte bulunması bir ihtiyaç olduğu anlamına gelmez; bu değerlendirme Belge 3'tedir.

Tablodaki "Var", modülün girişinin ya da kaynaktaki anlatımının görüldüğünü söyler; modüllerin iç işleyişi denenmedi. Boş hücreler kanıt olmadığını gösterir, modülün bulunmadığını değil.

**Bu bölüme girmez**

- Modüllerin iç işleyişi ve hesaplamaları
- Belge 3'ün alma kararı

| Ürün | Kanıt | Not |
|---|---|---|
| Money Manager | Canlı kare |  |
| Bluecoins | Canlı kare |  |
| Wallet | Canlı kare | Çekmece girişleri. |
| Hesap Defterim | Canlı kare | Not defteri, kupür aracı. |
| Goodbudget | Görülmedi | Ek modül görülmedi. |
| KolayBi | Kaynak görseli | İşletme modülleri. |
| Paraşüt | Kaynak beyanı |  |
| Logo İşbaşı | Kaynak beyanı |  |
| QuickBooks Solopreneur | Kaynak beyanı | Km takibi. |


## 11.1 · İşletme modülleri

Stok, personel, kıymetli evrak ve ürün kartı yalnız kaynakla incelenen ön muhasebe ürünlerinde görüldü. Hücredeki kimlik, dayanağın kanıt kimliğidir.

| Modül | Money Manager | Bluecoins | Wallet | Hesap Defterim | Goodbudget | KolayBi | Paraşüt | Logo İşbaşı | QuickBooks |
|---|---|---|---|---|---|---|---|---|---|
| Stok ve depo | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Kaynak görseli* · Var · E0218 | *Kaynak beyanı* · Var · E0011 | *Kaynak beyanı* · Var · E0009 | *Görülmedi* ·   |
| Personel ve maaş | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Kaynak görseli* · Var · E0201 | *Kaynak beyanı* · Var · E0011 | *Görülmedi* ·   | *Görülmedi* ·   |
| Çek ve senet | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Kaynak görseli* · Var · E0209 | *Kaynak beyanı* · Var · E0011 | *Kaynak beyanı* · Var · E0009 | *Görülmedi* ·   |
| Ürün ve hizmet kartı | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Kaynak görseli* · Var · E0218 | *Kaynak beyanı* · Var · E0011 | *Kaynak beyanı* · Var · E0009 | *Görülmedi* ·   |

- KolayBi'nin ürün sayfasında Tümü, Ürünler, Hizmetler, Depolar ve Varyantlar sekmeleri var; varyant ve depo işleyişi görülmedi.
- KolayBi personel carilerinde çalışma tipi ve bakiye var; maaş ve prim oluşturma, ödeme ve avans girişleri ayrı (→ 6.4, 7.1).
- KolayBi çek listesinde keşideci, hamil ve vade; senet listesinde kefil, ödenen ve kalan tutar kolonları var. İki tablo da boş.
- Logo İşbaşı'nın anlatımında teklif, sipariş ve sesli fatura girişi de geçiyor.
- Hesap Defterim'in Öğe eklemek diyaloğu kalemlerden tutar ve not üretiyor; bir ürün kataloğu değil.

*Dayanak.* Canlı kare: E0155. Kaynak görseli: E0201, E0202, E0203, E0209, E0210, E0218. Kaynak beyanı: E0011, E0009.

## 11.2 · Yardımcı araçlar

Günlük araçlar canlı incelenen ürünlerde, çoğunlukla çekmece veya ayar girişi olarak görüldü.

| Modül | Money Manager | Bluecoins | Wallet | Hesap Defterim | Goodbudget | KolayBi | Paraşüt | Logo İşbaşı | QuickBooks |
|---|---|---|---|---|---|---|---|---|---|
| Not defteri | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Canlı kare* · Var · E0165 | *Görülmedi* ·   | *Kaynak görseli* · Var · E0212 | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   |
| Takvim | *Canlı kare* · Var · E0228 | *Canlı kare* · Var · E0098 | *Görülmedi* ·   | *Canlı kare* · Var · E0144 | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   |
| Kupür sayan hesap makinesi | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Canlı kare* · Var · E0166 | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   |
| Alışveriş listesi | *Görülmedi* ·   | *Görülmedi* ·   | *Canlı kare* · Var · E0376 | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   |
| Garanti | *Görülmedi* ·   | *Görülmedi* ·   | *Canlı kare* · Var · E0376 | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   |
| Sadakat kartı | *Görülmedi* ·   | *Görülmedi* ·   | *Canlı kare* · Var · E0376 | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   |
| Seyahat modu | *Görülmedi* ·   | *Canlı kare* · Var · E0100 | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   |
| Kilometre takibi | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Görülmedi* ·   | *Kaynak beyanı* · Var · E0012 |

- Hesap Defterim'in Not Defteri tarih, onay kutusu, arama ve tamamlandı sayaçları taşıyor; finansal bir vade değil (→ 7.1).
- KolayBi notlarında hatırlatıcı ve özel/şirket notu seçimi var; bildirim denenmedi.
- Money Manager'ın takvimi kayıt listesinin sekmelerinden biri.
- Wallet kayıt ayrıntısında Warranty alanı da var. Shopping lists'te bir liste ve Share list; Warranties ve Loyalty cards boş, ikisi de ilk kaydı çağırıyor.
- Wallet çekmecesinde Investments ve Currency rates girişleri de var; Investments'ın görüldüğü kare hesap sahibinin adını taşıdığı için basılmadı.
- Money Manager'ın CalcBox ve PC'den Yönet girişleri uygulama içi modül değil: CalcBox başka bir ürünün mağaza sayfasını açıyor, PC'den Yönet ücretli sürüm ekranına gidiyor.
- Bluecoins'in seyahat modu anahtarına dokununca bir etiket seçici açılıyor; seçim yapılmadığı için modun ne yaptığı görülmedi. Gelişmiş ayarlarda döviz kuru tercihleri var.

*Dayanak.* Canlı kare: E0095, E0098, E0100, E0144, E0165, E0166, E0228, E0254, E0294, E0376. Kaynak görseli: E0212. Kaynak beyanı: E0012.
