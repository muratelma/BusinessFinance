# Ortak Sentetik Test Verisi

## Kurgu kişi ve işletme

- Kullanıcı adı: **Deniz Kaya**
- İşletme adı: **Deniz Tasarım**
- İş türü: Tek kişilik tasarım ve danışmanlık işletmesi
- Para birimi: TRY
- İncelenecek dönem: **Ağustos 2026**
- Müşteri: **Ada Reklam**
- Tedarikçi: **Mavi Yazılım**

Bu isimlerin tamamı kurgudur. Bir hizmet geçerli telefon, adres, VKN veya TCKN
isterse gerçek bilgi girilmez ve uydurma resmî kimlik üretilmez; kayıt engeli
not edilir.

## Başlangıç hesapları

| Hesap | Tür | Başlangıç değeri |
|---|---|---:|
| Ana Hesap | Banka/nakit hesabı | ₺20.000,00 |
| Ortak Cüzdan | Nakit | ₺2.000,00 |
| İş Kartı | Kredi kartı | ₺0,00 borç |

Uygulama başlangıç bakiyesi kabul etmiyorsa hesaplar sıfır açılır; bu durum
forma yazılır. Banka bağlama seçeneği kullanılmaz, hesaplar elle oluşturulur.

## Tur 1 çekirdek işlemleri

| # | Tarih | Olay | Tutar | Kaynak | Kapsam/not |
|---|---|---|---:|---|---|
| 1 | 3 Ağustos 2026 | Web tasarım hizmeti geliri | ₺25.000,00 | Ana Hesap | İşletme; müşteri Ada Reklam |
| 2 | 5 Ağustos 2026 | Market alışverişi | ₺850,00 | Ortak Cüzdan | Şahsi |
| 3 | 8 Ağustos 2026 | Tasarım yazılımı gideri | ₺1.200,00 | İş Kartı | İşletme; tedarikçi Mavi Yazılım |
| 4 | 12 Ağustos 2026 | Hesaplar arası aktarım | ₺3.000,00 | Ana Hesap → Ortak Cüzdan | Gelir veya gider değildir |
| 5 | 18 Ağustos 2026 | Kart borcu ödemesi | ₺1.200,00 | Ana Hesap → İş Kartı | İkinci kez gider değildir |

Tur 1'de bir uygulama kart ödemesini desteklemiyorsa 5 numara atlanır. İşletme
ve şahsi kapsam alanı yoksa kategoriyle yapay bir çözüm kurulmaz; bu özellik
`Desteklenmiyor` olarak kaydedilir.

## Kontrol değerleri

Uygulamanın hesabını doğru ilan etmek için değil, veri girişinde hata yapıp
yapmadığımızı anlamak için kullanılır:

- İşletme gelir eksi gider: **₺23.800,00**
- Şahsi gelir eksi gider: **−₺850,00**
- Transfer ve kart ödemesi yeniden gelir/gider sayılmamalıdır
- Son Ana Hesap bakiyesi: **₺40.800,00**
- Son Ortak Cüzdan bakiyesi: **₺4.150,00**
- Son kart borcu: **₺0,00**
- Toplam net varlık: **₺44.950,00**

Rakip farklı bir finansal model kullanıyorsa yalnız fark kaydedilir. Bu kontrol
değerleri rakibi BusinessFinance kurallarına uymaya zorlamak için kullanılmaz.

## Tur 2 ek olayları

Yalnız seçilen üç uygulamada ve özellik mevcutsa kullanılır:

| # | Olay | Tutar | Koşul |
|---|---|---:|---|
| D1 | 5 Eylül vadeli ofis kirası borcu | ₺10.000,00 | Planlanan/yükümlülük desteği varsa |
| D2 | Ada Reklam'a kesilmiş hizmet faturası | ₺12.000,00 | Taslak/demo fatura desteği varsa |
| D3 | D2 için kısmi tahsilat | ₺5.000,00 | Fatura ile tahsilat bağlanabiliyorsa |
| D4 | Aylık yazılım aboneliği | ₺600,00 | Tekrarlayan işlem desteği varsa |

Gerçek e-fatura/e-arşiv gönderilmez, vergi hesabı yapılmaz ve banka bağlantısı
kurulmaz. Yalnız taslak, demo veya yerel kayıt akışı incelenir.

