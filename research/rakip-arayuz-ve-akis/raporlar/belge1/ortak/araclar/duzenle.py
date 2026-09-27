"""Kullanim: python duzenle.py <icerik.py> <ciftler.py>
ciftler.py icinde CIFTLER = [(eski, yeni), ...] ve istege bagli REGEX_SIL = [desen, ...].
Her eski metin dosyada TAM BIR KEZ bulunmali."""
import re, runpy, sys
hedef, cift = sys.argv[1], sys.argv[2]
d = runpy.run_path(cift)
s = open(hedef, encoding='utf8').read()
for a, b in d.get('CIFTLER', []):
    n = s.count(a)
    if n != 1:
        raise SystemExit(f"{n} kez bulundu: {a[:90]!r}")
    s = s.replace(a, b)
for desen in d.get('REGEX_SIL', []):
    s, n = re.subn(desen, '', s, flags=re.M | re.S)
    print(f"regex sil {n}: {desen[:50]}")
open(hedef, 'w', encoding='utf8').write(s)
print('ok', hedef)
