# Belge 2 — çıkarım denetiminde değişen metinler (inceleme listesi)

23 Eylül 2026. Kaynak: kırpma öncesi yedeğin (`_yedek/2026-09-23-kirpma-oncesi/`)
`icerik.py` dosyaları ile bugünkü `icerik.py` dosyalarının farkı. Yalnız kırpma kutusu
değişen satırlar elendi; kalan her metin farkı aşağıda. Kırpma turu metne dokunmadı.
Gerekçelerin özeti `belge2-entegrasyon-plani.md` §12'de.

Sayfa numarası birleşik `tam/belge2.pdf` içindir. Tür: **Ç** = belgenin kendi gövdesiyle
çelişiyordu · **F** = kanıtın söylediğinden fazlasını söylüyordu · **B** = "kaybettirdiği"
hücresi bir kanıt boşluğunu anlatıyordu, ürünün bedeliyle değişti.

Kutucuğu işaretleyerek ilerleyebilirsiniz; itiraz ettiğiniz maddeyi numarasıyla yazın.

---

## Bölüm 1 — Gelir ve gider

- [ ] **1. s.11 · 1.8 çıkarım bloğu · F**
  - Eski: "Beş üründe de kategori paylaşılan bir kovadır … Hesap Defterim bu ilkeyi uca
    taşıyor: kategori seçicisi hiç yok, yalnız serbest metin var … Kimlik ile raporlama
    kovasını tek alanda birleştirmenin sonucu, ikisinin birden kaybolması."
  - Yeni: "Seçicisi olan dört üründe kategori — Goodbudget'ta zarf — paylaşılan bir
    kovadır … Hesap Defterim bu ayrımı tersinden kuruyor: kimlik Notlar alanında, kategori
    ise seçici değil, adı "Açıklama / Kategori" olan ikinci bir serbest metin kutusu.
    Açıklama ile kovayı tek kutuda birleştirmenin sonucu, kovanın kaybolması: aynı harcama
    her seferinde başka yazılabiliyor ve okuma yüzeyinde kategori kırılımı hiç yok."
  - Neden: Hesap Defterim'de kategori seçicisi yok, "beş üründe" yanlıştı. Kimlik de
    kategoriyle aynı alanda değil, Notlar'da.

- [ ] **2. s.12 · 1.8 Belge 3 sorusu · F**
  - Eski: "Beş üründe de kategori kovadır, kimlik değildir."
  - Yeni: "Seçicisi olan dört üründe kategori kovadır, kimlik değildir."

## Bölüm 2 — Hesaplar ve aktarım

- [ ] **3. s.20 · 2.7 çıkarım başlığı ve gövdesi · Ç**
  - Eski başlık: "Aktarım hiçbir üründe tek bir nesne değil"
  - Yeni başlık: "Aktarım bir üründe tek kayıt, üçünde iki satır"
  - Eski gövde: "Dört üründe de aktarım iki uç taşıyor ve üçünde listede iki satır olarak
    görünüyor. Toplamı koruyan şey tek bir nesne olması değil, iki bacağın aynı günde
    netlenmesi."
  - Yeni gövde: "Money Manager aktarımı tek bir Havale kaydı olarak tutuyor: iki hesabı
    aynı satır taşıyor ve günün başlığı 0/0 kalıyor. Bluecoins ve Wallet iki bacak
    gösteriyor ve ikisini aynı günde netliyor. Toplamı koruyan şey ya tek kayıt olması ya
    da iki bacağın birlikte yazılması."
  - Dayanağa E0228 eklendi.
  - Neden: 2.3, Money Manager'da tek Havale satırını gösteriyor.

- [ ] **4. s.21 · 2.7 Belge 3 sorusu · Ç**
  - Eski: "… Hesap Defterim'in örneği, iki bağımsız satırın sessizce
    tutarsızlaşabildiğini gösteriyor."
  - Yeni: "… Money Manager tek kayıt tutuyor; Hesap Defterim'in örneği iki bağımsız
    satırın sessizce tutarsızlaşabildiğini gösteriyor."

