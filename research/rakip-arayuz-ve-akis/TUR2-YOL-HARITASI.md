# Tur 2 Yol Haritası ve Boşluk Koşumu Planı

> Bu dosya, 10 Eyl 2026'da kullanıcıyla kararlaştırılan çalışma planının kalıcı
> kaydıdır. Oturum değişince buradan devam edilir. Canlı ilerleme `DURUM.md`
> tablolarındadır; bu dosya **planı** tutar, ilerlemeyi değil.

## Karar özeti (10 Eyl 2026)

- **Tur 1 video notları tamam:** Logo (2 Eyl), Paraşüt + KolayBi (10 Eyl).
  QuickBooks'ta video yok. 7 gözlem formu Tur 1 derinliğinde.
- **Sorun:** Sürülebilir 3 uygulamanın (Money Manager, Wallet, Bluecoins)
  formlarında işaretli Tur 2 boşlukları var; Tur 2 seçimini kör yapmamak için
  önce bu boşluklar **orta derinlikte** doldurulacak.
- **Boşluk koşumu ≠ Tur 2.** Boşluk koşumu = formun kendi eksik listesi +
  **kredi kartı, tekrarlayan işlem (B1), taksit planı (B2)** her uygulamada
  canlı kurulur; orta derinlik. Tur 2 = seçilen uygulamalarda tam derinlik
  (D1–D4 olayları, pipeline şeması, ayrıntılı fotoğraf).
- **Kapsam dışı (boşluk koşumu):** export, yedek/geri yükleme, bütçe kurulum
  ekranı — seçim-kritik değil.
- **Belge planı:** Belge 1 (arayüz) ve Belge 2 (akış) **tüm** uygulamaları
  genel anlatır; Tur 2 uygulamaları daha ayrıntılı (daha çok veri). Belge 3
  (öneri) **tüm** uygulamalar için. Karar filtresi yalnız Belge 3'te.
- **Yeni uygulama:** Goodbudget (dijital zarf bütçe — "harcamadan önce dağıt";
  bizim "kaydet sonra raporla" modelimizin tersi; elle giriş, banka bağlama
  yok → bizimle aynı giriş modeli, tek fark felsefe). Yedek: YNAB / Spendee.
- **Emülatör:** Bir uygulama, sırayla (paralel AVD yok). Yapay zekâ adb ile
  sürer; kayıt/SMS/hata kapılarında kullanıcıya devreder, kullanıcı bitirince
  devam eder. `emulator-5554`, Android 17.
- **Tur 2 nihai yapısı:** KolayBi (masa başı) + {Money Manager | Wallet |
  Bluecoins}'ten 1 + Goodbudget (elle bakıştan geçerse). Faz 5'te kesinleşir.

## Fazlar

### Faz 0 — Plan + commit  *(bu dosya + DURUM + protokol + tek `docs(research)` commit)*

### Faz 1 — Boşluk koşumu: Money Manager  *(yapay zekâ sürdü, 10 Eyl 2026)*
- [x] Uygulama yeni emülatörde temiz + **Türkçe** çıktı; 3 hesap Türkçe
      adlarla kuruldu (Ana Hesap ₺20.000, Ortak Cuzdan ₺2.000, Is Karti ₺0)
- [x] 5 çekirdek işlem **Ağustos 2026** tarihleriyle girildi (kart ödemesi
      dâhil). Kontrol değerleri birebir: Ana Hesap ₺40.800, net ₺44.950,
      Ağustos gelir ₺25.000 / gider ₺2.050
- [x] Kart ekstre modeli: "Bu Ay 01/08~31/08" / "Gelecek Ay" = Balance Payable
      vs Outstanding; Kaynak/Hesap Kesim/Son Ödeme alanları
- [x] "Ödeme" butonu = ön doldurulmuş Havale (kart ödemesi nötr)
- [x] Tekrar/Taksit → Tekrarlama (14 seçenek) + Taksit iki ayrı akış
- [x] Açılış bakiyesi "Bakiye Farkı" davranışı; sıfır tutar sessiz kabul;
      silme tek onaylı kalıcı; taslak uyarısı yok
- [x] 15 Türkçe kare (`kanitlar/money-manager/02`–`16`), eski İngilizce silindi
- [x] `gozlemler/money-manager.md` güncellendi (yeni "Faz 1 boşluk koşumu" bölümü)
- [x] B1 tekrarlayan (₺600/ay) canlı: tek onayla kaydediliyor; geçmiş/bugünkü
      tekrar **otomatik** gerçekleşiyor (onaysız), gelecek ayrı "önizleme"
      bölümünde sayılmıyor. Kareler `17`–`20`
