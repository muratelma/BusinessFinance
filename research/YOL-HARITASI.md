# Bütünsel düzenleme — yol haritası

**Durum:** güncel, 29 Eylül 2026 (karar denetimi bitti, kararlar kapanış listelerine işlendi). Tek başvuru noktası: genel ilerleyiş (§0), kararlar (§1), açık sorular
(§2), koddan önceki adımlar (§3), iş paketleri (§4), bekleyen girdiler (§5).
**İlke (kullanıcı, 28 Eylül):** "Uygulamadaki özelliklere tek tek yaklaşmak yerine bütün olarak yaklaşıp
sorunları toplu halletmemiz gerekiyor"; "açık kalan kısımları kapata kapata ilerleyelim". Yöntem: **inceleme
bütün, teslim paket paket** — her paket kendi başına çalışır, test edilir, emülatörde denenir.

---

## 0 · Genel ilerleyiş: sohbetin başındaki plan ve bugünkü hâli

**Sohbetin başındaki öneri (28 Eylül öğleden sonra):**
1. `ui-trials` dalını kapatmak (kalıcı parçaları 06.2 gruplarına bağlayıp `main`'e almak)
2. Kendi ürünümüzün gerçek hayat denetimi (bir haftalık esnaf senaryosu)
3. Belge 3, hafif sürüm (rakip araştırmasını kararlara çevirmek)
4. Kalan ekranları Claude Design'da tasarlamak: gider/gelir formu · kredi kartlarım · kart harcaması ve ödemesi ·
   hesaplar ve transfer · planlama
5. Seçilen özellikleri kendi aşamalarında eklemek

**Kullanıcının değiştirdiği sıra (aynı gün):** önce Kasa/POS'u tam işlevli yapmak → sonra birkaç arayüz düzenlemesi
(yukarıdaki 5 ekran) → sonra önerilen denetim ve Belge 3. İlerledikçe kapsam "bütünsel düzenleme"ye genişledi.

**Bugün itibarıyla:**

| Adım | Durum |
|---|---|
| Kasa/POS araştırması + kendi uygulamamızın emülatör denetimi (eski adım 2'nin Kasa kısmı) | **Bitti** |
| Para akışı haritası (uygulamanın bütün para yolları) | **Bitti** |
| Kasa ve para yolları teması | **Kapandı** (KP1–KP21) |
| Vergi teması (yeni; kapsam genişlemesi) | **Kapandı** (V-K1–V-K16, §6.5 ile) |
| Listeler ve filtreler teması (İşlemler filtresi, kategori görünümü) | Açık (G3) |
| Kararların temiz oturumda denetimi (kullanıcı isteği) | **Bitti** (29 Eylül): `DENETIM-2026-09-29.md` + dış kaynak eki; sonuçlar `KAPANIS.md` ve `YENI-YAKLASIM.md` §6.6'ya işlendi |
| ADR / PRD / kural / aşama belgeleri | **Sıradaki** (§3) |
| Uygulama paketleri P0–P9 (5 ekranlık tasarım turu P9'da; Kasa tasarımı P6; vergi ekranı tasarımı P3) | Denetim ve belgelerden sonra |
| Belge 3 | Uygulama paketlerinden sonra; bu temaların kararları Belge 3'ün "BusinessFinance bugün" bacağının girdisi olur. Belge 3 planının açık kararları (K1–K8, `research/rakip-arayuz-ve-akis/raporlar/belge3-plani.md`) hâlâ açık |

## 1 · Kararlar

| Tema | Geçerli belge |
|---|---|
| **Kasa ve para yolları** (kapandı, 29 Eylül denetimiyle güncellendi) | `kasa-pos-gun-sonu/KAPANIS.md` — KP1–KP22 |
| **Vergi** (kapandı, 29 Eylül denetimiyle güncellendi) | `vergi/YENI-YAKLASIM.md` **§6.6** (tek tablo) |
| Denetim (gerekçe) | `DENETIM-2026-09-29.md` (bulgular B1–B26) · `DENETIM-2026-09-29-dis-kaynak.md` (kaynaklar, düzeltmeler) |
| Fikirler (henüz karara bağlanmamış) | `fikir-kaydi.md` — F01–F24 |

Gerekçe belgeleri: `para-akisi/HARITA.md` (kapandı) · `kasa-pos-gun-sonu/` (ARASTIRMA-PLANI, BULGULAR, KARAR,
GUN-SONU-BELGELERI) · `vergi/` (ARASTIRMA, BULGULAR, KARAR — aşıldı).

## 2 · Açık sorular

| # | Soru | Karar (29 Eylül) |
|---|---|---|
| G1 | `ui-trials` dalındaki tasarım işi `main`'e alınsın mı? | **Hayır, şimdilik.** `ui-trials`'ta devam; bir bulut oturumu açıldığında `main`'e alınıp yeni dal açılır (kullanıcı) |
| G2 | Yeni ekranlar için önce tasarım mı, önce kod mu? | Kullanıcı Claude'a bıraktı: **her grupta önce backend; büyük ekranların brifi backend'le eşzamanlı, Flutter tasarımla birlikte**; kodda çıkan sorun brifi günceller (`stages/06.3-butunsel-duzenleme.md` "Tasarım yaklaşımı") |
| G3 | "Listeler ve filtreler" teması ne zaman? | 06.3 Grup 8; önce kendi karar adımı (kullanıcı: "işlemlerdeki filtreler ve kategori kısımlarını da halledelim") |

## 3 · Koddan önceki adımlar

1. ~~**Karar denetimi (temiz oturum).**~~ Bitti, 29 Eylül.
2. ~~**Denetim sonucuna göre düzeltme.**~~ Bitti, 29 Eylül (`KAPANIS.md`, `YENI-YAKLASIM.md` §6.6).
3. ~~**Belgeler**~~ — **yazıldı, 29 Eylül** (ADR 0018 ve 0019 kullanıcı
   onayıyla kabul edildi; PRD, `AGENTS.md`, `CLAUDE.md`, `PROJECT-ROADMAP.md`,
   `stages/06.3-butunsel-duzenleme.md` Aktif, 06.2 Beklemede). ADR'ler bu kararları
   da taşır: ADR 0018'e tutarsız plan, "kapatıldı" durumu, ödemede
   gün/hesap/kart, geri alma, vergi türü ve vergi işareti, kaynaksız plan (ADR 0005'i etkiler); ADR 0019'a gün
   sonu modeli (KP4, KP7), yoldaki paranın iki kaynağı (ADR 0015 §2–3'ün test kapısını genişletir), gün sonunun
   geri alınması. Denetimin ADR/PRD soru listesi: `DENETIM-2026-09-29.md` §7 (karar belgeleri, kendi başlarına commit edilebilir; kullanıcı onayıyla):
   - **Yeni ADR 0018 — Vergi bir nakit planıdır:** KDV alanları, indirilebilirlik ve muhasebeci paketi kalkar;
     vergi kendi ekranında tanımlanan, ödendiğinde etkileyen bir plan. **ADR 0016'nın** KDV/indirilebilirlik/paket
     kısımlarının yerini alır (vergi hesaplanmaz ilkesi kalır).
   - **Yeni ADR 0019 — Gün sonu, POS tanımı ve yatış:** **ADR 0015**'i genişletir (POS tanımı, gün sonu, yatış
     kaydı, kesinti) ve **ADR 0014** çerçevesinde kartla cari tahsilatı (gelir tanımayan yoldaki para) tanımlar.
   - **PRD:** §2 hedef ("muhasebeciye gidecek veri"), §6.3 ("hesap mutabakatı" — banka sayımı alınmadı),
     §6.8 Kasa ve POS, §6.9 Vergi ve muhasebeci, §16 "vergi alanları" riski, §17.
   - **`CLAUDE.md` / `AGENTS.md`:** KDV, indirilebilirlik ("kapsam ile indirilebilirlik ayrı alanlardır"),
     vergi takvimi ve muhasebeci paketi kuralları; belge haritasına yeni ADR'ler ve `research/` kararları.
   - **Aşama:** 06.2'nin kapsamı yeni özellik ve finansal davranış değişikliği almıyor. Uygulama için yeni aşama
     belgesi (öneri: **06.3 — Bütünsel düzenleme**), `stages/README.md` ve `PROJECT-ROADMAP.md`.
4. **Commit:** araştırma belgeleri ve karar belgeleri (kullanıcı onayıyla). `research/kasa-pos-gun-sonu/pos
   belgeleri/` Git dışında (gerçek işletme bilgisi).

## 4 · İş paketleri (önerilen sıra)

Her paket: **karar → (gerekirse) tasarım brifi → kod → test → emülatörde deneme → commit.**

| Paket | İçerik | Bağlı |
|---|---|---|
| ~~P0 · Temizlik~~ | Aşama belgesi yazıldı; dal birleştirme ertelendi (G1); commit'ler kullanıcı onayıyla sonra | §3 |
| **P1 · Kesin hatalar** | Kasa'nın yenilenmemesi (U11 — hesap bakiyesini değiştiren **her** olay) · POS iptali ve "hesaba geçti"nin geri alınması (U12) · sayımdan sonra değişen kasa (U10) · **içe aktarımda çifte sayım uyarısı + satırı atla** (U8'in ucuz koruması) · "quarterly" (V-U8) · yanıltıcı "Sonraki" (V-U9) · plan formunda üst üste binen etiket (V-U6) | — |
| **P2 · Kaldırmalar** | KDV alanları (5 form), indirilebilirlik, muhasebeci paketi (ekran, uç, dışa aktarma; **tamamen**); kolonlar ve veri | ADR 0018 |
| **P3 · Vergi ekranı** (tekrarlayan planın genişlemesi + ekran) | Tasarım brifi · **plan: vergi türü alanı, tutar ve kaynak isteğe bağlı, "kapatıldı" durumu, "seçilen aylarda" sıklığı** · kategoride "vergi" işareti · hazır türler (yalnız sıklık ve gün; MTV/emlak şahsi seçilebilir) · "Ödedim" (tutar, gün, hesap ya da kart zorunlu; ödeme gününe yazılır) · **ödemeyi geri alma** · "tutar belli oldu" · **toplu vergi ödemesi** (vadesi gelenler seçili) · "Ödenenler" · Yaklaşanlar'da tutarsız kalem · sil/duraklat | P2 |
| **P4 · POS tanımı** | POS'larım (yemek kartı dahil); form önden dolu; canlı net; iş günü (sahte "Gecikti" biter) | ADR 0019 |
| **P5 · Gün sonu ve yatış** | Gün sonu paneli ("+" ve Kasa'dan) · nakit + POS başına kart; elle girişte üç alandan ikisi · Z no ile tekrar kontrolü, "ek gün sonu" · gün sonunu bir bütün olarak geri alma · toplu Z (aralık kapalı, ay dönümü uyarısı) · atlanan Z uyarısı · **zaten girilmiş kayıtlar** listesi (satışlar ve kartla tahsilat varsayılan düşülür, KP7) · toplu yatış, gerçek tutar, kesinti, geri al · kartla tahsil (cari ve yükümlülük), yoldaki paranın iki kaynağı · Yaklaşanlar'da beklenen POS · bağımsız "Kasayı say" (beklenen = hesap bakiyesi, kaydedilmemiş fark ayrı satırda) · kayıtta Z no | P4 |
| **P6 · Kasa sekmesi** | Claude Design brifi → Bugün / Nakit / Kartla gelecek / Son günler | P5 |
| **P7 · Gün sonu okuma** | Z raporu ve banka gün sonu toplamını kamerayla okuma: tarih/aralık, Z no, NAKİT, KART, TOP okunur; KÜMSAT, KDV, BRÜT, iptal okunmaz. Z'nin faturalı satış, cari tahsilat ve yemek kartı satırları ve KP7 varsayılanları gerçek örnekle yeniden incelenir | P5, §5 |
| **P8 · Listeler ve filtreler** | İşlemler filtresi (hesap, kart, kategori, tarih), kategori görünümü, her yerden İşlemler'e kapılar | G3 |
| **P9 · Tasarım turu** | Sohbet başındaki 5 ekrandan kalanlar: gider/gelir formu · kredi kartlarım · kart harcaması ve ödemesi · hesaplar ve transfer · planlama (vergi ayrıldıktan sonra); Türkçe tarih ve binlik ayırıcı (F12); formların önden dolması (F11) | P3, P8 |

**Yedek şeması:** P2–P5 değişiklikleri **tek sürümle** ilerler (P5 sonunda); her paket ayrı sürüm açmaz.

**Sonraya kalanlar** (fikir kaydında): aylık toplu gün sonu (KP8) · CSV (F05) · tahakkuk fişi (F17) · cari hesap
geliştirme (F21) · Özet'te vergi raporu (F22) · yapılandırılan borç (F20) · geçmiş ayda hesap bakiyeleri (F13) ·
kasa sayımını yeniden düşünmek (F23).

## 5 · Bekleyen girdiler
- **Gün sonu görselleri (P7):** kartlı satışı olan bir **Z raporu**, bir **banka gün sonu fişi** (toplam ve detay),
  bir **mali hafıza aylık raporu**, bir **üye işyeri dökümü**, varsa **yemek kartı** dökümü. Ayrıntı
  `kasa-pos-gun-sonu/GUN-SONU-BELGELERI.md` §4. Beş gerçek fotoğraf incelendi (§5): toplu Z, boş Z, nakit fişler.
  Denetimden eklenen: **kartla ödenmiş faturalı satışı ve bir veresiye tahsilatı olan bir Z** (NAKİT/KART
  satırlarının bu işlemleri içerip içermediği — KP4, KP7) ve **yemek kartı satırı olan bir Z** (lokanta).
- **Bağkur'un dönemi:** Eylül sonunda ödenen prim Eylül'ün mü Ağustos'un mu — SGK kaynağından doğrulanacak.
- Motorlu taşıtlar (Ocak, Temmuz) ve tabela (Ocak) ayları **doğrulandı**.
