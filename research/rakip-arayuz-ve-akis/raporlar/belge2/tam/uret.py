# -*- coding: utf-8 -*-
"""Belge 2'nin birlesik PDF'i: kapak, icindekiler, Bolum 1-10, kanit eki.

Her bolumun tek icerik kaynagi kendi klasorundeki icerik.py'dir; bu betik onlari
belge sirasinda ayni tuvale basar. Bolum klasorlerindeki tekil PDF'lere dokunulmaz.

Calistirma (raporlar/ icinden):
    .pilot-tools/venv/Scripts/python -X utf8 belge2/tam/uret.py

Cikti: belge2.pdf, belge2.md, kanit-eki.md, rapor.json, onizleme/
"""
from __future__ import annotations

import importlib.util
import json
import pathlib
import sys

BURASI = pathlib.Path(__file__).resolve().parent
BELGE2 = BURASI.parent
sys.path.insert(0, str(BELGE2 / "ortak"))
import b2  # noqa: E402
import kalip as K  # noqa: E402
import motor as M  # noqa: E402
import pymupdf  # noqa: E402

BOLUMLER = [
    "bolum-01-gelir-gider", "bolum-02-hesaplar", "bolum-03-kart", "bolum-04-borc-cari",
    "bolum-05-zaman", "bolum-06-siniflandirma", "bolum-07-rapor", "bolum-08-veri",
    "bolum-09-belge", "bolum-10-kapanis",
]
EK_SATIR = 11.2


def yukle(klasor: str):
    yol = BELGE2 / klasor / "icerik.py"
    spec = importlib.util.spec_from_file_location(f"b2_icerik_{klasor.replace('-', '_')}", yol)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


# --------------------------------------------------------------------------
def kapak(t: M.Tuval) -> None:
    t.ust_metin = ""
    t.yeni()
    x = M.KENAR + 20
    t.kutu(M.KENAR, 150, M.KENAR + 6, 340, dolgu=M.VURGU)
    t.yazi(x, 150, 600, "BusinessFinance · Rakip araştırması", boy=11, font="sgb", renk=M.VURGU)
    t.yazi(x, 172, 700, "Belge 2", boy=18, font="sgb", renk=M.SOLUK)
    t.yazi(x, 198, 700, "Rakip finansal akışlar", boy=34, font="sgb")
    t.yazi(x, 252, 580,
           "Dokuz finans ve ön muhasebe ürününde bir kayıt girildikten sonra ne oluyor: hangi "
           "bakiye değişiyor, hangi toplama giriyor, hangi dönemde sayılıyor ve ekranda ne "
           "yazıyor. Aynı para, dokuz ayrı sonuç.",
           boy=10.5, renk=M.SOLUK, satir=1.42)
    t.yazi(x, 330, 700, "Canlı koşulan beş ürün", boy=8.2, font="sgb", renk=M.VURGU)
    t.yazi(x, 344, 700, "Money Manager · Bluecoins · Wallet · Hesap Defterim · Goodbudget",
           boy=8.6, renk=M.SOLUK)
    t.yazi(x, 368, 700, "Kaynaktan incelenen dört ürün", boy=8.2, font="sgb", renk=M.VURGU)
    t.yazi(x, 382, 700, "KolayBi · Paraşüt · Logo İşbaşı · QuickBooks Solopreneur",
           boy=8.6, renk=M.SOLUK)
    t.yazi(x, 430, 720,
           "Her iddia bir kanıta bağlıdır ve kanıt düzeyi sayfada rozetle yazılıdır: ölçülen "
           "davranış, kareye alınmamış koşum gözlemi, kaynak görseli, kaynağın beyanı ve "
           "kanıttan çıkarılan neden.", boy=8.4, renk=M.SOLUK, satir=1.4)
    t.yazi(x, 486, 720,
           "Belge 1 arayüz yaklaşımlarını anlatır, Belge 3 BusinessFinance önerisini kurar. "
           "Bu belge ürünleri puanlamaz ve tavsiye etmez; karar Belge 3'e kalır.",
           boy=8.0, renk=M.COK_SOLUK, satir=1.4)
    t.ust_metin = b2.UST


def icindekiler(t: M.Tuval, bolumler, baslangic: dict, ek_sayfa: int) -> None:
    t.yeni("İçindekiler")
    t.yazi(M.KENAR, 46, 400, "İçindekiler", boy=20, font="sgb")
    t.cizgi(M.KENAR, 78, M.KENAR + 150, M.VURGU, 1.6)
    sol = M.KENAR
    sag = M.KENAR + M.ICERIK_W / 2 + 14
    w = M.ICERIK_W / 2 - 14
    y = {0: 96.0, 1: 96.0}
    for i, (klasor, C) in enumerate(bolumler):
        sutun = 0 if i < 5 else 1
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
    t.yazi(sag + 30, yy + 17, w - 70, "Basılan her karenin kimliği, ürünü, kanıt türü ve "
                                      "dosyası; basılmayan dayanaklar.", boy=7.2, renk=M.SOLUK)
    t.alt_bant("Bölümler tek başına da okunur; bölümler arası gönderme alt başlık numarasıyla "
               "yapılır (→ 3.7). Her bölüm bir akış ve bir çıkarım sayfasıyla kapanır.")


