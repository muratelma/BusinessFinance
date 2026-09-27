# -*- coding: utf-8 -*-
"""Belge 1 - Bolum 3 - isaretli ekran anlatimi: PDF ve Markdown ureticisi.

Calistirma (raporlar/ icinden):
    .pilot-tools/venv/Scripts/python belge1/bolum-03-isaretli-ekran/uret.py

Adimlar:
  1. Ozgun kanitlarin SHA-256 degerlerini KANIT-ENVANTERI.md ile karsilastirir.
  2. Ozgun kopyalari kanit/ altina yazar (gizlilik karartmasi gerekenler ayrica
     isaretlenir; karartilmis kopyanin ozgunden farkli oldugu manifestte yazar).
  3. isaretli/ altinda numarali isaret tasiyan turevleri uretir.
  4. PDF'i dogrudan PyMuPDF ile cizer (Word turu yok).
  5. Ayni icerikten okunabilir bir Markdown kopyasi yazar.
  6. kaynaklar.md ve kanit-manifest.json dosyalarini yeniden yazar.
"""
from __future__ import annotations

import hashlib
import json
import re
import shutil
import sys
from pathlib import Path

import pymupdf
from PIL import Image, ImageDraw, ImageFont

sys.path.insert(0, str(Path(__file__).resolve().parent))
import icerik as C  # noqa: E402

# --------------------------------------------------------------------------
# Yollar
# --------------------------------------------------------------------------
BURASI = Path(__file__).resolve().parent
ARASTIRMA = BURASI.parents[2]            # research/rakip-arayuz-ve-akis
KANITLAR = ARASTIRMA / "kanitlar"
ENVANTER = ARASTIRMA / "KANIT-ENVANTERI.md"

KANIT_DIZIN = BURASI / "kanit"
ISARETLI_DIZIN = BURASI / "isaretli"
PDF_YOLU = BURASI / "bolum-03-isaretli-ekran.pdf"
MD_YOLU = BURASI / "bolum-03-isaretli-ekran.md"

FONT_DIZIN = Path("C:/Windows/Fonts")
F_DUZ = FONT_DIZIN / "segoeui.ttf"
F_KALIN = FONT_DIZIN / "segoeuib.ttf"
F_ISARET = FONT_DIZIN / "arialbd.ttf"

# --------------------------------------------------------------------------
# Renk ve olcu
# --------------------------------------------------------------------------
MUREKKEP = (0.08, 0.09, 0.12)
SOLUK = (0.36, 0.39, 0.45)
COK_SOLUK = (0.55, 0.58, 0.63)
CIZGI = (0.85, 0.87, 0.90)
VURGU = (0.70, 0.25, 0.12)
VURGU_RGB = (179, 64, 31)

SAYFA_W, SAYFA_H = 842.0, 595.0       # A4 yatay
KENAR = 34.0
ICERIK_W = SAYFA_W - 2 * KENAR

TELEFON_ORAN = 2400 / 1080

_uyarilar: list[str] = []

# Olcum icin ayri belge: cikti belgesinin sayfa yapisina hic dokunmaz.
_OLCU_BELGE = pymupdf.open()
_OLCU_SAYFA = None


def _olcu_sayfa() -> pymupdf.Page:
    global _OLCU_SAYFA
    if _OLCU_SAYFA is None:
        _OLCU_SAYFA = _OLCU_BELGE.new_page(width=1200, height=4000)
        _OLCU_SAYFA.insert_font(fontname="sg", fontfile=str(F_DUZ))
        _OLCU_SAYFA.insert_font(fontname="sgb", fontfile=str(F_KALIN))
    return _OLCU_SAYFA


# --------------------------------------------------------------------------
# Yardimcilar
# --------------------------------------------------------------------------
def sha256(yol: Path) -> str:
    h = hashlib.sha256()
    with open(yol, "rb") as f:
        for blok in iter(lambda: f.read(1 << 20), b""):
            h.update(blok)
    return h.hexdigest()


def envanter_hashleri() -> dict[str, str]:
    """KANIT-ENVANTERI.md icindeki | E0123 | [yol](yol) | boyut | `hash` | satirlari."""
    metin = ENVANTER.read_text(encoding="utf-8")
    desen = re.compile(r"^\|\s*(E\d{4})\s*\|[^|]*\|\s*\d+\s*\|\s*`([0-9a-f]{64})`\s*\|", re.M)
    return {m.group(1): m.group(2) for m in desen.finditer(metin)}


def kanit_yolu(eid: str) -> Path:
    return KANITLAR / C.KANITLAR[eid][0]


# --------------------------------------------------------------------------
# Gorsel isleme
# --------------------------------------------------------------------------
def _isaret_fontu(boy: int) -> ImageFont.FreeTypeFont:
    return ImageFont.truetype(str(F_ISARET), boy)


def isaretle(eid: str, isaretler, ek: str = "") -> Path:
    """Kareyi acar, varsa kirpar, gizlilik karartmasini ve isaretleri cizer.

    Isaret koordinatlari kirpma sonrasi uzaydadir; kirpilan kareler icin
    icerik.py'deki degerler buna gore yazilmistir.
    """
    kaynak = kanit_yolu(eid)
    im = Image.open(kaynak).convert("RGBA")
    if eid in getattr(C, "KIRPMA", {}):
        im = im.crop(C.KIRPMA[eid])
    ciz = ImageDraw.Draw(im)

    for kutu in C.KARARTMA.get(eid, []):
        ciz.rectangle(kutu, fill=(120, 124, 130, 255))

    # isaret capi karenin genisligine gore: PDF'te ~13 pt gorunsun
    cap = max(52, int(im.width * 0.082))
    if im.width > 1200:                      # genis masaustu kareleri
        cap = max(34, int(im.width * 0.030))
    r = cap // 2
    font = _isaret_fontu(int(cap * 0.62))

    for no, x, y, _metin in isaretler:
        ciz.ellipse((x - r - 4, y - r - 4, x + r + 4, y + r + 4), fill=(255, 255, 255, 255))
        ciz.ellipse((x - r, y - r, x + r, y + r), fill=VURGU_RGB + (255,))
        ciz.text((x, y + 1), str(no), font=font, fill=(255, 255, 255, 255), anchor="mm")

    ISARETLI_DIZIN.mkdir(parents=True, exist_ok=True)
    hedef = ISARETLI_DIZIN / f"{eid}{ek}.png"
    im.convert("RGB").save(hedef, "PNG")
    return hedef


def kirp(eid: str, kutu, ek: str) -> Path:
    im = Image.open(kanit_yolu(eid)).convert("RGBA")
    for k in C.KARARTMA.get(eid, []):
        ImageDraw.Draw(im).rectangle(k, fill=(120, 124, 130, 255))
    ISARETLI_DIZIN.mkdir(parents=True, exist_ok=True)
    hedef = ISARETLI_DIZIN / f"{eid}{ek}.png"
    im.crop(kutu).convert("RGB").save(hedef, "PNG")
    return hedef


