# ADR 0009 — Borcun kaynağı ve anüite faiz modeli

- Durum: Kabul edildi (17 Ağustos 2026, Aşama 12.8)
- Bağlam: Aşama 12.8 belgesi — önceki repoda (`Kisisel-Butce-Mobil`), bu repoya taşınmadı
- İlgili: ADR 0002 (transfer raporlaması), ADR 0005 (tekrarlayan plan kaynağı)

## Bağlam

Borç ve alacak parayı hesaptan çıkarıyor, hiçbir gelir/gider raporuna
girmiyordu. 300 TL borç üç taksitte ödendiğinde bakiye 300 TL azalıyor, aylık
gider raporu sıfır diyordu ve aradaki fark hiçbir yerde açıklanmıyordu.

Kodda doğrulandı: `DebtInstallment.MarkPaid` yalnız `PaymentAccountId` yazıyor,
hiçbir yazma modeli üretmiyordu; bakiye etkisi yedi ayrı repository'de elle
yazılmış LINQ ile hesaplanıyordu. Borç *açılışı* ise hiçbir kayıt
üretmiyordu — `DebtAgreement` yalnız taksit takvimi üretiyordu.

`AnnualInterestRate` saklanıyor ama hiçbir hesaba girmiyordu. Kullanıcı
anapara, toplam ve faizi birbirinden bağımsız girebiliyor, üçü çelişse bile
kabul ediliyordu.

## Karar

### 1. Eksik kayıt ödeme değil, açılıştır

Borç doğduğunda ya para hesaba girmiştir ya da bir şey tüketilmiştir. İkisi de
kaydedilmiyordu; bu yüzden defter kapanmıyordu.

`DebtAgreement` bir `DebtSourceType` taşır, tam olarak biri dolu
(`RecurringSourceType` deseninin aynısı: Domain invariant'ı **ve** SQL check
constraint'i):

| Kaynak | Açılışta | Her taksitte |
|---|---|---|
| `Cash` | hesap ± anapara, gelir/gider yok | hesap ∓ taksit, gider yok |
| `Expense` | **gider + anapara**, kategori zorunlu, bakiye etkisi yok | hesap − taksit, gider yok |

Ödeme semantiği her iki kaynakta da aynıdır: **yalnız bakiye hareketi.** Kredi
kartı kuralının aynısı — `CreditCardCharge` gideri ve kategoriyi taşır,
`CreditCardPayment` taşımaz. Taksit ödemesi gider yazsaydı aynı tüketim iki
kez sayılırdı.

### 2. Alacak tek kaynaklıdır

"Birinin yerine bir gideri biz ödedik" durumu mevcut araçlarla zaten
anlatılıyor: kendi payınızı gider olarak, karşı tarafın payını nakit alacak
olarak girersiniz. Alacağa `Expense` kaynağı eklemek ya gideri iki kez
saydırır ya da negatif gider (contra-expense) kavramı gerektirir. Somut ihtiyaç
doğmadan ikisi de eklenmez.

### 3. Faiz modeli anüitedir

Aylık oran = yıllık / 12 (nominal; Türk bankaları böyle ilan eder). Taksit =
`P·i / (1 − (1+i)^−n)`.

Basit faiz reddedildi: ödenmiş anaparaya da faiz işletir — gerçek kredilerde
olmayan bir şey — ve bir taksiti anapara ile faize bölemez. O bölme, faizin
gider olarak yazılmasının önkoşuludur. Fark ölçüldü: 300 TL, %10, 3 taksitte
basit faiz 307,50 der, anüite 305,0138.

### 4. Para kanoniktir, oran türetilmiştir

Oran kanonik olsaydı yuvarlama kullanıcının yazdığı toplamı değiştirirdi:
400,00 girip 399,99 görmek kabul edilemez. Bu yüzden anapara ile toplam
kanoniktir, taksit planı toplamdan bölünür, oran paradan çözülür.

İstek toplamı **veya** oranı gönderir, diğerini cevapta alır. İkisi birden
gelip çelişirse `debt.repayment_conflict` ile reddedilir; sessizce birini
seçmek kullanıcının yazdığından başka bir borç kaydetmek olurdu.

Ters çevirimin (toplam → oran) kapalı formülü yoktur; ikiye bölme kullanılır.
`(1+i)^n` doğrudan hesaplanamaz: %1000/360 ay ucunda 10^94 civarına çıkar ve
`decimal` taşar. Bunun yerine `1/(1+i)` çarpanı tekrarlanır; değer daima
(0,1] aralığında kalır.

### 5. Taksit başına anapara/faiz ayrımı saklanır

Bu, aşama planındaki ilk kararın **düzeltilmesidir.** Başlangıçta "saklanmaz,
hesaplanır" denmişti. Aylık gider raporu ödenen taksitlerin faiz kısmını
SQL'de toplamak zorunda; anüite ayrımı sıra numarasına bağlı bir üs alma
işlemi ve EF LINQ'e çevrilmiyor. Bellekte toplamak reponun bounded-query
kuralını çiğnerdi.

Saklama bir projeksiyon önbelleği değildir: ayrım, taksitin sözleşme anında
sabitlenen gerçek bir özelliğidir ve bankalar da ödeme planına basar. Aynı
gerekçeyle taksit takvimi zaten saklanıyor.

### 6. Açılışı kayıtsız borçlar

Bu ayrımdan önce açılmış kayıtların açılışında ne olduğu kayıtlı değil ve
kurtarılamıyor. Üç seçenek vardı:

1. Hepsini `Cash` saymak — o tarihte olmamış bir para girişi uydurur ve
   sözleşme tarihinden bugüne bütün bakiyeleri değiştirir.
2. Dokunmamak — kalıcı ve görünmez bir tutarsızlık bırakır.
3. **Seçilen:** üçüncü bir durum, `DebtSourceType.Unrecorded`.

Üçüncüsü ne geçmiş uydurur ne de eksiği gizler: uygulama sorar, gerçeği bilen
kullanıcı `DebtAgreement.RecordOpening` ile tamamlar. Yeni borç bu durumda
açılamaz — kurucu reddeder, API `unrecorded` değerini kabul etmez. Yalnız
migration ve şema 4 öncesi yedekler üretir.

Migration'ın kolon varsayılanı `0`'dır; mevcut satırlar check constraint'i
kendiliğinden sağlar ve ayrı bir backfill ifadesi gerekmez.

## Sonuçlar

- Bakiye ile gider raporu artık birbirini tutuyor.
- Kategori bağı gider kaynaklı borçlarda kendiliğinden kuruluyor; Özet
  ekranındaki kategori dağılımı grafiğinin önündeki engel kalktı.
- Yedek şeması 4'e çıktı; 2 ve 3 okunmaya devam ediyor ve borçları
  `Unrecorded` olarak yüklüyor.
- Yedekteki `AnnualInterestRate` artık geri yüklerken okunmuyor: oran paradan
  çözülüyor ve yedekteki değer hiçbir hesaba girmemiş serbest bir sayıydı.
- Anapara/toplam oranı %1000 yıllık sınırın üstünde bir orana denk gelirse
  borç açılamıyor. Bu yeni bir kısıt: eskiden oran hiçbir şey yapmadığı için
  böyle bir sözleşme sessizce kabul ediliyordu.
