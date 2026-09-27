# -*- coding: utf-8 -*-
"""motor.py duman testi — ortamin calistigini bir komutla dogrular.

Calistirma (arastirma kokunden):
    raporlar/.pilot-tools/venv/Scripts/python raporlar/belge1/ortak/motor-testi.py

Gercek bir bolum uretmez; iki sayfalik gecici bir PDF cizip butun motor
parcalarini yoklar ve sonra siler. Sifir uyariyla bitmeli.
"""
from __future__ import annotations

import pathlib
import shutil
import sys
import tempfile

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parent))
import motor as M  # noqa: E402

GECICI = pathlib.Path(tempfile.mkdtemp(prefix="motor-testi-"))
ISARETLI = GECICI / "isaretli"


def main() -> None:
    hata = []

    # 1) Kanit dizini ve kimlik/ad cevirisi
    k = M.Kanit()
    eid = k.kimlik("money-manager/03-dolu-ana-ekran.png")
    if eid != "E0228":
        hata.append(f"ad->kimlik cevirisi yanlis: {eid}")
    if not k.aciklama(eid):
        hata.append("E0228 icin icerik aciklamasi yok")

    # 2) Hash dogrulama
    dogrulama = k.dogrula(["E0228", "E0137", "E0211"])

    # 3) Gorsel islemleri: isaret, kirpma, karartma, genis kare
    telefon = M.isaretle(k, "E0228", [(1, 330, 190, "ay seçici"),
                                      (2, 55, 408, "üç özet sayısı")], ISARETLI, ek="-t")
    kirpinti = M.kirp(k, "E0228", (0, 2200, 1080, 2345), ISARETLI, ek="-nav")
    karartmali = M.sade(k, "E0115", ISARETLI, ek="-kara",
                        karartma=[(168, 170, 452, 248)])
    genis = M.isaretle(k, "E0211", [(1, 84, 308, "modül paneli")], ISARETLI,
                       ek="-t", kirpma=(96, 112, 1320, 872))
    for p in (telefon, kirpinti, karartmali, genis):
        if not p.exists():
            hata.append(f"görsel üretilmedi: {p.name}")

    # 4) Sayfa cizimi
    belge = M.pymupdf.open()
    t = M.Tuval(belge, "Motor duman testi")

    t.yeni("sayfa 1")
    y = t.baslik("1", "İşaretli kare ve numaralı notlar")
    y = t.giris(y, "Bu sayfa motorun metin ölçümünü, işaretli kare basmayı ve "
                   "numaralı not listesini yoklar. Şekil 1.1 · sınama karesi.")
    t.resim(telefon, M.KENAR, y, 392 / M.TELEFON_ORAN, 392)
    t.yazi(M.KENAR, y + 396, 200, "Şekil 1.1 · sınama karesi", boy=6.9, renk=M.SOLUK)
    sx = M.KENAR + 392 / M.TELEFON_ORAN + 22
    sw = M.SAYFA_W - M.KENAR - sx
    ny = t.numarali_notlar(sx, y, sw, [(1, 0, 0, "Dönem seçici."),
                                       (2, 0, 0, "Gelir, Gider ve Toplam.")])
    t.maddeler(sx, ny + 6, sw, ["Ölçüm ayrı belgede yapılıyor.",
                                "Sığmayan metin uyarı üretir."])
    t.alt_bant("Canlı kare: E0228. Bu sayfa bir bölüm değildir.")

    t.yeni("sayfa 2")
    y = t.baslik("", "Kırpıntı, karartma ve geniş kare")
    y = t.giris(y, "Şekil 2.1 kırpıntı, Şekil 2.2 karartılmış kopya, "
                   "Şekil 2.3 geniş masaüstü karesi.")
    h = t.resim_en(kirpinti, M.KENAR, y, 300)
    t.yazi(M.KENAR, y + h + 4, 300, "Şekil 2.1 · alt sekme kırpıntısı (ekranın altı)",
           boy=6.9, renk=M.SOLUK)
    t.resim(karartmali, M.KENAR + 320, y, 150 / M.TELEFON_ORAN * 2.22, 150)
    t.yazi(M.KENAR + 320, y + 154, 200, "Şekil 2.2 · karartılmış kopya",
           boy=6.9, renk=M.SOLUK)
    h3 = t.resim_en(genis, M.KENAR, y + 190, 420)
    t.yazi(M.KENAR, y + 190 + h3 + 4, 420, "Şekil 2.3 · geniş kare, siyah zemin kırpıldı",
           boy=6.9, renk=M.SOLUK)
    t.alt_bant("Kaynak görseli: E0211. Canlı kare: E0228, E0115.")

    pdf = GECICI / "motor-testi.pdf"
    belge.save(str(pdf), deflate=True, garbage=3)
    belge.close()

    # 5) Denetim dosyalari
    M.iddia_tablosu_yaz(GECICI / "iddia-tablosu.md", "Motor duman testi",
                        [("Şekil 1.1 · işaret 1", "Dönem seçici.", "E0228", "canli")],
                        [("Sayfa 1 · not", "Ölçüm ayrı belgede yapılıyor.", "E0228")],
                        [("Test", "Bu sayfa bir bölüm değildir")])
    M.eksik_listesi_yaz(GECICI / "eksik-listesi.md", "Motor duman testi",
                        [("Money Manager", "Sınama eksiği", "1", "Gerekmez", "Düşük")])
    M.kaynaklar_yaz(GECICI / "kaynaklar.md", "Motor duman testi", k,
                    [("1.1", "E0228"), ("2.2", "E0115")], dogrulama)
    M.manifest_yaz(GECICI / "kanit-manifest.json", "Motor duman testi",
                   "2026-09-16", dogrulama, ISARETLI)
    for ad in ("iddia-tablosu.md", "eksik-listesi.md", "kaynaklar.md", "kanit-manifest.json"):
        if not (GECICI / ad).exists():
            hata.append(f"denetim dosyası yazılmadı: {ad}")

    # 6) Otomatik kapilar ve onizleme
    sonuc = M.kontrol(pdf, gizli_sozcukler=("elma6",))
    n = M.onizleme(pdf, GECICI / "onizleme")

    if sonuc["sayfa"] != 2:
        hata.append(f"sayfa sayisi 2 degil: {sonuc['sayfa']}")
    if sonuc["kirik_atif"]:
        hata.append(f"kirik atif: {sonuc['kirik_atif']}")
    if sonuc["bozuk_karakter"]:
        hata.append(f"bozuk karakter: {sonuc['bozuk_karakter']}")
    if sonuc["metinde_gizli_sozcuk"]:
        hata.append(f"gizli sozcuk sizdi: {sonuc['metinde_gizli_sozcuk']}")
    if n != 2:
        hata.append(f"onizleme sayfa sayisi yanlis: {n}")
    if M.uyarilar:
        hata.append(f"yerlesim uyarisi: {M.uyarilar}")

    print(f"PDF: {sonuc['sayfa']} sayfa, fontlar: {', '.join(sonuc['fontlar'])}")
    print(f"Kanit: {len(dogrulama)} kare hash dogrulandi")
    print(f"Gorsel turevi: {len(list(ISARETLI.glob('*.png')))}")
    print(f"Denetim dosyasi: 4, onizleme: {n} PNG")

    shutil.rmtree(GECICI, ignore_errors=True)

    if hata:
        print("\nBASARISIZ:")
        for h in hata:
            print("  -", h)
        raise SystemExit(1)
    print("\nTUM KONTROLLER GECTI. motor.py kullanima hazir.")


if __name__ == "__main__":
    main()