def sade_kopya(eid: str) -> Path:
    """Isaretsiz ama gerekiyorsa karartilmis kopya (bolusum sayfasi icin)."""
    im = Image.open(kanit_yolu(eid)).convert("RGBA")
    for k in C.KARARTMA.get(eid, []):
        ImageDraw.Draw(im).rectangle(k, fill=(120, 124, 130, 255))
    ISARETLI_DIZIN.mkdir(parents=True, exist_ok=True)
    hedef = ISARETLI_DIZIN / f"{eid}-sade.png"
    im.convert("RGB").save(hedef, "PNG")
    return hedef


# --------------------------------------------------------------------------
# PDF cizim yardimcilari
# --------------------------------------------------------------------------
class Tuval:
    def __init__(self, belge: pymupdf.Document):
        self.belge = belge
        self.sayfa: pymupdf.Page | None = None
        self.no = 0

    def yeni(self) -> pymupdf.Page:
        self.sayfa = self.belge.new_page(width=SAYFA_W, height=SAYFA_H)
        self.sayfa.insert_font(fontname="sg", fontfile=str(F_DUZ))
        self.sayfa.insert_font(fontname="sgb", fontfile=str(F_KALIN))
        self.no += 1
        return self.sayfa

    # -- metin -------------------------------------------------------------
    def yazi(self, x, y, w, metin, *, boy=9.0, font="sg", renk=MUREKKEP,
             hiza=0, satir=1.30, etiket=""):
        h = 600.0
        kutu = pymupdf.Rect(x, y, x + w, y + h)
        art = self.sayfa.insert_textbox(kutu, metin, fontname=font, fontsize=boy,
                                        color=renk, align=hiza, lineheight=satir)
        if art < 0:
            _uyarilar.append(f"sayfa {self.no}: metin sigmadi ({etiket or metin[:40]!r}), eksik {art:.0f} pt")
        # gercek yuksekligi olc
        kul = h - art if art >= 0 else h
        return kul

    def olc(self, w, metin, *, boy=9.0, font="sg", satir=1.30) -> float:
        """Metnin w genisliginde kaplayacagi yukseklik.

        Olcum ayri bir belgede yapilir: cikti belgesine sayfa ekleyip silmek
        acik sayfa nesnesini gecersiz kilar.
        """
        art = _olcu_sayfa().insert_textbox(
            pymupdf.Rect(0, 0, w, 4000), metin, fontname=font, fontsize=boy,
            color=(0, 0, 0), lineheight=satir)
        return 4000 - art if art >= 0 else 4000

    # -- sekil -------------------------------------------------------------
    def cizgi(self, x0, y, x1, renk=CIZGI, kalin=0.6):
        self.sayfa.draw_line(pymupdf.Point(x0, y), pymupdf.Point(x1, y), color=renk, width=kalin)

    def kutu(self, x0, y0, x1, y1, *, dolgu=None, kenar=None, kalin=0.6):
        self.sayfa.draw_rect(pymupdf.Rect(x0, y0, x1, y1), color=kenar, fill=dolgu, width=kalin)

    def resim(self, yol: Path, x, y, w, h):
        self.sayfa.insert_image(pymupdf.Rect(x, y, x + w, y + h), filename=str(yol))
        self.sayfa.draw_rect(pymupdf.Rect(x, y, x + w, y + h), color=CIZGI, width=0.5)

    # -- sayfa iskeleti ----------------------------------------------------
    def ust_bant(self, sol: str, sag: str = ""):
        self.yazi(KENAR, 22, ICERIK_W * 0.7, sol, boy=7.5, renk=COK_SOLUK)
        if sag:
            self.yazi(KENAR + ICERIK_W * 0.7, 22, ICERIK_W * 0.3, sag, boy=7.5,
                      renk=COK_SOLUK, hiza=2)
        self.cizgi(KENAR, 36, SAYFA_W - KENAR)

    def alt_bant(self, dayanak: str = ""):
        self.cizgi(KENAR, SAYFA_H - 30, SAYFA_W - KENAR)
        if dayanak:
            self.yazi(KENAR, SAYFA_H - 26, ICERIK_W - 40, dayanak, boy=6.8, renk=COK_SOLUK,
                      etiket="dayanak")
        self.yazi(SAYFA_W - KENAR - 30, SAYFA_H - 26, 30, str(self.no), boy=7.5,
                  renk=COK_SOLUK, hiza=2)

    def baslik(self, no: str, metin: str, y=46) -> float:
        if no:
            self.yazi(KENAR, y, 26, no, boy=17, font="sgb", renk=VURGU)
            self.yazi(KENAR + 28, y + 2, ICERIK_W - 28, metin, boy=15, font="sgb")
        else:
            self.yazi(KENAR, y, ICERIK_W, metin, boy=15, font="sgb")
        return y + 26


# --------------------------------------------------------------------------
# Numarali not listesi
# --------------------------------------------------------------------------
def notlar_ciz(t: Tuval, x, y, w, isaretler, *, boy=8.2) -> float:
    for no, _x, _y, metin in isaretler:
        t.sayfa.draw_circle(pymupdf.Point(x + 5.5, y + 5.0), 5.5, color=None, fill=VURGU)
        t.yazi(x, y + 1.2, 11, str(no), boy=6.6, font="sgb", renk=(1, 1, 1), hiza=1)
        h = t.olc(w - 16, metin, boy=boy)
        t.yazi(x + 16, y, w - 16, metin, boy=boy, renk=MUREKKEP)
        y += max(h, 12.5) + 3.2
    return y


def madde_ciz(t: Tuval, x, y, w, maddeler, *, boy=8.2, renk=MUREKKEP) -> float:
    for m in maddeler:
        t.sayfa.draw_circle(pymupdf.Point(x + 2.4, y + 4.6), 1.5, color=None, fill=COK_SOLUK)
        h = t.olc(w - 10, m, boy=boy)
        t.yazi(x + 10, y, w - 10, m, boy=boy, renk=renk)
        y += h + 4.0
    return y


