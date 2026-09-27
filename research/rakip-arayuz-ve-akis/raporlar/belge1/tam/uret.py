# -*- coding: utf-8 -*-
"""Belge 1'in birlesik PDF'i: kapak, icindekiler, Bolum 1-12, kanit eki.

Her bolumun tek icerik kaynagi kendi klasorundeki icerik.py'dir; bu betik onlari
sirayla ayni tuvale basar, sayfa numarasi belge boyunca surer. Bolum klasorlerindeki
tekil PDF'lere dokunulmaz; isaretli turevler bu klasorun altinda yeniden uretilir.
Bolumler b1.Bolum ile basilir (durum cubugu kesmesi, kart yuvasi); tam metin b1'in
metin kapilarindan gecer. Kapakta tarih yoktur (A1).

Calistirma (arastirma kokunden):
    raporlar/.pilot-tools/venv/Scripts/python raporlar/belge1/tam/uret.py

Cikti: belge1.pdf, belge1.md, kanit-eki.md, rapor.json, onizleme/
"""
from __future__ import annotations

import importlib.util
import json
import pathlib
import sys

BURASI = pathlib.Path(__file__).resolve().parent
BELGE1 = BURASI.parent
sys.path.insert(0, str(BELGE1 / "ortak"))
import b1  # noqa: E402
import kalip as K  # noqa: E402
import motor as M  # noqa: E402
import pymupdf  # noqa: E402
import re  # noqa: E402

BOLUMLER = [
    "bolum-01-giris", "bolum-02-ana-ekran", "bolum-03-gorsel-dil", "bolum-04-islem-ekleme",
    "bolum-05-hesap-kart-transfer", "bolum-06-siniflandirma", "bolum-07-plan-ve-bekleyen",
    "bolum-08-borc-ve-tahsilat", "bolum-09-rapor-ve-donem", "bolum-10-veri-aktarimi",
    "bolum-11-diger-moduller", "bolum-12-ortak-tercihler",
]
SOL_SUTUN = 6         # icindekiler: ilk alti bolum solda, kalanlar ve kanit eki sagda
EK_SATIR = 11.2       # kanit eki satir yuksekligi


def yukle(klasor: str):
    yol = BELGE1 / klasor / "icerik.py"
    spec = importlib.util.spec_from_file_location(f"icerik_{klasor.replace('-', '_')}", yol)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


# --------------------------------------------------------------------------
# Kapak ve icindekiler
# --------------------------------------------------------------------------
def kapak(t: M.Tuval) -> None:
    t.ust_metin = ""
    t.yeni()
    x = M.KENAR + 20
    t.kutu(M.KENAR, 150, M.KENAR + 6, 330, dolgu=M.VURGU)
    t.yazi(x, 150, 600, "BusinessFinance · Rakip araştırması", boy=11, font="sgb", renk=M.VURGU)
    t.yazi(x, 172, 700, "Belge 1", boy=18, font="sgb", renk=M.SOLUK)
    t.yazi(x, 198, 700, "Rakip arayüz yaklaşımları", boy=34, font="sgb")
    t.yazi(x, 250, 560,
           "Dokuz finans ve ön muhasebe ürününün ana ekran, kayıt formu, hesap ve kart, sınıflandırma, planlama, "
           "borç, rapor, veri aktarımı ve yardımcı araç yüzeyleri; ekranın üzerinden, kanıt türüyle.",
           boy=10.5, renk=M.SOLUK, satir=1.4)
    urunler = ("Canlı incelenen: Money Manager · Bluecoins · Wallet · Hesap Defterim · Goodbudget\n"
               "Kaynakla incelenen: KolayBi · Paraşüt · Logo İşbaşı · QuickBooks Solopreneur")
    t.yazi(x, 440, 700, urunler, boy=8.6, renk=M.SOLUK, satir=1.5)
    t.yazi(x, 480, 700, "Belge 2: finansal akışlar · Belge 3: BusinessFinance önerisi. Bu belge ürünleri "
                        "puanlamaz; tercih Belge 3'e kalır.", boy=8.0, renk=M.COK_SOLUK)
    t.ust_metin = K.UST


