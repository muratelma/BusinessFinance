# İddia tablosu — Belge 1 · Bölüm 3 · Gezinme ve ana ekran

16 Eylül 2026 · v2. Bölüm metni yalnız bu tablodan yazılır. Kural özet, tablo, altyazı ve yorum için de geçerlidir. Önce mevcut dayanaklar aranır; ifadenin kapsamını taşıyan ek kare varsa eklenir. Yetersizse ifade gözlenen kapsamla sınırlanır. Birden çok ana bölüm karesi bütün alt ekranları kanıtlamaz.

Tür değerleri: **Kare** (ekran görüntüsünde görülüyor) · **Koşum kaydı** (gözlem formunda yazılı, karesi yok veya kare tek başına göstermiyor) · **Görülmedi** (kanıt yok; metinde açıkça "görülmedi" yazılır). Eksik kanıtlar [eksik listesinde](eksik-listesi.md).

Formlar: E0010 Money Manager, E0005 Bluecoins, E0014 Wallet, E0007 Hesap Defterim, E0006 Goodbudget.

## 3.1 İlk karşılaşma ve açılış ekranı

| No | Uygulama | İfade | Dayanak | Tür |
|---|---|---|---|---|
| 01 | Money Manager | Kayıt, giriş veya karşılama ekranı yok; doğrudan ana ekran açılıyor | E0010 K00 (kullanıcı doğruladı) | Koşum kaydı · E1 |
| 02 | Money Manager | Ana ekran İşlemler › Gün | E0010 K01; E0228 | Kare |
| 03 | Money Manager | Dil ve para birimi sorulmuyor; varsayılan USD | E0010 K00 | Koşum kaydı |
| 04 | Bluecoins | Karşılama metni, Hadi Başlayalım düğmesi, altta dil seçici | E0017 | Kare |
| 05 | Bluecoins | Veri yokken Hesaplar sekmesinde Hoş geldiniz, sıfır Güncel Bakiye, para birimi, İlk İşlemi Ekle kartı | E0026 | Kare |
| 06 | Wallet | İlk açılış görülmedi; koşumlar önceden açılmış hesapla yapıldı | E0014 K00 | Görülmedi · E2 |
| 07 | Wallet | Ana ekran Home › Accounts | E0014 K01; E0276 | Kare |
| 08 | Hesap Defterim | Kayıt yok; ilk açılışta "Hos geldin — Uygulama nasıl kullanılır?" diyaloğu; arkada hazır "Hesap Defterim" defteri | E0007 K00; E0135 | Kare |
| 09 | Hesap Defterim | Diyalog "Ücretli" ve "Alınan" düğmelerinden söz ediyor; ekrandaki düğmeler Ödendi ve Alındı | E0135; E0391 inceleme satırı | Kare |
| 10 | Goodbudget | Açılışta LOG IN ve CREATE NEW HOUSEHOLD | E0106 | Kare |
| 11 | Goodbudget | Yeni hanede önce bütçe kurulumu, sonra LATER seçenekli kayıt ekranı; atlama denenmedi | E0006 K00 | Koşum kaydı |
| 12 | Goodbudget | Kurulumdan sonra ana ekran ENVELOPES | E0006 K01; E0115 | Kare |

## 3.2 Ana gezinme

| No | Uygulama | İfade | Dayanak | Tür |
|---|---|---|---|---|
| 13 | Money Manager | İncelenen İşlemler, İstatistik, Hesaplar ve Daha/Ayarlar yüzeylerinde aynı dört alt sekme; İşlemler içinde Gün, Takvim, Ay, Toplam, Not | E0228, E0231, E0236, E0254 | Canlı koşum · kare; üç ek kare 16 Eylül görsel kontrolünde doğrulandı |
| 14 | Goodbudget | Üstte dört sabit sekme | E0115 | Kare |
| 15 | Bluecoins | Üstte yana kaydırılan sekmeler; son sekmenin adı ekrana sığmıyor | E0018, E0103 | Kare |
| 16 | Bluecoins | Sol menü kalemleri; farklı simgeli iki "Hesaplar"; hedefleri açılmadı | E0084 | Kare · E5 |
| 17 | Wallet | Sol menünün üst kısmındaki kalemler | E0275 (ad karartıldı) | Kare |
| 18 | Wallet | Sol menünün alt kısmındaki kalemler ve anahtarlar | E0376 | Kare |
| 19 | Wallet | Home içinde Accounts ve Budgets & Goals sekmeleri | E0276 | Kare |
| 20 | Hesap Defterim | Sol menü kalemleri, en üstte Reklamları kaldırmak | E0171 | Kare |
| 21 | Hesap Defterim | Başlıkta açılır defter seçici; farklı karelerde farklı defter seçili | E0137, E0136 | Kare |

