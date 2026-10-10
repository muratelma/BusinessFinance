# Gün sonu paneli — teslim karşılaştırması

10 Ekim 2026. Çalışma dalı `gun-sonu-panel`, başlangıç `ui-trials` son
commit'i `f041286`. Yalnız Flutter ve ilgili belgeler değişti; backend,
sözleşme ve migration değişmedi. Commit ve push yapılmadı.

Ana kaynak [teslim README'si](../design/gun-sonu-handoff/gunsonu/README.md).
Sekiz hâl gerçek `showDayCloseForm` panelinden, teslimdeki sentetik verilerle
çizildi. JSX/HTML'den kod alınmadı. Görseller Git dışındaki
`tasarim-onizleme/gun-sonu-panel/` klasöründedir.

## Kare kare sonuç

| Teslim | Gerçek panel | Karşılaştırma |
|---|---|---|
| [01 — Cevapsız](../design/gun-sonu-handoff/screenshots/01-g1-cevap-bekleyen.png) | [01](../tasarim-onizleme/gun-sonu-panel/gunsonu-01-g1-cevap-bekleyen.png) | Hazır işaretliler üstte; ray seçimsiz; üç `?` satırı; sayı yerine üç kayıt cümlesi; kaydet kapalı, uyarı yok |
| [02 — Cevaplı](../design/gun-sonu-handoff/screenshots/02-g2-cevaplanmis-ve-hesap.png) | [02](../tasarim-onizleme/gun-sonu-panel/gunsonu-02-g2-cevaplanmis-ve-hesap.png) | İki tahsilat dahil, veresiye satış dışarıda; karışık ray seçimsiz; yeni satış +₺1.000,00; zaten kayıtlı −₺670,00 ve dökümü |
| [03 — Bazıları](../design/gun-sonu-handoff/screenshots/03-g3-bazilari-dahil.png) | [03](../tasarim-onizleme/gun-sonu-panel/gunsonu-03-g3-bazilari-dahil.png) | Yalnız Ahmet'in tahsilatı dahil; yeni satış +₺1.120,00; zaten kayıtlı −₺550,00 |
| [04 — Kısmi cevap](../design/gun-sonu-handoff/screenshots/04-g4-satis-ve-tahsilat-birlikte.png) | [04](../tasarim-onizleme/gun-sonu-panel/gunsonu-04-g4-satis-ve-tahsilat-birlikte.png) | Mehmet'in çiftinin altında soru; üç seçenek ve sunucu tutarları; ₺200 kısmi cevapla sağda ₺600; yeni satış +₺1.000,00 |
| [05 — Toplam farkı](../design/gun-sonu-handoff/screenshots/05-g5-toplam-farki.png) | [05](../tasarim-onizleme/gun-sonu-panel/gunsonu-05-g5-toplam-farki.png) | Yalnız POS kartı; +₺500,00 satış; ₺1.300 fark için README'nin tam cümlesi; neden tahmini yok |
| [06 — Fatura](../design/gun-sonu-handoff/screenshots/06-g4b-alacak-faturasi-ve-tahsilat.png) | [06](../tasarim-onizleme/gun-sonu-panel/gunsonu-06-g4b-alacak-faturasi-ve-tahsilat.png) | Fatura ve kendi tahsilatı; `Tahsilat faturanın içinde`; yeni satış +₺1.200,00; dökümde alacak faturası |
| [07 — İki kişi](../design/gun-sonu-handoff/screenshots/07-g4c-ayni-gun-iki-kisi.png) | [07](../tasarim-onizleme/gun-sonu-panel/gunsonu-07-g4c-ayni-gun-iki-kisi.png) | İki ayrı soru kendi satırlarının altında; Mehmet ayrı, Ahmet'te satış tahsilatın içinde; yeni satış +₺1.100,00 |
| [08 — Nakit boş](../design/gun-sonu-handoff/screenshots/08-g6-nakit-bos-kartli-kayit.png) | [08](../tasarim-onizleme/gun-sonu-panel/gunsonu-08-g6-nakit-bos-kartli-kayit.png) | Nakit satırları ve soru gizli; kartlı kayıt görünür; ₺1.300 kart tutarı, −₺800 zaten kayıtlı, +₺500 yeni satış |

İlk çizim karşılaştırmasında eksik artı işareti, ham tarih, fazla kalın
döküm ve boş alanda görünmeyen para birimi düzeltildi. Dökümde satış/fatura
tahsilattan önce yazılır. Son sekiz kare elle incelendi.

## README ile kareler arasındaki ayrımlar

- Karelerde soru bloğunun adı ve sağdaki tutarın açıklaması yok. README
  istediği için ikisi de eklendi.
- Fatura karesinin dökümünde `veresiye satış` yazıyor. README'nin tür ayrımına
  ve `cash.deductions.invoicesAmount` alanına göre `alacak faturası` yazıldı.
- Ortak `AppFormSheet`/`AppAdaptiveSheet` kabuğunun tutamacı, 24 dp yatay
  boşluğu ve mevcut `Vazgeç` düğmesi korunur. Panel içindeki bölüm aralıkları
  16 dp, kayıt satırları en az 56 dp, sol hedef 48 dp, radyo satırları 48 dp,
  soru ve özet kartları 16 dp yarıçaplıdır. Çizimler 412 dp genişlikte,
  panelin tamamını gösterecek yükseklikte ve 2× çözünürlükte alınır.
- Repo `AppTextField` adlı bir bileşen içermiyor. Alanlar mevcut tema ile
  `TextFormField` kullanır; yeni bileşen sistemi veya token eklenmedi.

## Teslimde çizilmemiş, kurulan hâller

| Hâl | Uygulanan anlatım/davranış |
|---|---|
| Yalnız nakit + toplam | `Yalnız nakit satış kaydedilir. Toplamla arasındaki ₺… kaydedilmez.`; yalnız nakit kartı |
| Nakit + kart + toplam farkı | `Nakit ve kart satışı kaydedilir. Toplamla arasındaki ₺… kaydedilmez.`; sayı sunucudan |
| Ek gün sonu | Mevcut kapalı gün bildirimi ve `Ek gün sonu` kutusu; açılınca yeni yerleşim, sunucunun işaretsiz varsayılanları |
| Soru adı boş | Faturada `Alacak faturası`; kişi sorusunda `Satış ve tahsilat`; kayıtta boş ad türün adıyla gösterilir |
| Aynı kişide ikiden fazla kayıt | Kişinin satırları bitişik; sorusu son satırının altında. Dışarıda bırakılan başka satır, dahil satış/tahsilat çiftinin sorusunu gizlemez |
| Liste yenilenince bir kayıt kaybolması | Duran kayıtların cevapları kalır; eski soru cevabıyla birlikte seçili radyo ve kısmi alan da temizlenir |
| Kısmi seçenek seçili, alan boş | Sağda sonuç yok; nakit kartında `Yukarıdaki soru cevaplanınca hesaplanır.`; kaydet kapalı |
| Önizleme bekleniyor | Kaydet kapalı; nakit kartında `Hesaplanıyor…`; kısmi seçeneğin eski sonucu gizli |
| Yazılacak bütün tutarlar sıfır | Sıfır tutarlar ve `Yazılacak kayıt yok; gün kapatılır.`; gün sunucunun izniyle kapanır |
| Uzun ad / 2.0× yazı | Ad kırpılmaz; tutar başlığın altına iner. 375 dp genişlikte açık/koyu tema ve erişilebilirlik kontrolleri geçti |
| Kasa/kategori çözülememesi | Mevcut iki seçim alanı kendiliğinden açılır; eksik hedef adı açıkça söylenir |
| POS tanımı olmaması | Nakit ve toplam alanları kalır; POS alanı ya da kart kartı üretilmez |

## Para ve cevap güvenceleri

Bütün satış sonuçları, döküm, seçenek sonuçları, azami kısmi tutar, komisyon ve
toplam farkı mevcut önizleme alanlarından gösterilir. Eksik bir sunucu sayısı
yoktur. İstemci toplama, çıkarma veya tutar kıyası yapmaz. Mevcut
`MoneyMath.fromInput` yalnız girdi dönüşümü, `TurkishAmountInputFormatter`
yalnız Türkçe binlik biçimi için kullanılır; toplam/fark aritmetiği çağrılmaz.
19 haneli para ve dört ondalık testi hassasiyetin korunmasını doğrular.

Seçili radyo ayrı panel durumudur. Ayrı cevap `0.0000`, içinde cevabı
`maximumOverlap`, kısmi cevap yazılan tutardır. `DayCloseAnswers` değişmedi;
onun yenileme ve cevap iptali sonuçları panelin radyo/alan durumuna da
uygulanır. Cevapsız kayıt onay kutusu ve karışık durumuyla okunur.

## Teslim ve sonraki adım

Akış, tasarım sistemi, test kapsamı, aktif aşama ve proje durumu belgeleri
güncellendi. Kontroller: analyze temiz; `dart format --set-exit-if-changed
lib test` 338 dosyada 0 değişiklik; tam Flutter koşusunda **1135 test geçti,
129 görüntü testi atlandı**; gün sonu dosyasında **54 test** geçti; etkin
görüntü koşusu **13/13**; son kodla Android debug APK derlendi. Kontrollerin
kaydı `docs/project-status.md`'dedir.
`gunsonu-04-toplamdan` çizim testi kaldırıldı. Sekiz panel ve korunmuş beş
ayrıntı/Kasa karesi başarıyla çizildi; PNG'ler Git'e eklenmedi.

Sıradaki görev ekranların kullanıcı tarafından incelenmesi ve commit
onayıdır. Devir notunun §0/§5'teki commit öncesi inceleme kuralı nedeniyle
commit atılmadı; push yapılmadı. Canlı API yeniden başlatılmadı ve cihaza
kurulum yapılmadı; bu oturumun doğrulaması sentetik widget testleri,
gerçek panel çizimleri ve debug APK derlemesidir.

## İnceleme (aynı gün, ikinci göz)

Kareler bugünkü koddan yeniden üretilip teslimin sekiz görüntüsü ve
`GSParts.jsx` ile karşılaştırıldı. Dört yer teslime çekildi:

- Soru bloğunun altındaki "Sağdaki tutar bu ikisinden zaten kayıtlı sayılan."
  cümlesi kalktı; teslimde olduğu gibi tutar sütununun üstünde `Kayıtlı
  sayılan` yazar. Cümle teslimde yoktu, devir notunun ilk canvas turundan
  kalma tarifinden gelmişti.
- İşaretli gelen satır yokken kural cümlesinin altına çizgi çizilmez.
- Kayıt ve seçenek satırlarında tutar satırın ortasına hizalanır.
- `İkisinde de sayılan` alanı öbür tutar alanları gibi binlik ayırıcıyla yazar.

Bilinen, bırakılan farklar: `Vazgeç` ortak kabuktaki gibi düz yazı;
`Diğer POS'lar` yanında ok yok; tarih `12 Ekim Pazartesi` (teslimde `12 Eki`).
Soru bloğunda kişinin adı **yazılmaz** (kullanıcı kararı, 10 Ekim 2026: Claude
Design'da da yok; README'deki "ad" satırı uygulanmadı). Yukarıdaki "Soru adı
boş" satırı bu yüzden geçersizdir: blokta ad hiç yazmaz.

Kontroller (düzeltmelerden sonra): analyze ve format temiz; Flutter 1135
(129 atlanır); 13 kare yeniden üretildi.

## Üçüncü tur (kullanıcı kararları, aynı gün)

- **Yan boşluk 16 dp.** Kayan satırların nedeni iki şeydi: temanın küçük
  yazıdaki 0,4 harf aralığı (teslimde yok) ve ortak form kabuğunun 24 dp yan
  boşluğu (teslimde 16). Kural cümlesinde ve soru cümlesinde harf aralığı
  sıfırlandı; `AppFormSheet` `horizontalPadding` aldı ve yalnız gün sonu paneli
  16 dp verir. Öbür formlar 24 dp'de kalır; beğenilirse zamanla onlara da
  uygulanır, beğenilmezse bu panel 24'e döner (kullanıcı). Kural cümlesi ve
  fatura sorusu artık tek satırdır; kare yükseklikleri teslimle 1–24 dp
  içinde tutuyor.
- **Soru bloğunda ad yok**, **satırda saat yok.** Sunucunun gönderdiği
  `createdAtUtc` alanı kimse okumadığı için sözleşmeden ve modelden kalktı.
- **Gün ekranı panelle aynı kelimeleri kullanır:** adsız tahsilat `Tahsilat`
  (eskiden `Cari tahsilat` / `Alacak tahsilatı`), kartlı kayıt `Kartla`
  (eskiden `Kart`).
