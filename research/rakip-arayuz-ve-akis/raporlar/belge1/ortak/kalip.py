# -*- coding: utf-8 -*-
"""Belge 1 sayfa kaliplari — plan §6'daki bes kalip ve bolum acilisi.

motor.py tuvali, isareti ve denetimi tasir; bu dosya onlarin uzerine bolumden
bagimsiz sayfa yerlesimlerini koyar. Bir bolumun icerik.py'si yalniz VERI tasir:

    NO, BASLIK, ANA_SORU, GIRIS, GIRMEZ, KAPSAM   bolum acilisi
    (kanit turu kareden cikar: KolayBi destek gorseli "kaynak", digerleri "canli")
    KIRPMA     {E kimligi: (x0, y0, x1, y1)}       isaretten once uygulanir
    KARARTMA   {E kimligi: [(x0, y0, x1, y1), ...]} yalniz kopyalarda
    SAYFALAR   [sayfa sozlugu, ...]                 sirayla basilir
    CIKARILMAYAN, EKSIKLER                           denetim dosyalari

Sayfa turleri (sozlukte "tur"):

    soru     Kalip A · baslik, giris, 1-4 isaretli kare, sag sutunda notlar
    yanyana  Kalip B · esit yukseklikte kareler, altta kisa satirlar
    serit    Kalip C · ayni genislikte kirpintilar alt alta, konum yazili
    kartlar  Kalip D · urun kartlari (Bolum 2)
    tablo    Kalip E · urun x ozellik; hucre (metin, tur) olabilir
    metin    karesiz kaynak anlatimi; urun bloklari

Metinde `[[anahtar]]` bir sekle gondermedir; basimda `Sekil N.M` olur. Bilinmeyen
anahtar uretimi durdurur. Sekil numaralari bolum icinde basim sirasiyla verilir.

Kanit turleri motor.TURLER ile ayni bes anahtardir: canli, kosum, kaynak,
beyan, yok. Tur her zaman IFADEYE baglanir, urune degil.
"""
from __future__ import annotations

import json
import pathlib
import re

import pymupdf
from PIL import Image

import motor as M

UST = "Belge 1 · Rakip arayüz yaklaşımları"
TARIH = "16 Eylül 2026"
GIZLI = ("elma6", "muratelma", "murat elma", "murat", "6004")
ALT_SINIR = M.SAYFA_H - 38
BOSLUK = 12.0

TUR_RENK = {
    "canli": (0.12, 0.44, 0.70),
    "kosum": (0.30, 0.40, 0.50),
    "kaynak": (0.58, 0.42, 0.14),
    "beyan": (0.46, 0.30, 0.56),
    "yok": (0.56, 0.58, 0.61),
}
TUR_ACIKLAMA = {
    "canli": "Emülatörde açılan ekranın görüntüsü.",
    "kosum": "Gözlem formunda yazılı; karesi yok veya kare tek başına göstermiyor. Kullanıcı kontrolleri de buraya girer.",
    "kaynak": "Ürünün arayüzünü anlatmak için yayımladığı ekran görüntüsü (KolayBi destek sayfası).",
    "beyan": "Ürünün kılavuzu, yardım merkezi veya ürün sayfası metni. Görsel yok.",
    "yok": "Kanıt yok. Özelliğin bulunmadığı anlamına gelmez.",
}

_FONT_KALIN = pymupdf.Font(fontfile=str(M.F_KALIN))
_FONT_DUZ = pymupdf.Font(fontfile=str(M.F_DUZ))
_ATIF = re.compile(r"\[\[([a-z0-9-]+)\]\]")


def genislik(metin: str, boy: float, kalin: bool = False) -> float:
    return (_FONT_KALIN if kalin else _FONT_DUZ).text_length(metin, fontsize=boy)


