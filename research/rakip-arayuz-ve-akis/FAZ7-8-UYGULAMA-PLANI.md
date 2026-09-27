# Faz 7/7.5 kapanışı ve Faz 8 belge üretim planı

**15 Eylül son durum: Faz 8 kullanıcı onayıyla açıldı. P5-P pilot ve teknik dışa aktarım hazır; kullanıcı pilot değerlendirmesi bekleniyor.**
365/365 görsel incelendi ve envanterde (357 başlangıç + GB-U01 3, WL-U 3, BC-U01 2). B09 WL-U01 ile kapandı: Wallet'ın borç modeli ADR 0014'e eşdeğer değil.
B01–B19: 14 kanıtla kapalı, 5 kapsamı sınırlı (B06, B08, B10, B11, B12), 0 açık. P4-tema-01–10 kapandı. f7-54 hesap sahibinin adını gösterir: teslimde karartılmalı.
Ölçülmeyen davranışlar (Wallet Postpone/Dismiss sonucu, Bluecoins otomatik kol ve liste yenilenme kök nedeni, Hesap Defterim e-posta alıcısı/teslimi) doğrulanmış sayılmaz.
Mekanik kapı: dokuz form 0 hata/0 uyarı, 21/21 test. Geçiş kapısı 12/12; 15 Eylül kullanıcı onayı kaydedildi. Pilot değerlendirilmeden tam raporlar yazılmaz.
Sıradaki tek iş pilot değerlendirmesi. Teslim: raporlar/pilot/pilot-islem-ekleme.pdf ve .docx.
Aşağıdaki eski paket devirleri tarihseldir; güncel ayrıntı BULGU-DOGRULAMA-KAYDI.md sonundadır.


## Güncel ilerlemeye bağlantı

14 Eylül: P0.1 ve P0.2 tamamlandı. Kanıt envanteri ve B01–B19 başlangıç
kaydı mevcut; tüm bulgular açık. Money Manager, Hesap Defterim, Goodbudget, Paraşüt, Logo İşbaşı, QuickBooks, KolayBi, Bluecoins ve Wallet metin haritaları tamamlandı;
P0.4 kabul kontrolüyle P0 kapandı; P1-B01, P1-B02-B04, P1-B14 ve P1-goodbudget-G01 kapandı, B06 kapsam sınırıyla ve B07 kaynakla kapandı; B05/GB-U01 de kapandı; P1-kolaybi-G01/G02, P1-B08, P1-B15, P1-hesap-defterim-G01/G02, P1-B11, P1-B12, P1-money-manager-G01, P1-parasut-G01, P1-logo-isbasi-G01, P1-quickbooks-G01, P1-B13, P1-B18 ve P1-K kapandı; sıradaki paket P2-G01.
Güncel ilerleme DURUM.md'dedir; aşağıdaki başlangıç tablosu tarihsel plandır.

## Durum ve yetki sınırı — 13 Eylül 2026

Kullanıcının süreç değerlendirmesi üzerine hazırlanan plan kalıcı olarak
kaydedildi. **13 Eylül'de kullanıcı isteğiyle P0.1 uygulandı:** dosya listesi
ve kanıt envanteri iskeleti oluşturuldu. Ardından P0.2 bulgu iskeleti tamamlandı.
14 Eylül itibarıyla Money Manager, Hesap Defterim, Goodbudget, Paraşüt, Logo İşbaşı, QuickBooks, KolayBi, Bluecoins ve Wallet metin haritaları tamamlandı;
P0.4 ile P0 kapandı; P1 sürüyor (P1-B01, P1-B02-B04, P1-B14 kapandı), P2–P7 başlamadı. Bu ilerleme
testlerin yapıldığı, hataların kapandığı veya Faz 8'in açıldığı anlamına gelmez.

- Güncel ilerlemenin tek kaynağı: [DURUM.md](DURUM.md).
- Bu dosya: iş sırası, çalışma kayıtlarının şeması, kabul kapıları ve rapor üretim yöntemi.
- Geçmiş faz kapsamı: [Tur 2 yol haritası](TUR2-YOL-HARITASI.md).
- Nihai çıktı adları ve onay durumu: [raporlar/README.md](raporlar/README.md).
- Mevcut hata başlangıcı: [13 Eylül denetimi](raporlar/2026-09-13-kapsamli-yeniden-denetim.md), B01–B19.
- Önceki soru/cevaplar: [Soru 1](raporlar/Soru%201.md), [Soru 2](raporlar/soru%202.md).
  Bunlar tarihsel girdidir; güncel doğruluk veya tamamlanma kanıtı değildir.

Araştırma Faz 7/7.5/8, ürün geliştirme Aşama 07 ile aynı zincir değildir.
Araştırma ürünün tamamını besler; yalnız arayüz düzenine indirgenmez.
Ürün aşaması, PRD, ADR, uygulama kodu, altyapı veya sözleşme bu planla değişmez.
Repo AGENTS.md kuralları geçerlidir; bu belge tek başına commit gerekçesi değildir.

## 14 Eylül — Kullanıcı kararı: kapanış ve emülatör iş bölümü

Bu bölüm eski canlı test talimatlarıyla çeliştiğinde geçerlidir. P0–P7 sırası,
bütün görsellerin incelenmesi, üç belgenin kapsamı ve onay kapıları değişmez.
Önce Goodbudget kapanır; sonra mevcut uygulama/paket sırası izlenir.

- Önceki koşumlar ve gezinme notları araştırma girdisidir. Tek yanlış iddia bütün
  koşumu geçersiz kılmaz. Tamamlanmış inceleme somut yeni çelişki olmadan tekrarlanmaz.
  28/357 yalnız bu denetimin görsel sayacıdır; toplam araştırma ilerlemesi değildir.
