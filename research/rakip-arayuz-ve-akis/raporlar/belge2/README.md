# Belge 2 — Rakip finansal akışlar (v3 üretimi)

**Durum:** on bölümün hepsi yazıldı ve üretildi — **105 sayfa, 129 kare, 1.272 iddia.**
23 Eylül 2026'da ek eksik koşumunun bulguları işlendi; ne değiştiği ve neden
`../belge2-entegrasyon-plani.md` §10'dadır. Aynı gün kırpma turu (§11), çıkarım denetimi (§12),
çıkarım testleri (`../belge2-cikarim-degisiklikleri.md`) ve son kontrol
(`../belge2-son-kontrol.md`) yapıldı; **belge kapandı.** Yedekler `../_yedek/2026-09-23-*`.
Birleşik belge `tam/belge2.pdf`.

v1 üretimi `../belge2-v1/` altında duruyor, silinmedi. Neyin neden değiştiği
`../belge2-yeniden-kurgu.md` içinde — o belge bu üretimin tek başvuru kaynağıdır.

---

## v1'den ne değişti

| | v1 (83 sayfa) | v3 |
|---|---|---|
| Sayfa kalıbı | Kendi türleri: matris / zincir / dönem / şema | **Belge 1'in kanıtlanmış kalıbı** (`kalip.py`) olduğu gibi |
| Kare | 83 sayfada **5** | Bölüm başına **en az 12**, kapı zorluyor |
| "Görülmedi" | Kelimelerin %30'u | Eksikler `eksik-listesi.md`'de; metinde yalnız iddiayı sınırlayan cümle |
| Çıkarım | Yasak ("Çıkarılmayan sonuç" 36 kez) | **Ayrı bir kanıt katmanı**: `cikarim` rozeti, her blok dayanağıyla |
| Ürünün işine yarayan çıkış | Yok | Her bölümün sonunda **Belge 3'e taşınan soru** |
| Başlıklar | "5.C1 · İşleyiş" | Okurun dili; iç terimler kapıyla yasak |

---

## Bir bölümün anatomisi

Dört adım, dokuz bölümde de aynı:

```
N.1 – N.x   Ne oldu / ürünler nerede ayrışıyor      yanyana · tablo · soru
N.x+1       Yolun neresinde ayrıldılar              akis      [tek sayfa]
N.son       Neden ayrıldılar                        cikarim
            ├── mekanizma   kanıttan çıkan neden (dayanağı zorunlu)
            ├── kazanç      ürün başına ne kazandırıyor / ne kaybettiriyor
            └── soru        Belge 3'e taşınan 1–3 soru
```

Sıra bilinçlidir: **kanıt → yol nerede ayrıldı → neden ayrıldı.** Akış sayfası yeni
kanıt getirmez; bölümde kareyle gösterilen davranışları tek bir ortak sürecin üzerine
yerleştirir ve ürünlerin gerçekten ayrıldığı adımda çatallanır. Ürün başına ayrı şema
v1'in hatasıydı — her bölümde **tek akış** vardır ve kapı bunu zorlar.

`cikarim` dışındaki bütün sayfa türleri `belge1/ortak/kalip.py`'den gelir ve
**değiştirilmez**; `ortak/b2.py` yalnız `cikarim`'i, sayfa kırmayı ve kapıları ekler.

### Tabloda rozet kuralı

`canli` varsayılandır ve **rozet taşımaz** — 25 aynı rozet tabloyu okunmaz yapıyordu.
Rozet yalnız `kaynak` · `beyan` · `yok` · `cikarim` hücrelerinde durur. Rozetsiz hücre
de iddia tablosuna girer; izlenebilirlik hücrenin `d` alanındadır.

---

## Kapılar — ihlalinde üretim durur

