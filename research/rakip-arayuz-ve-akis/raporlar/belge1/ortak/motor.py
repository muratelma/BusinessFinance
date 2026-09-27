# -*- coding: utf-8 -*-
"""Belge 1 bolum ureticisi — ortak motor.

Her bolum klasorunde iki dosya olur:

    icerik.py   bolumun TEK icerik kaynagi: metin, isaret koordinatlari, kanit baglari
    uret.py     o bolume ozgu sayfa yerlesimi + main(); bu motoru import eder

Motor bolume ozgu hicbir sey bilmez. Tasidiklari:

  * Tuval        A4 yatay sayfa, gomulu font, olcumlu metin kutusu, cizim
  * isaretle()   kareye numarali isaret basar; kirpma ve gizlilik karartmasi
  * kirp()       kareden bolge kirpar
  * Kanit        envanter dizininden yol/hash/aciklama okur, hash dogrular
  * Denetim      iddia tablosu, eksik listesi, kaynaklar.md, manifest
  * kontrol()    bes otomatik kapi; hata varsa uretim durur

Bu motorun yapamadigi (bolum kapanis listesinde yazili): iddianin kanittan
gercekten ciktigini dogrulamak ve GORSELE GOMULU kisisel adi yakalamak.
Her bolum yine de sayfa sayfa gozle incelenir.
"""
from __future__ import annotations

import hashlib
import json
import pathlib
import re
import shutil

import pymupdf
from PIL import Image, ImageDraw, ImageFont

# --------------------------------------------------------------------------
# Yollar ve ortam
# --------------------------------------------------------------------------
ORTAK = pathlib.Path(__file__).resolve().parent
KOK = ORTAK.parents[2]                      # research/rakip-arayuz-ve-akis
KANITLAR = KOK / "kanitlar"
DIZIN_JSON = ORTAK / "kanit-dizini.json"

FONT_DIZIN = pathlib.Path("C:/Windows/Fonts")
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
BEYAZ = (1, 1, 1)

SAYFA_W, SAYFA_H = 842.0, 595.0             # A4 yatay
KENAR = 34.0
ICERIK_W = SAYFA_W - 2 * KENAR
TELEFON_ORAN = 2400 / 1080                  # kareler 1080x2400

# Kanit turleri — plan Bolum 1'deki anahtarla birebir
TURLER = {
    "canli": "Canlı kare",
    "kosum": "Koşum kaydı",
    "kaynak": "Kaynak görseli",
    "beyan": "Kaynak beyanı",
    "yok": "Görülmedi",
}

uyarilar: list[str] = []

_OLCU_BELGE = pymupdf.open()
_OLCU_SAYFA = None


def _olcu_sayfa() -> pymupdf.Page:
    """Olcum ayri belgede yapilir; cikti belgesine sayfa ekleyip silmek
    acik sayfa nesnesini gecersiz kilar."""
    global _OLCU_SAYFA
    if _OLCU_SAYFA is None:
        _OLCU_SAYFA = _OLCU_BELGE.new_page(width=1200, height=4000)
        _OLCU_SAYFA.insert_font(fontname="sg", fontfile=str(F_DUZ))
        _OLCU_SAYFA.insert_font(fontname="sgb", fontfile=str(F_KALIN))
    return _OLCU_SAYFA


# --------------------------------------------------------------------------
# Kanit erisimi
# --------------------------------------------------------------------------
def sha256(yol: pathlib.Path) -> str:
    h = hashlib.sha256()
    with open(yol, "rb") as f:
        for blok in iter(lambda: f.read(1 << 20), b""):
            h.update(blok)
    return h.hexdigest()