- Görseller arayüz/akış anlatımına kanıt sağlar; yalnız metin hatası bulmak için
  açılmaz. Mevcut kanıtla çözülen metin hatası canlı testi beklemeden düzeltilir.
- T paketleri otomatik emülatör koşumu değildir. Agent önce görselleri, koşum
  notlarını ve kaynakları kullanır. Rapor için gerekli kalan soruları uygulama
  başına tek kullanıcı listesinde toplar: soru, neden gerekli, ekran/adımlar,
  gereken gözlem/görsel ve bitiş ölçütü.
- Ek emülatör kontrolünü kullanıcı yapar; agent kendiliğinden canlı koşum başlatmaz.
  Beş emülatör uygulaması, Bluecoins ve Wallet dahil, aynı kurala tabidir.
  Sonuç kayda işlenir; gerekli cevap beklenirken tamamlanma uydurulmaz.
- Rakip kusurunun kök nedenini çözmek, vade beklemek veya geçmiş olayı yeniden
  üretmek kapanış şartı değildir. Bilinmeyen neden sınırlandırılır; gözlem korunur,
  kesin kusur veya öneri gerekçesi yapılmaz. Otomatik yeni test turu açılmaz.
- Her uygulama mevcut inceleme → düzeltmeler → gerekiyorsa kullanıcı kontrolü →
  forma/özetlere yayılım → kapanış sırasıyla tamamlanır. Ek kontrol gerekmiyorsa
  gerekçe yazılır; kaynak kontrolü canlı test yapılmış gibi gösterilmez.
- Kapanmış iş yalnız yeni somut çelişki veya kararı etkileyen kanıtla açılır.
  Agent değişikliği tek başına tekrar gerekçesi değildir.

## 15 Eylül — Kullanıcı kararı: görev paylaşımı ve token kullanımı

**15 Eylül devam oturumu güncellemesi:** Kullanıcı gpt-5.6-sol / high alt agent kullanımını verimli bulmadığı için istemedi. Aşağıdaki eski model ataması geçersizdir; bu oturum ana agent ile yürütülür.

- Ana agent raporun, kanıt yorumunun ve kapanış kararının sorumlusudur.
  Görsel inceleme, kanıt çıkarma, atıf tarama ve rutin kontroller gerektiğinde
  gpt-5.6-sol / high alt agent'a verilir. Küçük işler sırf delege etmek için bölünmez.
- Her görev tek paket ve açık dosya/kanıt listesiyle verilir; tüm konuşma veya
  araştırma külliyatı yeniden okutulmaz. Teslim: kanıt kimliği, bulgu, sınır,
  önerilen düzeltme ve çalıştırılan kontrolün sonucu. Ham çıktı yerine kısa kayıt kullanılır.
- Ana agent aynı görselleri ve kontrolleri baştan tekrarlamaz. Yalnız somut çelişki,
  önemli sayısal sonuç veya rapor kararını etkileyen belirsizlikte ilgili kanıtı açar.
  Başarılı kontroller yeni değişiklik veya hata gerekçesi olmadan yeniden koşulmaz.
- Aynı dosyanın yazım sorumlusu ve canlı emülatörün operatörü tek kişidir.
  Bu iş bölümü, yukarıdaki kullanıcı kontrolü ve canlı koşum yetkisi sınırlarını
  kendiliğinden değiştirmez. Yetkilendirilmiş bir emülatör görevi de aynı kısa
  kanıt teslimiyle yürütülür.
- İlk uygulama P2-T değerlendirmesidir: alt agent mevcut Bluecoins kayıtlarından
  kalan soruları çıkarır; ana agent rapor sınırlarını ve sıradaki paketi kayda geçirir.
  Görseller yeniden incelenmez; P2-K kapanışı ayrıca değerlendirilir.

## 1. Başlangıç gerçeği

- Bluecoins ve Wallet'ın Faz 7 derin koşumları geçmişte kaydedildi;
  **Faz 7.5 görsel–metin–emülatör doğrulamaları henüz tamamlanmadı.**
- Diğer yedi uygulamanın eski Faz 7.5 tamamlanma kayıtları korunur; sonradan
  bulunan hatalar nedeniyle yeni kapanış kapısını otomatik karşılamazlar.
- 13 Eylül denetiminde 357 PNG dosya yapısı/CRC/sıkıştırılmış veri kontrolünden
  geçti. Bu, görsel içerik doğrulaması değildir. O denetimde yalnız 11 görsel
  açılıp içerikle karşılaştırıldı; kalan 346 için o koşumdan içerik onayı çıkmaz.
- Aynı koşumda 181 görselin tam adı kendi formunda bulunmadı; bu sayı tek başına
  görsellerin hiç incelenmediğini kanıtlamaz. Üç tabloda 24 satır başlıkla uyuşmadı.
- Sayılar 13 Eylül koşumuna aittir; sonraki uygulama başlangıcında yeniden sayılır.
- Eski denetim.sh hata varken sıfır çıkış kodu verebildi; tek başına kapanış kapısı değildir.
- Üç nihai raporun taslağı başlamadı. P0.1'de kanıt envanteri iskeleti oluşturuldu;
  bulgu kaydı P0.2'de oluşturuldu. Güncel durum DURUM.md'dedir.
- Emülatörün eski kararsızlık kaydı ve Wallet oturum gereksinimi güncel erişim
  kontrolüyle yeniden değerlendirilir; bu planlama turunda emülatör çalıştırılmadı.

## 2. Uçtan uca sıra

**Envanter → hata ve doğrulama kaydı → yedi uygulamanın açıkları → Bluecoins
ve Wallet tam Faz 7.5 → tematik bulgu paketi → geçiş onayı → Belge 1/2
örnek bölüm → Belge 1/2 ve onay → Belge 3 ve onay → son teslim.**