## Bölüm 3 — Kart

- [ ] **5. s.32 · 3.9 taksit çıkarımı · F**
  - Eski başlık: "Taksit yalnız bir üründe aylık gider eğrisine dönüşüyor"
  - Yeni başlık: "Taksit yalnız bir üründe **kendiliğinden** aylık gider eğrisine dönüşüyor"
  - Eski (Bluecoins): "… kalan beşi hatırlatıcı olarak bekledi."
  - Yeni: "… kalan beşi hatırlatıcıda bekliyor ve ancak kullanıcı her birini
    kaydettikçe gidere giriyor."
  - Eski özet: "… birinde bir ay 1.000 sonra sessizlik …"
  - Yeni: "… altı ay boyunca kendiliğinden 1.000; birinde kullanıcı her ay kaydettiği
    sürece 1.000, kaydetmediği ay sıfır; birinde bir ay 6.000 sonra sıfır."
  - Dayanağa "kullanıcı kontrolü (Bluecoins)" eklendi.
  - Neden: sizin kontrolünüz, kalan taksitlerin elle kaydedildikçe gider olduğunu gösterdi.

- [ ] **6. s.33 · 3.9 kazanç tablosu, Wallet · Ç**
  - Eski: "… ve eşik uyarısı kart mantığına özgü"
  - Yeni: "… ve ödeme günü alanı kart hesabına özgü"
  - Neden: 3.2'ye göre uyarı, her hesapta bulunan en az bakiye bildiriminden geliyor (E0437).

- [ ] **7. s.33 · 3.9 kazanç tablosu, Goodbudget "kaybettirdiği" · B**
  - Eski: "Kart hesabı ücretsiz pakette açılamadığı için kart modeli hiç görülemedi"
  - Yeni: "Kart hesabı ücretsiz pakette açılamıyor; kart kullanan biri bu pakette kartını
    izleyemiyor"

## Bölüm 4 — Borç ve cari

- [ ] **8. s.43 · 4.7 kazanç tablosu, KolayBi "kaybettirdiği" · B**
  - Eski: "Yalnız yüzey görüldü; alanların gelir/gider ve bakiye etkisi ölçülemedi"
  - Yeni: "Cari kartı çok alanlı: vade, iskonto, durumu olan açılış bakiyesi ve proje tek
    formda soruluyor; karşı taraf açmak canlı ürünlerdeki bir hesap açmaktan uzun bir iş"

## Bölüm 5 — Zaman: plan, tekrar, bütçe

- [ ] **9. s.52 · 5.8 plan çıkarımı · F**
  - Eski başlık: "Gerçekleşmemiş plan hiçbir üründe toplama girmiyor"
  - Yeni başlık: "Gerçekleşmemiş plan **ölçülen** hiçbir üründe toplama girmiyor"
  - Eklenen paragraf: "Taksit bu kuralın dışında kalıyor: Money Manager gelecek
    taksitleri plan değil, önceden yazılmış kayıt olarak tutuyor ve her biri kendi ayının
    giderine giriyor (Bölüm 3). Aynı ürün tekrarlayan planı toplama katmıyor, taksidi
    katıyor."
  - Dayanağa E0407 eklendi.

- [ ] **10. s.52 · 5.8 bütçe çıkarımı · F**
  - Eklenen paragraf: "Money Manager da aynı tarafta ama gidişat göstergesi olmadan:
    bütçe kategori başına bir sınır, Toplam sekmesinde bir ilerleme çubuğuyla ölçülüyor."
  - Eski (Goodbudget): "Bu yüzden bu üründe "bütçeyi aştım" diye bir durum yok; zarf
    boşalıyor."
  - Yeni: "Bu yüzden aşımın ayrı bir bütçe ekranı yok; sınır zarfın kendi bakiyesi. Zarfı
    aşan bir harcama bu koşumda denenmedi."
  - Dayanağa E0405 ve E0431 eklendi.
  - Neden: aşım denenmedi, "durum yok" denemezdi.