- [x] B2 taksit (₺6.000 / 6) canlı: "(1/6)" başlık, ay ay ₺1.000 ekstreye
      bölünüyor, tam tutar bir kerede borç yazılmıyor. Kareler `21`–`23`
- [x] Kısmi kart ödemesi: "Ödeme" tutarı düzenlenebilir, kısmi yalnız kesilmiş
      ekstreye uygulanıyor. Kareler `24`–`25`
- [x] Fiş kamerası = ek dosya, OCR yok
- **MM Faz 1 TAMAM.** Kareler `02`–`25`. Not: B1/B2 sonrası veri saptı; temiz
      ₺44.950 durumu `11` karesinde.
- ~~bütçe kurulumu / export / yedek~~ — kapsam dışı (kullanıcı, 10 Eyl)

### Faz 2 — Boşluk koşumu: Wallet  *(yapay zekâ sürdü, 10 Eyl 2026 — TAMAM)*
- [x] Kullanıcı yeni emülatörde giriş yaptı; bulut verisi (Tur 1 çekirdek 5 + 3 hesap) geri geldi
- [x] Çekirdek doğrulama: 5 işlem doğru Ağustos tarihli, kontrol değerleri birebir (net ₺22.950)
- [x] Kredi kartı modeli: **basit negatif bakiye** — ekstre kesim/dönem yok, tek son ödeme tarihi (hatırlatıcı), limit + "Available Credit/Balance" gösterim seçeneği. Kart ödemesi ayrı buton değil, Ana Hesap→Kart transferi → **kısmi ödeme** = küçük transfer
- [x] B1 tekrarlayan (₺600/ay) canlı: Planned payments → Recurrent. **Geçmiş tarihe kurulamıyor** (10 Ağu reddedildi → 10 Eyl). Tanım para üretmez; her örnek **bekleyen** + Confirm/Postpone/Dismiss; Confirm'de tutar düzenlenebilir "Payment summary". İlk onayda **plan bazında** "otomatik mi / onaylı mı" sorusu → "onaylı" seçildi. Yalnız sonraki örnek listelenir. Kareler `15`–`20`, `33`
- [x] B2 taksit: **Wallet'ta taksit özelliği YOK.** ₺6.000 tek kart harcaması → tüm tutar aynı gün borç + tüm tutar o ayın gideri. Kareler `21`–`24`
- [x] Budgets **canlı kuruldu**: period + amount + kategori **+ hesap** filtresi; kart harcamasının tam tutarını sayar; Forecasted Spend + over-budget toast. Kareler `26`–`29`
- [x] Goals **canlı kuruldu**: hedef/biriken/tarih. Kareler `30`–`32`
- [x] Debts **canlı oluşturuldu** (10 Eyl ek koşum): I Lent "Ada Reklam" ₺5.000. Kaydederken **"Record oluştur → bakiye değişir" seçeneği** (No/Yes) — bakiyeye dokunmak borç bazında opsiyon. "Yes" → Ana Hesap'a "Loan, interests" gideri −₺5.000. Borç detayı = açık Total + bağlı Records. Kareler `34`–`36`, `41`–`44`
- [x] Split transaction: kaydı alt-kayda oyma akışı görüldü (`38`, `39`)
- [x] Fiş / kamera: **OCR yok** — yalnız dosya/foto eki (`40`)
- [x] Tekrarlayan plan yönetimi (10 Eyl): otomatik/onaylı kolu dişli ile her zaman değişir (`45`); plan silme düz onay, **gerçekleşmiş occurrence uyarısı yok** (`46`, `47`)
- [x] `gozlemler/wallet-budgetbakers.md` güncellendi (Faz 2 bölümü + 17 karar satırı); kareler `10`–`47`
- **Wallet Faz 2 TAMAM.** Veri: Ana Hesap ₺15.200 · Ortak Cuzdan ₺2.150 · İş Kartı −₺6.000 · net ₺11.350. Tur 2'ye seçilirse bulut sıfırlanır.
- ~~export / onboarding~~ kapsam dışı