İnceleme uygulama bazlı, sentez konu bazlıdır. Dokuz gözlem formu art arda
özetlenerek rapora dönüştürülmez. Bir uygulamaya erişim engeli diğer uygulama
ve masa başı işleri durdurmaz; engelli iş tamamlanmış gösterilmez.

| İş | Çıktı | Tamamlanma ölçütü | Durum |
|---|---|---|---|
| P0 — Güvenilirlik haritası | İki çalışma kaydı ve dokuz uygulamanın kapsamı | Malzeme ve B01–B19 izlenebilir | Başlanmadı |
| P1 — Hata kapanışı | Düzeltilmiş gözlemler ve ilişkili özetler | Her kapanışta kanıt + yayılım kontrolü | Başlanmadı |
| P2 — Bluecoins | Tam Faz 7.5 kaydı | Metin, tüm görseller, gerekli canlı testler | Başlanmadı |
| P3 — Wallet | Tam Faz 7.5 kaydı | Metin, tüm görseller, gerekli canlı testler | Başlanmadı |
| P4 — Aktarım ve geçiş | Tematik bulgular + açık sorular + geçiş özeti | Bölüm 7 kapısı ve kullanıcı onayı | Başlanmadı |
| P5 — Belge 1/2 | Örnek bölüm, tam taslaklar, onay | Kaynaklı ve tutarlı iki rapor | Başlanmadı |
| P6 — Belge 3 | Gerekçeli karar ve öneriler | Onaylı 1/2'ye izlenebilirlik, kullanıcı onayı | Başlanmadı |
| P7 — Teslim | Markdown, Word, PDF ve kalıcı kanıt paketi | İçerik/biçim/bağlantı kontrolleri | Başlanmadı |

Bu tablo planın başlangıç durumudur. Uygulama sırasında güncel iş ve tamamlanma
kanıtı DURUM.md'de tutulur; bu tabloyla ikinci bir canlı pano oluşturulmaz.

## 3. P0 — İki çalışma kaydı

Uygulama başladığında araştırma kökünde `KANIT-ENVANTERI.md` ve
`BULGU-DOGRULAMA-KAYDI.md` oluşturulur. Bunlar üç nihai belgeye ek rapor değil,
üretim ve izlenebilirlik araçlarıdır. Kanıt envanteri P0.1'de oluşturuldu;
bulgu/doğrulama kaydı P0.2'de oluşturuldu; B01–B19 açık durumdadır.

### Kanıt envanteri şeması

Her görsel ve kullanılan metin/doküman kaynağı için:

- Sabit kanıt kimliği; uygulama ve tam göreli dosya yolu veya kaynak URL'si.
- Kaynak türü; platform, ürün/paket ve sürüm (bilinmiyorsa açıkça bilinmiyor).
- Yakalama/erişim tarihi, varsa kaynak yayın tarihi ve gösterilen işlem dönemi;
  bunlar birbirinin yerine kullanılmaz. Dosya tarihi arayüz sürümünü kanıtlamaz.
- Görselin gerçekten gösterdiği ekran, durum ve kısa içerik açıklaması.
- İnceleme durumu, inceleme tarihi, ilgili MD bölümü ve bulgu kimlikleri.
- Kullanım: ana anlatım / kanıt eki / araştırma arşivi; seçimin gerekçesi.
- Varsa dosya hash'i yalnız bütünlük/kopya kontrolüdür, içerik onayı değildir.

Her görsel açılıp değerlendirilir. Süreç veya tekrar görseline yapay bir ürün
bulgusu uydurulmaz; arşiv rolü kaydedilebilir. Her görsel rapora girmek zorunda değildir.

### Bulgu ve doğrulama kaydı şeması

- Sabit bulgu kimliği; eski B01–B19 kimlikleriyle eşleme.
- Tek ve sınırları açık iddia; uygulama, platform/paket, dönem/ayar koşulları.
- Kanıt kimlikleri ve ilgili gözlem formu bölümü.
- Kanıt etiketi: araştırma README'sindeki **Kanıt etiketleri — tek kaynak**
  tanımları kullanılır; burada alternatif etiket sistemi kurulmaz.
- Çelişki veya eksik bilgi; gereken kaynak/kod kontrolü ya da test senaryosu.
- Öncelik: sonucu/kararı engeller; anlatımı sınırlar; isteğe bağlı ek derinlik.
- İş durumu: açık / inceleniyor / erişim engelli / kanıtla kapandı /
  iddia kapsamı sınırlandı. Son iki durumda gerekçe ve tarih zorunludur.
- Doğrulanan sonuç; ne kazandırdığı, ne kaybettirdiği; bunların gözlem mi yorum mu olduğu.
- Güncellenecek diğer dosyalar ve yayılım kontrolü sonucu.
- Hedef rapor ve bölüm; kalan açık soru ve karara etkisi.

P0'da dokuz uygulamanın MD içerik haritası çıkarılır; önemli iddialar ilerleyen
incelemelerde kayda işlenir. Önceki denetimlerin özetleri yeni doğrulama yerine geçmez.

## 4. P1 — Hata kapanışı ve yedi uygulamanın yeniden doğrulanacak alanları

Öncelik: finansal davranış → BusinessFinance karşılaştırması → görsel/iddia
uyuşmazlığı → paket/platform/tarih → mekanik sorunlar. Her hata şu üçlüyle kapanır:
**doğru sonuç + destekleyen kanıt + geçtiği diğer belgelerin güncellenmesi.**

Başlangıç iş grupları (ayrıntı ve kabul ölçütü B01–B19 kaynak raporundadır):

- B01–B04, B14: BusinessFinance cari tahsilat eşleştirmesi, kart özellikleri,
  plan bitişi, raporda tanıma zamanı ve kasa sayımı karşılaştırmalarını kod/ADR
  gerçeğiyle uzlaştır. Görünen özellik ile altında varsayılan modeli ayır.