- [ ] **11. s.53 · 5.8 kazanç tablosu, Goodbudget · F + B**
  - Kazandırdığı, eski: "Bütçe ile bakiye aynı şey olduğu için ayrı bir bütçe kavramı ve
    aşım durumu …" → yeni: "Bütçe ile zarfın bakiyesi aynı şey olduğu için ayrı bir bütçe
    ekranı …"
  - Kaybettirdiği, eski: "Planın gerçekleşmesi bu koşumda ölçülemedi; zarf modeli hesap
    katmanıyla uyuşmuyor"
  - Yeni: "Zarf katmanı hesap katmanıyla uyuşmuyor: bütçenin bıraktığı para ile hesaptaki
    para iki ayrı sayı"

## Bölüm 6 — Sınıflandırma

- [ ] **12. s.61 · 6.7 çıkarım bloğu, tek cep · F**
  - Eski: "Canlı beş ürün kişisel finans uygulaması ve kullanıcılarının tek bir cebi
    olduğunu varsayıyor. …"
  - Yeni: "Canlı beş ürün kişisel finans uygulaması ve **kayıt düzeyinde** tek bir cep
    varsayıyor. … Bluecoins'in etiket listesinde İş ve Kişisel'in bulunması, ayrımın akla
    geldiğini ama bir alana dönüşmediğini gösteriyor."
  - Neden: E0088 İş ve Kişisel etiketlerini gösteriyor.

- [ ] **13. s.61 · 6.7 çıkarım bloğu, QuickBooks filtresi · F**
  - Eski: "… Canlı ürünlerde karşılığı olmayan bu ayrım, kaydın kendisiyle o kaydın hangi
    toplama gireceğini birbirinden ayırıyor."
  - Yeni: "… Kaydın kendisiyle o kaydın hangi toplama gireceği birbirinden ayrılıyor.
    Canlı ürünlerde bu ayrımın karşılığı hesap düzeyinde duruyor: Money Manager'ın Toplama
    Dahil Et ve Wallet'ın Exclude from stats anahtarı bütün bir hesabı toplamdan
    çıkarıyor, hesabı silmeden. Kayıt düzeyinde bir karşılığı görülmedi."
  - Dayanağa E0252, E0253, E0437 eklendi.

- [ ] **14. s.62 · 6.7 kazanç tablosu, QuickBooks "kaybettirdiği" · B**
  - Eski: "Ürünün hiçbir iç ekranı görülmedi; ayrım ülkeye özgü bir vergi formuna bağlı"
  - Yeni: "Ayrım ülkeye özgü bir vergi formuna bağlı; alan iki değerli olduğu için kısmi
    durum ancak kaydı bölerek çözülüyor"

## Bölüm 7 — Rapor (Wallet'ın "varsayılan son 12 hafta" iddiası: 7.2–7.8 ve 10.3–10.4'te yedi yer)

Ortak neden: karelerde iki ayrı pencere seçili (E0280 son 30 gün, E0284 son 12 hafta).
Hangisinin varsayılan olduğunu gösteren kanıt yok. Desteklenen şey raporun takvim ayıyla
değil, kayan bir pencereyle açılması.

- [ ] **15. s.65 · 7.2 notu · F**
  - Eski: "Wallet'ın varsayılanı bir takvim ayı değil, son on iki hafta. … kullanıcı aralığı
    elle değiştirmezse üç aylık bir toplam oluyor."
  - Yeni: "Wallet'ın raporu takvim ayıyla değil kayan bir pencereyle açılıyor: bu karede
    son 12 hafta, Şekil 7.1'de son 30 gün. … kullanıcı aralığa bakmazsa bir aydan uzun bir
    toplam olabiliyor."

