# -*- coding: utf-8 -*-
"""Belge 2 · uretim altyapisi (v3).

TASARIM KARARI — v1'den fark:
    v1 kendi sayfa turlerini (matris/zincir/donem/sema) yazdi ve Belge 1'in
    kanitlanmis kalibini KULLANMADI. Sonuc: 83 sayfada 5 kare, kelimelerin
    %30'u "gorulmedi". v3 bunu tersine cevirir — `kalip.py` oldugu gibi
    kullanilir (soru / yanyana / serit / kartlar / tablo / metin) ve uzerine
    YALNIZ bir sey eklenir: `cikarim` sayfa turu.

Dil katmanlari (yeniden-kurgu K1):
    canli/kosum/kaynak/beyan   gozlem   — karede okunan
    cikarim                    CIKARIM  — kanittan cikan neden           [YENI]
    kazanc/kayip alani         degerlendirme — ne kazandiriyor/kaybettiriyor
    (karar "al/alma" Belge 3'te kalir; buraya girmez.)

Kapilar (K10) — ihlalinde uretim DURUR:
    G1  her bolum en az bir `cikarim` sayfasi tasir
    G2  her `cikarim` sayfasi dolu `mekanizma` ve `kazanc` tasir
    G3  bolum en az `C.EN_AZ_KARE` kare basar (varsayilan 4)
    G4  PDF metninde ic terim gecmez ("A kismi", "matris", "olay defteri", ...)
    G5  motorun kendi kapilari: kare hash, kirik atif, kisisel veri sozcugu
"""
from __future__ import annotations

import pathlib
import re
import sys

KLASOR = pathlib.Path(__file__).resolve().parent
sys.path.insert(0, str(KLASOR.parents[1] / "belge1" / "ortak"))

import kalip  # noqa: E402
import motor as M  # noqa: E402
import pymupdf  # noqa: E402
from PIL import Image  # noqa: E402

UST = "Belge 2 · Rakip finansal akışlar"
TARIH = "21 Eylül 2026"

kalip.UST = UST
kalip.TARIH = TARIH

# Yeni kanit turu: cikarim. Gozlemden ayri bir rozet tasir — okur neyin
# olculdugunu, neyin cikarildigini karistirmasin.
M.TURLER["cikarim"] = "Çıkarım"
kalip.TUR_RENK["cikarim"] = (0.36, 0.30, 0.52)

# G4 · belgede gecmeyecek ic terimler (yeniden-kurgu K5)
YASAK_TERIM = (
    "A kısmı", "C kısmı", "olay defteri", "konu bloğu", "Çıkarılmayan sonuç",
    "olay × etki", "matrisi", "matriste",
)

# G7 · kendi kavramlarimiz (yeniden-kurgu K12). Belge 2 rakibi anlatir; kendi
# modelimizle karsilastirma Belge 3'un isidir. Gozlem formlarinda "BusinessFinance
# karsiligi" sutunlari var ve bunlar kopyalanirsa belge Belge 3'e donusur.
YASAK_KAVRAM = (
    "BusinessFinance", "ADR 00", "TransactionScope", "CounterpartyCharge",
    "CounterpartyPayment", "BudgetTransaction", "RecurringTransaction",
    "ICurrentUser", "Aşama 05", "Aşama 07", "bizim modelimiz", "bizdeki",
)

ALT = kalip.ALT_SINIR


class KapiHatasi(SystemExit):
    """Kural ihlali. Uretimi durdurur; uyari degildir."""


# ---------------------------------------------------------------------------
# YENI SAYFA TURU · cikarim
# ---------------------------------------------------------------------------
# Telefon karesinde (1080x2400) ustteki durum cubugu (saat, pil) ~0-120 px, alttaki
# kaydirma cizgisi ~2330-2400 px. Iddia tasimazlar; kirpmasi yazilmamis telefon karesi
# bu kutuyla basilir. Kirpma yazilmissa o kullanilir. Isaretli karelere dokunulmaz:
# isaret koordinatlari kirpma sonrasi uzaydadir (motor.isaretle). Yalniz Belge 2.
TELEFON_KIRPMA = (0, 120, 1080, 2330)


