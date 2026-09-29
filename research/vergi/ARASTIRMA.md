# Vergi teması — inceleme çerçevesi

**Durum:** başladı, 28 Eylül 2026. Bütünsel düzenlemenin ikinci teması (ilki: `research/para-akisi/HARITA.md`).

## Neden
Kullanıcı, 28 Eylül: "uygulamayı bütün olarak ele alma kapsamında vergi kısmını da incelememiz gerekiyor";
"Muhasebe paketindeki KDV'ler başka bir yerden geliyor mu?"; "vergi takvimi … şu anki yapısı biraz garip".

## Her özelliğe sorulan yedi soru
S1 ihtiyaç ve kimin için · S2 en sade sürüm · S3 doldurulur mu · S4 maliyete değer mi · S5 işletme bütçesi
kapsamında mı · S6 diğer özelliklerle uyum · S7 eklemenin/değiştirmenin/kaldırmanın sonucu.
Sonuç kovaları: **tut · sadeleştir · değiştir · kaldır · ekle.**

## İncelenen özellikler (bugünkü hâl, koddan)
- **KDV alanları:** gelir/gider, kart harcaması, cari borçlandırma, ödenmemiş fatura, POS tahsilatında isteğe
  bağlı oran + tutar; taşınır, hesaplanmaz.
- **İndirilebilirlik:** gider tanıyan kayıtlarda, yalnız işletme kapsamında.
- **Muhasebeci paketi:** ayın işletme gelir/gideri; KDV özeti = kayıtlara **girilen** KDV tutarlarının toplamı
  (gelir ayrı, gider ayrı) + "KDV'siz satır sayısı". Gelir/giderden hesaplanmaz. (`AccountantPackageUseCases`)
- **Vergi takvimi:** öneriler tutarsız tekrarlayan plan olarak kurulur; gerçekleşince tutar girilir.
- **Vergi karşılığı:** hedef mekanizmasıyla para ayırma.

## Araştırma soruları
- V1 Şahıs şirketinin vergi rejimleri: basit usul, işletme hesabı, bilanço, serbest meslek, esnaf muaflığı —
  hangisinde KDV, hangi defter, hangi beyanlar.
- V2 Gerçek takvim: KDV, muhtasar ve prim hizmet, geçici vergi, yıllık gelir vergisi, Bağkur; rejime göre.
- V3 Muhasebeci bizden ne ister, neyi zaten başka yerden (e-fatura, e-arşiv, Z raporu, GİB) alır.
- V4 KDV indirimi ve indirilebilirlik: hangi gider, hangi durumda; şahsi harcama.
- V5 Kasa bulgularıyla bağ: Z raporu KDV'si, POS komisyonunun BSMV'si, yemek kartı.

## Çıktılar
`BULGULAR.md` (kaynaklı) → emülatör denemesi → `KARAR.md` (özellik başına yedi soru ve kova).
