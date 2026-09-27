# Bölüm 3 — işaretli ekran anlatımı

16 Eylül 2026 · Üçüncü bağımsız deneme · v2, inceleme düzeltmeleri uygulandı.

- Okuma: [bolum-03-isaretli-ekran.pdf](bolum-03-isaretli-ekran.pdf) — 12 sayfa, A4 **yatay**.
- Okunabilir metin kopyası: [bolum-03-isaretli-ekran.md](bolum-03-isaretli-ekran.md).
- Denetim: [iddia-tablosu.md](iddia-tablosu.md), [eksik-listesi.md](eksik-listesi.md).
- Kanıt: [kaynaklar.md](kaynaklar.md), `kanit-manifest.json`, `kanit/` (özgün kopyalar),
  `isaretli/` (işaretlenmiş türevler).

Diğer iki denemeden (`bolum-03-gezinme-ve-ana-ekran`, `bolum-03-bagimsiz`) ayrı bir klasördür;
onların dosyalarına dokunulmadı.

## Tezi

**Ekran argümandır, tablo ektir.** Diğer iki deneme tablo-önce kurulmuştu: okuyucu sayfa boyunca
aynı üç sütunu okuyor, ekran görüntüleri tablonun altında sıralanıyordu. Burada sıra tersine
çevrildi: kareler büyük basılır, üzerlerine numaralı işaretler konur ve metin o numaralara konuşur.
Dayanak, sayfanın altındaki tek satırlık denetim izine iner.

Bunun ölçülebilir sonucu şu: okuyucu bir cümleyi doğrulamak için başka bir dosyaya gitmek zorunda
değil. "Gecikmiş halkası birebir aynı tutarı gösteriyor" cümlesinin yanındaki ③ işareti o halkayı
gösteriyor ve tutar karede okunuyor.

## Ortak kurallara uyum

- Konu ekseni korundu; her sayfa bir soru sorar ve aynı soruyu ürünlere yöneltir.
- **Kazandırdığı / Bedeli** sütunları yok. O kalıp her gözleme zorla bir avantaj ve dezavantaj
  buldurduğu için kullanılmadı; ürün tercihi Belge 3'e bırakıldı.
- Kanıt niteliği ürüne değil **ifadeye** bağlı. Rozetler beş türü ayırır: canlı kare, koşum kaydı,
  kaynak görseli, temsili çizim, görülmedi. Destek sayfası görseli ile pazarlama çizimi aynı sayılmaz
  ve bu ayrım sayfa 10 ile 11'i birbirinden ayırır.
- İçine girilemeyen ürünler sorunun altında yer alır; yalnız canlı karşılığı olmayan iki pano kendi
  sayfasını alır.
- Platform her satırda yazılı; masaüstü panolar telefon ekranlarıyla aynı ölçüye sokulmadı.

## Diğer iki denemede olmayanlar

1. **Ölçülmüş bölüşüm (sayfa 2).** Beş ana ekran aynı yükseklikte yan yana; her karenin dikey
   bölgeleri şerit olarak çizilir ve oranlar karenin geometrisinden hesaplanır. Wallet'ın karesinde
   tanıtım alanının hesap kartlarından geniş olduğu böyle görünür. Oranlar uygulamanın kendi alanına
   göredir; sistem çubukları dışarıda bırakılır ve yalnız açılışta görünen kareyi anlatır.
2. **Gezinme şeridi (sayfa 5).** Beş üründe bölüm seçicinin bulunduğu bölge aynı genişlikte
   kırpılıp alt alta konur. "Beş ayrı yer" iddiası tek bakışta doğrulanır.
3. **Kaynak görselinin tarihi (sayfa 10).** KolayBi panosundaki baloncukta 5.1.2023 yazıyor. Görselin
   demo verisi Ocak 2023'e ait; bu, "güncel sürüm doğrulanmadı" cümlesine somut bir dayanak verir.
4. **Çizim olduğunun kanıtı (sayfa 11).** Paraşüt karesinin pazarlama çizimi olduğu dört işaretle
   gösterilir: etiketsiz menü kutuları, iki halkada yinelenen aynı tutar, uyarı kutusuyla uyuşmayan
   üçüncü tutar ve geçerli bir Türkçe biçime uymayan sayı yazımı. İddia edilmez, gösterilir.
5. **Mekanik iddia tablosu.** `iddia-tablosu.md` elle tutulmaz; PDF'teki her işaret ve not
   metninden üretilir. Metin değişince tablo da değişir, ikisi ayrışamaz.

## Gizlilik

Yalnız E0115'te e-postadan türeyen hesap adı karartıldı ve karartma bu bölümün kopyasına uygulandı;
özgün kanıt ve hash'i değişmedi. Kişisel ad gösteren Wallet menü karesi (E0375) kullanılmadı, alt
kısmı (E0376) kullanıldı. PDF metninde hesap adı geçmiyor (doğrulandı).

## Üretim

`raporlar/` içinden:

```
.pilot-tools/venv/Scripts/python belge1/bolum-03-isaretli-ekran/uret.py
```