## 3.3 Ekleme eylemi

| No | Uygulama | İfade | Dayanak | Tür |
|---|---|---|---|---|
| 22 | Money Manager | Sağ altta turuncu yuvarlak + | E0228 | Kare |
| 23 | Bluecoins | Sağ altta yuvarlak +; veri yokken İlk İşlemi Ekle kartı | E0103, E0026 | Kare |
| 24 | Wallet | Sağ altta mavi yuvarlak + | E0276 | Kare |
| 25 | Goodbudget | Sağ altta mavi yuvarlak + | E0115 | Kare |
| 26 | Hesap Defterim | + yok; altta yeşil Alındı ve kırmızı Ödendi düğmeleri | E0137 | Kare |
| 27 | Hesap Defterim | Hesaplar arası aktarım menüdeki Aktar kalemi | E0171 | Kare |
| 28 | Dört uygulama | Formda kayıt türü seçenekleri var; varsayılan tür ve seçim değiştirme gerekliliği doğrulanmadı; formlar Bölüm 5'te | E0229, E0074, E0277, E0116 | Kare (bu bölüme alınmadı) |

## 3.4 Ana ekranın bilgi sırası

| No | Uygulama | İfade | Dayanak | Tür |
|---|---|---|---|---|
| 29 | Money Manager | Üstte Gelir, Gider, Toplam; altında güne göre gruplu liste; gün başlığında gelir ve gider | E0228 | Kare |
| 30 | Money Manager | Hesap bakiyeleri ana ekranda yok, Hesaplar sekmesinde | E0228; E0010 arayüz taraması | Kare |
| 31 | Bluecoins | Hesaplar sekmesi kart panosu: Günlük Özet (grafik, 7 ve 30 gün ortalaması), reklam alanı, Bütçe Özeti | E0103 | Kare |
| 32 | Bluecoins | Ana Ekran ayarında kart anahtarları ve hesap yuvaları | E0025 | Kare (sonuç davranışı ölçülmedi) |
| 33 | Bluecoins | Bekleyen işler Hatırlatıcılar sekmesinde, tarih gruplu, "Dün bitti" ve "Bugün süresi doluyor" etiketleri | E0056 | Kare |
| 34 | Bluecoins | Veri varken uygulama açıldığında gelen sekme | E0005 K01 | Görülmedi · E7 |
| 35 | Wallet | Önce hesap kartları, hemen altında tanıtım kartları | E0276 | Kare |
| 36 | Wallet | Aşağıda Balance Trend, Upcoming planned payments (yükleniyor görünümü), Add more cards | E0274 | Kare · E8 |
| 37 | Hesap Defterim | Güne göre gruplu liste, satırda denge; altta sabit Toplam Alındı, Toplam Ödendi, Denge; grafik yok | E0137; E0007 bilgi hiyerarşisi | Kare |
| 38 | Hesap Defterim | Tekrarlayan veya planlı kayıt özelliği bulunamadı (beş yerde tarandı) | E0007 B1 satırı | Koşum kaydı |
| 39 | Hesap Defterim | Veri yokken üstte Google Drive yedekleme daveti ve reklam kaldırma şeridi, en altta reklam bandı | E0136 | Kare |
| 40 | Goodbudget | Son yedekleme ve Total; Monthly zarfları; Available grubu | E0115 | Kare |
| 41 | Goodbudget | Zarfta üst sayı kalan, alt sayı bütçelenen; ekranda etiket yok | E0006 K01 (anlam); E0115 (etiketsizlik) | Koşum kaydı + Kare · E6 |
| 42 | Goodbudget | Ana ekranda bekleyen işler alanı görülmedi | E0115 | Görülmedi |

## 3.5 Boş ana ekran

| No | Uygulama | İfade | Dayanak | Tür |
|---|---|---|---|---|
| 43 | Money Manager | Kayıtsız ayda çizim ve "Veri yok."; yönlendirme metni yok; kare ilk açılış değil | E0227; E0391 inceleme satırı | Kare |
| 44 | Bluecoins | Boş kartlarda "Bu dönemde hiçbir işlem yok." | E0018 | Kare |
| 45 | Bluecoins | Temiz başlangıçta İlk İşlemi Ekle kartı | E0026 | Kare |
| 46 | Hesap Defterim | Yalnız başlık satırı ve sıfır toplamlar; yönlendirme metni yok | E0136 | Kare |
| 47 | Wallet, Goodbudget | Boş ana ekran görülmedi | E0014 K00; E0006 K00 | Görülmedi · E3 |

## Kaynak ve erişim ifadeleri — 3.1–3.4 içine dağıtıldı