- B05–B07: Goodbudget gerçek gelir yolunu Credit/refund benzeri yoldan ayır;
  −600 farkının nedeni ve farklı bakiye düzeltmelerini ayrı senaryolarda araştır;
  ücretsiz koşumdan ürünün bütününe özellik yokluğu genellemesini kaldır.
- B08, B12, B13, B15: demo verisini kullanıcı talebi sayma; ekrandan veritabanı
  şeması çıkarma; QuickBooks ürün/paket ayrımını ve KolayBi kaynak bağlamını düzelt.
- B11: Hesap Defterim'de açık e-posta ayarını gönderim kanıtı sayma.
  Kullanıcının ayrı izni olmadan dışarıya e-posta gönderme.
- B09 ve B10: Wallet ve Bluecoins davranış açıkları P2/P3'te tam incelemeyle kapanır.
- B16–B19: yanlış görsel rolü, tam ad/bağlantı, tablo ve özet yayılım sorunlarını kapat.

Money Manager, Hesap Defterim, Goodbudget, Paraşüt, Logo İşbaşı, QuickBooks ve
KolayBi için önceki inceleme kanıtı değerlendirilir. Bütün MD içeriği ve görsel
kapsamı ele alınır; sağlam kanıtlı canlı testleri sebepsiz tekrar etmek gerekmez.
Yeni canlı koşum, açık davranış sorusuna cevap vermelidir.

Paraşüt, Logo İşbaşı, QuickBooks ve KolayBi'nin masa başı sınırı korunur.
Destek dokümanı/video davranışın canlı test edildiği anlamına gelmez.
KolayBi Ayarlar/Proje Takip, tekrarlayan liste, kısmi tahsilat ve Ödendi işaretinin
kasa etkisi gibi kalan sorular karar etkisine göre sınıflandırılır.

Eski denetim betiğinin kapı olarak kullanılabilmesi için bilinen bozuk dosya/yol,
kısa ve tam atıf, tablo hatası, olmayan giriş ve temiz örneklerle pozitif/negatif
kontroller gerekir. Hatalı durumda sıfır olmayan çıkış; olmayan dosyada açık hata
beklenir. PNG bütünlüğü ve tam ad taraması görsel içerik kontrolünden ayrı raporlanır.

## 5. P2/P3 — Kalan iki uygulamanın tam Faz 7.5 koşumu

Sıra Bluecoins, ardından Wallet. İkisinde de yalnız eski hata listesi değil,
**gözlem dosyasının tamamı ve mevcut tüm görseller** kapsam içindedir.
13 Eylül sayıları Bluecoins 90, Wallet 106; koşum başında yeniden sayılır.

Her uygulamada:

1. MD'nin tamamını oku; iddia, aritmetik, çelişki ve kaynak sınırlarını çıkar.
2. Bütün görselleri aç; içerik–metin eşlemesini envantere işle.
3. Görsellerin cevaplayamadığı davranış sorularını test listesine dönüştür.
4. Gerekli kalan soruları kullanıcıya ver; ek emülatör kontrolünü kullanıcı yapar.
5. Gerekli kalan başlangıç/sonuç kanıtlarını kullanıcıdan al; önceki zinciri tekrar koşma. Sonuçları ilgili hesap, borç ve rapor başlıklarına işle.
6. Metin, tam kanıt referansı, bulgu kaydı ve ilgili özetleri birlikte güncelle.

Bluecoins odakları:

- Tutar/bakiye zincirindeki aritmetik uyuşmazlık.
- B1 onaylı/otomatik tekrar ile B2 ilk/sonraki taksit davranışını ayırma.
- Silme, varsa çöp kutusu ve geri alma davranışını ayrı doğrulama.
- Menü/dışa aktarma adı taşıyan görsellerin gerçek içeriği; görünen seçenek ile
  fiilen üretilmiş çıktı arasındaki fark.

Wallet odakları:

- D2 borç oluşturma, Record oluşturmama ve D3 bağlı kayıt sonrası borç, hesap,
  işlem listesi ve rapor etkilerini ayrı karşılaştırma.
- No Record sonucunu tek başına gelir/gider tanıma modeliyle eşitlememe.
- Geçmiş tarih kısıtının ürün davranışı mı test/ayar koşulu mu olduğunu doğrulama.
- Aynı dönem, hesap ve ayarlar altında çelişen eski sonuçları uzlaştırma.

Yalnız sentetik veri; reset/uygulama verisi temizleme yok. Eski veriyi silerek
temiz başlangıç yaratılmaz. Zorunlu geri döndürülemez işlem ve dış etki için
ayrı kullanıcı yetkisi gerekir. Bir sonraki dönemi görmek adına sistem saati
değiştirilmez; güvenli biçimde doğrulanamayan zaman bağımlı davranış açık kalır.
Erişim engelinde iş engelli kaydedilir; ilgili diğer işler sürer. Tam koşum
yerine sınırlı kapsamla geçiş gerekiyorsa kullanıcıya açık karar olarak sunulur.

## 6. P4 — Fazlar arasında taşınacak paket

Faz 8'e ham dosyaları yığmak yerine şunlar taşınır:

1. Düzeltilmiş dokuz gözlem formu ve uygulama/platform/paket kapsam özeti.
2. Kanıt envanteri ve kaynak bağlamları.
3. Doğrulanmış veya açıkça sınırlandırılmış tematik bulgular.
4. Açık sorular, karar etkileri ve kullanılmaması gereken eski iddialar.
5. BusinessFinance karşılaştırmalarının kod/ADR referansları ve kontrol tarihi.
6. Rapor bölüm eşlemesi ve kısa Faz 8 geçiş özeti (DURUM.md içinde).

