# Belge 1 — Bütün harita

> **24 Eylül 2026 notu:** bu belge 17 Eylül'ün 13 bölümlük düzenini anlatır ve tarihseldir. Düzeltme
> turunda eski Bölüm 1 (okuma kılavuzu) ve 2 (ürün kimliği) yeni **Bölüm 1 · Giriş ve incelenen ürünler**de
> birleşti; eski 3–13 bugün 2–12'dir: 3→2 ana ekran · 4→3 görsel dil · 5→4 işlem ekleme · 6→5 hesap,
> kart, transfer · 7→6 sınıflandırma · 8→7 plan · 9→8 borç · 10→9 rapor · 11→10 veri · 12→11 modüller ·
> 13→12 ortak tercihler. Alt soru numaraları da birer azaldı (ör. eski 8.4 → 7.4). Güncel durum:
> [belge1-duzeltme-plani.md](../belge1-duzeltme-plani.md) §12.

16 Eylül 2026. Bölüm planının ([`../belge1-bolum-plani.md`](../belge1-bolum-plani.md)) belge
düzeyindeki karşılığıdır. Plan her bölümün **ne anlattığını** söyler; bu dosya on üç bölümün
**birlikte tek belge olarak** nasıl çalıştığını söyler. Plan ile çelişirse plan geçerlidir.

---

## 1 · Belgenin biçimi

- **Tek PDF:** `tam/belge1.pdf`. Kapak ve içindekiler → Bölüm 1–13 → Kanıt eki. Sayfa numarası
  belge boyunca sürer. Her bölüm ayrıca kendi klasöründe tek başına da üretilir (denetim için).
- **Tek yerleşim kaynağı:** `ortak/kalip.py`. Bölümler yalnız veri taşır (`icerik.py`); yerleşim
  kalıpları (A soru · B yan yana · C şerit · D kart · E tablo · metin) hepsinde aynıdır.
- **Şekil numarası** bölüm içinde sıralıdır (`Şekil 6.4`). Bölümler arası gönderme şekil
  numarasıyla değil **alt soru numarasıyla** yapılır (`→ 8.4`), çünkü bölümler tek başına da
  basılır ve otomatik kapı kırık şekil göndermesini hata sayar.
- **Kanıt türü kareden çıkar**, elle yazılmaz: KolayBi destek görseli *kaynak görseli*, canlı
  ürün karesi *canlı kare*. Paraşüt, Logo İşbaşı ve QuickBooks karesini basmaya kalkmak ve KolayBi
  video karesini basmak üretimi durdurur (K4, Ö1). Karesiz cümlelerin türü (koşum kaydı, kaynak
  beyanı, görülmedi) satırın yanında rozetle gösterilir.
- **Kişisel veri** belge genelinde tek listede karartılır (`kalip.KARARTMA_ORTAK`): Goodbudget'ın
  hane adını taşıyan altı karesi. Wallet E0375 hesap sahibinin adını taşıdığı için **hiç basılmaz**
  (`kalip.YASAKLI`); çekmecenin alt kısmı E0376 kullanılır.

## 2 · Ortak sözlük

Aynı şey bütün bölümlerde aynı adla anılır.

