# Borç, alacak ve cari — araştırma (16 Kasım 2026)

Bu belge kararların **gerekçesidir**. Karar `02-TASARIM.md`, iş sırası
`03-PLAN.md` içindedir. Kullanıcının isteğiyle inceleme repodaki önceki
kararlardan bağımsız yapıldı; önceki kararlarla ayrılan yerler tasarım
belgesinde açıkça yazılıdır.

## 1. Soru

Şahıs şirketi sahibi ve esnaf için bir bütçe uygulamasında "kime ne borcum var,
kimden ne alacağım var" işi nasıl kurulmalı ki:

- kullanıcı muhasebe bilmeden doğru kaydı yazsın,
- aynı para iki kez sayılmasın,
- ekranlar az ve tahmin edilebilir olsun,
- ön muhasebeye kaymasın?

## 2. Bugünkü durum (kod ve belgelerden)

**Sunucuda iki yazma modeli var:**

| | Cari defter | Borç (`Debt`) |
|---|---|---|
| Kayıt | `CounterpartyCharge` (alım/satış; gelir ya da gider yazar) | Borç: kaynak, plan, kalemler |
| Ödeme | `CounterpartyPayment`: kişiye yazılır, belirli alıma bağlanmaz | `DebtPayment`: tek borca bağlı, kalemlere dağılır |
| Bakiye | `Σ alım − Σ ödeme`; iki yön ayrı durur (alacak, borç) | Borcun kendi kalanı |
| Fazla ödeme | Kabul edilir, karşı yönde bakiye olur | Reddedilir |
| Gecikme | "Ödeme en eski alımı kapatır" varsayımıyla **okunurken** hesaplanır | Kalemin vadesinden |
| Kapsam | Her zaman İşletme | Kategoriden / seçimden |
| Kendi kartımızla ödeme | Yok | Var |
| Borç para (gelir/gider yazmayan) | Yok | Var |
| Taksit, faiz, plan değişikliği | Yok | Var |

**İstemcide** ekranlar eski: `Cari hesap` listesi ve kişi sayfası, eski
`Borç ve alacaklar` sayfası, `Yükümlülükler`. Sunucudaki yeni borç uçlarının
(kısmi ödeme, indirim, plan, `debt-book`) ekranı yok.

**Önceki tasarım turları (14–29 Ekim):** iki model kullanıcıya iki ayrı şey
olarak gösterildi. Sonuçları:

- Kayıt girerken `Nasıl takip edilsin?` sorusu: `Cari hesaba yaz` /
  `Ayrı borç olarak takip et`.
- Kişi sayfasında `Cari hesap` satırı ile ayrı kayıtlar yan yana; tahsilatta
  `Hangi alacağa kaydedeceksiniz?` sorusu.
- Dört liste: Borçlarım, Alacaklarım, Kişiler, ayrıca Cari hesap listesi; kişi
  sayfasının içinde ayrı bir cari sayfası.
- Kişiler'de beş `Göster` seçeneği, üç sıralama, gecikme anahtarı.
- 29 Ekim'de "her cari alım ayrı borç olsun, ödeme dağıtılsın" modeli denendi
  (dağıtım önerisi, onay özeti, fazla tutarı sonraki kayda uygulama) ve geri
  dönüldü.

Teşhis: sorun tek tek ekranlarda değil. **Kullanıcıya bir muhasebe yöntemi
sorusu soruluyor** ("bu kayıt açık hesapta mı yürüsün, tek başına mı kapansın")
ve bu sorunun cevabı her ekrana sızıyor. Turların uzaması bunun belirtisi.

## 3. Dışarıda bu iş nasıl çözülmüş

### 3.1 İki yöntem

Alacak takibinde iki yerleşik yöntem var:

| | Yürüyen bakiye (balance forward) | Açık kalem (open item) |
|---|---|---|
| Ödeme | Kişinin toplam bakiyesini düşürür; genelde en eski kayıttan | Belirli faturalara bağlanır |
| Fazla ödeme | Alacak bakiyesi olarak devreder | Bağlanmamış ödeme / avans kavramı gerekir |
| Gecikme | Toplu görünüm | Fatura fatura |
| Uygun olduğu yer | Süren, tekrarlayan ilişki | Tek tek onaylanan, itiraz edilen faturalar |
| Zayıf yanı | Seçerek ödeme ve itiraz zor | Kayıt yükü; ödeme eşleştirme işi |

Kaynak: Stripe'ın yöntem karşılaştırması; satıcı belgeleri aynı ayrımı yapıyor.

### 3.2 Veresiye / khata uygulamaları (hedef kitlemize en yakın)

Khatabook ve OkCredit (Hindistan, milyonlarca esnaf) ile Türkiye'deki veresiye
defteri uygulamaları aynı kalıbı kullanıyor:

- **Birim kişidir.** Liste kişilerden oluşur; satırda kişinin bakiyesi yazar.
- Kişinin sayfasında iki düğme: `Verdim` / `Aldım`. Tutar yazılır, kaydedilir.
- Ödeme hiçbir kayda bağlanmaz; yürüyen bakiye.
- En çok öne çıkarılan özellikler: hatırlatma (WhatsApp/SMS), ekstre paylaşımı
  (PDF), vade günü, gecikenleri süzme.