def icindekiler(t: M.Tuval, bolumler, baslangic: dict, ek_sayfa: int) -> None:
    t.yeni("İçindekiler")
    t.yazi(M.KENAR, 46, 400, "İçindekiler", boy=20, font="sgb")
    t.cizgi(M.KENAR, 78, M.KENAR + 150, M.VURGU, 1.6)
    sol = M.KENAR
    sag = M.KENAR + M.ICERIK_W / 2 + 14
    w = M.ICERIK_W / 2 - 14
    y = {0: 96.0, 1: 96.0}
    for i, (klasor, C) in enumerate(bolumler):
        sutun = 0 if i < SOL_SUTUN else 1
        x = sol if sutun == 0 else sag
        yy = y[sutun]
        t.yazi(x, yy, 30, str(C.NO), boy=12, font="sgb", renk=M.VURGU)
        t.yazi(x + 30, yy + 1, w - 70, C.BASLIK, boy=11, font="sgb")
        t.yazi(x + w - 34, yy + 1, 34, str(baslangic[klasor]), boy=10, font="sgb", hiza=2)
        yy += 16
        sorular = []
        for s in C.SAYFALAR:
            if s["tur"] != "acilis" and s.get("haritada", True) and s.get("no"):
                if (s["no"], s["baslik"]) not in sorular:
                    sorular.append((s["no"], s["baslik"]))
        metin = " · ".join(f"{no} {b}" for no, b in sorular)
        h = t.olc(w - 70, metin, boy=7.2, satir=1.3)
        t.yazi(x + 30, yy, w - 70, metin, boy=7.2, renk=M.SOLUK, satir=1.3)
        y[sutun] = yy + h + 10
    yy = y[1]
    t.yazi(sag + 30, yy + 1, w - 70, "Kanıt eki", boy=11, font="sgb")
    t.yazi(sag + w - 34, yy + 1, 34, str(ek_sayfa), boy=10, font="sgb", hiza=2)
    t.yazi(sag + 30, yy + 17, w - 70, "Basılan her karenin kimliği, ürünü, kanıt türü, dosyası ve "
                                      "basıldığı şekiller; basılmayan dayanaklar.", boy=7.2, renk=M.SOLUK)
    t.alt_bant("Bölümler tek başına da basılır; bölümler arası gönderme alt soru numarasıyla yapılır (→ 7.4).")


# --------------------------------------------------------------------------
# Kanit eki
# --------------------------------------------------------------------------
YONETIM = [
    ("E0010", "Money Manager gözlem formu"), ("E0005", "Bluecoins gözlem formu"),
    ("E0014", "Wallet gözlem formu"), ("E0007", "Hesap Defterim gözlem formu"),
    ("E0006", "Goodbudget gözlem formu"), ("E0008", "KolayBi gözlem formu"),
    ("E0011", "Paraşüt gözlem formu"), ("E0009", "Logo İşbaşı gözlem formu"),
    ("E0012", "QuickBooks Solopreneur gözlem formu"),
]
KIMLIK = re.compile(r"\bE\d{4}\b")


def anilan_kimlikler(bolumler) -> dict[str, list[int]]:
    """Her bolumun icerik.py metninde gecen E kimlikleri -> anildigi bolumler.
    EK_KANITLAR elle tutulan bir listedir ve eksik kalabilir; kaynak metnin kendisidir."""
    anilan: dict[str, list[int]] = {}
    for klasor, C in bolumler:
        metin = (BELGE1 / klasor / "icerik.py").read_text(encoding="utf-8")
        for eid in sorted(set(KIMLIK.findall(metin))):
            anilan.setdefault(eid, []).append(int(C.NO))
    return anilan


def anilan_turu(kanit: M.Kanit, eid: str) -> str:
    """Basilmayan karenin kanit turu. K.kare_turu basilan kare icindir ve basilmamasi gereken
    kareyi (KolayBi videosu, kaynak urun girisleri) durdurur; burada o kareler yalniz anilir.
    Tanitim, giris ve kayit kareleri ic arayuzu tek basina gostermez: Koşum kaydi."""
    uyg = kanit.uygulama(eid)
    if uyg == "kolaybi":
        return "kaynak" if "-destek-" in kanit.kayit[eid]["yol"] else "kosum"
    if uyg in K.BASILMAZ:
        return "kosum"
    return "canli"


def basilmayan_satirlari(kanit: M.Kanit, anilan: dict, basilan: set) -> list[tuple]:
    yonetim = {e for e, _a in YONETIM}
    satirlar = []
    for eid in sorted(anilan):
        if eid in basilan or eid in yonetim:
            continue
        yol = kanit.kayit[eid]["yol"].split("/", 1)[1]
        satirlar.append((eid, K.urun_adi(kanit.uygulama(eid)), anilan_turu(kanit, eid), yol,
                         ", ".join(str(n) for n in anilan[eid])))
    return satirlar


