# -*- coding: utf-8 -*-
"""Belge 1 · uretim katmani (24 Eylul 2026 duzeltme turu).

`kalip.py` ve `motor.py` iki belgenin ORTAK motorudur ve degistirilmez (Belge 2'nin
`b2.py` ornegi). Belge 1'e ozgu her kural burada, `kalip.Bolum`un alt sinifinda yasar:

  Kesme (karar 4)
      Telefon karesinde (1080x2400) ustteki durum cubugu (saat, pil) ~0-120 px
      iddia tasimaz ve kesilir. Isaret koordinatlari icerik.py'de OZGUN kare
      uzayinda yazilidir; kesme uygulanirken ayni miktarda kaydirilir. Kirpmasi
      yazilmis kareye (KolayBi masaustu) dokunulmaz. Alttaki kaydirma cizgisi
      kalir: alt kenara yakin isaretler var ve cizgi okumayi bozmuyor.

  Kapilar — ihlalinde uretim DURUR:
      K   isaret kesilmis karenin kenarina tasmaz
      G4  PDF metninde uretim dili gecmez ("Çıkarılmayan sonuç", "matrisi",
          "koşum" — "Koşum kaydı" rozeti haric, karar 8)
      G4b markdown kalinligi (**) metne sizmaz
      G7  kendi kavramlarimiz (ADR, sinif adlari) gecmez; urun adimiz sinir
          cumlelerinde serbest (karar 3)
      G8  acilis kutusunda sayilan urun govdede gecer; kutu yalniz kaniti listeler: rozetler
          govdede o urun icin kullanilan kanit turleridir, Gorulmedi yalniz urunun bolumde hic
          kaniti yoksa; Koşum kaydi ya da Gorulmedi tasiyan urunun altinda not vardir
      A1  koşum ve uretim tarihi yazilmaz ("12 Eylül", "Eylül 2026")

  Markdown: uretim tarihi yazilmaz; sekilsiz bolumde bos sekil dizini yazilmaz
  (dis goz notu 16, 17).
"""
from __future__ import annotations

import pathlib
import re
import sys

KLASOR = pathlib.Path(__file__).resolve().parent
sys.path.insert(0, str(KLASOR))

import kalip  # noqa: E402
import motor as M  # noqa: E402
import pymupdf  # noqa: E402
from PIL import Image  # noqa: E402

UST = kalip.UST
TELEFON = (1080, 2400)
UST_KESME = 120                      # durum cubugu
ISARET_PAYI = 48                     # motor.isaretle: cap 88 px -> r 44 + 4 px beyaz halka
# Icerigi durum cubugunun hizasindan baslayan kareler: kesme cubugun hemen altindan yapilir.
OZEL_KESME = {
    "E0376": 90,    # Wallet cekmecesinin alti: ilk kalem (Debts) y~98'de basliyor
    "E0026": 88,    # Bluecoins temiz kurulum: ust cubuk simgeleri y~95'te basliyor
    "E0171": 88,    # Hesap Defterim cekmecesi: baslik simgesi y~97'de basliyor
}

VARSAYILAN_ACILIS_DAYANAK = ("\"Görülmedi\", özelliğin incelenen sürümde ve yüzeyde görülmediğini söyler; "
                             "bulunmadığı anlamına gelmez.")

YASAK_TERIM = ("Çıkarılmayan sonuç", "çıkarılmayan sonuç", "matrisi", "matriste")
KOSUM = re.compile(r"koşum(?!\s+kayd)", re.IGNORECASE)          # rozet "Koşum kaydı" serbest
TARIH = re.compile(r"\b\d{1,2}(?:\s*[–-]\s*\d{1,2})?\s+Eylül\b|\bEylül\s+2026\b")
YASAK_KAVRAM = (
    "ADR 00", "TransactionScope", "CounterpartyCharge", "CounterpartyPayment", "BudgetTransaction",
    "RecurringTransaction", "ICurrentUser", "Aşama 05", "Aşama 07", "bizim modelimiz", "bizdeki",
)


class KapiHatasi(SystemExit):
    """Kural ihlali. Uretimi durdurur; uyari degildir."""