class Kanit:
    """kanit-dizini.json uzerinden E kimligi ile kare erisimi."""

    def __init__(self) -> None:
        if not DIZIN_JSON.exists():
            raise SystemExit(
                "kanit-dizini.json yok. Once sunu calistirin:\n"
                "  raporlar/.pilot-tools/venv/Scripts/python "
                "raporlar/belge1/ortak/kanit-dizini-uret.py")
        veri = json.loads(DIZIN_JSON.read_text(encoding="utf-8"))
        self.kayit = veri["kanitlar"]
        self.ad_to_id = veri["dosya_adindan_kimlige"]

    def yol(self, eid: str) -> pathlib.Path:
        if eid not in self.kayit:
            raise SystemExit(f"{eid} kanıt dizininde yok")
        return KOK / self.kayit[eid]["yol"]

    def kimlik(self, dosya_adi: str) -> str:
        """`money-manager/03-dolu-ana-ekran.png` -> `E0228`"""
        if dosya_adi not in self.ad_to_id:
            raise SystemExit(f"{dosya_adi} kanıt dizininde yok")
        return self.ad_to_id[dosya_adi]

    def aciklama(self, eid: str) -> list[str]:
        return self.kayit[eid]["aciklama"]

    def uygulama(self, eid: str) -> str | None:
        return self.kayit[eid]["uygulama"]

    def dogrula(self, kimlikler) -> list[dict]:
        """Her kimlik icin: dizinde var mi, diskte var mi, hash tutuyor mu."""
        sonuc = []
        for eid in sorted(set(kimlikler)):
            if eid not in self.kayit:
                raise SystemExit(f"{eid} kanıt dizininde yok")
            p = self.yol(eid)
            if not p.exists():
                raise SystemExit(f"{eid} diskte yok: {p}")
            gercek = sha256(p)
            beklenen = self.kayit[eid]["sha256"]
            if gercek != beklenen:
                raise SystemExit(f"{eid} hash envanterle UYUŞMUYOR: {p}")
            sonuc.append({"kimlik": eid, "yol": self.kayit[eid]["yol"],
                          "sha256": gercek, "envanter": "aynı"})
        return sonuc


# --------------------------------------------------------------------------
# Gorsel islemleri
# --------------------------------------------------------------------------
def _ac(kanit: Kanit, eid: str, kirpma=None, karartma=()) -> Image.Image:
    im = Image.open(kanit.yol(eid)).convert("RGBA")
    if kirpma:
        im = im.crop(kirpma)
    if karartma:
        ciz = ImageDraw.Draw(im)
        for kutu in karartma:
            ciz.rectangle(kutu, fill=(120, 124, 130, 255))
    return im


def isaretle(kanit: Kanit, eid: str, isaretler, hedef_dizin: pathlib.Path,
             *, ek: str = "", kirpma=None, karartma=()) -> pathlib.Path:
    """Kareye numarali isaret basar.

    isaretler: (no, x, y, metin) — koordinatlar KIRPMA SONRASI uzaydadir.
    Isaret, gosterdigi seyin ustunu kapatmamali; koordinat bos alana konur.
    """
    im = _ac(kanit, eid, kirpma, karartma)
    ciz = ImageDraw.Draw(im)

    cap = max(52, int(im.width * 0.082))
    if im.width > 1200:                       # genis masaustu kareleri
        cap = max(34, int(im.width * 0.030))
    r = cap // 2
    font = ImageFont.truetype(str(F_ISARET), int(cap * 0.62))

    for no, x, y, _metin in isaretler:
        ciz.ellipse((x - r - 4, y - r - 4, x + r + 4, y + r + 4), fill=(255, 255, 255, 255))
        ciz.ellipse((x - r, y - r, x + r, y + r), fill=VURGU_RGB + (255,))
        ciz.text((x, y + 1), str(no), font=font, fill=(255, 255, 255, 255), anchor="mm")

    hedef_dizin.mkdir(parents=True, exist_ok=True)
    hedef = hedef_dizin / f"{eid}{ek}.png"
    im.convert("RGB").save(hedef, "PNG")
    return hedef