Temalar: ürün kimliği; gezinme/özet; işlem girişi; hesap/kart/transfer;
işletme/şahsi sınıflandırma; planlama/tekrar/taksit; borç/fatura/tahsilat;
raporlama; veri aktarımı/entegrasyon; diğer ürün modülleri.
Her ürünün bütün özellik yüzeyi haritada yer alır; her alan aynı test derinliğine
sahipmiş gibi gösterilmez. Çok görsel veya uzun not daha yüksek kalite demek değildir.

Bir bulgu birden fazla raporu besleyebilir, fakat kanıt kimliği sabit kalır.
Örnek: Wallet borç seçeneğinin sunumu Belge 1; gözlenen finansal etkisi Belge 2;
bizim için koşullu karar Belge 3. Belirsiz etki ilk iki rapordan üçüncüye kesin
öneri olarak taşınmaz.

## 7. Faz 8 geçiş kapısı

- [x] Dokuz uygulamanın inceleme kapsamı ve erişim sınırı yazılı. *(E0392 P4-tema-01, kanıt grupları A–D)*
- [x] Bluecoins ve Wallet tam Faz 7.5 tamam; varsa farklı kapsam için açık kullanıcı kararı var. *(P2-K, P3-K; ölçülmeyen dallar 14 Eylül kullanıcı kapanış kararıyla gerekçeli sınır)*
- [x] Tüm mevcut görseller açılıp değerlendirilmiş ve envantere işlenmiş. *(365/365; G paketleri, GB-U01, WL-U, BC-U01)*
- [x] Uygulama MD'lerinin tamamı incelenmiş; önemli iddiaların dayanağı/etiketi belli. *(P0.3 dokuz harita, P2-T, P3-T)*
- [x] Sonucu değiştiren çelişkiler çözülmüş veya ilgili iddianın kullanımı sınırlandırılmış. *(P4-B19, tema taşıma nitelemeleri)*
- [x] B01–B19 ve yeni bulgular için kapanış/engelli/sınırlandırma gerekçeleri yazılı. *(14 kapalı, 5 sınırlı; GB-U01, WL-U01–U04, BC-U01, HD-U01)*
- [x] BusinessFinance karşılaştırmaları mevcut kod ve kararlarla uzlaştırılmış. *(P1-B01–B04, P1-B14, P3-T, tema 06–10; 14–15 Eylül kod anı)*
- [x] Paket, platform, sürüm ve tarih karışıklıkları giderilmiş veya bilinmiyor işaretli. *(P1-B13, P1-B15, WL-Q01, BC-Q01/Q16)*
- [x] Düzeltmeler ilgili özet ve durum ifadelerine yayılmış. *(P4-B19; P4-K başlık uzlaştırması)*
- [x] Mekanik kontroller geçmiş; kontrol aracının hatada başarısız olduğu doğrulanmış. *(P4-K: 0 hata, 21/21; eski desenle testler düşüyor)*
- [x] Tematik aktarım ve açık soruların rapora etkisi hazır. *(P4-tema-01–10)*
- [x] Kullanıcı geçiş özetini görüp Faz 8'i açıkça onaylamış. *(15 Eylül devam mesajındaki açık onay; P4-K kayıtları kontrol edildi.)*

P4-K değerlendirmesi (15 Eylül): 11/12 kutu kanıtla işaretlendi; ayrıntı E0392 P4-K.
Son madde yalnız kullanıcı kararını kaydeder: 15 Eylül devam mesajındaki açık onayla 12/12 tamamlandı. Önceki 11/12 değerlendirmesi tarihsel P4-K anıdır.

Hedef hiçbir bilinmeyen bırakmamak değildir. Bilinmeyeni kesin sonuç gibi
yazmamak ve karar engellerini gizlememektir. Kontrol kutuları kanıt olmadan kapanmaz.

## 8. P5 — Belge 1 ve 2 üretimi

Önce ortak içindekiler ve tek bir örnek konu hazırlanır: örneğin işlem ekleme
ve işlem sonrası görünür sonuç. Birkaç uygulama, seçilmiş görseller, kaynak
bağlantıları ve iki rapora uygun farklı anlatımla pilot bölüm yazılır.
Kullanıcı ayrıntı, okunabilirlik, görsel yoğunluğu ve gelecekte başvuru değerini
değerlendirir. Pilot onayı tam rapor veya Faz 8 tamamlanma onayı değildir.

### Belge 1 — Rakip arayüz yaklaşımları

Ana soru: ürün bilgiyi ve seçenekleri nasıl sunuyor?

- Kısa ürün/hedef kullanım haritası; inceleme sınırları.
- Bilgi hiyerarşisi, gezinme ve ana ekran yaklaşımları.
- Kart/liste/grafik, renk/tipografi, tutar ve durum gösterimi.
- Formlar, seçimler, geri bildirim ve gözlenmiş boş/hata/yükleme durumları.
- Yaklaşımların kazandırdıkları/kaybettirdikleri; uygulamaya özgü ayrıntılar ve kanıt eki.

Statik ekranlardan erişilebilirlik veya kullanılabilirlik testi geçmiş sonucu
çıkarılmaz. Ölçülmeyen başarı/hız/memnuniyet değeri yazılmaz.

### Belge 2 — Rakip finansal akışlar ve özellikler

Ana soru: kullanıcı işi nasıl tamamlıyor, gözlenebilir sistem sonucu ne?

- Ürün/özellik ve erişim haritası; kaynak yöntemlerinin asimetrisi.
- Gelir/gider, kapsam ayrımı, hesap/transfer/kart işlemleri.
- Planlama, tekrarlama, taksit ve otomasyon.
- Borç/fatura/tahsilat/ödeme/iptal zincirleri.
- Rapor, veri aktarımı, entegrasyon ve diğer modüller.
- Olaylar ve akışlar arası bağlantılar; bilinmeyen arka plan adımlarının sınırı.