| Terim | Anlamı | Kullanılmaz |
|---|---|---|
| ana ekran | Uygulamanın açılışta gösterdiği yüzey | pano (yalnız KolayBi'nin kendi adı "Güncel Durum panosu") |
| çekmece | Yandan açılan menü | hamburger menü, drawer |
| kayıt düğmesi | Yeni kayıt başlatan düğme | FAB (yalnız dosya adlarında) |
| kayıt | Kullanıcının girdiği hareket | işlem (ürün kendi arayüzünde "işlem" diyorsa etiket olarak aynen yazılır) |
| defter | Hesap Defterim'in hesap birimi | — |
| görülmedi | Kanıt yok | "yok" (yalnız karede ya da koşumda yokluk gözlendiyse, kapsamıyla) |
| incelenen beş canlı ürün | Money Manager, Bluecoins, Wallet, Hesap Defterim, Goodbudget | "hepsi", "rakipler" |
| kaynakla incelenen ürünler | KolayBi, Paraşüt, Logo İşbaşı, QuickBooks Solopreneur | — |

Ürün arayüz etiketleri ekranda göründüğü dilde ve harfle yazılır.

## 3 · Ayrıntının sahibi (Ö7)

Bir konu birden çok bölüme değiyorsa ayrıntı **tek** bölümde anlatılır; ötekiler bir cümle bağlam
verir ve gönderme yapar.

| Konu | Sahibi | Kısa bağlam veren |
|---|---|---|
| İlk açılış, ana ekran, bölüm seçici, çekmece, boş ana ekran | 3 | 2, 4 |
| Kayıt düğmesinin yeri | 3.5 | 5.1 |
| Form alanları, tür seçimi, tutar girişi, kayıt sonrası, hata, silme | 5 | 4.5, 7.1 |
| Hesap listesi, açılış alanı, kart yüzeyi, kart ödemesi, transfer | 6 | 10, 13 |
| Tutar biçimi, renk, durum sözcüğü, yükleme görünümü | 4 | — |
| Kategori, etiket, proje ekseni, işletme/şahsi alanı | 7 | 5.2, 9 |
| Tekrar, taksit, bekleyen liste, onay, bütçe, hedef | 8 | 3.2 (Wallet kartı), 6 (kartta taksit) |
| Cari/karşı taraf, borçlandırma, tahsilat, kalan | 9 | 7.4 |
| Rapor ekranı, toplamın etiketi, dönem, filtre, toplamdan çıkarma | 10 | 6.1 |
| Dışa aktarma, içe aktarma, ek dosya, **fiş okuma**, yedek, üçüncü kişi | 11 | 5.2 (ek alanının varlığı) |
| Yardımcı modüller | 12 | 3.4 (çekmecede görünmesi) |
| Kanıt türleri, erişim düzeyi, neden basılmıyor | 1 | her bölümün açılışı |
| Çapraz okuma | 13 | — |

Belge 2'nin konusu (kaydın bakiyeye ve rapora etkisi, toplamların içeriği) hiçbir bölümde
anlatılmaz; gerekiyorsa "→ Belge 2" yazılır.

## 4 · Karelerin dağılımı

Bir kare birden çok bölümde basılabilir; **işaretli ayrıntısı** yalnız sahibi olan bölümdedir.
Diğer bölüm kareyi sade, kırpılmış veya küçük basar.

| Bölüm | İşaretli basılan kareler |
|---|---|
| 2 | Ana ekranlar küçük ve sade (sahibi 3); KolayBi E0206 sade |
| 3 | E0017 E0135 E0106 · E0228 E0103 E0115 (+E0276 E0137 sade) · E0376 E0171 E0084 · E0137 + dört kayıt düğmesi · E0227 E0026 E0136 · E0211 |
| 4 | Kırpıntı şeritleri: tutar, renk, durum, dönem denetimi, boş/hata/yükleme |
| 5 | E0324 · E0229 E0020 E0277 E0138 E0116 · E0327 · E0339 · E0141 E0155 E0161 E0311 E0118 · E0232 E0281 E0117 E0347 E0162 · E0143 E0167 E0024 E0125 E0089 · E0192 E0200 |
| 6 | E0235 E0027 E0283 E0146 E0111 E0112 · E0239 E0240 E0048 E0145 · E0233 E0298 E0296 E0114 · E0236 E0255 · E0237 E0248 E0050 E0333 · E0230 E0029 E0030 E0327 E0285 E0147 E0140 E0119 · E0206 E0207 E0208 |
| 7 | E0229 (kategori paneli) E0349 E0116 E0139 E0151 · E0088 E0278 · E0188 E0189 E0190 · E0194 E0201 |
| 8 | E0241 E0244 E0246 · E0039 E0034 E0035 · E0288 E0291 E0292 · E0133 · E0056 E0047 E0306 · E0274 · E0302 E0304 E0305 · E0205 E0216 |
| 9 | E0072 E0074 E0075 E0079 · E0307 E0308 E0364 E0365 E0366 E0368 E0370 · E0194 E0195 E0196 E0197 |
| 10 | E0231 E0250 E0253 · E0023 E0082 E0086 E0098 · E0280 E0278 · E0121 E0122 · E0142 E0144 E0148 E0178 · E0213 E0214 |
| 11 | E0169 E0175 E0176 E0157 E0158 E0149 · E0105 E0091 · E0254 · E0313 · E0376 · E0196 E0198 |
| 12 | Karesiz tablo |
| 1, 13 | Karesiz |

## 5 · Sayfa bütçesi ve sıra

Plan §2'deki ≈78 sayfa hedef değildir. Yazım sırası planındır (3 → 5 → 6 → 2 → 4 → 7 → 8 → 9 →
10 → 11 → 12 → 1 → 13 → Ek); **belge sırası** 1 → 13 → Ek'tir.

## 6 · İlerleme ve devir notu — 17 Eylül 2026

**Yazıldı, üretildi, otomatik kapılar temiz, bütün sayfalar önizlemeden tek tek incelendi (Belge 1 tamam):**

