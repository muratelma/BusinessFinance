# Vergi teması — karar belgesi

**Durum: aşıldı (28 Eylül 2026).** Bu belge mevcut özellikleri iyileştirmeye çalışan "ön muhasebe"
bakışıyla yazıldı; kullanıcı yaklaşımı reddetti. Kullanıcı kararları, aynı gün:
1. **Muhasebeci paketi ve KDV alanları kaldırılır** (ADR 0016'nın ilgili kısmı ve PRD §2/§6.9 değişir).
2. **Vergi = nakit planı:** vergi takvimi kendi ekranı olur; vergiler orada tanımlanır, ödemesi onaylanır ve
   tutar onay anında girilir. Hazır vergi türleri kategori gibi seçilir.
3. Muhasebeciden belge alma (tahakkuk fişi) şimdilik yok → fikir kaydı. Kasa gün sonu raporunu kamerayla
   okuma ise yapılacak.
Vergi durumu sorusu (V1) geri çekildi. Güncel analiz: `research/vergi/YENI-YAKLASIM.md`.

## 1 · Tek cümle
Vergi özellikleri bugün **herkese aynı** davranıyor; oysa hedef kullanıcı ikiye bölünmüş: **basit usul**
esnafının KDV'si, geçici vergisi, gelir vergisi yok; 2026'da gerçek usule geçen bakkal, lokanta ve benzerleri
ise her ay KDV beyan ediyor. En büyük kazanç tek bir soruyla gelir: **"Vergi durumunuz ne?"**

## 2 · Özellik özellik

S1 ihtiyaç · S2 en sade · S3 doldurulur mu · S4 değer · S5 kapsam · S6 uyum · S7 sonuç.

| Özellik | Değerlendirme (S1–S7 özeti) | Kova |
|---|---|---|
| **Vergi durumu sorusu** (yeni): Basit usul · KDV mükellefi · Bilmiyorum / muhasebecim biliyor | S1 bugün herkes aynı alanları görüyor (V-U1, V-U3, V-U4). S2 tek soru, onboarding'deki "İşletmem var" sorusunun yanına; sonradan Hesabım'dan değişir. S3 bir kez. S4 çok düşük maliyet, bütün vergi ekranlarını sadeleştirir. S5 kapsamda: vergi hesaplamaz, yalnız neyin gösterileceğini seçer. S6 "İşletmem var" ile aynı desen: **hiçbir özelliği kapatmaz**, yalnız görünürlüğü ayarlar. S7 eski kullanıcılar "Bilmiyorum" başlar = bugünkü davranış | **Ekle** |
| **KDV alanları** (5 kayıt türü) | S1 KDV mükellefine var, basit usule yok. S3 bugün çoğu kayıtta boş (Ağustos: 9'dan 6'sı). S6 kasa gün sonunda sorulmuyor (F10). S7 basit usulde gizlenir, veri silinmez | **Sadeleştir:** basit usulde gizli, KDV mükellefinde bugünkü gibi |
| **İndirilebilirlik** | S1 yalnız gelir vergisi olan için anlamlı; basit usulde anlamsız. S3 Ağustos'ta 3 gider cevapsız. S4 muhasebeci zaten karar veriyor | **Sadeleştir:** basit usulde gizli. KDV mükellefinde kalıp kalmayacağı §4 soru 3 |
| **Muhasebeci paketi** | S1 muhasebeci Z, e-fatura, e-arşiv'i GİB'den alıyor; **alamadığı** kâğıt fiş, kasadan harcama ve banka hareketinin açıklaması. S5 kapsamda. S6 kasa gün sonu ve fiş okuma pakete kendiliğinden girer | **Değiştir:** odak **belgeler ve dökümler**; KDV bölümü yalnız KDV mükellefinde ve "N kayıtta KDV yok" uyarısıyla öne; basit usulde KDV bölümü yerine **aylık alış ve satış toplamı** (Defter-Beyan'a giden iki sayı) |
| **Vergi takvimi** | S1 öneriler rejime bakmıyor (V-U4); ekran bir öneri listesi, durum göstermiyor (V-U5); kurulu kalem ikinci kez kurulabiliyor. S2 takvim = **vergi kalemlerinin yaklaşanlar görünümü**: kurulu kalemler, sıradaki tarih, ödendi/ödenmedi; kurulmamış öneriler altta. S6 veri yine tekrarlayan plan; yeni kayıt türü yok | **Değiştir** |
| **Vergi planının tutarı** | Kullanıcı, 28 Eyl: "tutarlarının gözükmemesi iyi, her farklı tutar olabilir". V-U7: Planlama'da sabit tahmin gerçekmiş gibi duruyor | **Değiştir:** vergi planında tutar **isteğe bağlı**; boşsa "tutar ödeme günü girilir" yazar, yaklaşanlar toplamında "tutarsız N kalem" diye ayrı söylenir |
| **Takvim önerileri** | Rejime göre: basit usul → Bağkur (+ çalışanı varsa muhtasar); KDV mükellefi → KDV, geçici vergi, Bağkur, (muhtasar); tarihler bugünkü gibi öneri, kullanıcınındır | **Değiştir** |
| **Vergi ödemelerinin gider sayılması** | KDV ödemesi, kayıtlar KDV dahil tutulduğu sürece işletme netini doğru bırakıyor. Geçici/gelir vergisi ve Bağkur ise işletme netini "vergiden sonra"ya çeviriyor | **Sana soru** (§4 soru 1) |
| **Vergi karşılığı** (hedef) | Kullanımı doğru; takvimle bağlantısı yok | **Tut**; ileride takvim kalemine "karşılık ayır" kısayolu (fikir kaydı) |
| **Hatalar** | V-U6 tür ve etiket çakışması, V-U8 "quarterly", V-U9 yanıltıcı "Sonraki" | **Düzelt** |

## 3 · Açık kararlar

| # | Karar | Seçenekler | Önerim |
|---|---|---|---|
| **V1** | Vergi durumu sorusu | A · ekle (onboarding + Hesabım) · B · ekleme, herkes her şeyi görsün | **A.** Tek soru, bütün vergi ekranlarını doğru kullanıcıya göre sadeleştirir |
| **V2** | "Bilmiyorum" diyenin göreceği | A · bugünkü gibi her şey · B · basit usul gibi sade | **A.** Yanlışlıkla gizlemek, gerçekten KDV mükellefi olanın verisini eksik bırakır |
| **V3** | Takvimin yeri | A · ayrı ekran kalır ama durum gösterir · B · Planlama'nın "Yaklaşanlar"ında vergi filtresi olur, ayrı ekran kalkar | **A.** Esnafın "vergilerim" diye aradığı tek bir yer; veri yine tekrarlayan plan |
| **V4** | Muhasebeci paketinin odağı | A · bugünkü gibi (toplamlar + KDV) · B · belgeler ve dökümler öne, KDV yalnız KDV mükellefinde, basit usulde alış/satış toplamı | **B** |

## 4 · Sana sorular (ADR ile çatışan ya da ürün kararı gerektiren)

1. **Vergi ve Bağkur ödemesi işletme netini düşürsün mü?** Bugün düşürüyor; "işletme neti" = vergiler
   ödendikten sonra kalan. Alternatif: gelir vergisi, geçici vergi ve Bağkur **şahsi** ya da ayrı bir
   "vergi" çizgisi olarak gösterilir, işletme neti vergiden önceki para olur. KDV ödemesi her iki durumda
   işletme gideri kalır (net sonucu doğru bıraktığı için). *Önerim:* gelir vergisi ve geçici vergi işletme
   netini düşürmesin, Özet'te ayrı satır olsun; Bağkur senin kararın (şahıs şirketinde sahibin kendi sigortası).
2. **"Bu ay ödenecek KDV" tahmini gösterilsin mi?** Girilen KDV'lerden (satıştaki − alıştaki) bir tahmin
   çıkarılabilir; esnafın nakit planı için değerli bir soru. **ADR 0016 ile çatışır:** "uygulama hiçbir
   vergi tutarını hesaplamaz veya türetmez". Risk: kayıtların çoğunda KDV boş olduğu için sayı eksik çıkar,
   esnaf yanlış tutara göre para ayırır. *Önerim:* gösterme; yerine "girilen KDV'ler: satış X, alış Y;
   N kayıtta KDV yok" (bugün paket bunu zaten yapıyor). Karar senin.
3. **İndirilebilirlik KDV mükellefinde kalsın mı?** ADR 0016 bu alanı kurdu; kullanım düşük (Ağustos'ta 3
   gider cevapsız) ve muhasebeci zaten karar veriyor. *Önerim:* kalsın ama formda kapalı başlasın,
   yalnız pakette "cevapsız" sayısı görünsün. Kaldırmak da meşru bir seçenek.

## 5 · Bütünlük (kullanıcının 1. ilkesi)
- Vergi durumu sorusu "İşletmem var" sorusuyla aynı yerde ve aynı kuralla çalışır: hiçbir özelliği
  kapatmaz, yalnız görünürlüğü ayarlar; cevap sonradan değişirse veri kaybolmaz.
- Takvim yeni bir kayıt türü açmaz; tekrarlayan plan ve planlanan görünüm aynen kalır (para akışı
  haritası B8).
- Kasa gün sonunda KDV sorulmaz (F10); KDV mükellefi için Z raporu okuması (F04) geldiğinde kendiliğinden dolar.
- POS komisyonundaki BSMV KDV değildir; KDV alanına girmez (kasa bulguları).

## 6 · Kapsam dışı (işletme bütçesi sınırı)
Beyanname üretmek · ödenecek vergiyi hesaplamak (soru 2 açık) · basit usul had takibi · Defter-Beyan veya
GİB entegrasyonu · e-fatura düzenlemek · serbest meslek makbuzu (ayrıca incelenmedi).