- Türkiye'deki küçük uygulamalar da aynı: kişi ekle (rehberden), borç ekle,
  bakiye, vadesi yaklaşan bildirimi, ekstreyi WhatsApp'tan paylaş.

Bu uygulamaların özellik listeleri kendi tanıtım metinlerinden; bağımsız
doğrulama değil. Kalıbın kendisi (kişi, iki düğme, yürüyen bakiye) hepsinde
ortak.

### 3.3 Ön muhasebe ürünleri (fikir için; ölçüt değil)

- **Paraşüt:** tahsilat faturadan ya da müşteriden eklenir. Faturadan eklenen
  tahsilat fatura tutarını aşamaz. Müşteriden eklenen tahsilat sınırsızdır ve
  açık faturalarla **kendiliğinden, en çok gecikenden başlayarak** eşleşir.
- **QuickBooks:** `Receive payment`; ödeme açık faturalara en eskiden yeniye
  kendiliğinden uygulanır, fazlası müşteri alacağı olur. Topluluk forumlarında
  en sık şikâyet tam da bu eşleştirme: yanlış faturaya uygulanan ödeme,
  istenmeyen alacak, "uygulanmamış ödeme".
- **Vyapar / myBillBook:** `Payment in` / `Payment out`; faturaya bağlanmayan
  ödeme avans sayılır; faturaya bağlı ödeme önce bağ çözülmeden silinemez.

Çıkarım: fatura eşleştirmeyi yapan ürünler bile kullanıcıya dağıtım
sordurmuyor, en eskiden başlayıp kendisi uyguluyor; eşleştirmenin kendisi ise
bu ürünlerin en çok sorun çıkaran yeri.

### 3.4 Kişisel finans uygulamaları

- **Wallet (BudgetBakers):** `Borç verdim` / `Borç aldım`; her geri ödeme ayrı
  kaydedilir; verilen borç "bağışlanabilir". Belge açıkça söylüyor: bu özellik
  basit kişisel borçlar içindir, kredi ve mortgage için ayrı hesap türü
  kullanılır.
- **Money Lover:** borç, alacak, geri ödeme, tahsilat ayrı kategoriler; borca
  dokunup `Öde` ile tutar yazılır.
- Kullanıcı yorumlarında tekrar eden şikâyetler (az sayıda yorum, anekdot
  düzeyinde): kısmi ödemenin çalışmaması ya da kafa karıştırması, kaydın
  düzeltilememesi (silip yeniden yazma), yedek olmaması.

Çıkarım: basit kişisel borç ile planlı kredi bu ürünlerde de **ayrı iki şey**.

### 3.5 Apple

- **Apple Card:** taksitli alışverişler (`Monthly Installments`) kartın yürüyen
  bakiyesinden ayrı bir sayfada durur: ödenen, kalan, sıradaki taksit ve günü,
  ödeme geçmişi. Olağan ödeme bakiyeye gider; taksitleri erken ödemek ayrı bir
  eylemdir (`Pay Early`) ve fazla tutar **en eski plana** uygulanır.
- **Tasarım yönergeleri (HIG):**
  - Bilgiyi sistemden al, kullanıcıya sorma; mantıklı ön değerle doldur.
  - Yazdırmak yerine seçtir.
  - Değeri yazılırken doğrula; zorunlu alan dolmadan ilerletme.
  - Modal görev kısa ve tek amaçlı olsun; "uygulamanın içinde uygulama" gibi
    hissettiren çok adımlı modal kurma; modalın üstüne modal açma.
  - Modalın başlığı görevi söylesin; kapatma yolu belli olsun.
  - Ayrıntıya satırdan inilir (liste → ayrıntı); özet önce, ayrıntı sonra.

Çıkarım: Apple da yürüyen bakiye ile planlı taksidi tek kavramda eritmiyor;
ayırıyor ve aralarındaki kuralı kendisi koyuyor.

## 4. Bulgular

**B1 — Sektörde iki şey gerçekten ayrı:** kişiyle süren açık hesap (yürüyen
bakiye) ve kendi takvimi olan planlı borç (kredi, taksit). Veresiye
uygulamaları yalnız birincisini, kredi takipçileri yalnız ikincisini, Apple
Card ve Wallet ikisini ayrı ayrı sunuyor. İkisini tek modelde eriten ürünler
(ön muhasebe) eşleştirme yükünü üstleniyor.

**B2 — Hedef kullanıcımızın işi yürüyen bakiyedir.** Esnafın veresiyesi ve
toptancıyla hesabı fatura fatura değil, "hesaptan düş" diye yürür. Fatura
eşleştirme, itiraz ve fatura bazlı yaşlandırma ön muhasebe işidir ve ürün
sınırımızın dışındadır.

**B3 — Hangi yöntemin kullanılacağı kullanıcıya sorulmaz.** İncelenen hiçbir
üründe "bu kaydı açık hesapta mı, ayrı mı takip edeyim" sorusu yok. Ayrımı ürün
yapar; kural basittir (Apple Card: taksitliyse ayrı).