# --------------------------------------------------------------------------
# Bolum
# --------------------------------------------------------------------------
class Bolum:
    """Bir bolumun icerigini sayfalara doken ve denetim satirlarini toplayan nesne."""

    def __init__(self, C, klasor: pathlib.Path, kanit: M.Kanit | None = None) -> None:
        self.C = C
        self.klasor = klasor
        self.k = kanit or M.Kanit()
        self.isaretli = klasor / "isaretli"
        self._gorsel: dict[tuple, pathlib.Path] = {}
        self.sekil_no: dict[str, str] = {}
        self.sayfa_no: list[int] = []          # her sayfa sozlugunun basildigi sayfa
        self._numaralandir()
        self._sifirla()

    # -- kimlik / tur ----------------------------------------------------
    def tur(self, eid: str) -> str:
        return kare_turu(self.k, eid)

    def urun(self, eid: str) -> str:
        return urun_adi(self.k.uygulama(eid))

    def _sifirla(self) -> None:
        self.iddia: list[tuple] = []
        self.sentez: list[tuple] = []
        self.cikarilmayan: list[tuple] = []
        self.sekil_listesi: list[tuple] = []   # (no, eid, etiket)

    # -- sekil numaralari --------------------------------------------------
    @staticmethod
    def sekilleri(s: dict) -> list[tuple[str, str]]:
        """Sayfanin basim sirasindaki (anahtar, eid) listesi."""
        tur = s["tur"]
        out = []
        if tur in ("soru", "yanyana"):
            for i, f in enumerate(s.get("sekiller", [])):
                out.append((f.get("k") or f"{s.get('no', 'x')}-{i}", f["e"]))
            if s.get("kirpintilar"):
                out.append((s.get("kirpinti_k") or f"{s.get('no', 'x')}-kirp", s["kirpintilar"][0][0]))
        elif tur == "serit":
            for i, kp in enumerate(s["kirpintilar"]):
                out.append((kp.get("k") or f"{s.get('no', 'x')}-s{i}", kp["e"]))
        elif tur == "kartlar":
            for i, kart in enumerate(s["kartlar"]):
                if kart.get("sekil"):
                    out.append((kart["sekil"].get("k") or f"kart-{kart['urun']}", kart["sekil"]["e"]))
        return out

    def _numaralandir(self) -> None:
        n = 0
        for s in self.C.SAYFALAR:
            for anahtar, _eid in self.sekilleri(s):
                if anahtar in self.sekil_no:
                    raise SystemExit(f"şekil anahtarı iki kez kullanıldı: {anahtar}")
                n += 1
                self.sekil_no[anahtar] = f"{self.C.NO}.{n}"

    def m(self, metin: str) -> str:
        def degistir(mt):
            a = mt.group(1)
            if a not in self.sekil_no:
                raise SystemExit(f"bilinmeyen şekil göndermesi [[{a}]] — Bölüm {self.C.NO}")
            return f"Şekil {self.sekil_no[a]}"
        return _ATIF.sub(degistir, metin)

    # -- gorsel hazirlama ----------------------------------------------------
    def gorsel(self, eid: str, isaretler=(), *, kirpma=None, ek: str = "") -> pathlib.Path:
        kirpma = kirpma or getattr(self.C, "KIRPMA", {}).get(eid)
        karartma = karartma_kutulari(self.C, eid)
        if kirpma and karartma:
            x0, y0 = kirpma[0], kirpma[1]
            karartma_k = [(a - x0, b - y0, c - x0, d - y0) for a, b, c, d in karartma]
        else:
            karartma_k = karartma
        anahtar = (eid, tuple(isaretler), kirpma, ek)
        if anahtar in self._gorsel:
            return self._gorsel[anahtar]
        ad = f"-{ek}" if ek else f"-{len(self._gorsel)}"
        if isaretler:
            yol = M.isaretle(self.k, eid, isaretler, self.isaretli, ek=ad,
                             kirpma=kirpma, karartma=karartma_k)
        else:
            yol = M.sade(self.k, eid, self.isaretli, ek=ad, kirpma=kirpma, karartma=karartma_k)
        _kucult(yol)
        self._gorsel[anahtar] = yol
        return yol

    def kirpinti(self, eid: str, kutu, ek: str) -> pathlib.Path:
        anahtar = (eid, "kirp", tuple(kutu), ek)
        if anahtar in self._gorsel:
            return self._gorsel[anahtar]
        karartma = karartma_kutulari(self.C, eid)
        yol = M.kirp(self.k, eid, kutu, self.isaretli, ek=f"-{ek}", karartma=karartma)
        self._gorsel[anahtar] = yol
        return yol

    # -- basim -----------------------------------------------------------
    def ciz(self, t: M.Tuval, *, sayfa_haritasi: list[int] | None = None) -> None:
        """Bolumu verilen tuvale basar. sayfa_haritasi ilk geciste toplanir."""
        self._sifirla()
        self.sayfa_no = []
        self._harita = sayfa_haritasi
        for i, s in enumerate(self.C.SAYFALAR):
            self.sayfa_no.append(t.no + 1)
            cizici = {
                "acilis": self._acilis, "soru": self._soru, "yanyana": self._yanyana,
                "serit": self._serit, "kartlar": self._kartlar, "tablo": self._tablo,
                "metin": self._metin,
            }[s["tur"]]
            cizici(t, s)
            for c in s.get("cikarilmayan", []):
                self.cikarilmayan.append((s.get("no") or s.get("baslik", ""), c))

    # -- ortak parcalar ------------------------------------------------------
    def _yeni(self, t: M.Tuval) -> None:
        t.ust_metin = UST
        t.yeni(f"Bölüm {self.C.NO} · {self.C.BASLIK}")

    def _baslik(self, t: M.Tuval, no: str, metin: str, y: float = 46) -> float:
        if no:
            w = genislik(no, 17, True)
            t.yazi(M.KENAR, y, w + 4, no, boy=17, font="sgb", renk=M.VURGU)
            t.yazi(M.KENAR + w + 12, y + 2, M.ICERIK_W - w - 12, metin, boy=15, font="sgb")
        else:
            t.yazi(M.KENAR, y, M.ICERIK_W, metin, boy=15, font="sgb")
        return y + 27

    def _giris(self, t: M.Tuval, y: float, metin: str) -> float:
        if not metin:
            return y
        return t.giris(y, self.m(metin))

    def rozet(self, t: M.Tuval, x: float, y: float, tur: str, w: float = 60) -> None:
        t.kutu(x, y, x + w, y + 11, dolgu=TUR_RENK[tur])
        t.yazi(x + 1, y + 1.6, w - 2, M.TURLER[tur], boy=6.2, font="sgb", renk=M.BEYAZ, hiza=1)

    def _tasma(self, t: M.Tuval, y: float, etiket: str) -> None:
        if y > ALT_SINIR + 1:
            M.uyarilar.append(f"sayfa {t.no}: {etiket} sayfa altına taştı ({y:.0f} > {ALT_SINIR:.0f})")

    def _urunler(self, t: M.Tuval, x: float, y: float, w: float, s: dict,
                 baslik: str = "Aynı soruda diğer ürünler") -> float:
        satirlar = s.get("urunler", [])
        if not satirlar:
            return y
        t.yazi(x, y, w, s.get("urunler_baslik", baslik), boy=7.8, font="sgb", renk=M.VURGU)
        y += 13
        for satir in satirlar:
            ad, tur, cumle = satir[0], satir[1], satir[2]
            dayanak = satir[3] if len(satir) > 3 else "—"
            self.rozet(t, x, y + 0.5, tur)
            metin = self.m(f"{ad} — {cumle}")
            h = t.olc(w - 66, metin, boy=7.8)
            t.yazi(x + 66, y, w - 66, metin, boy=7.8, renk=M.MUREKKEP)
            self.iddia.append((f"{s.get('no', '')} · {ad}", self.m(cumle), dayanak, tur))
            y += max(h, 12) + 3.5
        return y

    def _notlar(self, t: M.Tuval, x: float, y: float, w: float, s: dict,
                kaynaklar: list[str]) -> float:
        notlar = s.get("notlar", [])
        if not notlar:
            return y
        metinler = []
        for n in notlar:
            if isinstance(n, tuple):
                metin, dayanak = n
            else:
                metin, dayanak = n, ", ".join(sorted(set(kaynaklar))) or "—"
            metin = self.m(metin)
            metinler.append(metin)
            self.sentez.append((f"{s.get('no', '')} · not", metin, dayanak))
        return t.maddeler(x, y, w, metinler, boy=8.0, renk=M.SOLUK)

    def _dayanak(self, t: M.Tuval, s: dict, eidler: list[str]) -> None:
        parca = []
        canli = sorted({e for e in eidler if self.tur(e) == "canli"})
        kaynak = sorted({e for e in eidler if self.tur(e) == "kaynak"})
        if canli:
            parca.append("Canlı kare: " + ", ".join(canli))
        if kaynak:
            parca.append("Kaynak görseli: " + ", ".join(kaynak))
        for ek in s.get("dayanak", []):
            parca.append(ek)
        if s.get("cikarilmayan"):
            parca.append("Çıkarılmayan sonuç: " + "; ".join(s["cikarilmayan"]))
        t.alt_bant(self.m(". ".join(parca) + ".") if parca else "")

    def _isaretler_kaydet(self, no: str, eid: str, isaretler) -> None:
        for k, _x, _y, metin in isaretler:
            self.iddia.append((f"Şekil {no} · işaret {k}", metin, eid, self.tur(eid)))

    def _sekil_kaydet(self, no: str, eid: str, etiket: str) -> None:
        self.sekil_listesi.append((no, eid, etiket))

    # -- ACILIS ------------------------------------------------------------
    def _acilis(self, t: M.Tuval, s: dict) -> None:
        C = self.C
        self._yeni(t)
        t.yazi(M.KENAR, 50, 300, f"Bölüm {C.NO}", boy=11, font="sgb", renk=M.VURGU)
        t.yazi(M.KENAR, 64, M.ICERIK_W, C.BASLIK, boy=26, font="sgb")
        t.cizgi(M.KENAR, 104, M.KENAR + 150, M.VURGU, 1.6)

        sol_w = 430.0
        y = 118.0
        if getattr(C, "ANA_SORU", ""):
            t.yazi(M.KENAR, y, sol_w, "Ana soru", boy=8.2, font="sgb", renk=M.VURGU)
            y += 12
            h = t.olc(sol_w, C.ANA_SORU, boy=11.5, font="sgb", satir=1.3)
            t.yazi(M.KENAR, y, sol_w, C.ANA_SORU, boy=11.5, font="sgb", satir=1.3)
            y += h + 10
        for p in C.GIRIS:
            p = self.m(p)
            h = t.olc(sol_w, p, boy=9.2, satir=1.42)
            t.yazi(M.KENAR, y, sol_w, p, boy=9.2, satir=1.42)
            y += h + 7
        if getattr(C, "GIRMEZ", None):
            y += 4
            t.yazi(M.KENAR, y, sol_w, "Bu bölüme girmez", boy=8.6, font="sgb", renk=M.VURGU)
            y += 14
            y = t.maddeler(M.KENAR, y, sol_w, C.GIRMEZ, boy=8.0, renk=M.SOLUK)
        self._tasma(t, y, "açılış sol sütun")

        sx = M.KENAR + sol_w + 36
        sw = M.SAYFA_W - M.KENAR - sx
        y = 118.0
        sorular = [(p.get("no", ""), p.get("baslik", ""), i) for i, p in enumerate(C.SAYFALAR)
                   if p["tur"] != "acilis" and p.get("haritada", True) and p.get("baslik")]
        if sorular:
            t.yazi(sx, y, sw, "Bu bölümdeki sorular", boy=9, font="sgb")
            y += 16
            gorulen = set()
            for no, baslik, i in sorular:
                if (no, baslik) in gorulen:
                    continue
                gorulen.add((no, baslik))
                sayfa = str(self._harita[i]) if self._harita else "—"
                t.yazi(sx, y, 30, no, boy=8.4, font="sgb", renk=M.VURGU)
                h = t.olc(sw - 70, baslik, boy=8.6)
                t.yazi(sx + 32, y, sw - 70, baslik, boy=8.6)
                t.yazi(sx + sw - 30, y, 30, sayfa, boy=8.0, renk=M.COK_SOLUK, hiza=2)
                y += max(h, 11) + 4
        if getattr(C, "KAPSAM", None):
            y += 10
            t.cizgi(sx, y - 4, sx + sw)
            t.yazi(sx, y + 2, sw, "Bu bölümde hangi ürün hangi kanıtla var", boy=9, font="sgb")
            y += 18
            for urun, turler, notu in C.KAPSAM:
                t.yazi(sx, y, 104, urun, boy=7.8, font="sgb")
                xx = sx + 106
                for tr in turler:
                    self.rozet(t, xx, y + 0.3, tr, w=58)
                    xx += 61
                y += 12.5
                if notu:
                    h = t.olc(sw - 106, notu, boy=7.0)
                    t.yazi(sx + 106, y - 0.5, sw - 106, notu, boy=7.0, renk=M.COK_SOLUK)
                    y += h + 1
                y += 2
        self._tasma(t, y, "açılış sağ sütun")
        t.alt_bant(getattr(C, "ACILIS_DAYANAK", "Kanıt türleri her cümleye ayrı bağlanır; "
                   "anahtar Bölüm 1'dedir. \"Görülmedi\", özelliğin bulunmadığı anlamına gelmez."))

    # -- SORU (Kalip A) ------------------------------------------------------
    def _soru(self, t: M.Tuval, s: dict) -> None:
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        ust = y + 2

        sekiller = s.get("sekiller", [])
        anahtarlar = self.sekilleri(s)
        hazir = []
        for i, f in enumerate(sekiller):
            yol = self.gorsel(f["e"], f.get("isaretler", ()), kirpma=f.get("kirpma"))
            with Image.open(yol) as im:
                oran = im.width / im.height
            hazir.append((f, yol, oran))

        n_tel = sum(1 for _f, _y, o in hazir if o < 1)
        alt_pay = 24
        uygun_h = ALT_SINIR - ust - alt_pay
        hedef = s.get("yukseklik") or {0: 0, 1: 404, 2: 384, 3: 350, 4: 300}.get(n_tel, 280)
        h_tel = min(hedef, uygun_h)
        not_w_min = s.get("not_genislik", 250)

        def olcule(h_tel):
            ws = []
            for f, _yol, oran in hazir:
                if oran < 1:
                    ws.append((h_tel * oran, h_tel))
                else:
                    w = f.get("genislik", 470)
                    h = w / oran
                    if h > uygun_h:
                        h = uygun_h
                        w = h * oran
                    ws.append((w, h))
            return ws

        dikey = s.get("dikey", False)
        boyut = olcule(h_tel)
        sol_w = sum(w for w, _h in boyut) + BOSLUK * max(0, len(boyut) - 1)
        if dikey:
            boyut = [(f.get("genislik", 320), f.get("genislik", 320) / o) for f, _y, o in hazir]
            sol_w = max(w for w, _h in boyut)
        while not dikey and sekiller and M.ICERIK_W - sol_w - 22 < not_w_min and h_tel > 150:
            h_tel -= 8
            boyut = olcule(h_tel)
            sol_w = sum(w for w, _h in boyut) + BOSLUK * max(0, len(boyut) - 1)

        x = M.KENAR
        fy = ust
        eidler = []
        tum_isaretler = []
        for (f, yol, _oran), (w, h), (anahtar, eid) in zip(hazir, boyut, anahtarlar):
            no = self.sekil_no[anahtar]
            y0 = fy if dikey else ust
            t.resim(yol, x, y0, w, h)
            etiket = f"Şekil {no} · {f['etiket']}"
            t.yazi(x, y0 + h + 3, max(w, 120), etiket, boy=6.9, renk=M.SOLUK, satir=1.22)
            if dikey:
                fy += h + 20
                self._tasma(t, fy, f"{s.get('no')} dikey şekiller")
            self._sekil_kaydet(no, eid, f["etiket"])
            self._isaretler_kaydet(no, eid, f.get("isaretler", ()))
            for sat in f.get("satirlar", ()):
                self.iddia.append((f"Şekil {no}", sat, eid, self.tur(eid)))
            tum_isaretler.extend(f.get("isaretler", ()))
            eidler.append(eid)
            if not dikey:
                x += w + BOSLUK

        sx = M.KENAR + (sol_w + 22 if sekiller else 0)
        sw = M.SAYFA_W - M.KENAR - sx
        yy = ust

        if s.get("kirpintilar"):
            yy = self._kirpinti_grubu(t, sx, yy, sw, s, eidler)

        if tum_isaretler:
            yy = t.numarali_notlar(sx, yy, sw, tum_isaretler, boy=8.1)
        if s.get("notlar"):
            if tum_isaretler or s.get("kirpintilar"):
                yy += 4
                t.cizgi(sx, yy, sx + sw)
                yy += 6
            yy = self._notlar(t, sx, yy, sw, s, eidler)
        if s.get("urunler"):
            yy += 6
            yy = self._urunler(t, sx, yy, sw, s)
        self._tasma(t, yy, f"{s.get('no')} sağ sütun")
        self._dayanak(t, s, eidler)

    def _kirpinti_grubu(self, t, sx, yy, sw, s, eidler) -> float:
        anahtar = self.sekilleri(s)[-1][0]
        no = self.sekil_no[anahtar]
        kutu_h = s.get("kirpinti_h", 58.0)
        t.yazi(sx, yy, sw, s.get("kirpinti_baslik", ""), boy=8.2, font="sgb")
        yy += 14
        xx = sx
        for i, (eid, ad, kutu) in enumerate(s["kirpintilar"]):
            yol = self.kirpinti(eid, kutu, f"k{t.no}-{i}")
            kw = kutu_h * (kutu[2] - kutu[0]) / (kutu[3] - kutu[1])
            if kw > sw:
                kw = sw
            if xx > sx and xx + kw > sx + sw:
                xx = sx
                yy += kutu_h + 20
            kh = kw * (kutu[3] - kutu[1]) / (kutu[2] - kutu[0])
            t.resim(yol, xx, yy, kw, kh)
            t.yazi(xx, yy + kutu_h + 2, kw + 14, ad, boy=6.6, renk=M.SOLUK, satir=1.15)
            self.iddia.append((f"Şekil {no}", f"{ad}: {s.get('kirpinti_etiket', '')}", eid, self.tur(eid)))
            eidler.append(eid)
            xx += kw + 14
        self._sekil_kaydet(no, s["kirpintilar"][0][0], s.get("kirpinti_etiket", ""))
        for eid, ad, _k in s["kirpintilar"][1:]:
            self._sekil_kaydet(no, eid, s.get("kirpinti_etiket", ""))
        yy += kutu_h + 22
        et = f"Şekil {no} · {s.get('kirpinti_etiket', '')}"
        h = t.olc(sw, et, boy=6.9, satir=1.22)
        t.yazi(sx, yy, sw, et, boy=6.9, renk=M.COK_SOLUK, satir=1.22)
        return yy + h + 8

    # -- YAN YANA (Kalip B) ----------------------------------------------------
    def _yanyana(self, t: M.Tuval, s: dict) -> None:
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        ust = y + 2
        sekiller = s["sekiller"]
        n = len(sekiller)
        anahtarlar = self.sekilleri(s)
        sutun = (M.ICERIK_W - BOSLUK * (n - 1)) / n
        h = s.get("yukseklik", 270.0)
        w_max = min(h / M.TELEFON_ORAN, sutun)
        h = w_max * M.TELEFON_ORAN
        eidler = []
        alt = ust
        for i, (f, (anahtar, eid)) in enumerate(zip(sekiller, anahtarlar)):
            no = self.sekil_no[anahtar]
            x = M.KENAR + i * (sutun + BOSLUK)
            yol = self.gorsel(eid, f.get("isaretler", ()), kirpma=f.get("kirpma"))
            with Image.open(yol) as im:
                oran = im.width / im.height
            w = h * oran
            ix = x + (sutun - w) / 2
            t.resim(yol, ix, ust, w, h)
            yy = ust + h + 4
            t.yazi(x, yy, sutun, f["ad"], boy=8.4, font="sgb", hiza=1)
            yy += 11
            et = f"Şekil {no} · {f['etiket']}"
            hh = t.olc(sutun, et, boy=6.6, satir=1.2)
            t.yazi(x, yy, sutun, et, boy=6.6, renk=M.COK_SOLUK, hiza=1, satir=1.2)
            yy += hh + 3
            for sat in f.get("satirlar", ()):
                hh = t.olc(sutun, sat, boy=7.2, satir=1.25)
                t.yazi(x, yy, sutun, sat, boy=7.2, renk=M.MUREKKEP, satir=1.25)
                yy += hh + 2
                self.iddia.append((f"Şekil {no}", sat, eid, self.tur(eid)))
            self._sekil_kaydet(no, eid, f["etiket"])
            self._isaretler_kaydet(no, eid, f.get("isaretler", ()))
            eidler.append(eid)
            alt = max(alt, yy)
        tum = [i for f in sekiller for i in f.get("isaretler", ())]
        y = alt + 4
        if tum or s.get("notlar") or s.get("urunler"):
            t.cizgi(M.KENAR, y, M.SAYFA_W - M.KENAR)
            y += 7
        sutun_w = (M.ICERIK_W - 26) / 2
        y_sol = y
        if tum:
            y_sol = t.numarali_notlar(M.KENAR, y, sutun_w, tum, boy=7.9)
        y_sol = self._notlar(t, M.KENAR, y_sol + (3 if tum else 0), sutun_w, s, eidler)
        y_sag = self._urunler(t, M.KENAR + sutun_w + 26, y, sutun_w, s)
        if s.get("sag_notlar"):
            y_sag = self._notlar(t, M.KENAR + sutun_w + 26, y_sag + 2, sutun_w,
                                 {"no": s.get("no"), "notlar": s["sag_notlar"]}, eidler)
        self._tasma(t, max(y_sol, y_sag), f"{s.get('no')} alt metin")
        self._dayanak(t, s, eidler)

    # -- SERIT (Kalip C) -----------------------------------------------------
    def _serit(self, t: M.Tuval, s: dict) -> None:
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        ust = y + 2
        anahtarlar = self.sekilleri(s)
        kirp_w = s.get("kirpinti_genislik", 380.0)
        # toplam yukseklik sigmazsa genisligi daralt
        oranlar = [(kp["kutu"][3] - kp["kutu"][1]) / (kp["kutu"][2] - kp["kutu"][0])
                   for kp in s["kirpintilar"]]
        gerek = sum(kirp_w * o for o in oranlar) + 8 * len(oranlar) + 14
        if ust + gerek > ALT_SINIR:
            kirp_w *= (ALT_SINIR - ust - 8 * len(oranlar) - 14) / (gerek - 8 * len(oranlar) - 14)
        metin_x = M.KENAR + kirp_w + 14
        metin_w = s.get("metin_genislik", 200.0)
        yy = ust
        eidler = []
        for i, (kp, (anahtar, eid)) in enumerate(zip(s["kirpintilar"], anahtarlar)):
            no = self.sekil_no[anahtar]
            yol = self.kirpinti(eid, kp["kutu"], f"s{t.no}-{i}")
            kh = kirp_w * oranlar[i]
            t.resim(yol, M.KENAR, yy, kirp_w, kh)
            t.yazi(metin_x, yy, metin_w, f"Şekil {no} · {kp['ad']}", boy=8.2, font="sgb")
            ty = yy + 12
            t.yazi(metin_x, ty, metin_w, kp["konum"], boy=7.4, font="sgb", renk=M.VURGU)
            ty += 10
            acik = self.m(kp["aciklama"])
            hh = t.olc(metin_w, acik, boy=7.6, satir=1.25)
            t.yazi(metin_x, ty, metin_w, acik, boy=7.6, renk=M.SOLUK, satir=1.25)
            self.iddia.append((f"Şekil {no}", f"{kp['ad']} · {kp['konum']}: {acik}", eid, self.tur(eid)))
            self._sekil_kaydet(no, eid, f"{kp['ad']} · {kp['konum']}")
            eidler.append(eid)
            yy += max(kh, ty + hh - yy) + 8
        not_metin = s.get("serit_notu", "Kırpıntılar aynı genişlikte ve her ürünün kendi ekranından alındı; "
                                        "kırpma dışında değiştirilmedi.")
        t.yazi(M.KENAR, yy, kirp_w + 14 + metin_w, self.m(not_metin), boy=7.2,
               renk=M.COK_SOLUK, satir=1.3)
        self._tasma(t, yy + 10, f"{s.get('no')} şerit")
        nx = metin_x + metin_w + 22
        nw = M.SAYFA_W - M.KENAR - nx
        ny = ust
        if s.get("notlar"):
            t.yazi(nx, ny, nw, s.get("notlar_baslik", "Kareden okunanlar"), boy=8.6, font="sgb")
            ny = self._notlar(t, nx, ny + 15, nw, s, eidler)
        if s.get("urunler"):
            ny = self._urunler(t, nx, ny + 6, nw, s)
        self._tasma(t, ny, f"{s.get('no')} şerit notları")
        self._dayanak(t, s, eidler)

    # -- KARTLAR (Kalip D) ---------------------------------------------------
    def _kartlar(self, t: M.Tuval, s: dict) -> None:
        self._yeni(t)
        y = 46.0
        if s.get("baslik"):
            y = self._baslik(t, s.get("no", ""), s["baslik"])
            y = self._giris(t, y, s.get("giris", ""))
        kartlar = s["kartlar"]
        n = len(kartlar)
        ara = 20.0
        kw = (M.ICERIK_W - ara * (n - 1)) / n
        anahtarlar = dict(self.sekilleri(s))
        eidler = []
        alt = y
        for i, kart in enumerate(kartlar):
            x = M.KENAR + i * (kw + ara)
            yy = y
            t.kutu(x, yy, x + kw, yy + 2.2, dolgu=M.VURGU)
            yy += 7
            t.yazi(x, yy, kw, kart["urun"], boy=13, font="sgb")
            yy += 18
            if kart.get("alt"):
                h = t.olc(kw, kart["alt"], boy=7.8)
                t.yazi(x, yy, kw, kart["alt"], boy=7.8, renk=M.SOLUK)
                yy += h + 5
            icx, icw = x, kw
            if kart.get("sekil"):
                f = kart["sekil"]
                anahtar = f.get("k") or f"kart-{kart['urun']}"
                no = self.sekil_no[anahtar]
                yol = self.gorsel(f["e"], f.get("isaretler", ()), kirpma=f.get("kirpma"))
                with Image.open(yol) as im:
                    oran = im.width / im.height
                if oran < 1:
                    ih = f.get("yukseklik", 250.0)
                    iw = ih * oran
                    t.resim(yol, x, yy, iw, ih)
                    t.yazi(x, yy + ih + 3, iw + 10, f"Şekil {no} · {f['etiket']}", boy=6.5,
                           renk=M.COK_SOLUK, satir=1.2)
                    icx, icw = x + iw + 12, kw - iw - 12
                    resim_alt = yy + ih + 22
                else:
                    iw = min(kw, f.get("genislik", kw))
                    ih = iw / oran
                    t.resim(yol, x, yy, iw, ih)
                    t.yazi(x, yy + ih + 3, iw, f"Şekil {no} · {f['etiket']}", boy=6.5,
                           renk=M.COK_SOLUK, satir=1.2)
                    yy += ih + 18
                    resim_alt = yy
                self._sekil_kaydet(no, f["e"], f["etiket"])
                self._isaretler_kaydet(no, f["e"], f.get("isaretler", ()))
                eidler.append(f["e"])
            else:
                resim_alt = yy
            ay = yy
            for ad, metin, tur, *dayanak in kart["alanlar"]:
                metin = self.m(metin)
                t.yazi(icx, ay, icw, ad, boy=7.2, font="sgb", renk=M.VURGU)
                if tur:
                    self.rozet(t, icx + icw - 58, ay - 0.5, tur, w=58)
                ay += 11
                h = t.olc(icw, metin, boy=7.8, satir=1.3)
                t.yazi(icx, ay, icw, metin, boy=7.8, satir=1.3)
                ay += h + 5
                if tur:
                    self.iddia.append((f"{s.get('no', '')} · {kart['urun']} · {ad}", metin,
                                       dayanak[0] if dayanak else "—", tur))
            alt = max(alt, ay, resim_alt)
            if kart.get("alt_alanlar"):
                ay = max(ay, resim_alt)
                for ad, metin, tur, *dayanak in kart["alt_alanlar"]:
                    metin = self.m(metin)
                    t.yazi(x, ay, kw, ad, boy=7.2, font="sgb", renk=M.VURGU)
                    if tur:
                        self.rozet(t, x + kw - 58, ay - 0.5, tur, w=58)
                    ay += 11
                    h = t.olc(kw, metin, boy=7.8, satir=1.3)
                    t.yazi(x, ay, kw, metin, boy=7.8, satir=1.3)
                    ay += h + 5
                    if tur:
                        self.iddia.append((f"{s.get('no', '')} · {kart['urun']} · {ad}", metin,
                                           dayanak[0] if dayanak else "—", tur))
                alt = max(alt, ay)
        self._tasma(t, alt, "kartlar")
        self._dayanak(t, s, eidler)

    # -- TABLO (Kalip E) -----------------------------------------------------
    def _tablo(self, t: M.Tuval, s: dict) -> None:
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        sutunlar = s["sutunlar"]
        toplam = sum(g for _a, g in sutunlar)
        tablo_w = s.get("tablo_genislik", M.ICERIK_W)
        ws = [tablo_w * g / toplam for _a, g in sutunlar]
        xs = [M.KENAR]
        for w in ws[:-1]:
            xs.append(xs[-1] + w)
        boy = s.get("boy", 7.4)

        def baslik_satiri(y):
            t.kutu(M.KENAR, y - 2, M.KENAR + tablo_w, y + 12, dolgu=(0.95, 0.95, 0.96))
            for (ad, _g), x, w in zip(sutunlar, xs, ws):
                t.yazi(x + 3, y, w - 6, ad, boy=7.2, font="sgb", renk=M.SOLUK)
            return y + 16

        y = baslik_satiri(y + 2)
        for satir in s["satirlar"]:
            if isinstance(satir, str):
                t.yazi(M.KENAR, y + 2, tablo_w, satir, boy=7.6, font="sgb", renk=M.VURGU)
                y += 15
                continue
            yuk = 0.0
            for hucre, w in zip(satir, ws):
                metin, tur = _hucre(hucre)
                pay = 13 if tur else 0
                yuk = max(yuk, t.olc(w - 6, self.m(metin), boy=boy, satir=1.25) + pay)
            for i, (hucre, x, w) in enumerate(zip(satir, xs, ws)):
                metin, tur = _hucre(hucre)
                metin = self.m(metin)
                yy = y
                if tur:
                    self.rozet(t, x + 3, yy, tur, w=min(58, w - 6))
                    yy += 13
                t.yazi(x + 3, yy, w - 6, metin, boy=boy, satir=1.25,
                       font="sgb" if i == 0 else "sg")
                if tur and i > 0:
                    d = hucre.get("d", "—") if isinstance(hucre, dict) else "—"
                    self.iddia.append((f"{s.get('no', '')} · {_hucre(satir[0])[0]} · {sutunlar[i][0]}",
                                       metin, d, tur))
            y += yuk + 4
            t.cizgi(M.KENAR, y - 2, M.KENAR + tablo_w)
        y += 4
        if s.get("notlar"):
            y = self._notlar(t, M.KENAR, y, tablo_w, s, [])
        if s.get("urunler"):
            y = self._urunler(t, M.KENAR, y + 4, tablo_w, s)
        self._tasma(t, y, f"{s.get('no')} tablo")
        self._dayanak(t, s, [])

    # -- METIN ---------------------------------------------------------------
    def _metin(self, t: M.Tuval, s: dict) -> None:
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        n = s.get("sutun", 3)
        ara = 22.0
        sw = (M.ICERIK_W - ara * (n - 1)) / n
        ys = [y + 2] * n
        for blok in s["bloklar"]:
            i = ys.index(min(ys))
            x = M.KENAR + i * (sw + ara)
            yy = ys[i]
            t.kutu(x, yy, x + sw, yy + 1.6, dolgu=M.CIZGI)
            yy += 6
            t.yazi(x, yy, sw - 66, blok["baslik"], boy=9.4, font="sgb")
            if blok.get("tur"):
                self.rozet(t, x + sw - 60, yy + 1, blok["tur"])
            yy += 16
            for p in blok["metin"]:
                if isinstance(p, tuple):
                    p, tur, d = p
                else:
                    tur, d = blok.get("tur"), blok.get("d", "—")
                p = self.m(p)
                h = t.olc(sw, p, boy=8.0, satir=1.35)
                t.yazi(x, yy, sw, p, boy=8.0, satir=1.35)
                yy += h + 5
                if tur:
                    self.iddia.append((f"{s.get('no', '')} · {blok['baslik']}", p, d, tur))
            ys[i] = yy + 10
        y = max(ys)
        if s.get("notlar"):
            y = self._notlar(t, M.KENAR, y, M.ICERIK_W, s, [])
        if s.get("urunler"):
            y = self._urunler(t, M.KENAR, y + 4, M.ICERIK_W, s)
        self._tasma(t, y, f"{s.get('no')} metin")
        self._dayanak(t, s, [])

    # -- cikti ---------------------------------------------------------------
    def kullanilan_eidler(self) -> list[str]:
        return sorted({e for _n, e, _a in self.sekil_listesi})


