# Belge 1 — Rakip arayüz yaklaşımları

**24 Eylül 2026 · Düzeltme turu uygulandı: 12 bölüm, kanıt eki ve birleşik PDF (`tam/belge1.pdf`,
97 sayfa, 154 basılı kare). Sıradaki adım kullanıcı incelemesidir.**
Turun planı ve sonucu: [`../belge1-duzeltme-plani.md`](../belge1-duzeltme-plani.md) (§12) ·
nerede kalındığı: [`../devir-2026-09-24-belge1-asama3.md`](../devir-2026-09-24-belge1-asama3.md).
17 Eylül'ün 13 bölümlük düzeni tarihseldir: [`butun-harita.md`](butun-harita.md) §6.

**Numara notu:** eski Bölüm 1 (okuma kılavuzu) ve 2 (ürün kimliği) yeni Bölüm 1'de birleşti; eski
3–13 bugün 2–12'dir. Aşağıda anılan `belge1-bolum-plani.md` eski numaraları kullanır.

Bu klasör Belge 1'in üretim yeridir. Yeni bir oturum buradan başlar.

---

## Önce bunu oku

| Sıra | Dosya | Ne için |
|---|---|---|
| 1 | [`../belge1-bolum-plani.md`](../belge1-bolum-plani.md) | **Tek başvuru kaynağı.** 13 bölüm, alt sorular, ürün–kanıt eşlemesi, bölüm sınırları, biçim kuralları, kararlar |
| 2 | [`butun-harita.md`](butun-harita.md) | Belge düzeyi kararlar: ortak sözlük, ayrıntının sahibi, karelerin bölümlere dağılımı, **ilerleme ve devir notu** |
| 3 | `ortak/motor.py` ve `ortak/kalip.py` başlıkları | Üretim motoru ve sayfa kalıpları |
| 4 | [`../../BULGU-DOGRULAMA-KAYDI.md`](../../BULGU-DOGRULAMA-KAYDI.md) `P4-tema-01`–`10` | Bölümlerin içerik kaynağı; her tema bir veya iki bölümü besler |
| 5 | [`../../gozlemler/`](../../gozlemler/) | Ürün başına ham gözlem formları; temada olmayan ayrıntı buradadır |

Plan ile bu README çelişirse **plan geçerlidir.**

---

## Klasör düzeni

```
belge1/
  ortak/                          üretim altyapısı (bütün bölümler paylaşır)
    motor.py                      sayfa tuvali, işaretleme, denetim, otomatik kapılar (Belge 2 ile ortak)
    kalip.py                      sayfa kalıpları (A–E, açılış, metin) ve bölüm üretimi (Belge 2 ile ortak)
    b1.py                         Belge 1 katmanı: durum çubuğu kesmesi, kart yuvası, G4/G7/G8/A1
                                  kapıları (ihlalde üretim durur). Ortak motora dokunmadan
    araclar/                      turun araçları: duzenle.py (eski/yeni çiftleriyle icerik.py
                                  düzenleme), kaydir.py (bölüm numarası kaydırma)
    izgara.py                     koordinat seçimi için ızgaralı kare basar (çıktı geçici klasöre)
    motor-testi.py                duman testi — ortamın çalıştığını doğrular
    kanit-dizini.json             461 kare + 35 yönetim dosyası: kimlik/yol/hash/içerik açıklaması
    kanit-dizini-uret.py          dizini envanterden yeniden üretir

  bolum-NN-<slug>/                her bölüm kendi klasörü (01–12; 01 = bolum-01-giris)
    icerik.py                     bölümün TEK içerik kaynağı (yalnız veri); EK_KANITLAR basılmayan
                                  ama anılan kareleri hash denetimine sokar
    uret.py                       üç satırlık başlatıcı; b1.calistir(__file__)
    bolum-NN.pdf / .md            çıktı ve okunabilir kopya
    iddia-tablosu.md              betiğin ürettiği denetim
    eksik-listesi.md              betiğin ürettiği denetim
    kaynaklar.md · kanit-manifest.json
    isaretli/                     işaretlenmiş türevler
    onizleme/                     sayfa PNG'leri (gözle inceleme için)

  tam/                            birleşik belge
    uret.py                       kapak, içindekiler, Bölüm 1–12, kanıt eki; sayfa numarası sürekli.
                                  Bölümleri b1.Bolum ile basar; "anılan ama basılmayan" listesini
                                  icerik.py metinlerindeki bütün E kimliklerinden üretir
    belge1.pdf / belge1.md        çıktı ve okunabilir kopya
    kanit-eki.md · rapor.json     basılan ve anılan karelerin dökümü; üretim raporu
    onizleme/                     sayfa PNG'leri
    _turevler/                    birleşik belge için işaretli kopyalar (geçici)

  _arsiv/                         kullanılmayan klasörler (silinmedi)
    bolum-01-okuma-kilavuzu/      17 Eylül Bölüm 1; yeni Bölüm 1'e birleşti
    bolum-02-urun-kimligi/        17 Eylül Bölüm 2; yeni Bölüm 1'e birleşti
    deneme-bolum-03-*/            ⚠ üç biçim denemesi — nihai bölüm değil
```