| Bölüm | Klasör | Sayfa | Basılan kare |
|---|---|---:|---:|
| 2 | `bolum-02-urun-kimligi` | 5 | 6 |
| 3 | `bolum-03-ana-ekran` | 9 | 15 |
| 4 | `bolum-04-gorsel-dil` | 6 | 23 |
| 5 | `bolum-05-islem-ekleme` | 11 | 22 |
| 6 | `bolum-06-hesap-kart-transfer` | 9 | 30 |
| 7 | `bolum-07-siniflandirma` | 7 | 12 |
| 8 | `bolum-08-plan-ve-bekleyen` | 10 | 25 |
| 9 | `bolum-09-borc-ve-tahsilat` | 7 | 15 |
| 10 | `bolum-10-rapor-ve-donem` | 8 | 17 |
| 11 | `bolum-11-veri-aktarimi` | 7 | 13 |
| 12 | `bolum-12-diger-moduller` | 3 | 0 (tablo) |
| 1 | `bolum-01-okuma-kilavuzu` | 5 | 0 |
| 13 | `bolum-13-ortak-tercihler` | 5 | 0 (tablo) |
| — | `tam/` birleşik belge | 98 | 145 farklı kare; 157 kimlik hash denetiminde |

Motorun raporundaki "Gözle sayfa incelemesi yapılmadı" satırı sabit bir hatırlatmadır; bu on üç bölümün
sayfaları incelendi ve bulunan sorunlar (üstüne binen işaret, kesik kırpıntı, nicelik hatası, fontta olmayan karakter) düzeltildi.

**Belge 1 tamamlandı (17 Eylül).** Birleşik belge `tam/belge1.pdf`: kapak, içindekiler, Bölüm 1–13 ve
dört sayfalık kanıt eki; bölüm başlangıçları `tam/rapor.json` içinde. Bütün `→ N.M` göndermelerinin var olan
bir alt soruya çıktığı betikle denetlendi. Sıradaki adım patron onayı; ardından Belge 2. Commit yapılmadı.

**Açık kalanlar:** her bölümün `eksik-listesi.md` dosyası (yeni koşum gerektirenler "Önerilmez");
planın §7 açık işleri (Wallet bekleyen kartının yüklenmiş hâli, Bluecoins ve Hesap Defterim açılış
sekmesi/defteri) değişmedi. WL-U ve GB-U kareleri kanıt dizininde olmadığı için basılamıyor.

**Bölüm 9–13 (17 Eylül) — plandan sapmalar:**

- Bölüm 9: 9.5 iki sayfa (cari akışı; ekstre önizlemesi ve karesi basılmayan ürünler). KolayBi E0197'deki
  PDF önizlemesinin dosya adı, adresi, telefonu ve e-postası bölüm kopyasında karartıldı.
- Bölüm 10: 10.1 iki sayfa (beşli genel karşılaştırma + işaretli tablo/takvim sayfası); plandaki "dönem
  seçimi" Kalıp C şeridiyle basıldı.
- Bölüm 11: plan "bir tablo + iki sayfa anlatım" diyordu; beş soru sayfası ve sonda özet tablosu oldu.
  Ortak sınır ("seçenek ≠ dosya ≠ teslim") yalnız açılışta.
- Bölüm 12: iki tablo (işletme modülleri, yardımcı araçlar); hücrede "Var · kanıt kimliği".
- Bölüm 1: planın 2 sayfası 5 oldu (açılış, ürün tablosu, kanıt türleri, basılmayan kareler, okuma).
  "Uzaktan kaynak" örneği Paraşüt'ün E0262 tanıtım karesidir; kare basılmadı.
- Bölüm 13: üç tablo; 13.3 iki sayfa. Kaynakla incelenen ürünlerin tek-ürün yüzeyleri kanıt düzeyi farklı
  olduğu için 13.3'e alınmadı, notta anıldı.
- İnceleme sırasında 8.1'in girişi düzeltildi: Wallet'ın planlı ödemesi normal formun içinde değil,
  kendi formunda. Bölüm 2'nin ilk kart sayfasının başlığındaki "— 1" kaldırıldı.
- Gözlem formları (E0005–E0014) çalışma boyunca güncellendiği için hash denetimine girmez; birleşik
  belgenin denetimi yalnız karelere uygulanır.

**Bölüm 8 (17 Eylül) — plandan sapmalar ve notlar:**

- Plan 7 sayfa diyordu; 10 sayfa oldu. 8.1 ve 8.6 ikişer sayfa (KolayBi tekrarlı maaş formu; Goodbudget
  zarfı ile KolayBi nakit akış raporu). Bölüm sonunda karesiz bir **özet tablosu** var; yeni kanıt taşımaz.