Her senaryoda uygun olan hesap, borç, gelir/gider, rapor, zaman ve geri alma
etkileri ayrı anlatılır. Görünmeyen veritabanı/pipeline ayrıntıları gerçek diye uydurulmaz.

Belge 1/2 rakibi anlatır, bizim ürünümüzü doğrulamak için puanlamaz. Stok,
banka bağlantısı, vergi hesaplama, e-belge gibi bizde kapsam dışı alanlar da
rakibin özellik yüzeyinde yer alır; uygulamamıza alma filtresi yalnız Belge 3'tedir.
Çalışma rekabet/ürün yaklaşımı araştırmasıdır; pazar büyüklüğü, müşteri talebi,
yaygınlık veya memnuniyet iddiası için ayrı veri gerekir. Fiyat kullanılacaksa
paket, ülke, dönem ve kaynak tarihi doğrulanır; eski fiyat güncel diye sunulmaz.

Pilot sonrası tema tema yazılır. Her bölüm için kaynak ve tutarlılık kontrolü
yapılır; sonunda çapraz rapor terminoloji ve sonuç kontrolüyle Belge 1/2
kullanıcı onayına sunulur. Onaylanan sürüm/tarih raporlar/README.md'ye kaydedilir.

## 9. P6 — Belge 3 üretimi

**Belge 1 ve 2 onaylanmadan Belge 3 taslağı yazılmaz.**
Her öneri şu yapıyla hazırlanır:

1. Kullanıcı ihtiyacı/problem ve bunun gözlem mi hipotez mi olduğu.
2. Onaylı raporlardaki bulgu kimlikleri ve rakip yaklaşımı.
3. BusinessFinance'in mevcut davranışı ve ilgili kod/karar referansı.
4. Öneri, alternatifler, kazanım, bedel ve belirsizlik.
5. Etkilenen ekran/akış/olay modeli/ortak bileşen; bağımlılıklar.
6. Araştırma README'sindeki tek kaynak karar sonuçlarından uygun olanı.
7. Geliştirmede doğrulanabilecek somut kabul ölçütü ve karar gereksinimi.

Kurucu kararla çelişen, hedef kullanıcı açısından önemli ve adı konmuş bir
boyutta avantajlı bulgular ayrı Açık sorular bölümünde tartışılır. Karar
sonuçlarının tanımları README'den kullanılır; burada ikinci tanım üretilmez.
Öneriler otomatik olarak Aşama 06.2'ye iş yazmaz; yeni özellik, finansal model
ve ADR değişiklikleri kendi karar/aşama sürecine gider. Kullanıcı onayı alınır.

## 10. P7 — Görsel politikası ve teslim

- Ana anlatımda bir farkı açıklayan seçilmiş görseller; ayrıntı kanıt ekinde;
  süreç/benzer/eski malzeme arşivde tutulur. Rapor dışı bırakmak silmek değildir.
- Görseller tam kaynak kimliği, okunabilir altyazı ve desteklediği sonuçla sunulur.
  Gerekirse kısa sıralı ekran grubu; gereksiz görsel kalabalığı oluşturulmaz.
- Markdown ana içerik kaynağıdır; onaylı Word/PDF aynı içerikten üretilir.
  Word üzerinde gelen değişiklik önce ana kaynağa işlenip tekrar üretilir.
- Pilot bölümde teknik dışa aktarım denenir: Türkçe karakter, font, tablo,
  sayfa bölünmesi, görsel okunabilirliği, altyazı ve bağlantılar. Bu deneme nihai teslim değildir.
- Nihai kontrolde üç belge arasında aynı terim/bulgu/sonuç tutarlılığı, onaylı
  sürüm eşleşmesi, kaynak yolu ve görüntülerin doğru yerleşimi doğrulanır.
- Araştırma klasörü ileride kaldırılacaksa rapor kanıtları kaybolmamalı:
  kalıcı kanıt paketi ve kaynak envanteri teslimin parçasıdır. Konum, taşıma
  ve silme ayrı kullanıcı kararıdır; bu plan hiçbir dosya silme yetkisi vermez.
- Kaynak sonradan değişirse etkilenen bulgu ve bölümler yeniden değerlendirilir;
  onaylı rapora sessizce yeni sonuç eklenmez, gerektiğinde yeniden onay alınır.

## 11. Risk ve durma kuralları

| Risk | Önlem |
|---|---|
| Çok metin, tekrar, oturumda bağlam kaybı | Kalıcı kimlikli kayıtlar; uygulama incelemesi ve bölüm yazımı küçük iş birimleri |
| Görsel sayısının kalite puanı olması | Aynı soruya verilen kanıtlı cevabı karşılaştır; asimetriyi göster |
| Her bilinmeyenin yeni teste dönüşmesi | Karar engeli / anlatım sınırı / isteğe bağlı derinlik ayrımı |
| Statik görselden davranış veya DB modeli çıkarılması | Kaynak/gözlem/çıkarım sınırlarını görünür tut |
| Kendi ürününü haklı çıkaran araştırma | İlk iki raporda tarafsız kazanım/bedel; karar filtresi üçüncüde |
| Emülatör/oturum/ücret engeli | Diğer işlere devam; engelli işi tamamlandı sayma, kapsam değişikliğini kullanıcıya sor |
| Sonradan hata bulunması | Bulgu kimliğinden etkilenen rapor bölümlerine dön, tüm süreci gereksiz yeniden başlatma |
| Görsel açıdan iyi ama işe yaramayan öneriler | İhtiyaç, bağımlılık ve kabul ölçütü zorunlu |
| Çıktı biçimi ve kaybolan kanıtlar | Erken dışa aktarım pilotu, tek içerik kaynağı, kalıcı kanıt paketi |

## 12. Sonraki oturumun kesin başlangıcı