class Bolum(kalip.Bolum):
    """kalip.Bolum + `cikarim` sayfa turu."""

    def _kirpma(self, f: dict):
        if f.get("kirpma"):
            return f["kirpma"]
        if f.get("isaretler"):
            return None
        with Image.open(self.k.yol(f["e"])) as im:
            return TELEFON_KIRPMA if im.size == (1080, 2400) else None

    def ciz(self, t: M.Tuval, *, sayfa_haritasi: list[int] | None = None) -> None:
        self._sifirla()
        self.sayfa_no = []
        self._harita = sayfa_haritasi
        for s in self.C.SAYFALAR:
            self.sayfa_no.append(t.no + 1)
            if s["tur"] == "cikarim":
                self._cikarim(t, s)
            elif s["tur"] == "akis":
                self._akis(t, s)
            else:
                {
                    "acilis": self._acilis, "soru": self._soru, "yanyana": self._yanyana,
                    "serit": self._serit, "kartlar": self._kartlar, "tablo": self._tablo,
                    "metin": self._metin,
                }[s["tur"]](t, s)

    # -- TABLO · rozet yalniz istisnada, iddia her dolu hucrede -------------
    def _tablo(self, t: M.Tuval, s: dict) -> None:
        """kalip._tablo, iki farkla:

        1. `canli` varsayilandir ve ROZET TASIMAZ — 25 ayni rozet tabloyu
           okunmaz yapiyordu; goz zayif kanita taksin diye rozet yalniz
           kaynak/beyan/gorulmedi/cikarim hucrelerinde durur.
        2. Rozetsiz hucre de iddia tablosuna girer; izlenebilirlik rozete
           degil, hucrenin `d` alanina baglidir.
        """
        gercek_rozet = self.rozet
        gizlenen: list = []

        def rozet(t_, x, y, tur, w=60):
            if tur == "canli":
                gizlenen.append(True)
                return
            gercek_rozet(t_, x, y, tur, w)

        eski_hucre = kalip._hucre

        def hucre(h):
            metin, tur = eski_hucre(h)
            return metin, (None if tur == "canli" else tur)

        self.rozet = rozet
        kalip._hucre = hucre
        try:
            super()._tablo(t, s)
        finally:
            self.rozet = gercek_rozet
            kalip._hucre = eski_hucre

        # rozeti gizlenen hucrelerin iddialari elle kaydedilir
        sutunlar = s["sutunlar"]
        for satir in s["satirlar"]:
            if isinstance(satir, str):
                continue
            for i, h in enumerate(satir):
                metin, tur = kalip._hucre(h)
                if i == 0 or tur != "canli":
                    continue
                d = h.get("d", "—") if isinstance(h, dict) else "—"
                self.iddia.append((f"{s.get('no', '')} · {kalip._hucre(satir[0])[0]} · "
                                   f"{sutunlar[i][0]}", self.m(metin), d, tur))

    # -- YANYANA · genisligi sutuna kirp ------------------------------------
    def _yanyana(self, t: M.Tuval, s: dict) -> None:
        """kalip._yanyana, tek farkla: kare genisligi SUTUNA KIRPILIR.

        Kalip yuksekligi sabit tutup `w = h * oran` hesapliyor. Telefon kareleri
        (oran ~0,45) sigar, ama masaustu karesi (oran ~1,6) sutunu asar ve yan
        karenin ustune biner. Belge 2 ayni satirda telefon ve masaustu karesini
        yan yana koydugu icin bu kirpma zorunlu.
        """
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        ust = y + 2
        sekiller = s["sekiller"]
        n = len(sekiller)
        anahtarlar = self.sekilleri(s)
        sutun = (M.ICERIK_W - kalip.BOSLUK * (n - 1)) / n
        h = s.get("yukseklik", 270.0)
        w_max = min(h / M.TELEFON_ORAN, sutun)
        h = w_max * M.TELEFON_ORAN

        # Once butun boyutlari olc: etiket hizasi EN YUKSEK karenin altina gelsin.
        # (Kirpilmis kareler nominal `h`den kisa kalir; sabit hiza bos bosluk birakiyordu.)
        boyut = []
        for f, (_a, eid) in zip(sekiller, anahtarlar):
            yol = self.gorsel(eid, f.get("isaretler", ()), kirpma=self._kirpma(f))
            with Image.open(yol) as im:
                oran = im.width / im.height
            w, hh = h * oran, h
            if w > sutun:                      # <-- kalipta olmayan kirpma
                w, hh = sutun, sutun / oran
            boyut.append((yol, w, hh))
        h_gorunen = max(hh for _y, _w, hh in boyut)

        eidler: list[str] = []
        alt = ust
        for i, (f, (anahtar, eid)) in enumerate(zip(sekiller, anahtarlar)):
            no = self.sekil_no[anahtar]
            x = M.KENAR + i * (sutun + kalip.BOSLUK)
            yol, w, hh = boyut[i]
            t.resim(yol, x + (sutun - w) / 2, ust, w, hh)
            yy = ust + h_gorunen + 4           # etiketler ortak hizada
            t.yazi(x, yy, sutun, f["ad"], boy=8.4, font="sgb", hiza=1)
            yy += 11
            et = f"Şekil {no} · {f['etiket']}"
            eh = t.olc(sutun, et, boy=6.6, satir=1.2)
            t.yazi(x, yy, sutun, et, boy=6.6, renk=M.COK_SOLUK, hiza=1, satir=1.2)
            yy += eh + 3
            for sat in f.get("satirlar", ()):
                sh = t.olc(sutun, sat, boy=7.2, satir=1.25)
                t.yazi(x, yy, sutun, sat, boy=7.2, renk=M.MUREKKEP, satir=1.25)
                yy += sh + 2
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

    # -- SORU · kalibin uzerine `satirlar` + `sag_notlar` -------------------
    def _soru(self, t: M.Tuval, s: dict) -> None:
        """kalip._soru'nun ayni duzeni, iki eksigi kapatilmis hali.

        kalip'in `soru` sayfasi Belge 1 icin yazildi: orada bir seklin iddialari
        `isaretler` ile, karenin ustune cizilen numarali baloncuklarla tasinir.
        Belge 2'nin kaynak sayfalari `yanyana`'nin anahtarlarini kullaniyor
        (`satirlar`, `sag_notlar`) ve kalip bu ikisini sessizce dusuruyordu:
        `satirlar` yalniz iddia tablosuna giriyor, sayfaya cizilmiyordu;
        `sag_notlar` hicbir yere gitmiyordu.

        Burada duzen aynen korunur, yalniz sag sutun dolar:
        sekil satirlari -> kirpintilar -> isaretler -> notlar -> urunler -> sag_notlar.
        `kalip.py` degistirilmez (README kurali); ek Belge 2'nin kendi katmaninda.
        """
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        ust = y + 2

        sekiller = s.get("sekiller", [])
        anahtarlar = self.sekilleri(s)
        hazir = []
        for f in sekiller:
            yol = self.gorsel(f["e"], f.get("isaretler", ()), kirpma=self._kirpma(f))
            with Image.open(yol) as im:
                oran = im.width / im.height
            hazir.append((f, yol, oran))

        n_tel = sum(1 for _f, _y, o in hazir if o < 1)
        uygun_h = ALT - ust - 24
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
        sol_w = sum(w for w, _h in boyut) + kalip.BOSLUK * max(0, len(boyut) - 1)
        if dikey:
            boyut = [(f.get("genislik", 320), f.get("genislik", 320) / o) for f, _y, o in hazir]
            sol_w = max(w for w, _h in boyut)
        while not dikey and sekiller and M.ICERIK_W - sol_w - 22 < not_w_min and h_tel > 150:
            h_tel -= 8
            boyut = olcule(h_tel)
            sol_w = sum(w for w, _h in boyut) + kalip.BOSLUK * max(0, len(boyut) - 1)

        x = M.KENAR
        fy = ust
        eidler: list[str] = []
        tum_isaretler: list = []
        sekil_satirlari: list[tuple[str, tuple]] = []
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
            satirlar = tuple(f.get("satirlar", ()))
            if satirlar:
                sekil_satirlari.append((no, satirlar))
            for sat in satirlar:
                self.iddia.append((f"Şekil {no}", sat, eid, self.tur(eid)))
            tum_isaretler.extend(f.get("isaretler", ()))
            eidler.append(eid)
            if not dikey:
                x += w + kalip.BOSLUK

        sx = M.KENAR + (sol_w + 22 if sekiller else 0)
        sw = M.SAYFA_W - M.KENAR - sx
        yy = ust

        if sekil_satirlari:
            # Baslik tek kareli sayfada da basilir: madde listesinin hangi sekle
            # ait oldugu her sayfada ayni bicimde yazili olsun.
            for i, (no, satirlar) in enumerate(sekil_satirlari):
                if i:
                    yy += 5
                t.yazi(sx, yy, sw, f"Şekil {no}", boy=7.6, font="sgb", renk=M.SOLUK)
                yy += 11
                yy = t.maddeler(sx, yy, sw, [self.m(v) for v in satirlar],
                                boy=7.9, renk=M.MUREKKEP)
            yy += 4
            t.cizgi(sx, yy, sx + sw)
            yy += 7

        if s.get("kirpintilar"):
            yy = self._kirpinti_grubu(t, sx, yy, sw, s, eidler)

        if tum_isaretler:
            yy = t.numarali_notlar(sx, yy, sw, tum_isaretler, boy=8.1)
        # `sag_notlar` urun blogunun USTUNDE, notlarin devami olarak basilir.
        # Altta tek madde kalinca "kaynaktan okunan uc urun" basliginin altinda
        # asili duruyordu; ikisi ayni madde listesi olunca duzen kapaniyor.
        notlar = list(s.get("notlar", ())) + list(s.get("sag_notlar", ()))
        if notlar:
            if tum_isaretler or s.get("kirpintilar"):
                yy += 4
                t.cizgi(sx, yy, sx + sw)
                yy += 6
            yy = self._notlar(t, sx, yy, sw, {"no": s.get("no"), "notlar": notlar}, eidler)
        if s.get("urunler"):
            yy += 6
            yy = self._urunler(t, sx, yy, sw, s)
        self._tasma(t, yy, f"{s.get('no')} sağ sütun")
        self._dayanak(t, s, eidler)

    def _yer(self, t: M.Tuval, y: float, gerek: float, s: dict, devam: str) -> float:
        """Sayfada `gerek` kadar yer yoksa yeni sayfa acar ve yeni y doner."""
        if y + gerek <= ALT:
            return y
        t.alt_bant("")  # biten sayfanin cizgisi ve sayfa numarasi
        self._yeni(t)
        return self._baslik(t, s.get("no", ""), f"{s['baslik']} · {devam}")

    # -- YENI SAYFA TURU · akis ---------------------------------------------
    def _akis(self, t: M.Tuval, s: dict) -> None:
        """Tek ortak yol, ayristigi noktalarda dallanma.

        v1'in hatasi her urune ayri sema cizip tabloyu cumleye cevirmekti. Burada
        TEK yol var: adimlar soldan saga ortak surectir, dal yalnizca urunlerin
        gercekten ayrildigi adimda acilir. Tablo NEYIN farkli oldugunu, bu sayfa
        surecin NERESINDE ayrildigini gosterir.
        """
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        X, W = M.KENAR, M.ICERIK_W
        adimlar = s["adimlar"]
        n = len(adimlar)
        ara = 20.0
        sw = (W - ara * (n - 1)) / n
        ust = y + 10

        # --- adim basliklari: numarali kutu + aradaki ok ------------------
        bh = 0.0
        for a in adimlar:
            bh = max(bh, t.olc(sw - 34, a["baslik"], boy=8.8, font="sgb", satir=1.25))
        bh = max(bh + 13, 30)
        for i, a in enumerate(adimlar):
            x = X + i * (sw + ara)
            t.kutu(x, ust, x + sw, ust + bh, dolgu=(0.925, 0.945, 0.975),
                   kenar=(0.76, 0.81, 0.88))
            t.yazi(x + 9, ust + 7, 20, str(i + 1), boy=11, font="sgb", renk=M.VURGU)
            t.yazi(x + 28, ust + 8, sw - 36, a["baslik"], boy=8.8, font="sgb", satir=1.25)
            if i < n - 1:
                oy = ust + bh / 2
                t.cizgi(x + sw + 3, oy, x + sw + ara - 8, (0.55, 0.60, 0.68), 1.0)
                t.kutu(x + sw + ara - 9, oy - 2.6, x + sw + ara - 3, oy + 2.6,
                       dolgu=(0.55, 0.60, 0.68))

        # --- dallar: her adimin altinda -----------------------------------
        alt = ust + bh
        for i, a in enumerate(adimlar):
            x = X + i * (sw + ara)
            yy = ust + bh + 12
            if a.get("ortak"):
                metin = self.m(a["ortak"])
                h = t.olc(sw - 14, metin, boy=8.0, satir=1.32)
                t.kutu(x, yy, x + sw, yy + h + 12, dolgu=(0.975, 0.975, 0.98))
                t.yazi(x + 7, yy + 6, sw - 14, metin, boy=8.0, satir=1.32, renk=M.SOLUK)
                self.sentez.append((f"{s.get('no', '')} · adım {i + 1}", metin, "—"))
                yy += h + 20
            for dal in a.get("dallar", []):
                urunler = dal["urunler"]
                metin = self.m(dal["metin"])
                hu = t.olc(sw - 16, urunler, boy=7.4, font="sgb", satir=1.22)
                hm = t.olc(sw - 16, metin, boy=8.0, satir=1.32)
                h = hu + hm + 15
                vurgu = dal.get("vurgu")
                t.kutu(x, yy, x + sw, yy + h,
                       dolgu=(0.995, 0.965, 0.945) if vurgu else (0.972, 0.974, 0.978),
                       kenar=(0.88, 0.76, 0.68) if vurgu else None)
                t.kutu(x, yy, x + 2.4, yy + h,
                       dolgu=M.VURGU if vurgu else (0.72, 0.75, 0.80))
                t.yazi(x + 9, yy + 5, sw - 16, urunler, boy=7.4, font="sgb",
                       renk=M.VURGU if vurgu else M.SOLUK, satir=1.2)
                t.yazi(x + 9, yy + hu + 8, sw - 16, metin, boy=8.0, satir=1.32)
                self.iddia.append((f"{s.get('no', '')} · adım {i + 1} · {urunler}",
                                   metin, dal.get("d", "—"), dal.get("tur", "canli")))
                yy += h + 7
            alt = max(alt, yy)

        self._tasma(t, alt, f"{s.get('no')} akış")
        y = alt + 6
        if s.get("notlar"):
            y = self._notlar(t, X, y, W, s, [])
        self._tasma(t, y, f"{s.get('no')} akış notu")
        self._dayanak(t, s, [])

    def _cikarim(self, t: M.Tuval, s: dict) -> None:
        self._yeni(t)
        y = self._baslik(t, s.get("no", ""), s["baslik"])
        y = self._giris(t, y, s.get("giris", ""))
        y += 4
        W = M.ICERIK_W
        X = M.KENAR

        # --- mekanizma: neden ayrisiyorlar -------------------------------
        for blok in s.get("mekanizma", []):
            # +16: "Dayanagi:" satiri blokla ayni sayfada kalsin. Aksi halde blok
            # sayfanin dibine sigip dayanagi tek basina sonraki sayfaya dusuyordu.
            gerek = 21.0 + 16.0 + sum(
                t.olc(W, self.m(p), boy=8.4, satir=1.38) + 4 for p in blok["metin"])
            y = self._yer(t, y, min(gerek, ALT - 80), s, "devam")
            t.kutu(X, y, X + 2.2, y + 11, dolgu=kalip.TUR_RENK["cikarim"])
            self.rozet(t, X + W - 58, y, "cikarim", w=58)
            t.yazi(X + 10, y, W - 72, blok["baslik"], boy=9.6, font="sgb")
            y += t.olc(W - 72, blok["baslik"], boy=9.6, font="sgb") + 5
            for p in blok["metin"]:
                p = self.m(p)
                h = t.olc(W, p, boy=8.4, satir=1.38)
                y = self._yer(t, y, h + 4, s, "devam")   # tek blok sayfadan uzun olabilir
                t.yazi(X, y, W, p, boy=8.4, satir=1.38)
                y += h + 4
            d = blok.get("dayanak", "—")
            y = self._yer(t, y, 16, s, "devam")
            t.yazi(X, y, W, f"Dayanağı: {d}", boy=6.9, renk=M.COK_SOLUK)
            self.iddia.append((f"{s.get('no', '')} · {blok['baslik']}",
                               self.m(blok["metin"][0]), d, "cikarim"))
            y += 16
        self._tasma(t, y, f"{s.get('no')} mekanizma")

        # --- ne kazandiriyor / ne kaybettiriyor --------------------------
        if s.get("kazanc"):
            # tabloyu bolmemeye calis: once tamaminin yuksekligini olc
            _uw, _kw = W * 0.19, (W * 0.81 - 14) / 2
            tam = 39.0 + sum(
                max(t.olc(_uw - 6, u, boy=7.6),
                    t.olc(_kw - 6, self.m(a), boy=7.6, satir=1.28),
                    t.olc(_kw - 6, self.m(k), boy=7.6, satir=1.28)) + 5
                for u, a, k in s["kazanc"])
            onceki = y
            y = self._yer(t, y, min(tam, ALT - 80), s, "ne kazandırıyor, ne kaybettiriyor")
            if y == onceki:  # sayfa kirilmadiysa ic baslik gerekli
                t.cizgi(X, y, X + W)
                y += 8
                t.yazi(X, y, W,
                       s.get("kazanc_baslik", "Bu yaklaşım ne kazandırıyor, ne kaybettiriyor"),
                       boy=9.0, font="sgb", renk=M.VURGU)
                y += 15
            else:
                y += 4
            uw, kw = W * 0.19, (W * 0.81 - 14) / 2

            def basliklar(y: float) -> float:
                t.kutu(X, y - 2, X + W, y + 12, dolgu=(0.95, 0.95, 0.96))
                t.yazi(X + 3, y, uw - 6, "Ürün", boy=7.2, font="sgb", renk=M.SOLUK)
                t.yazi(X + uw + 3, y, kw - 6, "Kazandırdığı", boy=7.2, font="sgb", renk=M.SOLUK)
                t.yazi(X + uw + kw + 17, y, kw - 6, "Kaybettirdiği", boy=7.2, font="sgb",
                       renk=M.SOLUK)
                return y + 16

            y = basliklar(y)
            for urun, kazanc, kayip in s["kazanc"]:
                hh = max(t.olc(uw - 6, urun, boy=7.6),
                         t.olc(kw - 6, self.m(kazanc), boy=7.6, satir=1.28),
                         t.olc(kw - 6, self.m(kayip), boy=7.6, satir=1.28))
                yeni = self._yer(t, y, hh + 10, s, "ne kazandırıyor, ne kaybettiriyor")
                if yeni != y:
                    y = basliklar(yeni)
                t.yazi(X + 3, y, uw - 6, urun, boy=7.6, font="sgb")
                t.yazi(X + uw + 3, y, kw - 6, self.m(kazanc), boy=7.6, satir=1.28)
                t.yazi(X + uw + kw + 17, y, kw - 6, self.m(kayip), boy=7.6, satir=1.28)
                self.sentez.append((f"{s.get('no', '')} · {urun} · kazanç", self.m(kazanc), "—"))
                self.sentez.append((f"{s.get('no', '')} · {urun} · kayıp", self.m(kayip), "—"))
                y += hh + 5
                t.cizgi(X, y - 2, X + W)
            y += 8
        self._tasma(t, y, f"{s.get('no')} kazanç")

        # --- Belge 3'e tasinan soru --------------------------------------
        if s.get("soru"):
            h = 15.0 + sum(t.olc(W - 24, self.m(q), boy=8.2, satir=1.3) + 5 for q in s["soru"])
            y = self._yer(t, y, h + 20, s, "Belge 3'e taşınan sorular")
            t.kutu(X, y, X + W, y + h + 6, dolgu=(0.965, 0.955, 0.93), kenar=(0.82, 0.78, 0.66))
            yy = y + 6
            t.yazi(X + 12, yy, W - 24, "Belge 3'e taşınan soru", boy=7.6, font="sgb", renk=M.VURGU)
            yy += 13
            for q in s["soru"]:
                q = self.m(q)
                hh = t.olc(W - 24, q, boy=8.2, satir=1.3)
                t.yazi(X + 12, yy, W - 24, q, boy=8.2, satir=1.3)
                self.sentez.append((f"{s.get('no', '')} · Belge 3 sorusu", q, "—"))
                yy += hh + 5
            y += h + 14
        self._tasma(t, y, f"{s.get('no')} soru")

        if s.get("notlar"):
            y = self._notlar(t, X, y, W, s, [])
        self._tasma(t, y, f"{s.get('no')} not")
        t.alt_bant("")