# --------------------------------------------------------------------------
YONETIM = [
    ("E0010", "Money Manager gözlem formu"), ("E0005", "Bluecoins gözlem formu"),
    ("E0014", "Wallet gözlem formu"), ("E0007", "Hesap Defterim gözlem formu"),
    ("E0006", "Goodbudget gözlem formu"), ("E0008", "KolayBi gözlem formu"),
    ("E0011", "Paraşüt gözlem formu"), ("E0009", "Logo İşbaşı gözlem formu"),
    ("E0012", "QuickBooks Solopreneur gözlem formu"),
]
BASILMAYAN = [
    ("E0375", "Wallet çekmecesinin üstü; hesap sahibinin adını taşıyor"),
    ("E0262", "Paraşüt tanıtım videosu karesi; ürünün iç arayüzü değil (→ 9.1)"),
    ("E0187", "KolayBi panosu; destek görseli değil, güncel arayüz yakalaması (→ 9.4)"),
]


def ek_satirlari(kanit: M.Kanit, sekiller: dict) -> list[tuple]:
    satirlar = []
    for eid in sorted(sekiller):
        tur = K.kare_turu(kanit, eid)
        yol = kanit.kayit[eid]["yol"].split("/", 1)[1]
        satirlar.append((eid, K.urun_adi(kanit.uygulama(eid)), tur, yol, ", ".join(sekiller[eid])))
    return satirlar


def kanit_eki(t: M.Tuval, satirlar, dogrulama_sayisi: int) -> int:
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
            giris = (f"Belgede basılan {len(satirlar)} kare. Kare, bir iddiayı taşıdığı yerde "
                     "basılır: bir sayının önce ve sonrası, bir formun sorduğu alan, ya da "
                     "ürünün kullanıcıya sorduğu bir soru. Her karenin hash'i üretim sırasında "
                     "kanıt envanteriyle karşılaştırıldı; özgün kareler değiştirilmedi ve "
                     "karartma yalnız basılan kopyalara uygulandı. Tablo hücrelerinin tek tek "
                     "dayanakları bölüm klasörlerindeki iddia tablolarındadır.")
            h = t.olc(M.ICERIK_W, giris, boy=8.6, satir=1.35)
            t.yazi(M.KENAR, y, M.ICERIK_W, giris, boy=8.6, satir=1.35, renk=M.SOLUK)
            y += h + 8
        t.kutu(M.KENAR, y - 2, M.KENAR + M.ICERIK_W, y + 11, dolgu=(0.95, 0.95, 0.96))
        for x, ad, w in kolon:
            t.yazi(x + 2, y, w, ad, boy=7.0, font="sgb", renk=M.SOLUK)
        return y + 15

    y = yeni_sayfa(True)
    for eid, urun, tur, yol, sek in satirlar:
        if y + EK_SATIR > K.ALT_SINIR:
            t.alt_bant("Kanıt türleri: Canlı kare, emülatör görüntüsü · Kaynak görseli, "
                       "ürünün destek sayfası.")
            y = yeni_sayfa(False)
        t.yazi(kolon[0][0] + 2, y, 44, eid, boy=6.8, font="sgb")
        t.yazi(kolon[1][0] + 2, y, 86, urun, boy=6.8)
        t.kutu(kolon[2][0] + 2, y + 0.5, kolon[2][0] + 58, y + 9, dolgu=K.TUR_RENK[tur])
        t.yazi(kolon[2][0] + 3, y + 1.4, 54, M.TURLER[tur], boy=5.6, font="sgb",
               renk=M.BEYAZ, hiza=1)
        t.yazi(kolon[3][0] + 2, y, 330, yol, boy=6.6, renk=M.SOLUK)
        t.yazi(kolon[4][0] + 2, y, 240, sek, boy=6.8)
        y += EK_SATIR
        t.cizgi(M.KENAR, y - 2, M.KENAR + M.ICERIK_W, kalin=0.3)
    gerek = 30 + EK_SATIR * (len(YONETIM) + len(BASILMAYAN) + 3)
    if y + gerek > K.ALT_SINIR:
        t.alt_bant("Kanıt türleri: Canlı kare, emülatör görüntüsü · Kaynak görseli, "
                   "ürünün destek sayfası.")
        sayfa += 1
        t.yeni("Kanıt eki")
        y = 46.0
    y += 14
    t.yazi(M.KENAR, y, 500, "Basılmayan dayanaklar", boy=11, font="sgb")
    y += 18
    t.yazi(M.KENAR, y, 760, "Gözlem formları: koşum kaydı ve kaynak beyanlarının tutulduğu "
                            "dosyalar. Çalışma boyunca güncellendikleri için hash denetimine "
                            "girmezler.", boy=7.4, font="sgb", renk=M.VURGU)
    y += EK_SATIR
    for eid, ad in YONETIM:
        t.yazi(M.KENAR + 2, y, 44, eid, boy=6.8, font="sgb")
        t.yazi(M.KENAR + 48, y, 300, ad, boy=6.8)
        t.yazi(M.KENAR + 250, y, 300, kanit_yolu(eid), boy=6.6, renk=M.SOLUK)
        y += EK_SATIR
    y += 4
    t.yazi(M.KENAR, y, 700, "Anılan ama basılmayan kareler", boy=7.4, font="sgb", renk=M.VURGU)
    y += EK_SATIR
    for eid, ad in BASILMAYAN:
        t.yazi(M.KENAR + 2, y, 60, eid, boy=6.8, font="sgb")
        t.yazi(M.KENAR + 64, y, 600, ad, boy=6.8)
        y += EK_SATIR
    if y > K.ALT_SINIR + 1:
        M.uyarilar.append(f"kanıt eki sayfa altına taştı ({y:.0f})")
    t.alt_bant(f"Hash denetimi: {dogrulama_sayisi} kimlik diskte ve envanterle aynı.")
    return sayfa


