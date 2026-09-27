# -*- coding: utf-8 -*-
"""KANIT-ENVANTERI.md icindeki E kimliklerini tek bir arama dizinine toplar.

Neden: tema kayitlari ve gozlem formlari kanitlari **dosya adiyla** aniyor
(`12-hesaplar-kart-borcu-bu-ay.png`), belgeler ise **E kimligiyle** atif yapmak
zorunda. Bu dizin ikisi arasinda ceviri yapar ve her kimligin envanterdeki
icerik aciklamasini da tasir; bolum yazarken hangi karenin ne gosterdigi
envanteri bastan okumadan bulunur.

Calistirma (arastirma kokunden):
    raporlar/.pilot-tools/venv/Scripts/python raporlar/belge1/ortak/kanit-dizini-uret.py

Cikti: raporlar/belge1/ortak/kanit-dizini.json
"""
from __future__ import annotations

import collections
import json
import pathlib
import re

KOK = pathlib.Path(__file__).resolve().parents[3]   # research/rakip-arayuz-ve-akis
ENVANTER = KOK / "KANIT-ENVANTERI.md"
BULGU = KOK / "BULGU-DOGRULAMA-KAYDI.md"
CIKTI = pathlib.Path(__file__).resolve().parent / "kanit-dizini.json"

# | E0115 | [kanitlar/goodbudget/08-...png](...) | 110839 | `hash` |
DOSYA = re.compile(
    r"^\|\s*(E\d{4})\s*\|\s*\[([^\]]+)\]\([^)]+\)\s*\|\s*(\d+)\s*\|\s*`([0-9a-f]{64})`\s*\|", re.M)

# Icerik aciklamasi tasiyan satirlar; aciklama sutunu tabloya gore degisiyor,
# bu yuzden butun sutunlar alinip en uzunu aciklama sayilir.
SATIR = re.compile(r"^\|\s*(E\d{4})\s*\|(.+)$", re.M)


def uret() -> dict:
    metin = ENVANTER.read_text(encoding="utf-8")
    kayit: dict[str, dict] = {}

    for eid, yol, boyut, sha in DOSYA.findall(metin):
        kare = yol.startswith("kanitlar/") and yol.lower().endswith(".png")
        kayit[eid] = {
            "kimlik": eid,
            "yol": yol,
            "uygulama": yol.split("/")[1] if kare else None,
            "tur": "kare" if kare else "yonetim",
            "boyut": int(boyut),
            "sha256": sha,
            "aciklama": [],
        }

    for eid, kalan in SATIR.findall(metin):
        if eid not in kayit:
            continue
        hucreler = [h.strip() for h in kalan.split("|")]
        aday = max((h for h in hucreler if len(h) >= 40), key=len, default="")
        if not aday or aday.startswith("["):
            continue
        aday = re.sub(r"\s+", " ", aday)
        if aday not in kayit[eid]["aciklama"]:
            kayit[eid]["aciklama"].append(aday)

    # Bluecoins ve Wallet karelerinin icerik aciklamalari envanterde degil,
    # bulgu kaydindaki P2/P3 paketlerinde ve baska bir tablo bicimindedir:
    #   | E0103 — f7-52-nav-check.png | <icerik> | <yorum/sinir> | <rol> |
    # Uc ayri tablo bicimi var:
    #   | E0103 — dosya.png | <icerik> | ...        (Bluecoins P2)
    #   | E0376 | dosya — <icerik> | ...            (Wallet P3)
    #   | E0115 | <icerik> | ...                    (envanter ayrinti)
    # Ucu de "E kimligiyle baslayan satirda en uzun hucre aciklamadir" kuralina uyuyor.
    if BULGU.exists():
        bulgu = BULGU.read_text(encoding="utf-8")
        ikinci = re.compile(r"^\|\s*(E\d{4})\s*(?:[—-]\s*[^|]*)?\|(.+)$", re.M)
        for eid, kalan in ikinci.findall(bulgu):
            if eid not in kayit:
                continue
            hucreler = [h.strip() for h in kalan.split("|")]
            aday = max((h for h in hucreler if len(h) >= 30), key=len, default="")
            if not aday or aday.startswith("["):
                continue
            aday = re.sub(r"\s+", " ", aday)
            if aday not in kayit[eid]["aciklama"]:
                kayit[eid]["aciklama"].append(aday)

    # dosya adindan kimlige ters arama (yalnizca kareler)
    ters = {
        v["yol"].removeprefix("kanitlar/"): eid
        for eid, v in kayit.items() if v["tur"] == "kare"
    }
    return kayit, ters


def main() -> None:
    kayit, ters = uret()
    kareler = {e: v for e, v in kayit.items() if v["tur"] == "kare"}

    eksik = [v["yol"] for v in kareler.values() if not (KOK / v["yol"]).exists()]
    if eksik:
        raise SystemExit("envanterde yazılı ama diskte yok:\n  " + "\n  ".join(eksik))

    CIKTI.write_text(json.dumps({
        "kaynak": "KANIT-ENVANTERI.md",
        "uretim": "raporlar/belge1/ortak/kanit-dizini-uret.py",
        "toplam_kimlik": len(kayit),
        "kare_sayisi": len(kareler),
        "kanitlar": kayit,
        "dosya_adindan_kimlige": ters,
    }, ensure_ascii=False, indent=1), encoding="utf-8")

    say = collections.Counter(v["uygulama"] for v in kareler.values())
    acik = sum(1 for v in kareler.values() if v["aciklama"])
    print(f"{len(kayit)} kimlik ({len(kareler)} kare, {len(kayit) - len(kareler)} yönetim dosyası)")
    print(f"içerik açıklaması olan kare: {acik}/{len(kareler)}")
    for u, n in sorted(say.items(), key=lambda x: -x[1]):
        ac = sum(1 for v in kareler.values() if v["uygulama"] == u and v["aciklama"])
        print(f"  {u:<22} {n:>3} kare, {ac:>3} açıklamalı")
    print(f"diskte eksik kare yok. Cikti: {CIKTI.relative_to(KOK)}")


if __name__ == "__main__":
    main()