# ---------------------------------------------------------------------------
# KAPILAR
# ---------------------------------------------------------------------------
def kapilar(C, b: Bolum, pdf: pathlib.Path) -> None:
    no = getattr(C, "NO", "?")
    cikarimlar = [s for s in C.SAYFALAR if s["tur"] == "cikarim"]
    if not cikarimlar:                                                    # G1
        raise KapiHatasi(
            f"Bölüm {no}: hiç `cikarim` sayfası yok. Yeniden-kurgu K10/G1: her bölüm "
            f"en az bir mekanizma çıkarımı ve bir kazanç/kayıp satırı taşır."
        )
    for s in cikarimlar:                                                  # G2
        if not s.get("mekanizma"):
            raise KapiHatasi(f"Bölüm {no} · {s.get('no')}: `cikarim` sayfası boş `mekanizma` taşıyor.")
        if not s.get("kazanc"):
            raise KapiHatasi(f"Bölüm {no} · {s.get('no')}: `cikarim` sayfası boş `kazanc` taşıyor.")
        for blok in s["mekanizma"]:
            if not blok.get("dayanak"):
                raise KapiHatasi(
                    f"Bölüm {no} · {s.get('no')}: '{blok['baslik']}' çıkarımı dayanaksız. "
                    f"Çıkarım da kanıta bağlanır — kaynağı yazılmadan basılmaz."
                )
    akislar = [s for s in C.SAYFALAR if s["tur"] == "akis"]
    if len(akislar) != 1:                                                 # G6
        raise KapiHatasi(
            f"Bölüm {no}: {len(akislar)} akış sayfası var, tam olarak 1 bekleniyor. "
            f"Her bölüm TEK ortak yol çizer; ürün başına ayrı şema v1'in hatasıydı."
        )
    a = akislar[0]
    if not any(adim.get("dallar") for adim in a["adimlar"]):
        raise KapiHatasi(f"Bölüm {no} · {a.get('no')}: akış hiç çatallanmıyor. "
                         f"Dalsız akış süreç anlatır, ayrışmayı göstermez.")
    for i, adim in enumerate(a["adimlar"], 1):
        for dal in adim.get("dallar", []):
            if not dal.get("d"):
                raise KapiHatasi(
                    f"Bölüm {no} · {a.get('no')} adım {i}: '{dal['urunler']}' dalı dayanaksız. "
                    f"Akış yeni kanıt üretmez; her dal bir kareye bağlanır."
                )
    akis_yeri = C.SAYFALAR.index(a)
    cikarim_yeri = C.SAYFALAR.index(cikarimlar[0])
    if akis_yeri > cikarim_yeri:
        raise KapiHatasi(f"Bölüm {no}: akış sayfası çıkarımdan sonra geliyor. Sıra: "
                         f"kanıt → yol nerede ayrıldı → neden ayrıldı.")

    en_az = getattr(C, "EN_AZ_KARE", 4)
    n_kare = len(b.sekil_listesi)
    if n_kare < en_az:                                                    # G3
        raise KapiHatasi(
            f"Bölüm {no}: {n_kare} kare basıldı, en az {en_az} bekleniyor (K6). "
            f"Belge 2 sonucu göstermek için kare basar; sayı yalnız metinde kalmaz."
        )
    metin = "\n".join(s.get_text() for s in pymupdf.open(str(pdf)))       # G4
    for terim in YASAK_TERIM:
        if terim.lower() in metin.lower():
            raise KapiHatasi(
                f"Bölüm {no}: iç terim '{terim}' belgeye sızmış (K5). Başlık ve metin "
                f"okurun dilinde yazılır; üretim kısaltmaları belgeye girmez."
            )
    if "**" in metin:                                                     # G4b
        raise KapiHatasi(
            f"Bölüm {no}: metinde '**' var. Motor markdown kalınlığı basmaz, yıldızları "
            f"olduğu gibi yazar. Vurgu gerekiyorsa cümleyi yeniden kur."
        )
    for kavram in YASAK_KAVRAM:                                           # G7
        if kavram.lower() in metin.lower():
            raise KapiHatasi(
                f"Bölüm {no}: kendi kavramımız '{kavram}' metne girmiş (K12). Belge 2 "
                f"rakibi anlatır; kendi modelimizle karşılaştırma Belge 3'ündür. "
                f"Karşılık kurulacaksa 'Belge 3'e taşınan soru' kutusuna soru olarak yazılır."
            )
    # G8 acilis sayfasini SAYMAZ: kapsam tablosu zaten orada, gövdede aranir
    belge = pymupdf.open(str(pdf))
    govde = "\n".join(s.get_text() for s in belge[1:]) if belge.page_count > 1 else ""
    for urun, turler, _n in getattr(C, "KAPSAM", []):                     # G8
        if urun.split()[0] not in govde:
            raise KapiHatasi(
                f"Bölüm {no}: '{urun}' kapsam tablosunda sayılmış ama gövdede hiç geçmiyor "
                f"({'/'.join(turler)}). Ya bölümde bir yerde geçer, ya kapsamdan çıkar — "
                f"kapsamda görünüp gövdede olmamak Bölüm 1'in ilk hatasıydı."
            )