### Üç Bölüm 3 denemesi hakkında

Bunlar **biçim denemeleridir**, Belge 1'in bölümü değildir. Silinmediler; düzeltme turunda
`_arsiv/deneme-bolum-03-*` adıyla taşındılar ki yeni `bolum-02-ana-ekran` (eski Bölüm 3) ile
karışmasınlar. Aşağıdaki tablo eski adları kullanır.

| Klasör | Yaklaşım | Sonuç |
|---|---|---|
| `bolum-03-gezinme-ve-ana-ekran` | Soru başına 3 sütunlu tablo, görsel altta | Tablo-önce; kullanıcı benimsemedi |
| `bolum-03-bagimsiz` | Aynı eksen, "çıkarılmayan sonuç" sütunlu iddia tablosu | Denetim fikri iyi; anlatım hâlâ tablo-önce |
| `bolum-03-isaretli-ekran` | **Ekran büyük, üstünde numaralı işaret; tablo ek** | Yön kabul edildi. `ortak/motor.py` bunun motorundan çıkarıldı |

Üçüncüsüne gelen geri bildirim plana işlendi: ölçü şeridi ve yüzde blokları kalktı, ilk açılış
öne alındı, çekmece sayfasına Bluecoins eklendi, bekleyen işler Bölüm 8'e taşındı, 3.2 ile 3.3
birleşti. Ayrıntı: plan §8.

---

## Bir bölüm nasıl üretilir

```bash
# 0) ortam çalışıyor mu (bir kez yeter)
raporlar/.pilot-tools/venv/Scripts/python -X utf8 raporlar/belge1/ortak/motor-testi.py

# 1) kanıt dizini güncel mi (envanter değiştiyse)
raporlar/.pilot-tools/venv/Scripts/python -X utf8 raporlar/belge1/ortak/kanit-dizini-uret.py

# 2) bölümü üret
raporlar/.pilot-tools/venv/Scripts/python -X utf8 raporlar/belge1/bolum-NN-<slug>/uret.py

# 3) birleşik belge (bölüm Markdown kopyaları önce güncel olmalı)
raporlar/.pilot-tools/venv/Scripts/python -X utf8 raporlar/belge1/tam/uret.py
```

`uret.py` içinden `sys.path`'e `ortak/` eklenip `b1.calistir(__file__)` çağrılır; `b1` kalıbı ve motoru
kullanır. Birleşik belge iki dakikayı geçer; arka planda çalıştırılır.

### Kanıt nasıl bulunur

Tema kayıtları kareleri **dosya adıyla** anıyor, belgeler **E kimliğiyle** atıf yapmak zorunda.
Çeviri ve içerik araması `kanit-dizini.json` üzerinden:

```python
k = M.Kanit()
k.kimlik("money-manager/03-dolu-ana-ekran.png")   # -> "E0228"
k.aciklama("E0228")        # envanterdeki içerik açıklaması
k.dogrula(["E0228", ...])  # diskte var mı + hash envanterle aynı mı
```

461 karenin **hepsinde** içerik açıklaması var (24 Eylül). Hangi karenin ne gösterdiğini bulmak için
envanteri baştan okumak gerekmez. **Ama açıklama kareye dayanak değildir:** yeni bir kareye dayanmadan
önce kare açılır (ortak listenin özeti iki turda da kareden fazlasını söyledi).

---

## Üretim ilkeleri — plandan özet

Tamamı ve gerekçesi planda; burada yalnız her gün lazım olanlar.

- **Ekran argümandır, tablo ektir.** Kare büyük basılır, üstüne numaralı işaret konur, metin o
  numaralara konuşur. Dayanak sayfanın altına iner