def kirp(kanit: Kanit, eid: str, kutu, hedef_dizin: pathlib.Path,
         *, ek: str, karartma=()) -> pathlib.Path:
    im = _ac(kanit, eid, None, karartma).crop(kutu)
    hedef_dizin.mkdir(parents=True, exist_ok=True)
    hedef = hedef_dizin / f"{eid}{ek}.png"
    im.convert("RGB").save(hedef, "PNG")
    return hedef


def sade(kanit: Kanit, eid: str, hedef_dizin: pathlib.Path,
         *, ek: str = "-sade", kirpma=None, karartma=()) -> pathlib.Path:
    im = _ac(kanit, eid, kirpma, karartma)
    hedef_dizin.mkdir(parents=True, exist_ok=True)
    hedef = hedef_dizin / f"{eid}{ek}.png"
    im.convert("RGB").save(hedef, "PNG")
    return hedef


# --------------------------------------------------------------------------
# Sayfa tuvali
# --------------------------------------------------------------------------
class Tuval:
    def __init__(self, belge: pymupdf.Document, ust_metin: str = "") -> None:
        self.belge = belge
        self.sayfa: pymupdf.Page | None = None
        self.no = 0
        self.ust_metin = ust_metin

    # -- sayfa ------------------------------------------------------------
    def yeni(self, sag_ust: str = "") -> pymupdf.Page:
        self.sayfa = self.belge.new_page(width=SAYFA_W, height=SAYFA_H)
        self.sayfa.insert_font(fontname="sg", fontfile=str(F_DUZ))
        self.sayfa.insert_font(fontname="sgb", fontfile=str(F_KALIN))
        self.no += 1
        if self.ust_metin or sag_ust:
            self.ust_bant(self.ust_metin, sag_ust)
        return self.sayfa

    def ust_bant(self, sol: str, sag: str = "") -> None:
        self.yazi(KENAR, 22, ICERIK_W * 0.7, sol, boy=7.5, renk=COK_SOLUK)
        if sag:
            self.yazi(KENAR + ICERIK_W * 0.7, 22, ICERIK_W * 0.3, sag,
                      boy=7.5, renk=COK_SOLUK, hiza=2)
        self.cizgi(KENAR, 36, SAYFA_W - KENAR)

    def alt_bant(self, dayanak: str = "") -> None:
        self.cizgi(KENAR, SAYFA_H - 30, SAYFA_W - KENAR)
        if dayanak:
            self.yazi(KENAR, SAYFA_H - 26, ICERIK_W - 40, dayanak,
                      boy=6.8, renk=COK_SOLUK, etiket="dayanak")
        self.yazi(SAYFA_W - KENAR - 30, SAYFA_H - 26, 30, str(self.no),
                  boy=7.5, renk=COK_SOLUK, hiza=2)

    def baslik(self, no: str, metin: str, y: float = 46) -> float:
        if no:
            self.yazi(KENAR, y, 26, no, boy=17, font="sgb", renk=VURGU)
            self.yazi(KENAR + 28, y + 2, ICERIK_W - 28, metin, boy=15, font="sgb")
        else:
            self.yazi(KENAR, y, ICERIK_W, metin, boy=15, font="sgb")
        return y + 26

    def giris(self, y: float, metin: str, *, boy: float = 9.2) -> float:
        h = self.olc(ICERIK_W, metin, boy=boy, satir=1.40)
        self.yazi(KENAR, y, ICERIK_W, metin, boy=boy, satir=1.40, etiket="giris")
        return y + h + 10

    # -- metin ------------------------------------------------------------
    def yazi(self, x, y, w, metin, *, boy=9.0, font="sg", renk=MUREKKEP,
             hiza=0, satir=1.30, etiket="") -> float:
        kutu = pymupdf.Rect(x, y, x + w, y + 600.0)
        art = self.sayfa.insert_textbox(kutu, metin, fontname=font, fontsize=boy,
                                        color=renk, align=hiza, lineheight=satir)
        if art < 0:
            uyarilar.append(
                f"sayfa {self.no}: metin sığmadı ({etiket or metin[:40]!r}), eksik {art:.0f} pt")
            return 600.0
        return 600.0 - art

    def olc(self, w, metin, *, boy=9.0, font="sg", satir=1.30) -> float:
        art = _olcu_sayfa().insert_textbox(
            pymupdf.Rect(0, 0, w, 4000), metin, fontname=font, fontsize=boy,
            color=(0, 0, 0), lineheight=satir)
        return 4000 - art if art >= 0 else 4000

    # -- cizim ------------------------------------------------------------
    def cizgi(self, x0, y, x1, renk=CIZGI, kalin=0.6) -> None:
        self.sayfa.draw_line(pymupdf.Point(x0, y), pymupdf.Point(x1, y),
                             color=renk, width=kalin)

    def kutu(self, x0, y0, x1, y1, *, dolgu=None, kenar=None, kalin=0.6) -> None:
        self.sayfa.draw_rect(pymupdf.Rect(x0, y0, x1, y1),
                             color=kenar, fill=dolgu, width=kalin)

    def resim(self, yol: pathlib.Path, x, y, w, h) -> None:
        self.sayfa.insert_image(pymupdf.Rect(x, y, x + w, y + h), filename=str(yol))
        self.sayfa.draw_rect(pymupdf.Rect(x, y, x + w, y + h), color=CIZGI, width=0.5)

    def resim_en(self, yol: pathlib.Path, x, y, w) -> float:
        """Genislige gore yukseklik hesaplayarak basar; yuksekligi dondurur."""
        im = Image.open(yol)
        h = w * im.height / im.width
        self.resim(yol, x, y, w, h)
        return h

    # -- liste ------------------------------------------------------------
    def numarali_notlar(self, x, y, w, isaretler, *, boy=8.2) -> float:
        for no, _x, _y, metin in isaretler:
            self.sayfa.draw_circle(pymupdf.Point(x + 5.5, y + 5.0), 5.5,
                                   color=None, fill=VURGU)
            self.yazi(x, y + 1.2, 11, str(no), boy=6.6, font="sgb", renk=BEYAZ, hiza=1)
            h = self.olc(w - 16, metin, boy=boy)
            self.yazi(x + 16, y, w - 16, metin, boy=boy)
            y += max(h, 12.5) + 3.2
        return y

    def maddeler(self, x, y, w, satirlar, *, boy=8.2, renk=SOLUK) -> float:
        for m in satirlar:
            self.sayfa.draw_circle(pymupdf.Point(x + 2.4, y + 4.6), 1.5,
                                   color=None, fill=COK_SOLUK)
            h = self.olc(w - 10, m, boy=boy)
            self.yazi(x + 10, y, w - 10, m, boy=boy, renk=renk)
            y += h + 4.0
        return y