def kanit_kapsami(C, b: Bolum) -> dict:
    """Bolumun andigi kareleri urun bazinda sayar (K4 raporu)."""
    anilan = set(re.findall(r"\bE0\d{3}\b", pathlib.Path(
        b.C.__file__ if hasattr(b.C, "__file__") else "").read_text(encoding="utf-8"))) \
        if hasattr(b.C, "__file__") else set()
    anilan |= {e for _n, e, _a in b.sekil_listesi}
    out: dict[str, int] = {}
    for e in anilan:
        out[b.k.uygulama(e) or "?"] = out.get(b.k.uygulama(e) or "?", 0) + 1
    return out


# ---------------------------------------------------------------------------
# URETIM
# ---------------------------------------------------------------------------
def uret(C, klasor: pathlib.Path) -> Bolum:
    klasor = pathlib.Path(klasor)
    ad = f"bolum-{int(C.NO):02d}"
    pdf = klasor / f"{ad}.pdf"
    M.temizle(klasor / "isaretli")
    b = Bolum(C, klasor)

    taslak = pymupdf.open()
    b.ciz(M.Tuval(taslak, UST))
    harita = list(b.sayfa_no)
    taslak.close()
    M.uyarilar.clear()

    belge = pymupdf.open()
    b.ciz(M.Tuval(belge, UST), sayfa_haritasi=harita)
    belge.set_metadata({"title": f"Belge 2 · Bölüm {C.NO} · {C.BASLIK}", "author": "",
                        "subject": "Rakip finansal akış araştırması", "keywords": "rakip akış"})
    belge.save(str(pdf), deflate=True, garbage=3)
    belge.close()

    kapilar(C, b, pdf)

    dogrulama = b.k.dogrula(b.kullanilan_eidler() + list(getattr(C, "EK_KANITLAR", [])))
    baslik = f"Belge 2 · Bölüm {C.NO} · {C.BASLIK}"
    M.iddia_tablosu_yaz(klasor / "iddia-tablosu.md", baslik, b.iddia, b.sentez, [],
                        getattr(C, "EK_NOTLAR", ()))
    M.eksik_listesi_yaz(klasor / "eksik-listesi.md", baslik, getattr(C, "EKSIKLER", []))
    M.kaynaklar_yaz(klasor / "kaynaklar.md", baslik, b.k,
                    [(no, e) for no, e, _a in b.sekil_listesi], dogrulama,
                    getattr(C, "KAYNAK_NOTLARI", ()))
    M.manifest_yaz(klasor / "kanit-manifest.json", baslik, "2026-09-21", dogrulama, b.isaretli)
    markdown_yaz(b, klasor / f"{ad}.md")

    sonuc = M.kontrol(pdf, gizli_sozcukler=kalip.GIZLI)
    n = M.onizleme(pdf, klasor / "onizleme", dpi=110)
    M.rapor_yaz(ad, pdf, sonuc, dogrulama)
    print(f"Önizleme: {n} PNG · basılan kare: {len(b.sekil_listesi)}")
    return b