# --------------------------------------------------------------------------
# SAYFA 1 - kapak
# --------------------------------------------------------------------------
def sayfa_kapak(t: Tuval):
    t.yeni()
    t.ust_bant(C.UST_BASLIK, C.ALT_BASLIK)

    t.yazi(KENAR, 62, ICERIK_W, C.BASLIK, boy=30, font="sgb")
    t.cizgi(KENAR, 112, KENAR + 150, VURGU, 1.6)

    sol_w = 400.0
    y = 130.0
    for p in C.GIRIS:
        h = t.olc(sol_w, p, boy=9.4, satir=1.42)
        t.yazi(KENAR, y, sol_w, p, boy=9.4, satir=1.42)
        y += h + 9

    y += 6
    t.yazi(KENAR, y, sol_w, "Bu bölüm neyi ölçmez", boy=9, font="sgb", renk=VURGU)
    y += 14
    t.yazi(KENAR, y, sol_w, C.OLCMEZ, boy=8.6, renk=SOLUK, satir=1.40)

    # sag sutun
    sx = KENAR + sol_w + 44
    sw = ICERIK_W - sol_w - 44
    y = 130.0
    t.yazi(sx, y, sw, "Kanıt anahtarı", boy=9, font="sgb")
    y += 16
    for kod, ad, aciklama in C.ROZETLER:
        renk = {"canli": (0.12, 0.44, 0.70), "kayit": (0.30, 0.42, 0.55),
                "kaynak": (0.55, 0.42, 0.18), "cizim": (0.62, 0.30, 0.30),
                "yok": (0.55, 0.57, 0.60)}[kod]
        gw = t.olc(58, ad, boy=7.4, font="sgb")
        t.kutu(sx, y, sx + 62, y + 12.5, dolgu=renk, kenar=None)
        t.yazi(sx + 3, y + 2.4, 56, ad, boy=7.2, font="sgb", renk=(1, 1, 1))
        t.yazi(sx + 70, y + 1.6, sw - 70, aciklama, boy=8.0, renk=SOLUK)
        y += max(16.5, gw + 6)

    y += 12
    t.yazi(sx, y, sw, "Kapsam", boy=9, font="sgb")
    y += 16
    for etiket, urunler, platform in C.KAPSAM:
        t.yazi(sx, y, sw, etiket + "  ·  " + platform, boy=7.8, font="sgb", renk=VURGU)
        y += 11
        h = t.olc(sw, urunler, boy=8.2)
        t.yazi(sx, y, sw, urunler, boy=8.2, renk=MUREKKEP)
        y += h + 9

    # bolum haritasi
    hy = max(y + 30, 392.0)
    t.cizgi(KENAR, hy - 12, SAYFA_W - KENAR)
    t.yazi(KENAR, hy, ICERIK_W, "Bölümün haritası", boy=9, font="sgb")
    hy += 16
    sutun = (ICERIK_W - 24) / 2
    yarim = (len(C.BOLUM_HARITASI) + 1) // 2
    for sutun_no, parca in enumerate((C.BOLUM_HARITASI[:yarim], C.BOLUM_HARITASI[yarim:])):
        x = KENAR + sutun_no * (sutun + 24)
        yy = hy
        for sayfa, ad, alt in parca:
            t.yazi(x, yy, 24, sayfa, boy=7.6, font="sgb", renk=VURGU, hiza=2)
            t.yazi(x + 32, yy - 0.6, sutun - 32, ad, boy=8.2, font="sgb")
            t.yazi(x + 32, yy + 10, sutun - 32, alt, boy=7.4, renk=COK_SOLUK)
            yy += 23

    t.alt_bant("Şekil ve kanıt dizini son sayfadadır. İşaretler bu bölümün kopyaları üzerine çizildi; "
               "özgün kareler değişmedi.")


# --------------------------------------------------------------------------
# SAYFA 2 - bes ekranin bolusumu
# --------------------------------------------------------------------------
def sayfa_bolusum(t: Tuval) -> dict:
    t.yeni()
    t.ust_bant(C.UST_BASLIK, "Bölüm 3 · sayfa 2")
    y = t.baslik("2", "Beş ekran, beş farklı bölüşüm")
    h = t.olc(ICERIK_W, C.S2_GIRIS, boy=9.2, satir=1.40)
    t.yazi(KENAR, y, ICERIK_W, C.S2_GIRIS, boy=9.2, satir=1.40)
    y += h + 12

    ekran_h = 258.0
    ekran_w = ekran_h / TELEFON_ORAN
    serit_w = 9.0
    birim = serit_w + 4 + ekran_w
    aralik = (ICERIK_W - 5 * birim) / 4
    ust = y
    oranlar: dict[str, dict] = {}

    for i, (eid, ad, bolgeler) in enumerate(C.BOLUSUM):
        x = KENAR + i * (birim + aralik)
        toplam_app = sum(b - a for a, b, tur in bolgeler if tur != "notr")
        pay: dict[str, float] = {}
        for a, b, tur in bolgeler:
            y0 = ust + (a / 2400.0) * ekran_h
            y1 = ust + (b / 2400.0) * ekran_h
            renk = tuple(k / 255 for k in C.BOLGE_RENKLERI[tur])
            t.kutu(x, y0, x + serit_w, y1, dolgu=renk, kenar=CIZGI, kalin=0.3)
            if tur != "notr":
                pay[tur] = pay.get(tur, 0.0) + (b - a) / toplam_app * 100
        oranlar[eid] = pay
        t.resim(sade_kopya(eid), x + serit_w + 4, ust, ekran_w, ekran_h)
        t.yazi(x, ust + ekran_h + 4, birim, ad, boy=8.2, font="sgb", hiza=1)
        t.yazi(x, ust + ekran_h + 14, birim, f"Şekil 2.{i + 1} · açılış karesi", boy=6.6,
               renk=COK_SOLUK, hiza=1)

        yy = ust + ekran_h + 25
        for tur, etiket in C.BOLGE_ADLARI:
            if tur in pay:
                renk = tuple(k / 255 for k in C.BOLGE_RENKLERI[tur])
                t.kutu(x + 4, yy + 1.8, x + 10, yy + 7.8, dolgu=renk, kenar=None)
                t.yazi(x + 14, yy, birim - 44, etiket, boy=6.9, renk=SOLUK)
                t.yazi(x + birim - 34, yy, 30, f"%{pay[tur]:.0f}", boy=6.9,
                       font="sgb", renk=MUREKKEP, hiza=2)
                yy += 9.4

    y = ust + ekran_h + 25 + 5 * 9.4 + 10
    t.cizgi(KENAR, y, SAYFA_W - KENAR)
    y += 7

    # bulgular iki sutunda
    sutun_w = (ICERIK_W - 26) / 2
    yarim = (len(C.S2_BULGULAR) + 1) // 2
    y_sol = madde_ciz(t, KENAR, y, sutun_w, C.S2_BULGULAR[:yarim], boy=8.0)
    y_sag = madde_ciz(t, KENAR + sutun_w + 26, y, sutun_w, C.S2_BULGULAR[yarim:], boy=8.0)
    y = max(y_sol, y_sag) + 2
    h = t.olc(ICERIK_W, C.S2_NOT_DUGME, boy=7.4, satir=1.35)
    t.yazi(KENAR, y, ICERIK_W, C.S2_NOT_DUGME, boy=7.4, renk=SOLUK, satir=1.35, etiket="s2 dugme")
    y += h + 3
    t.yazi(KENAR, y, ICERIK_W, C.S2_SINIR, boy=7.4, renk=COK_SOLUK, satir=1.35, etiket="s2 sinir")

    t.alt_bant("Canlı kare: E0228, E0103, E0276, E0137, E0115. Oranlar uygulamanın kendi alanına göredir; "
               "sistem durum çubuğu ve alt sistem şeridi dışarıda bırakıldı.")
    return oranlar


