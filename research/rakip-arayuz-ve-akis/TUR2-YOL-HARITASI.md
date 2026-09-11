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
- **"Bize benzemeyen" 3. slot adayları (10 Eyl) ve Faz 5 kararı (11 Eyl):**
  Goodbudget ve Hesap Defterim'in ikisi de tam Tur 1 + A/B/B1/B2 ek koşumuyla
  test edildi (Faz 4a/4b) ve ikisinde de **"Fatura/borç → tahsilat/ödeme →
  kapanış akışı: Yok"** sonucu net biçimde doğrulandı — yani Tur 2'nin asıl
  amacı olan D1–D4 (yükümlülük/fatura/kısmi tahsilat/tekrarlayan) derinliğini
  bu ikisinde koşturmak bilinen "yok" sonucunu tekrarlamaktan öteye geçmezdi.
  Kullanıcı kararıyla **3. slot Wallet'a kaydırıldı**: Wallet'ın Faz 2'de
  canlı kurulan **Debt özelliği** (I Lent/I Borrowed + bağlı Records + açık
  Total) D1 (yükümlülük) ve D3'e (kısmi tahsilat) yapısal olarak en yakın
  mekanizma ve fatura-benzeri (D2) bir senaryoyla henüz hiç zorlanmadı — gerçek
  keşif potansiyeli taşıyan tek "yakın" aday. Goodbudget ve Hesap Defterim
  **Tur 1 derinliğinde kalır**, Belge 1/2/3'e o derinlikle girer.
  - Yedek (Wallet'ta beklenmedik bir engel çıkarsa): Money Manager — ama D1-D4
    için elinde somut bir ipucu yok (form notu: "Fatura→tahsilat MM'de yok,
    Tur 2'de D1–D4" — tamamen keşfedilmemiş alan).
- **Emülatör:** Bir uygulama, sırayla (paralel AVD yok). Yapay zekâ adb ile
  sürer; kayıt/SMS/hata kapılarında kullanıcıya devreder, kullanıcı bitirince
  devam eder. `emulator-5554`, Android 17.
- **Tur 2 nihai yapısı (Faz 5'te kesinleşti, 11 Eyl):** KolayBi (masa başı,
  video+transkript) + **Bluecoins** ("yapımıza en yakın" — tekrarlayan/taksit/
  planlanan modeli bizimle birebir, kilitlendi) + **Wallet** (Debt/Records
  mekanizmasıyla D1-D4'ü genişletmek için en uygun aday, "bize yakın" 2. slot).

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
- **Wallet Faz 2 TAMAM.** Veri: Ana Hesap ₺15.200 · Ortak Cuzdan ₺2.150 · İş Kartı −₺6.000 · net ₺11.350. B/B1/B2 canlı koşuldu, **A (₺400 kısmi kart ödemesi) koşulmadı** — Faz 7'de tamamlanmalı. Wallet Tur 2'ye seçildi (11 Eyl); bulut verisi sıfırlanmaz, üzerine eklenir.
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

### Faz 4 — "Bize benzemeyen" 3. slot: Hesap Defterim + Goodbudget tam Tur 1 koşumu
**Revize (10 Eyl, kullanıcı isteği):** elle gezinti adımı atlandı. Yapay zekâ
ikisinin de **tam Tur 1 koşumunu** yapar (K00–K08 + arayüz taraması + sistem
işleyişi), tıpkı Money Manager/Bluecoins gibi. İkisi de emülatörde yüklü
(`cashbook.cashbook`, `com.dayspringtech.envelopes`).
- [x] **Faz 4a — Hesap Defterim (yapay zekâ, 10 Eyl):** tam Tur 1 koşumu +
      A/B/B1/B2 ek koşumu bitti. `gozlemler/hesap-defterim.md`, kareler `00`–`19`.
      Bulgu: khatabook türü tek-sütunlu yürüyen bakiye defteri; hesap türü/
      kategori/kapsam/kart-borcu/transfer-ayrımı **yok**; **B1/B2 (tekrarlayan,
      taksit) özelliği de yok**; transfer bacakları öksüz kalabiliyor; ağırlıkla
      negatif referans. A (kısmi kart ödemesi) ✓ jenerik Aktar'la çalışıyor.
      Alınabilir kenarlar: "Önceki denge" devir satırı, satır başına yürüyen
      Denge, soft-delete çöp kutusu. Kontrol değerleri tuttu (net 44.950), test
      kayıtları silindi.
- [x] **Faz 4a ek koşum 2 (yapay zekâ, 11 Eyl, kullanıcı isteği):** kalan
      form boşlukları + uygulamaya özgü özellikler canlı test edildi — Öğe
      eklemek (kalem dökümü tutar+not otomatik dolduruyor), İşlem adları/Özel
      (global yeniden adlandırma, export'a da yansıyor), zorunlu alan (boş
      tutar sessiz red, ₺0 uyarısız kabul), arama (canlı filtre + alt
      toplamlar da filtreleniyor), kalıcı silme (iki aşamalı: soft-delete +
      çöp kutusunda ayrı onaylı kalıcı silme), Bildiri/export (gerçek PDF
      doğrulandı, ama uyarı metnindeki klasör adı gerçek kayıt yeriyle
      tutarsız), Not Defteri + Nakit Hesap Makinesi (khatabook ailesi,
      muhasebeyle bağı yok). Kareler `20`–`35`. **Metodoloji düzeltmesi:** ek
      koşum verisi ilk aşamada sorulmadan silinmişti; kullanıcı fark edip
      düzeltilmesini istedi, iki test kaydı yeniden oluşturulup MM/Wallet/
      Bluecoins konvansiyonuna uyacak şekilde **silinmeden bırakıldı**
      (Ana Hesap ₺43.150, genel net ₺47.300). `gozlemler/hesap-defterim.md`
      tamamen güncellendi.
- [x] **Faz 4b — Goodbudget (yapay zekâ, 11 Eyl):** household kaydı (gerçek
      e-posta) kullanıcıya devredildi; gerisi K00–K08 + A/B/B1/B2 ek koşumu
      yapay zekâ tarafından yapıldı. `gozlemler/goodbudget.md`, kareler `01`–`22`.
      Bulgu: Envelopes (bütçe) ve Accounts (gerçek hesap) **iki ayrı katman**,
      Accounts varsayılan kapalı ve ücretsiz sürümde **toplam 1 hesap sınırı**
      (tüm türler ortak) — **K05 (kart gideri), K06 (transfer) ve A (kısmi kart
      ödemesi) bu yüzden canlı test edilemedi**. Gelir türü "Credit", zarf
      seçimi gelirde de zorunlu (Available'a doğrudan yatırma yok) — bu
      zorunluluk K07'de **"Spending by Envelope" raporunu bozdu** (Ağustos
      Total Spending -22.950,00, gerçek ₺850 harcama görünmüyor; "Income vs
      Spending" Income 0,00/Spending -22.950,00 — gerçeğin tam tersi). B1
      tekrarlayan karma model (ilk örnek anında gerçek kayıt, gelecek örnekler
      geçmişte görünmüyor); B2 taksit yok; B fiş/kamera hiç yok (5 uygulama
      arasında tek istisna). K08'de düzenlenip silinen kaydın bakiyeye etkisi
      tam geri alınmadı (kalıcı ₺16 sapma). Ağırlıkla negatif referans +
      gelir/gider ayrımı gerekçesine güçlü destek.
- [x] **Faz 5'te (11 Eyl) kıyaslandı → 3. slot seçilmedi, Wallet'a kaydırıldı.**
      Gerekçe: ikisi de D1-D4'ün sorduğu "fatura/borç/kısmi tahsilat" alanında
      zaten "Yok" diye kesinleşmişti (bkz. yukarı "Karar özeti"). İkisi de
      Tur 1 derinliğinde kalıyor, seçilmedi.
- [x] `MANUEL-TEST-PROTOKOLU.md` + `DURUM.md` tabloları Hesap Defterim için güncellendi

### Faz 5 — Tur 2 seçimi  *(TAMAM, 11 Eyl 2026)*
- [x] Bluecoins'i "yapımıza en yakın" slota kilitlendi
- [x] Faz 4'ten çıkan iki aday (Hesap Defterim, Goodbudget) D1-D4 için zaten
      "Yok" sonucu verdiğinden 3. slota konmadı; **Wallet** (Debt/Records
      mekanizması D1-D3'e yapısal olarak en yakın, henüz zorlanmamış) 3. slota
      seçildi — kullanıcı kararı
- [x] Tur 2'nin 3'ü kesinleşti: **Bluecoins + KolayBi + Wallet** —
      `DURUM.md` "Tur 2" bölümü + "Kilit bulgular" güncellendi

### Faz 6 — KolayBi masa başı derinleştirme  *(Faz 1–5 boyunca paralel)*
- [ ] **Kullanıcı:** "Kullanım Rehberi Bölüm 1/2" videoları → transkript + kareler
- [ ] **Yapay zekâ:** forma işle (proje ekranı, gider formu, cari ekstre)

### Faz 7 — Tur 2 derin koşum
- [x] **Bluecoins (11 Eyl 2026, yapay zekâ):** D1 tamamlama (hatırlatıcı →
      gerçek işlem) + D2 (fatura = sıradan Gelir, invoice nesnesi yok) + D3
      (kısmi tahsilat = sıradan Transfer, D2'ye sistemsel bağı yok) + arama/
      filtre/dışa aktarma (konu 5) + tam arayüz taraması (konu 6: Nakit Akım
      Ayarı, Kategori/Hesap Ayarları varsayılanları, çoklu para birimi,
      Kategoriler/Etiketler/Çöp Kutusu/Takvim). D4 atlandı (B1 ile birebir
      aynı senaryo). `gozlemler/bluecoins.md` "Faz 7" bölümü, kareler
      `f7-00`–`f7-54` (`kanitlar/bluecoins/`). **Bluecoins TAMAM.**
- [x] **Wallet (11 Eyl 2026, yapay zekâ):** A tamamlama (₺400 kısmi kart
      ödemesi, jenerik transfer) + D1 (planlı ödeme, geçmiş tarih tarih
      seçiciden engellendi, Confirm ile realize) + **D2 (I Lent borç,
      "Record oluşturursan bakiyen değişir" sorusu, No → bakiye değişmedi —
      ADR 0014'ün en güçlü olumlu kanıtı)** + **D3 (aynı Debt'e Record ile
      kısmi tahsilat, running balance otomatik güncellendi — Bluecoins'in
      bağımsız iki hareketinden daha doğru)** + konu 5 (arama) + konu 6 (tam
      arayüz taraması: Filters, Automatic rules/transfer tanıma, Currencies,
      Advanced settings/Initial day of the month). `gozlemler/wallet-
      budgetbakers.md` "Faz 7" bölümü, kareler `f7-00`–`f7-58`
      (`kanitlar/wallet/`). **Wallet TAMAM.**
- [ ] KolayBi — Faz 6 (video+transkript) bitince masa başı derinleştirme
- [ ] Formların "Tur 2" bölümleri tamamlanınca `DURUM.md` güncellenir

### Faz 8 — Belge yazımı
- [ ] Belge 1 (arayüz) + Belge 2 (akış) taslak → onay → Belge 3 (öneri)

## Commit ritmi

Her fazın sonunda gerçek ilerleme varsa `docs(research)` commit. Hafıza kuralı
gereği **her commit öncesi kullanıcıya sorulur** ([[ask-before-docs-commits]]).
Video transkript yöntemi: [[rakip-video-transkript-yontemi]].