def markdown_yaz(b: Bolum, hedef: pathlib.Path) -> None:
    """kalip.markdown_yaz, `akis` ve `cikarim` bolumleri YERINDE degistirilerek.

    kalip'in genel dongusu bu iki tur icin yalnizca baslik + giris + not yazar.
    Bu taslagi bulup tam icerikle degistiririz; sonuna eklemek ayni basligi iki
    kez yazdiriyordu.
    """
    kalip.markdown_yaz(b, hedef)
    metin = hedef.read_text(encoding="utf-8")
    # Uretim tarihi belgeye girmez (yeniden-kurgu, "Tarih yazilmaz").
    metin = metin.replace(f"{UST} · {TARIH}", UST, 1)
    for p in b.C.SAYFALAR:
        if p["tur"] not in ("akis", "cikarim"):
            continue
        baslik = f"## {p.get('no', '')} · {p['baslik']}"
        i = metin.find(baslik)
        if i < 0:
            continue
        j = metin.find("\n## ", i + len(baslik))
        j = len(metin) if j < 0 else j + 1
        metin = metin[:i] + "\n".join(_blok(b, p)) + "\n" + metin[j:]
    hedef.write_text(metin, encoding="utf-8")


def _blok(b: Bolum, p: dict) -> list[str]:
    s = [f"## {p.get('no', '')} · {p['baslik']}", ""]
    if p.get("giris"):
        s += [b.m(p["giris"]), ""]
    if p["tur"] == "akis":
        for i, a in enumerate(p["adimlar"], 1):
            s += [f"**{i}. {a['baslik']}**", ""]
            if a.get("ortak"):
                s += [b.m(a["ortak"]), ""]
            for dal in a.get("dallar", []):
                isaret = "**→**" if dal.get("vurgu") else "-"
                s.append(f"{isaret} *{dal['urunler']}* — {b.m(dal['metin'])} ({dal.get('d', '—')})")
            s.append("")
    else:
        for blok in p.get("mekanizma", []):
            s += [f"### {blok['baslik']} *(Çıkarım)*", ""]
            s += [b.m(x) + "\n" for x in blok["metin"]]
            s += [f"*Dayanağı: {blok.get('dayanak', '—')}*", ""]
        if p.get("kazanc"):
            s += ["| Ürün | Kazandırdığı | Kaybettirdiği |", "|---|---|---|"]
            s += [f"| **{u}** | {b.m(a)} | {b.m(k)} |" for u, a, k in p["kazanc"]]
            s.append("")
        if p.get("soru"):
            s += ["**Belge 3'e taşınan soru**", ""]
            s += [f"- {b.m(q)}" for q in p["soru"]] + [""]
    for n in p.get("notlar", []):
        s.append(f"- {b.m(n[0] if isinstance(n, tuple) else n)}")
    s.append("")
    return s


def icerik_yukle(klasor: pathlib.Path):
    return kalip.icerik_yukle(klasor)


def calistir(uret_dosyasi: str) -> None:
    klasor = pathlib.Path(uret_dosyasi).resolve().parent
    uret(icerik_yukle(klasor), klasor)