# --------------------------------------------------------------------------
# SORU SAYFALARI
# --------------------------------------------------------------------------
def sayfa_soru(t: Tuval, s: dict, sekil_no: int) -> int:
    t.yeni()
    t.ust_bant(C.UST_BASLIK, f"Bölüm 3 · sayfa {t.no}")
    y = t.baslik(s["no"], s["baslik"])
    h = t.olc(ICERIK_W, s["giris"], boy=9.2, satir=1.40)
    t.yazi(KENAR, y, ICERIK_W, s["giris"], boy=9.2, satir=1.40)
    y += h + 10
    ust = y

    sekiller = s["sekiller"]
    n = len(sekiller)
    ekran_h = {1: 392.0, 2: 372.0, 3: 340.0}[n]
    ekran_w = ekran_h / TELEFON_ORAN
    bosluk = 12.0
    sol_blok = n * ekran_w + (n - 1) * bosluk

    tum_isaretler = []
    for i, f in enumerate(sekiller):
        x = KENAR + i * (ekran_w + bosluk)
        yol = isaretle(f["e"], f["isaretler"], ek=f"-s{t.no}")
        t.resim(yol, x, ust, ekran_w, ekran_h)
        etiket = f"Şekil {s['no']}.{i + 1} · {f['etiket']}"
        t.yazi(x, ust + ekran_h + 4, ekran_w, etiket, boy=6.9, renk=SOLUK, satir=1.25)
        tum_isaretler.extend(f["isaretler"])

    sx = KENAR + sol_blok + 22
    sw = SAYFA_W - KENAR - sx
    yy = ust
    yy = notlar_ciz(t, sx, yy, sw, tum_isaretler)

    yy += 6
    t.cizgi(sx, yy, sx + sw)
    yy += 6
    yy = madde_ciz(t, sx, yy, sw, s["notlar"], boy=8.0, renk=SOLUK)

    if s.get("diger"):
        yy += 4
        t.yazi(sx, yy, sw, "Aynı soruda diğer üç ürün", boy=7.6, font="sgb", renk=VURGU)
        yy += 12
        for ad, cumle in s["diger"]:
            hh = t.olc(sw - 2, f"{ad} — {cumle}", boy=7.8)
            t.yazi(sx, yy, sw, f"{ad} — {cumle}", boy=7.8, renk=SOLUK)
            yy += hh + 3

    t.alt_bant(s["dayanak"])
    return sekil_no


# --- sayfa 4a: bolum seciciler serit halinde -------------------------------
def sayfa_gezinme_serit(t: Tuval, s: dict):
    t.yeni()
    t.ust_bant(C.UST_BASLIK, f"Bölüm 3 · sayfa {t.no}")
    y = t.baslik(s["no"], s["baslik"])
    h = t.olc(ICERIK_W, s["giris"], boy=9.2, satir=1.40)
    t.yazi(KENAR, y, ICERIK_W, s["giris"], boy=9.2, satir=1.40)
    y += h + 12

    kirp_w = 396.0
    metin_x = KENAR + kirp_w + 16
    metin_w = 210.0
    yy = y
    for i, (eid, ad, kutu, aciklama) in enumerate(s["serit"]):
        yol = kirp(eid, kutu, f"-nav{i}")
        im = Image.open(yol)
        kh = kirp_w * im.height / im.width
        t.resim(yol, KENAR, yy, kirp_w, kh)
        t.yazi(metin_x, yy + 1, metin_w, f"Şekil {s['no']}.{i + 1} · {ad}", boy=8.2, font="sgb")
        t.yazi(metin_x, yy + 13, metin_w, aciklama, boy=7.8, renk=SOLUK, satir=1.28)
        yy += kh + 8

    yy += 2
    t.yazi(KENAR, yy, kirp_w + 16 + metin_w,
           "Beş kırpıntı da aynı genişlikte ve her ürünün kendi ana ekranından alındı; "
           "kırpma dışında değiştirilmedi.", boy=7.4, renk=COK_SOLUK, satir=1.3)

    nx = metin_x + metin_w + 24
    nw = SAYFA_W - KENAR - nx
    t.yazi(nx, y, nw, "Kareden okunanlar", boy=8.6, font="sgb")
    madde_ciz(t, nx, y + 15, nw, C.SERIT_NOTLARI, boy=8.0, renk=SOLUK)

    t.alt_bant("Canlı kare: E0228, E0115, E0103, E0276, E0137. Çıkarılmayan sonuç: bu yerleşimlerden "
               "bulunabilirlik, öğrenme süresi veya işlem hızı.")


# --- sayfa 4b: cekmecelerin ici --------------------------------------------
def sayfa_cekmece(t: Tuval, s: dict):
    t.yeni()
    t.ust_bant(C.UST_BASLIK, f"Bölüm 3 · sayfa {t.no}")
    y = t.baslik("", "Çekmecenin içinde ne var?")
    giris = ("Çekmeceyi kullanan iki üründe çekmecenin içi tek düzeyde listeleniyor: finansal bölümler, "
             "yardımcı araçlar ve ürün bakımıyla ilgili kalemler aynı listede, aralarında başlık veya ayraç "
             "olmadan duruyor.")
    h = t.olc(ICERIK_W, giris, boy=9.2, satir=1.40)
    t.yazi(KENAR, y, ICERIK_W, giris, boy=9.2, satir=1.40)
    y += h + 10
    ust = y

    ekran_h = 364.0
    ekran_w = ekran_h / TELEFON_ORAN
    tum = []
    for i, f in enumerate(s["sekiller"]):
        x = KENAR + i * (ekran_w + 14)
        yol = isaretle(f["e"], f["isaretler"], ek=f"-s{t.no}")
        t.resim(yol, x, ust, ekran_w, ekran_h)
        t.yazi(x, ust + ekran_h + 4, ekran_w, f"Şekil {s['no']}.{i + 6} · {f['etiket']}",
               boy=6.9, renk=SOLUK, satir=1.25)
        tum.extend(f["isaretler"])

    nx = KENAR + 2 * ekran_w + 14 + 24
    nw = SAYFA_W - KENAR - nx
    ny = notlar_ciz(t, nx, ust, nw, tum)
    ny += 6
    t.cizgi(nx, ny, nx + nw)
    ny += 6
    madde_ciz(t, nx, ny, nw, s["notlar"], boy=8.0, renk=SOLUK)

    t.alt_bant(s["dayanak"])