# --------------------------------------------------------------------------
# Denetim dosyalari
# --------------------------------------------------------------------------
def iddia_tablosu_yaz(hedef: pathlib.Path, baslik: str, isaret_satirlari,
                      sentez_satirlari, cikarilmayanlar, ek_notlar=()) -> None:
    """isaret_satirlari: (yer, ifade, kimlik, tur) · sentez: (yer, cumle, dayanak)"""
    s = [f"# İddia tablosu — {baslik}", "",
         "Bu tablo elle tutulmaz: `icerik.py` içindeki işaret ve not metinlerinden üretilir.",
         "PDF'te okunan her cümle burada birebir görünür; metin değişirse tablo da değişir.", "",
         "**Kural.** Kanıt niteliği ürüne değil ifadeye bağlıdır. Bir işaret yalnız bağlı olduğu",
         "karede görüneni anlatır. Cümlenin kapsamı kareden genişse o kapsamı taşıyan kanıt ayrıca",
         "gösterilir; gösterilemiyorsa cümle gözlenen kapsamla sınırlanır.", "",
         "## 1 · Şekil işaretleri", "", "| Yer | İfade | Dayanak | Kanıt türü |", "|---|---|---|---|"]
    for yer, ifade, kimlik, tur in isaret_satirlari:
        s.append(f"| {yer} | {ifade} | {kimlik} | {TURLER.get(tur, tur)} |")

    s += ["", "## 2 · Sentez cümleleri", "",
          "Tek bir işarete değil, sayfadaki karelerin karşılaştırılmasına dayanır.", "",
          "| Yer | Cümle | Dayanak |", "|---|---|---|"]
    for yer, cumle, dayanak in sentez_satirlari:
        s.append(f"| {yer} | {cumle} | {dayanak} |")

    s += ["", "## 3 · Çıkarılmayan sonuçlar", "",
          "Bu bölüm aşağıdaki cümlelerin hiçbirini kurmaz.", "",
          "| Yer | Kurulmayan cümle |", "|---|---|"]
    s += [f"| {y} | {c} |" for y, c in cikarilmayanlar]
    if ek_notlar:
        s += ["", "## 4 · Ek notlar", ""] + [f"- {n}" for n in ek_notlar]
    hedef.write_text("\n".join(s) + "\n", encoding="utf-8")