# ---------------------------------------------------------------------------
# Bolum
# ---------------------------------------------------------------------------
class Bolum(kalip.Bolum):
    """kalip.Bolum + telefon karesinde durum cubugu kesmesi."""

    def gorsel(self, eid: str, isaretler=(), *, kirpma=None, ek: str = "") -> pathlib.Path:
        kirpma = kirpma or getattr(self.C, "KIRPMA", {}).get(eid)
        if not kirpma:
            with Image.open(self.k.yol(eid)) as im:
                boyut = im.size
            if boyut == TELEFON:
                kes = OZEL_KESME.get(eid, UST_KESME)
                kirpma = (0, kes, TELEFON[0], TELEFON[1])
                kaydir = []
                for no, x, y, metin in isaretler:
                    yy = y - kes
                    if yy - ISARET_PAYI < 0 or yy + ISARET_PAYI > kirpma[3] - kirpma[1]:
                        raise KapiHatasi(
                            f"Bölüm {self.C.NO} · {eid} işaret {no} (y={y}): kesilmiş karenin kenarına "
                            f"taşıyor. İşaret y ≥ {kes + ISARET_PAYI} olmalı.")
                    kaydir.append((no, x, yy, metin))
                isaretler = tuple(kaydir)
        return super().gorsel(eid, isaretler, kirpma=kirpma, ek=ek)

    def _kartlar(self, t, s: dict) -> None:
        """"yuva": sayfadaki kart yuvasi. Tek karti kalan sayfa, onceki sayfalarin kart
        genisligini korur (tam genislikte satirlar ve rozetler kopuk duruyordu). Ust bant
        _yeni'de gercek genislikle cizilir; daraltma yalniz kart alanina uygulanir."""
        yuva, n = s.get("yuva", 0), len(s["kartlar"])
        if yuva <= n:
            return super()._kartlar(t, s)
        tam, ara = M.ICERIK_W, 20.0
        dar = (tam - ara * (yuva - 1)) / yuva * n + ara * (n - 1)
        yeni = self._yeni

        def _yeni_dar(tt):
            yeni(tt)
            M.ICERIK_W = dar
        self._yeni = _yeni_dar
        try:
            super()._kartlar(t, s)
        finally:
            M.ICERIK_W = tam
            del self._yeni


# ---------------------------------------------------------------------------
# Kapilar
# ---------------------------------------------------------------------------
def metin_kapilari(ad: str, metin: str) -> None:
    kucuk = metin.lower()
    for terim in YASAK_TERIM:                                             # G4
        if terim.lower() in kucuk:
            raise KapiHatasi(f"{ad}: üretim dili '{terim}' belgeye sızmış (karar 5, B2).")
    m = KOSUM.search(metin)
    if m:
        bas = max(0, m.start() - 50)
        raise KapiHatasi(f"{ad}: üretim dili 'koşum' metinde: …{metin[bas:m.end() + 30]!r}…")
    if "**" in metin:                                                     # G4b
        raise KapiHatasi(f"{ad}: metinde '**' var; motor kalınlık basmaz.")
    for kavram in YASAK_KAVRAM:                                           # G7
        if kavram.lower() in kucuk:
            raise KapiHatasi(f"{ad}: kendi kavramımız '{kavram}' metne girmiş (karar 3).")
    m = TARIH.search(metin)                                               # A1
    if m:
        raise KapiHatasi(f"{ad}: koşum/üretim tarihi metinde: {m.group(0)!r} (A1: tarih yazılmaz).")


def _urunler(ad: str) -> list[str]:
    return [p.strip().split()[0].rstrip(",") for p in ad.split(",") if p.strip()]


def _anahtar(ad: str) -> str:
    """Urun adinin eslesme anahtari: ilk sozcuk ("Wallet (BudgetBakers)" -> Wallet); Hesap Defterim
    tam ad, cunku "Hesap" satir etiketlerinde de geciyor."""
    ad = ad.strip()
    return "Hesap Defterim" if ad.startswith("Hesap Defterim") else ad.split()[0].rstrip(",")


def govde_rozetleri(C, b) -> dict[str, set[str]]:
    """Govdede urun basina kullanilan rozet turleri: basilan kareler, "Ayni soruda" satirlari, tablo
    hucreleri (satir etiketi ya da sutun basligi urun adidir), kart alanlari. Matris tablosunun bos
    hucresi (dayanak "—") rozet sayilmaz: kanit yoklugunu gosterir, gozlem degildir."""
    rozet: dict[str, set[str]] = {}

    def ekle(etiket: str, tur) -> None:
        if not tur:
            return
        for parca in str(etiket).split(","):
            parca = parca.strip()
            if parca:
                rozet.setdefault(_anahtar(parca), set()).add(tur)

    for _no, eid, _a in b.sekil_listesi:
        ekle(URUN_ADI.get(b.k.uygulama(eid), ""), b.tur(eid))
    for s in C.SAYFALAR:
        for r in s.get("urunler", []):
            ekle(r[0], r[1])
        sutunlar = [ad for ad, _g in s.get("sutunlar", [])]
        for r in s.get("satirlar", []):
            if not isinstance(r, list):
                continue
            satir_urun = any(_anahtar(k) in str(r[0]) for k in URUN_ADI.values())
            for i, c in enumerate(r[1:], 1):
                if not isinstance(c, dict) or c.get("d") == "—":
                    continue
                ekle(r[0] if satir_urun else (sutunlar[i] if i < len(sutunlar) else ""), c.get("tur"))
        for k in s.get("kartlar", []):
            for a in k.get("alanlar", []) + k.get("alt_alanlar", []):
                ekle(k["urun"], a[2])
    return rozet


URUN_ADI = {"money-manager": "Money Manager", "bluecoins": "Bluecoins", "wallet-budgetbakers": "Wallet",
            "hesap-defterim": "Hesap Defterim", "goodbudget": "Goodbudget", "kolaybi": "KolayBi",
            "parasut": "Paraşüt", "logo-isbasi": "Logo İşbaşı", "quickbooks": "QuickBooks Solopreneur"}