# --- sayfa 5 ozel duzeni ---------------------------------------------------
def sayfa_kayit(t: Tuval, s: dict):
    t.yeni()
    t.ust_bant(C.UST_BASLIK, f"Bölüm 3 · sayfa {t.no}")
    y = t.baslik(s["no"], s["baslik"])
    h = t.olc(ICERIK_W, s["giris"], boy=9.2, satir=1.40)
    t.yazi(KENAR, y, ICERIK_W, s["giris"], boy=9.2, satir=1.40)
    y += h + 10
    ust = y

    f = s["sekiller"][0]
    ekran_h = 392.0
    ekran_w = ekran_h / TELEFON_ORAN
    yol = isaretle(f["e"], f["isaretler"], ek=f"-s{t.no}")
    t.resim(yol, KENAR, ust, ekran_w, ekran_h)
    t.yazi(KENAR, ust + ekran_h + 4, ekran_w + 60, f"Şekil {s['no']}.1 · {f['etiket']}",
           boy=6.9, renk=SOLUK, satir=1.25)

    # dort yuvarlak dugme kirpintisi
    sx = KENAR + ekran_w + 26
    kutu_w = 58.0
    t.yazi(sx, ust, 400, "Diğer dört üründe kayıt başlatma", boy=8.4, font="sgb")
    for i, (eid, ad, kkutu) in enumerate(s["serit2"]):
        x = sx + i * (kutu_w + 14)
        yol2 = kirp(eid, kkutu, f"-fab{i}")
        t.resim(yol2, x, ust + 14, kutu_w, kutu_w)
        t.yazi(x, ust + 14 + kutu_w + 3, kutu_w + 12, ad, boy=6.8, renk=SOLUK, satir=1.2)
    t.yazi(sx, ust + 14 + kutu_w + 22, 4 * (kutu_w + 14),
           f"Şekil {s['no']}.2 · Dördünde de sağ altta tek bir yuvarlak düğme.",
           boy=6.9, renk=COK_SOLUK, satir=1.25)

    ny = ust + 14 + kutu_w + 40
    nw = SAYFA_W - KENAR - sx
    ny = notlar_ciz(t, sx, ny, nw, f["isaretler"])
    ny += 6
    t.cizgi(sx, ny, sx + nw)
    ny += 6
    madde_ciz(t, sx, ny, nw, s["notlar"], boy=8.0, renk=SOLUK)

    t.alt_bant(s["dayanak"])


# --------------------------------------------------------------------------
# SAYFA 8 - kaynak panolari
# --------------------------------------------------------------------------
def sayfa_kaynak(t: Tuval, k: dict):
    t.yeni()
    t.ust_bant(C.UST_BASLIK, f"Bölüm 3 · sayfa {t.no}")
    y = t.baslik(k["no"], k["baslik"])
    h = t.olc(ICERIK_W, k["giris"], boy=9.2, satir=1.40)
    t.yazi(KENAR, y, ICERIK_W, k["giris"], boy=9.2, satir=1.40)
    y += h + 10
    ust = y

    f = k["sekil"]
    yol = isaretle(f["e"], f["isaretler"], ek=f"-s{t.no}")
    im = Image.open(yol)
    resim_w = 498.0
    resim_h = resim_w * im.height / im.width
    t.resim(yol, KENAR, ust, resim_w, resim_h)
    t.yazi(KENAR, ust + resim_h + 5, resim_w, f"Şekil {k['no']}.1 · {f['etiket']}",
           boy=7.2, renk=SOLUK, satir=1.25)

    nx = KENAR + resim_w + 24
    nw = SAYFA_W - KENAR - nx
    ny = notlar_ciz(t, nx, ust, nw, f["isaretler"])
    ny += 8
    t.cizgi(nx, ny, nx + nw)
    ny += 8
    madde_ciz(t, nx, ny, nw, k["notlar"], boy=8.0, renk=SOLUK)

    t.alt_bant(k["dayanak"])


# --------------------------------------------------------------------------
# SAYFA 9 - kanit eki
# --------------------------------------------------------------------------
def sayfa_ek(t: Tuval, kullanilan: list[tuple[str, str]]):
    t.yeni()
    t.ust_bant(C.UST_BASLIK, f"Bölüm 3 · sayfa {t.no}")
    y = t.baslik("", "Kanıt eki")

    sol_w = 340.0
    t.yazi(KENAR, y, sol_w, "Şekil dizini", boy=9, font="sgb")
    yy = y + 15
    t.yazi(KENAR, yy, 54, "Şekil", boy=6.8, font="sgb", renk=COK_SOLUK)
    t.yazi(KENAR + 54, yy, 40, "Kimlik", boy=6.8, font="sgb", renk=COK_SOLUK)
    t.yazi(KENAR + 96, yy, sol_w - 96, "Koşum · tür", boy=6.8, font="sgb", renk=COK_SOLUK)
    yy += 10
    t.cizgi(KENAR, yy, KENAR + sol_w)
    yy += 4
    tur_adi = {"canli": "canlı kare", "kaynak": "kaynak görseli", "cizim": "temsili çizim"}
    for sekil, eid in kullanilan:
        _yol, kosum, tur = C.KANITLAR[eid]
        t.yazi(KENAR, yy, 54, sekil, boy=7.2)
        t.yazi(KENAR + 54, yy, 40, eid, boy=7.2, font="sgb")
        hh = t.olc(sol_w - 96, f"{kosum} · {tur_adi[tur]}", boy=7.2)
        t.yazi(KENAR + 96, yy, sol_w - 96, f"{kosum} · {tur_adi[tur]}", boy=7.2, renk=SOLUK)
        yy += max(hh, 9) + 2.4

    yy += 4
    t.yazi(KENAR, yy, sol_w, C.ENVANTER_NOTU, boy=7.0, renk=COK_SOLUK, satir=1.32)

    # orta sutun: cikarilmayan sonuclar
    ox = KENAR + sol_w + 26
    ow = 220.0
    t.yazi(ox, y, ow, "Çıkarılmayan sonuçlar", boy=9, font="sgb")
    oy = y + 15
    t.yazi(ox, oy, ow, "Bu bölüm aşağıdaki cümlelerin hiçbirini kurmaz.", boy=7.2,
           renk=COK_SOLUK, satir=1.3)
    oy += 16
    for yer, cumle in C.CIKARILMAYAN:
        t.yazi(ox, oy, ow, yer, boy=7.0, font="sgb", renk=VURGU)
        oy += 9
        hh = t.olc(ow, cumle, boy=7.4)
        t.yazi(ox, oy, ow, cumle, boy=7.4, renk=SOLUK)
        oy += hh + 5

    # sag sutun: eksikler
    ex = ox + ow + 26
    ew = SAYFA_W - KENAR - ex
    t.yazi(ex, y, ew, "Eksik kanıt ve nasıl kapanır", boy=9, font="sgb")
    ey = y + 15
    hh = t.olc(ew, C.VERI_KURALI, boy=7.0, satir=1.3)
    t.yazi(ex, ey, ew, C.VERI_KURALI, boy=7.0, renk=COK_SOLUK, satir=1.3)
    ey += hh + 7
    for urun, eksik, sayfa, nasil, oncelik in C.EKSIKLER:
        renk = {"Yüksek": VURGU, "Orta": (0.42, 0.42, 0.25), "Düşük": SOLUK,
                "Önerilmez": COK_SOLUK}[oncelik]
        t.yazi(ex, ey, ew - 52, f"{urun}  ·  sayfa {sayfa}", boy=7.0, font="sgb")
        t.yazi(ex + ew - 52, ey, 52, oncelik, boy=6.8, font="sgb", renk=renk, hiza=2)
        ey += 9.5
        hh = t.olc(ew, eksik, boy=7.3)
        t.yazi(ex, ey, ew, eksik, boy=7.3, renk=MUREKKEP)
        ey += hh + 1.5
        hh = t.olc(ew, nasil, boy=6.9)
        t.yazi(ex, ey, ew, nasil, boy=6.9, renk=COK_SOLUK)
        ey += hh + 6

    t.alt_bant("Özgün dosyalar araştırmanın kanitlar/ klasöründedir. SHA-256 değerleri kaynaklar.md "
               "ve kanit-manifest.json içinde; her değer KANIT-ENVANTERI.md ile karşılaştırıldı.")