def ek_satirlari(kanit: M.Kanit, sekiller: dict) -> list[tuple]:
    satirlar = []
    for eid in sorted(sekiller):
        tur = K.kare_turu(kanit, eid)
        yol = kanit.kayit[eid]["yol"].split("/", 1)[1]
        satirlar.append((eid, K.urun_adi(kanit.uygulama(eid)), tur, yol, ", ".join(sekiller[eid])))
    return satirlar


def kanit_eki(t: M.Tuval, satirlar, basilmayan, dogrulama_sayisi: int) -> int:
    """Eki basar; basilan sayfa sayisini dondurur."""
    kolon = [(M.KENAR, "Kimlik", 44), (M.KENAR + 46, "Ürün", 86), (M.KENAR + 134, "Tür", 62),
             (M.KENAR + 200, "Dosya", 330), (M.KENAR + 534, "Basıldığı şekiller", 240)]
    sayfa = 0

    def yeni_sayfa(ilk: bool) -> float:
        nonlocal sayfa
        sayfa += 1
        t.yeni("Kanıt eki")
        y = 46.0
        if ilk:
            t.yazi(M.KENAR, y, 400, "Kanıt eki", boy=20, font="sgb")
            y += 30
            giris = (f"Belgede basılan {len(satirlar)} kare. Her karenin hash'i üretim sırasında kanıt envanteriyle "
                     "karşılaştırıldı; özgün kareler değiştirilmedi, işaret ve karartma yalnız kopyalara uygulandı. "
                     "\"Basıldığı şekiller\" bölüm içi şekil numarasıdır.")
            h = t.olc(M.ICERIK_W, giris, boy=8.6, satir=1.35)
            t.yazi(M.KENAR, y, M.ICERIK_W, giris, boy=8.6, satir=1.35, renk=M.SOLUK)
            y += h + 8
        return baslik_satiri(y, "Basıldığı şekiller")

    def baslik_satiri(y: float, son: str) -> float:
        t.kutu(M.KENAR, y - 2, M.KENAR + M.ICERIK_W, y + 11, dolgu=(0.95, 0.95, 0.96))
        for x, ad, w in kolon[:-1] + [(kolon[-1][0], son, kolon[-1][2])]:
            t.yazi(x + 2, y, w, ad, boy=7.0, font="sgb", renk=M.SOLUK)
        return y + 15

    ALT = "Kanıt türleri: Canlı kare, emülatör görüntüsü · Kaynak görseli, KolayBi destek sayfası."
    ALT_ANILAN = (ALT[:-1] + " · Koşum kaydı, iç arayüzü tek başına göstermeyen tanıtım, giriş veya "
                  "kayıt karesi.")
    ANILAN_SON = "Anıldığı bölümler"

    def satir_bas(y, eid, urun, tur, yol, son):
        t.yazi(kolon[0][0] + 2, y, 44, eid, boy=6.8, font="sgb")
        t.yazi(kolon[1][0] + 2, y, 86, urun, boy=6.8)
        t.kutu(kolon[2][0] + 2, y + 0.5, kolon[2][0] + 58, y + 9, dolgu=K.TUR_RENK[tur])
        t.yazi(kolon[2][0] + 3, y + 1.4, 54, M.TURLER[tur], boy=5.6, font="sgb", renk=M.BEYAZ, hiza=1)
        t.yazi(kolon[3][0] + 2, y, 330, yol, boy=6.6, renk=M.SOLUK)
        t.yazi(kolon[4][0] + 2, y, 240, son, boy=6.8)
        t.cizgi(M.KENAR, y + EK_SATIR - 2, M.KENAR + M.ICERIK_W, kalin=0.3)
        return y + EK_SATIR

    y = yeni_sayfa(True)
    for eid, urun, tur, yol, sek in satirlar:
        if y + EK_SATIR > K.ALT_SINIR:
            t.alt_bant(ALT)
            y = yeni_sayfa(False)
        y = satir_bas(y, eid, urun, tur, yol, sek)
    # basilmayan dayanaklar: gozlem formlari
    gerek = 30 + EK_SATIR * (len(YONETIM) + 4)
    if y + gerek > K.ALT_SINIR:
        t.alt_bant(ALT)
        sayfa += 1
        t.yeni("Kanıt eki")
        y = 46.0
    y += 14
    t.yazi(M.KENAR, y, 500, "Basılmayan dayanaklar", boy=11, font="sgb")
    y += 18
    t.yazi(M.KENAR, y, 760, "Gözlem formları: koşum kaydı ve kaynak beyanlarının kaydı. Çalışma boyunca "
                            "güncellenen belgeler olduğu için hash denetimine girmezler.", boy=7.4, font="sgb",
           renk=M.VURGU)
    y += EK_SATIR
    for eid, ad in YONETIM:
        t.yazi(M.KENAR + 2, y, 44, eid, boy=6.8, font="sgb")
        t.yazi(M.KENAR + 48, y, 300, ad, boy=6.8)
        t.yazi(M.KENAR + 250, y, 300, kanit_yolu(eid), boy=6.6, renk=M.SOLUK)
        y += EK_SATIR
    # anilan ama basilmayan kareler: metinden uretilir, hash denetimine girer.
    # Baslik altinda en az bes satir yoksa yeni sayfada baslar.
    if y + 6 + EK_SATIR * 2 + 15 + EK_SATIR * 5 > K.ALT_SINIR:
        t.alt_bant(ALT)
        sayfa += 1
        t.yeni("Kanıt eki")
        y = 40.0
    y += 6
    t.yazi(M.KENAR, y, 760, f"Anılan ama basılmayan {len(basilmayan)} kare. Metinde kimliğiyle anılır, "
                            "hash denetimine girer. Son sütun anıldığı bölümlerdir.", boy=7.4, font="sgb",
           renk=M.VURGU)
    y = baslik_satiri(y + EK_SATIR + 4, ANILAN_SON)
    for eid, urun, tur, yol, bol in basilmayan:
        if y + EK_SATIR > K.ALT_SINIR:
            t.alt_bant(ALT_ANILAN)
            sayfa += 1
            t.yeni("Kanıt eki")
            y = baslik_satiri(46.0, ANILAN_SON)
        y = satir_bas(y, eid, urun, tur, yol, "Bölüm " + bol)
    K.Bolum._tasma(None, t, y, "kanıt eki")
    t.alt_bant(f"Hash denetimi: {dogrulama_sayisi} kimlik diskte ve envanterle aynı. " + ALT_ANILAN)
    return sayfa