def kapilar(C, pdf: pathlib.Path, b=None) -> None:
    belge = pymupdf.open(str(pdf))
    metin = "\n".join(s.get_text() for s in belge)
    govde = "\n".join(s.get_text() for s in belge[1:]) if belge.page_count > 1 else ""
    belge.close()
    metin_kapilari(f"Bölüm {C.NO}", metin)
    rozet = govde_rozetleri(C, b) if b is not None else None
    for urun, turler, notu in (getattr(C, "KAPSAM", None) or []):        # G8
        for u in _urunler(urun):
            if u not in govde:
                raise KapiHatasi(
                    f"Bölüm {C.NO}: '{u}' açılış kutusunda sayılmış ama gövdede hiç geçmiyor "
                    f"({'/'.join(turler)}).")
        if rozet is None:
            continue
        # Kutu govdenin ozetidir: rozetler govdedeki kumeyle birebir. Govdede rozeti olmayan urun
        # (yalniz notta anilan) kutuda "Gorulmedi" tasir.
        gv = set().union(*(rozet.get(_anahtar(p), set()) for p in urun.split(",") if p.strip()))
        if getattr(C, "KUTU_KURALI", "kanit") == "kanit":
            # Kutu yalniz kaniti listeler: Gorulmedi yalniz urunun bu bolumde hic kaniti yoksa. Kismi
            # bosluklar govdede yerinde durur; bolum capindaki bulgu kutuda urun urun tekrarlanmaz.
            gv = (gv - {"yok"}) or {"yok"}
        if set(turler) != (gv or {"yok"}):
            raise KapiHatasi(
                f"Bölüm {C.NO}: '{urun}' açılış kutusu {sorted(turler)}, gövde {sorted(gv) or ['yok']} "
                f"(G8: kutu gövdedeki rozetlerle birebir olmalı).")
        if {"kosum", "yok"} & set(turler) and not notu:
            raise KapiHatasi(
                f"Bölüm {C.NO}: '{urun}' kutuda Koşum kaydı/Görülmedi taşıyor ama altında ne olduğunu "
                f"söyleyen not yok (G8).")


# ---------------------------------------------------------------------------
# Uretim
# ---------------------------------------------------------------------------
def hazirla(C) -> None:
    if not hasattr(C, "ACILIS_DAYANAK"):
        C.ACILIS_DAYANAK = VARSAYILAN_ACILIS_DAYANAK


def uret(C, klasor: pathlib.Path) -> Bolum:
    hazirla(C)
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
    belge.set_metadata({"title": f"Belge 1 · Bölüm {C.NO} · {C.BASLIK}", "author": "",
                        "subject": "Rakip arayüz araştırması", "keywords": "rakip arayüz"})
    belge.save(str(pdf), deflate=True, garbage=3)
    belge.close()

    kapilar(C, pdf, b)

    dogrulama = b.k.dogrula(b.kullanilan_eidler() + list(getattr(C, "EK_KANITLAR", [])))
    baslik = f"Belge 1 · Bölüm {C.NO} · {C.BASLIK}"
    M.iddia_tablosu_yaz(klasor / "iddia-tablosu.md", baslik, b.iddia, b.sentez, [],
                        getattr(C, "EK_NOTLAR", ()))
    M.eksik_listesi_yaz(klasor / "eksik-listesi.md", baslik, getattr(C, "EKSIKLER", []))
    M.kaynaklar_yaz(klasor / "kaynaklar.md", baslik, b.k,
                    [(no, e) for no, e, _a in b.sekil_listesi], dogrulama,
                    getattr(C, "KAYNAK_NOTLARI", ()))
    M.manifest_yaz(klasor / "kanit-manifest.json", baslik, "2026-09-24", dogrulama, b.isaretli)
    markdown_yaz(b, klasor / f"{ad}.md")

    sonuc = M.kontrol(pdf, gizli_sozcukler=kalip.GIZLI)
    n = M.onizleme(pdf, klasor / "onizleme", dpi=110)
    M.rapor_yaz(ad, pdf, sonuc, dogrulama)
    print(f"Önizleme: {n} PNG · basılan kare: {len(b.sekil_listesi)}")
    return b


def markdown_yaz(b: Bolum, hedef: pathlib.Path) -> None:
    kalip.markdown_yaz(b, hedef)
    metin = hedef.read_text(encoding="utf-8")
    metin = metin.replace(f"{UST} · {kalip.TARIH}", UST, 1)
    if not b.sekil_listesi:
        i = metin.find("\n## Şekil dizini")
        if i >= 0:
            metin = metin[:i].rstrip() + "\n"
    hedef.write_text(metin, encoding="utf-8")


def icerik_yukle(klasor: pathlib.Path):
    return kalip.icerik_yukle(klasor)


def calistir(uret_dosyasi: str) -> None:
    klasor = pathlib.Path(uret_dosyasi).resolve().parent
    uret(icerik_yukle(klasor), klasor)
