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
- **Boşluk koşumu ≠ Tur 2.** Boşluk koşumu = formun kendi eksik listesinden,
  orta derinlik. Tur 2 = seçilen uygulamalarda tam derinlik (D1–D4 olayları,
  pipeline şeması, ayrıntılı fotoğraf).
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

### Faz 1 — Boşluk koşumu: Money Manager  *(yapay zekâ sürer)*
- [ ] Mevcut veriyi temizle; çekirdek işlemleri **Ağustos 2026** tarihleriyle
      yeniden gir (3/5/8/12/18 Ağu) + **5. olay** kart ödemesi ₺1.200
      (Tur 1'de atlanmıştı). Kontrol: son Ana Hesap ₺40.800, net ₺44.950
- [ ] Kart "Pay" akışı → Settlement vs Payment takvimi
- [ ] "Balance Payable" vs "Outstanding Balance" ekranı
- [ ] Rep/Inst (tekrar/taksit) form alanı
- [ ] Bütçe kurulumu (Total → Budget Setting)
- [ ] Excel export (Total sekmesi), yedekleme menüsü
- [ ] Eksik kareler: kart takvimi, bütçe, export
- [ ] `gozlemler/money-manager.md` + kontrol değerleri güncelle

### Faz 2 — Boşluk koşumu: Wallet
- [ ] K00 onboarding (temiz bakış)
- [ ] Planned payments oluştur
- [ ] Debts oluştur
- [ ] Goals oluştur
- [ ] Budgets oluştur (Create Budget)
- [ ] Split transaction
- [ ] Export
- [ ] Eksik kareler → `gozlemler/wallet-budgetbakers.md` güncelle

### Faz 3 — Boşluk koşumu: Bluecoins
- [ ] Planlı işlem oluştur
- [ ] Taksit şartları (kart giderinde)
- [ ] Cari hesap türü → tahsilat bağı
- [ ] Hatırlatıcı oluştur
- [ ] Yedek / geri yükleme
- [ ] CSV/PDF/HTML export yüzeyi (Premium sınırına kadar)
- [ ] Eksik kareler → `gozlemler/bluecoins.md` güncelle

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