1. Repo AGENTS.md başlangıç sırasını uygula; mevcut Git değişikliklerini koru.
2. DURUM.md'nin güncel devir notunu, bu planı ve B01–B19 denetimini oku.
   İlgili uygulama MD'si ve test protokolü okunmadan canlı işleme geçme.
3. **P0 kapandı (P0.1–P0.4); P1-B01, P1-B02-B04, P1-B14 ve P1-goodbudget-G01 kapandı; B06/B07 kapandı; B05/GB-U01 kapandı; P1-kolaybi-G01/G02, P1-B08, P1-B15, P1-hesap-defterim-G01/G02, P1-B11, P1-B12, P1-money-manager-G01, P1-parasut-G01, P1-logo-isbasi-G01, P1-quickbooks-G01, P1-B13, P1-B18 ve P1-K kapandı; sıradaki paket P2-G01.** Mevcut kayıtları
   baştan üretme; paket listesi ve önerilen sıra bulgu kaydının
   P0.4 bölümündedir. Bölüm 13 paket/devir kuralları geçerlidir.
4. P0 bitmeden doğrudan rapor taslağına veya yalnız Bluecoins emülatörüne atlama.
5. Her iş birimi sonunda DURUM.md'ye tamamlanan iş, kanıt kimlikleri,
   açık engeller ve sıradaki tek işi yaz. Uygulama bazında son incelenen
   görsel/dosya ve sonraki kimliği kaydet; bütün klasör tekrar okunmuş varsayılmasın.
6. **P0 kapandı; P1 sürüyor (P1-B01, P1-B02-B04, P1-B14, P1-goodbudget-G01, P1-kolaybi-G01, P1-kolaybi-G02, P1-B08, P1-B15, P1-hesap-defterim-G01, P1-hesap-defterim-G02, P1-B11, P1-B12 kapandı); P2–P7 başlamadı, Faz 8 açılmadı.**

## 13. Küçük iş paketleri ve kesinti sonrası devam — 13 Eylül kullanıcı isteği

### Çalışma birimi

P0–P7 üst başlıklardır, tek oturumluk işler değildir. **Varsayılan olarak bir
başlatma/devam isteğinde yalnız sıradaki bir alt paket yürütülür.** Paket
kapanınca dosyalar kaydedilir, kısa sonuç ve sonraki paket bildirilir; kullanıcı
devam demeden sonraki pakete geçilmez. Böylece oturum değiştirmek için doğal
duraklar oluşur. Kullanıcı açıkça birden fazla paket isterse kapsam buna göre belirlenir.

**Görsel paketi hedefi 20–30 görseldir; sabit kota değildir.** Aynı uygulama
ve akış birlikte tutulur; bir akış sırf sayıyı doldurmak için bölünmez.
Yoğun ve çelişkili içerikte 10–15'e düşülebilir; benzer süreç ekranlarında
30–40'a çıkılabilir. Az görselli bir uygulama tek pakette ele alınabilir;
farklı uygulamalar yalnız kotayı doldurmak için birleştirilmez. Eski 5–10
görsellik varsayılan kaldırıldı. 357 görsel için 36–72 küçük durak yerine,
uygulama sınırları ve yoğunluğa göre kabaca 15–20 görsel paketi öngörülür;
kesin dağılım P0.4'te çıkarılır. Bu sayı MD, hata ve canlı test paketlerini içermez.

**Paket büyüklüğü ile kayıt sıklığı farklıdır:** paket 30 görsel olsa da
sonuç her görselden sonra kaydedilir. Böylece daha az durakla ilerlerken
kesinti kaybı büyümez. Her pakette bütün plan ve korpus tekrar yüklenmez;
zorunlu talimatlar, güncel devir ve ilgili içerik okunur.

Paket boyutları token garantisi değildir. İçerik yoğunluğu arttığında paket
bölünür; karmaşık senaryo tek başına paket olabilir.
Kesintisiz tamamlanma garantisi verilmez. Kayıtlar limit uyarısı veya oturum
kapanışı beklenmeden, iş ilerledikçe yazılır.

### Paket haritası

| Üst iş | Alt paket | Sınır / çıktı |
|---|---|---|
| P0 | P0.1 — Dosya listesi | Dosyaları say, kanıt envanterinin kimlik/yol iskeletini kur; görsel içerik incelemesi yapma |
| P0 | P0.2 — Bulgu iskeleti | B01–B19'u doğrulama kaydına aktar; yeni doğrulama yapmış sayma |
| P0 | P0.3-uygulama — İçerik haritası | Bir uygulamanın MD başlıkları, önemli iddiaları, kapsam ve açıkları; çok uzunsa bölüm bazında böl |
| P0 | P0.4 — Paket listesi | Dokuz uygulamanın kalan incelemelerini tam dosya/kimlik listelerine böl; P0 kabul kontrolü |
| P1 | P1-uygulama-Gnn | Hedef 20–30 görsel; yoğunluğa göre 10–40, aynı uygulama/akış; her sonuç hemen kaydedilir |
| P1 | P1-Bnn | Bir hata veya sıkı bağlı küçük hata grubu; kanıt/kod/kaynak kontrolü ve belge yayılımı |
| P1 | P1-uygulama-Tnn | Tek bağımsız canlı davranış senaryosu; yalnız açık soru için |
| P1 | P1-K | Mekanik kapı ve kendi pozitif/negatif kontrolleri; uzunsa ayrı örnek grupları |
| P2/P3 | P2-Mnn / P3-Mnn | Bluecoins/Wallet MD incelemesi, bölüm bazlı |
| P2/P3 | P2-Gnn / P3-Gnn | Hedef 20–30 görsel; yoğunluğa göre 10–40, aynı akış; her birinin sonucu kalıcı |
| P2/P3 | P2-Tnn / P3-Tnn | Tek bağımsız emülatör senaryosu; başlangıç ve sonuç kaydı |
| P2/P3 | P2-K / P3-K | Uygulama bütünlük kontrolü, açıklar ve özet yayılımı |
| P4 | P4-tema | Bir temanın doğrulanmış bulgularını ve hedef rapor bölümlerini eşle |
| P4 | P4-K | Geçiş kapısı ve kullanıcıya onay özeti; Faz 8'i kendiliğinden açma |
| P5 | P5-I / P5-P | İçindekiler; ardından örnek bölüm ve teknik dışa aktarım ayrı paketler |
| P5 | P5-belge-bölüm | Bir rapor bölümü; çok uzunsa alt başlık bazlı; kaynak kontrolü dahil |
| P5 | P5-K | Belge 1/2 çapraz kontrol ve onaya sunma; gerekirse rapor başına böl |
| P6 | P6-karar | Bir karar veya en fazla üç sıkı bağlı öneri; dayanak ve kabul ölçütü |
| P6 | P6-K | Öneriler arası tutarlılık ve kullanıcı onayı |
| P7 | P7-belge-biçim | Bir belgenin Word/PDF üretimi ve render kontrolü; gerekirse biçim bazlı böl |
| P7 | P7-K | Kalıcı kanıt paketi, bağlantılar ve son teslim kontrolü |

