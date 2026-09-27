# Eksik listesi — Belge 1 · Bölüm 3 · Gezinme ve ana ekran

16 Eylül 2026 · v2. Bölüm metninde koşum kaydıyla yazılan veya "görülmedi" kalan ifadeler. Her satırdaki ekran görüntüsü gelirse ilgili ifade kareyle doğrulanır; gelmezse metin olduğu gibi kalır ve bölüm bu hâliyle geçerlidir.

Kural: mevcut test verisi silinmez veya sıfırlanmaz. Yeni kurulum, oturum kapatma veya yeni hesap gerektiren işler bu yüzden önerilmez olarak işaretlendi.

| No | Uygulama | Eksik | Metindeki yeri | Nasıl tamamlanır | Öncelik |
|---|---|---|---|---|---|
| E1 | Money Manager | İlk açılışın ekran görüntüsü yok; "karşılama yok" kullanıcı beyanıdır | 3.1 | Temiz kurulum gerekir; mevcut veriyi etkiler | Önerilmez |
| E2 | Wallet | İlk açılış ve kayıt/giriş ekranı görülmedi | 3.1 | Oturum kapatma veya yeni kurulum gerekir; bulut hesabı etkilenir | Önerilmez |
| E3 | Wallet, Goodbudget | Boş ana ekran görülmedi | 3.5 | Yeni hesap veya hane gerekir | Önerilmez |
| E4 | Hesap Defterim | Birden fazla defter varken açılışta hangi defterin seçili geldiği bilinmiyor | 3.2, 3.4 | Uygulamayı son uygulamalardan tamamen kapatıp açın; ilk ekranın görüntüsü. İsterseniz başka bir defter seçip aynı işlemi tekrarlayın | Orta |
| E5 | Bluecoins | Sol menüdeki iki "Hesaplar" kaleminin hangi ekranlara gittiği | 3.2 | Menüyü açıp her "Hesaplar" kalemine ayrı ayrı dokunun; açılan iki ekranın görüntüsü | Orta |
| E6 | Goodbudget | Zarftaki iki sayıdan hangisinin kalan, hangisinin bütçelenen olduğu karede ayırt edilemiyor | 3.4 | Yeni kayıt girmeden mevcut ENVELOPES ekranının görüntüsü; iki sayının farklılığı tek başına anlamı kanıtlamaz; açıklama/yardım veya izlenmiş değişim kaydı gerekir | Orta |
| E7 | Bluecoins | Veri varken uygulamanın hangi sekmeyle açıldığı görülmedi | 3.4 | Uygulamayı tamamen kapatıp açın; ilk ekranın görüntüsü | Orta |
| E8 | Wallet | Home'un alt kısmı (Balance Trend, Upcoming planned payments) yalnız Tur 1 karesinde ve yükleniyor görünümünde | 3.4 | Home'u en alta kadar kaydırın; yüklenmiş kartların görüntüsü | Düşük |
| E9 | KolayBi, Paraşüt | Panoların bugünkü sürümü ve gerçek davranışı doğrulanmadı; kareler demo veya tanıtım çizimi | 3.2–3.4 | Uygulamaya giriş gerekir (web ücretli, şirket bilgisi istiyor); daha yeni bir resmî destek sayfası veya tanıtım videosu karesi bulunursa eklenebilir | Erişim yok |

Görüntüleri `kanitlar/<uygulama>/` klasörüne istediğiniz adla bırakmanız yeterli; yeniden adlandırma ve envanter kaydı bir sonraki adımda yapılır.

## v2 sınırları

- E10 · Dört + düğmeli ürün: varsayılan tür ve tür değiştirme gerekliliği doğrulanmadı (3.3). Metin seçeneklerin görünürlüğüyle sınırlıdır.
- E11 · Goodbudget: LATER ve alternatif kurulum yolları denenmedi (3.1). Zorunluluk iddiası kaldırıldı; yeni hane açılmadı.
- Money Manager gezinme dayanağı mevcut E0231, E0236, E0254 ile tamamlandı; yeni koşum yapılmadı. İfade incelenen dört ana bölümü kapsar.