_KANIT = None


def kanit_yolu(eid: str) -> str:
    return _KANIT.kayit[eid]["yol"]


# --------------------------------------------------------------------------
# Uretim
# --------------------------------------------------------------------------
def ciz_hepsi(belge, bolumler, nesneler, harita, baslangic, ek_sayfa, satirlar, basilmayan, dogrulama_sayisi):
    t = M.Tuval(belge, K.UST)
    kapak(t)
    icindekiler(t, bolumler, baslangic, ek_sayfa)
    yeni_baslangic = {}
    yeni_harita = {}
    for (klasor, C), b in zip(bolumler, nesneler):
        yeni_baslangic[klasor] = t.no + 1
        b.ciz(t, sayfa_haritasi=harita.get(klasor))
        yeni_harita[klasor] = list(b.sayfa_no)
    ek_bas = t.no + 1
    kanit_eki(t, satirlar, basilmayan, dogrulama_sayisi)
    return yeni_baslangic, yeni_harita, ek_bas


def main() -> None:
    global _KANIT
    kanit = M.Kanit()
    _KANIT = kanit
    bolumler = [(k, yukle(k)) for k in BOLUMLER]
    for _k, C in bolumler:
        b1.hazirla(C)
    gecici = BURASI / "_turevler"
    M.temizle(gecici)
    nesneler = [b1.Bolum(C, gecici / k, kanit) for k, C in bolumler]

    # 1. gecis: sekil listesi ve sayfa haritasi
    taslak = pymupdf.open()
    bas, har, ek = ciz_hepsi(taslak, bolumler, nesneler, {}, {k: 0 for k, _ in bolumler}, 0, [], [], 0)
    taslak.close()
    sekiller: dict[str, list[str]] = {}
    for b in nesneler:
        for no, eid, _a in b.sekil_listesi:
            sekiller.setdefault(eid, [])
            if no not in sekiller[eid]:
                sekiller[eid].append(no)
    satirlar = ek_satirlari(kanit, sekiller)
    basilmayan = basilmayan_satirlari(kanit, anilan_kimlikler(bolumler), set(sekiller))
    ek_kanit = set(sekiller) | {e for e, *_r in basilmayan}
    for _k, C in bolumler:
        ek_kanit |= set(getattr(C, "EK_KANITLAR", []))
    dogrulama = kanit.dogrula(sorted(ek_kanit))

    # 2. gecis: eki gercek satirlarla bas, sayfa numaralarini oturt
    taslak = pymupdf.open()
    bas, har, ek = ciz_hepsi(taslak, bolumler, nesneler, har, bas, ek, satirlar, basilmayan, len(dogrulama))
    taslak.close()
    M.uyarilar.clear()

    belge = pymupdf.open()
    bas2, har2, ek2 = ciz_hepsi(belge, bolumler, nesneler, har, bas, ek, satirlar, basilmayan, len(dogrulama))
    assert bas2 == bas and ek2 == ek, "sayfa haritası oturmadı"
    belge.set_metadata({"title": "Belge 1 · Rakip arayüz yaklaşımları", "author": "",
                        "subject": "Rakip arayüz araştırması", "keywords": "rakip arayüz"})
    toc = [[1, "Kapak", 1], [1, "İçindekiler", 2]]
    toc += [[1, f"Bölüm {C.NO} · {C.BASLIK}", bas[k]] for k, C in bolumler]
    toc += [[1, "Kanıt eki", ek]]
    belge.set_toc(toc)
    pdf = BURASI / "belge1.pdf"
    belge.save(str(pdf), deflate=True, garbage=3)
    belge.close()
    # Kapilar: bolumler tek tek gecti; kapak, icindekiler ve ek dahil tam metin bir kez daha.
    d = pymupdf.open(str(pdf))
    b1.metin_kapilari("Tam belge", "\n".join(p.get_text() for p in d))
    d.close()

    # Markdown kopyasi ve ek
    parcalar = ["# Belge 1 · Rakip arayüz yaklaşımları\n\n"
                "> Birleşik okunabilir kopya: bölüm Markdown dosyaları sırayla ve kanıt eki. "
                "İşaretler ve sayfa düzeni yalnız PDF'te görünür.\n"]
    for k, C in bolumler:
        md = BELGE1 / k / f"bolum-{int(C.NO):02d}.md"
        parcalar.append(md.read_text(encoding="utf-8"))
    ek_md = ["# Kanıt eki", "", "| Kimlik | Ürün | Tür | Dosya | Basıldığı şekiller |", "|---|---|---|---|---|"]
    ek_md += [f"| {e} | {u} | {M.TURLER[t]} | `{y}` | {s} |" for e, u, t, y, s in satirlar]
    ek_md += ["", "## Basılmayan dayanaklar", ""]
    ek_md += [f"- {e} · {a} · `{kanit_yolu(e)}`" for e, a in YONETIM]
    ek_md += ["", f"### Anılan ama basılmayan {len(basilmayan)} kare", "",
              "| Kimlik | Ürün | Tür | Dosya | Anıldığı bölümler |", "|---|---|---|---|---|"]
    ek_md += [f"| {e} | {u} | {M.TURLER[t]} | `{y}` | {b} |" for e, u, t, y, b in basilmayan]
    (BURASI / "kanit-eki.md").write_text("\n".join(ek_md) + "\n", encoding="utf-8")
    parcalar.append("\n".join(ek_md))
    (BURASI / "belge1.md").write_text("\n\n---\n\n".join(parcalar) + "\n", encoding="utf-8")

    sonuc = M.kontrol(pdf, gizli_sozcukler=K.GIZLI)
    n = M.onizleme(pdf, BURASI / "onizleme", dpi=80)
    (BURASI / "rapor.json").write_text(json.dumps({
        "sayfa": sonuc["sayfa"], "bolum_baslangic": bas, "kanit_eki": ek,
        "basilan_kare": len(satirlar), "anilan_basilmayan": len(basilmayan), "hash_denetimi": len(dogrulama),
        "kirik_atif": sonuc["kirik_atif"], "bozuk_karakter": sonuc["bozuk_karakter"],
        "metinde_gizli_sozcuk": sonuc["metinde_gizli_sozcuk"], "uyarilar": list(M.uyarilar),
    }, ensure_ascii=False, indent=2), encoding="utf-8")
    M.rapor_yaz("belge1", pdf, sonuc, dogrulama)
    print(f"Bölüm başlangıçları: {bas} · Kanıt eki: {ek}")
    print(f"Önizleme: {n} PNG")


if __name__ == "__main__":
    main()