# --------------------------------------------------------------------------
# Markdown kopyasi
# --------------------------------------------------------------------------
def markdown_yaz(oranlar: dict, kullanilan):
    s = [f"# Bölüm 3 · {C.BASLIK}", "", f"{C.UST_BASLIK} · {C.ALT_BASLIK}", "",
         "> Bu dosya PDF ile aynı içeriğin okunabilir kopyasıdır; ikisi de `icerik.py`den üretilir.",
         "> Sayfa düzeni, işaretler ve ölçüler yalnız PDF'te görünür.", ""]
    s += ["## Giriş", ""] + [p + "\n" for p in C.GIRIS]
    s += ["**Bu bölüm neyi ölçmez.** " + C.OLCMEZ, "", "### Kanıt anahtarı", ""]
    s += [f"- **{ad}** — {acik}" for _k, ad, acik in C.ROZETLER]
    s += ["", "### Kapsam", ""]
    s += [f"- **{e}** ({p}): {u}" for e, u, p in C.KAPSAM]

    s += ["", "## 2 · Beş ekran, beş farklı bölüşüm", "", C.S2_GIRIS, "",
          "| Ürün | Gezinme | Para bilgisi | Tanıtım | Kayıt bandı | Boş |",
          "|---|--:|--:|--:|--:|--:|"]
    for eid, ad, _b in C.BOLUSUM:
        p = oranlar[eid]
        s.append("| {} | {} | {} | {} | {} | {} |".format(
            ad, *[f"%{p[k]:.0f}" if k in p else "—"
                  for k in ("gezinme", "para", "tanitim", "kayit", "bos")]))
    s += [""] + [f"- {b}" for b in C.S2_BULGULAR] + ["", "*" + C.S2_SINIR + "*"]

    for soru in C.SORULAR:
        s += ["", f"## {soru['no']} · {soru['baslik']}", "", soru["giris"], ""]
        for i, f in enumerate(soru["sekiller"]):
            s += [f"**Şekil {soru['no']}.{i + 1} · {f['etiket']}** ({f['e']})", ""]
            s += [f"{no}. {metin}" for no, _x, _y, metin in f["isaretler"]]
            s.append("")
        if soru.get("serit"):
            s += ["Gezinme bölgesi kırpıntıları: " +
                  ", ".join(f"{ad} ({eid})" for eid, ad, _k, _a in soru["serit"]), ""]
        if soru.get("serit2"):
            s += [f"Kayıt düğmesi kırpıntıları (Şekil {soru['no']}.2): " +
                  ", ".join(f"{ad} ({eid})" for eid, ad, _k in soru["serit2"]), ""]
        s += [f"- {n}" for n in soru["notlar"]]
        if soru.get("diger"):
            s += ["", "**Aynı soruda diğer üç ürün**", ""]
            s += [f"- **{ad}** — {c}" for ad, c in soru["diger"]]
        s += ["", "*Dayanak.* " + soru["dayanak"]]

    for k in C.KAYNAK_SAYFALARI:
        f = k["sekil"]
        s += ["", f"## {k['no']} · {k['baslik']}", "", k["giris"], "",
              f"**Şekil {k['no']}.1 · {f['etiket']}** ({f['e']})", ""]
        s += [f"{no}. {metin}" for no, _x, _y, metin in f["isaretler"]]
        s.append("")
        s += [f"- {n}" for n in k["notlar"]] + ["", "*Dayanak.* " + k["dayanak"]]

    s += ["", "## Kanıt eki", "", "### Şekil dizini", "",
          "| Şekil | Kimlik | Koşum | Tür | Özgün dosya |", "|---|---|---|---|---|"]
    tur_adi = {"canli": "canlı kare", "kaynak": "kaynak görseli", "cizim": "temsili çizim"}
    for sekil, eid in kullanilan:
        yol, kosum, tur = C.KANITLAR[eid]
        s.append(f"| {sekil} | {eid} | {kosum} | {tur_adi[tur]} | `{yol}` |")
    s += ["", C.ENVANTER_NOTU, "", "### Çıkarılmayan sonuçlar", "", "| Yer | Kurulmayan cümle |", "|---|---|"]
    s += [f"| {y} | {c} |" for y, c in C.CIKARILMAYAN]
    s += ["", "### Eksik kanıt ve nasıl kapanır", "", C.VERI_KURALI, "",
          "| Ürün | Eksik | Sayfa | Nasıl tamamlanır | Öncelik |", "|---|---|---|---|---|"]
    s += [f"| {u} | {e} | {sa} | {n} | {o} |" for u, e, sa, n, o in C.EKSIKLER]
    MD_YOLU.write_text("\n".join(s) + "\n", encoding="utf-8")


# --------------------------------------------------------------------------
# Denetim dosyalari: iddia tablosu ve eksik listesi
# --------------------------------------------------------------------------
TUR_ADI = {"canli": "Canlı kare", "kaynak": "Kaynak görseli", "cizim": "Temsili çizim"}