def eksik_listesi_yaz(hedef: pathlib.Path, baslik: str, eksikler) -> None:
    """eksikler: (urun, eksik, sayfa, nasil, oncelik)"""
    s = [f"# Eksik listesi — {baslik}", "",
         "Bölüm metninde koşum kaydıyla yazılan veya \"görülmedi\" kalan ifadeler.",
         "Görüntü gelirse ilgili ifade kareyle doğrulanır; gelmezse metin olduğu gibi kalır.", "",
         "**Mevcut test verisi silinmez veya sıfırlanmaz.** Yeni kurulum, oturum kapatma veya yeni",
         "hesap gerektiren eksikler bu nedenle Önerilmez olarak işaretlidir.", "",
         "| Ürün | Eksik | Sayfa | Nasıl tamamlanır | Öncelik |", "|---|---|---|---|---|"]
    s += [f"| {u} | {e} | {sa} | {n} | {o} |" for u, e, sa, n, o in eksikler]
    s += ["", "Öncelik: **Yüksek** mevcut veriyle bir görüntüyle kapanır · **Orta** kısa bir",
          "uygulama açma-kapama gerektirir · **Düşük** yalnız teyit · **Önerilmez** yeni kurulum,",
          "oturum kapatma veya ücretli hesap gerektirir.", ""]
    hedef.write_text("\n".join(s) + "\n", encoding="utf-8")


def kaynaklar_yaz(hedef: pathlib.Path, baslik: str, kanit: Kanit,
                  sekil_eslemesi, dogrulama, notlar=()) -> None:
    """sekil_eslemesi: (sekil_no, kimlik) listesi"""
    sekil = {}
    for s_no, eid in sekil_eslemesi:
        sekil.setdefault(eid, []).append(s_no)
    sat = [f"# Kaynak dizini — {baslik}", "",
           "`isaretli/` altındaki türevler bu bölüm için üzerine numaralı işaret çizilmiş",
           "kopyalardır ve özgünden farklıdır; ölçüm ve alıntı her zaman özgüne dayanır.", "",
           "| Kimlik | Şekil | Uygulama | Özgün dosya | SHA-256 | Envanter |",
           "|---|---|---|---|---|---|"]
    for d in dogrulama:
        eid = d["kimlik"]
        # bolum klasorunden arastirma koküne: bolum -> belge1 -> raporlar -> kok
        sat.append("| {} | {} | {} | [{}](../../../{}) | `{}` | {} |".format(
            eid, ", ".join(sekil.get(eid, [])) or "—", kanit.uygulama(eid) or "—",
            d["yol"], d["yol"], d["sha256"], d["envanter"]))
    if notlar:
        sat += [""] + [f"- {n}" for n in notlar]
    hedef.write_text("\n".join(sat) + "\n", encoding="utf-8")