- **İşaret gösterdiği şeyin üstünü kapatmaz.** Koordinat boş alana konur
- **Kanıt niteliği ürüne değil ifadeye bağlıdır.** Beş tür: canlı kare · koşum kaydı · kaynak
  görseli · kaynak beyanı · görülmedi
- **"Görülmedi" ile "yok" ayrı şeylerdir**
- **Kazandırdığı/Bedeli kalıbı yok.** Ürün tercihi Belge 3'e kalır
- **Nicelik belirteci** ("her ekranda", "tek ürün", "hiçbirinde") kullanılacaksa o kapsamı taşıyan
  kanıt gösterilir; gösterilemiyorsa cümle gözlenen kapsamla sınırlanır
- **Paraşüt, Logo İşbaşı ve QuickBooks'un karesi basılmaz**; gözlemleri ve kaynak beyanları
  metinde kalır. KolayBi'nin destek görselleri basılır, video kareleri basılmaz ama anılır
- **Bir ürün, bir soruda kanıtı varken listeden düşmez**
- **Ayrıntının tek sahibi olur**; başka bölümde kısa bağlam ve gönderme serbesttir
- **Tarih yazılmaz** (koşum ve üretim tarihi; kapak dahil). **"Çıkarılmayan sonuç" satırı yok**:
  finansal sonuç Belge 2'nin alt bölümüne gönderilir, ölçülmeyen etki girişte tek cümledir
- **Telefon karesinin durum çubuğu kesilir** (`b1.py`); işaret koordinatları özgün kare uzayında
  yazılır, motor kaydırır

---

## Bölüm ne zaman biter

1. Alt soruların hepsi yazıldı; kanıtı olmayan ürünler için "görülmedi" yazıldı
2. `iddia-tablosu.md` ve `eksik-listesi.md` üretildi
3. `M.kontrol()` beş kapısı temiz — kırık şekil göndermesi, bozuk karakter, sığmayan metin,
   PDF metninde kişisel ad, kanıt hash'i — ve `b1` kapıları temiz: kenara taşan işaret, üretim dili
   (G4), kendi kavramlarımız (G7), açılış kutusu (G8), tarih (A1). **G8'in kuralı:** kutu yalnız kanıtı listeler.
   Bir ürünün kutudaki rozetleri, gövdede o ürün için kullanılan kanıt türleridir (basılan kareler,
   "Aynı soruda" satırları, tablo hücreleri, kart alanları); Görülmedi kutuda yalnız ürünün bölümde hiç
   kanıtı yoksa görünür. Koşum kaydı ya da Görülmedi taşıyan üründe altında kısa bir not vardır.
   **"Görülmedi" yazım kuralı:** bölüm çapındaki ortak yokluk girişte bir kez yazılır, satır ya da sütun
   olarak tekrarlanmaz; Bölüm 11'in modül matrisleri Görülmedi rozetini korur (kullanıcı kararı); özet tablosunda yokluk hücresi rozetsiz düz
   metindir; aynı cümleyi taşıyan ürünler tek satırda birleşir
4. **Bütün sayfalar `onizleme/` üzerinden tek tek gözle incelendi**
5. "Bu bölüme girmez" listesindeki hiçbir konu bölüme sızmadı

> **Motorun yapamadığı iki şey var ve bu yüzden 4. adım atlanamaz:** iddianın kanıttan gerçekten
> çıktığını doğrulayamaz, ve **görselin içine gömülü kişisel adı yakalayamaz** — metin araması
> yalnız PDF metnine bakar.

---

## Yazım sırası (17 Eylül, eski numaralar; tarihsel)

**3 → 5 → 6** (ağırlar önce; biçim onlarda oturur) → 2 → 4 → 7 → 8 → 9 → 10 → 11 → 12 →
**1 → 13** → Kanıt eki.

Okuma kılavuzu (1) ve ortak tercihler (13) en sona kalır; ikisi de diğer bölümler bitmeden
yazılamaz.

---

## Sınırlar

- Bu çalışma **hiçbir aşamayı açmaz** ve uygulama kodunu değiştirmez
- **Mevcut test verisi silinmez veya sıfırlanmaz**; yeni kurulum gerektiren eksikler
  "Önerilmez" olarak işaretlenir
- Yeni emülatör koşumu kullanıcı kararıdır; agent kendiliğinden canlı test başlatmaz
- Özgün kareler değiştirilmez; karartma ve işaret yalnız bölüm kopyalarına uygulanır