- [ ] **16. s.68 · 7.5 tablo hücresi · F**
  - "Son 12 hafta" → "Kayan pencere: son 30 gün ya da 12 hafta"

- [ ] **17. s.70 · 7.7 akış sayfası, Wallet dalı · F**
  - Eski: "Son on iki hafta. Varsayılan bir takvim ayı değil, kayan bir pencere."
  - Yeni: "Kayan bir pencere: son 30 gün ya da son 12 hafta. Takvim ayı ayrı bir sayfadan
    seçiliyor."

- [ ] **18. s.71 · 7.8 çıkarım bloğu, dönem · F**
  - Eski: "Wallet'ın rapor ekranı son on iki haftayla açılıyor … kullanıcı aralığı elle
    değiştirmezse üç üründe üç ayrı sayı."
  - Yeni: "Wallet'ın raporu takvim ayıyla değil kayan bir pencereyle açılıyor — karelerde
    son 30 gün ve son 12 hafta; takvim ayı ancak ayrı bir sayfadan seçiliyor. Goodbudget
    ve Money Manager takvim ayıyla açılıyor. … kullanıcı aralığa bakmazsa üründen ürüne
    değişiyor."
  - Dayanağa E0280 ve E0438 eklendi.

- [ ] **19. s.71 · 7.8 çıkarım bloğu, Bluecoins'in iki sayısı · F**
  - Eski: "Koşumda biri 21.350, öteki 43.850 okundu."
  - Yeni: "Koşumda biri 21.350, öteki 43.350 okundu (varlık 43.850, cari −500)."
  - Neden: Net Kazanç 43.350; 43.850 Varlıklar satırı.

- [ ] **20. s.72 · 7.8 kazanç tablosu, Wallet "kaybettirdiği" · F**
  - Eski: "Varsayılan aralık takvim ayı değil son on iki hafta; aralığa bakmayan kullanıcı
    üç aylık toplamı aylık sanabilir"
  - Yeni: "Rapor aralığı takvim ayı değil kayan bir pencere; aralığa bakmayan kullanıcı son
    30 günü ya da 12 haftayı "bu ay" sanabilir"

## Bölüm 8 — Veri

- [ ] **21. s.81 · 8.7 kazanç tablosu, KolayBi "kaybettirdiği" · B**
  - Eski: "Yalnız yüzey görüldü; çıktının gerçek içeriği ve sütun anahtarlarının etkisi
    ölçülemedi"
  - Yeni: "Çıktının biçimi her seferinde soruluyor: tarih aralığı, para birimi, açıklama ve
    yedi sütun anahtarı"

- [ ] **22. s.81 · 8.7 Belge 3 sorusu · F**
  - "… ürünler tek biçim sunuyor." → "… ürünlerde çıktı yalnız bir dosya."
  - Neden: canlı ürünlerde dört ayrı biçim var; ortak olan, çıktının bir dosya olması.

## Bölüm 9 — Belge ve vergi

- [ ] **23. s.84 · 9.2 notu · Ç**
  - "Canlı beş üründe kayıt kimseye ait değil …" → "Canlı ürünlerde **sıradan** kayıt
    kimseye ait değil …"

- [ ] **24. s.91 · 9.9 çıkarım, olay ile ödeme · Ç**
  - Eski: "Canlı beş üründe bu ayrım hiç yok. Bölüm 1'de görüldüğü gibi kayıt …"
  - Yeni: "Canlı ürünlerde bu ayrım genel bir kural değil, yalnız iki yerde beliriyor: kart
    harcaması borcu yazıp ödemeyi sonraya bırakıyor (Bölüm 3), Bluecoins'in cari hesabı
    faturayı gelire yazıp tahsilatı sonraya bırakıyor (Bölüm 4). Sıradan bir gelir ya da
    gider kaydında ise Bölüm 1'de görüldüğü gibi kayıt …"

