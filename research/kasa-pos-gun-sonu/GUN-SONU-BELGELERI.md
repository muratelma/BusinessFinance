# Gün sonu belgeleri — ne var, ne yok, kamerayla ne okunur

**Tarih:** 28 Eylül 2026. Kullanıcının isteği: "gün sonu alabildiğimiz raporları ve içeriklerini araştır";
arkadaşının muhasebecisinin ay sonunda cihazdan aylık rapor aldığı bilgisi; "Z raporunda her işlem var,
tek fotoğrafla okumak gerçekçi olmaz, yalnız sondaki tutarlar".

## 1 · Dört belge

| Belge | Nereden | Ne içerir | Ne içermez | Kaynak |
|---|---|---|---|---|
| **Z raporu** (günlük) | Yazar kasa (YN ÖKC); her gün zorunlu | Z no, tarih, günün toplam satışı (TOPSAT), toplam KDV ve oran kırılımı, **NAKİT** ve **KREDİ** tahsilat, fiş sayısı; faturalı satışlar ayrı "bilgi fişi" satırında ve **TOPSAT'a dahil değil** | Satışları tek tek **listelemez** (özet belge) | [1], [2], [3] |
| **Banka gün sonu fişi** | Banka POS'u ya da yazar kasa POS'un banka uygulaması | **Toplam rapor:** satış/iade/iptal adet ve tutarları. **Detay rapor:** her kart işlemi (kart son 4 hane, tutar, saat) + en altta toplamlar | Nakit | [4], [5] |
| **Mali hafıza raporu** | Yazar kasa; iki tarih arası, iki Z arası, dönem (aylık) | Her Z için tarih, Z no, satış toplamı, KDV (detaylı) ya da dönem toplamı (özet). Muhasebeci aylık raporu KDV beyanında kullanıyor | **Nakit/kart ayrımı yok** | [2], [6] |
| **Üye işyeri dökümü** (aylık / internet şubesi) | Banka | Gün gün: işlem tarihi, **hesaba geçiş tarihi**, kart tipi, adet, tutar, **komisyon** | Nakit | [7] |

**Kullanıcının arkadaşından gelen bilgi doğrulandı:** muhasebecinin ay sonunda cihazdan aldığı rapor mali
hafıza raporudur; her günün Z toplamı ayrı satırdır. **Düzeltme:** "her işlemin yazdığı" uzun liste Z raporu
değil, bankanın **detaylı gün sonu** fişidir; Z raporu bir özettir.

## 2 · Kamerayla okumaya etkisi

| Durum | Okunacak belge | Okunacak kısım | Sonuç |
|---|---|---|---|
| **Günlük** (her akşam) | Z raporu | Tamamı (kısa özet): NAKİT, KREDİ, TOPSAT, tarih, Z no | Nakit satış = NAKİT |
| | Banka gün sonu | **Yalnız alttaki toplam bloğu** (detaylı fiş uzunsa son kısım fotoğraflanır; ya da "Toplam rapor" alınır) | POS başına kartlı satış |
| **Aylık toplu** (ay sonu, yeni seçenek) | Mali hafıza aylık detaylı raporu | Gün satırları (1–2 fotoğraf) | Her günün toplam satışı |
| | Üye işyeri dökümü (internet şubesinden PDF) | Gün satırları | Her günün kartlı satışı, komisyonu, geçiş günü |
| | — | — | Günlük nakit = günün toplamı − günün kartlısı |

**Kullanıcının kuralı doğrulanıyor:** okuma işlem listesini değil, **toplamları** hedefler.

## 3 · Gün içinde tek tek girilen satışla çakışma (H3'ün yeniden değerlendirmesi)

Yalnız toplam okunduğunda "hangi satış Z'de" diye tek tek eşleştirmek mümkün değil. Ayrıca iki tür gün içi
satış farklı davranır:
- **Tezgâh satışı** (fişli): Z'nin içindedir → gün sonuna dahil; tek tek de girildiyse çifte sayım olur.
- **Faturalı satış** (e-arşiv / bilgi fişi): Z'nin TOPSAT'ına **dahil değildir** [3] → gün sonundan ayrıdır;
  düşülürse eksik sayım olur. Ama kartla ödendiyse banka gün sonunun toplamında **vardır**.

Buradan çıkan sade kural (eski H3 önerisinin yerine): gün sonu **düşme yapmaz, sorar**. O gün satış
kategorisinde ayrıca girilmiş kayıt varsa panel listeler ve **gün başına bir kez** sorar: "Bunlar gün sonu
raporunuzda var mı?" → *Evet, düş* / *Hayır, ayrı satış*. Kartlı taraf için aynı soru banka toplamına sorulur.

