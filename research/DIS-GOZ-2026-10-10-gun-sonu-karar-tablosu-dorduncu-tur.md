**Bu noktada önerilen çözümün yönünü onaylamanı tavsiye ederim; kategori ve hesap sayfalarından önce yapılması da yerinde.** Ortak tutarın saklanması gerekçeli, G4’ün kapsamı ve G5’in bilgi sınırı düzeltilmiş. Ancak G3’te sorulan cümle, saklanacak bilginin anlamıyla hâlâ aynı değil. Bunu uygulama koşulu olarak netleştirmek gerekiyor; yeni bir geniş araştırma turu gerekmiyor.

[Aşama belgesindeki güncel metni](C:/Users/elma6/Documents/BusinessFinance/stages/06.3-butunsel-duzenleme.md), sekiz tabloyu ve ADR 0019’un bağlayıcı sınırlarını inceledim. Bu rapor dış göz tavsiyesidir; kullanıcı adına uygulama onayı değildir.

**G3’te “Tahsilatın ne kadarı bu satışın parası?” sorusu değişmeli.**

Bu soru ekonomik ilişkiyi soruyor: ödeme hangi borcu kapatıyor? Gün sonunun ihtiyacı ise **girilen toplamda satış ve tahsilatın hangi tutarı ortak temsil ettiği**.

İkisi her durumda aynı değildir. Bunu ayıran iki örnek:

- Normal nakit satışlar 1.000 TL.
- Uygulamada 500 TL veresiye satış ve bu satış için alınmış 200 TL tahsilat kayıtlı.
- Satışın tamamının nakit gibi gösterildiği sorunlu giriş üzerinde düşünelim.

| Girilen nakit toplamının içeriği | Girilen | Ortak tutar | Düşülecek | Yeni satış |
|---|---:|---:|---:|---:|
| 500 TL satış dahil; 200 TL tahsilat ayrıca tekrar eklenmemiş | 1.500 | 200 | 500 | 1.000 |
| 500 TL satış dahil; 200 TL tahsilat ayrıca eklenmiş | 1.700 | 0 | 700 | 1.000 |

Her iki durumda da tahsilatın **tamamı aynı satışın parasıdır**. Buna rağmen ortak tutar ilkinde 200, ikincisinde 0’dır. İkisinde de doğru günlük gelir 1.500 TL, kasa girişi 1.200 TL olur.

İkinci satır, belirli bir cihazın böyle davrandığı iddiası değildir; desteklenen sorunlu kayıt yolunda girilen toplamın kapsamına ilişkin bir kabul senaryosudur.

Dolayısıyla formül doğru, fakat “aynı satışın ödemesi” cevabından ortak tutar otomatik çıkarılmamalı. Önceki raporumdaki “ortak tutar” ifadesinin de bu anlamı daha açık taşıması gerekiyordu.

Önerdiğim soru yönü:

> “Bu tahsilat, yazdığınız nakit toplamında satış tutarına ek olarak yer alıyor mu?”

Cevapların sonucu rakamla gösterilsin:

- **Tamamı ayrıca yer alıyor:** ortak tutar 0; satış ve tahsilat birlikte düşülür.
- **Ayrıca yer almıyor, satış tutarının içinde:** uygun tutar bir kez sayılır.
- **Bir kısmı ayrıca yer alıyor:** tutar belirtilir ve hesap gösterilir.

Son metin çizimde sadeleştirilebilir. Değişmemesi gereken kural: **ortak tutar ödeme dağılımı değil, girilen toplamın nasıl oluştuğuna ilişkin bilgidir.** Tahsilat satıştan büyükse de “tamamı ortak” gibi matematiksel olarak mümkün olmayan cevap sunulmamalı; mevcut üst sınır sunucuda korunmalı.

Aynı ayrım G2 için de geçerli: faturanın kendi kapanışına bağlı olması, rapor toplamında yalnız bir kez yer aldığını tek başına kanıtlamaz. Birleşik satır kullanılabilir; Tablo 5’in “raporda tek tutar” koşulu korunmalı ve ayrı ayrı dahil edilmiş tutarlar da ifade edilebilmeli.

**Yeni tablo ve migration gereksiz bir büyüme değil.**

Uygulamanın kendi kayıtlarından çıkaramadığı, kullanıcının verdiği bir bilgiyi saklamak gerekiyor. Bunu yalnız ekranda hesaplayıp kaybetmek, sonradan gösterilen gün sonu hesabını bozardı.

Şu sınırlarla öneriyi destekliyorum:

- Ortak tutar doğrudan gelir, gider veya hesap hareketi üretmez; gün sonunun oluşturacağı normal kayıtların tutarını belirler.
- Sonraki raporlar yine üretilen normal kayıtları okur; ikinci bir bakiye veya gelir kaynağı oluşmaz.
- Saklanan cevap belirli gün sonuna, kişiye, güne ve o cevapta kullanılan kayıtlara bağlı kalır. Seçili kayıtlar değişince eski ortak tutar sessizce kullanılmaz.
- Birleştirilmiş fatura/kapanış için zaten yapılan tekilleştirme, ortak tutar hesabında ikinci kez uygulanmaz.
- Geri alma ve yedekten dönüş, aynı düşümü ve açıklamasını yeniden kurabilir.

[ADR 0019](C:/Users/elma6/Documents/BusinessFinance/documentation/adr/0019-day-close-produces-existing-records.md) tutar taşımayan kimlik kaydına ilişkin açık bir sınır taşıyor. Yeni bilginin neden ikinci bir finansal kaynak olmadığı orada da netleştirilmeli. Sırf başka tabloya koymak mimari gerekçe değildir; gerekçe, bunun kullanıcının gün sonu girdisini açıklayan ve normal finansal kayıtların yerine geçmeyen bir bilgi olmasıdır.

**Claude’a vereceğin kararın çerçevesi şu olabilir:**

> G1–G4 ve G6’nın yönünü, kategori ve hesap sayfalarından önce uygulanmasını kabul ediyorum. G3’te ortak tutar, tahsilatın hangi satışa ait olduğundan değil, girilen toplamda hangi tutarın ortak temsil edildiğinden belirlensin. Aynı satışa ait ödeme toplamda ayrıca yer alıyorsa ortak tutar sıfır olabilsin. Bu ayrım G2’deki birleşik fatura satırında da korunsun. API, migration, yedek ve geri alma değişiklikleri bu kuralla tasarlansın. G5’in çizimi uygulamadan önce gösterilsin; kabul testlerine bu iki toplam senaryosu eklensin.

Bu alıntı bir öneri metnidir; ben senin adına onay vermedim. Sıradaki iş, bu tanımı sözleşmeye ve testlere geçirmek. Yeni bir özellik listesi açmaya gerek yok.

Uygulama kodunu veya karar belgesini değiştirmedim. Karşılaştırmadaki iki örneğin gelir ve kasa sonuçlarını aritmetik olarak doğruladım; build veya uygulama testi çalıştırmadım.