def manifest_yaz(hedef: pathlib.Path, baslik: str, tarih: str,
                 dogrulama, isaretli_dizin: pathlib.Path) -> None:
    hedef.write_text(json.dumps({
        "bolum": baslik,
        "tarih": tarih,
        "kanitlar": dogrulama,
        "isaretli_turevler": sorted(p.name for p in isaretli_dizin.glob("*.png")),
    }, ensure_ascii=False, indent=2), encoding="utf-8")


# --------------------------------------------------------------------------
# Otomatik kapilar
# --------------------------------------------------------------------------
def kontrol(pdf_yolu: pathlib.Path, *, gizli_sozcukler=()) -> dict:
    """Bes otomatik kapi. Hepsi gecerse rapor doner; gecmezse uyari listesi dolar.

    Yapamadiklari: iddianin kanittan ciktigini dogrulamak, GORSELE GOMULU
    kisisel adi yakalamak. Gozle sayfa incelemesi bu yuzden zorunludur.
    """
    d = pymupdf.open(str(pdf_yolu))
    metin = "\n".join(p.get_text() for p in d)

    atif = set(re.findall(r"Şekil (\d+\.\d+)", metin))
    basli = set(re.findall(r"Şekil (\d+\.\d+) ·", metin))
    kirik = sorted(atif - basli)

    bozuk = sorted({c for c in metin if ord(c) == 0xFFFD})
    sizinti = sorted({s for s in gizli_sozcukler if s.lower() in metin.lower()})
    fontlar = sorted({f[3] for p in d for f in p.get_fonts()})
    sayfa = d.page_count
    d.close()

    if kirik:
        uyarilar.append(f"karşılığı basılmayan şekil göndermesi: {kirik}")
    if bozuk:
        uyarilar.append(f"bozuk karakter: {bozuk}")
    if sizinti:
        uyarilar.append(f"PDF metninde gizli sözcük: {sizinti}")

    return {"sayfa": sayfa, "kirik_atif": kirik, "bozuk_karakter": bozuk,
            "metinde_gizli_sozcuk": sizinti, "fontlar": fontlar,
            "yerlesim_uyarisi": [u for u in uyarilar if "sığmadı" in u]}


def onizleme(pdf_yolu: pathlib.Path, hedef_dizin: pathlib.Path, dpi: int = 125) -> int:
    """Sayfalari PNG'ye cevirir; gozle inceleme adimi bunlarin uzerinden yapilir."""
    hedef_dizin.mkdir(parents=True, exist_ok=True)
    for eski in hedef_dizin.glob("s*.png"):
        eski.unlink()
    d = pymupdf.open(str(pdf_yolu))
    for i, p in enumerate(d, 1):
        p.get_pixmap(dpi=dpi).save(hedef_dizin / f"s{i:02d}.png")
    n = d.page_count
    d.close()
    return n


def rapor_yaz(ad: str, pdf_yolu: pathlib.Path, sonuc: dict, dogrulama) -> None:
    print(f"{ad}: {sonuc['sayfa']} sayfa, {pdf_yolu.stat().st_size / 1e6:.1f} MB")
    print(f"Kanit: {len(dogrulama)} kare, hepsi envanterle ayni")
    print(f"Fontlar: {', '.join(sonuc['fontlar'])}")
    if uyarilar:
        print("\nUYARILAR:")
        for u in uyarilar:
            print("  -", u)
    else:
        print("Otomatik kapilar temiz. Gozle sayfa incelemesi yapilmadi.")


def temizle(*dizinler: pathlib.Path) -> None:
    for d in dizinler:
        if d.exists():
            shutil.rmtree(d)