- [ ] **25. s.91 · 9.9 çıkarım, karşı taraf · Ç**
  - Eski: "Canlı beş üründe kayıt kimseye ait değil … Wallet'ın borç nesnesi bir istisna ama
    o da ayrı bir ekranda yaşıyor, kaydın kendisinde değil."
  - Yeni: "Canlı ürünlerde sıradan kayıt kimseye ait değil … Wallet'ın borç nesnesi ve
    Bluecoins'in cari hesabı istisna, ama ikisinde de karşı taraf kaydın kendisinde değil,
    ayrı bir nesnede ya da hesapta duruyor."

- [ ] **26. s.91 · 9.9 çıkarım, defter sayısı · Ç**
  - Eski: "Canlı beş üründe defter sayısı bir — Goodbudget'ın zarf katmanı hariç, ki o da
    ikinci bir defter değil, aynı paranın ikinci bir görünümü olmayı hedefliyor ve Bölüm
    1'de görüldüğü gibi ikisi tutmuyor."
  - Yeni: "Canlı ürünlerin dördünde defter sayısı bir. Goodbudget'ta iki — zarf ve hesap —
    ve Bölüm 1'de görüldüğü gibi ikisi tutmuyor."

- [ ] **27. s.91 · 9.9 çıkarım, KDV oranı · F**
  - Eski: "Bedeli sorumluluk: oran listesi ürünün içinde yaşıyor ve oran değişince rapor
    sessizce eskir. …"
  - Yeni: "Bedeli sorumluluk: oran listesi ürünün içinde yaşıyor. Oran değiştiğinde listeyi
    güncel tutmak ürüne ya da kullanıcıya düşüyor; güncellenmezse yeni belgeler eski oranla
    kesiliyor. …"
  - Neden: her fatura kendi oranını taşıyor; eski rapor eskimez.

- [ ] **28. s.92 · 9.9 çıkarım, muhasebeci erişimi · Ç**
  - Eski: "Canlı beş üründe böyle bir kavram yok; veri dışarı ancak bir dosyayla çıkıyor."
  - Yeni: "Canlı ürünlerde muhasebeciye ayrılmış bir erişim yok; veri dışarı bir dosyayla
    çıkıyor."

- [ ] **29. s.92 · 9.9 kazanç tablosu, KolayBi "kaybettirdiği" · B**
  - Eski: "Yalnız yüzey görüldü; hiçbir alanın kasaya, cariye veya rapora etkisi ölçülemedi"
  - Yeni: "Fatura her zaman bir cariye kesiliyor ve e-belge abonelikten ayrı, kontörle
    fiyatlanıyor; kayıt canlı ürünlerdeki tek satırdan uzun bir iş"

- [ ] **30. s.92 · 9.9 kazanç tablosu, Paraşüt "kaybettirdiği" · B**
  - Eski: "İç arayüz hiç görülmedi; elde yalnız mağaza karuseli ve tanıtım videosu
    kareleri var ve onlar da basılmıyor"
  - Yeni: "Kaynağa göre e-belge abonelikten ayrı bir modül ve kontörle fiyatlanıyor; kayıt
    ile ödeme iki ayrı adım olduğu için her gider iki kez ele alınıyor"

- [ ] **31. s.92 · 9.9 kazanç tablosu, Logo İşbaşı "kaybettirdiği" · B**
  - Eski: "İç arayüz hiç görülmedi; müşavirin yetki sınırı, iz katmanı, sesli faturanın
    onay adımı, çoklu depo ve verginin nerede toplandığı arandı, kaynakta yok. Ön muhasebe
    paketi tek başına e-fatura kesmiyor"
  - Yeni: "Ürünün kendi ifadesiyle üründe oluşturulan fatura kayıtlarının resmî değeri yok;
    e-fatura ayrı modülle geliyor. Müşavir müşteri adına işlem yapabiliyor ama yetki
    sınırı ve kaydı kimin yazdığını gösteren iz kaynakta tanımlı değil"

