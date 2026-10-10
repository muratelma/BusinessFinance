# Dış göz — Gün sonu G5 panel incelemesi

Tarih: 10 Ekim 2026

**Öneri: G5'in genel düzeni korunarak aşağıdaki metin ve yerleşim düzeltmeleriyle Flutter uygulamasına geçilebilir. Yeni bir karar veya araştırma turuna gerek görmüyorum.** Bu, kullanıcı adına verilmiş uygulama onayı değildir.

Beş PNG çizimi, çizimlerin Flutter kaynağı, DayClosePlan değişikliği, ilgili kabul testi kaynakları ve yedek politikası incelendi. Sunucu testlerini bu turda yeniden çalıştırmadım; bildirilen test sayıları Claude'un koşum raporudur. Çizimler çalışan panelin etkileşim testinin yerine geçmez.

## 1. Nakit girişinin adı, içeriğini doğru anlatmalı

Çizimde giriş alanı hâlâ **Nakit satış**. Ancak kullanıcı bu alana önceden kaydedilmiş borç tahsilatını da içeren bir tutar yazabiliyor. Girişe **Nakit tutarı**, hesap sonucuna **Yeni nakit satış** denmesini öneriyorum. Gerektiğinde girişin altında “Gün sonu için esas aldığınız nakit tutarını yazın. Kasa sayımını değil.” açıklaması kullanılabilir.

Tahsilatlar ile veresiye satışların ayrı bölümlerde olması ve Hepsi / Hiçbiri / Bazıları seçimi korunabilir. Soru daha somut olabilir: **“Bu kayıtlar yazdığınız 1.600 TL'ye dahil mi?”** Gösterilen tutar nakit alanıyla birlikte güncellenmeli.

“Bugünkü” başlığı yalnız seçilen gün gerçekten bugünse doğrudur. Geçmiş gün veya aralık kapanışında “Seçilen günün…” / “Seçilen dönemin…” kullanılmalı. Veresiye bölümündeki fatura kayıtlarının türü de satırda açıkça görünmeli.

## 2. Ortak tutar açıklaması hâlâ düzeltme istiyor

Ana soru artık doğru eksende: satış ve tahsilatın **girilen nakit tutarında nasıl sayıldığı** soruluyor. Fakat g5-04'teki **“İki kayıtta da görünen para.”** açıklaması tekrar uygulama kayıtlarının ilişkisini düşündürüyor. “Tahsilat satışın içinde” seçeneği de bağlamdan koparıldığında aynı riski taşıyor.

Önerilen soru: **“Yazdığınız nakit tutarında, tahsilat satışa ayrıca eklendi mi?”**

500 TL satış ve 300 TL tahsilat örneğinde seçenekler:

| Seçenek | Sonuç |
|---|---:|
| Tamamı ayrıca eklendi | 800 TL düşülür |
| Ayrıca eklenmedi; satış tutarının içinde | 500 TL düşülür |
| Bir kısmı ayrıca eklendi | Girilen ortak tutara göre hesaplanır |

Üçüncü seçenekte alan adı **“Ayrıca eklenmeyen tutar”** olabilir; açıklama **“Yazdığınız nakit tutarında satışın içinde kalan tahsilat kısmı.”** olur. 200 girildiğinde düşüm 500 + 300 − 200 = 600 TL'dir. API'deki ortak tutar anlamı değişmez; kullanıcıya anlatımı netleşir.

Tahsilat satıştan büyükse “Tahsilatın tamamı satışın içinde” seçeneği sunulmamalı. Örneğin satış 200, tahsilat 500 iken ortak tutar en çok 200 olabilir. Sunucuda min(satış, tahsilat) sınırı mevcut; istemci de seçeneklerini bu sınıra uydurmalı. Bu durum ve sıfır/tam/kısmi ortak tutar Flutter testinde yer almalı.

## 3. Hesap özeti yeni satış ile kasa bakiyesini ayırmalı

“Kasa · 1.600 − kayıtlı 600” yerine şu düzen daha açık:

- Girilen nakit: **1.600 TL**
- Bu tutara dahil, önceden kayıtlı: **−600 TL**
- Yeni nakit satış: **1.000 TL**

Hedef hesap ayrıca “Hesap: Kasa” diye gösterilebilir. Böylece 1.000 TL'nin kasanın toplam bakiyesi veya günün bütün nakit girişi olduğu izlenimi azalır. Dar ekranda hesabı tek satıra sıkıştırmak gerekmez.