def _hucre(h):
    if isinstance(h, dict):
        return h["t"], h.get("tur")
    if isinstance(h, tuple):
        return h[0], h[1]
    return h, None


def _kucult(yol: pathlib.Path, en_fazla: int = 1500) -> None:
    im = Image.open(yol)
    if max(im.size) > en_fazla:
        s = en_fazla / max(im.size)
        im = im.convert("RGB").resize((int(im.width * s), int(im.height * s)), Image.LANCZOS)
        im.save(yol, "PNG", optimize=True)


# Belge geneli kisisel veri karartmasi (ozgun kanit degismez). Bolum kendi
# KARARTMA sozlugunu ekleyebilir; ikisi birlestirilir.
KARARTMA_ORTAK: dict[str, list[tuple]] = {
    # Goodbudget ust cubugunda e-postadan tureyen hane adi.
    # 20 Eylul kosumunun kareleri ayni cozunurluk ve yerlesimdedir; ayni kutu gecerli.
    # E0419 ve E0424 "Fill Envelopes" basligindadir, hane adi tasimaz: listede yok.
    **{e: [(160, 170, 460, 250)] for e in (
        "E0111", "E0115", "E0121", "E0129", "E0130", "E0131",
        "E0417", "E0418", "E0420", "E0421", "E0422", "E0423",
        "E0494",                     # 24 Eylul: 39-islem-listesi, ayni ust cubuk
    )},
    # 24 Eylul: 41-ayarlar-ust, "Log Out" altindaki hane adi
    "E0495": [(50, 860, 330, 915)],
}
# Hesap sahibinin tam adini tasiyan kare hic basilmaz (yerine E0376).
YASAKLI = {"E0375": "Wallet çekmecesinin üstü hesap sahibinin adını taşıyor"}