Bu satırlardaki "Kare" resmî kaynak karesidir (destek sayfası görseli veya tanıtım videosu karesi); canlı koşum değildir.

| No | Uygulama | İfade | Dayanak | Tür |
|---|---|---|---|---|
| 48 | Paraşüt | Giriş ekranı geçilemedi; mobilde yalnız giriş öncesi tanıtım ekranları görüldü | E0011 oturum bilgisi ve K00; P4-tema-01 | Koşum kaydı |
| 49 | Logo İşbaşı | Yalnız giriş/kayıt ekranı görüldü; ana ekran hiçbir kaynakta görülmedi | E0009; P4-tema-01 | Görülmedi |
| 50 | QuickBooks | Yalnız mobil tanıtım ve plan ekranları; ana ekran görülmedi | E0012; P4-tema-01 | Görülmedi |
| 51 | KolayBi | Uygulamaya girilmedi; destek sayfaları ve tanıtım videolarından incelendi | E0008 oturum bilgisi; P4-tema-01 | Koşum kaydı |
| 52 | KolayBi | Ana ekran adı Güncel Durum; 13 kalemli sol modül menüsü; Güncel Durum / Stok Akışı / Notlar sekmeleri; sağ üstte Hızlı İşlemler | E0211; E0008 pano bölümü | Kare (kaynak) |
| 53 | KolayBi | Pano blokları: iki çizgili nakit akışı grafiği, Tahsilat Ve Ödeme Özetleri, Günü Gelen İşlemler (Bugün / Yaklaşanlar / Tarihi Geçenler) | E0211, E0181 | Kare (kaynak) |
| 54 | KolayBi | Video karesinde özetler üçer halka: Vadesi Gelmemiş, Vadesi Geçmiş, Vadesi Belirsiz; Faturalar, Genel Giderler, Çalışan Maaşları sayaçları | E0181; E0008 d33 satırı | Kare (kaynak) |
| 55 | KolayBi | Destek karesinde "Size Özel Ayrıcalıklar" kutucukları ve "KolayBi' Yolu Var!"; eski video karesinde takvim; yeni video karesinde menüde Fatura Ödeme; güncel sürüm doğrulanmadı | E0211, E0181, E0187; E0391 inceleme satırları | Kare (kaynak) |
| 56 | KolayBi | Destek ve yeni video karelerinde "Mobil Uygulamaya Giriş Yap" kartı; mobil uygulamanın içi görülmedi | E0211, E0187 | Kare (kaynak) + Görülmedi |
| 57 | Paraşüt | Güncel Durum: Tahsilatlar (TAHSİL EDİLECEK, GECİKMİŞ, FATURA YOK) ve Ödemeler (ÖDENECEK, ÖDEME YOK, PLANLANMIŞ) halkaları; "BUGÜN - 22 EYLÜL", "4 GÜN GECİKTİ" | E0262 | Kare (kaynak) |
| 58 | Paraşüt | Kare tanıtım çizimi: menü alanları boş, tahsil edilecek ve gecikmiş aynı tutar; gerçek ekran olduğu doğrulanmadı | E0262; E0391 inceleme satırı | Kare (kaynak) |

## v2 karşılaştırma ve özet denetimi

- 3.1: iddia 01–12, 48–51. Goodbudget yalnız izlenen yol; kurulum zorunluluğu iddia edilmez.
- 3.2: iddia 13–21, 52, 55, 58. Money Manager dayanakları genişletildi; bütün alt ekranlara genellenmedi.
- 3.3: iddia 22–28, 52, 58. Hızlı İşlemler görünür; kapalı menü seçenekleri bilinmiyor. Tür seçeneği ek dokunuş zorunluluğu değildir.
- 3.4: iddia 29–42, 53–58. Bluecoins kart ayarının sonuç davranışı ölçülmedi; Wallet kartı yükleniyor. Goodbudget tutar anlamı kayıt kaynaklı.
- 3.5: iddia 43–47, 49–51. Boş dönem, temiz başlangıç ve demo sıfırları birleştirilmez.
- Bölüm özeti yukarıdaki karşılaştırmaların sentezidir; yeni davranış iddiası eklemez. Kazandırdığı/Bedeli kaldırıldı; ölçülmüş kullanıcı etkisi ileri sürülmedi.
- QuickBooks ayrımı: E0012 ve ortak içindekilerin Ortak kapsam ve kaynaklar kaydı. Solopreneur kaynakları ile QBO Simple Start giriş kareleri ayrı tutulur.
- Kanıt türü ifade başınadır: canlı koşum karesi/kaydı; kaynak destek görseli/video/temsili çizim. Görülmedi, kanıt bulunmama durumudur.