Toplam farkındaki “1.300 TL kaydedilmez” ifadesi **“1.300 TL fark için bu işlemde yeni kayıt oluşturulmaz”** şeklinde netleştirilmeli. Farkın bir kısmı zaten kayıtlı veresiye satış olabilir; uygulama bunun nedenini tahmin etmemeli. Toplam alanına “Yalnız karşılaştırma için; eksik nakit veya kart tutarı hesaplanmaz.” yardımı uygundur.

## 4. Çizimde görülen ve cihazda doğrulanması gerekenler

- **g5-03'te ikinci satırın tutarı sağdan kesiliyor.** Bu somut yerleşim kusuru düzeltilmeli; tutar eksiksiz kalmalı, gerekirse isim veya açıklama alta akmalı.
- Sorular cevapsızken kaydetmeye basılırsa ilgili sorunun yanında neyin eksik olduğu yazmalı ve görünür alana getirilmeli. Yalnız genel hata veya açıklamasız pasif düğme yeterli değil.
- Ortak tutarı etkileyen kayıt seçimi değişince eski ortak cevap sessizce kullanılmamalı; etkilenen cevap yeniden istenmeli. Liste yenilendiğinde değişmeyen cevapların tamamı gereksiz yere silinmemeli.
- Büyük yazı, dar ekran, uzun müşteri adı ve klavye açıkken tutar/düğme görünürlüğü çalışan Flutter panelinde denenmeli. Siyah başlık kutusunun yalnız çizim aracına ait olduğu da gerçek cihazda doğrulanmalı.

## 5. Sunucu ve yedek notu

İncelenen DayClosePlan farkında toplamdan türetmenin kaldırılması, cevapsız kayıt engeli, ortak tutar sınırı ve listeden çıkmış kayıtların reddi bulunuyor. Yedek test kaynağında ortak tutarın yeni kimliklerle geri yüklenmesi ve sınır dışı tutarın reddedilmesi de var. Bu dar inceleme bütün backend'in bağımsız onayı değildir.

**v11'de kalmak yazılı yol haritasına uyuyor:** PROJECT-ROADMAP.md, Aşama 06.3 boyunca tek sürüm ve değişebilir şekil öngörüyor. Sırf bu değişiklik için sürüm artışında ısrar etmiyorum. “Eski v11 okunur” ifadesi yalnız bu yeni alanın eklenmesinden önceki, diğer zorunlu alanları taşıyan uyumlu v11 dosyaları için kullanılmalı; runbook daha eski bazı v11 biçimlerinin reddedildiğini açıkça söylüyor. Yeni yedeğin eski uygulamada okunacağı da varsayılmamalı.

API ve Flutter birlikte güncellenmeli. Yeni API'yi bu inceleme sırasında başlatmadım. Sonraki adım düzeltilmiş metinlerle Flutter panelini tamamlamak; ardından sözleşme, seçim değişikliği, kayıt/geri alma ve cihaz kabul kontrollerini yapmak.

## İncelenen yerel kaynaklar

- tasarim-onizleme/gun-sonu-g5/g5-01-sorular-cevapsiz.png
- tasarim-onizleme/gun-sonu-g5/g5-02-cevapli-ve-hesap.png
- tasarim-onizleme/gun-sonu-g5/g5-03-bazilari.png
- tasarim-onizleme/gun-sonu-g5/g5-04-ortak-tutar.png
- tasarim-onizleme/gun-sonu-g5/g5-05-toplam-farki.png
- mobile/business_finance_mobile/test/screenshots/gun_sonu_g5_tasarim_test.dart
- src/BusinessFinance.Application/DayCloses/DayClosePlan.cs
- src/BusinessFinance.Api.Tests/Features/DayCloses/DayCloseDeferredSalesEndpointTests.cs
- src/BusinessFinance.Infrastructure.Tests/DataPortability/DataPortabilityTests.cs
- PROJECT-ROADMAP.md, Yedek şeması sürümleri
- documentation/restore-runbook.md
- stages/06.3-butunsel-duzenleme.md, gün sonu kararları ve sunucu uygulaması

Arayüz incelemesinde .agents/skills/ui-ux-pro-max/SKILL.md ve form etiketi/hata geri bildirimi rehberi kullanıldı. Yeni internet araştırması yapılmadı; bu tur yerel çizim, kod ve etkileşim önerisi incelemesidir. Uygulama kodu değiştirilmedi, build/test çalıştırılmadı ve commit atılmadı.