## 4 · Gerekli görseller (P7 için, kullanıcı sağlayacak)
- 2–3 marka yazar kasadan **günlük Z raporu**
- **Banka gün sonu**: bir "toplam" ve bir "detay" örneği
- Bir **mali hafıza aylık (dönem) raporu**, tercihen detaylı
- Bir **üye işyeri dökümü** (internet şubesi PDF'i ya da ekran görüntüsü)
- Varsa bir **yemek kartı** dökümü
- İşletme adı, adres, vergi/sicil numarası, kart numaraları karartılmış

## 5 · Gerçek belge incelemesi (28 Eylül, kullanıcının getirdiği 5 fotoğraf)

Belgeler gerçek bir giyim dükkânına ait; `pos belgeleri/` klasörü Git dışında (`.gitignore`). İşletme adı,
adres ve vergi numarası buraya yazılmadı. Beş fotoğrafta: bir **toplu Z raporu**, iki **boş Z raporu**
(satışsız günler) ve bir günün üç **satış fişi** (hepsi nakit; kartlı işlem yok).

| # | Gözlem | Tasarıma etkisi |
|---|---|---|
| G1 | **"TOPLU Z RAPORU (22-09 – 24-09)"**: Z birkaç gün alınmayınca cihaz bu günleri **tek Z**'de topluyor; rapor 24 Eylül **sabah 09:34**'te basılmış | Bir gün sonu **birden çok günü** kapsayabilir. Gün sonu "tarih aralığı" kabul etmeli ya da toplamı son güne yazmalı (karar) |
| G2 | Normal Z akşam 20:00'de basılmış; ama toplu Z sabah basılmış | Z'nin **basıldığı gün** her zaman satış günü değil. Okunan tarih önerilir, kullanıcı onaylar |
| G3 | Z satırları: departman (ör. GİYİM, adet, tutar) · BRÜT · NET · **NAKİT** (adet, tutar) · geçerli satış fişi sayısı · mali fiş sayısı · **slip fiş sayısı** · oran başına TOP/KDV · TOP · KDV · **KÜMSAT / KÜMKDV** · EKÜ no · **Z no** · mali sicil | Okunacak alanlar: **tarih (veya aralık), Z no, NAKİT, kredi kartı, TOP**. KDV okunmaz (vergi kararı) |
| G4 | Sıfır olan ödeme satırı **basılmıyor**: kartlı satış olmayan Z'de "KREDİ" satırı yok | Satır yoksa değer 0 kabul edilir |
| G5 | **KÜMSAT** (cihazın ömür boyu toplamı, 1,4 milyon TL mertebesinde) TOP'a çok yakın duruyor | **Okuma riski:** model günlük toplam yerine kümülatif toplamı alabilir. Okuma kuralı: günlük toplam = TOP; KÜMSAT asla |
| G6 | Satışsız günlerde de Z alınıyor (BRÜT 0,00) | "Bugün satış yok" geçerli bir gün sonudur; boş gün kapatılabilir |
| G7 | **Z numaraları ardışık** (3139, 3140, 3141 … 3144) | **Kontrol fırsatı:** uygulama Z numaralarını izlerse **atlanmış gün sonunu** kendiliğinden söyler ("Z 3142 girilmedi") |
| G8 | **Satış fişlerinde de Z numarası yazıyor** ("Z NO: 3144") ve fiş numarası var | **H3 için net çözüm fırsatı:** gün içinde fiş fotoğrafıyla girilen satış, o günün Z numarasını taşır; akşam aynı Z girilince uygulama tam olarak o fişleri düşebilir. Elle girilen satışta yine "günde bir kez sor" |
| G9 | Bu cihazda kartlı işlem yok (slip fiş sayısı 0) | Kartlı satışlı bir Z ve bir banka gün sonu fişi hâlâ gerekli (§4) |

## 6 · Kullanıcı kabulleri (28 Eylül)
- Z raporu işlemleri tek tek değil, **adet ve toplam** olarak veriyor; özet olduğu için **tek fotoğrafa sığar**.
  Kartlı satışlı örnek bulunana kadar **kredi kartı satırının NAKİT satırıyla aynı biçimde** (adet + tutar,
  onun altında ya da üstünde) basıldığı varsayılır. Z numaraları kullanılır.
- **POS'suz, elle kasa defteri tutan esnaf** (ör. Excel'de gelen-giden): aynı gün sonu panelini fotoğrafsız,
  elle kullanır; Z numarası isteğe bağlıdır. Nakit girişleri, giderler ve kasa sayımı bugünkü gibi çalışır.
  Hiçbir akış fotoğrafa ya da POS'a bağımlı değildir.

## 7 · Uzman doğrulaması (kullanıcının görüştüğü kişi, 28 Eylül)
- Z raporunda NAKİT'in yanında bir **KART** satırı var; kartla ödenenleri yazar. **Kredi kartı / banka kartı
  ayrımı yok.** (§6'daki varsayım doğrulandı.)
- Z raporunda **iptal işlemleri de görünüyor.** Okuma kuralı: günün satışı **NET/TOP** (iptaller düşülmüş);
  BRÜT ve iptal satırı okunmaz.
- Aylık rapor gün gün gün sonu toplamlarını verir; **nakit/kart ayrımı yok** (§1 doğrulandı).

## Kaynaklar
1. Z raporu alanları — https://www.fisle.co/blog/z-raporu-muhasebe-kaydi
2. YN ÖKC raporları (Z, aylık Z, mali hafıza) — https://www.tokeninc.com/blog/yeni-nesil-yazar-kasa-poslardan-hangi-raporlar-alinir/
3. Bilgi fişi / fatura TOPSAT'a dahil değil — https://www.fisle.co/blog/z-raporu-muhasebe-kaydi
4. Banka gün sonu: detay ve toplam rapor — https://www.qnb.com.tr/isim-icin/kobi/kobi-pos-cozumleri/pos-kilavuzu/pos-menusu
5. Gün sonu özet/detaylı — https://odeal.com/blog/pos-cihazi-gun-sonu-alma-nasil-yapilir/
6. Mali hafıza rapor türleri — https://www.tokeninc.com/yardim-merkezi/urun-kullanimi-beko-x30tr/mali-hafiza-raporlari/ ; aylık rapor KDV beyanında: arama özeti (Token)
7. Üye işyeri dökümü alanları — https://www.bankkartpos.com.tr/pos-yardim/slip-ekstre-aciklamalari
