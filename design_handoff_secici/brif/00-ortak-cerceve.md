# Ortak çerçeve — her brifle birlikte ver

Bu projede daha önce Özet, İşlemler, İşlem ekle, Kasa (KasaV4), Diğer ve Hesabım'ı birlikte tasarladık
(`Son Tasarim.html`). Yeni ekranlar **aynı dilin devamıdır**: aynı token'lar, aynı bileşenler, aynı
yoğunluk. Yeni renk, boşluk, yarıçap ya da tipografi token'ı ekleme.

## Ürün

- Esnaf ve şahıs şirketi için **işletme bütçe uygulaması**; ön muhasebe değildir. Ekranlarda KDV, matrah,
  beyanname, fatura kesme, "vergi hesabı" ve **"kâr"** kelimesi geçmez. Uygulama hiçbir vergi tutarını
  hesaplamaz; tutarı kullanıcı yazar.
- İşletme ve şahsi para **tek havuzdadır**; kapsam (`İşletme` / `Şahsi`) yalnız gelir/gider raporunu böler.
  Bakiye, kart borcu ve net varlık kapsamdan etkilenmez.
- Bir kayıt ya **tanır** (gelir/gider yazar, bakiyeye dokunmaz) ya **taşır** (bakiyeyi değiştirir,
  gelir/gider yazmaz). Transfer, kart borcu ödemesi, POS parasının bankaya geçmesi "taşır"dır; ekranda gelir
  ya da gider gibi görünmemelidir.
- Hiçbir şey silinmez: yanlış kayıt **iptal edilir** ya da **geri alınır**. Her yeni akışın bir geri alma
  yolu olmalı ve onay metni sonucu söylemeli (ör. *"Kayıt silinmez; iptal edildi olarak işaretlenir ve
  toplamları artık etkilemez."*).

## Görsel kurallar (Son Tasarim'dan)

- 412 dp genişlik; sayfa zemini `--canvas`, kart beyaz, 1 px `--border`, yarıçap 20, gölge yok.
- Başlık sayfanın kendisinden (26/700). Alt çubuk: Özet · İşlemler · [+] · Kasa · Diğer; sayfaların kendi
  FAB'ı yok, sayfaya özel ekleme başlıktaki ikon ya da bölüm başlığındaki `+ Ekle` ile.
- **Alttan açılan paneller telefonun en altından açılır ve alt çubuğun üstüne biner.**
- Para her zaman `AppMoneyText`: `₺23.185,00`, iki basamak. Gelir `+` yeşil, gider `−` kırmızı, taşıma
  (transfer, yatış, yoldaki para) mavi ve **işaretsiz**.
- Durum: satır ve kart köşesinde dolgusuz `AppStatusTag` (ikon 16 + metin 13/600). Dolgulu `AppStatusChip`
  yalnız sayfadaki tek önemli durum için. Dar yerde kapsül kullanma (Son sayımlar'daki taşma dersi).
- Bölüm başlığı sağında `AppTextAction` (`+ Ekle`, `Tümü ›`); kart başlığı `AppCardHead` (52 dp).
- Panel içi etiket–değer bloğu `AppDetailBlock`; eksik bilgi notu `AppInlineNotice` (nötr mavi, hata gibi
  değil); seçim rayı `AppSegmentRail`; form paneli `AppFormSheet`.
- Tarihler Türkçe ve okunur (`29 Eylül Pazartesi`, `30 Eyl`); tutar alanında binlik ayırıcı (`23.100`).
- Ekran başına **tek birincil eylem**.
- Bilgi yalnız renkle taşınmaz; her dokunulabilir öğe en az 48 dp; **2.0× yazı ölçeğinde taşma olmaz**
  (başlık + eylem satırları alt satıra inebilmeli).

## Veri ve metin

- Ekrandaki toplamları sunucu verir; formdaki canlı önizleme (komisyon, net, fark) yalnız gösterimdir.
- Etiket ve cümleler kısa; bir şeyin *neden* öyle olduğunu bir cümleyle söyleyen kural metni kullanılabilir
  (Kasa'daki *"Sayım bir gözlemdir; kaydetmek bakiyeyi değiştirmez."* gibi).
- Örnek veriler sentetiktir.

## Çizilecek hâller

Her ekranın **dolu** hâli ve **ilk kullanım / boş** hâli çizilir. Yükleniyor, hata ve yetkisiz hâller mevcut
`AppStateView` ailesiyle çözülür; çizmen gerekmez.

## Teslim

- Yeni çerçeveleri ayrı bir sayfada topla; her çerçeve bir ekran ya da bir durum.
- Emin olmadığın ya da bir ürün kararı gerektiren yerde **tek çözüm uydurma**: iki seçeneği yan yana çiz,
  hangisini önerdiğini ve nedenini yaz. Benim seçimimden sonra son hâli kaydet.
- Bitince önceki paket gibi bir teslim klasörü: README (ekran ekran ölçü ve davranış) + ekran görüntüleri.
