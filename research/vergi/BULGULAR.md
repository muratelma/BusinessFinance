# Vergi teması — bulgular

**Tarih:** 28 Eylül 2026. Masa başı araştırma + kod okuması + emülatör (kareler `kareler/01`–`05`).
Kanıt düzeyi: **R** resmî · **M** muhasebe yayını / ürün blogu · **K** kod · **E** emülatör.

## 1 · Sektör: şahıs şirketi hangi vergilerle yaşıyor

### 1.1 Üç rejim, üç farklı kullanıcı

| Rejim | Kim | KDV | Gelir vergisi | Defter | Kaynak |
|---|---|---|---|---|---|
| **Basit usul** | Küçük esnaf; 2026 sınırları: alış 1,2 M TL, satış 1,9 M TL, hizmet 600 bin TL, büyükşehirde kira 99 bin TL | **Yok** (teslim ve hizmetleri istisna) | **Yok** (kazanç istisnası, yıllık beyan yok) | Defter-Beyan sistemi, muhasebeci/oda | M [1], [2] |
| **İşletme hesabı** (2. sınıf) | Satış 3,5 M TL, alış 2,5 M TL altı | **Var**, aylık KDV beyannamesi | **Var**: 3 ayda bir geçici vergi + Mart'ta yıllık | İşletme defteri (hasılat-gider) | M [3] |
| **Bilanço** (1. sınıf) | Sınırı aşanlar | Var | Var | Çift taraflı | M [3] |

Serbest meslek (serbest meslek makbuzu) bu turda incelenmedi.

### 1.2 2026 değişikliği: hedef kullanıcının bir kısmı KDV mükellefi oldu
10380 (9 Eylül 2025) ve 10679 (11 Aralık 2025) sayılı Cumhurbaşkanı kararlarıyla 1 Ocak 2026'dan beri
büyükşehirlerde, nüfusu 30 bini aşan ilçelerde bazı faaliyetler basit usulden çıkarılıp gerçek usule
geçti: mal üretimi ve alım-satımı (bakkal dahil), inşaat, motorlu taşıt bakım-onarımı, lokanta ve eğlence
yerleri, şehir içi yolcu taşımacılığı. Bir kaynak kuaför, berber, terzi ve tesisatçıyı da sayıyor; kesin
liste kararların metninden doğrulanmalı. Geçenler ilk KDV beyannamesini Şubat 2026'da verdi. M [2], [4]

**Sonuç:** hedef kullanıcımız KDV açısından ikiye bölünmüş durumda. Biri için KDV alanları hiç anlamlı
değil, diğeri için her ay beyan edilen bir yük.

### 1.3 Gerçek takvim
- KDV beyannamesi: izleyen ayın **28**'i (ödeme aynı gün). M [5]
- Muhtasar ve prim hizmet: her ayın **26**'sı beyan, ay sonuna kadar ödeme — **yalnız çalışanı olan ya da
  stopajlı ödeme yapanlar** için. M [5]
- Geçici vergi: 3 ayda bir, **yalnız gerçek usul**. M [3]
- Yıllık gelir vergisi: Mart, **yalnız gerçek usul**. M [3]
- Bağkur (4/b): ay sonu, **herkes** için. M [5]

### 1.4 Muhasebeci esnaftan ne alıyor
- Fatura, Z raporu, perakende satış fişi, gider pusulası, makbuzlar ve banka ekstresi; en geç izleyen ayın
  10'una kadar. M [6]