Tek komut; Word turu yok. PDF doğrudan PyMuPDF ile çizilir, işaretler Pillow ile kareye basılır.
Mevcut ortam kullanılır (Python 3.13, PyMuPDF 1.28.2, Pillow 12.3.0, Windows'un Segoe UI fontu);
yeni bağımlılık eklenmedi.

`icerik.py` tek içerik kaynağıdır: metin, işaret koordinatları, bölge ölçüleri ve kanıt bağları
oradadır. `uret.py` aynı veriden PDF'i, Markdown kopyasını, iddia tablosunu, eksik listesini,
kaynak dizinini ve manifesti üretir. Metin hiçbir yerde iki kez tutulmaz.

## Doğrulama (16 Eylül 2026)

- 12 sayfanın tamamı görüntüye çevrilip incelendi. İlk çıktıda işaretler gösterdikleri değerlerin
  üstünü kapatıyordu ("Alındı", "Total: 2,050.00", "138.89,20 ₺"); 40'tan fazla işaret boş alana
  taşındı ve alıntılanan her değerin okunur olduğu tek tek kontrol edildi.
- KolayBi karesi siyah bir zemin üzerine yerleştirilmişti; zemin kırpıldı ve pano büyütüldü.
- Yerleşim motoru sığmayan metinde uyarı üretir: **0 uyarı**.
- Kullanılan 19 özgün karenin SHA-256 değeri `KANIT-ENVANTERI.md` ile karşılaştırıldı: **19/19 aynı**.
  Karşılaştırma üretim betiğinin içindedir, elle yapılmaz.
- PDF metninde bozuk karakter yok; Segoe UI Regular ve Bold gömülü.
- Metindeki her `Şekil N.M` göndermesinin karşılığı basılı: **kırık gönderme yok** (makine kontrolü).
- Kişisel ad PDF metninde geçmiyor (makine kontrolü).
- Yeni emülatör koşumu yapılmadı, özgün kareler değişmedi, uygulama kodu değişmedi; commit yok.

## v2'de düzeltilenler (16 Eylül 2026 inceleme turu)

1. **Eksik soru eklendi.** Bölümün kapsamındaki "ilk karşılaşma" sorusu ilk sürümde düşmüştü; her iki
   önceki deneme de bu soruyu ayrı ele alıyor. Sayfa 3 olarak eklendi (Bluecoins karşılama, Hesap
   Defterim açıklama diyaloğu, Goodbudget giriş/hane seçimi). Sonraki sayfaların numarası kaydı.
2. **E0231 açılıp doğrulandı.** Karenin dosya adı `07-rapor-agustos.png` ama alt çubukta seçili sekme
   İstatistik. Eksik listesinden çıktı; ad/içerik uyuşmazlığı E0274'ünkiyle birlikte envanter notuna
   geçti.
3. **Şekil numaraları koda bağlandı.** Sayfa düzenleri artık sabit numaraya değil `duzen` bayrağına
   bakıyor; numaralar soru numarasından türüyor. Bir soru eklendiğinde numaralar elle düzeltilmiyor.
4. **Kırık çapraz gönderi kapatıldı.** Sayfa 2'nin kareleri numarasızdı, oysa sayfa 4 onlara atıf
   yapıyordu. Numaralar basıldı; her gönderinin karşılığının bulunduğu artık makineyle kontrol ediliyor.
5. **Kimlik doğrulaması eklendi.** Üretici, içerikte anılan her E kimliğinin sözlükte ve diskte
   bulunduğunu baştan kontrol ediyor; eksikse adıyla durup hata veriyor.
6. **Teslim kopyası da karartılıyor.** `kanit/E0115.png` artık karartılmış kopyadır; klasör tek başına
   paylaşılsa bile hesap adı çıkmıyor. Özgün dosya ve hash'i değişmedi, envanter karşılaştırması
   özgüne göre yapılıyor.
7. **Kayıt düğmesi kırpıntıları daraltıldı.** Wallet kırpıntısındaki tanıtım kartı kapatma işareti ve
   Bluecoins kırpıntısındaki komşu satır artık kadraja girmiyor.
8. **Bölge şeridi görünür oldu.** Açık renkli "boş alan" bantları beyaz zeminde kayboluyordu; ince
   kenarlık eklendi.
9. **Sayfa 5'in sağ sütunu dolduruldu.** Gezinme şeridi sayfasında boş kalan alana kareden okunanlar
   bloğu kondu.

## Bilinen sınırlar

- Sayfa 2'nin oranları yalnız açılışta görünen kareyi anlatır. Bluecoins ve Wallet karelerinde
  içerik alt kenarda kesiliyor; bu iki ürünün gerçek sayfa uzunluğu bu karelerden bilinmiyor.
- Sayfa 3'ün üç karesi kurulumun tamamını değil, yalnız ilk görülen ekranı gösteriyor; adımların
  atlanabilir olup olmadığı denenmedi.
- Eksik kanıtların hangisinin nasıl kapanacağı `eksik-listesi.md` içinde; mevcut test verisi
  silinmez kuralı orada yazılı.
