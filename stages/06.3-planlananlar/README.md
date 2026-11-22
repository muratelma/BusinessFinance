# Planlananlar, tekrarlayanlar ve abonelikler — giriş kapısı

Aşama 06.3'ün çalışma klasörü. Bu konuda oturum buradan başlar. Tasarım
kararlarının kaynağı Codex'in çalışma belgesidir:
`design/planlananlar/README.md` (17–21 Kasım 2026; panolar
`tasarim-onizleme/planlananlar-*`). O belge tasarım geçmişidir; uygulama
durumu, sıra ve günlük burada tutulur.

İşleyiş kullanıcı tarafından Claude Code'a bırakıldı (22 Kasım 2026).

## Sınırlar

- Borç-alacak satırlarının anlamı değişmez (`stages/06.3-borc-alacak/README.md`
  "Codex çalışması" maddesi): `debt-installment`, `receivable-installment`,
  `payable-obligation`, `receivable-obligation`, `counterparty-*-due` türleri,
  eylemleri ve hedefleri; `includeCounterpartyDues` bayrağı. Bu satırların
  ekrandaki eylemi borç-alacak Faz 5–7'de bağlanır.
- Özet'teki Yaklaşanlar kartı (21–22 Kasım teslimi) bu işin dışında kalır;
  yalnız Planlananlar'a geçişleri korunur.

## Fazlar

| Faz | İş | Durum |
|---|---|---|
| A1 | Sunucu: özel tarih aralığı, dönem özeti (ödenecek / beklenen giriş / karta yansıyacak, tutarı belirsiz sayısı), seçimden önce gecikenler, tür seçimi | Bitti (22 Kasım) |
| A2 | Flutter: Planlananlar listesi (04 pano), filtre paneli ve takvim (18), boş durumlar (06), yenileme uyarısı (18-01), bilinmeyen türün atlanması | Bitti (22 Kasım); emülatör kabulü bekliyor |
| A3 | Flutter: satırdan ilgili ödeme paneline (dönem, ekstre, vergi, taksit planı) ve "…görüntüle" bağlantıları | Başlamadı |
| B1 | Tekrarlayanlar listesi (09/11 pano, arama, `Abonelikler ›`, ilk kullanım) | Başlamadı |
| B2 | Plan ayrıntısı tam sayfa (07 pano): sıradaki vade, gecikmeler, son iki ödeme, takvim; Düzenle, Duraklat/Sürdür | Başlamadı |
| B3 | Sunucu: D1 dönemden itibaren tutar, D2 planı sona erdir | Başlamadı |
| C | Abonelikler ve ücretsiz deneme (katalog, form, hatırlatma, teyit) | Borç-alacaktan sonra |

Sıra: A → B → borç-alacak Faz 4–7 → C. Gerekçe: borç-alacak ekranları A'nın
Planlananlar yapısına bağlı; C yeni bir veri modeli ister ve ötekileri
beklemez.

## Kabul edilmiş davranış (kısa)

- Seçim başlangıcı ve sonu dahil, vadesi seçimdeki açık kayıtlar ana listededir.
  Gecikme bugüne göre belirlenir. Hem seçim başlangıcından hem bugünden önce
  vadesi geçmiş açık kayıtlar `15 Aralık'tan önce gecikenler` gibi tarihli,
  açılır satırdadır ve dönem toplamına girmez (19 Kasım kararı).
- Dönem toplamları ayrıdır: ödenecek, beklenen giriş, karta yansıyacak; yalnız
  ilgili kayıt varsa görünür. Tutarı bilinmeyen ödeme sayıyla belirtilir.
  Toplamlar birbirinden düşülmez.
- Satırın tamamı ve kısa eylem aynı hedefi açar; tek dokunuş mali kaydı
  onaylamaz.

## Günlük

### 22 Kasım 2026 — klasör açıldı, A1 başladı

- Özet düzeltmeleri `0040302` ile commit edildi (`ozet-tamamlama`).
- A1 sunucu tasarımı: planlanan uca isteğe bağlı `from` / `to`. Verilirse
  `daysAhead` yerine aralık kullanılır; cevap `period` özetini taşır. Eski
  istemci aynı cevabı alır.

### 22 Kasım 2026 (devam) — A1 ve A2

- **Sunucu:** planlanan uca `from`, `to`, `type`; cevapta `period` (üç ayrı
  toplam, tutarı belirsiz ödeme sayısı, seçimden önce gecikenler). Eski
  istemcinin okuması değişmedi. Sözleşme `financial-activity-api-contract.md`.
- **Ekran:** 04 panosundaki dönem kartı, gecikenler satırı, gün kartları ve
  satır dili; filtre paneli (tür menüsü, dönem, takvim); boş durumlar; yenileme
  uyarısı; bilinmeyen türün atlanması. Kart Hareketleri'nin takvim ızgarası ve
  dönem satırları ortak bileşene alındı (`AppRangeCalendarGrid`,
  `AppDateRangeDialog`, `AppFilterRadioRow`, `AppFilterRangeRow`).
- **Sapma:** satırdaki tutar renksiz (panodaki gibi); paranın yönü alttaki etki
  satırında yazılı. Gecikenler satırının başlığı `N geciken kayıt` (panoda
  `Geciken ödemeler`): seçimden önce gecikenler tahsilat da içerebiliyor.
- **Kalan (A3):** satırdan ödeme panellerinin kendisi (ekstre için kart ödeme
  formu, vergi için Ödedim paneli, taksit planı) ve `…görüntüle` bağlantıları;
  bugün ekstre ve taksit Kartlar'a, vergi Vergi takibi'ne gidiyor.
- **Not:** emülatördeki API (5284) bu değişiklikten önce açılmış; yeni aralık
  ve toplamlar API yeniden başlatılınca görünür.