def _sayfalar_icin_iddialar():
    """(yer, ifade, dayanak, tur) satirlarini icerik.py'den toplar."""
    sat = []
    for soru in C.SORULAR:
        no = soru["no"]
        if soru.get("duzen") == "serit":
            for i, (eid, ad, _k, aciklama) in enumerate(soru["serit"]):
                sat.append((f"Şekil {no}.{i + 1}", f"{ad}: {aciklama}", eid, C.KANITLAR[eid][2]))
            for i, f in enumerate(soru["sekiller"]):
                for m_no, _x, _y, metin in f["isaretler"]:
                    sat.append((f"Şekil {no}.{i + 6} · işaret {m_no}", metin, f["e"], C.KANITLAR[f["e"]][2]))
        else:
            for i, f in enumerate(soru["sekiller"]):
                for m_no, _x, _y, metin in f["isaretler"]:
                    sat.append((f"Şekil {no}.{i + 1} · işaret {m_no}", metin, f["e"], C.KANITLAR[f["e"]][2]))
            for eid, ad, _k in soru.get("serit2", []):
                sat.append((f"Şekil {no}.2", f"{ad}: sağ altta tek yuvarlak düğme", eid, C.KANITLAR[eid][2]))
    for k in C.KAYNAK_SAYFALARI:
        f = k["sekil"]
        for m_no, _x, _y, metin in f["isaretler"]:
            sat.append((f"Şekil {k['no']}.1 · işaret {m_no}", metin, f["e"], C.KANITLAR[f["e"]][2]))
    return sat


def _sentez_satirlari():
    sat = [("Bölüm 2 · bulgular", b, "E0228, E0103, E0276, E0137, E0115") for b in C.S2_BULGULAR]
    sat += [("Bölüm 4 · kareden okunanlar", b, "E0228, E0115, E0103, E0276, E0137") for b in C.SERIT_NOTLARI]
    for soru in C.SORULAR:
        kaynaklar = ", ".join(sorted({f["e"] for f in soru["sekiller"]}))
        sat += [(f"Bölüm {soru['no']} · not", n, kaynaklar) for n in soru["notlar"]]
    for k in C.KAYNAK_SAYFALARI:
        sat += [(f"Bölüm {k['no']} · not", n, k["sekil"]["e"]) for n in k["notlar"]]
    return sat


def denetim_yaz():
    s = ["# İddia tablosu — Bölüm 3 · işaretli ekran anlatımı", "",
         "16 Eylül 2026. Bu tablo elle tutulmaz: `icerik.py` içindeki işaret ve not metinlerinden",
         "üretilir, yani PDF'te okunan her cümle burada birebir görünür. Metin değişirse tablo da değişir.", "",
         "**Kural.** Kanıt niteliği ürüne değil ifadeye bağlıdır. Bir işaret yalnız bağlı olduğu karede",
         "görüneni anlatır. Cümlenin kapsamı kareden genişse (\"her ekranda\", \"tek ürün\" gibi) o kapsamı",
         "taşıyan kareler ayrıca gösterilir; gösterilemiyorsa cümle gözlenen kapsamla sınırlanır.", "",
         "## 1 · Şekil işaretleri", "",
         "| Yer | İfade | Dayanak | Kanıt türü |", "|---|---|---|---|"]
    for yer, ifade, eid, tur in _sayfalar_icin_iddialar():
        s.append(f"| {yer} | {ifade} | {eid} | {TUR_ADI[tur]} |")

    s += ["", "## 2 · Sentez cümleleri", "",
          "Aşağıdakiler tek bir işarete değil, o sayfadaki karelerin karşılaştırılmasına dayanır.", "",
          "| Yer | Cümle | Dayanak |", "|---|---|---|"]
    for yer, cumle, kaynaklar in _sentez_satirlari():
        s.append(f"| {yer} | {cumle} | {kaynaklar} |")

    s += ["", "## 3 · Çıkarılmayan sonuçlar", "",
          "Bu bölüm aşağıdaki cümlelerin hiçbirini kurmaz.", "",
          "| Yer | Kurulmayan cümle |", "|---|---|"]
    s += [f"| {y} | {c} |" for y, c in C.CIKARILMAYAN]
    s += ["", "## 4 · Ölçüm yöntemi", "", C.S2_SINIR, "", C.S2_NOT_DUGME, "",
          "## 5 · Envanter notu", "", C.ENVANTER_NOTU, ""]
    (BURASI / "iddia-tablosu.md").write_text("\n".join(s) + "\n", encoding="utf-8")

    e = ["# Eksik listesi — Bölüm 3 · işaretli ekran anlatımı", "",
         "16 Eylül 2026. Bölüm metninde koşum kaydıyla yazılan veya \"görülmedi\" kalan ifadeler.",
         "Her satırdaki görüntü gelirse ilgili ifade kareyle doğrulanır; gelmezse metin olduğu gibi",
         "kalır ve bölüm bu hâliyle geçerlidir.", "",
         "**" + C.VERI_KURALI + "**", "",
         "| Ürün | Eksik | Sayfa | Nasıl tamamlanır | Öncelik |", "|---|---|---|---|---|"]
    e += [f"| {u} | {x} | {sa} | {n} | {o} |" for u, x, sa, n, o in C.EKSIKLER]
    e += ["", "Öncelik değerleri: **Yüksek** mevcut veriyle bir görüntüyle kapanır · **Orta** kısa bir",
          "uygulama açma-kapama gerektirir · **Düşük** yalnız teyit · **Önerilmez** yeni kurulum, oturum",
          "kapatma veya ücretli hesap gerektirir.", ""]
    (BURASI / "eksik-listesi.md").write_text("\n".join(e) + "\n", encoding="utf-8")


# --------------------------------------------------------------------------
# Kanit kopyalari, kaynak dizini, manifest
# --------------------------------------------------------------------------
def kanitlari_hazirla(kullanilan_ids: list[str], env: dict) -> list[dict]:
    KANIT_DIZIN.mkdir(parents=True, exist_ok=True)
    kayit = []
    for eid in kullanilan_ids:
        kaynak = kanit_yolu(eid)
        oz = sha256(kaynak)
        beklenen = env.get(eid)
        if beklenen is None:
            _uyarilar.append(f"{eid}: envanterde hash satırı bulunamadı")
            durum = "envanterde yok"
        elif beklenen != oz:
            _uyarilar.append(f"{eid}: hash envanterle UYUŞMUYOR")
            durum = "uyuşmuyor"
        else:
            durum = "envanterle aynı"
        hedef = KANIT_DIZIN / f"{eid}.png"
        if C.KARARTMA.get(eid):
            # Teslim kopyasi karartilir; ozgun dosya ve hash'i degismez.
            km = Image.open(kaynak).convert("RGBA")
            for k in C.KARARTMA[eid]:
                ImageDraw.Draw(km).rectangle(k, fill=(120, 124, 130, 255))
            km.convert("RGB").save(hedef, "PNG")
        else:
            shutil.copy2(kaynak, hedef)
        kayit.append({
            "kimlik": eid,
            "ozgun": C.KANITLAR[eid][0],
            "kosum": C.KANITLAR[eid][1],
            "tur": C.KANITLAR[eid][2],
            "ozgun_sha256": oz,
            "envanter": durum,
            "kopya_sha256": sha256(hedef),
            "karartma": bool(C.KARARTMA.get(eid)),
        })
    return kayit