BASILMAZ = ("parasut", "logo-isbasi", "quickbooks")   # K4


def karartma_kutulari(C, eid: str) -> list[tuple]:
    return list(KARARTMA_ORTAK.get(eid, [])) + list(getattr(C, "KARARTMA", {}).get(eid, []))


def kare_turu(k: M.Kanit, eid: str) -> str:
    """Basilan karenin kanit turu. K4 ve O1 burada zorlanir."""
    if eid in YASAKLI:
        raise SystemExit(f"{eid} basılmaz: {YASAKLI[eid]}")
    uyg = k.uygulama(eid)
    yol = k.kayit[eid]["yol"]
    if uyg in BASILMAZ:
        raise SystemExit(f"{eid}: {uyg} karesi basılmaz (plan K4)")
    if uyg == "kolaybi":
        if "-destek-" not in yol:
            raise SystemExit(f"{eid}: KolayBi video/giriş karesi basılmaz, yalnız anılır (plan Ö1)")
        return "kaynak"
    return "canli"


URUN_ADLARI = {
    "money-manager": "Money Manager", "bluecoins": "Bluecoins", "wallet-budgetbakers": "Wallet",
    "hesap-defterim": "Hesap Defterim", "goodbudget": "Goodbudget", "kolaybi": "KolayBi",
    "parasut": "Paraşüt", "logo-isbasi": "Logo İşbaşı", "quickbooks": "QuickBooks Solopreneur",
}