_KANIT = None


def kanit_yolu(eid: str) -> str:
    return _KANIT.kayit[eid]["yol"]


# --------------------------------------------------------------------------
def ciz_hepsi(belge, bolumler, nesneler, harita, baslangic, ek_sayfa, satirlar, dogrulama_sayisi):
    t = M.Tuval(belge, b2.UST)
    kapak(t)
    icindekiler(t, bolumler, baslangic, ek_sayfa)
    yeni_baslangic = {}
    yeni_harita = {}
    for (klasor, C), b in zip(bolumler, nesneler):
        yeni_baslangic[klasor] = t.no + 1
        b.ciz(t, sayfa_haritasi=harita.get(klasor))
        yeni_harita[klasor] = list(b.sayfa_no)
    ek_bas = t.no + 1
    kanit_eki(t, satirlar, dogrulama_sayisi)
    return yeni_baslangic, yeni_harita, ek_bas


# Bolum kapilari (G4/G4b/G7/G8) yalniz bolum uretiminde kosar; kapak, icindekiler ve
# kanit eki burada taranir. Kapak kendi urun adimizi tasiyabilir (kullanici karari,
# 23 Eylul 2026); kapakta G7'nin geri kalani gecerlidir.
KAPAKTA_SERBEST = ("BusinessFinance",)
KAPSAM_URUNLER = ("Money Manager", "Bluecoins", "Wallet", "Hesap Defterim", "Goodbudget",
                  "KolayBi", "Paraşüt", "Logo İşbaşı", "QuickBooks")


def ek_kapilar(pdf: pathlib.Path, bas: dict, ek: int) -> None:
    belge = pymupdf.open(str(pdf))
    parcalar = {"Kapak": [0], "İçindekiler": [1],
                "Kanıt eki": list(range(ek - 1, belge.page_count))}
    for ad, sayfalar in parcalar.items():
        metin = "\n".join(belge[i].get_text() for i in sayfalar).lower()
        for terim in b2.YASAK_TERIM:                                          # G4
            if terim.lower() in metin:
                raise b2.KapiHatasi(f"{ad}: iç terim '{terim}' belgeye sızmış (K5).")
        if "**" in metin:                                                     # G4b
            raise b2.KapiHatasi(f"{ad}: metinde '**' var; motor kalınlık basmaz.")
        for kavram in b2.YASAK_KAVRAM:                                        # G7
            if ad == "Kapak" and kavram in KAPAKTA_SERBEST:
                continue
            if kavram.lower() in metin:
                raise b2.KapiHatasi(f"{ad}: kendi kavramımız '{kavram}' metne girmiş (K12).")
    ilk = min(bas.values()) - 1
    govde = "\n".join(belge[i].get_text() for i in range(ilk, ek - 1))
    kapak = belge[0].get_text()
    for urun in KAPSAM_URUNLER:                                               # G8
        if urun in kapak and urun not in govde:
            raise b2.KapiHatasi(f"Kapak: '{urun}' kapsamda sayılmış ama bölümlerde hiç geçmiyor.")
    belge.close()
    print("Kapak, içindekiler ve kanıt eki kapıları temiz.")