- [ ] **32. s.93 · 9.9 Belge 3 sorusu · Ç**
  - Eski: "… Üç kaynak ayırıyor, canlı beş ürün ayırmıyor ve ikisi de kendi kullanıcısı
    için tutarlı."
  - Yeni: "… Üç kaynak her kayıtta ayırıyor; canlı ürünler yalnız kartta ve caride
    ayırıyor, ve ikisi de kendi kullanıcısı için tutarlı."

## Bölüm 10 — Kapanış

- [ ] **33. s.97 ve s.98 · 10.3 ve 10.4 tabloları, Wallet · F** (7. bölümdeki iddianın
  kalan iki yeri)
  - s.97, eski: "Son on iki haftayı — varsayılan aralık takvim ayı değil" → yeni: "Kayan
    bir pencereyi — son 30 gün ya da 12 hafta; takvim ayı ayrıca seçiliyor"
  - s.98, eski: "Varsayılan rapor aralığı takvim ayı değil son on iki hafta; kart dönemsiz
    ve taksit bölünmüyor" → yeni: "Rapor aralığı takvim ayı değil kayan bir pencere; kart
    dönemsiz ve taksit bölünmüyor"

- [ ] **34. s.99 · 10.5 akış, adım 3 · Ç**
  - Eski: "… için bir kapı var." → yeni: "… ürüne göre kendiliğinden, onayla ya da elle."
  - Dayanak E0244 → E0244 · E0291 · E0430.
  - Neden: 5.2'ye göre Money Manager'da kapı yok.

- [ ] **35. s.99 · 10.5 akış, adım 4 · Ç**
  - Eski: "Tek sayı veriliyor; tanımı ve dönemi ekranda yazmıyor."
  - Yeni: "Tek sayı veriliyor; neyi saydığı ekranda yazmıyor."
  - Neden: Wallet dönemi yazıyor (LAST 30 DAYS / 12 WEEKS).

- [ ] **36. s.100 · 10.6 sentez · Ç + F**
  - Eski: "Dokuz bölümde ölçülen bütün farklar tek bir soruya bağlanıyor …" → yeni:
    "Dokuz bölümde ölçülen farkların çoğu — 10.1'de dokuz ayrımın yedisi — tek bir soruya
    bağlanıyor …"
  - Eski: "… kazancı yanlış okumanın imkânsız hâle gelmesi." → yeni: "… kazancı yanlış
    okumanın zorlaşması."
  - Eski: "Bu belgenin en çok tekrarlanan bulgusu bu ve dokuz bölümün her birinde başka
    bir konuda çıktı." → yeni: "Bu belgenin en çok tekrarlanan bulgusu bu: kart, borç ve
    rapor bölümlerinde üç ayrı konuda çıktı."
  - Tablo hücresi, eski: "Yanlış okuma neredeyse imkânsız hâle geliyor" → yeni: "Yanlış
    okuma zorlaşıyor: toplamın neyi içerdiği satır adında yazılı"

---

## Test sonucu (23 Eylül, ikinci oturum)