def urun_adi(kod: str | None) -> str:
    return URUN_ADLARI.get(kod or "", kod or "—")


# --------------------------------------------------------------------------
# Bir bolumu uret: iki gecis, PDF, Markdown, denetim dosyalari, onizleme
# --------------------------------------------------------------------------
def uret(C, klasor: pathlib.Path) -> Bolum:
    klasor = pathlib.Path(klasor)
    ad = f"bolum-{int(C.NO):02d}"
    pdf = klasor / f"{ad}.pdf"
    M.temizle(klasor / "isaretli")
    b = Bolum(C, klasor)

    # 1. gecis: sayfa haritasi
    taslak = pymupdf.open()
    b.ciz(M.Tuval(taslak, UST))
    harita = list(b.sayfa_no)
    taslak.close()
    M.uyarilar.clear()

    belge = pymupdf.open()
    b.ciz(M.Tuval(belge, UST), sayfa_haritasi=harita)
    belge.set_metadata({"title": f"Belge 1 · Bölüm {C.NO} · {C.BASLIK}", "author": "",
                        "subject": "Rakip arayüz araştırması", "keywords": "rakip arayüz"})
    belge.save(str(pdf), deflate=True, garbage=3)
    belge.close()

    dogrulama = b.k.dogrula(b.kullanilan_eidler() + list(getattr(C, "EK_KANITLAR", [])))
    baslik = f"Belge 1 · Bölüm {C.NO} · {C.BASLIK}"
    cikarilmayan = b.cikarilmayan + [(f"Bölüm {C.NO}", c) for c in getattr(C, "CIKARILMAYAN", [])]
    M.iddia_tablosu_yaz(klasor / "iddia-tablosu.md", baslik, b.iddia, b.sentez, cikarilmayan,
                        getattr(C, "EK_NOTLAR", ()))
    M.eksik_listesi_yaz(klasor / "eksik-listesi.md", baslik, getattr(C, "EKSIKLER", []))
    M.kaynaklar_yaz(klasor / "kaynaklar.md", baslik, b.k,
                    [(no, e) for no, e, _a in b.sekil_listesi], dogrulama,
                    getattr(C, "KAYNAK_NOTLARI", ()))
    M.manifest_yaz(klasor / "kanit-manifest.json", baslik, "2026-09-16", dogrulama, b.isaretli)
    markdown_yaz(b, klasor / f"{ad}.md")

    sonuc = M.kontrol(pdf, gizli_sozcukler=GIZLI)
    n = M.onizleme(pdf, klasor / "onizleme", dpi=110)
    M.rapor_yaz(ad, pdf, sonuc, dogrulama)
    print(f"Önizleme: {n} PNG")
    return b