def main() -> None:
    global _KANIT
    kanit = M.Kanit()
    _KANIT = kanit
    bolumler = [(k, yukle(k)) for k in BOLUMLER]
    gecici = BURASI / "_turevler"
    M.temizle(gecici)
    nesneler = [b2.Bolum(C, gecici / k, kanit) for k, C in bolumler]

    taslak = pymupdf.open()
    bas, har, ek = ciz_hepsi(taslak, bolumler, nesneler, {}, {k: 0 for k, _ in bolumler}, 0, [], 0)
    taslak.close()
    sekiller: dict[str, list[str]] = {}
    for b in nesneler:
        for no, eid, _a in b.sekil_listesi:
            sekiller.setdefault(eid, [])
            if no not in sekiller[eid]:
                sekiller[eid].append(no)
    satirlar = ek_satirlari(kanit, sekiller)
    ek_kanit = set(sekiller)
    for _k, C in bolumler:
        ek_kanit |= set(getattr(C, "EK_KANITLAR", []))
    dogrulama = kanit.dogrula(sorted(ek_kanit))

    taslak = pymupdf.open()
    bas, har, ek = ciz_hepsi(taslak, bolumler, nesneler, har, bas, ek, satirlar, len(dogrulama))
    taslak.close()
    M.uyarilar.clear()

    belge = pymupdf.open()
    bas2, har2, ek2 = ciz_hepsi(belge, bolumler, nesneler, har, bas, ek, satirlar, len(dogrulama))
    assert bas2 == bas and ek2 == ek, "sayfa haritası oturmadı"
    belge.set_metadata({"title": "Belge 2 · Rakip finansal akışlar", "author": "",
                        "subject": "Rakip finansal akış araştırması", "keywords": "rakip akış"})
    toc = [[1, "Kapak", 1], [1, "İçindekiler", 2]]
    toc += [[1, f"Bölüm {C.NO} · {C.BASLIK}", bas[k]] for k, C in bolumler]
    toc += [[1, "Kanıt eki", ek]]
    belge.set_toc(toc)
    pdf = BURASI / "belge2.pdf"
    belge.save(str(pdf), deflate=True, garbage=3)
    belge.close()
    ek_kapilar(pdf, bas, ek)

    parcalar = ["# Belge 2 · Rakip finansal akışlar\n\n"
                "> Birleşik okunabilir kopya: bölüm Markdown dosyaları sırayla ve kanıt eki. "
                "İşaretler, rozetler ve sayfa düzeni yalnız PDF'te görünür.\n"]
    for k, C in bolumler:
        md = BELGE2 / k / f"bolum-{int(C.NO):02d}.md"
        parcalar.append(md.read_text(encoding="utf-8"))
    ek_md = ["# Kanıt eki", "", "| Kimlik | Ürün | Tür | Dosya | Basıldığı şekiller |",
             "|---|---|---|---|---|"]
    ek_md += [f"| {e} | {u} | {M.TURLER[t]} | `{y}` | {s} |" for e, u, t, y, s in satirlar]
    ek_md += ["", "## Basılmayan dayanaklar", ""]
    ek_md += [f"- {e} · {a} · `{kanit_yolu(e)}`" for e, a in YONETIM]
    ek_md += [f"- {e} · {a}" for e, a in BASILMAYAN]
    (BURASI / "kanit-eki.md").write_text("\n".join(ek_md) + "\n", encoding="utf-8")
    parcalar.append("\n".join(ek_md))
    (BURASI / "belge2.md").write_text("\n\n---\n\n".join(parcalar) + "\n", encoding="utf-8")

    sonuc = M.kontrol(pdf, gizli_sozcukler=K.GIZLI)
    n = M.onizleme(pdf, BURASI / "onizleme", dpi=80)
    (BURASI / "rapor.json").write_text(json.dumps({
        "sayfa": sonuc["sayfa"], "bolum_baslangic": bas, "kanit_eki": ek,
        "basilan_kare": len(satirlar), "hash_denetimi": len(dogrulama),
        "kirik_atif": sonuc["kirik_atif"], "bozuk_karakter": sonuc["bozuk_karakter"],
        "metinde_gizli_sozcuk": sonuc["metinde_gizli_sozcuk"], "uyarilar": list(M.uyarilar),
    }, ensure_ascii=False, indent=2), encoding="utf-8")
    M.rapor_yaz("belge2", pdf, sonuc, dogrulama)
    print(f"Bölüm başlangıçları: {bas}")
    print(f"Kanıt eki: {ek} · Önizleme: {n} PNG")


if __name__ == "__main__":
    main()
