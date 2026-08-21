# ADR 0008 — Finansal Rol Başına İki Ton: Metin ve Dolgu

- Durum: Kabul edildi
- Tarih: 16 Ağustos 2026
- Bağlam aşaması: 12.7 (görsel tasarım yenilemesi)
- İlgili: `documentation/adr/0006-financial-color-theme-extension.md`,
  `documentation/adr/0007-achromatic-brand-and-own-surfaces.md`

## Bağlam

ADR 0007 marka rengini akromatik yaptı ve "ekranda anlamlı renk yalnız
ikidir" kuralını koydu. Kural doğruydu ama uygulama iki noktada onu tutmuyordu.

**1. Rol başına tek renk yoktu, tesadüfen üç ton vardı.**

Her finansal rolün tek bir "resmi" rengi var sanılıyordu (`income`,
`expense`, `neutral`), fakat ekranda üçü birden görünüyordu:

| Nerede | Hangi ton | Nasıl elde ediliyordu |
|---|---|---|
| Tutar metni | `#1B5E20` | `colors.income` |
| Halka dilimi, çubuk | `#1B5E20` | aynı token, grafik geometrisinde |
| Rozet zemini | `#C8E6C9` | `colors.incomeContainer` |
| Liste ikonu kapsülü | değişken | `colors.income.withValues(alpha: 0.12)` |

Alfa ile türetilen dördüncü ton en kötüsüydü: altındaki yüzey değiştiğinde
farklı bir renge dönüşüyor, yani hiçbir yerde ölçülmüş bir değer olmuyordu.
Kullanıcının "yeşilin ve kırmızının iki farklı tonu var" tespiti buydu.

**2. Metin eşiği, grafikleri de kısıtlıyordu.**

Tek token hem metin hem grafik için kullanılınca metnin eşiği (WCAG AA normal
metin, 4.5:1) grafiğe de dayatılıyordu. `#1B5E20` beyaz kart üzerinde 7,87:1 —
gerekenden çok daha koyu. Sonuç: halka ve çubuklar koyu, ölü, "kasvetli"
görünüyordu. Oysa metin olmayan geometrinin eşiği 3:1'dir.

**3. Akromatik marka rengi dolgu olarak kullanılıyordu.**

`colorScheme.primary` (neredeyse siyah `#16191D`) üç yerde **dolgu** olarak
duruyordu: bütçe ilerleme çubuğu, nakit akışı grafiğinin artı çubukları ve boş
durum ikonu. Buton zemininde doğru olan renk, 8 dp'lik bir ilerleme şeridinde
veya bir grafik sütununda siyah kütleye dönüşüyordu. ADR 0007 marka rengini
değiştirdi ama bu üç dolgu yerinde bırakıldı.

## Karar

**Her finansal rolün tam olarak iki tonu vardır ve ikisi de adı konmuş
token'dır:**

| Token | Nerede | Eşik |
|---|---|---|
| `income` / `expense` / `neutral` | metin ve ikon | 4.5:1 |
| `incomeFill` / `expenseFill` / `neutralFill` | halka dilimi, çubuk, ilerleme dolgusu | 3:1 |
| `*Container` + `on*Container` | rozet, ikon kapsülü, tintli kutu zemini | çift olarak 4.5:1 |

Ek kararlar:

1. **Alfa'dan ton türetilmez.** İkon kapsülü ve tintli kutu zemini
   `*Container`, üstündeki ikon/tutar `on*Container` kullanır. Bu çift zaten
   kontrast kapısında ölçülüyor.
2. **Nötr rol mordan maviye döndü** (`#4527A0` → `#0B6AD6`). Mor, gelir
   yeşili ve gider kırmızısıyla aynı ailede değildi; ekranda üçüncü bir
   doygun hue açıyor ve transferi finansal bir anlam yerine dekoratif bir
   vurgu gibi gösteriyordu.
3. **Marka rengi dolgu olarak kullanılmaz.** Üç `colorScheme.primary` dolgusu
   finans token'larına çevrildi. Marka rengi buton ve seçili sekme gibi
   **eylem** yüzeylerinde kalır.
4. **Metin tonları eşiğin hemen üstünde seçilir.** `#137D3F` (4,61:1),
   `#CF2E1F` (4,57:1), `#0B6AD6` (4,59:1) — yani kapının izin verdiği en canlı
   değerler. Daha koyu bir ton "daha güvenli" değil, yalnızca daha ölüdür.
5. **Kategori kırılımı tek hue'nun tonlarını kullanır.** Rastgele bir
   kategorik palet (mavi, yeşil, sarı, mor …) hem "yeşil yalnız gelirdir"
   kuralını kırardı hem anlamlı renk sayısını ikiden yediye çıkarırdı.
   Dilimler zaten efsanede ad, tutar ve yüzde ile yazılıdır.

## Reddedilen alternatifler

- **Tek token'ı canlılaştırmak.** Metin de grafik de aynı rengi kullansaydı,
  canlı bir yeşil metni AA eşiğinden düşürürdü. Ölçüldü: `#209E54` beyaz kart
  üzerinde 3,06:1 — metin için geçersiz.
- **Metni büyük metin sayıp 3:1 uygulamak.** Tutarların hepsi büyük değil;
  liste satırındaki tutar normal boyutta.
- **Grafikleri tamamen akromatik yapmak.** Gelir/gider ayrımı grafikte de
  gerekli; gri bir halka soruyu yanıtlamıyor.
- **Kategorilere çok renkli palet.** Yukarıdaki 5. maddede.

## Sonuçlar

### Kazanılan

- Ekranda rol başına iki ton var ve hangisinin nerede kullanılacağı token
  adından okunuyor; "üç farklı yeşil" tesadüfü mümkün değil.
- Grafikler kendi eşiğinde çalıştığı için belirgin biçimde canlı.
- Siyah kütleler kalktı; marka rengi yalnız eylem yüzeylerinde.

### Ödenen bedel

- Token sayısı arttı (üç `*Fill` alanı). Karşılığında ton seçimi tesadüf
  olmaktan çıktı.
- Metin ve dolgu tonu **kasıtlı olarak farklıdır**; yan yana durduklarında
  aynı rengin iki tonu görünür. Bu, kontrast eşiklerinin dayattığı bir
  gerçektir, bir tutarsızlık değil — testle de sabitlendi.

### Kapıların yakaladıkları

| Bulgu | Ölçülen | Nasıl çıktı |
|---|---|---|
| Tintli kutuda tutar okunmuyor | 3,95:1 | Zemin renklenince yüzeye göre seçilmiş `income` tonu eşiğin altına düştü; `onIncomeContainer`'a çevrildi |
| Gelir/nötr container'ı zeminden ayırt edilmiyor | 1,18–1,19:1 | Tint tonları koyulaştırıldı |

## Bu kararın dışında kalan

- Renk körlüğü modu ve kullanıcı tarafından seçilebilen tema.
- Kategori başına kullanıcı tanımlı renk.
- Borç/alacak ile kategori arasındaki parasal bağ kurulduğunda gerekecek
  **çok kategorili** dağılım grafiği; o zaman kategorik palet kararı yeniden
  açılır (Aşama 12.8+).
