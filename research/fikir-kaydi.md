# Fikir kaydı

İnceleme ve test sırasında aklımıza gelen fikirler burada toplanır. **Kayıt uygulama değildir:** bir fikir
buraya yazıldığı anda koda girmez. Temasının karar belgesi yazılınca orada seçenek olarak değerlendirilir,
kullanıcı karar verir, sonra aşamaya bağlanır.

Açılış: 28 Eylül 2026 (kullanıcı kararı).

## Nasıl kullanılır

- Her fikir tek satır: **ne**, **neden** (kimin sözü ya da hangi bulgu), **tema**, **durum**.
- Durumlar: `açık` · `karar belgesinde: <belge>` · `alındı: <aşama/commit>` · `alınmadı: <gerekçe>`.
- Temalar: **Para yolları** (Kasa, gün sonu, POS, cari, transfer) · **Listeler ve filtreler** (İşlemler,
  kategori, arama) · **Belge okuma** (fiş, dekont, Z raporu) · **Veri** (CSV, yedek) · **Arayüz** (tasarım
  turu, tutarlılık).

## Kayıt

| # | Fikir | Neden / kaynak | Tema | Durum |
|---|---|---|---|---|
| F01 | İşlemler'e gelişmiş filtre: hesap ya da kart, kategori, tarih aralığı, kapsam | Kullanıcı, 28 Eylül: "hesaplarını ayrı ayrı seçebilse, o kartta veya hesapta ne harcandığını görebilse". Sunucu ve istemci veri modeli hesap/kart/kategori filtresini bugün destekliyor; eksik olan arayüz | Listeler ve filtreler | açık — "Listeler ve filtreler" teması (G3), sonra P8 |
| F02 | Kategori giderlerini İşlemler'e taşımak: Özet'teki kategoriye dokununca İşlemler o kategoriyle açılır; İşlemler'de "kategoriye göre" gruplu görünüm | Kullanıcı, 28 Eylül. Toplamlar Özet raporundan gelir, ikinci hesap yolu açılmaz | Listeler ve filtreler | açık — F01 ile birlikte |
| F03 | İşlemler'i her yerden açılan tek liste yapmak: hesap, kart, kasa, kategori ve cari sayfaları filtreli İşlemler'e kapı olur | F01–F02'nin ortak sonucu | Listeler ve filtreler | açık |
| F04 | Gün sonu raporunu (Z raporu, banka POS gün sonu fişi) kamerayla okuyup gün sonu girişini doldurmak | Kullanıcı, 28 Eylül: "dekont okumanın altında değil, ayrı bir yerde"; aynı gün: "kasa kısmında gün sonu raporunu kamerayla okumayı yapacağız" | Belge okuma | **alındı: Kasa kapsamına** — örnek rapor görselleri gerekiyor |
| F05 | CSV içe aktarımını baştan gözden geçirmek; POS yatışının ikinci kez gelir sayılması (U8) bu turda çözülür | Kullanıcı, 28 Eylül: "bu kısımlar komple elden geçirilmesi gerekiyor". Bulgu U8 | Veri | açık — ayrı tur |
| F06 | POS yatışındaki kesinti (BSMV vb.) İşlemler'de yatışın detayında görünsün | Kullanıcı, 28 Eylül | Para yolları | alındı: `kasa-pos-gun-sonu/KAPANIS.md` KP11 |
| F07 | Banka bakiyesini uygulamadakiyle karşılaştıran "banka sayımı" | Karar belgesi önerisiydi | Para yolları | alınmadı: kullanıcı, 28 Eylül — "kullanıcıdan %100 bütün hareketleri girmesini bekler; teoride mantıklı, pratikte olmaz" |
| F08 | Kasa'da şahsi cüzdan görünmesin; Kasa yalnız işletme kasası | Kullanıcı, 28 Eylül: "şahsi kasası diye bir şey yok, ortalığı karıştırır" | Para yolları | alındı: KP15 |
| F09 | Yemek kartı ayrı özellik değil, adı verilmiş bir POS tanımı | Kullanıcı, 28 Eylül; araştırma doğruladı | Para yolları | alındı: KP10 |
| F10 | Gün sonu girişinde KDV sorulmasın; Z raporu okunursa kendiliğinden dolsun | Araştırma: Z raporunda KDV var, yazar kasa GİB'e gönderiyor, muhasebeci Z'den kaydediyor | Para yolları | alındı: KP19 (KDV alanları da kaldırıldı) |
| F11 | Formlar açıldıkları yerden önden dolsun (Kasa'dan açılan gider formunda kasa seçili) | Test T2 | Arayüz | açık |
| F12 | Tarih ve tutar alanları her formda aynı biçimde: Türkçe tarih, binlik ayırıcı | Test T2 (gelir formunda `2026-09-28`, `20790`) | Arayüz | açık — 5 ekranlık tasarım turu |
| F14 | "Vergi durumunuz ne?" sorusu | Vergi araştırması | Vergi | alınmadı: KDV alanları kalkınca açıp kapatacağı bir şey kalmıyor (kullanıcı, 28 Eylül); yerine hazır vergi türleri listesi |
| F15 | Vergi takvimi kalemine "karşılık ayır" kısayolu (hedef mekanizmasıyla) | Vergi araştırması | Vergi | açık |
| F16 | Basit usul esnafına yıllık satış/alış toplamını sınıra karşı göstermek (had takibi) | Vergi araştırması; kapsam sınırında, hesap sayılabilir | Vergi | açık — kapsam dışı önerildi |
| F17 | Muhasebecinin gönderdiği tahakkuk fişinden vergi tutarını ve son ödeme gününü almak (fotoğraf ya da elle) | Kullanıcı, 28 Eylül: "ileride ekleyebiliriz ama elimizde örnek yok; şimdilik muhasebeciden otomatik belge almayı es geçelim" | Belge okuma | açık — örnek görülünce |
| F18 | Muhasebecinin esnaf adına ödediği masraflar (araç sigortası, vergi): muhasebeci karşı taraf, masraf cari borç, esnafın muhasebeciye ödemesi borcu kapatır | Kullanıcı, 28 Eylül | Para yolları / Vergi | vergi ekranında yok (V-K11: cari hesaba bağlanmaz); vergi dışı masraflar cari hesapla → F21 |
| F19 | Birikmiş vergileri tek ödemeyle kapatmak (gecikme zammı dahil tutar) | Kullanıcı, 28 Eylül | Vergi | alındı: vergi §6.5 toplu vergi ödemesi |
| F20 | Yapılandırılan vergi/Bağkur borcunu mevcut borç sözleşmesiyle izlemek | Vergi araştırması | Vergi | açık |
| F21 | Cari hesabı geliştirmek: hazır karşı taraf şablonları (muhasebeci, ev sahibi, tedarikçi), muhasebecinin ödediği vergi dışı masraflar | Kullanıcı, 28 Eylül: "cari hesap kısmını da geliştirebiliriz, belki hazır paketler sunarız" | Para yolları | açık — ayrı tema |
| F22 | Özet'e vergi raporu / widget'ı (bu yıl ödenen vergiler, yaklaşanlar) | Kullanıcı, 28 Eylül: "ileride Özet'teki rapor kısımlarını ve widget sayılarını artıracağız" | Arayüz | açık |
| F23 | **Kasa sayımını geliştirirken yeniden düşün.** Bugünkü karar (28 Eylül): sayım gün sonundan ayrıldı, bağımsız "Kasayı say"; beklenen = son sayımdan bu yana; sıklığı kullanıcı seçer; fark kaydı isteğe bağlı, sebep sorulur. Açık kalan: aylık raporla (ve banka dökümüyle) ay sonu sayımının bağlanması; Z kullanan esnaf için sayımın değeri | Kullanıcı, 28 Eylül: "şimdilik katılıyorum, ileride geliştirirken bir kez daha düşünelim" | Para yolları | açık — P5/P6'da yeniden bakılacak. 29 Eylül denetimi: beklenen = kasanın hesap bakiyesi, kaydedilmemiş fark ayrı satırda (KP9) |
| F24 | Gün sonu panelinde gün içi satış listesinin varsayılanı (bugün: elle girilenler işaretsiz, Z no eşleşen fişler işaretli) | Kullanıcı, 28 Eylül: "geliştirmeye başladığımızda tekrar gözden geçiririz, değiştirmesi kolay" | Para yolları | alındı: KP7 (29 Eylül denetimi — satışlar ve kartla tahsilat varsayılan düşülür); P5/P7'de gerçek Z ile yeniden bakılır |
| F13 | Geçmiş ay görünümünde "Hesap bakiyeleri" o ayın sonunu göstersin ya da bugünün olduğunu yazsın | Test T7 | Arayüz | açık |
| F25 | POS komisyonunun gider kategorisi her seferinde seçilmesin; komisyon bankaya ödenen bir gider, varsayılan kategori "Banka ve POS komisyonu" kendiliğinden dolsun | Kullanıcı, 29 Eylül emülatör denemesi: "gider kısmında çok kategori seçeneği olduğu için problem oluyor … otomatik dolduralım" | Para yolları | alındı: 06.3 Grup 4 (POS tanımında bir kez, varsayılan "Banka ve POS komisyonu"; form tanımdan dolar) |
| F26 | Veri Araçları'nın (CSV içe/dışa aktarma, belgeler) bir bütün olarak ayrıca incelenmesi | Kullanıcı, 29 Eylül emülatör denemesi: "bu veri araçları kısmını sonra ayrı incelememiz lazım" | Veri | açık — F05 ile birlikte ayrı tur |
