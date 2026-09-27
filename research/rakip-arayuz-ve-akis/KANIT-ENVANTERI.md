# Kanıt envanteri

## P0.1 — Dosya listesi ve iskelet, 13 Eylül 2026

Durum: P0.1 envanteri oluşturuldu. Bu paket yalnız yerel dosya sayımı, sabit
kimlik, yol, boyut ve SHA-256 kaydıdır; görseller açılmadı, MD iddiaları
doğrulanmadı, emülatör çalıştırılmadı. Geçmişteki 11 görsel karşılaştırması
13 Eylül denetim raporunun kapsamıdır; bu tabloya yeni içerik onayı aktarılmadı.

Başlangıç: **390 dosya = 357 PNG + 23 Markdown + 10 diğer dosya**.
Bu envanterin eklenmesiyle beklenen toplam **391 dosya, 24 Markdown** olur.
Hash ve boyutlar envanter çıkarıldığı andaki dosya içeriğidir; aynı turda
güncellenen plan/durum/giriş belgelerinde eski anlık değer korunur. Hash bir
doğruluk veya görsel inceleme işareti değildir. Bu dosyanın kendine referanslı
hash'i tutulmaz; E0391 yönetim kaydıdır.

## Kullanım ve sonraki doldurma

- E kimlikleri sabittir. Dosya taşınırsa aynı kimliğin yolu güncellenir;
  eski yol not edilir. Yeni dosyaya yeni kimlik verilir; liste yeniden numaralanmaz.
- Bütün yollar bu araştırma köküne göredir; bağlantılar tıklanabilir.
- Tüm içerik durumu bu P0.1 turu için **incelenmedi**. Dosya adı içerik sayılmaz.
- Görsel paketlerinde her E kimliği için ayrıntı kaydı eklenir: gerçek ekran/içerik,
  platform/paket/sürüm, kaynak URL/türü, yakalama ve yayın tarihi, işlem dönemi,
  inceleme tarihi, ilgili MD bölümü, bulgu kimliği ve kullanım rolü/gerekçesi.
  Şimdilik bu alanlar **atanmadı/bilinmiyor**; dosya tarihinden türetilmez.
- Kanıt etiketi araştırma README'sindeki tek kaynaktan alınır. Teknik içerik
  inceleme durumu ile kanıt etiketi aynı şey değildir.
- Görsellerde kaynak incelendikçe ana anlatım / kanıt eki / arşiv kararı verilir.
  Yönetim dosyaları ve .gitkeep dosyaları ürün davranışına kanıt değildir.
- Dış URL kaynaklarının tam dökümü P0.3 MD içerik haritalamasında çıkarılır;
  P0.1 yalnız yerel dosya kapsamıdır.
- Görsel paketlerinin tam E kimliği aralıkları P0.4'te E0392 bulgu kaydında
  belirlendi (17 G paketi, 357 PNG). Henüz görsel paketi başlamadı.
  BULGU-DOGRULAMA-KAYDI.md P0.2'de E0392 olarak eklendi.

## Uygulama görsel sayıları

| Uygulama | PNG |
|---|---:|
| bluecoins | 90 |
| goodbudget | 28 |
| hesap-defterim | 45 |
| kolaybi | 39 |
| logo-isbasi | 6 |
| money-manager | 30 |
| parasut | 9 |
| quickbooks | 4 |
| wallet-budgetbakers | 106 |
| **Toplam** | **357** |

## Sabit kimlikli yerel dosyalar

Bütün satırlar için içerik incelemesi: **P0.1'de yapılmadı**.

| Kimlik | Dosya | Bayt (başlangıç) | SHA-256 (başlangıç) |
|---|---|---:|---|
| E0001 | [.gitignore](.gitignore) | 299 | `723037d73f65b7f225b2f1f83a542f4d5e4fe0a060a925e252b0083c3cf4ec93` |
| E0002 | [denetim.sh](denetim.sh) | 3812 | `d39818eb81fff26e6e38b9a792605913475788cff1e01c217576bd443422d473` |
| E0003 | [DURUM.md](DURUM.md) | 56765 | `6846b71ac7e5d6cc9c4f540196f0116a21646bd6a4160c94d0764c39aa76553a` |
| E0004 | [FAZ7-8-UYGULAMA-PLANI.md](FAZ7-8-UYGULAMA-PLANI.md) | 27907 | `5ec3dcf25a91f8a8192a40bec5d9cc3291e3299b7a74587ed6a6d5b93a8fbc11` |
| E0005 | [gozlemler/bluecoins.md](gozlemler/bluecoins.md) | 33552 | `b9eed5f684d8bd69f1eb019472eab57794a7867d2ecd1e1c126b09e869f9a059` |
| E0006 | [gozlemler/goodbudget.md](gozlemler/goodbudget.md) | 32073 | `02a036673d455696d12d388ed1625e4ad239701f4e758ee22ce17b872d2437b7` |
| E0007 | [gozlemler/hesap-defterim.md](gozlemler/hesap-defterim.md) | 48856 | `789f19dbb779373615f50303c0dae8f9691e3873f111c85384056e4d3dcfc68f` |
| E0008 | [gozlemler/kolaybi.md](gozlemler/kolaybi.md) | 82082 | `b0233d1959691fc9644ca925500baa2aff48e8412d13ea04f686b56ec8221944` |
| E0009 | [gozlemler/logo-isbasi.md](gozlemler/logo-isbasi.md) | 21908 | `cee51ae09da8194a354c0549b78e5f4d4d371a308638e731dd4382cef1d07f4f` |
| E0010 | [gozlemler/money-manager.md](gozlemler/money-manager.md) | 34540 | `e9f6449a4b73c76a40cfd8bd559ff63ea3822dce8306574f7895d8566c7bf7ec` |
| E0011 | [gozlemler/parasut.md](gozlemler/parasut.md) | 25547 | `ddcc29b4deb610efaaee3f325972a78724cceb693e66d04a9285928a70631eeb` |
| E0012 | [gozlemler/quickbooks.md](gozlemler/quickbooks.md) | 17818 | `9870e7d8bdb8f22111ad8262aee00e98f8df987239911c9d55a2b1bf83950bf2` |
| E0013 | [gozlemler/README.md](gozlemler/README.md) | 319 | `5f3775ff7833c3040a7409fc9fcfe406078023c09ae13bc3f8c3dcafebd93fa5` |
| E0014 | [gozlemler/wallet-budgetbakers.md](gozlemler/wallet-budgetbakers.md) | 35167 | `e424d5e4fff1c71b35e07a1e182fba25c80a3e5946af63bc1e3e6b6e4bbefb9b` |
| E0015 | [kanitlar/bluecoins/.gitkeep](kanitlar/bluecoins/.gitkeep) | 2 | `7eb70257593da06f682a3ddda54a9d260d4fc514f645237f5ca74b08f8da61a6` |
| E0016 | [kanitlar/bluecoins/00-magaza.png](kanitlar/bluecoins/00-magaza.png) | 232950 | `74b599ac9db51ca0b228780f7180c440f2aefc3a9bfa185c84dbeef8f07cf9dc` |
| E0017 | [kanitlar/bluecoins/01-ilk-acilis.png](kanitlar/bluecoins/01-ilk-acilis.png) | 216635 | `4838a0f0e720ef8ca9acfad07ba6e17276999565b138f520f4ccd6220a8b82f8` |
| E0018 | [kanitlar/bluecoins/02-bos-ana-ekran.png](kanitlar/bluecoins/02-bos-ana-ekran.png) | 136366 | `a37a124db839c994560c6feb12932353e47d37620260a28642127a88959b0c65` |
| E0019 | [kanitlar/bluecoins/03-dolu-ana-ekran.png](kanitlar/bluecoins/03-dolu-ana-ekran.png) | 104269 | `5768beb4401d9b5c0ce48c26abeb3373255b15745742010f210c3e5bfe541b2e` |
| E0020 | [kanitlar/bluecoins/04-islem-formu.png](kanitlar/bluecoins/04-islem-formu.png) | 115388 | `98709457bd0119ace1b1bd984ed3cea7a6235c05cbceb510f46805927bcfb657` |
| E0021 | [kanitlar/bluecoins/05-siniflandirma.png](kanitlar/bluecoins/05-siniflandirma.png) | 103094 | `41c7df480f60b4217b2ee6f770b6b7a5adfb3190399f3ee628034d8e7182f53e` |
| E0022 | [kanitlar/bluecoins/06-islem-listesi.png](kanitlar/bluecoins/06-islem-listesi.png) | 313902 | `88564021011316d20cd472de0ee19fe951309f06a06a23a8c0d734a70ee794a4` |
| E0023 | [kanitlar/bluecoins/07-rapor.png](kanitlar/bluecoins/07-rapor.png) | 148995 | `71b3c0f1c57a72aefc081b0ba4e255a3fda71d72554ade893fb11e8325df6664` |
| E0024 | [kanitlar/bluecoins/08-hata-veya-bos-durum.png](kanitlar/bluecoins/08-hata-veya-bos-durum.png) | 90878 | `9f01da0081768a789fe1974e3df3ae96a24a4affa13def0713e51a682705f431` |
| E0025 | [kanitlar/bluecoins/09-ozgun-ozellik.png](kanitlar/bluecoins/09-ozgun-ozellik.png) | 151227 | `ccbd296cddf9aa297f9b99de5cebbfeae020cae04ac2bf63d8af94f518800c7e` |
| E0026 | [kanitlar/bluecoins/10-fresh-bos-ana-ekran.png](kanitlar/bluecoins/10-fresh-bos-ana-ekran.png) | 93328 | `edb512bccba3b910a22460dbe81eebb908340b6a85d4f85bc471cf18fcd9f90f` |
| E0027 | [kanitlar/bluecoins/11-uc-hesap-kuruldu.png](kanitlar/bluecoins/11-uc-hesap-kuruldu.png) | 144983 | `079fb9a22658747525752e8c5086dc2a1f5ebc84ae938ed1c129acb4d3097879` |
| E0028 | [kanitlar/bluecoins/12-kart-gideri-taksit-alani.png](kanitlar/bluecoins/12-kart-gideri-taksit-alani.png) | 106970 | `6a102c75c8573c165fbbd84da82a7bc4596cb0f9e6d0b49775f624a55bc517c1` |
| E0029 | [kanitlar/bluecoins/13-transfer-formu.png](kanitlar/bluecoins/13-transfer-formu.png) | 99900 | `4f674c3324db09316070f3b215bf1095cd07d40a20a935217667ffe2724ac0fb` |
| E0030 | [kanitlar/bluecoins/14-islem-listesi-running-bakiye.png](kanitlar/bluecoins/14-islem-listesi-running-bakiye.png) | 232615 | `bc00d4c95637d85604c5c001fbb34cd14031b0993bf7aa20a0152fade6c6f458` |
| E0031 | [kanitlar/bluecoins/15-cekirdek-5-islem-tamamlandi.png](kanitlar/bluecoins/15-cekirdek-5-islem-tamamlandi.png) | 278474 | `aa58770ef436ed0f08ec2e4ae2ede6e687c9aa569eee99b62a55cab92b4abca9` |
| E0032 | [kanitlar/bluecoins/16-kontrol-degerleri-net-kazanc-44950.png](kanitlar/bluecoins/16-kontrol-degerleri-net-kazanc-44950.png) | 180782 | `37ce6d603c4dfb2b6ea6bac8001fc90adffee4a59c3293605843bbd932e9c31a` |
| E0033 | [kanitlar/bluecoins/17-b2-taksit-sartlari-sheet.png](kanitlar/bluecoins/17-b2-taksit-sartlari-sheet.png) | 119430 | `5fd44049be3782cadcc6370ad58954d14e44ed797521ea05da4e1a4426e96dbb` |
| E0034 | [kanitlar/bluecoins/18-b2-taksit-6ay-15agu.png](kanitlar/bluecoins/18-b2-taksit-6ay-15agu.png) | 122640 | `f46f3ba25217caabe15c427195ed09832fc3b650e71c650200cf57161fded437` |
| E0035 | [kanitlar/bluecoins/19-b2-6ay-hatirlatici-metni.png](kanitlar/bluecoins/19-b2-6ay-hatirlatici-metni.png) | 131043 | `c2ff16a53c41313245528592046a0a601acb132b7e5b27363efe7ff34e859bb2` |
| E0036 | [kanitlar/bluecoins/20-b2-1-6-taksit-1000-kayit.png](kanitlar/bluecoins/20-b2-1-6-taksit-1000-kayit.png) | 302997 | `eba7ba39a401b2070fae8cb1fefafd42dbdd30a0261926a0ff9cd76559c6d75d` |
| E0037 | [kanitlar/bluecoins/21-b2-kalan-5-taksit-hatirlatici.png](kanitlar/bluecoins/21-b2-kalan-5-taksit-hatirlatici.png) | 181365 | `1f9081cdeead076dd9b3c1b941b3acc1976dcb4861db7313901ae660aa5252ad` |
| E0038 | [kanitlar/bluecoins/22-b2-sonrasi-rapor-gider-3050.png](kanitlar/bluecoins/22-b2-sonrasi-rapor-gider-3050.png) | 180355 | `ab4b6905438d6b58d11a941a5cea67d29e8970db4432b120f8b968ffc453a560` |
| E0039 | [kanitlar/bluecoins/23-b1-planli-islem-aylik-sheet.png](kanitlar/bluecoins/23-b1-planli-islem-aylik-sheet.png) | 128030 | `0b5f28ad47f8fdc7c073b342ae41c7a4eb3e53aaf1bca0425c525e701d6ddc75` |
| E0040 | [kanitlar/bluecoins/24-b1-yenilenen-islem-banner.png](kanitlar/bluecoins/24-b1-yenilenen-islem-banner.png) | 121027 | `ef8e608ca58125ed8ffbc038a8b19cf3eebe5fc4e15be3e48ffa075f171d9a45` |
| E0041 | [kanitlar/bluecoins/25-b1-hatirlatici-gecikmeli-bugun.png](kanitlar/bluecoins/25-b1-hatirlatici-gecikmeli-bugun.png) | 275429 | `cb97af6cfa7e411d55445e282b8f43a8ab8ebf0e467a5348efb036e312ce021f` |
| E0042 | [kanitlar/bluecoins/26-b1-hatirlatici-detay-kaydet.png](kanitlar/bluecoins/26-b1-hatirlatici-detay-kaydet.png) | 117552 | `a3c8f27c8776a9c13b9610941067ec60de4702967c67d24b5ac8de38ddccf94f` |
| E0043 | [kanitlar/bluecoins/27-b1-islem-olarak-kaydet-bugun-mu.png](kanitlar/bluecoins/27-b1-islem-olarak-kaydet-bugun-mu.png) | 122949 | `cf083f207cb7cb8ccb98b3762615820993ada451ffbbe740eba5e6e714aca572` |
| E0044 | [kanitlar/bluecoins/28-b1-onay-sonrasi-rapor-gider-3650.png](kanitlar/bluecoins/28-b1-onay-sonrasi-rapor-gider-3650.png) | 175003 | `3eef3f793045a91480108abda013804dc18621a8685e50ee39219e583ea67154` |
| E0045 | [kanitlar/bluecoins/29-bolmek-split-modu.png](kanitlar/bluecoins/29-bolmek-split-modu.png) | 111041 | `51a98bd20cecc51d22889b59785b0738f20d529d95ca315261e682c0f39b0b58` |
| E0046 | [kanitlar/bluecoins/30-bagimsiz-hatirlatici-bir-kez-program.png](kanitlar/bluecoins/30-bagimsiz-hatirlatici-bir-kez-program.png) | 111495 | `1e9f8f51c6be7c756c9771b70563d193f49e3c78c1f9dc00c3ec074b3bf7b59e` |
| E0047 | [kanitlar/bluecoins/31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png](kanitlar/bluecoins/31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png) | 271095 | `a43b7c34c05d70c0acacda6f47bc9868ca5144cec100126e0d2598aa2a5a5107` |
| E0048 | [kanitlar/bluecoins/32-kart-hesap-kesim-gunu-limit-alanlari.png](kanitlar/bluecoins/32-kart-hesap-kesim-gunu-limit-alanlari.png) | 125931 | `123cb6c6e1ef2c684ca04573e91bdab970d6facbd80da65a7a89c1840285abb7` |
| E0049 | [kanitlar/bluecoins/33-cari-hesap-olusturuldu.png](kanitlar/bluecoins/33-cari-hesap-olusturuldu.png) | 150121 | `918101f321e7bdc9544ec7795b7c3b2abac2b7fae7e9c906bfca8e25426074e0` |
| E0050 | [kanitlar/bluecoins/34-kismi-kart-odemesi-500-transfer.png](kanitlar/bluecoins/34-kismi-kart-odemesi-500-transfer.png) | 312698 | `71a2be83c39d24474f03bc1ca34fbd50e588d148ba6155901287509bbb0418fd` |
| E0051 | [kanitlar/bluecoins/f7-00-baslangic.png](kanitlar/bluecoins/f7-00-baslangic.png) | 377936 | `d10ec73846e466694a679c0c5d998266321b3b96fbd4c2a95bcbd65407446439` |
| E0052 | [kanitlar/bluecoins/f7-01-hesaplar-scroll.png](kanitlar/bluecoins/f7-01-hesaplar-scroll.png) | 157483 | `23557f634e4d3bb1de963e7e466ac1cfa2fdc254c48d9f163ae9ce64f2710d8e` |
| E0053 | [kanitlar/bluecoins/f7-02-hesaplar-scroll2.png](kanitlar/bluecoins/f7-02-hesaplar-scroll2.png) | 186483 | `b32956ab4acb9da649d6e8ce55e49f65c257b3c69f72cc58d3bd1a0254fbf5cf` |
| E0054 | [kanitlar/bluecoins/f7-03-hesap-listesi.png](kanitlar/bluecoins/f7-03-hesap-listesi.png) | 189216 | `9a1e1de9f43dd3cb2319edbdebc6f764c0aa33b6028e9aa8ab49364fa2f1bee6` |
| E0055 | [kanitlar/bluecoins/f7-04-tum-hesaplar.png](kanitlar/bluecoins/f7-04-tum-hesaplar.png) | 174063 | `272550488e8863d393004dff1e6c2fc799afbdaa581e19ef331d77af99b927ff` |
| E0056 | [kanitlar/bluecoins/f7-05-hatirlaticilar.png](kanitlar/bluecoins/f7-05-hatirlaticilar.png) | 266067 | `fda3de10e08b9b5b342a5c55ca6171da2ffdbe010390e5675a4fe5cc743517c4` |
| E0057 | [kanitlar/bluecoins/f7-06-ofis-kirasi-detay.png](kanitlar/bluecoins/f7-06-ofis-kirasi-detay.png) | 266092 | `3b26581a318a111a79921249ae967cfe29cc790b4a3c42e7fba5e408baa72a47` |
| E0058 | [kanitlar/bluecoins/f7-07-tap-icon.png](kanitlar/bluecoins/f7-07-tap-icon.png) | 266075 | `eab8e3c68800ac2f991c6ff0d3bcc950d60793ccff67d21eac032265cae4e5cd` |
| E0059 | [kanitlar/bluecoins/f7-08-tap-retry.png](kanitlar/bluecoins/f7-08-tap-retry.png) | 265673 | `4c53804250f1f5a77c8b44cca404e84c1e3adf9687fd2dbe82c5b2ef87b37d85` |
| E0060 | [kanitlar/bluecoins/f7-09-ofis-kirasi-dialog.png](kanitlar/bluecoins/f7-09-ofis-kirasi-dialog.png) | 238927 | `df64fe1bffba0d71c8c87bb100d3253a914b522881222d769f6d0238ab5622a0` |
| E0061 | [kanitlar/bluecoins/f7-10-ofis-kirasi-kaydet-sonuc.png](kanitlar/bluecoins/f7-10-ofis-kirasi-kaydet-sonuc.png) | 188739 | `b43db1e42bb7acf3d7512f3a974479b97d7a211a59f400a54b312954dc097c62` |
| E0062 | [kanitlar/bluecoins/f7-11-kaydet-dogru.png](kanitlar/bluecoins/f7-11-kaydet-dogru.png) | 183873 | `ddfe9771172cc47b886a051994a6ad933004c61b4a642066bc7fb188dee2a9cd` |
| E0063 | [kanitlar/bluecoins/f7-12-ofis-kirasi-realized.png](kanitlar/bluecoins/f7-12-ofis-kirasi-realized.png) | 266025 | `347541ebd3778dd2591829c82a19d9f28123838f988b28496f60d95486238e5c` |
| E0064 | [kanitlar/bluecoins/f7-13-islemler.png](kanitlar/bluecoins/f7-13-islemler.png) | 329076 | `28c61175b872062b8097d8f8280efa19d7495ffa7e43fe6735e8a4b0bc9bff3f` |
| E0065 | [kanitlar/bluecoins/f7-14-islemler-guncel.png](kanitlar/bluecoins/f7-14-islemler-guncel.png) | 312056 | `d748216a794f7565bf26d2b80d0cab1f7c9763d825d848fe79f9de7211606f47` |
| E0066 | [kanitlar/bluecoins/f7-15-yeni-islem.png](kanitlar/bluecoins/f7-15-yeni-islem.png) | 311979 | `8bf1a5e9d5b90133c2743ff35482662d4b6c981dc83bb0e7cdbd96b4c506a128` |
| E0067 | [kanitlar/bluecoins/f7-16-fab-tap.png](kanitlar/bluecoins/f7-16-fab-tap.png) | 120118 | `03bc426e0cc3c7aefd5e9513c2cf7cbbad3d4eb96ae4c791547e0aaf64e35c08` |
| E0068 | [kanitlar/bluecoins/f7-17-isim-girildi.png](kanitlar/bluecoins/f7-17-isim-girildi.png) | 131085 | `3c10c1291456d05f30329d164de4b83b132bed1fce13c711488ad8bbc453775e` |
| E0069 | [kanitlar/bluecoins/f7-18-gelir-tutar.png](kanitlar/bluecoins/f7-18-gelir-tutar.png) | 131611 | `2c75e317e41b864226248ee7f56989a09fb621e74329cb252d076b425dbbed79` |
| E0070 | [kanitlar/bluecoins/f7-19-tutar-girildi.png](kanitlar/bluecoins/f7-19-tutar-girildi.png) | 132971 | `f03b167762e78155e8679534a1086d49402662260a5c58d786aafbffafc16087` |
| E0071 | [kanitlar/bluecoins/f7-20-hesap-secim.png](kanitlar/bluecoins/f7-20-hesap-secim.png) | 70574 | `a5dd209097f34a31e750f811b4736614552771a5657ad89a4e0456798fa3c52b` |
| E0072 | [kanitlar/bluecoins/f7-21-hesap-secici.png](kanitlar/bluecoins/f7-21-hesap-secici.png) | 102082 | `864c1b94d401210a259cbcd5df04cf6d7ac26c5350796de373dfa6312d7edc21` |
| E0073 | [kanitlar/bluecoins/f7-22-hesap-secildi.png](kanitlar/bluecoins/f7-22-hesap-secildi.png) | 107815 | `671383c6d758e0b5d6877d4989eb92782b023f231bfb2cb3b76e52019157b2df` |
| E0074 | [kanitlar/bluecoins/f7-23-hesap-dogru-secildi.png](kanitlar/bluecoins/f7-23-hesap-dogru-secildi.png) | 109145 | `0b07b81e1eeba033288985c65259c9ce003be8d1cefa69106cbecd063f60f267` |
| E0075 | [kanitlar/bluecoins/f7-24-d2-kaydedildi.png](kanitlar/bluecoins/f7-24-d2-kaydedildi.png) | 313105 | `91b8eef9cd165396c2dd6d494efe3c1ef9232b2f912b3aec5968a71ed113a2bf` |
| E0076 | [kanitlar/bluecoins/f7-25-transfer-formu.png](kanitlar/bluecoins/f7-25-transfer-formu.png) | 122477 | `8e01a0dce88197f36d1e328c64670c62c413f22a2ff6265c979c7fd25c326e96` |
| E0077 | [kanitlar/bluecoins/f7-26-check.png](kanitlar/bluecoins/f7-26-check.png) | 130264 | `78cf4cdf5e2a960763cddf372955a52988612151ae8968660ce0342bea47aace` |
| E0078 | [kanitlar/bluecoins/f7-27-transfer-hazir.png](kanitlar/bluecoins/f7-27-transfer-hazir.png) | 108288 | `6a9e82d1719e6d850feb4bbff38642a1146cbc1bf9ba08eb31aaafb5c59fc16f` |
| E0079 | [kanitlar/bluecoins/f7-28-d3-kaydedildi.png](kanitlar/bluecoins/f7-28-d3-kaydedildi.png) | 323590 | `5f7f11e65fa73e92494555baba55a975964e59f77c32c1885c455d9ce20d63e5` |
| E0080 | [kanitlar/bluecoins/f7-29-arama.png](kanitlar/bluecoins/f7-29-arama.png) | 325233 | `aad2f85b29cccde4bede32742cb01aea85b2763bfb5b15f0450cda8d285ceee9` |
| E0081 | [kanitlar/bluecoins/f7-30-arama2.png](kanitlar/bluecoins/f7-30-arama2.png) | 128674 | `bdd1479ab02a7291ca4615d378f5a3b3a704745e8b1a62c2870c31b0563d9f20` |
| E0082 | [kanitlar/bluecoins/f7-31-filtre.png](kanitlar/bluecoins/f7-31-filtre.png) | 140879 | `f4d787a3fdb710c3f309157a2fa49e2b8828b97c7eadb2f84e48b9417049b6ef` |
| E0083 | [kanitlar/bluecoins/f7-32-menu.png](kanitlar/bluecoins/f7-32-menu.png) | 140207 | `91b58366bc0750d44814551c27fef3b751ea93abfe35db3e04301022d898a9c2` |
| E0084 | [kanitlar/bluecoins/f7-33-menu2.png](kanitlar/bluecoins/f7-33-menu2.png) | 209240 | `899edfe2fef9cb5f210784d185a3d9ac01022928c1c56eef6a1a9c74bacbd1fa` |
| E0085 | [kanitlar/bluecoins/f7-34-kategoriler.png](kanitlar/bluecoins/f7-34-kategoriler.png) | 149790 | `b3b0ef801f4c2bc19d93633b2757d320701dcf924ab804276a0260d134801bca` |
| E0086 | [kanitlar/bluecoins/f7-35-nakit-akim-ayari.png](kanitlar/bluecoins/f7-35-nakit-akim-ayari.png) | 101693 | `64aa7f1fab1e2a558c7f9a8681bbcd8e166e43b8fd017791aaba1a0fb6faa716` |
| E0087 | [kanitlar/bluecoins/f7-36-kategoriler2.png](kanitlar/bluecoins/f7-36-kategoriler2.png) | 125679 | `78252571f90285e8446fd8aab97796dc64bb57196846d8b2484251b6f854fd86` |
| E0088 | [kanitlar/bluecoins/f7-37-etiketler.png](kanitlar/bluecoins/f7-37-etiketler.png) | 66602 | `6a89e3f8bf3abfa038560e9cfa4d7401e0fe113343beee6c4701fc9f1b5497d3` |
| E0089 | [kanitlar/bluecoins/f7-38-cop-kutusu.png](kanitlar/bluecoins/f7-38-cop-kutusu.png) | 29260 | `c563cb8476914eccb9c57d0e12d8060eedd5a35d13ca04d07f77ea34ca468c1a` |
| E0090 | [kanitlar/bluecoins/f7-39-ayarlar.png](kanitlar/bluecoins/f7-39-ayarlar.png) | 254677 | `8bff9fa814a503e9d060ae657eb93f3cfaf3e179c11507b94b05767cfc14d35e` |
| E0091 | [kanitlar/bluecoins/f7-40-diger-ayarlar.png](kanitlar/bluecoins/f7-40-diger-ayarlar.png) | 65081 | `9675748f193d6ed8e7e064fdf08b136f2022570dbabb5c5d1089e4bf30a2ec27` |
| E0092 | [kanitlar/bluecoins/f7-41-diger-ayarlar2.png](kanitlar/bluecoins/f7-41-diger-ayarlar2.png) | 71849 | `06d3dfeb71179f295dccd46edf1e2a5b9584d6c22f46c54009e6ce5c45fbfc14` |
| E0093 | [kanitlar/bluecoins/f7-42-hesap-ayarlari.png](kanitlar/bluecoins/f7-42-hesap-ayarlari.png) | 105930 | `6467cc7e3055b1946f1660b1d85e4aaf3d3b6d2be4c522a1423c61601220eddf` |
| E0094 | [kanitlar/bluecoins/f7-43-hesap-ayarlari2.png](kanitlar/bluecoins/f7-43-hesap-ayarlari2.png) | 72636 | `c090002f0737e55c47d38aefa3335b9cda214314a25c49e465c80d3de1fbae58` |
| E0095 | [kanitlar/bluecoins/f7-44-gelismis-ayarlar.png](kanitlar/bluecoins/f7-44-gelismis-ayarlar.png) | 237323 | `bbb5e7464c1294407dd0886e8c5fdd290ef641fd49e2c1bd806ed2d94b8f9804` |
| E0096 | [kanitlar/bluecoins/f7-45-gelismis-scroll.png](kanitlar/bluecoins/f7-45-gelismis-scroll.png) | 250720 | `53d21e9b5bb946915a3c7d5224ebc1baa6e68b64239eda20d8c36668291e893a` |
| E0097 | [kanitlar/bluecoins/f7-46-check-nav.png](kanitlar/bluecoins/f7-46-check-nav.png) | 29186 | `cd51c2a8aaf35d1b99f72c98a37ac4565a1f1a6bb6feb26c57411785ab21e8c3` |
| E0098 | [kanitlar/bluecoins/f7-47-takvim.png](kanitlar/bluecoins/f7-47-takvim.png) | 140288 | `99e6c09f6f8af4e750e4a647fba2e1922d25d529b955a77e2ea039cc49981218` |
| E0099 | [kanitlar/bluecoins/f7-48-takvim-ayarlari.png](kanitlar/bluecoins/f7-48-takvim-ayarlari.png) | 140296 | `ffe5ddba19ba95f09fa109509d0f09066d8b28bec1fd0a7c9a70ce25955c571c` |
| E0100 | [kanitlar/bluecoins/f7-49-seyahat-modu.png](kanitlar/bluecoins/f7-49-seyahat-modu.png) | 139287 | `949377b513ff0a310af685053c9733aafc9448bb94dab4b01866515d913dcb67` |
| E0101 | [kanitlar/bluecoins/f7-50-seyahat-toggle.png](kanitlar/bluecoins/f7-50-seyahat-toggle.png) | 113256 | `775c15e75c20fe40bd4550dfc44accef30be6186b54722002296ad9cc2c3914d` |
| E0102 | [kanitlar/bluecoins/f7-51-check.png](kanitlar/bluecoins/f7-51-check.png) | 139205 | `cd8c443f6cc2d588a2abea01daacb69245a299bc3cd31c4c5984dc6118815d35` |
| E0103 | [kanitlar/bluecoins/f7-52-nav-check.png](kanitlar/bluecoins/f7-52-nav-check.png) | 582028 | `80f998c40e27536190df0c6e5502d502150dd00f5f1744d223081bde9c68552b` |
| E0104 | [kanitlar/bluecoins/f7-53-export.png](kanitlar/bluecoins/f7-53-export.png) | 141719 | `214266213b94b52aae16ab92632530d388c5b197047514fea08f25ae8811ab7a` |
| E0105 | [kanitlar/bluecoins/f7-54-print.png](kanitlar/bluecoins/f7-54-print.png) | 256918 | `13a9353d0181d98791e31b7249523a93b9c65aa464655cdfcedcf7137d37f0ba` |
| E0106 | [kanitlar/goodbudget/01-ilk-acilis.png](kanitlar/goodbudget/01-ilk-acilis.png) | 131206 | `6f8b4b1bc9f8d33db20687fd8d145548900fb734da23332d1e9f12dd229b913d` |
| E0107 | [kanitlar/goodbudget/02b-add-envelope-formu.png](kanitlar/goodbudget/02b-add-envelope-formu.png) | 56549 | `6ffea364e21a5612d6dbae1f608e364347f123b9b26a4c357f57e50791a0c22e` |
| E0108 | [kanitlar/goodbudget/02c-fill-envelopes-prompt.png](kanitlar/goodbudget/02c-fill-envelopes-prompt.png) | 150390 | `79106a8c58c71a84f0ae81e09faaabba8ab8a1fe4589e61606fae20d72ccb2d7` |
| E0109 | [kanitlar/goodbudget/02-setup-budget-envelope.png](kanitlar/goodbudget/02-setup-budget-envelope.png) | 118119 | `5afb12d0ee478835c75ccc5da8bdc721af2b67dece0b5e52f5c286f5897dc439` |
| E0110 | [kanitlar/goodbudget/03-register-household.png](kanitlar/goodbudget/03-register-household.png) | 114337 | `dd3f3226559bc23b009cd971643586b974362a603a0363e4296d8d3b861616a1` |
| E0111 | [kanitlar/goodbudget/04-accounts-off-by-default.png](kanitlar/goodbudget/04-accounts-off-by-default.png) | 130260 | `8e2dc96951f0aee36ee346eac1ab8c732e79c64b927ac364594e4ce25fb59a27` |
| E0112 | [kanitlar/goodbudget/05-edit-accounts-empty.png](kanitlar/goodbudget/05-edit-accounts-empty.png) | 63111 | `0f84b8160222fa138d74ae9405b838d72caefa516a57bea84469deb641781537` |
| E0113 | [kanitlar/goodbudget/06-ana-hesap-created.png](kanitlar/goodbudget/06-ana-hesap-created.png) | 70193 | `90d6bab37ff4f8e7f708e58937efec7958424c0325fec08d61a80a61a9e4ce0b` |
| E0114 | [kanitlar/goodbudget/07-account-limit-paywall.png](kanitlar/goodbudget/07-account-limit-paywall.png) | 109800 | `70c10c7f18c66d502a14fdf56b3993c538874e138a6a8b51b3bc3f53e8badba8` |
| E0115 | [kanitlar/goodbudget/08-envelopes-filled-home.png](kanitlar/goodbudget/08-envelopes-filled-home.png) | 110839 | `1ef58076869d452b444e121827e6656c73368c8e26c9665e753c206c353fba06` |
| E0116 | [kanitlar/goodbudget/09-credit-type-selected.png](kanitlar/goodbudget/09-credit-type-selected.png) | 145253 | `bd107e0187240cef429321ed7f2ec2de31a5070c8a520c6bb4f61c7a11564a70` |
| E0117 | [kanitlar/goodbudget/10-income-requires-envelope.png](kanitlar/goodbudget/10-income-requires-envelope.png) | 164561 | `485fae5b059a8bce5800a405b5a7ae6c823c15de8c56951396e367a2b001112d` |
| E0118 | [kanitlar/goodbudget/11-save-location-prompt.png](kanitlar/goodbudget/11-save-location-prompt.png) | 149638 | `248ffb1b64151f89faf40cf05e218c9352106dec597a4fd08223f238a332e71f` |
| E0119 | [kanitlar/goodbudget/12-account-transfer-screen.png](kanitlar/goodbudget/12-account-transfer-screen.png) | 88111 | `0e61035306e9ba13a21127252fa1ef6c774493a021f9e44a62898cceff598eca` |
| E0120 | [kanitlar/goodbudget/13-transfer-single-account-blocked.png](kanitlar/goodbudget/13-transfer-single-account-blocked.png) | 100084 | `afe20f40045ade5bf488e69cf8a4fcd8ea705d06d9500c429e9d9175b70d4198` |
| E0121 | [kanitlar/goodbudget/14-reports-default-current-month.png](kanitlar/goodbudget/14-reports-default-current-month.png) | 112572 | `348aa794a685464c5a08f529bee241e741563cd6aa248fd20f4a023587acdde9` |
| E0122 | [kanitlar/goodbudget/15-report-empty-state.png](kanitlar/goodbudget/15-report-empty-state.png) | 76057 | `2b9f3d896dba758b0c148eaf057e1789a6c47b4ec28f74c2dadc410daf524dea` |
| E0123 | [kanitlar/goodbudget/16-spending-by-envelope-negative-bug.png](kanitlar/goodbudget/16-spending-by-envelope-negative-bug.png) | 82970 | `a10732ee2063504dcf3ebb3a6a745afa8f18a5cfd8727af495d9a91d10b7aaff` |
| E0124 | [kanitlar/goodbudget/17-income-vs-spending-bug.png](kanitlar/goodbudget/17-income-vs-spending-bug.png) | 86348 | `5828d9885cbce879e54dfcdf74ef3c72bceb00b542e2b4b10b3cced96b34233a` |
| E0125 | [kanitlar/goodbudget/18-delete-confirmation.png](kanitlar/goodbudget/18-delete-confirmation.png) | 121699 | `3a59f831a19a0e24d22712771f6b6e2166ca1c713437d22986a977bc0f47deec` |
| E0126 | [kanitlar/goodbudget/19-recurring-schedule-turkish-dates.png](kanitlar/goodbudget/19-recurring-schedule-turkish-dates.png) | 185781 | `2dd0f021cff50af7f9f402d5ed176e921a5f93e882db483e999aa141086bee07` |
| E0127 | [kanitlar/goodbudget/20-search-results.png](kanitlar/goodbudget/20-search-results.png) | 70871 | `150bb8f0538d28f4cfd98780a9cbc1654eaccea7f8dba15b09155820b1285632` |
| E0128 | [kanitlar/goodbudget/21-no-receipt-attachment.png](kanitlar/goodbudget/21-no-receipt-attachment.png) | 153942 | `a9f9b223da7faa547ba042676e2712ee060a8e51d81c17c3a237787fbc55f73a` |
| E0129 | [kanitlar/goodbudget/22-final-balance-discrepancy.png](kanitlar/goodbudget/22-final-balance-discrepancy.png) | 103226 | `c0e55dba7a42adfeb2105a7af1208e4e258dccb417f9c7bfb9e7c47590c2023f` |
| E0130 | [kanitlar/goodbudget/23-transactions-initial-envelope-fill.png](kanitlar/goodbudget/23-transactions-initial-envelope-fill.png) | 138542 | `a14af9fa9b1025e611632a0f5c0eb739bf7216a0f6a2675120dd7fc78db3ec97` |
| E0131 | [kanitlar/goodbudget/24-accounts-balance-41734.png](kanitlar/goodbudget/24-accounts-balance-41734.png) | 98783 | `9611a1e83af437e8160ee9be7a339034ede1aff1dc791426892fa28565de496e` |
| E0132 | [kanitlar/goodbudget/25-arama-tek-bulut-satiri.png](kanitlar/goodbudget/25-arama-tek-bulut-satiri.png) | 67571 | `73eb14e5e17df8c35cd37b92ecd583f06807b64e6f205d432142050b84bfd14c` |
| E0133 | [kanitlar/goodbudget/26-schedule-frequency-listesi.png](kanitlar/goodbudget/26-schedule-frequency-listesi.png) | 202823 | `6e7a5ea3aeac95eedd803bad1075c77921c3a6498ca7364ae441f284ff95c694` |
| E0134 | [kanitlar/hesap-defterim/00-magaza.png](kanitlar/hesap-defterim/00-magaza.png) | 234232 | `86a5534e3ef98c63c009cf531da7cd86eaf1420b5030ef57579b6b60d17c6334` |
| E0135 | [kanitlar/hesap-defterim/01-ilk-acilis-hosgeldin.png](kanitlar/hesap-defterim/01-ilk-acilis-hosgeldin.png) | 179603 | `23549a0555801bef59aff3622d5be3e046b0ffb73ad9ee153a0bdfb5b1b9f1af` |
| E0136 | [kanitlar/hesap-defterim/02-bos-ana-ekran.png](kanitlar/hesap-defterim/02-bos-ana-ekran.png) | 158459 | `322cf04db5bb5dfff169373d7e4fe87f25b6e3f5cc8f538f5f58ef11ea3af6de` |
| E0137 | [kanitlar/hesap-defterim/03-dolu-ana-ekran.png](kanitlar/hesap-defterim/03-dolu-ana-ekran.png) | 175050 | `60ae2b5b1ffec9fc37f29dee484df694189148605795835ed15434c231d200c6` |
| E0138 | [kanitlar/hesap-defterim/04-islem-formu-alindi.png](kanitlar/hesap-defterim/04-islem-formu-alindi.png) | 97008 | `edc6ddaa68a417b92084cbbd5b7a3677ed20fdad051bdf11176950a3e9be7854` |
| E0139 | [kanitlar/hesap-defterim/05-siniflandirma-islem-adlari.png](kanitlar/hesap-defterim/05-siniflandirma-islem-adlari.png) | 162241 | `565df2105845cb7c8fd2280cc7e77dc0581087fefeaf319ee8dbaa19cc8e0ad2` |
| E0140 | [kanitlar/hesap-defterim/06b-islemler-butun-hesaplar.png](kanitlar/hesap-defterim/06b-islemler-butun-hesaplar.png) | 224669 | `2b493aed4982c88a1e525901167493b4391888c7bcf34f3faf9138e730577b22` |
| E0141 | [kanitlar/hesap-defterim/06-islem-listesi.png](kanitlar/hesap-defterim/06-islem-listesi.png) | 112904 | `8e35e60101fa41c5e6cc6b42bf3ac63ce5797429f899a3f37982e81a52c3ca09` |
| E0142 | [kanitlar/hesap-defterim/07-rapor-aylik-butun-hesaplar.png](kanitlar/hesap-defterim/07-rapor-aylik-butun-hesaplar.png) | 228127 | `437c6c53fd77baae1ba02cd943c3de4e80ecc073287c36c1b08509f6519ed09b` |
| E0143 | [kanitlar/hesap-defterim/08-silinmis-islemler.png](kanitlar/hesap-defterim/08-silinmis-islemler.png) | 57653 | `81dc241f7f78a1377918a9bbcb4b8439e78d4072b4c0b1810386d79de0f25dae` |
| E0144 | [kanitlar/hesap-defterim/09-ozgun-ozellik-takvim.png](kanitlar/hesap-defterim/09-ozgun-ozellik-takvim.png) | 132485 | `f5fb1bad7c17739954f2b98ead571bc29e8c670bd9d3f7c2bc978249ce83887a` |
| E0145 | [kanitlar/hesap-defterim/10-hesap-eklem-formu.png](kanitlar/hesap-defterim/10-hesap-eklem-formu.png) | 87017 | `9f303615860af471f829545df0ce568f740fdfcf6a32cf7fe285399cae82c8f6` |
| E0146 | [kanitlar/hesap-defterim/11-hesaplar-defterler-listesi.png](kanitlar/hesap-defterim/11-hesaplar-defterler-listesi.png) | 55781 | `27da642580ffafe239fbaf4460897557e08fb2719b3bc41e963549dfd0bf6390` |
| E0147 | [kanitlar/hesap-defterim/12-aktar-transfer-formu.png](kanitlar/hesap-defterim/12-aktar-transfer-formu.png) | 74695 | `3ec0e6e1e0add36dd9df0d700b44b7ec9bcdca8aab871f34d6352bdf436c8797` |
| E0148 | [kanitlar/hesap-defterim/13-ozet-tasarruf-agustos.png](kanitlar/hesap-defterim/13-ozet-tasarruf-agustos.png) | 91713 | `2c7aa49189ce04ade941aa86bdb88676f95606d2c5cf5c5619cabcde792131ea` |
| E0149 | [kanitlar/hesap-defterim/14-yedekleme-nag-dialog.png](kanitlar/hesap-defterim/14-yedekleme-nag-dialog.png) | 224001 | `ad9d45c8a69241442cde3ebfd60ade5123fc4ed5ec28cdcd8a41f5963defe445` |
| E0150 | [kanitlar/hesap-defterim/15-ayarlar.png](kanitlar/hesap-defterim/15-ayarlar.png) | 130125 | `96da4a2a4e075ff8f728ddea1465d2e627d8a5f53807c03e122e7f8a8c5db1e7` |
| E0151 | [kanitlar/hesap-defterim/16-kategori-alani-acik-form.png](kanitlar/hesap-defterim/16-kategori-alani-acik-form.png) | 93444 | `04b63e6d100350eaf18bcbdbf21615481d9d37027e82cb79417280bec2cb638e` |
| E0152 | [kanitlar/hesap-defterim/17-onceki-denge-gunluk-gorunum.png](kanitlar/hesap-defterim/17-onceki-denge-gunluk-gorunum.png) | 116264 | `6e2325f30adf0225e0eda1da05e6ccd2229651ed1d5c36d96c02de5b378f8d48` |
| E0153 | [kanitlar/hesap-defterim/18-kismi-odeme-is-karti.png](kanitlar/hesap-defterim/18-kismi-odeme-is-karti.png) | 159192 | `5d4434f4956b2d5b1e0b125322e6b6e8b8459471f0694f020e82640d12352cad` |
| E0154 | [kanitlar/hesap-defterim/19-transfer-bacagi-desync.png](kanitlar/hesap-defterim/19-transfer-bacagi-desync.png) | 192176 | `4a4a0c3e78620c8ddd449dc3561309e61c433a6f4fec6141fee75f80e94da21f` |
| E0155 | [kanitlar/hesap-defterim/20-oge-eklemek-dialog.png](kanitlar/hesap-defterim/20-oge-eklemek-dialog.png) | 145026 | `0f6983efc6c35d38439b6d25da3e6fd903917212cc7a99c0361e317339f12dc4` |
| E0156 | [kanitlar/hesap-defterim/21-oge-eklemek-tutar-notlar-otomatik.png](kanitlar/hesap-defterim/21-oge-eklemek-tutar-notlar-otomatik.png) | 144457 | `b0351641c745ade3222172d6a88d4779af33726117879cc9bc7cea1a4d9e3527` |
| E0157 | [kanitlar/hesap-defterim/22-fatura-ekle-secenekler.png](kanitlar/hesap-defterim/22-fatura-ekle-secenekler.png) | 164851 | `6c79bf462f19e41e9fc7d6d2a00f762ca8ad8f0bc06fbddc9e74c085f08a03c0` |
| E0158 | [kanitlar/hesap-defterim/24-kayit-eklendi-atac-ikonu.png](kanitlar/hesap-defterim/24-kayit-eklendi-atac-ikonu.png) | 209933 | `c9876ede83bd19dd245a02526797bb40615c9dcf6836ea13b886aec19ee84f02` |
| E0159 | [kanitlar/hesap-defterim/25-fatura-tam-ekran-goruntuleme.png](kanitlar/hesap-defterim/25-fatura-tam-ekran-goruntuleme.png) | 94121 | `712b2e3b9195896a5ab00c9f15074dc9d586971418c2b0901b9804e95de933e2` |
| E0160 | [kanitlar/hesap-defterim/26-islem-adlari-ozel-form.png](kanitlar/hesap-defterim/26-islem-adlari-ozel-form.png) | 207961 | `a95d55fa380fbbb25507098aef79765fbb67223877a69e64a0208c1152a9d116` |
| E0161 | [kanitlar/hesap-defterim/27-yeniden-adlandirilmis-basliklar.png](kanitlar/hesap-defterim/27-yeniden-adlandirilmis-basliklar.png) | 208086 | `c89a5cc3a07b2622d1ae89f7a36dabf071a8cfc328dee2889eff3cc82f9cd32e` |
| E0162 | [kanitlar/hesap-defterim/28b-bos-tutar-sessiz-red.png](kanitlar/hesap-defterim/28b-bos-tutar-sessiz-red.png) | 130375 | `9b346dc0aebc19117bd90beb5b6df84e416ecf1bae82c8c3b58ffed563b1b4b5` |
| E0163 | [kanitlar/hesap-defterim/28-sifir-tutar-kabul-edildi.png](kanitlar/hesap-defterim/28-sifir-tutar-kabul-edildi.png) | 203381 | `3b9e811d16091d32881b5e436578d9afebf7eaf514e965e2acbb7d11264a07d8` |
| E0164 | [kanitlar/hesap-defterim/29-arama-canli-filtre.png](kanitlar/hesap-defterim/29-arama-canli-filtre.png) | 144892 | `4030b6b8cd623ea15bd6221398973cea6bc89c1645adc3e81bf932aca33fe864` |
| E0165 | [kanitlar/hesap-defterim/30-not-defteri-checklist.png](kanitlar/hesap-defterim/30-not-defteri-checklist.png) | 84383 | `453c0fefb6056090df90c1fd1234a90db14e2e95a74af2c4a3f2fe9f036a0655` |
| E0166 | [kanitlar/hesap-defterim/31-nakit-hesap-makinesi.png](kanitlar/hesap-defterim/31-nakit-hesap-makinesi.png) | 140207 | `0d395bc5b4baf5b5adc014ebede63b2e6c684d3ac0ec1a8639dcdb0836bfe3ee` |
| E0167 | [kanitlar/hesap-defterim/32-silinmis-islemler-context-menu.png](kanitlar/hesap-defterim/32-silinmis-islemler-context-menu.png) | 94292 | `144ffe76a813d4405c090568cbb0f191db028982839d3110c38db62c2d851bf8` |
| E0168 | [kanitlar/hesap-defterim/33-kalici-silme-onay.png](kanitlar/hesap-defterim/33-kalici-silme-onay.png) | 104077 | `c09b61eaa9ea32efe538ec251fc4df9c54049b5d6f060a1746d6a806b49129d8` |
| E0169 | [kanitlar/hesap-defterim/34-bildiri-pdf-excel-secim.png](kanitlar/hesap-defterim/34-bildiri-pdf-excel-secim.png) | 189913 | `e34106c532d357f2f4dad2c355bb98c2ecafa0ef552534a2c15357c7b7b838c9` |
| E0170 | [kanitlar/hesap-defterim/35-kontrol-degeri-geri-yuklendi.png](kanitlar/hesap-defterim/35-kontrol-degeri-geri-yuklendi.png) | 224972 | `28c5a281f6f6dccc723ebe04a4c0333239c8e3530a5566c09c571d0f0b69341c` |
| E0171 | [kanitlar/hesap-defterim/36-drawer-menu-ust.png](kanitlar/hesap-defterim/36-drawer-menu-ust.png) | 141652 | `94ad925cfbf9cd5141fa2a1f4426aa1df0db8b380ad71d55c8dc4139bdbcb60c` |
| E0172 | [kanitlar/hesap-defterim/37-kontrol-degeri-guncel-47300.png](kanitlar/hesap-defterim/37-kontrol-degeri-guncel-47300.png) | 213435 | `db299cd6a466fd9a82dffe17c835627ba90229b256051eef96bbb38d7172cf22` |
| E0173 | [kanitlar/hesap-defterim/38-ortak-cuzdan-defteri.png](kanitlar/hesap-defterim/38-ortak-cuzdan-defteri.png) | 146674 | `2e463cb0bc5cb93225be409a56578e5f60b9c9dda8e2d1cd5c4c9479bca9a1ae` |
| E0174 | [kanitlar/hesap-defterim/39-gecis-reklami-interstitial.png](kanitlar/hesap-defterim/39-gecis-reklami-interstitial.png) | 1149167 | `6fd7609d768e1b60109a7270a8ced8cbdc58dc50a5929f47564618d011f75ae1` |
| E0175 | [kanitlar/hesap-defterim/40-bildiri-defter-basina-pdf-excel.png](kanitlar/hesap-defterim/40-bildiri-defter-basina-pdf-excel.png) | 203994 | `bb39f35c0c6c276e92c037ac24a32806da3d77e57582c46395cabd07a45df1e8` |
| E0176 | [kanitlar/hesap-defterim/41-bildiri-kasadefteri-uyarisi.png](kanitlar/hesap-defterim/41-bildiri-kasadefteri-uyarisi.png) | 201077 | `4063fd4ff1c6688e36d9875632c253d72bee437f5b2b494ee02c603482d40adc` |
| E0177 | [kanitlar/hesap-defterim/42-drawer-diger-uygulamalar-veresiye-gelirgider.png](kanitlar/hesap-defterim/42-drawer-diger-uygulamalar-veresiye-gelirgider.png) | 154781 | `53f8a9b3c1df19684669dc94c1b701bce5a4c17810fb352e7e5cff76ca3b06dd` |
| E0178 | [kanitlar/hesap-defterim/43-ayarlar-alt-bolum-donem-baslangici.png](kanitlar/hesap-defterim/43-ayarlar-alt-bolum-donem-baslangici.png) | 122939 | `c9d2acb37ff94b1dbcca376c0e83ad2537f1df8c688071b45eb1c17824d3308c` |
| E0179 | [kanitlar/kolaybi/.gitkeep](kanitlar/kolaybi/.gitkeep) | 2 | `7eb70257593da06f682a3ddda54a9d260d4fc514f645237f5ca74b08f8da61a6` |
| E0180 | [kanitlar/kolaybi/d32-giris-ekrani.png](kanitlar/kolaybi/d32-giris-ekrani.png) | 87416 | `f3acb3e10050e13333a3c3720424d0e88f974cf368ce693a53ecd6a8d0f064c3` |
| E0181 | [kanitlar/kolaybi/d33-video-guncel-durum-panosu.png](kanitlar/kolaybi/d33-video-guncel-durum-panosu.png) | 415324 | `121b681351cbea4b347765ddee5eff4a2aff9656eb85a73b4f5fffc0f7dc5133` |
| E0182 | [kanitlar/kolaybi/d34-video-gunu-gelen-islemler.png](kanitlar/kolaybi/d34-video-gunu-gelen-islemler.png) | 55222 | `c7bd3fce0c51e23431676e5e7042ca911cbff7e7bbd3fb3101e9e8c281b0975a` |
| E0183 | [kanitlar/kolaybi/d35-video-vadesi-belirsiz-tahsilatlar.png](kanitlar/kolaybi/d35-video-vadesi-belirsiz-tahsilatlar.png) | 443800 | `fe915a28717edfedb9808cb2cce24049fb2f5c6097affc86b6c4aaa138d2c821` |
| E0184 | [kanitlar/kolaybi/d36-video-cari-hesaplar.png](kanitlar/kolaybi/d36-video-cari-hesaplar.png) | 220193 | `8835fd732ef8f4b5a3a2aa3c3b2e71f53ccaf1acdbf161e57c24e6faa6db002b` |
| E0185 | [kanitlar/kolaybi/d37-video-urun-ve-hizmetler.png](kanitlar/kolaybi/d37-video-urun-ve-hizmetler.png) | 195575 | `425a31352d8f874a8fb91eb6b69d16941a4e71dbf55f54fe0029bda65ff6f215` |
| E0186 | [kanitlar/kolaybi/d38-video-finans-kasalar.png](kanitlar/kolaybi/d38-video-finans-kasalar.png) | 169636 | `b4ce0d62ae270c04359b3a0d4596ec645184b939d6e26370b5af24aa6f72501e` |
| E0187 | [kanitlar/kolaybi/d39-guncel-arayuz-2026.png](kanitlar/kolaybi/d39-guncel-arayuz-2026.png) | 396751 | `3418fc4b6b454183bba16ad56435fed1c2e1e6313b157ebb15aead6a48aa1846` |
| E0188 | [kanitlar/kolaybi/d01-destek-proje-listesi.png](kanitlar/kolaybi/d01-destek-proje-listesi.png) | 260272 | `f31647914d301a83ef48f04ca96bc4728619e41b309858e8313f96ff11e22fc8` |
| E0189 | [kanitlar/kolaybi/d02-destek-yeni-proje-formu.png](kanitlar/kolaybi/d02-destek-yeni-proje-formu.png) | 144345 | `fe640283c1040f34f7718b862cda583263642c12123c6707a3898cc1c12d0288` |
| E0190 | [kanitlar/kolaybi/d03-destek-proje-detay-ozet.png](kanitlar/kolaybi/d03-destek-proje-detay-ozet.png) | 207567 | `33fe04a4f16ed4d7410479214644abe450273fd523a4dac621fd0bb37080a37c` |
| E0191 | [kanitlar/kolaybi/d04-destek-proje-belge-kirilimi.png](kanitlar/kolaybi/d04-destek-proje-belge-kirilimi.png) | 234039 | `4def5153c1da201dc7ae95d778d8f732e13dadf4d2372f051d1650e8596534b1` |
| E0192 | [kanitlar/kolaybi/d05-destek-yeni-gider-formu.png](kanitlar/kolaybi/d05-destek-yeni-gider-formu.png) | 223768 | `7c4421f8e6b5455479d3ded9e1017da7f97305c58f1333dc44680dfefcc4fe59` |
| E0193 | [kanitlar/kolaybi/d06-destek-gider-tipleri.png](kanitlar/kolaybi/d06-destek-gider-tipleri.png) | 265240 | `1dca15d1e3c5b0d621ab8c0ee9f1dac06d26e2242bb2ab6a1159b40195e80f1c` |
| E0194 | [kanitlar/kolaybi/d07-destek-cari-listesi.png](kanitlar/kolaybi/d07-destek-cari-listesi.png) | 279621 | `a67f6c8298bebeaaf7297e2aa6e297c6b53519ec24d6c23eaf36657ed01a7818` |
| E0195 | [kanitlar/kolaybi/d08-destek-cari-olusturma-formu.png](kanitlar/kolaybi/d08-destek-cari-olusturma-formu.png) | 124940 | `872edf19cffcdc0c64dbcfc1108b12f9e6c20a44ad60746affd33db4bd9d7130` |
| E0196 | [kanitlar/kolaybi/d09-destek-cari-detay-ekstre-dialog.png](kanitlar/kolaybi/d09-destek-cari-detay-ekstre-dialog.png) | 254175 | `bef298af9b52dddaa7c167c8318527f9d1737e0a334e8a61f964c8b1ec600a8a` |
| E0197 | [kanitlar/kolaybi/d10-destek-cari-ekstre-onizleme.png](kanitlar/kolaybi/d10-destek-cari-ekstre-onizleme.png) | 253030 | `df266dac59bdb8aac8cb41fa1fea0bb41fee260b85d82ff7ea8976475ce86826` |
| E0198 | [kanitlar/kolaybi/d11-destek-gider-listesi.png](kanitlar/kolaybi/d11-destek-gider-listesi.png) | 286024 | `902918ba960833d4f0f7ec7b4212264fc38595b19728e0af8ae25f26d7e07ee0` |
| E0199 | [kanitlar/kolaybi/d12-destek-gider-detay-islemler.png](kanitlar/kolaybi/d12-destek-gider-detay-islemler.png) | 223550 | `54490fe54c9f17f271c7fede5c8f89e882e156c87ee32fcd2e346ad19a6c507b` |
| E0200 | [kanitlar/kolaybi/d13-destek-alis-faturasi-formu.png](kanitlar/kolaybi/d13-destek-alis-faturasi-formu.png) | 221693 | `c18af26d3fd433b17a0b7ac5112462336a4281d86a831d230adc17a27c291a7f` |
| E0201 | [kanitlar/kolaybi/d14-destek-personel-carileri.png](kanitlar/kolaybi/d14-destek-personel-carileri.png) | 227398 | `39696a398a8f08fdd6ca494dc20b6707940734a86b24eea58c819c510ed849db` |
| E0202 | [kanitlar/kolaybi/d15-destek-personel-cari-detay.png](kanitlar/kolaybi/d15-destek-personel-cari-detay.png) | 325345 | `71453e76c96e9a06faa058afdd171952db22bc0ed35a33b333805091f297dd79` |
| E0203 | [kanitlar/kolaybi/d16-destek-maas-odeme-secimi.png](kanitlar/kolaybi/d16-destek-maas-odeme-secimi.png) | 225251 | `24a51045e443cff1054afb2e7abd00381a636f9ad45dcd3574286532e361e9f7` |
| E0204 | [kanitlar/kolaybi/d17-destek-calisan-maasi-detay.png](kanitlar/kolaybi/d17-destek-calisan-maasi-detay.png) | 192847 | `7341f50592d09197685e28a20ef9bc0fed37ba9e1fc9d93c078192ee674c96f3` |
| E0205 | [kanitlar/kolaybi/d18-destek-tekrarli-maas-formu.png](kanitlar/kolaybi/d18-destek-tekrarli-maas-formu.png) | 149790 | `e93058ba313b190283f9b64b089972ecc802975c852f03b9e77509498a4a850f` |
| E0206 | [kanitlar/kolaybi/d19-destek-banka-hesaplari.png](kanitlar/kolaybi/d19-destek-banka-hesaplari.png) | 191398 | `d10712f82206ca65a69d8362ccf89812427831765451b7363cffc8ae925d45e2` |
| E0207 | [kanitlar/kolaybi/d20-destek-kredi-kartlari-listesi.png](kanitlar/kolaybi/d20-destek-kredi-kartlari-listesi.png) | 187381 | `0c6260cf1346f25646e78e13389c55163fd1a5739741f603b872c2863749ad42` |
| E0208 | [kanitlar/kolaybi/d21-destek-yeni-kredi-karti-formu.png](kanitlar/kolaybi/d21-destek-yeni-kredi-karti-formu.png) | 166252 | `ff28440fb6af1ec6c95532c8fe581b53f20d4821cd5fe492910c9a377f379fe8` |
| E0209 | [kanitlar/kolaybi/d22-destek-cekler.png](kanitlar/kolaybi/d22-destek-cekler.png) | 177639 | `b7ac3e15ab97b8c4ea220e547aef11bd8dd95868de66029f1ff44caaa3ae0455` |
| E0210 | [kanitlar/kolaybi/d23-destek-senetler.png](kanitlar/kolaybi/d23-destek-senetler.png) | 193150 | `d8484f422cf6c85f83278bcb22f76e0ab42c24240a63ed96650b2647b1a71bf2` |
| E0211 | [kanitlar/kolaybi/d24-destek-guncel-durum-panosu.png](kanitlar/kolaybi/d24-destek-guncel-durum-panosu.png) | 330801 | `dc5912acbbda779b87f2e9b86a139601925c0c2f309c0062ee70a8d3f680a137` |
| E0212 | [kanitlar/kolaybi/d25-destek-notlar-hatirlatici.png](kanitlar/kolaybi/d25-destek-notlar-hatirlatici.png) | 143207 | `4306d29fc027a339bee91e0c11739815116aa38c764ab97ac2acd41e55d4cfac` |
| E0213 | [kanitlar/kolaybi/d26-destek-alis-satis-raporu.png](kanitlar/kolaybi/d26-destek-alis-satis-raporu.png) | 275853 | `fe40fd8427dddcaac11a2fb98a21a8fa401cdeafd2e2671e195b232f8be2c9c5` |
| E0214 | [kanitlar/kolaybi/d27-destek-kdv-raporu.png](kanitlar/kolaybi/d27-destek-kdv-raporu.png) | 337570 | `26d6205444f7460c8848c61b846c6f35198d2be2c1f4ee8790fa8d999b093e9a` |
| E0215 | [kanitlar/kolaybi/d28-destek-gelir-gider-raporu.png](kanitlar/kolaybi/d28-destek-gelir-gider-raporu.png) | 227972 | `d5765a6eb49c55eaa9067e69730d1eae49cc02979be8b0549c2c5dabaa3395bb` |
| E0216 | [kanitlar/kolaybi/d29-destek-nakit-akis-raporu.png](kanitlar/kolaybi/d29-destek-nakit-akis-raporu.png) | 245924 | `9846c467e7c0c8e49a36514effd31483764cec5af5a647d49b40be169dcbaa63` |
| E0217 | [kanitlar/kolaybi/d30-destek-satis-fatura-formu.png](kanitlar/kolaybi/d30-destek-satis-fatura-formu.png) | 217816 | `b96f0a2942998f8b9a16adc6f3152cb0709aaa51ac3408319f7959688d79dc7b` |
| E0218 | [kanitlar/kolaybi/d31-destek-urun-varyantlar.png](kanitlar/kolaybi/d31-destek-urun-varyantlar.png) | 112802 | `5581be7e54574634754ae32132d45029d31aa9f0ea79f4fad73e081504905637` |
| E0219 | [kanitlar/logo-isbasi/.gitkeep](kanitlar/logo-isbasi/.gitkeep) | 2 | `7eb70257593da06f682a3ddda54a9d260d4fc514f645237f5ca74b08f8da61a6` |
| E0220 | [kanitlar/logo-isbasi/01-giris-ekrani.png](kanitlar/logo-isbasi/01-giris-ekrani.png) | 632266 | `92cf13ecdc4fcae05b95fce4a88dfee21b86db040978e9debef3bc5de85d62d9` |
| E0221 | [kanitlar/logo-isbasi/02-kayit-formu.png](kanitlar/logo-isbasi/02-kayit-formu.png) | 664441 | `ec2dbb5ed66205889a594eae949cd1141dd069ffc0490fe19268e8cee9de3f4b` |
| E0222 | [kanitlar/logo-isbasi/03-sektor-listesi.png](kanitlar/logo-isbasi/03-sektor-listesi.png) | 258071 | `eebbb865d9f747c2848a0ec0e852dcf86635f62d76fbb26e11cefb00ef30a903` |
| E0223 | [kanitlar/logo-isbasi/04-sozlesme.png](kanitlar/logo-isbasi/04-sozlesme.png) | 382436 | `251cd60adbee16f77f3ef9bd9d3d57f165350c08c6e132695a8d6c427c508b04` |
| E0224 | [kanitlar/logo-isbasi/05-video-entegrasyonlar.png](kanitlar/logo-isbasi/05-video-entegrasyonlar.png) | 3306760 | `c91c87f1a4d35b3f1972bdf9cab092789c6ea195925f5cdff8dd4bb986e5ce80` |
| E0225 | [kanitlar/logo-isbasi/06-video-musavir-portal.png](kanitlar/logo-isbasi/06-video-musavir-portal.png) | 3144760 | `b20090d43d0a8bd6260b57f82d9ca400c9194ac9d59ebf159b2158c44ceb5bbc` |
| E0226 | [kanitlar/money-manager/.gitkeep](kanitlar/money-manager/.gitkeep) | 2 | `7eb70257593da06f682a3ddda54a9d260d4fc514f645237f5ca74b08f8da61a6` |
| E0227 | [kanitlar/money-manager/02-bos-ana-ekran.png](kanitlar/money-manager/02-bos-ana-ekran.png) | 89134 | `39e206728de88ca3fa3d759ad35979c322aee3acd1818204049d5d88c872a77f` |
| E0228 | [kanitlar/money-manager/03-dolu-ana-ekran.png](kanitlar/money-manager/03-dolu-ana-ekran.png) | 184232 | `f082d8ca8d965d32c38314209e5d4d49403fb52a1f6393a8b570956939842902` |
| E0229 | [kanitlar/money-manager/04-islem-formu-ve-kategori.png](kanitlar/money-manager/04-islem-formu-ve-kategori.png) | 119170 | `993fe4c26fdd4d515fdafbefc49740c37e7fdec84702bccb1c35058d25ee21e4` |
| E0230 | [kanitlar/money-manager/06-islem-listesi-transfer-notr.png](kanitlar/money-manager/06-islem-listesi-transfer-notr.png) | 162372 | `5b61ee7b99ca1fcf1df56d08879d485ed9e46c427e578270872abc862ab24b56` |
| E0231 | [kanitlar/money-manager/07-rapor-agustos.png](kanitlar/money-manager/07-rapor-agustos.png) | 101909 | `94f1656eaf25137710bb06065369f954d7adaf8ad73bb723dd57c2a3edb99719` |
| E0232 | [kanitlar/money-manager/08-hata-toast-hesap-sec.png](kanitlar/money-manager/08-hata-toast-hesap-sec.png) | 98209 | `d80d9b7483fb0cf9d27a9ea04109ff6d34a750c680e4dd1ff5e916740d057a3d` |
| E0233 | [kanitlar/money-manager/09-kart-hesap-ekstre-modeli.png](kanitlar/money-manager/09-kart-hesap-ekstre-modeli.png) | 105414 | `967d9adb56e5659cb7dc0e22be431deef3a9d33b3831b4abe23020d45e6554cc` |
| E0234 | [kanitlar/money-manager/10-takvim-gorunumu.png](kanitlar/money-manager/10-takvim-gorunumu.png) | 111759 | `fd1db058703eeed7b9d9a1b8a36acc14371e189b334e00613377b0f7b6737532` |
| E0235 | [kanitlar/money-manager/11-hesaplar-final-kontrol-degerleri.png](kanitlar/money-manager/11-hesaplar-final-kontrol-degerleri.png) | 95880 | `aa89c71709e1ca336e24877a3bd982e951ad0ee72bdace9ddbc71d82542ea387` |
| E0236 | [kanitlar/money-manager/12-hesaplar-kart-borcu-bu-ay.png](kanitlar/money-manager/12-hesaplar-kart-borcu-bu-ay.png) | 98307 | `59b282f4e83fee3dff54bb2495042001d69c3cecfc8449cc845af5102db7ecdb` |
| E0237 | [kanitlar/money-manager/13-odeme-butonu-onfoldurulmus-havale.png](kanitlar/money-manager/13-odeme-butonu-onfoldurulmus-havale.png) | 79384 | `0803715335547288d5ee3dca84309064fbdea5caeb05da62fbc667029b932019` |
| E0238 | [kanitlar/money-manager/14-tekrarlama-secenekleri.png](kanitlar/money-manager/14-tekrarlama-secenekleri.png) | 87662 | `c0dfe24a734d42f1c685282d881565db600142613439b9ac01786649328c6515` |
| E0239 | [kanitlar/money-manager/15-acilis-bakiye-farki-dialog.png](kanitlar/money-manager/15-acilis-bakiye-farki-dialog.png) | 76374 | `df951d82637e2882349d88f97281cba8000cb22e383c600c8538ba7b2f8e8f74` |
| E0240 | [kanitlar/money-manager/16-acilis-bakiye-farki-defter.png](kanitlar/money-manager/16-acilis-bakiye-farki-defter.png) | 104869 | `cc92528d1ece4a2e1f8ff042cbfa09c15c56d6387e6967fedd4df799c3fcfa9f` |
| E0241 | [kanitlar/money-manager/17-tekrarlayan-aylik-form.png](kanitlar/money-manager/17-tekrarlayan-aylik-form.png) | 82109 | `ea053f5f0dc25db4a6b6d7b7351c71360bc7d66a4e08b2d85e2471f63a764e0a` |
| E0242 | [kanitlar/money-manager/18-tekrarlayan-agustos-liste.png](kanitlar/money-manager/18-tekrarlayan-agustos-liste.png) | 209414 | `41c19af7ac813802ca4db883202dc5184946854278b77d63d80811a7dee9664f` |
| E0243 | [kanitlar/money-manager/19-tekrarlayan-eylul-otomatik.png](kanitlar/money-manager/19-tekrarlayan-eylul-otomatik.png) | 93770 | `3b2d4bbadf990eee6b8a6e5bd3f48e7b05a7e2a51a159af29ba71991b3fc8977` |
| E0244 | [kanitlar/money-manager/20-tekrarlayan-ekim-onizleme.png](kanitlar/money-manager/20-tekrarlayan-ekim-onizleme.png) | 88008 | `4f6146da8c52df74e568a8dc3c76ef6baa655bbdfa22aa2f14cb8980f5f0f76d` |
| E0245 | [kanitlar/money-manager/21-taksit-6ay-form.png](kanitlar/money-manager/21-taksit-6ay-form.png) | 87872 | `bc5527c868ba600d36d1cd91b0352e7f7201dfdc3df7af0d18cd864f539daaa0` |
| E0246 | [kanitlar/money-manager/22-taksit-1-6-agustos.png](kanitlar/money-manager/22-taksit-1-6-agustos.png) | 217107 | `0e99069fcd73dff1292de493d69d8313de66c12489a92eb61d7110bfd1af5197` |
| E0247 | [kanitlar/money-manager/23-taksit-kart-borcu-bu-gelecek-ay.png](kanitlar/money-manager/23-taksit-kart-borcu-bu-gelecek-ay.png) | 96950 | `fc7991973c37248a380611ca1864d46682f3a6e8681f0fe3d18e59ea5f566cea` |
| E0248 | [kanitlar/money-manager/24-kismi-kart-odemesi-400.png](kanitlar/money-manager/24-kismi-kart-odemesi-400.png) | 77070 | `7f86909b29f9a82087fd1501d993611b9e9493c4ed62d1a774f8f67b138bc45f` |
| E0249 | [kanitlar/money-manager/25-kismi-odeme-sonrasi-borc.png](kanitlar/money-manager/25-kismi-odeme-sonrasi-borc.png) | 99153 | `fccfff898e93388b0678643ba1b9648ce1b0f530abf8d55bc340aee878a26c84` |
| E0250 | [kanitlar/money-manager/26-toplam-sekmesi-agustos.png](kanitlar/money-manager/26-toplam-sekmesi-agustos.png) | 156619 | `c101130074dbf1024a2fcb0a0ad16903244428756d420d7b3560f3e532969dc4` |
| E0251 | [kanitlar/money-manager/27-filtre-paneli-agustos-hesap.png](kanitlar/money-manager/27-filtre-paneli-agustos-hesap.png) | 130236 | `04cda8794263f3b7f2e13754498a4ee15d0892fcc19558df51afeada16a3a59e` |
| E0252 | [kanitlar/money-manager/28-toplama-dahil-et-anahtari.png](kanitlar/money-manager/28-toplama-dahil-et-anahtari.png) | 70200 | `70ec6413ffa894cee43a63549f3af120dc2d5825f01ccded384933e74e0decf1` |
| E0253 | [kanitlar/money-manager/29-toplama-dahil-kapali-net-varlik.png](kanitlar/money-manager/29-toplama-dahil-kapali-net-varlik.png) | 100367 | `a5919fd6af848de2acfaf5d223f264b01d47b79a828bc16fd1d29ec633d3aa8f` |
| E0254 | [kanitlar/money-manager/30-ayarlar-izgarasi.png](kanitlar/money-manager/30-ayarlar-izgarasi.png) | 92886 | `bbf378f9db079e75a8ef49bc88ee1ea02d7067a39caa117c495126fe0a298e5f` |
| E0255 | [kanitlar/money-manager/31-kart-defteri-agustos-hareketler.png](kanitlar/money-manager/31-kart-defteri-agustos-hareketler.png) | 164848 | `4d0cbc2d1d5df525ae84f08242fb30ad87b4611c3e7de51cc19214c717f4dae3` |
| E0256 | [kanitlar/money-manager/32-kart-defteri-eylul-taksit-2-6.png](kanitlar/money-manager/32-kart-defteri-eylul-taksit-2-6.png) | 142888 | `5ed7bcaa7b97cbc6a5fe970b1489fb0f2df088fa2ff144a66d69bbd3d4a731a7` |
| E0257 | [kanitlar/parasut/.gitkeep](kanitlar/parasut/.gitkeep) | 2 | `7eb70257593da06f682a3ddda54a9d260d4fc514f645237f5ca74b08f8da61a6` |
| E0258 | [kanitlar/parasut/01b-carousel.png](kanitlar/parasut/01b-carousel.png) | 65625 | `11a897182f7695d13f66181916d7bef5ce54981b82944da185935a0b4eface22` |
| E0259 | [kanitlar/parasut/01-ilk-acilis-carousel4.png](kanitlar/parasut/01-ilk-acilis-carousel4.png) | 81032 | `0e070d4574e4cd9c129054b5fc0a613f046c100deadd2b2c72afc7a344b2c681` |
| E0260 | [kanitlar/parasut/02-video-fatura-gonderme.png](kanitlar/parasut/02-video-fatura-gonderme.png) | 256689 | `c2b3fefb23d63520d05cd323c8279c24a021732b49eaee7485378c09d7a9dd64` |
| E0261 | [kanitlar/parasut/03-video-satis-faturasi-detay.png](kanitlar/parasut/03-video-satis-faturasi-detay.png) | 309912 | `1c17614c27a630f1a2ffce9a28ecb1536f42f95714fe472c4c560181451c01ec` |
| E0262 | [kanitlar/parasut/04-video-cari-hesap-durumu.png](kanitlar/parasut/04-video-cari-hesap-durumu.png) | 320384 | `c100a6b1254a47a7b76559682a89a6d3eda240813ba9f501af8de5e22e9e5883` |
| E0263 | [kanitlar/parasut/05-video-banka-entegrasyonu.png](kanitlar/parasut/05-video-banka-entegrasyonu.png) | 558756 | `8d854647cd61a111378420323b6442ea68136effd088251dbc3ce63988d75dfc` |
| E0264 | [kanitlar/parasut/06-video-stok-depo.png](kanitlar/parasut/06-video-stok-depo.png) | 248484 | `ee7b894c11bdd1d2deb1ddee0e7763bfa896b6e3f27e15604e5096abfc1a7cbd` |
| E0265 | [kanitlar/parasut/07-video-gelir-gider-raporu.png](kanitlar/parasut/07-video-gelir-gider-raporu.png) | 309369 | `88775b2e0b0b96a15a4f24ba78b970587481459d6bb7a1228d0c612093cfff4a` |
| E0266 | [kanitlar/parasut/08-video-nakit-akisi-raporu.png](kanitlar/parasut/08-video-nakit-akisi-raporu.png) | 306876 | `4e35cce54c4dcfc747b4db020c543460d8b30587ba2c2caf2c17e2cd6a84e70c` |
| E0267 | [kanitlar/quickbooks/.gitkeep](kanitlar/quickbooks/.gitkeep) | 2 | `7eb70257593da06f682a3ddda54a9d260d4fc514f645237f5ca74b08f8da61a6` |
| E0268 | [kanitlar/quickbooks/01-onboarding.png](kanitlar/quickbooks/01-onboarding.png) | 94528 | `ed8366cd9287ab41b577c60876a0f7d86f68137922b1132ff461c90279911351` |
| E0269 | [kanitlar/quickbooks/02-welcome-get-started.png](kanitlar/quickbooks/02-welcome-get-started.png) | 115028 | `783c6998b04dbefc87dbc6055451dd3a9707a2b8382c1e795eae29aa939de920` |
| E0270 | [kanitlar/quickbooks/03-onboarding-basic-info.png](kanitlar/quickbooks/03-onboarding-basic-info.png) | 73767 | `e4694caed10ce5444ea1aa0183ffda293601642b1b406f90885c7c180a1c5e4c` |
| E0271 | [kanitlar/quickbooks/04-choose-plan-paywall.png](kanitlar/quickbooks/04-choose-plan-paywall.png) | 131120 | `84f780f89db950fa6be861ddcb7270f9c194fe70314e573a811c8b6f43b1f058` |
| E0272 | [kanitlar/README.md](kanitlar/README.md) | 3703 | `313c4b8abe9c4bccc30ef76bb7e13d0bee92150badda2f16a142824ab8dd1a0c` |
| E0273 | [kanitlar/wallet-budgetbakers/.gitkeep](kanitlar/wallet-budgetbakers/.gitkeep) | 2 | `7eb70257593da06f682a3ddda54a9d260d4fc514f645237f5ca74b08f8da61a6` |
| E0274 | [kanitlar/wallet-budgetbakers/00-magaza.png](kanitlar/wallet-budgetbakers/00-magaza.png) | 99097 | `67aaec7b65243d9e3edbd63cd4b97dc7232c9bfd1fc09a90751d5dc8de96bbca` |
| E0275 | [kanitlar/wallet-budgetbakers/02b-menu.png](kanitlar/wallet-budgetbakers/02b-menu.png) | 156184 | `5d270ad7a3782c6161341713f275bee2061ba609806c0c42ece1dcf84f4a14f2` |
| E0276 | [kanitlar/wallet-budgetbakers/03-dolu-ana-ekran.png](kanitlar/wallet-budgetbakers/03-dolu-ana-ekran.png) | 205464 | `748ad90068419686c108484e0237641c9d89e594a2c3846da16252a78196ff73` |
| E0277 | [kanitlar/wallet-budgetbakers/04-islem-formu.png](kanitlar/wallet-budgetbakers/04-islem-formu.png) | 101071 | `394a14c018fcce672de97b9bbd7afdb9e506aa95d51fc1b0f4afedfd1bd60d15` |
| E0278 | [kanitlar/wallet-budgetbakers/05-siniflandirma.png](kanitlar/wallet-budgetbakers/05-siniflandirma.png) | 165310 | `8a6dccc4c6d869ffff9be1a537e7887b8ad94a5ef27e30c00e78e0523a952d59` |
| E0279 | [kanitlar/wallet-budgetbakers/06-islem-listesi.png](kanitlar/wallet-budgetbakers/06-islem-listesi.png) | 254977 | `eb5723aa1da7c29525f8b47948ec0dc6cb8b8744e3e272f39a6ecc6073522fcd` |
| E0280 | [kanitlar/wallet-budgetbakers/07-rapor.png](kanitlar/wallet-budgetbakers/07-rapor.png) | 160770 | `afebcf904a27d3fc7e630d845f4558a7e96072ee612037cb1ff7d5a0eef67ba6` |
| E0281 | [kanitlar/wallet-budgetbakers/08-hata-veya-bos-durum.png](kanitlar/wallet-budgetbakers/08-hata-veya-bos-durum.png) | 100630 | `10c0cfcc05871a57e802ad5f35870eef1b42b576c5b95d97e71249affbb73c6c` |
| E0282 | [kanitlar/wallet-budgetbakers/09-ozgun-ozellik.png](kanitlar/wallet-budgetbakers/09-ozgun-ozellik.png) | 87675 | `8ac0cde37cf85c00bd7422f4832255f7af2abbd0ac683c43e76f9366aa4f03af` |
| E0283 | [kanitlar/wallet-budgetbakers/10-kontrol-hesaplar.png](kanitlar/wallet-budgetbakers/10-kontrol-hesaplar.png) | 223013 | `7933fafd97f2daa3a052486a0387b15fa50b234a316f2c456a040192bbeb4df3` |
| E0284 | [kanitlar/wallet-budgetbakers/11-kontrol-cash-flow-agustos.png](kanitlar/wallet-budgetbakers/11-kontrol-cash-flow-agustos.png) | 157025 | `3ca8ba453671dd6257fac4b92442c1fedb5c12fd9653bfd4b37bda673970c228` |
| E0285 | [kanitlar/wallet-budgetbakers/12-kontrol-records-listesi.png](kanitlar/wallet-budgetbakers/12-kontrol-records-listesi.png) | 235824 | `c14853808e292435b13396ec3cd61d6afebd2f6c84c34e62bf14a9319626de07` |
| E0286 | [kanitlar/wallet-budgetbakers/13-planned-payments-bos-durum.png](kanitlar/wallet-budgetbakers/13-planned-payments-bos-durum.png) | 86091 | `f5c46d6f2e4ca3946eb5acf18a861661cd05e53db9926ff8742c9f8734012e29` |
| E0287 | [kanitlar/wallet-budgetbakers/14-planned-gecmis-tarih-kapali.png](kanitlar/wallet-budgetbakers/14-planned-gecmis-tarih-kapali.png) | 146402 | `156af5bd7d3c36f921200d08e5d70a2db212b7713731b6f44f99646e087f7ac2` |
| E0288 | [kanitlar/wallet-budgetbakers/15-b1-tekrarlayan-form.png](kanitlar/wallet-budgetbakers/15-b1-tekrarlayan-form.png) | 115066 | `2b29d8772ac6b74200deb15de38354e7d7d275273cf1ec1c4d24e7a8ff5f8c16` |
| E0289 | [kanitlar/wallet-budgetbakers/16-b1-plan-detay-due-today.png](kanitlar/wallet-budgetbakers/16-b1-plan-detay-due-today.png) | 86493 | `458e1453fc2ee47a55c7ca58132c0196aba3d9feafa091ee318b045eda4ac297` |
| E0290 | [kanitlar/wallet-budgetbakers/17-b1-confirm-payment-summary.png](kanitlar/wallet-budgetbakers/17-b1-confirm-payment-summary.png) | 118424 | `33eb23add1a80c616cde627512899c47cb9f38da59b9b5ba737b0e363cb81d5d` |
| E0291 | [kanitlar/wallet-budgetbakers/18-b1-otomatik-mi-onayli-mi-secimi.png](kanitlar/wallet-budgetbakers/18-b1-otomatik-mi-onayli-mi-secimi.png) | 166116 | `802c340aa020082c98fb33dbc0ea1afbde2588a442c922da8a5c012955a37350` |
| E0292 | [kanitlar/wallet-budgetbakers/19-b1-onay-sonrasi-paid-today-siradaki-pending.png](kanitlar/wallet-budgetbakers/19-b1-onay-sonrasi-paid-today-siradaki-pending.png) | 95604 | `12bc59ce369eb3470b56e145709d7d8e6d199384472c341c1695a46edc848a8a` |
| E0293 | [kanitlar/wallet-budgetbakers/20-b1-sonrasi-ana-hesap-20200.png](kanitlar/wallet-budgetbakers/20-b1-sonrasi-ana-hesap-20200.png) | 229435 | `8acfc573a7eebd7ec02633f56b15e9f9a20d21bbdbc72d0841d5312908377885` |
| E0294 | [kanitlar/wallet-budgetbakers/21-b2-islem-detay-taksit-alani-yok.png](kanitlar/wallet-budgetbakers/21-b2-islem-detay-taksit-alani-yok.png) | 96493 | `34db218a83fa470df0094489cee18b7860e3475da355576986c304067973b0f6` |
| E0295 | [kanitlar/wallet-budgetbakers/22-tarih-secici-onceki-ay-yok.png](kanitlar/wallet-budgetbakers/22-tarih-secici-onceki-ay-yok.png) | 160063 | `569493d19950b35567711ed7fbfb5d5b3c1404b6ed69ce8730cf81079039d048` |
| E0296 | [kanitlar/wallet-budgetbakers/23-b2-6000-tek-kart-borcu.png](kanitlar/wallet-budgetbakers/23-b2-6000-tek-kart-borcu.png) | 223263 | `645090ff4fe790877d1f674ba402fa86edeab932ca1421798ae0bc224e5f23bd` |
| E0297 | [kanitlar/wallet-budgetbakers/24-kart-hesap-detay-negatif-bakiye.png](kanitlar/wallet-budgetbakers/24-kart-hesap-detay-negatif-bakiye.png) | 183876 | `694eb5714e956eeec1a92a137f41ff1f9d0bd794737f269e1319372e4e2e05b3` |
| E0298 | [kanitlar/wallet-budgetbakers/25-kart-hesap-ayarlari.png](kanitlar/wallet-budgetbakers/25-kart-hesap-ayarlari.png) | 99706 | `1fc012a413dfd00285b86c8bc7f37e78dad6873e22ca7e0a9b4a6b65fd8c65ca` |
| E0299 | [kanitlar/wallet-budgetbakers/26-butce-olusturma-formu.png](kanitlar/wallet-budgetbakers/26-butce-olusturma-formu.png) | 86713 | `2f2f34e2ce650fa38a60a502206e6f27802bffc1f4ef31e53e044f85e340d8ea` |
| E0300 | [kanitlar/wallet-budgetbakers/27-butce-formu-dolu.png](kanitlar/wallet-budgetbakers/27-butce-formu-dolu.png) | 96026 | `215a16322a8a9035dd43121267bc4277faa0c33526f66c4e0224982bd94e5868` |
| E0301 | [kanitlar/wallet-budgetbakers/28-butce-olusturuldu-over-budget.png](kanitlar/wallet-budgetbakers/28-butce-olusturuldu-over-budget.png) | 117310 | `c15df9afc90d7c0fa60751e3ce4c0aa9ff58cb411059f005307ab5c0bbe11235` |
| E0302 | [kanitlar/wallet-budgetbakers/29-butce-detay-6600-harcama.png](kanitlar/wallet-budgetbakers/29-butce-detay-6600-harcama.png) | 156371 | `0e54e0cc5af8ad75c4e63873dedc05d155d3f0ad2280fc67c06331371abfbabc` |
| E0303 | [kanitlar/wallet-budgetbakers/30-goal-olusturma.png](kanitlar/wallet-budgetbakers/30-goal-olusturma.png) | 136960 | `9ed9140dfeb05ebfe1cb7e847ba8d5f1a677593a4a674f5af0014a9de154b00a` |
| E0304 | [kanitlar/wallet-budgetbakers/31-goal-detay-formu.png](kanitlar/wallet-budgetbakers/31-goal-detay-formu.png) | 68807 | `682fa2b9e2b2837143e04a3bacba4ae0bd8558f59f25bc8f180141c4b5f6aca4` |
| E0305 | [kanitlar/wallet-budgetbakers/32-butce-ve-goal-birlikte.png](kanitlar/wallet-budgetbakers/32-butce-ve-goal-birlikte.png) | 93150 | `19fdc8097951071844ccd9388e9292d733bbe3b7e9ff9da3cf2db7f50de250e5` |
| E0306 | [kanitlar/wallet-budgetbakers/33-planned-payments-b1-siradaki.png](kanitlar/wallet-budgetbakers/33-planned-payments-b1-siradaki.png) | 85654 | `364eb8b84e9b198f32f8edb6c508b7029a6450b5d2922b88964a75d38f3ff2d6` |
| E0307 | [kanitlar/wallet-budgetbakers/34-debts-bos-durum.png](kanitlar/wallet-budgetbakers/34-debts-bos-durum.png) | 82851 | `6cd1ec8ce7e40d5c9a0c4926b216dd33319905cb06ac34bbf85fd8242e4c364c` |
| E0308 | [kanitlar/wallet-budgetbakers/35-debt-kayit-baglama-sorusu.png](kanitlar/wallet-budgetbakers/35-debt-kayit-baglama-sorusu.png) | 75807 | `419d908e109a33a393a07de40a866def38beb74862d549e19e182cd2adbdbf7c` |
| E0309 | [kanitlar/wallet-budgetbakers/36-debt-i-lent-formu.png](kanitlar/wallet-budgetbakers/36-debt-i-lent-formu.png) | 95430 | `62fa8d17d9c14bc75ca86356440221e851626e05a7dd4dc7ce7d3c482865b687` |
| E0310 | [kanitlar/wallet-budgetbakers/37-records-listesi-b1-b2.png](kanitlar/wallet-budgetbakers/37-records-listesi-b1-b2.png) | 232763 | `3ffd815638f28f1714e8321fb082af41b009929e906747bccb826a008387fac0` |
| E0311 | [kanitlar/wallet-budgetbakers/38-split-record-ekrani.png](kanitlar/wallet-budgetbakers/38-split-record-ekrani.png) | 64483 | `dedb1d16cc8a7a2441f82c8552e78625df340cdc3ecbd93d73ce39c964dc53d9` |
| E0312 | [kanitlar/wallet-budgetbakers/39-split-yeni-kayit-dialog.png](kanitlar/wallet-budgetbakers/39-split-yeni-kayit-dialog.png) | 96127 | `ce00adbf451de1fe37139da01b8c2bc4395b8504973992fc87d77139cfd916df` |
| E0313 | [kanitlar/wallet-budgetbakers/40-add-receipt-dosya-veya-foto.png](kanitlar/wallet-budgetbakers/40-add-receipt-dosya-veya-foto.png) | 113043 | `5795a9e34e1235920d71e5c4530869d9704ef3df430b2e388d8ffa18d0c4b412` |
| E0314 | [kanitlar/wallet-budgetbakers/41-debt-i-lent-formu-dolu.png](kanitlar/wallet-budgetbakers/41-debt-i-lent-formu-dolu.png) | 76406 | `32eec25d351ec6a579514f533edcaea60157c3bea35fa57bff5c7841afd93aee` |
| E0315 | [kanitlar/wallet-budgetbakers/42-debt-kayit-olustur-mu-bakiye-degisir.png](kanitlar/wallet-budgetbakers/42-debt-kayit-olustur-mu-bakiye-degisir.png) | 111996 | `115368f11aa2a6b9d8469ed87546afe36b288f426ca7137d6e29dab5f1cca971` |
| E0316 | [kanitlar/wallet-budgetbakers/43-debt-olusturuldu-i-lent.png](kanitlar/wallet-budgetbakers/43-debt-olusturuldu-i-lent.png) | 70629 | `862dd8ffe026500a0b39715420f6c2a713d85cfa1cf134b35a5f7e96adee0d2c` |
| E0317 | [kanitlar/wallet-budgetbakers/44-debt-records-loan-interests-kaydi.png](kanitlar/wallet-budgetbakers/44-debt-records-loan-interests-kaydi.png) | 70241 | `ee6664e0d836daa0c743fdf23f5d847786b34f58d3340a8bfd03733d81852a86` |
| E0318 | [kanitlar/wallet-budgetbakers/45-planned-otomatik-onayli-toggle-her-zaman.png](kanitlar/wallet-budgetbakers/45-planned-otomatik-onayli-toggle-her-zaman.png) | 166279 | `b1778814e208166452fe526931604438d15f48dc9afe392780a45b03555306bf` |
| E0319 | [kanitlar/wallet-budgetbakers/46-planned-duzenleme-formu-cop-ikonu.png](kanitlar/wallet-budgetbakers/46-planned-duzenleme-formu-cop-ikonu.png) | 113382 | `e6d09e4256ed2c193f59404fb711dc6a9afa10fc3e63780d39dc0941034f4a4a` |
| E0320 | [kanitlar/wallet-budgetbakers/47-planned-silme-basit-onay-gecmis-uyarisi-yok.png](kanitlar/wallet-budgetbakers/47-planned-silme-basit-onay-gecmis-uyarisi-yok.png) | 120671 | `2304422813078610ece4ee1a23fcd8cfadde0a3d58ba98c9530810379ef02806` |
| E0321 | [kanitlar/wallet-budgetbakers/f7-00-baslangic.png](kanitlar/wallet-budgetbakers/f7-00-baslangic.png) | 213963 | `1e41c6ddd5d13d26e0ab85073fd695cc7329bffcbbe90627e2b28f74ef5a9399` |
| E0322 | [kanitlar/wallet-budgetbakers/f7-01-debts.png](kanitlar/wallet-budgetbakers/f7-01-debts.png) | 214135 | `3afaf4197da911ff9c80ca8c55fe1d6b96e80d1c8c51ce98c6d64e8923dd014e` |
| E0323 | [kanitlar/wallet-budgetbakers/f7-02-debts.png](kanitlar/wallet-budgetbakers/f7-02-debts.png) | 71004 | `65aca34833906ea375bad65d128bc57a88d4adbaa6da6ef26de58ad7149344c0` |
| E0324 | [kanitlar/wallet-budgetbakers/f7-03-fab-menu.png](kanitlar/wallet-budgetbakers/f7-03-fab-menu.png) | 229454 | `d2ab5471680dddc8f54071b41eb4aee05293987911a102ee042d280b481000f3` |
| E0325 | [kanitlar/wallet-budgetbakers/f7-04-transfer-form.png](kanitlar/wallet-budgetbakers/f7-04-transfer-form.png) | 85707 | `c665bad83520b6b5a0a88956cf4511216a80eaba527acdc3ae84ccff8ac209f1` |
| E0326 | [kanitlar/wallet-budgetbakers/f7-05-back-check.png](kanitlar/wallet-budgetbakers/f7-05-back-check.png) | 214184 | `e0119ca478be887b55e923675b088d0fad682e18f83afbf3ef13608343f0342e` |
| E0327 | [kanitlar/wallet-budgetbakers/f7-06-transfer-form2.png](kanitlar/wallet-budgetbakers/f7-06-transfer-form2.png) | 88769 | `002f4ffdb74a2f681286e19485dcbc3eb280d73d7cc7b3f07eac5beeb5a01ecb` |
| E0328 | [kanitlar/wallet-budgetbakers/f7-07-tutar-400.png](kanitlar/wallet-budgetbakers/f7-07-tutar-400.png) | 78070 | `bb35fc0495e8361dc4ca1a9c489a8aaa47d6c7eb74b7517fff781c203114bdfc` |
| E0329 | [kanitlar/wallet-budgetbakers/f7-08-back-transfer.png](kanitlar/wallet-budgetbakers/f7-08-back-transfer.png) | 92520 | `7fb925eec0c5e4ee5f86150d8331537a98c61314e6f13065a10cdf51a04b5d80` |
| E0330 | [kanitlar/wallet-budgetbakers/f7-09-tutar-400-v2.png](kanitlar/wallet-budgetbakers/f7-09-tutar-400-v2.png) | 104317 | `19a17099c5a0642baef4b6fe6f2deed8bb54e8ad64ee0f3f2505ebac85c2d386` |
| E0331 | [kanitlar/wallet-budgetbakers/f7-10-cleared.png](kanitlar/wallet-budgetbakers/f7-10-cleared.png) | 92176 | `4c6c229f95e957d0c22a9ad761df49fc411a7c4cfab8f0e5f929ac700f9010b3` |
| E0332 | [kanitlar/wallet-budgetbakers/f7-11-cleared2.png](kanitlar/wallet-budgetbakers/f7-11-cleared2.png) | 89067 | `d89115166efd853dc53907e3115f66985894646d5b8c2cdbe33d1249a55bd09b` |
| E0333 | [kanitlar/wallet-budgetbakers/f7-12-tutar-final.png](kanitlar/wallet-budgetbakers/f7-12-tutar-final.png) | 99538 | `6be4d6eeaf18b8a207ff3a1dd88509d1c4c6112a4aacad70cd68d739751672e0` |
| E0334 | [kanitlar/wallet-budgetbakers/f7-13-a-kaydedildi.png](kanitlar/wallet-budgetbakers/f7-13-a-kaydedildi.png) | 215081 | `f2898992f6ac461ac9dddbc2d663cdc8ffd9e69abc8daeed94229ca88d5c825c` |
| E0335 | [kanitlar/wallet-budgetbakers/f7-14-planned.png](kanitlar/wallet-budgetbakers/f7-14-planned.png) | 85730 | `971162abb171903935a054b4dd24ab016a9ce9bc459dbceb773266b44751cd46` |
| E0336 | [kanitlar/wallet-budgetbakers/f7-15-add-planned.png](kanitlar/wallet-budgetbakers/f7-15-add-planned.png) | 96894 | `8cddd5f6d7059b34c484c3bbc2d6c80a3f05f37afbaf68a495399061788faff9` |
| E0337 | [kanitlar/wallet-budgetbakers/f7-16-name-amount.png](kanitlar/wallet-budgetbakers/f7-16-name-amount.png) | 66122 | `1fda3a818eb6de28f0494db897b543f61da9e58aa77164d1c9d1196096186be2` |
| E0338 | [kanitlar/wallet-budgetbakers/f7-17-back-form.png](kanitlar/wallet-budgetbakers/f7-17-back-form.png) | 122012 | `b10b77a8cd0a7a9c8c3bb1f850b04b57a4d81bb0c4d3279962ba4607fb1f4802` |
| E0339 | [kanitlar/wallet-budgetbakers/f7-18-form-check.png](kanitlar/wallet-budgetbakers/f7-18-form-check.png) | 129439 | `9a51be6d18de11b4ef7aba2b754b12a69bbe9536e94eb84dc303389f35aa8deb` |
| E0340 | [kanitlar/wallet-budgetbakers/f7-19-amount-10000.png](kanitlar/wallet-budgetbakers/f7-19-amount-10000.png) | 130074 | `398599775800bf6b7b98664788f7a4adf7d097f4ab96f9cbebfe403009e991bd` |
| E0341 | [kanitlar/wallet-budgetbakers/f7-20-amount-check2.png](kanitlar/wallet-budgetbakers/f7-20-amount-check2.png) | 131811 | `d26c5b8dbbcef9b125bdcfd6853cea2cab5a1dd7073504af03fe4bdb26a64791` |
| E0342 | [kanitlar/wallet-budgetbakers/f7-21-cleared.png](kanitlar/wallet-budgetbakers/f7-21-cleared.png) | 130780 | `c605027ca4d488809c7fef7dc767d25b37ec0de94ffbf232340490df0aad1f22` |
| E0343 | [kanitlar/wallet-budgetbakers/f7-22-amount-verify.png](kanitlar/wallet-budgetbakers/f7-22-amount-verify.png) | 130317 | `d5ad5024276c7078d0cef5edee1c79f98dabebd682655dbf499ab9d76d1beb6b` |
| E0344 | [kanitlar/wallet-budgetbakers/f7-23-form-with-amount.png](kanitlar/wallet-budgetbakers/f7-23-form-with-amount.png) | 125149 | `22e8ccceeefa8e1ba092081024a019a673015cb6994af6c3e8f87ef2c2640591` |
| E0345 | [kanitlar/wallet-budgetbakers/f7-24-date-picker.png](kanitlar/wallet-budgetbakers/f7-24-date-picker.png) | 136306 | `72a42b5e7a6f27405ada792e5c2029e319281c9a851d2d129dd32f4d7e201305` |
| E0346 | [kanitlar/wallet-budgetbakers/f7-25-day5-attempt.png](kanitlar/wallet-budgetbakers/f7-25-day5-attempt.png) | 136488 | `9567a94cc6832f248a7dad80b447fbba96dff4487dd0b375ab459d4e1a774e2b` |
| E0347 | [kanitlar/wallet-budgetbakers/f7-26-d1-saved.png](kanitlar/wallet-budgetbakers/f7-26-d1-saved.png) | 99452 | `a7b951593a5a6d24c59c34620c54c40f32cb62feb532f28b97b5a320fb79a777` |
| E0348 | [kanitlar/wallet-budgetbakers/f7-27-category.png](kanitlar/wallet-budgetbakers/f7-27-category.png) | 99090 | `f2d5991fc7c73970ac07902a8e2ebbd6294547ca875c198e71b58caf3770864e` |
| E0349 | [kanitlar/wallet-budgetbakers/f7-28-category-list.png](kanitlar/wallet-budgetbakers/f7-28-category-list.png) | 131153 | `e910b01216256a6f9dc9da8ec449f1897d4ab404c476a8e44ab35e304bbf8967` |
| E0350 | [kanitlar/wallet-budgetbakers/f7-29-category-selected.png](kanitlar/wallet-budgetbakers/f7-29-category-selected.png) | 125889 | `0eccdeaa870827c5a798ef39069e1c6c96173a8cbc0cd833f4341d113a49ce19` |
| E0351 | [kanitlar/wallet-budgetbakers/f7-30-housing.png](kanitlar/wallet-budgetbakers/f7-30-housing.png) | 94492 | `7cf4f2ac627b8a943b551a897e32b825315ef179ea04ddc75c61e9e423dd12f6` |
| E0352 | [kanitlar/wallet-budgetbakers/f7-31-rent-selected.png](kanitlar/wallet-budgetbakers/f7-31-rent-selected.png) | 95851 | `251b450b40d87ba6ea21dadbe97dede9b473be6341bc191d8abd71992196f452` |
| E0353 | [kanitlar/wallet-budgetbakers/f7-32-d1-final.png](kanitlar/wallet-budgetbakers/f7-32-d1-final.png) | 110895 | `60c1c6ab337c5414955caadfacdd59fc76dc08355ef07143dc4f90ed8cfe1d1c` |
| E0354 | [kanitlar/wallet-budgetbakers/f7-33-tap-planned.png](kanitlar/wallet-budgetbakers/f7-33-tap-planned.png) | 73605 | `279b5b2dd9c5c1751c5649ce0158860715b45b5595f083fd5f075c5220a3268f` |
| E0355 | [kanitlar/wallet-budgetbakers/f7-34-confirm-dialog.png](kanitlar/wallet-budgetbakers/f7-34-confirm-dialog.png) | 104721 | `0aa5674d51a0e78dc45cd3d4d43ebc524068b2cffc6c3175140187477a46dc78` |
| E0356 | [kanitlar/wallet-budgetbakers/f7-35-d1-confirmed.png](kanitlar/wallet-budgetbakers/f7-35-d1-confirmed.png) | 66213 | `c003d544133ac246b9b416a13ad097f3227b6afb91377b1bcff3fd16bb5ee75a` |
| E0357 | [kanitlar/wallet-budgetbakers/f7-36-nav-check.png](kanitlar/wallet-budgetbakers/f7-36-nav-check.png) | 214708 | `98a056385a528878c9401c60a561dd0690c3a68173513b70221e3c78c247c442` |
| E0358 | [kanitlar/wallet-budgetbakers/f7-37-debts-fab.png](kanitlar/wallet-budgetbakers/f7-37-debts-fab.png) | 90066 | `a18d3677e89a81fa43ef08f491e1a31c1cc2854c5e5b6887270fb4e004418e55` |
| E0359 | [kanitlar/wallet-budgetbakers/f7-38-i-lent-form.png](kanitlar/wallet-budgetbakers/f7-38-i-lent-form.png) | 71868 | `ee7fab77ff44ecf276dca572f9457c44ab29e283a0a78927b699f8cfb30fff4d` |
| E0360 | [kanitlar/wallet-budgetbakers/f7-39-i-lent-form2.png](kanitlar/wallet-budgetbakers/f7-39-i-lent-form2.png) | 74561 | `77f21acef4fbd46622c39670c3678a5dd723d799767d283e38d99cec81684109` |
| E0361 | [kanitlar/wallet-budgetbakers/f7-40-i-lent-form3.png](kanitlar/wallet-budgetbakers/f7-40-i-lent-form3.png) | 74074 | `bf587901ad90cd38749c7753ccf192f0f50c11fcf9524dc629453dab17dc5311` |
| E0362 | [kanitlar/wallet-budgetbakers/f7-41-i-lent-form4.png](kanitlar/wallet-budgetbakers/f7-41-i-lent-form4.png) | 78550 | `134cb77aae1ce530846594b734abf415645e7de646705411f8dd33683910d114` |
| E0363 | [kanitlar/wallet-budgetbakers/f7-42-form-filled.png](kanitlar/wallet-budgetbakers/f7-42-form-filled.png) | 71605 | `a61934283d5ccb7fb756d01d95f957e60dc0fb993206a375fabd1511ce814a47` |
| E0364 | [kanitlar/wallet-budgetbakers/f7-43-amount-12000.png](kanitlar/wallet-budgetbakers/f7-43-amount-12000.png) | 108129 | `08f92cc38fe0ec5c31f410c7522c18645dabd0655dd372fb2a130f135438d241` |
| E0365 | [kanitlar/wallet-budgetbakers/f7-44-d2-saved.png](kanitlar/wallet-budgetbakers/f7-44-d2-saved.png) | 126810 | `a2c70de593a0436f9bbcd26adceda3de04338c78e380113feebd8914cbd81682` |
| E0366 | [kanitlar/wallet-budgetbakers/f7-45-d2-final.png](kanitlar/wallet-budgetbakers/f7-45-d2-final.png) | 101156 | `2bbb7ccb7dfd951ccb8dca3f05314485e3fa489f6eceb106b8d6215ed6d508dc` |
| E0367 | [kanitlar/wallet-budgetbakers/f7-46-add-record-form.png](kanitlar/wallet-budgetbakers/f7-46-add-record-form.png) | 129289 | `155aa75cebf3b7e0368fe95456ed1567236abeee9711943fd375244a9a09caaa` |
| E0368 | [kanitlar/wallet-budgetbakers/f7-47-new-record-form.png](kanitlar/wallet-budgetbakers/f7-47-new-record-form.png) | 87519 | `acf97982bbd0c372bff5d0b3584de527477aab82d1567ba1d6bb35b7ab52c903` |
| E0369 | [kanitlar/wallet-budgetbakers/f7-48-amount-5000.png](kanitlar/wallet-budgetbakers/f7-48-amount-5000.png) | 83107 | `63cfb0c0e14d3a02471dbd57fb3c6389bc66138e89142767527cee505b1994eb` |
| E0370 | [kanitlar/wallet-budgetbakers/f7-49-d3-saved.png](kanitlar/wallet-budgetbakers/f7-49-d3-saved.png) | 100964 | `040018d95b1ac948048487b857eae8c8d8a12b482aaefdf101a1ffe5b2c351a9` |
| E0371 | [kanitlar/wallet-budgetbakers/f7-50-balance-check.png](kanitlar/wallet-budgetbakers/f7-50-balance-check.png) | 215063 | `1282fb0da9c664c1ce07f2f558913765c4047268cac92b28982df2340eacecc5` |
| E0372 | [kanitlar/wallet-budgetbakers/f7-51-records.png](kanitlar/wallet-budgetbakers/f7-51-records.png) | 230703 | `1a47854c880aef482b34a99208890aeecb78418009130eecb4a1e020dedfa863` |
| E0373 | [kanitlar/wallet-budgetbakers/f7-52-search.png](kanitlar/wallet-budgetbakers/f7-52-search.png) | 189334 | `0d9c4c9b612a75c9105aa0c68197717bb6260f5f105dbefb5b0eace2f9f541aa` |
| E0374 | [kanitlar/wallet-budgetbakers/f7-53-more-options.png](kanitlar/wallet-budgetbakers/f7-53-more-options.png) | 253859 | `52433d422165a1e650aa86d0de045a2a7c26343ad3b933601527b7154d1fa33d` |
| E0375 | [kanitlar/wallet-budgetbakers/f7-54-hamburger.png](kanitlar/wallet-budgetbakers/f7-54-hamburger.png) | 156669 | `63c8335193c51b15d5656ba7fdf26eedba211876a7ce49ff2d69eb1375d994c8` |
| E0376 | [kanitlar/wallet-budgetbakers/f7-55-menu-scroll.png](kanitlar/wallet-budgetbakers/f7-55-menu-scroll.png) | 152736 | `02e7d8b84050955879bc27dc62117eb586dc4cf7e4d008079034dcd875ab85ab` |
| E0377 | [kanitlar/wallet-budgetbakers/f7-56-settings.png](kanitlar/wallet-budgetbakers/f7-56-settings.png) | 177734 | `f8d27b98a70664c4bd5548c789215b9b3ecf5363681879a07c02c3e99dacd212` |
| E0378 | [kanitlar/wallet-budgetbakers/f7-57-settings-scroll.png](kanitlar/wallet-budgetbakers/f7-57-settings-scroll.png) | 181622 | `5228e1f2fd25b00b53851079db5354dfd3dc08c23dc468a3a2980c2133370c2c` |
| E0379 | [kanitlar/wallet-budgetbakers/f7-58-advanced.png](kanitlar/wallet-budgetbakers/f7-58-advanced.png) | 82339 | `c124d849e81e57aa4e2b191e1c2f6a941202bf5d0e57fe6e79ad2e969f5566db` |
| E0380 | [MANUEL-TEST-PROTOKOLU.md](MANUEL-TEST-PROTOKOLU.md) | 19659 | `e31c61422049520883b5bdfa1760533384e07f947c5fa17005e17992c9517ae1` |
| E0381 | [raporlar/2026-09-13-kapsamli-yeniden-denetim.md](raporlar/2026-09-13-kapsamli-yeniden-denetim.md) | 26503 | `f4812e8714f16bda2f8af73412485414d5f0f46682bf8f2b3131337c477e99c5` |
| E0382 | [raporlar/denetim-2026-09-13.cjs](raporlar/denetim-2026-09-13.cjs) | 5224 | `82e343c191fa8be8e916b3a757b06a108887f091c24a2669630ea6ab10106bfc` |
| E0383 | [raporlar/README.md](raporlar/README.md) | 3555 | `dc2ecd50db3fb8ac1913213bdc893806a7cd2f67023f1d903a7360d848853608` |
| E0384 | [raporlar/Soru 1.md](raporlar/Soru%201.md) | 3733 | `8ea7a3022bf17a041d6e4297a7520525df6f35f0669fb1b6b5404fbefa3d24a0` |
| E0385 | [raporlar/soru 2.md](raporlar/soru%202.md) | 11647 | `ce7bc47c844e5ae351388519872ddeaf4a71a804f42043f892bac2ff147e396c` |
| E0386 | [raporlar/turk-on-muhasebe-vs-businessfinance.md](raporlar/turk-on-muhasebe-vs-businessfinance.md) | 6584 | `6a75f99c5904c86f7440bda3357f78b5eb0b33743d5b5b969171e3d6812260b7` |
| E0387 | [README.md](README.md) | 15967 | `e522c0b61a092ca506894031f9ee5630f2770a921c567dd7363ad32a433df038` |
| E0388 | [SENTETIK-TEST-VERISI.md](SENTETIK-TEST-VERISI.md) | 6128 | `cfdfd33ee8009a9022bf092cc77aaf563def430466906f1101fa1edb5b6800ca` |
| E0389 | [TUR2-YOL-HARITASI.md](TUR2-YOL-HARITASI.md) | 19948 | `d5faea930b9ed6731cd50f8d58b79155688da7a6aad082f86384ea9b04a39f42` |
| E0390 | [UYGULAMA-GOZLEM-SABLONU.md](UYGULAMA-GOZLEM-SABLONU.md) | 7222 | `e7fdde9d378133dccf38dd0ba50242ec64253127cff1e41f66e39f2fad9b74f7` |
| E0391 | [KANIT-ENVANTERI.md](KANIT-ENVANTERI.md) | — | Kendisi; tutulmaz |
| E0392 | [BULGU-DOGRULAMA-KAYDI.md](BULGU-DOGRULAMA-KAYDI.md) | — | P0.2 yönetim kaydı; değişken içerik |
| E0393 | [denetim.cjs](denetim.cjs) | 9016 | `546e1473ae39c227eca0f32ff50df35b4e53738c419c42a0f981a8d7562f444f` |
| E0394 | [denetim-test.cjs](denetim-test.cjs) | 6776 | `e568019f9fc5698d0e44bf26877f83545b04ed5eea8588cb9bad769710e5aa03` |

P0.2 eklemesiyle güncel toplam 392 dosya, 25 Markdown; PNG sayısı 357.
P1-K eklemesiyle (14 Eylül 2026) E0393–E0394 eklendi; 394 kimlik. E0002
`denetim.sh` aynı turda ince sarmalayıcıya dönüştü (1098 bayt,
`cb3710f27206ee59a58c73f87ae7f62cc92649ae894b1583e38398cd345e58cb`);
tablodaki değer P0.1 anınındır.
Başlangıç boyut/hash sütunları P0.1 anlık kaydı olarak korunur.

## İnceleme ayrıntıları

### E0010 — Money Manager metin haritası

13 Eylül 2026 P0.3-money-manager: 379 satırın tamamı okundu; hash başlangıç
kaydıyla aynı. E0392 bulgu kaydında MM-M01–MM-M16 ve MM-Q01–MM-Q10 yazıldı.
Bu, MD içerik haritasıdır; rakip iddialarının doğrulanması değildir.
E0227–E0256 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/30.
Diğer dosyaların içerik durumu bu işlemle değişmez. Üstteki P0.1 durumları
o başlangıç koşumunun kaydıdır; yeni incelemeler bu ayrıntı bölümünde izlenir.

### E0007 — Hesap Defterim metin haritası

14 Eylül 2026 P0.3-hesap-defterim: 367 satırın tamamı okundu; SHA-256
`789f19dbb779373615f50303c0dae8f9691e3873f111c85384056e4d3dcfc68f`
başlangıç kaydıyla aynı. E0392'de HD-M01–HD-M19 ve HD-Q01–HD-Q12 yazıldı.
Bu, MD içerik haritasıdır; rakip iddialarının doğrulanması değildir.
E0134–E0178 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/45.
B11/B12/B14 kontrolüne bağlandı; B01–B19 açık kalır. Kaynak form değişmedi.

### E0006 — Goodbudget metin haritası

14 Eylül 2026 P0.3-goodbudget: 293 satırın tamamı okundu; SHA-256
`02a036673d455696d12d388ed1625e4ad239701f4e758ee22ce17b872d2437b7`
başlangıç kaydıyla aynı. E0392'de GB-M01–GB-M17 ve GB-Q01–GB-Q12 yazıldı.
Bu, MD içerik haritasıdır; rakip iddialarının doğrulanması değildir.
E0106–E0133 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/28.
B05/B06/B07 kontrolüne bağlandı; B01–B19 açık kalır. Kaynak form değişmedi.
B05/B07 dış URL'leri önceki denetimden devralındı; bu tur açılmadı.

### E0011 — Paraşüt metin haritası

14 Eylül 2026 P0.3-parasut: 241 satırın tamamı okundu; SHA-256
`ddcc29b4deb610efaaee3f325972a78724cceb693e66d04a9285928a70631eeb`
başlangıç kaydıyla aynı. E0392'de PS-M01–PS-M14 ve PS-Q01–PS-Q12 yazıldı.
Bu, MD içerik haritasıdır; kılavuz/ürün davranışı doğrulaması değildir.
E0258–E0266 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/9.
E0257 .gitkeep görsel değildir. Kaynak URL eksikleri PS-Q01'e, numarasız
aynı fatura/mahsup devirleri PS-Q03/Q04'e bağlandı. B01–B19 açık kalır.
Kaynak form değişmedi; masa başı inceleme sınırı genişletilmedi.

### E0009 — Logo İşbaşı metin haritası

14 Eylül 2026 P0.3-logo-isbasi: 248 satırın tamamı okundu; SHA-256
`cee51ae09da8194a354c0549b78e5f4d4d371a308638e731dd4382cef1d07f4f`
başlangıç kaydıyla aynı. E0392'de LI-M01–LI-M13 ve LI-Q01–LI-Q10 yazıldı.
Bu, MD içerik haritasıdır; ürün/kaynak davranışı doğrulaması değildir.
E0220–E0225 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/6.
E0219 .gitkeep görsel değildir. OCR/insan onayı devri LI-Q05'e,
yetki yayılımı B19/LI-Q04'e bağlandı. B01–B19 açık kalır.
Kaynak form değişmedi; kayıt/SMS kapısı aşılmadı, dış kaynak açılmadı.

### E0012 — QuickBooks metin haritası

14 Eylül 2026 P0.3-quickbooks: 169 satırın tamamı okundu; SHA-256
`9870e7d8bdb8f22111ad8262aee00e98f8df987239911c9d55a2b1bf83950bf2`
başlangıç kaydıyla aynı. E0392'de QB-M01–QB-M13 ve QB-Q01–QB-Q12 yazıldı.
Bu, MD içerik haritasıdır; yardım merkezi/ürün davranışı doğrulaması değildir.
E0268–E0271 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/4.
E0267 .gitkeep görsel değildir. B13 gereği kareler QuickBooks mobil
onboarding'i ve QBO Simple Start plan ekranı bağlamında tutulur; Solopreneur
arayüzü veya fiyatı kanıtı sayılmaz (QB-Q01/Q03). E0272'deki klasör etiketi
B19 yayılımı olarak QB-Q01'e bağlandı. B01–B19 açık kalır. Kaynak form
değişmedi; ödeme/deneme kapısı aşılmadı, dış kaynak açılmadı.

### E0008 — KolayBi metin haritası

14 Eylül 2026 P0.3-kolaybi: 799 satırın tamamı okundu; SHA-256
`b0233d1959691fc9644ca925500baa2aff48e8412d13ea04f686b56ec8221944`
başlangıç kaydıyla aynı. E0392'de KB-M01–KB-M23 ve KB-Q01–KB-Q17 yazıldı.
Bu, MD içerik haritasıdır; destek/video/ürün davranışı doğrulaması değildir.
E0180–E0218 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/39.
E0179 .gitkeep görsel değildir. Rol ayrımı metin beyanıdır: E0180 mobil giriş,
E0181–E0186 eski video, E0187 ayrı 2026 videosu, E0188–E0218 destek
mockup'ları; tarih/rol doğrulaması B15/KB-Q03'te açık. B02/B03/B04/B08/B17
bağları KB-Q10/Q11/Q16'da. B01–B19 açık kalır. Kaynak form değişmedi;
kayıt, video ve destek sayfası erişimi yapılmadı.

### E0005 — Bluecoins metin haritası

14 Eylül 2026 P0.3-bluecoins: 362 satırın tamamı okundu; SHA-256
`b9eed5f684d8bd69f1eb019472eab57794a7867d2ecd1e1c126b09e869f9a059`
başlangıç kaydıyla aynı. E0392'de BC-M01–BC-M18 ve BC-Q01–BC-Q16 yazıldı.
Bu, MD içerik haritasıdır; P2 tam Faz 7.5 doğrulaması değildir.
E0016–E0105 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/90.
E0015 .gitkeep görsel değildir. Koşum grupları form atıflarına göre:
E0016–E0025 Tur 1, E0026–E0050 Faz 3, E0051–E0105 Faz 7; f7 eşlemeleri
dosya adı adayıdır (BC-Q10). B10/B16/B17/B18 bağları BC-Q02–Q04/Q09–Q11'de.
B01–B19 açık kalır. Kaynak form değişmedi; emülatör açılmadı.

### E0014 — Wallet metin haritası

14 Eylül 2026 P0.3-wallet: 380 satırın tamamı okundu; SHA-256
`e424d5e4fff1c71b35e07a1e182fba25c80a3e5946af63bc1e3e6b6e4bbefb9b`
başlangıç kaydıyla aynı. E0392'de WL-M01–WL-M18 ve WL-Q01–WL-Q16 yazıldı.
Bu, MD içerik haritasıdır; P3 tam Faz 7.5 doğrulaması değildir.
E0274–E0379 için bu tur açılmış görsel **yok**; yeni görsel incelemesi 0/106.
E0273 .gitkeep görsel değildir. Koşum grupları form atıflarına göre:
E0274–E0282 Tur 1, E0283–E0320 Faz 2 ve ek koşum, E0321–E0379 Faz 7.
E0286/E0303/E0307/E0310 metinde anılmıyor; f7 eşlemeleri dosya adı adayıdır
(WL-Q11). Klasör yeniden adlandırması staged kullanıcı değişikliğidir; bu tur
taşıma yapılmadı. B01/B09/B12/B17/B18 bağları WL-Q02/Q03/Q09/Q11–Q13'te.
B01–B19 açık kalır. Kaynak form değişmedi; emülatör ve bulut hesabı açılmadı.

### P0.4 — Envanter kabul kontrolü ve görsel paket ataması

14 Eylül 2026 P0.4: bu envanterin 392 satırı diskle yeniden karşılaştırıldı.
Kimlikler sıralı ve benzersiz; eksik veya envanter dışı dosya yok; 357 PNG'nin
boyut ve SHA-256'sı P0.1 kaydıyla aynı. P0.1 hash'inden farklı metin dosyaları
yalnız E0003/E0004/E0383/E0387/E0389 devir belgeleridir; dokuz gözlem formu
aynıdır. Görsel başına ayrıntı alanları hâlâ atanmadı. Her PNG tam olarak bir
G paketine atandı (17 paket); aralıklar ve ilk/son dosyalar E0392 P0.4
bölümündedir. Hiçbir görsel bu tur açılmadı.

### P1-B01 — B01 kapsamında düzeltilen gözlem formları

14 Eylül 2026 P1-B01: dört formda BusinessFinance'e fatura bazlı tahsilat bağı
atfeden dokuz yer düzeltildi (ayrıntı E0392 P1-B01 bölümü). Üstteki tablodaki
boyut/hash değerleri P0.1 anlık kaydı olarak korunur; güncel değerler:

| Kimlik | Dosya | Satır (önce → sonra) | Güncel SHA-256 |
|---|---|---|---|
| E0005 | gozlemler/bluecoins.md | 362 → 369 | `78cf3ec2f6e533d261ce3ad678b01908d6ce2f25f287f4358d289e64705288e8` |
| E0008 | gozlemler/kolaybi.md | 799 → 804 | `c05604dd85a76389586f6d5f3a125da2715d78e722a7b76b70548c6049250cf7` |
| E0011 | gozlemler/parasut.md | 241 → 241 | `c04001d4e6d4f1e543fb3737f7ce7d737dacb23e2aaba404c405802a9c5a33ac` |
| E0014 | gozlemler/wallet-budgetbakers.md | 380 → 384 | `3ab183d70d9762d0427bb99fe1275a6405bc6b6114eb35b4f0075b528c93746f` |

Görseller ve diğer beş form değişmedi. P0.3 haritalarının satır atıfları eski
sürüme aittir; sonraki paketler bu dört formu güncel hash ile okur.

### P1-B02-B04 — B02/B03/B04 kapsamında düzeltilen gözlem formları

14 Eylül 2026 P1-B02-B04: üç formda mevcut BusinessFinance yeteneklerini yanlış
anlatan on bir yer düzeltildi (ayrıntı E0392 P1-B02-B04 bölümü). E0005 ve E0008
için bir önceki satır P1-B01 sonrası değerdir. Güncel değerler:

| Kimlik | Dosya | Satır (önce → sonra) | Güncel SHA-256 |
|---|---|---|---|
| E0005 | gozlemler/bluecoins.md | 369 → 369 | `00a4dbec8aeef086df5b9c30fcb048de722cfd923981081173d767eb149e69b5` |
| E0007 | gozlemler/hesap-defterim.md | 367 → 367 | `c71f9ac6b4e8d6a8d59f063799278d715f6f6e3c0e344f5f6f83d92c61dc3673` |
| E0008 | gozlemler/kolaybi.md | 804 → 807 | `f6567dfc21ccaf9111459fb4ee03c4abf0bd87f39ea38666a0d87eb9b0ae7178` |

P1-B01 kaydındaki E0005 ve E0008 hash'leri bu paketle geçersizleşti; E0011 ve
E0014'ün P1-B01 hash'leri hâlâ günceldir. Görseller değişmedi.

### P1-B14 — B14 kapsamında düzeltilen gözlem formu

14 Eylül 2026 P1-B14: Hesap Defterim formunda kupür hesap makinesini
BusinessFinance'te karşılığı olmayan bir ihtiyaç gibi anlatan tek karar satırı
düzeltildi (ayrıntı E0392 P1-B14 bölümü). Bir önceki satır P1-B02-B04 sonrası
değerdir. Güncel değer:

| Kimlik | Dosya | Satır (önce → sonra) | Güncel SHA-256 |
|---|---|---|---|
| E0007 | gozlemler/hesap-defterim.md | 367 → 367 | `39562088dbd49018efc7e9aae43187f596bd4b2198ccc50e0a55c47205ca8685` |

P1-B02-B04 kaydındaki E0007 hash'i bu paketle geçersizleşti; E0005 ve E0008'in
P1-B02-B04 hash'leri hâlâ günceldir. Görseller değişmedi.

### P1-goodbudget-G01 — E0106–E0133 görsel içerik incelemesi

14 Eylül 2026. Hedef: 28 PNG, E0106–E0133. Her görsel açıldı ve E0006
(SHA `02a03667…`) metniyle karşılaştırıldı; sonuç görsel açıldıktan hemen sonra
yazıldı. Platform/sürüm form beyanıdır (Goodbudget 2.24.26013, Pixel 8 AVD,
Android 17, ücretsiz plan); görsel sürüm numarası taşımıyorsa bu satırlarda
doğrulanmış sayılmaz. Yakalama tarihi form beyanıdır: 01–22 11 Eylül, 23–26
12 Eylül; dosya tarihi arayüz sürümünü kanıtlamaz. Hash değişmedi; hash içerik
onayı değildir. Rol: **Ana** (bir farkı açıklayan aday), **Ek** (kanıt eki),
**Arşiv** (süreç/tekrar). Emülatör açılmadı; veri/ayar değişmedi.

| Kimlik | Görselin gerçekten gösterdiği | Formla uyum | Rol | Bağ |
|---|---|---|---|---|
| E0106 | Açılış ekranı, saat 08:26: `LOG IN` / `CREATE NEW HOUSEHOLD`, "Secured using bank-grade 256-bit SSL", `Version 2.24.26013 (180)` | Uyumlu: sürüm (form 8) ve iki düğme (form 31) karede okunuyor. SSL ifadesi uygulamanın beyanıdır, doğrulanmadı | Ek — sürüm kanıtı | GB-M01, GB-M03; GB-Q10 |
| E0107 | Boş `Add Envelope` formu, 08:30: `Envelope Name`, `Budget Amount 0.00`, `Budget Period: Monthly` açılırı, `Hide on this device` kutusu, `SAVE` | Uyumlu: 12 Eyl yeniden adlandırması doğru, "envelope suggestions" değil. Formda anılmayan ayrıntı: zarf başına dönem seçimi ve cihazda gizleme. Dönem seçeneklerinin listesi karede yok | Arşiv — kurulum süreci; dönem alanı Belge 1 form deseni için ek aday | GB-M03, GB-M10; GB-Q07, GB-Q10 |
| E0108 | `Setup Budget` üzerinde diyalog, 08:54: "Great! Your budget is set. Now let's put money in your Envelopes. Then you can spend from them." `LET ME DO İT` / `FİLL 'EM FOR ME!`. Arkada `Monthly (2)`: Market 850, Tasarim Yazilimi 1,200, "8 of 10 free Envelopes left"; `More Envelopes (0)` "10 of 10 free Envelopes left"; altta `Estimated Monthly Income 25,000`, `Remaining 22,950` | Uyumlu: planlama ile doldurmanın iki adım olduğu (form 31) ve diyalog metni doğru. Formda anılmayan iki şey: (1) iki grupta sayaç farklı (8/10 ve 10/10), 10 zarflık sınırın grup başına mı toplam mı olduğu karede belirsiz; (2) `Remaining 22,950` = tahmini gelir 25.000 − bütçelenen 2.050. Ağustos raporundaki `−22,950` ile aynı büyüklük; form o sayıyı 1.200 + 850 − 25.000 diye açıklıyor. Eşitlik iki açıklamayı ayırt etmiyor, yalnız ek bir aday gösteriyor | Ana — "bütçe kur" ile "parayı zarfa koy" ayrımı | GB-M03, GB-M08; GB-Q01, GB-Q02, GB-Q07; B05 |
| E0109 | `Setup Budget`, diyalogsuz, 08:38: E0108'in arka planıyla aynı liste, sayaçlar ve `Estimated Monthly Income 25,000` / `Remaining 22,950`; `BACK` / `NEXT` | Uyumlu: form 182'nin `8 of 10 free Envelopes left` sayacı karede var. Sayaç yalnız `Monthly` grubunda 8/10, `More Envelopes`'ta 10/10; form bunu "10 zarf sınırı" diye tek sayı olarak anlatıyor, grup başına mı toplam mı olduğu belirsiz | Arşiv — E0108 aynı ekranı diyalogla ve daha bilgilendirici gösteriyor | GB-M03, GB-M11; GB-Q07, GB-Q10 |
| E0110 | `Register Household`, 09:00: banner "Envelopes Filled! Register to see your budget on the web, share to your partner's phone, and get reports too."; `Email Address` alanında gri `johnj@email.com`, iki parola alanı, `Get feature updates` ve `Terms of Use` / `I agree.` kutuları; `LATER` / `FİNİSH` | Kısmen uyumlu: kayıt ekranının zarf doldurmadan sonra geldiği ve placeholder'ın gerçek görünümlü olduğu doğru (form 31, 141–145). **"`LATER` ile atlanabiliyor" karede yalnız düğmenin varlığıdır**; atlama denenmedi. E0111'de üst barda household adı görünüyor, yani koşumda kayıt tamamlandı. Banner'daki "get reports too" raporların kayıt gerektirdiğini ima eder ama doğrulanmadı | Ana — kayıt öncesi bütçe kurma sırası | GB-M03, GB-M10; GB-Q08, GB-Q10 |
| E0111 | `ACCOUNTS` sekmesi, 09:03: "Set up your Accounts — Keep track of where your money is in real life, like Checking, Savings, or Cards." dört madde (no account numbers or passwords required dahil) ve `TURN ON ACCOUNTS`; dört üst sekme, sağ altta `+`, üst barda household adı ve bir indirme/senkron simgesi | Uyumlu: hesap katmanının açılana kadar kapalı olduğu (form 33) doğru. Karede hesap türü olarak Checking/Savings/Cards anılıyor; "hesap türü yok" genellemesine karşı kanıttır (B07). "Varsayılan kapalı" yalnız bu yeni household için gözlendi | Ana — opsiyonel hesap katmanı | GB-M02, GB-M03, GB-M11; GB-Q06, GB-Q07; B07 |
| E0112 | `Edit Accounts`, boş, 09:03: `ADD ACCOUNT` / `GET UNLİMİTED ACCOUNTS`; üç grup `Checking, Savings, or Cash (0)` · `Credit Card (0)` · `Debt (0)` | Uyumlu: form 33'teki üç grup doğru. Formun "hesap formu sade: ad, açılış bakiyesi, tür" cümlesini bu karelerin hiçbiri göstermiyor; hesap ekleme formu G01'de kanıtsız. Kart ve borç türünün üründe bulunduğunu gösterir; "hesap türü yok" (form 25) bununla çelişiyor | Ana — üç hesap türü | GB-M02, GB-M03; GB-Q06; B07 |
| E0113 | `Edit Accounts`, 09:05: `Checking, Savings, or Cash (1)` altında `Ana Hesap 20,000`; `Credit Card (0)`, `Debt (0)` | Uyumlu: açılış bakiyesi 20.000 (kontrol tablosu 54) karede. Hesabın hangi alt türde (checking/savings/cash) açıldığı görünmüyor | Arşiv — E0114 aynı durumu diyalogla gösteriyor | GB-M03, GB-M07; GB-Q07 |
| E0114 | E0113 üzerinde diyalog, 09:06: "Account Limit Reached — You've reached your limit of 1 Account. Subscribe for more Accounts!" `NO THANKS` / `SUBSCRİBE` | Uyumlu: alıntı birebir (form 33). **"Üç tür ortak havuzu paylaşıyor"** karede görünmüyor: diyalog `ADD ACCOUNT` aşamasında, tür seçilmeden çıkmış olabilir; ortak havuz çıkarımdır. Kart/transfer testlerinin engeli ücretsiz plan sınırıdır, üründe yokluk değildir (B07) | Ana — ücretsiz plan sınırı | GB-M01, GB-M05, GB-M09; GB-Q06; B07 |
| E0115 | `ENVELOPES`, 09:02: `Last Backup: <1m ago`, `Total: 2,050.00`; `Monthly 2,050.00` altında Market 850.00 / 850.00 ve Tasarim Yazilimi 1,200.00 / 1,200.00, her birinde yeşil çubuk ve ortada dikey bir işaret; `Available 0.00` altında iki ayrı `[Available] 0.00` satırı; `+` | Uyumlu: form 32'deki düzen, iki `[Available]` satırı ve zarf toplamı doğru. Formda anılmayan iki şey: (1) saat 09:02, hesap 09:05'te açıldı (E0113) — zarflar hiçbir hesap yokken dolduruldu; "zarf doldurma hesaba dokunmuyor" (form 59, 204) ile tutarlı, kanıtı güçlendiriyor; (2) çubuk üstündeki dikey işaretin anlamı yazmıyor (dönem içi zaman işareti olabilir, doğrulanmadı). Doldurmanın `FİLL 'EM FOR ME!` mi `LET ME DO İT` ile mi yapıldığı görünmüyor | Ana — zarf katmanı ve iki toplam | GB-M03, GB-M07, GB-M12; GB-Q01, GB-Q07 |
| E0116 | `Add Transaction`, 09:10: Payee `Ada Reklam`, Amount `0.00`, tür `Credit`, Envelope `- Select Envelope -`, Account `Ana Hesap [20,000.00]`, Date `09/11/2026`, `Check # (Optional)`, `Schedule this…`; alt alanlar Android klavye araç çubuğunun altında kısmen kapalı (`…nal)`, `…d to Quick Transactions widget (…it from Settings)`, `…e location to transaction`) | Kısmen uyumlu: alanlar ve bugünkü tarih varsayılanı (form 34) doğru; `Credit` adı karede. Form 121 bu kareyi "formun tüm alanlarını gösteriyor" diye B2 negatif kanıtı sayıyor; alt kısım kapalı ve `Split into multiple Envelopes` karede görünmüyor, bu yüzden taksit yokluğunun kanıtı olamaz. Tür açılırının seçenekleri de görünmüyor | Ek — form alanları; Belge 1 için sınırlı | GB-M04, GB-M09, GB-M11; GB-Q01, GB-Q09 |
| E0117 | Aynı form, 09:16: Amount `25000.00`, `Credit`, Envelope seçilmemiş, Date `08/03/2026`; alt toast "Select an Envelope." | Uyumlu: zarf seçilmeden kaydın reddedildiği (form 34, 182) karede. **"`Available` havuzuna doğrudan yatırma bu formda yok"** bu kareden çıkmıyor: zarf açılırının seçenekleri görünmüyor. Kare yalnız `Credit` türünde zarfın zorunlu olduğunu gösterir; normal gelir yolu (B05) hakkında bilgi vermez | Ana — zorunlu zarf reddi | GB-M04, GB-M11; GB-Q01, GB-Q05; B05 |
| E0118 | Aynı form üzerinde diyalog, 09:18: "Save Location? Goodbudget will suggest this payee when you're at this location. Select Allow on the next dialog." `CANCEL` / `NEXT`. Arkada Amount `25000.00` `Credit`, Envelope **`Market [850.00 left]`**, Account `Ana Hesap [20,000.00]` | Uyumlu: alıntı doğru (form 183), yalnız ikinci cümle (sistem izin diyaloğuna yönlendirme) formda yok. Arka plan ₺25.000 Credit'in Market zarfına yazıldığının (form 34) tek karesidir. Önerinin çalıştığı, konumun kaydedildiği veya hangi düğmeye basıldığı görünmüyor; beyan olarak kalır | Ana — Credit → Market bağı; Ek — konum önerisi beyanı | GB-M04, GB-M08, GB-M11; GB-Q01, GB-Q08; B05 |
| E0119 | `Account Transfer`, 09:29: `From` / `To` `- Select Account -`, `Amount 0.00`, `Description: Account Transfer` önceden dolu, `Date 09/11/2026`, `Schedule this…`, `Notes (Optional)` | Uyumlu: alan seti ve otomatik açıklama (form 37, 255) doğru. Formda anılmayan: transfer formunda **zarf alanı yok**. Bu, transferin zarf katmanına yazılmadığını düşündürür ama raporda nötr olduğunu kanıtlamaz (GB-Q06) | Ana — ayrı transfer türü | GB-M05, GB-M11; GB-Q06 |
| E0120 | Aynı form, 09:30: `From` açılırı açık, seçenekler `- Select Account -` ve `Ana Hesap [42,950.00]` | Uyumlu: tek hesap ve 42.950 kontrol değeri (form 63) karede. "`From`/`To` açılırları aynı tek kaydı listeliyor" cümlesinin yalnız `From` yarısı görünüyor; `To` açılmadı | Ek — kontrol değeri 09:30 | GB-M05, GB-M07; GB-Q02, GB-Q06 |
| E0121 | `REPORTS`, 09:33: `Spending by Envelope` 1–30 Eyl 2026, gri pasta, `Total Spending 0`; `Income vs Spending` 1–30 Eyl, `Income 2,050` / `Spending 0` / `Net Total 2,050`, yalnız `Sep` çubuğu | Uyumlu: raporun cari takvim ayında açıldığı ve Eylül'ün 2.050 gelir gösterdiği (form 38, 95) doğru. Kare, varsayılan dönemin cari ay olduğunu tek açılışta gösterir; her açılışta böyle olduğu genellemedir | Ana — varsayılan dönem ve fonlamanın gelir sayılması | GB-M06, GB-M08, GB-M11; GB-Q01, GB-Q02 |
| E0122 | `Spending by Envelope` tam ekran, 09:34: 1–30 Eyl 2026, gri pasta, `Total Spending: 0.00`, "No transactions found.", üst barda takvim simgesi | Uyumlu: boş durum ve takvimle dönem değiştirme girişi (form 177, 181) doğru. Takvimin açıldığı ekran karede yok | Arşiv — E0121/E0123 arası geçiş; boş durum için Ek | GB-M06, GB-M11 |
| E0123 | Aynı rapor, 09:36: 1–31 Ağu 2026, tamamen yeşil pasta, `Total Spending: -22,950.00`, tek satır `Tasarim Yazilimi 100% 1,200.00` | Uyumlu: negatif toplam ve tek satır (form 38, 96–101) doğru. **Zaman sırası GB-Q02'yi kısmen cevaplıyor:** 09:30'da hesap 42.950'ydi (E0120) ve bu değer 600'lük B1 kaydını içermiyor, yani 09:36 raporu 600 yazılmadan önce çekildi (B1 zamanı E0126 ile doğrulanacak). −22.950 = 1.200 + 850 − 25.000 bununla tutarlı; K07'deki "Ağustos'ta 2.650 gider" (form 38) ise 600'ü de sayıyor ve bu raporun anına ait değil. Market'in listede görünmemesi, zarfın net harcamasının negatif (−24.150) olmasıyla da açıklanabilir; iki açıklama aynı toplamı verir, doğrulanmadı | Ana — rapor işaretinin dönmesi | GB-M06, GB-M08; GB-Q02, GB-Q05; B05 |
| E0124 | `Income vs Spending` tam ekran, 09:37: 1 Tem – 30 Eyl 2026 tablosu `Jul 0.00 / 0.00 / 0.00`, `Aug 0.00 / -22,950.00 / 22,950.00`, `Sep 2,050.00 / 0.00 / 2,050.00`; çubuk grafikte Ağustos kırmızı çubuğu aşağı, Eylül yeşil çubuğu yukarı | Uyumlu: tablo değerleri (form 38, 102–104, 152–154) birebir. Formda anılmayan: Ağustos çubuğu grafikte yaklaşık −19.5k çizgisinde bitiyor, tablodaki −22.950 ile görsel olarak uyuşmuyor (çizim ölçeği veya hata; doğrulanmadı). E0123 ile aynı checkpoint (600 öncesi) | Ana — gelirin harcama eksenine düşmesi | GB-M06, GB-M08; GB-Q02, GB-Q05; B05 |
| E0125 | `Edit Transaction` üzerinde silme diyaloğu, 09:43: Payee `K08 Test`, Amount `13.00`, `Expense`, Envelope `Market [24,984.00 left]`, Account `Ana Hesap [42,934.00]`; "Are you sure you want to delete this transaction?" `NO` / `YES`; üst barda ✓ ve çöp kutusu | Kısmen uyumlu: tek onaylı silme ve formda 42.934 (form 39, 70) doğru. Köşeli parantez değerleri 16'lık ilk tutarı yansıtıyor: 42.950 − 16 = 42.934; Market 850 + 25.000 − 850 − 16 = 24.984. Bu, 25.000 Credit'in Market zarfına eklendiğini ikinci kez doğrular. **"Düzenleme anında, onaysız kaydediliyor"** (form 39) karede desteklenmiyor: 13 yazılı ama bakiyeler hâlâ 16'yı gösteriyor; 13'ün hiç kaydedilip kaydedilmediği ve `YES`'e basıldığı görünmüyor | Ana — silme onayı ve bakiye izi | GB-M06, GB-M07; GB-Q04; B06 |
| E0126 | `Add Transaction`, 09:50: Payee `Bulut Yazilim Abonelik`, `600.00`, `Expense`, Envelope `Market [24,984.00 left]`, Account `Ana Hesap [42,934.00]`, Date `08/10/2026`, `Schedule this…` işaretli `Every 2 Weeks`; önizleme "…ansactions will be on: … 2026, Eyl 7, 2026, Eyl 21, 2026"; "…mail 3 days before"; `Save location to transaction` işaretsiz; önizlemenin başı klavye araç çubuğunun altında | Uyumlu: 12 Eyl düzeltmesi (form 120, 136–140) doğru, `Every 2 Weeks` ve Eyl 7/21 karede. Zaman bilgisi: 09:43 silmeden sonra hesap hâlâ 42.934, yani 16 geri gelmemiş (form −16 ile tutarlı); B1 09:50'de kuruldu, E0123/E0124 bundan önce çekildi. **GB-Q03 için sınır:** 10 Ağu'dan iki haftada bir tarihler 24 Ağu ve 7 Eyl'i de geçmişte bırakıyor; iki geçmiş örnek otomatik yazılsaydı fark −1.200 olurdu, gözlenen −600. "Sonraki örnek" açıklaması bununla tam uyuşmuyor; neden hâlâ bilinmiyor. Plan `Market` zarfına bağlı | Ana — tekrar formu ve önizleme | GB-M09, GB-M13; GB-Q03, GB-Q08; B06 |
| E0127 | `Transaction Search`, 09:54: "Searching for: Bulut", tek sonuç `08/10 Bulut Yazilim Abonelik 600.00`, alt satır `Market \| Ana Hesap`; `+` | Uyumlu: B1'in ilk örneğinin gerçek kayıt olarak aramada çıktığı (form 120) doğru. "Arama hızlı" (form 38, 180) karede ölçülemez; yalnız sonucun doğru olduğu görülür. Kayıt 11 Eyl'de tek örnek beklenen anda alındı; 12 Eyl'deki E0132 ile karşılaştırılır | Ek — ilk örnek kayıt | GB-M06, GB-M09, GB-M11; GB-Q03, GB-Q11 |
| E0128 | Boş `Add Transaction`, 09:55: overflow menüsü açık, tek seçenek `Help`; Payee yer tutucusu "Whom did you pay?", `Expense`, Account `Ana Hesap [42,334.00]`, Date `09/11/2026`; alt alanlar klavye araç çubuğunun altında | Uyumlu: menüde yalnız `Help` olduğu (form 119) doğru. Kare yalnız bu formun menüsünü gösterir; "dosya/foto eki hiç yok" Settings açılmadan bu yüzeyle sınırlıdır, "incelenen uygulamalar arasında tek" ifadesi başka formlara dayanır (GB-Q09). Formda anılmayan: 09:55'te hesap 42.334, yani 600 ve silinmeyen 16 bu anda zaten düşülmüş | Ek — fiş/ek yokluğu, sınırlı | GB-M09, GB-M11; GB-Q09 |
| E0129 | `ACCOUNTS`, 09:56: `All Accounts: 42,334.00`, `Checking, Savings, or Cash` altında `Ana Hesap 42,334.00`, `Subtotal: 42,334.00`, `GET UNLİMİTED ACCOUNTS! »` | Uyumlu: 42.334 (form 64) doğru. −16 yorumu (form 69–72) kareler zinciriyle tutarlı: 13'lük düzenleme kaydedilip silinseydi veya silme 16'yı geri alsaydı sonuç 42.350 olurdu. Bu çıkarım silmenin gerçekten yapıldığına dayanır; kaydın listede olmadığı E0130'da görülür. İç mekanizma bilinmiyor | Ana — mutabakat sapması −16 | GB-M06, GB-M07; GB-Q04; B06 |
| E0130 | `TRANSACTİONS`, 12:20, beş satır ve altında boşluk (liste tam): `09/11 Initial Envelope Fill +2,050.00` (alt satırsız), `08/10 Bulut Yazilim Abonelik 600.00 Market \| Ana Hesap`, `08/08 Mavi Yazilim 1,200.00 Tasarim Yazilimi \| Ana Hesap`, `08/05 Market 850.00 Market \| Ana Hesap`, `08/03 Ada Reklam +25,000.00 Market \| Ana Hesap`; üst barda arama simgesi | Uyumlu: beş satır, fonlama satırı ve Credit'in Market'e bağlı olduğu (form 49–59, 91–95) doğru. `K08 Test` listede yok; E0129'daki −16 yorumunun "silme yapıldı" öncülünü doğrular. 12 Eyl tarihi karede görünmüyor, form beyanıdır. Bu an itibarıyla −600'ü açıklayan ikinci bir listelenmiş satır yok | Ana — fonlamanın işlem satırı olması; listenin tamamı | GB-M04, GB-M07, GB-M08, GB-M10; GB-Q01, GB-Q02, GB-Q03, GB-Q04; B05, B06 |
| E0131 | `ACCOUNTS`, 12:21: `All Accounts: 41,734.00`, `Ana Hesap 41,734.00`, `Subtotal: 41,734.00` | Uyumlu: 41.734 (form 65) doğru. Karede tarih yok; iki gün arasında uygulamaya dokunulmadığı form beyanıdır. 42.334 → 41.734 farkı yalnız E0129 ile karşılaştırınca görülür | Ana — mutabakat sapması −600 | GB-M07, GB-M13; GB-Q03; B06 |
| E0132 | `Transaction Search`, 12:23: "Searching for: Bulut", tek sonuç `08/10 Bulut Yazilim Abonelik 600.00 Market \| Ana Hesap` | Uyumlu: tek satır (form 75–76). E0127 ile birebir aynı içerik; bakiye 600 düşmüşken aramanın değişmediğini gösterir. Aramanın planlanmış/otomatik örnekleri kapsayıp kapsamadığı bilinmediği için nedenin kanıtı değildir (B06 kaynak ölçütü) | Ek — E0127 ile çift | GB-M07, GB-M11; GB-Q03; B06 |
| E0133 | Boş `Add Transaction`, 12:24: Date **`09/12/2026`**, Account `Ana Hesap [41,734.00]`, `Schedule this…` işaretli, sıklık açılırı açık: `Once` · `Weekly` · `Every 2 Weeks` · `Every 4 Weeks` · `Every Month` · `Last Day of Month` · `Every 2 Months` · `Every 3 Months` · `Every 6 Months` ve kesilmiş, okunamayan bir seçenek daha; "…mail 3 days before" | Uyumlu: dokuz seçenek ve ekrana sığmayan en az bir seçenek (form 120) doğru. Formdaki 12 Eyl tarihlerini doğrudan destekleyen tek kare budur (tarih alanı bugünü gösteriyor); 12:20–12:23 kareleri aynı oturumun devamıdır. **"Varsayılan `Once`"** karede görünmüyor: kutu işaretli ve açılır açık | Ek — sıklık seçenekleri | GB-M09, GB-M10; GB-Q03, GB-Q10 |

**Paket sonucu:** 28/28 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 17,
Ek 7, Arşiv 4; hiçbir görsel silinmedi veya yeniden adlandırılmadı. Formla
metin arasında bulunan uyuşmazlıklar ve soru ilerlemesi E0392
P1-goodbudget-G01 bölümündedir.


## Goodbudget kaynak eki — 14 Eylül 2026

Yerel E kimlikleri yeniden numaralanmadı. Aşağıdaki URL kimlikleri kaynak
incelemesidir, PNG/canlı test sayısına eklenmez; erişim 14 Eylül 2026.

| Kimlik | Kaynak | Bağlam / kullanım |
|---|---|---|
| GB-S01 | https://goodbudget.com/help/getting-started-guide/step-3-add-income/ | Android Fill Envelopes / Keep Unallocated; sayfa etiketi 21 Şubat 2020; B05, Belge 2; GB-U01-A/C sürüm ekranı eklendi; etiketi Keep Available |
| GB-S02 | https://goodbudget.com/help/using-accounts/getting-started-with-accounts/ | Bir ana hesap ve sınırsız bütçe dışı Debt; sayfa etiketi 1 Şubat 2020; B07, Belge 1/2 |
| GB-S03 | https://goodbudget.com/help/using-accounts/link-bank-accounts/ | Premium banka senkronizasyonu; sayfa etiketi 5 Şubat 2020; B07, Belge 2; bağlantı kurulmadı |

G01'deki 28 görsel incelemesi korunur; formdaki düzeltme adayları uygulanmıştır.
Önceki hash'ler kendi yakalama anlarına aittir; değişen Markdown dosyalarının
güncel hash'iyle aynı oldukları iddia edilmez. PNG'ler bu tur değiştirilmedi.


## GB-U01 kullanıcı kanıtları — 14 Eylül 2026

Kullanıcının konuşmada gönderdiği üç PNG doğrudan incelendi ve değiştirilmeden
arşivlendi. SHA-256 karşılaştırmasında eski 28 PNG ile birebir kopya bulunmadı.
E kimlikleri korunur; ekler aşağıdaki sabit GB-U01 kimlikleriyle izlenir.
Platform: mevcut Android emülatörü; bu karelerde sürüm numarası görünmüyor.
Yöntem: kullanıcı ekran kontrolü; yeni işlem kaydı/finansal sonuç testi değil.

| Kimlik | Dosya | Gösterdiği / kullanım | SHA-256 |
|---|---|---|---|
| GB-U01-A | [kanitlar/goodbudget/27-fill-from-new-income.png](kanitlar/goodbudget/27-fill-from-new-income.png) | From New Income: gelir kaynağı, tutar, hesap, tarih ve dağıtım seçimi; 14:55; Belge 1/2, B05 | 0c3650407147656cda0d57178da5536a9dffbd1e6ad003f8be986680f463bace |
| GB-U01-B | [kanitlar/goodbudget/28-fill-from-available.png](kanitlar/goodbudget/28-fill-from-available.png) | From Available: mevcut 20.000, kullanılan 0 ve zarf dağıtımı; 14:56. Sağ altta ekran aracı kısmen örtüyor; ana alanlar görünür; Belge 1/2, B05 | d25b1e82254a657afd75d04108d1c6fd13104c7fbfc8faa08d34d3291a4e9faa |
| GB-U01-C | [kanitlar/goodbudget/29-income-keep-available.png](kanitlar/goodbudget/29-income-keep-available.png) | Fill Each Envelope / Keep Available seçenekleri; Keep Available seçili; 14:57; Belge 1/2, B05 | 9c0e9bbd1fc9b85cf611b5d4bff1963a29ba65b4e98a060d85193c8f4c745dde |

Güncel fiziksel toplam 360 PNG; incelenen 31 (eski korpus 28/357 + kullanıcıdan 3/3).
Goodbudget 31/31; diğer uygulamaların 329 görseli bu tur incelenmedi.


## P1-kolaybi-G01 — 14 Eylül 2026

Mevcut görseller doğrudan açılarak incelendi. Mobil giriş dışında video/destek kaynaklı karelerdir; canlı davranış testi değildir. Rol seçimi rapor önerisidir; E kimlikleri değişmez.

| Kanıt | Dosya | Rol | İçerik, metin karşılaştırması ve sınır |
|---|---|---|---|
| E0180 | d32-giris-ekrani.png | Ana | Giriş: E-Posta/Şifre zorunlu yıldızları, göz, Beni Hatırla, Giriş Yap, QR ve parola sıfırlama, TR/EN bayrakları. Kayıt bağlantısı karede yok; QR eşleşmesi ve dil değiştirme çalıştırılmadı. |
| E0181 | d33-video-guncel-durum-panosu.png | Ana | Güncel Durum: 13 sol modül, nakit akışı, Mayıs 2020 takvimi, üç vade etiketi ve tutarlar, işlem sayaçları. Ödemeler satırının altı kesik; her iki satırın bütün değerleri bu kareden okunamaz. Takvim içerik tarihidir, video yayın tarihi kanıtı değildir. |
| E0182 | d34-video-gunu-gelen-islemler.png | Ek | Günü Gelen İşlemler yakın görünümü: Bugün/Yaklaşanlar/Tarihi Geçenler; Faturalar açık, $118,00 ve 71 Gün Gecikti. Adet rozetleri var. Diğer iki grup kapalı; sayaç renklerinin anlamı ve BF model eşdeğerliği bu kareden çıkmaz. |
| E0183 | d35-video-vadesi-belirsiz-tahsilatlar.png | Ana | Vadesi Belirsiz Tahsilatlar: fatura/yerel tutar/bakiye/durum ayrı kolonlar. TEST MAİLSİZ CARİ 118.000 / 116.741 ve Kısmen Ödendi. Dolu demo satırıdır; ödeme öncesi/sonrası veya gerçek müşteri verisi kanıtı değildir. |
| E0184 | d36-video-cari-hesaplar.png | Arşiv | Genel Cari Hesapları: dört sekme, tek sıfır bakiyeli demo cari, içe/dışa aktarma ve detaylı arama. Ortaklar sekmesi kapalı. Eski görünüm referansı; ürünün başından beri ortak parası modeli olduğu kanıtlanmaz. |
| E0185 | d37-video-urun-ve-hizmetler.png | Ek | Ürün Ve Hizmetler: Tümü/Ürünler/Hizmetler; boş tablo, KDV ve İndirim kolonları, oluşturma/aktarma. KDV'nin başka forma otomatik taşınması kanıtlanmaz. Alt URL test-ofis bağlamını gösterir. |
| E0186 | d38-video-finans-kasalar.png | Ek | Finans/Kasalar: Banka Hesapları, Kasalar, Online Banka Hesapları, Çekler; Ana Kasa, 03.03.2020, TRY, sıfır bakiye. Banka ekranıyla aynı sekme değildir; menüde görünmeyen kartı ürün genelinde yok saydırmaz. |
| E0187 | d39-guncel-arayuz-2026.png | Ana | KolayBi Eğitim panosu: ayrı Fatura Ödeme, ayrıcalık kartları, iWallet/müşavir daveti; Son 1 Haftalık Nakit Akışı, 1.8.2024 sıfır tooltip. İçerik tarihi yayın/sürüm tarihi değildir. Özetin altı kesik; menü benzerliği altı yıllık değişmezlik kanıtı değil. |
| E0188 | d01-destek-proje-listesi.png | Ana | Projeler: kod/ad/etiket/para/durum/tarih/gelir/gider/net; Ev Elektrik ve Bebek Bakım gibi demo adları. Hane ihtiyacının gerçekliği veya amaç dışı kullanım kanıtlanmaz. Liste d03'teki Proje1'i göstermiyor; aynı sayıya iki ad iddiası desteklenmez. |
| E0189 | d02-destek-yeni-proje-formu.png | Ana | Yeni Proje: kod dolu, ad/para zorunlu, başlangıç-bitiş, açıklama, Devam Eden Proje kapalı, Vazgeç/Kaydet. Kodun otomatik üretimi ve proje sayısının sınırsızlığı tek kareden çıkmaz. |
| E0190 | d03-destek-proje-detay-ozet.png | Ana | Proje1 özeti: Aktif/Pasif menüsü, ayrıca çöp kutusu; sekmeler ve Kar/Zarar–Nakit Durumu; toplam/tahsil edilen/ödenen/bekleyen kırılımları sıfır. Arayüz ayrımı kanıtlı; nakit sekmesi içeriği, tanıma zamanı ve silme yerine pasifleştirme kuralı kanıtlanmaz. |
| E0191 | d04-destek-proje-belge-kirilimi.png | Ana | Belge kartları: satış/alış/iade/para girişi/genel gider; iade başlıklarında -/+ ve ters renk; tutarlar sıfır. Görünen renk/başlık deseni kanıtlı; net formülü, gerçekleşen nakit etkisi ve BF sourceGroup eşdeğerliği değil. |
| E0192 | d05-destek-yeni-gider-formu.png | Ana | Yeni Genel Gider: Cari Takibi Yok/Var, Ödenmedi/Ödendi, vadesiz seçimi, proje kapatma yardım metni, açıklama şablonu, 5 MB dosya alanı. Alt bant kısmen kesik ve iki Toplam KDV başlığı var; hesaplama/tevkifat açıklaması bilinmiyor. Radyo seçimi veritabanı/çifte sayım davranışını kanıtlamaz. |
| E0193 | d06-destek-gider-tipleri.png | Ana | Kategori kartları ve tip satırları: Ulaşım/Konaklama, Temel Giderler, Vergi, Diğer; Kategoriyi Düzenle/Yeni Tip/Yeni Kategori. Üst kategori başlıkları kesik. Vergi adlı kart olması ayrı vergi altyapısını dışlamaz. |
| E0194 | d07-destek-cari-listesi.png | Ana | Cari listesi: beş sekme, telefon sütunu, dört cari tipi, işaretli/renkli yerel bakiye, favori ve toplu seç. 1000 kayıt mesajı daha fazlası için detaylı arama diyor; toplam ürün kapasitesi değildir. Borç/alacak işaret yönü bu karede açıklanmıyor. |
| E0195 | d08-destek-cari-olusturma-formu.png | Ana | Cari Detay Bilgileri: Vade Günü ve Sabit İskonto Yok/Var; Açılış Bakiyesi tutar/para/durum/proje/tarih; vade seçimi ve Borç Alacak Ekle/Banka Ekle. Durumu açılırı kapalı; gelir/gider etkisi, cari varsayılan vade aktarımı ölçülmemiş. |
| E0196 | d09-destek-cari-detay-ekstre-dialog.png | Ana | Cari Ekstresi Oluştur: tarih aralığı, dikey düzen, para birimi, açıklama, yedi isteğe bağlı sütun anahtarı, Kapat/Yazdır/Oluştur. Arka menüde hem pasifleştir hem sil var. Anahtarların gerçekten çıktıya etkisi veya silme kuralı kanıtlanmaz. |
| E0197 | d10-destek-cari-ekstre-onizleme.png | Ana | PDF önizleme: tek sayfa Cari Hesap Ekstresi, 1.800 borç/0 alacak/1.800 bakiye; yeni sekme, e-posta ve kapat. Doküman önizlemesi kaynakta var; bizim PDF üretim/gönderim testi değil. Önceki form bitişi 26.09 iken çıktıda tarih aralığı 25.09; ardışık aynı işlem garantisi yok. |

**Paket sonucu:** 18/18 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 14,
Ek 3, Arşiv 1; hiçbir görsel silinmedi, taşınmadı veya yeniden adlandırılmadı.
Form düzeltmeleri, bağlı soruların ilerlemesi ve kalan sınırlar E0392
P1-kolaybi-G01 bölümündedir.

Güncel fiziksel toplam 360 PNG; incelenen 49 (eski korpus 46/357 + kullanıcıdan 3/3).
Goodbudget 31/31, KolayBi 18/39; sonraki kimlik E0198 (P1-kolaybi-G02).


## P1-kolaybi-G02 — 14 Eylül 2026

Destek mockup'ları doğrudan açılarak inceleniyor; canlı davranış testi değildir.
Ad sütunu güncel dosya adıdır; E kimlikleri değişmez. 14 Eylül'de yanlış
anlaşılan bir istekle bu 31 kare yeniden adlandırıldı ve aynı gün geri alındı.

| Kanıt | Dosya | Rol | İçerik, metin karşılaştırması ve sınır |
|---|---|---|---|
| E0198 | d11-destek-gider-listesi.png | Ana | Genel Giderler listesi: üç sekme, Detaylı Arama, 1000 kayıt notu, Toplu Seç, Genel Gider Oluştur vurgulu. Sekiz satır; hepsinde Cari Bilgisi "Genel Gider", Proje ve Son Ödeme Tarihi boş. Bakiye deseni: iki Ödendi satırı tutar=bakiye, beş Ödenmedi satırı ₺0, Bedelsiz 1.250/1.250. Formun Ödenmedi listesi beşinci satırı (1.250/₺0) atlıyor. e-Fatura "Aktarıldı" yalnız iki Ödendi satırında; ilişki gözlemdir, kural değil. Kolon anlamı doğrulanmadı. Ad uygun. |
| E0199 | d12-destek-gider-detay-islemler.png | Ana | Genel Gider detayı: Bedelsiz/Yeni/Doğalgaz/Tahsilata Kapalı rozetleri; Durum Değiştir ve Ödeme Ekle kapalı; İşlemler açık (Gönder, Tekrarlı Genel Gidere Dönüştür vurgulu, Tahsilata Aç, Düzenle, Kopyala, Sil); dört sekme; GENEL GELİR/GİDER BİLGİLERİ ve boş PROJE BİLGİLERİ; 22.09.2023; Yerel Toplamlar 0,00. d11'deki Doğalgaz satırıyla (29.07, ₺2.000 Ödendi) aynı kayıt değil. Başlıktaki "GELİR/GİDER" ifadesinden serbest gelir kaydı sonucu çıkarılmaz; Taslağa Al bu karede yok (d17'de). Ad uygun. |
| E0200 | d13-destek-alis-faturasi-formu.png | Ana | Yeni Alış Faturası: beş satın alma sekmesi; Cari* (zorunlu), Cari Adresi, Düzenlenme Tarihi 22.09.2023, Düzenlenme Saati* 16:46, Seri No, Ödeme Durumu Ödenmedi seçili, Vade Tarihi "Vade Günü Girilmemiştir", Para/Takip Para Birimi TRY; Proje notu, Etiketler, Açıklama şablonu, 5 MB dosya; ÜRÜN/HİZMET BİLGİLERİ başlığı, tablo kesik. Sol menüde Raporlar da vurgulu görünüyor (mockup üzerindeki imleç izi olabilir). Formla uyumlu. Ad uygun. |
| E0201 | d14-destek-personel-carileri.png | Ana | Personel Cari Hesapları: beş cari sekmesi, Personel Carileri vurgulu; altı satır; EMP000037–40 ve iki CAR00001; tipler Serbest/Yarı/Tam Zamanlı; Telefon boş, VKN hepsinde 123456789 (demo); bakiyeler ₺0, -₺100 (kırmızı, Ceren Günay), ₺85.809,72 ve ₺1.563,94 (yeşil). Listede Helin Kınay Serbest Çalışan ve ₺0,00; d15'te aynı ad Yarı Zamanlı ve -₺100 gösteriyor: iki kare aynı veri anının ardışık görüntüsü sayılmaz. Ad uygun. |
| E0202 | d15-destek-personel-cari-detay.png | Ana | Helin Kınay detayı: Yarı Zamanlı Çalışan/Aktif Cari/Genel Müdür Yardımcısı rozetleri; beş eylem düğmesi; İşlemler açık (Mahsuplaştır, Per. Cari Ekstresi Oluştur, Sık Kullanılanlara Ekle, Cariyi Pasifleştir, Sil); TRY/Toplam; Toplam Borç ₺0 yeşil, Alacak ₺100 kırmızı, Bakiye -₺100; iki maddelik proje nakit uyarısı; hareket tablosu başlığı, satırlar kesik. Ödeme Yap alt menüsü bu karede kapalı (d16'da açık). Uyarı metni formdaki alıntıyla aynı; projeye akışın fiilen gerçekleştiği canlı doğrulanmadı. Ad uygun. |
| E0203 | d16-destek-maas-odeme-secimi.png | Ana | Aynı sayfa karartılmış; Ödeme Yap menüsü (Maaş Ödemesi Yap, Prim Ödemesi Yap, Avans Ver) ve "Ödeme yapmak istediğiniz maaşı seçiniz." diyaloğu: iki 22.04.2023 ₺1.000,00 satırı, gri ₺1.000,00 rozeti, Kapat. Diyalog "ödenmemiş" demiyor; listelenenlerin ödenmemiş/kalan tahakkuk olduğu çıkarımdır. İki ₺1.000 satırı ile -₺100 toplam bakiye aynı demo anında uzlaşmıyor. Seçim sonrası ödeme formu görünmüyor. Ad uygun. |
| E0204 | d17-destek-calisan-maasi-detay.png | Ana | Çalışan Maaşı: Ödenmedi/Yeni; Ödeme Ekle; İşlemler açık (Gönder, Tekrarlı Maaşa Dönüştür vurgulu, Taslağa Al, Düzenle, Sil); dört sekme; boş CARİ BİLGİSİ; Düzenlenme ve Maaş Ödeme Tarihi 22.04.2023; Yerel Toplamlar Ara/Brüt/Net/Genel hepsi 1.000,00. Brüt ile net aynı; kesinti hesabı görülmedi, "bordro mantığı" alan adından çıkarımdır. Cari bilgisi boş olduğu için hangi personele ait olduğu karede yok. Ad uygun. |
| E0205 | d18-destek-tekrarli-maas-formu.png | Ana | Tekrarlı Maaş Oluştur diyaloğu, d17 Çalışan Maaşı sayfasının üstünde; arkada İşlemler > Tekrarlı Maaşa Dönüştür vurgulu. Oluşturma Periyodu* boş (değerler görünmüyor), Başlangıç Tarihi 22.04.2023, Maaş Ödeme Tarihi Belirsiz seçili / Belirli, Maaş Oluşturma Tekrar Sayısı* boş; Kapat/Kaydet. Formdaki "varsayılan bugün" desteklenmiyor: tarih kaynak maaşın tarihiyle aynı. Diyalog bağımsız plan formu değil, mevcut kayıttan dönüştürme yolu. Onay adımı yokluğu çıkarımdır. Ad genel kalıyor (kare bir dönüştürme diyaloğu). |
| E0206 | d19-destek-banka-hesaplari.png | Ana | Finans altı sekme (Banka Hesapları, Kasalar, Kredi Kartları, Online Banka Hesapları, Çekler, Senetler); Banka Hesabı Ekle vurgulu; Toplam ve TRY Bakiye ₺19.543,53; tek satır Türkiye İş Bankası, Ticari etiketi, Banka-Şube ve IBAN boş, açılış 24.03.2023, TRY, ₺19.543,53. Bakiyenin d29 Güncel Bakiye ile eşitliği iki karede görülür; aynı veri anı olduğu kanıtlanmaz. Altı sekme formla uyumlu; sekmelerin eklenme zamanı bu kareden çıkmaz. Ad uygun. |
| E0207 | d20-destek-kredi-kartlari-listesi.png | Ana | Kredi Kartları: Kredi Kartı Ekle vurgulu; kolonlar Kredi Kartı Adı, Etiketler, Kart Numarası, Hesap Kesim Günü, Son Ödeme Günü, Kart Limiti, Ek Bilgiler, Açılış Tarihi, Para Birimi, Kalan Limit; "Tabloda herhangi bir veri mevcut değil / Kayıt yok". Formla uyumlu; Kalan Limit hesabı çıkarım. Ad uygun. |
| E0208 | d21-destek-yeni-kredi-karti-formu.png | Ana | Yeni Kredi Kartı: Ad*, Etiketler, Kart Numarası*, Hesap Kesim Günü* ve Son Ödeme Günü* açılır, Açılış Tarihi* 26.09.2023, Para Birimi* TRY, Kart Limiti*, Minimum Ödeme Oranı (%)*; Detay ekle kapalı; Vazgeç/Kaydet (vurgulu). Formdaki alan listesiyle uyumlu; kaydetme ve borç/ödeme davranışı görülmedi. Ad uygun. |
| E0209 | d22-destek-cekler.png | Ek | Çekler: Toplu Çek Ekle vurgulu, Bordrolar, Dışarıya Aktar; kolonlar Alış/Veriliş Tarihi, İşlem Türü, Proje, Cari, Seri No, Keşideci, Hamil, Keşide (Vade) Tarihi, Tutar, Durum, İşlem Tarihi, Etiketler; tablo boş. Formla uyumlu; çek durumu/ciro akışı görülmedi. Ad uygun. |
| E0210 | d23-destek-senetler.png | Ek | Senetler: Senet Gösterimi anahtarı "Toplam Özet" açık, ciro edilmiş senet notu; Senet Ekle vurgulu, Dışarıya Aktar, Bordrolar; kolonlar Alış/Veriliş Tarihi, İşlem Türü, Cari, Keşideci, Hamil, Kefil, Tutar, Ödenen Toplam Senet Tutarı, Taksit Durumu, Kalan Toplam Senet Tutarı; Proje kolonu yok; tablo boş. Taksit/kısmi ödeme yalnız kolon adından çıkarım. Ad uygun. |
| E0211 | d24-destek-guncel-durum-panosu.png | Ana | Güncel Durum: üç sekme; Size Özel Ayrıcalıklar yedi kutucuk (TotalEnergies ve OYAK YENİ, DijitalKöprü, adı kısmen kesik …urada, KolayBi Banka, ÇiçekSepeti, paynet); Son 1 Haftalık Nakit Akışı 30 Aralık–5 Ocak, iki çizgi, lejant yok; 5.1.2023 ipucu Gelir 2 B ₺ / Gider 1 B ₺ (renk eşlemesi ipucundan: yeşil gelir, kırmızı gider); KolayBi' Yolu Var! (Davet Et & Kazan, iWallet, BiLink, Blog', Sosyal Medya); Günü Gelen İşlemler üç bağlantı + Faturalar 1/0; Tahsilat Ve Ödeme Özetleri başlığı var, içeriği kesik. Sol menüde 13 modül, Fatura Ödeme yok. 08'den eski sürüm olduğu menü farkından çıkarımdır; grafik tarihi yayın tarihi değildir. Ad uygun. |
| E0212 | d25-destek-notlar-hatirlatici.png | Ek | Notlar sayfası karartılmış, Yeni Not Ekle diyaloğu: Başlık*, Not*, Hatırlatıcı Pasif seçili / Aktif, Görünürlük Özel Not / Şirket Notu (seçili, bilgi simgeli); Kapat/Kaydet. Arkada Not Grubu Tümü, Hatırlatma Tarihi Var seçili ve 22.09.202…, Oluşturma/Okunma Tarihi Yok, Not İçeriği, Filtrele/Temizle/Tüm Notlar. Hatırlatıcının bildirim davranışı görülmedi; Şirket Notu varsayılanı yalnız bu karenin durumu. Ad uygun. |
| E0213 | d26-destek-alis-satis-raporu.png | Ana | Alış/Satış Raporları: on rapor sekmesi iki satırda (bu karede de tamamı görünüyor); alt sekmeler Alış/Satış Raporları, Cari, Proje, Ürün ve Hizmetler, Etiket; filtreler Cari, Ödeme Durumu, Para Birimi, Şube, Fatura tarihi 08.08–15.08.2023, Fatura Tipi, Etiketler, Proje, Vade 08.08–15.08.2023 (kutular işaretsiz); KDV Dahil açık; Satış Faturası ₺20.000, Alış Faturası -₺12.000, iade ve Genel Gider boş, Toplam ₺8.000; altta beş belge sekmesi, Satış İade Faturası seçili. Aritmetik tutuyor. Tarih aralığı d27/d28'den (19–26.09.2023) farklı; raporlar aynı veri kesiti sayılmaz. Ad uygun. |
| E0214 | d27-destek-kdv-raporu.png | Ana | KDV Raporu 19.09–26.09.2023, Matrahlı anahtarı açık; üst blok Satış Faturası + Alış İade Faturası → Hesaplanan KDV Toplam; oran sütunları %20/%18/%10/%8/%1/Diğer/Toplam, her birinde Matrah ve Tutar; yalnız %20 Satış Faturası 12.000,00 / 2.400,00 dolu. Alt blokta Alış Faturası, Satış İade Faturası ve adı kesik üçüncü satır; alt bloğun toplam satırı ve "İndirilecek" etiketi karede görünmüyor. Tutarın orandan hesaplandığı mı girilen KDV'nin toplandığı mı ekrandan çıkmaz (KB-Q13). Satır başlarında + genişletici var. Ad uygun. |
| E0215 | d28-destek-gelir-gider-raporu.png | Ek | Gelir/Gider Raporu 19.09–26.09.2023: Gelirler/Giderler × TRY/USD/EUR/GBP + Yerel Toplam matrisi boş, Toplam ₺0,00; Gelirler/Giderler alt sekmeleri; kolonlar İşlem Türü, Cari Adı, Gelir/Gider İşlem Türü, Belge No, Açıklama, İşlem Tarihi, Tutar, Ödeme Yöntemi, İşleme Verilen Banka; "Kayıt yok". Aynı aralıklı d27'de ₺12.000 satış faturası görünürken bu rapor boş; faturaların bu rapora girmediği veya karelerin farklı veri anında çekildiği ayırt edilemez. Boş rapor içerik kapsamını kanıtlamaz. Ad uygun. |
| E0216 | d29-destek-nakit-akis-raporu.png | Ana | Nakit Akış Raporu: Güncel Bakiye ₺19.543,53, Tahsilatlar ₺143.252,50, Ödemeler ₺0,00 (kırmızı), Tahmini Dönem Sonu Bakiyesi ₺162.796,03; matriste yalnız "Tahsilatlar (TRY)" satırı, Belirsiz ₺143.252,50, Geçmiş ve Eylül 2023–Ağustos 2024 kovaları boş, Toplam ₺143.252,50; belge görüntüleme notu. Aritmetik tutuyor; formül kaynak tanımı değil kareden türetilmiş. Ödemeler satırı matriste yok (değer sıfır olduğu için olabilir). Ad uygun. |
| E0217 | d30-destek-satis-fatura-formu.png | Ana | Başlık "Yeni Alış İade Faturası"; sol menüde Satış Yönetimi, üstte beş satış sekmesi. TEMEL BİLGİLER + E-ARŞİV FATURA BİLGİLERİ; Cari*, Cari Adresi, Düzenlenme Tarihi 27.09.2023, Seri No, Vade Tarihi Yok (seçili)/Var, Para ve Takip Para Birimi TRY; Proje notu, Etiketler, Açıklama şablonu, 5 MB dosya; ÜRÜN/HİZMET BİLGİLERİ ve Ürün Bilgileri + düğmesi. Ödeme Durumu ve Düzenlenme Saati yok. Formdaki üç fark alış iade formu ile alış faturası arasında gözlenir; satış faturası formu hakkında sonuç vermez. Başlık–menü uyuşmazlığı KolayBi görselinde. Dosya adı karedeki başlıkla çelişiyor (formda kare adı uyarısı kayıtlı); ad değiştirilmedi. |
| E0218 | d31-destek-urun-varyantlar.png | Ek | Ürün ve Hizmetler: Tümü, Ürünler, Hizmetler, Depolar, Varyantlar (açık) sekmeleri; içerik yalnız vurgulu "Yeni Varyant" düğmesi, liste/boş durum metni yok. Formla uyumlu; varyant ve depo işleyişi görülmedi. Ad uygun. |

**Paket içerik sonucu:** 21/21 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 16,
Ek 5, Arşiv 0. Adı içerikle çelişen: E0217 (formda uyarı olarak kayıtlı). 31 destek karesinin
adları değişmedi. Kullanıcı kararıyla eski 01–08 kareleri (E0180–E0187) d32–d39
olarak serinin sonuna eklendi; 8 hash önce/sonra aynı. Ana tablo E kimliği sırasını
korur. raporlar/ altındaki tarihsel belgeler değiştirilmedi.

| Kanıt | Eski ad | Yeni ad |
|---|---|---|
| E0180 | 01-giris-ekrani.png | d32-giris-ekrani.png |
| E0181 | 02-video-guncel-durum-panosu.png | d33-video-guncel-durum-panosu.png |
| E0182 | 03-video-gunu-gelen-islemler.png | d34-video-gunu-gelen-islemler.png |
| E0183 | 04-video-vadesi-belirsiz-tahsilatlar.png | d35-video-vadesi-belirsiz-tahsilatlar.png |
| E0184 | 05-video-cari-hesaplar.png | d36-video-cari-hesaplar.png |
| E0185 | 06-video-urun-ve-hizmetler.png | d37-video-urun-ve-hizmetler.png |
| E0186 | 07-video-finans-kasalar.png | d38-video-finans-kasalar.png |
| E0187 | 08-guncel-arayuz-2026.png | d39-guncel-arayuz-2026.png |

Güncel fiziksel toplam 360 PNG; incelenen 70 (eski korpus 67/357 + kullanıcıdan 3/3).
Goodbudget 31/31, KolayBi 39/39.


## P1-B08 — B08 kapsamında düzeltilen belgeler — 14 Eylül 2026

Görsel incelemesi yapılmadı; PNG ve dosya adları değişmedi. Metin düzeltmeleri:
E0008 KolayBi formu (10 yer), E0009 Logo İşbaşı formu (6), E0011 Paraşüt formu
(2), DURUM.md (5; ayrıca G02 KDV cümlesinin yayılımı) ve
raporlar/turk-on-muhasebe-vs-businessfinance.md (2).
Dayanak kanıtlar: E0188 (demo proje adları), E0222 (Logo sektör listesi),
d36/d07 cari sekmeleri. Tablo ve kalan sorular E0392 P1-B08 bölümündedir.
Önceki hash değerleri kendi kayıt anlarına aittir; değişen Markdown'ların
güncel hash'iyle aynı oldukları iddia edilmez.


## P1-B15 — KolayBi kaynak ve tarih rolleri — 14 Eylül 2026

Görsel değişikliği yok. E0180 mobil giriş (manuel), E0181–E0186 ~2020 tanıtım
videosu, E0187 ayrı video (2026 başı yükleme kullanıcı aktarımı), E0188–E0218
destek mockup'ı. Karelerdeki işlem tarihleri demo verisidir, sürüm tarihi
değildir. Metin düzeltmeleri: E0008 KolayBi formu (30 yer) ve DURUM.md (5).
Eşleşme tablosu ve kalan sorular E0392 P1-B15 bölümündedir.


## P1-hesap-defterim-G01 — 14 Eylül 2026

Görseller doğrudan açılarak incelendi. Kaynak: 10 Eylül Tur 1 + A/B/B1/B2
koşumu (Android emülatör, manuel gözlem); durum çubuğu saatleri sıralama
içindir. Canlı yeni test yapılmadı. E kimlikleri ve dosya adları değişmez.
E0144 12 Eylül yeniden çekimidir.

| Kanıt | Dosya | Rol | İçerik, metin karşılaştırması ve sınır |
|---|---|---|---|
| E0134 | 00-magaza.png | Ek | Google Play sayfası, 18:21: Hesap Defterim, ANKIT SARAF, "Reklam içerir • Uygulama içi satın alma", Kaldır/Aç (kurulu), Son güncelleme 23 Ağu 2026 ve yenilik notu; puan verme ve Beta programı bölümü. Formun bu kareye bağladığı 4,8★ / 139 B yorum / 10 Mn+ / PEGI 3 / etiketler ve versionName=235 karede görünmüyor; bu değerler kareyle desteklenmez. |
| E0135 | 01-ilk-acilis-hosgeldin.png | Ana | 17:58 ilk açılış: "Hos geldin — Uygulama nasıl kullanılır?" diyaloğu, TAMAM MI; metin "Ücretli butonuna… Alınan butonuna…" derken ekrandaki butonlar Ödendi/Alındı. Arkada Drive yedekleme daveti (Atla/Yedeklemeyi Aç), reklam kaldırma şeridi, Herşey…Yıllık çipleri, sıfır toplamlar, "Hesap Defterim" defteri. Formla uyumlu. |
| E0136 | 02-bos-ana-ekran.png | Ana | 17:58 boş ana ekran: aynı iki davet şeridi, Tarih/Alındı/Ödendi başlığı, boş liste, Alındı/Ödendi butonları, 0/0/0 toplamlar (Denge 0 mavi), altta "Test Ad" banner. Yönlendirici boş durum metni yok. Formla uyumlu; Denge renginin sıfırda mavi olduğu ek gözlem. |
| E0137 | 03-dolu-ana-ekran.png | Ana | 18:17 Ana Hesap / Herşey: Açılış bilançosu 20.000 (Ağu 01), Web tasarim geliri - Ada Reklam 25.000 (Ağu 03), Kime Ortak Cuzdan 3.000 (Ağu 12), Kime Is Karti 1.200 (Ağu 18); satır başı yürüyen Denge 20.000→40.800; Toplam Alındı 45.000 / Ödendi 4.200 / Denge 40.800. Davet şeritleri ve banner bu karede yok. Kontrol değeri 40.800 ve açılış bilançosunun Herşey görünümünde Alındı satırı olması formla uyumlu. |
| E0138 | 04-islem-formu-alindi.png | Ana | 18:05 Alındı formu: Alındı/Ödendi seçici, tarih Eyl-10-2026 + 06:04 ÖS, tutar 25000 (hesap makinesi ikonu), Notlar "Web tasarim geliri - Ada Reklam" (mikrofon), Fatura ekle, Öğe eklemek, Kaydet ve çık / Kaydet ve devam Et; Açıklama/Kategori alanı yok (ayar kapalı). Tarih alanı 10 Eylül'ü gösteriyor, kayıt listede Ağu 03: kare kaydın son hâli değil, tarih sonradan değiştirilmiş olmalı. Form alanları formla uyumlu; "varsayılan bugün" bu kareyle tutarlı. |
| E0139 | 05-siniflandirma-islem-adlari.png | Ek | 18:18 İşlem adları diyaloğu: Ödendi Alındı / Gelir Gider / Özel radyoları (hiçbiri işaretli görünmüyor), İptal etmek / Kayıt etmek; arkada Ana Hesap 45.000/4.200/40.800. Etiket seçenekleri formla uyumlu; Özel alanları ve yeniden adlandırma sonucu bu karede yok (ek koşum 2 kareleri). Etkin seçimin hangisi olduğu kareden okunamıyor. |
| E0140 | 06b-islemler-butun-hesaplar.png | Ana | 18:13 İşlemler-Bütün Hesaplar / Herşey: Tarih/Hesaplar/Miktar kolonları; Ağu 18 "Kime Is Karti" Ana Hesap 1.200 kırmızı + "Kimden: Ana Hesap" Is Karti 1.200 yeşil; Ağu 12 Ortak Cuzdan transferinin iki satırı 3.000; Ağu 08 Tasarim yazilimi Is Karti 1.200; Ağu 05 Market alisverisi Ortak Cuzdan 850; Ağu 03 Web tasarim geliri 25.000; liste altı kesik. Toplam Alındı 51.200 / Ödendi 6.250 / Denge 44.950. Görünen Ödendi satırları 6.250'yi tam veriyor; görünen Alındı 29.200, kalan 22.000 kesik alandaki açılış satırlarıyla tutarlı ama karede görünmüyor. Transferin iki ayrı satır olması ve toplamlara girmesi formla uyumlu. |
| E0141 | 06-islem-listesi.png | Ana | 18:08 Is Karti / Herşey: Ağu 08 "Tasarim yazilimi - Mavi Yazilim" Ödendi 1.200, satır Denge -1.200; toplamlar 0 / 1.200 / -1.200 (kırmızı); "İşlem Eklendi" toast'u. Kart ödemesinden (18:09) önceki an; K05 "defter bakiyesi -1.200'e düştü" ve başarı toast'u formla uyumlu. Kart borcu/ekstre yokluğu bu tek karede negatif olarak görülür, bütün ürün taraması değildir. |
| E0142 | 07-rapor-aylik-butun-hesaplar.png | Ek | 18:14 İşlemler-Bütün Hesaplar / Aylık, Ağu-01-2026 -> Ağu-31-2026 ve oklar; satırlar ve 51.200 / 6.250 / 44.950 toplamları E0140 (Herşey) ile aynı. Ayrı bir rapor ekranı değil, dönem filtresi uygulanmış birleşik liste; adı "rapor" olsa da içerik liste görünümüdür. Tüm işlemler Ağustos'ta olduğu için Herşey ile Aylık'ın aynı çıkması bu veri koşuludur. |
| E0143 | 08-silinmis-islemler.png | Ana | 18:16 Silinmiş işlemler: tek satır "Tasarim yazilimi - Mavi Yazilim", Is Karti, 1.200, Cmt Ağu 08 2026 06:07 ÖS; altta boş mavi bant. Kayıt 18:13–18:14 karelerinde aktif, 18:16'da çöp kutusunda: K08 silme denemesinin ara anı. Geri Yükle menüsü ve geri yükleme sonrası bu karede yok (ek koşum 2 kareleri); çöp kutusu varlığı formla uyumlu. |
| E0144 | 09-ozgun-ozellik-takvim.png | Ana | 11:39 (12 Eylül yeniden çekim) Takvim, Ağu-01–31-2026: 1: 20.000 yeşil, 3: 25.000 yeşil, 12: 3.000 kırmızı, 18: 1.200 kırmızı, 20: 150 kırmızı, 22: 2.500 yeşil; toplam 47.500 / 4.350 / 43.150 (Ana Hesap defteri), Test Ad banner. Toplamlar hücrelerle tutarlı. Hiçbir günde hem giriş hem çıkış olmadığı için "günde tek net sayı, giriş/çıkış ayrı gösterilmiyor" iddiası bu kareyle doğrulanamaz; yön renkle ve hücre içi konumla (gelir üstte, gider altta) veriliyor. 400'lük eski A bacağı bu defterde görünmüyor. |
| E0145 | 10-hesap-eklem-formu.png | Ana | 18:00 Hesaplar ekranı karartılmış, "Hesap Eklem" diyaloğu: İsim, Açılış bilançosu [İsteğe bağlı], + (seçili) / − radyosu, tarih Eyl-10-2026, İptal etmek / Kayıt etmek; arkada yalnız "Hesap Defterim" defteri ve HESAP EKLEM eylemi. Tür/para birimi alanı yok: formla uyumlu. Tarih varsayılanı bugün; formdaki Ağu-01 açılış tarihi sonradan girilen değerdir. |
| E0146 | 11-hesaplar-defterler-listesi.png | Ana | 18:03 Hesaplar: arama kutusu, HESAP EKLEM; Ana Hesap, Hesap Defterim, Is Karti, Ortak Cuzdan — yalnız adlar, bakiye yok. "Liste bakiye göstermiyor" formla uyumlu. Varsayılan "Hesap Defterim" defteri de listede duruyor: formun "3 defter" anlatımı kullanılan defterleri sayar, cihazda dört defter var. |
| E0147 | 12-aktar-transfer-formu.png | Ana | 18:09 Aktar formu: Miktar 3000, Kimden: Ana Hesap, Kime Ortak Cuzdan, Ağu-12-2026 06:08 ÖS, Notlar boş, Aktar düğmesi. Tek form iki deftere yazar; iki satırın oluştuğu E0140'ta görülür. Kategori/not zorunluluğu yok. Kaydedildikten sonraki satırlarla (06:08 ÖS) tutarlı; formla uyumlu. |
| E0148 | 13-ozet-tasarruf-agustos.png | Ana | 18:12 Is Karti özet görünümü: takvim ve dışa aktarma ikonları; çipler Herşey/Haftalık/Aylık/Yıllık (Günlük yok); Ağu-01–31-2026; Ağu-08 0 / 1.200 / Tasarruf -1.200, Ağu-18 1.200 / 0 / 1.200; toplam 1.200 / 1.200 / Denge 0. Başlıkta "Özet" adı görünmüyor, gün gün özet tablosu olarak tanınıyor. Kart defterinin Ağustos'ta sıfırlandığını gösterir (HD-Q02); dönemden bağımsız sıfır için E0149 ile birlikte okunur. |
| E0149 | 14-yedekleme-nag-dialog.png | Ana | 18:14 Is Karti / Herşey üstünde "Yedekleme kapalı" diyaloğu: telefon değişimi/kaldırma/sıfırlamada kayıt kaybı, Drive'a kendi kopyası, "Kayıtlarınızı sunucularımızda saklamıyoruz, bu nedenle yedekleme kapalıysa verilerinizi geri yükleyemeyiz."; Atla / Yedeklemeyi Aç. Arkada kart defteri 1.200 / 1.200 / Denge 0 (Herşey). Alıntı formla birebir. "Her açılışta tekrarlıyor" sıklığı tek kareden kanıtlanmaz; E0135/E0136 şeridiyle birlikte iki ayrı yüzey görülür. Diyalog beyanı ağ denetimi değildir (HD-Q06). |
| E0150 | 15-ayarlar.png | Ana | 18:19 Ayarlar üst yarı: Tarihi Biçimlendir + "Gösteri zamanı" açık; Zaman formatı; Para birimi biçimi; Her kayıtta tarihi göster ☐; Dilim; Karanlık Mod ☐; Parmak İzi Şifresi ☐; Şifre Ayarları ☐; Önceki denge ☑; Her işlemden sonra bakiyeyi göster ☑; Açıklama / kategori ekle ☐; Not önerileri ☑. Formdaki üst yarı listesi ve 10 Eylül'de kategori anahtarının kapalı olduğu formla uyumlu. Alt yarı bu karede yok. |
| E0151 | 16-kategori-alani-acik-form.png | Ana | 18:20 Alındı formu: tarih Eyl-10-2026 06:20 ÖS, boş Alındı ve Notlar, ek "Açıklama / Kategori" serbest metin kutusu (seçici/ikon yok), Fatura ekle, Öğe eklemek. Alanın liste değil düz metin olması formla uyumlu. Kare E0150'den bir dakika sonra aynı 10 Eylül oturumunda anahtarın açıldığını gösteriyor; formdaki "ek koşum 2'de açıldı" anlatımı en az bu kare için eksik (düzeltme adayı). |
| E0152 | 17-onceki-denge-gunluk-gorunum.png | Ana | 18:04 Ana Hesap / Günlük "Bugün": Drive şeridi, italik "Önceki denge 20.000" satırı; alt özet 0 / 0 / Denge 0 + "Önceki denge 20.000" + "Denge 20.000". Kare gelir kaydından (18:05) önce çekildi; o anda defterde yalnız açılış vardı. Günlük görünümde açılışın Toplam Alındı'ya girmediği formla uyumlu; Herşey tarafı E0137'de. |
| E0153 | 18-kismi-odeme-is-karti.png | Ana | 19:04 Is Karti / Herşey: Eyl 10 "Kimden: Ana Hesap" 400 (Denge -600) ve "Test taksit ekipman" 1.000 Ödendi (Denge -1.000); Ağu 18 1.200 (Denge 0); Ağu 08 1.200 (Denge -1.200); toplam 1.600 / 2.200 / -600. A testi (-1.000 → 400 kısmi ödeme → -600) formla uyumlu. Test kayıtları Ağustos değil 10 Eylül tarihli (dönem kuralı istisnası); kısmi ödeme jenerik Aktar bacağıdır, ekstreye tahsis kanıtı değildir (HD-Q12). |
| E0154 | 19-transfer-bacagi-desync.png | Ana | 19:06 Ana Hesap / Herşey: Eyl 10 "Kime Is Karti" 400 (Denge 40.400), Ağu 18 1.200, Ağu 12 3.000, Ağu 03 25.000, Ağu 01 Açılış 20.000; toplam 45.000 / 4.600 / 40.400. Kare yalnız Ana Hesap defterini gösterir: 40.400 bu defterin 400'lük transfer sonrası normal bakiyesidir, iki bacak da dururken aynı çıkar. Karşı bacağın silindiği, birleşik net varlığın bozulduğu veya düzenlemenin diğer bacağı güncellemediği bu karede görünmüyor; formdaki öksüz bacak iddiası koşum anlatımıdır, bu kare kanıtı değildir (HD-Q02/Q03, B12). |

**Paket sonucu:** 21/21 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 18, Ek 3,
Arşiv 0; hiçbir görsel silinmedi veya yeniden adlandırılmadı. Form düzeltmeleri
ve soru ilerlemesi E0392 P1-hesap-defterim-G01 bölümündedir.

Güncel fiziksel toplam 360 PNG; incelenen 91 (eski korpus 88/357 + kullanıcıdan 3/3).
Goodbudget 31/31, KolayBi 39/39, Hesap Defterim 21/45; sonraki kimlik E0155
(P1-hesap-defterim-G02).


## P1-hesap-defterim-G02 — 14 Eylül 2026

Görseller doğrudan açılarak incelendi. Kaynak: 11 Eylül ek koşum 2 ve 12
Eylül Faz 7.5 ekleri (Android emülatör, manuel gözlem). Canlı yeni test
yapılmadı; E kimlikleri ve dosya adları değişmez. `23-kamera-onay-ekrani.png`
envantere hiç alınmamıştır ve bu pakette sayılmaz.

| Kanıt | Dosya | Rol | İçerik, metin karşılaştırması ve sınır |
|---|---|---|---|
| E0155 | 20-oge-eklemek-dialog.png | Ana | 05:30 Öğe eklemek diyaloğu, Ödendi formu üstünde: Öğe, Miktar, Birim, Fiyat, Ekle; "Öğe (2) — Toplam 150"; Kagit 5 adet @ 20 = 100, Kalem …adet @ 5 = 50 (miktar klavye araç çubuğu altında kesik); satır başı çöp kutusu; İptal etmek / TAMAM MI. Döküm ve toplam formla uyumlu; Birim alanı boş bırakılmış. |
| E0156 | 21-oge-eklemek-tutar-notlar-otomatik.png | Ana | 05:31 Ödendi formu: Ağu-20-2026 05:28 ÖÖ, tutar 150, Notlar "Kagit 5 adet @ 20 = 100 / Kalem 10 adet @ 5 = 50", boş Açıklama / Kategori alanı (anahtar açık). Diyalog sonrası tutarın ve notun dolu olduğu formla uyumlu; otomatik doldurma ile kullanıcının yazması arasındaki ayrım yalnız ardışık karelerden (05:30→05:31) çıkarılır. |
| E0157 | 22-fatura-ekle-secenekler.png | Ana | 05:33 aynı form, Açıklama / Kategori "Ofis malzemesi"; Fatura ekle menüsü: Kamera / Fotoğraf Galerisi / PDF. Salt ek seçenekleri formla uyumlu; OCR yokluğu bu menüyle sınırlıdır, bütün üründe yokluk kanıtı değildir (HD-Q07). |
| E0158 | 24-kayit-eklendi-atac-ikonu.png | Ana | 05:36 Ana Hesap listesi (çipler görünmüyor): Ağu 20 satırı iki satırlık döküm + "Ofis malzemesi" + 05:28 ÖÖ + ataç ikonu, Ödendi 150, Denge 40.650; altında Ağu 18/12/03/01 satırları; toplam 45.000 / 4.350 / 40.650; "İşlem Eklendi" toast'u. Ataç ve başarı toast'u formla uyumlu. Ana Hesap'ta Eyl 10 -400 bacağı yok (Ödendi 3.000 + 1.200 + 150): 10 Eylül'deki öksüz bacak en geç 11 Eylül 05:36'da kaldırılmış; nasıl kaldırıldığı görünmüyor (HD-Q02/Q03). |
| E0159 | 25-fatura-tam-ekran-goruntuleme.png | Ana | 05:38 İşlemi Düzenle (üst barda kopyala ikonu) üstünde tam ekran ek görüntüleme diyaloğu: emülatör sanal kamera sahnesi, İptal etmek / Silme; altta küçük önizleme, alt barda Silme / Kayıt etmek. Ek görüntüleme formla uyumlu. Formun K08'de bu kareye bağladığı Hesaplar alanı ve başka deftere taşıma diyalog altında görünmüyor (HD-Q08). Görsel sentetik kamera sahnesidir, fiş değildir. |
| E0160 | 26-islem-adlari-ozel-form.png | Ana | 05:40 İşlem adları: Özel seçili; "Aldığınız para için ad" Tahsilat, "Verdiğiniz para için ad" FaturaOdemesi (odakta kırmızı çerçeve); İptal etmek / Kayıt etmek; arkada Ana Hesap 45.000 / 4.350 / 40.650. Formla uyumlu. |
| E0161 | 27-yeniden-adlandirilmis-basliklar.png | Ana | 05:41 Ana Hesap / Herşey: kolon başlıkları Tahsilat / FaturaOdemesi ("FaturaOdemes-i" iki satıra kırılıyor), butonlar ve alt toplam etiketleri Tahsilat / FaturaOdemesi ("Toplam" öneki kalkıyor), değerler 45.000 / 4.350 / 40.650. Ana ekran, buton ve toplam etiketlerinin değişmesi formla uyumlu; form başlığı E0162'de; PDF başlıklarının değiştiği bu karelerde görünmüyor. Uzun özel adın başlıkta kırılması yeni gözlem. |
| E0162 | 28b-bos-tutar-sessiz-red.png | Ana | 05:47 FaturaOdemesi formu: Tahsilat/FaturaOdemesi seçici, Eyl-11-2026 05:45 ÖÖ, tutar boş ve odakta, Notlar ve Açıklama / Kategori boş, Kaydet ve çık / Kaydet ve devam Et. Kare yalnız boş formu gösterir: kaydetmeye basıldığı, formun kapanmadığı ve uyarı çıkmadığı statik kareden kanıtlanmaz. Etiketler yeniden adlandırılmış ve tarih 11 Eylül: kare 12 Eylül yeniden ölçümünün değil ek koşum 2'nin karesidir (HD-Q08). |
| E0163 | 28-sifir-tutar-kabul-edildi.png | Ana | 05:47 Ana Hesap / Herşey (yeniden adlandırılmış): Eyl 11 05:45 başlıksız "0" FaturaOdemesi satırı, Denge 43.150; Ağu 22 Notlar'ı boş, alt satırında "Danismanlik geliri" olan 2.500 Tahsilat; Ağu 20 150; toplam 47.500 / 4.350 / 43.150; "İşlem Eklendi" toast'u. Sıfırın uyarısız kabul edilmesi, başlıksız satır ve Denge'ye etkisizlik formla uyumlu. 05:45 saati E0162 formuyla aynı: bu kare o form oturumunun kaydedilmiş sonucudur. |
| E0164 | 29-arama-canli-filtre.png | Ana | 05:51 arama çubuğunda "Ada": tek sonuç Ağu 03 Web tasarim geliri 25.000; toplam 25.000 / 0 / 25.000. Alt toplamların filtreye göre yeniden hesaplanması formla uyumlu. Yeni gözlem: satırın yürüyen Denge'si de filtre içinde 25.000 gösteriyor (defterdeki gerçek 45.000 değil). Eşleşme Notlar metninde; Açıklama/Kategori alanında aradığı bu karede görünmüyor. Canlılık tek kareden ölçülmez. |
| E0165 | 30-not-defteri-checklist.png | Ek | 05:53 Not Defteri / Aylık, Eyl-01–30-2026: tek not "Odeme takibi", Cum Eyl 11 2026 05:52 ÖÖ, işaretli onay kutusu; Tamamlandı 1 / Beklemede 0 / Toplam 1; arama, takvim, + FAB; dönem çipleri. Formla uyumlu. Notun silinmesi ve kalıcı silme davranışı bu karede yok. |
| E0166 | 31-nakit-hesap-makinesi.png | Ek | 05:59 Nakit Hesap Makinesi: üstte 1.000; 200 × 5 = 1.000; 100, 50, 20, 10 ve (klavye araç çubuğu altında kısmen) küçük kupür satırları, 0,10 ve 0,05; alt Toplam satırı kesik (adet 5, 1.000). Kupür × adet ve canlı toplam formla uyumlu. Para birimi simgesi görünmüyor; "TL kupürleri" değerlerden çıkarımdır. Muhasebe kaydı üretmediği bu kareden görülmez. |
| E0167 | 32-silinmis-islemler-context-menu.png | Ana | 06:01 Silinmiş işlemler: tek satır Ana Hesap, 0, Cum Eyl 11 2026 05:45 ÖÖ; bağlam menüsü Geri Yükle / Silme; Test Ad banner. E0163'teki sıfır kaydın silinip çöp kutusuna düştüğünü ve iki seçeneği gösterir; formla uyumlu. Yeni gözlem: listede kırmızı görünen 0 çöp kutusunda yeşil. |
| E0168 | 33-kalici-silme-onay.png | Ana | 06:02 aynı ekran karartılmış, "İşlemi Sil" diyaloğu: açıklama metni yok, İptal etmek / Silme. İkinci onayın varlığı formla uyumlu; onay sonrası kaydın kalıcı silindiği bu karede görünmüyor, sonraki karelerde sıfır satırın yokluğuyla tutarlı. |
| E0169 | 34-bildiri-pdf-excel-secim.png | Ana | 06:03 Ana Hesap (Tahsilat/FaturaOdemesi) karartılmış; dışa aktarma diyaloğu: Herşey / Tarih Aralığı Seçin, PDF (seçili) / EXCEL, TAMAM MI. Arkadaki defterde sıfır satır artık yok, 47.500 / 4.350 / 43.150. Diyalog formdaki "Bildiri-Bütün Hesaplar" tanımıyla eşleşiyor ama giriş noktası karede görünmüyor. Bu diyalogdan sonraki paylaşım anı bu karede yok ("2 dosya paylaşılıyor" defter başına akışta E0176'da görünür); üretilen PDF'in içeriği hiçbir G02 karesinde yok, PDF içerik doğrulaması görsel kanıta dayanmaz (HD-Q05). |
| E0170 | 35-kontrol-degeri-geri-yuklendi.png | Ek | 06:12 İşlemler-Bütün Hesaplar / Herşey: E0140 ile aynı satırlar, Toplam Alındı 51.200 / Ödendi 6.250 / Denge 44.950; Ağu 20 ve Ağu 22 test kayıtları listede yok, etiketler varsayılan (Toplam Alındı/Ödendi). "Test kayıtları geçici silinmişken alınan ara kare" anlatımı ve İşlem adları denemesinin geri alınması formla uyumlu; nihai durum değildir. |
| E0171 | 36-drawer-menu-ust.png | Ana | 11:38 (12 Eylül) menü: Reklamları kaldırmak, Özet, Hesaplar Özet, İşlemler-Bütün Hesaplar, Hesaplar, Aktar, Bildiri-Bütün Hesaplar, İşlem adları, Not Defteri, Takvim, Nakit Hesap Makinesi, Yedekleme ve geri yükleme, Ayarlar, Silinmiş işlemler, Yardım, Bizi değerlendirin — 16 kalem görünüyor; 17. kalem "Önermek" ve Diğer uygulamalar E0177'de. Menüde tekrar/plan kalemi yok. Formdaki 17 kalemlik liste ile uyumlu. |
| E0172 | 37-kontrol-degeri-guncel-47300.png | Ana | 11:38 (12 Eylül) İşlemler-Bütün Hesaplar / Herşey: Ağu 22 Danismanlik geliri 2.500 (07:38 ÖÖ, başlıksız + alt satır), Ağu 20 150 ataçlı (07:29 ÖÖ), Ağu 18 ve Ağu 12 transfer çiftleri, Ağu 08 kesik; Toplam Alındı 53.700 / Ödendi 6.400 / Denge 47.300, varsayılan etiketler. 51.200 + 2.500 ve 6.250 + 150 tutuyor. İki test kaydının saatleri 11 Eylül'deki 05:28/05:42'den farklı (07:29/07:38): metodoloji notundaki silinip yeniden oluşturma kareyle destekleniyor. |
| E0173 | 38-ortak-cuzdan-defteri.png | Ana | 11:39 (12 Eylül) Ortak Cuzdan / Herşey: Ağu 12 Kimden: Ana Hesap 3.000 (Denge 4.150), Ağu 05 Market alisverisi 850 (Denge 1.150), Ağu 01 Açılış bilançosu 2.000; toplam 5.000 / 850 / 4.150. Kontrol değeri 4.150 ve K04'teki "o andaki Denge 1.150" formla uyumlu; 1.150 tarih sırasına göre yürüyen bakiyedir. |
| E0174 | 39-gecis-reklami-interstitial.png | Ek | Tam ekran "Test Ad" geçiş reklamı: sessiz video, İspanyolca klima reklamı, Daha Fazla, "Kapat" düğmesi onay işaretiyle etkin görünüyor. Durum çubuğu yok, tarih/saat okunmuyor. Tam ekran reklamın varlığı formla uyumlu; "Kapat birkaç saniye pasif" iddiası bu karede desteklenmiyor (düğme etkin), "12 Eylül'de yeniden görüldü" tarihi kareden doğrulanamaz. Reklam içeriği AdMob test modudur. |
| E0175 | 40-bildiri-defter-basina-pdf-excel.png | Ana | 11:41 (12 Eylül) Ana Hesap karartılmış, "Bildiri" diyaloğu: yalnız PDF ve Excel satırları, dönem seçimi yok; arkada 47.500 / 4.350 / 43.150, varsayılan etiketler. Defter başına dönemsiz dışa aktarma formla uyumlu. |
| E0176 | 41-bildiri-kasadefteri-uyarisi.png | Ana | 11:42 "Dışa Aktarılan Veriler — Veriler, SD kartta veya Dahili Depolamada kasadefteri adlı bir klasöre kaydedilir." (TAMAM MI) ve altta Android paylaşım sayfası "2 dosya paylaşılıyor" (Quick Share, Drive, Haritalar, Mesajlar, Fotoğraflar). Uyarı metni ve iki dosyanın paylaşıldığı formla uyumlu. Klasörün gerçekte olmadığı ve dosyaların Documents/Hesap Defterim/ altına yazıldığı dosya yöneticisi karesiyle belgelenmemiş; koşum notudur. İki dosyanın türü karede yazmıyor. |
| E0177 | 42-drawer-diger-uygulamalar-veresiye-gelirgider.png | Ana | 11:43 menünün aşağı kaydırılmış hâli: … Bizi değerlendirin, Önermek; "Diğer uygulamalar": Veresiye Defteri "Borcu yönet.İndir", Gelir Gider "Kategori bilge harcamaları yönetin". 17 kalemlik menü ve çapraz tanıtım formla uyumlu. Uygulamalar kurulmadı; ayrılmış ürün hattı ve geliştirici niyeti bu tanıtımdan çıkarımdır (HD-Q07). |
| E0178 | 43-ayarlar-alt-bolum-donem-baslangici.png | Ana | 11:44 Ayarlar alt bölüm: Her işlemden sonra bakiyeyi göster ☑, Açıklama / kategori ekle ☑, Not önerileri ☑, Raporlarda zamanı göster ☐, Yılın ilk gününü ayarlayın (değer yok), Ayın ilk gününü ayarlayın 1, Haftanın ilk gününü ayarlayın (değer yok), Varsayılan süreyi ayarla Herşey, Verileri sil (kırmızı), İşlem dökümünü e-posta ile otomatik gönder ☑, Ekranını açık tut ☐, Gizlilik Politikası. E0150 ile birlikte 21 ayar sayısı tutuyor; kategori anahtarının açık bırakıldığı formla uyumlu. E-posta ayarının açık olması kurulum varsayılanını veya gönderimi kanıtlamaz (B11/HD-Q06); dönem ayarlarının sınır etkisi denenmedi (HD-Q10). |

**Paket sonucu:** 24/24 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 20, Ek 4,
Arşiv 0; hiçbir görsel silinmedi veya yeniden adlandırılmadı. Hesap Defterim
görselleri 45/45. Form düzeltmeleri ve soru ilerlemesi E0392
P1-hesap-defterim-G02 bölümündedir.

Güncel fiziksel toplam 360 PNG; incelenen 115 (eski korpus 112/357 + kullanıcıdan 3/3).
Goodbudget 31/31, KolayBi 39/39, Hesap Defterim 45/45.


## P1-B11 — B11 kapsamında düzeltilen belgeler — 14 Eylül 2026

Görsel değişikliği yok. Dayanak: E0178 (e-posta ayarı işaretli), E0149
(sunucuda saklamama beyanı). Metin düzeltmeleri: E0007 Hesap Defterim formu
(7 yer) ve DURUM.md uygulama tablosu (1 yer). Ayrıntı E0392 P1-B11 bölümünde.
Önceki hash değerleri kendi kayıt anlarına aittir.


## P1-B12 — B12 kapsamında düzeltilen belgeler — 14 Eylül 2026

Görsel değişikliği yok. Metin düzeltmeleri: E0014 Wallet formu (2 yer), E0007
Hesap Defterim formu (5), DURUM.md (2), TUR2-YOL-HARITASI.md (1). Fiziksel
şema iddiaları gözlenebilir liste/arama/toplam davranışıyla değiştirildi.
Ayrıntı E0392 P1-B12 bölümünde. Önceki hash değerleri kendi kayıt anlarına aittir.


## P1-money-manager-G01 — 14 Eylül 2026

Görseller doğrudan açılarak incelendi. Kaynak: 10 Eylül Faz 1 ve ek koşum
(E0227–E0249) ile 12 Eylül Faz 7.5 ekleri (E0250–E0256); Android emülatör,
manuel gözlem. Saatler durum çubuğundandır; gün, dosya zamanıyla eşleşir (dosya
zamanı durum çubuğunun tam 3 saat ilerisi). 30 dosyanın SHA-256 değeri başlangıç
kaydıyla aynı. Canlı yeni test yapılmadı; E kimlikleri ve dosya adları değişmez.
`05` numaralı dosya envantere hiç alınmamıştır, sayılmadı.

| Kanıt | Dosya | Rol | İçerik, metin karşılaştırması ve sınır |
|---|---|---|---|
| E0227 | 02-bos-ana-ekran.png | Ek | 10 Eyl 11:28 İşlemler / Gün, Eyl 2026: 0/0/0, astronot illüstrasyonu + "Veri yok.", toast "Çıkmak için 'Geri' tuşuna tekrar basın."; reklam alanı yok. Ağustos verisi girildikten sonra boş Eylül ayıdır, ilk açılış veya temiz kurulum karesi değil. Beş sekme, üç sütun ve FAB formla uyumlu. |
| E0228 | 03-dolu-ana-ekran.png | Ana | 11:29 Ağu 2026 Gün: 18 Havale "Ödeme Bilgisi" Ana Hesap → Is Karti 1.200; 12 Havale "Nakit aktarim" → Ortak Cuzdan 3.000; 08 Diğer "Mavi Yazilim" Is Karti 1.200; 05 Yiyecek "Market" Ortak Cuzdan 850; 03 Diğer "Ada Reklam" Ana Hesap 25.000; 25.000 / 2.050 / 22.950. Havale gün başlıkları 0/0, başlık = not, kategori solda: formla uyumlu. |
| E0229 | 04-islem-formu-ve-kategori.png | Ana | 11:29 Gider formu: Gelir/Gider/Havale, Tarih 10.09.2026 11:29, Tekrar/Taksit, Tutar/Kategori/Hesap/Not, Detay + kamera; kategori paneli 11 kutu (Yiyecek, Eğlence, Taşıma, Hobiler, Günlük Yaşam, Giyim, Kozmetik, Sağlık, Eğitim, Olay, Diğer) ve kalem ikonu. 11 kişisel gider kategorisi ve kapsam alanı yokluğu formla uyumlu; Kaydet/Devam et panel altında görünmüyor. Gelir kategori ızgarası bu karede yok. |
| E0230 | 06-islem-listesi-transfer-notr.png | Ana | 11:23 Ağu Gün: 12 Nakit aktarim 3.000 (başlık 0/0), 08 Mavi Yazilim 1.200, 05 Market 850, 03 Ada Reklam 25.000; 25.000 / 2.050 / 22.950. 18 Ağu kart ödemesi henüz yok: kare ödeme kaydından önceki ana aittir. Transferin nötr satır ve 0/0 başlığı formla uyumlu. |
| E0231 | 07-rapor-agustos.png | Ana | 11:26 İstatistik / Ay, Ağu 2026: sekmeler Gelir ₺25.000 / Gider ₺2.050 (seçili); pasta Diğer %58,5, Yiyecek %41,5; liste Diğer 1.200 (%59), Yiyecek 850 (%41). Formla uyumlu. Kategoriye dokununca açılan işlem listesi (drill-down) bu karede yok. |
| E0232 | 08-hata-toast-hesap-sec.png | Ana | 11:30 Gider formu: Kategori Diğer, Tutar ve Hesap boş (Hesap odakta), Kaydet / Devam et, Hesaplar paneli Ortak Cuzdan / Ana Hesap / Is Karti; toast "Lütfen hesabı seçiniz." Toast ve inline hata yokluğu formla uyumlu. Formun K08'de bu kareye bağladığı satır içi düzenleme, Sil/Kopya/Hızlı erişim ve silme onayı karede yok; sıfır tutar kabulü ve taslak uyarısı yokluğu da görünmüyor (MM-Q09). |
| E0233 | 09-kart-hesap-ekstre-modeli.png | Ana | 11:11 Hesap Bilgisi: Tür Kredi Kartı, Ad "Kredi Kartı" (henüz Is Karti adı verilmemiş), Tutar ₺0, Kaynak Ana Hesap, Detay, Hesap Kesim Tarihi 1, Son Ödeme Tarihi 1; kutu "Bu Ay 01/08 ~ 31/08 (Ödeme: 01/09)", "Gelecek Ay 01/09 ~ 30/09 (Ödeme: 01/10)"; Toplama Dahil Et açık, Göster/Gizle, çöp kutusu. Alanlar formla uyumlu; hazır hesabın düzenlendiği adla destekleniyor. 10 Eylül'de "Bu Ay" son kesilmiş Ağustos dönemidir. |
| E0234 | 10-takvim-gorunumu.png | Ana | 11:29 Takvim Ağu: 3: 25.000 mavi, 5: 850 kırmızı, 8: 1.200 kırmızı, 12 ve 18: 0,00 siyah; Paz kırmızı, Cmt mavi; 25.000 / 2.050 / 22.950. Havale/ödeme günlerinin 0,00 olması formla uyumlu. Hiçbir günde iki yön olmadığından hücrenin net mi ayrı mı gösterdiği doğrulanamaz. |
| E0235 | 11-hesaplar-final-kontrol-degerleri.png | Ana | 11:25 Hesaplar: Varlıklar 44.950 / Borçlar 0 / Toplam 44.950; Nakit–Ortak Cuzdan 4.150; Banka Hesapları–Ana Hesap 40.800; Kredi Kartı–Is Karti Bu Ay 0 / Gelecek Ay 0; boş "Ad" yer tutucusu. Çekirdek kontrol değerleri formla birebir. |
| E0236 | 12-hesaplar-kart-borcu-bu-ay.png | Ana | 11:24 Hesaplar: Varlıklar 46.150 / Borçlar 1.200 / Toplam 44.950; Ana Hesap 42.000; Is Karti Bu Ay 1.200 (kırmızı) / Gelecek Ay 0. Kart ödemesinden önceki an: harcamanın Bu Ay'a düştüğünü gösterir, sıfırlanmış hâli E0235'tedir. Borç işaretsiz kırmızı; formdaki "₺ -1.200,00" gösterimi bu ekranda yok. Ödeme öncesi ve sonrası net 44.950 aynı. |
| E0237 | 13-odeme-butonu-onfoldurulmus-havale.png | Ana | 11:24 Havale formu: Tarih 10.09.2026 11:24, Tutar 1.200, Kaynak Ana Hesap, Giriş Is Karti, Not "Ödeme Bilgisi", Detay, tek Kaydet (koyu). Tutar E0236'daki o anki borca eşit: ön dolum formla uyumlu. Ödeme düğmesinin kendisi bu karede yok (E0255/E0256'da). Tarih bugün; kayıt listede 18 Ağu, tarih sonradan değiştirilmiş. Kategori alanı yok. |
| E0238 | 14-tekrarlama-secenekleri.png | Ek | 11:28 "Tekrarlama" listesi: Hiçbiri, Günlük, Haftanın günleri, Haftasonu, Haftalık, İki Haftada Bir, Dört Haftada Bir, Aylık, Ayın Son Günü, İki Ayda Bir, Üç Aylık, Her dört ayda bir, Her 6 ayda bir, Yıllık (14). Liste formla birebir. Alt gezinmede Daha seçili ve saat tekrar testinden (12:04) önce: formdaki Tekrar/Taksit → Tekrarlama/Taksit açılır menüsü bu karede yok. |
| E0239 | 15-acilis-bakiye-farki-dialog.png | Ana | 11:08 Hesap Bilgisi (Nakit, Ortak Cuzdan, Tutar 2.000, Toplama Dahil Et açık) üstünde diyalog "Fark hesap detaylarına kaydedildi, 'İşlemler' bölümünde gösterilmesini ister misiniz?" HAYIR / EVET. Metin formla birebir; hangi seçeneğe basıldığı karede yok, E0228/E0243 feed'inde fark satırının olmaması HAYIR ile tutarlı. Nakit hesabında Kaynak/kesim alanları yok. |
| E0240 | 16-acilis-bakiye-farki-defter.png | Ana | 11:08 Ortak Cuzdan defteri, Eyl 2026, Gün/Ay/Yıllık, "Faturalama donemi 1.09.2026 ~ 30.09"; Para Yatırma 2.000 / Çekme 0 / Toplam 2.000 / Bakiye 2.000; 10 Eyl "Bakiyeyi Düz…" · "Bakiye Farkı" 2.000 (Bakiye 2.000,00). Bugün tarihli fark hareketi formla uyumlu. Yeni gözlem: nakit hesabı defterinde de "Faturalama donemi" etiketi var; Ödeme düğmesi yok. |
| E0241 | 17-tekrarlayan-aylik-form.png | Ana | 12:04 Gider formu: 10.08.2026 (Pzt), "Aylık" rozeti, 600, Diğer, Ana Hesap, Not "Bulut yazilim aboneligi"; yalnız Kaydet (Devam et yok). Rozet formla uyumlu. Kaydet sonrası onay diyaloğu bu karede yok. |
| E0242 | 18-tekrarlayan-agustos-liste.png | Ana | 12:05 Ağu Gün: 10 Ağu "Bulut yazilim aboneligi" Ana Hesap (Aylık) 600 eklenmiş; diğer satırlar E0228 ile aynı; 25.000 / 2.650 / 22.350. Geçmiş tarihli ilk occurrence'ın normal gider olarak sayılması formla uyumlu. |
| E0243 | 19-tekrarlayan-eylul-otomatik.png | Ana | 12:06 Eyl Gün: 10 Eyl "Bulut yazilim aboneligi" Ana Hesap (Aylık) 600; 0 / 600 / −600. Bugünkü occurrence'ın gerçek işlem olarak listelenmesi formla uyumlu. "Onaysız" olduğu tek kareden görülmez, karede onay izi de yok. Bakiye Farkı satırı feed'de yok. |
| E0244 | 20-tekrarlayan-ekim-onizleme.png | Ana | 12:06 Eki Gün: 0/0/0; üstte ayrı "Tekrarlama" satırı "Bulut yazilim aboneligi 10/10 ₺ -600,00"; altında kedi illüstrasyonu + "Veri yok.". Gelecek occurrence'ın sayılmayan önizleme olması formla uyumlu. |
| E0245 | 21-taksit-6ay-form.png | Ana | 12:09 Gider formu: 15.08.2026 (Cmt) 12:07, "6 Ay" rozeti, 6.000, Diğer, Is Karti, Not "Tasarim ekipmani", Kaydet / Devam et. Rozet formla uyumlu; "Aylık taksit (Ay sayısı)" alanı bu karede yok. |
| E0246 | 22-taksit-1-6-agustos.png | Ana | 12:10 Ağu Gün: 15 Ağu "Tasarim ekipmani (1/6)" Is Karti 1.000; 10 Ağu abonelik 600; 03 satırı altta kesik; 25.000 / 3.650 / 21.350. Tek taksitin ve (1/6) başlığının listeye düşmesi, 2.050 + 600 + 1.000 = 3.650 formla uyumlu. |
| E0247 | 23-taksit-kart-borcu-bu-gelecek-ay.png | Ana | 12:10 Hesaplar: Varlıklar 43.750 / Borçlar 2.000 / Toplam 41.750; Ortak Cuzdan 4.150; Ana Hesap 39.600 (40.800 − 600 Ağu − 600 Eyl); Is Karti Bu Ay 1.000 / Gelecek Ay 1.000. İki taksitin iki döneme dağılması ve kalan dördünün borçta olmaması formla uyumlu. Bu an Ana Hesap 39.600'dür; formdaki 39.200 E0249 anına aittir. |
| E0248 | 24-kismi-kart-odemesi-400.png | Ana | 12:12 Havale formu: 10.09.2026 12:11, Tutar 400, Kaynak Ana Hesap, Giriş Is Karti, Not "Ödeme Bilgisi", Kaydet. Karede yalnız düzenlenmiş 400 var; ön dolu 1.000 değeri görünmüyor, düzenlenebilirlik E0237 ile birlikte çıkarılır. |
| E0249 | 25-kismi-odeme-sonrasi-borc.png | Ana | 12:12 Hesaplar: Varlıklar 43.350 / Borçlar 1.600 / Toplam 41.750; Ana Hesap 39.200; Is Karti Bu Ay 600 / Gelecek Ay 1.000; boş "Ad" yer tutucusu. Kısmi ödemenin yalnız Bu Ay'ı düşürmesi formla uyumlu. Net 41.750 ödeme öncesiyle aynı. Formun güven bölümündeki 39.200 bu andır. |
| E0250 | 26-toplam-sekmesi-agustos.png | Ana | 12 Eyl 09:23 Toplam / Ağu 2026: 25.000 / 3.050 / 21.950; Bütçe + "Bütçe Ayarları >"; Hesaplar 1.08.2026 ~ 31.08: Giderleri Karşılaştır (Son ay) %100, Gider (Nakit, Banka Hesapları) 850, Gider (Kredi Kartı, Ödeme) 2.200(1.200), Havale (Nakit, Banka Hesapları →) 0,00; "Excel(.xlsx) e-posta olarak gönder". Satırlar formla uyumlu. 2.200 Ağustos kart harcamasıdır (1.200 + 1.000, E0255 Çekme), borç değil; parantez ödemedir. Gider 3.050 = 3.650 − 600: Ağu 10 aboneliği bu anda yok. Formun `₺1.000,00(₺400,00)` örneği hiçbir karede yok. |
| E0251 | 27-filtre-paneli-agustos-hesap.png | Ana | 09:24 filtre paneli, Ağu 2026: "Eğer filtre uygulamak istediğiniz öğeyi seçin"; Gelir donut %0 25.000, Gider donut %0 3.050, Toplam 21.950; GELİR/GİDER/HESAP; başlık "Her şey", sütun başlıkları sol "Gelir / Giden Havale", sağ "Gider / Gelen Havale"; Ortak Cuzdan 0/3.000 · 850/0; Ana Hesap 25.000/0 · 0/4.200; Is Karti 0/1.200 · 2.200/0. Havale başlıklarının ters olduğu formla uyumlu. Düzen iki sütunda ikişer satırdır, "dört sütun" değil. Ana Hesap Ağustos gideri 0: abonelik silinmiş. |
| E0252 | 28-toplama-dahil-et-anahtari.png | Ana | 08:59 Hesap Bilgisi: Nakit, Ortak Cuzdan, Tutar 4.150, Detay, Toplama Dahil Et açık, Göster/Gizle (göz), Kaydet. İki anahtar formla uyumlu. Tutar alanı açılış değil güncel bakiyeyi gösteriyor. Anahtarın açık olması bu anın durumudur; kurulum varsayılanını kanıtlamaz (E0233/E0239 da açık). |
| E0253 | 29-toplama-dahil-kapali-net-varlik.png | Ana | 09:00 Hesaplar: Varlıklar 39.800 / Borçlar 1.600 / Toplam 38.200; Nakit grubu 0,00 siyah, Ortak Cuzdan 4.150 gri; Ana Hesap 39.800; Is Karti 600 / 1.000. Kapalı satır formla birebir. Açık satırın (43.950 / 42.350) 12 Eylül karesi yok, aritmetiktir. Ana Hesap 39.800 = E0249'daki 39.200 + silinen Ağu aboneliği 600. |
| E0254 | 30-ayarlar-izgarasi.png | Ek | 09:07 Daha → Ayarlar, sağ üst "4.12.8 AD"; boş "Ad" yer tutucusu; Ayarlar, Hesaplar, Giriş Kodu, CalcBox, PC'den Yönet, Yedekle, İletişim, Yardım, Tavsiye et; altta "Reklamlar Kaldır". Formla birebir. Alt ayarlar ve PC'den Yönet/Yedekle içerikleri açılmadı. |
| E0255 | 31-kart-defteri-agustos-hareketler.png | Ana | 09:14 Is Karti defteri, Ağu 2026, Gün/Ay/Yıllık, "Faturalama donemi 1.08.2026 ~ 31.08"; Para Yatırma 1.200 / Çekme 2.200 / Toplam −1.000 / Bakiye (?) 1.000; 18 Havale Ödeme Bilgisi 1.200 (Bakiye −1.000,00); 15 Diğer Tasarim ekipmani (1/6) 1.000 (−2.200,00); 08 Diğer Mavi Yazilim 1.200 (−1.200,00); "Ödeme" düğmesi + FAB. Tablo ve üç okuma formla birebir. Bakiye başlığında yardım (?) ikonu var, nakit defterinde yok. |
| E0256 | 32-kart-defteri-eylul-taksit-2-6.png | Ana | 09:14 Is Karti defteri, Eyl 2026, "Faturalama donemi 1.09.2026 ~ 30.09"; Para Yatırma 400 / Çekme 1.000 / Toplam −600 / Bakiye 1.600; 15 Eyl Tasarim ekipmani (2/6) 1.000 (Bakiye −1.600,00); 10 Eyl Havale Ödeme Bilgisi 400 (−600,00); Ödeme + FAB. Formla birebir. 15 Eyl tarihli taksit 12 Eyl'de gerçek satır olarak duruyor; 10 Eyl'de de Gelecek Ay borcundaydı (E0247). Kalan dört taksitin durumu karede yok (MM-Q03). |

**Paket sonucu:** 30/30 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 27, Ek 3,
Arşiv 0; hiçbir görsel silinmedi veya yeniden adlandırılmadı. Money Manager
görselleri 30/30. Form düzeltmeleri ve soru ilerlemesi E0392
P1-money-manager-G01 bölümündedir.

Güncel fiziksel toplam 360 PNG; incelenen 145 (eski korpus 142/357 + kullanıcıdan 3/3).
Goodbudget 31/31, KolayBi 39/39, Hesap Defterim 45/45, Money Manager 30/30.


## P1-parasut-G01 — 14 Eylül 2026

Görseller doğrudan açılarak incelendi. Kaynak: E0258/E0259 1 Eylül mobil giriş
öncesi manuel gözlem (durum çubuğu 1:18/1:19; dosyalar 7 Eylül'de kopyalanmış);
E0260–E0266 "Paraşüt ile neler yapabilirsiniz?" tanıtım videosu kareleri
(kullanıcı aktarımı, 10 Eylül). 9 dosyanın SHA-256 değeri başlangıç kaydıyla
aynı. Canlı test yok; E kimlikleri ve dosya adları değişmez. Çalışma ağacında
silinmiş görünen `01c-carousel.png` envantere hiç alınmamıştır; geri getirilmedi,
karusel slayt 2/3 kanıtı sayılmadı. Karedeki kişisel e-posta adresleri kayda
aktarılmadı.

| Kanıt | Dosya | Rol | İçerik, metin karşılaştırması ve sınır |
|---|---|---|---|
| E0258 | 01b-carousel.png | Ana | 1:19 mobil giriş öncesi: PARAŞÜT mikrogrup logosu, pano illüstrasyonu, "Paraşüt ile işletmenizin güncel durumunu anlık takip edin!", dört noktadan 1. etkin, `Giriş yap`, `Parolanızı mı unuttunuz?`. Slayt 1 metni ve iki eylem formla birebir. Sürüm numarası karede yok; kayıt eylemi yokluğu yalnız bu yüzey içindir. |
| E0259 | 01-ilk-acilis-carousel4.png | Ana | 1:18 aynı yüzey, dört noktadan 4. etkin, roket/dizüstü illüstrasyonu, "Oluşturduğunuz faturaları, müşterinizle veya tedarikçinizle hemen paylaşın", aynı iki eylem. Formla birebir. Slayt 4 slayt 1'den bir dakika önce çekilmiş; web kayıt koşulları ve fiyat hiçbir karede yok (PS-Q02). |
| E0260 | 02-video-fatura-gonderme.png | Ana | Video karesi: solda telefonda `E-Fatura` belgesi ve "Fatura Gönderildi!"; sağda pencere çerçevesinde `Satış Faturaları > Fatura`, MRT Yapı Malzemeleri, 21 Eylül 2022, #FA020200000409; Çimento 499,00 TL %5 523,00 TL; ARA TOPLAM 499,00 / TOPLAM KDV 24 TL / GENEL TOPLAM 523,00 TL; panel FATURA GÖNDERİLDİ, e-Arşiv FATURA, PAYLAŞ, KALAN 523,00 TL, "7 gün sonra tahsil edilecek", Müşteri hatırlatma ekle, Tahsilat talep et, TAHSİLAT EKLE (yeşil), İrsaliyeli Fatura, Müşteri Ekranı Açık, Fatura Geçmişi. Alanlar formla uyumlu; formda son üç panel satırı yok. Telefonda E-Fatura, panelde e-Arşiv yazması kurgu tutarsızlığı. Cari borcun yazıldığı karede görünmüyor. |
| E0261 | 03-video-satis-faturasi-detay.png | Ana | Video başlığı "Cari Hesap Takibi"; `Satış Faturaları > Satış Faturası`, ÇİMENTO çipi, BARKOD / CARİSİZ / DEPO düğmeleri, MRT Yapı Malzemeleri, 08 Haziran 2022, #GÖNDERİLİYOR; sütunlar HİZMET/ÜRÜN, ÇIKIŞ DEPO, MİKTAR, BİRİM FİYAT, VERGİ, TOPLAM; Çimento IST 2,00 499,00 TL %5 523,00 TL, İNDİRİM %8; 499 / 24 / 523; panel Temel e-Fatura • GÖNDERİLİYOR, iki alıcı "Yolda", PAYLAŞ, ✓ TAHSİL EDİLDİ 523,00 TL, TAHSİLAT EKLE soluk, İrsaliyeli Fatura, Müşteri Ekranı Açık. Formun aritmetik notu doğru. E0260 ile tarih, numara, belge türü, sayfa başlığı ve miktar sütunu farklı: aynı faturanın iki durumu değil, iki ayrı örnek (PS-Q03). CARİSİZ bir düğmedir; anlamı çıkarım, karedeki faturada müşteri seçili. |
| E0262 | 04-video-cari-hesap-durumu.png | Ana | Video başlığı "Cari Hesap Takibi"; Güncel Durum: Tahsilatlar 138.89,20 ₺ TAHSİL EDİLECEK, 138.89,20 ₺ GECİKMİŞ, ✓ FATURA YOK; Ödemeler 94.45,10 ₺ ÖDENECEK, ✓ ÖDEME YOK, 39.12,00 ₺ PLANLANMIŞ; sağda BUGÜN - 22 EYLÜL ve 4 GÜN GECİKTİ, Tahsilat: 19.989,00 ₺. Etiketler ve bozuk ayraçlar formla uyumlu. Ek tutarsızlık: "yok" onayları tutarlı donut'larla yan yana, tahsil edilecek = gecikmiş. PLANLANMIŞ kovasının içeriği karede yazmıyor (PS-Q05). |
| E0263 | 05-video-banka-entegrasyonu.png | Ek | Üstte oynatıcı başlığı "Paraşüt ile neler yapabilirsiniz?", altta beğen/yorum/paylaş simgeleri; "Banka Entegrasyonu"; solda 12 banka logosu; Kasa ve Bankalar: BANKA HESABI BAĞLA / KASA EKLE / BANKA EKLE; HESAP İSMİ / IBAN / DÖVİZ CİNSİ / BAKİYE; Yapı Kredi Maaş 40.000, Ziraat Bankası Vadeli 10.000, Akbank - Kadıköy 1.000, Garanti Bankası Vadesiz 500,00, Kasa Hesabı 5.000; hepsi TRL ve aynı IBAN. Formla uyumlu; döviz kodu TRL ve kasa hesabında IBAN yeni gözlem. Oynatıcı öğeleri kaynak türünü gösterir, URL/yayın tarihi değildir (PS-Q01). Bağlama akışının koşulları karede yok (PS-Q08). |
| E0264 | 06-video-stok-depo.png | Ek | "Stok ve Depo Takibi"; Depolar > Depo, Ana Depo (Varsayılan Depo), DÜZENLE, Adres Pendik/IST, Ürünler / Stok Geçmişi; ÜRÜN ADI / STOK MİKTARI / ALIŞ (VERGİLER HARİÇ) / SATIŞ (VERGİLER HARİÇ); Çimento 20 Adet 5.000/10.000, Beton Blok 35 300/550, Kereste 20 700/1.100, Çelik Konstrüksiyon 10 650/1.000 (kırmızı zemin). Formla birebir. Kırmızı satır en düşük stoklu satır ama "kritik stok" etiketi yok, çıkarım korunur. VERGİLER HARİÇ stok sütunudur, E0265 açılırının seçeneğini kanıtlamaz (PS-Q07). |
| E0265 | 07-video-gelir-gider-raporu.png | Ana | "Finansal Raporlama"; Gelir ve Gider Raporu: Filtrele, 10 Şubat 2022 - 31 Temmuz 2022, Vergiler dahil ▾; Gelirler 38.987,98 (Kategorisiz 10.987,98, Diğer 13.107,00, Altın 10.107,00, Gümüş 5.147,00); Giderler 10.387,91 (Kargo 1.100,91, Diğer 7.107,00, Kategorisiz 2.100,98); iki pasta; NET 28.600,07; DIŞARI AKTAR. Aritmetik notlar (39.348,98; 10.308,89; NET tutarlı) yeniden hesaplandı, doğru. Kategorisiz boş daireli bir liste satırıdır; ayrı kategori nesnesi olduğu görünmez (PS-Q10). Hariç seçeneği karede yok. |
| E0266 | 08-video-nakit-akisi-raporu.png | Ana | "Finansal Raporlama"; Kasa / Banka Raporu: 80.987,98 Toplam Nakit Girişi, 22.987,98 Toplam Nakit Çıkışı, 58.765,00 Net Nakit Akışı; Ocak–Ağustos yalnız mavi çubuklar, eksen 0 m–20 m; Nakit Girişi / Nakit Çıkışı göstergesi; GÜN HAFTA AY YIL; "01 Ocak 2022 - 10 Ağustos 2022 Arası Yapılan Tahsilat ve Ödemeler": Tahsilat 15 Ağustos 2022 Ali Demirci 120,00 giriş; Ödeme 14 Ağustos 2022 Yıldız Butik İade 248,00 çıkış; Tahsilat 12 Ağustos 2022 Trend Aksesuar 570,99 giriş. Formun 58.000 ≠ 58.765 notu doğru. Ek tutarsızlık: eksen ölçeği toplamla, satır tarihleri başlık aralığıyla uyuşmuyor. Liste tahsilat/ödeme hareketidir; E0265'in tahakkuk saydığı karelerden çıkmaz (PS-Q05). |

**Paket sonucu:** 9/9 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 7, Ek 2,
Arşiv 0; hiçbir görsel silinmedi veya yeniden adlandırılmadı. Paraşüt görselleri
9/9. Form düzeltmeleri ve soru ilerlemesi E0392 P1-parasut-G01 bölümündedir.

Güncel fiziksel toplam 360 PNG; incelenen 154 (eski korpus 151/357 + kullanıcıdan 3/3).
Goodbudget 31/31, KolayBi 39/39, Hesap Defterim 45/45, Money Manager 30/30, Paraşüt 9/9.


## P1-logo-isbasi-G01 — 14 Eylül 2026

Görseller doğrudan açılarak incelendi. Kaynak: E0220–E0223 1 Eylül emülatörde
giriş/kayıt yüzeyi, manuel gözlem (durum çubuğu 1:27–1:30; dosyalar 7 Eylül'de
kopyalanmış); E0224/E0225 kullanıcıya 2 Eylül'de e-postayla gelen tanıtım
videosunun kareleri (3155x2022 ve 3240x1892, telefon ekranı değil). 6 dosyanın
SHA-256 değeri başlangıç kaydıyla aynı. Canlı test, kayıt veya SMS işlemi yok;
E kimlikleri ve dosya adları değişmez.

| Kanıt | Dosya | Rol | İçerik, metin karşılaştırması ve sınır |
|---|---|---|---|
| E0220 | 01-giris-ekrani.png | Ana | 1:27 giriş: bulut arka planı, "logo İŞBAŞI" logosu + çay bardağı; e-mail (kişi ikonu), password (kilit ikonu), beyaz `Login`, "or" ayracı, Google ve Apple daire düğmeleri, dolu kırmızı `Register`, `FORGOT PASSWORD?`. Formla birebir. Sürüm numarası karede yok; SSO düğmeleri oturum başarısı kanıtı değildir (LI-Q09). |
| E0221 | 02-kayit-formu.png | Ana | 1:29 kayıt: logo; `Company name*` (çanta ikonu), `Sector*` `Select` (açılır ikon), `Phone Number*` (telefon ikonu); işaretli onay kutusu ve altı çizili "I agree to terms of service and privacy policy."; `Register`; solda yüzen klavye araç çubuğu (TR). Üç alan ve VKN/TCKN yokluğu formla birebir; kutunun işaretli olması yeni gözlem, varsayılan mı kullanıcı mı işaretledi bilinmiyor. Company name zorunluluğu "firma bilgisi tanımlamadan" tezini sınırlar (LI-Q02). Telefon ön eki karede yok. |
| E0222 | 03-sektor-listesi.png | Ana | 1:29 açık liste: Select, Ticaret ve Perakende, İmalat ve Üretim, Kurye, Öğrenci, İnşaat ve Taahhüt, Hizmet Sektörü, Sağlık ve Medikal, Bilişim ve Teknoloji, Eğitim ve Danışmanlık, Otelcilik, Restoran ve Kafe; sağda kaydırma çubuğu listenin devam ettiğini gösteriyor. Altta klavye açık, öneriler "Tasarim / Tasarım / Tasarımı", TR • EN. Liste formla birebir. Klavye önerisi formdaki "Deniz Tasarim" deneme adıyla tutarlı. Listenin tamamı çekilmedi (LI-Q08). |
| E0223 | 04-sozlesme.png | Ana | 1:30 "LOGO BULUT HİZMETLERİ ÇERÇEVE SÖZLEŞMESİ": 1. GİRİŞ, 2. ONAY VE BAĞLAYICILIK, 3. KONU, 4. TANIMLAR ("Alt Hizmet Sağlayıcı", "Bölge", "Dokümantasyon"…); altta gri `CANCEL`, mavi `I AGREE`. Formla birebir. Formun sahte numara denemesinde bu kareye bağladığı ~10 sn spinner, sessiz geri dönüş ve hesap oluşmaması karede yok; koşum notudur (LI-Q02). |
| E0224 | 05-video-entegrasyonlar.png | Ek | Video karesi: üst kenarda kesik oynatıcı başlığı ("…e-Fatura ve ön muhasebe programı… Logo İşbaşı!"), altta kırmızı ilerleme çubuğu; "Entegrasyonlarımız İle / Verimliliğinizi Artırın"; 8 kutu: Pazaryeri ve e-Ticaret, GİB e-Arşiv Portal, Banka Hesap Hareketleri (üzerinde imleç), Akıllı Fiş Okuma, Online Tahsilat, Müşavir Portal, İşbaşı POS, Kargo Yönetimi. Sekiz ad formla birebir. Adlar işlev, paket veya çalışma biçimi kanıtı değildir (LI-Q05/Q06); oynatıcı öğeleri URL veya yayın tarihi vermez (LI-Q01). |
| E0225 | 06-video-musavir-portal.png | Ana | Video karesi: solda "Alış-Satış Faturalarınızı / Gider Fişlerinizi / Muhasebecinize Kolayca Aktarın"; sağda stilize pencere, "MÜŞAVİR PORTAL", fotoğraflı profil simgesi, boş arama çubuğu, dört satır: farklı logo simgesi (çiçek, şef şapkası, daire, kamyon) + üç boş şablon çubuğu + kalem / liste / çöp kutusu ikonları. Metin ve iki okuma olasılığı formla uyumlu. Formdaki "web arayüzü" ve "firma ikonu" ifadeleri karede yazmıyor; ikon anlamları ve logo simgelerinin firma olduğu çıkarımdır. Gerçek düzenleme/silme yetkisi kanıtı değildir (LI-Q04). |

**Paket sonucu:** 6/6 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 5, Ek 1,
Arşiv 0; hiçbir görsel silinmedi veya yeniden adlandırılmadı. Logo İşbaşı
görselleri 6/6. Form düzeltmeleri ve soru ilerlemesi E0392 P1-logo-isbasi-G01
bölümündedir.

Güncel fiziksel toplam 360 PNG; incelenen 160 (eski korpus 157/357 + kullanıcıdan 3/3).
Goodbudget 31/31, KolayBi 39/39, Hesap Defterim 45/45, Money Manager 30/30, Paraşüt 9/9, Logo İşbaşı 6/6.


## P1-quickbooks-G01 — 14 Eylül 2026

Görseller doğrudan açılarak incelendi. Kaynak: Android emülatörde QuickBooks
mobil onboarding'i, manuel gözlem. Durum çubuğu E0268 1:34 (formun 1 Eylül
beyanı; aynı gecenin Paraşüt 1:18–1:19 ve Logo 1:27–1:30 kareleriyle aynı
oturum aralığı), E0269–E0271 7:32–7:33 (kullanıcının hesap açtığı 2 Eylül
beyanı). Dosyalar 7 Eylül'de kopyalanmış; 4 dosyanın SHA-256 değeri başlangıç
kaydıyla aynı. Hesap, deneme veya ödeme işlemi yapılmadı; E kimlikleri ve dosya
adları değişmez. B13 bağlamı: dört kare Solopreneur değil, QuickBooks mobil
onboarding'i ve QBO Simple Start plan ekranıdır.

| Kanıt | Dosya | Rol | İçerik, metin karşılaştırması ve sınır |
|---|---|---|---|
| E0268 | 01-onboarding.png | Ana | 1:34 karusel: telefonlu kadın ve bisiklet illüstrasyonu, "Get paid anywhere / Send and track custom invoices.", üç noktadan 1. etkin (yeşil), yeşil `Create account`, çerçeveli `Sign in`. Formla birebir. Karede ürün adı (QuickBooks veya Solopreneur) ve sürüm yok. |
| E0269 | 02-welcome-get-started.png | Ana | 7:32 "Welcome to QuickBooks, we're glad you're here"; üç madde ve telefon illüstrasyonları: "First, let's talk about your business.", "Then, choose a plan or try it out for free.", "After that, we'll get some things set up for you."; `Get started`. Metinler formla birebir. Başlık "QuickBooks" diyor, Solopreneur değil. Formun bu kareye bağladığı e-posta + SMS hesap açma adımları karede yok. |
| E0270 | 03-onboarding-basic-info.png | Ana | 7:32 `Sign out`; "Let's begin with some basic info", "We'll use this to get you started in QuickBooks."; `Business name` alanı boş; "No business name? Use your name"; `Next`. Alan ve bağlantı formla birebir. Zorunluluk işareti yok; formdaki "Deniz Tasarim" girişi karede görünmüyor (koşum notu). |
| E0271 | 04-choose-plan-paywall.png | Ana | 7:33 geri oku + başlık `Simple Start`; mavi bant "Subscribe and save or try free for 1 month*"; kart `Simple Start`, üstü çizili TRY819.99, TRY244.99/mo, "for 6 months", `Get started`; ⓘ simgeli beş özellik (Track income/expenses, Send custom invoices/estimates, Auto-track mileage, Create custom categories, Run reports); altta Simple Start details* / Privacy / Terms of service. K00 satırı birebir. Deneme ifadesi karede var; formun "04'te ücretsiz yol görünmüyor" hükmü düzeltildi: ayrı deneme düğmesi yok, tek eylem Get started, ne başlattığı belli değil (QB-Q03). Oran 819,99 / 244,99 ≈ 3,35; kampanya sonrası fiyat ve yıldız koşulları karede yok. Plan listesi yok; özellikler Simple Start'a aittir, Solopreneur'e değil (B13). |

**Paket sonucu:** 4/4 görsel açıldı ve kaydedildi. Rol dağılımı: Ana 4, Ek 0,
Arşiv 0; hiçbir görsel silinmedi veya yeniden adlandırılmadı. QuickBooks
görselleri 4/4. Form düzeltmeleri ve soru ilerlemesi E0392 P1-quickbooks-G01
bölümündedir.

Güncel fiziksel toplam 360 PNG; incelenen 164 (eski korpus 161/357 + kullanıcıdan 3/3).
Goodbudget 31/31, KolayBi 39/39, Hesap Defterim 45/45, Money Manager 30/30, Paraşüt 9/9, Logo İşbaşı 6/6, QuickBooks 4/4.


## P1-B13 — QuickBooks ürün/paket ayrımı — 14 Eylül 2026

Görsel değişikliği yok. Dayanak: E0268–E0271 (P1-quickbooks-G01; Solopreneur
adı hiçbir karede yok, E0271 QBO Simple Start). Metin düzeltmeleri: E0012
QuickBooks formu (10 yer), E0272 kanitlar/README.md klasör etiketi (1),
DURUM.md (1), E0380 MANUEL-TEST-PROTOKOLU.md (1), E0387 README.md (2).
Ayrıntı E0392 P1-B13 bölümünde. Önceki hash değerleri kendi kayıt anlarına aittir.


## P1-B18 — Arayüz taraması tablolarının kanıt sütunu — 14 Eylül 2026

Görsel değişikliği yok. Metin düzeltmeleri: E0005 Bluecoins, E0010 Money
Manager ve E0014 Wallet formlarında arayüz taraması tablosunun başlık ve
ayracına `Kanıt` sütunu eklendi (24 veri satırı hizalandı); E0010'da E0242,
E0243, E0246 ve E0249 birer kez tam dosya adıyla anıldı. Ayrıntı E0392 P1-B18
bölümünde. Önceki hash değerleri kendi kayıt anlarına aittir.


## P1-K — Mekanik kanıt kapısı — 14 Eylül 2026

Görsel değişikliği yok. Yeni dosyalar: E0393 `denetim.cjs` (kapı), E0394
`denetim-test.cjs` (15 pozitif/negatif test). E0002 `denetim.sh` sarmalayıcıya
dönüştü. Metin düzeltmeleri: E0010 Money Manager, E0007 Hesap Defterim, E0006
Goodbudget, E0009 Logo İşbaşı, E0008 KolayBi ve E0012 QuickBooks formlarında 22
belirsiz aralık atıfı (kapsam bildirenler E kimliği aralığına, iddia
destekleyenler tam ad listesine) ve Hesap Defterim'de doğrulanamayan "on kare"
sayısı; E0380 MANUEL-TEST-PROTOKOLU (2), DURUM (1), E0008 KolayBi notu (1),
E0272 kanitlar/README klasör etiketleri (3). P1'in yedi formu kapıdan 0 hatayla
geçiyor. Ayrıntı E0392 P1-K bölümünde. Önceki hash değerleri kendi kayıt
anlarına aittir.


## P2-G01 ve P1-K sıkı atıf kararı — 14 Eylül 2026

E0016–E0032 (17 PNG) ayrı ayrı açıldı; içerik, iddia sınırı ve kullanım rolü
E0392 BULGU-DOGRULAMA-KAYDI.md P2-G01 tablosunda. Bluecoins 17/90,
toplam 181/360. Kaynak görsellerin hiçbiri değiştirilmedi veya yeniden
adlandırılmadı; yanıltıcı dosya adları E0005 formunda gerçek içerikle açıklandı.
E0033 ve sonrasındaki Bluecoins kareleri bu tur incelenmedi.

E0005 formunun Tur 1 ve Faz 3 çekirdek iddiaları düzeltildi. P1'in yedi
formunda 112 kısa atıf tam ada dönüştü; Money Manager 05 boş numara notu
atıf biçiminden çıkarıldı. E0002/E0393/E0394 sıkı kapı ve testleri,
E0380 protokol ve mevcut durum/giriş belgeleri güncellendi. Yeni dosya veya
kimlik eklenmedi. Önceki metin hash'leri kendi kayıt anlarını gösterir.


## P2-G02 — Bluecoins E0033–E0050 — 15 Eylül 2026

18/18 görsel ayrı açıldı; Bluecoins 35/90, toplam 199/360.
Kaynak türü önceki manuel Android emülatör ekran görüntüleridir; yakalama
tarihi koşum kaydında 10 Eylül 2026, ürün/sürüm cihazdan bu tur doğrulanmadı.
İşlem/vade tarihleri, görünen içerik, B10 ve BC soru bağları E0392
BULGU-DOGRULAMA-KAYDI.md P2-G02 tablosunda; hedef form E0005.
Kullanım rolleri rapor adaylarıdır, yayın veya ürün kararı değildir.
Form/ayar için dönem uygulanmaz. Kaynak dosya/PNG/hash/kimlik değişmedi.

| Kimlik | Görsel | Kullanım rolü | Kontrol |
|---|---|---|---|
| E0033 | [17-b2-taksit-sartlari-sheet.png](kanitlar/bluecoins/17-b2-taksit-sartlari-sheet.png) | Ana anlatım — taksit ayar yüzeyi | İncelendi; ayrıntı E0392 P2-G02 |
| E0034 | [18-b2-taksit-6ay-15agu.png](kanitlar/bluecoins/18-b2-taksit-6ay-15agu.png) | Ana anlatım — geçmiş tarihli kurulum | İncelendi; ayrıntı E0392 P2-G02 |
| E0035 | [19-b2-6ay-hatirlatici-metni.png](kanitlar/bluecoins/19-b2-6ay-hatirlatici-metni.png) | Ana anlatım — plan önizlemesi | İncelendi; ayrıntı E0392 P2-G02 |
| E0036 | [20-b2-1-6-taksit-1000-kayit.png](kanitlar/bluecoins/20-b2-1-6-taksit-1000-kayit.png) | Ana anlatım — gerçekleşen ilk taksit | İncelendi; ayrıntı E0392 P2-G02 |
| E0037 | [21-b2-kalan-5-taksit-hatirlatici.png](kanitlar/bluecoins/21-b2-kalan-5-taksit-hatirlatici.png) | Ana anlatım — kalan taksitler | İncelendi; ayrıntı E0392 P2-G02 |
| E0038 | [22-b2-sonrasi-rapor-gider-3050.png](kanitlar/bluecoins/22-b2-sonrasi-rapor-gider-3050.png) | Ana anlatım — taksit sonrası rapor | İncelendi; ayrıntı E0392 P2-G02 |
| E0039 | [23-b1-planli-islem-aylik-sheet.png](kanitlar/bluecoins/23-b1-planli-islem-aylik-sheet.png) | Ana anlatım — tekrar ve otomasyon seçimi | İncelendi; ayrıntı E0392 P2-G02 |
| E0040 | [24-b1-yenilenen-islem-banner.png](kanitlar/bluecoins/24-b1-yenilenen-islem-banner.png) | Ana anlatım — geçmiş başlangıç | İncelendi; ayrıntı E0392 P2-G02 |
| E0041 | [25-b1-hatirlatici-gecikmeli-bugun.png](kanitlar/bluecoins/25-b1-hatirlatici-gecikmeli-bugun.png) | Ana anlatım — birleşik bekleyen liste | İncelendi; ayrıntı E0392 P2-G02 |
| E0042 | [26-b1-hatirlatici-detay-kaydet.png](kanitlar/bluecoins/26-b1-hatirlatici-detay-kaydet.png) | Ana anlatım — hatırlatıcı detayı | İncelendi; ayrıntı E0392 P2-G02 |
| E0043 | [27-b1-islem-olarak-kaydet-bugun-mu.png](kanitlar/bluecoins/27-b1-islem-olarak-kaydet-bugun-mu.png) | Ana anlatım — gerçekleşme tarihi kararı | İncelendi; ayrıntı E0392 P2-G02 |
| E0044 | [28-b1-onay-sonrasi-rapor-gider-3650.png](kanitlar/bluecoins/28-b1-onay-sonrasi-rapor-gider-3650.png) | Ana anlatım — onay sonrası akış raporu | İncelendi; ayrıntı E0392 P2-G02 |
| E0045 | [29-bolmek-split-modu.png](kanitlar/bluecoins/29-bolmek-split-modu.png) | Ana anlatım — bölme formu; işleyiş kanıtı değil | İncelendi; ayrıntı E0392 P2-G02 |
| E0046 | [30-bagimsiz-hatirlatici-bir-kez-program.png](kanitlar/bluecoins/30-bagimsiz-hatirlatici-bir-kez-program.png) | Ana anlatım — tek seferlik plan formu | İncelendi; ayrıntı E0392 P2-G02 |
| E0047 | [31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png](kanitlar/bluecoins/31-hatirlatici-listesi-bagimsiz-tekrar-taksit.png) | Ana anlatım — birleşik görünüm | İncelendi; ayrıntı E0392 P2-G02 |
| E0048 | [32-kart-hesap-kesim-gunu-limit-alanlari.png](kanitlar/bluecoins/32-kart-hesap-kesim-gunu-limit-alanlari.png) | Ana anlatım — kart hesap formu | İncelendi; ayrıntı E0392 P2-G02 |
| E0049 | [33-cari-hesap-olusturuldu.png](kanitlar/bluecoins/33-cari-hesap-olusturuldu.png) | Ana anlatım — hesap durumu ve cari satırı | İncelendi; ayrıntı E0392 P2-G02 |
| E0050 | [34-kismi-kart-odemesi-500-transfer.png](kanitlar/bluecoins/34-kismi-kart-odemesi-500-transfer.png) | Ana anlatım — kısmi ödeme ve B1 gerçek kayıt | İncelendi; ayrıntı E0392 P2-G02 |

B1/B2 koşul ayrımı, 43.950→43.350 net zinciri ve 500 transferin nötrlüğü
kaydedildi. Cari alan yokluğu, ekstre işleyişi, otomatik açık kol,
taslak/fiş/gerçek cihaz iddiaları doğrulanmış sayılmadı. B10 bütünü açık.

## P2-G03 görsel inceleme kaydı — 15 Eylül 2026

29/29 incelendi; Bluecoins 64/90, toplam 228/360. Kaynak önceki manuel
Android emülatör koşumu (11 Eylül); bu tur cihaz/sürüm yeniden doğrulanmadı.
Hedef E0005; içerik, tarih, sınır ve soru bağları E0392 P2-G03 tablosunda.
Roller rapor adayıdır; kaynak PNG/hash/kimlik değişmedi.

| Kimlik | Görsel | Kullanım rolü | Kontrol |
|---|---|---|---|
| E0051 | [f7-00-baslangic.png](kanitlar/bluecoins/f7-00-baslangic.png) | Kanıt eki — başlangıç ve reklam | İncelendi; ayrıntı E0392 P2-G03 |
| E0052 | [f7-01-hesaplar-scroll.png](kanitlar/bluecoins/f7-01-hesaplar-scroll.png) | Kanıt eki — takvim ve akış | İncelendi; ayrıntı E0392 P2-G03 |
| E0053 | [f7-02-hesaplar-scroll2.png](kanitlar/bluecoins/f7-02-hesaplar-scroll2.png) | Ana anlatım — başlangıç toplamları | İncelendi; ayrıntı E0392 P2-G03 |
| E0054 | [f7-03-hesap-listesi.png](kanitlar/bluecoins/f7-03-hesap-listesi.png) | Ana anlatım — başlangıç hesapları | İncelendi; ayrıntı E0392 P2-G03 |
| E0055 | [f7-04-tum-hesaplar.png](kanitlar/bluecoins/f7-04-tum-hesaplar.png) | Ana anlatım — dönem ve cari başlangıç | İncelendi; ayrıntı E0392 P2-G03 |
| E0056 | [f7-05-hatirlaticilar.png](kanitlar/bluecoins/f7-05-hatirlaticilar.png) | Ana anlatım — D1 öncesi bekleyenler | İncelendi; ayrıntı E0392 P2-G03 |
| E0057 | [f7-06-ofis-kirasi-detay.png](kanitlar/bluecoins/f7-06-ofis-kirasi-detay.png) | Arşiv — gezinme ara karesi | İncelendi; ayrıntı E0392 P2-G03 |
| E0058 | [f7-07-tap-icon.png](kanitlar/bluecoins/f7-07-tap-icon.png) | Arşiv — sonuç üretmeyen ara kare | İncelendi; ayrıntı E0392 P2-G03 |
| E0059 | [f7-08-tap-retry.png](kanitlar/bluecoins/f7-08-tap-retry.png) | Arşiv — tekrar kontrolü | İncelendi; ayrıntı E0392 P2-G03 |
| E0060 | [f7-09-ofis-kirasi-dialog.png](kanitlar/bluecoins/f7-09-ofis-kirasi-dialog.png) | Ana anlatım — D1 detay ve eylemler | İncelendi; ayrıntı E0392 P2-G03 |
| E0061 | [f7-10-ofis-kirasi-kaydet-sonuc.png](kanitlar/bluecoins/f7-10-ofis-kirasi-kaydet-sonuc.png) | Arşiv — ara kare | İncelendi; ayrıntı E0392 P2-G03 |
| E0062 | [f7-11-kaydet-dogru.png](kanitlar/bluecoins/f7-11-kaydet-dogru.png) | Ana anlatım — onay | İncelendi; ayrıntı E0392 P2-G03 |
| E0063 | [f7-12-ofis-kirasi-realized.png](kanitlar/bluecoins/f7-12-ofis-kirasi-realized.png) | Ana anlatım — bekleyen listesinin sonrası | İncelendi; ayrıntı E0392 P2-G03 |
| E0064 | [f7-13-islemler.png](kanitlar/bluecoins/f7-13-islemler.png) | Arşiv — kaydırma ara karesi | İncelendi; ayrıntı E0392 P2-G03 |
| E0065 | [f7-14-islemler-guncel.png](kanitlar/bluecoins/f7-14-islemler-guncel.png) | Ana anlatım — D1 sonucu | İncelendi; ayrıntı E0392 P2-G03 |
| E0066 | [f7-15-yeni-islem.png](kanitlar/bluecoins/f7-15-yeni-islem.png) | Arşiv — form öncesi | İncelendi; ayrıntı E0392 P2-G03 |
| E0067 | [f7-16-fab-tap.png](kanitlar/bluecoins/f7-16-fab-tap.png) | Ana anlatım — ortak form | İncelendi; ayrıntı E0392 P2-G03 |
| E0068 | [f7-17-isim-girildi.png](kanitlar/bluecoins/f7-17-isim-girildi.png) | Kanıt eki — ad girişi | İncelendi; ayrıntı E0392 P2-G03 |
| E0069 | [f7-18-gelir-tutar.png](kanitlar/bluecoins/f7-18-gelir-tutar.png) | Arşiv — tutar odağı | İncelendi; ayrıntı E0392 P2-G03 |
| E0070 | [f7-19-tutar-girildi.png](kanitlar/bluecoins/f7-19-tutar-girildi.png) | Kanıt eki — tutar girişi | İncelendi; ayrıntı E0392 P2-G03 |
| E0071 | [f7-20-hesap-secim.png](kanitlar/bluecoins/f7-20-hesap-secim.png) | Kanıt eki — kategori seçicisi | İncelendi; ayrıntı E0392 P2-G03 |
| E0072 | [f7-21-hesap-secici.png](kanitlar/bluecoins/f7-21-hesap-secici.png) | Ana anlatım — hesap seçicisi | İncelendi; ayrıntı E0392 P2-G03 |
| E0073 | [f7-22-hesap-secildi.png](kanitlar/bluecoins/f7-22-hesap-secildi.png) | Arşiv — hesap düzeltmesi öncesi | İncelendi; ayrıntı E0392 P2-G03 |
| E0074 | [f7-23-hesap-dogru-secildi.png](kanitlar/bluecoins/f7-23-hesap-dogru-secildi.png) | Ana anlatım — D2 hazır form | İncelendi; ayrıntı E0392 P2-G03 |
| E0075 | [f7-24-d2-kaydedildi.png](kanitlar/bluecoins/f7-24-d2-kaydedildi.png) | Ana anlatım — D2 sonucu | İncelendi; ayrıntı E0392 P2-G03 |
| E0076 | [f7-25-transfer-formu.png](kanitlar/bluecoins/f7-25-transfer-formu.png) | Kanıt eki — transfer başlangıcı | İncelendi; ayrıntı E0392 P2-G03 |
| E0077 | [f7-26-check.png](kanitlar/bluecoins/f7-26-check.png) | Arşiv — ad girişi | İncelendi; ayrıntı E0392 P2-G03 |
| E0078 | [f7-27-transfer-hazir.png](kanitlar/bluecoins/f7-27-transfer-hazir.png) | Ana anlatım — D3 formu | İncelendi; ayrıntı E0392 P2-G03 |
| E0079 | [f7-28-d3-kaydedildi.png](kanitlar/bluecoins/f7-28-d3-kaydedildi.png) | Ana anlatım — D3 sonucu | İncelendi; ayrıntı E0392 P2-G03 |

## P2-G04 görsel inceleme kaydı — 15 Eylül 2026

26/26 incelendi; Bluecoins 90/90, toplam 254/360. Kaynak önceki manuel
Android emülatör koşumu (11 Eylül); bu tur cihaz/sürüm yeniden doğrulanmadı.
Hedef E0005; içerik, sınır ve soru bağları E0392 P2-G04 tablosunda.
Roller rapor adayıdır; kaynak PNG/hash/kimlik değişmedi.

| Kimlik | Görsel | Kullanım rolü | Kontrol |
|---|---|---|---|
| E0080 | [f7-29-arama.png](kanitlar/bluecoins/f7-29-arama.png) | Arşiv — arama öncesi | İncelendi; ayrıntı E0392 P2-G04 |
| E0081 | [f7-30-arama2.png](kanitlar/bluecoins/f7-30-arama2.png) | Ana anlatım — arama sonucu | İncelendi; ayrıntı E0392 P2-G04 |
| E0082 | [f7-31-filtre.png](kanitlar/bluecoins/f7-31-filtre.png) | Ana anlatım — filtre yüzeyi | İncelendi; ayrıntı E0392 P2-G04 |
| E0083 | [f7-32-menu.png](kanitlar/bluecoins/f7-32-menu.png) | Arşiv — filtre tekrarı | İncelendi; ayrıntı E0392 P2-G04 |
| E0084 | [f7-33-menu2.png](kanitlar/bluecoins/f7-33-menu2.png) | Ana anlatım — gezinme çekmecesi | İncelendi; ayrıntı E0392 P2-G04 |
| E0085 | [f7-34-kategoriler.png](kanitlar/bluecoins/f7-34-kategoriler.png) | Ana anlatım — D3 sonrası hesaplar | İncelendi; ayrıntı E0392 P2-G04 |
| E0086 | [f7-35-nakit-akim-ayari.png](kanitlar/bluecoins/f7-35-nakit-akim-ayari.png) | Ana anlatım — nakit akışı kapsam ayarı | İncelendi; ayrıntı E0392 P2-G04 |
| E0087 | [f7-36-kategoriler2.png](kanitlar/bluecoins/f7-36-kategoriler2.png) | Ana anlatım — kategori hiyerarşisi | İncelendi; ayrıntı E0392 P2-G04 |
| E0088 | [f7-37-etiketler.png](kanitlar/bluecoins/f7-37-etiketler.png) | Ana anlatım — etiket listesi | İncelendi; ayrıntı E0392 P2-G04 |
| E0089 | [f7-38-cop-kutusu.png](kanitlar/bluecoins/f7-38-cop-kutusu.png) | Ana anlatım — boş çöp kutusu | İncelendi; ayrıntı E0392 P2-G04 |
| E0090 | [f7-39-ayarlar.png](kanitlar/bluecoins/f7-39-ayarlar.png) | Ana anlatım — ayar merkezi | İncelendi; ayrıntı E0392 P2-G04 |
| E0091 | [f7-40-diger-ayarlar.png](kanitlar/bluecoins/f7-40-diger-ayarlar.png) | Ana anlatım — veri yönetimi | İncelendi; ayrıntı E0392 P2-G04 |
| E0092 | [f7-41-diger-ayarlar2.png](kanitlar/bluecoins/f7-41-diger-ayarlar2.png) | Ana anlatım — diğer ayarlar | İncelendi; ayrıntı E0392 P2-G04 |
| E0093 | [f7-42-hesap-ayarlari.png](kanitlar/bluecoins/f7-42-hesap-ayarlari.png) | Ana anlatım — kategori ayarları | İncelendi; ayrıntı E0392 P2-G04 |
| E0094 | [f7-43-hesap-ayarlari2.png](kanitlar/bluecoins/f7-43-hesap-ayarlari2.png) | Ana anlatım — hesap ayarları | İncelendi; ayrıntı E0392 P2-G04 |
| E0095 | [f7-44-gelismis-ayarlar.png](kanitlar/bluecoins/f7-44-gelismis-ayarlar.png) | Ana anlatım — gelişmiş ayarlar üstü | İncelendi; ayrıntı E0392 P2-G04 |
| E0096 | [f7-45-gelismis-scroll.png](kanitlar/bluecoins/f7-45-gelismis-scroll.png) | Kanıt eki — gelişmiş ayarlar altı | İncelendi; ayrıntı E0392 P2-G04 |
| E0097 | [f7-46-check-nav.png](kanitlar/bluecoins/f7-46-check-nav.png) | Arşiv — boş ekran tekrarı | İncelendi; ayrıntı E0392 P2-G04 |
| E0098 | [f7-47-takvim.png](kanitlar/bluecoins/f7-47-takvim.png) | Ana anlatım — günlük takvim kırılımı | İncelendi; ayrıntı E0392 P2-G04 |
| E0099 | [f7-48-takvim-ayarlari.png](kanitlar/bluecoins/f7-48-takvim-ayarlari.png) | Kanıt eki — takvim toplamı | İncelendi; ayrıntı E0392 P2-G04 |
| E0100 | [f7-49-seyahat-modu.png](kanitlar/bluecoins/f7-49-seyahat-modu.png) | Kanıt eki — seyahat anahtarı | İncelendi; ayrıntı E0392 P2-G04 |
| E0101 | [f7-50-seyahat-toggle.png](kanitlar/bluecoins/f7-50-seyahat-toggle.png) | Arşiv — bağlamı belirsiz etiket seçimi | İncelendi; ayrıntı E0392 P2-G04 |
| E0102 | [f7-51-check.png](kanitlar/bluecoins/f7-51-check.png) | Arşiv — seyahat durumu tekrarı | İncelendi; ayrıntı E0392 P2-G04 |
| E0103 | [f7-52-nav-check.png](kanitlar/bluecoins/f7-52-nav-check.png) | Kanıt eki — dashboard dönüşü | İncelendi; ayrıntı E0392 P2-G04 |
| E0104 | [f7-53-export.png](kanitlar/bluecoins/f7-53-export.png) | Arşiv — filtre tekrarı | İncelendi; ayrıntı E0392 P2-G04 |
| E0105 | [f7-54-print.png](kanitlar/bluecoins/f7-54-print.png) | Ana anlatım — çıktı seçenekleri | İncelendi; ayrıntı E0392 P2-G04 |

## P3-G01 görsel inceleme kaydı — 15 Eylül 2026

25/25 incelendi; Wallet 25/106, toplam 279/360. Kaynaklar önceki manuel
Android emülatör koşumlarıdır: E0274–E0282 Tur 1, E0283–E0298 Faz 2
(10 Eylül 2026). Bu tur cihaz, oturum ve sürüm yeniden doğrulanmadı.
Hedef E0014; içerik, sınır ve soru bağları E0392 P3-G01 tablosunda.
Roller rapor adayıdır; kaynak PNG/hash/kimlik değişmedi.

| Kimlik | Görsel | Kullanım rolü | Kontrol |
|---|---|---|---|
| E0274 | [00-magaza.png](kanitlar/wallet-budgetbakers/00-magaza.png) | Ana anlatım — Tur 1 Home alt kartları; mağaza kanıtı değil | İncelendi; ayrıntı E0392 P3-G01 |
| E0275 | [02b-menu.png](kanitlar/wallet-budgetbakers/02b-menu.png) | Ana anlatım — Tur 1 gezinme çekmecesinin üst bölümü | İncelendi; ayrıntı E0392 P3-G01 |
| E0276 | [03-dolu-ana-ekran.png](kanitlar/wallet-budgetbakers/03-dolu-ana-ekran.png) | Ana anlatım — Tur 1 Home hesapları ve tanıtım kartları | İncelendi; ayrıntı E0392 P3-G01 |
| E0277 | [04-islem-formu.png](kanitlar/wallet-budgetbakers/04-islem-formu.png) | Kanıt eki — Tur 1 hızlı form; doğru gelir sonucun kanıtı değil | İncelendi; ayrıntı E0392 P3-G01 |
| E0278 | [05-siniflandirma.png](kanitlar/wallet-budgetbakers/05-siniflandirma.png) | Ana anlatım — Tur 1 Spending kategori raporu | İncelendi; ayrıntı E0392 P3-G01 |
| E0279 | [06-islem-listesi.png](kanitlar/wallet-budgetbakers/06-islem-listesi.png) | Ana anlatım — Tur 1 Records ve çift satırlı transferler | İncelendi; ayrıntı E0392 P3-G01 |
| E0280 | [07-rapor.png](kanitlar/wallet-budgetbakers/07-rapor.png) | Ana anlatım — Tur 1 Cash Flow özeti | İncelendi; ayrıntı E0392 P3-G01 |
| E0281 | [08-hata-veya-bos-durum.png](kanitlar/wallet-budgetbakers/08-hata-veya-bos-durum.png) | Ana anlatım — Tur 1 sıfır tutar doğrulaması | İncelendi; ayrıntı E0392 P3-G01 |
| E0282 | [09-ozgun-ozellik.png](kanitlar/wallet-budgetbakers/09-ozgun-ozellik.png) | Ana anlatım — Tur 1 Planned payments boş durumu | İncelendi; ayrıntı E0392 P3-G01 |
| E0283 | [10-kontrol-hesaplar.png](kanitlar/wallet-budgetbakers/10-kontrol-hesaplar.png) | Ana anlatım — Faz 2 çekirdek hesap bakiyeleri | İncelendi; ayrıntı E0392 P3-G01 |
| E0284 | [11-kontrol-cash-flow-agustos.png](kanitlar/wallet-budgetbakers/11-kontrol-cash-flow-agustos.png) | Ana anlatım — Faz 2 çekirdek Cash Flow; 12 hafta | İncelendi; ayrıntı E0392 P3-G01 |
| E0285 | [12-kontrol-records-listesi.png](kanitlar/wallet-budgetbakers/12-kontrol-records-listesi.png) | Ana anlatım — Faz 2 çekirdek Records listesi | İncelendi; ayrıntı E0392 P3-G01 |
| E0286 | [13-planned-payments-bos-durum.png](kanitlar/wallet-budgetbakers/13-planned-payments-bos-durum.png) | Arşiv — Faz 2 öncesi boş durum; E0282 tekrarı | İncelendi; ayrıntı E0392 P3-G01 |
| E0287 | [14-planned-gecmis-tarih-kapali.png](kanitlar/wallet-budgetbakers/14-planned-gecmis-tarih-kapali.png) | Ana anlatım — B1 başlangıç tarihi seçicisi | İncelendi; ayrıntı E0392 P3-G01 |
| E0288 | [15-b1-tekrarlayan-form.png](kanitlar/wallet-budgetbakers/15-b1-tekrarlayan-form.png) | Ana anlatım — B1 plan formunun üst bölümü | İncelendi; ayrıntı E0392 P3-G01 |
| E0289 | [16-b1-plan-detay-due-today.png](kanitlar/wallet-budgetbakers/16-b1-plan-detay-due-today.png) | Ana anlatım — bekleyen örnek ve Confirm | İncelendi; ayrıntı E0392 P3-G01 |
| E0290 | [17-b1-confirm-payment-summary.png](kanitlar/wallet-budgetbakers/17-b1-confirm-payment-summary.png) | Ana anlatım — onay öncesi ödeme özeti | İncelendi; ayrıntı E0392 P3-G01 |
| E0291 | [18-b1-otomatik-mi-onayli-mi-secimi.png](kanitlar/wallet-budgetbakers/18-b1-otomatik-mi-onayli-mi-secimi.png) | Ana anlatım — otomatik/onaylı sorusu; Yes önceden seçili | İncelendi; ayrıntı E0392 P3-G01 |
| E0292 | [19-b1-onay-sonrasi-paid-today-siradaki-pending.png](kanitlar/wallet-budgetbakers/19-b1-onay-sonrasi-paid-today-siradaki-pending.png) | Ana anlatım — gerçekleşen ve sıradaki bekleyen örnek | İncelendi; ayrıntı E0392 P3-G01 |
| E0293 | [20-b1-sonrasi-ana-hesap-20200.png](kanitlar/wallet-budgetbakers/20-b1-sonrasi-ana-hesap-20200.png) | Kanıt eki — B1 sonrası Ana Hesap bakiyesi | İncelendi; ayrıntı E0392 P3-G01 |
| E0294 | [21-b2-islem-detay-taksit-alani-yok.png](kanitlar/wallet-budgetbakers/21-b2-islem-detay-taksit-alani-yok.png) | Ana anlatım — kayıt ayrıntı alanları; görünen bölümde taksit yok | İncelendi; ayrıntı E0392 P3-G01 |
| E0295 | [22-tarih-secici-onceki-ay-yok.png](kanitlar/wallet-budgetbakers/22-tarih-secici-onceki-ay-yok.png) | Kanıt eki — normal kayıt tarih seçicisi; B1 başlangıcı değil | İncelendi; ayrıntı E0392 P3-G01 |
| E0296 | [23-b2-6000-tek-kart-borcu.png](kanitlar/wallet-budgetbakers/23-b2-6000-tek-kart-borcu.png) | Ana anlatım — tek parça kart borcu ve eşik uyarısı | İncelendi; ayrıntı E0392 P3-G01 |
| E0297 | [24-kart-hesap-detay-negatif-bakiye.png](kanitlar/wallet-budgetbakers/24-kart-hesap-detay-negatif-bakiye.png) | Ana anlatım — kart hesabı detayı ve son kayıtlar | İncelendi; ayrıntı E0392 P3-G01 |
| E0298 | [25-kart-hesap-ayarlari.png](kanitlar/wallet-budgetbakers/25-kart-hesap-ayarlari.png) | Ana anlatım — kart hesap ayarları | İncelendi; ayrıntı E0392 P3-G01 |

## P3-G02 görsel inceleme kaydı — 15 Eylül 2026

22/22 incelendi; Wallet 47/106, toplam 301/360. Kaynak önceki manuel Android
emülatör koşumudur (Faz 2 ve ek koşum, 10 Eylül 2026); bu tur cihaz, oturum ve
sürüm yeniden doğrulanmadı. Hedef E0014; içerik,
sınır ve soru bağları E0392 P3-G02 tablosunda. Roller rapor adayıdır; kaynak
PNG/hash/kimlik değişmedi.

| Kimlik | Görsel | Kullanım rolü | Kontrol |
|---|---|---|---|
| E0299 | [26-butce-olusturma-formu.png](kanitlar/wallet-budgetbakers/26-butce-olusturma-formu.png) | Kanıt eki — boş bütçe formu ve varsayılanlar | İncelendi; ayrıntı E0392 P3-G02 |
| E0300 | [27-butce-formu-dolu.png](kanitlar/wallet-budgetbakers/27-butce-formu-dolu.png) | Ana anlatım — bütçe kurulum alanları | İncelendi; ayrıntı E0392 P3-G02 |
| E0301 | [28-butce-olusturuldu-over-budget.png](kanitlar/wallet-budgetbakers/28-butce-olusturuldu-over-budget.png) | Ana anlatım — kurulduğu anda aşım | İncelendi; ayrıntı E0392 P3-G02 |
| E0302 | [29-butce-detay-6600-harcama.png](kanitlar/wallet-budgetbakers/29-butce-detay-6600-harcama.png) | Ana anlatım — bütçe detayı, tahmin ve kıyas | İncelendi; ayrıntı E0392 P3-G02 |
| E0303 | [30-goal-olusturma.png](kanitlar/wallet-budgetbakers/30-goal-olusturma.png) | Kanıt eki — hedef şablonları; yerelleştirme kusuru | İncelendi; ayrıntı E0392 P3-G02 |
| E0304 | [31-goal-detay-formu.png](kanitlar/wallet-budgetbakers/31-goal-detay-formu.png) | Ana anlatım — hedef alanları | İncelendi; ayrıntı E0392 P3-G02 |
| E0305 | [32-butce-ve-goal-birlikte.png](kanitlar/wallet-budgetbakers/32-butce-ve-goal-birlikte.png) | Ana anlatım — bütçe ve hedef özet kartları | İncelendi; ayrıntı E0392 P3-G02 |
| E0306 | [33-planned-payments-b1-siradaki.png](kanitlar/wallet-budgetbakers/33-planned-payments-b1-siradaki.png) | Kanıt eki — plan listesi ve sıradaki vade | İncelendi; ayrıntı E0392 P3-G02 |
| E0307 | [34-debts-bos-durum.png](kanitlar/wallet-budgetbakers/34-debts-bos-durum.png) | Ana anlatım — Debts boş durumu | İncelendi; ayrıntı E0392 P3-G02 |
| E0308 | [35-debt-kayit-baglama-sorusu.png](kanitlar/wallet-budgetbakers/35-debt-kayit-baglama-sorusu.png) | Ana anlatım — borcu mevcut kayda bağlama sorusu | İncelendi; ayrıntı E0392 P3-G02 |
| E0309 | [36-debt-i-lent-formu.png](kanitlar/wallet-budgetbakers/36-debt-i-lent-formu.png) | Ana anlatım — boş borç formu ve varsayılan vade | İncelendi; ayrıntı E0392 P3-G02 |
| E0310 | [37-records-listesi-b1-b2.png](kanitlar/wallet-budgetbakers/37-records-listesi-b1-b2.png) | Ana anlatım — B1/B2 sonrası kayıt listesi | İncelendi; ayrıntı E0392 P3-G02 |
| E0311 | [38-split-record-ekrani.png](kanitlar/wallet-budgetbakers/38-split-record-ekrani.png) | Kanıt eki — split girişi | İncelendi; ayrıntı E0392 P3-G02 |
| E0312 | [39-split-yeni-kayit-dialog.png](kanitlar/wallet-budgetbakers/39-split-yeni-kayit-dialog.png) | Ana anlatım — split alt kayıt alanları | İncelendi; ayrıntı E0392 P3-G02 |
| E0313 | [40-add-receipt-dosya-veya-foto.png](kanitlar/wallet-budgetbakers/40-add-receipt-dosya-veya-foto.png) | Ana anlatım — fiş eki seçenekleri ve kilitli kural | İncelendi; ayrıntı E0392 P3-G02 |
| E0314 | [41-debt-i-lent-formu-dolu.png](kanitlar/wallet-budgetbakers/41-debt-i-lent-formu-dolu.png) | Kanıt eki — ek koşum borç değerleri | İncelendi; ayrıntı E0392 P3-G02 |
| E0315 | [42-debt-kayit-olustur-mu-bakiye-degisir.png](kanitlar/wallet-budgetbakers/42-debt-kayit-olustur-mu-bakiye-degisir.png) | Ana anlatım — borçta kayıt oluşturma sorusu | İncelendi; ayrıntı E0392 P3-G02 |
| E0316 | [43-debt-olusturuldu-i-lent.png](kanitlar/wallet-budgetbakers/43-debt-olusturuldu-i-lent.png) | Ana anlatım — oluşan borç kartı | İncelendi; ayrıntı E0392 P3-G02 |
| E0317 | [44-debt-records-loan-interests-kaydi.png](kanitlar/wallet-budgetbakers/44-debt-records-loan-interests-kaydi.png) | Ana anlatım — borcun ürettiği kayıt ve toplam | İncelendi; ayrıntı E0392 P3-G02 |
| E0318 | [45-planned-otomatik-onayli-toggle-her-zaman.png](kanitlar/wallet-budgetbakers/45-planned-otomatik-onayli-toggle-her-zaman.png) | Ana anlatım — mod değişikliği; No kayıtlı | İncelendi; ayrıntı E0392 P3-G02 |
| E0319 | [46-planned-duzenleme-formu-cop-ikonu.png](kanitlar/wallet-budgetbakers/46-planned-duzenleme-formu-cop-ikonu.png) | Kanıt eki — plan düzenleme ve silme girişi | İncelendi; ayrıntı E0392 P3-G02 |
| E0320 | [47-planned-silme-basit-onay-gecmis-uyarisi-yok.png](kanitlar/wallet-budgetbakers/47-planned-silme-basit-onay-gecmis-uyarisi-yok.png) | Ana anlatım — geçmiş uyarısız silme onayı | İncelendi; ayrıntı E0392 P3-G02 |

## P3-G03 görsel inceleme kaydı — 15 Eylül 2026

37/37 incelendi; Wallet 84/106, toplam 338/360. Kaynak önceki manuel Android
emülatör koşumudur (Faz 7, 11 Eylül 2026); bu tur cihaz, oturum ve sürüm yeniden
doğrulanmadı. Hedef E0014; içerik, sınır ve soru
bağları E0392 P3-G03 tablosunda. Dosya adları çoğu karede içerikten sapıyor;
roller içeriğe göre verildi. Kaynak PNG/hash/kimlik değişmedi.

| Kimlik | Görsel | Kullanım rolü | Kontrol |
|---|---|---|---|
| E0321 | [f7-00-baslangic.png](kanitlar/wallet-budgetbakers/f7-00-baslangic.png) | Ana anlatım — Faz 7 başlangıç bakiyeleri | İncelendi; ayrıntı E0392 P3-G03 |
| E0322 | [f7-01-debts.png](kanitlar/wallet-budgetbakers/f7-01-debts.png) | Arşiv — Home tekrarı | İncelendi; ayrıntı E0392 P3-G03 |
| E0323 | [f7-02-debts.png](kanitlar/wallet-budgetbakers/f7-02-debts.png) | Kanıt eki — başlangıçtaki tek açık borç | İncelendi; ayrıntı E0392 P3-G03 |
| E0324 | [f7-03-fab-menu.png](kanitlar/wallet-budgetbakers/f7-03-fab-menu.png) | Kanıt eki — hızlı eylem menüsü | İncelendi; ayrıntı E0392 P3-G03 |
| E0325 | [f7-04-transfer-form.png](kanitlar/wallet-budgetbakers/f7-04-transfer-form.png) | Kanıt eki — şablon formu; transfer değil | İncelendi; ayrıntı E0392 P3-G03 |
| E0326 | [f7-05-back-check.png](kanitlar/wallet-budgetbakers/f7-05-back-check.png) | Arşiv — geri dönüş tekrarı | İncelendi; ayrıntı E0392 P3-G03 |
| E0327 | [f7-06-transfer-form2.png](kanitlar/wallet-budgetbakers/f7-06-transfer-form2.png) | Ana anlatım — transfer hızlı formu | İncelendi; ayrıntı E0392 P3-G03 |
| E0328 | [f7-07-tutar-400.png](kanitlar/wallet-budgetbakers/f7-07-tutar-400.png) | Kanıt eki — hesap seçici; Wallet dışı hedef | İncelendi; ayrıntı E0392 P3-G03 |
| E0329 | [f7-08-back-transfer.png](kanitlar/wallet-budgetbakers/f7-08-back-transfer.png) | Arşiv — tutar girişi ara karesi | İncelendi; ayrıntı E0392 P3-G03 |
| E0330 | [f7-09-tutar-400-v2.png](kanitlar/wallet-budgetbakers/f7-09-tutar-400-v2.png) | Arşiv — hatalı otomatik giriş | İncelendi; ayrıntı E0392 P3-G03 |
| E0331 | [f7-10-cleared.png](kanitlar/wallet-budgetbakers/f7-10-cleared.png) | Arşiv — hatalı otomatik giriş; çok büyük tutar | İncelendi; ayrıntı E0392 P3-G03 |
| E0332 | [f7-11-cleared2.png](kanitlar/wallet-budgetbakers/f7-11-cleared2.png) | Arşiv — temizlenmiş form | İncelendi; ayrıntı E0392 P3-G03 |
| E0333 | [f7-12-tutar-final.png](kanitlar/wallet-budgetbakers/f7-12-tutar-final.png) | Ana anlatım — ₺400 kısmi kart ödemesi formu | İncelendi; ayrıntı E0392 P3-G03 |
| E0334 | [f7-13-a-kaydedildi.png](kanitlar/wallet-budgetbakers/f7-13-a-kaydedildi.png) | Ana anlatım — A sonrası bakiyeler | İncelendi; ayrıntı E0392 P3-G03 |
| E0335 | [f7-14-planned.png](kanitlar/wallet-budgetbakers/f7-14-planned.png) | Kanıt eki — Faz 7'de süren B1 planı | İncelendi; ayrıntı E0392 P3-G03 |
| E0336 | [f7-15-add-planned.png](kanitlar/wallet-budgetbakers/f7-15-add-planned.png) | Ana anlatım — D1 tek seferlik plan formu | İncelendi; ayrıntı E0392 P3-G03 |
| E0337 | [f7-16-name-amount.png](kanitlar/wallet-budgetbakers/f7-16-name-amount.png) | Kanıt eki — plan hesap seçicisi | İncelendi; ayrıntı E0392 P3-G03 |
| E0338 | [f7-17-back-form.png](kanitlar/wallet-budgetbakers/f7-17-back-form.png) | Arşiv — yanlış sekmeye kaymış form | İncelendi; ayrıntı E0392 P3-G03 |
| E0339 | [f7-18-form-check.png](kanitlar/wallet-budgetbakers/f7-18-form-check.png) | Kanıt eki — plan tutarı hesap makinesi diyaloğu | İncelendi; ayrıntı E0392 P3-G03 |
| E0340 | [f7-19-amount-10000.png](kanitlar/wallet-budgetbakers/f7-19-amount-10000.png) | Arşiv — hatalı otomatik giriş | İncelendi; ayrıntı E0392 P3-G03 |
| E0341 | [f7-20-amount-check2.png](kanitlar/wallet-budgetbakers/f7-20-amount-check2.png) | Arşiv — hatalı otomatik giriş | İncelendi; ayrıntı E0392 P3-G03 |
| E0342 | [f7-21-cleared.png](kanitlar/wallet-budgetbakers/f7-21-cleared.png) | Arşiv — temizlenmiş diyalog | İncelendi; ayrıntı E0392 P3-G03 |
| E0343 | [f7-22-amount-verify.png](kanitlar/wallet-budgetbakers/f7-22-amount-verify.png) | Arşiv — doğru tutar girişi | İncelendi; ayrıntı E0392 P3-G03 |
| E0344 | [f7-23-form-with-amount.png](kanitlar/wallet-budgetbakers/f7-23-form-with-amount.png) | Kanıt eki — kategori seçilmemiş D1 formu | İncelendi; ayrıntı E0392 P3-G03 |
| E0345 | [f7-24-date-picker.png](kanitlar/wallet-budgetbakers/f7-24-date-picker.png) | Ana anlatım — planlı ödemede geçmiş günler soluk | İncelendi; ayrıntı E0392 P3-G03 |
| E0346 | [f7-25-day5-attempt.png](kanitlar/wallet-budgetbakers/f7-25-day5-attempt.png) | Kanıt eki — geçmiş gün denemesi sonrası seçim değişmedi | İncelendi; ayrıntı E0392 P3-G03 |
| E0347 | [f7-26-d1-saved.png](kanitlar/wallet-budgetbakers/f7-26-d1-saved.png) | Ana anlatım — zorunlu kategori hatası | İncelendi; ayrıntı E0392 P3-G03 |
| E0348 | [f7-27-category.png](kanitlar/wallet-budgetbakers/f7-27-category.png) | Arşiv — hata durumu tekrarı | İncelendi; ayrıntı E0392 P3-G03 |
| E0349 | [f7-28-category-list.png](kanitlar/wallet-budgetbakers/f7-28-category-list.png) | Kanıt eki — kategori seçici | İncelendi; ayrıntı E0392 P3-G03 |
| E0350 | [f7-29-category-selected.png](kanitlar/wallet-budgetbakers/f7-29-category-selected.png) | Arşiv — yanlış alt kategori dalı | İncelendi; ayrıntı E0392 P3-G03 |
| E0351 | [f7-30-housing.png](kanitlar/wallet-budgetbakers/f7-30-housing.png) | Kanıt eki — Housing alt kategorileri; D1 Rent ile kaydedilmedi | İncelendi; ayrıntı E0392 P3-G03 |
| E0352 | [f7-31-rent-selected.png](kanitlar/wallet-budgetbakers/f7-31-rent-selected.png) | Kanıt eki — D1 formu; kategori Property insurance, Rent değil | İncelendi; ayrıntı E0392 P3-G03 |
| E0353 | [f7-32-d1-final.png](kanitlar/wallet-budgetbakers/f7-32-d1-final.png) | Ana anlatım — kaydedilmiş D1 ve B1 plan listesi | İncelendi; ayrıntı E0392 P3-G03 |
| E0354 | [f7-33-tap-planned.png](kanitlar/wallet-budgetbakers/f7-33-tap-planned.png) | Ana anlatım — bekleyen tek seferlik plan | İncelendi; ayrıntı E0392 P3-G03 |
| E0355 | [f7-34-confirm-dialog.png](kanitlar/wallet-budgetbakers/f7-34-confirm-dialog.png) | Kanıt eki — D1 ödeme özeti | İncelendi; ayrıntı E0392 P3-G03 |
| E0356 | [f7-35-d1-confirmed.png](kanitlar/wallet-budgetbakers/f7-35-d1-confirmed.png) | Ana anlatım — gerçekleşmiş tek seferlik plan | İncelendi; ayrıntı E0392 P3-G03 |
| E0357 | [f7-36-nav-check.png](kanitlar/wallet-budgetbakers/f7-36-nav-check.png) | Ana anlatım — D1 sonrası bakiyeler | İncelendi; ayrıntı E0392 P3-G03 |

## P3-G04 görsel inceleme kaydı — 15 Eylül 2026

Kaynak önceki manuel Android emülatör koşumudur (Faz 7, 11 Eylül 2026); bu tur
cihaz, oturum ve sürüm yeniden doğrulanmadı. Hedef E0014; içerik, sınır ve soru
bağları E0392 P3-G04 tablosunda. 22/22 incelendi; Wallet 106/106, toplam 360/360.
Dosya adları çoğu karede içerikten sapıyor;
roller içeriğe göre verildi. Kaynak PNG/hash/kimlik değişmedi.

| Kimlik | Görsel | Kullanım rolü | Kontrol |
|---|---|---|---|
| E0358 | [f7-37-debts-fab.png](kanitlar/wallet-budgetbakers/f7-37-debts-fab.png) | Kanıt eki — I Lent / I Borrowed girişi | İncelendi; ayrıntı E0392 P3-G04 |
| E0359 | [f7-38-i-lent-form.png](kanitlar/wallet-budgetbakers/f7-38-i-lent-form.png) | Arşiv — Debts listesi tekrarı | İncelendi; ayrıntı E0392 P3-G04 |
| E0360 | [f7-39-i-lent-form2.png](kanitlar/wallet-budgetbakers/f7-39-i-lent-form2.png) | Kanıt eki — mevcut kayda bağlama sorusu | İncelendi; ayrıntı E0392 P3-G04 |
| E0361 | [f7-40-i-lent-form3.png](kanitlar/wallet-budgetbakers/f7-40-i-lent-form3.png) | Arşiv — soru ekranı tekrarı | İncelendi; ayrıntı E0392 P3-G04 |
| E0362 | [f7-41-i-lent-form4.png](kanitlar/wallet-budgetbakers/f7-41-i-lent-form4.png) | Arşiv — boş borç formu | İncelendi; ayrıntı E0392 P3-G04 |
| E0363 | [f7-42-form-filled.png](kanitlar/wallet-budgetbakers/f7-42-form-filled.png) | Arşiv — D2 öncesi tek borç | İncelendi; ayrıntı E0392 P3-G04 |
| E0364 | [f7-43-amount-12000.png](kanitlar/wallet-budgetbakers/f7-43-amount-12000.png) | Ana anlatım — D2 faturası I Lent formunda, aynı ad | İncelendi; ayrıntı E0392 P3-G04 |
| E0365 | [f7-44-d2-saved.png](kanitlar/wallet-budgetbakers/f7-44-d2-saved.png) | Ana anlatım — D2'de kayıt oluşturma sorusu | İncelendi; ayrıntı E0392 P3-G04 |
| E0366 | [f7-45-d2-final.png](kanitlar/wallet-budgetbakers/f7-45-d2-final.png) | Ana anlatım — aynı adlı iki ayrı borç kartı | İncelendi; ayrıntı E0392 P3-G04 |
| E0367 | [f7-46-add-record-form.png](kanitlar/wallet-budgetbakers/f7-46-add-record-form.png) | Ana anlatım — borca kayıt bağlama/oluşturma seçimi | İncelendi; ayrıntı E0392 P3-G04 |
| E0368 | [f7-47-new-record-form.png](kanitlar/wallet-budgetbakers/f7-47-new-record-form.png) | Ana anlatım — D3 Repay debt formu; kalan 12.000 | İncelendi; ayrıntı E0392 P3-G04 |
| E0369 | [f7-48-amount-5000.png](kanitlar/wallet-budgetbakers/f7-48-amount-5000.png) | Kanıt eki — kısmi tahsilat tutarı | İncelendi; ayrıntı E0392 P3-G04 |
| E0370 | [f7-49-d3-saved.png](kanitlar/wallet-budgetbakers/f7-49-d3-saved.png) | Ana anlatım — D3 sonrası borç 7.000 | İncelendi; ayrıntı E0392 P3-G04 |
| E0371 | [f7-50-balance-check.png](kanitlar/wallet-budgetbakers/f7-50-balance-check.png) | Ana anlatım — D2/D3 sonrası bakiyeler | İncelendi; ayrıntı E0392 P3-G04 |
| E0372 | [f7-51-records.png](kanitlar/wallet-budgetbakers/f7-51-records.png) | Ana anlatım — D2'siz kayıt listesi ve D3 kaydı | İncelendi; ayrıntı E0392 P3-G04 |
| E0373 | [f7-52-search.png](kanitlar/wallet-budgetbakers/f7-52-search.png) | Ana anlatım — “Ada” araması; D2 sonuçta yok | İncelendi; ayrıntı E0392 P3-G04 |
| E0374 | [f7-53-more-options.png](kanitlar/wallet-budgetbakers/f7-53-more-options.png) | Kanıt eki — Records ⋮ girişi; menü açık değil | İncelendi; ayrıntı E0392 P3-G04 |
| E0375 | [f7-54-hamburger.png](kanitlar/wallet-budgetbakers/f7-54-hamburger.png) | Ana anlatım — gezinme çekmecesi üstü; kişisel ad görünür | İncelendi; ayrıntı E0392 P3-G04 |
| E0376 | [f7-55-menu-scroll.png](kanitlar/wallet-budgetbakers/f7-55-menu-scroll.png) | Ana anlatım — gezinme çekmecesi altı | İncelendi; ayrıntı E0392 P3-G04 |
| E0377 | [f7-56-settings.png](kanitlar/wallet-budgetbakers/f7-56-settings.png) | Ana anlatım — ayar merkezi üstü | İncelendi; ayrıntı E0392 P3-G04 |
| E0378 | [f7-57-settings-scroll.png](kanitlar/wallet-budgetbakers/f7-57-settings-scroll.png) | Ana anlatım — otomatik kural, para birimi, gelişmiş ayar girişleri | İncelendi; ayrıntı E0392 P3-G04 |
| E0379 | [f7-58-advanced.png](kanitlar/wallet-budgetbakers/f7-58-advanced.png) | Ana anlatım — muhasebe dönemi başlangıç günü | İncelendi; ayrıntı E0392 P3-G04 |

## WL-U kullanıcı kanıtları — 15 Eylül 2026

Kullanıcı mevcut Wallet verisinde yeni kayıt eklemeden çekti ve klasöre ekledi; ana agent
inceledi, seriye uygun adla yeniden adlandırdı. Durum çubuğu 08:07–08:20; önceki
koşumlarla aynı cihaz (çerçeve farkı ekran görüntüsü alma yönteminden kaynaklanır).
E kimlikleri korunur; ekler aşağıdaki sabit WL-U kimlikleriyle izlenir. Ayrıntı E0392 WL-U01.

| Kimlik | Görsel | Kullanım rolü / içerik | Boyut | SHA-256 |
|---|---|---|---:|---|
| WL-U01-A | [kanitlar/wallet-budgetbakers/48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png](kanitlar/wallet-budgetbakers/48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png) | Ana anlatım — Cash-flow LAST 30 DAYS: net −16.600, Income 5.000, Expenses −21.600; Record'lu borç kayıtları gelir/gidere dahil; B09. Özgün adı `cash-flow.png` | 152883 | `cb450a49d2f54c8f5750d84e1150802fe8a24f1c2de30a6153e2c6354a28abe2` |
| WL-U03-A | [kanitlar/wallet-budgetbakers/49-u03-records-menu-bakiye-planli-secenekleri.png](kanitlar/wallet-budgetbakers/49-u03-records-menu-bakiye-planli-secenekleri.png) | Kanıt eki — Records ⋮ menüsü: yalnız Show balance in every record / Show planned payments; filtre/dışa aktarma yok; WL-Q10. Özgün adı `record.png` | 196163 | `20d0093b21bf7a5c9bfc482b8a456bdd7c84884b646b5c44ec27a7a7358d6c12` |
| WL-U02-A | [kanitlar/wallet-budgetbakers/50-u02-plan-menu-postpone-dismiss.png](kanitlar/wallet-budgetbakers/50-u02-plan-menu-postpone-dismiss.png) | Ana anlatım — B1 plan detayında bekleyen örneğin ⋮ menüsü: Postpone / Dismiss; sonuçlar denenmedi; WL-Q08. Özgün adı `bulut yazılım aboneliği (2).png`; menüsü kapalı ilk çekim kullanıcı tarafından kaldırıldı | 92168 | `7682e200087fc8ba10aa6c75668c887985cea6194c927b616b06f41a21063ebc` |

WL-U01–WL-U04 tamam: üç görsel bu tabloda, WL-U04 görselsiz kullanıcı beyanı (E0392).


## 15 Eylül ek kullanıcı kanıtı — BC-U01

İki görsel açılıp incelendi; bulgu kaydı BC-U01/HD-U01 ve Bluecoins formuna işlendi. Önceki 363 görsele 2 eklendi; toplam 365. Yeni kanıtlar önceki hash kayıtlarını değiştirmez.

| Kimlik | Yol | Rol | SHA-256 |
|---|---|---|---|
| BC-U01-A | [kanitlar/bluecoins/işlemler.png](kanitlar/bluecoins/işlemler.png) | Liste/rapor farkı; B10 | d308326802898643be6563697c633eb83bc03aeb214889ab6d24dddff0f29d15 |
| BC-U01-B | [kanitlar/bluecoins/ögeler özeti.png](kanitlar/bluecoins/ögeler özeti.png) | Liste/rapor farkı; B10 | 9a9bf1913cdc20d1ec744d496d6c3b62b5572e68b23ce95ccebbee44d6ccc2bf |

## P4-K — Envanter bütünlüğü ve araç hash'i — 15 Eylül 2026

Diskte 365 PNG; her birinin satırı var, 581 yerel bağlantının 0'ı kırık, BC-U01 hash'leri
diskle aynı. Görseller yeniden incelenmedi. Denetim aracı Türkçe/boşluklu adlar için
değişti; E0393/E0394 satırları P1-K anını gösterir. Yeni SHA-256: E0393 `denetim.cjs`
`1a16dfbd1e1b7ceb68e644d50d15890971c208b5d1158b77f0da20ec5469a1dc`, E0394 `denetim-test.cjs`
`09a33396ab376ba8fc4b3786be656ad5587ff93ad79afb1bbc069c40aa94432e`. Ayrıntı E0392 P4-K.


## P5-P teslim çıktıları — 15 Eylül 2026

Bunlar rapor çıktılarıdır; yeni rakip gözlemi değildir. Altı kanıt kopyası mevcut E kimliklerine bağlıdır ve özgün 365 görsel sayısını artırmaz. Yerel .pilot-tools ortamı ve render önizlemeleri teslim envanterine dahil değildir.

| Çıktı kimliği | Dosya | Bayt | SHA-256 |
|---|---|---:|---|
| P5P-001 | [raporlar/pilot/export-word.ps1](raporlar/pilot/export-word.ps1) | 625 | 3df07253e6d643fc548f38fce1e1e9a373ecfeed2e4c912436f63815c1f8699d |
| P5P-002 | [raporlar/pilot/kanit/E0138.png](raporlar/pilot/kanit/E0138.png) | 97008 | edc6ddaa68a417b92084cbbd5b7a3677ed20fdad051bdf11176950a3e9be7854 |
| P5P-003 | [raporlar/pilot/kanit/E0141.png](raporlar/pilot/kanit/E0141.png) | 112904 | 8e35e60101fa41c5e6cc6b42bf3ac63ce5797429f899a3f37982e81a52c3ca09 |
| P5P-004 | [raporlar/pilot/kanit/E0228.png](raporlar/pilot/kanit/E0228.png) | 184232 | f082d8ca8d965d32c38314209e5d4d49403fb52a1f6393a8b570956939842902 |
| P5P-005 | [raporlar/pilot/kanit/E0229.png](raporlar/pilot/kanit/E0229.png) | 119170 | 993fe4c26fdd4d515fdafbefc49740c37e7fdec84702bccb1c35058d25ee21e4 |
| P5P-006 | [raporlar/pilot/kanit/E0277.png](raporlar/pilot/kanit/E0277.png) | 101071 | 394a14c018fcce672de97b9bbd7afdb9e506aa95d51fc1b0f4afedfd1bd60d15 |
| P5P-007 | [raporlar/pilot/kanit/E0279.png](raporlar/pilot/kanit/E0279.png) | 254977 | eb5723aa1da7c29525f8b47948ec0dc6cb8b8744e3e272f39a6ecc6073522fcd |
| P5P-008 | [raporlar/pilot/kanit-manifest.json](raporlar/pilot/kanit-manifest.json) | 1041 | 127fe2273197c5b366c9b0533f39b38b20bf97b8a40f19976075f9249d6c6e0d |
| P5P-009 | [raporlar/pilot/kaynaklar.md](raporlar/pilot/kaynaklar.md) | 1641 | 996bfb318edd106d665b55e73f49b50433e9a8d924e28e7dbbfa2d8c50545db5 |
| P5P-010 | [raporlar/pilot/pilot-islem-ekleme.docx](raporlar/pilot/pilot-islem-ekleme.docx) | 802684 | 65269e4d0dedc6a9bca14bf3d06f7ff8688b4b64edbd4b888c8f602a0675315e |
| P5P-011 | [raporlar/pilot/pilot-islem-ekleme.md](raporlar/pilot/pilot-islem-ekleme.md) | 10467 | d3dc4ef312d23f3a34feaa2aa18868e965b9886b452fbcc5b1ada5fb95f4f704 |
| P5P-012 | [raporlar/pilot/pilot-islem-ekleme.pdf](raporlar/pilot/pilot-islem-ekleme.pdf) | 562942 | 15e13abef10d307d575c98c1eb63ba68551494d252a94638267a5c83c444159b |
| P5P-013 | [raporlar/pilot/README.md](raporlar/pilot/README.md) | 2368 | 0e16c16e3acd21600c83db425149f4b9a81714ee422048d1a732a0371761ccbd |
| P5P-014 | [raporlar/pilot/requirements.txt](raporlar/pilot/requirements.txt) | 35 | 79bbc13680fa34123949cf6f2b6dd913257c6d93e178be1ee49d83b56d07317f |
| P5P-015 | [raporlar/pilot/uret.py](raporlar/pilot/uret.py) | 6425 | e041bb86c18cb2fe020aee3972e855889a7ecc9db9476862a23675f6aa7221f0 |
| P5P-016 | [raporlar/pilot-deneme-paketi.zip](raporlar/pilot-deneme-paketi.zip) | 2081023 | 130c58debbfa49a9df931e35cbdaaf04b9cfa5e94c7b3471f07d2108994f2389 |
| P5P-017 | [raporlar/0-ortak-icindekiler.md](raporlar/0-ortak-icindekiler.md) | 6540 | 519b945f991f5596efc804b10e9f3a1016c7e9ccab8339b6d58335d16bb32e2d |

## 20 Eylül 2026 — Belge 2 koşumu ve geriye dönük E kimlikleri

**E0395–E0402:** daha önce `GB-U01-*`, `WL-U*` ve `BC-U01-*` kodlarıyla kayıtlı olan sekiz kare.
Dosyaları diskteydi ama E kimliği yoktu; `kanit-dizini.json` yalnız `E####` okuduğu için
belgelerde basılamıyorlardı. İçerik değişmedi, yalnız kimlik verildi.

**E0403–E0424:** 20 Eylül 2026 koşumu (`raporlar/belge2-kosum-listesi.md` K1–K3).
Money Manager'da hiçbir kayıt eklenmedi, silinmedi veya düzenlenmedi; yalnız okuma yapıldı.
Goodbudget'ta **tek bir sentetik gelir kaydı eklendi** (Beta Tasarim, ₺1.234, 20.09.2026) ve
silinmedi; kontrol değeri buna göre güncellenir.

**Karartma:** Goodbudget karelerinin başlığı hane adını taşır; teslim kopyasında karartılır
(Wallet `f7-54` ile aynı kural, P3-G04). **Hangi kare:** bu koşumun sekiz karesinden altısı
(E0417, E0418, E0420, E0421, E0422, E0423) üst çubukta hane adını taşır; E0419 ve E0424
`Fill Envelopes` başlığındadır ve taşımaz. 20 Eylül devir kontrolünde altısı üretim motorunun
`KARARTMA_ORTAK` listesine eklendi — uyarı artık yalnız düz metinde değil, basımda uygulanıyor.
Kutu mevcut altı karenin kutusuyla aynıdır (çözünürlük ve yerleşim aynı); özgün kanıt değişmedi.

### Dosya bütünlüğü

| Kimlik | Dosya | Bayt | SHA-256 |
|---|---|---:|---|
| E0395 | [kanitlar/goodbudget/27-fill-from-new-income.png](kanitlar/goodbudget/27-fill-from-new-income.png) | 81513 | `0c3650407147656cda0d57178da5536a9dffbd1e6ad003f8be986680f463bace` |
| E0396 | [kanitlar/goodbudget/28-fill-from-available.png](kanitlar/goodbudget/28-fill-from-available.png) | 78329 | `d25b1e82254a657afd75d04108d1c6fd13104c7fbfc8faa08d34d3291a4e9faa` |
| E0397 | [kanitlar/goodbudget/29-income-keep-available.png](kanitlar/goodbudget/29-income-keep-available.png) | 60378 | `9c0e9bbd1fc9b85cf611b5d4bff1963a29ba65b4e98a060d85193c8f4c745dde` |
| E0398 | [kanitlar/wallet-budgetbakers/48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png](kanitlar/wallet-budgetbakers/48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png) | 152883 | `cb450a49d2f54c8f5750d84e1150802fe8a24f1c2de30a6153e2c6354a28abe2` |
| E0399 | [kanitlar/wallet-budgetbakers/49-u03-records-menu-bakiye-planli-secenekleri.png](kanitlar/wallet-budgetbakers/49-u03-records-menu-bakiye-planli-secenekleri.png) | 196163 | `20d0093b21bf7a5c9bfc482b8a456bdd7c84884b646b5c44ec27a7a7358d6c12` |
| E0400 | [kanitlar/wallet-budgetbakers/50-u02-plan-menu-postpone-dismiss.png](kanitlar/wallet-budgetbakers/50-u02-plan-menu-postpone-dismiss.png) | 92168 | `7682e200087fc8ba10aa6c75668c887985cea6194c927b616b06f41a21063ebc` |
| E0401 | [kanitlar/bluecoins/işlemler.png](kanitlar/bluecoins/işlemler.png) | 248860 | `d308326802898643be6563697c633eb83bc03aeb214889ab6d24dddff0f29d15` |
| E0402 | [kanitlar/bluecoins/ögeler özeti.png](kanitlar/bluecoins/ögeler özeti.png) | 93823 | `9a9bf1913cdc20d1ec744d496d6c3b62b5572e68b23ce95ccebbee44d6ccc2bf` |
| E0403 | [kanitlar/money-manager/33-eylul-islem-listesi.png](kanitlar/money-manager/33-eylul-islem-listesi.png) | 130777 | `281cc95bc34ab7c106820918e40c35d21525867547da0f71ea695df24cdd4978` |
| E0404 | [kanitlar/money-manager/34-istatistik-eylul-gider.png](kanitlar/money-manager/34-istatistik-eylul-gider.png) | 76108 | `ac95f8d98d1c57a79e90c7a984b430d95342f05fbdb5cb51e42e85d29fd1b65e` |
| E0405 | [kanitlar/money-manager/35-toplam-sekmesi-eylul.png](kanitlar/money-manager/35-toplam-sekmesi-eylul.png) | 177842 | `824296b69b613acc56efb25caa7dcc270a976c6ea8f3f6d15b109c9220904ee6` |
| E0406 | [kanitlar/money-manager/36-tekrar-taksit-menusu.png](kanitlar/money-manager/36-tekrar-taksit-menusu.png) | 110146 | `0ce0be7880ea05568c1b1154f2eb8bd227be9ee094dabef5e9bf080e40d535fc` |
| E0407 | [kanitlar/money-manager/37-ekim-islemler-taksit-3-6.png](kanitlar/money-manager/37-ekim-islemler-taksit-3-6.png) | 100922 | `ab0f5b498f2946c1c1ffd0bd0b6a2e2e5afc4b97142edcf244633a814cf1fa5e` |
| E0408 | [kanitlar/money-manager/38-kasim-islemler-taksit-4-6.png](kanitlar/money-manager/38-kasim-islemler-taksit-4-6.png) | 126469 | `6c98960e91059852f3a9d2a62eeb59d433e8e2ad8212f5d4f6d3c601b9531beb` |
| E0409 | [kanitlar/money-manager/39-aralik-islemler-taksit-5-6.png](kanitlar/money-manager/39-aralik-islemler-taksit-5-6.png) | 126974 | `098af51a841cb204acefe6f242899e177a34926a0f77e84e2ccb937a29392372` |
| E0410 | [kanitlar/money-manager/40-ocak-islemler-taksit-6-6.png](kanitlar/money-manager/40-ocak-islemler-taksit-6-6.png) | 125929 | `6b3f45a8c0d4ffc7fe4e83af672172a9003f2b530eeeb99f4f0f8f1dc38dad31` |
| E0411 | [kanitlar/money-manager/41-subat-islemler-veri-yok.png](kanitlar/money-manager/41-subat-islemler-veri-yok.png) | 115048 | `7058e21c2ce1df8feae87c6cda671f5e94097f438f00407776601df93e377a4c` |
| E0412 | [kanitlar/money-manager/42-kart-defteri-ekim-bakiye-2600.png](kanitlar/money-manager/42-kart-defteri-ekim-bakiye-2600.png) | 156634 | `835d6e7f10911d6fbf7444ef70c8be98fd5d83f94d0ba5a1c2176a8ff81e53e1` |
| E0413 | [kanitlar/money-manager/43-kart-defteri-ocak-bakiye-5600.png](kanitlar/money-manager/43-kart-defteri-ocak-bakiye-5600.png) | 155448 | `4cc3dbd47090007c41c6deb1ac6304795d5bee9e814d4a3a279851cdcb6f7bc1` |
| E0414 | [kanitlar/money-manager/44-hesaplar-borclar-1600.png](kanitlar/money-manager/44-hesaplar-borclar-1600.png) | 101688 | `b89ca7963a7f6348d715416fac51574406f73a093941fc0951954e9b018e0866` |
| E0415 | [kanitlar/money-manager/45-daha-sekmesi.png](kanitlar/money-manager/45-daha-sekmesi.png) | 94373 | `cfb41dbbfd09c88b4b9ffb38a9523c26db54e28d2c229da49e365b94ce1d414b` |
| E0416 | [kanitlar/money-manager/46-ayarlar-tekrarlayan-islemler.png](kanitlar/money-manager/46-ayarlar-tekrarlayan-islemler.png) | 74219 | `01b6911424ef2f785a879f75d36400405ceb3a12f1b82e984a6382fa5d6d8a97` |
| E0417 | [kanitlar/goodbudget/30-oncesi-zarflar.png](kanitlar/goodbudget/30-oncesi-zarflar.png) | 115831 | `3fc9be1ccc797badac65afc2205f2b3743339314b6d6ec08b14234a695ba1767` |
| E0418 | [kanitlar/goodbudget/31-oncesi-hesap-bakiyesi.png](kanitlar/goodbudget/31-oncesi-hesap-bakiyesi.png) | 100019 | `67fe614463fa941ea52f77e31cc324015df4b1c99f70ae25e11883cf41671014` |
| E0419 | [kanitlar/goodbudget/32-gelir-formu-dolu.png](kanitlar/goodbudget/32-gelir-formu-dolu.png) | 134155 | `78ebefd94413d2406cb7ded17e9d9c1528969e95060b251858d0ce8f3cc1338c` |
| E0420 | [kanitlar/goodbudget/33-sonrasi-zarflar.png](kanitlar/goodbudget/33-sonrasi-zarflar.png) | 116458 | `c06f0e40c09ab95512b0d2d31f60b6151656f19e404ce4dbb078019f9e441d44` |
| E0421 | [kanitlar/goodbudget/34-sonrasi-hesap-bakiyesi-degismedi.png](kanitlar/goodbudget/34-sonrasi-hesap-bakiyesi-degismedi.png) | 100067 | `ac104a4395916ff1637225817bbaebc127b0b259e9877459572c9ead16a32068` |
| E0422 | [kanitlar/goodbudget/35-sonrasi-islem-satiri.png](kanitlar/goodbudget/35-sonrasi-islem-satiri.png) | 154929 | `dfd3447f7d0faa56047ad61ce51d5f90c454b4943ad12f529b4be6ca01c0c695` |
| E0423 | [kanitlar/goodbudget/36-rapor-income-vs-spending-3284.png](kanitlar/goodbudget/36-rapor-income-vs-spending-3284.png) | 110078 | `eb8facb5e65c8b0fdc2427eae7210d7976089370ee82047175eb0e9014a1f2a9` |
| E0424 | [kanitlar/goodbudget/37-zarf-doldurma-secenekleri.png](kanitlar/goodbudget/37-zarf-doldurma-secenekleri.png) | 163015 | `2862b28336be56a4572a6bed35e667b1251c194209e6660b0a2d404f0d3d7e2c` |

### İçerik açıklamaları

| Kimlik | Dosya | İçerik |
|---|---|---|
| E0395 | 27-fill-from-new-income.png | GB-U01-A. From New Income formu: Received from, How to fill Envelopes, Amount, Account, Date, Schedule ve zarflar; 14 Eyl 14:55. Belge 1 form; Belge 2 ayrı gelir girişi; B05 |
| E0396 | 28-fill-from-available.png | GB-U01-B. From Available: Currently Available 20.000, Used this Fill 0, Left Available 20.000; zarflar No change. Belge 2 mevcut paradan dağıtım yüzeyi |
| E0397 | 29-income-keep-available.png | GB-U01-C. How do you want to fund your Envelopes? diyaloğu; Fill Each Envelope ve Keep Available, ikincisi seçili |
| E0398 | 48-u01-cash-flow-30-gun-borc-kayitlari-dahil.png | WL-U01-A. Cash-flow LAST 30 DAYS: net −16.600, Income 5.000, Expenses −21.600; Record'lu borç kayıtları gelir/gidere dahil; B09 |
| E0399 | 49-u03-records-menu-bakiye-planli-secenekleri.png | WL-U03-A. Records menüsü: satır bakiyesi ve Show planned payments anahtarları |
| E0400 | 50-u02-plan-menu-postpone-dismiss.png | WL-U02-A. Planlı ödeme menüsü: Postpone / Dismiss; sonuçları denenmedi |
| E0401 | işlemler.png | BC-U01-A. 15 Eylül gün toplamı −15 TL; görünen tek satır gider −5 TL, Cüzdan satır bakiyesi −15 TL; Silme denemesi satırı bu görünümde yok; B10 |
| E0402 | ögeler özeti.png | BC-U01-B. Eylül raporunda Silme denemesi −10 TL ve gider −5 TL ayrı görünür; rapor geri yüklenen tutarı içerirken listede satır yoktu; B10 |
| E0403 | 33-eylul-islem-listesi.png | K1. İşlemler/Gün, Eyl 2026: Gelir 0,00 / Gider 1.600,00 / Toplam −1.600,00. 15 Eyl Tasarim ekipmani (2/6) Is Karti 1.000; 10 Eyl Havale Ödeme Bilgisi Ana Hesap → Is Karti 400 (nötr; o günün gider başlığı 600) ve Bulut yazilim aboneligi Ana Hesap (Aylık) 600 |
| E0404 | 34-istatistik-eylul-gider.png | K1. İstatistik/Ay, Eyl 2026, Gider ₺1.600,00; tek kategori Diğer %100 |
| E0405 | 35-toplam-sekmesi-eylul.png | K1 karar karesi. Toplam sekmesi, Eyl 2026: Gider (Nakit, Banka Hesapları) ₺600,00 · Gider (Kredi Kartı, Ödeme) ₺1.000,00(₺400,00) · Havale ₺0,00; Giderleri Karşılaştır %52. Kısmi kart ödemesi gider toplamına girmiyor, parantezde dönem içinde ödenen olarak görünüyor. Ayrıca Bütçe bloğu: Toplam 1.400,00 / Yiyecek 1.400,00, %0 |
| E0406 | 36-tekrar-taksit-menusu.png | K2. İşlem formu sağ üst Tekrar/Taksit açılır menüsü: yalnız iki seçenek — Tekrarlama ve Taksit. Önceki koşumlarda karesi yoktu |
| E0407 | 37-ekim-islemler-taksit-3-6.png | K2. İşlemler/Gün, Eki 2026: Gider 1.000,00; üstte ayrı Tekrarlama bölümü Bulut yazilim aboneligi 10/10 ₺−600,00 (toplama girmiyor); 15 Eki Tasarim ekipmani (3/6) Is Karti 1.000 görünür işlem satırı ve ayın gider toplamında; saklama biçimi B12 kapsamında bilinmiyor |
| E0408 | 38-kasim-islemler-taksit-4-6.png | K2. Kas 2026: Gider 1.000,00; 15 Kas Tasarim ekipmani (4/6) 1.000; Tekrarlama önizlemesi 10/11 −600 |
| E0409 | 39-aralik-islemler-taksit-5-6.png | K2. Ara 2026: Gider 1.000,00; 15 Ara Tasarim ekipmani (5/6) 1.000; Tekrarlama önizlemesi 10/12 −600 |
| E0410 | 40-ocak-islemler-taksit-6-6.png | K2. Oca 2027: Gider 1.000,00; 15 Oca Tasarim ekipmani (6/6) 1.000 — zincirin son taksiti; Tekrarlama önizlemesi 10/1 −600 |
| E0411 | 41-subat-islemler-veri-yok.png | K2. Şub 2027: Gelir/Gider/Toplam 0,00, Veri yok; yalnız Tekrarlama önizlemesi 10/2 −600 kalıyor. Taksit zincirinin bittiği ay |
| E0412 | 42-kart-defteri-ekim-bakiye-2600.png | K2. Is Karti defteri, Eki 2026, Faturalama donemi 1.10.2026 ~ 31.10: Para Yatırma 0,00 / Çekme 1.000,00 / Toplam −1.000,00 / Bakiye 2.600,00. Yürüyen bakiye gelecek dönemlere uzuyor |
| E0413 | 43-kart-defteri-ocak-bakiye-5600.png | K2. Is Karti defteri, Oca 2027, Faturalama donemi 1.01.2027 ~ 31.01: Çekme 1.000,00; satırda (Bakiye −5.600,00). Aynı kart için Hesaplar ekranı 1.600 derken defter 5.600 gösteriyor |
| E0414 | 44-hesaplar-borclar-1600.png | K2. Hesaplar (20 Eyl 2026): Varlıklar 43.950,00 / Borçlar 1.600,00 / Toplam 42.350,00; Ortak Cuzdan 4.150,00; Ana Hesap 39.800,00; Is Karti Bu Ay ₺600,00, Gelecek Ay ₺1.000,00. Ekim–Ocak taksitlerinin toplamı 4.000; Ocak defterinin 5.600 bakiyesi ile bu 1.600 arasındaki farka eşit. İki yüzey farklı dönem kapsamındadır |
| E0415 | 45-daha-sekmesi.png | K2 tarama. Daha sekmesi (sürüm 4.12.8 AD): Ayarlar, Hesaplar, Giriş Kodu, CalcBox, PC'den Yönet, Yedekle, İletişim, Yardım, Tavsiye et, Reklamlar Kaldır. Taksit veya plan listesi girişi görülmedi |
| E0416 | 46-ayarlar-tekrarlayan-islemler.png | K2 tarama. Ayarlar → Kategori/Tekrar → Tekrarlayan İşlemler listesi: yalnız Bulut yazilim aboneligi (Gider ₺600,00, 10.10.2026, Aylık, Ana Hesap/Diğer). Taksit planı bu listede yok. Üstte Tekrar ne zaman uygulanır? → Tarihte ayarı |
| E0417 | 30-oncesi-zarflar.png | K3 önce. Envelopes: Total 43.784,00; Monthly 23.784,00 (Market 23.784,00 / bütçe 850,00; Tasarim Yazilimi 0,00 / bütçe 1.200,00); Available 20.000,00, iki [Available] satırı (0,00 ve 20.000,00). Başlıkta hane adı — teslimde karartılır |
| E0418 | 31-oncesi-hesap-bakiyesi.png | K3 önce. Accounts: All Accounts 41.734,00; Ana Hesap 41.734,00; Subtotal 41.734,00. Zarf toplamıyla arasındaki 2.050 fark önceki koşumda kayıtlı |
| E0419 | 32-gelir-formu-dolu.png | K3. Fill Envelopes → FROM NEW INCOME formu dolu: Received from Beta Tasarim, How to fill Envelopes Fill Each Envelope, Amount 1234.00, Account Ana Hesap, Date 09/20/2026, Tasarim Yazilimi Add specific amount of 1,234.00. Tutar tam dağıtıldığı için sweep satırı yok |
| E0420 | 33-sonrasi-zarflar.png | K3 sonra. Envelopes: Total 45.018,00 (önce 43.784,00); Monthly 25.018,00; Tasarim Yazilimi 1.234,00 (önce 0,00); Market ve Available değişmedi |
| E0421 | 34-sonrasi-hesap-bakiyesi-degismedi.png | K3 sonra. Accounts: All Accounts 41.734,00 — değişmedi. Koşum kaydına göre uygulama kapatılıp yeniden açıldıktan sonra da aynı. Kare son bakiyeyi gösterir; yeniden açma adımını ve değişmemenin nedenini tek başına kanıtlamaz. Zarf toplamı ile hesap arasındaki fark 2.050'den 3.284'e çıktı |
| E0422 | 35-sonrasi-islem-satiri.png | K3 sonra. Transactions: 09/20 Beta Tasarim +1.234,00 Ana Hesap. Kayıt hesaba atfedilmiş görünüyor; hesap bakiyesi buna rağmen değişmedi. Altında 09/11 Initial Envelope Fill +2.050,00 (hesap atfı yok) |
| E0423 | 36-rapor-income-vs-spending-3284.png | K3 sonra. Reports: Spending by Envelope Total Spending 0; Income vs Spending 1 Eyl–30 Eyl 2026 Income 3.284, Spending 0, Net Total 3.284. Yeni gelir raporda sayılıyor (2.050 Initial Fill + 1.234 Beta Tasarim) |
| E0424 | 37-zarf-doldurma-secenekleri.png | K3. Zarf doldurma diyaloğu: Add Budget Amt (1.200,00) · Set to Budget Amt (1.200,00) · Add Specific Amount · Set to Specific Amount · DONE. No change ile birlikte beş seçenek; bunların dördü doldurma kipidir |

## 23 Eylül 2026 — ek eksik koşumu, Belge 2 için seçilen kareler

**E0425–E0462:** 22 Eylül 2026 ek eksik koşumunun 112 karesinden Belge 2 entegrasyonunda
basılacak ya da dayanak olarak anılacak **38 kare**
(`raporlar/eksik-kosum-ortak-listesi.md`, `raporlar/belge2-entegrasyon-plani.md`).
Kalan 74 kare envantere girmedi; Belge 1 turunda aynı yöntemle değerlendirilir.
Her kare bu kayıt yazılırken tek tek açılıp içerik açıklaması gözle yazıldı; ortak
listedeki koşum özetiyle çelişen yerlerde karenin kendisi esas alındı.

**Test verisi:** koşumda yazılan kayıtlar silinmedi (22 Eylül kullanıcı kararı).
Money Manager Eylül gideri 1.600 → 2.180, Wallet ay gideri 21.600 → 21.750, Bluecoins
Eylül gideri 11.015 → 11.181. Bu değerleri taşıyan kareler açıklamada işaretlidir.

**Basılmaz:** E0437 (içe aktarma e-postası — kişisel veri), E0455 (Google Play hesap
baş harfi), E0459 (yarı saydam üst şerit iddiayı örtüyor), E0460 ve E0461 (kare
koşumun iddiasını göstermiyor; yalnız bu sınırı kayda geçirmek için kimlik aldı).

### Dosya bütünlüğü

| Kimlik | Dosya | Bayt | SHA-256 |
|---|---|---:|---|
| E0425 | [kanitlar/wallet-budgetbakers/f7-80-labels-gorunumu-isletme-sahsi-yalniz-150.png](kanitlar/wallet-budgetbakers/f7-80-labels-gorunumu-isletme-sahsi-yalniz-150.png) | 140799 | `cd59b7d1511b51ae88ac72fcb10c5ed51c13da752caa29b351f9d8cb769471db` |
| E0426 | [kanitlar/bluecoins/f7-67-etiket-secici-is-kisisel-hazir.png](kanitlar/bluecoins/f7-67-etiket-secici-is-kisisel-hazir.png) | 110553 | `95d6adbf6ed3cac4586c1b8d5207751f2650748a019452e93b3992d52a08e89a` |
| E0427 | [kanitlar/hesap-defterim/49-uretilen-pdf-icerigi.png](kanitlar/hesap-defterim/49-uretilen-pdf-icerigi.png) | 212099 | `0dd9a031baa97b10dc78c9dd4f90797d70ff2d557e9ec8b0630ab6dcbde5c431` |
| E0428 | [kanitlar/wallet-budgetbakers/f7-105-disa-aktarma-formu-pdf-xls-csv.png](kanitlar/wallet-budgetbakers/f7-105-disa-aktarma-formu-pdf-xls-csv.png) | 75126 | `ab284ab4d4909b96cff0e1d6652c8fe48b1e48ae721db1ce7631832a103ae1d6` |
| E0429 | [kanitlar/money-manager/69-tekrar-ne-zaman-uygulanir-secenekleri.png](kanitlar/money-manager/69-tekrar-ne-zaman-uygulanir-secenekleri.png) | 98963 | `0f14bad56b9eb9a944852b48c0f8ea767e321888d8f377d7ac0f8a70bd41f341` |
| E0430 | [kanitlar/money-manager/66-tekrarli-kaydetme-onay-diyalogu.png](kanitlar/money-manager/66-tekrarli-kaydetme-onay-diyalogu.png) | 111877 | `94d70a33573321d577eac215e36354428fa9c68c86d8368082b00bf745693bbf` |
| E0431 | [kanitlar/money-manager/71-butce-listesi-yalniz-yiyecek-1400.png](kanitlar/money-manager/71-butce-listesi-yalniz-yiyecek-1400.png) | 66918 | `1eca8493c36fa0977cd474b4e8bdd9ba088eff4c1f14cac5232b97f93b0c1d35` |
| E0432 | [kanitlar/money-manager/72-butce-duzenleme-varsayilan-ve-aylik.png](kanitlar/money-manager/72-butce-duzenleme-varsayilan-ve-aylik.png) | 134979 | `b5553a462f996ef16c2131cbbce23798db18006f5a6e4fd076f40edd57f5df15` |
| E0433 | [kanitlar/money-manager/70-toplam-sekmesi-ekim-butce-blogu-yuzde-0.png](kanitlar/money-manager/70-toplam-sekmesi-ekim-butce-blogu-yuzde-0.png) | 168337 | `bd62179842fca0ba22130deb832d3fb9342c1637f6a0c66de3effb304e82e053` |
| E0434 | [kanitlar/money-manager/50-sifir-tutarli-kayit-kabul-edildi-eylul-1600.png](kanitlar/money-manager/50-sifir-tutarli-kayit-kabul-edildi-eylul-1600.png) | 148394 | `9e5956670b5001ed07da64301cb7c2e7f3cca0b059c4ae59ecf0f77654481645` |
| E0435 | [kanitlar/bluecoins/f7-75-transfer-ucreti-kendi-hesabi-ve-kategorisi.png](kanitlar/bluecoins/f7-75-transfer-ucreti-kendi-hesabi-ve-kategorisi.png) | 117949 | `433a0b3f53e1dc9ef684533d43446ac6ad98b8dec78fbca6c8020bb8180dd36e` |
| E0436 | [kanitlar/wallet-budgetbakers/f7-66-aktarim-formu.png](kanitlar/wallet-budgetbakers/f7-66-aktarim-formu.png) | 91174 | `307fb6a3557647411a0db12f4e20b7fa56f530aa6a602fdf4e58a0d4d0f69e52` |
| E0437 | [kanitlar/wallet-budgetbakers/f7-65-hesap-duzenleme-alt-min-max-bakiye.png](kanitlar/wallet-budgetbakers/f7-65-hesap-duzenleme-alt-min-max-bakiye.png) | 117563 | `bf23543a5f5aa0688ac0aab22dde63894c6b84d03ec4a2df426273cdb013b4ee` |
| E0438 | [kanitlar/wallet-budgetbakers/f7-68-aktarim-sonrasi-gider-toplami-21600-degismedi.png](kanitlar/wallet-budgetbakers/f7-68-aktarim-sonrasi-gider-toplami-21600-degismedi.png) | 213154 | `a19d6eefe92f019da68e1d2f42e04648312fb546f7515dddda476534627f9003` |
| E0439 | [kanitlar/wallet-budgetbakers/f7-81-spending-tum-hesaplar-21750.png](kanitlar/wallet-budgetbakers/f7-81-spending-tum-hesaplar-21750.png) | 166540 | `195efa940fadc6506aad0c92bd548db6a301dc446b0e10f51e62a76bcf4ae103` |
| E0440 | [kanitlar/wallet-budgetbakers/f7-76-spending-kategoriler-eylul-21600.png](kanitlar/wallet-budgetbakers/f7-76-spending-kategoriler-eylul-21600.png) | 151256 | `170450c194a707323645ef9dff17223362e73bddd1e9ee552d6f6804da14462b` |
| E0441 | [kanitlar/wallet-budgetbakers/f7-86-mevcut-kayda-baglama-listesi.png](kanitlar/wallet-budgetbakers/f7-86-mevcut-kayda-baglama-listesi.png) | 222675 | `34d7f924fd2cbb926b787d958e635a8b50dc8cf9982d9ff6e06baa644d7086a6` |
| E0442 | [kanitlar/bluecoins/f7-56-butce-ozeti-ayrinti-butce-sifir.png](kanitlar/bluecoins/f7-56-butce-ozeti-ayrinti-butce-sifir.png) | 113351 | `9ab925da04d0b3fa40e7cb47a2fe8d9b0e7a5b98b92a6e25422f7f8400c78938` |
| E0443 | [kanitlar/bluecoins/f7-58-bolmek-satir-yapisi.png](kanitlar/bluecoins/f7-58-bolmek-satir-yapisi.png) | 112680 | `253f353b5b76bb50985290594955a65acb81297daa70e0ffe32343084832eddd` |
| E0444 | [kanitlar/bluecoins/f7-73-cikti-secimi-pdf-csv-html.png](kanitlar/bluecoins/f7-73-cikti-secimi-pdf-csv-html.png) | 206270 | `8a41c854f476f8bc1a0ce05effb77e9b5260562d79cfdcfbda90fc3d8900574f` |
| E0445 | [kanitlar/bluecoins/f7-71-veri-yonetimi-ice-aktarma-csv-qif.png](kanitlar/bluecoins/f7-71-veri-yonetimi-ice-aktarma-csv-qif.png) | 68468 | `869f26d0ba19274f4204c60167af21e36a48e4f3b668477d5278fff142e4dacd` |
| E0446 | [kanitlar/bluecoins/f7-63-nakit-akim-ayari-hesap-secimi.png](kanitlar/bluecoins/f7-63-nakit-akim-ayari-hesap-secimi.png) | 104763 | `67ea858a2012046d0f8a43554c849c95772d1defdb3fa0f9167eaed07700cbfa` |
| E0447 | [kanitlar/hesap-defterim/48-kasadefteri-klasoru-uyarisi.png](kanitlar/hesap-defterim/48-kasadefteri-klasoru-uyarisi.png) | 199141 | `35f25a877611bbc05915e548743edcb29b05567e5092f0e02084c59037f28e68` |
| E0448 | [kanitlar/bluecoins/f7-66-durum-alani-dort-deger.png](kanitlar/bluecoins/f7-66-durum-alani-dort-deger.png) | 92887 | `ab0efb0cb1b18e3c562e58215d6a3fc2ccd121a48ef61acd503527b580a556d7` |
| E0449 | [kanitlar/bluecoins/f7-77-cari-hesap-formu-vade-alani-yok.png](kanitlar/bluecoins/f7-77-cari-hesap-formu-vade-alani-yok.png) | 131977 | `f314204bba8331e5070edd2ad3fb841d22b97dba839485bee20bd480a6d54231` |
| E0450 | [kanitlar/wallet-budgetbakers/f7-104-cekmece-others-imports-exports-locations.png](kanitlar/wallet-budgetbakers/f7-104-cekmece-others-imports-exports-locations.png) | 163529 | `f0b6e2cd504c9e7c360e7cd1acf000d947a80572f7f491dd39f01567cff55815` |
| E0451 | [kanitlar/wallet-budgetbakers/f7-88-debt-action-iki-deger.png](kanitlar/wallet-budgetbakers/f7-88-debt-action-iki-deger.png) | 96140 | `f8dc17fb4af40f8b3bbe6e44199d0b9a94bab5d610b96bf10f3c43222f240bb1` |
| E0452 | [kanitlar/wallet-budgetbakers/f7-103-kayit-ayrintisi-delete-split-save.png](kanitlar/wallet-budgetbakers/f7-103-kayit-ayrintisi-delete-split-save.png) | 111386 | `b9a32b7f237c51a9c3cadd701d15dcaacce5f0b16c6db61020da867260dc0616` |
| E0453 | [kanitlar/goodbudget/42-ayarlar-alt.png](kanitlar/goodbudget/42-ayarlar-alt.png) | 163175 | `6407c1f07b7c72160db9c4dbf7f35fe415922038f41c33678652ded180176c58` |
| E0454 | [kanitlar/bluecoins/f7-78-seyahat-modu-etiket-seciyor.png](kanitlar/bluecoins/f7-78-seyahat-modu-etiket-seciyor.png) | 118831 | `66ffe490bb76ae061bdd8f2b90a136b860008fafae9356d9b59656b96827d329` |
| E0455 | [kanitlar/money-manager/61-calcbox-ayri-uygulama-play-sayfasi.png](kanitlar/money-manager/61-calcbox-ayri-uygulama-play-sayfasi.png) | 538370 | `f18fce516b438865a4c0db097812ab08c60951b2f13806d63fe62e26a84c9d19` |
| E0456 | [kanitlar/money-manager/62-pcden-yonet-ucretli-surum-ekrani.png](kanitlar/money-manager/62-pcden-yonet-ucretli-surum-ekrani.png) | 307082 | `89a994eedfc636ce953665f48acdd4fce19f66111fc346ed9322e0a3ae60109a` |
| E0457 | [kanitlar/money-manager/55-filtre-is-karti-secili-1000-havale-ayri.png](kanitlar/money-manager/55-filtre-is-karti-secili-1000-havale-ayri.png) | 120012 | `2436f937bcba100188f1c183b94b017527c7e298c9c457216ed8a252a36bc581` |
| E0458 | [kanitlar/hesap-defterim/45-haftalik-cipi-onceki-denge-43150.png](kanitlar/hesap-defterim/45-haftalik-cipi-onceki-denge-43150.png) | 116337 | `9977a88d1ebbdf4168894e3af796f02dbec535b14d8fc7e23e21e62b9d952afe` |
| E0459 | [kanitlar/bluecoins/f7-79-net-kazanclar-ve-net-deger-bloklari.png](kanitlar/bluecoins/f7-79-net-kazanclar-ve-net-deger-bloklari.png) | 206798 | `ae6720d8c2cf71b3d5088b7f250dfe3866ab16d6993f14ed7f22a607beb93731` |
| E0460 | [kanitlar/wallet-budgetbakers/f7-67-sifir-tutar-sessiz-red.png](kanitlar/wallet-budgetbakers/f7-67-sifir-tutar-sessiz-red.png) | 93326 | `867f358c095e4e2978baaf2e76323c0edae33e780df4cdc18f8f540c1d759795` |
| E0461 | [kanitlar/wallet-budgetbakers/f7-82-credit-sekmesi-limit-0-kullanim-2147483647.png](kanitlar/wallet-budgetbakers/f7-82-credit-sekmesi-limit-0-kullanim-2147483647.png) | 150469 | `2960188c7742146ffe473c4809c7c3a317f596785c69077534f8456ebb288e06` |
| E0462 | [kanitlar/money-manager/57-havale-formu-harc-alani.png](kanitlar/money-manager/57-havale-formu-harc-alani.png) | 117409 | `f9d5588ab93bcbed6289416d30fa96406a0c0b3f96a8388e62ec27f2c310c96e` |

### İçerik açıklamaları

| Kimlik | Dosya | İçerik |
|---|---|---|
| E0425 | f7-80-labels-gorunumu-isletme-sahsi-yalniz-150.png | Statistics › Spending, This month, Labels sekmesi seçili: toplam ₺150,00; donut Isletme ₺100 ve Sahsi. Aynı ayın Categories toplamı ₺21.750 (E0439). Labels görünümü yalnız etiketli kayıtları topluyor; etiketsiz 21.600 bu görünümde yok. Categories/Labels geçişi aynı karede. Koşumda eklenen iki etiketli kayıt (₺100, ₺50) |
| E0426 | f7-67-etiket-secici-is-kisisel-hazir.png | Gider formunda Etiket seçici: Doğum günü · Film · İş · Kişisel · Tatil, çoklu seçim onay halkaları, arama kutusu. Listede İş ve Kişisel var; ürünle hazır mı geldikleri veya daha önce mi eklendikleri karede görünmüyor |
| E0427 | 49-uretilen-pdf-icerigi.png | Hesap Defterim dışa aktarma PDF'i (Ana Hesap, Oca-01-2026 Bitiş Ara-31-2026): Tarih · Notlar · Açıklama/Kategori · Gelir · Gider · Denge; üstte Önceki denge 0. Açılış bilançosu 20.000 Gelir sütununda; Kime Ortak Cuzdan 3.000 ve Kime Is Karti 1.200 Gider satırı; Toplam Gelir 47.500 · Toplam Gider 4.350 · Denge 43.150. Ekrandaki toplam tanımı dosyaya aynen taşınıyor |
| E0428 | f7-105-disa-aktarma-formu-pdf-xls-csv.png | Wallet Exports formu: Account All · Type Both · Payment Type All · From 22 Ağu 2026 · To 22 Eyl 2026 · Include account transfers (işaretsiz) · PDF / XLS / CSV düğmeleri. Dosya üretilmedi |
| E0429 | 69-tekrar-ne-zaman-uygulanir-secenekleri.png | Ayarlar › Tekrarlayan İşlemler: "Tekrar ne zaman uygulanır?" diyaloğu, iki değer Tarihte (seçili) · Her ayın ilk günü. Arkada liste: Bulut yazilim aboneligi 10.10.2026 ₺600 ve koşumda eklenen 22.10.2026 Günlük Yaşam ₺150, Gider ₺750. Kayıt başına onay seçeneği yok |
| E0430 | 66-tekrarli-kaydetme-onay-diyalogu.png | Gider formu (22.09.2026, ₺150, Günlük Yaşam, Ana Hesap, Aylık rozeti) kaydedilirken diyalog: "Tarihte tekrar eden işlemler uygulanır. Tekrarlı olarak kaydetmek istiyor musunuz?" HAYIR / EVET. Altta üçüncü taraf reklam şeridi — basılırsa kırpılır |
| E0431 | 71-butce-listesi-yalniz-yiyecek-1400.png | Bütçe ekranı: "Bütçe İşlemler > Toplam içerisinde gösterilecektir." Tek satır Yiyecek ₺1.400,00. Bütçe kurulu ve kategori başına |
| E0432 | 72-butce-duzenleme-varsayilan-ve-aylik.png | Yiyecek bütçesi, 2026: "Her ayın bütçe ayarlarını yapabilirsiniz. Varsayılan bütçeyi değiştirirseniz, önümüzdeki aydan itibaren uygulanır." Varsayılan ₺1.400 ve on iki ayın her biri ₺1.400 |
| E0433 | 70-toplam-sekmesi-ekim-butce-blogu-yuzde-0.png | Toplam sekmesi, Eki 2026: Gelir 0 · Gider 1.000 · Toplam −1.000; Bütçe bloğu Toplam ₺1.400 / Yiyecek ₺1.400, harcanan 0, %0. Gider (Nakit, Banka) 0 · Gider (Kredi Kartı) 1.000 · Havale 0. %0'ın nedeni bütçeli kategoride harcama olmaması |
| E0434 | 50-sifir-tutarli-kayit-kabul-edildi-eylul-1600.png | İşlemler/Gün, Eyl 2026: Gelir 0 · Gider 1.600 · Toplam −1.600. 22 Eyl satırı: Günlük Yaşam · Ana Hesap · ₺0,00 — tutarı boş kaydedilen kayıt listeye 0 olarak girdi, uyarı yok. Eylül toplamı kontrol değeriyle aynı (1.600) |
| E0435 | f7-75-transfer-ucreti-kendi-hesabi-ve-kategorisi.png | Bluecoins TRANSFER formu: 500 · Nakit/Cüzdan → Banka/Ana Hesap; açılmış Transfer ücreti bloğu kendi tutar alanı, kendi hesabı (Cüzdan) ve kendi kategorisi (Diğer/Others) taşıyor; Durum ve Etiket formda. Ücret aktarımın parçası değil, ayrı bir gider satırı olarak kuruluyor |
| E0436 | f7-66-aktarim-formu.png | Wallet işlem formu, TRANSFER sekmesi (INCOME / EXPENSE / TRANSFER aynı formun üç sekmesi): From account ANA HESAP → To account IS KARTI, Target amount ~ ₺0, hesap makineli tuş takımı (÷ × − + =) |
| E0437 | f7-65-hesap-duzenleme-alt-min-max-bakiye.png | Edit account alt yarısı: TRY · Color · Import email (kişisel veri) + Enable automatic imports · Exclude from stats · Archive · Minimum balance "Get notified when balance drops below this amount" açık, Minimum amount 0.00 · Maximum balance kapalı. Açılış bakiyesi alanı yok. Minimum balance, kart eksiye inince çıkan eşik uyarısının kaynağı. **Basılmaz** |
| E0438 | f7-68-aktarim-sonrasi-gider-toplami-21600-degismedi.png | Home: Ana Hesap ₺9.500 · Is Karti −₺5.300 · Ortak Cuzdan ₺2.150; Expenses structure LAST 30 DAYS ₺21.600. ₺300 Ana Hesap → Is Karti aktarımından sonra; kart borcu 5.600 → 5.300, gider toplamı değişmedi |
| E0439 | f7-81-spending-tum-hesaplar-21750.png | Spending, This month, Categories: ₺21.750 (etiketli iki koşum kaydı dâhil); legend Housing (₺10.000) · Shopping · Financial expenses · Communication, PC · Food & Drinks · Income. Hesap kırılımı göstermiyor |
| E0440 | f7-76-spending-kategoriler-eylul-21600.png | Spending, This month, Categories: ₺21.600, Accounts All, Filter None. Etiketli kayıtlar eklenmeden önce. 21.600 = 10.000 (Property insurance) + 6.000 (Electronics, Is Karti) + 5.000 (borç verme) + 600 (Software) — kalemler E0441 ve E0398'den; kart harcaması dönem giderinin içinde |
| E0441 | f7-86-mevcut-kayda-baglama-listesi.png | Borca kayıt eklerken Select Record: mevcut kayıtlar onay kutusuyla — Lending, renting −50 (Sahsi) · Groceries −100 (Isletme) · Property insurance −10.000 · Electronics, accessories Is Karti −6.000 "Tasarim ekipmani" · Software −600 · Software Is Karti −1.200 · Groceries Ortak Cuzdan −850 · Sale +25.000. Etiketler satırda çip |
| E0442 | f7-56-butce-ozeti-ayrinti-butce-sifir.png | Bütçe Özeti: Others, Güncel ₺11.015 / Bütçe ₺0; pasta %100 Others; İşlem tipi Gider · Tarih Aralığı Bu Ay · Grafik Türü Kategoriye Göre; yazıcı simgesi. Hiçbir kategoriye bütçe kurulmamış. Koşum kayıtlarından önce |
| E0443 | f7-58-bolmek-satir-yapisi.png | Gider formunda Bölmek açık: Hepsini temizle · Ekle · Toplam tutar 0,00; satır başına tutar, kategori (Diğer/Others), hesap (Cüzdan), Not, **Durum** ve **Etiket** — her parça kendi etiketini taşıyabiliyor |
| E0444 | f7-73-cikti-secimi-pdf-csv-html.png | İşlemler ekranında yazıcı simgesi → "İşlemi seçin": PDF veya Yazıcıya gönder · Excel (.csv) · HTML. Arkada 22 Eyl: 56 (İş çipi), +1, 110 "2 Kategoriler" satırları; 15 Eyl Silme denemesi −10. Dosya üretilmedi |
| E0445 | f7-71-veri-yonetimi-ice-aktarma-csv-qif.png | Ayarlar › Veri Yönetimi: Yedekleme ve geri yükleme — Telefon hafızası; Verileri İçe Aktar — Excel (.csv) · QIF; Verileri Sıfırla. Banka ekstresi ayrıştırıcısı yok |
| E0446 | f7-63-nakit-akim-ayari-hesap-secimi.png | Nakit Akım Ayarı: "Nakit akışı hesaplarken kullanılacak nakit hesapları seçiniz." Banka: Ana Hesap kapalı · Birikimler açık · Çek açık; Nakit: Cüzdan açık · Ortak Cuzdan kapalı. Anahtarı değiştirip raporun değiştiği ölçülmedi |
| E0447 | 48-kasadefteri-klasoru-uyarisi.png | Dışa Aktarılan Veriler diyaloğu: "Veriler, SD kartta veya Dahili Depolamada kasadefteri adlı bir klasöre kaydedilir." Yıllık çip, Toplam Gelir 47.500 · Gider 4.350 · Denge 43.150. Koşumun dosya sistemi araması (karesiz): /sdcard altında kasadefteri adlı klasör yok; PDF Documents/Hesap Defterim altına, Excel uygulamanın kendi Android/data dizinine yazılmış |
| E0448 | f7-66-durum-alani-dort-deger.png | Bluecoins gider formu, Durum alt sayfası: Yok · Kontrol · Mutabık · İptal edildi. Formda Bölmek, Durum, Etiket; fatura bağlama alanı yok, ek yalnız üstteki ataç |
| E0449 | f7-77-cari-hesap-formu-vade-alani-yok.png | Hesabı Düzenle — Ada Reklam cari: Not · Başlangıç bakiyesi 0.00 · Son Bakiye ₺7.000 · Açılış tarihi 10 Eylül 2026 · Hesap Tipi Cari hesap · Hesap seçiminden gizle · Hesap hareketlerini dahil etme · İşlem Listesi. Vade alanı yok |
| E0450 | f7-104-cekmece-others-imports-exports-locations.png | Wallet çekmecesi (başlık kaydırılmış): Budgets · Debts · Goals · ShareCost · Shopping lists · Warranties · Loyalty cards · Currency rates · Group sharing · Others açık → Imports · Exports · Locations; Dark mode · Hide Amounts. Dışa aktarma katlanmış Others altında |
| E0451 | f7-88-debt-action-iki-deger.png | Create Debt Record: Debt action açılır listesi tam iki değer — Repay debt · Increase debt; Amount yer tutucu "₺7.000,00 to Repay debt" |
| E0452 | f7-103-kayit-ayrintisi-delete-split-save.png | Wallet Record detail: araç çubuğunda sil · böl (split) · kaydet; Expense, Lending, renting, Ana Hesap, 50,00, 22 Eyl 2026, Labels Sahsi. Silme bu yüzeyden; geri alma yüzeyi yok |
| E0453 | 42-ayarlar-alt.png | Goodbudget Settings alt yarısı: Localization (Date Order, Decimal Digits, Decimal Mark) · Advanced (Clear Default Payees, Calculator "Use calculator to enter amounts" açık, Refill Notifications) · About (Terms, Privacy, Close Household). Çöp kutusu veya geri alma kalemi yok |
| E0454 | f7-78-seyahat-modu-etiket-seciyor.png | Çekmecede seyahat modu anahtarına dokununca Etiketler seçicisi açılıyor (Doğum günü · Film · İş · Kişisel · Tatil). Koşumda seçim yapılmadan iptal edildi; seçilen etiketin ne yaptığı görülmedi |
| E0455 | 61-calcbox-ayri-uygulama-play-sayfasi.png | Daha › CalcBox, Google Play'de ayrı bir ürün sayfası açıyor: "CalcBox - Hesap Makinesi", Realbyte Inc., "Cihazınız bu sürümle uyumlu değil". Uygulama içi araç değil. Sağ üstte Play hesap baş harfi — **basılmaz** |
| E0456 | 62-pcden-yonet-ucretli-surum-ekrani.png | Daha › PC'den Yönet, "Ücretli versiyona yükselt" ekranı: Reklam yok · Hesap ekleme sınırı yok · PC Yönetimi ("PC Manager'ı kullanmak için bir yönlendirici gereklidir"). Ücretsiz sürümde açılmıyor |
| E0457 | 55-filtre-is-karti-secili-1000-havale-ayri.png | İstatistik filtre paneli, Eyl 2026, HESAP sekmesinde yalnız Is Karti seçili: Gelir %0 · Gider %45 ₺1.000 · Toplam −1.000 · "Havale : ₺400,00" ayrı satır. Sütunlar Gelir/Giden Havale · Gider/Gelen Havale; Ana Hesap satırı 1.180 (koşumda düzenlenen 580 dâhil — test verisi) |
| E0458 | 45-haftalik-cipi-onceki-denge-43150.png | Haftalık çip, Eyl-21-2026 → Eyl-27-2026: hareket yok; Toplam Gelir 0 · Gider 0 · Denge 0 · Önceki denge 43.150 · Denge 43.150. Dönem yalnız kendi aralığının hareketlerini topluyor, önceki dengeyi taşıyor |
| E0459 | f7-79-net-kazanclar-ve-net-deger-bloklari.png | Bluecoins Hesaplar panosu: Net Kazanç bloğu Varlıklar · Cari hesap · Net Kazanç (Ağu 22.350 / −1.000 / 21.350; Eyl 38.670 / 5.500 / 44.170); üstte yarı saydam Net Kazançlar bloğu (Gelir · Gider · Net Kazançlar). Dosya adındaki "net-deger" yanlış; bloğun adı Net Kazanç. Koşum kayıtları dâhil. **Basılmaz** (üst şerit) |
| E0460 | f7-67-sifir-tutar-sessiz-red.png | Wallet formu, EXPENSE, 0 TRY, Ana Hesap / Groceries; ekranda uyarı yok. Koşum "sessizce reddediliyor" dedi; ancak E0281'de aynı durumda "Please fill in the amount." baloncuğu görünüyor ve baloncuk kısa süre kalıyor. Bu kare mesajın yokluğunu **kanıtlamaz** |
| E0461 | f7-82-credit-sekmesi-limit-0-kullanim-2147483647.png | Statistics › Credit: Credit Limits Utilization TODAY %0, Credit Balance ₺5.300, Total Credit Limit ₺0. Dosya adındaki %2147483647 bu karede **görünmüyor** (alt panel örtüyor); o iddia bu kareyle kanıtlanmaz |
| E0462 | 57-havale-formu-harc-alani.png | Havale formu (22.09.2026, ₺300, Kaynak Ana Hesap → Giriş Ortak Cuzdan): Tutar alanının sağında **Harç** düğmesi, Kaynak/Giriş arasında yer değiştirme oku, Tekrar/Taksit, Not, Detay + kamera. Aktarım ücreti formda ayrı bir giriş. Koşum test kaydı |

## 24 Eylül 2026 — Belge 1 düzeltme turu için seçilen kareler

**E0463–E0496:** 22 Eylül 2026 ek eksik koşumunun envantere girmemiş 74 karesinden Belge 1'in
düzeltme turunda basılacak ya da dayanak olarak anılacak **34 kare**
(`raporlar/belge1-duzeltme-plani.md` §6, karar K7: aynı iddia için tek kare). Her kare bu kayıt
yazılırken tek tek açılıp içerik açıklaması gözle yazıldı; ortak listedeki koşum özetiyle çelişen
yerlerde karenin kendisi esas alındı (E0466 çizgi grafik, E0478 Manage debt yok, E0482 ayar yalnız
ondalık, E0493 "cvs" yazımı). Kalan 40 kare envantere girmedi.

**Test verisi:** koşumda yazılan kayıtlar silinmedi (22 Eylül kullanıcı kararı); bazı kareler
Belge 1'in 17 Eylül karelerinden farklı toplam taşır. Belge 1'in kuralı geçerli: kareler aynı anın
görüntüsü değildir, sayılar birbiriyle karşılaştırılmaz.

**Basılırsa karartılır veya kırpılır:** E0494, E0495 (Goodbudget hane adı) · E0463, E0465, E0466
(Money Manager reklam şeridi) · E0486 (Bluecoins test reklamı).

### Dosya bütünlüğü
| Kimlik | Dosya | Bayt | SHA-256 |
|---|---|---:|---|
| E0463 | [kanitlar/money-manager/47-form-tutar-tus-takimi.png](kanitlar/money-manager/47-form-tutar-tus-takimi.png) | 124509 | `ab68ed14f0ed3c3c3860e2f64a68f66ef55d71bbcc82e78ad31b4c3b98649eeb` |
| E0464 | [kanitlar/money-manager/48-tam-ekran-hesap-makinesi.png](kanitlar/money-manager/48-tam-ekran-hesap-makinesi.png) | 49396 | `f721715a9793515f55c8554b58697d8f593a4d4887a344d67272c2da26a4910b` |
| E0465 | [kanitlar/money-manager/49-tutar-bos-form-kaydet-oncesi.png](kanitlar/money-manager/49-tutar-bos-form-kaydet-oncesi.png) | 139273 | `b40bdc65ee8bee3321da99f408625bf7660693112e9764390f616c627ea60022` |
| E0466 | [kanitlar/money-manager/53-pastadan-kategori-ayrintisi-diger.png](kanitlar/money-manager/53-pastadan-kategori-ayrintisi-diger.png) | 144014 | `2553db982276cd91416d70d24d3b1aeac1e99e52129e44be4a7ffe9bd0414ae4` |
| E0467 | [kanitlar/money-manager/56-filtre-uygulanmis-liste-1000.png](kanitlar/money-manager/56-filtre-uygulanmis-liste-1000.png) | 103297 | `605f2010bd0c4fd7e73beffccdc7186c8f03887bf6dd8bcc936ab6fd8ebb78f4` |
| E0468 | [kanitlar/money-manager/59-silme-onay-diyalogu.png](kanitlar/money-manager/59-silme-onay-diyalogu.png) | 82437 | `6b46f2621a397d4653585a87b2f1ad84a17e1401db2ddeff67f0adb96a679c03` |
| E0469 | [kanitlar/wallet-budgetbakers/f7-59-home-balance-trend-ve-bekleyen-odeme-yuklenmis.png](kanitlar/wallet-budgetbakers/f7-59-home-balance-trend-ve-bekleyen-odeme-yuklenmis.png) | 131611 | `e70953040ea5bbcb980baebbd15df167ad2cce6753a9c2b04d1e21a5aa947d15` |
| E0470 | [kanitlar/wallet-budgetbakers/f7-60-hedef-ayrintisi-0-20000.png](kanitlar/wallet-budgetbakers/f7-60-hedef-ayrintisi-0-20000.png) | 106814 | `db0a19880c85ff7cad87cbf19d356d54bc4d7dd24d1094b4d0c49edcbab4d454` |
| E0471 | [kanitlar/wallet-budgetbakers/f7-62-hesap-turu-secimi-bank-sync-import-manual.png](kanitlar/wallet-budgetbakers/f7-62-hesap-turu-secimi-bank-sync-import-manual.png) | 181539 | `171dede327de4b6058ebb472bcfecaf51b51890ac523bfae2dc4953c37c8367e` |
| E0472 | [kanitlar/wallet-budgetbakers/f7-63-dorduncu-hesap-premium-duvarina-carpiyor.png](kanitlar/wallet-budgetbakers/f7-63-dorduncu-hesap-premium-duvarina-carpiyor.png) | 155422 | `b1e88f7320fce450bb5972e93866641038f0ef10fdc2ff3a0114536a292e68c9` |
| E0473 | [kanitlar/wallet-budgetbakers/f7-69-donem-secici-goreli-araliklar-6m-1y-kilitli.png](kanitlar/wallet-budgetbakers/f7-69-donem-secici-goreli-araliklar-6m-1y-kilitli.png) | 148708 | `f1aca61ce1ac351affd9369724d09df197869b7096c1bbe723a9dcfd6a2c24c2` |
| E0474 | [kanitlar/wallet-budgetbakers/f7-70-donem-secici-ozel-tarih-araligi.png](kanitlar/wallet-budgetbakers/f7-70-donem-secici-ozel-tarih-araligi.png) | 135076 | `f4a2ff5e285e2fd5abed58025dcc35ceda258c855210e2000255101cbc5bb467` |
| E0475 | [kanitlar/wallet-budgetbakers/f7-71-donem-listesi-today-week-month-year.png](kanitlar/wallet-budgetbakers/f7-71-donem-listesi-today-week-month-year.png) | 149443 | `45db657e4ebfe65ba8bdaa7a5a37b2c0bbae54cf1ff03701cd4211dfb818b2f4` |
| E0476 | [kanitlar/wallet-budgetbakers/f7-74-filtre-formu-ust.png](kanitlar/wallet-budgetbakers/f7-74-filtre-formu-ust.png) | 78868 | `c079e546c60a877bf2ac738cb1d7ea3fa6e1a1cf923c6fe7e0dd23f967230cf0` |
| E0477 | [kanitlar/wallet-budgetbakers/f7-75-filtre-formu-transfers-debts-include.png](kanitlar/wallet-budgetbakers/f7-75-filtre-formu-transfers-debts-include.png) | 79232 | `f943c357013108698e70e503f7bc54b4c3dbb00b9df313666999e147e12ef9d6` |
| E0478 | [kanitlar/wallet-budgetbakers/f7-84-borc-kayit-listesi-5000.png](kanitlar/wallet-budgetbakers/f7-84-borc-kayit-listesi-5000.png) | 77148 | `0f71a06b61fe11193597b8194fd4acc3f50ffef3b809763892fa9eb32f61ba58` |
| E0479 | [kanitlar/wallet-budgetbakers/f7-89-warranties-bos-durum.png](kanitlar/wallet-budgetbakers/f7-89-warranties-bos-durum.png) | 77734 | `8b20ef329ceff795be116c81fca005ca1963dd7f1018d3232670f76d5d707e5c` |
| E0480 | [kanitlar/wallet-budgetbakers/f7-91-shopping-lists-elbise.png](kanitlar/wallet-budgetbakers/f7-91-shopping-lists-elbise.png) | 53153 | `9573eb32422ec691c197cb0b5b22e8c24eec66d0e452b160609744821a8fc610` |
| E0481 | [kanitlar/wallet-budgetbakers/f7-93-loyalty-cards-bos-durum.png](kanitlar/wallet-budgetbakers/f7-93-loyalty-cards-bos-durum.png) | 77493 | `a5e2ed94932abf29144d8ab3eb29ad4224811a80370fbcbcb3d7647ce8aebf8b` |
| E0482 | [kanitlar/wallet-budgetbakers/f7-96-gelismis-ayarlar-sayi-bicimi.png](kanitlar/wallet-budgetbakers/f7-96-gelismis-ayarlar-sayi-bicimi.png) | 82837 | `a55ebf40a1e95df79e7911fe2292a9913a7b0fab6fe6299a6342f37144d714ae` |
| E0483 | [kanitlar/wallet-budgetbakers/f7-99-payment-due-date-ayin-gunu-secici.png](kanitlar/wallet-budgetbakers/f7-99-payment-due-date-ayin-gunu-secici.png) | 97432 | `1c92b7801315d81e1dd296eed04cfd9721fa4e3bdfc3e10c0c9287d36b0e1e86` |
| E0484 | [kanitlar/wallet-budgetbakers/f7-102-kayit-silme-onayi.png](kanitlar/wallet-budgetbakers/f7-102-kayit-silme-onayi.png) | 119030 | `6f4934e10021ed51a52ec1e614ca5507127f06c7b48ae1fa3c2fb7e897b5c9c3` |
| E0485 | [kanitlar/wallet-budgetbakers/f7-106-sayi-bicimi-kapali-ondaliksiz-gosterim.png](kanitlar/wallet-budgetbakers/f7-106-sayi-bicimi-kapali-ondaliksiz-gosterim.png) | 208632 | `57c3f6071d13d83f1ab09eba7d2bd23acd7aaf56b34439451891c6a7808154d8` |
| E0486 | [kanitlar/bluecoins/f7-55-soguk-acilis-hesaplar-sekmesi-secili.png](kanitlar/bluecoins/f7-55-soguk-acilis-hesaplar-sekmesi-secili.png) | 296115 | `a01bc5147ec83869bf66d42be0bd9dd312d4cfc6b8b4ab864bcf911823398c30` |
| E0487 | [kanitlar/bluecoins/f7-57-islem-formu-varsayilan-tur-gider.png](kanitlar/bluecoins/f7-57-islem-formu-varsayilan-tur-gider.png) | 120682 | `dacafdaef32c08d0c5b655fd4ab583696fefaa532c1c6d26b9c51763f405b56a` |
| E0488 | [kanitlar/bluecoins/f7-61-cekmece-iki-hesaplar-kalemi.png](kanitlar/bluecoins/f7-61-cekmece-iki-hesaplar-kalemi.png) | 162588 | `d2254247f7e2ef2aaa965cc2bac58a01fffd5a17178a9325c8d4819746ce0a8f` |
| E0489 | [kanitlar/bluecoins/f7-62-ikinci-hesaplar-hesap-kurulum-ekrani.png](kanitlar/bluecoins/f7-62-ikinci-hesaplar-hesap-kurulum-ekrani.png) | 155544 | `0d466b8174a01074af41ca5ef261247a32675bb90d0f90b72632b0b0d9d97fd8` |
| E0490 | [kanitlar/bluecoins/f7-65-bolunmus-kayit-listede-2-kategoriler.png](kanitlar/bluecoins/f7-65-bolunmus-kayit-listede-2-kategoriler.png) | 320657 | `021e6ddc6a14a7a2a9bde13ed4a46d15f409b90a676a3f28f69d92869dedfd2d` |
| E0491 | [kanitlar/bluecoins/f7-68-etiketli-kayit-listede-is-cipi.png](kanitlar/bluecoins/f7-68-etiketli-kayit-listede-is-cipi.png) | 299576 | `669b822ba1eb6cd0a71b78279bf6febbd7a5083ad68d1d0304419b43d4a1969b` |
| E0492 | [kanitlar/bluecoins/f7-70-filtre-profili-kaydet-ac-sifirla.png](kanitlar/bluecoins/f7-70-filtre-profili-kaydet-ac-sifirla.png) | 127185 | `070251815828b7d3d0c77e2f3c1078373b9e2e865b27e934f2ad49ea2a26d165` |
| E0493 | [kanitlar/bluecoins/f7-72-csv-ice-aktarma-ve-cikti-aciklamasi.png](kanitlar/bluecoins/f7-72-csv-ice-aktarma-ve-cikti-aciklamasi.png) | 119672 | `40db47bc52ad278a961dd71175c13dfdad014dde42ea09b541cab8e64dc5b2eb` |
| E0494 | [kanitlar/goodbudget/39-islem-listesi-gelir-yesil-arti.png](kanitlar/goodbudget/39-islem-listesi-gelir-yesil-arti.png) | 155270 | `5418664f5f545111796a4dc88ed00b6ddefe0c53152f5ee615a08d692dd5dec0` |
| E0495 | [kanitlar/goodbudget/41-ayarlar-ust-cop-kutusu-yok.png](kanitlar/goodbudget/41-ayarlar-ust-cop-kutusu-yok.png) | 160721 | `dc8c6ff6b8347d92ac0009b7b99792eb26397546171ee94bb25f7587ce83e3a2` |
| E0496 | [kanitlar/hesap-defterim/44-soguk-acilis-ana-hesap-defteri.png](kanitlar/hesap-defterim/44-soguk-acilis-ana-hesap-defteri.png) | 185203 | `058762b0d604b05c93069d72b1cebc00f89eb64ea8cb1ae748c09337a77aa868` |

### İçerik açıklamaları

| Kimlik | Dosya | İçerik |
|---|---|---|
| E0463 | 47-form-tutar-tus-takimi.png | Gider formu (22.09.2026) açılır açılmaz: Gelir / Gider / Havale segmentinde **Gider** seçili (çerçeveli), Tarih, Tutar (odakta), Kategori, Hesap, Not boş; Detay + kamera; Kaydet ve Devam et. Altta "Tutar" başlıklı rakam tuş takımı: sağ sütunda ⌫, −, **hesap makinesi simgesi** ve kırmızı **Bitti**. En altta üçüncü taraf reklam şeridi — basılırsa kırpılır |
| E0464 | 48-tam-ekran-hesap-makinesi.png | Tuş takımındaki simgeden açılan tam ekran hesap makinesi: gösterge 0, AC · ÷ · × · ⌫ / 7–9 · − / 4–6 · + / 1–3 · = / 00 · 0 · , · BİTTİ; sağ üstte kapat (×) |
| E0465 | 49-tutar-bos-form-kaydet-oncesi.png | Gider formu, **Tutar boş**; Kategori Günlük Yaşam, Hesap Ana Hesap dolu; imleç Detay'da. Kaydet öncesi hâl (sonucu E0434). Altta üçüncü taraf reklam — basılırsa kırpılır. Koşum kaydındaki "Kaydet önce hesap alanına atladı" bu karede görünmüyor |
| E0466 | 53-pastadan-kategori-ayrintisi-diger.png | İstatistik pastasından açılan **Diğer** ayrıntısı, Eyl 2026: Toplam ₺1.600,00; Şub–Eyl **çizgi** grafik (ortak listedeki "çubuk" değil); altında kategorinin kayıtları (15 Eyl Tasarim ekipmani (2/6) Is Karti 1.000 · 10 Eyl Bulut yazilim aboneligi Ana Hesap (Aylık) 600). Altta üçüncü taraf reklam — basılırsa kırpılır |
| E0467 | 56-filtre-uygulanmis-liste-1000.png | Filtre uygulanmış İşlemler/Gün, Eyl 2026: Gelir 0 · Gider 1.000 · Toplam −1.000; satırlar Tasarim ekipmani (2/6) Is Karti 1.000 ve Havale Ödeme Bilgisi Ana Hesap → Is Karti 400. Altta koyu bant: seçili filtre "Is Karti" ve **Düzenle** düğmesi |
| E0468 | 59-silme-onay-diyalogu.png | Gider formu (₺580, Günlük Yaşam, Ana Hesap) üstünde silme onayı: "Silmek istediğinize emin misiniz?" **HAYIR / EVET** |
| E0469 | f7-59-home-balance-trend-ve-bekleyen-odeme-yuklenmis.png | Home'un aşağısı: **Balance Trend** kartı TODAY ₺6.350,00, vs past period −%72, 23 Ağu–Today çizgisi; **Upcoming planned payments** kartı yüklenmiş: Bulut yazilim aboneligi · Software, apps, games · −₺600,00 · 10 Eki; Show more; Add more cards; FAB. Üstte durum çubuğunun altında önceki kartın kesik satırı |
| E0470 | f7-60-hedef-ayrintisi-0-20000.png | Goal detail: Yeni ekipman fonu, No target date; halka %0, **0 / 20.000 ₺**; Last added Week amount ₺0; Estimated time to reach goal: No data yet; **Add saved amount** ve **Set goal as reached** |
| E0471 | f7-62-hesap-turu-secimi-bank-sync-import-manual.png | Choose an account type — dört seçenek: **Bank Sync** ("Connect to your bank account…") · **Investments** · **File Import** ("Import CSV, Excel, OFX, … Update your account by importing your transactions as data files to Wallet via email, from any source, including your bank.") · **Manual Input**. Kişisel veri yok |
| E0472 | f7-63-dorduncu-hesap-premium-duvarina-carpiyor.png | Hesap türü seçiminin üstünde premium diyaloğu: "To unlock unlimited accounts, upgrade to Premium"; SHOW PREMIUM PLANS · TRY FOR FREE. Ücretsiz sürümde yeni hesap açılamıyor |
| E0473 | f7-69-donem-secici-goreli-araliklar-6m-1y-kilitli.png | Statistics › Cash-flow, dönem seçicinin **ilk sayfası**: 7D · 30D · **12W** (seçili) · 6M 🔒 · 1Y 🔒; altında üç sayfa noktası, Accounts All, Filter None, Instant filter. Üstte Cash Flow kartı LAST 12 WEEKS ₺6.350,00 |
| E0474 | f7-70-donem-secici-ozel-tarih-araligi.png | Aynı seçicinin **üçüncü sayfası**: özel aralık 15.09.2026 — Today; kart başlığı "15 EYL — TODAY", ₺0 |
| E0475 | f7-71-donem-listesi-today-week-month-year.png | Aynı seçicinin **ikinci sayfası**: oklu "This week" ve açık liste Today · This week · This month · **This year 🔒** (kilitli); kart başlığı THIS WEEK |
| E0476 | f7-74-filtre-formu-ust.png | Settings › Filters › **Add filter** formunun üstü: Name · Type (Both) · Record confirmation (All) · Categories (All) · Labels (Add label) · Currencies (TRY) · Payment Type (All) · Status (All) · Transfers (kesik). Kişisel veri yok |
| E0477 | f7-75-filtre-formu-transfers-debts-include.png | Add filter formunun altı: Categories · Labels · Currencies · Payment Type · Status · **Transfers: Include** · **Debts: Include** · **Text search** |
| E0478 | f7-84-borc-kayit-listesi-5000.png | Debt Records: **Total ₺5.000,00**; tek satır Lending, renting · Ana Hesap · "Ada Reklam → Me : Hizmet faturasi - Ada Reklam" · +₺5.000,00 (yeşil) · 11 Eyl; altta **Add Record**. Tahsilattan sonraki listedir; ortak listedeki "Manage debt" bu karede **yok** |
| E0479 | f7-89-warranties-bos-durum.png | Warranties boş durumu: "Keep your warranties in one place — Manage all your warranties here. Tap the plus button to add the first one."; arama, FAB |
| E0480 | f7-91-shopping-lists-elbise.png | Shopping lists: tek liste **Elbise** · ₺0 Estimate · 0/0 items · **Share list**; FAB |
| E0481 | f7-93-loyalty-cards-bos-durum.png | Loyalty cards boş durumu: "Keep your loyalty cards in one place — … Tap the plus button to add the first one."; arama, FAB |
| E0482 | f7-96-gelismis-ayarlar-sayi-bicimi.png | Advanced settings: **Number format — Use decimals within amounts** (kapalı) · Active module after launch — Dashboard module (kapalı) · Initial day of the month — Beginning of the accounting period: 1. Sayı biçiminin tek seçeneği ondalık; para kodu ya da ayraç seçeneği yok |
| E0483 | f7-99-payment-due-date-ayin-gunu-secici.png | Is Karti Edit account üstünde **Payment Due Date** seçicisi: ayın günü tekerleği (30 · **31** · 1), İptal / Tamam; arkada alanın değeri Not set |
| E0484 | f7-102-kayit-silme-onayi.png | Record detail (Expense, Lending, renting, Ana Hesap, 50, Labels Sahsi) üstünde **"Do you really want to delete this item?" No / Yes**. Araç çubuğunda sil · böl (split) · kaydet |
| E0485 | f7-106-sayi-bicimi-kapali-ondaliksiz-gosterim.png | Number format kapatılıp uygulama yeniden açıldıktan sonra Home: Ana Hesap **₺9.350**, Is Karti −₺5.300, Ortak Cuzdan ₺2.150, Expenses structure ₺6.000 — tutarlar ondalıksız; simge ve Türkçe ayraç değişmedi |
| E0486 | f7-55-soguk-acilis-hesaplar-sekmesi-secili.png | Hesaplar sekmesi seçili kart panosu: Günlük Özet ("Bu dönemde hiçbir işlem yok.", 7 ve 30 gün ortalaması), **Test Reklamı**, Bütçe Özeti %100 Others, GIDER ₺11.015,00. Açılışın soğuk olduğu karede değil, koşum kaydında. Reklam — basılırsa kırpılır |
| E0487 | f7-57-islem-formu-varsayilan-tur-gider.png | Ekle formu açılır açılmaz: İsim + ataç · 22 Eylül 2026 09:33 · Planlı İşlemler · tutar 0,00 yanında **kırmızı −** · hesap makinesi · TRY · Diğer/Others · Nakit/Cüzdan · Bölmek · Durum · Etiket · Not; altta **GIDER** (dolu kırmızı) · GELIR · TRANSFER. Sağda yüzen klavye araç çubuğu formun altının sağ kısmını örtüyor |
| E0488 | f7-61-cekmece-iki-hesaplar-kalemi.png | Çekmece: **Hesaplar** (ızgara simgesi, seçili; arkada kart panosu) · Takvim · **Hesaplar** (banka simgesi) · Kategoriler · Etiketler · Çöp Kutusu · Ayarlar · QuickSync · Seyahat Modu (kapalı) · Arkadaşa Öner · Geri Bildirim Gönder |
| E0489 | f7-62-ikinci-hesaplar-hesap-kurulum-ekrani.png | İkinci Hesaplar kaleminin açtığı ekran: üstte **Nakit Akım Ayarı**; VARLIKLAR — Banka ₺34.700 (Ana Hesap, Birikimler, Çek), Nakit ₺4.025 (Cüzdan −125, Ortak Cuzdan 4.150); **CARI HESAP** — Cari hesap ₺7.000 (Ada Reklam cari), Kredi Kartı −₺1.500 (Is Karti) |
| E0490 | f7-65-bolunmus-kayit-listede-2-kategoriler.png | İşlemler listesi: bölünmüş kayıt **tek satır** "Diğer · 2 Kategoriler" −₺110, Cüzdan −125; altında 15 ve 11 Eylül kayıtları, iki bacaklı transferler, (New Account) satırları |
| E0491 | f7-68-etiketli-kayit-listede-is-cipi.png | İşlemler listesi: "Diğer · Others −₺56" satırının altında **İş** çipi; aynı günde +1 ve "2 Kategoriler" −110 |
| E0492 | f7-70-filtre-profili-kaydet-ac-sifirla.png | Filtre alt sayfası: sağ üstte **sıfırla · kaydet · aç** simgeleri, Satır tarzı; metin araması, başlangıç/bitiş tutarı, Tarih Aralığı, İşlem tipi, Kategori, Hesap, Etiketler; altta kaydet simgesinin açtığı **Kaydet** sayfası ve boş ad alanı |
| E0493 | f7-72-csv-ice-aktarma-ve-cikti-aciklamasi.png | Excel (CSV) Verileri: İşlemleri Excel'den içe aktarma; Talimatlar; Bilgi — "Tüm raporları PDF, Excel **(cvs)** veya Html'e aktarmak için soldaki yazıcı simgesinin olduğu her yerde bulunur." (karedeki yazım) |
| E0494 | 39-islem-listesi-gelir-yesil-arti.png | TRANSACTIONS: gelir satırları **yeşil ve + önekli** (+1,234.00 Beta Tasarim, +2,050.00 Initial Envelope Fill, +25,000.00 Ada Reklam); gider satırları düz koyu ve işaretsiz (600.00, 1,200.00, 850.00); tarih ay/gün. Üst çubukta **hane adı** — basılırsa karartılır ya da kırpıntı çubuğu dışarıda bırakır |
| E0495 | 41-ayarlar-ust-cop-kutusu-yok.png | Settings üstü: Household (Manage Household on the Web · Subscribe to Plus · Log Out + **hane adı**) · Device (Quick Transactions · Device Nickname · Use Current Location ✓ · Theme) · Localization (Date Order · Decimal Digits). Çöp kutusu ya da geri alma kalemi yok (alt yarısı E0453). Hane adı — basılırsa karartılır |
| E0496 | 44-soguk-acilis-ana-hesap-defteri.png | Ana Hesap defteri: Drive yedekleme daveti (Atla / Yedeklemeyi Aç), dönem çipleri (Herşey seçili), Ağu 22–12 kayıtları (ataçlı 150'lik kayıt dâhil), yön düğmeleri, Toplam Gelir 47.500 · Toplam Gider 4.350 · Denge 43.150. Soğuk açılış olduğu karede değil, koşum kaydında |

### Ek: E0497–E0498 (aynı tur, uygulama denetiminden sonra)

Belge 1 düzeltme turunun uygulama denetiminde, planın yazıp uygulamanın atladığı iki güncellemenin
(WL-07 etiket formu, WL-21 sıralama) kareleri açıldı. İkisi de Belge 1'de basılmaz, dayanak olarak
anılır. Aynı serinin `f7-77` (etiketsiz Labels görünümü, E0425 aynı işi görüyor) ve `f7-100`
(liste, E0306 aynı işi görüyor) kareleri K7 gereği envantere girmedi. Kişisel veri yok.

| Kimlik | Dosya | Bayt | SHA-256 |
|---|---|---:|---|
| E0497 | [kanitlar/wallet-budgetbakers/f7-79-etiket-olusturma-formu.png](kanitlar/wallet-budgetbakers/f7-79-etiket-olusturma-formu.png) | 50077 | `5da926222c886b4bdb9e857babf89dbf57db6cca6efca7c2be30e6322fdc4f09` |
| E0498 | [kanitlar/wallet-budgetbakers/f7-101-planned-payments-siralama-secenekleri.png](kanitlar/wallet-budgetbakers/f7-101-planned-payments-siralama-secenekleri.png) | 140296 | `529ea30bd67c9553f52e42953256a50a17983426583791c8149ae53b35d39cf8` |

| Kimlik | Dosya | İçerik |
|---|---|---|
| E0497 | f7-79-etiket-olusturma-formu.png | Add label formu, boş: Name · Color (açılır renk seçici) · **Auto assign to new records** anahtarı kapalı; sağ üstte onay. Etiketi kullanıcı adıyla oluşturuyor |
| E0498 | f7-101-planned-payments-siralama-secenekleri.png | Planned payments, araç çubuğunda arama ve sıralama simgeleri; açık **Sorting** diyaloğu: By due date - newest (seçili) · By due date - oldest · By name - A -> Z · By name - Z -> A; Cancel · Default. Arkada tek plan satırı (Bulut yazilim aboneligi, Every 1 month, −₺600, 10.10.2026); altta All / Income / Expense / Transfer |

### Gözlem formlarının güncel hash'i (24 Eylül 2026)

Formların değişiklik kaydı 14 Eylül'de (P1-B14) kalmıştı. 15–24 Eylül arasında formlara Belge 2
koşumu, ek eksik koşumu ve Belge 1 düzeltme turunun bölümleri eklendi; bu değişiklikler tek tek
kaydedilmedi. Aşağıdaki satırlar dokuz formun 24 Eylül hâlidir ve önceki bütün form hash'lerinin
yerine geçer. Formlar Belge 1 ve Belge 2'nin hash denetimine girmez (çalışma boyunca güncellenen
belgelerdir); bu kayıt yalnız envanterin formların bugünkü hâlini göstermesi içindir. Görseller
değişmedi.

| Kimlik | Dosya | Bayt (başlangıç → bugün) | Satır | Güncel SHA-256 |
|---|---|---|---:|---|
| E0010 | gozlemler/money-manager.md | 34540 → 50953 | 531 | `2cd12a1b92902d57cf859f9d60f4ddb1004ccc50e1b3f519c62fc8bc63ccc226` |
| E0005 | gozlemler/bluecoins.md | 33552 → 53755 | 606 | `511256bc3099a48a25bafba6b07290113bc3890850f1521c49424e3d06f2baac` |
| E0014 | gozlemler/wallet-budgetbakers.md | 35167 → 62553 | 587 | `3a09eeb02b6163807336ab932304c67181ad2c3489ba4fe8bf3695bd71fe7212` |
| E0007 | gozlemler/hesap-defterim.md | 48856 → 57987 | 418 | `68528cab6a6b8fa074958a015ca84907e86aec610badc7b622f954e0ffaaa172` |
| E0006 | gozlemler/goodbudget.md | 32073 → 40177 | 399 | `b7222fdc8153aa7a8b40775854073df2629a2551042a2875c292e6dd67ec7de6` |
| E0008 | gozlemler/kolaybi.md | 82082 → 98489 | 957 | `7468ff50d8c9975bfe5f2ce898e6e162d742f8617872569f88986635fe02e6ef` |
| E0011 | gozlemler/parasut.md | 25547 → 31754 | 316 | `7dc0ab20327ed1e382b643f0c0fe51d7f838837bd462bbe385777dc69b65c78b` |
| E0009 | gozlemler/logo-isbasi.md | 21908 → 27987 | 337 | `94341ac0e84dba7579c291490af710ae46f3f2f01c61cf72082362a50d19dc01` |
| E0012 | gozlemler/quickbooks.md | 17818 → 19936 | 176 | `3664831b338a22fd8754de606cb253335dc04beaa14502018b93db8f65cb8494` |
