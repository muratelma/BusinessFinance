# Ortak içindekiler ve pilot kapsamı

> **16 Eylül 2026 — Belge 1 için bu dosya artık tek kaynak değildir.**
> Belge 1'in bölümleri, alt soruları ve sınırları [`belge1-bolum-plani.md`](belge1-bolum-plani.md)
> içinde yeniden kararlaştırıldı. Aşağıdaki Belge 1 listesi pilot turunun taslağıdır ve plandan
> farklıdır: plan 13 bölümdür, ağırlıkları eşit değildir, "bekleyen işler" Bölüm 8'e taşınmıştır ve
> ürün kartları Bölüm 2'dedir. **Çelişkide plan geçerlidir.**
>
> Belge 2'nin listesi ve on temanın bölüm eşlemesi geçerliliğini korur; Belge 2 planlandığında
> aynı biçimde gözden geçirilecektir.

**P5-I — 15 Eylül 2026.** Faz 8 kullanıcı onayıyla açıldı. İçindekiler taslağı hazır; [Pilot](pilot/pilot-islem-ekleme.md) üretildi; kullanıcı değerlendirmesi bekleniyor. Tam raporlar henüz yazılmadı. Düzen pilot geri bildirimiyle değişebilir.

## Belge 1 — Rakip arayüz yaklaşımları *(pilot turu taslağı — güncel hâli planda)*

Ana soru: ürün bilgiyi ve seçenekleri nasıl sunuyor?

1. **Okuma kılavuzu:** kapsam, tarih, ürün/paket/platform ve kanıt türleri.
2. **Ürün kimliği:** dokuz ürünün kullanım odağı, ilk karşılaşma ve erişim sınırları.
3. **Gezinme ve ana ekran:** sekme/menü, birincil aksiyon, bakiye ve bekleyen işlerin bilgi sırası.
4. **Görsel anlatım:** kart/liste/tablo/grafik; renk, tipografi, tutar, durum ve tarih/filtre bağlamı.
5. **İşlem ekleme ve geri bildirim — pilot:** giriş noktası, tür/tutar/tarih/hesap/kategori seçimi, kaydetme ve görünür sonuç.
6. **Hesap, kart ve transfer yüzeyleri:** hesap seçimi, borç/limit/dönem sunumu, transfer yönü.
7. **Sınıflandırma:** işletme/şahsi ifadeleri, kategori, etiket, proje ve cari eksenleri.
8. **Plan, vade ve borç ekranları:** tekrar/taksit, bekleyen/tamamlanan durumlar, fatura/tahsilat aksiyonları.
9. **Rapor ve dönem seçimi:** özet/ayrıntı, filtre, karşılaştırma ve toplam etiketleri.
10. **Veri yönetimi ve diğer modüller:** çıktı, içe aktarma, ekler, yedek/paylaşım; stok/personel/çek/senet ve yardımcı araçların yerleşimi.
11. **Yaklaşımların kazanım ve bedelleri:** ortak tercihler, ürüne özgü farklar, gözlenmiş boş/hata/yükleme durumları.
12. **Kaynak ve görsel eki:** bölüm/kanıt kimliği, bağlam, görsel ve ayrıntı kaydı.

## Belge 2 — Rakip finansal akışlar ve özellikler

Ana soru: kullanıcı işi nasıl tamamlıyor ve gözlenebilir sonuç ne?

1. **Kapsam ve özellik haritası:** dokuz ürünün erişim/test derinliği, kanıt türleri, bilinmeyen alanlar.
2. **Gelir/gider ekleme ve sonuç — pilot:** başlangıç, giriş, kaydetme, liste ve hesap/rapor etkisi; düzeltme/silme/geri yükleme ayrı alt senaryolar.
3. **Hesap ve açılış bakiyesi:** nakit/banka/diğer hesaplar, toplam ve net varlığa katılım.
4. **Transfer, kart borcu ve ödeme:** gönderen/alıcı, giderden ayrılma, borç/limit ve dönem sınırları.
5. **İşletme/şahsi sınıflandırma:** kapsam/kategori/etiket/proje/cari alanlarının gözlenen işleyişi.
6. **Planlama, tekrar, taksit ve bütçe:** tanım/gerçekleşme, onay/otomasyon, zaman, bekleyen görünüm, hedef ve tahmini bakiye sınırları.
7. **Borç, fatura, tahsilat ve ödeme:** kayıt oluşumu, para hareketi, kısmi ödeme, kalan tutar; hesap toplamı ile belirli borca bağ farkı.
8. **Raporların anlamı:** gelir/gider, nakit akışı, bakiye, dönem/filtre; girilmiş vergi alanları ve kaynaklarda görülen muhasebe raporları.
9. **Veri aktarımı ve entegrasyon:** çıktı/teslim, içe aktarma, ekler, yedek/geri yükleme, senkronizasyon; banka/e-belge/muhasebeci temasları.
10. **Diğer modüller:** stok/depo, personel/maaş, çek/senet, not/kupür ve yardımcı araçlar.
11. **Akışlar arası bağlar ve sınırlar:** hesap, borç, rapor, zaman ve geri alma etkilerinin ortak özeti; görünmeyen fiziksel veri modeli bilinmiyor.
12. **Senaryo ve kanıt eki:** başlangıç/eylem/sonuç zinciri, kaynak bağlamı, denenmeyen dallar.