P0 görsellerin tamamını açma işi değildir: kapsamı ve takip kayıtlarını kurar.
Görsel içerik incelemesi P1, P2 ve P3'ün küçük G paketlerinde yapılır. Paketlerin
tam dosya listeleri P0.4'te güncel envanterden oluşturulur; sıra numarası tek
başına yeterli değildir. Yeni dosya eklenmesi tamamlanan işin kimliğini kaydırmaz.
P1'in bir uygulamayı yeniden derinleştirmesi P2/P3 işini tekrar üretmemelidir;
aynı bulgu ve kanıt kimliği bir kez tutulur, diğer paket ona bağlanır.

### Kalıcı ilerleme kaydı

Yeni ayrı oturum raporları açılmaz. Kanıt envanteri görsel bazlı sonucu,
bulgu kaydı test/hata sonucunu, DURUM.md ise küçük güncel devir özetini tutar.
Her paket için bulgu/doğrulama kaydında paket kimliği, tam hedef listesi,
durum, tamamlanan kayıtlar ve kalanlar bulunur. İlk P0.1 paketi kurulmadan
önceki başlangıç noktası DURUM.md'de yeterlidir.

Her görsel incelendikten sonra kanıt satırına sonucu ve ilgili bulgu yazılır;
paketin tümünü veya oturum sonunu bekleme. Bir testte değişiklikten önce
başlangıç durumu ve niyet, değişiklikten sonra gözlenen sonuç kaydedilir.
Raporlarda biten alt başlık kaydedilir; yarım bölüm açıkça taslak kalır.

DURUM.md güncel devir şeması:

- Aktif/son paket kimliği ve durum: başlanmadı / sürüyor / kısmi / kapandı / engelli.
- Kapsamın bulunduğu dosya/bölüm; tamamlanan kanıt ve bulgu kimlikleri.
- Yarım kalan tam dosya veya senaryo; son doğrulanmış adım.
- Kaydedilmiş çıktılar; henüz kontrol edilmemiş sonuçlar.
- Emülatörde yapılmış veri/ayar değişiklikleri ve doğrulanacak mevcut durum.
- Sıradaki tek somut işlem; varsa kullanıcıdan gereken erişim/karar.

Paket ancak hedef listesindeki her öğe sonuç veya gerekçeli açık durumla
kaydedildiğinde kapanır. Açık davranışın kayıt altına alınması, testin başarıyla
tamamlandığı anlamına gelmez; üst fazın kabul kapısı ayrıca değerlendirilir.

### Oturum aniden kapanırsa

1. Yeni oturum repo başlangıç kuralları ve DURUM.md devir notunu okur.
2. İlgili paketin hedef listesini kanıt/bulgu kayıtlarıyla karşılaştırır.
   DURUM.md geride kalmışsa ayrıntı kayıtlarını esas alıp devri uzlaştırır.
3. Sonuçları kaydedilmiş, dosyası değişmemiş öğeleri yeniden incelemez.
   Değişmiş bir kaynak varsa yalnız etkilenen öğe/bulgu tekrar açılır.
4. Açıldığı halde sonucu yazılmamış görsel **tamamlanmış sayılmaz**; o görsel
   yeniden incelenir. Kayıtsız akıl yürütmenin yeni oturuma taşındığı varsayılmaz.
5. Canlı testin sonucu belirsizse kayıt ekleme/ödeme/tekrar üretme gibi işlemi
   körlemesine yeniden yapmaz. Önce uygulamadaki mevcut veri ve son kanıtı
   salt okunur karşılaştırır; aksi hâlde çift kayıt oluşabilir. Belirsizlik
   giderilemiyorsa güvenli yeniden kurulum veya kullanıcı yönlendirmesi ister.
6. Kullanıcının devam isteğiyle ilk tamamlanmamış öğeden, yalnız mevcut paketi
   tamamlayacak şekilde sürdürür. Aynı erişim/çalışma alanı yoksa bunu açıklar.

Örnek (temsili, gerçek ilerleme değildir): P2-G04'te on görselin altısının
sonucu kaydedilmiş, yedincisi açıkken oturum kesilmiş olsun. Yeni oturum
yedinciyi yeniden açar, kalan dördü bitirir; ilk altıyı ve bütün Bluecoins
klasörünü baştan incelemez. Son kaydedilmemiş öğe için sınırlı tekrar gerekebilir.

Bu kuralın amacı tüm tekrar ihtimalini sıfırlamak değil, kesintinin etkisini
son kaydedilmemiş küçük işe indirmektir. Kayıtlar aynı çalışma alanında
korunduğunda devam sohbet hafızasına değil dosyalara dayanır.