Ölçüt: metinler çıkarımdır. Kare bir şeyi birebir yazmıyor diye iddia "fazla" sayılmadı.
Her madde için önce karelerden bağımsız bir çıkarım yapıldı, sonra metinle karşılaştırıldı.
Değişiklik önerisi yalnız çıkarımın görünenle ya da belgenin başka bir sayfasıyla
çeliştiği yerde yapıldı. **Kullanıcı kararıyla A, B, C ve D uygulandı** (C, 7.3'teki Şekil
7.10'un satırına da: "Net Kazanç: Varlıklar 43.850, Cari hesap −500 (kart borcu)"). C'deki
isteğe bağlı ay ayrımı uygulanmadı. Bölüm 1, 3, 7 ve 10 yeniden üretildi; `tam/` henüz üretilmedi.

36 maddenin 33'ü çıkarım olarak tutuyor. Üç yerde öneri var, bir yerde de isteğe bağlı bir
dayanak eklemesi:

- **A · Madde 1 (s.11), bloğun değişmeyen ilk cümlesi.** "Money Manager'ın Ağustos
  pastasında iki gider tek başlık altında 'Diğer %58,5' olarak birleşiyor." E0231'de Diğer
  diliminde tek bir gider var: Mavi Yazılım, 1.200 (1.200 / 2.050 = %58,5). Bu bir çıkarım
  değil, sayım hatası. Bloğun vardığı sonuç ise geçerli: pasta kaydın ne olduğunu
  söylemiyor, üstelik "Diğer" adını Ada Reklam geliri de taşıyor (E0228).
  Öneri: "Money Manager'ın Ağustos pastasında Mavi Yazılım gideri yalnız 'Diğer %58,5'
  olarak okunuyor; aynı 'Diğer' adını Ada Reklam geliri de taşıyor."
- **B · Madde 7 (s.33), Goodbudget.** Gözlem formu: ücretsiz pakette **toplam bir hesap**
  açılabiliyor. Kart hesabı, tek hak Ana Hesap'a harcandığı için açılamadı. "Kart hesabı
  ücretsiz pakette açılamıyor" ifadesi bu durumu genelliyor. Benim çıkarımım: banka
  hesabı ile kart aynı anda tutulamıyor. Öneri: "Ücretsiz pakette tek hesap açılabiliyor;
  banka hesabının yanına kart eklenemediği için kart kullanan biri bu pakette kart borcunu
  ayrı izleyemiyor."
- **C · Madde 19 (s.71), Bluecoins.** "(varlık 43.850, cari −500)". Karedeki etiket
  "Cari hesap", ama −500 kart borcunun kalanı (−1.000'dan 500 ödeme; gözlem formundaki
  net zinciri). Bu belgede "cari" karşı taraf demek (Bölüm 4), bu yüzden okur −500'ü Ada
  Reklam'ın carisi sanabilir. Öneri: "(Varlıklar 43.850; ekranın 'Cari hesap' satırında
  kalan kart borcu −500)". Aynı karede Ağustos sütununda iki blok da 21.350 gösteriyor,
  yalnız Eylül'de ayrılıyorlar (0 ve 43.350). İstenirse cümle ay adıyla yazılabilir.
- **D · İsteğe bağlı dayanak.** 7.7, 7.8 ve 10.3'teki "takvim ayı ayrı bir sayfadan
  seçiliyor" iddiası, Spending sayfasının "This month" gösterdiği E0425/E0439 karelerinden
  çıkıyor, ama dayanakta yalnız E0284 · E0280 yazıyor. E0439 eklenebilir.

Tutan ve değişiklik gerektirmeyen, üzerinde durulan maddeler:

- **5.** Bluecoins taksidi elle kaydediliyor (BC-X2 kullanıcı kontrolü).
- **9.** Money Manager'ın taksitleri "önceden yazılmış kayıt gibi". Saklama biçimi
  bilinmiyor (B12), ama satırlar ay listesinde duruyor, toplama giriyor ve kart bakiyesi
  geleceğe yürüyor; çıkarım makul.
- **12.** Bluecoins'in etiket listesinde İş ve Kişisel var (E0088).
- **13.** Exclude from stats'ın etkisi adından çıkarılıyor.
- **24.** Bluecoins'in carisi Bölüm 4.3 ile tutarlı.
- **7.5 tablosu.** "Varsayılan aralık takvim ayı değil" hücresi: iki karede de kayan bir
  pencere var; düzeltilmiş ifadeyle çelişmiyor.

## E · Bluecoins'in "Cari hesap" satırı (kullanıcı kontrolüyle çözüldü)