def markdown_yaz(b: Bolum, hedef: pathlib.Path) -> None:
    C = b.C
    m = b.m
    s = [f"# Bölüm {C.NO} · {C.BASLIK}", "", f"{UST} · {TARIH}", "",
         "> PDF ile aynı içeriğin okunabilir kopyası; ikisi de `icerik.py`den üretilir.",
         "> İşaretler ve sayfa düzeni yalnız PDF'te görünür.", ""]
    if getattr(C, "ANA_SORU", ""):
        s += [f"**Ana soru.** {C.ANA_SORU}", ""]
    s += [m(p) + "\n" for p in C.GIRIS]
    if getattr(C, "GIRMEZ", None):
        s += ["**Bu bölüme girmez**", ""] + [f"- {g}" for g in C.GIRMEZ] + [""]
    if getattr(C, "KAPSAM", None):
        s += ["| Ürün | Kanıt | Not |", "|---|---|---|"]
        s += [f"| {u} | {', '.join(M.TURLER[x] for x in tr)} | {n or ''} |" for u, tr, n in C.KAPSAM]
        s.append("")

    def urunler(p):
        out = []
        if p.get("urunler"):
            out += ["", f"**{p.get('urunler_baslik', 'Aynı soruda diğer ürünler')}**", ""]
            out += [f"- *{M.TURLER[r[1]]}* · **{r[0]}** — {m(r[2])}" for r in p["urunler"]]
        return out

    def notlar(p, anahtar="notlar"):
        return [f"- {m(n[0] if isinstance(n, tuple) else n)}" for n in p.get(anahtar, [])]

    for p in C.SAYFALAR:
        tur = p["tur"]
        if tur == "acilis":
            continue
        baslik = f"{p.get('no', '')} · {p['baslik']}" if p.get("no") else p.get("baslik", "")
        if baslik:
            s += ["", f"## {baslik}", ""]
        if p.get("giris"):
            s += [m(p["giris"]), ""]
        anahtarlar = b.sekilleri(p)
        if tur in ("soru", "yanyana"):
            for f, (a, e) in zip(p.get("sekiller", []), anahtarlar):
                s += [f"**Şekil {b.sekil_no[a]} · {f['etiket']}** ({e})", ""]
                s += [f"{k}. {x}" for k, _a, _b, x in f.get("isaretler", ())]
                s += [f"- {x}" for x in f.get("satirlar", ())]
                s.append("")
            if p.get("kirpintilar"):
                a = anahtarlar[-1][0]
                s += [f"**Şekil {b.sekil_no[a]} · {p.get('kirpinti_etiket', '')}** — " +
                      ", ".join(f"{ad} ({e})" for e, ad, _k in p["kirpintilar"]), ""]
        elif tur == "serit":
            for kp, (a, e) in zip(p["kirpintilar"], anahtarlar):
                s.append(f"- **Şekil {b.sekil_no[a]} · {kp['ad']}** ({e}) — *{kp['konum']}.* {m(kp['aciklama'])}")
            s.append("")
        elif tur == "kartlar":
            for kart in p["kartlar"]:
                s += [f"### {kart['urun']}", ""]
                if kart.get("alt"):
                    s += [kart["alt"], ""]
                if kart.get("sekil"):
                    f = kart["sekil"]
                    a = f.get("k") or f"kart-{kart['urun']}"
                    s += [f"*Şekil {b.sekil_no[a]} · {f['etiket']}* ({f['e']})", ""]
                for ad, metin, tr, *_d in kart["alanlar"] + kart.get("alt_alanlar", []):
                    etiket = f" *({M.TURLER[tr]})*" if tr else ""
                    s.append(f"- **{ad}**{etiket} — {m(metin)}")
                s.append("")
        elif tur == "tablo":
            s.append("| " + " | ".join(a for a, _g in p["sutunlar"]) + " |")
            s.append("|" + "---|" * len(p["sutunlar"]))
            for satir in p["satirlar"]:
                if isinstance(satir, str):
                    s.append(f"| **{satir}** |" + " |" * (len(p["sutunlar"]) - 1))
                    continue
                hucreler = []
                for h in satir:
                    metin, tr = _hucre(h)
                    metin = m(metin).replace("\n", " ")
                    hucreler.append(f"*{M.TURLER[tr]}* · {metin}" if tr else metin)
                s.append("| " + " | ".join(hucreler) + " |")
            s.append("")
        elif tur == "metin":
            for blok in p["bloklar"]:
                etiket = f" *({M.TURLER[blok['tur']]})*" if blok.get("tur") else ""
                s += [f"### {blok['baslik']}{etiket}", ""]
                for x in blok["metin"]:
                    if isinstance(x, tuple):
                        s += [f"{m(x[0])} *({M.TURLER[x[1]]})*", ""]
                    else:
                        s += [m(x), ""]
        s += notlar(p) + notlar(p, "sag_notlar") + urunler(p)
        dayanak = list(p.get("dayanak", []))
        if p.get("cikarilmayan"):
            dayanak.append("Çıkarılmayan sonuç: " + "; ".join(p["cikarilmayan"]))
        if dayanak:
            s += ["", "*Dayanak.* " + m(". ".join(dayanak)) + "."]
    s += ["", "## Şekil dizini", "", "| Şekil | Kimlik | Ürün | Tür | Etiket |", "|---|---|---|---|---|"]
    for no, e, et in b.sekil_listesi:
        s.append(f"| {no} | {e} | {b.urun(e)} | {M.TURLER[b.tur(e)]} | {et} |")
    hedef.write_text("\n".join(s) + "\n", encoding="utf-8")


def icerik_yukle(klasor: pathlib.Path):
    """Bolum klasorundeki icerik.py'yi benzersiz modul adiyla yukler."""
    import importlib.util
    klasor = pathlib.Path(klasor)
    spec = importlib.util.spec_from_file_location(f"icerik_{klasor.name.replace('-', '_')}",
                                                  klasor / "icerik.py")
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


def calistir(uret_dosyasi: str) -> None:
    klasor = pathlib.Path(uret_dosyasi).resolve().parent
    uret(icerik_yukle(klasor), klasor)