### Faz 3 — Boşluk koşumu: Bluecoins  *(yapay zekâ sürdü, 10 Eyl 2026 — TAMAM)*
- [x] Bulut yok → tam yeniden koşum. 3 hesap (Ana Hesap ₺20.000 / Ortak Cuzdan ₺2.000 / Is Karti ₺0)
- [x] Çekirdek 5 işlem Ağustos tarihli; kontrol birebir (Net Kazanç ₺44.950, İş Kartı ₺0, Ana Hesap ₺40.800)
- [x] Kredi kartı = negatif bakiyeli hesap (Cari Hesap grubu); kart seçilince "Taksit şartlarını seçin" dinamik alan
- [x] **B2 taksit canlı:** oran (%) + ay sayısı (2–24 + Özel) + ilk ödeme tarihi. ₺6.000 → 6×₺1.000. **Taksit 1/6 anında gerçek harcama** (ilk-ödeme tarihinde, "1/6" etiket, kart borcu −₺1.000, Ağustos gideri +₺1.000). **2/6–6/6 aylık hatırlatıcı.** BusinessFinance InstallmentPlan'a çok yakın. Kareler `17`–`22`
- [x] **B1 tekrarlayan canlı:** "Planlı İşlemler" → Bir Defa/Günlük/Haftalık/Aylık/Yıllık + ayın günü + son ödeme + otomatik checkbox. **Geçmiş tarihe kurulabiliyor.** Tanım hiçbir şey üretmez. Geçmiş/bugün/gelecek occurrence'lar Hatırlatıcılar'da bekliyor ("31 gün gecikmeli"/"bugün süresi doluyor"). Kaydet → "bugün mü / planlanan tarih mi" → materyalize. **Tüm rakiplerin BusinessFinance'e en yakını.** Kareler `23`–`28`
- [x] Hatırlatıcılar sekmesi = tekrarlayan + taksit occurrence'larının birleşik tarih-sıralı listesi
- [x] **Cari hesap** (10 Eyl ek koşum): "Cari hesap" tipi canlı kuruldu ("Ada Reklam cari") — **sıradan bakiye hesabı, fatura nesnesi/tahsilat bağı YOK**. Kareler `32`, `33`
- [x] **Bağımsız hatırlatıcı** (10 Eyl): "Bir Defa" Planlı İşlem = "Ofis kirasi" ₺10.000 → Hatırlatıcılar'da "Yarın borçlanacak". Kareler `30`, `31`
- [x] Ek bulgular (10 Eyl): Bölmek/split modu (`29`), taslak uyarısı, Kredi Kartı **ekstre kesim günü** alanı (`32`), kısmi kart ödemesi canlı ₺500 (`34`)
- [x] `gozlemler/bluecoins.md` güncellendi (Faz 3 bölümü + 15 karar satırı); kareler `10`–`34`
- **Bluecoins Faz 3 TAMAM.** Veri: Ana Hesap ₺39.700 · Ortak Cuzdan ₺4.150 · İş Kartı −₺500 · Ada Reklam cari ₺0 · net ₺43.350. Emülatörde kararsız (modal diyaloglar dokunuşu işlemiyor → force-stop / keyevent).
- ~~yedek / CSV-PDF export~~ kapsam dışı

### Faz 4 — Yeni uygulama: Goodbudget
- [ ] **Kullanıcı:** Play Store'dan indir → e-posta ile hesap → 10 dk gezinti
      → beğeni kararı
- [ ] Geçerse: K00–K08 + arayüz taraması + sistem işleyişi (Tur 1 protokolü)
- [ ] Geçmezse: YNAB veya Spendee'ye geç, fazı tekrarla
- [ ] Yeni `gozlemler/goodbudget.md`; `MANUEL-TEST-PROTOKOLU.md` + `DURUM.md`
      tablosuna ekle

### Faz 5 — Tur 2 seçimi
- [ ] 4 uygulama eşit derinlikte kıyaslanır
- [ ] Tur 2'nin 3'ü kesinleşir → `DURUM.md` "Tur 2" bölümü yazılır

### Faz 6 — KolayBi masa başı derinleştirme  *(Faz 1–5 boyunca paralel)*
- [ ] **Kullanıcı:** "Kullanım Rehberi Bölüm 1/2" videoları → transkript + kareler
- [ ] **Yapay zekâ:** forma işle (proje ekranı, gider formu, cari ekstre)

### Faz 7 — Tur 2 derin koşum
- [ ] Seçilen 3 uygulamada D1–D4 olayları + protokol 1–5 konuları
- [ ] Detaylı fotoğraflama + pipeline şemaları
- [ ] Formların "Tur 2" bölümleri

### Faz 8 — Belge yazımı
- [ ] Belge 1 (arayüz) + Belge 2 (akış) taslak → onay → Belge 3 (öneri)

## Commit ritmi

Her fazın sonunda gerçek ilerleme varsa `docs(research)` commit. Hafıza kuralı
gereği **her commit öncesi kullanıcıya sorulur** ([[ask-before-docs-commits]]).
Video transkript yöntemi: [[rakip-video-transkript-yontemi]].