- §4'teki listeye eklenen kareler: E0245 (MM taksit formu), E0204 (KolayBi İşlemler menüsü), E0293
  (Wallet ana ekranın üstü). E0238, E0287, E0290, E0318 notlarda veya şekillerde kullanıldı.
- 8.2'de plan "kaydetmeden önce altı kalem özeti" diyordu; kare **tek cümlelik bir özet** gösteriyor
  (E0035), metin buna göre yazıldı.
- 8.4, 3.2'deki "→ 8.4" göndermesini karşılıyor. KolayBi ve Paraşüt satırları 3.7'ye gönderiyor.
- Wallet'ın Postpone/Dismiss (WL-U02-A) ve Records menüsü (WL-U03-A) kareleri **kanıt dizininde yok**
  (dizin yalnız E kimliği okuyor); basılmadı, metinde koşum kaydı olarak anıldı. Basılması gerekirse
  `kanit-dizini-uret.py` önce WL-U/GB-U kimliklerini okuyacak şekilde genişletilmeli.
- **Motorun kaçırdığı bir sorun:** `⋮` karakteri Segoe UI'da yok ve PDF'te kutu olarak basıldı; "bozuk
  karakter" kapısı bunu yakalamadı. Metinde "üç nokta menüsü" yazıldı. Özel simge karakterlerinden kaçının.

**Nasıl çalışıldı — yeni oturum aynı yolu izler:**

1. Karenin ne gösterdiği `ortak/kanit-dizini.json` açıklamasından bulunur.
2. `ortak/izgara.py <geçici.png> E.... E.... E....` ile özgün piksel koordinatlı ızgara basılır ve
   görüntü açılır. İşaret koordinatı **metnin, simgenin ve düğmenin üstüne binmeyecek** boş alana konur
   (çekmece simgeleri ve satır tutarları Bölüm 3 ve 6'da bu yüzden kaydırıldı).
3. `icerik.py` yazılır; `uret.py` her bölümde aynı üç satırlık başlatıcıdır (Bölüm 3'ten kopyalanır).
4. Üretimden sonra `onizleme/sNN.png` sayfaları tek tek açılır. Sık görülen sorunlar: kırpıntı kutusunun
   satırı yarıda kesmesi, işaretin metne değmesi, "dört üründe" gibi nicelik belirtecinin karelerle
   uyuşmaması, bir hücrede rozet ile metnin aynı sözcüğü tekrarlaması.

**Kalıbın (`ortak/kalip.py`) bu oturumda eklenen yetenekleri:** sayfa türleri `acilis` · `soru` ·
`yanyana` · `serit` · `kartlar` · `tablo` · `metin`; şekil göndermesi `[[anahtar]]`; geniş kareleri üst
üste dizen `"dikey": True`; sağ sütunda kırpıntı grubu (`kirpintilar`, `kirpinti_h`, satır kırma);
sayfa haritası için iki geçişli basım; kanıt türünün kareden türetilmesi ve K4/Ö1'in zorlanması;
belge geneli karartma (`KARARTMA_ORTAK`) ve hiç basılmayan kare (`YASAKLI`: E0375).

**Açık kararlar ve dikkat edilecekler:**

- KolayBi destek görsellerindeki demo kişi adları içerik için gerekmiyorsa bölümün `KARARTMA`
  sözlüğüyle karartılır (Bölüm 7'de E0194, E0201). Bölüm 9 ve 12'de E0196, E0197, E0202, E0203
  kullanılırsa aynısı yapılmalı.
- KolayBi karelerinin çoğunda siyah zemin var; `kirpma: (96, 112, 1305, 865)` iç pencereyi verir.
  Alt yarısı boş olan ekranlar daha kısa kırpılır (Bölüm 6: E0206, E0208).
- "Kar / Zarar" gibi rakip etiketleri tırnak içinde, ekranda göründüğü gibi yazılır; kendi cümlemizde
  "kâr" kullanılmaz.
- Bölümler arası gönderme şekil numarasıyla değil alt soru numarasıyla yapılır (`→ 6.4`).

## 7 · Nasıl üretilir

```bash
# bir bölüm (denetim dosyaları + önizleme onun klasörüne)
raporlar/.pilot-tools/venv/Scripts/python raporlar/belge1/bolum-03-ana-ekran/uret.py
# bütün belge (on üç bölüm + kapak + kanıt eki)
raporlar/.pilot-tools/venv/Scripts/python raporlar/belge1/tam/uret.py
```
