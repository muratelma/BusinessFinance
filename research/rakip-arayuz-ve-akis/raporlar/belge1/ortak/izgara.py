# -*- coding: utf-8 -*-
"""Kareleri ozgun piksel koordinatli izgarayla yan yana basar (koordinat secimi icin).

kullanim: python izgara.py cikti.png E0017 E0135 E0106

Cikti gecici bir klasore yazilmali (bolum klasorune degil). Izgaradaki sayilar ozgun
piksel koordinatidir; isaretler bos alana konur (plan §6).
"""
import json, sys, pathlib
from PIL import Image, ImageDraw, ImageFont

KOK = pathlib.Path(__file__).resolve().parents[3]          # research/rakip-arayuz-ve-akis
D = json.loads((KOK / "raporlar/belge1/ortak/kanit-dizini.json").read_text(encoding="utf-8"))["kanitlar"]
F = ImageFont.truetype(r"C:\Windows\Fonts\arialbd.ttf", 15)

cikti = sys.argv[1]
ids = [a for a in sys.argv[2:] if not a.startswith("--")]
H = 1000 if len(ids) > 1 else 1400
paneller = []
for eid in ids:
    im = Image.open(KOK / D[eid]["yol"]).convert("RGB")
    W0, H0 = im.size
    s = H / H0
    if W0 > H0:
        s = min(1400 / W0, 1000 / H0) if len(ids) == 1 else 700 / W0
    im = im.resize((int(W0 * s), int(H0 * s)))
    c = ImageDraw.Draw(im)
    adim = 100 if W0 <= 1500 else 200
    for x in range(0, W0, adim):
        c.line([(x * s, 0), (x * s, im.height)], fill=(255, 0, 255) if x % 500 == 0 else (0, 200, 255), width=1)
        c.text((x * s + 2, 2), str(x), font=F, fill=(255, 0, 255))
    for y in range(0, H0, adim):
        c.line([(0, y * s), (im.width, y * s)], fill=(255, 0, 255) if y % 500 == 0 else (0, 200, 255), width=1)
        c.text((2, y * s + 1), str(y), font=F, fill=(255, 0, 255))
    c.text((im.width - 70, im.height - 20), eid, font=F, fill=(255, 0, 0))
    paneller.append(im)
tw = sum(p.width for p in paneller) + 10 * (len(paneller) - 1)
th = max(p.height for p in paneller)
out = Image.new("RGB", (tw, th), "white")
x = 0
for p in paneller:
    out.paste(p, (x, 0))
    x += p.width + 10
out.save(cikti)
print(cikti, out.size)