Kullanıcı emülatörde gördü: Hesaplar ekranının üst başlıkları Banka, Nakit, Cari hesap,
Kredi kartı, İpotekler; Ada Reklam carisi Cari hesap başlığının altında. E0049 ve E0055'te
bu başlıklar iki bölüme ayrılıyor: **VARLIKLAR** (Banka, Nakit) ve **CARİ HESAP** (Cari
hesap, Kredi Kartı). Bu, C'deki "kalan kart borcu" okumasını tamamlıyor. Rapordaki "Cari
hesap" satırı, cari ile kartın birlikte toplandığı bölümün toplamı; −500'ün kart olması o
anda carinin 0 olmasından geliyor. Belge 4, Bluecoins'teki alacağın "varlık toplamının
içinde" durduğunu söylüyordu; kare bunun tersini gösteriyor. Düzeltilenler:

| Yer | Eski | Yeni |
|---|---|---|
| 4.1 Şekil 4.1 satırı | Bakiyesi diğer hesapların yanında duruyor. | Grubu, kredi kartıyla aynı CARİ HESAP bölümünde. |
| 4.3 Şekil 4.12 satırları | Varlık 22.350 → 43.850 · Cari −1.000 → −500; Ada Reklam carisi 0 · Cari hesaplar varlık toplamında ayrı satır. | VARLIKLAR 22.350 → 43.850 · CARİ HESAP −1.000 → −500: cari 0, kart −500 · Cari, varlıklarda değil, kartla aynı bölümde. |
| 4.7 çıkarım | Karşılığında alacak, varlık toplamının içinde sıradan bir hesap gibi duruyor; "elimdeki para" ile "tahsil edeceğim para" aynı listede yan yana. | Karşılığında alacak hesap listesinde duruyor, ama VARLIKLAR bölümünde değil: kredi kartı ve ipotekle aynı CARİ HESAP bölümünde, yani borç tarafında. Net değere giriyor, varlık toplamına girmiyor; "tahsil edeceğim para" elimdeki paranın değil, kartın yanında okunuyor. (Dayanağa E0049 ve kullanıcı kontrolü eklendi.) |
| 4.7 kazanç, Bluecoins "kaybettirdiği" | Tahsil edilmemiş para varlık listesinde gerçek parayla yan yana duruyor; cari hesap formunda vade alanı yok | Tahsil edilmemiş alacak kartla aynı bölümde, borç tarafında duruyor; varlık toplamı onu göstermiyor ve cari hesap formunda vade alanı yok |
| 4.7 Belge 3 sorusu | Bluecoins ikisini yan yana koyuyor, Wallet ayrı bir ekrana alıyor. | Bluecoins ikisini aynı listede tutuyor ama alacağı kartla aynı bölüme koyuyor; Wallet ayrı bir ekrana alıyor. |
| 7.3 Şekil 7.10 satırı | Net Kazanç: Varlıklar 43.850, Cari hesap −500 (kart borcu). | Net Kazanç: Varlıklar 43.850, Cari hesap −500. |
| 7.3 sağ not | … varlık, banka, nakit ve cari satırlarını ayrı ayrı veriyor. | … VARLIKLAR ve CARİ HESAP bölümlerini, altındaki grup ve hesaplarla ayrı ayrı veriyor. |
| 7.8 çıkarım | … 43.350 okundu (Varlıklar 43.850; ekranın "Cari hesap" satırında kalan kart borcu −500). | … 43.350 okundu: Varlıklar 43.850 ile "Cari hesap" −500'ün toplamı. "Cari hesap" satırı Bluecoins'in cariyi ve kartı birlikte topladığı bölüm; o anda içindeki tek tutar kart borcuydu. |

Bölüm 4 ve 7 yeniden üretildi, sayfa sayıları değişmedi. Gözlem formuna
(`gozlemler/bluecoins.md`, 23 Eylül bölümü) satır olarak işlendi.