- Z raporu, e-fatura ve e-arşiv **GİB'e elektronik olarak zaten gidiyor**; 3.000 TL üstü satışta işletme
  hesabı ve basit usul mükellefi de GİB portalından e-arşiv düzenliyor (tam dijitalleşme 2027'ye ertelendi). M [6], [7]
- **Muhasebecinin elektronik olarak alamadığı şey:** kâğıt gider belgeleri (fiş, gider pusulası), kasadan
  yapılan harcamaların listesi, banka hareketlerinin açıklaması.

### 1.5 KDV'nin mantığı
Ödenecek KDV = satışlardaki KDV (hesaplanan) − alışlardaki KDV (indirilecek); negatifse devreder. Her
giderin KDV'si indirilemez (mevzuat sınırları). M [8]

## 2 · Uygulamamız (koddan ve emülatörden)

| # | Bulgu | Kanıt |
|---|---|---|
| V-U1 | **KDV alanları rejime bakmıyor.** Basit usul esnafına da her formda "Vergi bilgisi (isteğe bağlı)" gösteriliyor | E (gelir formu, kasa testi kare 08) |
| V-U2 | **Paketteki KDV = kullanıcının girdiği KDV'lerin toplamı**; gelir/giderden hesaplanmıyor. Ağustos'ta 9 kaydın 6'sında KDV yok; özet gerçek KDV'nin bir parçası | K `AccountantPackageUseCases`, E kare 05 |
| V-U3 | Paket basit usul kullanıcısına da KDV satırlarını gösteriyor | E kare 05 |
| V-U4 | **Vergi takvimi herkese aynı dört kalemi öneriyor** (KDV, muhtasar, SGK/Bağkur, geçici vergi). Basit usulde üçü yok; çalışanı olmayanda muhtasar yok | K `TaxCalendarSuggestions`, E kare 01 |
| V-U5 | **Takvim bir durum ekranı değil.** Kurulu kalemi, sıradaki tarihi, ödenip ödenmediğini göstermiyor; kaleme dokununca Planlama'ya geçip **yeni plan formu** açıyor, kurulu kalemin ikincisi kurulabilir | E kare 01, 02 |
| V-U6 | Vergi planında tür "Fatura / abonelik", kaynak boş geliyor; "Bitiş tarihi" etiketi üst üste binmiş | E kare 02 |
| V-U7 | Planlama'da vergi planları **sabit tahmini tutarla** görünüyor (KDV 6.250, geçici vergi 8.500); kullanıcı bunu gerçek tutar sanar | E kare 04 |
| V-U8 | Geçici vergi satırında **"quarterly"** yazıyor (çevrilmemiş) | E kare 04 |
| V-U9 | Planlama'da "Sonraki 28 Kasım" yazan KDV, Özet'te "vadesi 28 Eylül, bugün" görünüyor; "sonraki" etiketi yanıltıcı | E kare 04, kasa T7 |
| V-U10 | Dört takvim kalemi de "SGK ve vergi ödemesi" kategorisinde **işletme gideri** olarak kuruluyor. KDV için bu, kayıtlar KDV dahil tutulduğu sürece işletme netini doğru bırakıyor (satıştaki KDV gelire, alıştaki KDV gidere dahil; ödeme ikisinin farkını düşüyor). **Gelir vergisi / geçici vergi ve Bağkur** için ise bir tercih: işletme neti vergiden sonraki para oluyor | K `TaxCalendarSuggestions` |
| V-U11 | Z raporundaki KDV uygulamaya girmiyor (gün sonu henüz yok); POS komisyonundaki BSMV KDV değildir ve KDV alanına girmemeli | Kasa bulguları |

## Kaynaklar
1. Basit usul 2026 hadleri — https://www.parasut.com/blog/basit-usulde-vergilendirme , https://birfatura.com/basit-usul-hadleri-basit-usul-kira-siniri/ (M)
2. 2026 gerçek usule geçiş — https://www.parasut.com/blog/basit-usulde-vergilendirme , https://www.aydinserhat.com.tr/news/10380-cumhurbaskani-karari-basit-usulden-gercek-usule-gecis , https://lebibyalkin.com.tr/makale/basit-usule-tabi-bazi-mukelleflerin-gercek-usulde-vergilendirilmesine-yonelik-yapilan-duzenlemenin-getirdikleri (M; karar metinleriyle doğrulanmalı)
3. İşletme hesabı esası — https://devredin.com/sozluk/isletme-hesabi-esasi , https://www.defterbeyan.gov.tr/tr/yardim/isletme-hesabi-esasi (M/R)
4. Geçişte ilk KDV beyanı — https://www.interpay.com.tr/blogdetay/basit-usulden-gercek-usule-gecis-2026-ocakta-isletmeleri-neler-bekliyor (M)
5. Vergi takvimi — https://gib.gov.tr/vergi-takvimi , https://www.muhasebetr.com/vergi-takvimi/ (R/M)
6. Muhasebeciye iletilen belgeler — https://alikaraahmet.com.tr/index.php?id=dosya%2Fmukellefin_gorevleri.php , https://nesbilgi.com.tr/okc-z-raporu-mevzuati/ (M)
7. e-Fatura / e-Arşiv 2026 — https://www.parasut.com/blog/e-fatura-ve-e-arsiv-zorunlulugu , https://faturaport.com/blog/e-fatura/e-arsiv-fatura-limitleri-2026-kim-hangi-tutarda-nasil-kesmek-zorunda (M)
8. KDV hesabı ve devreden KDV — https://faturaport.com/blog/vergi/kdv-beyannamesi-nedir-alis-kdv-satis-kdv-devreden-kdv-ve-gider-kdv-nasil-hesaplanir (M)