def kaynaklar_yaz(kayit: list[dict], kullanilan):
    sekil = {}
    for s, e in kullanilan:
        sekil.setdefault(e, []).append(s)
    sat = ["# Kaynak dizini — Bölüm 3 · işaretli ekran anlatımı", "",
           "16 Eylül 2026. `kanit/` altındaki kopyalar özgün dosyaların birebir kopyasıdır.",
           "`isaretli/` altındaki türevler bu bölüm için üzerine numaralı işaret çizilmiş",
           "kopyalardır ve bu nedenle özgünden farklıdır; ölçüm ve alıntı her zaman özgüne dayanır.", "",
           "Gizlilik: E0115'te e-postadan türeyen hesap adı yalnız bu bölümün kopyalarında",
           "karartıldı. Özgün kanıt ve hash'i değişmedi.", "",
           "| Kimlik | Şekil | Koşum | Tür | Özgün dosya | SHA-256 | Envanter |",
           "|---|---|---|---|---|---|---|"]
    for k in kayit:
        sat.append("| {} | {} | {} | {} | [{}](../../../kanitlar/{}) | `{}` | {} |".format(
            k["kimlik"], ", ".join(sekil.get(k["kimlik"], [])), k["kosum"], k["tur"],
            k["ozgun"], k["ozgun"], k["ozgun_sha256"], k["envanter"]))
    sat += ["", C.ENVANTER_NOTU, ""]
    (BURASI / "kaynaklar.md").write_text("\n".join(sat) + "\n", encoding="utf-8")


# --------------------------------------------------------------------------
# Ana akis
# --------------------------------------------------------------------------
def _kimlikleri_dogrula():
    """Icerikte anilan her E kimligi KANITLAR'da ve diskte olmali."""
    anilan = set()
    for soru in C.SORULAR:
        anilan |= {f["e"] for f in soru["sekiller"]}
        anilan |= {e for e, *_ in soru.get("serit", [])}
        anilan |= {e for e, *_ in soru.get("serit2", [])}
    anilan |= {k["sekil"]["e"] for k in C.KAYNAK_SAYFALARI}
    anilan |= {e for e, *_ in C.BOLUSUM}

    eksik = sorted(anilan - set(C.KANITLAR))
    if eksik:
        raise SystemExit("KANITLAR sözlüğünde tanımsız kimlik: " + ", ".join(eksik))
    yok = [f"{e} -> {C.KANITLAR[e][0]}" for e in sorted(anilan) if not kanit_yolu(e).exists()]
    if yok:
        raise SystemExit("diskte bulunamayan kanıt:\n  " + "\n  ".join(yok))


def main():
    if not ENVANTER.exists():
        raise SystemExit(f"envanter bulunamadı: {ENVANTER}")
    _kimlikleri_dogrula()
    env = envanter_hashleri()

    for d in (ISARETLI_DIZIN, KANIT_DIZIN):
        if d.exists():
            shutil.rmtree(d)

    belge = pymupdf.open()
    t = Tuval(belge)

    sayfa_kapak(t)
    oranlar = sayfa_bolusum(t)

    kullanilan: list[tuple[str, str]] = [
        ("2.1", "E0228"), ("2.2", "E0103"), ("2.3", "E0276"), ("2.4", "E0137"), ("2.5", "E0115"),
    ]

    for soru in C.SORULAR:
        duzen = soru.get("duzen")
        if duzen == "serit":
            sayfa_gezinme_serit(t, soru)
            kullanilan += [(f"{soru['no']}.{i + 1}", e) for i, (e, _a, _k, _x) in enumerate(soru["serit"])]
            sayfa_cekmece(t, soru)
            kullanilan += [(f"{soru['no']}.{i + 6}", f["e"]) for i, f in enumerate(soru["sekiller"])]
        elif duzen == "kayit":
            sayfa_kayit(t, soru)
            kullanilan += [(f"{soru['no']}.1", soru["sekiller"][0]["e"])]
            kullanilan += [(f"{soru['no']}.2", e) for e, _a, _k in soru["serit2"]]
        else:
            sayfa_soru(t, soru, 0)
            kullanilan += [(f"{soru['no']}.{i + 1}", f["e"]) for i, f in enumerate(soru["sekiller"])]

    for k in C.KAYNAK_SAYFALARI:
        sayfa_kaynak(t, k)
        kullanilan.append((f"{k['no']}.1", k["sekil"]["e"]))

    # dizinde tekrar eden kimlikleri birlestir
    gorulen, tekil = set(), []
    for sekil, eid in kullanilan:
        if (sekil, eid) not in gorulen:
            gorulen.add((sekil, eid))
            tekil.append((sekil, eid))

    sayfa_ek(t, tekil)

    belge.set_metadata({
        "title": f"Belge 1 · Bölüm 3 · {C.BASLIK} (işaretli ekran anlatımı)",
        "author": "", "subject": "Rakip arayüz araştırması · taslak",
        "keywords": "rakip arayüz, ana ekran, gezinme",
    })
    belge.save(str(PDF_YOLU), deflate=True, garbage=3)
    belge.close()

    ids = sorted({e for _s, e in tekil} | {"E0181", "E0084"})
    kayit = kanitlari_hazirla(ids, env)
    kaynaklar_yaz(kayit, tekil)
    (BURASI / "kanit-manifest.json").write_text(
        json.dumps({"bolum": "Belge 1 · Bölüm 3 · işaretli ekran anlatımı",
                    "tarih": "2026-09-16", "kanitlar": kayit,
                    "isaretli_turevler": sorted(p.name for p in ISARETLI_DIZIN.glob("*.png"))},
                   ensure_ascii=False, indent=2), encoding="utf-8")
    markdown_yaz(oranlar, tekil)
    denetim_yaz()

    print(f"PDF: {PDF_YOLU.name}  ({pymupdf.open(str(PDF_YOLU)).page_count} sayfa)")
    print(f"Kanıt: {len(kayit)} özgün kopya, {len(list(ISARETLI_DIZIN.glob('*.png')))} işaretli türev")
    uyusmaz = [k["kimlik"] for k in kayit if k["envanter"] != "envanterle aynı"]
    print(f"Envanter karşılaştırması: {len(kayit) - len(uyusmaz)}/{len(kayit)} aynı"
          + (f" — SORUNLU: {uyusmaz}" if uyusmaz else ""))
    if _uyarilar:
        print("\nUYARILAR:")
        for u in _uyarilar:
            print("  -", u)
    else:
        print("Yerleşim uyarısı yok.")


if __name__ == "__main__":
    main()