| Kapı | Kural |
|---|---|
| G1 | Her bölüm en az bir `cikarim` sayfası taşır |
| G2 | Her `cikarim` dolu `mekanizma` + `kazanc` taşır; **her mekanizma bloğu `dayanak` taşır** |
| G3 | Bölüm en az `EN_AZ_KARE` kare basar |
| G6 | Bölümde tam olarak **bir** akış sayfası var, en az bir kez çatallanıyor, her dalı bir kareye bağlı ve çıkarımdan **önce** geliyor |
| G4 | PDF metninde iç terim geçmez: "A kısmı", "C kısmı", "olay defteri", "konu bloğu", "Çıkarılmayan sonuç" |
| G7 | Metinde kendi kavramlarımız geçemez (kendi ürün adımız, ADR numaraları, kendi sınıf adlarımız) — o karşılaştırma Belge 3'ündür |
| G8 | Kapsam tablosunda sayılan her ürün **gövdede** en az bir kez geçer; geçmiyorsa kapsamdan da çıkar |
| G5 | Motorun kendi kapıları: kare hash'i, kırık atıf, kişisel veri sözcüğü, sayfa taşması |
| G9 | `tam/uret.py`: kapak, içindekiler ve kanıt eki G4, G4b ve G7'den geçer; kapakta sayılan her ürün bölümlerde geçer. **Kapakta kendi ürün adımız serbest** (kullanıcı kararı, 23 Eylül 2026); G7'nin geri kalanı kapakta da geçerli |

G2'nin `dayanak` şartı bilinçlidir: **çıkarım serbestliği kanıtı gevşetmez.** Gözlem
formunda yazılı olmayan bir çıkarım basılmaz.

---

## Bir bölüm nasıl üretilir

```bash
# raporlar/ içinden
./.pilot-tools/venv/Scripts/python.exe -X utf8 belge2/bolum-NN-<slug>/uret.py
```

Çıktı: `bolum-NN.pdf` · `.md` · `iddia-tablosu.md` · `eksik-listesi.md` ·
`kaynaklar.md` · `kanit-manifest.json` · `onizleme/sNN.png`.

**Otomatik kapılar yeterli değildir.** Her sayfa `onizleme/` üzerinden gözle incelenir.
Kapılar yapıyı korur, iddianın kanıttan çıktığını doğrulayamaz — o denetim elle yapılır:
her hücrenin `d` alanı açılır ve kanıt dizinindeki açıklamayla karşılaştırılır.
Bölüm 1'de gözle incelemede bulunup düzeltilenler: her hücrede tekrarlayan rozet,
taşan çıkarım sayfası, ikiye bölünen kazanç tablosu, Goodbudget karelerinde okunmayan
sayılar (kırpma ile büyütüldü).

### Kanıt arama

```bash
python ara.py <urun|.> <anahtar> ...     # scratchpad'deki yardımcı
```

`belge1/ortak/kanit-dizini.json` 460 kimlik / 425 kare taşır ve her karenin içerik
açıklaması vardır.

---

## Bölümler

| # | Klasör | Durum |
|---|---|---|
| 1 | `bolum-01-gelir-gider` | **Bitti** — 10 sayfa, 19 kare |
| 2 | `bolum-02-hesaplar` | **Bitti** — 9 sayfa, 14 kare |
| 3 | `bolum-03-kart` | **Bitti** — 12 sayfa, 22 kare |
| 4 | `bolum-04-borc-cari` | **Bitti** — 10 sayfa, 18 kare |
| 5 | `bolum-05-zaman` | **Bitti** — 10 sayfa, 18 kare |
| 6 | `bolum-06-siniflandirma` | **Bitti** — 9 sayfa, 11 kare (QuickBooks kendi sayfasında) |
| 7 | `bolum-07-rapor` | **Bitti** — 10 sayfa, 16 kare |
| 8 | `bolum-08-veri` | **Bitti** — 9 sayfa, 14 kare |
| 9 | `bolum-09-belge` | **Bitti** — 12 sayfa, 6 kare (Paraşüt ve Logo karesi basılmaz) |
| 10 | `bolum-10-kapanis` | **Bitti** — 8 sayfa, kare yok (sentez bölümü) |
| — | `tam/` | Kapak · içindekiler · on bölüm · kanıt eki |

---

## Tarih yazılmaz

Koşum tarihleri belgeye girmez — okurun işi değil, üretim ayrıntısıdır. Kapakta da
üretim tarihi olmaz. **İstisna: verinin dönemi** (Ağustos 2026, Eylül 2026) kalır,
çünkü hangi ayın toplamından söz edildiğini belirler ve sayıyı değiştirir.
Ayrı bir koşumda ölçülen bir sonuç "ayrı bir koşumda uçtan uca izlendi" diye yazılır.