**B4 — Birim kişidir.** Kişinin sayfası her şeyin toplandığı yerdir: bakiye,
iki eylem, hareketler. Listeler oraya götürür.

**B5 — Ödeme hedef sormaz.** Kişiye yapılan ödeme kişinin bakiyesini düşürür.
Planlı borcun taksidi kendi sayfasından ödenir.

**B6 — Fazla ödeme kaybolmaz.** Yürüyen bakiyede fazla ödeme kendiliğinden
karşı yönde bakiye olur; ayrı bir "avans" kavramı gerekmez. Bugünkü cari
modelimiz bunu zaten yapıyor.

**B7 — Kullanıcının istediği yan özellikler:** vade ve geciken görünümü,
hatırlatma, ekstre paylaşımı, kaydı düzeltebilme. Dağıtım, mahsup, yaşlandırma
raporu istenenler arasında değil.

**B8 — Bugünkü sunucu modelimiz bu ayrıma zaten yakın.** Cari defter yürüyen
bakiyedir; `Debt` planlı borçtur. Eksik olan modelin kendisi değil:

1. Hangi kaydın nereye gideceği kullanıcıya soruluyor.
2. Cari defter yalnız işletmeye ve yalnız alım/satıma açık; "arkadaşa borç
   verdim" oraya yazılamıyor ve planı olmayan basit borçlar ağır borç formuna
   düşüyor.
3. Cariye kendi kartımızla ödeme yazılamıyor.
4. Ekranlar iki modeli iki ayrı dünya gibi gösteriyor.

## 5. Değerlendirilen üç yol

| Yol | Ne | Artı | Eksi |
|---|---|---|---|
| A — Bugünkü ayrım, kullanıcı seçer | 29 Ekim son-hal panoları | Sunucu hazır | Teşhisteki sorunun kendisi; soru ve iki hedef her ekrana sızıyor |
| B — Her şey açık kalem | Her alım ayrı borç, ödeme dağıtılır | Tek model | Dağıtım kaydı, geri alma sırası, bağlanmamış ödeme; ön muhasebe yükü; QuickBooks'un en sorunlu yeri; 29 Ekim'de denendi ve bırakıldı |
| C — Kişinin hesabı + planlı borç, **kuralı ürün koyar** | Kişiyle taksitsiz her şey kişinin hesabına; taksitli ya da kişisiz olan ayrı kayıt | Kullanıcıya soru yok; tek ödeme düğmesi; sunucunun büyük kısmı hazır; khata ve Apple Card kalıbı | Seçerek ödeme yok (ödeme en eski kaydı kapatmış sayılır); cari defter genişlemeli |

Seçilen: **C**. Gerekçe ve ayrıntı `02-TASARIM.md`.

## 6. Kaynaklar

- [Stripe — Balance forward billing](https://stripe.com/resources/more/balance-forward-billing)
- [Khatabook — uygulama özellikleri](https://khatabook.com/blog/khatabook-app-features)
- [OkCredit — hatırlatma](https://okcredit.in/payment-reminder-app), [OkCredit](https://okcredit.in/en/)
- [Paraşüt — veresiye takibi](https://www.parasut.com/blog/veresiye-programi-parasut)
- [Paraşüt — mobilde tahsilat/ödeme ekleme](https://www.parasut.com/kullanim-kilavuzu/mobilde-tahsilat-odeme-ekleme)
- [Paraşüt — cari hesap bakiyesi takibi](https://www.parasut.com/kullanim-kilavuzu/cari-hesap-bakiyesi-takibi)
- [QuickBooks topluluğu — birden çok faturaya ödeme](https://quickbooks.intuit.com/learn-support/en-us/reports-and-accounting/i-received-a-payment-for-multiple-invoices-in-the-payment-a/00/1056012)
- [myBillBook — Payment In / Payment Out](https://knowledge.mybillbook.in/articles/5969819-how-to-record-payments-on-mybillbook-payment-in-and-payment)
- [BudgetBakers — Debts](https://support.budgetbakers.com/hc/en-us/articles/7149520322706) (sayfa doğrudan okunamadı; arama özetinden)
- [Apple — Apple Card Monthly Installments](https://support.apple.com/en-us/HT210644)
- [Apple HIG — Entering data](https://developer.apple.com/design/human-interface-guidelines/entering-data)
- [Apple HIG — Modality](https://developer.apple.com/design/human-interface-guidelines/modality)
- [Apple HIG — Sheets](https://developer.apple.com/design/human-interface-guidelines/sheets)
- [Apple HIG — Lists and tables](https://developer.apple.com/design/human-interface-guidelines/lists-and-tables)
- [App Store — Dijital Veresiye Defteri](https://apps.apple.com/us/app/id6463430967)

**Araştırmanın sınırı:** rakip uygulamalar bu turda cihazda denenmedi; bilgiler
yardım sayfaları, mağaza açıklamaları ve arama özetlerinden. Wallet, Money
Manager ve Bluecoins'in cihaz gözlemleri `research/rakip-arayuz-ve-akis/`
altında duruyor; borç ekranları orada ayrıca incelenmedi.