## On temanın bölüm eşlemesi

| Tema | Belge 1 | Belge 2 |
|---|---|---|
| 01 Ürün kimliği | 1–2 | 1 |
| 02 Gezinme/özet | 3–4 | 1, 8, 11 |
| 03 İşlem girişi | 5 | 2 |
| 04 Hesap/kart/transfer | 6 | 3–4 |
| 05 İşletme/şahsi | 7 | 5 |
| 06 Planlama/tekrar/taksit | 8 | 6 |
| 07 Borç/fatura/tahsilat | 8 | 7 |
| 08 Raporlama | 9 | 8 |
| 09 Veri aktarımı/entegrasyon | 10 | 9 |
| 10 Diğer modüller | 10 | 10 |

Bölüm başlığı özelliğin her üründe test edildiği anlamına gelmez. Ortak anlatım ve kaynak ekleri bütün temaları bağlar.

## P5-P — Küçük deneme bölümü

**Konu:** İşlem ekleme ve kayıttan sonra görünen sonuç. İlk adaylar Money Manager, Wallet ve Hesap Defterim. Mevcut kanıt yeterliliğine göre seçim gerekçesi pilotta yazılır; yeni emülatör koşumu yapılmaz. Mevcut senaryoların gerçek tarih/tutarları korunur.

- Belge 1 örneği giriş noktası, form düzeni, seçimler ve geri bildirimi anlatır.
- Belge 2 aynı kanıtı başlangıç → eylem → görünür sonuç zinciriyle işler; hesap/rapor etkisini yalnız desteklendiği kadar anlatır.
- Hedef yaklaşık 4–6 sayfalık birleşik deneme ve 4–6 seçilmiş görsel; okunabilirlik için uzunluk değişebilir. Ürün puanlaması yapılmaz.
- Görseller kaynak kimliği, uygulama/koşum bağlamı ve kısa altyazı taşır. Kişisel alan varsa teslim kopyasında karartılır, özgün kanıt korunur.
- Markdown kaynağından Word/PDF denemesi üretilir; Türkçe karakter, font, tablo, sayfa bölünmesi, görsel ve bağlantılar gerçek çıktıda kontrol edilir.
- Kullanıcı ayrıntı düzeyi, dil, görsel yoğunluğu ve başvuru değerini değerlendirir. Pilot değerlendirilmeden tam rapor üretimine geçilmez; pilot kabulü tam rapor kabulü değildir.

## Ortak kapsam ve kaynaklar

Dokuz ürün: Money Manager, Bluecoins, Wallet, Hesap Defterim, Goodbudget, KolayBi, Paraşüt, Logo İşbaşı ve QuickBooks. QuickBooks Solopreneur kaynakları ve QBO Simple Start giriş kareleri ayrı tutulur. Görsel, koşum notu, kullanıcı beyanı ve çıkarım ayrılır; kaynak beyanı canlı test sayılmaz. Ölçülmemiş erişilebilirlik, başarı/hız ve kullanıcı talebi sonucu yazılmaz.

B06 nedensellik, B08 demo adlarından talep çıkarımı, B10 denenmemiş dallar, B11 e-posta teslimi ve B12 fiziksel şema sınırları korunur. Bluecoins satırının yeniden açınca gelmesi kullanıcı beyanıdır; teknik kök neden değildir. Stok/banka/vergi/e-belge rakip kapsamının parçasıdır; BusinessFinance'e alma kararı değildir.

Belge 3, ilk iki rapor onaylandıktan sonra hazırlanır. İlk iki rapor BusinessFinance'i puan ölçütü yapmaz; öneriler ürün/aşama belgelerine otomatik aktarılmaz.

- [Üretim planı](../FAZ7-8-UYGULAMA-PLANI.md)
- [Tematik bulgular ve P4-K](../BULGU-DOGRULAMA-KAYDI.md)
- [Kanıt envanteri](../KANIT-ENVANTERI.md)
- [Güncel durum](../DURUM.md)
- Pilot adayları: [Money Manager](../gozlemler/money-manager.md), [Wallet](../gozlemler/wallet-budgetbakers.md), [Hesap Defterim](../gozlemler/hesap-defterim.md).
